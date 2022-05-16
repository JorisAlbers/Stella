#include <SPI.h>
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Arduino.h>

void sendBroadCast();
void getMacAdress(uint8_t *mac);

unsigned int localPort = 8888;
byte mac[6];
const int CONNECTION_REQUEST_PACKET_SIZE = 2* sizeof(int32_t) + 2* sizeof(byte) + sizeof(mac) ;
byte packetBuffer[CONNECTION_REQUEST_PACKET_SIZE];
EthernetUDP Udp;




void setup() {  
    
    // Open serial communications and wait for port to open:
    Serial.begin(9600);
    while (!Serial) {
        ; // wait for serial port to connect. Needed for native USB port only
    }
    getMacAdress((u_int8_t*)&mac);
        Serial.println("Starting");
    // start Ethernet and UDP
    if (Ethernet.begin(mac) == 0) {
        Serial.println("Failed to configure Ethernet using DHCP");
        // Check for Ethernet hardware present
        if (Ethernet.hardwareStatus() == EthernetNoHardware) {
        Serial.println("Ethernet shield was not found.  Sorry, can't run without hardware. :(");
        } else if (Ethernet.linkStatus() == LinkOFF) {
        Serial.println("Ethernet cable is not connected.");
        }
        // no point in carrying on, so do nothing forevermore:
        while (true) {
        delay(1);
        }
    }
    Udp.begin(localPort);
}

void loop() {
  sendBroadCast();
  // wait to see if a reply is available
  delay(5000); 
  Ethernet.maintain();
}

void sendBroadCast()
{
    byte key = 73;
    byte version = 1;
    int messageType = 4;
    memset(packetBuffer, 0, CONNECTION_REQUEST_PACKET_SIZE);
    memcpy(&packetBuffer, &CONNECTION_REQUEST_PACKET_SIZE, sizeof(int));
    memcpy(&packetBuffer[sizeof(int)], &messageType, sizeof(int));
    memcpy(&packetBuffer[sizeof(int) *2], &key, sizeof(byte));
    memcpy(&packetBuffer[sizeof(int) *2 + sizeof(byte)], &version, sizeof(byte));
    memcpy(&packetBuffer[sizeof(int) *2 + sizeof(byte) *2 ], &mac, sizeof(mac));

       
    // LENGTH KEY VERSION MAC

    IPAddress MyServer(255, 255, 255, 255);
    Udp.beginPacket(MyServer, 20055);
    Udp.write(packetBuffer, CONNECTION_REQUEST_PACKET_SIZE);
    Udp.endPacket();
    Serial.println("Send broadcast");
}

void getMacAdress(uint8_t *mac) {
    for(uint8_t by=0; by<2; by++) mac[by]=(HW_OCOTP_MAC1 >> ((1-by)*8)) & 0xFF;
    for(uint8_t by=0; by<4; by++) mac[by+2]=(HW_OCOTP_MAC0 >> ((3-by)*8)) & 0xFF;
    Serial.printf("MAC: %02x:%02x:%02x:%02x:%02x:%02x\n", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);
}

