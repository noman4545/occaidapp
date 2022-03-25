using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using OCC_Aid_App.DatabaseContext;
using OCC_Aid_App.Interfaces;
using OCC_Aid_App.Models;
using OCC_Aid_App.Services;
using OCC_Aid_App.SignalR;
using OCC_Aid_App.Utilities;
using OCC_Aid_App.Utility;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson();

builder.Services.AddSignalR(e => e.MaximumReceiveMessageSize = 102400000);

builder.Services.AddSpaStaticFiles(configuration =>
{
	configuration.RootPath = "ClientApp/dist";
});

builder.Services.AddDbContext<AppDatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("default")));

builder.Services.AddIdentity<User, IdentityRole>()
	.AddEntityFrameworkStores<AppDatabaseContext>()
	.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.SaveToken = true;
	options.RequireHttpsMetadata = false;
	options.TokenValidationParameters = new TokenValidationParameters()
	{
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
		ValidateLifetime = true
	};
});

builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequiredLength = 8;
	options.Password.RequireNonAlphanumeric = false;
	options.User.RequireUniqueEmail = true;
});

builder.Services.AddSingleton<IServiceScopeFactory<AppDatabaseContext>, ServiceScopeFactory<AppDatabaseContext>>();
builder.Services.AddSingleton<IServiceScopeFactory<UserManager<User>>, ServiceScopeFactory<UserManager<User>>>();
builder.Services.AddSingleton<IServiceScopeFactory<RoleManager<IdentityRole>>, ServiceScopeFactory<RoleManager<IdentityRole>>>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IIOSCodeService, IOSCodeService>();
builder.Services.AddSingleton<IACIDService, ACIDService>();
builder.Services.AddSingleton<ILogService, LogService>();
builder.Services.AddSingleton<ITMCSService, TMCSService>();
builder.Services.AddSingleton<ISMSService, SMSService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<FileUtility>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSpaStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

using(var scope = app.Services.CreateScope())
{
	var factory = app.Services.GetRequiredService<IServiceScopeFactory>();
	using var scopeF = factory.CreateScope();
	var context = scopeF.ServiceProvider.GetRequiredService<AppDatabaseContext>();
	var userManager = scopeF.ServiceProvider.GetRequiredService<UserManager<User>>();
	var roleManager = scopeF.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	SeedAdmin.SeedAdminUser(userManager, roleManager, context);
}

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllerRoute(
		name: "default",
		pattern: "{controller}/{action}/{id?}");
	endpoints.MapHub<NotificationHub>("/NotificationHub");
	endpoints.MapHub<TMCSHub>("/TMCSHub");
});

app.UseSpa(spa =>
{
	spa.Options.SourcePath = "ClientApp";

	if (app.Environment.IsDevelopment())
	{
		spa.UseAngularCliServer(npmScript: "start");
	}
});

using (var scope = app.Services.CreateScope())
{
	var dataContext = scope.ServiceProvider.GetRequiredService<AppDatabaseContext>();
	dataContext.Database.Migrate();
}

app.Run();