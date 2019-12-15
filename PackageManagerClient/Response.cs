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
	public class Response
	{
		public HttpStatusCode Code { get; }

		public bool Success => Code == HttpStatusCode.OK;

		public Response(Task<HttpResponseMessage> response)
		{
			if (response != null)
			{
				Code = response.Result.StatusCode;
			}
		}

		public static implicit operator bool(Response response) => response.Success;
	}

	public class Response<T> : Response
	{
		public T Data { get; }

		public Response(Task<HttpResponseMessage> response) : base(response)
		{
			if (Code == HttpStatusCode.OK)
			{
				Task<Stream> stream = response.Result.Content.ReadAsStreamAsync();

				if (stream != null)
				{
					using (StreamReader reader = new StreamReader(stream.Result))
					{
						Data = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
					}
				}
				stream.Dispose();
			}
		}
	}

}
