using Data.Models.Data.Common;
using Data.Models.Data.Control;
using Data.Models.Data.EGovPayment;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.ExternalExpertCommission
{
    /// <summary>
    /// Експерт
    /// </summary>
    [Table("ExpComm_Expert")]
    [Display(Name = "Експерт")]
    public class Expert : IEntity, IModifiable, IDataMigration
    {
        public Expert()
        {
            this.ExpertProfessionalDirections = new List<ExpertProfessionalDirection>();
            this.ExpertDocuments = new List<ExpertDocument>();
            this.ExpertExpertCommissions = new List<ExpertExpertCommission>();
            this.ExpertDOCs = new List<ExpertDOC>();
            this.ProcedureExternalExperts = new List<ProcedureExternalExpert>();
            this.ProcedureDocuments = new List<ProcedureDocument>();
            this.FollowUpControlExperts = new List<FollowUpControlExpert>();

            this.ExpertNapoos = new List<ExpertNapoo>();
            


        }

        [Key]
        public int IdExpert { get; set; }
        public int IdEntity => IdExpert;

        [Display(Name = "Връзка с лице")]
        [ForeignKey(nameof(Person))]
        public int? IdPerson { get; set; }
        public Person Person { get; set; }

        [Display(Name = "Лицето е външен експерт")]
        public bool IsExternalExpert { get; set; }//Лицето е външен експерт

        [Display(Name = "Лицето е член на комисия")]
        public bool IsCommissionExpert { get; set; }//Лицето е член на комисия


        [Display(Name = "Лицето е Служител на НАПОО")]
        public bool IsNapooExpert { get; set; }//Лицето е Служител на НАПОО

        [Display(Name = "Лицето е член на работни групи за разработване на ДОС")]
        public bool IsDOCExpert { get; set; }//Лицето е член на работни групи за разработване на ДОС

        public virtual ICollection<ExpertProfessionalDirection> ExpertProfessionalDirections { get; set; }
        public virtual ICollection<ExpertDocument> ExpertDocuments { get; set; }
        public virtual ICollection<ExpertExpertCommission> ExpertExpertCommissions { get; set; }
        public virtual ICollection<ExpertDOC> ExpertDOCs { get; set; }
        public virtual ICollection<ProcedureExternalExpert> ProcedureExternalExperts { get; set; }
        public virtual ICollection<ProcedureDocument> ProcedureDocuments { get; set; }
        public virtual ICollection<FollowUpControlExpert> FollowUpControlExperts { get; set; }

        public virtual ICollection<ExpertNapoo> ExpertNapoos { get; set; }

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
