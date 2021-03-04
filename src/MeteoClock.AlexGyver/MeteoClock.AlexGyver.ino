/*
  Скетч к проекту "Домашняя метеостанция"
  Страница проекта (схемы, описания): https://alexgyver.ru/meteoclock/
  Исходники на GitHub: https://github.com/AlexGyver/MeteoClock
  Нравится, как написан и закомментирован код? Поддержи автора! https://alexgyver.ru/support_alex/
  Автор: AlexGyver Technologies, 2018
  http://AlexGyver.ru/
*/

/*
  Время и дата устанавливаются атвоматически при загрузке прошивки (такие как на компьютере)
  График всех величин за час и за сутки (усреднённые за каждый час)
  В модуле реального времени стоит батарейка, которая продолжает отсчёт времени после выключения/сброса питания
  Как настроить время на часах. У нас есть возможность автоматически установить время на время загрузки прошивки, поэтому:
    - Ставим настройку RESET_CLOCK на 1
  - Прошиваемся
  - Сразу ставим RESET_CLOCK 0
  - И прошиваемся ещё раз
  - Всё
*/

/* Версия 1.5
  - Добавлено управление яркостью
  - Яркость дисплея и светодиода СО2 меняется на максимальную и минимальную в зависимости от сигнала с фоторезистора
  Подключите датчик (фоторезистор) по схеме. Теперь на экране отладки справа на второй строчке появится величина сигнала
  с фоторезистора.
*/

// ------------------------- НАСТРОЙКИ --------------------
#define RESET_CLOCK 0           // сброс часов на время загрузки прошивки (для модуля с несъёмной батарейкой). Не забудь поставить 0 и прошить ещё раз!
#define SENS_TIME 10000         // sensor data acquisition time, ms
#define DRAW_SENS_TIME 30000    // время обновления показаний сенсоров на экране, миллисекунд
#define RESET_TO_MAIN_SCREEN_DELAY 30000

// управление яркостью
#define BRIGHT_THRESHOLD 150    // величина сигнала, ниже которой яркость переключится на минимум (0-1023)
#define LED_BRIGHT_MAX 255      // макс яркость светодиода СО2 (0 - 255)
#define LED_BRIGHT_MIN 10       // мин яркость светодиода СО2 (0 - 255)
#define LCD_BRIGHT_MAX 255      // макс яркость подсветки дисплея (0 - 255)
#define LCD_BRIGHT_MIN 0        // мин яркость подсветки дисплея (0 - 255)

#define DISP_MODE 1             // в правом верхнем углу отображать: 0 - год, 1 - день недели, 2 - секунды
#define WEEK_LANG 0             // язык дня недели: 0 - английский, 1 - русский (транслит)
#define DEBUG 0                 // вывод на дисплей лог инициализации датчиков при запуске. Дублируется через порт!
#define PRESSURE 1              // 0 - график давления, 1 - график прогноза дождя (вместо давления). Не забудь поправить пределы гроафика
#define DISPLAY_ADDR 0x27       // адрес платы дисплея: 0x27 или 0x3f. Если дисплей не работает - смени адрес! На самом дисплее адрес не указан

// CO2
#define MinCO2 1000             // green
#define NormalCO2 1500          // orange/blue
#define MaxCO2 2000             // red

// CO2 led color
#define COLOR_GREEN 2
#define COLOR_ORANGE 3
#define COLOR_RED 1
#define COLOR_OFF 0

// пределы отображения для графиков
#define TEMP_MIN 15
#define TEMP_MAX 35
#define HUM_MIN 0
#define HUM_MAX 100
#define PRESS_MIN -100
#define PRESS_MAX 100
#define CO2_MIN 500
#define CO2_MAX 3000


// адрес BME280 жёстко задан в файле библиотеки Adafruit_BME280.h
// стоковый адрес был 0x77, у китайского модуля адрес 0x76.
// Так что если юзаете НЕ библиотеку из архива - не забудьте поменять

