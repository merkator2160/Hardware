namespace Common.Contracts.Api.DeviceMonitor
{
    public class DeviceUnderTrackingAm
    {
        public String Name { get; set; }
        public String AvailabilityTopic { get; set; }
        public DateTime LastAvailable { get; set; }
    }
}