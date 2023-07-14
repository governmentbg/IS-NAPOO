using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class NotificationVM : IMapFrom<Notification>, IMapTo<Notification>
    {
        public NotificationVM()
        {
            this.PersonFrom = new PersonVM();
            this.PersonTo = new PersonVM();
            this.ProcedureDocumentNotifications = new HashSet<ProcedureDocumentNotificationVM>();
        }

        [Key]
        public int IdNotification { get; set; }

        [Required(ErrorMessage = "Полето 'Относно' е задължително!")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Относно' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Относно")]
        public string About { get; set; }

        [Required(ErrorMessage = "Полето 'Коментар' е задължително!")]
        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'Коментар' не може да съдържа повече от 1000 символа.")]
        [Display(Name = "Коментар")]
        public string NotificationText { get; set; }


        [Display(Name = "Изпратено от")]
        public int? IdPersonFrom { get; set; }
        public PersonVM PersonFrom { get; set; }

        [Display(Name = "Изпратено до")]
        public int? IdPersonTo { get; set; }
        public PersonVM PersonTo { get; set; }

        [Display(Name = "Статус на известие")]
        public int IdStatusNotification { get; set; }//Чернова, Изпратено, Прегледано

        public string StatusNotificationName { get; set; }

        [Display(Name = "Дата на изпращане")]
        public DateTime? SendDate { get; set; }

        public DateOnly? SendDateOnly { get { return SendDate.HasValue ? DateOnly.FromDateTime(SendDate.Value.Date) : null; } }

        public string SendDateAsStr => this.SendDate.HasValue ? $"{this.SendDate.Value.ToString("dd.MM.yyyy hh:mm:ss")} ч." : string.Empty;

        public string SendDateStrTime
        {
            get { return SendDate.HasValue ? $"{SendDate.Value.ToString("dd.MM.yyyy hh:mm:ss")} EET" : string.Empty; }

            set { SendDateStrTime = value; }
        }

        [Display(Name = "Дата на преглед")]
        public DateTime? ReviewDate { get; set; }

        public string ReviewDateAsStr => this.ReviewDate.HasValue ? $"{this.ReviewDate.Value.ToString("dd.MM.yyyy HH:mm:ss")} ч." : string.Empty;

        public DateOnly? ReviewDateOnly { get { return ReviewDate.HasValue ? DateOnly.FromDateTime(ReviewDate.Value.Date) : null; } }

        public string ReviewDateStrTime
        {
            get { return ReviewDate.HasValue ? $"{ReviewDate.Value.ToString("dd.MM.yyyy HH:mm:ss")} EET" : string.Empty; }

            set { ReviewDateStrTime = value; }
        }

        public int SendTimeSpan()
        {
            var timeDiff = DateTime.Now - this.SendDate.Value;
            return Convert.ToInt32(timeDiff.TotalMinutes);
        }

        [Display(Name = "Токен за валидация")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Токен за валидация' не може да съдържа повече от 255 символа.")]
        public string? Token { get; set; }

        public int? IdCandidateProvider { get; set; }

        public virtual ICollection<ProcedureDocumentNotificationVM> ProcedureDocumentNotifications { get; set; }

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
