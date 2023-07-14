using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.EGovPayment
{

    public class PaymentStatusResult
    {
        public PaymentStatus[] paymentStatuses { get; set; }
    }

    public class PaymentStatus
    {
        public string Id { get; set; }

        /// <summary>
        /// “Pending” (Очаква плащане), 
        /// “Authorized” (Получена е авторизация от виртуалния ПОС терминал), 
        /// “Ordered” (Плащането е наредено), 
        /// “Paid” (Плащането е получено по сметката на доставчика), 
        /// “Expired” (Заявката за плащане е изтекла), 
        /// “Canceled” (Заявката за плащане е отказана от потребителя), 
        /// “Suspended” (Заявката за плащане е отказана от АИС)
        /// “InProcess” (В процес на обработка), 
        /// </summary>
        public string Status { get; set; }
        public DateTime ChangeTime { get; set; }
    }
}
