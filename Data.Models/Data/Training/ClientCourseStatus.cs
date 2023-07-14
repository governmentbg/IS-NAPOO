using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// История на статуса на завършване на курс за обучение от курсист
    /// </summary>
    [Table("Training_ClientCourseStatus")]
    [Comment("История на статуса на завършване на курс за обучение от курсист")]
    public class ClientCourseStatus : IEntity, IModifiable
    {
        [Key]
        public int IdClientCourseStatus { get; set; }
        public int IdEntity => IdClientCourseStatus;

        [Required]
        [Comment("Връзка с Получател на услугата(обучаем) връзка с курс")]
        [ForeignKey(nameof(ClientCourse))]
        public int IdClientCourse { get; set; }

        public ClientCourse ClientCourse { get; set; }

        [Comment("Приключване на курс")]
        public int IdFinishedType { get; set; }//Таблица 'code_education' завършил с документ, прекъснал по уважителни причини, прекъснал по неуважителни причини, завършил курса, но не положил успешно изпита, придобил СПК по реда на чл.40 от ЗПОО, издаване на дубликат

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
