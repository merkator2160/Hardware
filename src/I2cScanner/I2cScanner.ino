#include <Wire.h>
#include <Streaming.h>

void setup()
{
	Wire.begin();

	Serial.begin(9600);
	while (!Serial);
	Serial << "\nI2C Scanner";
}

void loop()
{
	Serial.println("Scanning...");

	getAddresses();

	delay(5000);
	// delay(LOOP_DELAY);
}
void getAddresses()
{
	auto nDevices = 0;
	for (byte address = 8; address < 127; address++)
	{
		Wire.beginTransmission(address);
		const auto error = Wire.endTransmission();

		if (error == 0)
		{
			Serial << "I2C device found at address 0x";
			if (address < 16)
				Serial << "0";
			Serial << _HEX(address);
			Serial << " !" << "\n";

			nDevices++;
		}
		else if (error == 4)
		{
			Serial << "Unknown error at address 0x";
			if (address < 16)
				Serial << "0";
			Serial << _HEX(address) << "\n";
		}
	}
	if (nDevices == 0)
		Serial << "No I2C devices found" << "\n";
	else
		Serial << "Done" << "\n\n";
}