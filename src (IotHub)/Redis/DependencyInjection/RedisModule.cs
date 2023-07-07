using Autofac;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Module = Autofac.Module;

namespace Redis.DependencyInjection
{
    public class RedisModule : Module
    {
        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public const String ConnectionStringName = "RedisConnection";
        

        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(scope =>
            {
                var connectionString = scope.Resolve<IConfiguration>().GetConnectionString(ConnectionStringName);

                return ConnectionMultiplexer.Connect(connectionString);
            }).SingleInstance();

            builder.RegisterType<RedisClient>().AsSelf().AsImplementedInterfaces();
        }
    }
}