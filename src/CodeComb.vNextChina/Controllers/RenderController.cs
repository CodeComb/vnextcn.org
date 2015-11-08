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
                .Include(x => x.Topic)
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
        #endregion
    }
}
