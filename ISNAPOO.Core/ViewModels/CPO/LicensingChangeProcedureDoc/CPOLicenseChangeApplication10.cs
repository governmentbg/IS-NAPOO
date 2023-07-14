namespace ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc
{
    using System;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    
    public class CPOLicenseChangeApplication10
    {
        //<summary>
        //  Информация за лицето за контакт
        //</summary>
        public ContactPersonData ContactPersonData { get; set; }

        //<summary>
        //  Номер на лицензия
        //</summary>
        public string LicenseNumber { get; set; }

        //<summary>
        //  Основни данни на ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Реф. номер на известие
        //</summary>
        public string NotificationLetterNumber { get; set; }

        //<summary>
        //  Краен срок
        //</summary>
        public DateTime DueDate { get; set; }
        public string DueDateFormatted { get { return DueDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Реф. номер на заповед
        //</summary>
        public string OrderNumber { get; set; }

        public DateTime? OrderDate { get; set; }
        public string OrderDateFormatted { get { return this.OrderDate.HasValue ? this.OrderDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "........"; } }

        //<summary>
        //  Име на главен експерт 
        //</summary>
        public string ChiefExpert { get; set; }
    }
}
