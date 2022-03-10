using System;

namespace IotHub.Common.Const.IrController
{
	// TODO: Stateful command system is on the transmitter side. Conditioner itself is dump. Deeper investigation with real device testing required.
	public static class Haier
	{
		public const String Model = "YR-W02";

		public const Int64 HealthOn = 33591718;
		public const Int64 HealthOff = 37286;

		public const Int64 On = 33591974;
		public const Int64 Off = 33591462;

		public const Int64 Fan = 33591718;
		public const Int64 Mode = 0;
		public const Int64 Swing = 0;

		public const Int64 TempUp = 0;
		public const Int64 TempDown = 0;

		public const Int64 AirflowOn = 33591462;
		public const Int64 AirflowOff = 33594534;

		public const Int64 Timer = 0;
		public const Int64 Set = 0;
		public const Int64 Sleep = 33594534;

		public const Int64 LongPush = 33587878;
	}
}