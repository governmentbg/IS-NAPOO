
namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using System;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;

    public class CPOLicensingApplication8
    {
        //<summary>
        //  Име на главен експерт
        //</summary>
        public string ChiefExpert { get; set; }

        //<summary>
        //  Реф. номер на уведомително писмо
        //</summary>
        public string NotificationLetterNumber { get; set; }

        //<summary>
        //  Изходяща дата на уведомително писмо
        //</summary>
        public DateTime NotificationLetterOutputDate { get; set; }
        public string NotificationLetterOutputDateFormatted { get { return NotificationLetterOutputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Основна информация за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Реф. номер на зявление
        //</summary>
        public string ApplicationNumber { get; set; }

        //<summary>
        //  Дата на входиране на заявление
        //</summary>
        public DateTime ApplicationInputDate { get; set; }
        public string ApplicationInputDateFormatted { get { return ApplicationInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Реф. номер на доклад
        //</summary>
        public string ReportNumber { get; set; }

        //<summary>
        //  Дата на входиране на доклад
        //</summary>
        public DateTime ReportInputDate { get; set; }
        public string ReportInputDateFormatted { get { return ReportInputDate.ToString(GlobalConstants.DATE_FORMAT); } }


        //<summary>
        //  Краен срок за отстраняване на нередовности
        //</summary>
        public DateTime DueDate { get; set; }
        public string DueDateAddOneMonthFormatted { get { return NotificationLetterOutputDate.AddMonths(1).ToString(GlobalConstants.DATE_FORMAT); } }
        public string DueDateFormatted { get { return DueDate.ToString(GlobalConstants.DATE_FORMAT); } }
    }
}
