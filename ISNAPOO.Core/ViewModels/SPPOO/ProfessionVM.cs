using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ValidationAttributes;
using ISNAPOO.Core.ViewModels.DOC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.SPPOO
{
    public class ProfessionVM : IMapFrom<Profession>, IMapTo<Profession>
    {
        public ProfessionVM()
        {
            this.ProfessionOrders = new List<ProfessionOrderVM>();
            this.Specialities = new List<SpecialityVM>();
            this.OrderVMs = new List<OrderVM>();
            this.DocVMList = new List<DocVM>();
        }

        [Key]
        public int IdProfession { get; set; }

        public int IdArea { get; set; }

        public int IdProfessionalDirection { get; set; }

        public string CodeAndArea { get; set; }

        public string CodeAndProffessionalDirection { get; set; }

        public string CodeAndName { get { return $"{this.Code} {this.Name}"; } set { } }

        [StringLength(100)]
        [Display(Name = "Код на професията")]
        [Required(ErrorMessage = "Полето 'Код' е задължително!")]
        [ValidSPPOOCode]
        public string Code { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на професионално направление' е задължително!")]
        [StringLength(255)]
        [Display(Name = "Наименование на професията")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на професионално направление ма английски език' е задължително!")]
        [StringLength(255)]
        [Display(Name = "Наименование на професията - англйски")]
        public string NameEN { get; set; }

        // Номенклатура: KeyTypeIntCode - "LegalCapacityOrdinanceType"
        [Comment("Вид на наредбата за правоспособност")]
        public int? IdLegalCapacityOrdinanceType { get; set; }

        public string ProfessionComboFilter { get; set; }

        public int IdTypeChange { get; set; }

        public int IdStatus { get; set; }

        public long oldId { get; set; }

        public ProfessionalDirectionVM ProfessionalDirection { get; set; }

        public List<SpecialityVM> Specialities { get; set; }

        public List<DocVM> DocVMList { get; set; }

        public List<OrderVM> OrderVMs { get; set; }


        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }


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

        public ICollection<ProfessionOrderVM> ProfessionOrders { get; set; }

        [Display(Name = "Професията предполага ли придобиването на правоспособност")]
        public bool IsPresupposeLegalCapacity { get; set; }//(Да/Не)

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
