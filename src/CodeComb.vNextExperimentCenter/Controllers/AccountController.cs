using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using CodeComb.vNextExperimentCenter.Models;

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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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
            var url = Url.Link("default", new { controller = "Account", action = "RegisterDetail", key = aes_email });
            await Mail.SendEmailAsync(email, "vNext China 新用户注册验证信", $@"<html>
            <head></head>
            <body>
            <p><a href=""{url}"">点击继续注册</a></p>
            </body>
            </html>");
            
            return Prompt(new Prompt
            {
                Title = "请验证您的邮箱",
                Details = $"我们向您的邮箱{email}中发送了一条包含验证链接的邮件，请通过邮件打开链接继续完成注册操作",
                RedirectText = "进入邮箱",
                RedirectUrl = "http://mail." + email.Split('@')[1]
            });
        }
        
        [HttpGet]
        public IActionResult RegisterDetail(string key)
        {
            // 此时仍然需要检测一遍邮箱是否被注册
            var email = Aes.Decrypt(key);
            ViewBag.Key = key;
            ViewBag.Email = email;
            if (DB.Users.Any(x => x.Email == email))
                return Prompt(new Prompt 
                {
                    Title = "注册失败",
                    Details = $"电子邮箱{email}已经被注册，请更换后重试！",
                    StatusCode = 400
                });
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterDetail(string key, string username, string password)
        {
            // 此时仍然需要检测一遍邮箱是否被注册
            var email = Aes.Decrypt(key);
            if (DB.Users.Any(x => x.Email == email))
                return Prompt(new Prompt 
                {
                    Title = "注册失败",
                    Details = $"电子邮箱{email}已经被注册，请更换后重试！",
                    StatusCode = 400
                });
            var user = new User
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true
            };
            var result = await UserManager.CreateAsync(user, password);
            if (result.Succeeded)
                return Prompt(new Prompt
                {
                    Title = "注册成功",
                    Details = "现在您可以使用这个帐号登录vNext China了！",
                    RedirectText = "现在登录",
                    RedirectUrl = Url.Link("default", new { controller = "Account", Action = "Login" })
                });
            else return Prompt(new Prompt
            {
                Title = "注册失败",
                Details = result.Errors.First().Description,
                StatusCode = 400
            });
        }
    }
}
