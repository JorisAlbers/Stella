## Publish dotnet core for rpi

Dotnet core can't be build from a linux computer. 

Therefore, build from a different OS with the following command:
'''
dotnet publish -o ./publish -r linux-arm
'''