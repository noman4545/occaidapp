using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OCC_Aid_App.DatabaseContext;
using OCC_Aid_App.Interfaces;
using OCC_Aid_App.Mapping;
using OCC_Aid_App.Models;
using OCC_Aid_App.Models.ViewModels;
using OCC_Aid_App.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCC_Aid_App.Services
{
	public class TMCSService : ITMCSService
	{
		#region Members
		private readonly AppDatabaseContext _dbContext;
        private readonly IServiceScopeFactory<AppDatabaseContext> factory;
		private readonly IConfiguration configuration;
		private FileUtility utility;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ITypeMapper _typeMapper;
		#endregion

		public TMCSService(
			IWebHostEnvironment environment,
			ITypeMapper typeMapper,
			AppDatabaseContext dbContext,
			IServiceScopeFactory<AppDatabaseContext> factory,
			IConfiguration configuration,
			FileUtility utility)
		{
            _dbContext = dbContext;
            _hostEnvironment = environment;
            _typeMapper = typeMapper;
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

		#region V1
		public async Task<bool> SaveZoneV1Async(V1_ZoneRequest zone)
		{
			var exists = await _dbContext.V1_Zones.FirstOrDefaultAsync(f => f.Name == zone.Name);
			if (exists == null)
			{
				var zoneV1 = _typeMapper.Map<V1_ZoneRequest, V1_Zone>(zone);
				var a = FileHelper.GetFileExtension(zone.ZoneLayout);

				zoneV1.CctvLayout = zone.Name.Replace("/", "") + FileHelper.GetFileExtension(zone.CctvLayout);
                zoneV1.ZoneLayout = zone.Name.Replace("/", "") + FileHelper.GetFileExtension(zone.ZoneLayout);
				zoneV1.ZoneBlocks = new List<V1_ZoneBlock>();
                foreach (var block in zone.Blocks)
                {
					var esistingBlock = await _dbContext.V1_Blocks.FirstOrDefaultAsync(b => b.Name.ToLower() == block.Name.ToLower());
					if(esistingBlock == null)
                    {
						zoneV1.ZoneBlocks.Add(new V1_ZoneBlock()
						{
							Block = new V1_Block()
                            {
								Name = block.Name,
                            }
						});
					}
                    else
                    {
						zoneV1.ZoneBlocks.Add(new V1_ZoneBlock()
						{
							BlockId = esistingBlock.Id
						});
					}
                }
				zone.ZoneLayout = await utility.UploadFile(zone.ZoneLayout, "Zone_ZoneLayout", zone.Name.Replace("/", ""));
				zone.CctvLayout = await utility.UploadFile(zone.CctvLayout, "Zone_CctvLayout", zone.Name.Replace("/", ""));
				await _dbContext.V1_Zones.AddAsync(zoneV1);
				await _dbContext.SaveChangesAsync();
				return true;
			}
			return false;
		}

		public async Task<int> UpdateZoneV1(V1_ZoneRequest zone)
		{
			int res = 0;
			var strategy = _dbContext.Database.CreateExecutionStrategy();
			await strategy.ExecuteAsync(async () =>
			{
				var existingZone = await _dbContext.V1_Zones
				.Include(z => z.ZoneBlocks).ThenInclude(zb => zb.Block)
				.FirstOrDefaultAsync(f => f.Id == zone.Id);
				if (existingZone != null)
				{
					existingZone.ModifyDate = DateTime.Now;
					existingZone.Name = zone.Name;
					existingZone.ShaftName = zone.ShaftName;
					existingZone.FanDirection = zone.FanDirection;
                    if (zone.FanDirection.Equals("Left/Right"))
                    {
						existingZone.UpName = string.Empty;
						existingZone.LeftName = zone.LeftName;
						existingZone.RightName = zone.RightName;
					}
                    else
                    {
						existingZone.LeftName = string.Empty;
						existingZone.RightName = string.Empty;
						existingZone.UpName = zone.UpName;
					}
					
					if (zone.ZoneLayout.Contains("base64"))
					{
						existingZone.ZoneLayout = zone.ZoneLayout;
						zone.ZoneLayout = await utility.UploadFile(zone.ZoneLayout, "Zone_ZoneLayout", zone.Name.Replace("/", ""));
					}

					if (zone.CctvLayout.Contains("base64"))
					{
						existingZone.CctvLayout = zone.CctvLayout;
						zone.CctvLayout = await utility.UploadFile(zone.CctvLayout, "Zone_CctvLayout", zone.Name.Replace("/", ""));
					}

					var removedBlocks = new List<V1_ZoneBlock>();
					existingZone.ZoneBlocks.ForEach(zoneBlock =>
					{
						if (zone.Blocks == null)
							removedBlocks.Add(zoneBlock);
						else if (!zone.Blocks.Any(b => b.Name.ToLower().Equals(zoneBlock.Block.Name.ToLower())))
							removedBlocks.Add(zoneBlock);
					});

					removedBlocks.ForEach(zoneBlock => existingZone.ZoneBlocks.Remove(zoneBlock));

					if (zone.Blocks != null)
					{
						zone.Blocks.ForEach(block =>
						{
							if (block.Id > 0)
							{
								var zoneBlock = existingZone.ZoneBlocks.FirstOrDefault(zb => zb.Block.Id == block.Id);
								if (zoneBlock.Block.Name != block.Name)
								{
									zoneBlock.Block.Name = block.Name;
									zoneBlock.Block.ModifyDate = DateTime.Now;
									zoneBlock.ModifyDate = DateTime.Now;

								}
							}
							else
							{
								var esistingBlock = _dbContext.V1_Blocks.FirstOrDefault(b => b.Name == block.Name);
								if (esistingBlock == null)
								{
									existingZone.ZoneBlocks.Add(new V1_ZoneBlock()
									{
										Block = new V1_Block()
										{
											Name = block.Name,
										}
									});
								}
								else
								{
									existingZone.ZoneBlocks.Add(new V1_ZoneBlock()
									{
										BlockId = esistingBlock.Id
									});
								}
							}
						});
					}
                }
				res = await _dbContext.SaveChangesAsync();
			});
			return res;
        }

		public async Task<V1_GetZoneResponse> GetZonesV1Async(int page, int take, string search, bool Deleted)
		{
			if (!string.IsNullOrEmpty(search))
			{
				var searched = await _dbContext.V1_Zones.Where(w => w.IsDeleted == Deleted && (w.Name.Contains(search)
				|| w.ShaftName.Contains(search) || w.ZoneBlocks.Any(a => a.Block.Name.Contains(search))))
				.Include(z => z.ZoneBlocks).ThenInclude(zb => zb.Block)
				.OrderByDescending(o => o.CreatedDate)
				.Select(p => new V1_Zone()
				{
					Id = p.Id,
					Name = p.Name,
					FanDirection = p.FanDirection,
					UpName = p.UpName,
					LeftName = p.LeftName,
					RightName = p.RightName,
					ShaftName = p.ShaftName,
					CreatedDate = p.CreatedDate,
					ModifyDate = p.ModifyDate,
					IsDeleted = p.IsDeleted,
					DeletedDate = p.DeletedDate,
					ZoneBlocks = p.ZoneBlocks
                })
				.Skip((page - 1) * take).Take(take)
				.ToListAsync();

				var total = await _dbContext.V1_Zones.CountAsync(w => w.IsDeleted == Deleted && (w.Name.Contains(search)
				|| w.ShaftName.Contains(search) || w.ZoneBlocks.Any(a => a.Block.Name.Contains(search))));
				var eesponseV1 = _typeMapper.Map<List<V1_Zone>, List<V1_ZoneResponse>>(searched);

				return new V1_GetZoneResponse { Zones = eesponseV1, Total = total };
			}
			var zonesV1 = await _dbContext.V1_Zones.Where(w => w.IsDeleted == Deleted)
				.Include(z=>z.ZoneBlocks).ThenInclude(zb=>zb.Block)
				.OrderByDescending(o => o.CreatedDate)
				.Select(p => new V1_Zone()
				{
					Id = p.Id,
					Name = p.Name,
					FanDirection = p.FanDirection,
					UpName = p.UpName,
					LeftName = p.LeftName,
					RightName = p.RightName,
					ShaftName = p.ShaftName,
					CreatedDate = p.CreatedDate,
					ModifyDate = p.ModifyDate,
					IsDeleted = p.IsDeleted,
					DeletedDate = p.DeletedDate,
					ZoneBlocks = p.ZoneBlocks
				})
				.Skip((page - 1) * take).Take(take)
				.ToListAsync();
			var totalZones = await _dbContext.V1_Zones.CountAsync(w => w.IsDeleted == Deleted);

			var zonesResponseV1 = _typeMapper.Map<List<V1_Zone>, List<V1_ZoneResponse>>(zonesV1);
			
			return new V1_GetZoneResponse { Zones = zonesResponseV1, Total = totalZones };
		}

		public async Task<int> DeleteZoneV1(int id)
		{
			var zone = await _dbContext.V1_Zones
				.Include(z=>z.ZoneBlocks)
				.FirstOrDefaultAsync(f => f.Id == id);
			int res = 0;
			if (zone != null)
			{
				zone.IsDeleted = true;
				zone.DeletedDate = DateTime.Now;
                zone.ZoneBlocks.ForEach(zoneBblock =>
                {
                    zoneBblock.IsDeleted = true;
                    zoneBblock.DeletedDate = DateTime.Now;
				});
				res = await _dbContext.SaveChangesAsync();
			}
			return res;
		}

		public async Task<int> RecoverZoneV1(int id)
		{
			var zone = await _dbContext.V1_Zones
				.Include(z => z.ZoneBlocks).ThenInclude(zb => zb.Block)
				.FirstOrDefaultAsync(f => f.Id == id);
			int res = 0;
			if (zone != null)
			{
				zone.IsDeleted = false;
				zone.DeletedDate = null;
				zone.ZoneBlocks.ForEach(zoneBblock =>
				{
					zoneBblock.IsDeleted = false;
					zoneBblock.DeletedDate = null;
					zoneBblock.Block.IsDeleted = false;
					zoneBblock.Block.DeletedDate = null;
				});
				res = await _dbContext.SaveChangesAsync();
			}
			return res;
		}

		public async Task<bool> AddBlockAsync(V1_Block block)
		{
			var exists = await _dbContext.V1_Blocks.FirstOrDefaultAsync(f => f.Name == block.Name);
			if (exists == null)
			{
				await _dbContext.V1_Blocks.AddAsync(block);
				await _dbContext.SaveChangesAsync();
				return true;
			}
			return false;
		}

		public async Task<List<V1_BlockResponse>> GetAllBlocksAsync()
		{
			var blocks = await _dbContext.V1_Blocks.Where(b=>b.IsDeleted == false).ToListAsync();
			return _typeMapper.Map<List<V1_Block>, List<V1_BlockResponse>>(blocks);
		}

        public async Task<List<V1_ZoneResponse>> GetBlockZonesAsync(int blockId)
        {
            var zoneBlocks = await _dbContext.V1_ZoneBlocks
                .Where(zb => !zb.IsDeleted && !zb.Zone.IsDeleted && zb.BlockId == blockId)
                .Include(b => b.Zone).ToListAsync();
            var zones = zoneBlocks.Select(b => b.Zone).Distinct().ToList();
            return _typeMapper.Map<List<V1_Zone>, List<V1_ZoneResponse>>(zones);
        }

		public async Task<V1_TMCSEmergencyResponse> ActivateZoneV1Async(int zoneId, int blockId)
		{
			var exists = await _dbContext.V1_TMCSEmergencies.Where(w => w.ZoneId == zoneId && !w.Completed).ToListAsync();
			if (exists.Count() == 0)
			{
				var tmcsEmergency = new V1_TMCSEmergency
				{
					ZoneId = zoneId,
					BlockId = blockId
				};
				await _dbContext.V1_TMCSEmergencies.AddAsync(tmcsEmergency);
				int res = await _dbContext.SaveChangesAsync();
				if (res > 0)
                {
					var tEmergency = await _dbContext.V1_TMCSEmergencies.Include(tmcs => tmcs.Zone).Include(tmcs => tmcs.Block).FirstOrDefaultAsync(f => f.Id == tmcsEmergency.Id);
					return _typeMapper.Map<V1_TMCSEmergency, V1_TMCSEmergencyResponse>(tEmergency);
				}
				return null;
			}
			return null;
		}

		public async Task<List<V1_TMCSEmergencyResponse>> GetEmergencyZonesV1Async()
		{
			var tmcsEmergencies = await _dbContext.V1_TMCSEmergencies.Where(w => !w.Completed)
				.Include(te=>te.Zone).Include(te => te.Block).ToListAsync();
            return _typeMapper.Map<List<V1_TMCSEmergency>, List<V1_TMCSEmergencyResponse>>(tmcsEmergencies);
        }

		public async Task<int> SelectFanDirectionV1Async(int id, string direction)
		{
			var tmcs = await _dbContext.V1_TMCSEmergencies.FindAsync(id);
			int res = 0;
			if (tmcs != null)
			{
				tmcs.DmDecision = direction;
				res = await _dbContext.SaveChangesAsync();
			}
			return res;
		}

		public async Task<int> MarkAsCompleteV1Async(int id)
		{
			var tmcs = await _dbContext.V1_TMCSEmergencies.FindAsync(id);
			int res = 0;
			if (tmcs != null)
			{
				tmcs.Completed = true;
				res = await _dbContext.SaveChangesAsync();
			}
			return res;
		}
		
		public async Task<int> MarkEfcAsCompleteV1Async(int id)
		{
			var tmcs = await _dbContext.V1_TMCSEmergencies.FindAsync(id);
			int res = 0;
			if (tmcs != null)
			{
				tmcs.EfcMarkedCompleted = true;
				res = await _dbContext.SaveChangesAsync();
			}
			return res;
		}
		#endregion

		
	}
}
