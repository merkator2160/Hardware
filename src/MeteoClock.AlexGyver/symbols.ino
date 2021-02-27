// Symbols //
// Chart
byte row8[8] = { 0b11111,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111 };
byte row7[8] = { 0b00000,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111 };
byte row6[8] = { 0b00000,  0b00000,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111 };
byte row5[8] = { 0b00000,  0b00000,  0b00000,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111 };
byte row4[8] = { 0b00000,  0b00000,  0b00000,  0b00000,  0b11111,  0b11111,  0b11111,  0b11111 };
byte row3[8] = { 0b00000,  0b00000,  0b00000,  0b00000,  0b00000,  0b11111,  0b11111,  0b11111 };
byte row2[8] = { 0b00000,  0b00000,  0b00000,  0b00000,  0b00000,  0b00000,  0b11111,  0b11111 };
byte row1[8] = { 0b00000,  0b00000,  0b00000,  0b00000,  0b00000,  0b00000,  0b00000,  0b11111 };


// Digits
uint8_t LT[8] = { 0b00111,  0b01111,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111 };
uint8_t UB[8] = { 0b11111,  0b11111,  0b11111,  0b00000,  0b00000,  0b00000,  0b00000,  0b00000 };
uint8_t RT[8] = { 0b11100,  0b11110,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111 };
uint8_t LL[8] = { 0b11111,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111,  0b01111,  0b00111 };
uint8_t LB[8] = { 0b00000,  0b00000,  0b00000,  0b00000,  0b00000,  0b11111,  0b11111,  0b11111 };
uint8_t LR[8] = { 0b11111,  0b11111,  0b11111,  0b11111,  0b11111,  0b11111,  0b11110,  0b11100 };
uint8_t UMB[8] = { 0b11111,  0b11111,  0b11111,  0b00000,  0b00000,  0b00000,  0b11111,  0b11111 };
uint8_t LMB[8] = { 0b11111,  0b00000,  0b00000,  0b00000,  0b00000,  0b11111,  0b11111,  0b11111 };


// FUNCTIONS //////////////////////////////////////////////////////////////////////////////////////
void loadClock()
{
    lcd.createChar(0, LT);
    lcd.createChar(1, UB);
    lcd.createChar(2, RT);
    lcd.createChar(3, LL);
    lcd.createChar(4, LB);
    lcd.createChar(5, LR);
    lcd.createChar(6, UMB);
    lcd.createChar(7, LMB);
}
void loadPlot()
{
    lcd.createChar(0, row8);
    lcd.createChar(1, row1);
    lcd.createChar(2, row2);
    lcd.createChar(3, row3);
    lcd.createChar(4, row4);
    lcd.createChar(5, row5);
    lcd.createChar(6, row6);
    lcd.createChar(7, row7);
}