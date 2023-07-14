using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Models.Data.ProviderData
{


    /// <summary>
    /// Данни за Процедура за лицензиране 
    /// </summary>
    [Table("Procedure_StartedProcedure")]
    [Display(Name = " Данни за Процедура за лицензиране")]
    public class StartedProcedure : IEntity, IModifiable, IDataMigration
    {
        public StartedProcedure()
        {
            this.StartedProcedureProgresses = new List<StartedProcedureProgress>();
            this.ProcedureDocuments = new List<ProcedureDocument>();
            this.ProcedureExternalExperts = new List<ProcedureExternalExpert>();
            this.ProcedureExpertCommissions = new List<ProcedureExpertCommission>();
            this.CandidateProviders = new List<CandidateProvider>();
            this.NegativeIssues = new List<NegativeIssue>();
        }

        [Key]
        public int IdStartedProcedure { get; set; }
        public int IdEntity => IdStartedProcedure;


        [Comment("Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        //[ForeignKey(nameof(CandidateProvider))]
        public int? IdCandidate_Provider { get; set; }
        //public CandidateProvider CandidateProvider { get; set; }



        [Comment("Дата на заявката")]
        public DateTime? TS { get; set; }

        [Comment("Kраен срок на доклад на експертната комисия")]
        public DateTime? NapooReportDeadline { get; set; }


        [StringLength(DBStringLength.StringLength255)]
        [Comment( "Час на заседание в Писмо-покана за заседание на ЕК ")]
        public string? MeetingHour { get; set; }

        [Comment( "Дата на заседание в Писмо-покана за заседание на ЕК ")]
        public DateTime? MeetingDate { get; set; }


        [StringLength(DBStringLength.StringLength255)]
        [Comment("Приложение към издадена лицензия - Номер на лицензия")]
        public string? LicenseNumber { get; set; }
        
        [Comment("Приложение към издадена лицензия - Дата на издаване")]
        public DateTime? LicenseDate { get; set; }
      

        [Comment("Срок за представяне на доклад на външния експерт ")]
        public DateTime? ExpertReportDeadline { get; set; }

        //vc_negative_issues -  установени непълноти и неточности

        public virtual ICollection<StartedProcedureProgress> StartedProcedureProgresses { get; set; }
        public virtual ICollection<NegativeIssue> NegativeIssues { get; set; }
        public virtual ICollection<ProcedureDocument> ProcedureDocuments { get; set; }
        public virtual ICollection<ProcedureExternalExpert> ProcedureExternalExperts { get; set; }
        public virtual ICollection<ProcedureExpertCommission> ProcedureExpertCommissions { get; set; }
        public virtual ICollection<CandidateProvider> CandidateProviders { get; set; }


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
