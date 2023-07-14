using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class ApplicationListFilterVM
    {
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Име на ЦПО,ЦИПО' не може да съдържа повече от 512 символа.")]
        public string? ProviderName { get; set; }
        [StringLength(18, ErrorMessage = "Полето 'УИН' не може да съдържа повече от 18 символа!")]
        public string UIN { get; set; }

        public string ProviderOwner { get; set; }

        public int? IdLocation { get; set; }

        public string ProviderAddressCorrespondence { get; set; }

        public string PersonNameCorrespondence { get; set; }

        public string ProviderPhoneCorrespondence { get; set; }

        public string ProviderEmailCorrespondence { get; set; }

        public int? IdTypeApplication { get; set; }

        public int? IdApplicationStatus { get; set; }

        public string LicenceNumber { get; set; }

        public DateTime? LicenceDate { get; set; }

        public int? IdNAPOOExpert { get; set; }
    }
}
