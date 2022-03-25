using OCC_Aid_App.Models;
using System.Collections.Generic;

namespace OCC_Aid_App.Models.ViewModels
{
	public class GetLogsResponse
	{
		public List<Log> Logs { get; set; }
		public int Total { get; set; }
	}
}
