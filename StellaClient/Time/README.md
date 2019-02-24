## Time

The raspberry pi lacks an internal clock. This mean that each time the rpi is turned off, the time is lost on startup.
Normally it would use NTP to sync the time with a time server, but this needs an internet connection.

## 
Raspian uses timedatectl to control the time. 

To check if the time is synced via NTP:
```
timedatectl status
```

To turn of NTP for testing purposes
```
timedatectl set-ntp 0
```

