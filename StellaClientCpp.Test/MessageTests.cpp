#include "MessageTests.h"
#include "message.h"
#include "FrameProtocol.h"
#include <iostream>
#include "TcpClient.h"
#include "UdpClient.h"


#include <chrono>
#include <thread>


int main(int argc, char* argv[])
{
	std::this_thread::sleep_for(std::chrono::milliseconds(4000));

	const int id = 2;
	stella::net::TcpClient c(id,"127.0.0.1", 20512);
	c.Connect();

	stella::net::UdpClient  udp("127.0.0.1", 20055);
	udp.Connect();
	
	while (true)
	{
		if (!c.Incoming().empty())
		{
			auto message_in_tcp = c.Incoming().pop_front();

			std::cout << message_in_tcp << "\n";
		}

		if(!udp.Incoming().empty())
		{
			auto message_in_udp = udp.Incoming().pop_front();

			frame_protocol_header* header = reinterpret_cast<frame_protocol_header*>(message_in_udp.body);

			std::cout << header << std::endl;

			int startIndex = sizeof(frame_protocol_header);
			// iterate till end of package
			for (int i = 0; i < header->number_of_pixel_instructions; i++)
			{
				int index = startIndex + sizeof(rgb) * i;
				rgb* x = reinterpret_cast<rgb*>(&message_in_udp.body[index]);

				auto y = x->r;

				break;
			}			
		}

	}


	return 0;

}