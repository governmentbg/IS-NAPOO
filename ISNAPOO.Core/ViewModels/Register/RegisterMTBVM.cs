using ISNAPOO.Core.ViewModels.Candidate;
using System.Collections.Generic;
using System.Linq;

namespace ISNAPOO.Core.ViewModels.Register
{
    public class RegisterMTBVM
    {
        public RegisterMTBVM()
        {
            this.CandidateProviderPremisesSpecialities = new HashSet<CandidateProviderPremisesSpecialityVM>();
            this.CandidateProviderPremisesCheckings = new HashSet<CandidateProviderPremisesCheckingVM>();
        }

        public CandidateProviderVM CandidateProvider { get; set; }

        public CandidateProviderPremisesVM CandidateProviderPremises { get; set; }

        public virtual ICollection<CandidateProviderPremisesSpecialityVM> CandidateProviderPremisesSpecialities { get; set; }

        public virtual ICollection<CandidateProviderPremisesCheckingVM> CandidateProviderPremisesCheckings { get; set; }

        public bool haveCandidateProviderPremisesCheckings { get; set; }
    }
}
