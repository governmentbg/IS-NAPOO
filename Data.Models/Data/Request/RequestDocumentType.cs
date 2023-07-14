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
    /// Тип докумет за заявка към печатница на МОН 
    /// </summary>
    [Table("Request_RequestDocumentType")]
    [Display(Name = "Статус на докумет от заявка към печатница на МОН")]
    public class RequestDocumentType : IEntity, IModifiable, IDataMigration
    {
        public RequestDocumentType() { }

        [Key]
        public int IdRequestDocumentType { get; set; }
        public int IdEntity => IdRequestDocumentType;

        [Comment("Връзка с  CPO,CIPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }


        [Comment("Връзка със заявка подадена от ЦПО/ЦИПО")]
        [ForeignKey(nameof(ProviderRequestDocument))]
        public int IdProviderRequestDocument { get; set; }
        public ProviderRequestDocument ProviderRequestDocument { get; set; }


        [Comment("Връзка с Тип документ")]
        [ForeignKey(nameof(TypeOfRequestedDocument))]
        public int IdTypeOfRequestedDocument { get; set; }
        public TypeOfRequestedDocument TypeOfRequestedDocument { get; set; }

        [Comment("Брой документи")]
        public int DocumentCount { get; set; }


        [Comment("Връзка с получен документ")]
        [ForeignKey(nameof(RequestDocumentManagement))]
        public int? IdRequestDocumentManagement { get; set; }
        /// <summary>
        /// Връзка с получен документ
        /// </summary>
        public RequestDocumentManagement RequestDocumentManagement { get; set; }


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
