using System.Collections.Generic;

namespace OCC_Aid_App.Models.ViewModels
{
	public class GetSMSResponse
	{
		public List<SMS> SMSs { get; set; }
		public int Total { get; set; }
	}
}
