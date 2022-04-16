using Autofac;
using System.Reflection;
using uPLibrary.Networking.M2Mqtt;
using Module = Autofac.Module;

namespace IotHub.Api.Services.Mqtt
{
    public class MosquittoClientModule : Module
    {
        private readonly Assembly[] _assembliesToScan;


        public MosquittoClientModule(Assembly[] assembliesToScan)
        {
            _assembliesToScan = assembliesToScan;
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(new Dictionary<String, MqttClient.MqttMsgPublishEventHandler>());

            builder.RegisterType<MosquittoClient>().AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterType<DeviceMonitor>().AsSelf().AsImplementedInterfaces().SingleInstance();





            RegisterHandlers(builder);
        }
        private void RegisterHandlers(ContainerBuilder builder)
        {
            //configure.AddMaps(_assembliesToScan);     // Dynamically load all handlers

            //builder.RegisterType<MiddleRoomGreenhouseLightHandler>().AsSelf().SingleInstance();
        }
    }
}