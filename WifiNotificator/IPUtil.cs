using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WifiNotificator
{
    public static class IPUtil
    {
        public static IPAddress GetMyIpAddress()
        {
            var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            return (from addr in hostEntry.AddressList
                    where addr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork // IPv4
                    select addr).First();
        }

        /// <summary>
        /// Converts 10.0.1.254 to 10.0.1.0
        /// </summary>
        /// <param name="myip"></param>
        /// <returns></returns>
        public static long ConvertToBaseIP(IPAddress myip)
        {
            byte[] bytes = myip.GetAddressBytes().Reverse().ToArray();
            return (long)(
                // all but the last byte: 16777216 * (long)bytes[0] +
                65536 * (long)bytes[1] +
                256 * (long)bytes[2] +
                (long)bytes[3]);
        }

        public static string TryResolveName(string hostnameOrAddress)
        {
            try
            {
                var hostname = Dns.GetHostEntry(hostnameOrAddress);
                return hostname.HostName ?? hostnameOrAddress;
            }catch (System.Net.Sockets.SocketException ex)
            {
                return hostnameOrAddress;
            }
        }
    }
}
