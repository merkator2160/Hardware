using System.Threading.Tasks;

namespace IotHub.Common.Hangfire.Interfaces
{
	public interface IJob<T>
	{
		Task Execute(T parameter);
	}
}