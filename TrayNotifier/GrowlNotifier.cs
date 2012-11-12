using Growl.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayNotifier
{
    public class GrowlNotifier : INotifier
    {
        private GrowlConnector growl;
        private NotificationType notificationType;
        private Growl.Connector.Application application;
        private string sampleNotificationType = "MacNotifications";

        public GrowlNotifier()
        {
            this.notificationType = new NotificationType(sampleNotificationType, "Network ARP change notification");

            this.growl = new GrowlConnector();
            //this.growl = new GrowlConnector("password");    // use this if you need to set a password - you can also pass null or an empty string to this constructor to use no password
            //this.growl.NotificationCallback += new GrowlConnector.CallbackEventHandler(growl_NotificationCallback);
            // set this so messages are sent in plain text (easier for debugging)

            this.growl.EncryptionAlgorithm = Cryptography.SymmetricAlgorithmType.PlainText;

            this.application = new Growl.Connector.Application("MacNotifier");

            this.growl.Register(application, new NotificationType[] { notificationType });
        }

        public void SendNotification(string title, string body)
        {
            Notification notification = new Notification(this.application.Name, this.notificationType.Name, DateTime.Now.Ticks.ToString(), title, body);
            this.growl.Notify(notification);
        }
    }
}