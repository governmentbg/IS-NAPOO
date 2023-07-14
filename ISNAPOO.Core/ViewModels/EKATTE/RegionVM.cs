using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.EKATTE
{
    public class RegionVM : IMapFrom<Region>, IMapTo<Region>
    {
        public int idRegion { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'RegionName' не може да съдържа повече от 255 символа.")]
        public string RegionName { get; set; }

        public int? idMunicipality { get; set; }

        public MunicipalityVM Municipality { get; set; }

        public int int_municipality_details_id_old { get; set; } //Старо id на municipality_details
    }
}