// если дисплей не заводится - поменяйте адрес (строка 54)

// пины
#define BACKLIGHT 10
#define PHOTO A3

#define MHZ_RX 2
#define MHZ_TX 3

#define LED_COM 7
#define LED_R 9
#define LED_G 6
#define LED_B 5
#define BTN_PIN 4

#define BL_PIN 10     // пин подсветки дисплея
#define PHOTO_PIN 0   // пин фоторезистора

// библиотеки
#include <Wire.h>
#include <LiquidCrystal_I2C.h>
#include <Streaming.h>

LiquidCrystal_I2C lcd(DISPLAY_ADDR, 20, 4);

#include "RTClib.h"
RTC_DS3231 rtc;
DateTime now;

#include <Adafruit_Sensor.h>
#include <Adafruit_BME280.h>
#define SEALEVELPRESSURE_HPA (1013.25)
Adafruit_BME280 bme;

#include <MHZ19_uart.h>
MHZ19_uart mhz19;

#include <GyverTimer.h>
GTimer_ms sensorsTimer(SENS_TIME);
GTimer_ms drawSensorsTimer(DRAW_SENS_TIME);
GTimer_ms clockTimer(500);
GTimer_ms hourPlotTimer((long)4 * 60 * 1000);         // 4 минуты
GTimer_ms dayPlotTimer((long)1.6 * 60 * 60 * 1000);   // 1.6 часа
GTimer_ms plotTimer(240000);
GTimer_ms predictTimer((long)10 * 60 * 1000);         // 10 минут
GTimer_ms resetToMainScreenTimer(RESET_TO_MAIN_SCREEN_DELAY);
//GTimer_ms brightTimer(2000);

#include "GyverButton.h"
GButton button(BTN_PIN, LOW_PULL, NORM_OPEN);

/*
  0 часы и данные
  1 график температуры за час
  2 график температуры за сутки
  3 график влажности за час
  4 график влажности за сутки
  5 график давления за час
  6 график давления за сутки
  7 график углекислого за час
  8 график углекислого за сутки
*/
byte mode = 0;
int8_t hrs, mins, secs;

// переменные для вывода
float dispTemp;
byte dispHum;
int dispPres;
int dispCO2;
int dispRain;

// массивы графиков
int tempHour[15], tempDay[15];
int humHour[15], humDay[15];
int pressHour[15], pressDay[15];
int co2Hour[15], co2Day[15];
int delta;
uint32_t pressure_array[6];
uint32_t sumX, sumY, sumX2, sumXY;
float a, b;
byte time_array[6];


#if (WEEK_LANG == 0)
static const char* dayNames[] = {
  "SUN",
  "MON",
  "TUE",
  "WED",
  "THU",
  "FRY",
  "SAT",
};
#else
static const char* dayNames[] = {
  "BOCK",
  "POND",
  "BTOP",
  "CPED",
  "4ETB",
  "5YAT",
  "CYBB",
};
#endif

const byte ledMax = (255 - LED_BRIGHT_MAX);
const byte ledMin = (255 - LED_BRIGHT_MIN);
byte ledCurrent = ledMax;


