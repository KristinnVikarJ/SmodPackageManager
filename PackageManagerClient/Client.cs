using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PackageManagerClient
{
	public class Client
	{
		static HttpClient client = new HttpClient();
		
		public static async Task<HttpResponseMessage> GetAsync(string url)
		{
			try
			{
				HttpResponseMessage message = await client.GetAsync(Program.apiEndpoint + url);
				return message;
			}
			catch(Exception e)
			{
				Logger.WriteLine("not got :(");
				Logger.WriteLine($"error: {e.Message}, stack: {e.StackTrace}");
			}
			return null;
		}

		public static Response Get(string url) => new Response(GetAsync(url));
		public static Response<T> Get<T>(string url) => new Response<T>(GetAsync(url));

		public static void Download(string url, string fullpath)
		{
			var GetTask = client.GetAsync(url);
			GetTask.Wait(50000);

			if (!GetTask.Result.IsSuccessStatusCode)
			{
				// write an error
				Logger.WriteLine("Download Failed! StatusCode: " + GetTask.Result.StatusCode.ToString(), ConsoleColor.Red);
				return;
			}

			using (var fs = new FileStream(fullpath, FileMode.OpenOrCreate))
			{
				var ResponseTask = GetTask.Result.Content.CopyToAsync(fs);
				ResponseTask.Wait(50000);
			}
		}
	}

}
