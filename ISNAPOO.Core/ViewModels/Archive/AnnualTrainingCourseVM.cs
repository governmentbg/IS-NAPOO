using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Archive
{
    public class AnnualTrainingCourseVM
    {

        public long IdAnnualTrainingCourse { get; set; }
        public string ProfessionCode { get; set; }
        public string SpecialityCode { get; set; }
        public string VQSName { get; set; }
        public string DistrictName { get; set; }
        public long CountIncludedMan { get; set; }
        public long CountIncludedWomen { get; set; }
        public long CountCertificateMan { get; set; }
        public long CountCertificateWomen { get; set; }
        public long Horarium { get; set; }
        public decimal Cost { get; set; }
        public long CountTestimony { get; set; }
        public string SourceFunding { get; set; }
    }
}
