using Autofac;
using Module = Autofac.Module;

namespace Mediator.DependencyInjection
{
    public class MediatorModule : Module
    {
        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Messenger>().AsSelf().AsImplementedInterfaces().SingleInstance();
        }
    }
}