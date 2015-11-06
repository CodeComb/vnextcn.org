using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class StatusController : BaseController
    {
        public IActionResult Index()
        {
            var ret = DB.Statuses
                .Include(x => x.Experiment)
                .Include(x => x.Project)
                .Include(x => x.User)
                .OrderByDescending(x => x.Time);
            return AjaxPagedView(ret, ".lst-statuses");
        }

        [Route("Status/{id:long}")]
        public IActionResult Show(long id)
        {
            var status = DB.Statuses
                .Include(x => x.User)
                .Include(x => x.Experiment)
                .Include(x => x.Details)
                .Include(x => x.Project)
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (status == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            return View(status);
        }
        
        public async Task<IActionResult> Rerun(long id)
        {
            var status = DB.Statuses
                .Include(x => x.Experiment)
                .Include(x => x.Details)
                .Include(x => x.Project)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (status == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            if (status.Result == Models.StatusResult.Queued || status.Result == Models.StatusResult.Building)
                return Prompt(x =>
                {
                    x.Title = "现在不能重新运行";
                    x.Details = "该任务还没有运行或正在运行，在该状态下不能执行重新运行的操作！";
                    x.StatusCode = 400;
                });
            status.Result = Models.StatusResult.Queued;
            if (status.RunWithWindows)
                status.WindowsResult = Models.StatusResult.Queued;
            if (status.RunWithLinux)
                status.LinuxResult = Models.StatusResult.Queued;
            if (status.RunWithOsx)
                status.OsxResult = Models.StatusResult.Queued;
            status.WindowsOutput = "";
            status.LinuxOutput = "";
            status.OsxOutput = "";
            status.TimeUsage = 0;
            foreach (var x in status.Details)
                DB.StatusDetails.Remove(x);
            DB.SaveChanges();

            if (status.RunWithLinux)
            {
                var node = NodeProvider.GetFreeNode(Package.OSType.Linux);
                if (node == null)
                    status.LinuxResult = Models.StatusResult.Ignored;
                else
                {
                    if (status.ExperimentId.HasValue)
                        node.SendJudgeTask(status.Id, status.Archive, status.Experiment.TestArchive, status.NuGet + "\r\n" + status.Experiment.NuGet);
                    else
                        node.SendCIBuildTask(status.Id, status.Project.ZipUrl, status.Project.AdditionalEnvironmentVariables);
                }
            }
            if (status.RunWithWindows)
            {
                var node = NodeProvider.GetFreeNode(Package.OSType.Windows);
                if (node == null)
                    status.WindowsResult = Models.StatusResult.Ignored;
                else
                {
                    if (status.ExperimentId.HasValue)
                        node.SendJudgeTask(status.Id, status.Archive, status.Experiment.TestArchive, status.NuGet + "\r\n" + status.Experiment.NuGet);
                    else
                        node.SendCIBuildTask(status.Id, status.Project.ZipUrl, status.Project.AdditionalEnvironmentVariables);
                }
            }
            if (status.RunWithOsx)
            {
                var node = NodeProvider.GetFreeNode(Package.OSType.OSX);
                if (node == null)
                    status.OsxResult = Models.StatusResult.Ignored;
                else
                {
                    if (status.ExperimentId.HasValue)
                        node.SendJudgeTask(status.Id, status.Archive, status.Experiment.TestArchive, status.NuGet + "\r\n" + status.Experiment.NuGet);
                    else
                        node.SendCIBuildTask(status.Id, status.Project.ZipUrl, status.Project.AdditionalEnvironmentVariables);
                }
            }
            DB.SaveChanges();

            return Prompt(x =>
            {
                x.Title = "重新运行指令已下达";
                x.Details = "该任务已经加入等待队列中，稍后即将开始重新运行！";
            });
        }
        
        [HttpGet]
        public IActionResult Output(long id, Package.OSType os)
        {
            var status = DB.Statuses
                .Where(x => x.Id == id)
                .SingleOrDefault();

            if (status == null)
                return Content("Not found.");

            switch (os)
            {
                case Package.OSType.Linux:
                    return Content(status.LinuxOutput);
                case Package.OSType.OSX:
                    return Content(status.OsxOutput);
                case Package.OSType.Windows:
                    return Content(status.WindowsOutput);
                default: return Content("Not found.");
            }
        }

        [HttpGet]
        [Route("Status/{id}/Badge.svg")]
        public IActionResult Badge(long id, [FromServices] IHostingEnvironment env)
        {
            var status = DB.Statuses
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (status == null)
                return new HttpNotFoundResult();
            var path = $"{env.WebRootPath}/images/build-{status.Result.ToString().ToLower()}.svg";
            return File(System.IO.File.ReadAllBytes(path), "image/svg+xml");
        }

        [HttpGet]
        [Route("Status/{id}/{os}/Badge.svg")]
        public IActionResult BadgeWithOS(long id, Package.OSType os, [FromServices] IHostingEnvironment env)
        {
            var status = DB.Statuses
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (status == null)
                return new HttpNotFoundResult();
            if (os == Package.OSType.Linux)
                return File(System.IO.File.ReadAllBytes($"{env.WebRootPath}/images/linux-{status.LinuxResult.ToString().ToLower()}.svg"), "image/svg+xml");
            else if (os == Package.OSType.OSX)
                return File(System.IO.File.ReadAllBytes($"{env.WebRootPath}/images/osx-{status.OsxResult.ToString().ToLower()}.svg"), "image/svg+xml");
            else
                return File(System.IO.File.ReadAllBytes($"{env.WebRootPath}/images/windows-{status.WindowsResult.ToString().ToLower()}.svg"), "image/svg+xml");
        }
    }
}
