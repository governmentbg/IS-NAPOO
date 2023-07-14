using Data.Models.Data.Candidate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Framework;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Връзка между валидирано лице и промяна на учебен план
    /// </summary>
    [Table("Training_ValidationClientCandidateCurriculumModification")]
    [Comment("Връзка между валидирано лице и промяна на учебен план")]
    public class ValidationClientCandidateCurriculumModification : IEntity, IDataMigration
    {
        [Key]
        public int IdValidationClientCandidateCurriculumModification { get; set; }

        public int IdEntity => this.IdValidationClientCandidateCurriculumModification;

        [Required]
        [Comment("Връзка с валидирано лице")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public virtual ValidationClient ValidationClient { get; set; }

        [Required]
        [Comment("Връзка с Програмa за обучение, предлагани от ЦПО")]
        [ForeignKey(nameof(CandidateCurriculumModification))]
        public int IdCandidateCurriculumModification { get; set; }

        public virtual CandidateCurriculumModification CandidateCurriculumModification { get; set; }

        #region IDataMigration
        public int? OldId { get; set; }

        public string? MigrationNote { get; set; }
        #endregion
    }
}
