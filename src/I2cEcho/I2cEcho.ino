#include "Streaming.h"
#include "Wire.h"


uint8_t values[3];


void setup()
{
	Serial.begin(9600);

	Wire.begin(8);			// Join i2c bus by the address
	Wire.onRequest(requestEvent);
	Wire.onReceive(receiveEvent);
}
void loop()
{
	delay(1000);
}


// HANDLERS ///////////////////////////////////////////////////////////////////////////////////////
void requestEvent()
{
	Wire.write(values, sizeof(values));
}
void receiveEvent(int howMany)
{
	for (int i = 0; i < howMany; i++)
	{
		auto value = Wire.read();
		values[i] = value;

		Serial << value << "\n";
	}
}
