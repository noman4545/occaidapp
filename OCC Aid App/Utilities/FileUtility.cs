using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OCC_Aid_App.Utilities
{
	public class FileUtility
	{
		public async Task<string> UploadFile(IFormFile file, string directoryName, string filename)
		{
			string path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images\" + directoryName + @"\");
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			var filePath = Path.Combine(path, filename + Path.GetExtension(file.FileName));
			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(fileStream);
				var returnPath = @"images\" + directoryName + @"\" + filename + Path.GetExtension(file.FileName);
				return returnPath;
			}
		}

		public async Task<string> UploadFile(string base64File, string directoryName, string filename)
		{
			string[] splittedBase64 = base64File.Split(',');
			string[] temp = splittedBase64[0].Split(';');
			temp = temp[0].Split('/');

			string fileExtension = "." + temp[1];

			byte[] bytes = Convert.FromBase64String(splittedBase64[1]);
			MemoryStream stream = new MemoryStream(bytes);
			IFormFile file = new FormFile(stream, 0, bytes.Length, filename, filename);

			string path = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Images\" + directoryName + @"\");
			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
			var filePath = Path.Combine(path, filename + fileExtension);
			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(fileStream);
				var returnPath = @"images\" + directoryName + @"\" + filename + fileExtension;
				return returnPath;
			}
		}
	}

	public static class FileHelper
    {
		public static string ConvertToBase64(this Stream stream)
		{
			byte[] bytes;
			using (var memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);
				bytes = memoryStream.ToArray();
			}

			return Convert.ToBase64String(bytes);
		}

		public static string GetFileExtension(string base64File)
		{
			string[] splittedBase64 = base64File.Split(',');
			string[] temp = splittedBase64[0].Split(';');
			return $".{temp[0].Split('/')[1]}";
		}
	}

    
}
