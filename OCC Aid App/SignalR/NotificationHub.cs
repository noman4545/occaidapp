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
	public class NotificationHub : Hub
	{
		public async Task SendNotification(object notification)
		{
			await Clients.Others.SendAsync("NotificationReceived", notification);
		}

		public async Task SendSMS(object sms)
		{
			await Clients.Others.SendAsync("SMSReceived", sms);
		}
	}
}
