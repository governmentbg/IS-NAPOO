namespace ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc
{
    using System;
    
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    
    public class CPOLicenseChangeApplication4
    {
        //<summary>
        //  Реф. номер на договор
        //</summary>
        public string ContractNumber { get; set; }

        //<summary>
        //  Дата на създаване
        //</summary>
        public DateTime DateOfDraft { get; set; }
        public string DateOfDraftFormatted { get { return DateOfDraft.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Реф. номер на заповед
        //</summary>
        public string OrderNumber { get; set; }

        //<summary>
        //  Дата на входиране на заповед
        //</summary>
        public DateTime OrderInputDate { get; set; }
        public string OrderInputDateFormatted { get { return OrderInputDate.ToString(GlobalConstants.DATE_FORMAT); } }

        //<summary>
        //  Основни данни за ЦПО
        //</summary>
        public CPOMainData CPOMainData { get; set; }

        //<summary>
        //  Данни за ВЕ
        //</summary>
        public Expert ExternalExpertData { get; set; }

        public ExpertVM ExpertDataVM { get; set; }

        //<summary>
        //  Срок за изпълнение на договор
        //</summary>
        public DateTime ContractTerm { get; set; }
        public string ContractTermFormatted { get { return ContractTerm.ToString(GlobalConstants.DATE_FORMAT); } }
    }
}
