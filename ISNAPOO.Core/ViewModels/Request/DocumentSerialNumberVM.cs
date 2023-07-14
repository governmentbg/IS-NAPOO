using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class DocumentSerialNumberVM
    {
        public DocumentSerialNumberVM()
        {
            this.ClientCourseDocuments = new HashSet<ClientCourseDocumentVM>();    
            this.ValidationClientDocuments = new HashSet<ValidationClientDocumentVM>();    
        }

        public int IdDocumentSerialNumber { get; set; }

        [Comment("Връзка със получени/предадени документи")]
        public int IdRequestDocumentManagement { get; set; }

        public virtual RequestDocumentManagementVM RequestDocumentManagement { get; set; }

        [Comment("Връзка с  CPO - Обучаваща институция")]
        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Comment("Връзка с  Тип документ към печатница на МОН")]
        public int IdTypeOfRequestedDocument { get; set; }

        public virtual TypeOfRequestedDocumentVM TypeOfRequestedDocument { get; set; }

        [Comment("Връзка с Отчет на документи с фабрична номерация по наредба 8")]
        public int? IdRequestReport { get; set; }

        public virtual RequestReportVM RequestReport { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на получаване' е задължително!")]
        [Comment("Дата на Получаване/Предаване")]
        public DateTime DocumentDate { get; set; }

        [Required(ErrorMessage = "Полето 'Фабричен номер' е задължително!")]
        [Comment("Сериен номер на документ")]
        [StringLength(10, ErrorMessage = "Полето 'Фабричен номер' може да съдържа до 10 числа!")]
        public string SerialNumber { get; set; }

        [Required(ErrorMessage = "Полето 'Вид операция' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид операция' е задължително!")]
        [Comment("Вид операция")]
        public int IdDocumentOperation { get; set; } //Номенклатура (получен, предаден, отпечатан, анулиран, унищожен, чакащ потвърждение, изгубен)

        public string DocumentOperationName { get; set; }

        public string DocumentSeriesName { get; set; }

        public int ReceiveDocumentYear { get; set; } //Календарна година

        public int SerialNumberAsIntForOrderBy => !string.IsNullOrEmpty(this.SerialNumber) ? int.Parse(this.SerialNumber) : 0;

        public string DocumentReceivedFrom { get; set; }

        public string ClientName { get; set; }

        public string DocumentDateAsStr => $"{this.DocumentDate.ToString(GlobalConstants.DATE_FORMAT)} г.";

        public bool HasUploadedFile { get; set; }

        public virtual ICollection<ClientCourseDocumentVM> ClientCourseDocuments { get; set; }

        public virtual ICollection<ValidationClientDocumentVM> ValidationClientDocuments { get; set; }

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
