using Hangfire;
using IotHub.Common.Hangfire.Interfaces;

namespace IotHub.Common.Hangfire.Extensions
{
	public static class BackgroundJobExtension
	{
		public static void Enqueue<T>(this BackgroundJob job) where T : IAsyncJob
		{
			BackgroundJob.Enqueue<T>(p => p.ExecuteAsync());
		}
	}
}