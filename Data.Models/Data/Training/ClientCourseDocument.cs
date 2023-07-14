using Data.Models.Data.Framework;
using Data.Models.Data.Request;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{


    /// <summary>
    /// Издадени документи на курсисти tb_clients_courses_documents
    /// </summary>
    [Table("Training_ClientCourseDocument")]
    [Comment("Издадени документи на курсисти")]
    public class ClientCourseDocument : IEntity, IModifiable
    {
        public ClientCourseDocument()
        {
            this.ClientCourseDocumentStatuses = new HashSet<ClientCourseDocumentStatus>();
            this.CourseDocumentUploadedFiles = new HashSet<CourseDocumentUploadedFile>();
        }

        [Key]
        public int IdClientCourseDocument { get; set; }
        public int IdEntity => IdClientCourseDocument;

        [Required]
        [Comment("Връзка с курс/обучаем")]
        [ForeignKey(nameof(ClientCourse))]
        public int IdClientCourse { get; set; }
        public ClientCourse ClientCourse { get; set; }

        [ForeignKey(nameof(CourseProtocol))]
        [Comment("Връзка с протокол от курс за обучение")]
        public int? IdCourseProtocol { get; set; }

        public CourseProtocol CourseProtocol { get; set; }

        [Comment("Връзка с фабричен номер на документ от печатница на МОН")]
        [ForeignKey(nameof(DocumentSerialNumber))]
        public int? IdDocumentSerialNumber { get; set; }

        public DocumentSerialNumber DocumentSerialNumber { get; set; }

        [Comment("Връзка с тип докумет към печатница на МОН")]
        [ForeignKey(nameof(TypeOfRequestedDocument))]
        public int? IdTypeOfRequestedDocument { get; set; }

        public TypeOfRequestedDocument TypeOfRequestedDocument { get; set; }

        [Comment("Документи за завършено обучение")]
        public int? IdDocumentType { get; set; }//Сочи към ID на TypeOfRequestedDocument; Таблица 'code_document_type' Свидетелство за придобита СПК, Удостоверение за професионално обучение, Свидетелство за правоспособност, Удостоверение за правоспособност


        [Comment("Година на приключване")]
        public int? FinishedYear { get; set; }//


        [StringLength(DBStringLength.StringLength50)]
        [Comment("Фабричен номер")]
        public string? DocumentPrnNo { get; set; }


        [StringLength(DBStringLength.StringLength50)]
        [Comment("Регистрационен номер")]
        public string? DocumentRegNo { get; set; }

        [Comment("Дата на регистрационен документ")]
        public DateTime? DocumentDate { get; set; }


        [StringLength(DBStringLength.StringLength50)]
        [Comment("Протокол")]
        public string? DocumentProtocol { get; set; }


        [Column(TypeName = "decimal(3, 2)")]
        [Comment("Оценка по теория")]
        public decimal? TheoryResult { get; set; }

        [Column(TypeName = "decimal(3, 2)")]
        [Comment("Оценка по практика")]
        public decimal? PracticeResult { get; set; }

        [Column(TypeName = "decimal(3, 2)")]
        [Comment("Обща оценка от теория и практика")]
        public decimal? FinalResult { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Comment("Правоспособност")]
        public string? QualificationName { get; set; } 

        [StringLength(DBStringLength.StringLength50)]
        [Comment("Степен")]
        public string? QualificationLevel { get; set; }


        [StringLength(DBStringLength.StringLength50)]
        [Comment("Сериен номер")]
        public string? DocumentSerNo { get; set; }

        [Comment("Статус на документ за завършено обучение")]
        public int? IdDocumentStatus { get; set; }//Таблица 'code_document_status' Неподаден, Подаден,Върнат,Вписан в Регистъра,Съотвестващ,Частично съотвестващ,Несъответсващ,Отказан,Архивиран,Приключил,Скрит от РИДПК

        [ForeignKey(nameof(OriginalClientCourseDocument))]
        [Comment("Връзка с оригиналния документ от курс за обучение")]
        public int? IdOriginalClientCourseDocument { get; set; }

        public ClientCourseDocument OriginalClientCourseDocument { get; set; }

        public ICollection<ClientCourseDocumentStatus> ClientCourseDocumentStatuses { get; set; }
        public virtual ICollection<CourseDocumentUploadedFile> CourseDocumentUploadedFiles { get; set; }

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


