using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Data.Models.Data.SPPOO
{

    /// <summary>
    /// Професия
    /// </summary>
    [Table("SPPOO_ProfessionalDirection")]
    [Display(Name = "Професионално направление")]
    public class ProfessionalDirection : IEntity, IModifiable, IDataMigration
    {
        public ProfessionalDirection()
        {
            this.ProfessionalDirectionOrders = new HashSet<ProfessionalDirectionOrder>();
            this.Professions = new HashSet<Profession>();
        }

        [Key]
        public int IdProfessionalDirection { get; set; }
        public int IdEntity => IdProfessionalDirection;

        [Required]
        [ForeignKey(nameof(Area))]
        public int IdArea { get; set; }

        public virtual Area Area { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Шифър на професионално направление")]
        public string Code { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Наименование на професионално направление")]
        public string Name { get; set; }


        [StringLength(255)]
        [Display(Name = "Наименование на професионално направление - англйски")]
        public string NameEN { get; set; }

        public int IdStatus { get; set; }

        [Display(Name = "Eкспертна комисия")]
        public int IdExpertCommission { get; set; }//KeyType -Eкспертни комисии( KeyTypeIntCode = ExpertCommission )

        public virtual ICollection<Profession> Professions { get; set; }

        public virtual ICollection<ProfessionalDirectionOrder> ProfessionalDirectionOrders { get; set; }

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