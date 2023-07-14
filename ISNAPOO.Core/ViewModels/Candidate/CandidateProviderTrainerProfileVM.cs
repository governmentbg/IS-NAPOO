using ISNAPOO.Core.ViewModels.SPPOO;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderTrainerProfileVM
    {
        public int IdCandidateProviderTrainerProfile { get; set; }

        [Display(Name = "Връзка с Преподавател")]
        public int IdCandidateProviderTrainer { get; set; }
        public virtual CandidateProviderTrainerVM CandidateProviderTrainer { get; set; }

        [Display(Name = "Връзка с Професионално направление ")]
        public int IdProfessionalDirection { get; set; }
        public virtual ProfessionalDirectionVM ProfessionalDirection { get; set; }

        public string ProfessionalDirectionCodeAndName { get; set; }

        [Display(Name = "Отговарящ на изискванията за това Професионално направление")]
        public bool IsProfessionalDirectionQualified { get; set; }//(Да/Не) В старата база поле bool_vet_area_qualified

        [Display(Name = "Преподава по теория")]
        public bool IsTheory { get; set; }//(Да/Не) В старата база поле bool_vet_area_theory

        [Display(Name = "Преподава по практика")]
        public bool IsPractice { get; set; }//(Да/Не) В старата база поле bool_vet_area_practice

        public string? Usage => IsPractice && IsTheory ? "Теория и Практика" : IsPractice ? "Практика" : "Теория";

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
