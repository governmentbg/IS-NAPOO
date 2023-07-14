using Data.Models.Data.Framework;
using Data.Models.Data.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.SPPOO
{

    /// <summary>
    /// Област
    /// </summary>
    [Table("SPPOO_Area")]
    [Display(Name = "Област")]
    public class Area : IEntity, IModifiable, IDataMigration
    {
        public Area()
        {
            this.ProfessionalDirections = new HashSet<ProfessionalDirection>();
        }

        [Key]
        public int IdArea { get; set; }
        public int IdEntity => IdArea;

        [StringLength(100)]
        [Display(Name = "Шифър на областта")]
        public string Code { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Наименование на областта")]
        public string Name { get; set; }


        [StringLength(255)]
        [Display(Name = "Наименование на областта - англйски")]
        public string NameEN { get; set; }

        public int IdStatus { get; set; }

        public virtual ICollection<ProfessionalDirection> ProfessionalDirections { get; set; }


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



        #region IDataMigration
        public int? OldId { get; set; }

        public string? MigrationNote { get; set; }
        #endregion




    }
}
