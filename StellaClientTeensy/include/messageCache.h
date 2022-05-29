#pragma once
#include <SPI.h>
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Arduino.h>
#include "message.h"

namespace net
{
    class messageCache
    {
        private:
            
            
            net::message m_message1;



        public:
            net::message* getForWriting()
            {
                return &m_message1;
            }

            net::message* getForReading()
            {
                return &m_message1;
            }

            void markReadyForReading()
            {
                // TODO
            }

            void resetMessage(message* message)
            {
                m_message1.reset();
            }
    };
}
