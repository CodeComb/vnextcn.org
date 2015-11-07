using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Framework.Configuration;

namespace CodeComb.vNextChina.Node.Controllers
{
    public class BaseController : Controller
    {
        [FromServices]
        public IConfiguration Configuration { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.Headers["private-key"].ToString() != Configuration["PrivateKey"].ToString())
            {
                context.Result = new ChallengeResult();
                return;
            }
            base.OnActionExecuting(context);
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Headers["private-key"].ToString() != Configuration["PrivateKey"].ToString())
            {
                context.Result = new ChallengeResult();
                return Task.FromResult(403);
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
