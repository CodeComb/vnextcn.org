using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Configuration;
using CodeComb.Package;
using CodeComb.CI.Runner;

namespace CodeComb.vNextExperimentCenter.Node.Controllers
{
    [Route("api/common/[action]")]
    public class CommonController : BaseController
    {
        [FromServices]
        public ICIRunner Runner { get; set; }

        [HttpGet]
        public ObjectResult GetNodeInfo()
        {
            var ret = new
            {
                MaxThread = Convert.ToInt32(Configuration["Node:MaxThread"]),
                Platform = OS.Current.ToString(),
                CurrentThread = Runner.TaskQueue.Count
            };
            return new ObjectResult(ret);
        }

        [HttpGet]
        public string HeartBeat()
        {
            return "ok";
        }
    }
}
