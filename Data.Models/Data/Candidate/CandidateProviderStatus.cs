using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Candidate
{


    /// <summary>
    /// Статуси на Кандидат Обучаваща институция
    /// </summary>
    [Table("Candidate_ProviderStatus")]
    [Display(Name = "Статуси на Кандидат Обучаваща институция")]
    public class CandidateProviderStatus : IEntity
    {
        public CandidateProviderStatus()
        {

        }

        [Key]
        public int IdCandidateProviderStatus { get; set; }
        public int IdEntity => IdCandidateProviderStatus;

        [Display(Name = "Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }


        [Display(Name = "Статус")]       
        public int IdStatus { get; set; }

        [Display(Name = "Дата на Статус")]
        public DateTime StatusDate { get; set; }


    }
}

