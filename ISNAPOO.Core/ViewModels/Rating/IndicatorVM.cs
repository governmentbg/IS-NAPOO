using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Framework;
using ISNAPOO.Core.Mapping;
using Data.Models.Data.Rating;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ValidationAttributes;

namespace ISNAPOO.Core.ViewModels.Rating
{
    public class IndicatorVM :  IModifiable, IMapFrom<Indicator>, IMapTo<Indicator>
    {
        public IndicatorVM()
        {
            this.IndicatorDetails = new KeyValueVM();
        }

        [Key]
        public int IdIndicator { get; set; }


        [Comment("Година")]
        [Required(ErrorMessage = "Полето година е задължително")]
        public int Year { get; set; }

        [Comment("Показател вид")]
        public int IdIndicatorType { get; set; }//Брой обучени лица;Квалификация на преподавателите за последната година, o	Наличие на собствена материално-техническа база;o	Отразени /неотразени препоръки от последващ контрол;
        [Comment("Номенлатура на индикатора")]
        public KeyValueVM IndicatorDetails { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Comment("Диапазаон от")]
        [CustomIndicatorValidation(ErrorMessage = "Полето \"Диапазоът от\" не може да бъде по-малко или равно на 0")]
        public decimal RangeFrom { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Comment("Диапазаон до")]
        [CustomIndicatorValidation(ErrorMessage = "Полето \"Диапазоът до\" не може да бъде по-малко или равно на 0")]
        public decimal RangeTo { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        [Comment("Точки")]
        [CustomIndicatorValidation(ErrorMessage = "Полето \"Точки\" не може да бъде по-малко или равно на 0")]
        public decimal Points { get; set; }


        [Column(TypeName = "decimal(5, 2)")]
        [Comment("Точки Да")]
        [CustomIndicatorValidation(ErrorMessage = "Полето \"Точки Да\" не може да бъде по-малко или равно на 0")]
        public decimal PointsYes { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        [Comment("Точки НЕ")]
        [CustomIndicatorValidation(ErrorMessage = "Полето \"Точки Нe\" не може да бъде по-малко или равно на 0")]
        public decimal PointsNo { get; set; }

        [Comment("Тежест на индикатор")]
        public decimal Weight { get; set; }
        public string YearAsString => this.Year.ToString();
        public string IndicatorFullName => this.IndicatorDetails.Name + $" От: {RangeFrom.ToString()} до:{RangeTo.ToString()}";



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
