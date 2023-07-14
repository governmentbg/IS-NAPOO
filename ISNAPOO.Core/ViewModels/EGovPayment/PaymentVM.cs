using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.EGovPayment
{
    public class PaymentVM
    {     
        public int IdPayment { get; set; }
       
        public int IdCandidate_Provider { get; set; }
        public virtual CandidateProviderVM CandidateProvider { get; set; }
        public int IdProcedurePrice { get; set; }

        [Comment("Такси за лицензиране")]
        public ProcedurePriceVM ProcedurePrice { get; set; }

        [Comment("Вид на задължението")]
        public string ObligationType { get; set; }

        [Comment("Статус на плащане")]
        public int IdPaymentStatus { get; set; }//"Очаква плащане" Pending = 1, Платено с карта Authorized = 2,Платено по банков път Ordered = 3, Получено плащане Paid = 4, Изтекъл срок Expired = 5, Отказано Canceled = 6, Прекратена услуга Suspended = 7,В процес на обработка InProcess = 9

        [Comment("Статус на плащане име")]
        public string PaymentStatus { get; set; }

        [Comment("Статус на плащане - IntCode")]
        public string PaymentStatusIntCode { get; set; }

        [Comment("Статус на плащане - [DefaultValue1]")]
        public virtual string PaymentStatusDefVal { get; set; }

        [Comment("Доставчик на ЕАУ")]
        public string ServiceProviderName { get; set; }

        [Comment("Име на банката, в която е сметката на доставчика на ЕАУ")]
        public string ServiceProviderBank { get; set; }
       
        [Comment("BIC код на сметката на доставчика на ЕАУ")]
        public string ServiceProviderBIC { get; set; }

        [Comment("IBAN код на сметката на доставчика на ЕАУ")]
        public string ServiceProviderIBAN { get; set; }
           
        [Required]     
        public string Currency { get; set; }

        [Comment("Код на плащане")]
        public string? PaymentTypeCode { get; set; }

        [Required(ErrorMessage = "Полето 'Сума на задължението' е задължително!")]
        [Range(0.00, 999999999.99, ErrorMessage = "Сума на задължението трябва да е положително число!")]
        public double? PaymentAmount { get; set; }

        [Required(ErrorMessage = "Полето 'Основание за плащане' е задължително!")]
        [Comment("Основание за плащане")]
        public string PaymentReason { get; set; }

        [Comment("тип на идентификатора на задължено лице (\"1\", \"2\" или \"3\" -> ЕГН = 1, ЛНЧ = 2, БУЛСТАТ = 3)")]
        public int ApplicantUinTypeId { get; set; }

        [Comment("тип на идентификатора на задължено лице - [DefaultValue1] (1,2 или 3)")]
        public virtual string ApplicantUinIntDefVal { get; set; }

        [Comment("Идентификатор на задължено лице")]
        public string ApplicantUin { get; set; }

        [Required(ErrorMessage = "Полето 'Име на задължено лице' е задължително!")]
        [Comment("Име на задължено лице")]
        public string ApplicantName { get; set; }

        [Comment("Дата на изтичане на заявката за плащане")]
        [Required(ErrorMessage = "Полето 'Дата на изтичане на заявката за плащане' е задължително!")]
        public DateTime? ExpirationDate { get; set; }

        [Required(ErrorMessage = "Полето 'Допълнителна информация' е задължително!")]
        [Comment("Допълнителна информация")]
        public string AdditionalInformation { get; set; }
        
        [Comment("Тип на документ (референтен документ за плащане)")]
        public string ReferenceType { get; set; }
         
        [Comment("Номер на документ (референтен документ за\r\nплащане")]
        public string ReferenceNumber { get; set; }


        [Comment("Дата на документ (референтен документ за плащане)")]
        public DateTime? ReferenceDate { get; set; }


        [Comment("Номер на документ (референтен документ за\r\nплащане")]
        public string AdministrativeServiceUri { get; set; }


        [Comment("Номер на документ (референтен документ за\r\nплащане")]
        public string AdministrativeServiceSupplierUri { get; set; }
                

        [Comment("URL за нотификации при смяна на статус на\r\nзадължение.")]
        public string AdministrativeServiceNotificationURL { get; set; }

     
        [Comment("Номер на заявка в pay.egov.bg")]
        public string? ReceiptId { get; set; }


        [Comment("Дата на заявка в pay.egov.bg")]
        public DateTime? RegistrationTime { get; set; }

        [Comment("Време на промяна на статуса на заявката за плащане")]
        public DateTime? ChangeTime { get; set; }

        [Comment("Код за достъп, който се показва на екрана")]
        public string AccessCode { get; set; }

        [Comment("Съобщение, което ни връщат при различни заявки")]
        public string message { get; set; }

        public string PaymentOrderURl { get; set; }
        public string CreatePersonName { get; set; }
        public string ModifyPersonName { get; set; }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int IdModifyUser { get; set; }

        [Required]
        public DateTime ModifyDate { get; set; }
        #endregion

    }
}
