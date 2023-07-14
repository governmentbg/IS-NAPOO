using Data.Models.Data.Training;
using ISNAPOO.Core.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ValidationProtocolGradeVM : IMapFrom<ValidationProtocolGrade>
    {
        public int IdValidationProtocolGrade { get; set; }

        [Comment("Връзка с протокол към курс за валидиране")]
        [ForeignKey(nameof(ValidationProtocol))]
        public int IdValidationProtocol { get; set; }

        public ValidationProtocolVM ValidationProtocol { get; set; }

        [Comment("Връзка с курс за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClientVM ValidationClient { get; set; }

        [Display(Name = "Оценка от протокол")]
        [Comment("Оценка от протокол от курс за валидиране")]
        public double? Grade { get; set; }

        public string GradeAsStr { get; set; }


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
