using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Authorization;
using CodeComb.vNextExperimentCenter.Models;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    [Authorize]
    public class CIController : BaseController
    {
        public IActionResult Index()
        {
            var ret = DB.CISets
                .Include(x => x.Projects)
                .Where(x => x.UserId == User.Current.Id)
                .OrderByDescending(x => x.LastBuildingTime);
            return View(ret);
        }

        [HttpGet]
        [Route("CI/Set/Create")]
        public IActionResult CreateCISet()
        {
            return View();
        }

        [HttpPost]
        [Route("CI/Set/Create")]
        public IActionResult CreateCISet(CISet Model)
        {
            Model.UserId = User.Current.Id;
            Model.CreationTime = DateTime.Now;
            DB.CISets.Add(Model);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "创建成功";
                x.Details = "项目集合创建成功，您可以进入该集合添加项目！";
                x.RedirectUrl = Url.Action("Show", "CI", new { id = Model.Id });
                x.RedirectText = "编辑项目集合";
            });
        }
    }
}
