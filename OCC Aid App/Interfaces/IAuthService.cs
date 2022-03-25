using OCC_Aid_App.Models;
using OCC_Aid_App.Models.APIModels;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace OCC_Aid_App.Interfaces
{
    public interface IAuthService
    {
        Task<ReturnResponse> Login(LoginModel userLogin);
        Task<ReturnResponse> Register(User userRegister);
		Task<ReturnResponse> SetNewPassword(NewPasswordModel newPassword);
		Task<ReturnResponse> VerifyLoginToken();
        Task<ReturnResponse> VerifyAdminToken();
        Task<ReturnResponse> GetAllUsers();
        Task<ReturnResponse> DeleteUsers(string userid);
    }
}
