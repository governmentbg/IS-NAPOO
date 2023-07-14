using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.DOC
{

    /// <summary>
    /// СПИСЪК НА ДЛЪЖНОСТИТЕ - В НАЦИОНАЛНАТА КЛАСИФИКАЦИЯ НА ПРОФЕСИИТЕ И ДЛЪЖНОСТИТЕ, 2011 г.    
    /// </summary>
    [Table("DOC_NKPD")]
    [Display(Name = "NKPD")]
    public class NKPD : IEntity, IModifiable
    {

        [Key]
        public int IdNKPD { get; set; }
        public int IdEntity => IdNKPD;

        [Required]
        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength10)]
        [Display(Name = "Код")]
        public string Code { get; set; }


        [Required]        
        [Display(Name = "Код на клас")]
        public int IdClassCode { get; set; }

        [Required]
        [Display(Name = "Код на подклас")]
        public int IdSubclassCode { get; set; }

        [Required]
        [Display(Name = "Код на група")]
        public int IdGroupCode { get; set; }


        [Required]
        [Display(Name = "Код на единична група")]
        public int IdIndividualGroupCode { get; set; }


        [Required]
        [StringLength(DBStringLength.StringLength10)]
        [Display(Name = "Код на образователно и квалификационно ниво")]
        public string EducationLevelCode { get; set; }

        

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
