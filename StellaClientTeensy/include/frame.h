#pragma once
#include <Arduino.h>

namespace net
{
    struct pixel_instruction
    {
        uint8_t red;
        uint8_t green;
        uint8_t blue;
    };

    struct frame_header
    {
        uint32_t index;
        uint32_t total_sections;
        uint32_t items;
    };

    struct frame_section_header
    {
         uint32_t frame_index;
         uint32_t section_index;
         uint32_t items;
    };
}