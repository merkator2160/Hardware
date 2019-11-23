#include <DS1302.h>

const int kCePin = 5;		// Chip Enable
const int kIoPin = 6;		// Input/Output
const int kSclkPin = 7;		// Serial Clock

DS1302 rtc(kCePin, kIoPin, kSclkPin);


void setup()
{
	Serial.begin(9600);
	
	rtc.writeProtect(false);
	rtc.halt(false);

	ScreenInit();	
}
void loop()
{
	if(Serial.available() > 0)
	{
		HandleCommand(Serial.readString());
	}

	PrintTime(rtc.time());
	delay(1000);
}