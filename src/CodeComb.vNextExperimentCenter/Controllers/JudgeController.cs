using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    [Route("api/judge/{action}")]
    public class JudgeController : ApiController
    {
        [HttpPost]
        public string Output(long id, string text)
        {
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            if (status.Output == null)
                status.Output = "";
            status.Output += text;
            status.Result = Models.StatusResult.Building;
            DB.SaveChanges();
            return "ok";
        }

        [HttpPost]
        public string Failed(long id, [FromHeader(Name = "private-key")]string PrivateKey)
        {
            NodeProvider.Nodes[NodeProvider.Nodes.IndexOf(NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single())].CurrentThread--;
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            status.Result = Models.StatusResult.Failed;
            DB.SaveChanges();
            return "ok";
        }

        [HttpPost]
        public string Successful(long id, [FromHeader(Name = "private-key")]string PrivateKey)
        {
            NodeProvider.Nodes[NodeProvider.Nodes.IndexOf(NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single())].CurrentThread--;
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            status.Result = Models.StatusResult.Successful;
            DB.SaveChanges();
            return "ok";
        }

        [HttpPost]
        public string TimeLimitExceeded(long id, [FromHeader(Name = "private-key")]string PrivateKey)
        {
            NodeProvider.Nodes[NodeProvider.Nodes.IndexOf(NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single())].CurrentThread--;
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            status.Result = Models.StatusResult.Failed;
            DB.SaveChanges();
            return "ok";
        }
    }
}
