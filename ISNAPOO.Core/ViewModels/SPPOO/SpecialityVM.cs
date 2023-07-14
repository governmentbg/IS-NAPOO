using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ValidationAttributes;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.SPPOO
{
    public class SpecialityVM : IMapFrom<Speciality>
    {
        public SpecialityVM()
        {
            this.IdsNkpd = new List<int>();
            this.OrderVMs = new List<OrderVM>();
            this.SpecialityNKPDs = new HashSet<SpecialityNKPDVM>();
            this.SpecialityOrders = new HashSet<SpecialityOrderVM>();
            this.CandidateProviderTrainerSpecialities = new HashSet<CandidateProviderTrainerSpecialityVM>();
            this.CandidateProviderPremisesSpecialities = new HashSet<CandidateProviderPremisesSpecialityVM>();
            this.Doc = new DocVM();
            this.Profession = new ProfessionVM();
            //this.DocSpecialities = new HashSet<DocSpeciality>();
        }

        [Key]
        public int IdSpeciality { get; set; }

        public string CodeAndArea { get; set; }

        public string CodeAndAreaForAutoCompleteSearch => $"{this.Code} {this.Name}";

        public string CodeAndProfessionalDirection { get; set; }

        public string CodeAndProfession { get; set; }

        public int IdArea { get; set; }

        public int IdProfessionalDirection { get; set; }

        public int IdProfession { get; set; }

        public ProfessionVM Profession { get; set; }

        public string SpecialityComboFilter { get; set; }

        [Required(ErrorMessage = "Полето 'Код' е задължително!")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Код на Специалност' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Код на Специалност")]
        [ValidSPPOOCode]
        public string Code { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на специалност' е задължително!")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Наименование на Специалност' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Наименование на Специалност")]
        public string Name { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Наименование на Специалност - англйски' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Наименование на Специалност - англйски")]
        [Required(ErrorMessage = "Полето 'Наименование на специалност на английски език' е задължително!")]
        public string NameEN { get; set; }


        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Степен на професионална квалификация' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Степен на професионална квалификация")]
        ///Степен на професионална квалификация
        public string VQS_Name { get; set; }

        public long OldId { get; set; }
        public string ComboBoxName
        {
            get
            {
                if (string.IsNullOrEmpty(this.Code))
                {
                    return string.Empty;
                }
                else
                {
                    return this.Code + " " + this.Name;
                }
            }
        }

        public int IdStatus { get; set; }

        [Display(Name = "Степен на професионална квалификация")]
        [Required]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Степен на професионална квалификация' е задължително!")]
        public int IdVQS { get; set; }//code_speciality_vqs: I СПК, II СПК....V СПК
        public KeyValueVM VQS { get; set; }
        [Display(Name = "Нива по Националната квалификационна рамка (НКР)")]
        [Required]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Ниво по Националната квалификационна рамка' е задължително!")]
        public int IdNKRLevel { get; set; }//1,2,3,4,5

        [Display(Name = "Нива по Европейската квалификационна рамка (ЕКР)")]
        [Required]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Ниво по Европейската квалификационна рамка' е задължително!")]
        public int IdEKRLevel { get; set; }//1,2,3,4,5

        public int? IdDOC { get; set; }

        [Display(Name = "Включване в „Списък със защитените от държавата специалности от професии“")]
        public bool IsStateProtectedSpecialties { get; set; }//(Да/Не)

        [Display(Name = "Включване в „Списък със специалности от професии, по които е налице очакван недостиг от специалисти на пазара на труда“")]
        public bool IsShortageSpecialistsLaborMarket { get; set; }//(Да/Не)

        [Display(Name = "Допустимост  за обучение на ученици")]
        public bool IsTrainingStudents { get; set; }//(Да/Не)

        [Display(Name = "Допустимост  за обучение на възрастни")]
        public bool IsAdultEducation { get; set; }//(Да/Не)

        [Display(Name = "Допустимост  за обучение по част от професия")]
        public bool IsTrainingPartProfession { get; set; }//(Да/Не)

        [Display(Name = "Допустимост  за дистанционно обучение")]
        public bool IsDistanceLearning { get; set; }//(Да/Не)

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Линк към Националните изпитни програми")]
        public string LinkNIP { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Линк към Учебни планове и учебни програми на МОН' не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Линк към Учебни планове и учебни програми на МОН")]
        public string LinkMON { get; set; }

        public List<int> IdsNkpd { get; set; }

        public List<OrderVM> OrderVMs { get; set; }

        public int IdTypeChange { get; set; }

        public int? IdFrameworkProgram { get; set; }

        public int? IdFormEducation { get; set; }

        public virtual DocVM Doc { get; set; }

        public string CodeAndName { get { return $"{this.Code} {this.Name}"; } set { } }
        public string CodeAndNameAndVQS { get; set; }

        public string CodeAndNameArea { get; set; }

        public string CodeAndNameProfessionalDirection { get; set; }

        public string CodeAndNameProfession { get; set; }

        public string DOCTrainerRequirements { get; set; }

        public string DOCMTBRequirements { get; set; }

        [Comment("Вид на провежданото обучение")]
        [Required]
        public int IdUsage { get; set; }

        [Comment("Очакване за DOC")]
        [Required]
        public int IdComplianceDOC { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }

        public int CodeAsIntForOrderBy => int.Parse(this.Code);

        public string SpecialityLicenceDateAsStr { get; set; }

        public string SpecialityCodeAndNameAndProfessionCodeAndName
        {
            get
            {
                if (this.Profession is not null)
                    return $"{CodeAndName} | {Profession.CodeAndName}";
                else
                    return CodeAndName;
            }
        }

        public CandidateProviderSpecialityVM CandidateProviderSpeciality { get; set; }

        public string? CurriculumModificationUploadedFileName { get; set; }

        public virtual ICollection<SpecialityNKPDVM> SpecialityNKPDs { get; set; }

        public virtual ICollection<SpecialityOrderVM> SpecialityOrders { get; set; }

        public virtual ICollection<CandidateProviderTrainerSpecialityVM> CandidateProviderTrainerSpecialities { get; set; }

        public virtual ICollection<CandidateProviderPremisesSpecialityVM> CandidateProviderPremisesSpecialities { get; set; }
        //  public virtual ICollection<DocSpeciality> DocSpecialities { get; set; }


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
    }
}
