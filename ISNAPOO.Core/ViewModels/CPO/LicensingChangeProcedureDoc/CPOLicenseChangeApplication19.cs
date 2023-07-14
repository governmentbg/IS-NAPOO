
namespace ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc
{
    using System;
    using System.Collections.Generic;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using ISNAPOO.Core.ViewModels.SPPOO;

    public class CPOLicenseChangeApplication19
    {
        //<summary>
        //  Номер на заповед
        //</summary>
        public string OrderNumber { get; set; }

        //<summary>
        //  Име на експертна комисия
        //</summary>
        public string ExpertCommissionName { get; set; }

        //<summary>
        //  Номер на доклад
        //</summary>
        public string ReportNumber { get; set; }

        //<summary>
        //  Дата на входиране на доклад
        //</summary>
        public DateTime ReportInputDate { get; set; }
        public string ReportInputDateFormatted { get { return ReportInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Номер на заявление
        //</summary>
        public string ApplicationNumber { get; set; }

        //<summary>
        //  Дата на входиране на заявление
        //</summary>
        public DateTime ApplicationInputDate { get; set; }
        public string ApplicationInputDateFormatted { get { return ApplicationInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Основни данни за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Номер на лицензия
        //</summary>
        public string LicenseNumber { get; set; }

        //<summary>
        //  Дата на входиране на лицензия
        //</summary>
        public DateTime LicenseInputDate { get; set; }
        public string LicenseInputDateFormatted { get { return LicenseInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Брой професии
        //</summary>
        public string ProfessionsCount { get; set; }

        //<summary>
        //  Брой специалности
        //</summary>
        public string SpecialitiesCount { get; set; }

        //<summary>
        //  Главен експерт
        //</summary>
        public string ChiefExpert { get; set; }

        //<summary>
        //  Списък с професионални направления
        //</summary>
        public List<ProfessionalDirectionVM> ProfessionalDirections { get; set; }
    }
}