// MAIN FUNCTIONS /////////////////////////////////////////////////////////////////////////////////
void setup()
{
    Serial.begin(9600);

    pinMode(BACKLIGHT, OUTPUT);
    pinMode(LED_COM, OUTPUT);
    pinMode(LED_R, OUTPUT);
    pinMode(LED_G, OUTPUT);
    pinMode(LED_B, OUTPUT);
    setLed(0);

    digitalWrite(LED_COM, 1);               // 1 for main anode, 0 main cathode
    analogWrite(BACKLIGHT, LCD_BRIGHT_MAX);

    lcd.init();
    lcd.backlight();
    lcd.clear();

#if (DEBUG == 1)
    drawDebug();
#else
    mhz19.begin(MHZ_TX, MHZ_RX);
    mhz19.setAutoCalibration(false);

    rtc.begin();
    bme.begin(&Wire);
#endif

    bme.setSampling(Adafruit_BME280::MODE_FORCED,
        Adafruit_BME280::SAMPLING_X1, // temperature
        Adafruit_BME280::SAMPLING_X1, // pressure
        Adafruit_BME280::SAMPLING_X1, // humidity
        Adafruit_BME280::FILTER_OFF);

    if (RESET_CLOCK || rtc.lostPower())
        rtc.adjust(DateTime(F(__DATE__), F(__TIME__)));

    now = rtc.now();
    secs = now.second();
    mins = now.minute();
    hrs = now.hour();

    bme.takeForcedMeasurement();
    uint32_t pressure = bme.readPressure();
    for (byte i = 0; i < 6; i++) 
    {   // счётчик от 0 до 5
        pressure_array[i] = pressure;  // забить весь массив текущим давлением
        time_array[i] = i;             // забить массив времени числами 0 - 5
    }

    loadClock();
    drawClock(hrs, mins, 0, 0, 1);
    drawData();
	
    readSensors();
    drawSensors();
}
void drawDebug()
{
    boolean status = true;

    setLed(1);
    
    lcd.setCursor(0, 0);
    lcd.print(F("MHZ-19... "));
    Serial.print(F("MHZ-19... "));
    mhz19.begin(MHZ_TX, MHZ_RX);
    mhz19.setAutoCalibration(false);
    mhz19.getStatus();    // первый запрос, в любом случае возвращает -1
    delay(500);
    if (mhz19.getStatus() == 0)
    {
        lcd.print(F("OK"));
        Serial.println(F("OK"));
    }
    else
    {
        lcd.print(F("ERROR"));
        Serial.println(F("ERROR"));
        status = false;
    }
	
    setLed(2);
	
    lcd.setCursor(0, 1);
    lcd.print(F("RTC... "));
    Serial.print(F("RTC... "));
    delay(50);
    if (rtc.begin()) {
        lcd.print(F("OK"));
        Serial.println(F("OK"));
    }
    else {
        lcd.print(F("ERROR"));
        Serial.println(F("ERROR"));
        status = false;
    }

    setLed(3);
    lcd.setCursor(0, 2);
    lcd.print(F("BME280... "));
    Serial.print(F("BME280... "));
    delay(50);
    if (bme.begin(&Wire)) {
        lcd.print(F("OK"));
        Serial.println(F("OK"));
    }
    else {
        lcd.print(F("ERROR"));
        Serial.println(F("ERROR"));
        status = false;
    }

    setLed(0);
    lcd.setCursor(0, 3);
    if (status) 
    {
        lcd.print(F("All good"));
        Serial.println(F("All good"));
    }
    else 
    {
        lcd.print(F("Check wires!"));
        Serial.println(F("Check wires!"));
    }
	
    while (1) 
    {
        lcd.setCursor(14, 1);
        lcd.print("P:    ");
        lcd.setCursor(16, 1);
        lcd.print(analogRead(PHOTO), 1);
        Serial.println(analogRead(PHOTO));
        delay(300);
    }
}


void loop()
{
    //if (brightTimer.isReady()) checkBrightness(); // яркость          // TODO: add photo resistor
    if (sensorsTimer.isReady()) readSensors();    // читаем показания датчиков с периодом SENS_TIME
    if (clockTimer.isReady()) clockTick();        // два раза в секунду пересчитываем время и мигаем точками

    plotSensorsTick();                            // тут внутри несколько таймеров для пересчёта графиков (за час, за день и прогноз)
    modesTick();                                  // тут ловим нажатия на кнопку и переключаем режимы
	
    if (mode == 0)
    {                                  // в режиме "главного экрана"
        if (drawSensorsTimer.isReady())
            drawSensors();  // обновляем показания датчиков на дисплее с периодом SENS_TIME
    }
    else
    {                                  // в любом из графиков
        if (plotTimer.isReady())
            redrawPlot();   // перерисовываем график
    }
}


