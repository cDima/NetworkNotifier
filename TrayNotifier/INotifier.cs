using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrayNotifier
{
    public interface INotifier
    {
        void SendNotification(string title, string body);
    }
}
