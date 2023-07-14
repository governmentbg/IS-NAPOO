using Data.Models.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.ExternalExpertCommission
{

    /// <summary>
    /// Член на комисия
    /// </summary>
    [Table("ExpComm_ExpertExpertCommission")]
    [Display(Name = "Член на комисия")]
    public class ExpertExpertCommission : IEntity, IModifiable, IDataMigration

    {
        public ExpertExpertCommission()
        {
            //this.ProcedureExpertExpertCommissions = new List<ProcedureExpertCommission>();
        }

        [Key]
        public int IdExpertExpertCommission { get; set; }
        public int IdEntity => IdExpertExpertCommission;

        [Required]
        [Display(Name = "Експерт")]
        [ForeignKey(nameof(Expert))]
        public int IdExpert { get; set; }
        public virtual Expert Expert { get; set; }


        [Comment("Eкспертна комисия - KeyTypeIntCode = ExpertCommission")]
        public int IdExpertCommission { get; set; }//KeyType -Eкспертни комисии( KeyTypeIntCode = ExpertCommission )

        [Comment("Вид експерт")]
        public int IdRole { get; set; }//KeyType -Роля на експерта KeyTypeIntCode = ExpertRoleCommission: Прецедател, Член )

        
        [StringLength(DBStringLength.StringLength512)]
        [Comment("Институция, която представя")]
        public string Institution { get; set; }

         
        [StringLength(DBStringLength.StringLength512)]
        [Comment("Длъжност ")]
        public string Occupation { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Протокол")]
        public string Protokol { get; set; }

        [Required]
        [Comment("Дата на Протокол")]
        public DateTime ProtokolDate { get; set; }

        [Comment("Статус")]
        public int IdStatus { get; set; }//Активен/Неактивен

        [StringLength(DBStringLength.StringLength512)]
        [Comment("История на промяната")]
        public string? Comment { get; set; }

        //public virtual ICollection<ProcedureExpertCommission> ProcedureExpertExpertCommissions { get; set; }


        [Comment("Тип на участника")]
        public int IdMemberType { get; set; }//Титуляр/Заместник

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public  int IdModifyUser { get; set; }

        [Required]
        public  DateTime ModifyDate { get; set; }
        #endregion

        #region IDataMigration
        public int? OldId { get; set; }

        public string? MigrationNote { get; set; }
        #endregion
    }
}

