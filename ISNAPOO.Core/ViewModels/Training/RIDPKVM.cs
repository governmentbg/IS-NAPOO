using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.SPPOO;
using System;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class RIDPKVM
    {
        public int IdRIDPK { get; set; }

        public int IdClientCourseDocument { get; set; }

        public int IdValidationClientDocument { get; set; }

        public CandidateProviderVM CandidateProvider { get; set; }

        public CourseVM Course { get; set; }

        public ValidationClientVM ValidationClient { get; set; }

        public FrameworkProgramVM FrameworkProgram { get; set; }

        public SpecialityVM Speciality { get; set; }

        public int SubmittedDocumentCount { get; set; }

        public DateTime? SubmissionDate { get; set; }

        public string SubmitDateAsStr => this.SubmissionDate.HasValue ? $"{this.SubmissionDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;
    }
}
