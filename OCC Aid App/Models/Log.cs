using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Models
{
	public class Log
	{
		[Key]
		public int Id { get; set; }
		public string UserId { get; set; }
		[Required]
		public string Message { get; set; }
		public string Role { get; set; } = "";
		public string ActionRole { get; set; }
		public string ActionUserId { get; set; }
		[Required]
		public string Screen { get; set; }
		[Required]
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		[Required]
		public bool Read { get; set; } = false;
	}
}
