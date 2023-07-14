using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateCurriculumModificationVM
    {
        public CandidateCurriculumModificationVM()
        {
            this.CandidateCurriculums = new HashSet<CandidateCurriculumVM>();
        }

        public int IdCandidateCurriculumModification { get; set; }

        [Comment("Връзка с лицензирана специалност")]
        public int IdCandidateProviderSpeciality { get; set; }

        public virtual CandidateProviderSpecialityVM CandidateProviderSpeciality { get; set; }

        [Comment("Вид на причина за промяна на учебната програма")] // Номенклатура - KeyTypeIntCode: "CurriculumModificationReasonType"
        public int IdModificationReason { get; set; }

        public string ModificationReasonValue { get; set; }

        [Comment("Статус на промяната на учебната програма")] // Номенклатура - KeyTypeIntCode: "CurriculumModificationStatusType"
        public int IdModificationStatus { get; set; }

        public string ModificationStatusValue { get; set; }

        [Comment("Дата на влизане в сила на промяната на учебната програма")]
        public DateTime? ValidFromDate { get; set; }

        public string ValidFromDateAsStr => this.ValidFromDate.HasValue ? $"{this.ValidFromDate.Value.Date.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл с учебната програма")]
        public string? UploadedFileName { get; set; }

        public string FileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    var arrNameParts = this.UploadedFileName.Split(new string[2] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);

                    return (arrNameParts.Length > 0 ? arrNameParts.Last() : string.Empty);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public virtual ICollection<CandidateCurriculumVM> CandidateCurriculums { get; set; }

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

        #region IDataMigration
        public int? OldId { get; set; }

        public string? MigrationNote { get; set; }
        #endregion
    }
}
