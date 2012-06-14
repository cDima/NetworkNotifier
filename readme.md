#Network Watcher 
## Notify of newly connected MACs and disconnected devices in the same LAN subnetwork

Notifies of new MACs connecting and disconnecting onto the same LAN network as machine's subnet domain.

Example, my machine has IP 10.0.1.5. The iPhone connects to the same LAN and router assignes DHCP dynamically.
The IP Range  10.0.1.1-127 will be pinged and arp -a command will be launched every second to see changes.

Eample code:

        var networkWatcher = new NetworkMACWatcher();
        networkWatcher.MacConnected += new EventHandler<string>(networkWatcher_MacConnected);
        networkWatcher.MacDisconnected += new EventHandler<string>(networkWatcher_MacDisconnected);
        networkWatcher.WatchNetwork();
    
