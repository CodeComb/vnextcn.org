using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using CodeComb.Package;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    [Route("api/runner/[action]")]
    public class RunnerController : BaseController
    {
        [HttpPost]
        public string Output(long id, string text, [FromHeader(Name = "private-key")]string PrivateKey)
        {
            var node = NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single();
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            switch(node.OS)
            {
                case OSType.Windows:
                    status.WindowsOutput += text;
                    break;
                case OSType.OSX:
                    status.OsxOutput += text;
                    break;
                case OSType.Linux:
                    status.LinuxOutput += text;
                    break;
            }
            status.Result = status.GenerateResult();
            var result = DB.SaveChanges();
            return "ok";
        }

        [HttpPost]
        public string BeginBuilding(long id, [FromHeader(Name = "private-key")]string PrivateKey)
        {
            var node = NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single();
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            if (status.Result == Models.StatusResult.Queued)
                status.Result = Models.StatusResult.Building;
            switch (node.OS)
            {
                case OSType.Windows:
                    status.WindowsResult = Models.StatusResult.Building;
                    status.WindowsResult = Models.StatusResult.Building;
                    break;
                case OSType.OSX:
                    status.OsxResult = Models.StatusResult.Building;
                    status.OsxResult = Models.StatusResult.Building;
                    break;
                case OSType.Linux:
                    status.LinuxResult = Models.StatusResult.Building;
                    status.LinuxResult = Models.StatusResult.Building;
                    break;
            }
            DB.SaveChanges();
            return "ok";
        }

        [HttpPost]
        public string Failed(long id, [FromHeader(Name = "private-key")]string PrivateKey, string Output)
        {
            var node = NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single();
            NodeProvider.Nodes[NodeProvider.Nodes.IndexOf(node)].CurrentThread--;
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            switch (node.OS)
            {
                case OSType.Windows:
                    status.WindowsOutput = Output;
                    status.WindowsResult = Models.StatusResult.Failed;
                    break;
                case OSType.OSX:
                    status.OsxOutput = Output;
                    status.OsxResult = Models.StatusResult.Failed;
                    break;
                case OSType.Linux:
                    status.LinuxOutput = Output;
                    status.LinuxResult = Models.StatusResult.Failed;
                    break;
            }
            status.Result = status.GenerateResult();
            DB.SaveChanges();
            return "ok";
        }

        [HttpPost]
        public string Successful(long id, [FromHeader(Name = "private-key")]string PrivateKey, string Output)
        {
            var node = NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single();
            NodeProvider.Nodes[NodeProvider.Nodes.IndexOf(node)].CurrentThread--;
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            switch (node.OS)
            {
                case OSType.Windows:
                    status.WindowsOutput = Output;
                    status.WindowsResult = Models.StatusResult.Successful;
                    break;
                case OSType.OSX:
                    status.OsxOutput = Output;
                    status.OsxResult = Models.StatusResult.Successful;
                    break;
                case OSType.Linux:
                    status.LinuxOutput = Output;
                    status.LinuxResult = Models.StatusResult.Successful;
                    break;
            }
            status.Result = status.GenerateResult();
            DB.SaveChanges();
            return "ok";
        }

        [HttpPost]
        public string TimeLimitExceeded(long id, [FromHeader(Name = "private-key")]string PrivateKey, string Output, int TimeUsage)
        {
            var node = NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single();
            NodeProvider.Nodes[NodeProvider.Nodes.IndexOf(node)].CurrentThread--;
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            status.Result = Models.StatusResult.Failed;
            switch (node.OS)
            {
                case OSType.Windows:
                    status.WindowsOutput = Output;
                    status.WindowsResult = Models.StatusResult.Failed;
                    break;
                case OSType.OSX:
                    status.OsxOutput = Output;
                    status.OsxResult = Models.StatusResult.Failed;
                    break;
                case OSType.Linux:
                    status.LinuxOutput = Output;
                    status.LinuxResult = Models.StatusResult.Failed;
                    break;
            }
            status.Result = status.GenerateResult();
            DB.SaveChanges();
            return "ok";
        }

        [HttpPost]
        public string UpdateCodeLength(long id, long length, [FromHeader(Name = "private-key")]string PrivateKey)
        {
            var node = NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single();
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            status.MemoryUsage = length;
            DB.SaveChanges();
            return "ok";
        }

        [HttpPost]
        public string PushTestCase(long id, string title, Models.TestCaseResult result, float time, string method, [FromHeader(Name = "private-key")]string PrivateKey)
        {
            var node = NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single();
            var statusDetail = new Models.StatusDetail
            {
                Result = result,
                StatusId = id,
                Time = time,
                Title = title,
                Method = method,
                OS = node.OS
            };
            DB.StatusDetails.Add(statusDetail);
            DB.SaveChanges();

            var status = DB.Statuses
                .Include(x => x.Details)
                .Where(x => x.Id == id)
                .Single();

            status.Accepted = status.Details.Where(x => x.Result == Models.TestCaseResult.Pass).Count();
            status.Total = status.Details.Where(x => x.Result != Models.TestCaseResult.Skip).Count();
            status.TimeUsage = Convert.ToInt64(status.Details.Sum(x => x.Time) * 1000);
            DB.SaveChanges();
            return "ok";
        }
    }
}
