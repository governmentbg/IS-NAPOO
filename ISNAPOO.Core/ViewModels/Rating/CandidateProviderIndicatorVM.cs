using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models.Data.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Data.Models.Data.Rating;

namespace ISNAPOO.Core.ViewModels.Rating
{
    public class CandidateProviderIndicatorVM
    {
        public CandidateProviderIndicatorVM()
        {
            this.IndicatorDetails = new KeyValueVM();
            this.CandidateProvider = new CandidateProviderVM();
        }

        [Key]
        public int IdCandidateProviderIndicator { get; set; }


        [Comment("Показател вид")]
        public int IdIndicatorType { get; set; }
        public virtual KeyValueVM IndicatorDetails { get; set; }

        [Comment("Година")]
        public int Year { get; set; }
        public string YearAsString => this.Year.ToString();

        [Column(TypeName = "decimal(5, 2)")]
        [Comment("Точки")]
        public decimal Points { get; set; }

        [Comment("Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }
        public CandidateProviderVM CandidateProvider { get; set; }

        [Comment("Връзка с Показател")]
        [ForeignKey(nameof(Indicator))]
        public int? IdIndicator { get; set; }

        public IndicatorVM Indicator { get; set; }

        #region CandidateProvider

        public string CandidateProviderName => this.CandidateProvider.ProviderName;

        #endregion

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
