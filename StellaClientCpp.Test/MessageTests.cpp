#include "MessageTests.h"
#include "message.h"
#include <iostream>
#include "TcpClient.h"


int main(int argc, char* argv[])
{
	stella::net::message message;
	message.header.type = stella::net::StellaMessageTypes::Init;
	message << 1; //  my id = 1

	stella::net::TcpClient c("127.0.0.1", 20512);
	c.Connect();

	
	c.Send(message);
	
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