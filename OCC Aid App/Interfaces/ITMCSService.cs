﻿using OCC_Aid_App.Models;
using OCC_Aid_App.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OCC_Aid_App.Interfaces
{
	public interface ITMCSService
	{
		Task<GetZoneResponse> GetZones(int page, int take, string search, bool deleted);
		Task<int> SaveZone(Zone zone);
		Task<int> UpdateZone(Zone zone);
		Task<int> DeleteZone(int id);
		Task<int> RecoverZone(int id);
		Task<Zone> SearchZoneByBlockId(int blockId);
		Task<TMCSEmergency> ActivateZone(int zoneId, int blockId);
		Task<List<TMCSEmergency>> GetEmergencyZones();
		Task<int> MarkAsComplete(int id);
		Task<int> SelectFanDirection(int id, string direction);
		Task<GetTMCSResponse> GetPossibleExt1Blocks(int ext1);
	}
}
