#include <ESP8266WiFi.h>
#include <SoftwareSerial.h>
#include <MHZ.h>
#include <Streaming.h>


MHZ co2(D4, D0, D1, MHZ19B);    //Rx, Tx, PWM


void setup()
{
    pinMode(D1, INPUT);       // CO2_IN
	
    Serial.begin(115200);   
	
    delay(100);
    Serial.println("MHZ 19B");

    // enable debug to get addition information
    // co2.setDebug(true);

    /*if (co2.isPreHeating()) 
    {
        Serial.print("Preheating");
        while (co2.isPreHeating()) 
        {
            Serial.print(".");
            delay(5000);
        }
        Serial.println();
    }*/
}
void loop()
{
    //Serial.print("\n----- Time from start: ");
    //Serial.print(millis() / 1000);
    //Serial.println(" s");

    int ppmUart = co2.readCO2UART();
    int ppmPwm = co2.readCO2PWM();
    int temperature = co2.getLastTemperature();

    Serial << 
        "PPM uart: " << ppmUart <<
        ", PPM pwm: " << ppmPwm <<
        ", temperature: " << temperature << 
        "\n";
	
    delay(5000);
}
