using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Candidate
{


    /// <summary>
    /// Преподавателска дейност връзка с Преподавател"
    /// </summary>
    [Table("Candidate_ProviderTrainerProfile")]
    [Display(Name = "Преподавателска дейност връзка с Преподавател")]
    public class CandidateProviderTrainerProfile : IEntity, IModifiable, IDataMigration
    {
        public CandidateProviderTrainerProfile()
        {

        }

        [Key]
        public int IdCandidateProviderTrainerProfile { get; set; }
        public int IdEntity => IdCandidateProviderTrainerProfile;

        [Display(Name = "Връзка с Преподавател")]
        [ForeignKey(nameof(CandidateProviderTrainer))]
        public int IdCandidateProviderTrainer { get; set; }
        public CandidateProviderTrainer CandidateProviderTrainer { get; set; }


        [Display(Name = "Връзка с Професионално направление ")]
        [ForeignKey(nameof(ProfessionalDirection))]
        public int IdProfessionalDirection { get; set; }
        public ProfessionalDirection ProfessionalDirection { get; set; }

        [Display(Name = "Отговарящ на изискванията за това Професионално направление")]
        public bool IsProfessionalDirectionQualified { get; set; }//(Да/Не) В старата база поле bool_vet_area_qualified

        [Display(Name = "Преподава по теория")]
        public bool IsTheory { get; set; }//(Да/Не) В старата база поле bool_vet_area_theory

        [Display(Name = "Преподава по практика")]
        public bool IsPractice { get; set; }//(Да/Не) В старата база поле bool_vet_area_practice

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