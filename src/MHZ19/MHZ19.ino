#include <LiquidCrystal.h>
#include <SoftwareSerial.h>
#include <MHZ19.h>
#include <Streaming.h>
#include <LiquidCrystal_I2C.h> 

const uint8_t delayMs = 1000;

LiquidCrystal_I2C lcd(0x27, 16, 2);     // IIC Address, strLength, numberOfRows
SoftwareSerial ss(3, 2);                // RX, TX
MHZ19 mhz(&ss);


void setup()
{
    lcd.init();
    lcd.backlight();
	
    ss.begin(9600);	
	Serial.begin(9600);
}
void loop()
{
	auto data = mhz.retrieveData();
    if (data == MHZ19_RESULT_OK)
    {
        //sendData();
        printData();
    }
    else
    {
        Serial << "Error, code: " << data << "\n";
    }

    delay(delayMs);
}


// FUNCTIONS //////////////////////////////////////////////////////////////////////////////////////
void sendData()
{
    Serial <<
        "CO2:" << mhz.getCO2() << "," <<
        "MinCO2:" << mhz.getMinCO2() << "," <<
        "Temperature:" << mhz.getTemperature() << "," <<
        "Accuracy:" << mhz.getAccuracy() << "\n";
}
void printData()
{
    lcd.setCursor(0, 0);
    lcd.print(mhz.getCO2());
    lcd.setCursor(0, 1);
    lcd.print(mhz.getTemperature());
}