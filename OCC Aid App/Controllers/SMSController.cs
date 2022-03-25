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
	public class SMSController : ControllerBase
	{
		private readonly ISMSService service;
		public SMSController(ISMSService service)
		{
			this.service = service;
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> GetSMSs(int page, int take, string search, bool deleted)
		{
			return Ok(await service.GetSMSs(page, take, search, deleted));
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetAllSMS()
		{
			return Ok(await service.GetAllSMS());
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> SaveSMS(SMS sms)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (await service.SaveSMS(sms) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public async Task<IActionResult> UpdateSMS(SMS sms)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (await service.UpdateSMS(sms) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> DeleteSMS(int id)
		{
			if (await service.DeleteSMS(id) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize(Roles = "Admin")]
		[HttpGet]
		public async Task<IActionResult> RecoverSMS(int id)
		{
			if (await service.RecoverSMS(id) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetArchieveSMS()
		{
			return Ok(await service.GetArchieveSMS());
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> SaveArchieveSMS(ArchievedSMS sms)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			if (await service.SaveArchieveSMS(sms) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> MarkArchieveSMSComplete(int id)
		{
			if (await service.MarkArchieveSMSComplete(id) > 0)
				return Ok();

			return StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
