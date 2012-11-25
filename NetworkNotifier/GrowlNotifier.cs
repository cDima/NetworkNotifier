using Growl.Connector;
using System;

namespace NetworkNotifier
{
    public class GrowlNotifier
    {
        private GrowlConnector growl;
        private NotificationType notificationType;
        private Growl.Connector.Application application;
        private string sampleNotificationType = "MacNotifications";

        public GrowlNotifier()
        {
            notificationType = new NotificationType(sampleNotificationType, "Network ARP change notification");

            growl = new GrowlConnector();
            //this.growl = new GrowlConnector("password");    // use this if you need to set a password - you can also pass null or an empty string to this constructor to use no password
            //this.growl.NotificationCallback += new GrowlConnector.CallbackEventHandler(growl_NotificationCallback);
            // set this so messages are sent in plain text (easier for debugging)

            growl.EncryptionAlgorithm = Cryptography.SymmetricAlgorithmType.PlainText;

            application = new Application("MacNotifier");

            growl.Register(application, new NotificationType[] { notificationType });
        }

        public void SendNotification(string title, string body)
        {
            Notification notification = new Notification(this.application.Name, this.notificationType.Name, DateTime.Now.Ticks.ToString(), title, body);
            growl.Notify(notification);
        }
    }
}