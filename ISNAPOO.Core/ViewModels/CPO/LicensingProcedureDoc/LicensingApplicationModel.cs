namespace ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc
{
    using ISNAPOO.Core.ViewModels.Candidate;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using ISNAPOO.Core.ViewModels.SPPOO;
    using System.Collections.Generic;
    
    public class LicensingApplicationModel
    {
        public CandidateProviderVM CandidateProvider { get; set; }

        public List<SpecialityVM> Specialities { get; set; }
    }
}
