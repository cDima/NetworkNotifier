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

namespace TrayNotifier
{
    public partial class LanNotifier : Form
    {
        bool mAllowVisible;     // ContextMenu's Show command used
        bool mAllowClose;       // ContextMenu's Exit command used
        bool mLoadFired;        // Form was shown once

        NetworkMACWatcher networkWatcher;
        GrowlNotifier growl;

        public LanNotifier()
        {
            InitializeComponent();

            networkWatcher = new NetworkMACWatcher();
            networkWatcher.MacConnected += new EventHandler<KeyValuePair<string, string>>(networkWatcher_MacConnected);
            networkWatcher.MacDisconnected += new EventHandler<KeyValuePair<string, string>>(networkWatcher_MacDisconnected);
            networkWatcher.WatchNetworkAsync();

            growl = new GrowlNotifier();
        }

        void networkWatcher_MacConnected(object sender, KeyValuePair<string, string> mac)
        {
            //var name = IPUtil.TryResolveName(mac.Value);
            var title = "Device Connected";
            this.systemTrayIcon.ShowBalloonTip(2000, title, mac.Value, ToolTipIcon.Info);
            growl.SendNotification(title, mac.Value);
            Trace.WriteLine(DateTime.Now.ToShortTimeString() + " mac newly connected: " + mac);
        }

        void networkWatcher_MacDisconnected(object sender, KeyValuePair<string, string> mac)
        {
            this.systemTrayIcon.ShowBalloonTip(2000, "Device Disconnected", mac.Value, ToolTipIcon.Info);
            growl.SendNotification("Device Disconnected", mac.Value);
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
    }
}
