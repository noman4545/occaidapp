using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCC_Aid_App.Models
{
	public class ArchievedSMS
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
		[Required]
		public bool Completed { get; set; } = false;
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public DateTime? ModifyDate { get; set; }
	}
}
