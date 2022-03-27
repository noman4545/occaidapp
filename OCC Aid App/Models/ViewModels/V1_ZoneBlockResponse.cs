using System;

namespace OCC_Aid_App.Models
{
    public class V1_ZoneBlockResponse
    {
        public int Id { get; set; }
        public int ZoneId { get; set; }
        public int BlockId { get; set; }
        public V1_BlockResponse Block { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}
