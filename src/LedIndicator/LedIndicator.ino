int ledPin = 13;
boolean ledState = HIGH;

void setup()
{
	pinMode(ledPin, OUTPUT);

	Serial.begin(9600);
}
void loop()
{
	ledState = !ledState;
	digitalWrite(ledPin, ledState);
	delay(1000);

	if (ledState)
	{
		Serial.println("Led is OFF!");
	}
	else
	{
		Serial.println("Led is ON!");
	}
	delay(100);
}