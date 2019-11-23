#include "Streaming.h"
#include "Wire.h"


const uint8_t SLAVE_ADDRESS = 8;
const uint8_t NUMBER_OF_CHANNELS = 9;
int channels[NUMBER_OF_CHANNELS];


void setup() 
{
	Serial.begin(9600);

	Wire.begin();			// Join i2c bus as master
}
void loop() 
{
	readData();
	printToSerialAsJson();
}
void readData()
{
	Wire.requestFrom(SLAVE_ADDRESS, sizeof(channels));	

	uint8_t temp[sizeof(channels)];
	for (int i = 0; i < sizeof(channels); i++)
	{
		temp[i] = Wire.read();		
	}

	auto channelsTemp = (int32_t*)temp;

	for (int i = 0; i < NUMBER_OF_CHANNELS; i++)
	{
		channels[i] = channelsTemp[i];
	}
}
void printToSerialAsJson()
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
		<< "]"
		<< "\n";
}
