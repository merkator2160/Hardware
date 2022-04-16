using Autofac;
using IotHub.Common.BasicMediator;
using IotHub.Common.BasicMediator.Interfaces;

namespace Sandbox.Units.Mediator
{
    public static class MediatorUnit
    {
        public static void Run()
        {
            var container = CreateContainer();
            using (var scope = container.BeginLifetimeScope())
            {
                UseMessenger(scope);
                Console.ReadKey();
            }
        }
        private static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Messenger>().AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<Subscriber>().AsSelf();

            return builder.Build();
        }
        private static void UseMessenger(ILifetimeScope scope)
        {
            var subscriber = scope.Resolve<Subscriber>();
            var messenger = scope.Resolve<IMessenger>();

            messenger.Send("Hello world!");
        }
    }
}