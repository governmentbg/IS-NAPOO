using Data.Models.Data.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.SPPOO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderSearchVM
    {
        public CandidateProviderSearchVM()
        {
            this.Specialities = new List<SpecialityVM>();
            this.Professions = new List<ProfessionVM>();
        }
        public int? id { get; set; }

        public string Owner { get; set; }//Юридическо лице 
        public string LicenseNumber { get; set; }//№ на лицензия
        public string Name { get; set; }//Име
        public string MiddleName { get; set; }//Презиме
        public string FamilyName { get; set; }//Фамилия
        public string Indent { get; set; }//ЕГН/ИДН/ЛНЧ
        public int IdEducation { get; set; }//Образователно-квалификационна степен
        public string EducationSpecialityNotes { get; set; }//Специалност по диплома
        public string EducationCertificateNotes { get; set; }//Свидетелство за правоспособност
        public string EducationAcademicNotes { get; set; }//Специална научна подготовка
        public int? IdContractType { get; set; }//Вид на договора
        public int IdStatus { get; set; }//Статус
        public int IdProfessionalDirection { get; set; }//Професионално направление
        public int IdProfession { get; set; }//Професия
        public int? SpecialityId { get; set; }//Специалност
        public int? kvPracticeOrTheory { get; set; }//Вид на провежданото обучение
        public int? IdConsulting { get; set; }//Вид на услуги
        public KeyValueVM kvVMPracticeOrTheory { get; set; }
        public int? IdComplianceDOC { get; set; }//Сътветствие с ДОС
        public DateTime? CreateionDateFrom { get; set; }//Дата на създаване От
        public DateTime? CreateionDateTo { get; set; }//Дата на създаване До
        public DateTime? ModifyDateFrom { get; set; }//Дата на последна актуализация От
        public DateTime? ModifyDateTo { get; set; }//Дата на последна актуализация До
        public bool IsNAPOOCheck { get; set; }//Извършена проверка от експерт на НАПОО
        public DateTime? NAPOOCheckDateFrom { get; set; }//Дата на проверка От
        public DateTime? NAPOOCheckDateTo { get; set; }//Дата на проверка До
        public List<SpecialityVM> Specialities { get; set; }//Лист от специалности
        public List<ProfessionVM> Professions { get; set; }//Лист от професии
        public LocationVM Location { get; set; }
        public MunicipalityVM MunicipalityVM { get; set; }
        public DistrictVM DistrictVM { get; set; }
        public int? VQS { get; set; }
    }
}
