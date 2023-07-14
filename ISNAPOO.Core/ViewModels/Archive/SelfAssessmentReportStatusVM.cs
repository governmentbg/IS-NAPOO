using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;


namespace ISNAPOO.Core.ViewModels.Archive
{
    public class SelfAssessmentReportStatusVM
    {
        public int IdSelfAssessmentReportStatus { get; set; }

        [Comment("Връзка с доклад за самооценка")]
        public int IdSelfAssessmentReport { get; set; }

        public virtual SelfAssessmentReportVM SelfAssessmentReport { get; set; }

        // Номенклатура: KeyTypeIntCode - "SelfAssessmentReportStatus"
        [Comment("Статус на доклад за самооценка")]
        public int IdStatus { get; set; }

        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Коментар при операция с доклад за самооценка")]
        public string? Comment { get; set; }


        /// <summary>        
        /// Създаден,Подаден,Одобрен,Върнат
        /// </summary>
        public string StatusValue { get; set; }

        /// <summary>
        /// Created,Submitted,Approved,Returned        
        /// </summary>
        public string StatusValueIntCode { get; set; }

        //Потребител
        public string PersonName { get; set; }

        public DateTime? StatusDate { get; set; }


        public string UploadedFileName { get; set; }
        public string FileName { get; set; }

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
