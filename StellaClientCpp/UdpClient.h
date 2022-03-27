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

			asio::ip::udp::endpoint m_local_endpoint;


		public:
			explicit UdpClient(const std::string& host, const uint16_t port)
			{
				m_local_endpoint = asio::ip::udp::endpoint(asio::ip::address::from_string(host), port);				
			}

			virtual ~UdpClient()
			{
				// If the client is destroyed, always try and disconnect from server
				Disconnect();
				delete m_socket;
			}

			bool Connect()
			{
				try
				{
					m_socket = new asio::ip::udp::socket(m_context);
					m_socket->open(asio::ip::udp::v4());
					m_socket->bind(m_local_endpoint);
							
					ReadHeader();

					threadContext = std::thread([this]() {m_context.run();});				
				}
				catch (std::exception& e)
				{
					std::cerr << "Failed to connect to server. Error: " << e.what() << std::endl;
					return false;
				}
				return true;
			}

			bool Disconnect()
			{
				if (IsConnected())
				{
					asio::post(m_context, [this]() { m_socket->close(); });
					return true;
				}
				return false;
			};

			bool IsConnected()
			{
				return m_socket->is_open();
			};

		private:
			void ReadHeader()
			{
				m_socket->async_receive(asio::buffer(&m_msgTemporaryIn.header, sizeof(message_header)),
					[this](std::error_code ec, std::size_t length)
					{
						if (!ec)
						{
							// A complete message header has been read, check if this message
							// has a body to follow...
							if (m_msgTemporaryIn.header.size > 0)
							{
								ReadBody();
							}
							else
							{
								if (m_msgTemporaryIn.header.type == StellaMessageTypes::Unknown)
								{
									// Keep alive message received.
									std::cout << "Keep alive message received." << std::endl;
									ReadHeader();
									return;
								}

								// it doesn't, so add this bodyless message to the connections
								// incoming message queue
								AddToIncomingMessageQueue();
							}
						}
						else
						{
							int id = 10;
							// Reading form the client went wrong, most likely a disconnect
							// has occurred. Close the socket and let the system tidy it up later.
							std::cout << "[" << id << "] Read Header Fail.\n";
							std::cout << ec.message() << "\n";
							m_socket->close();
						}

					});
			}

			// ASYNC - Prime context ready to read a message body
			void ReadBody()
			{
				// If this function is called, a header has already been read, and that header
				// request we read a body, The space for that body has already been allocated
				// in the temporary message object, so just wait for the bytes to arrive...
				m_socket-> async_receive(asio::buffer(m_msgTemporaryIn.body, UDP_BUFFER_SIZE),
					[this](std::error_code ec, std::size_t length)
					{
						if (!ec)
						{
							// ...and they have! The message is now complete, so add
							// the whole message to incoming queue
							AddToIncomingMessageQueue();
						}
						else
						{
							// As above!
							int id = 10;
							std::cout << "[" << id << "] Read Body Fail.\n";
							std::cout << "Reason: " << ec.message() << std::endl;
							m_socket->close();
						}
					});
			}

			// Once a full message is received, add it to the incoming queue
			void AddToIncomingMessageQueue()
			{
				// Shove it in queue, converting it to an "owned message", by initialising
				// with the a shared pointer from this connection object
				queueIn.push_back(m_msgTemporaryIn);

				ReadHeader();
			}


		};
	}
}




