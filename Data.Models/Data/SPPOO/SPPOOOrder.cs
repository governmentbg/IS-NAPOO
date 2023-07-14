using Data.Models.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Data.Models
{
    /// <summary>
    /// Заповед за промяна
    /// </summary>
    [Table("SPPOO_Order")]
    [Display(Name = "Заповед за промяна")]
    public class SPPOOOrder : AbstractUploadFile, IEntity, IModifiable
    {
        public SPPOOOrder()
        {
            this.OrderNumber = string.Empty;
            this.UploadedFileName = string.Empty;

            this.ProfessionOrders = new HashSet<ProfessionOrder>();
            this.ProfessionalDirectionOrders = new HashSet<ProfessionalDirectionOrder>();
            this.SpecialityOrders = new HashSet<SpecialityOrder>();
        }

        [Key]
        public int IdOrder { get; set; }
        public int IdEntity => IdOrder;

        [Required]
        [StringLength(100)]
        [Display(Name = "Номер на Заповед")]
        public string OrderNumber { get; set; }

        [Required]
        [Display(Name = "Дата на Заповед")]
        public DateTime OrderDate { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Път до заповед")]
        public override string UploadedFileName { get; set; }

        public virtual ICollection<ProfessionalDirectionOrder> ProfessionalDirectionOrders { get; set; }

        public virtual ICollection<SpecialityOrder> SpecialityOrders { get; set; }

        public virtual ICollection<ProfessionOrder> ProfessionOrders { get; set; }

        public override string? MigrationNote { get; set; }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public override int IdModifyUser { get; set; }

        [Required]
        public override DateTime ModifyDate { get; set; }
        #endregion
    }
}