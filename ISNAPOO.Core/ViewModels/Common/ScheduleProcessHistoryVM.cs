using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class ScheduleProcessHistoryVM
    {
        public int IdScheduleProcessHistory { get; set; }

        [Comment("Дата на изпълнение")]
        public DateTime ExecuteDate { get; set; }

        [Comment("Дата на стариране")]
        public DateTime RunTime { get; set; }

        [Comment("Дата на приключване")]
        public DateTime EndTime { get; set; }

        [Comment("Статус на изпълнение")]
        public bool Successful { get; set; }

        [Comment("Изключение - Грешка")]
        [StringLength(DBStringLength.StringLength1000)]
        public string Exception { get; set; }
    }
}
