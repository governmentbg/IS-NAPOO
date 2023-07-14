using Data.Models.Data.Control;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Control
{
    public class FollowUpControlExpertVM
    {
        public int IdFollowUpControlExpert { get; set; }

        [Required]
        [Display(Name = "Връзка с проследяващ контрол")]
        public int IdFollowUpControl { get; set; }

        public FollowUpControlVM FollowUpControl { get; set; }

        [Required]
        [Display(Name = "Връзка с експерт на НАПОО")]
        public int IdExpert { get; set; }

        public ExpertVM Expert { get; set; }

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
