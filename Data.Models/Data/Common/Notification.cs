using Data.Models.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Common
{
     
    /// <summary>
    /// Известие
    /// </summary>
    [Table("Notification")]
    [Display(Name = "Известие")]
    public class Notification : AbstractUploadFile, IEntity, IModifiable
    {
        public Notification()
        {
        } 

        [Key]
        public int IdNotification { get; set; }
        public int IdEntity => IdNotification;

        [Required]
        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Относно")]
        public string About { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength1000)]
        [Display(Name = "Съдържане на известието")]
        public string NotificationText { get; set; }


        [Display(Name = "Известие от")]
        [ForeignKey(nameof(PersonFrom))]
        public int? IdPersonFrom { get; set; }
        public Person PersonFrom { get; set; }

        [Display(Name = "Известие до")]
        [ForeignKey(nameof(PersonTo))]
        public int? IdPersonTo { get; set; }
        public Person PersonTo { get; set; }

        [Display(Name = "Статус на известие")]
        public int IdStatusNotification { get; set; }//Чернова, Изпратено, Прегледано

        [Display(Name = "Дата на изпращане")]
        public DateTime  SendDate { get; set; }

        [Display(Name = "Дата на преглед")]
        public DateTime? ReviewDate { get; set; }

        [Display(Name = "Токен за валидация ")]
        [StringLength(DBStringLength.StringLength255)]
        public string? Token { get; set; }

        public override string? MigrationNote { get; set; }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public override int IdModifyUser { get; set; }
        [Required]
        public override DateTime ModifyDate { get; set; }
        #endregion

        [NotMapped]
        public override string UploadedFileName { get; set; }
    }
}
