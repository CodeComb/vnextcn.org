using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using CodeComb.CI.Runner;
using CodeComb.Package;

namespace CodeComb.vNextExperimentCenter.Node.Controllers
{
    [Route("api/judge/[action]")]
    public class JudgeController : BaseController
    {
        [FromServices]
        public ICIRunner Runner { get; set; }

        [HttpPost]
        public string New(long id, IFormFile user, IFormFile problem, string nuget)
        {
            var identifier = id;
            var directory = Path.GetTempPath() + $@"/vec/{identifier}/";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            user.SaveAs(directory + $@"/{identifier}.zip");
            Unzip.ExtractAll(directory + $@"/{identifier}.zip", directory, true);
            var tempDirectory = Path.GetTempPath() + "/vec/";
            if (!Directory.Exists(tempDirectory))
                Directory.CreateDirectory(tempDirectory);
            if (!Directory.Exists(tempDirectory + identifier))
                Directory.CreateDirectory(tempDirectory + identifier);
            problem.SaveAs(tempDirectory + identifier + "/" + identifier + ".zip");
            Unzip.ExtractAll(tempDirectory + identifier + "/" + identifier + ".zip", tempDirectory + identifier + "/", true);
            CopyDirectory(FindRoot(tempDirectory + identifier), Configuration["JudgePool"] + "/" + identifier);
            CopyDirectory(FindProject(directory), Configuration["JudgePool"] + "/" + identifier + "/src/web");
            System.IO.File.WriteAllText(Configuration["JudgePool"] + "/" + identifier + "/Nuget.config", GenerateNuGetConfig(nuget.Split('\n')));
            Runner.PushTask(directory, identifier);
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
