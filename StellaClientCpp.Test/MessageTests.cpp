#include "MessageTests.h"
#include "message.h"
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

	stella::net::UdpClient  udp("127.0.0.1", 20513);
	udp.Connect();
	
	while (true)
	{
		if (!c.Incoming().empty())
		{
			auto message_in_tcp = c.Incoming().pop_front();

			std::cout << message_in_tcp << "\n";
		}
	}


	return 0;

}