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

    struct frame_header_index
    {
        uint32_t index;
    };

    struct frame_header
    {
        uint32_t index;
        uint32_t timestamp_relative;
        uint32_t items;
        bool has_frame_sections;
    };
}