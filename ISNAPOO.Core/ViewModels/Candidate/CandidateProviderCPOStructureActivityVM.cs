using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderCPOStructureActivityVM :  IModifiable
    {
        public int IdCandidateProviderCPOStructureActivity { get; set; }

        [Display(Name = "Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        public int IdCandidate_Provider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Управление на центъра' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Управление на центъра")]
        public string? Management { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Организация на процеса на обучение' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Организация на процеса на обучение")]
        public string? OrganisationTrainingProcess { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Завършване и удостоверяване на професионалното обучение' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Завършване и удостоверяване на професионалното обучение")]
        public string? CompletionCertificationTraining { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Вътрешна система за осигуряване на качеството на обучението, което извършва и прилагането ѝ' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Вътрешна система за осигуряване на качеството на обучението, което извършва и прилагането ѝ")]
        public string? InternalQualitySystem { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Информационно осигуряване и поддържане на архива на центъра' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Информационно осигуряване и поддържане на архива на центъра")]
        public string? InformationProvisionMaintenance { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Актуализиране на учебната документация' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Актуализиране на учебната документация")]
        public string? TrainingDocumentation { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Подбор на преподаватели' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Подбор на преподаватели")]
        public string? TeachersSelection { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Описание на материалната база за провеждане на обучението по теория и по практика в съответствие с изискванията на ДОС за придобиване на квалификация по професията и специалността, за която се кандидатства' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Описание на материалната база за провеждане на обучението по теория и по практика в съответствие с изискванията на ДОС за придобиване на квалификация по професията и специалността, за която се кандидатства")]
        public string? MTBDescription { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Поддържане на актуални данни на центъра за професионално обучение и провежданите от него обучения в информационната система на Националната агенция за професионално образование и обучение' не може да съдържа повече от 4000 символа!")]
        [Display(Name = "Поддържане на актуални данни на центъра за професионално обучение и провежданите от него обучения в информационната система на Националната агенция за професионално образование и обучение")]
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
            return string.IsNullOrEmpty(this.Management) && string.IsNullOrEmpty(this.OrganisationTrainingProcess) && string.IsNullOrEmpty(this.CompletionCertificationTraining)
                && string.IsNullOrEmpty(this.InternalQualitySystem) && string.IsNullOrEmpty(this.InformationProvisionMaintenance) && string.IsNullOrEmpty(this.TrainingDocumentation) && string.IsNullOrEmpty(this.TeachersSelection)
                && string.IsNullOrEmpty(this.MTBDescription) && string.IsNullOrEmpty(this.DataMaintenance);
        }
    }
}
