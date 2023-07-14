using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Control;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.EGovPayment
{
    /// <summary>
    /// Плащане
    /// </summary>
    [Table("Procedure_Payment")]
    [Display(Name = "Плащане")]
    public class Payment : IEntity, IModifiable
    {
        public Payment()
        {
        }

        [Key]
        public int IdPayment { get; set; }
        public int IdEntity => IdPayment;

        [Comment("Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }


        [Comment("Такси за лицензиране")]
        [ForeignKey(nameof(ProcedurePrice))]
        public int IdProcedurePrice { get; set; }
        public ProcedurePrice ProcedurePrice { get; set; }



        [Comment("Статус на плащане")]
        public int IdPaymentStatus { get; set; }//"Очаква плащане" Pending = 1, Платено с карта Authorized = 2,Платено по банков път Ordered = 3, Получено плащане Paid = 4, Изтекъл срок Expired = 5, Отказано Canceled = 6, Прекратена услуга Suspended = 7,В процес на обработка InProcess = 9



        [StringLength(DBStringLength.StringLength512)]
        [Comment("Доставчик на ЕАУ")]
        public string ServiceProviderName { get; set; }


        [StringLength(DBStringLength.StringLength255)]
        [Comment("Име на банката, в която е сметката на доставчика на ЕАУ")]
        public string ServiceProviderBank { get; set; }

        [StringLength(DBStringLength.StringLength50)]
        [Comment("BIC код на сметката на доставчика на ЕАУ")]
        public string ServiceProviderBIC { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Comment("IBAN код на сметката на доставчика на ЕАУ")]
        public string ServiceProviderIBAN { get; set; }


        [StringLength(DBStringLength.StringLength5)]     
        public string Currency { get; set; }


        [StringLength(DBStringLength.StringLength20)]
        [Comment("Код на плащане")]
        public string? PaymentTypeCode { get; set; }



        [Column(TypeName = "decimal(10, 2)")]
        [Comment("Сума на задължението (десетичен разделител \".\", до 2 символа след десетичния разделител, пр. \"2.33\")")]
        public double? PaymentAmount { get; set; }


        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Основание за плащане")]
        public string PaymentReason { get; set; }

        [Comment("тип на идентификатора на задължено лице (\"1\", \"2\" или \"3\" -> ЕГН = 1, ЛНЧ = 2, БУЛСТАТ = 3)")]
        public int ApplicantUinTypeId { get; set; }

        [StringLength(DBStringLength.StringLength50)]
        [Comment("Идентификатор на задължено лице")]
        public string ApplicantUin { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Comment("Име на задължено лице")]
        public string ApplicantName { get; set; }
      
        [Comment("Дата на изтичане на заявката за плащане")]
        public DateTime? ExpirationDate { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Допълнителна информация")]
        public string? AdditionalInformation { get; set; }


        [StringLength(DBStringLength.StringLength100)]
        [Comment("Тип на документ (референтен документ за плащане)")]
        public string? ReferenceType { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Номер на документ (референтен документ за\r\nплащане")]
        public string? ReferenceNumber { get; set; }


        [Comment("Дата на документ (референтен документ за плащане)")]
        public DateTime? ReferenceDate { get; set; }



        [StringLength(DBStringLength.StringLength100)]
        [Comment("Номер на заявка в pay.egov.bg")]
        public string? ReceiptId { get; set; }


        [Comment("Дата на заявка в pay.egov.bg")]
        public DateTime? RegistrationTime { get; set; }


        [Comment("Време на промяна на статуса на заявката за плащане")]
        public DateTime? ChangeTime { get; set; }



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

