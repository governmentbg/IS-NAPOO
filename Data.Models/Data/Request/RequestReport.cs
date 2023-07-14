using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.Request
{
    /// <summary>
    /// Отчет на документи с фабрична номерация по наредба 8
    [Table("Request_Report")]
    [Display(Name = "Отчет на документи с фабрична номерация по наредба 8")]
    public class RequestReport : IEntity, IModifiable
    {
        public RequestReport()
        {
            this.ReportUploadedDocs = new HashSet<ReportUploadedDoc>();
            this.DocumentSerialNumbers = new HashSet<DocumentSerialNumber>();
        }

        [Key]
        public int IdRequestReport { get; set; }
        public int IdEntity => IdRequestReport;

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }

        [Required]
        public int Year { get; set; }

        /// <summary>
        /// върнат
        /// одобрен
        /// подаден
        /// </summary>
        [Comment("Статус на одобрение на отчета")]
        public int IdStatus { get; set; }//Нова номенклатура Статус на одобрение на отчета:  RequestReportStatus

        
        [Comment("Дата на унищожаване")]
        public DateTime DestructionDate { get; set; }

        public virtual ICollection<ReportUploadedDoc> ReportUploadedDocs { get; set; }

        public virtual ICollection<DocumentSerialNumber> DocumentSerialNumbers { get; set; }

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
        #region IDataMigration
        public int? OldId { get; set; }

        public string? MigrationNote { get; set; }
        #endregion
    }
}
