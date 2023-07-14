using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.Archive
{
    /// <summary>
    /// Годишна информация за отчет към НСИ
    [Table("Arch_AnnualReportNSI")]
    [Display(Name = "Годишна информация за отчет към НСИ")]
    public class AnnualReportNSI : IEntity, IModifiable
    {
        public AnnualReportNSI()
        {

        }

        [Key]
        public int IdAnnualReportNSI { get; set; }

        public int IdEntity => IdAnnualReportNSI;

        [Comment("Година")]
        public int Year { get; set; }


        [Comment("Статус")]
        public int IdStatus { get; set; }


        [Comment("Дата на подаване")]
        public DateTime SubmissionDate { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Comment("Име на служител подал годишната информация")]
        public string? Name { get; set; }


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
