#include <SPI.h>
#include <MFRC522.h>

#define SS_PIN 10
#define RST_PIN 9
#define R_LED 5
#define G_LED 4
#define B_LED 3
#define BUZ_PIN 6

MFRC522 reader(SS_PIN, RST_PIN);

void setup() {
  pinMode(BUZ_PIN, OUTPUT);

  Serial.begin(9600);
  SPI.begin();
  reader.PCD_Init();
}

void loop() {  
  read_input();

  if(!reader.PICC_IsNewCardPresent() || !reader.PICC_ReadCardSerial())
    return;

  char buf[10] = "";
  bytes_to_string(reader.uid.uidByte, 4, buf);
  Serial.println(buf);

  reader.PICC_HaltA();
}

void read_input() {
  if(Serial.available() > 0) {
    byte input = Serial.read();

    switch(input) {
      case '0':
        RGB_color(255,0,0);
        tone(BUZ_PIN,1000,500);
        delay(500);
        RGB_color(0,0,0);
        break;
      case '1':
        RGB_color(0,255,0);
        tone(BUZ_PIN,1000,500);
        delay(500);
        RGB_color(0,0,0);
        break;
      case 'c':
        Serial.println("STELLA_READER");
        break;
    }
  }
}

void bytes_to_string(byte array[], unsigned int len, char buffer[]) {
  for (unsigned int i = 0; i < len; i++) {
    byte nib1 = (array[i] >> 4) & 0x0F;
    byte nib2 = (array[i] >> 0) & 0x0F;
    buffer[i*2+0] = nib1 < 0xA ? '0' + nib1 : 'A' + nib1 - 0xA;
    buffer[i*2+1] = nib2 < 0xA ? '0' + nib1 : 'A' + nib1 - 0xA;
  }
  buffer[len*2] = '\0';
}

void RGB_color(int red, int green, int blue) {
  analogWrite(R_LED, red);
  analogWrite(G_LED, green);
  analogWrite(B_LED, blue);
}
