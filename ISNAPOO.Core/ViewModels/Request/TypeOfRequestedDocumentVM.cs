using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class TypeOfRequestedDocumentVM
    {
        public TypeOfRequestedDocumentVM()
        {
            this.RequestDocumentTypes = new HashSet<RequestDocumentTypeVM>();
            this.RequestDocumentManagements = new HashSet<RequestDocumentManagementVM>();
            this.DocumentSeries = new HashSet<DocumentSeriesVM>();
            this.DocumentSerialNumbers = new HashSet<DocumentSerialNumberVM>();
            this.ClientCourseDocuments = new HashSet<ClientCourseDocumentVM>();
        }

        public int IdTypeOfRequestedDocument { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето '№ на документа' може да съдържа до 255 символа!")]
        [Comment("Официален номер на документ")]
        [Required(ErrorMessage = "Полето '№ на документа' е задължително!")]
        public string? DocTypeOfficialNumber { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Вид на документа' може да съдържа до 255 символа!")]
        [Comment("Наименование на документ")]
        [Required(ErrorMessage = "Полето 'Вид на документа' е задължително!")]
        public string? DocTypeName { get; set; }

        [Comment("Докуента е валиден")]
        public bool IsValid { get; set; }

        [Comment("Текущ период")]
        public int CurrentPeriod { get; set; }

        [Comment("Единична цена")]
        [Range(0, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Единична цена' не може да има отрицателна стойност!")]
        public float? Price { get; set; }

        [Comment("Номер по ред")]
        [Range(0, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Номер по ред' не може да има отрицателна стойност!")]
        [Required(ErrorMessage = "Полето 'Номер по ред' е задължително!")]
        public int? Order { get; set; }

        [Comment("Има серийни номера")]
        public bool HasSerialNumber { get; set; }

        [Comment("IsDestroyable")]
        public bool IsDestroyable { get; set; }

        public string NumberWithName => $"{this.DocTypeOfficialNumber} {this.DocTypeName}";

        public string DocumentStatus => this.IsValid ? "Активен" : "Неактивен";

        public string HasSerialNumberAsText => this.HasSerialNumber ? "Да" : "Не";

        public string IsDestroyableAsText => this.IsDestroyable ? "Да" : "Не";

        public int Quantity { get; set; }

        public string PriceAsStr { get; set; }

        [Comment("Вид на курса за обучение, по който може да се получи типът документ")]
        public int? IdCourseType { get; set; } // Номенклатура TypeFrameworkProgram Валидиране на степен на професионална квалификация, Валидиране на част от професия, Издаване на дубликат, обучение за ключови компетентности, професионално обучение по част от професия и др.

        public string CourseTypeName { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Стойност по подразбиране")]
        public string? DefaultValue1 { get; set; }

        public virtual ICollection<RequestDocumentTypeVM> RequestDocumentTypes { get; set; }

        public virtual ICollection<RequestDocumentManagementVM> RequestDocumentManagements { get; set; }

        public virtual ICollection<DocumentSeriesVM> DocumentSeries { get; set; }

        public virtual ICollection<DocumentSerialNumberVM> DocumentSerialNumbers { get; set; }

        public virtual ICollection<ClientCourseDocumentVM> ClientCourseDocuments { get; set; }

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
    