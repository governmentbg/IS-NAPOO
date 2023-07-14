using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.SPPOO
{
    /// <summary>
    /// Професия
    /// </summary>
    [Table("SPPOO_Profession")]
    [Display(Name = "Професия")]
    public class Profession : IEntity, IModifiable, IDataMigration
    {
        public Profession()
        {
            this.Specialities = new HashSet<Speciality>();
            this.DOCs = new HashSet<Data.DOC.DOC>();
            this.ProfessionOrders = new HashSet<ProfessionOrder>();
        }

        [Key]
        public int IdProfession { get; set; }
        public int IdEntity => IdProfession;

        [StringLength(100)]
        [Display(Name= "Шифър на професията")]
        public string Code { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Наименование на професията")]
        public string Name { get; set; }
        
        [StringLength(255)]
        [Display(Name = "Наименование на професията - англйски")]
        public string NameEN { get; set; }

        [Required]        
        [Display(Name = "Професионално направление")]
        [ForeignKey(nameof(ProfessionalDirection))]
        public int IdProfessionalDirection { get; set; }

        [Display(Name = "Професията предполага ли придобиването на правоспособност")]
        public bool IsPresupposeLegalCapacity { get; set; }//(Да/Не)
        
        public int IdStatus { get; set; }

        // Номенклатура: KeyTypeIntCode - "LegalCapacityOrdinanceType"
        [Comment("Вид на наредбата за правоспособност")]
        public int? IdLegalCapacityOrdinanceType { get; set; }

        public virtual ProfessionalDirection ProfessionalDirection {get; set; }

        public virtual ICollection<Speciality> Specialities { get; set; }

        public virtual ICollection<Data.DOC.DOC> DOCs { get; set; }

        public virtual ICollection<ProfessionOrder> ProfessionOrders { get; set; }

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