// FUNCTIONS //////////////////////////////////////////////////////////////////////////////////////
void checkBrightness()
{
	const auto photoValue = analogRead(PHOTO);
	
    if (photoValue < BRIGHT_THRESHOLD)          // если темно
    {
        analogWrite(BACKLIGHT, LCD_BRIGHT_MIN);
        ledCurrent = ledMin;
    }
    else                                        // если светло
    {
        analogWrite(BACKLIGHT, LCD_BRIGHT_MAX);
        ledCurrent = ledMax;
    }
}
void readSensors()
{
    bme.takeForcedMeasurement();
    dispTemp = bme.readTemperature();
    dispHum = bme.readHumidity();
    dispPres = (float)bme.readPressure() * 0.00750062;    // 1 hectopascal [gPa] = 0,750063755419211 pressure in millimeters of mercury pillar (0°C) [mm mer.pill.]

	dispCO2 = mhz19.getPPM();    
}
void drawSensors()
{
    lcd.setCursor(0, 2);
    lcd.print(String(dispTemp, 1));
    lcd.write(223);
    lcd.setCursor(6, 2);
    lcd.print(" " + String(dispHum) + "%  ");

    lcd.print(String(dispCO2) + " ppm");
    if (dispCO2 < 1000) lcd.print(" ");

    lcd.setCursor(0, 3);
    lcd.print(String(dispPres) + " mm  rain ");
    lcd.print(F("       "));
    lcd.setCursor(13, 3);
    lcd.print(String(dispRain) + "%");   
	
    setCo2LedColor(dispCO2);
}
void setCo2LedColor(int ppm)
{
    if (ppm < MinCO2)
    {
    	setLed(COLOR_GREEN);        
        return;
    }

    if (ppm < NormalCO2)
    {
        setLed(COLOR_ORANGE);
        return;
    }

    if (ppm >= MaxCO2)
    {
        setLed(COLOR_RED);
        return;
    }

    setLed(COLOR_OFF);
}
void setLed(byte color)
{
    setCo2LedOff();
	
    switch (color)      // 0 выкл, 1 красный, 2 зелёный, 3 синий (или жёлтый)
    {
    case 0:
        break;
    case 1:
        analogWrite(LED_R, ledCurrent);
        break;
    case 2:
        analogWrite(LED_G, ledCurrent);
        break;
    case 3:
        //analogWrite(LED_B, LED_ON);          // blue       

        analogWrite(LED_R, map(ledCurrent, 0, 255, 128, 255));    // yellow, red is slightly brighter than green
        analogWrite(LED_G, ledCurrent);
    	
        break;
    }
}
void setCo2LedOff()
{
    analogWrite(LED_R, 255);
    analogWrite(LED_G, 255);
    analogWrite(LED_B, 255);    
}

boolean dotFlag;
boolean lightEnabled = true;

