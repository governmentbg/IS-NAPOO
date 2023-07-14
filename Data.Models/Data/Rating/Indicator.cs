using Data.Models.Data.Common;
using Data.Models.Data.Control;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Rating
{
    /// <summary>
    /// Показател
    /// </summary>
    [Table("Rating_Indicator")]
    [Display(Name = "Показател")]
    public class Indicator : IEntity, IModifiable
    {
        public Indicator()
        {
             
        }

        [Key]
        public int IdIndicator { get; set; }
        public int IdEntity => IdIndicator;

        [Comment("Година")]
        public int Year { get; set; }

        [Comment("Показател вид")]
        public int IdIndicatorType { get; set; }//Брой обучени лица;Квалификация на преподавателите за последната година, o	Наличие на собствена материално-техническа база;o	Отразени /неотразени препоръки от последващ контрол;

        [Column(TypeName = "decimal(10, 2)")]
        [Comment("Диапазаон от")]
        public decimal? RangeFrom { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Comment("Диапазаон до")]
        public decimal? RangeTo { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        [Comment("Точки")]
        public decimal? Points { get; set; }


        [Column(TypeName = "decimal(5, 2)")]
        [Comment("Точки Да")]
        public decimal? PointsYes { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        [Comment("Точки НЕ")]
        public decimal? PointsNo { get; set; }






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

