using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Control
{
    /// <summary>
    /// Последващ контрол, изпълняван от служител/и на НАПОО
    /// </summary>
    [Table("Control_FollowUpControl")]
    [Comment("Последващ контрол, изпълняван от служител/и на НАПОО")]
    public class FollowUpControl : IEntity, IModifiable
    {
        public FollowUpControl()
        {
            this.FollowUpControlExperts = new HashSet<FollowUpControlExpert>();
            this.FollowUpControlDocuments = new HashSet<FollowUpControlDocument>();
            this.FollowUpControlUploadedFiles = new HashSet<FollowUpControlUploadedFile>();
        }

        [Key]
        public int IdFollowUpControl { get; set; }
        public int IdEntity => IdFollowUpControl;

        [Required] 
        [Comment("Връзка с проверяван ЦПО/ЦИПО")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }

        public CandidateProvider CandidateProvider { get; set; }

        // Вид на последващия контрол - нова номенклатура "FollowUpControlType" (Периодичен, Текущ)
        [Comment("Връзка с номенклатура за вид на последващия контрол")]
        public int IdFollowUpControlType { get; set; }

        // Вид на проверката - нова номенклатура "ControlType" (Комплексна, Тематична, Извънредна)
        [Comment("Връзка с номенклатура за вид на проверката")]
        public int IdControlType { get; set; }

        [Comment("Срок на проверката от")]
        public DateTime ControlStartDate { get; set; }

        [Comment("Срок на проверката до")]
        public DateTime ControlEndDate { get; set; }

        [Comment("По документи в ИС на НАПОО и въз основа на допълнително изискани документи - чек бокс")]
        public bool IsFollowUpControlOnline { get; set; }

        [Comment("Последващият контрол се извършва на място")]
        public bool IsFollowUpControlOnsite { get; set; }

        [Comment("Дата на последващ контрол от, само ако се провежда на място")]
        public DateTime? OnsiteControlDateFrom { get; set; }

        [Comment("Дата на последващ контрол до, само ако се провежда на място")]
        public DateTime? OnsiteControlDateTo { get; set; }

        [Comment("Срок за изпълнение на препоръки")]
        public DateTime? TermImplRecommendation { get; set; }

        [Comment("Период от")]
        public DateTime? PeriodFrom { get; set; }

        [Comment("Период до")]
        public DateTime? PeriodTo { get; set; }

        /// <summary>
        /// Статус на проверката
        /// </summary>
        [Comment("Статус на проверката")]
        public int? IdStatus { get; set; } ////Предстояща, Текуща, Приключила, Анулирана

        // Срок за отстраняване на констатираните нередовности/нарушения - нова номенклатура "ControlDeadlinePeriodType" (1 месец, 2 месеца, 3 месеца)
        [Comment("Срок за отстраняване на констатираните нередовности/нарушения")]
        public int? IdDeadlinePeriodType { get; set; }

        public ICollection<FollowUpControlExpert> FollowUpControlExperts { get; set; }

        public ICollection<FollowUpControlDocument> FollowUpControlDocuments { get; set; }

        public ICollection<FollowUpControlUploadedFile> FollowUpControlUploadedFiles { get; set; }

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
