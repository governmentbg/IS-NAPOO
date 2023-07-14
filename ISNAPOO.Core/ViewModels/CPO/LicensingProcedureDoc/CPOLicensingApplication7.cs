namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using System.Collections.Generic;
   
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    
    public class CPOLicensingApplication7
    {
        //<summary>
        //  Информация за лицето за контакт
        //</summary>
        public ContactPersonData ContactPersonData { get; set; }

        //<summary>
        //  Основна информация за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

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

        //<summary>
        //  Списък с установени нередовности
        //</summary>
        public List<string> Issues { get; set; }
    }
}
