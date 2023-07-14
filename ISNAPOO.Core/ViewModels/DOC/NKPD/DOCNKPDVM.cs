using Data.Models.Data.DOC;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.DOC.NKPD
{
    public class DOCNKPDVM : IMapFrom<DOC_DOC_NKPD>, IMapTo<DOC_DOC_NKPD>
    {
        [Key]
        public int IdDOC_DOC_NKPD { get; set; }

        [Required]
        [Display(Name = "DOC")]
        public int IdDOC { get; set; }
        public DocVM DOC { get; set; }


        [Required]
        [Display(Name = "НКПД")]
        public int IdNKPD { get; set; }
        public NKPDVM NKPD { get; set; }
    }
}
