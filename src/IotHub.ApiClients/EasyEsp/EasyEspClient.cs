using IotHub.ApiClients.EasyEsp.Interfaces;
using IotHub.ApiClients.EasyEsp.Models.Config;
using IotHub.ApiClients.EasyEsp.Models.Exceptions;
using IotHub.Common.Http;
using System;
using System.Threading.Tasks;

namespace IotHub.ApiClients.EasyEsp
{
	public class EasyEspClient : TypedHttpClient, IEasyEspClient
	{
		private readonly EasyEspConfig _easyEspConfig;


		public EasyEspClient(EasyEspConfig easyEspConfig)
		{
			_easyEspConfig = easyEspConfig;
		}


		// IEasyEspClient /////////////////////////////////////////////////////////////////////////
		public async Task<String> Unit2PlaySoundAsync(String rtttl)
		{
			return await PlaySoundAsync(_easyEspConfig.Unit2Uri, rtttl);
		}


		public async Task<String> PlaySoundAsync(String url, String rtttl)
		{
			return await ExecuteCommandAsync(url, $"rtttl,{rtttl}");
		}
		public async Task<String> ExecuteCommandAsync(String url, String cmd)
		{
			using(var response = await GetAsync($"http://{url}/control?cmd={cmd}"))
			{
				if(response.IsSuccessStatusCode)
					return await response.Content.ReadAsStringAsync();

				throw new EasyEspClientException(await response.Content.ReadAsStringAsync());
			}
		}
	}
}