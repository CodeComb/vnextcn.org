using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Authorization;
using CodeComb.Package;
using CodeComb.vNextChina.Models;
using CodeComb.vNextChina.ViewModels;

namespace CodeComb.vNextChina.Controllers
{
    [Authorize]
    public class CIController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            var ownedSets = (await UserManager.GetClaimsAsync(User.Current))
                .Where(x => x.Type == "Owned CI set")
                .Select(x => x.Value);
            var ret = DB.CISets
                .Include(x => x.Projects)
                .Where(x => ownedSets.Contains(x.Id.ToString()))
                .OrderByDescending(x => x.LastBuildingTime);
            return View(ret);
        }

        [HttpGet]
        [Route("CI/Set/Create")]
        public IActionResult CreateCISet()
        {
            return View();
        }

        [HttpPost]
        [Route("CI/Set/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCISet(CISet Model)
        {
            Model.CreationTime = DateTime.Now;
            DB.CISets.Add(Model);
            DB.SaveChanges();
            await UserManager.AddClaimAsync(User.Current, new System.Security.Claims.Claim("Owned CI set", Model.Id.ToString()));
            return Prompt(x =>
            {
                x.Title = "创建成功";
                x.Details = "项目集合创建成功，您可以进入该集合添加项目！";
                x.RedirectUrl = Url.Action("Show", "CI", new { id = Model.Id });
                x.RedirectText = "编辑项目集合";
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CI/Set/Build/{id:Guid}")]
        [AnyRolesOrClaims("Root, Master", "Owned CI set")]
        public IActionResult Build(Guid id, Guid pid)
        {
            var project = DB.Projects
                .Where(x => x.CISetId == id && x.Id == pid)
                .SingleOrDefault();
            if (project == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            project.CurrentVersion++;
            var ciset = DB.CISets
                .Where(x => x.Id == id)
                .SingleOrDefault();
            ciset.LastBuildingTime = DateTime.Now;
            var status = new Status
            {
                ProjectId = project.Id,
                Type = StatusType.Project,
                UserId = User.Current.Id,
                Result = StatusResult.Queued,
                Time = DateTime.Now
            };
            DB.Statuses.Add(status);
            DB.SaveChanges();
            if (project.RunWithLinux)
            {
                status.RunWithLinux = true;
                status.LinuxResult = StatusResult.Queued;
                var node = NodeProvider.GetFreeNode(Package.OSType.Linux);
                if (node != null)
                    node.SendCIBuildTask(status.Id, (int)project.RestoreMethod, project.Url, project.Branch, string.Format(project.VersionRule, project.CurrentVersion), project.AdditionalEnvironmentVariables);
                else
                    status.LinuxResult = StatusResult.Ignored;
            }
            if (project.RunWithOsx)
            {
                status.RunWithOsx = true;
                status.OsxResult = StatusResult.Queued;
                var node = NodeProvider.GetFreeNode(Package.OSType.OSX);
                if (node != null)
                    node.SendCIBuildTask(status.Id, (int)project.RestoreMethod, project.Url, project.Branch, string.Format(project.VersionRule, project.CurrentVersion), project.AdditionalEnvironmentVariables);
                else
                    status.OsxResult = StatusResult.Ignored;
            }
            if (project.RunWithWindows)
            {
                status.RunWithWindows = true;
                status.WindowsResult = StatusResult.Queued;
                var node = NodeProvider.GetFreeNode(Package.OSType.Windows);
                if (node != null)
                    node.SendCIBuildTask(status.Id, (int)project.RestoreMethod, project.Url, project.Branch, string.Format(project.VersionRule, project.CurrentVersion), project.AdditionalEnvironmentVariables);
                else
                    status.WindowsResult = StatusResult.Ignored;
            }
            status.Result = status.GenerateResult();
            DB.SaveChanges();
            vNextChinaHub.Clients.Group("StatusList").OnStatusChanged(status.Id);
            return RedirectToAction("Show", "CI", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CI/Set/Build/All")]
        [Route("CI/Set/Build/All/{id:Guid}")]
        [AnyRolesOrClaims("Root, Master", "Owned CI set")]
        public IActionResult BuildAll(Guid id)
        {
            var ciset = DB.CISets
                .Include(x => x.Projects)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (ciset == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            ciset.LastBuildingTime = DateTime.Now;
            foreach (var x in ciset.Projects.OrderBy(x => x.PRI))
            {
                x.CurrentVersion++;
                var status = new Status
                {
                    ProjectId = x.Id,
                    Type = StatusType.Project,
                    UserId = User.Current.Id,
                    Result = StatusResult.Queued,
                    Time = DateTime.Now
                };
                DB.Statuses.Add(status);
                DB.SaveChanges();
                if (x.RunWithLinux)
                {
                    status.RunWithLinux = true;
                    status.LinuxResult = StatusResult.Queued;
                    var node = NodeProvider.GetFreeNode(Package.OSType.Linux);
                    if (node != null)
                        node.SendCIBuildTask(status.Id, (int)x.RestoreMethod, x.Url, x.Branch, string.Format(x.VersionRule, x.CurrentVersion), x.AdditionalEnvironmentVariables);
                    else
                        status.LinuxResult = StatusResult.Ignored;
                }
                if (x.RunWithOsx)
                {
                    status.RunWithOsx = true;
                    status.OsxResult = StatusResult.Queued;
                    var node = NodeProvider.GetFreeNode(Package.OSType.OSX);
                    if (node != null)
                        node.SendCIBuildTask(status.Id, (int)x.RestoreMethod, x.Url, x.Branch, string.Format(x.VersionRule, x.CurrentVersion), x.AdditionalEnvironmentVariables);
                    else
                        status.OsxResult = StatusResult.Ignored;
                }
                if (x.RunWithLinux)
                {
                    status.RunWithWindows = true;
                    status.WindowsResult = StatusResult.Queued;
                    var node = NodeProvider.GetFreeNode(Package.OSType.Windows);
                    if (node != null)
                        node.SendCIBuildTask(status.Id, (int)x.RestoreMethod, x.Url, x.Branch, string.Format(x.VersionRule, x.CurrentVersion), x.AdditionalEnvironmentVariables);
                    else
                        status.WindowsResult = StatusResult.Ignored;
                }
                status.Result = status.GenerateResult();
                DB.SaveChanges();
                vNextChinaHub.Clients.Group("StatusList").OnStatusChanged(status.Id);
            }
            return RedirectToAction("Show", "CI", new { id = ciset.Id });
        }

        [HttpGet]
        [Route("CI/Set/Edit/{id:Guid}")]
        [AnyRolesOrClaims("Root, Master", "Owned CI set")]
        public IActionResult EditCISet(Guid id)
        {
            var ciset = DB.CISets
                .Include(x => x.Projects)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (ciset == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            return View(ciset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CI/Set/Edit/{id:Guid}")]
        [AnyRolesOrClaims("Root, Master", "Owned CI set")]
        public IActionResult EditCISet(Guid id, CISet Model)
        {
            var ciset = DB.CISets
                .Include(x => x.Projects)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (ciset == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            ciset.Title = Model.Title;
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = "项目集合信息已经修改成功！";
                x.RedirectUrl = Url.Action("Show", "CI", new { id = id });
                x.RedirectText = "项目信息";
            });
        }

        [AllowAnonymous]
        [Route("CI/{id:Guid}")]
        public IActionResult Show(Guid id)
        {
            var ciset = DB.CISets
                .Include(x => x.Projects)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (ciset == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            return View(ciset);
        }

        [HttpGet]
        [Route("CI/{id}/Badge.svg")]
        public IActionResult Badge(Guid id, [FromServices] IHostingEnvironment env)
        {
            var status = DB.Statuses
                .Where(x => x.ProjectId == id)
                .OrderByDescending(x => x.Time)
                .FirstOrDefault();
            if (status == null)
                return File(System.IO.File.ReadAllBytes($"{env.WebRootPath}/images/build-queued.svg"), "image/svg+xml");
            else
                return File(System.IO.File.ReadAllBytes($"{env.WebRootPath}/images/build-{status.Result.ToString().ToLower()}.svg"), "image/svg+xml");
        }

        [HttpGet]
        [Route("CI/{id}/{os}/Badge.svg")]
        public IActionResult BadgeWithOS(Guid id, Package.OSType os, [FromServices] IHostingEnvironment env)
        {
            var status = DB.Statuses
                .Where(x => x.ProjectId == id)
                .OrderByDescending(x => x.Time)
                .FirstOrDefault();
            if (status == null)
                return File(System.IO.File.ReadAllBytes($"{env.WebRootPath}/images/{os.ToString().ToLower()}-queued.svg"), "image/svg+xml");
            else
            {
                if (os == Package.OSType.Windows)
                    return File(System.IO.File.ReadAllBytes($"{env.WebRootPath}/images/windows-{status.WindowsResult.ToString().ToLower()}.svg"), "image/svg+xml");
                else if (os == Package.OSType.OSX)
                    return File(System.IO.File.ReadAllBytes($"{env.WebRootPath}/images/osx-{status.OsxResult.ToString().ToLower()}.svg"), "image/svg+xml");
                else
                    return File(System.IO.File.ReadAllBytes($"{env.WebRootPath}/images/linux-{status.LinuxResult.ToString().ToLower()}.svg"), "image/svg+xml");
            }
        }

        [HttpGet]
        [Route("CI/Set/{id:Guid}/Project/Add")]
        [AnyRolesOrClaims("Root, Master", "Owned CI set")]
        public IActionResult AddProject(Guid id)
        {
            var ciset = DB.CISets
                .Include(x => x.Projects)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (ciset == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            return View(ciset);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CI/Set/{id:Guid}/Project/Add")]
        [AnyRolesOrClaims("Root, Master", "Owned CI set")]
        public IActionResult AddProject(Guid id, Project Model)
        {
            var ciset = DB.CISets
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (ciset == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            Model.CISetId = id;
            Model.Id = Guid.NewGuid();
            DB.Projects.Add(Model);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "添加成功";
                x.Details = "项目已经成功添加至该集合中，您可以返回项目列表进行构建";
                x.RedirectText = "返回项目列表";
                x.RedirectUrl = Url.Action("Show", "CI", new { id = id });
            });
        }

        [HttpGet]
        [Route("CI/Set/{id:Guid}/Project/{pid:Guid}/Edit")]
        [AnyRolesOrClaims("Root, Master", "Owned CI set")]
        public IActionResult EditProject(Guid id, Guid pid)
        {
            var ciset = DB.CISets
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (ciset == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            var project = DB.Projects
                .Where(x => x.CISetId == id && x.Id == pid)
                .SingleOrDefault();
            if (project == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            ViewBag.Project = project;
            return View(ciset);
        }

        [HttpPost]
        [Route("CI/Set/{id:Guid}/Project/{pid:Guid}/Edit")]
        [AnyRolesOrClaims("Root, Master", "Owned CI set")]
        public IActionResult EditProject(Guid id, Guid pid, 
            string AdditionalEnvironmentVariables,
            string Alias,
            string NuGetHost,
            string NuGetPrivateKey,
            int PRI,
            bool RunWithLinux,
            bool RunWithWindows,
            bool RunWithOsx,
            string VersionRule,
            string Url,
            int CurrentVersion,
            string Branch,
            ProjectRestoreMethod RestoreMethod)
        {
            var ciset = DB.CISets
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (ciset == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            var project = DB.Projects
                .Where(x => x.CISetId == id && x.Id == pid)
                .SingleOrDefault();
            if (project == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            project.AdditionalEnvironmentVariables = AdditionalEnvironmentVariables;
            project.Alias = Alias;
            project.CurrentVersion = CurrentVersion;
            project.NuGetHost = NuGetHost;
            project.NuGetPrivateKey = NuGetPrivateKey;
            project.PRI = PRI;
            project.RunWithLinux = RunWithLinux;
            project.RunWithOsx = RunWithOsx;
            project.RunWithWindows = RunWithWindows;
            project.VersionRule = VersionRule;
            project.Url = Url;
            project.Branch = Branch;
            project.RestoreMethod = RestoreMethod;
            DB.SaveChanges();
            return Prompt(x => 
            {
                x.Title = "修改成功";
                x.Details = "项目信息已经修改成功！";
                x.RedirectText = "返回项目列表";
                x.RedirectUrl = this.Url.Action("Show", "CI", new { id = id });
            });
        }
        
        [AllowAnonymous]
        [Route("CI/Project/{id}")]
        public IActionResult Project(Guid id)
        {
            var project = DB.Projects
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (project == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            var status = DB.Statuses
                .Where(x => x.ProjectId == id)
                .OrderByDescending(x => x.Time)
                .FirstOrDefault();
            ViewBag.Status = status;
            return View(project);
        }
    }
}
