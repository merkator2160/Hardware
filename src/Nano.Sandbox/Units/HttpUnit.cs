using Nano.Common.Extensions;
using Nano.Sandbox.Models;
using nanoFramework.Json;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Nano.Sandbox.Units
{
	public static class HttpUnit
	{
		public static void Run()
		{
			var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://worldtimeapi.org/api/timezone/Europe/Moscow");
			httpWebRequest.Method = "GET";

			using(var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				using(var stream = httpWebResponse.GetResponseStream())
				{
					var buffer = new Byte[1024];
					var bufferUsed = stream.Read(buffer, 0, buffer.Length);

					var responseStr = Encoding.UTF8.GetString(buffer);
					Debug.WriteLine(responseStr);

					var response = (WorldTimeResponse)JsonConvert.DeserializeObject(responseStr, typeof(WorldTimeResponse));
					Debug.WriteLine(response.abbreviation);
				}
			}
		}
	}
}