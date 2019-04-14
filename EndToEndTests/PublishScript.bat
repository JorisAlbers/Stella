ECHO Publishing
dotnet restore
dotnet publish -o .\publish\windows
dotnet publish -o .\publish\linux -r linux-arm