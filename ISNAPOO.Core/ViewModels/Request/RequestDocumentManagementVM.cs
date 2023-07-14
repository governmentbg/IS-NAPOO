using Data.Models.Data.Request;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class RequestDocumentManagementVM
    {
        public RequestDocumentManagementVM()
        {
            this.DocumentSerialNumbers = new HashSet<DocumentSerialNumberVM>();
            this.RequestDocumentTypes = new HashSet<RequestDocumentTypeVM>();
            this.RequestDocumentManagementUploadedFiles = new HashSet<RequestDocumentManagementUploadedFileVM>();
        }

        public int IdRequestDocumentManagement { get; set; }

        [Comment("Връзка със заявка за документация, подадена от ЦПО")]
        public int? IdProviderRequestDocument { get; set; }

        public virtual ProviderRequestDocumentVM ProviderRequestDocument { get; set; }

        [Comment("Връзка с  CPO - Обучаваща институция")]
        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Comment("Връзка с  CPO - Обучаваща институция")]
        public int? IdCandidateProviderPartner { get; set; } // ЦПО - партньор

        public virtual CandidateProviderVM CandidateProviderPartner { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид на документа' е задължително!")]
        [Comment("Връзка с  Тип документ към печатница на МОН")]
        public int IdTypeOfRequestedDocument { get; set; }

        public virtual TypeOfRequestedDocumentVM TypeOfRequestedDocument { get; set; }

        [Comment("Брой документи - Получени/Предадени")]
        [Range(0, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Брой' е задължително!")]
        public int DocumentCount { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на получаване' е задължително!")]
        [Comment("Дата на Получаване/Предаване")]
        public DateTime? DocumentDate { get; set; } = DateTime.Today;

        [Comment("Вид операция")]
        public int IdDocumentOperation { get; set; } //Номенклатура (получен, предаден, отпечатан, анулиран, унищожен, чакащ потвърждение, изгубен)

        public string DocumentOperationName { get; set; }

        [Comment("Начин на получаване на заявката за документи")]
        public int? IdDocumentRequestReceiveType { get; set; } //Печатница, Друго ЦПО

        public string DocumentRequestReceiveTypeName { get; set; }

        public int? IdRequestReport { get; set; } // ЦПО - партньор

        public virtual RequestReportVM RequestReport { get; set; }

        public string DocumentSeriesName { get; set; }

        [Required(ErrorMessage = "Полето 'Година' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Година' е задължително!")]
        [Comment("Календарна година")]
        public int? ReceiveDocumentYear { get; set; } //Календарна година

        public string? SerialNumber { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }

        public bool HasUploadedProtocol => this.RequestDocumentManagementUploadedFiles.Any() && this.RequestDocumentManagementUploadedFiles.All(x => !string.IsNullOrEmpty(x.UploadedFileName));

        public virtual ICollection<DocumentSerialNumberVM> DocumentSerialNumbers { get; set; }

        public virtual ICollection<RequestDocumentTypeVM> RequestDocumentTypes { get; set; }

        public virtual ICollection<RequestDocumentManagementUploadedFileVM> RequestDocumentManagementUploadedFiles { get; set; }

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
