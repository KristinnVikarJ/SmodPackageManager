using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace PackageManagerClient
{
	public class Response
	{
		public HttpStatusCode Code { get; }

		public bool Success => Code == HttpStatusCode.OK;

		public Response(HttpWebResponse response)
		{
			if (response != null)
			{
				Code = response.StatusCode;
			}
		}

		public static implicit operator bool(Response response) => response.Success;
	}

	public class Response<T> : Response
	{
		public T Data { get; }

		public Response(HttpWebResponse response) : base(response)
		{
			if (Code == HttpStatusCode.OK)
			{
				Stream stream = response.GetResponseStream();

				if (stream != null)
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						Data = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
					}
				}
				stream.Dispose();
			}
		}
	}

}
