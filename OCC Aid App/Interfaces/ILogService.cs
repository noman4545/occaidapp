using OCC_Aid_App.Models;
using OCC_Aid_App.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Interfaces
{
	public interface ILogService
	{
		Log LogMessage(Log log);
		Task<List<Log>> GetLogs();
		Task<GetLogsResponse> GetSpecialLogs(int page, int take, string search, string role, string screen);
		Task<int> MarkAsRead();
	}
}
