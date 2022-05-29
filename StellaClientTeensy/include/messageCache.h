#pragma once
#include <SPI.h>
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Arduino.h>
#include "message.h"
#include <vector>

namespace net
{
    class messageCache
    {
        private:                        
            std::vector<net::message*> m_buffers { new net::message};
            uint32_t m_buffer_index;

            void growSectionsArrayIfNecessary(uint32_t index)
            {
                if(m_buffers.size() > index)
                {
                    return;
                }

                m_buffers.resize(index + 1);
                m_buffers[index] = new net::message;
            }



        public:
            net::message* currentBuffer()
            {
                return m_buffers[m_buffer_index];
            }

            void switchBuffer()
            {
                m_buffer_index++;
                growSectionsArrayIfNecessary(m_buffer_index);
            }     

            void reset()
            {
                for (size_t i = 0; i < m_buffer_index+1; i++) // +1 to also reset the buffer at the current index.
                {
                    m_buffers[i]->reset();
                }

                m_buffer_index = 0;                
            }
    };
}
