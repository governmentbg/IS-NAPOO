using System;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class CoursesFilterVM
    {
        public int IdCandidateProvider { get; set; }

        public string LicenceNumber { get; set; }

        public int IdDistrict { get; set; }

        public int IdMunicipality { get; set; }

        public int IdLocation { get; set; }

        public string CourseName { get; set; }

        public int? IdCourseType { get; set; }

        public int? IdStatus { get; set; }
        public int? IdIndentType { get; set; }

        public int? IdAssignType { get; set; }

        public int IdProfession { get; set; }

        public int IdSpeciality { get; set; }

        public bool IsArchived { get; set; }

        public DateTime? StartFrom { get; set; }

        public DateTime? StartTo { get; set; }

        public DateTime? EndFrom { get; set; }

        public DateTime? EndTo { get; set; }
    }
}
