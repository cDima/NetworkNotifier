namespace NetworkMacExample
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using WifiNotificator;

    class Program
    {
        static void Main(string[] args)
        {
            var networkWatcher = new NetworkMACWatcher();
            networkWatcher.MacConnected += new EventHandler<string>(networkWatcher_MacConnected);
            networkWatcher.MacDisconnected += new EventHandler<string>(networkWatcher_MacDisconnected);
            networkWatcher.WatchNetwork();
        }

        static void networkWatcher_MacConnected(object sender, string mac)
        {
            Console.WriteLine("mac newly connected: " + mac);
        }

        static void networkWatcher_MacDisconnected(object sender, string mac)
        {
            Console.WriteLine("mac disconnected: " + mac);
        }
    }
}
