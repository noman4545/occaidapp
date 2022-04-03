using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OCC_Aid_App.Interfaces;
using OCC_Aid_App.Models;
using System.Threading.Tasks;

namespace OCC_Aid_App.Controllers
{
    [Route("api/[controller]/[action]")]
	[ApiController]
	public class TMCSController : ControllerBase
	{
		private readonly ITMCSService service;
		public TMCSController(ITMCSService service)
		{
			this.service = service;
		}

		#region V1
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> SaveZoneV1(V1_ZoneRequest zone)
		{
			if (await service.SaveZoneV1Async(zone) == true)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> UpdateZoneV1(V1_ZoneRequest zone)
		{
			if (await service.UpdateZoneV1(zone) > 0)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> GetZonesV1(int page, int take, string search, bool deleted)
		{
			return Ok(await service.GetZonesV1Async(page, take, search, deleted));
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> DeleteZoneV1(int id)
		{
			if (await service.DeleteZoneV1(id) > 0)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> RecoverZoneV1(int id)
		{
			if (await service.RecoverZoneV1(id) > 0)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> AddBlockV1(V1_Block block)
		{
			if (await service.AddBlockAsync(block) == true)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize]
		[HttpGet]
        public async Task<IActionResult> GetBlocksV1()
        {
            return Ok(await service.GetAllBlocksAsync());
        }

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetBlockZonesV1(int blockId)
		{
			return Ok(await service.GetBlockZonesAsync(blockId));
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> ActivateZoneV1(int zoneId, int blockId)
		{
			var tmcs = await service.ActivateZoneV1Async(zoneId, blockId);
			if (tmcs != null)
				return Ok(tmcs);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetEmergencyZonesV1()
		{
			return Ok(await service.GetEmergencyZonesV1Async());
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> SelectFanDirectionV1(int id, string direction)
		{
			if (await service.SelectFanDirectionV1Async(id, direction) > 0)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> MarkAsCompleteV1(int id)
		{
			if (await service.MarkAsCompleteV1Async(id) > 0)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
		#endregion

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> GetZones(int page, int take, string search, bool deleted)
		{
			return Ok(await service.GetZones(page, take, search, deleted));
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> SaveZone(Zone zone)
		{
			if (await service.SaveZone(zone) > 0)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> UpdateZone(Zone zone)
		{
			if (await service.UpdateZone(zone) > 0)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> DeleteZone(int id)
		{
			if (await service.DeleteZone(id) > 0)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> RecoverZone(int id)
		{
			if (await service.RecoverZone(id) > 0)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> ActivateZone(int zoneId, int blockId)
		{
			var tmcs = await service.ActivateZone(zoneId, blockId);
			if (tmcs != null)
				return Ok(tmcs);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetEmergencyZones()
		{
			return Ok(await service.GetEmergencyZones());
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> MarkAsComplete(int id)
		{
			if (await service.MarkAsComplete(id) > 0)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> SelectFanDirection(int id, string direction)
		{
			if (await service.SelectFanDirection(id, direction) > 0)
				return Ok();
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> SearchZoneByBlockId(int id)
		{
			var zone = await service.SearchZoneByBlockId(id);
			if (zone != null)
				return Ok(zone);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetPossibleExt1Blocks(int ext1)
		{
			return Ok(await service.GetPossibleExt1Blocks(ext1));
		}
	}
}
