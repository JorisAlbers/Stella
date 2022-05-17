#include <SPI.h>
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Arduino.h>
#include "message.h"

class connection
{
    private:        
        EthernetUDP m_udp;
        const uint16_t m_port;
        const uint16_t m_server_broadcast_port;

    public:
        byte m_mac[6];

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