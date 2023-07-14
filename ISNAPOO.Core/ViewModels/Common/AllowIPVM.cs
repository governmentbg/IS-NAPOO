using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class AllowIPVM
    {        
        public int idAllowIP { get; set; }

        public int IdEntity => this.idAllowIP;

        [Required(ErrorMessage = "Полето 'IP' е задължително!")]
        [StringLength(50)]
        public string IP { get; set; }

        [Required(ErrorMessage = "Полето 'Commnet' е задължително!")]
        [StringLength(255)]
        public string Commnet { get; set; }

        public bool IsAllow { get; set; }

    }
}
