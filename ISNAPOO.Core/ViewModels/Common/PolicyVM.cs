using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class PolicyVM : IMapFrom<Policy>, IMapTo<Policy>
    {

        [Key]
        public int idPolicy { get; set; }

        [Required]
        [Display(Name = "Код на Policy")]       
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Код на Policy' не може да съдържа повече от 100 символа.")]
        public string PolicyCode { get; set; }


        [Display(Name = "Описание на Policy")]        
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Описание на Policy' не може да съдържа повече от 255 символа.")]
        public string PolicyDescription { get; set; }
    }
}
