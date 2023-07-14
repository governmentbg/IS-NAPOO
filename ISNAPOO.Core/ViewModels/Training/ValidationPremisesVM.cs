using Data.Models.Data.Candidate;
using Data.Models.Data.Training;
using ISNAPOO.Core.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISNAPOO.Core.ViewModels.Candidate;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ValidationPremisesVM : IMapFrom<ValidationPremises>
    {
        public int IdValidationPremises { get; set; }

        [Display(Name = "Връзка с MTB")]
        [ForeignKey(nameof(CandidateProviderPremises))]
        public int IdPremises { get; set; }

        public CandidateProviderPremisesVM CandidateProviderPremises { get; set; }

        [Comment("Връзка с Курс за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClientVM ValidationClient { get; set; }

        [Comment("Вид обучение")]
        public int? IdТrainingType { get; set; } // Номенклатура - KeyTypeIntCode: "TrainingType"

        public string TrainingTypeName { get; set; }

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
