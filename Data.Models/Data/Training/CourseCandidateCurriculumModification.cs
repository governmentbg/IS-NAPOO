using Data.Models.Data.Candidate;
using Data.Models.Data.DOC;
using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Връзка между курс и промяна на учебен план
    /// </summary>
    [Table("Training_CourseCandidateCurriculumModification")]
    [Comment("Връзка между курс и промяна на учебен план")]
    public class CourseCandidateCurriculumModification : IEntity, IDataMigration
    {
        [Key]
        public int IdCourseCandidateCurriculumModification { get; set; }

        public int IdEntity => this.IdCourseCandidateCurriculumModification;

        [Required]
        [Comment("Връзка с курс за обучение, предлаган от ЦПО")]
        [ForeignKey(nameof(Course))]
        public int IdCourse { get; set; }

        public virtual Course Course { get; set; }

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
