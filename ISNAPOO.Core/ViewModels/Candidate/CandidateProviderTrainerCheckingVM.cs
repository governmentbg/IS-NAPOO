using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Candidate
{
     

     
    [Display(Name = "Връзка на преподавател и направена проверка")]
    public class CandidateProviderTrainerCheckingVM  
    {
        public CandidateProviderTrainerCheckingVM()
        {
        }

      
        public int IdCandidateProviderTrainerChecking { get; set; }

        [Required(ErrorMessage = "Полето 'Преподавател' е задължително!")]
        public int? IdCandidateProviderTrainer { get; set; }
        public CandidateProviderTrainerVM CandidateProviderTrainer { get; set; }


        [Display (Name = "Последващ контрол, изпълняван от служител/и на НАПОО")]
        public int? IdFollowUpControl { get; set; }

        public bool CheckDone { get; set; } = true;

        [Required(ErrorMessage = "Полето 'Дата на проверка' е задължително!")]
        public DateTime? CheckingDate { get; set; }

        
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Коментар' не може да съдържа повече от 512 символа.")]
        public string? Comment { get; set; }
         

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
