using System;
using System.Collections.Generic;
using System.Text;

namespace ISNAPOO.Common.HelperClasses
{
    public class RequestMeasure
    {
        public string GUID { get; set; }
        private DateTime startRequest;

        public DateTime StartRequest
        {
            get { return startRequest; }
            set { startRequest = value; }
        }
        private DateTime endRequest;

        public DateTime EndRequest
        {
            get { return endRequest; }
            set { endRequest = value; }
        }
        private string pageName;

        public string PageName
        {
            get { return pageName; }
            set { pageName = value; }
        }

        public RequestMeasure(string _pageName)
        {

            this.pageName = _pageName;
            //GUID = Guid.NewGuid().ToString();
            startRequest = DateTime.Now;

        }
        public double Duration
        {
            get
            {
                endRequest = DateTime.Now;
                return (endRequest - startRequest).TotalMilliseconds;
            }
        }

        public override string ToString()
        {
            return String.Format("Page:{0,-60}\tDuration:\t{1,10} ms\t{2,-50}", this.pageName, this.Duration.ToString("N2"), "(" + startRequest.ToString("hh:mm:ss.ffff ") + " - " + endRequest.ToString("hh:mm:ss.ffff") + ")");
        }


    }
}
