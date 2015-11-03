using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Configuration;
using CodeComb.CI.Runner;
using CodeComb.Package;

namespace CodeComb.vNextExperimentCenter.Node.Controllers
{
    [Route("api/judge/[action]")]
    public class JudgeController : BaseController
    {
        private string FindDirectory(string path)
        {
            string[] files;
            files = Directory.GetFiles(path, "build.cmd", SearchOption.AllDirectories);
            //if (OS.Current == OSType.Windows)
            //    files = Directory.GetFiles(path, "build.cmd", SearchOption.AllDirectories);
            //else
            //    files = Directory.GetFiles(path, "build.sh", SearchOption.AllDirectories);
            if (files.Count() == 0)
                throw new FileNotFoundException();
            return Path.GetDirectoryName(files.First());
        }

        [FromServices]
        public CIRunner Runner { get; set; }

        [HttpPost]
        public string NewJudge(long id, IFormFile user, IFormFile problem, string nuget)
        {
            var identifier = id;
            var directory = Path.GetTempPath() + $@"/vec/{identifier}/";
            if (OS.Current == OSType.Windows)
                directory = directory.Replace('/', '\\');
            else
                directory = directory.Replace('\\', '/');
            directory = directory.Replace(@"\\", @"\");
            directory = directory.Replace(@"//", @"/");
            Console.WriteLine("工作路径 "+ Path.GetTempPath() + $@"/vec/{identifier}/");
            if (Directory.Exists(directory))
                Directory.Delete(directory.TrimEnd('\\').TrimEnd('/'), true);
            Directory.CreateDirectory(directory);
            user.SaveAs(directory + $@"/{identifier}.zip");
            Console.WriteLine("用户程序保存成功 " + directory + $@"/{identifier}.zip");
            Unzip.ExtractAll(directory + $@"/{identifier}.zip", directory + "/user/", true);
            Console.WriteLine("用户程序解压成功 " + directory + "/user");
            var tempDirectory = Path.GetTempPath() + "/vec/";
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);
            if (!Directory.Exists(tempDirectory + identifier))
                Directory.CreateDirectory(tempDirectory + identifier);
            problem.SaveAs(tempDirectory + identifier + "/" + identifier + ".zip");
            Console.WriteLine("测试程序保存成功 " + tempDirectory + identifier + "/" + identifier + ".zip");
            Unzip.ExtractAll(tempDirectory + identifier + "/" + identifier + ".zip", tempDirectory + identifier + "/experiment/", true);
            Console.WriteLine("测试程序解压成功 " + tempDirectory + identifier + "/experiment/");
            CopyDirectory(FindRoot(tempDirectory + identifier + "/experiment"), Configuration["JudgePool"] + "/" + identifier);
            CopyDirectory(FindProject(directory + "/user"), Configuration["JudgePool"] + "/" + identifier + "/src/web");
            Console.WriteLine($"生成评测目录 {Configuration["JudgePool"] + "/" + identifier}");
            if (nuget == null) nuget = "";
            System.IO.File.WriteAllText(Configuration["JudgePool"] + "/" + identifier + "/Nuget.config", GenerateNuGetConfig(nuget.Split('\n')));
            Console.WriteLine($"生成NuGet.config {Configuration["JudgePool"] + "/" + identifier + "/Nuget.config"}");
            Runner.WaitingTasks.Enqueue(new CITask (Configuration["JudgePool"] + "/" + identifier, Runner.MaxTimeLimit)
            {
                Identifier = identifier.ToString()
            });
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
                System.IO.File.Copy(files[i].FullName, target.FullName + @"\" + files[i].Name, true);
            }

            DirectoryInfo[] dirs = source.GetDirectories();

            for (int j = 0; j < dirs.Length; j++)
            {
                CopyDirectory(dirs[j].FullName, target.FullName + @"\" + dirs[j].Name);
            }
        }
    }
}
