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
		private:
			const int m_id;

			asio::io_context m_context;

			asio::ip::tcp::socket* m_socket;

			std::thread threadContext;

			ConcurrentDoubleEndedQueue<message_out> queueOut;
			ConcurrentDoubleEndedQueue<message_in_tcp> queueIn; // should be provided by the concrete implementation

			// Incoming messages are constructed asynchronously, so we will
			// store the part assembled message here, until it is ready
			message_in_tcp m_msgTemporaryIn;

			asio::ip::tcp::resolver::results_type m_endpoints;


		public:
			explicit TcpClient(const int id, const std::string& host, const uint16_t port) : m_id(id)
			{
				asio::ip::tcp::resolver resolver(m_context);
				m_endpoints = resolver.resolve(host, std::to_string(port));
			}

			virtual ~TcpClient()
			{
				// If the client is destroyed, always try and disconnect from server
				Disconnect();
				delete m_socket;
			}

			bool Connect()
			{
				try 
				{
					// Request asio attempts to connect to an endpoint

					m_socket = new asio::ip::tcp::socket(m_context);

					asio::async_connect(*m_socket, m_endpoints,
						[this](std::error_code ec, asio::ip::tcp::endpoint endpoint)
						{
							if (ec)
							{
								std::cerr << "Failed to connect to server at "<< " \n";
								std::cerr << ec.message() << "\n";
								return;
							}

							OnConnected();
							
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

			
		public:
			// ASYNC - Send a message, connections are one-to-one so no need to specifiy
			// the target, for a client, the target is the server and vice versa
			void Send(const message_out& msg)
			{
				asio::post(m_context,
					[this, msg]()
					{
						// If the queue has a message in it, then we must 
						// assume that it is in the process of asynchronously being written.
						// Either way add the message to the queue to be output. If no messages
						// were available to be written, then start the process of writing the
						// message at the front of the queue.
						bool bWritingMessage = !queueOut.empty();
						queueOut.push_back(msg);
						if (!bWritingMessage)
						{
							WriteHeader();
						}
					});
			}

		public:
			// Retrieve queue of messages from server
			ConcurrentDoubleEndedQueue<message_in_tcp>& Incoming()
			{
				return queueIn;
			}

		private:

			void OnConnected()
			{
				std::cout << "Connected to server via tcp.\n";
				std::cout << "Sending INIT message. My id is " << m_id << "\n";

				stella::net::message_out message_in_tcp;
				message_in_tcp.header.type = stella::net::StellaMessageTypes::Init;
				message_in_tcp << m_id;

				Send(message_in_tcp);
			}

			void OnDisconnect()
			{
				std::cout << "Disconnected with server.\n";
				std::cout << "Attempting to connect\n";
				Connect();
			}

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
							std::cout << ec.message() <<"\n";
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
				asio::async_read(*m_socket, asio::buffer(m_msgTemporaryIn.body, TCP_BUFFER_SIZE),
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

			private:
				// ASYNC - Prime context to write a message header
				void WriteHeader()
				{
					// If this function is called, we know the outgoing message queue must have 
					// at least one message to send. So allocate a transmission buffer to hold
					// the message, and issue the work - asio, send these bytes
					asio::async_write(*m_socket, asio::buffer(&queueOut.front().header, sizeof(message_header)),
						[this](std::error_code ec, std::size_t length)
						{
							// asio has now sent the bytes - if there was a problem
							// an error would be available...
							if (!ec)
							{
								// ... no error, so check if the message header just sent also
								// has a message body...
								if (queueOut.front().currentIndex > 0)
								{
									// ...it does, so issue the task to write the body bytes
									WriteBody();
								}
								else
								{
									// ...it didnt, so we are done with this message. Remove it from 
									// the outgoing message queue
									queueOut.pop_front();

									// If the queue is not empty, there are more messages to send, so
									// make this happen by issuing the task to send the next header.
									if (!queueOut.empty())
									{
										WriteHeader();
									}
								}
							}
							else
							{
								// ...asio failed to write the message, we could analyse why but 
								// for now simply assume the connection has died by closing the
								// socket. When a future attempt to write to this client fails due
								// to the closed socket, it will be tidied up.
								int id = 10;
								std::cout << "[" << id << "] Write Header Fail.\n";
								m_socket->close();
							}
						});
				}

				// ASYNC - Prime context to write a message body
				void WriteBody()
				{
					// If this function is called, a header has just been sent, and that header
					// indicated a body existed for this message. Fill a transmission buffer
					// with the body data, and send it!
					asio::async_write(*m_socket, asio::buffer(queueOut.front().body, queueOut.front().header.size - sizeof(int)), // size = message type+ body size. 
						[this](std::error_code ec, std::size_t length)
						{
							if (!ec)
							{

								// Sending was successful, so we are done with the message
								// and remove it from the queue
								queueOut.pop_front();

								// If the queue still has messages in it, then issue the task to 
								// send the next messages' header.
								if (!queueOut.empty())
								{
									WriteHeader();
								}
							}
							else
							{
								int id = 10;
								std::cout << "[" << id << "] Write Body Fail.\n";
								m_socket->close();
							}
						});
				}
		
		};
	}
}



