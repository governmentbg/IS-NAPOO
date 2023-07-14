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
    /// Служители на НАПОО
    /// </summary>
    [Table("ExpComm_ExpertNapoo")]
    [Display(Name = "Служители на НАПОО")]
    public class ExpertNapoo : IEntity, IModifiable
    {
        public ExpertNapoo()
        {

        }

        [Key]
        public int IdExpertNapoo { get; set; }
        public int IdEntity => IdExpertNapoo;

        [Required]
        [Comment("Експерт")]
        [ForeignKey(nameof(Expert))]
        public int IdExpert { get; set; }
        public Expert Expert { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Длъжност")]
        public string? Occupation { get; set; }
       
        [Comment("Дата на назначаване")]
        public DateTime AppointmentDate { get; set; }

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

      
    }
}

