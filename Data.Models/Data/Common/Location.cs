using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Common
{

    /// <summary>
    /// Населено място - ЕКАТТЕ
    /// </summary>
    [Table("Location")]
    [Display(Name = "Населено място - ЕКАТТЕ")]
    public class Location : IEntity
    {
        [Key]
        public int idLocation { get; set; }
        public int IdEntity => idLocation;

        [ForeignKey(nameof(Municipality))]
        public int idMunicipality { get; set; }

        public Municipality Municipality { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength50)]
        public string kati { get; set; }//vc_kati

        [Required]
        [StringLength(DBStringLength.StringLength100)]
        public string LocationName { get; set; }//vc_name

        [StringLength(DBStringLength.StringLength100)]
        public string LocationNameEN { get; set; }//vc_name



        [StringLength(DBStringLength.StringLength100)]
        public string? Kmetstvo { get; set; }///BLG52-01

        [Required]
        [StringLength(DBStringLength.StringLength5)]
        public string LocationCode { get; set; }//vc_text_code

        [Required]
        [StringLength(DBStringLength.StringLength10)]
        public string Cat { get; set; }//vc_cat

        [Required]
        [StringLength(DBStringLength.StringLength10)]
        public string Height { get; set; }//vc_height

        [Required]
        public int PostCode { get; set; }//int_post_code

        [Required]
        public int PhoneCode { get; set; }//vc_phone_code

        public int int_ekatte_id_old { get; set; }//Старо id на населено място

    }
}
