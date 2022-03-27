using OCC_Aid_App.Models.Base;
using System;
using System.ComponentModel.DataAnnotations;

namespace OCC_Aid_App.Models
{
    public class V1_ZoneBlock : BaseEntity
    {
        public int ZoneId { get; set; }
        public virtual V1_Zone Zone { get; set; }
        public int BlockId { get; set; }
        public virtual V1_Block Block { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifyDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