void clockTick()
{
    dotFlag = !dotFlag;
    if (dotFlag)
    {          // каждую секунду пересчёт времени
        secs++;
        if (secs > 59)
        {      // каждую минуту
            secs = 0;
            mins++;
            if (mins <= 59 && mode == 0)
                drawClock(hrs, mins, 0, 0, 1);
        }
        if (mins > 59)
        {      // каждый час
            now = rtc.now();
            secs = now.second();
            mins = now.minute();
            hrs = now.hour();

            if (mode == 0)
                drawClock(hrs, mins, 0, 0, 1);

            if (hrs > 23)
            {
                hrs = 0;
            }

            if (mode == 0)
                drawData();
        }
        if (DISP_MODE == 2 && mode == 0)
        {
            lcd.setCursor(16, 1);
            if (secs < 10)
                lcd.print(" ");

            lcd.print(secs);
        }
    }

    if (mode == 0)
        drawDots(7, 0, dotFlag);
}
void plotSensorsTick()
{
    // 4 минутный таймер
    if (hourPlotTimer.isReady()) 
    {
        for (byte i = 0; i < 14; i++) {
            tempHour[i] = tempHour[i + 1];
            humHour[i] = humHour[i + 1];
            pressHour[i] = pressHour[i + 1];
            co2Hour[i] = co2Hour[i + 1];
        }
        tempHour[14] = dispTemp;
        humHour[14] = dispHum;
        co2Hour[14] = dispCO2;

        if (PRESSURE) pressHour[14] = dispRain;
        else pressHour[14] = dispPres;
    }

    // 1.5 часовой таймер
    if (dayPlotTimer.isReady()) 
    {
        long averTemp = 0, averHum = 0, averPress = 0, averCO2 = 0;

        for (byte i = 0; i < 15; i++) {
            averTemp += tempHour[i];
            averHum += humHour[i];
            averPress += pressHour[i];
            averCO2 += co2Hour[i];
        }
        averTemp /= 15;
        averHum /= 15;
        averPress /= 15;
        averCO2 /= 15;

        for (byte i = 0; i < 14; i++) {
            tempDay[i] = tempDay[i + 1];
            humDay[i] = humDay[i + 1];
            pressDay[i] = pressDay[i + 1];
            co2Day[i] = co2Day[i + 1];
        }
        tempDay[14] = averTemp;
        humDay[14] = averHum;
        pressDay[14] = averPress;
        co2Day[14] = averCO2;
    }

    // 10 минутный таймер
    if (predictTimer.isReady())
    {
        // тут делаем линейную аппроксимацию для предсказания погоды
        long averPress = 0;
        for (byte i = 0; i < 10; i++) {
            bme.takeForcedMeasurement();
            averPress += bme.readPressure();
            delay(1);
        }
        averPress /= 10;

        for (byte i = 0; i < 5; i++) {                   // счётчик от 0 до 5 (да, до 5. Так как 4 меньше 5)
            pressure_array[i] = pressure_array[i + 1];     // сдвинуть массив давлений КРОМЕ ПОСЛЕДНЕЙ ЯЧЕЙКИ на шаг назад
        }
        pressure_array[5] = averPress;                    // последний элемент массива теперь - новое давление
        sumX = 0;
        sumY = 0;
        sumX2 = 0;
        sumXY = 0;
        for (int i = 0; i < 6; i++) {                    // для всех элементов массива
            sumX += time_array[i];
            sumY += (long)pressure_array[i];
            sumX2 += time_array[i] * time_array[i];
            sumXY += (long)time_array[i] * pressure_array[i];
        }
        a = 0;
        a = (long)6 * sumXY;             // расчёт коэффициента наклона приямой
        a = a - (long)sumX * sumY;
        a = (float)a / (6 * sumX2 - sumX * sumX);
        delta = a * 6;      // расчёт изменения давления
        dispRain = map(delta, -250, 250, 100, -100);  // пересчитать в проценты
        //Serial.println(String(pressure_array[5]) + " " + String(delta) + " " + String(dispRain));   // дебаг
    }
}
void modesTick()
{
    button.tick();
    auto changeFlag = false;

    if (button.isClick())
    {
        mode++;
        if (mode > 8) mode = 0;

        resetToMainScreenTimer.reset();
        changeFlag = true;
    }
    if (button.isHolded())
    {
    	lightEnabled = !lightEnabled;
        if(lightEnabled)
        {
            analogWrite(BACKLIGHT, LCD_BRIGHT_MAX);
            //ledCurrent = ledMax;
            //setCo2LedColor(dispCO2);
        }
        else
        {        	
            analogWrite(BACKLIGHT, LCD_BRIGHT_MIN);
            //ledCurrent = ledMin;
            //setCo2LedColor(dispCO2);
        }        
    }
    if(resetToMainScreenTimer.isReady())
    {
    	mode = 0;             // return to main screen
    	changeFlag = true;
    }

    if (changeFlag)
    {
        if (mode == 0)
        {
            lcd.clear();
            loadClock();
            drawClock(hrs, mins, 0, 0, 1);

            drawData();
            drawSensors();
        }
        else
        {
            lcd.clear();
            loadPlot();
            redrawPlot();
        }
    }
}
void redrawPlot()
{
    lcd.clear();
    switch (mode)
    {
    case 1: drawPlot(0, 3, 15, 4, TEMP_MIN, TEMP_MAX, (int*)tempHour, "t hr");
        break;
    case 2: drawPlot(0, 3, 15, 4, TEMP_MIN, TEMP_MAX, (int*)tempDay, "t day");
        break;
    case 3: drawPlot(0, 3, 15, 4, HUM_MIN, HUM_MAX, (int*)humHour, "h hr");
        break;
    case 4: drawPlot(0, 3, 15, 4, HUM_MIN, HUM_MAX, (int*)humDay, "h day");
        break;
    case 5: drawPlot(0, 3, 15, 4, PRESS_MIN, PRESS_MAX, (int*)pressHour, "p hr");
        break;
    case 6: drawPlot(0, 3, 15, 4, PRESS_MIN, PRESS_MAX, (int*)pressDay, "p day");
        break;
    case 7: drawPlot(0, 3, 15, 4, CO2_MIN, CO2_MAX, (int*)co2Hour, "c hr");
        break;
    case 8: drawPlot(0, 3, 15, 4, CO2_MIN, CO2_MAX, (int*)co2Day, "c day");
        break;
    }
}
void drawPlot(byte pos, byte row, byte width, byte height, int min_val, int max_val, int* plot_array, String label)
{
    int max_value = -32000;
    int min_value = 32000;

    for (byte i = 0; i < 15; i++)
    {
        if (plot_array[i] > max_value) max_value = plot_array[i];
        if (plot_array[i] < min_value) min_value = plot_array[i];
    }
    lcd.setCursor(16, 0); lcd.print(max_value);
    lcd.setCursor(16, 1); lcd.print(label);
    lcd.setCursor(16, 2); lcd.print(plot_array[14]);
    lcd.setCursor(16, 3); lcd.print(min_value);

    for (byte i = 0; i < width; i++)
    {                  // каждый столбец параметров
        int fill_val = plot_array[i];
        fill_val = constrain(fill_val, min_val, max_val);
        byte infill, fract;
        // найти количество целых блоков с учётом минимума и максимума для отображения на графике
        if (plot_array[i] > min_val)
            infill = floor((float)(plot_array[i] - min_val) / (max_val - min_val) * height * 10);
        else infill = 0;
        fract = (float)(infill % 10) * 8 / 10;                   // найти количество оставшихся полосок
        infill = infill / 10;

        for (byte n = 0; n < height; n++) {     // для всех строк графика
            if (n < infill && infill > 0) {       // пока мы ниже уровня
                lcd.setCursor(i, (row - n));        // заполняем полными ячейками
                lcd.write(0);
            }
            if (n >= infill) {                    // если достигли уровня
                lcd.setCursor(i, (row - n));
                if (fract > 0) lcd.write(fract);          // заполняем дробные ячейки
                else lcd.write(16);                       // если дробные == 0, заливаем пустой
                for (byte k = n + 1; k < height; k++) {   // всё что сверху заливаем пустыми
                    lcd.setCursor(i, (row - k));
                    lcd.write(16);
                }
                break;
            }
        }
    }
}

