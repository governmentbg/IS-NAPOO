using Data.Models.Data.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.Common
{

    /// <summary>
    /// Позволени ИП адреси
    /// </summary>
    [Table("AllowIP")]
    [Display(Name = "Позволени ИП адреси")]
    public class AllowIP : IEntity //, IModifiable
    {
        [Key]
        public int idAllowIP { get; set; }

        public int IdEntity => idAllowIP;

        [Required]
        [StringLength(50)]
        public string IP { get; set; }


        [StringLength(255)]
        public string Commnet { get; set; }

        public bool IsAllow { get; set; }



    }
}
