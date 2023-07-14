using Data.Models.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.Request
{
    /// <summary>
    /// Обобщена заявка на документи към печатницата на МОН 
    /// </summary>
    [Table("Request_NAPOORequestDoc")]
    [Display(Name = "Обобщена заявка на документи към печатницата на МОН ")]
    public class NAPOORequestDoc: AbstractUploadFile, IEntity, IModifiable, IDataMigration
    {
        public NAPOORequestDoc(){

            this.ProviderRequestDocuments = new List<ProviderRequestDocument>();
        }

        [Key]
        public int IdNAPOORequestDoc { get; set; }
        public int IdEntity => IdNAPOORequestDoc;

        [Comment("Дата на заявка")]
        public DateTime? RequestDate { get; set; }

        [Comment("Година на заявка")]
        public int? RequestYear { get; set; }

        [Comment("Заявката е изпратена към печатницата")]
        public bool IsSent { get; set; }//Заявката е изпратена към печатницата

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string UploadedFileName { get; set; }

        [Comment("Ноемер на заявка")]
        public long? NAPOORequestNumber { get; set; }

        public virtual ICollection<ProviderRequestDocument> ProviderRequestDocuments { get; set; }


        [Comment("Изпратено известие към ЦПО")]
        public bool IsNotificationSent { get; set; }//

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


        #region IDataMigration
        public int? OldId { get; set; }

        public override string? MigrationNote { get; set; }
        #endregion
    }
}