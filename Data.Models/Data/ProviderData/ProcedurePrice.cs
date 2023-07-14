using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.ProviderData
{
    /// <summary>
    /// Такси за лицензиране 
    /// </summary>
    [Table("Procedure_ProcedurePrice")]
    [Display(Name = "Такси за лицензиране")]
    public class ProcedurePrice : IEntity, IModifiable
    {
        [Key]
        public int IdProcedurePrice { get; set; }
		public int IdEntity => IdProcedurePrice;

		[Required]
        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "Наименование на услугата")]
        public string Name { get; set; }



        [StringLength(DBStringLength.StringLength50)]
        [Comment("Допълнителна информация")]
        public string? AdditionalInformation { get; set; }


        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required]
        [Display(Name = "Вид на заявлението")]
        public int IdTypeApplication { get; set; }

        [Display(Name = "Брой професии от")]
        public int? CountProfessionsFrom { get; set; }

        [Display(Name = "Брой професии до")]
        public int? CountProfessionsTo { get; set; }

        [Required]
        [Display(Name = "Дата на валидност от")]
        public DateTime ExpirationDateFrom { get; set; }

        [Display(Name = "Дата на валидност до")]
        public DateTime? ExpirationDateTo { get; set; }

        [Display(Name = "Статус на обработка на заявлението")]
        public int? IdApplicationStatus { get; set; }


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
