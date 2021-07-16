using System;
using System.Threading.Tasks;

namespace IotHub.ApiClients.EasyEsp.Interfaces
{
	public interface IEasyEspClient
	{
		Task<String> Unit2PlaySoundAsync(String rtttl);
		Task<String> PlaySoundAsync(String url, String rtttl);
		Task<String> ExecuteCommandAsync(String url, String cmd);
	}
}