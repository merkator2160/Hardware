#include <Wire.h> 
#include <LiquidCrystal_I2C.h>
#include <DS1302.h>

LiquidCrystal_I2C lcd(0x27, 16, 2);		// For screen 16х2


void ScreenInit()
{
	lcd.begin();
	lcd.backlight();
}
void PrintTime(Time time)
{
	
	lcd.clear();

	lcd.setCursor(0, 0);
	String day = DayAsString(time.day);

	char buf[50];
	snprintf(buf, sizeof(buf), "%s %04d-%02d-%02d", day.c_str(), time.yr, time.mon, time.date);
	lcd.print(buf);

	lcd.setCursor(0, 1);
	snprintf(buf, sizeof(buf), "%02d:%02d:%02d", time.hr, time.min, time.sec);
	lcd.print(buf);
}