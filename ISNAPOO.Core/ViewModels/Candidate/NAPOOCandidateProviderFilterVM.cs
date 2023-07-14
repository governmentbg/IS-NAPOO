using ISNAPOO.Core.ViewModels.SPPOO;
using System;
using System.Collections.Generic;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class NAPOOCandidateProviderFilterVM
    {
        public NAPOOCandidateProviderFilterVM()
        {
            this.LicensedSpecialities = new List<SpecialityVM>();
        }

        public string ProviderName { get; set; }
        public int? IdCandidateProvider { get;set; }
        public string Bulstat { get; set; }

        public int IdLocationCorrespondence { get; set; }

        public int IdLocationAdmin { get; set; }
        public int? IdProfession { get; set; }

        public string Profession { get; set; }

        public string LicenceNumber { get; set; }

        public DateTime? LicenceDateFrom { get; set; }

        public DateTime? LicenceDateTo { get; set; }

        public List<SpecialityVM> LicensedSpecialities { get; set; }
    }
}
