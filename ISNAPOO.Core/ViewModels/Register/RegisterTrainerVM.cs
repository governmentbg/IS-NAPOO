using System;

namespace ISNAPOO.Core.ViewModels.Register
{
    public class RegisterTrainerVM
    {
        public int? IdCandidateProvider { get; set; }

        public int IdEntity { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string FamilyName { get; set; }

        public string LicenseNumber { get; set; }

        public DateTime? LicenseDate { get; set; }

        public string Indent { get; set; }//ЕГН/ИДН/ЛНЧ

        public string OwnerAndProvider { get; set; }

        public bool IsCPO { get; set; }

        public int? IdEducation { get; set; }//Образователно-квалификационна степен

        public string EducationSpecialityNotes { get; set; }//Специалност по диплома

        public string EducationCertificateNotes { get; set; }//Свидетелство за правоспособност

        public int? IdProfessionalDirection { get; set; }

        public int? IdProfession { get; set; }

        public int? IdSpeciality { get; set; }

        public int? IdTrainingType { get; set; }

        public int? IdContractType { get; set; }

        public int? IdStatus { get; set; }

        public int? IdFilterDataType { get; set; }

        public DateTime? CreationDateFrom { get; set; }

        public DateTime? CreationDateTo { get; set; }

        public DateTime? ModifyDateFrom { get; set; }

        public DateTime? ModifyDateTo { get; set; }

        public DateTime? NAPOOCheckDateFrom { get; set; }

        public DateTime? NAPOOCheckDateTo { get; set; }

        public string ProfessionalDirections { get; set; }

        public string StatusValue { get; set; }
    }
}
