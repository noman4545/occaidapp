using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
	public class TMCSService : ITMCSService
	{
		private readonly IServiceScopeFactory<AppDatabaseContext> factory;
		private readonly IConfiguration configuration;
		private FileUtility utility;
		public TMCSService(IServiceScopeFactory<AppDatabaseContext> factory, IConfiguration configuration, FileUtility utility)
		{
			this.factory = factory;
			this.configuration = configuration;
			this.utility = utility;
		}

		public async Task<TMCSEmergency> ActivateZone(int zoneId, int blockId)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			var exists = await context.TMCSEmergencies.Where(w => w.ZoneId == zoneId && w.Completed == false).ToListAsync();
			if (exists.Count() == 0)
			{
				var tmcsEmergency = new TMCSEmergency();
				tmcsEmergency.ZoneId = zoneId;
				tmcsEmergency.BlockId = blockId;
				await context.TMCSEmergencies.AddAsync(tmcsEmergency);
				int res = await context.SaveChangesAsync();
				if (res > 0)
					return await context.TMCSEmergencies.Include("Zone").Include("Block").FirstOrDefaultAsync(f => f.Id == tmcsEmergency.Id);
				return null;
			}
			return null;
		}

		public async Task<int> DeleteZone(int id)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			var zone = await context.Zones.Include("Blocks").FirstOrDefaultAsync(f => f.ZoneId == id);
			int res = 0;
			if (zone != null)
			{
				zone.Deleted = true;
				zone.DeletedDate = DateTime.Now;
				zone.Blocks.ForEach(block => { block.Deleted = true; block.DeletedDate = DateTime.Now; });
				context.Update(zone);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<List<TMCSEmergency>> GetEmergencyZones()
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			return await context.TMCSEmergencies.Where(w => w.Completed == false).Include("Zone").Include("Block").ToListAsync();
		}

		public async Task<GetZoneResponse> GetZones(int page, int take, string search, bool Deleted)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			if (!string.IsNullOrEmpty(search))
			{
				var searched = await context.Zones.Where(w => w.Deleted == Deleted && (w.Name.Contains(search)
				|| w.Blocks.Any(a => a.Name.Contains(search) || a.ShaftName.Contains(search)))).Include("Blocks").OrderByDescending(o => o.CreatedDate).ToListAsync();

				return new GetZoneResponse { Zones = searched.Skip((page - 1) * take).Take(take).ToList(), Total = searched.Count };
			}
			var all = await context.Zones.Where(w => w.Deleted == Deleted).Include("Blocks").OrderByDescending(o => o.CreatedDate).ToListAsync();
			return new GetZoneResponse { Zones = all.Skip((page - 1) * take).Take(take).ToList(), Total = all.Count };
		}

		public async Task<int> MarkAsComplete(int id)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			var tmcs = await context.TMCSEmergencies.FindAsync(id);
			int res = 0;
			if (tmcs != null)
			{
				tmcs.Completed = true;
				context.TMCSEmergencies.Update(tmcs);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<int> SelectFanDirection(int id, string direction)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			var tmcs = await context.TMCSEmergencies.FindAsync(id);
			int res = 0;
			if (tmcs != null)
			{
				tmcs.DmDecision = direction;
				context.TMCSEmergencies.Update(tmcs);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<int> SaveZone(Zone zone)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int res = 0;
			var exists = await context.Zones.FirstOrDefaultAsync(f => f.Name == zone.Name);
			if (exists == null)
			{
				zone.ZoneLayout = await utility.UploadFile(zone.ZoneLayout, "Zone_ZoneLayout", zone.Name.Replace("/", ""));
				zone.CctvLayout = await utility.UploadFile(zone.CctvLayout, "Zone_CctvLayout", zone.Name.Replace("/", ""));
				await context.AddAsync(zone);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<Zone> SearchZoneByBlockId(int blockId)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			var block = await context.Blocks.FindAsync(blockId);

			Zone SelectedZone = null;
			if (block != null)
			{
				SelectedZone = await context.Zones.FindAsync(block.ZoneId);

				SelectedZone.Blocks.Clear();
				SelectedZone.Blocks.Add(block);
			}

			return SelectedZone;
		}

		public async Task<int> UpdateZone(Zone zone)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			zone.ModifyDate = DateTime.Now;
			if (zone.ZoneLayout.Contains("base64"))
			{
				zone.ZoneLayout = await utility.UploadFile(zone.ZoneLayout, "Zone_ZoneLayout", zone.Name.Replace("/", ""));
			}
			if (zone.CctvLayout.Contains("base64"))
			{
				zone.CctvLayout = await utility.UploadFile(zone.CctvLayout, "Zone_CctvLayout", zone.Name.Replace("/", ""));
			}
			zone.Blocks.ForEach(block => block.ModifyDate = DateTime.Now);
			var deleteBlocks = context.Blocks.AsNoTracking().AsEnumerable().Where(w => w.ZoneId == zone.ZoneId && zone.Blocks.All(a => a.BlockId != w.BlockId)).ToList();
			context.Blocks.RemoveRange(deleteBlocks);
			context.Update(zone);
			return await context.SaveChangesAsync();
		}

		public async Task<int> RecoverZone(int id)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			var zone = await context.Zones.Include("Blocks").FirstOrDefaultAsync(f => f.ZoneId == id);
			int res = 0;
			if (zone != null)
			{
				zone.Deleted = false;
				zone.DeletedDate = null;
				zone.Blocks.ForEach(block => { block.Deleted = false; block.DeletedDate = null; });
				context.Update(zone);
				res = await context.SaveChangesAsync();
			}
			return res;
		}

		public async Task<GetTMCSResponse> GetPossibleExt1Blocks(int ext1)
		{
			using var scope = factory.CreateScope();
			var context = scope.GetRequiredService();
			int tolerance = !string.IsNullOrEmpty(configuration["Tolerance"]) ? Convert.ToInt32(configuration["Tolerance"]) : 0;
			var blocks = await context.Blocks.Where(w => w.Deleted == false).Where(w => w.StartLength <= ext1 && w.EndLength >= ext1).ToListAsync();
			
			List<Block> allPossibleBlocks = new();
			List<Zone> zones = new();
			if(blocks.Count != 0)
			{

				for(int i = 0; i < blocks.Count; i++)
				{
					var ext2 = ext1 + (72 + tolerance);
					if(ext2 > blocks[i].EndLength)
					{
						int count = 0;
						List<Block> availableBlock = new();
						availableBlock.Add(blocks[i]);
						Block blockForZone = null;
						var trainLength = 0;
						bool skip = false;
						while (ext2 > availableBlock[count].EndLength)
						{
							var nextBlock = context.Blocks.AsEnumerable().OrderBy(o => o.CreatedDate).SkipWhile(b => b.BlockId != availableBlock[count].BlockId).Skip(1).FirstOrDefault();
							if(nextBlock != null)
							{
								if (nextBlock.Name != availableBlock[count].Name)
								{
									ext2 = ext2 - availableBlock[count].EndLength;
								}
								availableBlock.Add(nextBlock);
								count++;
							}
							else
							{
								skip = true;
								break;
							}
						}

						for (int j = 0; j < availableBlock.Count; j++)
						{
							if (j == 0)
							{
								var newTrainLength = availableBlock[j].EndLength - ext1;
								if (newTrainLength > trainLength)
								{
									trainLength = newTrainLength;
									blockForZone = availableBlock[j];
								}
							}
							else if(j == (availableBlock.Count - 1))
							{
								var newTrainLength = ext2 - availableBlock[j].StartLength;
								if (newTrainLength > trainLength)
								{
									trainLength = newTrainLength;
									blockForZone = availableBlock[j];
								}
							}
							else
							{
								var newTrainLength = availableBlock[j].EndLength - availableBlock[j].StartLength;
								if (newTrainLength > trainLength)
								{
									trainLength = newTrainLength;
									blockForZone = availableBlock[j];
								}
							}
						}
						if (!skip)
						{
							var blockToConsider = availableBlock.LastOrDefault();
							blockToConsider.Ext2 = ext2;
							allPossibleBlocks.Add(blockToConsider);

							var zone = await context.Zones.FirstOrDefaultAsync(f => f.ZoneId == blockForZone.ZoneId);
							zone.Blocks = new();
							zone.Blocks.Add(blockToConsider);
							zones.Add(zone);
						}
					}
					else
					{
						var addingBlock = blocks[i];
						addingBlock.Ext2 = ext2;
						allPossibleBlocks.Add(addingBlock);

						var zone = await context.Zones.FirstOrDefaultAsync(f => f.ZoneId == addingBlock.ZoneId);
						zone.Blocks = new();
						zone.Blocks.Add(addingBlock);
						zones.Add(zone);
					}
				}
			}

			return new GetTMCSResponse { Blocks = allPossibleBlocks, Zones = zones };
		}
	}
}
