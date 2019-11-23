#include <stdio.h>
#include <DS1302.h>

String DayAsString(Time::Day day);
String GetValue(String data, char separator, int index);

void HandleCommand(String commandStr)
{	
	//Serial.println(GetValue(commandStr, ':', 0).toInt());

	int command = GetValue(commandStr, ':', 0).toInt();
	switch ((Command)command)
	{
		case Command::GetTime:
			SendTime();
			break;
		case Command::SetTime: 
			SetClockTime(commandStr);
			break;
		default:
			Serial.println("Unknown command");
			break;
	}
}


// COMMANDS ///////////////////////////////////////////////////////////////////////////////////////
void SendTime()
{
	Time time = rtc.time();
	String day = DayAsString(time.day);

	// Format the time and date and insert into the temporary buffer.
	char buf[50];
	snprintf(buf, sizeof(buf), "%s %04d-%02d-%02d %02d:%02d:%02d", day.c_str(), time.yr, time.mon, time.date, time.hr, time.min, time.sec);

	Serial.println(buf);
}
void SetClockTime(String commandStr)
{
	uint16_t year = GetValue(commandStr, ':', 1).toInt();
	uint8_t mounth = GetValue(commandStr, ':', 2).toInt();
	uint8_t date = GetValue(commandStr, ':', 3).toInt();
	uint8_t hour = GetValue(commandStr, ':', 4).toInt();
	uint8_t min = GetValue(commandStr, ':', 5).toInt();
	uint8_t sec = GetValue(commandStr, ':', 6).toInt();
	Time::Day dayOfWeek = (Time::Day)(GetValue(commandStr, ':', 6).toInt());

	Time timeToSet(year, mounth, date, hour, min, sec, dayOfWeek);
	rtc.time(timeToSet);
}