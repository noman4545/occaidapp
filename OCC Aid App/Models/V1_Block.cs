using OCC_Aid_App.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OCC_Aid_App.Models
{
    public class V1_Block : BaseEntity
	{
		[Required]
		public string Name { get; set; }
		public virtual List<V1_ZoneBlock> ZoneBlocks { get; set; }

		[Required]
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public DateTime? ModifyDate { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime? DeletedDate { get; set; }
	}
}
