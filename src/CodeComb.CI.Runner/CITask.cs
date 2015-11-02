using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CodeComb.Package;
using CodeComb.CI.Runner.EventArgs;

namespace CodeComb.CI.Runner
{
    public enum CITaskStatus
    {
        Queued,
        Building,
        Successful,
        TimeLimitExceeded,
        Failed
    }

    public class CITask : Process
    {
        #region Delegates
        public delegate void OutputReceivedHandle(object sender, OutputReceivedEventArgs args);
        public static event OutputReceivedHandle OnOutputReceived;
        public delegate void BuildSuccessfulHandle(object sender, BuildSuccessfulArgs args);
        public static event BuildSuccessfulHandle OnBuildSuccessful;
        public delegate void BuildFailedHandle(object sender, BuildFailedArgs args);
        public static event BuildFailedHandle OnBuiledFailed;
        public delegate void TimeLimitExceededHandle(object sender, TimeLimitExceededArgs args);
        public static event TimeLimitExceededHandle OnTimeLimitExceeded;
        public delegate void BeginBuildingHandle(object sender, BeginBuildingArgs args);
        public static event BeginBuildingHandle OnBeginBuilding;
        public delegate void TestCaseFound(object sender, TestCaseArgs args);
        public static event TestCaseFound OnTestCaseFound;
        #endregion

        public string Output { get; private set; }
        private string EntryDirectory { get; set; }
        private int MaxTimeLimit { get; set; }
        public CITask(string workingDirectory, int maxTimeLimit, Dictionary<string, string> AdditionalEnvironmentVariables = null)
        {
            MaxTimeLimit = maxTimeLimit;
            EntryDirectory = workingDirectory;
            this.StartInfo = new ProcessStartInfo();
            this.StartInfo.UseShellExecute = false;
            this.StartInfo.RedirectStandardError = true;
            this.StartInfo.RedirectStandardOutput = true;
            this.StartInfo.WorkingDirectory = FindDirectory(workingDirectory);
            this.StartInfo.FileName = "cmd.exe";
            this.StartInfo.Arguments = "/c \"build.cmd\"";
            if (OS.Current != OSType.Windows)
            {
                this.StartInfo.FileName = "bash";
                this.StartInfo.Arguments = "-c \"./build.sh\"";
            }

            // Clean DNX environment variables
            var keys = new string[] {
                "DNX_HOME",
                "DNX_FEED",
                "DNX_UNSTABLE_FEED",
                "DNX_CONSOLE_HOST",
                "DNX_COMPILATION_SERVER_PORT",
                "DNX_PACKAGES",
                "DNX_PACKAGES_CACHE",
                "DNX_SERVICING",
                "DNX_TRACE",
                "DNX_BUILD_KEY_FILE",
                "DNX_BUILD_DELAY_SIGN",
                "DNX_BUILD_PORTABLE_PDB",
                "DNX_ASPNET_LOADER_PATH",
                "DNX_NO_MIN_VERSION_CHECK",
                "DNX_GLOBAL_PATH"
            };

            foreach (var x in keys)
            {
#if DNXCORE50 || DOTNET5_4
                try { this.StartInfo.Environment.Remove(x); } catch { }
#else
                try { this.StartInfo.EnvironmentVariables.Remove(x); } catch { }
#endif
            }
            if (AdditionalEnvironmentVariables != null)
            {
                foreach (var ev in AdditionalEnvironmentVariables)
                {
#if DNXCORE50 || DOTNET5_4
                    if (this.StartInfo.Environment[ev.Key] != null)
                        this.StartInfo.Environment[ev.Key] = this.StartInfo.Environment[ev.Key].TrimEnd(' ').TrimEnd(';') + ";" + ev.Value;
                    else
                        this.StartInfo.Environment.Add(ev.Key, ev.Value);
#else
                    if (this.StartInfo.EnvironmentVariables[ev.Key] != null)
                        this.StartInfo.EnvironmentVariables[ev.Key] = this.StartInfo.EnvironmentVariables[ev.Key].TrimEnd(' ').TrimEnd(';') + ";" + ev.Value;
                    else
                        this.StartInfo.EnvironmentVariables.Add(ev.Key, ev.Value);
#endif
                }
            }
            this.ErrorDataReceived += (sender, e) =>
            {
                Output += e.Data + "\r\n";
                if (OnOutputReceived != null)
                    OnOutputReceived(this, new OutputReceivedEventArgs { Output = e.Data + "\r\n" });
            };
            this.OutputDataReceived += (sender, e) =>
            {
                Output += e.Data + "\r\n";
                if (OnOutputReceived != null)
                    OnOutputReceived(this, new OutputReceivedEventArgs { Output = e.Data + "\r\n" });
            };
        }
        public string Identifier { get; set; }
        public CITaskStatus Status { get; set; }
        private string FindDirectory(string path)
        {
            string[] files;
            if (OS.Current == OSType.Windows)
                files = Directory.GetFiles(path, "build.cmd", SearchOption.AllDirectories);
            else
                files = Directory.GetFiles(path, "build.sh", SearchOption.AllDirectories);
            if (files.Count() == 0)
                throw new FileNotFoundException();
            return Path.GetDirectoryName(files.First());
        }

