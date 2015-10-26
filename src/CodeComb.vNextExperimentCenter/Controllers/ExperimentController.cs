using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using CodeComb.vNextExperimentCenter.Models;

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
    }
}
