#include "Streaming.h"
#include "Wire.h"

int bme280Address = 72;

void setup() 
{
	Serial.begin(9600);
	Wire.begin();
}
void loop()
{
	// Отправляем запрос
	// Начинаем общение с датчиком
	Wire.beginTransmission(bme280Address);
	// Запрашиваем нулевой регистр
	Wire.write(0);
	// Выполняем передачу
	Wire.endTransmission;
	// Запрашиваем 1 байт
	Wire.requestFrom(bme280Address, 1);
	// Ждем ответа
	while (Wire.available() == 0);
	// Получаем температуру с датчика
	int c = Wire.read();

	// Переводим температуру в фаренгейт
	int f = round(c * 9.0 / 5.0 + 32.0);

	// Выводим данные на экран
	/*Serial.print(c);
	Serial.print("C, ");
	Serial.print(f);
	Serial.println("F");*/

	Serial << c << "C, " << f << "F" << "\n;";

	delay(500);
}
