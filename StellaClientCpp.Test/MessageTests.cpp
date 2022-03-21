#include "MessageTests.h"
#include "message.h"
#include <iostream>
#include "TcpClient.h"

class CustomClient : public stella::net::TcpClient
{
public:
	bool SendMessage()
	{
		stella::net::message message;
		message.header.id = stella::net::StellaMessageTypes::Init;
		message << 1;
		//Send();
	}
};


int main(int argc, char* argv[])
{
	stella::net::message message;
	message.header.id = stella::net::StellaMessageTypes::Init;
	message << 1;

	CustomClient c;
	c.Connect("127.0.0.1", 20512);
	
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