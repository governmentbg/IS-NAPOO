using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.EKATTE
{
    public class DistrictVM : IMapTo<District>, IMapFrom<DistrictVM>
    {
        public int idDistrict { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'DistrictName' не може да съдържа повече от 100 символа.")]
        public string DistrictName { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        public string DistrictNameEN { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength3, ErrorMessage = "Полето 'DistrictCode' не може да съдържа повече от 3 символа.")]
        public string DistrictCode { get; set; }


        public virtual ICollection<MunicipalityVM> Municipalities { get; set; }

        public int int_obl_id_old { get; set; } //Старо id на област

        public int? NSICode { get; set; }
    }
}
