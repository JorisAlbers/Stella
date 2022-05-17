#include <SPI.h>
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Arduino.h>
//#include "message.h"
#include "connection.h"

unsigned int localPort = 20050;
unsigned int server_broadcast_port = 20055;

const int CONNECTION_REQUEST_PACKET_SIZE = 2* sizeof(int32_t) + 2* sizeof(byte) + 6 * sizeof(byte);
byte packetBuffer[60000]; // TODO smaller?

connection con(localPort,server_broadcast_port);

void sendConnectionRequest();

void setup() {  
    
    // Open serial communications and wait for port to open:
    Serial.begin(9600);
    while (!Serial) {
        ; // wait for serial port to connect. Needed for native USB port only
    }

    Serial.println("-- Stella client teensy --");
    
    if(!con.open())
    {
        // We cant continue
        Serial.println("Failed to connection to internet. Restart to continue");
        while(true)
        {
            delay(1);
        }
    };
}

void loop() {   
    Serial.println("Sending connection request");
    sendConnectionRequest();
    delay(2000); 
    con.maintain();    
}

/* void readHeader()
{
    int bytesRead = 0;
    if (Udp.readBytes(packetBuffer,)) {
        // We've received a packet, read the data from it
        bytesRead = Udp.read(packetBuffer, UDP_BUFFER_SIZE); // read the packet into the buffer
        Serial.printf("Received %d bytes\n",bytesRead);
    }
} */

void sendConnectionRequest()
{
    byte key = 73;
    byte version = 1;
    int messageType = 4;
    memset(packetBuffer, 0, CONNECTION_REQUEST_PACKET_SIZE);
    memcpy(&packetBuffer, &CONNECTION_REQUEST_PACKET_SIZE, sizeof(int));
    memcpy(&packetBuffer[sizeof(int)], &messageType, sizeof(int));
    memcpy(&packetBuffer[sizeof(int) *2], &key, sizeof(byte));
    memcpy(&packetBuffer[sizeof(int) *2 + sizeof(byte)], &version, sizeof(byte));
    memcpy(&packetBuffer[sizeof(int) *2 + sizeof(byte) *2 ], &con.m_mac, sizeof(con.m_mac));
       
    // LENGTH KEY VERSION MAC

    con.Broadcast(packetBuffer, CONNECTION_REQUEST_PACKET_SIZE);
}



