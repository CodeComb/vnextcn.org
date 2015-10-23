using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class ExperimentController : BaseController
    {
        public IActionResult Index()
        {
            return AjaxPagedView(DB.Problems, ".lst-experiments", 100);
        }
    }
}
