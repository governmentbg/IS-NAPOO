namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using System;
    using System.Collections.Generic;
    
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.CPO.ProviderData;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;

    public class CPOLicensingApplication3
    {
        public CPOLicensingApplication3()
        {
            this.ExternalExperts = new List<Expert>();
            this.ProcedureExternalExperts = new List<ProcedureExternalExpertVM>();
        }

        //<summary>
        //  Информация за лицето за контакт
        //</summary>
        public ContactPersonData ContactPerson { get; set; }

        //<summary>
        //  Фамилно име на лицето за контакт
        //</summary>
        public string ContactPersonSirname { get; set; }

        //<summary>
        //  Реф. номер на заявление
        //</summary>
        public string ApplicationNumber { get; set; }

        //<summary>
        //  Дата на входиране на заявление
        //</summary>
        public DateTime ApplicationInputDate { get; set; }
        public string ApplicationInputDateFormatted { get { return ApplicationInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Основни данни за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

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
        //  Име на ВЕ
        //</summary>
        public string ChiefExpert { get; set; }

        //<summary>
        //  Размер на такса в цифри
        //</summary>
        public string IntegerTax { get; set; }
        
        //<summary>
        //  Размер ма такса в думи
        //</summary>
        public string StringTax { get; set; }

        //<summary>
        //  Списък с ВЕ-ти
        //</summary>
        public List<Expert> ExternalExperts { get; set; }

        /// <summary>
        /// Експерти за професионални направления 
        /// </summary>
        public List<ProcedureExternalExpertVM> ProcedureExternalExperts { get; set; }
    }
}
