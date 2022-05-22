#include <SPI.h>
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Arduino.h>
#include "connection.h"
#include <TimeLib.h>

const long ONE_MINUTE = 60;
unsigned int localPort = 20050;
unsigned int server_broadcast_port = 20055;

const int CONNECTION_REQUEST_PACKET_SIZE = 2* sizeof(int32_t) + 2* sizeof(byte) + 6 * sizeof(byte);
byte packetBuffer[60000]; // TODO smaller?

connection con(localPort,server_broadcast_port);

bool initialized = false;
time_t lastMessageReceived;

void sendConnectionRequest();
void parsePackage(net::message* message);
void initializeLeds(uint32_t pixels, uint8_t brightness);

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
    bool packageReceived = con.ReadData();
    if(packageReceived)
    {
        // TODO make sure it is from the server
        lastMessageReceived = now();
        net::message* message = con.m_message_cache.getForReading();
        Serial.printf("Received message of type %d\n", message->header.type);
        parsePackage(message);  

        con.m_message_cache.resetMessage(message);
    }
    
    if(!initialized)
    {
        Serial.println("Attempting to register at the server.");
        sendConnectionRequest();
        delay(1000);
    }
    else if(now() - lastMessageReceived > ONE_MINUTE)
    {
        Serial.println("Did not get a message in a long time. Broadcasting our info again.");
        sendConnectionRequest();
        lastMessageReceived = now();
    }

    con.maintain();    
}

void parsePackage(net::message* message)
{
    switch (message->header.type)
        {
            case net::message_type::Init:                
                net::init_message* init_message = reinterpret_cast<net::init_message*>(&message->body);
                Serial.printf("INIT received: pixels = %d, brightness = %d\n", init_message->pixels, init_message->brightness);
                initializeLeds(init_message->pixels, init_message->brightness);
                break;
            
            default:
                break;
        }

}

void initializeLeds(uint32_t pixels, uint8_t brightness)
{
    initialized = true;
    // TODO, octolib?
}



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



