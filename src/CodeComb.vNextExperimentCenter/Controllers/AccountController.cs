using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace CodeComb.vNextExperimentCenter.Controllers
{
    public class AccountController : BaseController
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, bool remember, [FromHeader] string Referer)
        {
            var result = await SignInManager.PasswordSignInAsync(username, password, remember, false);
            if (result.Succeeded)
                return Redirect(Referer ?? Url.Link("defailt", new { controller = "Home", action = "Index" }));
            else
                return Prompt(new Prompt
                {
                    Title = "登录失败",
                    Details = "请检查用户名密码是否正确后返回上一页重试！",
                    RedirectText = "忘记密码",
                    RedirectUrl = Url.Link("default", new { controller = "Home", action = "Index" }),
                    StatusCode = 403
                });
        }
        
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Register(string email, [FromHeader] string host)
        {
            // 判断该邮箱是否已经被注册
            if (DB.Users.Any(x => x.Email == email))
                return Prompt(new Prompt 
                {
                    Title = "注册失败",
                    Details = $"电子邮箱{email}已经被注册，请更换后重试！",
                    StatusCode = 400
                });
            
            // 发送激活信
            var aes_email = Aes.Encrypt(email);
            await Mail.SendEmailAsync(email, "vNext China 新用户注册验证信", $@"<html>
            <head></head>
            <body>
            <p><a href=""http://{host}/Account/RegisterDetail?key={aes_email}"">点击继续注册</a></p>
            </body>
            </html>");
            
            return Prompt(new Prompt
            {
                Title = "请验证您的邮箱",
                Details = $"我们向您的邮箱{email}中发送了一条包含验证链接的邮件，请通过邮件打开链接继续完成注册操作",
                RedirectText = "进入邮箱",
                RedirectUrl = "mail." + email.Split('@')[1]
            });
        }
    }
}
