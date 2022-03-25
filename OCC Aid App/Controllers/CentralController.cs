using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class CentralController : ControllerBase
	{
		private readonly IConfiguration configuration;
		public CentralController(IConfiguration Configuration)
		{
			configuration = Configuration;
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public IActionResult GetRoles()
		{
			var roles = configuration.GetSection("AppRoles").Get<List<string>>();
			if (roles != null)
				return Ok(roles);
			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
