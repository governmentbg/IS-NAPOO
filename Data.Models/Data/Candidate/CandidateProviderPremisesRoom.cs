using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Candidate
{
    /// <summary>
    /// Помещения  връзка с Метериална техническа база
    /// </summary>
    [Table("Candidate_ProviderPremisesRoom")]
    [Display(Name = "Помещения  връзка с Метериална техническа база")]
    public class CandidateProviderPremisesRoom : IEntity, IModifiable
    {
        public CandidateProviderPremisesRoom()
        {
        }

        [Key]
        public int IdCandidateProviderPremisesRoom { get; set; }
        public int IdEntity => IdCandidateProviderPremisesRoom;

        [Required]
        [Comment("Метериална техническа база")]
        [ForeignKey(nameof(CandidateProviderPremises))]
        public int IdCandidateProviderPremises { get; set; }
        public CandidateProviderPremises CandidateProviderPremises { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Наименование на помещението' не може да съдържа повече от 512 символа.")]
        [Comment("Наименование на помещението")]
        public string PremisesRoomName { get; set; }


        [Column(TypeName = "ntext")]
        [Comment("Кратко описание на оборудването")]
        public string? Equipment { get; set; }



        /// <summary>
        /// "обучение по теория"
        /// "обучение по практика"
        /// "обучение по теория и практика"
        /// </summary>
        [Comment("Вид на провежданото обучение")]
        public int IdUsage { get; set; }
        
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
        public int IdPremisesType { get; set; }

        [Comment("Приблизителна площ (кв. м.")]
        public int? Area { get; set; }

        [Comment("Брой работни места")]
        public int? Workplace { get; set; }


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



