using Data.Models.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Training;
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
    /// Тип докумет към печатница на МОН 
    /// </summary>
    [Table("Request_TypeOfRequestedDocument")]
    [Display(Name = "Тип докумет към печатница на МОН")]
    public class TypeOfRequestedDocument : IEntity, IModifiable, IDataMigration
    {
        public TypeOfRequestedDocument() 
        {
            this.RequestDocumentTypes = new HashSet<RequestDocumentType>();
            this.ProviderDocumentOffers = new HashSet<ProviderDocumentOffer>();
            this.RequestDocumentManagements = new HashSet<RequestDocumentManagement>();
            this.DocumentSerialNumbers = new HashSet<DocumentSerialNumber>();
            this.DocumentSeries = new HashSet<DocumentSeries>();
            this.ClientCourseDocuments = new HashSet<ClientCourseDocument>();
        }

        [Key]
        public int IdTypeOfRequestedDocument { get; set; }
        public int IdEntity => IdTypeOfRequestedDocument;

        [StringLength(DBStringLength.StringLength255)]
        [Comment("Официален номер на документ")]
        public string? DocTypeOfficialNumber { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Comment( "Наименование на документ")]
        public string? DocTypeName { get; set; }


        [Comment("Докуента е валиден")]
        public bool IsValid { get; set; }

        [Comment("Текущ период")]
        public int CurrentPeriod { get; set; }

        [Comment("Единична цена")]
        public float Price { get; set; }

        [Comment("Номер по ред")]
        public int Order { get; set; }

        [Comment("Има серийни номера")]
        public bool HasSerialNumber { get; set; }

        [Comment("IsDestroyable")]
        public bool IsDestroyable { get; set; }

        [Comment("Вид на курса за обучение, по който може да се получи типът документ")]
        public int? IdCourseType { get; set; } // Номенклатура TypeFrameworkProgram Валидиране на степен на професионална квалификация, Валидиране на част от професия, Издаване на дубликат, обучение за ключови компетентности, професионално обучение по част от професия и др.

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Стойност по подразбиране")]
        public string? DefaultValue1 { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Comment("Наименование на документ на английски")]
        public string? DocTypeNameEN { get; set; }

        public virtual ICollection<RequestDocumentType> RequestDocumentTypes { get; set; }

        public virtual ICollection<ProviderDocumentOffer> ProviderDocumentOffers { get; set; }

        public virtual ICollection<RequestDocumentManagement> RequestDocumentManagements { get; set; }

        public virtual ICollection<DocumentSeries> DocumentSeries { get; set; }

        public virtual ICollection<DocumentSerialNumber> DocumentSerialNumbers { get; set; }

        public virtual ICollection<ClientCourseDocument> ClientCourseDocuments { get; set; }

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
