using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class EventLogVM
    {
        [Key]
        public int idEventLog { get; set; }

        [Comment("Потребител направил завката")]
        public int? idUser { get; set; }

        [Comment("Потребител направил завката")]
        [StringLength(DBStringLength.StringLength100)]
        public string? idAspNetUsers { get; set; }

        [Comment("Дата час на действието")]
        public DateTime EventDate { get; set; }

        [Comment("Допълнителна информация")]
        [StringLength(DBStringLength.StringLength1000)]
        public string EventMessage { get; set; }

        [Comment("Вид събитие - UPDATE, INSERT, DELETE")]
        [StringLength(DBStringLength.StringLength100)]
        public string EventAction { get; set; }


        [Comment("Oбект, над който е извършено действието")]
        [StringLength(DBStringLength.StringLength100)]
        public string EntityID { get; set; }

        [Comment("EntityName - таблицата")]
        [StringLength(DBStringLength.StringLength100)]
        public string EntityName { get; set; }


        [Comment("Име на потребителя напавил действието")]
        [StringLength(DBStringLength.StringLength255)]
        public string? PersonName { get; set; }

        [Comment("IP адрес")]
        [StringLength(DBStringLength.StringLength50)]
        public string IP { get; set; }

        [Comment("Браузър на потребителя")]
        [StringLength(DBStringLength.StringLength50)]
        public string BrowserInformation { get; set; }
        [Comment("Текущ адрес на страницата")]
        [StringLength(DBStringLength.StringLength512)]
        public string? CurrentUrl { get; set; }

        [Comment("Наименование на менюто")]
        [StringLength(DBStringLength.StringLength512)]
        public string? CurrentMenu { get; set; }

    }
}
