using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using ISNAPOO.Core.ViewModels.Candidate;
using Data.Models.Data.Archive;
using System.Collections.Generic;
using Data.Models.Data.Framework;

namespace ISNAPOO.Core.ViewModels.Archive
{
    public class AnnualInfoVM : IDataMigration
    {
        public AnnualInfoVM()
        {
            this.AnnualInfoStatuses = new HashSet<AnnualInfoStatusVM>();
        }
        public int IdAnnualInfo { get; set; }

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]        
        public int IdCandidateProvider { get; set; }
        public CandidateProviderVM CandidateProvider { get; set; }

        [Required]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Година' е задължително!")]
        public int Year { get; set; }


        [Required(ErrorMessage = "Полето 'Име на лица подало годишната информаця' е задължително!")]        
        public string Name { get; set; }

        
        [Comment("Длъжност")]
        public string? Title { get; set; }


        
        [Comment("Телефон")]
        public string? Phone { get; set; }

        
        [Comment("E-mail")]
        [EmailAddress(ErrorMessage = "Невалиден E-mail адрес!")]
        public string? Email { get; set; }

        [Comment("Статус на отчета за годишна информация")]
        public int IdStatus { get; set; } // Номенклатура - KeyTypeIntCode: "AnnualInfoStatusType"

        [Comment("Статус на отчета за годишна информация - Име")]
        public string StatusName { get; set; }

        [Comment("Статус на отчета за годишна информация - IntCode")]
        public string StatusIntCode { get; set; }

        [Comment("Коментар на статус на отчета за годишна информация")]
        public string CommentAnnualInfoStatus { get; set; }
        public virtual ICollection<AnnualInfoStatusVM> AnnualInfoStatuses { get; set; }

        [Comment("Показва дали има приключили курсове през годината")]
        public bool? HasCoursePerYear { get; set; } = false;

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
