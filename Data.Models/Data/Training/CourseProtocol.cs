using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Протокол към курс за обучение, предлагани от ЦПО
    /// </summary>
    [Table("Training_CourseProtocol")]
    [Comment("Протокол към курс за обучение, предлагани от ЦПО")]
    public class CourseProtocol : AbstractUploadFile, IEntity, IModifiable
    {
        public CourseProtocol()
        {
            this.CourseProtocolGrades = new HashSet<CourseProtocolGrade>();
            this.ClientCourseDocuments = new HashSet<ClientCourseDocument>();
        }

        [Key]
        public int IdCourseProtocol { get; set; }
        public int IdEntity => IdCourseProtocol;

        [Comment("Връзка с курс за обучение, предлагани от ЦПО")]
        [ForeignKey(nameof(Course))]
        public int IdCourse { get; set; }

        public Course Course { get; set; }

        [Comment("Връзка с ЦПО")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }

        public CandidateProvider CandidateProvider { get; set; }

        [Comment("Връзка с член на изпитна комисия към курс за обучение")] 
        [ForeignKey(nameof(CourseCommissionMember))]
        public int? IdCourseCommissionMember { get; set; }

        public CourseCommissionMember CourseCommissionMember { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength50)]
        [Comment("Номер на протокол")]
        public string CourseProtocolNumber { get; set; }

        [Required]
        [Comment("Дата на протокол")]
        public DateTime? CourseProtocolDate { get; set; }

        [Comment("Описание на протокол")]
        [StringLength(DBStringLength.StringLength255)]
        public string? Description { get; set; }

        [Required]
        [Comment("Вид на протокол")]
        public int IdCourseProtocolType { get; set; } // Номенклатура 'CourseProtocolType'

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string UploadedFileName { get; set; }

        public ICollection<CourseProtocolGrade> CourseProtocolGrades { get; set; }

        public ICollection<ClientCourseDocument> ClientCourseDocuments { get; set; }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public override int IdModifyUser { get; set; }

        [Required]
        public override DateTime ModifyDate { get; set; }
        #endregion

        #region IDataMigration
        public int? OldId { get; set; }

        public override string? MigrationNote { get; set; }
        #endregion
    }
}
