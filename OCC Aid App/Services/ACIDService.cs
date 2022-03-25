using Microsoft.EntityFrameworkCore;
using OCC_Aid_App.DatabaseContext;
using OCC_Aid_App.Interfaces;
using OCC_Aid_App.Models;
using OCC_Aid_App.Models.ViewModels;
using OCC_Aid_App.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Services
{
	public class ACIDService : IACIDService
	{
		private readonly IServiceScopeFactory<AppDatabaseContext> factory;
		private FileUtility utility;
		public ACIDService(IServiceScopeFactory<AppDatabaseContext> factory, FileUtility utility)
		{
			this.factory = factory;
			this.utility = utility;
		}

		public async Task<int> DeleteAcid(int id)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int res = 0;
			var acid = await context.ACIDs.FindAsync(id);
			if (acid != null)
			{
				acid.Deleted = true;
				acid.DeletedDate = DateTime.Now;
				context.ACIDs.Update(acid);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<ACID> GetACIDDetailsByATSNumber(string atsname)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			return await context.ACIDs.Where(w => w.AcidNameInAts == atsname).FirstOrDefaultAsync();
		}

		public async Task<GetACIDResponse> GetAcids(int page, int take, string search, bool deleted)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			if (!string.IsNullOrEmpty(search))
			{
				var searched = await context.ACIDs.Where(w => w.Deleted == deleted && (w.Territory.Contains(search) || w.AcidNameInAts.Contains(search)
				|| w.AcidNameInIsm.Contains(search) || w.PedEepEesName.Contains(search) || w.TrackNo.Contains(search) || w.Cctv == search)).OrderByDescending(o => o.CreatedDate).ToListAsync();

				return new GetACIDResponse { Acids = searched.Skip((page - 1) * take).Take(take).ToList(), Total = searched.Count };
			}
			var all = await context.ACIDs.Where(w => w.Deleted == deleted).OrderByDescending(o => o.CreatedDate).ToListAsync();
			return new GetACIDResponse { Acids = all.Skip((page - 1) * take).Take(take).ToList(), Total = all.Count };
		}

		public async Task<List<string>> GetAcidsByTerritory(string territory)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			return await context.ACIDs.Where(w => w.Deleted == false).Where(w => w.Territory == territory).Select(s => s.AcidNameInAts).ToListAsync();
		}

		public async Task<List<string>> GetTerritories()
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			return await context.ACIDs.Where(w => w.Deleted == false).Select(s => s.Territory).Distinct().ToListAsync();
		}

		public async Task<int> RecoverAcid(int id)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int res = 0;
			var acid = await context.ACIDs.FindAsync(id);
			if (acid != null)
			{
				acid.Deleted = false;
				acid.DeletedDate = null;
				context.ACIDs.Update(acid);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<int> SaveAcid(ACID acid)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int res = 0;
			var exist = await context.ACIDs.FirstOrDefaultAsync(f => f.AcidNameInAts == acid.AcidNameInAts.Replace("/", ""));
			if (exist == null)
			{
				acid.Layout = await utility.UploadFile(acid.Layout, "ACID_" + acid.Territory, acid.AcidNameInAts.Replace("/", ""));
				await context.ACIDs.AddAsync(acid);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<int> UpdateAcid(ACID acid)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int res = 0;
			acid.ModifyDate = DateTime.Now;
			if (acid.Layout.Contains("base64"))
			{
				acid.Layout = await utility.UploadFile(acid.Layout, "ACID_" + acid.Territory, acid.AcidNameInAts.Replace("/", ""));
			}
			context.ACIDs.Update(acid);
			res = await context.SaveChangesAsync();
			return res;
		}
	}
}
