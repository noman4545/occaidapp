using OCC_Aid_App.Interfaces;
using OCC_Aid_App.Models;
using OCC_Aid_App.Models.APIModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OCC_Aid_App.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService service;
        public AuthController(IAuthService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel userLogin)
        {
            var response = await service.Login(userLogin);
            if (response.Status == StatusCodes.Status200OK)
                return Ok(response.Message);
            else if (response.Status == StatusCodes.Status401Unauthorized)
                return Unauthorized(response);
            else
                return StatusCode(response.Status, response);
        }

        [HttpPost]
        public async Task<IActionResult> Register(User userRegister)
        {
            var response = await service.Register(userRegister);
            if (response.Status == StatusCodes.Status200OK)
                return Ok(response.Message);
            else
                return StatusCode(response.Status, response);
        }

        [Authorize]
        public async Task<IActionResult> VerifyLoginToken()
        {
            var response = await service.VerifyLoginToken();
            if (response.Status == StatusCodes.Status200OK)
                return Ok(response.Message);
            return Unauthorized();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyAdminToken()
        {
            var response = await service.VerifyAdminToken();
            if (response.Status == StatusCodes.Status200OK)
                return Ok(response.Message);
            return Unauthorized();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await service.GetAllUsers();
            return Ok(response.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteUsers(string userid)
        {
            var response = await service.DeleteUsers(userid);
            if (response.Status == StatusCodes.Status200OK)
                return Ok(response.Message);
            return StatusCode(response.Status);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> SetNewPassword(NewPasswordModel newPassword)
        {
            var response = await service.SetNewPassword(newPassword);
            if (response.Status == StatusCodes.Status200OK)
                return Ok(response.Message);
            return StatusCode(response.Status);
        }
    }
}
