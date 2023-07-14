using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.CPO.ProviderData
{
    public class StartedProcedureVM : IMapFrom<StartedProcedure>, IMapTo<StartedProcedure>
    {
        public StartedProcedureVM()
        {
            this.StartedProcedureProgresses = new List<StartedProcedureProgressVM>();
            this.ProcedureDocuments = new List<ProcedureDocumentVM>();
            this.ProcedureExternalExperts = new List<ProcedureExternalExpertVM>();
            this.ProcedureExpertCommissions = new List<ProcedureExpertCommissionVM>();
            this.CandidateProviders = new List<CandidateProviderVM>();
            this.NegativeIssues = new List<NegativeIssueVM>();
        }

        [Key]
        public int IdStartedProcedure { get; set; }

        [Display(Name = "Дата на заявката")]
        public DateTime? TS { get; set; }

        [Display(Name = "краен срок на доклад за резултата от проверката на редовността на подаденото заявление и документи за изменение на издадена лицензия на ЦПО")]
        public DateTime? NapooReportDeadline { get; set; }


        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Час на заседание в Писмо-покана за заседание на ЕК ' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Час на заседание в Писмо-покана за заседание на ЕК ")]
        public string? MeetingHour { get; set; }

        [Display(Name = "Дата на заседание в Писмо-покана за заседание на ЕК ")]
        public DateTime? MeetingDate { get; set; }


        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Приложение към издадена лицензия - Номер на лицензия' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Приложение към издадена лицензия - Номер на лицензия")]
        public string? LicenseNumber { get; set; }

        [Display(Name = "Приложение към издадена лицензия - Дата на издаване")]
        public DateTime? LicenseDate { get; set; }


        [Display(Name = "Срок за представяне на доклад на външния експерт ")]
        public DateTime? ExpertReportDeadline { get; set; }

        public long? oldProviderId { get; set; }

        public long OldId { get; set; }

        public virtual ICollection<StartedProcedureProgressVM> StartedProcedureProgresses { get; set; }
        public virtual ICollection<ProcedureDocumentVM> ProcedureDocuments { get; set; }
        public virtual ICollection<ProcedureExternalExpertVM> ProcedureExternalExperts { get; set; }
        public virtual ICollection<ProcedureExpertCommissionVM> ProcedureExpertCommissions { get; set; }
        public virtual ICollection<CandidateProviderVM> CandidateProviders { get; set; }
        public virtual ICollection<NegativeIssueVM> NegativeIssues { get; set; }


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
