using Microsoft.AspNetCore.Identity;
using OCC_Aid_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OCC_Aid_App.DatabaseContext;

namespace OCC_Aid_App.Utility
{
	public static class SeedAdmin
	{
		public static void SeedAdminUser(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, AppDatabaseContext context)
		{
			if (userManager.FindByEmailAsync("admin@occaidapp.com").Result == null)
			{
				User user = new User()
				{
					Name = "Admin",
					Email = "admin@occaidapp.com",
					UserName = "admin@occaidapp.com",
					PhoneNumber = "",
					Role = "Admin",
					SecurityStamp = Guid.NewGuid().ToString(),
					TwoFactorEnabled = true,
					Verified = true
				};

				IdentityResult result = userManager.CreateAsync(user, "Admin$@occaidapp1").Result;

				if (result.Succeeded)
				{
					if (!roleManager.RoleExistsAsync("Admin").Result)
						roleManager.CreateAsync(new IdentityRole("Admin")).Wait();

					if (roleManager.RoleExistsAsync("Admin").Result)
						userManager.AddToRoleAsync(user, "Admin").Wait();
				}
			}
		}
	}
}
