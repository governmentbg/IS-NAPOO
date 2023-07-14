using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Archive
{
    public class SelfAssessmentSummaryProfessionalTrainingVM
    {

        public int ProfessionalTrainingIndicatorId { get; set; }
        public string ProfessionalTrainingIndicatorName { get; set; }
        public string ProfessionalTrainingIndicatorGroup{ get; set; }
        public int ProfessionalTrainingIndicatorCount { get; set; }

        public bool ShowTotal { get; set; }
    }
}
