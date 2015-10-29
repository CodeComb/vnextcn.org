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
                .Include(x => x.Problem)
                .Include(x => x.User)
                .OrderByDescending(x => x.Time);
            return AjaxPagedView(ret, ".lst-statuses");
        }

        [Route("Status/{id:long}")]
        public IActionResult Show(long id)
        {
            var status = DB.Statuses
                .Include(x => x.User)
                .Include(x => x.Problem)
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
                .Include(x => x.Problem)
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
            status.Output = "";
            status.MemoryUsage = 0;
            status.TimeUsage = 0;
            DB.SaveChanges();
            await NodeProvider.GetFreeNode().SendJudgeTask(status.Id, status.Archive, status.Problem.TestArchive, status.NuGet + "\r\n" + status.Problem.NuGet);
            return Prompt(x =>
            {
                x.Title = "重新运行指令已下达";
                x.Details = "该任务已经加入待运行队列中，稍后即将开始重新运行！";
            });
        }
    }
}
