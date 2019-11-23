int ledPin = 13;
int relayPin = 12;
int buttonPin = 8;


void setup()
{
	pinMode(ledPin, OUTPUT);
	pinMode(relayPin, OUTPUT);
	pinMode(buttonPin, INPUT);
}
void loop()
{
	digitalWrite(ledPin, digitalRead(buttonPin));
	digitalWrite(relayPin, !digitalRead(buttonPin));
}