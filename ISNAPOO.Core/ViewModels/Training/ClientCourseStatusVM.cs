using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ClientCourseStatusVM
    {
        public int IdClientCourseStatus { get; set; }

        [Required]
        [Comment("Връзка с Получател на услугата(обучаем) връзка с курс")]
        public int IdClientCourse { get; set; }

        public virtual ClientCourseVM ClientCourse { get; set; }

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
