#pragma once
#include <asio.hpp>
#include "ConcurrentDoubleEndedQueue.h"
#include "message.h"
#include <memory>

namespace stella
{
	namespace net
	{
		class TcpClient
		{
		public:
			TcpClient()
			{
			}

			virtual ~TcpClient()
			{
				// If the client is destroyed, always try and disconnect from server
				Disconnect();
				delete m_socket;
			}

			bool Connect(const std::string& host, const uint16_t port)
			{
				try 
				{
					asio::ip::tcp::resolver resolver(m_context);
					asio::ip::tcp::resolver::results_type endpoints = resolver.resolve(host, std::to_string(port));

					// Request asio attempts to connect to an endpoint

					m_socket = new asio::ip::tcp::socket(m_context);

					asio::async_connect(*m_socket, endpoints,
						[this](std::error_code ec, asio::ip::tcp::endpoint endpoint)
						{
							if (ec)
							{
								std::cerr << "Failed to connect to server at "<< " \n";
								std::cerr << ec.message() << "\n";
								return;
							}
							
							ReadHeader();							
							
						});

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

			//bool Send(const message<T>& message);

		public:
			// Retrieve queue of messages from server
			ConcurrentDoubleEndedQueue<message>& Incoming()
			{
				return queueIn;
			}

		private:
			void ReadHeader()
			{
				asio::async_read(*m_socket, asio::buffer(&m_msgTemporaryIn.header, sizeof(message_header)),
					[this](std::error_code ec, std::size_t length)
					{
						if (!ec)
						{
							// A complete message header has been read, check if this message
							// has a body to follow...
							if (m_msgTemporaryIn.header.size > 0)
							{
								// ...it does, so allocate enough space in the messages' body
								// vector, and issue asio with the task to read the body.
								m_msgTemporaryIn.body.resize(m_msgTemporaryIn.header.size);
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
				asio::async_read(*m_socket, asio::buffer(m_msgTemporaryIn.body.data(), m_msgTemporaryIn.body.size()),
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



		private:
			asio::io_context m_context;

			asio::ip::tcp::socket* m_socket;

			std::thread threadContext;

			ConcurrentDoubleEndedQueue<message> queueOut;
			ConcurrentDoubleEndedQueue<message> queueIn; // should be provided by the concrete implementation

			// Incoming messages are constructed asynchronously, so we will
			// store the part assembled message here, until it is ready
			message m_msgTemporaryIn;
		};
	}
}



