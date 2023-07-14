using System.Collections.Generic;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateCurriculumExcelVM
    {
        public CandidateCurriculumExcelVM()
        {
            this.MissingTopicERUs = new List<string>();
            this.MissingDOCERUs = new List<string>();
        }

        public List<string> MissingTopicERUs { get; set; }

        public List<string> MissingDOCERUs { get; set; }

        public bool MinimumCompulsoryHoursNotReached { get; set; }

        public bool MinimumChoosableHoursNotReached { get; set; }

        public bool MaximumPercentNotReached { get; set; }

        public bool MinimumPercentNotReached { get; set; }

        public bool CurriculumNotAdded { get; set; }

        public bool FrameworkProgramNotAdded { get; set; }

        public double CompulsoryHours { get; set; }

        public double NonCompulsoryHours { get; set; }

        public double PercentCompulsoryHours { get; set; }

        public double PercentSpecificTraining { get; set; }
    }
}
