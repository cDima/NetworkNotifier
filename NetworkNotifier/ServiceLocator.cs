using Hue;
using LanNotifier;

namespace NetworkNotifier
{
    public static class ServiceLocator
    {
        public static MacWatcher NetworkWatcher = new MacWatcher();
        public static GrowlNotifier Growl = new GrowlNotifier();
        public static HueBridge HueBridge = null;
    }
}
