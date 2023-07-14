using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using Data.Models.Data.Training;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Request
{
    /// <summary>
    /// Описание на документи със серийни номера
    /// </summary>
    [Table("Request_DocumentSerialNumber")]
    [Display(Name = "Описание на документи със серийни номера")]
    public class DocumentSerialNumber : IEntity, IModifiable, IDataMigration
    {
        public DocumentSerialNumber()
        {
            this.ClientCourseDocuments = new HashSet<ClientCourseDocument>();
            this.ValidationClientDocuments = new HashSet<ValidationClientDocument>();
        }

        [Key]
        public int IdDocumentSerialNumber { get; set; }
        public int IdEntity => IdDocumentSerialNumber;

        [Comment("Връзка със получени/предадени документи")]
        [ForeignKey(nameof(RequestDocumentManagement))]
        public int IdRequestDocumentManagement { get; set; }
        public RequestDocumentManagement RequestDocumentManagement { get; set; }

        [Comment("Връзка с  CPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }

        [Comment("Връзка с  Тип документ към печатница на МОН")]
        [ForeignKey(nameof(TypeOfRequestedDocument))]
        public int IdTypeOfRequestedDocument { get; set; }
        public TypeOfRequestedDocument TypeOfRequestedDocument { get; set; }


        [Comment("Връзка с Отчет на документи с фабрична номерация по наредба 8")]
        [ForeignKey(nameof(RequestReport))]
        public int? IdRequestReport { get; set; }
        public RequestReport RequestReport { get; set; }


        [Required]
        [Comment("Дата на Получаване/Предаване")]
        public DateTime DocumentDate { get; set; }

        [Required]
        [Comment("Сериен номер на документ")]
        public string SerialNumber { get; set; }

        [Required]
        [Comment("Вид операция")]
        public int IdDocumentOperation { get; set; } //Номенклатура (получен, предаден, отпечатан, анулиран, унищожен, чакащ потвърждение, изгубен)


        [Required]
        [Comment("Календарна година")]
        public int ReceiveDocumentYear { get; set; } //Календарна година

        public ICollection<ClientCourseDocument> ClientCourseDocuments { get; set; }

        public ICollection<ValidationClientDocument> ValidationClientDocuments { get; set; }

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
