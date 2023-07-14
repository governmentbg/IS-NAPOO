using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.CPO.ProviderData
{
    public class ProcedurePriceVM : IMapFrom<ProcedurePrice>, IMapTo<ProcedurePrice>
    {
        public ProcedurePriceVM()
        {
            this.ExpirationDateFrom = DateTime.Now;
        }

        public int IdProcedurePrice { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на услугата' е задължително!")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Наименование на услугата' може да съдържа до 50 символа!")]
        [Display(Name = "Наименование на услугата")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Полето 'Допълнителна информация' е задължително!")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Допълнителна информация' може да съдържа до 50 символа!")]
        [Display(Name = "Допълнителна информация")]
        public string AdditionalInformation { get; set; }

        public string Concated => $"{this.Name} {this.AdditionalInformation}";

        [Required(ErrorMessage = "Полето 'Цена' е задължително!")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }
        public string PriceAsStr { get; set; }
        public string PriceAsStaticStr => Price.ToString();

        [Required]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид на заявлението' е задължително!")]
        [Display(Name = "Вид на заявлението")]
        public int IdTypeApplication { get; set; }

        public string TypeApplicationName { get; set; }

        [Display(Name = "Брой професии от")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Моля, въведете положителна стойност за Брой професии от!")]
        public int? CountProfessionsFrom { get; set; }

        [Display(Name = "Брой професии до")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Моля, въведете положителна стойност за Брой професии до!")]
        public int? CountProfessionsTo { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на валидност от' е задължително!")]
        [Display(Name = "Дата на валидност от")]
        public DateTime? ExpirationDateFrom { get; set; }

        [Display(Name = "Дата на валидност до")]
        public DateTime? ExpirationDateTo { get; set; }

        [Display(Name = "Статус на обработка на заявлението")]
        public int? IdApplicationStatus { get; set; }

        public string ApplicationStatusName { get; set; }


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
