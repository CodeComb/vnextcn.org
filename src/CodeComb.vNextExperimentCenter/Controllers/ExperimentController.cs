using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            IEnumerable<Problem> ret = DB.Problems;
            if (!(User.IsInRole("Root") || User.IsInRole("Manager")))
                ret = ret.Where(x => x.CheckPassed);
            return AjaxPagedView(ret, ".lst-experiments", 100);
        }
        
        [Route("Experiment/{id:long}")]
        public IActionResult Show(long id)
        {
            var exp = DB.Problems
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (exp == null)
                return Prompt(x => 
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            if (!(User.IsInRole("Root") || User.IsInRole("Manager")) && exp.CheckPassed == false)
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
            var exp = DB.Problems
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (exp == null)
                return Prompt(x => 
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            if (!(User.IsInRole("Root") || User.IsInRole("Manager")) && exp.CheckPassed == false)
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
                ProblemId = id,
                Archive = await file.ReadAllBytesAsync()
            };
            DB.Statuses.Add(Status);
            DB.SaveChanges();
            
            NodeProvider.GetFreeNode().SendJudgeTask(Status.Id, Status.Archive, exp.TestArchive, Status.NuGet + "\r\n" + exp.NuGet);

            return RedirectToAction("Show", "Status", new { id = Status.Id });
        }

        [AnyRoles("Root, Master")]
        public IActionResult Edit(long id)
        {
            var exp = DB.Problems
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
    }
}
