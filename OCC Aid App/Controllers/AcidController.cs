using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OCC_Aid_App.Interfaces;
using OCC_Aid_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class AcidController : ControllerBase
	{
		private readonly IACIDService service;
		public AcidController(IACIDService service)
		{
			this.service = service;
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> GetAcids(int page, int take, string search, bool deleted)
		{
			return Ok(await service.GetAcids(page, take, search, deleted));
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetACIDDetailsByATSNumber(string atsname)
		{
			var acid = await service.GetACIDDetailsByATSNumber(atsname);
			if (acid != null)
				return Ok(acid);
			return NotFound();
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetAcidsByTerritory(string territory)
		{
			return Ok(await service.GetAcidsByTerritory(territory));
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetTerritories()
		{
			return Ok(await service.GetTerritories());
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> SaveAcid(ACID acid)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (await service.SaveAcid(acid) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> UpdateAcid(ACID acid)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (await service.UpdateAcid(acid) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> DeleteAcid(int id)
		{
			if (await service.DeleteAcid(id) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> RecoverAcid(int id)
		{
			if (await service.RecoverAcid(id) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
