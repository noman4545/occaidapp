using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OCC_Aid_App.Interfaces;
using OCC_Aid_App.Models;
using OCC_Aid_App.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace OCC_Aid_App.Controllers
{
    [Route("api/[controller]/[action]")]
	[ApiController]
	public class TMCSController : ControllerBase
	{
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly ITMCSService service;
		public TMCSController(ITMCSService service, IWebHostEnvironment environment)
		{
			_hostEnvironment = environment;
			this.service = service;
		}

		#region V1
		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> SaveZoneV1(V1_ZoneRequest zone)
		{
            try
			{
				if (await service.SaveZoneV1Async(zone) == true)
					return Ok();
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			catch(Exception ex)
            {
				throw new Exception(ex.Message);
            }
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> UpdateZoneV1(V1_ZoneRequest zone)
		{
			try
			{
				if (await service.UpdateZoneV1(zone) > 0)
					return Ok();
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> GetZonesV1(int page, int take, string search, bool deleted)
		{
			try
			{
				return Ok(await service.GetZonesV1Async(page, take, search, deleted));
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> DeleteZoneV1(int id)
		{
			try
			{
				if (await service.DeleteZoneV1(id) > 0)
					return Ok();
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> RecoverZoneV1(int id)
		{
			try
			{
				if (await service.RecoverZoneV1(id) > 0)
					return Ok();
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> AddBlockV1(V1_Block block)
		{
			try
			{
				if (await service.AddBlockAsync(block) == true)
					return Ok();
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[Authorize]
		[HttpGet]
        public async Task<IActionResult> GetBlocksV1()
        {
			try
			{
				return Ok(await service.GetAllBlocksAsync());
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
        }

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetBlockZonesV1(int blockId)
		{
			try
			{
				return Ok(await service.GetBlockZonesAsync(blockId));
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> ActivateZoneV1(int zoneId, int blockId)
		{
			try
			{
				var tmcs = await service.ActivateZoneV1Async(zoneId, blockId);
				if (tmcs != null)
					return Ok(tmcs);
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetEmergencyZonesV1()
		{
			try
			{
				return Ok(await service.GetEmergencyZonesV1Async());
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> SelectFanDirectionV1(int id, string direction)
		{
			try
			{
				if (await service.SelectFanDirectionV1Async(id, direction) > 0)
					return Ok();
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[Authorize]
		[HttpGet]
        public async Task<IActionResult> MarkAsCompleteV1(int id)
        {
            try
            {
                if (await service.MarkAsCompleteV1Async(id) > 0)
					return Ok();
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [Authorize]
		[HttpGet]
		public async Task<IActionResult> MarkEfcAsCompleteV1(int id)
		{
			try
			{
				if (await service.MarkEfcAsCompleteV1Async(id) > 0)
					return Ok();
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> EfcRequireDMReviewV1(int id)
		{
			try
			{
				if (await service.EfcRequireDMReviewV1Async(id) > 0)
					return Ok();
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[Authorize]
		[HttpGet]
		public IActionResult GetZonesImageV1(string zoneFileName)
		{
            try
            {
                FileInfo fi = new FileInfo(zoneFileName);
                string path = Path.Combine(_hostEnvironment.WebRootPath, "Images", "Zone_ZoneLayout", zoneFileName);
                var imageFileStream = System.IO.File.OpenRead(path);
                return File(imageFileStream, $"image/{fi.Extension.Substring(1)}");
            }
            catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		[Authorize]
		[HttpGet]
		public IActionResult GetCCTVImageV1(string cctvFileName)
		{
			try
			{
				FileInfo fi = new FileInfo(cctvFileName);
				string path = Path.Combine(_hostEnvironment.WebRootPath, "Images", "Zone_CctvLayout", cctvFileName);
				var imageFileStream = System.IO.File.OpenRead(path);
				return File(imageFileStream, $"image/{fi.Extension.Substring(1)}");
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
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
