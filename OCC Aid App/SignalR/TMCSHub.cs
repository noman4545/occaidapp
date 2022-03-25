using OCC_Aid_App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OCC_Aid_App.SignalR
{
	public class TMCSHub : Hub
	{
		public async Task SendTMCS(object tmcs)
		{
			await Clients.Others.SendAsync("TMCSReceived", tmcs);
		}
		public async Task MarkCompleteTMCS(object id)
		{
			await Clients.Others.SendAsync("MarkCompleteTMCSReceived", id);
		}
	}
}
