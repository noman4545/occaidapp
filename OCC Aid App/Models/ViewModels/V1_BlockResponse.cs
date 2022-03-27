using System;

namespace OCC_Aid_App.Models
{
    public class V1_BlockResponse
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime CreatedDate { get; set; }
		public DateTime? ModifyDate { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime? DeletedDate { get; set; }
	}
}
