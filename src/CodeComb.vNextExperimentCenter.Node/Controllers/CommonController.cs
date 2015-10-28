using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using CodeComb.Package;

namespace CodeComb.vNextExperimentCenter.Node.Controllers
{
    [Route("api/[controller]")]
    public class CommonController : Controller
    {
        [FromServices]
        public IConfiguration Configuration { get; set; }

        [HttpGet]
        public ObjectResult GetNodeInfo()
        {
            var ret = new
            {
                MaxThread = Convert.ToInt32(Configuration["Node:MaxThread"]),
                Platform = OS.Current.ToString(),
                CurrentThread = 0
            };
            return new ObjectResult(ret);
        }
    }
}
