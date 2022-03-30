#pragma once
#include <iostream>

struct frame_protocol_header
{
	int frame_index;
	int relative_timestamp;
	int number_of_pixel_instructions;
	bool has_frame_sections;

	friend std::ostream& operator<<(std::ostream& os, frame_protocol_header const& arg)
	{
		os << "frame index: " << arg.frame_index << ", relative timestamp:" << arg.relative_timestamp << ", pixel instr. :" << arg.number_of_pixel_instructions << ", has_frame_sections: "<< arg.has_frame_sections<< "]";
	}
};

struct rgb
{
	uint8_t r;
	uint8_t g;
	uint8_t b;
};

