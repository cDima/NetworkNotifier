#Network Watcher
## Notifier of newly connected and disconnected devices 

Notifies of new MACs connecting and disconnecting onto the same LAN network as machine's subnet domain.

Example, my machine has IP 10.0.1.5. The iPhone connects to the same LAN and router assignes DHCP dynamically.
The IP Range  10.0.1.1-127 will be pinged and arp -a command will be launched every second to see changes.

Eample code:

        var networkWatcher = new NetworkMACWatcher();
        networkWatcher.MacConnected += new EventHandler<string>(networkWatcher_MacConnected);
        networkWatcher.MacDisconnected += new EventHandler<string>(networkWatcher_MacDisconnected);
        networkWatcher.WatchNetwork();

## Tray notifications 

The TrayNotifier app sits in the tray and alerts of newly connected devices. Integrated with [Growl for windows](http://www.growlforwindows.com).

![Tray notification](https://raw.github.com/cDima/NetworkWatcher/master/systray-demo.png)
