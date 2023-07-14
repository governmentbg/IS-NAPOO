using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.ExternalExpertCommission
{

    /// <summary>
    /// Външен експерт/Член на експертна комисия връзка с Професионално направление
    /// </summary>
    [Table("ExpComm_ExpertProfessionalDirection")]
    [Display(Name = "Външен експерт/Член на експертна комисия връзка с Професионално направление")]
    public class ExpertProfessionalDirection : IEntity, IModifiable, IDataMigration
    {
        public ExpertProfessionalDirection()
        {
            
        }

        [Key]
        public int IdExpertProfessionalDirection { get; set; }
        public int IdEntity => IdExpertProfessionalDirection;

        [Required]
        [Display(Name = "Експерт")]
        [ForeignKey(nameof(Expert))]
        public int IdExpert { get; set; }
        public virtual Expert Expert { get; set; }

        [Required]
        [Display(Name = "Вид експерт")]//Външен експерт(ExternalExpert), Член на експертна комисия(MemberExpertCommission)
        public int IdExpertType { get; set; }

        [Required]
        [Display(Name = "Професионално направление")]
        [ForeignKey(nameof(ProfessionalDirection))]
        public int IdProfessionalDirection { get; set; }
        public virtual ProfessionalDirection ProfessionalDirection { get; set; }

        [Required]
        [Display(Name = "Статус")]//Активен/Неактивен
        public int IdStatus { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Коментар при промяна на статуса")]
        public string? Comment { get; set; }

        [Display(Name = "Дата на утвърждаване, като външен експерт")]
        public DateTime? DateApprovalExternalExpert { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Display(Name = "Номер на заповед за утвърждаване, като външен експерт ")]
        public string? OrderNumber { get; set; }

        [Display(Name = "Дата на заповедта с която е включен в ЕК")]
        public DateTime? DateOrderIncludedExpertCommission { get; set; }



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
