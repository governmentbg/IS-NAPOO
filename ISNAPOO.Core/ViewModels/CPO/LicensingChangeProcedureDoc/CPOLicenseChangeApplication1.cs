namespace ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc
{
    using System;
    using System.Collections.Generic;
    
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.CPO.ProviderData;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using ISNAPOO.Core.ViewModels.SPPOO;
    
    public class CPOLicenseChangeApplication1
    {
        public CPOLicenseChangeApplication1()
        {
            this.ProfessionalDirections = new List<ProfessionalDirectionVM>();
            this.ExternalExperts = new List<Expert>();
            this.ProcedureExternalExperts = new List<ProcedureExternalExpertVM>();
        }

        /// <summary>
        /// Главен експерт в дирекция „Професионална квалификация и лицензиране“
        /// </summary>
        public string ChiefExpert { get; set; }

        /// <summary>
        /// Номер на заявление 
        /// </summary>
        public string ApplicationNumber { get; set; }

        /// <summary>
        /// Дата на заявлеие
        /// </summary>
        public DateTime ApplicationDate { get; set; }
        public string ApplicationDateFormatted { get { return ApplicationDate.ToString(GlobalConstants.DATE_FORMAT); } }

        /// <summary>
        /// Основни данни за ЦПО
        /// </summary>
        public CPOMainData CPOMainData { get; set; }


        /// <summary>
        ///     Номер на лицензия
        /// </summary>
        public string LicenseNumber { get; set; }

        /// <summary>
        /// Професионални направления, Професии, Специалности
        /// </summary>
        public List<ProfessionalDirectionVM> ProfessionalDirections { get; set; }

        /// <summary>
        /// Експерти за професионални направления 
        /// </summary>
        public List<Expert> ExternalExperts { get; set; }

        public List<ProcedureExternalExpertVM> ProcedureExternalExperts { get; set; }

        /// <summary>
        /// Експертната  комисия  
        /// </summary>
        public string ExpertCommissionName { get; set; }

        /// <summary>
        /// Краен срок за изготяване на доклад 
        /// </summary>
        public DateTime ExpertCommissionReportTerm { get; set; }
        public string ExpertCommissionReportTermFormatted { get { return ExpertCommissionReportTerm.ToString(GlobalConstants.DATE_FORMAT); } }
    }
}
