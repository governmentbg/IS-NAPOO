using Data.Models.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.Data.Role
{
    public class ApplicationRole : IdentityRole,  IModifiable
    {
        public ApplicationRole(): this(null)
        {
            
        }

        public ApplicationRole(string name) : base(name)
        {
            this.Id = Guid.NewGuid().ToString();
        }


        /// <summary>
        /// Наименование на роля използва се за визуализране в приложението 
        /// </summary>
        [StringLength(DBStringLength.StringLength512)]
        [Comment("Наименование на роля")]
        public string RoleName { get; set; }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public   int IdModifyUser { get; set; }

        [Required]
        public   DateTime ModifyDate { get; set; }
        #endregion
    }
}
