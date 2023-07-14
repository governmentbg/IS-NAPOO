using Data.Models.Common;
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
    /// Серия за тип на документ
    /// </summary>
    [Table("Request_DocumentSeries")]
    [Display(Name = "Серия за тип на документ")]
    public class DocumentSeries : IEntity, IModifiable, IDataMigration
    {
        [Key]
        public int IdDocumentSeries { get; set; }
        public int IdEntity => IdDocumentSeries;

        [Comment("Връзка с  Тип документ към печатница на МОН")]
        [ForeignKey(nameof(TypeOfRequestedDocument))]
        public int IdTypeOfRequestedDocument { get; set; }
        public TypeOfRequestedDocument TypeOfRequestedDocument { get; set; }

        [Required]
        public int Year { get; set; }

        public string? SeriesName { get; set; }

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
