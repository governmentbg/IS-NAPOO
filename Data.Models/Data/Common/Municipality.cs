using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Common
{
    /// <summary>
    /// Община - ЕКАТТЕ
    /// </summary>
    [Table("Municipality")]
    [Display(Name = "Община - ЕКАТТЕ")]
    public class Municipality : IEntity
    {
        [Key]
        public int idMunicipality { get; set; }
        public int IdEntity => idMunicipality;

        [ForeignKey(nameof(District))]
        public int idDistrict { get; set; }
        
        public District District { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength100)]
        public string MunicipalityName { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        public string MunicipalityNameEN { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength5)]
        public string MunicipalityCode { get; set; }

        public virtual ICollection<Location> Locations { get; set; }

        public virtual ICollection<Region> Regions { get; set; }

        public int int_municipality_id_old { get; set; } //Страро ID на община
        


    }
}
