using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.EGovPayment
{
    public class ReceiptResult
    {
        public Unacceptedreceiptjson unacceptedReceiptJson { get; set; }
        public Acceptedreceiptjson acceptedReceiptJson { get; set; }
    }

    public class Unacceptedreceiptjson
    {
        public string message { get; set; }
        public DateTime validationTime { get; set; }
        public string[] errors { get; set; }
    }

    public class Acceptedreceiptjson
    {
        public string id { get; set; }
        public DateTime registrationTime { get; set; }
    }
}
