using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Configuration;
using CodeComb.Package;
using CodeComb.CI.Runner;

namespace CodeComb.vNextChina.Node.Controllers
{
    [Route("api/common/[action]")]
    public class CommonController : BaseController
    {
        [FromServices]
        public CIRunner Runner { get; set; }

        [HttpGet]
        public ObjectResult GetNodeInfo()
        {
            var ret = new
            {
                MaxThread = Runner.MaxThreads,
                Platform = OS.Current.ToString(),
                CurrentThread = Runner.CurrentTasks.Count
            };
            return new ObjectResult(ret);
        }

        [HttpGet]
        public string HeartBeat()
        {
            Console.WriteLine($"收到心跳测试请求，当前队列任务数：{Runner.CurrentTasks.Count}");
            return "ok";
        }
    }
}
