using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Configuration;
using Newtonsoft.Json;
using CodeComb.CI.Runner;
using CodeComb.Package;

namespace CodeComb.vNextChina.Node.Controllers
{
    [Route("api/ci/[action]")]
    public class CIController : BaseController
    {
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

        [FromServices]
        public CIRunner Runner { get; set; }

        [HttpPost]
        public async Task<string> NewCI(long id, int RestoreMethod, string Url, string Branch, string version = null, string AdditionalEnvironmentVariables = null)
        {
            try
            {
                var identifier = id;
                var path = Configuration["Pool"] + "/" + identifier + "/";

                Startup.Client.PostAsync("/api/Runner/BeginBuilding", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "id", identifier.ToString() }
                }));

                var action = "Downloading";
                if (RestoreMethod == 1)
                    action = "Cloning";
                if (RestoreMethod == 2)
                    action = "Checking out";

                Startup.Client.PostAsync("/api/Runner/Output", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "id", identifier.ToString() },
                    { "text", $"{action} from " + Url + "\r\n" }
                }));

                if (RestoreMethod == 0)
                    await Download.DownloadAndExtractAll(Url, path);
                else if (RestoreMethod == 1)
                {
                    var result = GitClone.Clone(Url, path, Branch);
                    if (!result.IsSucceeded)
                    {
                        Startup.Client.PostAsync("/api/Runner/Output", new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            { "id", identifier.ToString() },
                            { "text", $"{result.Error}\r\n" }
                        }));
                        return "ok";
                    }
                }
                else
                {
                    var result = GitClone.Clone(Url, path);
                    if (!result.IsSucceeded)
                    {
                        Startup.Client.PostAsync("/api/Runner/Output", new FormUrlEncodedContent(new Dictionary<string, string>
                        {
                            { "id", identifier.ToString() },
                            { "text", $"{result.Error}\r\n" }
                        }));
                        return "ok";
                    }
                }

                Startup.Client.PostAsync("/api/Runner/Output", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "id", identifier.ToString() },
                    { "text", "Extracted\r\n" }
                }));

                path = FindDirectory(path);
                var environmentvariables = new Dictionary<string, string>();
                try
                {
                    environmentvariables = JsonConvert.DeserializeObject<Dictionary<string, string>>(AdditionalEnvironmentVariables);
                }
                catch { }
                Runner.WaitingTasks.Enqueue(new CITask(identifier.ToString(), path, Runner.MaxTimeLimit, version, environmentvariables));
            }
            catch (Exception e)
            {
                Startup.Client.PostAsync("/api/Runner/Failed", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "id", id.ToString() },
                    { "Output", e.ToString() }
                }));
            }
            return "ok";
        }

        [HttpPost]
        public string NewJudge(long id, IFormFile user, IFormFile problem, string nuget)
        {
            var identifier = id;
            var directory = Configuration["Pool"] + $@"/temp/{identifier}/";
            if (OS.Current == OSType.Windows)
                directory = directory.Replace('/', '\\');
            else
                directory = directory.Replace('\\', '/');
            directory = directory.Replace(@"\\", @"\");
            directory = directory.Replace(@"//", @"/");
            Console.WriteLine("工作路径 "+ Configuration["Pool"] + $@"/temp/{identifier}/");
            if (Directory.Exists(directory))
                Directory.Delete(directory.TrimEnd('\\').TrimEnd('/'), true);
            Directory.CreateDirectory(directory);
            user.SaveAs(directory + $@"/{identifier}.zip");
            Console.WriteLine("用户程序保存成功 " + directory + $@"/{identifier}.zip");
            Unzip.ExtractAll(directory + $@"/{identifier}.zip", directory + "/user/", true);
            Console.WriteLine("用户程序解压成功 " + directory + "/user");
            var tempDirectory = Configuration["Pool"] + "/temp/";
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);
            if (!Directory.Exists(tempDirectory + identifier))
                Directory.CreateDirectory(tempDirectory + identifier);
            problem.SaveAs(tempDirectory + identifier + "/" + identifier + ".zip");
            Console.WriteLine("测试程序保存成功 " + tempDirectory + identifier + "/" + identifier + ".zip");
            Unzip.ExtractAll(tempDirectory + identifier + "/" + identifier + ".zip", tempDirectory + identifier + "/experiment/", true);
            Console.WriteLine("测试程序解压成功 " + tempDirectory + identifier + "/experiment/");
            CopyDirectory(FindRoot(tempDirectory + identifier + "/experiment"), Configuration["Pool"] + "/" + identifier);
            CopyDirectory(FindProject(directory + "/user"), Configuration["Pool"] + "/" + identifier + "/src/user");
            Console.WriteLine($"生成评测目录 {Configuration["Pool"] + "/" + identifier}");
            if (nuget == null) nuget = "";
            var nugetPath = Configuration["Pool"] + "/" + identifier + "/NuGet.config";
            System.IO.File.WriteAllText(nugetPath, GenerateNuGetConfig(nuget.Split('\n')));
            Console.WriteLine($"生成NuGet.config {nugetPath}");
            Runner.WaitingTasks.Enqueue(new CITask(identifier.ToString(), Configuration["Pool"] + "/" + identifier, Runner.MaxTimeLimit));
            return "ok";
        }

        private string GenerateNuGetConfig(string[] feeds)
        {
            var tmp = "";
            foreach (var x in feeds)
                if (!string.IsNullOrEmpty(x.Trim()))
                    tmp += $@"    <add key=""AdditionalFeed"" value=""{x.Trim()}"" />";
            return $@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <packageSources>
    <add key=""nuget.org"" value=""https://www.nuget.org/api/v2"" />
{tmp}
  </packageSources>
  <disabledPackageSources />
</configuration>";
        }

        private string FindProject(string path)
        {
            var tmp = Directory.GetFiles(path, "project.json", SearchOption.AllDirectories);
            var target = tmp.FirstOrDefault();
            if (string.IsNullOrEmpty(target))
                return null;
            return Path.GetDirectoryName(target);
        }

        private string FindRoot(string path)
        {
            string patten;
            patten = "build.cmd";
            if (OS.Current == OSType.Windows)
                patten = "build.cmd";
            else
                patten = "build.sh";
            var tmp = Directory.GetFiles(path, patten, SearchOption.AllDirectories);
            var target = tmp.FirstOrDefault();
            if (string.IsNullOrEmpty(target))
                return null;
            return Path.GetDirectoryName(target);
        }

        private void CopyDirectory(string srcDir, string tgtDir)
        {
            srcDir = srcDir.Replace("//", "/").Replace(@"\\", "\\");
            tgtDir = tgtDir.Replace("//", "/").Replace(@"\\", "\\");

            if (!Directory.Exists(tgtDir))
                Directory.CreateDirectory(tgtDir);

            DirectoryInfo source = new DirectoryInfo(srcDir);
            DirectoryInfo target = new DirectoryInfo(tgtDir);

            if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("父目录不能拷贝到子目录！");
            }

            if (!source.Exists)
            {
                return;
            }

            if (!target.Exists)
            {
                target.Create();
            }

            FileInfo[] files = source.GetFiles();

            for (int i = 0; i < files.Length; i++)
            {
                System.IO.File.Copy(files[i].FullName, target.FullName + (OS.Current == OSType.Windows ? "\\" : "/") + files[i].Name, true);
            }

            DirectoryInfo[] dirs = source.GetDirectories();

            for (int j = 0; j < dirs.Length; j++)
            {
                CopyDirectory(dirs[j].FullName, target.FullName + (OS.Current == OSType.Windows ? "\\" : "/") + dirs[j].Name);
            }
        }

        [HttpPost]
        public string Abort(string id)
        {
            Runner.Abort(id);
            return "ok";
        }
    }
}
