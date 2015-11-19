using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace CodeComb.vNextChina.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            ViewBag.Forums = DB.Forums
                .Include(x => x.SubForums)
                .Where(x => x.ParentId == null)
                .OrderBy(x => x.PRI)
                .ToList();
            var contests = DB.Contests
                .Where(x => x.Begin <= DateTime.Now && DateTime.Now <= x.End)
                .OrderBy(x => x.End)
                .ToList();
            if (contests.Count < 5)
                contests.InsertRange(contests.Count,DB.Contests
                .Where(x => x.Begin > DateTime.Now)
                .OrderBy(x => x.End)
                .ToList());
            if (contests.Count < 5)
                contests.InsertRange(contests.Count, DB.Contests
                .Where(x => x.End < DateTime.Now)
                .OrderBy(x => x.End)
                .ToList());
            ViewBag.Contests = contests;
            return View();
        }

        public IActionResult Node()
        {
            return View(NodeProvider.Nodes);
        }
    }
}
