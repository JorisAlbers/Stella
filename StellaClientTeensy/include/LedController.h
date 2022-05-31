#pragma once

#include <OctoWS2811.h>
#include "frame.h"
const int config = WS2811_GRB | WS2811_800kHz;

class LedController
{
    private:
        OctoWS2811* m_controller;
        int* m_displayMemory; // TODO use DWAMEM
        int* m_drawingMemory;


    public:
        explicit LedController(uint32_t ledsPerStrip) 
        {
            Serial.printf("Initializing led controller with %d leds per strip", ledsPerStrip);
           m_displayMemory = (int*) calloc(sizeof(int),ledsPerStrip*6);
           m_drawingMemory = (int*) calloc(sizeof(int),ledsPerStrip*6);

           
            m_controller = new OctoWS2811(ledsPerStrip, m_displayMemory, m_drawingMemory, config);
            m_controller->begin();
            m_controller->show();
        };

        void setPixels(net::pixel_instruction* instructions, uint32_t size, uint32_t startIndex)
        {
            for (uint32_t i = 0; i < size; i++)
            {
                net::pixel_instruction instruction = instructions[i];
                m_controller->setPixel(i + startIndex, instruction.red, instruction.green, instruction.blue);
            }
        }

        void show()
        {
            m_controller->show();
        }
};