using Data.Models.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Заповед към курс за обучение
    /// </summary>
    [Table("Training_CourseOrder")]
    [Comment("Заповед към курс за обучение")]
    public class CourseOrder : AbstractUploadFile, IEntity, IModifiable
    {
        [Key]
        public int IdCourseOrder { get; set; }
        public int IdEntity => IdCourseOrder;

        [Comment("Връзка с курс за обучение, предлагани от ЦПО")]
        [ForeignKey(nameof(Course))]
        public int IdCourse { get; set; }

        public Course Course { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength50)]
        [Comment("Номер на заповед")]
        public string OrderNumber { get; set; }

        [Required]
        [Comment("Дата на заповед")]
        public DateTime OrderDate { get; set; }

        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Описание на заповед")]
        public string? Description { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string UploadedFileName { get; set; }
        [NotMapped]
        public string FileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    var arrNameParts = this.UploadedFileName.Split(new string[2] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);

                    return (arrNameParts.Length > 0 ? arrNameParts[arrNameParts.Length - 1] : string.Empty);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        [NotMapped]
        public bool HasUploadedFile
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override string? MigrationNote { get; set; }

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
    }
}
