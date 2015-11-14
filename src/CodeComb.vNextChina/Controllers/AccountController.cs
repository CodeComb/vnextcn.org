using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Net;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Authorization;
using CodeComb.vNextChina.Models;

namespace CodeComb.vNextChina.Controllers
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
                return Redirect(Referer ?? Url.Action("Index", "Home"));
            else
                return Prompt(x =>
                {
                    x.Title = "登录失败";
                    x.Details = "请检查用户名密码是否正确后返回上一页重试！";
                    x.RedirectText = "忘记密码";
                    x.RedirectUrl = Url.Action("Index", "Home");
                    x.StatusCode = 403;
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
                return Prompt(x =>
                {
                    x.Title = "注册失败";
                    x.Details = $"电子邮箱{email}已经被注册，请更换后重试！";
                    x.StatusCode = 400;
                });

            // 发送激活信
            var aes_email = Aes.Encrypt(email);
            //var url = Url.Link("default", new { action = "RegisterDetail", controller = "Account", key = aes_email });
            var url = $"http://vnextcn.org/Account/RegisterDetail?key={WebUtility.UrlEncode(aes_email)}";
            await Mail.SendEmailAsync(email, "vNext China 新用户注册验证信", $@"<html>
            <head></head>
            <body>
            <p><a href=""{url}"">点击继续注册</a></p>
            </body>
            </html>");

            return Prompt(x =>
            {
                x.Title = "请验证您的邮箱";
                x.Details = $"我们向您的邮箱{email}中发送了一条包含验证链接的邮件，请通过邮件打开链接继续完成注册操作";
                x.RedirectText = "进入邮箱";
                x.RedirectUrl = "http://mail." + email.Split('@')[1];
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
                return Prompt(x =>
                {
                    x.Title = "注册失败";
                    x.Details = $"电子邮箱{email}已经被注册，请更换后重试！";
                    x.StatusCode = 400;
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
                return Prompt(x =>
                {
                    x.Title = "注册失败";
                    x.Details = $"电子邮箱{email}已经被注册，请更换后重试！";
                    x.StatusCode = 400;
                });
            var user = new User
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true,
                RegisteryTime = DateTime.Now
            };
            var result = await UserManager.CreateAsync(user, password);
            await UserManager.AddToRoleAsync(user, "Member");
            if (result.Succeeded)
                return Prompt(x =>
                {
                    x.Title = "注册成功";
                    x.Details = "现在您可以使用这个帐号登录vNext China了！";
                    x.RedirectText = "现在登录";
                    x.RedirectUrl = Url.Action("Login", "Account");
                });
            else
                return Prompt(x =>
                {
                    x.Title = "注册失败";
                    x.Details = result.Errors.First().Description;
                    x.StatusCode = 400;
                });
        }

        [HttpGet]
        public IActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Forgot(string email)
        {
            // 判断该邮箱是否已经存在 不存在不能密码重置
            if (!DB.Users.Any(x => x.Email == email))
                return Prompt(x =>
                {
                    x.Title = "找回密码失败";
                    x.Details = $"电子邮箱{email}不存在，请更换后重试！";
                    x.StatusCode = 400;
                });

            // 发送激活信
            var aes_email = Aes.Encrypt(email);
            //var url = Url.Link("default", new { action = "ForgotDetail", controller = "Account", key = aes_email });
            var url = $"http://vnextcn.org/Account/ForgotDetail?key={WebUtility.UrlEncode(aes_email)}";
            await Mail.SendEmailAsync(email, "vNext China  密码找回验证信", $@"<html>
            <head></head>
            <body>
            <p><a href=""{url}"">点击继续完成找回密码</a></p>
            </body>
            </html>");

            return Prompt(x =>
            {
                x.Title = "请验证您的邮箱";
                x.Details = $"我们向您的邮箱{email}中发送了一条包含验证链接的邮件，请通过邮件打开链接继续完成密码找回操作";
                x.RedirectText = "进入邮箱";
                x.RedirectUrl = "http://mail." + email.Split('@')[1];
            });
        }

        [HttpGet]
        public IActionResult ForgotDetail(string key)
        {
            // 判断该邮箱是否已经存在 不存在不能密码重置
            var email = Aes.Decrypt(key);
            ViewBag.Key = key;
            ViewBag.Email = email;
            if (!DB.Users.Any(x => x.Email == email))
                return Prompt(x =>
                {
                    x.Title = "找回密码失败";
                    x.Details = $"电子邮箱{email}不存在，请更换后重试！";
                    x.StatusCode = 400;
                });
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotDetail(string key, string password, string confirm)
        {
            if (password != confirm)
            {
                return Prompt(x =>
                {
                    x.Title = "找回密码失败";
                    x.Details = $"两次输入密码不一致！";
                    x.StatusCode = 400;
                });
            }
            // 判断该邮箱是否已经存在 不存在不能密码重置
            var email = Aes.Decrypt(key);
            if (!DB.Users.Any(x => x.Email == email))
                return Prompt(x =>
                {
                    x.Title = "密码重置失败";
                    x.Details = $"电子邮箱{email}不存在，请更换后重试！";
                    x.StatusCode = 400;
                });
            var user = await UserManager.FindByEmailAsync(email);
            string resetToken = await UserManager.GeneratePasswordResetTokenAsync(user);
            var result = await UserManager.ResetPasswordAsync(user, resetToken, password);
            if (result.Succeeded)
                return Prompt(x =>
                {
                    x.Title = "密码重置成功";
                    x.Details = "现在您可以使用这个帐号登录vNext China了！";
                    x.RedirectText = "现在登录";
                    x.RedirectUrl = Url.Action("Login", "Account");
                });
            else
                return Prompt(x =>
                {
                    x.Title = "注册失败";
                    x.Details = result.Errors.First().Description;
                    x.StatusCode = 400;
                });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await SignInManager.SignOutAsync();
            return Prompt(x =>
            {
                x.Title = "您已注销";
                x.Details = "您已成功注销了登录状态。";
                x.RedirectText = "重新登录";
                x.RedirectUrl = Url.Action("Login", "Account");
                x.HideBack = true;
            });
        }

        [Route("Account/{id:long}")]
        public IActionResult Show(long id)
        {
            var user = DB.Users.Where(x => x.Id == id).SingleOrDefault();
            if (user == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });

            return View(user);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(long id)
        {
            var user = DB.Users.Where(x => x.Id == id).SingleOrDefault();
            if (user == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            var roles = await UserManager.GetRolesAsync(user);
            if (roles.Contains("Root") && !User.IsInRole("Root"))
                return Prompt(x =>
                {
                    x.Title = "没有权限";
                    x.Details = "您的权限不足以编辑该用户，请使用更高权限帐号执行本操作。";
                    x.StatusCode = 403;
                });
            if (User.Current.Id != id && !User.AnyRoles("Root, Master"))
                return Prompt(x =>
                {
                    x.Title = "没有权限";
                    x.Details = "您的权限不足以编辑该用户，请使用更高权限帐号执行本操作。";
                    x.StatusCode = 403;
                });
            return View(user);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, IFormFile avatar, User Model)
        {
            var user = DB.Users.Where(x => x.Id == id).SingleOrDefault();
            if (user == null)
                return Prompt(x =>
                {
                    x.Title = "资源没有找到";
                    x.Details = "您请求的资源没有找到，请返回重试！";
                    x.StatusCode = 404;
                });
            var roles = await UserManager.GetRolesAsync(user);
            if (roles.Contains("Root") && !User.IsInRole("Root"))
                return Prompt(x =>
                {
                    x.Title = "没有权限";
                    x.Details = "您的权限不足以编辑该用户，请使用更高权限帐号执行本操作。";
                    x.StatusCode = 403;
                });
            if (User.Current.Id != id && !User.AnyRoles("Root, Master"))
                return Prompt(x =>
                {
                    x.Title = "没有权限";
                    x.Details = "您的权限不足以编辑该用户，请使用更高权限帐号执行本操作。";
                    x.StatusCode = 403;
                });
            if (avatar != null)
            {
                user.Avatar = await avatar.ReadAllBytesAsync();
                user.AvatarContentType = avatar.ContentType;
            }
            user.Motto = Model.Motto;
            user.WebSite = Model.WebSite;
            user.Organization = Model.Organization;
            DB.SaveChanges();
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = "用户资料已经保存成功！";
            });
        }

        [HttpGet]
        public IActionResult Avatar(long id)
        {
            var user = DB.Users.Where(x => x.Id == id).SingleOrDefault();
            if (user == null || user.Avatar == null)
            {
                return File(System.IO.File.ReadAllBytes(WebRoot + "/images/NoAvatar.png"), "image/x-png");
            }
            else
            {
                return File(user.Avatar, user.AvatarContentType);
            }
        }

        [HttpGet]
        [AnyRoles("Root, Master")]
        public async Task<IActionResult> Role(long id)
        {
            var user = await UserManager.FindByIdAsync(id.ToString());
            if (User.AnyRoles("Master") && await UserManager.IsInAnyRolesAsync(user, "Root, Master"))
            {
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您无权编辑这个用户的角色！";
                });
            }
            else
            {
                ViewBag.Roles = DB.Roles
                    .Select(x => x.Name)
                    .ToList();
                ViewBag.CurrentRole = (await UserManager.GetRolesAsync(user)).First();
                return View(user);
            }
        }

        [HttpPost]
        [AnyRoles("Root, Master")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Role(long id, string Role)
        {
            var user = await UserManager.FindByIdAsync(id.ToString());
            if (User.AnyRoles("Master") && await UserManager.IsInAnyRolesAsync(user, "Root, Master"))
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您无权编辑这个用户的角色！";
                    x.StatusCode = 500;
                });
            await UserManager.RemoveFromRolesAsync(user, await UserManager.GetRolesAsync(user));
            await UserManager.AddToRoleAsync(user, Role);
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = $"用户{user.UserName}已经成为了{Role}！";
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Password(long id)
        {
            var user = await UserManager.FindByIdAsync(id.ToString());
            if (User.AnyRoles("Master") && await UserManager.IsInAnyRolesAsync(user, "Root, Master") && User.Current.Id != user.Id)
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您无权修改这个用户的密码！";
                    x.StatusCode = 500;
                });
            return View(user);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Password(long id, string old, string password, string confirm)
        {
            if (confirm != password)
                return Prompt(x =>
                {
                    x.Title = "修改失败";
                    x.Details = "两次密码输入不一致，请返回重试！";
                    x.StatusCode = 400;
                });
            var user = await UserManager.FindByIdAsync(id.ToString());
            if (User.AnyRoles("Master") && await UserManager.IsInAnyRolesAsync(user, "Root, Master") && User.Current.Id != user.Id)
                return Prompt(x =>
                {
                    x.Title = "权限不足";
                    x.Details = "您无权修改这个用户的密码！";
                    x.StatusCode = 500;
                });
            if (!User.AnyRoles("Root, Master") && !await UserManager.CheckPasswordAsync(user, old))
                return Prompt(x =>
                {
                    x.Title = "修改失败";
                    x.Details = "旧密码不正确，请返回重试！";
                    x.StatusCode = 400;
                });
            var token = await UserManager.GeneratePasswordResetTokenAsync(user);
            await UserManager.ResetPasswordAsync(user, token, password);
            return Prompt(x =>
            {
                x.Title = "修改成功";
                x.Details = "新密码已生效！";
            });
        }
    }
}
