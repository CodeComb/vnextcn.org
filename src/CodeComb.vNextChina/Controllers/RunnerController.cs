using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using CodeComb.Package;

namespace CodeComb.vNextChina.Controllers
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
            vNextChinaHub.Clients.Group("Status" + status.Id).OnStatusOutputed(node.OS.ToString(), text);
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
            vNextChinaHub.Clients.Group("StatusList").OnStatusChanged(status.Id);
            if (status.ProjectId.HasValue)
                vNextChinaHub.Clients.Group("CI").OnStatusChanged(status.ProjectId.Value);
            vNextChinaHub.Clients.Group("Status" + status.Id).OnStatusDetailChanged(status.Id);
            return "ok";
        }

        [HttpPost]
        public async Task<string> Failed(long id, [FromHeader(Name = "private-key")]string PrivateKey)
        {
            var node = NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single();
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            switch (node.OS)
            {
                case OSType.Windows:
                    status.WindowsResult = Models.StatusResult.Failed;
                    break;
                case OSType.OSX:
                    status.OsxResult = Models.StatusResult.Failed;
                    break;
                case OSType.Linux:
                    status.LinuxResult = Models.StatusResult.Failed;
                    break;
            }
            status.Result = status.GenerateResult();
            DB.SaveChanges();
            if (status.Result != Models.StatusResult.Building && status.Result != Models.StatusResult.Building)
                await Helpers.FlagBuilder.BuildAsync(DB, status.UserId);
            vNextChinaHub.Clients.Group("Status" + status.Id).OnStatusDetailChanged(status.Id);
            if (status.ProjectId.HasValue)
                vNextChinaHub.Clients.Group("CI").OnStatusChanged(status.ProjectId.Value);
            return "ok";
        }

        [HttpPost]
        public async Task<string> Successful(long id, [FromHeader(Name = "private-key")]string PrivateKey)
        {
            var node = NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single();
            var status = DB.Statuses
                .Include(x => x.Experiment)
                .Where(x => x.Id == id)
                .Single();
            switch (node.OS)
            {
                case OSType.Windows:
                    status.WindowsResult = Models.StatusResult.Successful;
                    break;
                case OSType.OSX:
                    status.OsxResult = Models.StatusResult.Successful;
                    break;
                case OSType.Linux:
                    status.LinuxResult = Models.StatusResult.Successful;
                    break;
            }
            status.Result = status.GenerateResult();
            if (status.ExperimentId.HasValue && status.Result == Models.StatusResult.Successful)
                status.Experiment.Accepted++;
            DB.SaveChanges();
            if (status.Result != Models.StatusResult.Building && status.Result != Models.StatusResult.Building)
                await Helpers.FlagBuilder.BuildAsync(DB, status.UserId);
            vNextChinaHub.Clients.Group("StatusList").OnStatusChanged(status.Id);
            if (status.ProjectId.HasValue)
                vNextChinaHub.Clients.Group("CI").OnStatusChanged(status.ProjectId.Value);
            return "ok";
        }

        [HttpPost]
        public async Task<string> TimeLimitExceeded(long id, [FromHeader(Name = "private-key")]string PrivateKey)
        {
            var node = NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single();
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            status.Result = Models.StatusResult.Failed;
            switch (node.OS)
            {
                case OSType.Windows:
                    status.WindowsResult = Models.StatusResult.Failed;
                    break;
                case OSType.OSX:
                    status.OsxResult = Models.StatusResult.Failed;
                    break;
                case OSType.Linux:
                    status.LinuxResult = Models.StatusResult.Failed;
                    break;
            }
            status.Result = status.GenerateResult();
            DB.SaveChanges();
            if (status.Result != Models.StatusResult.Building && status.Result != Models.StatusResult.Building)
                await Helpers.FlagBuilder.BuildAsync(DB, status.UserId);
            vNextChinaHub.Clients.Group("StatusList").OnStatusChanged(status.Id);
            vNextChinaHub.Clients.Group("Status" + status.Id).OnStatusDetailChanged(status.Id);
            if (status.ProjectId.HasValue)
                vNextChinaHub.Clients.Group("CI").OnStatusChanged(status.ProjectId.Value);
            return "ok";
        }

        [HttpPost]
        public string UpdateCodeLength(long id, long length, [FromHeader(Name = "private-key")]string PrivateKey)
        {
            var node = NodeProvider.Nodes.Where(x => x.PrivateKey == PrivateKey).Single();
            var status = DB.Statuses.Where(x => x.Id == id).Single();
            status.MemoryUsage = length;
            DB.SaveChanges();
            vNextChinaHub.Clients.Group("StatusList").OnStatusChanged(status.Id);
            vNextChinaHub.Clients.Group("Status" + status.Id).OnStatusDetailChanged(status.Id);
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
            vNextChinaHub.Clients.Group("StatusList").OnStatusChanged(status.Id);
            vNextChinaHub.Clients.Group("Status" + status.Id).OnStatusDetailChanged(status.Id);
            vNextChinaHub.Clients.Group("Status" + status.Id).OnStatusCasesChanged(status.Id);
            return "ok";
        }
    }
}
