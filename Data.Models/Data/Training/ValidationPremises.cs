using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Връзка между MTB и курс за валидиране
    /// </summary>
    [Table("Training_ValidationPremises")]
    [Comment("Връзка между MTB и курс за валидиране")]
    public class ValidationPremises : IEntity, IModifiable
    {
        [Key]
        public int IdValidationPremises { get; set; }
        public int IdEntity => IdValidationPremises;

        [Display(Name = "Връзка с MTB")]
        [ForeignKey(nameof(CandidateProviderPremises))]
        public int IdPremises { get; set; }

        public CandidateProviderPremises CandidateProviderPremises { get; set; }

        [Comment("Връзка с Курс за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClient ValidationClient { get; set; }

        [Comment("Вид обучение")]
        public int? IdТrainingType { get; set; } // Номенклатура - KeyTypeIntCode: "TrainingType"

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
