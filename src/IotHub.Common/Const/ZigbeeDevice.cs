using System;

namespace IotHub.Common.Const
{
	public static class ZigbeeDevice
	{
		public const String LargeRoomThermometer = "0x00158D0006A0D2FE";        // Xiaomi Aqara, end device
		public const String SideRoomThermometer = "0x00158D0006A039E3";         // Xiaomi Aqara, end device
		public const String Button1 = "0x00158D00067271F0";                     // Xiaomi Aqara model: WXKG11LM, end device
		public const String Button2 = "0x00158D000672710E";                     // Xiaomi Aqara model: WXKG11LM, end device
		public const String SonoffThermometer = "0x00124B0022CF75B3";           // Model: SNZB-02, Battery model: SNZB-02, end device
		public const String SideRoomCircuitRelay = "0x60A423FFFED2EFAF";        // Model: BW-SHP13, router
		public const String LargeRoomCircuitRelay = "0x60A423FFFEFF8B47";       // Model: BW-SHP13, router
		public const String LargeRoomCircuitRelay2 = "0x60A423FFFEF8E07B";      // Model: BW-SHP13, router
		public const String KitchenKaktusSensor = "0x00124B0022609BE6";          // Model: modkam.ru	DIYRuZ_Flower, end device
		public const String KitchenFikusSensor = "0x00124B002203B9B9";  // Model: modkam.ru	DIYRuZ_Flower, end device
		public const String ButtonPad12 = "0x00124B0022609BBE";                 // Model: modkam.ru	DIYRuZ_FreePad, end device
		public const String IrrigationStation = "0x00124B002257349F";                   // Model: modkam.ru	DIYRuZ_Flower_WS, end device
	}
}