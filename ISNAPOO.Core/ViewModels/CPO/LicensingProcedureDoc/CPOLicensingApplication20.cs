namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using System.Collections.Generic;

    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using ISNAPOO.Core.ViewModels.SPPOO;

    public class CPOLicensingApplication20
    {
        //<summary>
        //  Лице за контакт
        //</summary>
        public ContactPersonData ContactPersonData { get; set; }

        //<summary>
        //  Основна информация за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Главен експерт
        //</summary>
        public string ChiefExpert { get; set; }

        //<summary>
        //  Списък с професионални направления
        //</summary>
        public List<ProfessionalDirectionVM> ProfessionalDirections { get; set; }
    }
}
