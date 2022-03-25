namespace OCC_Aid_App.Models.APIModels
{
    public class ReturnResponse
    {
		public int Status { get; set; }
		public dynamic Message { get; set; }
		public dynamic Errors { get; set; }
	}
}
