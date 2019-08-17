ECHO Publishing
dotnet restore
dotnet publish -o .\publish\windows -c Release -r win10-x64

ECHO Starting
dotnet run publish/mac/StellaServerConsole -m Resources/Configuration/MappingExample.yaml -s Resources/Storyboards -ip 192.168.2.3 -port 20055 -api_ip 192.168.2.3 -api_port 20060 -udp_port 20056 -b Resources/Bitmaps/
