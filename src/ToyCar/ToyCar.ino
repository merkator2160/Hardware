#include "Streaming.h"


const uint8_t _rollChannelPin = 2;
const uint8_t _pitchChannelPin = 3;

const uint8_t _rightEnginePin1 = 5;
const uint8_t _rightEnginePin2 = 6;

const uint8_t _leftEnginePin1 = 9;
const uint8_t _leftEnginePin2 = 10;

const uint8_t _joystickActivityConstrain = 50;
const uint8_t _joystickPullUpThreshold = 100;

const uint16_t _stickCentralValue = 1500;
const uint16_t _stickMinValue = 1000;
const uint16_t _stickMaxValue = 2000;


void setup() 
{
	pinMode(_rollChannelPin, INPUT);
	pinMode(_pitchChannelPin, INPUT);

	pinMode(_rightEnginePin1, OUTPUT);
	pinMode(_rightEnginePin2, OUTPUT);
	pinMode(_leftEnginePin1, OUTPUT);
	pinMode(_leftEnginePin2, OUTPUT);

	//Serial.begin(9600);
}


// FUNCTIONS //////////////////////////////////////////////////////////////////////////////////////
void loop() 
{
	delay(100);

	uint16_t pitchChannel = pulseIn(_pitchChannelPin, HIGH);
	uint16_t rollChannel = pulseIn(_rollChannelPin, HIGH);

	//Serial << pitchChannel << "," << rollChannel << "\n";		

	pitchChannel = pullupChannel(pitchChannel);
	rollChannel = pullupChannel(rollChannel);

	//Serial << pitchChannel << "," << rollChannel << "\n";

	auto isPitchInTheCenter = isStickInTheCenter(pitchChannel);
	auto isRollInTheCenter = isStickInTheCenter(rollChannel);
	
	//Serial << isPitchInTheCenter << "," << isRollInTheCenter << "\n";

	if (isPitchInTheCenter && isRollInTheCenter)
	{
		//Serial << "standby" << "\n";

		standby();
		return;
	}

	if (isPitchInTheCenter && !isRollInTheCenter)
	{
		//Serial << "moveCircle" << "\n";

		moveCircle(rollChannel);
		return;
	}

	if (!isPitchInTheCenter && !isRollInTheCenter)
	{
		//Serial << "moveBoth" << "\n";

		moveBoth(pitchChannel, rollChannel);
		return;
	}

	//Serial << "moveStright" << "\n";
	
	moveStright(pitchChannel);	
}


// ACTIONS ////////////////////////////////////////////////////////////////////////////////////////
void moveBoth(uint16_t pitchChannel, uint16_t rollChannel)
{
	auto xSpeed = calculateSpeed(pitchChannel, 127);
	auto ySpeed = calculateSpeed(rollChannel, 127);

	auto isMovingForward = calculateDirection(pitchChannel);	
	auto isMovingRight = calculateRotation(rollChannel);

	//Serial << isMovingForward << "," << isMovingRight << "\n";

	if (isMovingForward && isMovingRight)
	{
		auto rightSpeed = xSpeed + ySpeed;
		auto leftSpeed = xSpeed - ySpeed;

		rightForward(rightSpeed);
		leftForward(leftSpeed);

		//Serial << rightSpeed << "," << leftSpeed << "\n";

		return;
	}

	if (isMovingForward && !isMovingRight)
	{
		auto rightSpeed = xSpeed - ySpeed;
		auto leftSpeed = xSpeed + ySpeed;

		rightForward(rightSpeed);
		leftForward(leftSpeed);

		//Serial << rightSpeed << "," << leftSpeed << "\n";

		return;
	}

	if (!isMovingForward && isMovingRight)
	{
		auto rightSpeed = xSpeed + ySpeed;
		auto leftSpeed = xSpeed - ySpeed;

		rightBackward(xSpeed + ySpeed);
		leftBackward(xSpeed - ySpeed);

		//Serial << rightSpeed << "," << leftSpeed << "\n";

		return;
	}

	if (!isMovingForward && !isMovingRight)
	{
		auto rightSpeed = xSpeed - ySpeed;
		auto leftSpeed = xSpeed + ySpeed;

		rightBackward(rightSpeed);
		leftBackward(leftSpeed);

		//Serial << rightSpeed << "," << leftSpeed << "\n";
	}	
}
void moveStright(uint16_t pitchChannel)
{
	auto xSpeed = calculateSpeed(pitchChannel, 255);
	auto isMovingForward = calculateDirection(pitchChannel);
	if (isMovingForward)
	{
		rightForward(xSpeed);
		leftForward(xSpeed);
	}
	else
	{
		rightBackward(xSpeed);
		leftBackward(xSpeed);
	}
}
void moveCircle(uint16_t rollChannel)
{
	auto ySpeed = calculateSpeed(rollChannel, 255);
	auto isMovingRight = calculateRotation(rollChannel);

	//Serial << ySpeed << "," << isMoveRight << "\n";

	if (isMovingRight)
	{
		rightForward(ySpeed);
		leftBackward(ySpeed);
	}
	else
	{
		rightBackward(ySpeed);
		leftForward(ySpeed);		
	}
}
void standby()
{
	analogWrite(_rightEnginePin1, LOW);
	analogWrite(_rightEnginePin2, LOW);

	analogWrite(_leftEnginePin1, LOW);
	analogWrite(_leftEnginePin2, LOW);
}
void stop()
{
	analogWrite(_rightEnginePin1, HIGH);
	analogWrite(_rightEnginePin2, HIGH);

	analogWrite(_leftEnginePin1, HIGH);
	analogWrite(_leftEnginePin2, HIGH);
}
void rightForward(uint16_t speed)
{
	analogWrite(_rightEnginePin1, speed);
	analogWrite(_rightEnginePin2, LOW);
}
void rightBackward(uint16_t speed)
{
	analogWrite(_rightEnginePin1, LOW);
	analogWrite(_rightEnginePin2, speed);
}
void leftForward(uint16_t speed)
{
	analogWrite(_leftEnginePin1, speed);
	analogWrite(_leftEnginePin2, LOW);
}
void leftBackward(uint16_t speed)
{
	analogWrite(_leftEnginePin1, LOW);
	analogWrite(_leftEnginePin2, speed);
}


// SUPPORT FUNCTIONS //////////////////////////////////////////////////////////////////////////////
boolean isStickInTheCenter(uint16_t speedChannelValue)
{
	if (speedChannelValue > _stickCentralValue - _joystickActivityConstrain &&
		speedChannelValue < _stickCentralValue + _joystickActivityConstrain)
		return true;

	return false;
}
uint8_t calculateSpeed(uint16_t speedChannelValue, uint16_t maxValue)
{
	if (speedChannelValue > _stickCentralValue)
		return map(speedChannelValue, _stickCentralValue, _stickMaxValue, 0, maxValue);

	return map(speedChannelValue, _stickCentralValue, _stickMinValue, 0, maxValue);
}
boolean calculateDirection(uint16_t channelValue)
{
	if (channelValue <= _stickCentralValue)
		return 1;

	return 0;
}
boolean calculateRotation(uint16_t channelValue)
{
	if (channelValue <= _stickCentralValue)
		return 0;

	return 1;
}
uint16_t pullupChannel(uint16_t channelValue)
{
	if (channelValue > _stickMaxValue - _joystickPullUpThreshold)
		return _stickMaxValue;

	if (channelValue < _stickMinValue + _joystickPullUpThreshold)
		return _stickMinValue;

	return channelValue;
}
