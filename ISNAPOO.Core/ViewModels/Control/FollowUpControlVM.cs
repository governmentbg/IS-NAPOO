using Data.Models.Data.Control;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Control
{
    public class FollowUpControlVM : IMapFrom<FollowUpControl>, IMapTo<FollowUpControl>
    {
        public FollowUpControlVM()
        {
            this.FollowUpControlExperts = new List<FollowUpControlExpertVM>();
            this.FollowUpControlDocuments = new List<FollowUpControlDocumentVM>();
            
        }
        public int IdFollowUpControl { get; set; }

        [Display(Name = "ЦПО")]
        public int? IdCandidateProvider { get; set; }

        public CandidateProviderVM CandidateProvider { get; set; }

        // Вид на последващия контрол - нова номенклатура "FollowUpControlType" (Периодичен, Текущ)
        [Required(ErrorMessage = "Полето 'Вид на последващия контрол' е задължително!")]
        [Display(Name = "Вид на последващия контрол")]
        public int? IdFollowUpControlType { get; set; }
        public string FollowUpControlTypeName { get; set; }

        // Вид на проверката - нова номенклатура "ControlType" (Комплексна, Тематична, Извънредна)
        [Required(ErrorMessage = "Полето 'Вид на проверката' е задължително!")]
        [Display(Name = "Вид на проверката")]
        public int? IdControlType { get; set; }
        public string ControlTypeName { get; set; }

        [Required(ErrorMessage = "Полето 'Срок на проверката от' е задължително!")]
        [Display(Name = "Срок на проверката от")]
        public DateTime? ControlStartDate { get; set; }

        [Required(ErrorMessage = "Полето 'Срок на проверката до' е задължително!")]
        [Display(Name = "Срок на проверката до")]
        public DateTime? ControlEndDate { get; set; }

        [Display(Name = "По документи в ИС на НАПОО и въз основа на допълнително изискани документи")]
        public bool IsFollowUpControlOnline { get; set; }

        [Display(Name = "На място")]
        public bool IsFollowUpControlOnsite { get; set; }

        [Display(Name = "Дата на последващ контрол от, само ако се провежда на място")]
        public DateTime? OnsiteControlDateFrom { get; set; }

        [Display(Name = "Дата на последващ контрол до, само ако се провежда на място")]
        public DateTime? OnsiteControlDateTo { get; set; }

        [Display(Name = "Срок за изпълнение на препоръки")]
        public DateTime? TermImplRecommendation { get; set; }

        [Required(ErrorMessage = "Полето 'Статус на проверката' е задължително!")]
        [Display(Name = "Статус на проверката")]
        public int? IdStatus { get; set; } ////Предстояща, Текуща, Приключила, Анулирана
        public string StatusName { get; set; } ////Предстояща, Текуща, Приключила, Анулирана

        [Display(Name = "Период от")]
        public DateTime? PeriodFrom { get; set; }

        [Display(Name = "Период до")]
        public DateTime? PeriodTo { get; set; }

        // Срок за отстраняване на констатираните нередовности/нарушения - нова номенклатура "ControlDeadlinePeriodType" (1 месец, 2 месеца, 3 месеца)
        [Comment("Срок за отстраняване на констатираните нередовности/нарушения")]
        public int? IdDeadlinePeriodType { get; set; }

        public List<FollowUpControlExpertVM> FollowUpControlExperts { get; set; }
               
        public List<FollowUpControlDocumentVM> FollowUpControlDocuments { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }

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
