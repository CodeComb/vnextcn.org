using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Dnx.Runtime;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Entity;
using CodeComb.vNextExperimentCenter.Models;

namespace CodeComb.vNextExperimentCenter
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            IConfiguration Configuration;
            services.AddConfiguration(out Configuration);

            services.AddEntityFramework()
                .AddDbContext<CenterContext>(x => x.UseSqlServer(Configuration["Database:ConnectionString"]))
                .AddSqlServer();

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
            services.AddSmtpEmailSender("smtp.163.com", 25, "vNext China", "codecomb@163.com", "codecomb@163.com", "CodeComb123");
            services.AddAesCrypto();
        }

        public async void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            
            app.UseIISPlatformHandler();
            app.UseExceptionHandler("/Shared/Prompt");
            app.UseStaticFiles();
            app.UseMvc(x => x.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"));
            
            await SampleData.InitDB(app.ApplicationServices);
        }
    }
}
