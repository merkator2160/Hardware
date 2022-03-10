using System.Threading.Tasks;

namespace IotHub.Common.Hangfire.Interfaces
{
	public interface IAsyncJob
	{
		Task ExecuteAsync();
	}
}