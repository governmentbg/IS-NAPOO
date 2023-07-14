using System;
using System.Collections.Generic;
using System.Linq;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.SPPOO;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.DOC;
using ISNAPOO.Core.ViewModels.Candidate;

namespace ISNAPOO.Core.ViewModels.DOC
{
    public class ERUVM 
    {
        public ERUVM()
        {
            this.ERUSpecialities = new List<ERUSpecialityVM>();
            this.Specialities = new HashSet<SpecialityVM>();
            this.CandidateCurriculumERUs = new HashSet<CandidateCurriculumERUVM>();
            this.RUText = string.Empty;
        }

        [Key]
        public int IdERU { get; set; }

        [Required(ErrorMessage = "Полето 'Шифър на ЕРУ' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Шифър на ЕРУ' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Шифър на ЕРУ")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на ЕРУ' е задължително!")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Наименование на ЕРУ' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Наименование на ЕРУ")]
        public string Name { get; set; }

        [Required]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид професионална подготовка' е задължително!")]
        [Display(Name = "Вид професионална подготовка")]
        public int IdProfessionalTraining { get; set; }

        public string ProfessionalTrainingName { get; set; }

        [Required]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Нива по НКР' е задължително!")]
        [Display(Name = "Нива по Националната квалификационна рамка (НКР)")]
        public int IdNKRLevel { get; set; }//1,2,3,4,5

        public string NKRLevelName { get; set; }

        [Required]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Нива по ЕКР' е задължително!")]
        [Display(Name = "Нива по Европейската квалификационна рамка (ЕКР)")]
        public int IdEKRLevel { get; set; }//1,2,3,4,5

        public string EKRLevelName { get; set; }


        [Required(ErrorMessage = "Полето 'РУ, Знания, умения, компетентности, Средства за оценяване, Условия за провеждане на оценяването, Критерии за оценяване' е задължително!")]
        [Display(Name = "РУ, Знания, умения, компетентности, Средства за оценяване, Условия за провеждане на, оценяването, Критерии за оценяване")]
        public string RUText { get; set; }

        public int IdDOC { get; set; }
        public virtual DocVM DOC { get; set; }

        // парсва Шифъра на ЕРУ към число, за да се ползва за сортиране
        public int ERUIntCodeSplit
        {
            get
            {
                var arr = this.Code.Split(" ");
                int intCode = 0;
                if (arr.Count() > 1)
                {
                     int.TryParse(arr[1], out intCode);
                }

                return intCode;
            }
        }

        public string CodeWithName => $"{this.Code} {this.Name}";

        public string NameOfDOC { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int IdModifyUser { get; set; }

        [Required]
        public DateTime ModifyDate { get; set; }
        #endregion


        public virtual List<ERUSpecialityVM> ERUSpecialities { get; set; }

        public virtual ICollection<SpecialityVM> Specialities { get; set; }

        public virtual ICollection<CandidateCurriculumERUVM> CandidateCurriculumERUs { get; set; }
    }
}
