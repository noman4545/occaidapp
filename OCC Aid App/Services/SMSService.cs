using Microsoft.EntityFrameworkCore;
using OCC_Aid_App.DatabaseContext;
using OCC_Aid_App.Interfaces;
using OCC_Aid_App.Models;
using OCC_Aid_App.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Services
{
	public class SMSService : ISMSService
	{
		private readonly IServiceScopeFactory<AppDatabaseContext> factory;

		public SMSService(IServiceScopeFactory<AppDatabaseContext> factory)
		{
			this.factory = factory;
		}
		public async Task<int> DeleteSMS(int id)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int res = 0;
			var sms = await context.SMSs.FindAsync(id);
			if (sms != null)
			{
				sms.Deleted = true;
				sms.DeletedDate = DateTime.Now;
				context.SMSs.Update(sms);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<GetSMSResponse> GetSMSs(int page, int take, string search, bool deleted)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			if (!string.IsNullOrEmpty(search))
			{
				var searched = await context.SMSs.Where(w => w.Deleted == deleted && (w.TypeOfFailure.Contains(search) || w.SystemBehaviour.Contains(search)
			   || w.WorkInstruction.Contains(search) || w.Message.Contains(search) || w.TimeToReturnToTimetable.Contains(search)))
				.OrderByDescending(o => o.CreatedDate).ToListAsync();

				return new GetSMSResponse { SMSs = searched.Skip((page - 1) * take).Take(take).ToList(), Total = searched.Count };
			}
			var all = await context.SMSs.Where(w => w.Deleted == deleted).OrderByDescending(o => o.CreatedDate).ToListAsync();
			return new GetSMSResponse { SMSs = all.Skip((page - 1) * take).Take(take).ToList(), Total = all.Count };
		}

		public async Task<List<SMS>> GetAllSMS()
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			return await context.SMSs.Where(w => w.Deleted == false).OrderByDescending(o => o.CreatedDate).ToListAsync();
		}

		public async Task<int> SaveSMS(SMS sms)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int res = 0;
			var exist = await context.SMSs.FirstOrDefaultAsync(f => f.TypeOfFailure == sms.TypeOfFailure);
			if (exist == null)
			{
				await context.SMSs.AddAsync(sms);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<int> UpdateSMS(SMS sms)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			sms.ModifyDate = DateTime.Now;
			context.SMSs.Update(sms);
			return await context.SaveChangesAsync();
		}

		public async Task<int> RecoverSMS(int id)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int res = 0;
			var sms = await context.SMSs.FindAsync(id);
			if (sms != null)
			{
				sms.Deleted = false;
				sms.DeletedDate = null;
				context.SMSs.Update(sms);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<int> SaveArchieveSMS(ArchievedSMS sms)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			sms.Id = 0;
			await context.ArchievedSMSs.AddAsync(sms);
			return await context.SaveChangesAsync();
		}

		public async Task<int> MarkArchieveSMSComplete(int id)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			var archieveSMS = await context.ArchievedSMSs.FindAsync(id);
			archieveSMS.Completed = true;
			context.Update(archieveSMS);
			return await context.SaveChangesAsync();
		}

		public async Task<List<ArchievedSMS>> GetArchieveSMS()
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			return await context.ArchievedSMSs.Where(w => w.Completed == false).ToListAsync();
		}
	}
}
