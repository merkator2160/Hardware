using Autofac;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Module = Autofac.Module;

namespace Hangfire.DependencyInjection
{
    public class HangfireObjectStorageModule : Module
    {
        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public const String HangfireDbConnectionStringName = "HangfireDbConnection";


        // COMPONENT REGISTRATION /////////////////////////////////////////////////////////////////
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new MongoStorageOptions()
            {
                CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection, // https://github.com/Hangfire-Mongo/Hangfire.Mongo/issues/300
                MigrationOptions = new MongoMigrationOptions
                {
                    MigrationStrategy = new DropMongoMigrationStrategy(),
                    BackupStrategy = new NoneMongoBackupStrategy()
                }
            });
            builder.Register(scope =>
            {
                var configuration = scope.Resolve<IConfiguration>();
                var connectionString = configuration.GetConnectionString(HangfireDbConnectionStringName);
                var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
                var clientSettings = MongoClientSettings.FromConnectionString(connectionString);

                return new MongoStorage(clientSettings, mongoUrlBuilder.DatabaseName, scope.Resolve<MongoStorageOptions>());
            });
        }
    }
}