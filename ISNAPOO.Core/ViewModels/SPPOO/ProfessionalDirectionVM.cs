using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ValidationAttributes;

namespace ISNAPOO.Core.ViewModels.SPPOO
{
    public class ProfessionalDirectionVM : IMapFrom<ProfessionalDirection>, IMapTo<ProfessionalDirection>
    {
        public ProfessionalDirectionVM()
        {
            this.Professions = new List<ProfessionVM>();
            this.ProfessionalDirectionOrders = new List<ProfessionalDirectionOrderVM>();
            this.OrderVMs = new List<OrderVM>();
        }

        [Key]
        public int IdProfessionalDirection { get; set; }

        public string CodeAndArea { get; set; }

        public int IdArea { get; set; }

        public virtual AreaVM Area { get; set; }

        [Required(ErrorMessage = "Полето 'Код' е задължително!")]
        [StringLength(100)]
        [Display(Name = "Код на професионално направление")]
        [ValidSPPOOCode]
        public string Code { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на професионалното направление' е задължително!")]
        [StringLength(255, ErrorMessage = "Полето 'Наименование на професионалното направление' може да бъде до 255 символа!")]
        [Display(Name = "Наименование на професионално направление")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на професионалното направление на английски' е задължително!")]
        [StringLength(255)]
        [Display(Name = "Наименование на професионално направление - англйски")]
        public string NameEN { get; set; }

        public string DisplayNameAndCode => this.Code + " " + this.Name;

        public string DisplayNameFilter { get; set; }
        
        public int IdStatus { get; set; }

        public int IdTypeChange { get; set; }

        public bool UpdateAllOrders { get; set; }

        public string NumericValue { get; set; }

        public bool IsTheory { get; set; }
        public bool IsPractice { get; set; }
        public bool IsQualified { get; set; }

        public long oldId { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }

        public List<OrderVM> OrderVMs { get; set; }

        /// <summary>
        /// Списък с професии
        /// </summary>
        public virtual List<ProfessionVM> Professions { get; set; }

        public virtual ICollection<ProfessionalDirectionOrderVM> ProfessionalDirectionOrders { get; set; }

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
