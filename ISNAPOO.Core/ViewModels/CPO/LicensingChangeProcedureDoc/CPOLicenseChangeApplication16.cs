namespace ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc
{
    using System;
    using System.Collections.Generic;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using ISNAPOO.Core.ViewModels.SPPOO;

    public class CPOLicenseChangeApplication16
    {
        //<summary>
        //  Номер на протокол
        //</summary>
        public string ProtcolNumber { get; set; }

        //<summary>
        //  Име на експертна комисия
        //</summary>
        public string ExpertCommissionName { get; set; }

        //<summary>
        //  Дата на изготвяне
        //</summary>
        public DateTime? DateOfDraft { get; set; }
        public string DateOfDraftFormatted { get { return DateOfDraft.HasValue ? DateOfDraft.Value.ToString(GlobalConstants.DATE_FORMAT) : string.Empty; } }


        //<summary>
        //  Час на заседание
        //</summary>
        public TimeOnly Time { get; set; }
        public string TimeFormatted { get { return Time.ToString(GlobalConstants.TIME_FORMAT); } }

        public string MeetingHour { get; set; }

        //<summary>
        //  Име на софтуер за дистанционна комуникация
        //</summary>
        public string DistanceConnectionSoftwareName { get; set; }

        //<summary>
        //  Заповед номер
        //</summary>
        public string OrderNumber { get; set; }

        //<summary>
        //  Номер на лицензия
        //</summary>
        public string LicenseNumber { get; set; }

        //<summary>
        //  Основни данни за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Дата на входиране на заповед
        //</summary>
        public DateTime OrderInputDate { get; set; }
        public string OrderInputDateFormatted { get { return OrderInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Общ резултат
        //</summary>
        public string TotalScore { get; set; }

        //<summary>
        //  Главен експерт
        //</summary>
        public string ChiefOfExpertCommission { get; set; }

        //<summary>
        //  Протокольор
        //</summary>
        public string Protocoler { get; set; }

        //<summary>
        //  Списък с присъстващи на заседанието
        //</summary>
        public List<string> MeetingAttendance { get; set; }

        //<summary>
        //  Резултати от разглеждане
        //</summary>
        public string ReviewResults { get; set; }

        //<summary>
        //  Оценки на експертна комисия
        //</summary>
        public Dictionary<string, string> ExpertCommissionScores { get; set; }

        //<summary>
        //  Списък с професионални направления
        //</summary>
        public List<ProfessionalDirectionVM> ProfessionalDirections { get; set; }
    }
}
