int ledPin = 13;
int buttonPin = 8;


void setup()
{
	pinMode(ledPin, OUTPUT);
	pinMode(buttonPin, INPUT);
}
void loop()
{
	digitalWrite(ledPin, digitalRead(buttonPin));
}