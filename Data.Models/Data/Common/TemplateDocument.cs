using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Data.Models.Data.Framework;

using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;

namespace Data.Models.Data.Common
{
    /// <summary>
    /// Шаблони на документи
    /// </summary>
    [Table("TemplateDocument")]
    [Display(Name = "Шаблони на документи")]
    public class TemplateDocument : IEntity, IModifiable
    {
        [Key]
        public int idTemplateDocument { get; set; }
        public int IdEntity => idTemplateDocument;

        [Required]
        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Наименование на шаблона")]
        public string TemplateName { get; set; }//Доклад длъжностно лице при лицензиране на ЦПО


        [StringLength(DBStringLength.StringLength1000)]
        [Display(Name = "Описание на шаблона")]
        public string TemplateDescription { get; set; }


        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Път до шаблона")]
        public string TemplatePath { get; set; }

        [Required]
        [Display(Name = "Статус на валидност")]
        public int IdStatus { get; set; }//Активен, Неактивен

        [Required(ErrorMessage = "Изберете опция от списъка")]
        [Display(Name = "Модул")]
        public int IdModule { get; set; }//ЦПО - Лицензиране, ЦПО - Изменение на лиценз, ЦПО - Последващ контрол

        [Required]        
        public int IdApplicationType { get; set; }//Prilojenie 1 - doklad dlajnostno lize, Prilojenie 2_zapoved, Prilojenie 3_uvedomitelno pismo_taksa

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Път до заповед")] 
        public string UploadedFileName { get; set; }



        [Comment("Активна от")]
        public DateTime? DateFrom { get; set; }

        [Comment("Активна до")]
        public DateTime? DateTo { get; set; }

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
