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
            ViewBag.Forum = forum;
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
            ViewBag.Count = DB.Posts
                .Where(x => x.TopicId == id)
                .Count();
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
        [Route("Forum/Post")]
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
            topic.LastReplyTime = DateTime.Now;
            topic.Forum.PostCount++;
            DB.Posts.Add(p);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "发表成功";
                x.Details = "您的回复已经成功发表！";
            });
        }

        [HttpPost]
        [Route("Forum/Topic/Open")]
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
            var topic = new Topic
            {
                Content = Content,
                ForumId = id,
                LastReplyTime = DateTime.Now,
                CreationTime = DateTime.Now,
                Title = Title,
                UserId = User.Current.Id
            };
            DB.Topics.Add(topic);
            forum.TopicCount++;
            DB.SaveChanges();
            return RedirectToAction("Topic", "Forum", new { id = topic.Id });
        }

        [HttpPost]
        [AnyRoles("Root, Master")]
        [Route("Forum/Topic/Top")]
        [Route("Forum/Topic/Top/{id}")]
        [ValidateAntiForgeryToken]
        public string Top(long id)
        {
            var topic = DB.Topics
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (topic == null)
                return "没有找到该主题";
            topic.IsTop = !topic.IsTop;
            DB.SaveChanges();
            if (topic.IsTop)
                return "已经将该主题置顶";
            else
                return "已经取消该主题置顶";
        }

        [HttpPost]
        [AnyRoles("Root, Master")]
        [Route("Forum/Topic/Lock")]
        [Route("Forum/Topic/Lock/{id}")]
        [ValidateAntiForgeryToken]
        public string Lock(long id)
        {
            var topic = DB.Topics
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (topic == null)
                return "没有找到该主题";
            topic.IsLocked = !topic.IsLocked;
            DB.SaveChanges();
            if (topic.IsLocked)
                return "已经将该主题锁定";
            else
                return "已经取消该主题锁定";
        }

        [HttpPost]
        [AnyRoles("Root, Master")]
        [Route("Forum/Topic/Notice")]
        [Route("Forum/Topic/Notice/{id}")]
        [ValidateAntiForgeryToken]
        public string Notice(long id)
        {
            var topic = DB.Topics
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (topic == null)
                return "没有找到该主题";
            topic.IsAnnouncement = !topic.IsAnnouncement;
            DB.SaveChanges();
            if (topic.IsAnnouncement)
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
            return "回复已成功删除";
        }

        [HttpPost]
        [Route("Forum/Topic/Remove")]
        [Route("Forum/Topic/Remove/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveTopic(long id)
        {
            var topic = DB.Topics
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (topic == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            if (topic.UserId != User.Current.Id && !User.AnyRoles("Root, Master"))
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您没有权限删除该主题";
                    x.StatusCode = 500;
                });
            DB.Topics.Remove(topic);
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "操作成功";
                x.Details = "已经成功删除该主题！";
            });
        }
    }
}
