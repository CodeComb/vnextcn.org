using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    [Route("api/judge/[action]")]
    public class JudgeController : BaseController
    {
        [HttpPost]
        public string Output(long id, string text)
        {
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            status.Output += text;
            if (status.Result == Models.StatusResult.Queued)
                status.Result = Models.StatusResult.Building;
            var result = DB.SaveChanges();
            return "ok";
        }

        [HttpPost]
        public string Failed(long id, [FromHeader(Name = "private-key")]string PrivateKey, string Output)
        {
            NodeProvider.Nodes[NodeProvider.Nodes.IndexOf(NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single())].CurrentThread--;
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            status.Result = Models.StatusResult.Failed;
            status.Output = Output;
            DB.SaveChanges();
            return "ok";
        }

        [HttpPost]
        public string Successful(long id, [FromHeader(Name = "private-key")]string PrivateKey, string Output)
        {
            NodeProvider.Nodes[NodeProvider.Nodes.IndexOf(NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single())].CurrentThread--;
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            status.Result = Models.StatusResult.Successful;
            status.Output = Output;
            DB.SaveChanges();
            return "ok";
        }

        [HttpPost]
        public string TimeLimitExceeded(long id, [FromHeader(Name = "private-key")]string PrivateKey, string Output)
        {
            NodeProvider.Nodes[NodeProvider.Nodes.IndexOf(NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single())].CurrentThread--;
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            status.Result = Models.StatusResult.Failed;
            status.Output = Output;
            DB.SaveChanges();
            return "ok";
        }
    }
}
