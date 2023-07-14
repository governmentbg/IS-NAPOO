namespace ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc
{
    using System;
    using System.Collections.Generic;
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;

    public class CPOLicenseChangeApplication21
    {
        //<summary>
        //  Номер на заповед
        //</summary>
        public string OrderNumber { get; set; }

        public DateTime OrderInputDate { get; set; }
        public string OrderInputDateFormatted { get { return OrderInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Име на експертна комисия
        //</summary>
        public string ExpertCommissionName { get; set; }

        //<summary>
        //  Номер на протокол
        //</summary>
        public string ProtocolNumber { get; set; }

        public DateTime ProtocolInputDate { get; set; }
        public string ProtocolInputDateFormatted { get { return ProtocolInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Номер на лицензия
        //</summary>
        public string LicenseNumber { get; set; }

        //<summary>
        // Основни данни за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Номер на заповед по Приложение 19
        //</summary>
        public string App19OrderNumber { get; set; }

        public DateTime App19OrderInputDate { get; set; }
        public string App19OrderInputDateFormatted { get { return App19OrderInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Председател на експертна комисия
        //</summary>
        public string ExpertCommissionChairman { get; set; }

        //<summary>
        //  Главен експерт
        //</summary>
        public string ChiefExpert { get; set; }

        //<summary>
        //  Списък с членове
        //</summary>
        public List<string> MemberList { get; set; }
    }
}
