using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

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

        [Route("Forum/Topic/{p}")]
        [Route("Forum/Topic")]
        public IActionResult Topic (long id)
        {
            var topic = DB.Topics
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
    }
}
