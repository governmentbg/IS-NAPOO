using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class ManagementDeadlineProcedureVM
    {
        public int IdManagementDeadlineProcedure  { get; set; }
        public string LicensingTypeName { get; set; }
        public string ApplicationStatusName { get; set; }


        public int IdLicensingType { get; set; }
        public int IdApplicationStatus { get; set; }

        public int Term { get; set; }
        public string TermAsStr { get; set; }



        public string ModifyPersonName { get; set; }

        public string CreatePersonName { get; set; }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public  int IdModifyUser { get; set; }

        [Required]
        public DateTime ModifyDate { get; set; }
        #endregion
    }
}
