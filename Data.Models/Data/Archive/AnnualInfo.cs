using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Archive
{
    /// <summary>
    /// Годишна информация
    [Table("Arch_AnnualInfo")]
    [Display(Name = "Годишна информация")]
    public class AnnualInfo : IEntity, IModifiable, IDataMigration
    {
        public AnnualInfo()
        {
            this.AnnualInfoStatuses = new HashSet<AnnualInfoStatus>();
        }

        [Key]
        public int IdAnnualInfo { get; set; }
        public int IdEntity => IdAnnualInfo;

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }

        [Required]
        public int Year { get; set; }


        [Required]
        [StringLength(DBStringLength.StringLength512)]
        [Comment("Име на лица подало годишната информация")]
        public string Name { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Comment("Длъжност")]
        public string? Title { get; set; }


        [StringLength(DBStringLength.StringLength100)]
        [Comment("Телефон")]
        public string? Phone { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Comment("E-mail")]
        public string? Email { get; set; }

        [Comment("Статус на отчета за годишна информация")]
        public int IdStatus { get; set; } // Номенклатура - KeyTypeIntCode: "AnnualInfoStatusType"

        public virtual ICollection<AnnualInfoStatus> AnnualInfoStatuses { get; set; }


        [Comment("Оказва дали има приключили курсове през годината")]
        public bool? HasCoursePerYear{ get; set; } = false;

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
