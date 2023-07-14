using Data.Models.Common;
using Data.Models.Data.Candidate;
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
    /// Борса на документи
    /// </summary>
    [Table("Request_ProviderDocumentOffer")]
    [Display(Name = "Борса на документи")]
    public class ProviderDocumentOffer : IEntity, IModifiable
    {
        [Key]
        public int IdProviderDocumentOffer { get; set; }
        public int IdEntity => IdProviderDocumentOffer;

        [Comment("Връзка с  CPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }

        [Required]
        [Comment("Вид на оферта")]
        public int IdOfferType { get; set; } //Номенклатура (Търся, Предлагам)

        [Comment("Връзка с Тип документ")]
        [ForeignKey(nameof(TypeOfRequestedDocument))]
        public int IdTypeOfRequestedDocument { get; set; }
        public TypeOfRequestedDocument TypeOfRequestedDocument { get; set; }

        [Comment("Брой предлагани/търсени документи")]
        public int CountOffered { get; set; }

        [Required]
        [Comment("Начална дата на офертата")]
        public DateTime OfferStartDate { get; set; }

        [Comment("Крайна дата на офертата")]
        public DateTime? OfferEndDate { get; set; }

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
