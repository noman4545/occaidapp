using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Models
{
	public class IOSCode
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string IOSNumber { get; set; }
		[Required]
		public string Function { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public string Level { get; set; }
		[Required]
		public string OccAction { get; set; }
		[Required]
		public string TrainRescueAction { get; set; }
		[Required]
		public string MaintenanceAction { get; set; }
		public string Remarks { get; set; }
		[Required]
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public DateTime? ModifyDate { get; set; }
		public bool Deleted { get; set; } = false;
		public DateTime? DeletedDate { get; set; }
	}
}
