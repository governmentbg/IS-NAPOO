using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using ISNAPOO.Core.ViewModels.Candidate;
using System.Collections.Generic;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class PremisesCourseVM
    {
        [Key]
        public int IdPremisesCourse { get; set; }

        [Display(Name = "Връзка с MTB")]
        public int IdPremises { get; set; }

        public virtual CandidateProviderPremisesVM CandidateProviderPremises { get; set; }

        [Comment("Връзка с Курс за обучение, предлагани от ЦПО")]
        public int IdCourse { get; set; }

        public virtual CourseVM Course { get; set; }

        [Comment("Вид обучение")]
        public int? IdТraininType { get; set; }//Таблица 'code_training_type' Теоретично, Практическо, Избираеми учебни часове

        public string TrainingTypeName { get; set; }

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
