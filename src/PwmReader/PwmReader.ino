#include "Streaming.h"
#include "Wire.h"


int channels[9];


void setup() 
{
	pinMode(2, INPUT);
	pinMode(3, INPUT);
	pinMode(4, INPUT);
	pinMode(5, INPUT);
	pinMode(6, INPUT);
	pinMode(7, INPUT);
	pinMode(8, INPUT);
	pinMode(9, INPUT);
	pinMode(10, INPUT);

	Serial.begin(9600);

	Wire.begin(8);			// Join i2c bus by the address
	Wire.onRequest(requestEvent);
	Wire.onReceive(receiveEvent);
}


// FUNCTIONS //////////////////////////////////////////////////////////////////////////////////////
void loop()
{
	readChannels();
	printChannelsToSerialAsJson();
}
void readChannels()
{
	channels[0] = pulseIn(2, HIGH);
	channels[1] = pulseIn(3, HIGH);
	channels[2] = pulseIn(4, HIGH);
	channels[3] = pulseIn(5, HIGH);
	channels[4] = pulseIn(6, HIGH);
	channels[5] = pulseIn(7, HIGH);
	channels[6] = pulseIn(8, HIGH);
	channels[7] = pulseIn(9, HIGH);
	channels[8] = pulseIn(10, HIGH);
}
void printChannelsToSerialAsJson()
{
	Serial
		<< "["
		<< channels[0] << ","
		<< channels[1] << ","
		<< channels[2] << ","
		<< channels[3] << ","
		<< channels[4] << ","
		<< channels[5] << ","
		<< channels[6] << ","
		<< channels[7] << ","
		<< channels[8]
		<< "]";

	Serial << "\n";
}


// HANDLERS ///////////////////////////////////////////////////////////////////////////////////////
void requestEvent()
{	
	Wire.write((uint8_t*)channels, sizeof(channels));
}
void receiveEvent(int howMany)
{
	for (int i = 0; i < howMany; i++)
	{
		Serial << Wire.read() << "\n";
	}
}