
const int delayDuration = 2000;


void setup()
{
    Serial.begin(9600);
    Serial1.begin(115200);
}
void loop()
{
    auto ppmCO2 = readCO2();
    Serial1.println(ppmCO2);

    delay(delayDuration);
}


// FUNCTIONS //////////////////////////////////////////////////////////////////////////////////////
int readCO2()
{
    while (Serial.available() > 0) 
    {
        Serial.read();
    }
	
    byte cmd[9] = { 0xFF, 0x01, 0x86, 0x00, 0x00, 0x00, 0x00, 0x00, 0x79 };
	
    char response[9]; // for answer	
    Serial.write(cmd, 9); //request PPM CO2
	
    memset(response, 0, 9);
	
    Serial.readBytes(response, 9);
    int CRC = getCheckSum(response);
    if (response[0] != 0xFF || response[1] != 0x86 || CRC != response[8]) 
    {    	
        return -1;
    }
    auto responseHigh = (int)response[2];
    auto responseLow = (int)response[3];
	
    return (256 * responseHigh) + responseLow;
}
int getCheckSum(char* packet)
{
    byte checksum = 0;
    for (int i = 1; i < 8; i++) 
    {
        checksum += packet[i];
    }
    checksum = 0xff - checksum;
    checksum++;
	
    return checksum;
}