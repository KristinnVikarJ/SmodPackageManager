using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PackageManagerClient
{
	public class Client
	{
		private static HttpWebRequest PrepRequest(string url, string method)
		{
			HttpWebRequest request = HttpWebRequest.CreateHttp("http://piebot.xyz/api/smod" + url);
			request.ContentType = "application/json";
			request.Method = method;

			return request;
		}

		private static HttpWebRequest PrepRequest(string url, string method, byte[] data)
		{
			HttpWebRequest request = PrepRequest(url, method);

			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(data, 0, data.Length);
			}

			return request;
		}

		private static HttpWebResponse SendRequest(string url, string method, object data)
		{
			try
			{
				HttpWebResponse response = data != null ?
					(HttpWebResponse)PrepRequest(url, method, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))).GetResponse() :
					(HttpWebResponse)PrepRequest(url, method).GetResponse();

				return response;
			}
			catch (WebException e)
			{
				HttpWebResponse response = (HttpWebResponse)e.Response;
				if (response == null)
				{
					Logger.WriteLine("Could not connect to server.", ConsoleColor.Red);
					return null;
				}

				return response;
			}
		}

		public static Response Get(string url) => new Response(SendRequest(url, "GET", null));
		public static Response<T> Get<T>(string url) => new Response<T>(SendRequest(url, "GET", null));
		public static Response Head(string url) => new Response(SendRequest(url, "HEAD", null));
		public static Response<T> Head<T>(string url) => new Response<T>(SendRequest(url, "HEAD", null));
		public static Response Delete(string url) => new Response(SendRequest(url, "DELETE", null));
		public static Response<T> Delete<T>(string url) => new Response<T>(SendRequest(url, "DELETE", null));
		public static Response Post(string url, object data) => new Response(SendRequest(url, "POST", data));
		public static Response<T> Post<T>(string url, object data) => new Response<T>(SendRequest(url, "POST", data));
		public static Response Put(string url, object data) => new Response(SendRequest(url, "PUT", data));
		public static Response<T> Put<T>(string url, object data) => new Response<T>(SendRequest(url, "PUT", data));
		public static Response Patch(string url, object data) => new Response(SendRequest(url, "PATCH", data));
		public static Response<T> Patch<T>(string url, object data) => new Response<T>(SendRequest(url, "PATCH", data));

		public static void Download(string url, string fullpath)
		{
			using (WebClient cln = new WebClient())
				cln.DownloadFile(url, fullpath);
		}
	}

}
