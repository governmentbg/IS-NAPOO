using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;

namespace Data.Models.Data.Candidate
{
    /// <summary>
    /// Устройство и дейност на ЦИПО
    /// </summary>
    [Table("Candidate_ProviderCIPOStructureActivity")]
    [Display(Name = "Устройство и дейност на ЦПО")]
    public class CandidateProviderCIPOStructureActivity : IEntity, IModifiable
    {
        [Key]
        public int IdCandidateProviderCIPOStructureActivity { get; set; }
        public int IdEntity => IdCandidateProviderCIPOStructureActivity;

        [Display(Name = "Връзка с  CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }

        public CandidateProvider CandidateProvider { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Управление на центъра")]
        public string? Management { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Организация на дейността по информиране и професионално ориентиране")]
        public string? OrganisationInformationProcess { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Вътрешна система за осигуряване на качеството на дейността по информиране и професионално ориентиране")]
        public string? InternalQualitySystem { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Информационно осигуряване и поддържането на архива на центъра")]
        public string? InformationProvisionMaintenance { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Актуализиране на документацията за информиране и професионално ориентиране")]
        public string? TrainingDocumentation { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Подбор на лицата, които извършват информиране и професионално ориентиране")]
        public string? ConsultantsSelection { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Осигуряване на материалната база")]
        public string? MTBDescription { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
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
    }
}
