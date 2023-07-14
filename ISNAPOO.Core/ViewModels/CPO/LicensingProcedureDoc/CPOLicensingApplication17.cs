
namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using System;
    using System.Collections.Generic;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using ISNAPOO.Core.ViewModels.SPPOO;

    public class CPOLicensingApplication17
    {
        //<summary>
        //  Председател на експертна комисия
        //</summary>
        public string ExpertCommissionChairman { get; set; }

        //<summary>
        //  Име на експертна комисия
        //</summary>
        public string ExpertCommissionName { get; set; }

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
        //  Основни данни за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

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
        //  Главен експерт
        //</summary>
        public string ChiefExpert { get; set; }

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
        //  Общ резултат
        //</summary>
        public string TotalScore { get; set; }

        //<summary>
        //  Резултати от оценка на експертна комисия
        //</summary>
        public Dictionary<string, string> ExpertCommissionScores { get; set; }

        //<summary>
        //  Списък с професионални направления
        //</summary>
        public List<ProfessionalDirectionVM> ProfessionalDirections { get; set; }
    }
}
