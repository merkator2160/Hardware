namespace IotHub.Common.Const
{
    public static class ZigbeeDevice
    {
        public static class LargeRoom
        {
            public const String Thermometer = "0x00158D0006A0D2FE";
            public const String CircuitRelayRight = "0x60A423FFFEFF8B47";
            public const String CircuitRelayLeft = "0x60A423FFFEF8E07B";
        }
        public static class MiddleRoom
        {
            public const String GreenhouseLightRelay = "0x842E14FFFE15AFF5";
        }
        public static class SideRoom
        {
            public const String Thermometer = "0x00158D0006A039E3";
            public const String GreenhouseLightButton = "0x00158D000672710E";
            public const String GreenhouseLightCircuitRelay = "0x60A423FFFED2EFAF";
        }
        public static class Kitchen
        {
            public const String Thermometer = "0x00124B0022CF75B3";
            public const String KaktusSensor = "0x00124B0022609BE6";
            public const String KratonSensor = "0x00124B002203B9B9";
            public const String TornadoUltrasonicCockroachRepellerSwitch = "0x00124B0022630C8A";
            public const String MotionSensor = "0x00158D00075FC1A3";
            public const String SinkLight = "0xCC86ECFFFE9A119C";
        }

        public const String Button1 = "0x00158D00067271F0";
        public const String ModkamButtonPad12 = "0x00124B0022609BBE";
        public const String TuyaButtonPad4 = "0x540F57FFFECF71F3";
        public const String IrrigationStation = "0x00124B002257349F";
        public const String SonoffSwitch1 = "0x00124B00226D1518";
    }
}