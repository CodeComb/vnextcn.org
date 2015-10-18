using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            services.AddIdentity<User, IdentityRole<long>>()
                .AddEntityFrameworkStores<CenterContext, long>()
                .AddDefaultTokenProviders();

            services.AddMvc();
            services.AddSmartUser<User, long>();
            services.AddSmartCookies();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.MinimumLevel = LogLevel.Information;
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            app.UseIISPlatformHandler();
            app.UseStaticFiles();
            app.UseMvc(x => x.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"));
        }
    }
}
