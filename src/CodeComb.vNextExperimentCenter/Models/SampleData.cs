using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CodeComb.vNextExperimentCenter.Models
{
	public static class SampleData
	{
		public static async Task InitDB(IServiceProvider services)
		{
			var DB = services.GetRequiredService<CenterContext> ();
			var UserManager = services.GetRequiredService<UserManager<User>> ();
			var RoleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>> ();
			if (DB.Database.EnsureCreated())
			{
				await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Root" });
				await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Master" });
				await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Member" });
				await RoleManager.CreateAsync(new IdentityRole<long> { Name = "Banned" });
				
				var user = new User { UserName = "root", Email = "1@1234.sh" };
				await UserManager.CreateAsync(user, "123456");
				await UserManager.AddToRoleAsync(user, "Root");
				
				DB.Problems.Add(new Problem
				{
					Title = "编写Hello World网站",
					OS = OSType.CrossPlatform,
					Version = "beta8",
					Description = "编写一个vNext网站程序，仅输出Hello World",
					TimeLimit = 10000,
					CheckPassed = true,
					Difficulty = 0,
					Namespace = "HelloWorld"
				});
				
				DB.SaveChanges();
			}
		}
	}
}