using OCC_Aid_App.Models;
using OCC_Aid_App.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Interfaces
{
	public interface IIOSCodeService
	{
		Task<GetIOSCodesResponse> GetIOSCodes(int page, int take, string search, bool deleted);
		Task<int> SaveIOSCode(IOSCode iOSCode);
		Task<int> UpdateIOSCode(IOSCode iOSCode);
		Task<int> DeleteIOSCode(int id);
		Task<int> RecoverIOSCode(int id);
		Task<IOSCode> GetIOSCodeDetailsByIOSNumber(string IOSNumber);
	}
}
