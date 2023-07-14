using Data.Models.Data.ExternalExpertCommission;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.DOC;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.ExternalExpertCommission
{
    public class ExpertDOCVM : IMapFrom<ExpertDOC>, IMapTo<ExpertDOC>
    {
        public int IdExpertDOC { get; set; }


        [Required]
        [Comment("Експерт")]
        public int IdExpert { get; set; }
        public virtual ExpertVM Expert { get; set; }

        /// <summary>
        /// Проект на ДОС
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "Полето 'Проект на ДОС' е задължително!")]
        [Comment("Проект на ДОС")]
        public int IdDOC { get; set; }
        public virtual DocVM DOC { get; set; }

        [Required(ErrorMessage = "Полето 'Номер на заповед' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Номер на заповед' не може да съдържа повече от 100 символа.")]
        [Comment("Номер на заповед")]
        public string? OrderNumber { get; set; }
        
        [Required(ErrorMessage = "Полето 'Дата на утвърждаване' е задължително!")]
        [Comment("Дата на утвърждаване")]
        public DateTime? DateOrder { get; set; }
        
        [Required(ErrorMessage = "Изборът на 'Статус' е задължителен")]
        [Comment("Статус")]
        public int IdStatus { get; set; }//Активен/Неактивен

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "История на промяната")]
        public string? Comment { get; set; }

        public string StatusName { get; set; }
        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }


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
