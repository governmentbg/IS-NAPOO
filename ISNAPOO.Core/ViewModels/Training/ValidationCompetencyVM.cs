using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
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
    public class ValidationCompetencyVM : IMapFrom<ValidationCompetency>
    {
        public int IdValidationCompetency { get; set; }

        [Comment("Връзка с обучаем от курс за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClientVM ValidationClient { get; set; }

        [Required(ErrorMessage = "Полето '№ на компетентност' е задължително!")]
        [Comment("Номер на компетентност")]
        public int? CompetencyNumber { get; set; }

        [Required(ErrorMessage = "Полето 'Компетентност' е задължително!")]
        [StringLength(DBStringLength.StringLength255)]
        [Comment("Компетентност")]
        public string Competency { get; set; }

        [Comment("Дали се признава компетентността")]
        public bool IsCompetencyRecognized { get; set; }

        public string IsCompetencyRecognizedStr => IsCompetencyRecognized ? "Да" : "Не";

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }

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