void drawClock(byte hours, byte minutes, byte x, byte y, boolean dotState)
{
    // clean
    lcd.setCursor(x, y);
    lcd.print("               ");
    lcd.setCursor(x, y + 1);
    lcd.print("               ");

    //if (hours > 23 || minutes > 59) return;
    if (hours / 10 == 0) drawDig(10, x, y);
    else drawDig(hours / 10, x, y);
    drawDig(hours % 10, x + 4, y);
	
    // тут должны быть точки. Отдельной функцией
    drawDig(minutes / 10, x + 8, y);
    drawDig(minutes % 10, x + 12, y);
}
void drawDig(byte dig, byte x, byte y)
{
    switch (dig) {
    case 0:
        lcd.setCursor(x, y); // set cursor to column 0, line 0 (first row)
        lcd.write(0);  // call each segment to create
        lcd.write(1);  // top half of the number
        lcd.write(2);
        lcd.setCursor(x, y + 1); // set cursor to colum 0, line 1 (second row)
        lcd.write(3);  // call each segment to create
        lcd.write(4);  // bottom half of the number
        lcd.write(5);
        break;
    case 1:
        lcd.setCursor(x + 1, y);
        lcd.write(1);
        lcd.write(2);
        lcd.setCursor(x + 2, y + 1);
        lcd.write(5);
        break;
    case 2:
        lcd.setCursor(x, y);
        lcd.write(6);
        lcd.write(6);
        lcd.write(2);
        lcd.setCursor(x, y + 1);
        lcd.write(3);
        lcd.write(7);
        lcd.write(7);
        break;
    case 3:
        lcd.setCursor(x, y);
        lcd.write(6);
        lcd.write(6);
        lcd.write(2);
        lcd.setCursor(x, y + 1);
        lcd.write(7);
        lcd.write(7);
        lcd.write(5);
        break;
    case 4:
        lcd.setCursor(x, y);
        lcd.write(3);
        lcd.write(4);
        lcd.write(2);
        lcd.setCursor(x + 2, y + 1);
        lcd.write(5);
        break;
    case 5:
        lcd.setCursor(x, y);
        lcd.write(0);
        lcd.write(6);
        lcd.write(6);
        lcd.setCursor(x, y + 1);
        lcd.write(7);
        lcd.write(7);
        lcd.write(5);
        break;
    case 6:
        lcd.setCursor(x, y);
        lcd.write(0);
        lcd.write(6);
        lcd.write(6);
        lcd.setCursor(x, y + 1);
        lcd.write(3);
        lcd.write(7);
        lcd.write(5);
        break;
    case 7:
        lcd.setCursor(x, y);
        lcd.write(1);
        lcd.write(1);
        lcd.write(2);
        lcd.setCursor(x + 1, y + 1);
        lcd.write(0);
        break;
    case 8:
        lcd.setCursor(x, y);
        lcd.write(0);
        lcd.write(6);
        lcd.write(2);
        lcd.setCursor(x, y + 1);
        lcd.write(3);
        lcd.write(7);
        lcd.write(5);
        break;
    case 9:
        lcd.setCursor(x, y);
        lcd.write(0);
        lcd.write(6);
        lcd.write(2);
        lcd.setCursor(x + 1, y + 1);
        lcd.write(4);
        lcd.write(5);
        break;
    case 10:
        lcd.setCursor(x, y);
        lcd.write(32);
        lcd.write(32);
        lcd.write(32);
        lcd.setCursor(x, y + 1);
        lcd.write(32);
        lcd.write(32);
        lcd.write(32);
        break;
    }
}

void drawData()
{
    lcd.setCursor(15, 0);
    if (now.day() < 10) lcd.print(0);
    lcd.print(now.day());
    lcd.print(".");
    if (now.month() < 10) lcd.print(0);
    lcd.print(now.month());

    if (DISP_MODE == 0)
    {
        lcd.setCursor(16, 1);
        lcd.print(now.year());
    }
    else if (DISP_MODE == 1)
    {
        lcd.setCursor(16, 1);
        int dayofweek = now.dayOfTheWeek();
        lcd.print(dayNames[dayofweek]);
    }
}
void drawDots(byte x, byte y, boolean state)
{
    byte code;
    if (state)
        code = 165;
    else
        code = 32;

    lcd.setCursor(x, y);
    lcd.write(code);
    lcd.setCursor(x, y + 1);
    lcd.write(code);
}