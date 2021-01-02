#include "iarduino_RTC.h"


iarduino_RTC time(RTC_DS1302,5,4,3);


void setup() 
{
    delay(300);
    Serial.begin(9600);
    time.begin();
}
void loop()
{
    if(millis()%1000==0)
    {
      Serial.println(time.gettime("d-m-Y, H:i:s, D"));
      delay(1);
    }
}

void setup() 
{
    delay(300);
    Serial.begin(9600);
    time.begin();
    time.settime(0,34,7,11,9,19,3);
}
void loop()
{
    if(millis()%1000==0)
    {
      Serial.println(time.gettime("d-m-Y, H:i:s, D"));
      delay(1);
    }
}
