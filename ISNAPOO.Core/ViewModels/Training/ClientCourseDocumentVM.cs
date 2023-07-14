using Data.Models.Data.Framework;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISNAPOO.Core.ViewModels.Training
{

    [Comment("Издадени документи на курсисти")]
    public class ClientCourseDocumentVM : IMapFrom<ClientCourseDocument>, IDataMigration
    {
        public ClientCourseDocumentVM()
        {
            this.CourseDocumentUploadedFiles = new HashSet<CourseDocumentUploadedFileVM>();
        }

        public int IdClientCourseDocument { get; set; }

        [Required]
        [Comment("Връзка с курс/обучаем")]
        [ForeignKey(nameof(ClientCourse))]
        public int IdClientCourse { get; set; }

        public virtual ClientCourseVM ClientCourse { get; set; }

        [Comment("Връзка с протокол от курс за обучение")]
        public int? IdCourseProtocol { get; set; }

        public virtual CourseProtocolVM CourseProtocol { get; set; }

        [Comment("Връзка с фабричен номер на документ от печатница на МОН")]
        public int? IdDocumentSerialNumber { get; set; }

        public virtual DocumentSerialNumberVM DocumentSerialNumber { get; set; }

        [Comment("Връзка с тип докумет към печатница на МОН")]
        [ForeignKey(nameof(TypeOfRequestedDocument))]
        public int? IdTypeOfRequestedDocument { get; set; }

        public virtual TypeOfRequestedDocumentVM TypeOfRequestedDocument { get; set; }

        [Comment("Документи за завършено обучение")]
        public int? IdDocumentType { get; set; }//Сочи към ID на TypeOfRequestedDocument; Таблица 'code_document_type' Свидетелство за придобита СПК, Удостоверение за професионално обучение, Свидетелство за правоспособност, Удостоверение за правоспособност

        public KeyValueVM DocumentType { get; set; }

        [Comment("Година на приключване")]
        public int? FinishedYear { get; set; }

        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Фабричен номер' не може да съдържа повече от 50 символа.")]
        [Comment("Фабричен номер")]
        public string? DocumentPrnNo { get; set; }

        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Регистрационен номер' не може да съдържа повече от 50 символа.")]
        [Comment("Регистрационен номер")]
        public string? DocumentRegNo { get; set; }

        [Comment("Дата на регистрационен документ")]
        public DateTime? DocumentDate { get; set; }

        public string DocumentDateAsStr => this.DocumentDate.HasValue ? $"{this.DocumentDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Протокол' не може да съдържа повече от 50 символа.")]
        [Comment("Протокол")]
        public string? DocumentProtocol { get; set; }

        [Comment("Оценка по теория")]
        public decimal? TheoryResult { get; set; }

        [Comment("Оценка по практика")]
        public decimal? PracticeResult { get; set; }

        [Comment("Обща оценка от теория и практика")]
        public decimal? FinalResult { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Правоспособност' не може да съдържа повече от 255 символа.")]
        [Comment("Правоспособност")]
        public string? QualificationName { get; set; } 

        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Степен' не може да съдържа повече от 50 символа.")]
        [Comment("Степен")]
        public string? QualificationLevel { get; set; }

        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Сериен номер' не може да съдържа повече от 50 символа.")]
        [Comment("Сериен номер")]
        public string? DocumentSerNo { get; set; }

        [Comment("Статус на документ за завършено обучение")]
        public int? IdDocumentStatus { get; set; }// Номенклатура - KeyTypeIntCode: "ClientDocumentStatusType"

        public string DocumentStatusValue { get; set; }

        [Comment("Връзка с оригиналния документ от курс за обучение")]
        public int? IdOriginalClientCourseDocument { get; set; }

        public virtual ClientCourseDocumentVM OriginalClientCourseDocument { get; set; }

        public DateTime? docDateFrom { get; set; }

        public DateTime? docDateTo { get; set; }

        public virtual ICollection<CourseDocumentUploadedFileVM> CourseDocumentUploadedFiles { get; set; }

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


