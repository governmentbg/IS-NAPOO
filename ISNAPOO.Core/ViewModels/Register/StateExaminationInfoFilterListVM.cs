using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Register
{
    public class StateExaminationInfoFilterListVM
    {
        public int? IdCandidateProvider { get; set; }

        public string? CourseName { get; set; }

        public string? LicenceNumber { get; set; }

        public string TrainingTypeIntCode { get; set; }

        public DateTime? ExamDateFrom { get; set; }

        public DateTime? ExamDateTo { get; set; }

    }
}
