using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Entity;
using CodeComb.vNextChina.Models;
using CodeComb.vNextChina.Hubs;

namespace CodeComb.vNextChina
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var _serv = services.BuildServiceProvider();
            var appRoot = _serv.GetRequiredService<IApplicationEnvironment>().ApplicationBasePath;
            
            IConfiguration Configuration;
            services.AddConfiguration(out Configuration);

            if (Configuration["Data:DefaultConnection:Mode"] == "SQLite")
            {
                services.AddEntityFramework()
                    .AddDbContext<vNextChinaContext>(x => x.UseSqlite(Configuration["Data:DefaultConnection:ConnectionString"].Replace("{appRoot}", appRoot)))
                    .AddSqlite();
            }
            else if (Configuration["Data:DefaultConnection:Mode"] == "SqlServer")
            {
                services.AddEntityFramework()
                    .AddDbContext<vNextChinaContext>(x => x.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]))
                    .AddSqlServer();
            }
            else
            {
                services.AddEntityFramework()
                    .AddDbContext<vNextChinaContext>(x => x.UseInMemoryDatabase())
                    .AddInMemoryDatabase();
            }
            
            services.AddIdentity<User, IdentityRole<long>>(x => 
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonLetterOrDigit = false;
                x.Password.RequireUppercase = false;
                x.User.AllowedUserNameCharacters = null;
            })
                .AddEntityFrameworkStores<vNextChinaContext, long>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddSmartUser<User, long>();
            services.AddSmartCookies();
            services.AddAntiXss();
            services.AddSmtpEmailSender("smtp.ym.163.com", 25, "vNext China", "noreply@vnextcn.org", "noreply@vnextcn.org", "123456");
            services.AddAesCrypto();
            services.AddEFNodeProvider<vNextChinaContext>();
            services.AddSignalR();
        }

        public async void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Warning;
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            app.UseAutoAjax();
            app.UseIdentity();
            app.UseExceptionHandler("/Shared/Prompt");
            app.UseStaticFiles();
            app.UseSignalR();
            app.UseMvc(x => x.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"));

            await SampleData.InitDB(app.ApplicationServices);
        }
    }
}
