#include <SPI.h>
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Arduino.h>
#include "connection.h"
#include "frame.h"
#include "frame_buffer.h"
#include <TimeLib.h>
#include "LedController.h"

const long ONE_MINUTE = 60;
const long TEN_SECONDS = 10;
const long FIVE_SECONDS = 10;
unsigned int localPort = 20050;
unsigned int server_broadcast_port = 20055;

const int CONNECTION_REQUEST_PACKET_SIZE = 2* sizeof(int32_t) + 2* sizeof(byte) + 6 * sizeof(byte);
byte packetBuffer[60000]; // TODO smaller?

connection con(localPort,server_broadcast_port);
LedController* ledcontroller;


bool initialized = false;
time_t lastMessageReceived;

void sendConnectionRequest();
void parsePackage(net::message* message);
void initializeLeds(uint32_t pixels, uint8_t brightness);
void displayFrame(net::frame_header*, net::message*);
void displayFrame(frame_buffer*);

void setup() {  
    
    // Open serial communications and wait for port to open:
    Serial.begin(9600);
    while (!Serial) {
        ; // wait for serial port to connect. Needed for native USB port only
    }

    

    Serial.println("-- Stella client teensy --");

    if ( Serial && CrashReport ) 
    { // Make sure Serial is alive and there is a CrashReport stored.
        Serial.println("Detected crash at previous run. Error:");
        Serial.print(CrashReport); // Once called any crash data is cleared
        // In this case USB Serial is used - but any Stream capable output will work : SD Card or other UART Serial
    }
    
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
    time_t t0 = now();
    bool packageReceived = con.ReadData();

    if(packageReceived)
    {
        // TODO make sure it is from the server
        lastMessageReceived = t0;
        net::message* message = con.m_message_cache.currentBuffer();
        Serial.printf("Received message of type %d\n", message->header.type);
        parsePackage(message);        
    }
    


    if(initialized)
    {
        if(t0 - lastMessageReceived > TEN_SECONDS)
        {
            Serial.println("Did not get a message in a long time. Broadcasting our info again.");
            sendConnectionRequest();
            lastMessageReceived = t0;
        }
    }
    else if(t0 - lastMessageReceived > FIVE_SECONDS)
    {
        Serial.println("Attempting to register at the server.");
        sendConnectionRequest();
        lastMessageReceived = t0;
    }

    con.maintain();    
}

void parsePackage(net::message* message)
{
    switch (message->header.type)
    {
        case net::message_type::Init:
        {
            net::init_message* init_message = reinterpret_cast<net::init_message*>(&message->body);
            Serial.printf("INIT received: pixels = %d, brightness = %d\n", init_message->pixels, init_message->brightness);
            initializeLeds(init_message->pixels, init_message->brightness);
            // only one package needed. mark done.
            con.m_message_cache.reset();
            break;
        }              


        case net::message_type::FrameHeader:
        {
            net::frame_header* header = reinterpret_cast<net::frame_header*>(&message->body);
            Serial.printf("FRAME_HEADER received: frameindex = %d, sections = %d, items = %d\n", header->index, header->total_sections, header->items);
                        
            if(header->total_sections < 2)
            {
               // Frame is small enough to fit in one package.
               displayFrame(header, message);
               // only one package needed. mark done.
               con.m_message_cache.reset();  
            }
            else 
            {
                // TODO: implement frame sections.
                con.m_message_cache.reset();
                
/*                     // We are receiving a new frame split over multiple packages.
                    m_frame_buffer.clear();
                    m_frame_buffer.addHeader(header);
                    con.m_message_cache.switchBuffer(); // we need more than one message.
                                // TODO keep message in right buffer cache
                                // TODO what if previous buffer is missing just one section, we should wait a bit more.
                 */
          
            }
            break;  
        }

        case net::message_type::FrameSection:
        {
            net::frame_section_header* section = reinterpret_cast<net::frame_section_header*>(&message->body);
            Serial.printf("FRAME_SECTION received: frameindex = %d, sectionIndex = %d\n", section->frame_index,section->section_index);

            // TODO: implement frame section
            con.m_message_cache.reset();
          /*   if(m_frame_buffer.m_frame_header != NULL && 
               m_frame_buffer.m_frame_header->index == section->frame_index)
            {
                if(m_frame_buffer.addSection(section))
                {
                    displayFrame(&m_frame_buffer);
                    con.m_message_cache.reset();
                }
                else
                {
                    con.m_message_cache.switchBuffer(); // we need more than one message.
                }
            }
            else
            {
                Serial.printf("FRAME_SECTION with frameindex = %d, sectionIndex = %d\n is not of the right frame index", section->frame_index,section->section_index);
                // TODO create new message cache?
            }           */  
            break;  
        }    

        default:
        {
            Serial.printf("Unknown message type or not handled type %d", message->header.type);
            break;
        }
    }

}

void initializeLeds(uint32_t pixels, uint8_t brightness)
{
    if(initialized)
    {
        return;
    }
    initialized = true;

    ledcontroller = new LedController(pixels / 8);
}

void displayFrame(net::frame_header* header, net::message* message)
{
    Serial.printf("Displaying frame %D of length %d\n",header->index, header->items);
    ledcontroller->setPixels(reinterpret_cast<net::pixel_instruction*>(&message->body[sizeof(net::frame_header)]), header->items, 0);
    ledcontroller->show();
}

void displayFrame(frame_buffer* buffer)
{
    // TODO
    Serial.printf("Displaying frame from frame buffer");
}



void sendConnectionRequest()
{
    byte key = 73;
    byte version = 1;
    int messageType = static_cast<int>(net::message_type::ConnectionRequest);
    memset(packetBuffer, 0, CONNECTION_REQUEST_PACKET_SIZE);
    memcpy(&packetBuffer, &CONNECTION_REQUEST_PACKET_SIZE, sizeof(int));
    memcpy(&packetBuffer[sizeof(int)], &messageType, sizeof(int));
    memcpy(&packetBuffer[sizeof(int) *2], &key, sizeof(byte));
    memcpy(&packetBuffer[sizeof(int) *2 + sizeof(byte)], &version, sizeof(byte));
    memcpy(&packetBuffer[sizeof(int) *2 + sizeof(byte) *2 ], &con.m_mac, sizeof(con.m_mac));
       
    // LENGTH KEY VERSION MAC

    con.Broadcast(packetBuffer, CONNECTION_REQUEST_PACKET_SIZE);
}



