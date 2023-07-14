namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using System;
    using System.Collections.Generic;
    
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;

    public class CPOLicensingApplication13
    {
        //<summary>
        //  Началник на експертната комисия
        //</summary>
        public string HeadOfExpertCommission { get; set; }

        //<summary>
        //  Членове на експертната комисия
        //</summary>
        public List<string> ExpertCommissionMembers { get; set; }

        //<summary>
        //  Име на експертната комисия
        //</summary>
        public string ExpertCommissionName { get; set; }

        //<summary>
        //  Ден на срещата
        //</summary>
        public DateTime? DateOfMeeting { get; set; }
        public string DateOfMeetingFormatted { get { return DateOfMeeting.HasValue ? DateOfMeeting.Value.ToString(GlobalConstants.DATE_FORMAT) : string.Empty; } }

        //<summary>
        //  Ден от седмицата
        //</summary>
        public string DayOfWeek { get; set; }

        //<summary>
        //  Час на съвещанието
        //</summary>
        public TimeOnly Time { get; set; }
        //<summary>
        //  Час на съвещанието текст
        //</summary>
        public string MeetingHour { get; set; }

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
        //  Данни за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Главен експерт на ПКЛ
        //</summary>
        public string ChiefExpert { get; set; }

    }
}
