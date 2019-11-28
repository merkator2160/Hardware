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
	// ���������� ������
	// �������� ������� � ��������
	Wire.beginTransmission(bme280Address);
	// ����������� ������� �������
	Wire.write(0);
	// ��������� ��������
	Wire.endTransmission;
	// ����������� 1 ����
	Wire.requestFrom(bme280Address, 1);
	// ���� ������
	while (Wire.available() == 0);
	// �������� ����������� � �������
	int c = Wire.read();

	// ��������� ����������� � ���������
	int f = round(c * 9.0 / 5.0 + 32.0);

	// ������� ������ �� �����
	/*Serial.print(c);
	Serial.print("C, ");
	Serial.print(f);
	Serial.println("F");*/

	Serial << c << "C, " << f << "F" << "\n;";

	delay(500);
}
