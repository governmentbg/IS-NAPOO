using Data.Models.Data.Request;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Request;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ValidationClientDocumentVM : IMapFrom<ValidationClientDocument>
    {
        public ValidationClientDocumentVM()
        {
            this.ValidationDocumentUploadedFiles = new HashSet<ValidationDocumentUploadedFileVM>();
        }

        [Key]
        public int IdValidationClientDocument { get; set; }

        [Required]
        [Comment("Връзка с обучаем")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClientVM ValidationClient { get; set; }

        [ForeignKey(nameof(ValidationProtocol))]
        [Comment("Връзка с протокол от курс за валидиране")]
        public int? IdValidationProtocol { get; set; }

        public ValidationProtocol ValidationProtocol { get; set; }

        [Comment("Връзка с фабричен номер на документ от печатница на МОН")]
        [ForeignKey(nameof(DocumentSerialNumber))]
        public int? IdDocumentSerialNumber { get; set; }

        public DocumentSerialNumber DocumentSerialNumber { get; set; }

        [Comment("Връзка с тип докумет към печатница на МОН")]
        [ForeignKey(nameof(TypeOfRequestedDocument))]
        public int? IdTypeOfRequestedDocument { get; set; }

        public virtual TypeOfRequestedDocumentVM TypeOfRequestedDocument { get; set; }

        [Comment("Документи за завършено обучение")]
        public int? IdDocumentType { get; set; }//Сочи към ID на TypeOfRequestedDocument; Таблица 'code_document_type' Свидетелство за придобита СПК, Удостоверение за професионално обучение, Свидетелство за правоспособност, Удостоверение за правоспособност

        [Comment("Година на приключване")]
        public int? FinishedYear { get; set; }//

        [StringLength(DBStringLength.StringLength50)]
        [Comment("Регистрационен номер")]
        public string? DocumentRegNo { get; set; }

        [Comment("Дата на регистрационен документ")]
        public DateTime? DocumentDate { get; set; }

        public string DocumentDateAsStr => this.DocumentDate.HasValue ? $"{this.DocumentDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

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

        [StringLength(DBStringLength.StringLength50)]
        [Comment("Степен")]
        public string? QualificationLevel { get; set; }

        [Comment("Статус на документ за завършено обучение")]
        public int? IdDocumentStatus { get; set; }//Таблица 'code_document_status' Неподаден, Подаден,Върнат,Вписан в Регистъра,Съотвестващ,Частично съотвестващ,Несъответсващ,Отказан,Архивиран,Приключил,Скрит от РИДПК

        [Comment("Връзка с оригиналния документ от курс за валидиране")]
        public int? IdOriginalValidationClientDocument { get; set; }

        public string DocumentTypeName { get; set; }

        public string DocumentStatusValue { get; set; }

        public virtual ValidationClientDocumentVM OriginalValidationClientDocument { get; set; }

        public virtual ICollection<ValidationDocumentUploadedFileVM> ValidationDocumentUploadedFiles { get; set; }

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
