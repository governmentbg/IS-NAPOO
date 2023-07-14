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
    /// Връзка между MTB и курс
    /// </summary>
    [Table("Training_PremisesCourse")]
    [Comment("Връзка между MTB и курс")]
    public class PremisesCourse : IEntity, IModifiable
    {
        public PremisesCourse()
        {
        }

        [Key]
        public int IdPremisesCourse { get; set; }
        public int IdEntity => IdPremisesCourse;

        [Display(Name = "Връзка с MTB")]
        [ForeignKey(nameof(CandidateProviderPremises))]
        public int IdPremises { get; set; }
        public CandidateProviderPremises CandidateProviderPremises { get; set; }

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



