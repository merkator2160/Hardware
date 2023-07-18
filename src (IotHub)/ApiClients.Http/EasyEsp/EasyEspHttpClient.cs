using ApiClients.Http.EasyEsp.Models.Config;
using ApiClients.Http.EasyEsp.Models.Exceptions;
using Common.Http;

namespace ApiClients.Http.EasyEsp
{
    public class EasyEspHttpClient : TypedHttpClient
	{
		private readonly EasyEspConfig _easyEspConfig;


		public EasyEspHttpClient(EasyEspConfig easyEspConfig)
		{
			_easyEspConfig = easyEspConfig;
		}


		// IEasyEspHttpClient /////////////////////////////////////////////////////////////////////////
		public async Task<String> Unit2PlaySoundAsync(String rtttl)
		{
			return await PlaySoundAsync(_easyEspConfig.Unit2Uri, 14, rtttl);
		}
		public async Task<String> PlaySoundAsync(String url, Byte pinNumber, String rtttl)
		{
			return await ExecuteCommandAsync(url, $"rtttl,{pinNumber}:{rtttl}");
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