using System.Collections.Generic;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class RIDPKDocumentVM
    {
        public RIDPKDocumentVM()
        {
            this.GradesFromOldIS = new List<string>();
        }

        public int IdEntity { get; set; }

        public int IdClientCourse { get; set; }

        public string DocumentRegNo { get; set; }

        public string DocumentDate { get; set; }

        public string DocumentSerialNumber { get; set; }

        public string DocumentTypeName { get; set; }

        public string ClientFirstName { get; set; }

        public string ClientSecondName { get; set; }

        public string ClientFamilyName { get; set; }

        public string BirthDateAsStr { get; set; }

        public string YearsOldAsStr { get; set; }

        public string DocumentStatusValue { get; set; }

        public List<KeyValuePair<int, List<string>>> ClientDocuments { get; set; }

        public List<KeyValuePair<int, string>> CourseProtocolsWithGrades { get; set; }

        public List<string> GradesFromOldIS { get; set; }
    }
}
