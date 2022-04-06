using Hangfire;
using IotHub.Api.Services.Interfaces;
using IotHub.Common.Hangfire.Interfaces;
using uPLibrary.Networking.M2Mqtt.Messages;
using ILogger = NLog.ILogger;

namespace IotHub.Api.Middleware.Hangfire.Jobs
{
    internal class UpTimeJob : IJob
    {
        private readonly IMqttPublisher _mqttPublisher;
        private readonly ILogger _logger;

        private static readonly DateTime _startDate;


        static UpTimeJob()
        {
            _startDate = DateTime.Now;
        }
        public UpTimeJob(IMqttPublisher mqttPublisher, ILogger logger)
        {
            _mqttPublisher = mqttPublisher;
            _logger = logger;
        }


        // IJob ///////////////////////////////////////////////////////////////////////////////////
        [AutomaticRetry(Attempts = 0)]
        public void Execute()
        {
            try
            {
                if (_mqttPublisher.IsConnected)
                {
                    _mqttPublisher.Publish("iotHub/upTime", (DateTime.Now - _startDate).ToString(@"dd\:hh\:mm\:ss"), MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE, true);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"{ex.Message}\r\n{ex.StackTrace}");
                throw;
            }
        }
    }
}