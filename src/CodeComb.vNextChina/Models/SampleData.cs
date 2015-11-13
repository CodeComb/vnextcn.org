using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Data.Entity;

namespace CodeComb.vNextChina.Models
{
	public static class SampleData
	{
		public static async Task InitDB(IServiceProvider services)
		{
			var DB = services.GetRequiredService<vNextChinaContext> ();
			var UserManager = services.GetRequiredService<UserManager<User>> ();
			var RoleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>> ();
            var env = services.GetRequiredService<IApplicationEnvironment>();
            
            if (DB.Database.EnsureCreated())
			{
                await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Root" });
				await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Master" });
				await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Member" });
				await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Banned" });
				
				var user = new User
                {
                    UserName = "雨宫优子",
                    Email = "1@1234.sh",
                    Organization = "Code Comb Co., Ltd.",
                    WebSite = "http://1234.sh",
                    RegisteryTime = DateTime.Now
                };
				await UserManager.CreateAsync(user, "123456");
				await UserManager.AddToRoleAsync(user, "Root");

                // 添加Hello World实验
                DB.Experiments.Add(new Experiment
                {
					Title = "编写Hello World网站",
					OS = OSType.CrossPlatform,
					Version = "beta8",
					Description = "编写一个vNext网站程序，对于匹配到的任何路由均输出`Hello World!`",
					TimeLimit = 10000,
					CheckPassed = true,
					Difficulty = 0,
					Namespace = "HelloWorld",
                    NuGet = "https://www.myget.org/F/codecomb-beta8/api/v3/index.json",
                    TestArchive = File.ReadAllBytes(env.ApplicationBasePath + "/Setup/HelloWorld.zip"),
                    AnswerArchive = File.ReadAllBytes(env.ApplicationBasePath + "/Setup/HelloWorldAnswer.zip")
                });

                // 添加运行节点
                DB.Nodes.Add(new Hub.Models.Node
                {
                    Alias = "Code Comb #1",
                    PrivateKey = "4IbkO2iRm0bm0hxj2VVTUR8rsNOmCIh5",
                    Server = "localhost",
                    Port = 6070
                });

                // 添加比赛
                DB.Contests.Add(new Contest
                {
                    Id = "vnext-china-test-round-1",
                    Title = "vNext China Test Round #1",
                    Begin = DateTime.Now,
                    End = DateTime.Now.AddHours(3),
                    Description = "欢迎来到vNext China"
                });
                DB.ContestExperiments.Add(new ContestExperiment
                {
                    ContestId = "vnext-china-test-round-1",
                    ExperimentId = 1,
                    Point = 500
                });

                // 添加项目
                var ciset = new CISet
                {
                    Title = "Code Comb vNext Libraries",
                    CreationTime = DateTime.Now,
                    LastBuildingTime = null
                };
                DB.CISets.Add(ciset);
                DB.Projects.Add(new Project
                {
                    CurrentVersion = 10000,
                    AdditionalEnvironmentVariables = "{ }",
                    PRI = 0,
                    RunWithLinux = true,
                    RunWithOsx = true,
                    RunWithWindows = true,
                    CISetId = ciset.Id,
                    RestoreMethod = ProjectRestoreMethod.Git,
                    Url = "https://github.com/CodeComb/vnextcn.org.git",
                    VersionRule = "2.0.0-rc2-{0}",
                    Alias = "vnextcn.org"
                });
                DB.Projects.Add(new Project
                {
                    CurrentVersion = 10000,
                    AdditionalEnvironmentVariables = "{ }",
                    PRI = 0,
                    RunWithLinux = true,
                    RunWithOsx = true,
                    RunWithWindows = true,
                    CISetId = ciset.Id,
                    RestoreMethod = ProjectRestoreMethod.Git,
                    Url = "https://github.com/CodeComb/Extensions.git",
                    VersionRule = "2.0.0-rc2-{0}",
                    Alias = "CodeComb.AspNet.Extensions"
                });
                DB.Projects.Add(new Project
                {
                    CurrentVersion = 10000,
                    AdditionalEnvironmentVariables = "{ }",
                    PRI = 0,
                    RunWithLinux = true,
                    RunWithOsx = true,
                    RunWithWindows = true,
                    CISetId = ciset.Id,
                    RestoreMethod = ProjectRestoreMethod.Git,
                    Url = "https://github.com/CodeComb/CodeComb.Security.Aes.git",
                    VersionRule = "2.0.0-rc2-{0}",
                    Alias = "CodeComb.Security.Aes"
                });

                // 添加论坛
                var parentForum = new Forum
                {
                    Id = "vnext",
                    Title = "vNext技术交流",
                    PRI = 1
                };
                var parentForum2 = new Forum
                {
                    Id = "vnext-cn",
                    Title = "vNext China",
                    PRI = 0
                };
                var subforum = new Forum
                {
                    Id = "asp-net-5",
                    Title = "ASP.Net 5",
                    ParentId = "vnext",
                    Description = "ASP.Net 5、MVC、Web Pages等技术交流",
                    PRI = 1
                };
                var subforum2 = new Forum
                {
                    Id = "dot-net-core",
                    Title = ".Net Core",
                    ParentId = "vnext",
                    Description = ".Net Core、CoreFx、CoreCLR等技术讨论",
                    PRI = 0
                };
                var subforum3 = new Forum
                {
                    Id = "cross-plat",
                    Title = "跨平台开发",
                    ParentId = "vnext",
                    Description = "跨平台开发技术交流",
                    PRI = 2
                };
                var subforum4 = new Forum
                {
                    Id = "mvc-6",
                    Title = "MVC 6",
                    ParentId = "vnext",
                    Description = "MVC 6开发技术交流",
                    PRI = 3
                };
                var subforum5 = new Forum
                {
                    Id = "ef-7",
                    Title = "Entity Framework 7",
                    ParentId = "vnext",
                    Description = "Entity Framework 7 ORM 技术交流",
                    PRI = 4
                };
                var subforum6 = new Forum
                {
                    Id = "frameworks",
                    Title = "其他框架交流",
                    ParentId = "vnext",
                    Description = "其他框架交流专区",
                    PRI = 5
                };
                var subforum7 = new Forum
                {
                    Id = "feedback",
                    Title = "BUG反馈",
                    ParentId = "vnext-cn",
                    Description = "vNext China使用时发生的错误请在本版块反馈",
                    PRI = 1
                };
                var subforum8 = new Forum
                {
                    Id = "announcements",
                    Title = "公告板",
                    ParentId = "vnext-cn",
                    Description = "vNext CN 公告栏",
                    PRI = 0,
                    IsReadOnly = true
                };
                var subforum9 = new Forum
                {
                    Id = "suggestions",
                    Title = "发展建议",
                    ParentId = "vnext-cn",
                    Description = "在该板块提出您对vNext China的宝贵意见",
                    PRI = 2
                };
                DB.Forums.Add(parentForum);
                DB.Forums.Add(parentForum2);
                DB.SaveChanges();
                DB.Forums.Add(subforum);
                DB.Forums.Add(subforum2);
                DB.Forums.Add(subforum3);
                DB.Forums.Add(subforum4);
                DB.Forums.Add(subforum5);
                DB.Forums.Add(subforum6);
                DB.Forums.Add(subforum7);
                DB.Forums.Add(subforum8);
                DB.Forums.Add(subforum9);
                DB.SaveChanges();
                await UserManager.AddClaimAsync(user, new System.Security.Claims.Claim("Owned CI set", ciset.Id.ToString()));
            }
        }
	}
}