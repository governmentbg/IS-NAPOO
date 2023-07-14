namespace ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc
{
    using System;
    using System.Collections.Generic;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;

    public class CPOLicenseChangeApplication13
    {
        //<summary>
        //  Главен експерт
        //</summary>
        public string HeadOfExpertCommission { get; set; }

        //<summary>
        //  Членове на експертна комисия
        //</summary>
        public List<string> ExpertCommissionMembers { get; set; }

        //<summary>
        //  Име на експертна комисия
        //</summary>
        public string ExpertCommissionName { get; set; }

        //<summary>
        //  Дата на заседние
        //</summary>
        public DateTime? DateOfMeeting { get; set; }
        public string DateOfMeetingFormatted { get { return DateOfMeeting.HasValue ? DateOfMeeting.Value.ToString(GlobalConstants.DATE_FORMAT) : string.Empty; } }

        //<summary>
        //  Ден от седмицата
        //</summary>
        public string DayOfWeek { get; set; }

        //<summary>
        //  Час на срещата
        //</summary>
        public TimeOnly Time { get; set; }

        public string MeetingHour { get; set; }

        //<summary>
        //  Номер на заповед
        //</summary>
        public string OrderNumber { get; set; }

        public DateTime? OrderDate { get; set; }
        public string OrderDateFormatted { get { return this.OrderDate.HasValue ? this.OrderDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "........"; } }

        //<summary>
        //  Основни данни за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Главен експерт
        //</summary>
        public string ChiefExpert { get; set; }

    }
}
