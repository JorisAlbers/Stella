#include <SPI.h>
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Arduino.h>
#include "message.h"
#include "messageCache.h"

class connection
{
    private:        
        EthernetUDP m_udp;
        const uint16_t m_port;
        const uint16_t m_server_broadcast_port;

    public:
        byte m_mac[6];
        net::messageCache m_message_cache;     


    public:
        explicit connection(const uint16_t port, const uint16_t server_broadcast_port) : m_port(port) , m_server_broadcast_port(server_broadcast_port)
        {
            getMacAdress((u_int8_t*)&m_mac);
        }

        bool open()
        {
            if (Ethernet.begin(m_mac) == 0) 
            {
                Serial.println("Failed to configure Ethernet using DHCP");
                // Check for Ethernet hardware present
                if (Ethernet.hardwareStatus() == EthernetNoHardware) 
                {
                    Serial.println("Ethernet shield was not found.  Sorry, can't run without hardware. :(");
                } 
                else if (Ethernet.linkStatus() == LinkOFF) 
                {
                    Serial.println("Ethernet cable is not connected.");
                }
                return false;
            }

            m_udp.begin(m_port);
            return true;
        }

        void Broadcast(byte* data, int length)
        {
            IPAddress MyServer(255, 255, 255, 255);
            m_udp.beginPacket(MyServer, m_server_broadcast_port);
            m_udp.write(data, length);
            m_udp.endPacket();
        }

        bool ReadData()
        {
            int bytesReceived = m_udp.parsePacket();      
            
            if(bytesReceived == 0)
            {
                return false;
            }

            Serial.printf("Message in: %d bytes, ", bytesReceived);

            net::message* message = m_message_cache.currentBuffer();
            
            int i = 0;
            while(i != bytesReceived)
            {
                int bytesAvailable = bytesReceived - i;
                if(message->headerBytesReceived < sizeof(message->header))
                {
                    // we need to receive the header.
                    int bytesRequested = sizeof(message->header) - message->headerBytesReceived;
                    int bytesTransferred = min(bytesRequested, bytesAvailable);
                    i+= bytesTransferred;
                    message->headerBytesReceived += bytesTransferred;

                    m_udp.readBytes(reinterpret_cast<byte*>(&message->header),bytesTransferred);
                    continue;
                }

                int bytesRequested = (message->header.size - sizeof(message->header.type)) - message->messageBytesReceived; // stella server sends as length messagetype + data

                if(bytesRequested > 0) 
                {
                    // we need to receive the data                    
                    int bytesTransferred = min(bytesRequested, bytesAvailable);
                    i+= bytesTransferred;
                    message->messageBytesReceived += bytesTransferred;

                    m_udp.readBytes(reinterpret_cast<byte*>(&message->body),bytesTransferred);
                    continue;
                }
            }

            Serial.printf(" headerbr = %d, bodybr = %d, size = %d, type = %d,", message->headerBytesReceived, message->messageBytesReceived, message->header.size, message->header.type);

            if(message->headerBytesReceived == sizeof(message->header) &&
                (message->header.size - sizeof(message->header.type)) == message->messageBytesReceived)
                {
                    Serial.printf("Full end\n", sizeof(message->header) + message->header.size);
                    // Signal that a full package is received
                    return true;
                }
            else
            {
                // Wait for more data to arrive.
                Serial.println("partial package");
            }
            return false;
        }

        void maintain()
        {
            Ethernet.maintain();
        }

    private:
        void getMacAdress(uint8_t *mac) {
            for(uint8_t by=0; by<2; by++) mac[by]=(HW_OCOTP_MAC1 >> ((1-by)*8)) & 0xFF;
            for(uint8_t by=0; by<4; by++) mac[by+2]=(HW_OCOTP_MAC0 >> ((3-by)*8)) & 0xFF;
            Serial.printf("MAC: %02x:%02x:%02x:%02x:%02x:%02x\n", mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);
        }


};