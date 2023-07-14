namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using System;
    using System.Collections.Generic;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using ISNAPOO.Core.ViewModels.SPPOO;

    public class CPOLicensingApplication16
    {
        //<summary>
        //  Номер на договор
        //</summary>
        public string ProtcolNumber { get; set; }

        //<summary>
        //  Номер на договор
        //</summary>
        public string ExpertCommissionName { get; set; }

        //<summary>
        //  Номер на договор
        //</summary>
        public DateTime? DateOfDraft { get; set; }
        public string DateOfDraftFormatted { get { return DateOfDraft.HasValue ? DateOfDraft.Value.ToString(GlobalConstants.DATE_FORMAT) : string.Empty; } }

        //<summary>
        //  Номер на договор
        //</summary>
        public TimeOnly Time { get; set; }
        public string TimeFormatted { get { return Time.ToString(GlobalConstants.TIME_FORMAT); } }

        public string MeetingHour { get; set; }

        //<summary>
        //  Номер на договор
        //</summary>
        public string DistanceConnectionSoftwareName { get; set; }

        //<summary>
        //  Номер на договор
        //</summary>
        public string OrderNumber { get; set; }

        //<summary>
        //  Номер на договор
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Номер на договор
        //</summary>
        public DateTime OrderInputDate { get; set; }
        public string OrderInputDateFormatted { get { return OrderInputDate.ToString(GlobalConstants.DATE_FORMAT); } }
        
        //<summary>
        //  Общ резултат от оценяване
        //</summary>
        public string TotalScore { get; set; }

        //<summary>
        //  Началник на експертна комисия
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
        //  Резултати от оценка на експертна комисия
        //</summary>
        public Dictionary<string, string> ExpertCommissionScores { get; set; }

        //<summary>
        //  Списък с професионални направления
        //</summary>
        public List<ProfessionalDirectionVM> ProfessionalDirections { get; set; }
    }
}
