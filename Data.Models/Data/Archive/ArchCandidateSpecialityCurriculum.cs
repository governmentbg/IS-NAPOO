using Data.Models.Data.Archive;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Models.Data.Archive
{

    // <summary>
    /// Учебна програма за специалност
    /// </summary>
    [Table("Arch_Candidate_Curriculum")]
    [Display(Name = "Учебна програма за специалност")]
    public class ArchCandidateCurriculum : IEntity, IModifiable
    {
        public ArchCandidateCurriculum()
        {
           
        }

        [Key]
        public int IdArchCandidateCurriculum { get; set; }
        public int IdEntity => IdArchCandidateCurriculum;

        [Display(Name = "Архив - Специалност връзка с CPO, CIPO - Обучаваща институция")]
        [ForeignKey(nameof(ArchCandidateProviderSpeciality))]
        public int IdArchCandidateProviderSpeciality { get; set; }
        public ArchCandidateProviderSpeciality ArchCandidateProviderSpeciality { get; set; }



        public int IdCandidateCurriculum { get; set; } 

        [Required]
        [Display(Name = "Връзка със Специалност")]
        [ForeignKey(nameof(CandidateProviderSpeciality))]
        public int IdCandidateProviderSpeciality { get; set; }
        public CandidateProviderSpeciality CandidateProviderSpeciality { get; set; }

        [Display(Name = "Връзка с промяна на учебна програма за специалност")]
        [ForeignKey(nameof(CandidateCurriculumModification))]
        public int? IdCandidateCurriculumModification { get; set; }

        public CandidateCurriculumModification CandidateCurriculumModification { get; set; }

        [Required]
        [Display(Name = "Вид професионална подготовка")]//А1 Обща професионална подготовка, А2 Отраслова професионална подготовка,  А3 Специфична професионална подготовка
        public int IdProfessionalTraining { get; set; }        

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Предмет' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Предмет")]
        public string Subject { get; set; }// Предмет

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Тема' не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Тема")]
        public string Topic { get; set; }// Тема

        [Display(Name = "Tеория")]
        public double? Theory { get; set; }//Tеория

        [Display(Name = "Практика")]
        public double? Practice { get; set; }//Практика

     


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
