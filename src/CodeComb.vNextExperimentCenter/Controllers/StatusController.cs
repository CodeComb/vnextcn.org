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
    }
}
