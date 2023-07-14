using Data.Models.Common;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.ProviderData
{
    public class ApplicationUser : IdentityUser, IModifiable
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
            
        }



        
        public int IdUser { get; set; }

        [Display(Name = "Връзка с лице")]
        [ForeignKey(nameof(Person))]
        public int? IdPerson { get; set; }
        public Person Person { get; set; }

        /// <summary>
        /// Активен,Неактивен
        /// </summary>
        [Display(Name = "Статус на потребителя")]        
        public int? IdUserStatus { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

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
