using Data.Models.Data.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Data.Models.Common;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Data.Models.Data.Candidate;

namespace Data.Models.Data.Request
{

    [Table("Request_RequestCandidateProviderDocument")]
    [Display(Name = "Серия за тип на документ")]
    public class RequestCandidateProviderDocument : AbstractUploadFile, IEntity, IModifiable, IDataMigration
    {
        [Key]
        public int IdRequestCandidateProviderDocument { get;set; }

        public int IdEntity => IdRequestCandidateProviderDocument;


        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string? UploadedFileName { get; set; }

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