        public void FindTestResults()
        {
            var files = Directory.GetFiles(EntryDirectory, "result.xml", SearchOption.AllDirectories);
            foreach (var x in files)
            {
                try
                {
                    var xml = new XmlDocument();
                    using (var fs = new FileStream(x, FileMode.Open, FileAccess.Read))
                    {
                        xml.Load(fs);
                        var root = xml.DocumentElement;
                        foreach (XmlElement y in root.GetElementsByTagName("test"))
                        {
                            try
                            {
                                var args = new TestCaseArgs
                                {
                                    Title = y.GetAttribute("name"),
                                    Time = Convert.ToSingle(y.GetAttribute("time")),
                                    Result = y.GetAttribute("result"),
                                    Method = y.GetAttribute("method")
                                };
                                if (OnTestCaseFound != null)
                                    OnTestCaseFound(this, args);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public void Clean()
        {
            try
            {
                Directory.Delete(EntryDirectory, true);
            }
            catch { }
        }
        public void Run()
        {
            this.Status = CITaskStatus.Building;
            if (OnBeginBuilding != null)
                OnBeginBuilding(this, new BeginBuildingArgs());
            this.Start();
            this.BeginOutputReadLine();
            this.BeginErrorReadLine();
            var flag = this.WaitForExit(MaxTimeLimit);
            if (!flag)
                this.Kill();
            if (this.ExitCode == 0)
            {
                Status = CITaskStatus.Successful;
                if (OnBuildSuccessful != null)
                    OnBuildSuccessful(this, new BuildSuccessfulArgs
                    {
                        ExitCode = this.ExitCode,
                        StartTime = this.StartTime,
                        ExitTime = this.ExitTime,
                        PeakMemoryUsage = 0,
                        TimeUsage = this.TotalProcessorTime,
                        Output = Output
                    });
            }
            else if (!flag)
            {
                Status = CITaskStatus.TimeLimitExceeded;
                if (OnTimeLimitExceeded != null)
                    OnTimeLimitExceeded(this, new TimeLimitExceededArgs
                    {
                        ExitCode = this.ExitCode,
                        StartTime = this.StartTime,
                        ExitTime = this.ExitTime,
                        PeakMemoryUsage = this.PeakWorkingSet64,
                        TimeUsage = this.TotalProcessorTime,
                        Output = Output
                    });
            }
            else
            {
                Status = CITaskStatus.Failed;
                if (OnBuiledFailed != null)
                    OnBuiledFailed(this, new BuildFailedArgs
                    {
                        ExitCode = this.ExitCode,
                        StartTime = this.StartTime,
                        ExitTime = this.ExitTime,
                        PeakMemoryUsage = 0,
                        TimeUsage = this.TotalProcessorTime,
                        Output = Output
                    });
            }
            FindTestResults();
            Clean();
        }
    }
}
