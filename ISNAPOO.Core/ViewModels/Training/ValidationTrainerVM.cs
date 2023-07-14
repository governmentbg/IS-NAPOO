using Data.Models.Data.Candidate;
using Data.Models.Data.Training;
using ISNAPOO.Core.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ValidationTrainerVM : IMapFrom<ValidationTrainer>
    {
        public int IdValidationTrainer { get; set; }

        [Display(Name = "Връзка с Преподавател")]
        [ForeignKey(nameof(CandidateProviderTrainer))]
        public int IdTrainer { get; set; }

        public CandidateProviderTrainer CandidateProviderTrainer { get; set; }

        [Comment("Връзка с Курс за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClient ValidationClient { get; set; }

        [Comment("Вид обучение")]
        public int? IdТrainingType { get; set; } // Номенклатура - KeyTypeIntCode: "TrainingType"

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
