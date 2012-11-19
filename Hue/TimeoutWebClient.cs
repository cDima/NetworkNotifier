using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Hue
{
    public class TimeoutWebClient : WebClient
    {
        private int _timeout;
        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        public TimeoutWebClient()
        {
            this._timeout = 1000;
        }

        public TimeoutWebClient(int timeout)
        {
            this._timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var result = base.GetWebRequest(address);
            result.Timeout = this._timeout;
            return result;
        }
    }
}
