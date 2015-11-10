using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using Microsoft.Data.Entity;
using CodeComb.vNextChina.Models;

namespace CodeComb.vNextChina.Controllers
{
    public class ForumController : BaseController
    {
        public IActionResult Index()
        {
            var begin = DateTime.Today;
            var end = DateTime.Today.AddDays(1);
            var ret = DB.Forums
                .Include(x => x.SubForums)
                .Where(x => x.SubForums.Count > 0 && x.ParentId == null)
                .OrderBy(x => x.PRI)
                .ToList();
            foreach (var x in ret)
            {
                foreach (var y in x.SubForums)
                {
                    y.LastPost = DB.Posts
                        .Include(z => z.User)
                        .Include(z => z.Thread)
                        .ThenInclude(z => z.User)
                        .Where(z => z.Thread.ForumId == y.Id)
                        .OrderByDescending(z => z.Time)
                        .FirstOrDefault();
                    y.TodayCount = DB.Posts
                        .Include(z => z.Thread)
                        .Where(z => z.Thread.ForumId == y.Id && z.Time >= begin && z.Time < end)
                        .Count() + DB.Threads
                        .Where(z => z.ForumId == y.Id && z.CreationTime >= begin && z.CreationTime < end)
                        .Count();
                }
            }
            return View(ret);
        }

