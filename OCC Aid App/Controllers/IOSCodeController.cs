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
	public class IOSCodeController : ControllerBase
	{
		private readonly IIOSCodeService service;
		public IOSCodeController(IIOSCodeService service)
		{
			this.service = service;
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> GetIOSCodes(int page, int take, string search, bool deleted)
		{
			return Ok(await service.GetIOSCodes(page, take, search, deleted));
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetIOSCodeDetailsByIOSNumber(string iosnumber)
		{
			var ios = await service.GetIOSCodeDetailsByIOSNumber(iosnumber);
			if (ios != null)
				return Ok(ios);
			return NotFound();
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> SaveIOSCode(IOSCode iOSCode)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (await service.SaveIOSCode(iOSCode) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> UpdateIOSCode(IOSCode iOSCode)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (await service.UpdateIOSCode(iOSCode) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> DeleteIOSCode(int id)
		{
			if (await service.DeleteIOSCode(id) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> RecoverIOSCode(int id)
		{
			if (await service.RecoverIOSCode(id) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
