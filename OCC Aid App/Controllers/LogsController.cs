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
	public class LogsController : ControllerBase
	{
		private readonly ILogService service;
		public LogsController(ILogService service)
		{
			this.service = service;
		}

		[Authorize]
		[HttpPost]
		public IActionResult Log(Log log)
		{
			var savedLog = service.LogMessage(log);
			if (savedLog != null)
				return Ok(savedLog);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetLogs()
		{
			return Ok(await service.GetLogs());
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> GetSpecialLogs(int page, int take, string search, string role, string screen)
		{
			return Ok(await service.GetSpecialLogs(page, take, search, role, screen));
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> MarkAsRead()
		{
			await service.MarkAsRead();
			return Ok();
		}
	}
}
