using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Models
{
	public class Zone
	{
		[Key]
		public int ZoneId { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string FanDirection { get; set; }
		[Required]
		public string CctvLayout { get; set; }
		[Required]
		public string ZoneLayout { get; set; }
		public string UpName { get; set; }
		public string LeftName { get; set; }
		public string RightName { get; set; }
		[Required]
		public string TrackNo { get; set; }
		[Required]
		public List<Block> Blocks { get; set; }
		[Required]
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public DateTime? ModifyDate { get; set; }
		public bool Deleted { get; set; } = false;
		public DateTime? DeletedDate { get; set; }
	}
}
