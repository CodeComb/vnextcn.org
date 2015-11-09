using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;

namespace CodeComb.vNextChina.Controllers
{
    public class RenderController : BaseController
    {
        #region Forum
        public IActionResult Post(Guid id)
        {
            var post = DB.Posts
                .Include(x => x.Thread)
                .ThenInclude(x =>x.User)
                .Include(x => x.User)
                .Include(x => x.SubPosts)
                .ThenInclude(x => x.User)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (post == null)
            {
                Response.StatusCode = 404;
                return Content("");
            }
            return View(post);
        }

        public IActionResult PostContent(Guid id)
        {
            var post = DB.Posts
                .Where(x => x.Id == id)
                .Select(x => x.Content)
                .SingleOrDefault();
            return Content(post);
        }

        public IActionResult Thread(long id)
        {
            var thread = DB.Threads
                .Include(x => x.User)
                .Include(x => x.Posts)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            thread.LastPost = DB.Posts
                .Where(x => x.ThreadId == thread.Id)
                .OrderByDescending(x => x.Time)
                .FirstOrDefault();
            if (thread == null)
                return Content("");
            return View(thread);
        }

        public IActionResult ThreadContent(long id)
        {
            var thread = DB.Threads
                .Where(x => x.Id == id)
                .Select(x => x.Content)
                .SingleOrDefault();
            if (thread == null)
                return Content("");
            else
                return Content(Marked.Marked.Parse(thread));
        }
        #endregion
        #region Status
        public IActionResult Status(long id)
        {
            var status = DB.Statuses
                .Include(x => x.User)
                .Include(x => x.Project)
                .Include(x => x.Experiment)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (status == null)
                return null;
            return View(status);
        }

        public IActionResult StatusDetail(long id)
        {
            var status = DB.Statuses
                .Include(x => x.User)
                .Include(x => x.Project)
                .Include(x => x.Experiment)
                .Include(x => x.Details)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (status == null)
                return null;
            return View(status);
        }

        public IActionResult StatusCases(long id)
        {
            var status = DB.Statuses
                .Include(x => x.User)
                .Include(x => x.Project)
                .Include(x => x.Experiment)
                .Include(x => x.Details)
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (status == null)
                return null;
            return View(status);
        }
        #endregion
        #region CI
        public IActionResult CI(Guid id)
        {
            var project = DB.Projects
                .Where(x => x.Id == id)
                .SingleOrDefault();
            if (project == null)
                return Content("");
            return View(project);
        }
        #endregion
    }
}
