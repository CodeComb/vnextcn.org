using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;
using Microsoft.Data.Entity;
using CodeComb.vNextExperimentCenter.Models;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class ForumController : BaseController
    {
        public IActionResult Index()
        {
            var ret = DB.Forums
                .Include(x => x.SubForums)
                .Where(x => x.SubForums.Count > 0 && x.ParentId == null)
                .OrderBy(x => x.PRI)
                .ToList();
            foreach (var x in ret)
                foreach (var y in x.SubForums)
                    y.LastPost = DB.Posts
                        .Include(z => z.Topic)
                        .ThenInclude(z => z.User)
                        .Where(z => z.Topic.ForumId == y.Id)
                        .OrderByDescending(z => z.Time)
                        .FirstOrDefault();
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
            ViewBag.Title = forum.Title;
            var ret = DB.Topics
                .Include(x => x.User)
                .Include(x => x.Posts)
                .Where(x => x.ForumId == id && !x.IsAnnouncement)
                .OrderByDescending(x => x.IsTop)
                .ThenByDescending(x => x.LastReplyTime);
            foreach (var x in ret)
            {
                x.LastPost = DB.Posts
                    .Include(y => y.User)
                    .Where(y => y.TopicId == x.Id)
                    .OrderByDescending(y => y.Time)
                    .FirstOrDefault();
            }
            var announcements = DB.Topics
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
                    .Where(y => y.TopicId == x.Id)
                    .OrderByDescending(y => y.Time)
                    .FirstOrDefault();
            }
            ViewBag.Announcements = announcements;
            return PagedView(ret, 20);
        }

        [Route("Forum/Topic/{id:long}/{p:int?}")]
        public IActionResult Topic (long id)
        {
            var topic = DB.Topics
                .Include(x => x.Forum)
                .Include(x => x.User)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (topic == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            var posts = DB.Posts
                .Include(x => x.User)
                .Include(x => x.SubPosts)
                .ThenInclude(x => x.User)
                .Where(x => x.TopicId == id && x.ParentId == null)
                .OrderBy(x => x.Time);
            topic.Visit++;
            DB.SaveChanges();
            ViewBag.Topic = topic;
            return PagedView(posts, 10);
        }

        [HttpPost]
        [Authorize]
        [Route("Forum/Post/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult Post(long id, Guid? pid, string Content)
        {
            var topic = DB.Topics
                .Include(x => x.Forum)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (topic == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            if (topic.IsLocked && !User.AnyRoles("Root, Master"))
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您没有权限在已经锁定的主题中发表回复";
                    x.StatusCode = 500;
                });
            var p = new Post
            {
                Content = Content,
                TopicId = id,
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
            topic.Forum.PostCount++;
            DB.Posts.Add(p);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "发表成功";
                x.Details = "您的回复已经成功发表！";
            });
        }
    }
}
