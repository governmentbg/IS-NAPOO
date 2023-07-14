namespace ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc
{
    using System;
    using System.Collections.Generic;
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using ISNAPOO.Core.ViewModels.SPPOO;

    public class CPOLicenseChangeApplication18
    {
        //<summary>
        //  Главен експерт
        //</summary>
        public string ChiefExpert { get; set; }

        //<summary>
        //  Номер на лицензия
        //</summary>
        public string LicenseNumber { get; set; }

        //<summary>
        //  Основни данни за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Номер на заповед
        //</summary>
        public string OrderNumber { get; set; }

        //<summary>
        //  Дата на входиране на заповед
        //</summary>
        public DateTime OrderInputDate { get; set; }
        public string OrderInputDateFormatted { get { return OrderInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Номер на протокол
        //</summary>
        public string ProtocolNumber { get; set; }

        //<summary>
        //  Дата на входиране на протокол
        //</summary>
        public DateTime ProtocolInputDate { get; set; }
        public string ProtocolInputDateFormatted { get { return ProtocolInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Имена на председател на експертна комисия
        //</summary>
        public string ExpertCommissionChairmanFullName { get; set; }

        //<summary>
        //  Фамилно име на председател на експертна комисия
        //</summary>
        public string ExpertCommissionChairmanSirname { get; set; }

        //<summary>
        //  Име на експертна комисия
        //</summary>
        public string ExpertCommissionName { get; set; }

        //<summary>
        //  Брой професии
        //</summary>
        public string ProfessionsCount { get; set; }

        //<summary>
        //  Брой специалности
        //</summary>
        public string SpecialitiesCount { get; set; }

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
        //  Списък с професионални направления
        //</summary>
        public List<ProfessionalDirectionVM> ProfessionalDirections { get; set; }
    }
}
