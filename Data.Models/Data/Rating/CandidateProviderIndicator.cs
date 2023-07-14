using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Control;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Rating
{
    /// <summary>
    /// Показател детайли
    /// </summary>
    [Table("Rating_CandidateProviderIndicator")]
    [Display(Name = "Показател детайли")]
    public class CandidateProviderIndicator : IEntity, IModifiable
    {
        public CandidateProviderIndicator()
        {

        }

        [Key]
        public int IdCandidateProviderIndicator { get; set; }
        public int IdEntity => IdCandidateProviderIndicator;

        [Comment("Показател вид")]
        public int IdIndicatorType { get; set; }// 

        [Comment("Година")]        
        public int Year { get; set; } 

        [Column(TypeName = "decimal(5, 2)")]
        [Comment("Точки")]
        public decimal Points { get; set; }

        [Comment("Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }

        [Comment("Връзка с Показател")]
        [ForeignKey(nameof(Indicator))]
        public int? IdIndicator { get; set; }

        public Indicator Indicator { get; set; }

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