        [Route("Forum/{id}")]
        [Route("Forum/{id}/{p:int}")]
        public IActionResult Show(string id)
        {
            var forum = DB.Forums
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (forum == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            ViewBag.Forum = forum;
            ViewBag.Title = forum.Title;
            var ret = DB.Threads
                .Include(x => x.User)
                .Include(x => x.Posts)
                .Where(x => x.ForumId == id && !x.IsAnnouncement)
                .OrderByDescending(x => x.IsTop)
                .ThenByDescending(x => x.LastReplyTime);
            foreach (var x in ret)
            {
                x.LastPost = DB.Posts
                    .Include(y => y.User)
                    .Where(y => y.ThreadId == x.Id)
                    .OrderByDescending(y => y.Time)
                    .FirstOrDefault();
            }
            var announcements = DB.Threads
                .Include(x => x.User)
                .Include(x => x.Posts)
                .Where(x => x.IsAnnouncement)
                .OrderByDescending(x => x.IsTop)
                .ThenByDescending(x => x.LastReplyTime)
                .ToList();
            foreach (var x in announcements)
            {
                x.LastPost = DB.Posts
                    .Include(y => y.User)
                    .Where(y => y.ThreadId == x.Id)
                    .OrderByDescending(y => y.Time)
                    .FirstOrDefault();
            }
            ViewBag.Announcements = announcements;
            return PagedView(ret, 20);
        }

        [Route("Forum/Thread/{id:long}/{p:int?}")]
        public IActionResult Thread (long id)
        {
            var thread = DB.Threads
                .Include(x => x.Forum)
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (thread == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            ViewBag.Count = DB.Posts
                .Where(x => x.ThreadId == id)
                .Count();
            var posts = DB.Posts
                .Include(x => x.User)
                .Include(x => x.SubPosts)
                .ThenInclude(x => x.User)
                .Where(x => x.ThreadId == id && x.ParentId == null)
                .OrderBy(x => x.Time);
            thread.Visit++;
            DB.SaveChanges();
            ViewBag.Thread = thread;
            return PagedView(posts, 10);
        }

        [HttpPost]
        [Authorize]
        [Route("Forum/Post")]
        [Route("Forum/Post/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Post(long id, Guid? pid, string content)
        {
            var thread = DB.Threads
                .Include(x => x.Forum)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (thread == null)
            {
                Response.StatusCode = 404;
                return Content("没有找到主题");
            }
            if (thread.IsLocked && !User.AnyRoles("Root, Master"))
            {
                Response.StatusCode = 500;
                return Content("权限不足");
            }
            var p = new Post
            {
                Content = content,
                ThreadId = id,
                UserId = User.Current.Id,
                Time = DateTime.Now
            };
            if (pid.HasValue)
            {
                var post = DB.Posts
                    .Where(x => x.Id == pid.Value)
                    .SingleOrDefault();
                if (post != null)
                    p.ParentId = post.Id;
            }
            thread.LastReplyTime = DateTime.Now;
            thread.Forum.PostCount++;
            DB.Posts.Add(p);
            DB.SaveChanges();
            if (pid.HasValue)
            {
                vNextChinaHub.Clients.Group("Thread-" + thread.Id).OnPostChanged(p.ParentId);
                return Content(p.ParentId.ToString());
            }
            else
            {
                vNextChinaHub.Clients.Group("Thread-" + thread.Id).OnPostChanged(p.Id);
                return Content(p.Id.ToString());
            }
        }

        [HttpPost]
        [Route("Forum/Thread/Open")]
        [Route("Forum/{id}/Open")]
        [ValidateAntiForgeryToken]
        public IActionResult Open(string id, string Title, string Content)
        {
            var forum = DB.Forums
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (forum == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            var thread = new Thread
            {
                Content = Content,
                ForumId = id,
                LastReplyTime = DateTime.Now,
                CreationTime = DateTime.Now,
                Title = Title,
                UserId = User.Current.Id
            };
            DB.Threads.Add(thread);
            forum.ThreadCount++;
            DB.SaveChanges();
            vNextChinaHub.Clients.Group("Forum-" + thread.ForumId).OnThreadChanged(thread.Id);
            return RedirectToAction("Thread", "Forum", new { id = thread.Id });
        }

        [HttpPost]
        [AnyRoles("Root, Master")]
        [Route("Forum/Thread/Top")]
        [Route("Forum/Thread/Top/{id}")]
        [ValidateAntiForgeryToken]
        public string Top(long id)
        {
            var thread = DB.Threads
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (thread == null)
                return "没有找到该主题";
            thread.IsTop = !thread.IsTop;
            DB.SaveChanges();
            vNextChinaHub.Clients.Group("Forum-" + thread.ForumId).OnThreadChanged(thread.Id);
            if (thread.IsTop)
                return "已经将该主题置顶";
            else
                return "已经取消该主题置顶";
        }

        [HttpPost]
        [AnyRoles("Root, Master")]
        [Route("Forum/Thread/Lock")]
        [Route("Forum/Thread/Lock/{id}")]
        [ValidateAntiForgeryToken]
        public string Lock(long id)
        {
            var thread = DB.Threads
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (thread == null)
                return "没有找到该主题";
            thread.IsLocked = !thread.IsLocked;
            DB.SaveChanges();
            vNextChinaHub.Clients.Group("Forum-" + thread.ForumId).OnThreadChanged(thread.Id);
            if (thread.IsLocked)
                return "已经将该主题锁定";
            else
                return "已经取消该主题锁定";
        }

        [HttpPost]
        [AnyRoles("Root, Master")]
        [Route("Forum/Thread/Notice")]
        [Route("Forum/Thread/Notice/{id}")]
        [ValidateAntiForgeryToken]
        public string Notice(long id)
        {
            var thread = DB.Threads
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (thread == null)
                return "没有找到该主题";
            thread.IsAnnouncement = !thread.IsAnnouncement;
            DB.SaveChanges();
            if (thread.IsAnnouncement)
                return "已经将该主题设置为公告";
            else
                return "该主题已不再是公告帖";
        }

        [HttpPost]
        [Route("Forum/Post/Remove", Order = 0)]
        [Route("Forum/Post/Remove/{id}", Order = 1)]
        [ValidateAntiForgeryToken]
        public string RemovePost(Guid id)
        {
            var post = DB.Posts
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (post == null)
                return "没有找到该回复";
            if (post.UserId != User.Current.Id && !User.AnyRoles("Root, Master"))
                return "权限不足";
            DB.Posts.Remove(post);
            DB.SaveChanges();
            vNextChinaHub.Clients.Group("Thread-" + post.ThreadId).OnPostRemoved(post.Id);
            return "回复已成功删除";
        }

        [HttpPost]
        [Route("Forum/Thread/Edit")]
        [Route("Forum/Thread/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditThread(long id, string content)
        {
            var thread = DB.Threads
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (thread == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            if (thread.UserId != User.Current.Id && !User.AnyRoles("Root, Master"))
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您没有权限删除该主题";
                    x.StatusCode = 500;
                });
            thread.Content = content;
            DB.SaveChanges();
            vNextChinaHub.Clients.Group("Thread-" + thread.Id).OnThreadEdited(thread.Id);
            return Content(Marked.Marked.Parse(content));
        }

        [HttpPost]
        [Route("Forum/Post/Edit")]
        [Route("Forum/Post/Edit/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPost(long id, Guid pid, string content)
        {
            var thread = DB.Threads
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (thread == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            var post = DB.Posts
                .Where(x => x.Id == pid)
                .SingleOrDefault();
            if (post == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            if (post.UserId != User.Current.Id && !User.AnyRoles("Root, Master"))
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您没有权限删除该主题";
                    x.StatusCode = 500;
                });
            post.Content = content;
            DB.SaveChanges();
            vNextChinaHub.Clients.Group("Thread-" + thread.Id).OnPostChanged(post.Id);
            return Content(Marked.Marked.Parse(content));
        }

        [HttpPost]
        [Route("Forum/Thread/Remove")]
        [Route("Forum/Thread/Remove/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveThread(long id)
        {
            var thread = DB.Threads
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (thread == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            if (thread.UserId != User.Current.Id && !User.AnyRoles("Root, Master"))
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您没有权限删除该主题";
                    x.StatusCode = 500;
                });
            DB.Threads.Remove(thread);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "操作成功";
                x.Details = "已经成功删除该主题！";
            });
        }
    }
}
