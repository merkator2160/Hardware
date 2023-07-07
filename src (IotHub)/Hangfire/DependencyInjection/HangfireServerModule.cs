using Autofac;
using Hangfire.Models;
using Hangfire.Mongo;
using System.Diagnostics;

namespace Hangfire.DependencyInjection
{
    public class HangfireServerModule : Module
    {
        private readonly String[] _queues;


        public HangfireServerModule(String[] queues)
        {
            _queues = queues;
        }


        // COMPONENT REGISTRATION /////////////////////////////////////////////////////////////////
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(CreateHangfireServerOptions);
            builder.Register(scope => new BackgroundJobServer(scope.Resolve<BackgroundJobServerOptions>(), scope.Resolve<MongoStorage>()));
        }
        private BackgroundJobServerOptions CreateHangfireServerOptions(IComponentContext scope)
        {
            var hangfireConfig = scope.Resolve<HangfireConfig>();
            return new BackgroundJobServerOptions()
            {
                WorkerCount = hangfireConfig.MaxParallelThreads,
                Queues = _queues,
                Activator = new Hangfire.AutofacJobActivator(scope.Resolve<ILifetimeScope>())
            };
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public static void RunHangfireServer(ILifetimeScope scope, CancellationToken cancellationToken = default)
        {
            using (var server = scope.Resolve<BackgroundJobServer>())
            {
                var monitoringApi = JobStorage.Current.GetMonitoringApi();
                while (!cancellationToken.IsCancellationRequested)
                {
                    var statistics = monitoringApi.GetStatistics();
                    var message = $"{nameof(statistics.Enqueued)}: {statistics.Enqueued}";

                    Trace.TraceInformation(message);
                    Console.WriteLine(message);

                    Thread.Sleep(1000);
                }
            }
        }
    }
}