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

			int i = FrameProtocol::GetFrameIndex(message_in_udp.body);
			int x = FrameProtocol::GetFrameIndex(&message_in_udp.body[4]);
			int y = FrameProtocol::GetFrameIndex(&message_in_udp.body[8]);
			int z = FrameProtocol::GetFrameIndex(&message_in_udp.body[12]);



			std::cout << " UDP package received. Size: " << message_in_udp.header.size << "type: " << static_cast<int>(message_in_udp.header.type) << "frame index: " << FrameProtocol::GetFrameIndex(message_in_udp.body) << "\n";
		}

	}


	return 0;

}