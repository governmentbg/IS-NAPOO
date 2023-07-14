namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using System;

    public class CPOLicensingApplication5
    {
        //<summary>
        //  Информация за лицето за контакт
        //</summary>
        public ContactPersonData ContactPersonData { get; set; }
        
        //<summary>
        //  Размер на такса в цифри
        //</summary>
        public string IntegerTax { get; set; }

        //<summary>
        //  Размер на такса в думи
        //</summary>
        public string StringTax { get; set; }

        //<summary>
        //  Изходящ номер на уведмително писмо
        //</summary>
        public string OutputNumber { get; set; }

        //<summary>
        //  Дата на уведмително писмо
        //</summary>
        public DateTime OutputDate { get; set; }
        public string OutputDateFormatted { get { return OutputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Име на главен експерт
        //</summary>
        public string ChiefExpert { get; set; }

        //<summary>
        //  Телефонен номер за контакт с главен експерт
        //</summary>
        public string TelephoneNumber { get; set; }

        //<summary>
        //  Адрес на електронна поща за контакт с главен експерт
        //</summary>
        public string EmailAddress { get; set; }
    }
}
