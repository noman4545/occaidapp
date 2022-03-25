using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Models
{
	public class ACID
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public string Territory { get; set; }
		[Required]
		public string AcidNameInAts { get; set; }
		[Required]
		public string AcidNameInIsm { get; set; }
		[Required]
		public string PedEepEesName { get; set; }
		[Required]
		public string TrackNo { get; set; }
		[Required]
		public string Cctv { get; set; }
		[Required]
		public string Layout { get; set; }
		[Required]
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public DateTime? ModifyDate { get; set; }
		public bool Deleted { get; set; } = false;
		public DateTime? DeletedDate { get; set; }
	}
}
