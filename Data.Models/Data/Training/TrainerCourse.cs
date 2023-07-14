using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{


    /// <summary>
    /// Връзка между лектор и курс
    /// </summary>
    [Table("Training_TrainerCourse")]
    [Comment("Връзка между лектор и курс")]
    public class TrainerCourse : IEntity, IModifiable
    {
        public TrainerCourse()
        {
        }

        [Key]
        public int IdTrainerCourse { get; set; }
        public int IdEntity => IdTrainerCourse;

        [Display(Name = "Връзка с Преподавател")]
        [ForeignKey(nameof(CandidateProviderTrainer))]
        public int IdTrainer { get; set; }
        public CandidateProviderTrainer CandidateProviderTrainer { get; set; }

        [Comment("Връзка с Курс за обучение, предлагани от ЦПО")]
        [ForeignKey(nameof(Course))]
        public int IdCourse { get; set; }
        public Course Course { get; set; }

        [Comment("Вид обучение")]
        public int? IdТraininType { get; set; }//Таблица 'code_training_type' Теоретично, Практическо, Избираеми учебни часове



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



