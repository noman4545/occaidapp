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
	public class IOSCodeService : IIOSCodeService
	{
		private readonly IServiceScopeFactory<AppDatabaseContext> factory;

		public IOSCodeService(IServiceScopeFactory<AppDatabaseContext> factory)
		{
			this.factory = factory;
		}

		public async Task<int> DeleteIOSCode(int id)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int res = 0;
			var iOSCode = await context.IOSCodes.FindAsync(id);
			if (iOSCode != null)
			{
				iOSCode.Deleted = true;
				iOSCode.DeletedDate = DateTime.Now;
				context.IOSCodes.Update(iOSCode);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<IOSCode> GetIOSCodeDetailsByIOSNumber(string IOSNumber)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			return await context.IOSCodes.Where(w => w.Deleted == false && w.IOSNumber == IOSNumber).FirstOrDefaultAsync();
		}

		public async Task<GetIOSCodesResponse> GetIOSCodes(int page, int take, string search, bool deleted)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			if (!string.IsNullOrEmpty(search))
			{
				var searched = await context.IOSCodes.Where(w => w.Deleted == deleted && (w.IOSNumber.Contains(search) || w.Function.Contains(search)
			   || w.Description.Contains(search) || w.Level.Contains(search) || w.OccAction.Contains(search)
			   || w.TrainRescueAction.Contains(search) || w.MaintenanceAction.Contains(search) || w.Remarks.Contains(search))).OrderByDescending(o => o.CreatedDate).ToListAsync();

				return new GetIOSCodesResponse { IOSCodes = searched.Skip((page - 1) * take).Take(take).ToList(), Total = searched.Count };
			}
			var all = await context.IOSCodes.Where(w => w.Deleted == deleted).OrderByDescending(o => o.CreatedDate).ToListAsync();
			return new GetIOSCodesResponse { IOSCodes = all.Skip((page - 1) * take).Take(take).ToList(), Total = all.Count };
		}

		public async Task<int> RecoverIOSCode(int id)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int res = 0;
			var iOSCode = await context.IOSCodes.FindAsync(id);
			if (iOSCode != null)
			{
				iOSCode.Deleted = false;
				iOSCode.DeletedDate = null;
				context.IOSCodes.Update(iOSCode);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<int> SaveIOSCode(IOSCode iOSCode)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int res = 0;
			var exist = await context.IOSCodes.FirstOrDefaultAsync(f => f.IOSNumber == iOSCode.IOSNumber);
			if (exist == null)
			{
				await context.IOSCodes.AddAsync(iOSCode);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<int> UpdateIOSCode(IOSCode iOSCode)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			iOSCode.ModifyDate = DateTime.Now;
			context.IOSCodes.Update(iOSCode);
			return await context.SaveChangesAsync();
		}
	}
}
