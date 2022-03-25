using OCC_Aid_App.Models;
using OCC_Aid_App.Models.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OCC_Aid_App.Interfaces
{
	public interface ISMSService
	{
		Task<GetSMSResponse> GetSMSs(int page, int take, string search, bool deleted);
		Task<List<SMS>> GetAllSMS();
		Task<int> SaveSMS(SMS sms);
		Task<int> UpdateSMS(SMS sms);
		Task<int> DeleteSMS(int id);
		Task<int> RecoverSMS(int id);
		Task<int> SaveArchieveSMS(ArchievedSMS sms);
		Task<int> MarkArchieveSMSComplete(int id);
		Task<List<ArchievedSMS>> GetArchieveSMS();
	}
}
