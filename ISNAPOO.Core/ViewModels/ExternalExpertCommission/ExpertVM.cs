namespace ISNAPOO.Core.ViewModels.ExternalExpertCommission
{
    using Data.Models.Data.ExternalExpertCommission;
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Common.Framework;
    using ISNAPOO.Core.Mapping;
    using ISNAPOO.Core.ViewModels.Common;
    using ISNAPOO.Core.ViewModels.CPO.ProviderData;
    using ISNAPOO.Core.ViewModels.EKATTE;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ExpertVM : IMapFrom<Expert>, IMapTo<Expert>
    {
        public ExpertVM()
        {
            this.ExpertProfessionalDirections = new List<ExpertProfessionalDirectionVM>();
            this.ExpertDocuments = new List<ExpertDocumentVM>();
            this.Person = new PersonVM();
            this.ExpertExpertCommissions = new List<ExpertExpertCommissionVM>();
            this.ExpertDOCs = new List<ExpertDOCVM>();
            this.ProcedureExternalExperts = new List<ProcedureExternalExpertVM>();
            this.ProcedureDocuments = new List<ProcedureDocumentVM>();
        }

        public int IdExpert { get; set; }

        [Display(Name = "Връзка с лице")]
        public int? IdPerson { get; set; }
        public PersonVM Person { get; set; }

        // Инфо. за професионални направления
        public string ProfessionalDirectionsInfo { get; set; }
        public string ProfessionalDirectionsInfoTrim { get; set; }
        // Инфо. за експертните комисии
        public string CommissionsInfo { get; set; }  
        // Инфо. за РГ/Рецензенти на ДОС
        public string DOCsInfo { get; set; }        

        // Филтър полета
        public int IdExpertTypeFilter { get; set; }
        public int IdProfessionalDirectionFilter { get; set; }
        public int IdStatusFilter { get; set; }
        public DateTime? DateApprovalExternalExpertFilter { get; set; }
        public string? OrderNumberFilter { get; set; }
        public DateTime? DateOrderIncludedExpertCommissionFilter { get; set; }

        [Display(Name = "Лицето е външен експерт")]
        public bool IsExternalExpert { get; set; }
        [Display(Name = "Лицето е член на комисия")]
        public bool IsCommissionExpert { get; set; }
        [Display(Name = "Лицето е Служител на НАПОО")]
        public bool IsNapooExpert { get; set; }

        [Display(Name = "Лицето е член на работни групи за разработване на ДОС")]
        public bool IsDOCExpert { get; set; }

        public string ProfessionalDirectionStr { get; set; }
        public string ProfessionalDirectionCode { get; set; }
        public string FullNameAndOccupation { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }
        public int ProcedureCount { get { return ProcedureExternalExperts.Count; } }

        // Свързани списъци
        public List<ExpertProfessionalDirectionVM> ExpertProfessionalDirections { get; set; }
        public List<ExpertDocumentVM> ExpertDocuments { get; set; }

        public List<ExpertExpertCommissionVM> ExpertExpertCommissions { get; set; }
        public List<ExpertDOCVM> ExpertDOCs { get; set; }
        public List<ProcedureExternalExpertVM> ProcedureExternalExperts { get; set; }
        public List<ProcedureDocumentVM> ProcedureDocuments { get; set; }

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