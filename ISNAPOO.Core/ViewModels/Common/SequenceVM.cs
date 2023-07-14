using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class SequenceVM
    {
        [Key]
        public int idSequence { get; set; }

        [Required]
        [Display(Name = "Ресурс")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Ресурс' не може да съдържа повече от 100 символа.")]
        public string Resource { get; set; }

        [Display(Name = "ИД На ресурс")]
        public int? IdResource { get; set; }

        [Display(Name = "Година")]
        public int? Year { get; set; }

        [Display(Name = "Следваща стойност")]
        public int NextVal { get; set; }
    }
}
