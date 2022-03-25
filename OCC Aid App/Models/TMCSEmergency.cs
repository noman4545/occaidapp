using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Models
{
	public class TMCSEmergency
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public int ZoneId { get; set; }
		public int? BlockId { get; set; }
		public string DmDecision { get; set; }
		[Required]
		public bool Completed { get; set; } = false;
		[Required]
		public DateTime CreatedDate { get; set; } = DateTime.Now;

		[ForeignKey("ZoneId")]
		public Zone Zone { get; set; }
		[ForeignKey("BlockId")]
		public Block Block { get; set; }
	}
}
