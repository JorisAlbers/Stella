#pragma once
#include <Arduino.h>
#include "frame.h"
#include <vector>

class frame_buffer
{
    private:
        
        std::vector<net::frame_section_header*> m_sections; // TODO: send the max number of sections to expect on init with server.
        int32_t m_sections_received;

        void growSectionsArrayIfNecessary(int sectionIndex)
        {
            if(m_sections.size() > sectionIndex)
            {
                return;
            }

            m_sections.resize(sectionIndex + 1);
        }

    public:
        net::frame_header* m_frame_header;

        void addHeader(net::frame_header* header)
        {
            m_frame_header = header;
        }

        bool addSection(net::frame_section_header* section)
        {
            growSectionsArrayIfNecessary(section->section_index);

            m_sections[section->section_index] = section;

            return m_sections.size() == m_frame_header->items;
        }

        void clear()
        {
            m_sections_received = 0;
            m_frame_header = NULL;
            for (size_t i = 0; i < m_sections.size(); i++)
            {
                m_sections[i] = NULL;
            }            
        }
};