using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ISNAPOO.Common.Constants;

namespace Data.Models.Data.Common
{
    /// <summary>
    /// История за изпълнението на Schedule процеси
    [Table("ScheduleProcessHistory")]
    [Display(Name = "История за изпълнението на Schedule процеси")]
    public class ScheduleProcessHistory : IEntity
    {
        public ScheduleProcessHistory()
        {

        }

        [Key]
        public int IdScheduleProcessHistory { get; set; }
        public int IdEntity => IdScheduleProcessHistory;

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
