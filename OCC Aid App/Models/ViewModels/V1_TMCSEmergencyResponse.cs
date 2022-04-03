using System;

namespace OCC_Aid_App.Models
{
    public class V1_TMCSEmergencyResponse
	{
		public int Id { get; set; }
		public int ZoneId { get; set; }
		public int? BlockId { get; set; }
		public string DmDecision { get; set; }
		public bool EfcMarkedCompleted { get; set; }
		public bool Completed { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public V1_ZoneResponse Zone { get; set; }
		public V1_BlockResponse Block { get; set; }
	}
}
