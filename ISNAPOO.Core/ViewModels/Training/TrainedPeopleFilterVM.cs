using System;
using System.Collections.Generic;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class TrainedPeopleFilterVM
    {
        public int? IdCandidateProvider { get; set; }

        public string? LicenceNumber { get; set; }

        public int? IdCourseLocation { get; set; }

        public int? IdCourseDistrict { get; set; }

        public int? IdCourseMunicipality { get; set; }

        public string? FirstName { get; set; }

        public string? FamilyName { get; set; }

        public string? Indent { get; set; }

        public int? IdNationality { get; set; }

        public int? IdSex { get; set; }

        public int? IdMeasureType { get; set; }

        public string? CourseName { get; set; }

        public int? IdProfession { get; set; }

        public int? IdSpeciality { get; set; }

        public DateTime? CourseStartFrom { get; set; }

        public DateTime? CourseStartTo { get; set; }

        public DateTime? CourseEndFrom { get; set; }

        public DateTime? CourseEndTo { get; set; }

        public string? DocumentRegNo { get; set; }

        public int? IdTypeOfRequestedDocument { get; set; }

        public DateTime? DocumentDateFrom { get; set; }

        public DateTime? DocumentDateTo { get; set; }

        public List<int?> TypeOfRequestDocuments { get; set; }
    }
}
