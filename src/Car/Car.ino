#define D1 2          // Right engine direction
#define M1 3          // Right engine speed control
#define D2 4          // Left engine direction
#define M2 5          // Left engine speed control

#define JOYSTICK_BTN 6
#define LED 13

#define JOYSTICK_X 0
#define JOYSTICK_Y 1

#define JOYSTICK_SENSITIVITY_CONSTRAIN 30


void setup()
{
	pinMode(JOYSTICK_BTN, INPUT);
	pinMode(LED, OUTPUT);
	pinMode(D1, OUTPUT);
	pinMode(D2, OUTPUT);

	Serial.begin(9600);
}
void loop()
{
	auto x = analogRead(JOYSTICK_X);
	auto y = analogRead(JOYSTICK_Y);
	auto joystickButton = digitalRead(JOYSTICK_BTN);
	digitalWrite(LED, !joystickButton);

	int centralSpeed = SetSensitivityConstrain(ConvertToSpeed(y), JOYSTICK_SENSITIVITY_CONSTRAIN);
	int speedOffset = SetSensitivityConstrain(ConvertToSpeed(x), JOYSTICK_SENSITIVITY_CONSTRAIN);

	CalculateWheelActions(centralSpeed, speedOffset);
}


// FUNCTIONS //////////////////////////////////////////////////////////////////////////////////
int SetSensitivityConstrain(int value, int limit)
{
	if (value <= limit && value >= -limit)
		return 0;
	return value;
}
int ConvertToSpeed(int value)
{
	if (value >= 512)
	{
		return map(value, 512, 1023, 0, -255);
	}
	else
	{
		return map(value, 512, 0, 0, 255);
	}
}
void CalculateWheelActions(int centralSpeed, int speedOffset)
{
	int leftSpeed;
	int rightSpeed;

	if (centralSpeed != 0)
	{
		if (speedOffset > 0)
		{
			leftSpeed = centralSpeed - speedOffset;
			rightSpeed = centralSpeed;
		}
		else
		{
			leftSpeed = centralSpeed;
			rightSpeed = centralSpeed - speedOffset;
		}
	}
	else
	{
		Serial.println(speedOffset);
		leftSpeed = -speedOffset;
		rightSpeed = speedOffset;
	}

	moveChassis(leftSpeed, rightSpeed);
}
void moveChassis(int speedLeft, int speedRight)
{
	analogWrite(M2, speedLeft);
	if (speedLeft >= 0)
	{
		digitalWrite(D2, false);
	}
	else
	{
		digitalWrite(D2, true);
	}

	analogWrite(M1, speedRight);
	if (speedRight >= 0)
	{
		digitalWrite(D1, false);
	}
	else
	{
		digitalWrite(D1, true);
	}
}