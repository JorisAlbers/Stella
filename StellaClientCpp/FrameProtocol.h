#pragma once
#include <iostream>

class FrameProtocol
{
public:
	static int GetFrameIndex(uint8_t* bytes)
	{
		int value;
		std::memcpy(&value, bytes, sizeof(int));
		return value;
	}
};

