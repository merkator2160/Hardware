#include <MHZ19.h>
#include <Streaming.h>

const int rx_pin = 13;
const int tx_pin = 15;

const int pwmpin = 14;

auto mhz19_uart = MHZ19(rx_pin, tx_pin);
auto mhz19_pwm = MHZ19(pwmpin);


void setup()
{
    Serial.begin(115200);
	
    mhz19_uart.begin(rx_pin, tx_pin);
    mhz19_uart.setAutoCalibration(true);
	
    delay(3000);
	
    Serial << "MH-Z19 now warming up...  status:" << mhz19_uart.getStatus() << "\n";
	
    delay(1000);
}
void loop()
{
    auto m = mhz19_uart.getMeasurement();
    auto co2pwm = mhz19_pwm.getPpmPwm();
    //auto co2ppm = map(co2pwm, 400, 2000, 0, 5000);      // map(value, fromLow, fromHigh, toLow, toHigh)
	
    Serial << "CO2: " << m.co2_ppm << ", co2 pwm: " << co2pwm << ", temp: " << m.temperature << "\n";

    delay(5000);
}