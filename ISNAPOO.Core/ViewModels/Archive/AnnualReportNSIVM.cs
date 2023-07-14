using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Archive
{
    public class AnnualReportNSIVM
    {
        [Key]
        public int IdAnnualReportNSI { get; set; }

        [Comment("Година")]
        public int Year { get; set; }


        [Comment("Статус")]
        public int IdStatus { get; set; }
        public KeyValueVM? Status { get; set; }

        [Comment("Дата на подаване")]
        public DateTime SubmissionDate { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Comment("Име на служител подал годишната информация")]
        public string? Name { get; set; }

        public string? FileName { get; set; } 

        public MemoryStream? memoryZipFile { get; set; }

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
