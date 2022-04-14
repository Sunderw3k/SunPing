# SunPing
Nothing really special, just console app to send ICMP packets.
Usage simple as always
```py
Usage: sping <Address (string)> <BufferSize (int)> <Timeout (ms)> <TTL (int)>
If an argument isn't specified, the default value is used (1.1.1.1, 32, 10000, 128)

Example:
# Doesn't use default values
sping 192.168.1.1 64 50000 64

# Uses default values for timeout and TTL
sping 192.168.1.1 16

# Uses default values for everything
sping 
```

I am aware of a few bugs:
- When using CTRL + C output gets printed
- When using CTRL + C the ^C text stays after clearing the screen

Let me know if anything crashes with an issue or a PR
