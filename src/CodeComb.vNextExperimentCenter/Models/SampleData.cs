using System;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.PlatformAbstractions;

namespace CodeComb.vNextExperimentCenter.Models
{
	public static class SampleData
	{
		public static async Task InitDB(IServiceProvider services)
		{
			var DB = services.GetRequiredService<CenterContext> ();
			var UserManager = services.GetRequiredService<UserManager<User>> ();
			var RoleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>> ();
            var env = services.GetRequiredService<IApplicationEnvironment>();
			if (DB.Database.EnsureCreated())
			{
				await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Root" });
				await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Master" });
				await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Member" });
				await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Banned" });
				
				var user = new User { UserName = "root", Email = "1@1234.sh", Organization = "Code Comb Co,. Ltd.", WebSite = "http://1234.sh" };
				await UserManager.CreateAsync(user, "123456");
				await UserManager.AddToRoleAsync(user, "Root");

                var user2 = new User { UserName = "user", Email = "911574351@qq.com", Organization = "Code Comb Co,. Ltd.", WebSite = "http://1234.sh" };
                await UserManager.CreateAsync(user2, "123456");
                await UserManager.AddToRoleAsync(user, "Member");

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
                DB.Experiments.Add(new Experiment
                {
                    Title = "编写Hello World网站[备份]",
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
                    CompetitorCount = 0,
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
                    ZipUrl = "https://github.com/CodeComb/vnextcn.org/archive/dev.zip",
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
                    ZipUrl = "https://github.com/CodeComb/Extensions/archive/dev.zip",
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
                    ZipUrl = "https://github.com/CodeComb/Aes/archive/dev.zip",
                    VersionRule = "2.0.0-rc2-{0}",
                    Alias = "CodeComb.Security.Aes"
                });

                // 添加论坛
                var parentForum = new Forum
                {
                    Id = "vnext",
                    Title = "vNext交流",
                    PRI = 1
                };
                var parentForum2 = new Forum
                {
                    Id = "vnext-cn",
                    Title = "vNext China",
                    PRI = 0
                };
                var subform = new Forum
                {
                    Id = "asp-net-5",
                    Title = "ASP.Net 5",
                    ParentId = "vnext",
                    Description = "ASP.Net 5、MVC、Web Pages等技术交流",
                    PostCount = 1,
                    TopicCount = 1
                };
                var subform2 = new Forum
                {
                    Id = "dot-net-core",
                    Title = ".Net Core",
                    ParentId = "vnext",
                    Description = ".Net Core、CoreFx、CoreCLR等技术讨论"
                };
                var subform3 = new Forum
                {
                    Id = "feedback",
                    Title = "错误反馈",
                    ParentId = "vnext-cn",
                    Description = "vNext China使用时发生的错误请在本版块反馈",
                    PRI = 1
                };
                var subform4 = new Forum
                {
                    Id = "announcements",
                    Title = "公告板",
                    ParentId = "vnext-cn",
                    Description = "vNext CN 公告栏",
                    PRI = 0,
                    IsReadOnly = true
                };
                DB.Forums.Add(parentForum);
                DB.Forums.Add(parentForum2);
                DB.Forums.Add(subform);
                DB.Forums.Add(subform2);
                DB.Forums.Add(subform3);
                DB.Forums.Add(subform4);
                var topic = new Topic
                {
                    Title = "Test",
                    Content = "## My first topic\r\nHi, I'm Amamiya Yuuko",
                    CreationTime = DateTime.Now,
                    ForumId = "asp-net-5",
                    UserId = user.Id
                };
                var topic2 = new Topic
                {
                    Title = "Announce test",
                    Content = "## My first topic\r\nHi, I'm Amamiya Yuuko",
                    CreationTime = DateTime.Now,
                    ForumId = "asp-net-5",
                    UserId = user.Id,
                    IsAnnouncement = true
                };
                DB.Topics.Add(topic);
                DB.Topics.Add(topic2);
                var post = new Post
                {
                    Content = "## My first topic\r\nHi, I'm Amamiya Yuuko",
                    Time = DateTime.Now,
                    TopicId = topic.Id,
                    UserId = user.Id
                };
                DB.Posts.Add(post);

                DB.SaveChanges();
                await UserManager.AddClaimAsync(user, new System.Security.Claims.Claim("Owned CI set", ciset.Id.ToString()));
            }
        }
	}
}