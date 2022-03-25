using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Models
{
	public class User : IdentityUser
	{
		[Required]
		public string Name { get; set; }
		public bool Verified { get; set; } = false;
		public DateTime LastAccessTime { get; set; }
		public string Role { get; set; }
	}
}
