#include <IRremoteESP8266.h>
#include <IRrecv.h>
#include <IRutils.h>
#include <Streaming.h>

const uint16_t kRecvPin = 14;

IRrecv irrecv(kRecvPin);
decode_results results;

void setup()
{
    Serial.begin(115200);
    irrecv.enableIRIn();
	
    while (!Serial)  // Wait for the serial connection to be establised.
        delay(50);
	
    Serial.println();
    Serial.print("IRrecvDemo is now running and waiting for IR message on Pin ");
    Serial.println(kRecvPin);
}

void loop()
{
    if (irrecv.decode(&results)) 
    {
        if(results.value != 0xFFFFFFFFFFFFFFFF)
        {
            serialPrintUint64(results.value, DEC);
            Serial.print("\n");
        }        
    	
        irrecv.resume();
    }
	
    delay(100);
}