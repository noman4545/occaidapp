using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Models.ViewModels
{
	public class GetIOSCodesResponse
	{
		public List<IOSCode> IOSCodes { get; set; }
		public int Total { get; set; } = 0;
	}
}
