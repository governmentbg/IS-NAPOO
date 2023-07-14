using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Candidate
{


    /// <summary>
    /// Лице връзка с CPO,CIPO - Обучаваща институция"
    /// </summary>
    [Table("Candidate_ProviderPerson")]
    [Display(Name = "Лице връзка с CPO,CIPO - Обучаваща институция")]
    public class CandidateProviderPerson : IEntity
    {
        public CandidateProviderPerson()
        {
            
        }

        [Key]
        public int IdCandidateProviderPerson { get; set; }
        public int IdEntity => IdCandidateProviderPerson;

        [Display(Name = "Връзка с лице")]
        [ForeignKey(nameof(Person))]
        public int IdPerson { get; set; }
        public Person Person { get; set; }

        [Display(Name = "Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }

        [Comment("Използва се при изпращането на официални съобщения към ЦПО")]
        public bool IsAllowedForNotification { get; set; }

        [Display(Name = "Потребителят е администратор на профила на ЦПО/ЦИПО")]
        public bool IsAdministrator { get; set; }
    }
}
