#include <iarduino_IR_RX.h>


iarduino_IR_RX ir(2);


void setup()
{
    Serial.begin(9600);
    ir.begin();
}
void loop()
{
    if (ir.check())
    {
        Serial.println(ir.data);
        //Serial.println(ir.data, HEX);
        //Serial.println(ir.length);
    }
}
