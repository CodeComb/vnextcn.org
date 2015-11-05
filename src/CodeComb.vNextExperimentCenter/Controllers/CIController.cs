using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Authorization;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class CIController : BaseController
    {
        [Authorize]
        public IActionResult Index()
        {
            var ret = DB.CISets
                .Include(x => x.Projects)
                .Where(x => x.UserId == User.Current.Id)
                .OrderByDescending(x => x.LastBuildingTime);
            return View(ret);
        }
    }
}
