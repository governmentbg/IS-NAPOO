using Data.Models.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.ExternalExpertCommission
{

    /// <summary>
    /// РГ/Рецензенти ДОС
    /// </summary>
    [Table("ExpComm_ExpertDOC")]
    [Display(Name = "РГ/Рецензенти ДОС")]
    public class ExpertDOC : IEntity, IModifiable, IDataMigration

    {
        public ExpertDOC()
        {
            
        }

        [Key]
        public int IdExpertDOC { get; set; }
        public int IdEntity => IdExpertDOC;

        [Required]
        [Comment("Експерт")]
        [ForeignKey(nameof(Expert))]
        public int IdExpert { get; set; }
        public  Expert Expert { get; set; }

        /// <summary>
        /// Проект на ДОС
        /// </summary>
        [Required]
        [ForeignKey(nameof(DOC))]
        public int IdDOC { get; set; }
        public Models.Data.DOC.DOC DOC {get;set;}


        [StringLength(DBStringLength.StringLength100)]
        [Comment("Номер на заповед")]
        public string? OrderNumber { get; set; }

        [Comment("Дата на утвърждаване")]
        public DateTime? DateOrder { get; set; }

        [Comment("Статус")]
        public int IdStatus { get; set; }//Активен/Неактивен

        [StringLength(DBStringLength.StringLength512)]
        [Comment("История на промяната")]
        public string? Comment { get; set; }


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
