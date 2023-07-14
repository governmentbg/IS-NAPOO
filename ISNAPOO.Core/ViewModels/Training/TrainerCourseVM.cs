using Data.Models.Data.Training;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Training
{
    /// <summary>
    /// Връзка между лектор и курс
    /// </summary>
    [Comment("Връзка между лектор и курс")]
    public class TrainerCourseVM : IMapFrom<TrainerCourse>
    {
        public int IdTrainerCourse { get; set; }

        [Display(Name = "Връзка с Преподавател")]
        public int IdTrainer { get; set; }

        public virtual CandidateProviderTrainerVM CandidateProviderTrainer { get; set; }

        [Comment("Връзка с Курс за обучение, предлагани от ЦПО")]
        public int IdCourse { get; set; }

        public virtual CourseVM Course { get; set; }

        [Comment("Вид обучение")]
        public int? IdТraininType { get; set; }//Таблица 'code_training_type' Теоретично, Практическо, Избираеми учебни часове

        public string TrainingTypeName { get; set; }

        public List<KeyValuePair<int, List<string>>> TrainerDocuments { get; set; }

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



