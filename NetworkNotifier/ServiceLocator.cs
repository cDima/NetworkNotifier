using Hue;
using WifiNotificator;
namespace NetworkNotifier
{
    public static class ServiceLocator
    {
        public static NetworkMACWatcher NetworkWatcher = new NetworkMACWatcher();
        public static GrowlNotifier Growl = new GrowlNotifier();
        public static HueBridge HueBridge = null;
    }
}
