using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;

namespace CodeComb.vNextChina.Controllers
{
    public class ApiController : BaseController
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var pk = context.HttpContext.Request.Headers["PrivateKey"].ToString();
            if (DB.Nodes.Where(x => x.PrivateKey == pk).Count() == 0)
            {
                context.Result = new ChallengeResult();
                return;
            }
            base.OnActionExecuting(context);
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var pk = context.HttpContext.Request.Headers["PrivateKey"].ToString();
            if (DB.Nodes.Where(x => x.PrivateKey == pk).Count() == 0)
            {
                context.Result = new ChallengeResult();
                return Task.FromResult(403);
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
