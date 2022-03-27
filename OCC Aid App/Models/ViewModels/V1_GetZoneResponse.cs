using System.Collections.Generic;

namespace OCC_Aid_App.Models.ViewModels
{
    public class V1_GetZoneResponse
	{
		public List<V1_ZoneResponse> Zones { get; set; }
		public int Total { get; set; } = 0;
	}
}
