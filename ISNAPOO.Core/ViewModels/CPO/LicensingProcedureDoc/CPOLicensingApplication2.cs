﻿namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using System;
    using System.Collections.Generic;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.CPO.ProviderData;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;

    public class CPOLicensingApplication2
    {
        public CPOLicensingApplication2()
        {
            this.ExternalExperts = new List<Expert>();
            this.ProcedureExternalExperts = new List<ProcedureExternalExpertVM>();
        }

        //<summary>
        //  Номер на заповед
        //</summary>
        public string OrderNumber { get; set; }

        //<summary>
        //  Дата на входиране
        //</summary>
        public DateTime? OrderDate { get; set; }
        public string OrderDateFormatted { get { return this.OrderDate.HasValue ? this.OrderDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "........"; } }

        //<summary>
        //  Основни данни за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Брой професии
        //</summary>
        public int ProfessionsCount { get; set; }

        //<summary>
        //  Брой специалности
        //</summary>
        public int SpecialtiesCount { get; set; }

        /// <summary>
        /// Експерти за професионални направления 
        /// </summary>
        public List<ProcedureExternalExpertVM> ProcedureExternalExperts { get; set; }

        //<summary>
        //  Списък с външни експерти
        //</summary>
        public List<Expert> ExternalExperts { get; set; }

        //<summary>
        //  Срок за предаване на доклад от Външна комисия
        //</summary>
        public DateTime ExternalExpertCommissionReportTerm { get; set; }
        public string ExternalExpertCommissionReportTermFormatted { get { return ExternalExpertCommissionReportTerm.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Име на Експертна комисия
        //</summary>
        public string ExpertCommissionName { get; set; }

        //<summary>
        //  Срок за предаване на доклад от Експертна комисия
        //</summary>
        public DateTime ExpertCommissionReportTerm { get; set; }
        public string ExpertCommissionReportTermFormatted { get { return ExpertCommissionReportTerm.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        // Главен експерт
        //</summary>
        public string ChiefExpert { get; set; }
    }
}
