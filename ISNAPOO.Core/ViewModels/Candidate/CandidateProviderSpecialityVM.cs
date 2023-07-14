using Data.Models.Data.Candidate;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderSpecialityVM : IMapFrom<CandidateProviderSpeciality>, IMapTo<CandidateProviderSpeciality>
    {
        public CandidateProviderSpecialityVM()
        {
            this.CandidateCurriculums = new HashSet<CandidateCurriculumVM>();
            this.CandidateCurriculumModifications = new HashSet<CandidateCurriculumModificationVM>();
        }

        [Key]
        public int IdCandidateProviderSpeciality { get; set; }


        [Required]
        [Display(Name = "CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }
        public CandidateProviderVM CandidateProvider { get; set; }

        [Required]
        [Display(Name = "Специалност")]
        [ForeignKey(nameof(Speciality))]
        public int IdSpeciality { get; set; }
        public SpecialityVM Speciality { get; set; }



        [Comment("Рамкова програма")]
        [ForeignKey(nameof(FrameworkProgram))]
        public int? IdFrameworkProgram { get; set; }
        public FrameworkProgramVM FrameworkProgram { get; set; }

        [Comment("Форма на обучение")]
        public int? IdFormEducation { get; set; }//Дневна, Вечерна, Дистанционна, Индивидуална, самостоятелна, Задочна, Дуална

        [Comment("Дата на получаване на лицензия за специалността")]
        public DateTime? LicenceData { get; set; }

        public string LicenceDateAsStr => this.LicenceData.HasValue ? $"{this.LicenceData.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Номер на протокол/заповед за лицензиране на специалността' не може да съдържа повече от 20 символа.")]
        [Comment("Номер на протокол/заповед за лицензиране на специалността")]
        public string? LicenceProtNo { get; set; }

        public string? Speciality_Code { get; set; }

        public string CurriculumModificationUploadedFileName { get; set; }

        public virtual ICollection<CandidateCurriculumVM> CandidateCurriculums { get; set; }

        public virtual ICollection<CandidateCurriculumModificationVM> CandidateCurriculumModifications { get; set; }

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
