#include <IRremote.h>
#include <Streaming.h>


IRrecv irrecv(2);
uint32_t previousCommand;


void setup()
{
	Serial.begin(9600);
	irrecv.enableIRIn();
}
void loop()
{
    if (irrecv.decode())
    {
        //Serial << irrecv.results.value << "\n";
    	
        /*irrecv.printResultShort(&Serial);
        Serial.println();*/
    	
        if (irrecv.results.value != previousCommand)
        {
            Serial << irrecv.results.value << "\n";
                    	
            previousCommand = irrecv.results.value;
        }
    	           
        irrecv.resume();
    }

    delay(100);
}
