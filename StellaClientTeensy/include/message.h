#pragma once
#include <Arduino.h>

namespace net
{
    enum class message_type : uint32_t
    {
        Unknown,
        Init,
        Standard,
        AnimationRenderFrame,
        ConnectionRequest,
    };

    struct message_header
    {
        uint32_t size; // excludes this 4 byte int field. messagetype + message size
        message_type type{};
    };

    const int UDP_BUFFER_SIZE = 1024 * 2 * 7 - sizeof(message_header);
 
    struct message
    {
        message_header header{ };
        uint8_t  body[UDP_BUFFER_SIZE];
        uint32_t messageBytesReceived = 0;
        uint32_t headerBytesReceived = 0;

        size_t size() const
        {
            return sizeof(header.size) + header.size; // size excludes itself.
        }

        template<typename DataType>
        friend message& operator << (message& msg, const DataType& data)
        {
            // copy data
            memcpy(&msg.body[msg.messageBytesReceived], &data, sizeof(DataType));

            // increment index
            msg.messageBytesReceived += sizeof(DataType);

            msg.header.size = sizeof(int) + msg.messageBytesReceived; // int = message type 

            // Return the target message so it can be "chained"
            return msg;
        }

        void reset()
        {
            messageBytesReceived = 0;
            headerBytesReceived = 0;
        }
    };

    struct init_message
    {
        uint32_t version;
        uint32_t pixels;
        uint8_t brightness;
    };
}


