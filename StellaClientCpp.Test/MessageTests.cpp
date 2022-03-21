#include "MessageTests.h"
#include "message.h"
#include <iostream>
#include "TcpClient.h"

enum class MyMessageType : uint32_t
{
	Aap,
	Noot,
	Mies
};

class CustomClient : public stella::net::TcpClient<MyMessageType>
{
public:
	bool SendMessage()
	{
		stella::net::message<MyMessageType> message;
		message.header.id = MyMessageType::Noot;
		message << "test";
		//Send();
	}
};


int main(int argc, char* argv[])
{
	stella::net::message<MyMessageType> message;
	message.header.id = MyMessageType::Aap;
	message << " Test";

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