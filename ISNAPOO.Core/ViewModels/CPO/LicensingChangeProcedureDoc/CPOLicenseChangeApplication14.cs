namespace ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc
{
    using System;

    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    
    public class CPOLicenseChangeApplication14
    {
        //<summary>
        //  Номер на договор
        //</summary>
        public string ContractNumber { get; set; }

        //<summary>
        //  Дата на изготвяне
        //</summary>
        public DateTime DateOfDraft { get; set; }
        public string DateOfDraftFormatted { get { return DateOfDraft.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Име на експертна комисия
        //</summary>
        public string ExpertCommissionName { get; set; }

        //<summary>
        //  Главен експерт
        //</summary>
        public Expert ChiefExpert { get; set; }

        public ExpertVM ExpertDataVM { get; set; }

        //<summary>
        //  Основни данни за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Номер на заповед
        //</summary>
        public string OrderNumber { get; set; }

        public DateTime? OrderDate { get; set; }
        public string OrderDateFormatted { get { return this.OrderDate.HasValue ? this.OrderDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "........"; } }
    }
}
