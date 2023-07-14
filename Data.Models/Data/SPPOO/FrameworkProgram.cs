using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.SPPOO
{

    /// <summary>
    /// Рамкова програма
    /// </summary>
    [Table("SPPOO_FrameworkProgram")]
    [Display(Name = "Рамкова програма")]
    public class FrameworkProgram : IEntity, IModifiable, IDataMigration
    {
        [Key]
        public int IdFrameworkProgram { get; set; }
        public int IdEntity => IdFrameworkProgram;

        [Required]
        [StringLength(DBStringLength.StringLength20)]
        [Display(Name = "Наименование на рамковата програма")]
        public string Name { get; set; } // Старо 'vc_course_fr_curr_name' 

        [Display(Name = "Степен на професионална квалификация")]
        public int IdVQS { get; set; }//Старо 'int_vqs ' code_speciality_vqs: I СПК, II СПК....V СПК

        [Display(Name = "Вид рамкова програма")]
        public int IdTypeFrameworkProgram { get; set; }//Професионална квалификация (СПК), Част от професия

        [Display(Name = "Квалификационно равнище")]
        public int IdQualificationLevel { get; set; }//Без входящо квалификационно равнище, С входящо квалификационно равнище,  Без входящо квалификационно равнище(начално обучение), С входящо квалификационно равнище(актуализация и разширяване)

        //[Display(Name = "Форма на обучение")]
        //public int IdFormEducation { get; set; }//Дневна, Вечерна, Дистанционна, Индивидуална, самостоятелна, Задочна, Дуална

        // Форма за обучение : //Дневна, Вечерна, Дистанционна, Индивидуална, самостоятелна, Задочна, Дуална
        public virtual ICollection<FrameworkProgramFormEducation> FrameworkProgramFormEducations { get; set; } // Старо 'vc_ed_forms '

        [Display(Name = "Минимално образователно равнище")]
        public int IdMinimumLevelEducation { get; set; }// Старо 'vc_desc_in_edu ' начален етап на основното образование, успешно завършен курс за ограмотяване, Първи гимназиален етап, X клас

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Минимално квалификационно равнище")]
        public string MinimumLevelQualification { get; set; }// квалификация по част от същата професия, професия от същата област

        [Display(Name = "Срок на обучение")]
        public int IdTrainingPeriod { get; set; }//Старо 'int_duration_months ' до 6 месеца, до 12 месеца, до 18 месеца

        [Display(Name = "Минимален брой задължителни учебни часове")]
        public double SectionА { get; set; }//Старо 'int_mandatory_hours ' Минимален брой задължителни учебни часове

        [Display(Name = "Максимален % часове обща професионална подготовка спрямо общия брой задължителни часове")]
        public double SectionА1 { get; set; }//Старо 'int_min_perc_common ' Максимален % часове обща професионална подготовка спрямо общия брой задължителни часове

        [Display(Name = "Минимален брой избираеми учебни часове")]
        public double SectionB { get; set; }//Старо 'int_selectable_hours ' Минимален брой избираеми учебни часове

        [Display(Name = "Tеория")]
        public double Theory { get; set; }//Tеория

        [Display(Name = "Минимален % на учебните часове за практическо обучение спрямо общия брой часове за отраслова и специфична професионална подготовка")]
        public double Practice { get; set; }// Старо 'int_min_perc_practice ' Минимален % на учебните часове за практическо обучение спрямо общия брой часове за отраслова и специфична професионална подготовка

        [Display(Name = "Срок на обучение")]
        public int? IdFrameworkProgramConnection { get; set; }//Връзка с РП Професионална квалификация (СПК) 


        [StringLength(DBStringLength.StringLength100)]
        [Comment("Кратко описание")]
        public string ShortDescription { get; set; }// Старо 'vc_short_desc '

        [StringLength(DBStringLength.StringLength512)]
        [Comment( "Описание")]
        public string Description { get; set; }// Старо 'vc_description'


        [Comment("Валидно ли е")]
        public bool IsValid { get; set; }// Старо 'bool_valid '

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Стойност по подразбиране")]
        public string? DefaultValue1 { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Завършване и удостоверяване на професионалното обучение")]
        public string CompletionVocationalTraining { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Пояснителни бележки")]
        public string ExplanatoryNotes { get; set; }

        [Comment("Статус на рамкова програма")]
        public int? IdStatus { get; set; } // Номенклатура KeyTypeIntCode - "StatusTemplate"

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
    }
}  