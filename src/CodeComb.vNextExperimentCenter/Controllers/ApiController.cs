using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class ApiController : BaseController
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!DB.Nodes.Any(x => x.PrivateKey == context.HttpContext.Request.Headers["PrivateKey"].ToString()))
            {
                context.Result = new ChallengeResult();
                return;
            }
            base.OnActionExecuting(context);
        }

        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!DB.Nodes.Any(x => x.PrivateKey == context.HttpContext.Request.Headers["PrivateKey"].ToString()))
            {
                context.Result = new ChallengeResult();
                return Task.FromResult(403);
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
