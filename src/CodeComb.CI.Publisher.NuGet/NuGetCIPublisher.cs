using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using CodeComb.Package;

namespace CodeComb.CI.Publisher.NuGet
{
    public class NuGetCIPublisher : ICIPublisher
    {
        public NuGetCIPublisher() { }
        
        public IList<string> Discover(string path,string rules)
        {
            return Directory.GetFiles(path, rules, SearchOption.AllDirectories).ToList();
        }

        public void Publish(string path, string rules, string apikey, string host)
        {
            foreach(var x in Discover(path,rules))
            {
                string fileName, arguments;
                if (OS.Current == OSType.Windows)
                {
                    fileName = "NuGet.exe";
                    arguments = $"push {x} -s {host} {apikey}";
                }
                else
                {
                    fileName = "mono";
                    arguments = $"nuget.exe push {x} -s {host} {apikey}";
                }
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    FileName = fileName,
                    Arguments = arguments
                };
                process.Start();
                process.WaitForExit();
            }
        }
    }
}
