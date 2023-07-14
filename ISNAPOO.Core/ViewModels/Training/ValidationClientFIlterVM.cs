using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ValidationClientFIlterVM
    {
        public int IdCandidateProvider { get; set; }

        public string? LicenceNumber { get; set; }

        public string? FirstName { get; set; }

        public string? FamilyName { get; set; }

        public string? Indent { get; set; }

        public int? IdNationality { get; set; }

        public int? IdSex { get; set; }
        public int idDistrict { get; set; }
        public int idMunicipality { get; set; }
        public int idLocation { get; set; }

        //public int? IdMeasureType { get; set; }

        //public string? CourseName { get; set; }

        public int IdProfession { get; set; }

        public int IdSpeciality { get; set; }

        public DateTime? CourseStartFrom { get; set; }

        public DateTime? CourseStartTo { get; set; }

        public DateTime? CourseEndFrom { get; set; }

        public DateTime? CourseEndTo { get; set; }

        public string? DocumentRegNo { get; set; }

        public int? IdTypeOfRequestedDocument { get; set; }

        public DateTime? DocumentDateFrom { get; set; }

        public DateTime? DocumentDateTo { get; set; }

        public bool IsEmpty()
        {
            return IdCandidateProvider == 0 && idDistrict == 0 && idMunicipality == 0 && idLocation == 0
                && IdProfession == 0 && IdSpeciality == 0 
                 && !IdNationality.HasValue && !IdSex.HasValue
                && !CourseStartFrom.HasValue && !CourseStartTo.HasValue && !CourseEndFrom.HasValue && !CourseEndTo.HasValue
                && !IdTypeOfRequestedDocument.HasValue && !DocumentDateFrom.HasValue && !DocumentDateTo.HasValue
                && string.IsNullOrEmpty(DocumentRegNo) && string.IsNullOrEmpty(Indent)
                && string.IsNullOrEmpty(FamilyName) && string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LicenceNumber);
            //&& !IdCourseLocation.HasValue && !IdCourseMunicipality.HasValue && !IdCourseDistrict.HasValue  && !IdMeasureType.HasValue && string.IsNullOrEmpty(CourseName);
        }
    }
}
