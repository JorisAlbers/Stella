#pragma once
#include <asio.hpp>

namespace stella
{
	namespace net
	{
		const int UDP_BUFFER_SIZE = 60000;
		const int TCP_BUFFER_SIZE = 1024;
		const int MAX_MESSAGE_OUT_SIZE = sizeof(int) + sizeof(int);

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

		
		struct message_in_tcp
		{
			message_header header{ };
			char body[TCP_BUFFER_SIZE]; 
			int currentIndex = 0;
			

			size_t size() const
			{
				return sizeof(header.size) + header.size; // size excludes itself.
			}

			friend std::ostream& operator << (std::ostream& os, const message_in_tcp& msg)
			{
				os << "ID:" << int(msg.header.type) << " Size:" << msg.header.size;
				return os;
			}

			// TODO we know the length of the message when we create it.
			// TODO redesign vector to array?
			// Pushes any POD-like data into the message buffer
			template<typename DataType>
			friend message_in_tcp& operator << (message_in_tcp& msg, const DataType& data)
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

		struct message_in_udp
		{
			message_header header{ };
			char body[UDP_BUFFER_SIZE];
			int currentIndex = 0;


			size_t size() const
			{
				return sizeof(header.size) + header.size; // size excludes itself.
			}

			friend std::ostream& operator << (std::ostream& os, const message_in_udp& msg)
			{
				os << "ID:" << int(msg.header.type) << " Size:" << msg.header.size;
				return os;
			}

			// TODO we know the length of the message when we create it.
			// TODO redesign vector to array?
			// Pushes any POD-like data into the message buffer
			template<typename DataType>
			friend message_in_udp& operator << (message_in_udp& msg, const DataType& data)
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

		/// <summary>
		/// An out message is small. Only a small buffer needed.
		/// </summary>
		struct message_out 
		{
			message_header header{ };
			char body[MAX_MESSAGE_OUT_SIZE];
			int currentIndex = 0;


			size_t size() const
			{
				return sizeof(header.size) + header.size; // size excludes itself.
			}

			friend std::ostream& operator << (std::ostream& os, const message_out& msg)
			{
				os << "ID:" << int(msg.header.type) << " Size:" << msg.header.size;
				return os;
			}

			// TODO we know the length of the message when we create it.
			// TODO redesign vector to array?
			// Pushes any POD-like data into the message buffer
			template<typename DataType>
			friend message_out& operator << (message_out& msg, const DataType& data)
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

