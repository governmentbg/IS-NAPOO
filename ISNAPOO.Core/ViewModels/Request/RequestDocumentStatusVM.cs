using ISNAPOO.Core.ViewModels.Candidate;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class RequestDocumentStatusVM
    {
        public int IdRequestDocumentStatus { get; set; }

        [Comment("Връзка с  CPO,CIPO - Обучаваща институция")]
        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Comment("Връзка със заявка подадена от ЦПО/ЦИПО")]
        public int IdProviderRequestDocument { get; set; }

        public virtual ProviderRequestDocumentVM ProviderRequestDocument { get; set; }

        /// <summary>
        /// създадена
        /// подадена
        /// обработена
        /// обобщена
        /// изпълнена
        /// </summary>
        [Comment("Статус")]
        public int IdStatus { get; set; }//Нова номенклатура статус на заявка:  RequestDocumetStatus

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
