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

namespace Data.Models.Data.Request
{
    /// <summary>
    /// Получени/Предадени документи
    /// </summary>
    [Table("Request_RequestDocumentManagement")]
    [Display(Name = "Получени/Предадени документи")]
    public class RequestDocumentManagement : IEntity, IModifiable, IDataMigration
    {
        public RequestDocumentManagement()
        {
            this.DocumentSerialNumbers = new HashSet<DocumentSerialNumber>();
            this.RequestDocumentTypes = new HashSet<RequestDocumentType>();
            this.RequestDocumentManagementUploadedFiles = new HashSet<RequestDocumentManagementUploadedFile>();
        }

        [Key]
        public int IdRequestDocumentManagement { get; set; }
        public int IdEntity => IdRequestDocumentManagement;

        [Comment("Връзка със заявка за документация, подадена от ЦПО")]
        [ForeignKey(nameof(ProviderRequestDocument))]
        public int? IdProviderRequestDocument { get; set; }
        public ProviderRequestDocument ProviderRequestDocument { get; set; }

        [Comment("Връзка с  CPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }

        [Comment("Връзка с  CPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProviderPartner))]
        public int? IdCandidateProviderPartner { get; set; } // ЦПО - партньор
        public CandidateProvider CandidateProviderPartner { get; set; }

        [Comment("Връзка с  Тип документ към печатница на МОН")]
        [ForeignKey(nameof(TypeOfRequestedDocument))]
        public int IdTypeOfRequestedDocument { get; set; }
        public TypeOfRequestedDocument TypeOfRequestedDocument { get; set; }

        [Required]
        [Comment("Брой документи - Получени/Предадени")]
        public int DocumentCount { get; set; }

        [Required]
        [Comment("Дата на Получаване/Предаване")]
        public DateTime DocumentDate { get; set; }

        [Required]
        [Comment("Вид операция")]
        public int IdDocumentOperation { get; set; } //Номенклатура (получен, предаден, отпечатан, анулиран, унищожен, чакащ потвърждение, изгубен)


      
        [Comment("Начин на получаване на заявката за документи")]
        public int? IdDocumentRequestReceiveType { get; set; } //Печатница, Друго ЦПО


        [Required]
        [Comment("Календарна година")]
        public int ReceiveDocumentYear { get; set; } //Календарна година


        [Comment("Отчет на документи с фабрична номерация по наредба 8")]
        [ForeignKey(nameof(RequestReport))]
        public int? IdRequestReport { get; set; } // ЦПО - партньор
        public RequestReport RequestReport { get; set; }


        public virtual ICollection<DocumentSerialNumber> DocumentSerialNumbers { get; set; }

        public virtual ICollection<RequestDocumentType> RequestDocumentTypes { get; set; }

        public virtual ICollection<RequestDocumentManagementUploadedFile> RequestDocumentManagementUploadedFiles { get; set; }

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
