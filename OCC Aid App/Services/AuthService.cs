using OCC_Aid_App.DatabaseContext;
using OCC_Aid_App.Interfaces;
using OCC_Aid_App.Models;
using OCC_Aid_App.Models.APIModels;
using OCC_Aid_App.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OCC_Aid_App.Services
{
	public class AuthService : IAuthService
	{
        private readonly IServiceScopeFactory<AppDatabaseContext> factory;
        private IHttpContextAccessor userContext;
        private readonly IServiceScopeFactory<UserManager<User>> userFactory;
        private readonly IServiceScopeFactory<RoleManager<IdentityRole>> roleFactory;
        private readonly IConfiguration Configuration;
        public AuthService(
            IServiceScopeFactory<AppDatabaseContext> factory,
            IHttpContextAccessor userContext,
            IServiceScopeFactory<UserManager<User>> userFactory,
            IServiceScopeFactory<RoleManager<IdentityRole>> roleFactory, 
            IConfiguration Configuration) 
		{
            this.factory = factory;
            this.userContext = userContext;
            this.userFactory = userFactory;
            this.roleFactory = roleFactory;
            this.Configuration = Configuration;
        }

		public async Task<ReturnResponse> Login(LoginModel userLogin)
		{
            try
            {
                using var userScope = userFactory.CreateScope();
                var userManager = userScope.GetRequiredService();

                var User = await userManager.FindByEmailAsync(userLogin.Email);
                if (User != null && await userManager.CheckPasswordAsync(User, userLogin.Password))
                {
                    var userRole = await userManager.GetRolesAsync(User);
                    var role = userRole.FirstOrDefault();
                    if (User.Verified)
                    {
                        User.LastAccessTime = DateTime.Now;
                        await userManager.UpdateAsync(User);
                        return new ReturnResponse { Status = StatusCodes.Status200OK, Message = await GenerateJWTToken(User.Id), Errors = "" };
					}
                    else
                    {
                        await userManager.DeleteAsync(User);
                        return new ReturnResponse { Status = StatusCodes.Status401Unauthorized, Message = "Invalid email or password.", Errors = "" };
                    }
                }
                return new ReturnResponse { Status = StatusCodes.Status401Unauthorized, Message = "Invalid email or password.", Errors = "" };
            }
            catch (Exception ex)
            {
                return new ReturnResponse { Status = StatusCodes.Status500InternalServerError, Message = "Something went wrong, try again.", Errors = ex };
            }
        }

        public async Task<ReturnResponse> Register(User userRegister)
        {
            try
            {
				using var userScope = userFactory.CreateScope();
				var userManager = userScope.GetRequiredService();

                using var roleScope = roleFactory.CreateScope();
                var roleManager = roleScope.GetRequiredService();

                var userExists = await userManager.FindByEmailAsync(userRegister.Email);
                if (userExists != null && userExists.Verified)
                {
                    return new ReturnResponse { Status = StatusCodes.Status500InternalServerError, Message = "Email already exists", Errors = "" };
                }

                User User = new User()
                {
                    Name = userRegister.Name,
                    Email = userRegister.Email,
                    UserName = userRegister.Email,
                    PhoneNumber = userRegister.PhoneNumber,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    Role = userRegister.Role,
                    TwoFactorEnabled = true,
                    Verified = true,
                    LastAccessTime = DateTime.Now
                };

                var result = await userManager.CreateAsync(User, userRegister.PasswordHash);

                if (!result.Succeeded)
                    return new ReturnResponse { Status = StatusCodes.Status500InternalServerError, Message = "User Creation Failed! Please check user details and try again.", Errors = result.Errors };

                if (!await roleManager.RoleExistsAsync(userRegister.Role))
                    await roleManager.CreateAsync(new IdentityRole(userRegister.Role));

                if (await roleManager.RoleExistsAsync(userRegister.Role))
                    await userManager.AddToRoleAsync(User, userRegister.Role);

                return new ReturnResponse { Status = StatusCodes.Status200OK, Message = true, Errors = result.Errors };
			}
            catch (Exception ex)
            {
                return new ReturnResponse { Status = StatusCodes.Status500InternalServerError, Message = "Something went wrong, try again.", Errors = ex };
            }
        }

        public async Task<ReturnResponse> GetAllUsers()
        {
            using var scope = factory.CreateScope();
            var context = scope.GetRequiredService();

            string selfUserId = userContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var AllUsers = await context.Users.Where(w => w.Verified && w.Email.ToLower() != "admin@occaidapp.com" && w.Id != selfUserId).ToListAsync();
            
            return new ReturnResponse
            {
                Status = StatusCodes.Status200OK,
                Message = AllUsers.Select(sel => new
                {
                    UserId = sel.Id,
                    Name = sel.Name,
                    Email = sel.Email,
                    Role = sel.Role,
                }),
                Errors = ""
            };
        }

        public async Task<ReturnResponse> DeleteUsers(string userid)
        {
            using var scope = factory.CreateScope();
            var context = scope.GetRequiredService();
            var user = await context.Users.FindAsync(userid);
            if (user != null)
            {
                context.Users.Remove(user);
                int res = await context.SaveChangesAsync();
                if (res > 0)
                    return new ReturnResponse { Status = StatusCodes.Status200OK, Message = true, Errors = "" };
            }
            return new ReturnResponse { Status = StatusCodes.Status404NotFound, Message = "", Errors = "" }; ;
        }

		public async Task<ReturnResponse> SetNewPassword(NewPasswordModel newPassword)
		{
			using var userScope = userFactory.CreateScope();
			var userManager = userScope.GetRequiredService();

			var User = await userManager.FindByIdAsync(newPassword.UserId);
			User.PasswordHash = userManager.PasswordHasher.HashPassword(User, newPassword.password);
			var result = await userManager.UpdateAsync(User);
			if (result.Succeeded)
				return new ReturnResponse { Status = StatusCodes.Status200OK, Message = true, Errors = "" };
			else
				return new ReturnResponse { Status = StatusCodes.Status500InternalServerError, Message = "Something went wrong, unable to change password.", Errors = "" };
		}

		public async Task<ReturnResponse> VerifyLoginToken()
        {
            using var scope = factory.CreateScope();
            var context = scope.GetRequiredService();

            var userExists = await context.Users.FirstOrDefaultAsync(f => f.Id == userContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) && f.Verified == true);
            if (userExists != null && DateTime.Now - userExists.LastAccessTime <= TimeSpan.FromMinutes(30))
            {
                userExists.LastAccessTime = DateTime.Now;
                context.Users.Update(userExists);
                return new ReturnResponse { Status = StatusCodes.Status200OK, Message = true, Errors = "" };
            }
            return new ReturnResponse { Status = StatusCodes.Status401Unauthorized, Message = "", Errors = "" };
        }

        public async Task<ReturnResponse> VerifyAdminToken()
        {
            using var scope = factory.CreateScope();
            var context = scope.GetRequiredService();

            var userExists = await context.Users.FirstOrDefaultAsync(f => f.Id == userContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) && f.Verified == true);
            if (userExists != null)
                return new ReturnResponse { Status = StatusCodes.Status200OK, Message = true, Errors = "" };
            return new ReturnResponse { Status = StatusCodes.Status401Unauthorized, Message = "", Errors = "" };
        }

        private async Task<dynamic> GenerateJWTToken(string userId)
        {
            using var userScope = userFactory.CreateScope();
            var userManager = userScope.GetRequiredService();

            var User = await userManager.FindByIdAsync(userId);
            var userRole = await userManager.GetRolesAsync(User);
            var role = userRole.FirstOrDefault();
            DateTime expiryDate = DateTime.Now.AddDays(1);
            var authClaims = new List<Claim>
                            {
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                //new Claim(ClaimTypes.MobilePhone, User.PhoneNumber),
                                new Claim(ClaimTypes.Email, User.Email),
                                new Claim(ClaimTypes.NameIdentifier, User.Id),
                                new Claim(ClaimTypes.Role, role),
                                new Claim(ClaimTypes.Expiration, expiryDate.ToString()),
                            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                expires: expiryDate,
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                UserName = User.Name,
                Email = User.Email,
                UserId = User.Id,
                Role = role,
            };
        }
    }
}
