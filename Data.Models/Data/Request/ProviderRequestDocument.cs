using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
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

namespace Data.Models.Data.Request
{
    /// <summary>
    /// Заявка подадена от ЦПО/ЦИПО за документи към печатницата на МОН 
    /// </summary>
    [Table("Request_ProviderRequestDocument")]
    [Display(Name = "Заявка подадена от ЦПО/ЦИПО за документи към печатницата на МОН ")]
    public class ProviderRequestDocument : AbstractUploadFile, IEntity, IModifiable, IDataMigration
    {
        public ProviderRequestDocument() { 

            this.RequestDocumentStatuses = new List<RequestDocumentStatus>();
            this.RequestDocumentTypes = new HashSet<RequestDocumentType>();
            this.RequestDocumentManagements = new HashSet<RequestDocumentManagement>();
        }

        [Key]
        public int IdProviderRequestDocument { get; set; }
        public int IdEntity => IdProviderRequestDocument;

        [Display(Name = "Връзка с  CPO,CIPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }


        [Display(Name = "Връзка с обобщена заявка на документи към печатницата на МОН")]
        [ForeignKey(nameof(NAPOORequestDoc))]
        public int? IdNAPOORequestDoc { get; set; }
        public NAPOORequestDoc NAPOORequestDoc { get; set; }


        [Comment("Година на заявка")]
        public int? CurrentYear { get; set; }

        [Comment("Дата на заявка")]
        public DateTime? RequestDate { get; set; }


        [StringLength(DBStringLength.StringLength100)]
        [Comment("Длъжност на заявител")]
        public string Position { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Имена на заявител")]
        public string Name { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Comment("Адрес")]
        public string Address { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Телефон")]
        public string Telephone { get; set; }

        [Comment("Заявката е изпратена към печатницата")]
        public bool IsSent { get; set; }//Заявката е изпратена към печатницата

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string UploadedFileName { get; set; }

        [Comment("Ноемер на заявка")]
        public long? RequestNumber { get; set; }

        [Comment("Статус на заявката")]
        public int IdStatus { get; set; }// Номенклатура статус на заявка: RequestDocumetStatus

        [Comment("Населено място за кореспондениця на ЦПО,ЦИПО")]
        [ForeignKey(nameof(LocationCorrespondence))]
        public int? IdLocationCorrespondence { get; set; }

        public Location LocationCorrespondence { get; set; }

        [NotMapped]
        public string FileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    var arrNameParts = this.UploadedFileName.Split(new string[2] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);

                    return (arrNameParts.Length > 0 ? arrNameParts[arrNameParts.Length - 1] : string.Empty);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        [NotMapped]
        public bool HasUploadedFile
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public virtual ICollection<RequestDocumentStatus> RequestDocumentStatuses { get; set; }

        public virtual ICollection<RequestDocumentType> RequestDocumentTypes { get; set; }

        public virtual ICollection<RequestDocumentManagement> RequestDocumentManagements { get; set; }

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