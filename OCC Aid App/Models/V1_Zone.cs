using OCC_Aid_App.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OCC_Aid_App.Models
{
    public class V1_Zone : BaseEntity
	{
		[Required]
		public string Name { get; set; }
		[Required]
		public string FanDirection { get; set; }
		public string UpName { get; set; }
		public string LeftName { get; set; }
		public string RightName { get; set; }
		public string ShaftName { get; set; }
		[Required]
		public string CctvLayout { get; set; }
		[Required]
		public string ZoneLayout { get; set; }
		public virtual List<V1_ZoneBlock> ZoneBlocks { get; set; }
		[Required]
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public DateTime? ModifyDate { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime? DeletedDate { get; set; }
	}
}
