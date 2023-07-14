using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Компетентност към курс за валидиране
    /// </summary>
    [Table("Training_ValidationCompetency")]
    [Comment("Компетентност към курс за валидиране")]
    public class ValidationCompetency : IEntity, IModifiable
    {
        [Key]
        public int IdValidationCompetency { get; set; }
        public int IdEntity => IdValidationCompetency;

        [Comment("Връзка с обучаем от курс за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClient ValidationClient { get; set; }

        [Comment("Номер на компетентност")]
        public int CompetencyNumber { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Компетентност")]
        public string Competency { get; set; }

        [Comment("Дали се признава компетентността")]
        public bool IsCompetencyRecognized { get; set; }

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
