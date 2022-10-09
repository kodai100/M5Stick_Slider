#include "Fader.hpp"

Fader::Fader(uint8_t pin){
    m_pinNumber = pin;
}

Fader::~Fader(){

}

uint16_t Fader::getFaderValue(){
    return analogRead(m_pinNumber); // 0 - 4096 (返り値はuint16_tだが、最大値が4096であることに注意)
}