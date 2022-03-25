using OCC_Aid_App.Models;
using OCC_Aid_App.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Interfaces
{
	public interface IACIDService
	{
		Task<GetACIDResponse> GetAcids(int page, int take, string search, bool deleted);
		Task<int> SaveAcid(ACID acid);
		Task<int> UpdateAcid(ACID acid);
		Task<int> DeleteAcid(int id);
		Task<int> RecoverAcid(int id);
		Task<ACID> GetACIDDetailsByATSNumber(string atsname);
		Task<List<string>> GetTerritories();
		Task<List<string>> GetAcidsByTerritory(string territory);
	}
}
