using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using CodeComb.vNextChina.Models;

namespace Microsoft.AspNet.Mvc.Rendering
{
    public static class UserNameHelper
    {
        public static async Task<HtmlString> ColorUserNameAsync(this IHtmlHelper self, User user, string @class = "", string tag = "span")
        {
            var UserManager = self.ViewContext.HttpContext.RequestServices.GetService<UserManager<User>>();
            if (await UserManager.IsInRoleAsync(user, "Root") || await UserManager.IsInRoleAsync(user, "Master"))
                return new HtmlString($"<{tag} class=\"{@class} user-master\">{user.UserName}</{tag}>");
            else
                return new HtmlString($"<{tag} class=\"{@class} user-member\">{user.UserName}</{tag}>");
        }
    }
}
