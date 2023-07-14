using Data.Models.Data.ExternalExpertCommission;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.ExternalExpertCommission
{
    public class ExpertNapooVM : IMapFrom<ExpertNapoo>
    {
        public ExpertNapooVM()
        {

        }
        public int IdExpertNapoo { get; set; }


       // [Required]
        [Display(Name = "Експерт")]
        public int IdExpert { get; set; }
        public ExpertVM Expert { get; set; }

      
        [StringLength(DBStringLength.StringLength100)]
        [Display(Name = "Длъжност")]
        public string? Occupation { get; set; }

        //[Required(ErrorMessage = "Полето 'Дата на назначаване' е задължително!")]
        [Display(Name = "Дата на назначаване")]
        public DateTime? AppointmentDate { get; set; }

       // [Required(ErrorMessage = "Полето 'Статус' е задължително!")]
        [Display(Name = "Статус")]
        public int IdStatus { get; set; }//Активен/Неактивен
        public string StatusName { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "История на промяната")]
        public string? Comment { get; set; }

        public string CreatePersonName  { get; set; }
        public string ModifyPersonName { get; set; }

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
