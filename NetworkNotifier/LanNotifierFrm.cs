using Hue;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WifiNotificator;

namespace NetworkNotifier
{
    public partial class LanNotifier : Form
    {
        bool mAllowVisible;     // ContextMenu's Show command used
        bool mAllowClose;       // ContextMenu's Exit command used
        bool mLoadFired;        // Form was shown once


        public LanNotifier()
        {
            InitializeComponent();

            var networkWatcher = ServiceLocator.NetworkWatcher;
            networkWatcher.FirstAddressesLoaded += networkWatcher_FirstAddressesLoaded;
            networkWatcher.MacConnected += new EventHandler<KeyValuePair<string, string>>(networkWatcher_MacConnected);
            networkWatcher.MacDisconnected += new EventHandler<KeyValuePair<string, string>>(networkWatcher_MacDisconnected);
            networkWatcher.WatchNetworkAsync();
        }

        async void networkWatcher_FirstAddressesLoaded(object sender, Dictionary<string, string> e)
        {
            ServiceLocator.HueBridge = await HueBridgeLocator.LocateBridge(e.Values.ToList());
            if (ServiceLocator.HueBridge != null)
            {
                ServiceLocator.HueBridge.PushButtonOnBridge += HueBridge_PushButtonOnBridge;
                ServiceLocator.HueBridge.InitializeRouter();
            }
        }

        void HueBridge_PushButtonOnBridge(object sender, EventArgs e)
        {
            systemTrayIcon.ShowBalloonTip(60 * 1000, "Philips Hue Bridge Found", "Please press the button on the bridge in the next minute.", ToolTipIcon.Info);
        }

        void networkWatcher_MacConnected(object sender, KeyValuePair<string, string> mac)
        {
            //var name = IPUtil.TryResolveName(mac.Value);
            var title = "Device Connected";

            this.systemTrayIcon.ShowBalloonTip(2000, title, mac.Value, ToolTipIcon.Info);
            
            ServiceLocator.Growl.SendNotification(title, mac.Value);
            
            if (ServiceLocator.HueBridge != null) 
                ServiceLocator.HueBridge.FlashLights();
            
            Trace.WriteLine(DateTime.Now.ToShortTimeString() + " mac newly connected: " + mac);
        }

        void networkWatcher_MacDisconnected(object sender, KeyValuePair<string, string> mac)
        {
            this.systemTrayIcon.ShowBalloonTip(2000, "Device Disconnected", mac.Value, ToolTipIcon.Info);
            
            ServiceLocator.Growl.SendNotification("Device Disconnected", mac.Value);
            
            if (ServiceLocator.HueBridge != null)
                ServiceLocator.HueBridge.FlashLights();

            Trace.WriteLine(DateTime.Now.ToShortTimeString() + " mac disconnected: " + mac);
        }

        protected override void SetVisibleCore(bool value)
        {
            if (!mAllowVisible) value = false;
            base.SetVisibleCore(value);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!mAllowClose)
            {
                this.Hide();
                e.Cancel = true;
            }
            base.OnFormClosing(e);
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mAllowVisible = true;
            mLoadFired = true;
            Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mAllowClose = mAllowVisible = true;
            if (mLoadFired) 
                Close(); 
            else 
                Application.Exit();
        }

        private void flashHueLightsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ServiceLocator.HueBridge != null)
                ServiceLocator.HueBridge.FlashLights();
        }

        private void turnOffLightsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ServiceLocator.HueBridge != null)
                ServiceLocator.HueBridge.TurnOffLights();
        }
    }
}
