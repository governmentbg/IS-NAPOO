using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ClientFilterVM
    {
        public int IdCandidate_Provider { get; set; }
        public int idLocation { get; set; }
        public int idDistrict { get; set; }
        public int idMunicipality { get; set; }
        public int? IdNationality { get; set; }
        public int? IdIndentType { get; set; }
        public int? IdTypeLicense { get; set; }
        public int? IdSex { get; set; }
        public string? LicenceNumber { get; set; }
        public string? FirstName { get; set; }
        public string? FamilyName { get; set; }
        public string? Indent { get; set; }

        public bool IsEmpty()
        {
            return IdCandidate_Provider == 0 && idLocation == 0 && idDistrict == 0 && idMunicipality == 0
                && !IdNationality.HasValue && !IdSex.HasValue && !IdIndentType.HasValue && !IdTypeLicense.HasValue && !string.IsNullOrEmpty(LicenceNumber)
                && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(FamilyName)
                && !string.IsNullOrEmpty(Indent);
        }
    }
}
