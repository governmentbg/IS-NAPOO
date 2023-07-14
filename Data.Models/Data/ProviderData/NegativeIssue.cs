 
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Models.Data.ProviderData
{


    /// <summary>
    /// Данни за установени непълноти и неточности
    /// </summary>
    [Table("Procedure_NegativeIssue")]
    [Display(Name = " Данни за установени непълноти и неточности")]
    public class NegativeIssue : IEntity, IModifiable
    {
        

        [Key]
        public int IdNegativeIssue { get; set; }
        public int IdEntity => IdNegativeIssue;

        [Display(Name = "Връзка с  Данни за Процедура за лицензиране")]
        [ForeignKey(nameof(StartedProcedure))]
        public int IdStartedProcedure { get; set; }
        public StartedProcedure StartedProcedure { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Установени непълноти и неточности")]
        public string? NegativeIssueText { get; set; }

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

    }
}

