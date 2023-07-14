using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderCIPOStructureActivityVM :  IModifiable
    {
        public int IdCandidateProviderCIPOStructureActivity { get; set; }

        [Display(Name = "Връзка с  CIPO - Кандидат Обучаваща институция")]
        public int IdCandidate_Provider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Управление на центъра' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Управление на центъра")]
        public string? Management { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Организация на дейността по информиране и професионално ориентиране' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Организация на дейността по информиране и професионално ориентиране")]
        public string? OrganisationInformationProcess { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Вътрешна система за осигуряване на качеството на дейността по информиране и професионално ориентиране' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Вътрешна система за осигуряване на качеството на дейността по информиране и професионално ориентиране")]
        public string? InternalQualitySystem { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Информационно осигуряване и поддържането на архива на центъра' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Информационно осигуряване и поддържането на архива на центъра")]
        public string? InformationProvisionMaintenance { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Актуализиране на документацията за информиране и професионално ориентиране' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Актуализиране на документацията за информиране и професионално ориентиране")]
        public string? TrainingDocumentation { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Подбор на лицата, които извършват информиране и професионално ориентиране' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Подбор на лицата, които извършват информиране и професионално ориентиране")]
        public string? ConsultantsSelection { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Осигуряване на материалната база' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Осигуряване на материалната база")]
        public string? MTBDescription { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Поддържане на актуални данни за центъра и провежданото от него информиране и професионално ориентиране в информационната система на Националната агенция за професионално образование и обучение' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Поддържане на актуални данни за центъра и провежданото от него информиране и професионално ориентиране в информационната система на Националната агенция за професионално образование и обучение")]
        public string? DataMaintenance { get; set; }

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

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.Management) && string.IsNullOrEmpty(this.OrganisationInformationProcess) && string.IsNullOrEmpty(this.ConsultantsSelection)
                && string.IsNullOrEmpty(this.InternalQualitySystem) && string.IsNullOrEmpty(this.InformationProvisionMaintenance) && string.IsNullOrEmpty(this.TrainingDocumentation)
                && string.IsNullOrEmpty(this.MTBDescription) && string.IsNullOrEmpty(this.DataMaintenance);
        }
    }
}
