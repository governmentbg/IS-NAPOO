using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderPersonVM : IMapFrom<CandidateProviderPerson>, IMapTo<CandidateProviderPerson>
    {
        public CandidateProviderPersonVM()
        {
            this.Person = new PersonVM();
        }

        [Key]
        public int IdCandidateProviderPerson { get; set; }

        [Display(Name = "Връзка с лице")]
        public int IdPerson { get; set; }
        public PersonVM Person { get; set; }

        [Display(Name = "CPO,CIPO - Кандидат Обучаваща институция")]         
        public int IdCandidate_Provider { get; set; }
        public CandidateProviderVM CandidateProvider { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }

        public string Username { get; set; }
        public KeyValueVM Status { get; set; }

        [Comment("Използва се при изпращането на официални съобщения към ЦПО")]
        public bool IsAllowedForNotification { get; set; }

        [Display(Name = "Потребителят е администратор на профила на ЦПО/ЦИПО")]
        public bool IsAdministrator { get; set; }

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
