namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using System;
    using System.Collections.Generic;
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;

    public class CPOLicensingApplication21
    {
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
        //  Номер на експертна комисия
        //</summary>
        public string ExpertCommissionName { get; set; }

        //<summary>
        //  Номер на протокол
        //</summary>
        public string ProtocolNumber { get; set; }

        //<summary>
        //  Дата на входиране на заповед от Приложение 19
        //</summary>
        public DateTime ProtocolInputDate { get; set; }
        public string ProtocolInputDateFormatted { get { return ProtocolInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Основна информация за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Номер на заповед от Приложение 19
        //</summary>
        public string App19OrderNumber { get; set; }

        //<summary>
        //  Дата на входиране на заповед от Приложение 19
        //</summary>
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
