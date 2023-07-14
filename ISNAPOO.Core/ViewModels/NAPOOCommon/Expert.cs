using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.NAPOOCommon
{
    public class Expert
    {
        public string ProfessionalDirection { get; set; }

        public string Name { get; set; }

        public string EGN { get; set; }

        public string IdCardNumber { get; set; }

        public DateTime IdCardIssueDate { get; set; }
        public string IdCardIssueDateFormatted { get { return IdCardIssueDate.ToString(GlobalConstants.DATE_FORMAT); } }

        public string IdCardCityOfIssue { get; set; }

        public string AddressByIdCard { get; set; }

        public string TaxOffice { get; set; }
    }
}
