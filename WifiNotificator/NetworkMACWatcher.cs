using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WifiNotificator
{
    /// <summary>
    /// Notifies of new MACs connecting and disconnecting onto the same LAN network as machine's subnet domain;
    /// Example, my machine has IP 10.0.1.5. The iPhone connects to the same LAN and router assignes DHCP dynamically; 
    /// The IP Range  10.0.1.1-127 will be pinged and arp -a command will be launched every second to see changes.
    /// </summary>
    /// <example><code>
    ///    var networkWatcher = new ArpPinger();
    ///    networkWatcher.MacConnected += new EventHandler&lt;string&gt;(networkWatcher_MacConnected);
    ///    networkWatcher.MacDisconnected += new EventHandler&lt;string&gt;(networkWatcher_MacDisconnected);
    ///    networkWatcher.WatchNetwork();</code>
    /// </example>
    /// <param name="args"></param>
    /// <remarks>http://stackoverflow.com/questions/2567107/ping-or-otherwise-tell-if-a-device-is-on-the-network-by-mac-in-c-sharp</remarks>
    public class NetworkMACWatcher
    {
        public event EventHandler<Dictionary<string, string>> FirstAddressesLoaded;
        public event EventHandler<KeyValuePair<string, string>> MacConnected;
        public event EventHandler<KeyValuePair<string, string>> MacDisconnected;

        private delegate void WatchNetworkDelegate();
        private WatchNetworkDelegate watchNetworkDelegate;

        ConcurrentDictionary<string, string> macCache = null;

        public NetworkMACWatcher()
        {
            watchNetworkDelegate = new WatchNetworkDelegate(this.WatchNetwork);
        }

        public void WatchNetworkAsync()
        {
            watchNetworkDelegate.BeginInvoke(null, null);
        }

        //public List<string> GetLanIPs()
        //{
        //    if (macCache == null) return null;
        //    return macCache.Values.Distinct().ToList();
        //}

        /// <summary>
        /// Infinatelly loops to fire off MAC notification events. On same thread.
        /// </summary>
        public void WatchNetwork()
        {
            while (true)
            {
                var macs = new NetworkMACWatcher().GetMacsInLan();

                if (macCache == null)
                {
                    macCache = new ConcurrentDictionary<string, string>(macs);
                    if (this.FirstAddressesLoaded != null)
                    {
                        ConsoleDebug(macs);
                        FirstAddressesLoaded(this, new Dictionary<string,string>(macCache));
                    }
                }

                var newlyconnected = macs.Except(macCache);
                var disconnected = macCache.Except(macs);

                foreach (var mac in disconnected)
                    if (this.MacDisconnected != null) 
                        this.MacDisconnected(this, mac);
                
                foreach (var mac in newlyconnected)
                    if (this.MacConnected != null)
                        this.MacConnected(this, mac);

                macCache = new ConcurrentDictionary<string, string>(macs);

                System.Threading.Thread.Sleep(1000);
            }
        }

        [Conditional("DEBUG")]
        private void ConsoleDebug(Dictionary<string, string> macs)
        {
            foreach (var mac in macs) 
                Console.WriteLine("found Mac: " + mac);
        }

        public Dictionary<string,string> GetMacsInLan()
        {
            var foundMacs = new Dictionary<string,string>();
            // refresh by pinging all subnets
            var baseIP = IPUtil.ConvertToBaseIP(IPUtil.GetMyIpAddress());

            // clear arp cache, it gets reloaded every 30 seconds they say
            //ClearARPCache();

            PingAllSubnet(baseIP);
            
            // We're going to parse the output of arp.exe to find out if any all MACs online
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "arp.exe";
            psi.Arguments = "-a";
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using (Process pro = Process.Start(psi))
            {
                using (StreamReader sr = pro.StandardOutput)
                {
                    string s = sr.ReadLine();

                    while (s != null)
                    {
                        if (s.Contains("Interface") || s.Trim() == "" || s.Contains("Address"))
                        {
                            s = sr.ReadLine();
                            continue;
                        }

                        string[] parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        var hostnameOrAddress = parts[0].Trim(); // may be IP address, must resolve.
                        
                        var foundMac = parts[1].Trim().ToLower();
                        if (!foundMacs.ContainsKey(foundMac))
                            foundMacs.Add(foundMac, hostnameOrAddress);
                        s = sr.ReadLine();
                    }
                }
            }

            return foundMacs;
        }

        private static void ClearARPCache()
        {
            ProcessStartInfo netsh = new ProcessStartInfo();
            netsh.FileName = "netsh.exe";
            netsh.Arguments = "interface ip delete arpcache";
            netsh.RedirectStandardOutput = true;
            netsh.UseShellExecute = false;
            netsh.CreateNoWindow = true;
            using (Process netshexe = Process.Start(netsh))
            {
                using (StreamReader sr = netshexe.StandardOutput)
                {
                    string s = sr.ReadLine();
                    if (s != "Ok.") throw new Exception("Can't clease ARP cache");
                    sr.Close();
                }
                netshexe.Close();
            }
        }

        /// <summary>
        /// Pings from current machine ip's basenet (10.0.1.1 to 
        /// </summary>
        private void PingAllSubnet(long startIPasLong, int upTo = 128)
        {
            for (int i = 1; i < 128; i++)
            {
                // ping every address in the DHCP pool, in a separate threads 
                IPAddress ip = new IPAddress(startIPasLong + i * 16777216);
                Thread t = new Thread(() => { try { Ping p = new Ping(); p.Send(ip, 1000); } catch { } });
                t.Start();
            }

            // Give all of the ping threads time to exit
            Thread.Sleep(1000);
        }

    }
}
