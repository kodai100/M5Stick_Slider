#include <M5StickCPlus.h>
#include <WiFi.h>
#include <WiFiUdp.h>
#include <WiFiMulti.h>

#include "Fader.hpp"

#define INPUT_PINS 33

//接続先wifi
const char* ssid01 = "****";
const char* pass01 = "*****";

const char* serverAddress = "*****"; //送り先IPアドレス
const int serverPort = 20100;  //送り先のポート番号

const int myPort = 22224;  //このデバイスのポート番号

WiFiMulti wifiMulti;
WiFiUDP wifiUdp;

Fader myFader1(33);

void setup() {
    M5.begin();
    Serial.begin(9600);
   
    M5.Lcd.setRotation(3);        // Rotate the screen.

    WiFi.disconnect(true, true);  //wifi初期化
    wifiMulti.addAP(ssid01, pass01);

    wifiMulti.addAP("wifi1", "213123"); //add wifi
    M5.Lcd.println("Hello!"); 
    M5.Lcd.println("Connecting Wifi..."); 

    while ( wifiMulti.run() != WL_CONNECTED) {
      delay(100);
    }

    M5.lcd.print("WiFi connected\n\nSSID:");
    M5.lcd.println(WiFi.SSID());  // Output Network name.  输出网络名称
    M5.lcd.print("IP address: ");
    M5.lcd.println(WiFi.localIP());
    M5.lcd.print("Send port: ");
    M5.lcd.println(serverPort);

    wifiUdp.begin(myPort);
    delay(500);
}

uint16_t sendUDP(){
  wifiUdp.beginPacket(serverAddress, serverPort); //UDPパケットの開始

  auto faderValue = myFader1.getFaderValue();
  uint8_t arr[sizeof(faderValue)];
  memcpy(arr, &faderValue, sizeof(faderValue));

  wifiUdp.write(arr, sizeof(arr));
  
  wifiUdp.endPacket();

  return faderValue;
}

void loop() {

    if (wifiMulti.run() == WL_CONNECTED) {  // If the connection to wifi is established

        M5.lcd.setCursor(0, 60);

        M5.lcd.print("RSSI: ");
        M5.lcd.println(WiFi.RSSI());  // Output signal strength.  输出信号强度
        
        M5.lcd.print("FADER value: ");
        M5.lcd.println(sendUDP());
        
        delay(1000/60.0); // 60fps
        
        M5.lcd.fillRect(0, 60, 180, 300, BLACK);  // It's equivalent to partial screen clearance.
    } else {
        // If the connection to wifi is not established successfully.
        M5.lcd.print(".");
        delay(1000);
    }

}