using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Връзка между лектор и курс за валидиране
    /// </summary>
    [Table("Training_ValidationTrainer")]
    [Comment("Връзка между лектор и курс за валидиране")]
    public class ValidationTrainer : IEntity, IModifiable
    {
        [Key]
        public int IdValidationTrainer { get; set; }
        public int IdEntity => IdValidationTrainer;

        [Display(Name = "Връзка с Преподавател")]
        [ForeignKey(nameof(CandidateProviderTrainer))]
        public int IdTrainer { get; set; }

        public CandidateProviderTrainer CandidateProviderTrainer { get; set; }

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
