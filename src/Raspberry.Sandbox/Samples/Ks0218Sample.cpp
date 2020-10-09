#include <wiringPi.h>
#include <stdio.h>
#include <string.h>
#include <errno.h>
#include <wiringSerial.h>

#define PIN 27	
#define IO digitalRead(PIN)

unsigned char i, idx, cnt;
unsigned char count;
unsigned char data[4];
char a;
long  up = 0x40;
long  back = 0x15;
long  stop = 0x46;
long  left = 0x44;
long  right = 0x43;
long  esc = 0x4a;


int main()
{
    wiringPiSetup();
    pinMode(PIN, INPUT);
    pullUpDnControl(PIN, PUD_UP);
    pinMode(1, OUTPUT);
    int fd;

    pinMode(22, OUTPUT);
    pinMode(26, OUTPUT);
    pinMode(21, OUTPUT);
    pinMode(30, OUTPUT);
    printf("L298P test......\n");
    for (a = 0; a < 2; a++)
    {
        digitalWrite(21, HIGH);
        digitalWrite(26, HIGH);
        digitalWrite(22, LOW);
        digitalWrite(30, LOW);
        delay(1000);
        digitalWrite(22, HIGH);
        digitalWrite(30, HIGH);
        delay(1000);
        digitalWrite(21, LOW);
        digitalWrite(26, LOW);
        delay(1000);
        digitalWrite(22, LOW);
        digitalWrite(30, LOW);
        delay(1000);
    }
    printf("IRM Test Program ... \n");

    while (data[2] != esc)
    {
        if (IO == 0)
        {
            count = 0;
            while (IO == 0 && count++ < 200)   //9ms
                delayMicroseconds(60);

            count = 0;
            while (IO == 1 && count++ < 80)	  //4.5ms
                delayMicroseconds(60);

            idx = 0;
            cnt = 0;
            data[0] = 0;
            data[1] = 0;
            data[2] = 0;
            data[3] = 0;
            for (i = 0; i < 32; i++)
            {
                count = 0;
                while (IO == 0 && count++ < 15)  //0.56ms
                    delayMicroseconds(60);

                count = 0;
                while (IO == 1 && count++ < 40)  //0: 0.56ms; 1: 1.69ms
                    delayMicroseconds(60);

                if (count > 25)data[idx] |= (1 << cnt);
                if (cnt == 7)
                {
                    cnt = 0;
                    idx++;
                }
                else cnt++;
            }

            if (data[0] + data[1] == 0xFF && data[2] + data[3] == 0xFF)	//check	
                printf("Get the key: 0x%02x\n", data[2]);
            if (data[2] == up)
            {
                digitalWrite(21, HIGH);
                digitalWrite(26, HIGH);
                digitalWrite(22, LOW);
                digitalWrite(30, LOW);
                delay(1000);
            }
            if (data[2] == back)
            {
                digitalWrite(21, LOW);
                digitalWrite(26, LOW);
                digitalWrite(22, HIGH);
                digitalWrite(30, HIGH);
                delay(1000);
            }
            if (data[2] == stop)
            {
                digitalWrite(21, HIGH);
                digitalWrite(26, HIGH);
                digitalWrite(22, HIGH);
                digitalWrite(30, HIGH);
                delay(1000);
            }
            if (data[2] == left)
            {
                digitalWrite(21, HIGH);
                digitalWrite(26, LOW);
                digitalWrite(22, LOW);
                digitalWrite(30, HIGH);
                delay(1000);
            }
            if (data[2] == right)
            {
                digitalWrite(21, LOW);
                digitalWrite(26, HIGH);
                digitalWrite(22, HIGH);
                digitalWrite(30, LOW);
                delay(1000);
            }
        }
    }
    printf("Bluetooth Test Program ... \n");
    if ((fd = serialOpen("/dev/ttyAMA0", 9600)) < 0)
    {
        fprintf(stderr, "Unable to open serial device: %s\n", strerror(errno));
        return 1;
    }

    // Loop, getting and printing characters
    while (1)
    {
        putchar(serialGetchar(fd));
        fflush(stdout);
    }
}