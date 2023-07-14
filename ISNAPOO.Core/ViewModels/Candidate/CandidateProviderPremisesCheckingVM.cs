using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISNAPOO.Core.ViewModels.Control;

namespace ISNAPOO.Core.ViewModels.Candidate
{

    /// <summary>
    /// Връзка на Метериална техническа база и направена проверка
    /// </summary>
   
    [Display(Name = "Връзка на Метериална техническа база и направена проверка")]
    public class CandidateProviderPremisesCheckingVM
    {

        public CandidateProviderPremisesCheckingVM()
        {
        }

      
        public int IdCandidateProviderPremisesChecking { get; set; }

        [Required(ErrorMessage = "Полето 'Материално-техническа база' е задължително!")]
        [Display(Name = "Връзка с MTB") ]
        public int? IdCandidateProviderPremises { get; set; }
        public CandidateProviderPremisesVM CandidateProviderPremises { get; set; }

        [Display(Name = "Последващ контрол, изпълняван от служител/и на НАПОО")]
        public int? IdFollowUpControl { get; set; }

        public FollowUpControlVM FollowUpControl { get; set; }


        [Display(Name = "Извършена проверка от експерт на НАПОО")]
        public bool CheckDone { get; set; } = true;


        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Коментар' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Коментар")]
        public string? Comment { get; set; }


        [Required(ErrorMessage = "Полето 'Дата на проверка' е задължително!")]
        [Display(Name = "Дата на проверка")]
        public DateTime? CheckingDate { get; set; }


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

    }
}
