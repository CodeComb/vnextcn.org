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
using CodeComb.vNextChina.Hub;

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

            if (Configuration["Database:Mode"] == "SQLite")
            {
                services.AddEntityFramework()
                    .AddDbContext<CenterContext>(x => x.UseSqlite(Configuration["Database:ConnectionString"].Replace("{appRoot}", appRoot)))
                    .AddSqlite();
            }
            else if (Configuration["Database:Mode"] == "SqlServer")
            {
                services.AddEntityFramework()
                    .AddDbContext<CenterContext>(x => x.UseSqlServer(Configuration["Database:ConnectionString"]))
                    .AddSqlServer();
            }
            else
            {
                services.AddEntityFramework()
                    .AddDbContext<CenterContext>(x => x.UseInMemoryDatabase())
                    .AddSqlite();
            }
            
            services.AddIdentity<User, IdentityRole<long>>(x => 
            {
                x.Password.RequireDigit = false;
                x.Password.RequiredLength = 0;
                x.Password.RequireLowercase = false;
                x.Password.RequireNonLetterOrDigit = false;
                x.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<CenterContext, long>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddSmartUser<User, long>();
            services.AddSmartCookies();
            services.AddAntiXss();
            services.AddSmtpEmailSender("smtp.qq.com", 25, "vNext China", "911574351@qq.com", "911574351", "XXX");
            services.AddAesCrypto();
            services.AddEFNodeProvider<CenterContext>();
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
            app.UseMvc(x => x.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"));
            
            await SampleData.InitDB(app.ApplicationServices);
        }
    }
}
