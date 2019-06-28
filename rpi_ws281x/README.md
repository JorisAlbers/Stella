## Source

This project is a clone of [rpi-ws281x-csharp](https://github.com/rpi-ws281x/rpi-ws281x-csharp) but has a minor adjustment to be able to run on dotnet core.
See [this](https://github.com/rpi-ws281x/rpi-ws281x-csharp/issues/2) issue.

## Installation

Install rpi_ws281x
```
sudo apt-get update
sudo apt-get install build-essential python-dev git scons swig

git clone https://github.com/jgarff/rpi_ws281x.git
cd rpi_ws281x
scons

cd python
sudo python setup.py install
```
In order to get the wrapper working, you need build the shared C library first.
The required source code is included via a git submodule and is located under lib/rpi-ws281x.
This is a link to the original project.
Switch to the directory and execute following commands:
```
scons
gcc -shared -o ws2811.so *.o
```
The P/Invoke functinality has a special search pattern to find the required assembly. _For my tests I copied the ws2811.so assembly to /usr/lib_ (as mentioned in the link above).
