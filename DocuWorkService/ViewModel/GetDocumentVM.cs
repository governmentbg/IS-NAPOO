using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocuWorkService.ViewModel
{
    public class GetDocumentVM
    {
        public int? DocID { get; set; }
        public string? GUID { get; set; }
        public int? OfficialDocID { get; set; }
        public string? OfficialGUID { get; set; }
        public string? DocNumber { get; set; }
        public string? OfficialDocNumber { get; set; }
        public int DeloSerial { get; set; }
        public DateTime? DocDate { get; set; }
        public DateTime? OfficialDocDate { get; set; }
        public int VidCode { get; set; }
        //Създадено за CandidateProviderLicenceChange документите тъй като могат да бъдат (P1_pril19, P2_pril19 или P3_pril19a)
        public int[] VidCodes { get; set; }
    }
}
