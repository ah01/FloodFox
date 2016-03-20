#include <Arduino.h>

#define LED_R 10
#define LED_G 11
#define LED_B 9

#define BTN_A A0
#define BTN_B A1

#define TEMP 3

#define RED 255,0,0
#define GREEN 0,255,0
#define BLUE 0,0,255
#define NONE 0,0,0
#define ORANGE 0xFF,0x90,0

void minishield_setup()
{    
    pinMode(LED_R, OUTPUT);
    pinMode(LED_G, OUTPUT);
    pinMode(LED_B, OUTPUT);

    pinMode(BTN_A, INPUT);
    digitalWrite(BTN_A, HIGH); // pull-up 
    
    pinMode(BTN_B, INPUT);
    digitalWrite(BTN_B, HIGH); // pull-up
}

void color(byte r, byte g, byte b)
{
    analogWrite(LED_R, r);
    analogWrite(LED_G, g);
    analogWrite(LED_B, b);
}

inline int btnA()
{
    return digitalRead(BTN_A) == HIGH ? LOW : HIGH;
}

inline int btnB()
{
    return digitalRead(BTN_B) == HIGH ? LOW : HIGH;
}

inline int rawTemp()
{
    return analogRead(TEMP);
}

float getTemp()
{
     return rawTemp() * 0.488281 - 273;
}
