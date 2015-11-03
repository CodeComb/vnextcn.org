using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using CodeComb.vNextExperimentCenter.Models;
using CodeComb.vNextExperimentCenter.Hub;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class ExperimentController : BaseController
    {
        public IActionResult Index()
        {
            IEnumerable<Experiment> ret = DB.Experiments;
            if (!User.AnyRoles("Root, Manager"))
                ret = ret.Where(x => x.CheckPassed);
            return AjaxPagedView(ret, ".lst-experiments", 100);
        }
        
        [Route("Experiment/{id:long}")]
        public IActionResult Show(long id)
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
                
            var Status = new Status
            {
                UserId = User.Current.Id,
                Time = DateTime.Now,
                Result = StatusResult.Queued,
                ExperimentId = id,
                Archive = await file.ReadAllBytesAsync(),
                MemoryUsage = file.Length / 1024,
                
            };
            DB.Statuses.Add(Status);
            DB.SaveChanges();
            
            await NodeProvider.GetFreeNode().SendJudgeTask(Status.Id, Status.Archive, exp.TestArchive, Status.NuGet + "\r\n" + exp.NuGet);

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
