namespace ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc
{
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using System;

    public class CPOLicenseChangeApplication9
    {
        //<summary>
        //  Номер на заповед
        //</summary>
        public string OrderNumber { get; set; }

        public DateTime? OrderDate { get; set; }
        public string OrderDateFormatted { get { return this.OrderDate.HasValue ? this.OrderDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "........"; } }

        //<summary>
        //  Номер на лицензия
        //</summary>
        public string LicenseNumber { get; set; }

        //<summary>
        //  Основна информация за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Име на главен експерт
        //</summary>
        public string ChiefExpert { get; set; }
    }
}
