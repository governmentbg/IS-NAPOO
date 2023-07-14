using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Candidate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class RequestReportVM
    {
        public RequestReportVM()
        {
            this.ReportUploadedDocs = new HashSet<ReportUploadedDocVM>();
            this.DocumentSerialNumbers = new HashSet<DocumentSerialNumberVM>();
        }

        public int IdRequestReport { get; set; }

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Required(ErrorMessage = "Полето 'Година' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Година' е задължително!")]
        public int? Year { get; set; }

        /// <summary>
        /// създаден
        /// върнат
        /// одобрен
        /// подаден
        /// </summary>
        [Comment("Статус на одобрение на отчета")]
        public int IdStatus { get; set; }//Нова номенклатура Статус на одобрение на отчета:  RequestReportStatus

        public string StatusName { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на унищожаване' е задължително!")]
        [Comment("Дата на унищожаване")]
        public DateTime? DestructionDate { get; set; } = DateTime.Today;

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }

        public virtual ICollection<ReportUploadedDocVM> ReportUploadedDocs { get; set; }

        public virtual ICollection<DocumentSerialNumberVM> DocumentSerialNumbers { get; set; }

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
