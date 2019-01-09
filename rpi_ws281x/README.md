## Source

This project is a clone of [rpi-ws281x-csharp](https://github.com/rpi-ws281x/rpi-ws281x-csharp) but has a minor adjustment to be able to run on dotnet core.
See [this](https://github.com/rpi-ws281x/rpi-ws281x-csharp/issues/2) issue.

## Installation
In order to get the wrapper working, you need build the shared C library first.
The required source code is included via a git submodule and is located under lib/rpi-ws281x.
This is a link to the original project.
Switch to the directory and execute following commands:
```
scons
gcc -shared -o ws2811.so *.o

The P/Invoke functinality has a special search pattern to find the required assembly. __For my tests I copied the ws2811.so assembly to /usr/lib__ (as mentioned in the link above).
