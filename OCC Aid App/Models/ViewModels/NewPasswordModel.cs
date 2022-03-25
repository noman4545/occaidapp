using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Models.APIModels
{
	public class NewPasswordModel
	{
		public string UserId { get; set; }
		public string password { get; set; }
		public string confirmPassword { get; set; }
	}
}
