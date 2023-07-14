using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ValidationClientCheckingVM : IMapFrom<ValidationClientChecking>
    {
        public int IdValidationClientChecking { get; set; }

        [Required(ErrorMessage = "Полето 'Валидирано лице' е задължително!")]
        [Display(Name = "Връзка с валидирано лице")]
        public int? IdValidationClient { get; set; }

        public ValidationClientVM ValidationClient { get; set; }

        [Display(Name = "Последващ контрол, изпълняван от служител/и на НАПОО")]
        public int? IdFollowUpControl { get; set; }

        public FollowUpControlVM FollowUpControl { get; set; }

        [Display(Name = "Извършена проверка от експерт на НАПОО")]
        public bool CheckDone { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Коментар' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Коментар")]
        public string? Comment { get; set; }

        [Display(Name = "Дата на проверка")]
        public DateTime? CheckingDate { get; set; }

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
