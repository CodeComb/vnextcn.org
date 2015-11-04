using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
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
            status.WindowsResult = Models.StatusResult.Queued;
            status.LinuxResult = Models.StatusResult.Queued;
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
                    await node.SendJudgeTask(status.Id, status.Archive, status.Experiment.TestArchive, status.NuGet + "\r\n" + status.Experiment.NuGet);
            }
            if (status.RunWithWindows)
            {
                var node = NodeProvider.GetFreeNode(Package.OSType.Windows);
                if (node == null)
                    status.WindowsResult = Models.StatusResult.Ignored;
                else
                    await node.SendJudgeTask(status.Id, status.Archive, status.Experiment.TestArchive, status.NuGet + "\r\n" + status.Experiment.NuGet);
            }
            if (status.RunWithOsx)
            {
                var node = NodeProvider.GetFreeNode(Package.OSType.OSX);
                if (node == null)
                    status.OsxResult = Models.StatusResult.Ignored;
                else
                    await node.SendJudgeTask(status.Id, status.Archive, status.Experiment.TestArchive, status.NuGet + "\r\n" + status.Experiment.NuGet);
            }
            DB.SaveChanges();

            return Prompt(x =>
            {
                x.Title = "重新运行指令已下达";
                x.Details = "该任务已经加入待运行队列中，稍后即将开始重新运行！";
            });
        }
    }
}
