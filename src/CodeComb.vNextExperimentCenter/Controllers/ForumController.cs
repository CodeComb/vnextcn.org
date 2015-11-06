using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class ForumController : BaseController
    {
        public IActionResult Index()
        {
            var ret = DB.Forums
                .Include(x => x.SubForums)
                .Where(x => x.SubForums.Count > 0 && x.ParentId == null)
                .OrderBy(x => x.PRI)
                .ToList();
            foreach (var x in ret)
                foreach (var y in x.SubForums)
                    y.LastPost = DB.Posts
                        .Include(z => z.Topic)
                        .ThenInclude(z => z.User)
                        .Where(z => z.Topic.ForumId == y.Id)
                        .OrderByDescending(z => z.Time)
                        .FirstOrDefault();
            return View(ret);
        }
    }
}
