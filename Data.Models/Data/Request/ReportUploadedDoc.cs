using System;
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
    
    [Table("Request_ReportUploadedDoc")]
    [Display(Name = "Прикачени документи към отчети")]
    public class ReportUploadedDoc : AbstractUploadFile, IEntity, IModifiable
    {
        [Key]
        public int IdReportUploadedDoc { get; set; }
        public int IdEntity => IdReportUploadedDoc;

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }


        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        [ForeignKey(nameof(RequestReport))]
        public int IdRequestReport { get; set; }
        public RequestReport RequestReport { get; set; }

        /// <summary>
        /// Отчет на документи с фабрична номерация
        /// Протокол за унищожени документи с фабрична номерация        
        /// Приемно-предавателен протокол за документи с фабрична номерация           
        /// </summary>
        [Comment("Тип документ")]        
        public int IdTypeReportUploadedDocument { get; set; }//Нова номенклатура TypeReportUploadedDocument: 

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string? UploadedFileName { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Описание")]
        public string Description { get; set; }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public override int IdModifyUser { get; set; }

        [Required]
        public override DateTime ModifyDate { get; set; }
        #endregion
        #region IDataMigration
        public int? OldId { get; set; }

        public override string? MigrationNote { get; set; }
        #endregion
    }
}

