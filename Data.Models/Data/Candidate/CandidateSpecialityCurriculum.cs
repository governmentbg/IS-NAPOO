using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Models.Data.Candidate
{

    // <summary>
    /// Учебна програма за специалност
    /// </summary>
    [Table("Candidate_Curriculum")]
    [Display(Name = "Учебна програма за специалност")]
    public class CandidateCurriculum : IEntity, IModifiable
    {
        public CandidateCurriculum()
        {
            this.CandidateCurriculumERUs = new HashSet<CandidateCurriculumERU>();
        }

        [Key]
        public int IdCandidateCurriculum { get; set; }
        public int IdEntity => IdCandidateCurriculum;

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

        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'Предмет' не може да съдържа повече от 1000 символа.")]
        [Display(Name = "Предмет")]
        public string Subject { get; set; }// Предмет

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Тема' не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Тема")]
        public string Topic { get; set; }// Тема

        [Display(Name = "Tеория")]
        public double? Theory { get; set; }//Tеория

        [Display(Name = "Практика")]
        public double? Practice { get; set; }//Практика

        public virtual ICollection<CandidateCurriculumERU> CandidateCurriculumERUs { get; set; }



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
