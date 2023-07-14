using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.EKATTE
{
    public class LocationVM : IMapFrom<Location>, IMapTo<Location>
    {
        public int idLocation { get; set; }

        public int? idMunicipality { get; set; }

        public MunicipalityVM Municipality { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'kati' не може да съдържа повече от 20 символа.")]
        public string kati { get; set; }//vc_kati

        [Required]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'LocationName' не може да съдържа повече от 100 символа.")]
        public string LocationName { get; set; }//vc_name

        [StringLength(DBStringLength.StringLength100)]
        public string LocationNameEN { get; set; }//vc_name

        [Required]
        [StringLength(DBStringLength.StringLength5, ErrorMessage = "Полето 'LocationCode' не може да съдържа повече от 5 символа.")]
        public string LocationCode { get; set; }//vc_text_code

        [Required]
        [StringLength(DBStringLength.StringLength10, ErrorMessage = "Полето 'Cat' не може да съдържа повече от 10 символа.")]
        public string Cat { get; set; }//vc_cat

        [Required]
        [StringLength(DBStringLength.StringLength10, ErrorMessage = "Полето 'Height' не може да съдържа повече от 10 символа.")]
        public string Height { get; set; }//vc_height

        [Required]
        public int PostCode { get; set; }//int_post_code

        [Required]
        public string Kmetstvo { get; set; }///BLG52-01

        [Required]
        public int PhoneCode { get; set; }//vc_phone_code

        public int int_ekatte_id_old { get; set; }//Старо id на населено място

        public string MunicipalityName { get; set; }
        public string DistrictName { get; set; }

        public string DisplayJoinedNames2 => this.kati + ", общ. " + this.MunicipalityName + ", обл. " + this.DistrictName;
        public string DisplayJoinedNames
        {
            get
            {
                return this.LocationName + (this.Municipality != null ? ", общ. " + this.Municipality.MunicipalityName : string.Empty) + (this.Municipality != null && this.Municipality.District != null ? ", обл. " + this.Municipality.District.DistrictName : string.Empty);
            }
            set
            {
                this.DisplayJoinedNames = value;
            }
        }

        public string DisplayJoinedNamesForTrainingInstitution
        {
            get
            {
                return this.LocationName + (this.Municipality != null ? ", общ. " + this.Municipality.MunicipalityName : string.Empty) + (this.Municipality != null && this.Municipality.District != null ? ", обл. " + this.Municipality.District.DistrictName : string.Empty);
            }
        }

        public string LocationJoinedFilter { get; set; }
    }
}
