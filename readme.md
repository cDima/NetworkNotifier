# Network Notifier
## Notifier of newly connected and disconnected devices 

Notifies of new MACs connecting and disconnecting onto the same LAN network as machine's subnet domain. For example, my machine has IP 10.0.1.5. The iPhone connects to the same LAN and router assignes DHCP dynamically. The IP Range  10.0.1.1-127 will be pinged and arp -a command will be launched every second to see changes.

## Tray notifications 

The TrayNotifier app sits in the tray and alerts of newly connected devices. Integrated with [Growl for windows](http://www.growlforwindows.com) and Philips Hue lights.

![Tray notification](https://raw.github.com/cDima/NetworkNotifier/master/systray-demo.png)

## Philips Hue lights alert

Have the lights blink on&off when a phone enters or leaves the WiFi network (when your phone is near the house).
