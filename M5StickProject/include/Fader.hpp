#pragma once
#include <M5StickCPlus.h>

class Fader {

public:
    Fader(uint8_t pin);
    ~Fader();

    uint16_t getFaderValue();

private:
    uint8_t m_pinNumber;

};