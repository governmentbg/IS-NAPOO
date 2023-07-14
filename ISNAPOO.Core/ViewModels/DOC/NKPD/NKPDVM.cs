using ISNAPOO.Core.Mapping;
using System;
using System.ComponentModel.DataAnnotations;
using ISNAPOO.Common.Constants;

namespace ISNAPOO.Core.ViewModels.DOC.NKPD
{
    public class NKPDVM : IMapTo<Data.Models.Data.DOC.NKPD>, IMapFrom<Data.Models.Data.DOC.NKPD>
    {
        public int IdNKPD { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Наименование' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength10, ErrorMessage = "Полето 'Код' не може да съдържа повече от 10 символа.")]
        [Display(Name = "Код")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Код на клас")]
        public int IdClassCode { get; set; }
        public string ClassName { get; set; }
        public string ClassCode { get; set; }

        [Required]
        [Display(Name = "Код на подклас")]
        public int IdSubclassCode { get; set; }
        public string SubclassName { get; set; }
        public string SubclassCode { get; set; }

        [Required]
        [Display(Name = "Код на група")]
        public int IdGroupCode { get; set; }
        public string GroupName { get; set; }
        public string GroupCode { get; set; }

        [Required]
        [Display(Name = "Код на единична група")]
        public int IdIndividualGroupCode { get; set; }
        public string IndividualGroupName { get; set; }
        public string IndividualGroupCode { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength10, ErrorMessage = "Полето 'Код на образователно и квалификационно ниво' не може да съдържа повече от 10 символа.")]
        [Display(Name = "Код на образователно и квалификационно ниво")]
        public string EducationLevelCode { get; set; }
        public string CodeAndName => $"{this.Code} - {this.Name}";
        public string ClassCodeAndName => $"{this.ClassCode} - {this.ClassName}";
        public string SubclassCodeAndName => $"{this.SubclassCode} - {this.SubclassName}";
        public string GroupCodeAndName => $"{this.GroupCode} - {this.GroupName}";
        public string IndividualGroupCodeAndName => $"{this.IndividualGroupCode} - {this.IndividualGroupName}";

        public int CodeFormattedInt
        {
            get
            {
                if (!string.IsNullOrEmpty(this.Code))
                {
                    return int.Parse(this.Code);
                }

                return GlobalConstants.INVALID_ID;
            }
        }

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
