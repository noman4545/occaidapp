using System.Collections.Generic;

namespace OCC_Aid_App.Models.ViewModels
{
	public class GetTMCSResponse
	{
		public List<Block> Blocks { get; set; }
		public List<Zone> Zones { get; set; }
	}
}
