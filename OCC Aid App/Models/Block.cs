using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Models
{
	public class Block
	{
		[Key]
		public int BlockId { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public int StartLength { get; set; }
		[Required]
		public int EndLength { get; set; }
		public string ShaftName { get; set; }
		[Required]
		[ForeignKey("ZoneId")]
		public int ZoneId { get; set; }
		[Required]
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public DateTime? ModifyDate { get; set; }
		public bool Deleted { get; set; } = false;
		public DateTime? DeletedDate { get; set; }

		[NotMapped]
		public int Ext2 { get; set; }
	}
}
