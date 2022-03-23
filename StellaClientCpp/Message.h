#pragma once
#include <asio.hpp>

namespace stella
{
	namespace net
	{
		const int UDP_HEADER_SIZE = 60000;


		enum class StellaMessageTypes : uint32_t
		{
			Unknown,
			Init,
			Standard,
			AnimationRenderFrame,
		};

				
		struct message_header
		{
			uint32_t size; // excludes this 4 byte int field. messagetype + message size
			StellaMessageTypes type{};
		};

		
		struct message
		{
			message_header header{ };
			char body[UDP_HEADER_SIZE]; // UDP max size. TODO for sending data a smaller size is enough
			int currentIndex = 0;
			

			size_t size() const
			{
				return sizeof(header.size) + header.size; // size excludes itself.
			}

			friend std::ostream& operator << (std::ostream& os, const message& msg)
			{
				os << "ID:" << int(msg.header.type) << " Size:" << msg.header.size;
				return os;
			}

			// TODO we know the length of the message when we create it.
			// TODO redesign vector to array?
			// Pushes any POD-like data into the message buffer
			template<typename DataType>
			friend message& operator << (message& msg, const DataType& data)
			{
				// Check that the type of the data being pushed is trivially copyable
				static_assert(std::is_standard_layout<DataType>::value, "Data is too complex to be pushed into vector");

				// copy data
				std::memcpy(&msg.body[msg.currentIndex], &data, sizeof(DataType));

				// increment index
				msg.currentIndex += sizeof(DataType);

				msg.header.size = sizeof(int) + msg.currentIndex; // int = message type 
				
				// Return the target message so it can be "chained"
				return msg;
			}
		};
	}
}

