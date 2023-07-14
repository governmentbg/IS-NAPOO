using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.SPPOO
{

    [Display(Name = "Рамкова програма")]
    public class FrameworkProgramVM : IMapFrom<FrameworkProgram>
    {
        public FrameworkProgramVM()
        {
            FrameworkProgramFormEducations = new List<FrameworkProgramFormEducationVM>();
        }

        [Key]
        public int IdFrameworkProgram { get; set; }

        [Required(ErrorMessage = "Полето \'Наименование на рамковата програма\' е задължително!")]
        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Дължината на името на рамковата програма е твърде дълго. Максималната дължина е 10 символа.")]
        [Display(Name = "Наименование на рамковата програма")]
        public string Name { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето \'Степен на професионална квалификация\' е задължително!")]
        [Display(Name = "Степен на професионална квалификация")]
        public int IdVQS { get; set; }//code_speciality_vqs: I СПК, II СПК....V СПК
        public string VQSName { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето \'Вид рамкова програма\' е задължително!")]
        [Display(Name = "Вид рамкова програма")]
        public int IdTypeFrameworkProgram { get; set; }//Професионална квалификация (СПК), Част от професия
        public string TypeFrameworkProgramName { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето \'Квалификационно равнище\' е задължително!")]
        [Display(Name = "Квалификационно равнище")]
        public int IdQualificationLevel { get; set; }//Без входящо квалификационно равнище, С входящо квалификационно равнище,  Без входящо квалификационно равнище(начално обучение), С входящо квалификационно равнище(актуализация и разширяване)
        public string QualificationLevelName { get; set; }


        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето \'Минимално образователно равнище\' е задължително!")]
        [Display(Name = "Минимално образователно равнище")]
        public int IdMinimumLevelEducation { get; set; }// начален етап на основното образование, успешно завършен курс за ограмотяване, Първи гимназиален етап, X клас

        public string MinimumLevelEducationName { get; set; }

        [Display(Name = "Минимално квалификационно равнище")]
        public string MinimumLevelQualification { get; set; }// квалификация по част от същата професия, професия от същата област

        public string MinimumQualificationLevelName { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето \'Срок на обучение\' е задължително!")]
        [Display(Name = "Срок на обучение")]
        public int IdTrainingPeriod { get; set; }//до 6 месеца, до 12 месеца, до 18 месеца

        public string TrainingPeriodName { get; set; }

        public string FrameworkProgramNameFormatted { get; set; }

        [Display(Name = "Раздел А")]
        [Range(0, int.MaxValue, ErrorMessage = "Полето \'Мин. брой задължителни учебни часове\' трябва да е с положителна стойност!")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Полето 'Мин. брой задължителни учебни часове' трябва да е с целочислена стойност!")]
        public double SectionА { get; set; }//Професионална подготовка 

        [Display(Name = "Раздел А")]
        public bool HasSectionА { get; set; }//Професионална подготовка 

        [Display(Name = "Раздел А1")]
        [Range(0, int.MaxValue, ErrorMessage = "Полето \'Макс. % часове А1\' е с невалидна процентна стойност!")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Полето 'Макс. % часове А1' трябва да е с целочислена стойност!")]
        public double SectionА1 { get; set; }//Раздел А1 Обща професионална подготовка

        public string SectionА1WithPercent { get { return this.SectionА1 + "%"; } }

        [Display(Name = "Раздел Б")]
        [Range(0, int.MaxValue, ErrorMessage = "Полето \'Мин. брой избираеми учебни часове\' трябва да е с положителна стойност!")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Полето 'Мин. брой избираеми учебни часове' трябва да е с целочислена стойност!")]
        public double SectionB { get; set; }//Избираеми учебни часове

        [Display(Name = "Tеория")]
        public double Theory { get; set; }//Tеория

        [Display(Name = "Практика")]
        [Range(0, 100, ErrorMessage = "Полето \'Мин. % часове за практическо обучение\' е с невалидна процентна стойност!")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Полето 'Мин. % часове за практическо обучение' трябва да е с целочислена стойност!")]
        public double Practice { get; set; }//Практика

        public string PracticeWithPercent { get { return this.Practice + "%"; } }

        [Display(Name = "Срок на обучение")]
        public int? IdFrameworkProgramConnection { get; set; }//Връзка с РП Професионална квалификация (СПК)

        [RequiredValue(ErrorMessage = "Полето \'Форма на обучение\' е задължително!")]
        [Display(Name = "Форма на обучение")]
        public int[] FormEducationIds { get; set; }//Дневна, Вечерна, Дистанционна, Индивидуална, самостоятелна, Задочна, Дуална
        public string FormEducationNames { get; set; }
        public List<FrameworkProgramFormEducationVM> FrameworkProgramFormEducations { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Кратко описание")]
        public string ShortDescription { get; set; }// Старо 'vc_short_desc '

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Описание")]
        public string Description { get; set; }// Старо 'vc_description'


        [Comment("Валидно ли е")]
        public bool IsValid { get; set; }// Старо 'bool_valid '
        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Стойност по подразбиране")]
        public string? DefaultValue1 { get; set; }

        [Required(ErrorMessage = "Полето 'Завършване и удостоверяване на професионалното обучение' е задължително!")]
        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'Завършване и удостоверяване на професионалното обучение' може да съдържа до 1000 символа!")]
        [Comment("Завършване и удостоверяване на професионалното обучение")]
        public string CompletionVocationalTraining { get; set; }

        [Required(ErrorMessage = "Полето 'Пояснителни бележки' е задължително!")]
        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'Пояснителни бележки' може да съдържа до 1000 символа!")]
        [Comment("Пояснителни бележки")]
        public string ExplanatoryNotes { get; set; }

        [Required(ErrorMessage = "Полето 'Статус' е задължително!")]
        public int? IdStatus { get; set; }  // Номенклатура KeyTypeIntCode - "StatusTemplate"

        public string StatusValue { get; set; }

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

        #region IDataMigration
        public int? OldId { get; set; } // Старо 'id'

        public string? MigrationNote { get; set; }
        #endregion

        class RequiredValueAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)// Return a boolean value: true == IsValid, false != IsValid
            {
                int[] arr = value as int[];

                if (arr is null || arr.Length == 0)
                {
                    return false;
                }

                return true;
            }
        }
    }
}
