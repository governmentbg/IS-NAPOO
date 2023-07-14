using System;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ISNAPOO.Common.Constants;
using Data.Models.Data.Control;

namespace Data.Models.Data.Candidate
{


    /// <summary>
    /// Връзка на Метериална техническа база и направена проверка
    /// </summary>
    [Table("Candidate_ProviderPremisesChecking")]
    [Display(Name = "Връзка на Метериална техническа база и направена проверка")]
    public class CandidateProviderPremisesChecking : IEntity, IModifiable
    {
        public CandidateProviderPremisesChecking()
        {
        }

        [Key]
        public int IdCandidateProviderPremisesChecking { get; set; }
        public int IdEntity => IdCandidateProviderPremisesChecking;

        [Comment("Връзка с MTB")]
        [ForeignKey(nameof(CandidateProviderPremises))]
        public int IdCandidateProviderPremises { get; set; }
        public CandidateProviderPremises CandidateProviderPremises { get; set; }

        [Comment("Последващ контрол, изпълняван от служител/и на НАПОО")]
        [ForeignKey(nameof(FollowUpControl))]
        public int? IdFollowUpControl { get; set; }

        public FollowUpControl FollowUpControl { get; set; }

        [Comment("Извършена проверка от експерт на НАПОО")]
        public bool CheckDone { get; set; }


        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Коментар' не може да съдържа повече от 512 символа.")]
        [Comment("Коментар")]
        public string? Comment { get; set; }


        [Comment("Дата на проверка")]
        public DateTime? CheckingDate { get; set; }


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