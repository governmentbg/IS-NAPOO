using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;

namespace Data.Models.Data.Candidate
{
    /// <summary>
    /// Устройство и дейност на ЦПО
    /// </summary>
    [Table("Candidate_ProviderCPOStructureActivity")]
    [Display(Name = "Устройство и дейност на ЦПО")]
    public class CandidateProviderCPOStructureActivity : IEntity, IModifiable
    {
        [Key]
        public int IdCandidateProviderCPOStructureActivity { get; set; }
        public int IdEntity => IdCandidateProviderCPOStructureActivity;

        [Display(Name = "Връзка с  CPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }

        public CandidateProvider CandidateProvider { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Управление на центъра")]
        public string? Management { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Организация на процеса на обучение")]
        public string? OrganisationTrainingProcess { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Завършване и удостоверяване на професионалното обучение")]
        public string? CompletionCertificationTraining { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Вътрешна система за осигуряване на качеството на обучението, което извършва и прилагането ѝ")]
        public string? InternalQualitySystem { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Информационно осигуряване и поддържане на архива на центъра")]
        public string? InformationProvisionMaintenance { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Актуализиране на учебната документация")]
        public string? TrainingDocumentation { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Подбор на преподаватели")]
        public string? TeachersSelection { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Описание на материалната база за провеждане на обучението по теория и по практика в съответствие с изискванията на ДОС за придобиване на квалификация по професията и специалността, за която се кандидатства")]
        public string? MTBDescription { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Поддържане на актуални данни на центъра за професионално обучение и провежданите от него обучения в информационната система на Националната агенция за\r\nпрофесионално образование и обучение")]
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
    }
}
