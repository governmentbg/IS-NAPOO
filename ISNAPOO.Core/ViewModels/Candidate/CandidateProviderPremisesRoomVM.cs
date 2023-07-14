using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderPremisesRoomVM
    {
        public int IdCandidateProviderPremisesRoom { get; set; }

        [Required]
        [Comment("Метериална техническа база")]
        public int IdCandidateProviderPremises { get; set; }

        public CandidateProviderPremisesVM CandidateProviderPremises { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на помещението' е задължително!")]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Наименование на помещението' не може да съдържа повече от 512 символа.")]
        [Comment("Наименование на помещението")]
        public string PremisesRoomName { get; set; }

        [Required(ErrorMessage = "Полето 'Кратко описание на оборудването' е задължително!")]
        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Кратко описание на оборудването' не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Кратко описание на оборудването")]
        public string? Equipment { get; set; }

        /// <summary>
        /// "обучение по теория"
        /// "обучение по практика"
        /// "обучение по теория и практика"
        /// </summary>
        [Comment("Вид на провежданото обучение")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид на провежданото обучение' е задължително!")]
        public int IdUsage { get; set; }

        public string UsageName { get; set; }

        /// <summary>
        /// "учебен кабинет"
        /// "специализиран кабинет за теоретично обучение по ПП"
        /// "компютърна зала"
        /// "лаборатория"
        /// "работилница"
        /// "производствена база"
        /// "професионална библиотека"
        /// "друг вид"
        /// </summary>
        [Comment("Вид на помещението")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид на помещението' е задължително!")]
        public int IdPremisesType { get; set; }

        public string PremisesTypeName { get; set; }

        [Comment("Приблизителна площ (кв. м.")]
        public int? Area { get; set; }

        [Comment("Брой работни места")]
        public int? Workplace { get; set; }

        public int IdForGrid { get; set; }

        public string IdForGridAsStr => $"{this.IdForGrid}.";

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
        #region IDataMigration
        public int? OldId { get; set; }

        public string? MigrationNote { get; set; }
        #endregion
    }
}
