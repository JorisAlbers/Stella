#pragma once
#include <asio.hpp>
#include "ConcurrentDoubleEndedQueue.h"
#include "message.h"
#include <memory>

namespace stella
{
	namespace net 
	{
		class UdpClient
		{
		private:
			asio::io_context m_context;

			asio::ip::udp::socket* m_socket;

			std::thread threadContext;

			ConcurrentDoubleEndedQueue<message_out> queueOut;
			ConcurrentDoubleEndedQueue<message_in_udp> queueIn;

			// Incoming messages are constructed asynchronously, so we will
			// store the part assembled message here, until it is ready
			message_in_udp m_msgTemporaryIn;

			asio::ip::udp::resolver::results_type m_endpoints;


		public:
			explicit UdpClient(const std::string& host, const uint16_t port)
			{
				asio::ip::udp::resolver resolver(m_context);
				m_endpoints = resolver.resolve(host, std::to_string(port));
			}

			virtual ~UdpClient()
			{
				// If the client is destroyed, always try and disconnect from server
				//Disconnect();
				delete m_socket;
			}


		};
	}
}




