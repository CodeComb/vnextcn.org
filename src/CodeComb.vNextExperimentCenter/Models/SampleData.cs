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
                DB.SaveChanges();

                // 添加运行节点
                DB.Nodes.Add(new Hub.Models.Node
                {
                    Alias = "Code Comb #1",
                    PrivateKey = "4IbkO2iRm0bm0hxj2VVTUR8rsNOmCIh5",
                    Server = "localhost",
                    Port = 6070
                });
                DB.SaveChanges();

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
                    ExperimentId = 1
                });
				DB.SaveChanges();
			}
		}
	}
}