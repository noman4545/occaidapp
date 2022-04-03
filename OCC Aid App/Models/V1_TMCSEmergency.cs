using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCC_Aid_App.Models
{
    public class V1_TMCSEmergency
	{
		[Key]
		public int Id { get; set; }
		[Required]
		public int ZoneId { get; set; }
		public int? BlockId { get; set; }
		public string DmDecision { get; set; }
        public bool EfcMarkedCompleted { get; set; }
        [Required]
		public bool Completed { get; set; } = false;
		[Required]
		public DateTime CreatedDate { get; set; } = DateTime.Now;

		[ForeignKey("ZoneId")]
		public V1_Zone Zone { get; set; }
		[ForeignKey("BlockId")]
		public V1_Block Block { get; set; }
	}
}
