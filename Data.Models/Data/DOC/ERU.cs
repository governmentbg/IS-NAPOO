using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.DOC
{
     

    /// <summary>
    /// Област
    /// </summary>
    [Table("DOC_ERU")]
    [Display(Name = "ЕРУ")]
    public class ERU : IEntity, IModifiable
    {
        public ERU()
        {
            this.ERUSpecialities = new HashSet<ERUSpeciality>();
        }

        [Key]
        public int IdERU { get; set; }
        public int IdEntity => IdERU;

        [Required]
        [StringLength(DBStringLength.StringLength100)]
        [Display(Name = "Шифър на ЕРУ")]
        public string Code { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Наименование на ЕРУ")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Вид професионална подготовка")]//А1 Обща професионална подготовка, А2 Отраслова професионална подготовка,  А3 Специфична професионална подготовка
        public int IdProfessionalTraining { get; set; }

        [Required]
        [Display(Name = "Нива по Националната квалификационна рамка (НКР)")]
        public int IdNKRLevel { get; set; }//1,2,3,4,5

        [Required]
        [Display(Name = "Нива по Европейската квалификационна рамка (ЕКР)")]
        public int IdEKRLevel { get; set; }//1,2,3,4,5


        [Column(TypeName = "ntext")]
        [Display(Name = "РУ, Знания, умения, компетентности, Средства за оценяване, Условия за провеждане на, оценяването, Критерии за оценяване")]
        public string RUText { get; set; }

        /// <summary>
        /// Връзка с ДОС
        /// </summary>
        [Required]
        [ForeignKey(nameof(DOC))]
        public int IdDOC { get; set; }
        public Models.Data.DOC.DOC DOC { 
            get; 
            set; 
        }

        public virtual ICollection<ERUSpeciality> ERUSpecialities { get; set; }

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
