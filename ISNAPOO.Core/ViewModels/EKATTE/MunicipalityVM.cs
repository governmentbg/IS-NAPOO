using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.EKATTE
{
    public class MunicipalityVM : IMapFrom<Municipality>, IMapTo<MunicipalityVM>
    {
        public int idMunicipality { get; set; }

        public int idDistrict { get; set; }

        public DistrictVM District { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'MunicipalityName' не може да съдържа повече от 100 символа.")]
        public string MunicipalityName { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        public string MunicipalityNameEN { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength5, ErrorMessage = "Полето 'MunicipalityCode' не може да съдържа повече от 5 символа.")]
        public string MunicipalityCode { get; set; }

        public virtual ICollection<LocationVM> Locations { get; set; }

        public virtual ICollection<RegionVM> Regions { get; set; }

        public int int_municipality_id_old { get; set; } //Страро ID на община
    }
}
