using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class RequestDocumentTypeVM
    {
        public int IdRequestDocumentType { get; set; }

        [Comment("Връзка с  CPO,CIPO - Обучаваща институция")]
        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Comment("Връзка със заявка подадена от ЦПО/ЦИПО")]
        public int IdProviderRequestDocument { get; set; }

        public virtual ProviderRequestDocumentVM ProviderRequestDocument { get; set; }

        [Comment("Връзка с Тип документ")]
        public int IdTypeOfRequestedDocument { get; set; }

        public virtual TypeOfRequestedDocumentVM TypeOfRequestedDocument { get; set; }

        [Comment("Брой документи")]
        public int DocumentCount { get; set; }

        [Comment("Връзка с получен документ")]
        public int? IdRequestDocumentManagement { get; set; }
        /// <summary>
        /// Връзка с получен документ
        /// </summary>
        public virtual RequestDocumentManagementVM RequestDocumentManagement { get; set; }

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
