using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OCC_Aid_App.DatabaseContext;
using OCC_Aid_App.Interfaces;
using OCC_Aid_App.Models;
using OCC_Aid_App.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OCC_Aid_App.Services
{
	public class LogService : ILogService
	{
		private readonly IServiceScopeFactory<AppDatabaseContext> factory;
		private IHttpContextAccessor userContext;
		public LogService(IServiceScopeFactory<AppDatabaseContext> factory, IHttpContextAccessor userContext)
		{
			this.factory = factory;
			this.userContext = userContext;
		}

		public async Task<List<Log>> GetLogs()
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			var userId = userContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			return await context.Logs.Where(w => w.UserId == userId).OrderByDescending(o => o.CreatedDate).ToListAsync();
		}

		public async Task<GetLogsResponse> GetSpecialLogs(int page, int take, string search, string role, string screen)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			var userId = userContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			if (!string.IsNullOrEmpty(search))
			{
				var searched = await context.Logs.Where(w => w.UserId == userId && (w.Message.Contains(search)
				|| w.ActionRole.Contains(search))).OrderByDescending(o => o.CreatedDate).ToListAsync();

				if (!string.IsNullOrEmpty(role))
				{
					searched = searched.Where(w => w.ActionRole == role).ToList();
				}

				if (!string.IsNullOrEmpty(screen))
				{
					searched = searched.Where(w => w.Screen == screen).ToList();
				}

				return new GetLogsResponse { Logs = searched.Skip((page - 1) * take).Take(take).ToList(), Total = searched.Count };
			}
			var all = await context.Logs.Where(w => w.UserId == userId).OrderByDescending(o => o.CreatedDate).ToListAsync();

			if (!string.IsNullOrEmpty(role))
			{
				all = all.Where(w => w.ActionRole == role).ToList();
			}

			if (!string.IsNullOrEmpty(screen))
			{
				all = all.Where(w => w.Screen == screen).ToList();
			}

			return new GetLogsResponse { Logs = all.Skip((page - 1) * take).Take(take).ToList(), Total = all.Count };
		}

		public Log LogMessage(Log log)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int res = 0;
			var userId = userContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			var currentUser = context.Users.Find(userId);
			var users = context.Users.Where(w => w.Id != userId).ToList();
			users.ForEach(user =>
			{
				log.Role = user.Role;
				log.ActionRole = currentUser.Role;
				log.UserId = user.Id;
				log.ActionUserId = userId;
				log.Id = 0;
				context.Logs.Add(log);
				res += context.SaveChanges();
			});
			if (res > 0)
				return new Log { Message = log.Message, CreatedDate = log.CreatedDate, Read = log.Read };
			return null;
		}

		public async Task<int> MarkAsRead()
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			var userId = userContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			var allUnreadLogs = await context.Logs.Where(w => !w.Read && w.UserId == userId).ToListAsync();
			allUnreadLogs.ForEach(f => f.Read = true);
			context.Logs.UpdateRange(allUnreadLogs);
			return await context.SaveChangesAsync();
		}
	}
}
