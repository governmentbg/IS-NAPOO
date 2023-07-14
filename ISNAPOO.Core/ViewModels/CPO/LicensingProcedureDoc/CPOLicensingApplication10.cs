namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using System;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    
    public class CPOLicensingApplication10
    {
        //<summary>
        //  Информация за лицето за контакт
        //</summary>
        public ContactPersonData ContactPersonData { get; set; }

        //<summary>
        //  Фамилно име лицето за контакт
        //</summary>
        public string ContactPersonSirname { get; set; }

        //<summary>
        //  Основни данни на ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Реф. номер на известие
        //</summary>
        public string NotificationLetterNumber { get; set; }

        //<summary>
        //  Изходяща дата на известие
        //</summary>
        public DateTime NotificationLetterOutputDate { get; set; }
        public string NotificationLetterOutputDateFormatted { get { return NotificationLetterOutputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Краен срок
        //</summary>
        public DateTime DueDate { get; set; }
        public string DueDateFormatted { get { return DueDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Реф. номер на заповед
        //</summary>
        public string OrderNumber { get; set; }

        //<summary>
        //  Изходяща дата на заповед
        //</summary>
        public DateTime OrderInputDate { get; set; }
        public string OrderInputDateFormatted { get { return OrderInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Име на главен експерт 
        //</summary>
        public string ChiefExpert { get; set; }
    }
}
