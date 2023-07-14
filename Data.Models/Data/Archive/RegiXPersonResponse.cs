using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using ISNAPOO.Common.Constants;

namespace Data.Models.Data.Archive
{
    /// <summary>
    /// Отговор от RegiX при проверка на лице по ЕГН
    /// </summary>
    [Table("Arch_RegiXPersonResponse")]
    [Display(Name = "Отговор от RegiX при справка на лица по ЕГН")]
    public class RegiXPersonResponse : IEntity, IModifiable
    {
        [Key]
        public int IdRegiXPersonResponse { get; set; }

        public int IdEntity => this.IdRegiXPersonResponse;

        [Comment("Стойност на ЕГН")]
        [StringLength(DBStringLength.StringLength10)]
        public string EGN { get; set; }

        [Comment("Име")]
        [StringLength(DBStringLength.StringLength50)]
        public string FirstName { get; set; }

        [Comment("Презиме")]
        [StringLength(DBStringLength.StringLength50)]
        public string SecondName { get; set; }

        [Comment("Фамилия")]
        [StringLength(DBStringLength.StringLength50)]
        public string FamilyName { get; set; }

        [Comment("Дата на раждане")]
        public DateTime BirthDate { get; set; }

        [Comment("Дата на смърт")]
        public DateTime? DeathDate { get; set; }

        [Comment("Дата на проверката в RegiX")]
        public DateTime CheckDate { get; set; }

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
