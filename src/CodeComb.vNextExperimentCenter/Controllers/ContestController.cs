using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class ContestController : BaseController
    {
        public IActionResult Index()
        {
            ViewBag.InProgress = DB.Contests
                .Where(x => x.Begin <= DateTime.Now && DateTime.Now <= x.End)
                .OrderBy(x => x.End)
                .ToList();

            return View();
        }
    }
}
