using Data.Models.Common;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class NAPOORequestDocVM
    {
        public NAPOORequestDocVM()
        {
            this.ProviderRequestDocuments = new HashSet<ProviderRequestDocumentVM>();
        }

        public int IdNAPOORequestDoc { get; set; }

        [Comment("Дата на заявка")]
        public DateTime? RequestDate { get; set; }

        [Comment("Година на заявка")]
        public int? RequestYear { get; set; }

        [Comment("Заявката е изпратена към печатницата")]
        public bool IsSent { get; set; }//Заявката е изпратена към печатницата

        public string IsSentAsStr => this.IsSent ? "Да" : "Не";

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Прикачен файл' може да съдържа до 512 символа!")]
        [Comment("Прикачен файл")]
        public string UploadedFileName { get; set; }

        public long? NAPOORequestNumber { get; set; }

        [Comment("Изпратено известие към ЦПО")]
        public bool IsNotificationSent { get; set; }//

        public string ReqStatus => this.IsSent ? "Изпратена към печатница" : "В процес на обобщаване";

        public string IsNotificationSentAsStr => this.IsNotificationSent ? "Да" : "Не";

        public string ModifyPersonName { get; set; }

        public string CreatePersonName { get; set; }

        public virtual ICollection<ProviderRequestDocumentVM> ProviderRequestDocuments { get; set; }

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

        #region IDataMigration
        public int? OldId { get; set; }

        public string? MigrationNote { get; set; }
        #endregion
    }
}
