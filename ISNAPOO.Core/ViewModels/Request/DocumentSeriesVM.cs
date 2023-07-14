using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class DocumentSeriesVM
    {
        public int IdDocumentSeries { get; set; }

        [Range(2, int.MaxValue, ErrorMessage = "Полето 'Вид на документа' е задължително!")]
        [Comment("Връзка с  Тип документ към печатница на МОН")]
        public int IdTypeOfRequestedDocument { get; set; }

        public virtual TypeOfRequestedDocumentVM TypeOfRequestedDocument { get; set; }

        [Range(2, int.MaxValue, ErrorMessage = "Полето 'Година' е задължително!")]
        [Required(ErrorMessage = "Полето 'Година' е задължително!")]
        public int? Year { get; set; }

        [Required(ErrorMessage = "Полето 'Серия' е задължително!")]
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
