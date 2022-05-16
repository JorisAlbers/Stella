#include <Arduino.h>

namespace net
{
    const int UDP_BUFFER_SIZE = 60000;

    enum class stellaMessageTypes : uint32_t
    {
        Unknown,
        Init,
        Standard,
        AnimationRenderFrame,
    };

    struct message_header
    {
        uint32_t size; // excludes this 4 byte int field. messagetype + message size
        stellaMessageTypes type{};
    };

    struct message_in_udp
    {
        message_header header{ };
        uint8_t  body[UDP_BUFFER_SIZE];
        int currentIndex = 0;

        size_t size() const
        {
            return sizeof(header.size) + header.size; // size excludes itself.
        }

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
}


