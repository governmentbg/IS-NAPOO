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
    /// Заявки към RegiX
    [Table("Arch_RegiXLogRequest")]
    [Display(Name = "Заявки към RegiX")]
    public class RegiXLogRequest : IEntity, IModifiable
    {
        public RegiXLogRequest()
        {

        }

        [Key]
        public int IdRegiXLogRequest { get; set; }

        public int IdEntity => IdRegiXLogRequest;

        /// <summary>
        /// Наименование на администрацията, ползваща системата. Пример: НАПОО
        /// </summary>
        [StringLength(DBStringLength.StringLength255)]
        [Comment("Наименование на администрацията, ползваща системата")]
        public string AdministrationName { get; set; }//НАПОО

        /// <summary>
        /// Идентификационен код на администрация (oID от eAuth)
        /// </summary>
        [StringLength(DBStringLength.StringLength255)]
        [Comment("Идентификационен код на администрация (oID от eAuth)")]
        public string AdministrationOId { get; set; }//2.16.100.1.1.23.1.3.


        /// <summary>
        /// Идентификатор на служител на администрацията
        /// Например:EMAIL адрес, с който служителя влиза в АД на съответната администрация,Потребителско име в информационната система - клиент
        /// </summary>
        [StringLength(DBStringLength.StringLength255)]
        [Comment("Идентификатор на служител на администрацията")]
        public string? EmployeeIdentifier { get; set; }

        /// <summary>
        /// Длъжност или позиция на служителя в администрацията
        /// </summary>
        [StringLength(DBStringLength.StringLength255)]
        [Comment("Длъжност или позиция на служителя в администрацията")]
        public string? EmployeePosition { get; set; }


        [StringLength(DBStringLength.StringLength255)]
        [Comment("Имена на служител на администрацията, иментата на служителя")]
        public string? EmployeeNames { get; set; }

        /// <summary>
        /// Опционален идентификатор на човека отговорен за справката. Тук трябва да се слага стойност, когато заявките не се инициират ръчно от конкретен служител, а се генерират автоматично от информационна система. Това може да бъде ръководител на звено в Администрацията, ползваща съответната справка в информационната система-клиент.
        /// </summary>
        [StringLength(DBStringLength.StringLength255)]
        [Comment("Опционален идентификатор на човека отговорен за справката.")]       
        public string? ResponsiblePersonIdentifier { get; set; }

        /// <summary>
        /// Контекст на правното основание
        /// </summary>
        [StringLength(DBStringLength.StringLength255)]
        [Comment("Контекст на правното основание")]
        public string? LawReason { get; set; }

        /// <summary>
        /// Допълнително поле в свободен текст
        /// </summary>
        [StringLength(DBStringLength.StringLength255)]
        [Comment("Допълнително поле в свободен текст")]
        public string? Remark { get; set; }


        /// <summary>
        /// Вид на услугата, във връзка с която се извиква операцията
        /// </summary>
        [StringLength(DBStringLength.StringLength255)]
        [Comment("Вид на услугата, във връзка с която се извиква операцията")]
        public string? ServiceType { get; set; }


        /// <summary>
        /// Идентификатор на инстанцията на административната услуга или процедура в администрацията (например: номер на преписка)
        /// </summary>
        [StringLength(DBStringLength.StringLength255)]
        [Comment("Идентификатор на инстанцията на административната услуга или процедура в администрацията (например: номер на преписка)")]
        public string? ServiceURI { get; set; }


        /// <summary>
        /// Статус на заявката към RegiX
        /// </summary>
        [StringLength(DBStringLength.StringLength255)]
        [Comment("Статус на заявката към RegiX")]
        public string? ServiceResultStatus { get; set; }


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
