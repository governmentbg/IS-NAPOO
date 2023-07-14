namespace ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc
{
    using System;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;

    public class CPOLicenseChangeApplication8
    {
        //<summary>
        //  Име на главен експерт
        //</summary>
        public string ChiefExpert { get; set; }

        //<summary>
        //  Основна информация за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Номер на лицензия
        //</summary>
        public string LicenseNumber { get; set; }

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
        //  Реф. номер на уведомително писмо
        //</summary>
        public string NotificationLetterNumber { get; set; }

        //<summary>
        //  Краен срок за отстраняване на нередовности
        //</summary>
        public DateTime DueDate { get; set; }
        public string DueDateFormatted { get { return DueDate.ToString(GlobalConstants.DATE_FORMAT); } }
    }
}
