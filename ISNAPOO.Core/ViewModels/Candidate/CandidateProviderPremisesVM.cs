using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderPremisesVM : IEmpty
    {
        public CandidateProviderPremisesVM()
        {
            this.CandidateProviderPremisesRooms = new HashSet<CandidateProviderPremisesRoomVM>();
            this.CandidateProviderPremisesSpecialities = new HashSet<CandidateProviderPremisesSpecialityVM>();
            this.CandidateProviderPremisesDocuments = new HashSet<CandidateProviderPremisesDocumentVM>();
            this.CandidateProviderPremisesCheckings = new HashSet<CandidateProviderPremisesCheckingVM>();
            this.SelectedSpecialities = new List<SpecialityVM>();
        }

        public int IdCandidateProviderPremises { get; set; }

        [Required]
        [Comment("CPO,CIPO - Кандидат Обучаваща институция")]
        public int IdCandidate_Provider { get; set; }

        public CandidateProviderVM CandidateProvider { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на материално-техническата база' е задължително!")]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Наименование на материално-техническата база' не може да съдържа повече от 512 символа.")]
        [Comment("Наименование на материално-техническата база")]
        public string PremisesName { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Кратко описание' не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Кратко описание")]
        public string? PremisesNote { get; set; }

        [Required(ErrorMessage = "Полето 'Населено място' е задължително!")]
        [Comment("Населено място")]
        [Range(1, double.MaxValue, ErrorMessage = "Полето 'Населено място' е задължително!")]
        public int? IdLocation { get; set; }

        public LocationVM Location { get; set; }

        [Required(ErrorMessage = "Полето 'Адрес' е задължително!")]
        [Comment("Адрес")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес' не може да съдържа повече от 255 символа.")]
        public string ProviderAddress { get; set; }

        [Required(ErrorMessage = "Полето 'Пощенски код' е задължително!")]
        [Comment("Пощенски код")]
        [StringLength(DBStringLength.StringLength4, ErrorMessage = "Полето 'Пощенски код' не може да съдържа повече от 4 символа!")]
        public string ZipCode { get; set; }

        [Comment("Телефон")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Телефон' не може да съдържа повече от 255 символа.")]
        public string? Phone { get; set; }

        /// <summary>
        /// "държавна"
        /// "общинска"
        /// "частна"
        /// </summary>
        [Comment("Форма на собственост")]
        [Range(1, double.MaxValue, ErrorMessage = "Полето 'Форма на собственост на материало-техническата база' е задължително!")]
        public int IdOwnership { get; set; }

        public string OwnershipValue { get; set; }

        /// <summary>
        /// "активен"
        /// "неактивен"
        /// </summary>
        [Comment("Статус")]
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Полето 'Статус на материало-техническата база' е задължително!")]
        public int IdStatus { get; set; }
        public KeyValueVM? Status { get; set; }
        public string StatusValue { get; set; }
        public string PremisesNameAndLocation => this.Location != null ? $"{this.PremisesName} - {this.Location.LocationName}" : this.PremisesName;

        public string ModifyPersonName { get; set; }

        public string CreatePersonName { get; set;  }

        [Comment("Дата на деактивиране на базата")]
        [Display(Name = "Дата на деактивиране на базата")]
        public DateTime? InactiveDate { get; set; }

        public List<SpecialityVM> SelectedSpecialities { get; set; }

        public virtual ICollection<CandidateProviderPremisesRoomVM> CandidateProviderPremisesRooms { get; set; }

        public virtual ICollection<CandidateProviderPremisesSpecialityVM> CandidateProviderPremisesSpecialities { get; set; }

        public virtual ICollection<CandidateProviderPremisesDocumentVM> CandidateProviderPremisesDocuments { get; set; }
        public virtual ICollection<CandidateProviderPremisesCheckingVM> CandidateProviderPremisesCheckings { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.PremisesName) 
                && this.IdOwnership == 0 
                && string.IsNullOrEmpty(this.Phone) 
                && string.IsNullOrEmpty(this.ZipCode) 
                && string.IsNullOrEmpty(this.ProviderAddress)
                && this.IdLocation == null
                && string.IsNullOrEmpty(this.PremisesName)
                && string.IsNullOrEmpty(this.PremisesNote);
        }

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
