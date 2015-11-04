using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using CodeComb.vNextExperimentCenter.Models;
using CodeComb.vNextExperimentCenter.Hub;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class ExperimentController : BaseController
    {
        public IActionResult Index()
        {
            IEnumerable<Experiment> ret = DB.Experiments
                .Include(x => x.Contests)
                .ThenInclude(x => x.Contest);
            if (!User.AnyRoles("Root, Master"))
                ret = ret.Where(x => x.CheckPassed)
                    .Where(x => x.Contests.Count == 0 || x.Contests.Max(y => y.Contest.End) <= DateTime.Now); // 隐藏比赛题目
            return AjaxPagedView(ret, ".lst-experiments", 100);
        }
        
        [Route("Experiment/{id:long}")]
        public IActionResult Show(long id)
        {
            var exp = DB.Experiments
                .Include(x => x.Contests)
                .ThenInclude(x => x.Contest)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (exp == null)
                return Prompt(x => 
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            if (!User.AnyRoles("Root, Master") && exp.CheckPassed == false)
                return Prompt(x => 
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            if (!User.AnyRoles("Root, Master") && exp.Contests.Count > 0 && exp.Contests.Max(y => y.Contest.End) > DateTime.Now)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            return View(exp);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(long id, IFormFile file, string nuget)
        {
            var exp = DB.Experiments
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (exp == null)
                return Prompt(x => 
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            if (!User.AnyRoles("Root, Master") && exp.CheckPassed == false)
                return Prompt(x => 
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            if (!User.AnyRoles("Root, Master") && exp.Contests.Count > 0 && exp.Contests.Max(y => y.Contest.End) > DateTime.Now)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            var Status = new Status
            {
                UserId = User.Current.Id,
                Time = DateTime.Now,
                Result = StatusResult.Queued,
                ExperimentId = id,
                Archive = await file.ReadAllBytesAsync(),
                MemoryUsage = file.Length / 1024
            };

            switch(exp.OS)
            {
                case OSType.CrossPlatform:
                    Status.RunWithLinux = true;
                    Status.RunWithOsx = true;
                    Status.RunWithWindows = true;
                    Status.LinuxResult = StatusResult.Queued;
                    Status.OsxResult = StatusResult.Queued;
                    Status.WindowsResult = StatusResult.Queued;
                    break;
                case OSType.Random:
                    switch (NodeProvider.GetFreeNode().OS)
                    {
                        case Package.OSType.Linux:
                            Status.RunWithLinux = true;
                            Status.LinuxResult = StatusResult.Queued;
                            break;
                        case Package.OSType.OSX:
                            Status.RunWithOsx = true;
                            Status.OsxResult = StatusResult.Queued;
                            break;
                        case Package.OSType.Windows:
                            Status.RunWithWindows = true;
                            Status.WindowsResult = StatusResult.Queued;
                            break;
                    }
                    break;
                case OSType.Linux:
                    Status.RunWithLinux = true;
                    Status.LinuxResult = StatusResult.Queued;
                    break;
                case OSType.OSX:
                    Status.RunWithOsx = true;
                    Status.OsxResult = StatusResult.Queued;
                    break;
                case OSType.Windows:
                    Status.RunWithWindows = true;
                    Status.WindowsResult = StatusResult.Queued;
                    break;
            }

            DB.Statuses.Add(Status);
            DB.SaveChanges();

            if (Status.RunWithLinux)
            {
                var node = NodeProvider.GetFreeNode(Package.OSType.Linux);
                if (node == null)
                    Status.LinuxResult = Models.StatusResult.Ignored;
                else
                    await node.SendJudgeTask(Status.Id, Status.Archive, Status.Experiment.TestArchive, Status.NuGet + "\r\n" + Status.Experiment.NuGet);
            }
            if (Status.RunWithWindows)
            {
                var node = NodeProvider.GetFreeNode(Package.OSType.Windows);
                if (node == null)
                    Status.WindowsResult = Models.StatusResult.Ignored;
                else
                    await node.SendJudgeTask(Status.Id, Status.Archive, Status.Experiment.TestArchive, Status.NuGet + "\r\n" + Status.Experiment.NuGet);
            }
            if (Status.RunWithOsx)
            {
                var node = NodeProvider.GetFreeNode(Package.OSType.OSX);
                if (node == null)
                    Status.OsxResult = Models.StatusResult.Ignored;
                else
                    await node.SendJudgeTask(Status.Id, Status.Archive, Status.Experiment.TestArchive, Status.NuGet + "\r\n" + Status.Experiment.NuGet);
            }
            Status.Result = Status.GenerateResult();
            DB.SaveChanges();

            return RedirectToAction("Show", "Status", new { id = Status.Id });
        }

        [AnyRoles("Root, Master")]
        public IActionResult Edit(long id)
        {
            var exp = DB.Experiments
                .Where(x => x.Id == id)
                .SingleOrDefault();

            if (exp == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });

            return View(exp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, IFormFile TestArchive, IFormFile AnswerArchive, Experiment Model)
        {
            var exp = DB.Experiments
               .Where(x => x.Id == id)
               .SingleOrDefault();

            if (exp == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });

            exp.Title = Model.Title;
            exp.Namespace = Model.Namespace;
            exp.NuGet = Model.NuGet;
            exp.OS = Model.OS;
            exp.TimeLimit = Model.TimeLimit;
            if (TestArchive != null)
                exp.TestArchive = await TestArchive.ReadAllBytesAsync();
            if (AnswerArchive != null)
                exp.AnswerArchive = await AnswerArchive.ReadAllBytesAsync();
            exp.Difficulty = Model.Difficulty;
            exp.Version = Model.Version;
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = "该实验已保存成功！";
            });
        }
    }
}
