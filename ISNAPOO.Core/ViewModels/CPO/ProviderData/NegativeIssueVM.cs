using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.SPPOO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.CPO.ProviderData
{
    public class NegativeIssueVM : IMapFrom<NegativeIssue>, IMapTo<NegativeIssue>
    {
        
        public int IdNegativeIssue { get; set; }

        [Display(Name = "Връзка с  Данни за Процедура за лицензиране")]
        
        public int IdStartedProcedure { get; set; }
        public StartedProcedureVM StartedProcedure { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Установени непълноти и неточности' не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Установени непълноти и неточности")]
        public string? NegativeIssueText { get; set; }


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
