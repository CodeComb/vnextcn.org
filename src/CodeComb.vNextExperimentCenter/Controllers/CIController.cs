using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.AspNet.Authorization;
using CodeComb.vNextExperimentCenter.Models;

namespace CodeComb.vNextExperimentCenter.Controllers
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
        [Route("CI/Set/Build/{id:Guid}")]
        [ValidateAntiForgeryToken]
        [AnyRolesOrClaims("Root, Master", "Owned CI set")]
        public async Task<IActionResult> Build(Guid id, Guid pid)
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
            var status = new Status
            {
                ProjectId = project.Id,
                Type = StatusType.Project,
                UserId = User.Current.Id,
                Result = StatusResult.Queued
            };
            DB.Statuses.Add(status);
            DB.SaveChanges();
            if (project.RunWithLinux)
            {
                status.RunWithLinux = true;
                status.LinuxResult = StatusResult.Queued;
                var node = NodeProvider.GetFreeNode(Package.OSType.Linux);
                if (node != null)
                    await node.SendCIBuildTask(status.Id, project.ZipUrl, project.AdditionalEnvironmentVariables);
                else
                    status.LinuxResult = StatusResult.Ignored;
            }
            if (project.RunWithOsx)
            {
                status.RunWithLinux = true;
                status.OsxResult = StatusResult.Queued;
                var node = NodeProvider.GetFreeNode(Package.OSType.OSX);
                if (node != null)
                    await node.SendCIBuildTask(status.Id, project.ZipUrl, project.AdditionalEnvironmentVariables);
                else
                    status.OsxResult = StatusResult.Ignored;
            }
            if (project.RunWithLinux)
            {
                status.RunWithWindows = true;
                status.WindowsResult = StatusResult.Queued;
                var node = NodeProvider.GetFreeNode(Package.OSType.Windows);
                if (node != null)
                    await node.SendCIBuildTask(status.Id, project.ZipUrl, project.AdditionalEnvironmentVariables);
                else
                    status.WindowsResult = StatusResult.Ignored;
            }
            DB.SaveChanges();
            return RedirectToAction("Show", "CI", new { id = id });
        }

        [HttpPost]
        [Route("CI/Set/Build/All/{id:Guid}")]
        [ValidateAntiForgeryToken]
        [AnyRolesOrClaims("Root, Master", "Owned CI set")]
        public async Task<IActionResult> BuildAll(Guid id)
        {
            var ciset = DB.CISets
                .Include(x => x.Projects)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            foreach(var x in ciset.Projects.OrderBy(x => x.PRI))
            {
                var status = new Status
                {
                    ProjectId = x.Id,
                    Type = StatusType.Project,
                    UserId = User.Current.Id,
                    Result = StatusResult.Queued
                };
                DB.Statuses.Add(status);
                DB.SaveChanges();
                if (x.RunWithLinux)
                {
                    status.RunWithLinux = true;
                    status.LinuxResult = StatusResult.Queued;
                    var node = NodeProvider.GetFreeNode(Package.OSType.Linux);
                    if (node != null)
                        await node.SendCIBuildTask(status.Id, x.ZipUrl, x.AdditionalEnvironmentVariables);
                    else
                        status.LinuxResult = StatusResult.Ignored;
                }
                if (x.RunWithOsx)
                {
                    status.RunWithLinux = true;
                    status.OsxResult = StatusResult.Queued;
                    var node = NodeProvider.GetFreeNode(Package.OSType.OSX);
                    if (node != null)
                        await node.SendCIBuildTask(status.Id, x.ZipUrl, x.AdditionalEnvironmentVariables);
                    else
                        status.OsxResult = StatusResult.Ignored;
                }
                if (x.RunWithLinux)
                {
                    status.RunWithWindows = true;
                    status.WindowsResult = StatusResult.Queued;
                    var node = NodeProvider.GetFreeNode(Package.OSType.Windows);
                    if (node != null)
                        await node.SendCIBuildTask(status.Id, x.ZipUrl, x.AdditionalEnvironmentVariables);
                    else
                        status.WindowsResult = StatusResult.Ignored;
                }
                DB.SaveChanges();
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
    }
}
