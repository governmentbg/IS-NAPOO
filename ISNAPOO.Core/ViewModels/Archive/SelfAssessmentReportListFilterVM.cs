using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Archive
{
    public class SelfAssessmentReportListFilterVM
    {
        public int? Year { get; set; }
        public DateTime? FillingDateFrom { get; set; }
        public DateTime? FillingDateTo { get; set; }
    }
}
