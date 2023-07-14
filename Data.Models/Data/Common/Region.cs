using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Common
{
    

    /// <summary>
    /// Район - ЕКАТТЕ
    /// </summary>
    [Table("Region")]
    [Display(Name = "Район - ЕКАТТЕ")]
    public class Region : IEntity
    {
        [Key]
        public int idRegion { get; set; }
        public int IdEntity => idRegion;

        [Required]
        [StringLength(DBStringLength.StringLength255)]
        public string RegionName { get; set; }


        [ForeignKey(nameof(Municipality))]
        public int idMunicipality { get; set; }

        public Municipality Municipality { get; set; }
        
        public int int_municipality_details_id_old { get; set; } //Старо id на municipality_details



    }
}
