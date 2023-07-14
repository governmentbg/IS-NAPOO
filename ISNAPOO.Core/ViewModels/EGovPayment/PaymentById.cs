using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.EGovPayment
{
    public class PaymentRequest
    {
        public int id { get; set; }
        public RequestJson requestJson { get; set; }
    }

    public class RequestJson
    {
        public object aisPaymentId { get; set; }
        public string serviceProviderName { get; set; }
        public string serviceProviderBank { get; set; }
        public string serviceProviderBIC { get; set; }
        public string serviceProviderIBAN { get; set; }
        public string currency { get; set; }
        public object paymentTypeCode { get; set; }
        public object obligationType { get; set; }
        public double paymentAmount { get; set; }
        public string paymentReason { get; set; }
        public int applicantUinTypeId { get; set; }
        public string applicantUin { get; set; }
        public string applicantName { get; set; }
        public string paymentReferenceType { get; set; }
        public string paymentReferenceNumber { get; set; }
        public DateTime paymentReferenceDate { get; set; }
        public DateTime expirationDate { get; set; }
        public string additionalInformation { get; set; }
        public object administrativeServiceUri { get; set; }
        public object administrativeServiceSupplierUri { get; set; }
        public object administrativeServiceNotificationURL { get; set; }
    }

    public class PaymentById
    {
        public List<PaymentRequest> paymentRequests { get; set; }
    }
}
