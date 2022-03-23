#include "MessageTests.h"
#include "message.h"
#include <iostream>
#include "TcpClient.h"


#include <chrono>
#include <thread>


int main(int argc, char* argv[])
{
	std::this_thread::sleep_for(std::chrono::milliseconds(4000));

	const int id = 2;
	stella::net::TcpClient c(id,"127.0.0.1", 20512);
	c.Connect();
	
	while (true)
	{
		if (!c.Incoming().empty())
		{
			auto message = c.Incoming().pop_front();

			std::cout << message << "\n";
		}
	}


	return 0;

}