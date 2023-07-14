namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using System;
    using System.Collections.Generic;

    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using ISNAPOO.Common.Constants;

    public class CPOLicensingApplication6
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
        //  Реф. номер на заявление
        //</summary>
        public string ApplicationNumber { get; set; }

        //<summary>
        //  Дата на входиране на заявление
        //</summary>
        public DateTime ApplicationInputDate { get; set; }
        public string ApplicationInputDataFormatted { get { return ApplicationInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Списък с установени нередовности
        //</summary>
        public List<string> Issues { get; set; }
    }
}
