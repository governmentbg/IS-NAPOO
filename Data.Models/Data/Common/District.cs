using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Common
{


    /// <summary>
    /// Област - ЕКАТТЕ
    /// </summary>
    [Table("District")]
    [Display(Name = "Област - ЕКАТТЕ")]
    public class District : IEntity
    {
        [Key]
        public int idDistrict { get; set; }

        public int IdEntity => idDistrict;

        [Required]
        [StringLength(DBStringLength.StringLength100)]
        public string DistrictName { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        public string DistrictNameEN { get; set; }


        [Required]
        [StringLength(DBStringLength.StringLength3)]
        public string DistrictCode { get; set; }


        public virtual ICollection<Municipality> Municipalities { get; set; }

        public int int_obl_id_old { get; set; } //Старо id на област

        public int? NSICode { get; set; }

    }
}
