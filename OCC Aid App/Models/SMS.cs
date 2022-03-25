using System;
using System.ComponentModel.DataAnnotations;

namespace OCC_Aid_App.Models
{
	public class SMS
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string TypeOfFailure { get; set; }
		[Required]
		public string SystemBehaviour { get; set; }
		[Required]
		public string WorkInstruction { get; set; }
		[Required]
		public string Message { get; set; }
		[Required]
		public string TimeToReturnToTimetable { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public DateTime? ModifyDate { get; set; }
		public bool Deleted { get; set; } = false;
		public DateTime? DeletedDate { get; set; }
	}
}
