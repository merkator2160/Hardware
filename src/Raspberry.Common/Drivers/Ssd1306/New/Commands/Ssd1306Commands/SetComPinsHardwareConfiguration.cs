using System;

namespace Common.Drivers.Ssd1306.New.Commands.Ssd1306Commands
{
	/// <summary>
	/// Represents SetComPinsHardwareConfiguration command
	/// </summary>
	public class SetComPinsHardwareConfiguration : ISsd1306Command
	{
		/// <summary>
		/// This command sets the COM signals pin configuration to match the OLED panel hardware layout.
		/// </summary>
		/// <param name="alternativeComPinConfiguration">Alternative COM pin configuration.</param>
		/// <param name="enableLeftRightRemap">Enable left/right remap.</param>
		public SetComPinsHardwareConfiguration(Boolean alternativeComPinConfiguration = true, Boolean enableLeftRightRemap = false)
		{
			AlternativeComPinConfiguration = alternativeComPinConfiguration;
			EnableLeftRightRemap = enableLeftRightRemap;
		}

		/// <summary>
		/// The value that represents the command.
		/// </summary>
		public Byte Id => 0xDA;

		/// <summary>
		/// Alternative COM pin configuration.
		/// </summary>
		public Boolean AlternativeComPinConfiguration { get; }

		/// <summary>
		/// Enable left/right remap.
		/// </summary>
		public Boolean EnableLeftRightRemap { get; }

		/// <summary>
		/// Gets the bytes that represent the command.
		/// </summary>
		/// <returns>The bytes that represent the command.</returns>
		public Byte[] GetBytes()
		{
			Byte comPinsHardwareConfiguration = 0x02;

			if(AlternativeComPinConfiguration)
			{
				comPinsHardwareConfiguration |= 0x10;
			}

			if(EnableLeftRightRemap)
			{
				comPinsHardwareConfiguration |= 0x20;
			}

			return new Byte[] { Id, comPinsHardwareConfiguration };
		}
	}
}
