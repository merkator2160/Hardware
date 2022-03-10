using System.Threading.Tasks;

namespace IotHub.Common.Hangfire.Interfaces
{
	public interface IAsyncJob<T>
	{
		Task ExecuteAsync(T parameter);
	}
}