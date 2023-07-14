using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Data.Models.Data.Common
{


    /// <summary>
    /// Лице
    /// </summary>
    [Table("Person")]
    [Display(Name = "Лице")]
    public class Person : IEntity, IModifiable, IDataMigration
    {
        public Person()
        {
            this.FirstName = string.Empty;
            this.SecondName = string.Empty;
            this.FamilyName = string.Empty;

            CandidateProviderPersons = new List<CandidateProviderPerson>();
        }

        [Key]
        public int IdPerson { get; set; }
        public int IdEntity => IdPerson;

        [Required]
        [StringLength(DBStringLength.StringLength100)]
        [Display(Name = "Име")]
        public string FirstName { get; set; }

       
        [StringLength(DBStringLength.StringLength100)]
        [Display(Name = "Презиме")]
        public string? SecondName { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength100)]
        [Display(Name = "Фамилия")]
        public string FamilyName { get; set; }

        [Display(Name = "Вид на идентификатора")]//ЕГН/ЛНЧ/ИДН
        public int? IdIndentType { get; set; }

        [StringLength(DBStringLength.StringLength20)]
        [Display(Name = "ЕГН/ЛНЧ/ИДН")]
        public string? Indent { get; set; }

        [StringLength(DBStringLength.StringLength10)]
        [Display(Name = "Номер на лична карта")]
        public string? PersonalID { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Данъчна служба")]
        public string TaxOffice { get; set; }
        public DateTime? PersonalIDDateFrom { get; set; }

        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "Издадена от")]
        public string? PersonalIDIssueBy { get; set; }

        [Display(Name = "Дата на раждане")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Пол")]
        public int? IdSex { get; set; }

        [Display(Name = "Населено място")]
        [ForeignKey(nameof(Location))]
        public int? IdLocation { get; set; }
        public Location? Location { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Адрес")]
        public string? Address { get; set; }

        [StringLength(DBStringLength.StringLength10)]
        [Display(Name = "Пощенски код")]
        public string? PostCode { get; set; }

        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }
        
        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "Титла")]
        public string? Title {get; set;}

        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Длъжност")]
        public string? Position { get; set; }


        [Comment("Сключва се договор")]
        public bool IsSignContract { get; set; }

        [Comment("Договорът се регистрира в деловодната система")]
        public bool IsContractRegisterDocu { get; set; }


        [Comment("Дата на нулиране на парола")]
        public DateTime? PasswordResetDate { get; set; }

        public virtual ICollection<CandidateProviderPerson> CandidateProviderPersons { get; set; }

        /// <summary>
        /// Зарежда имената на лицето на базата на пълно име
        /// </summary>
        /// <param name="fullPersonName"></param>
        public void PreSetPersonName(string fullPersonName)
        {
            string[] personNames = fullPersonName.Split(' ');

            FirstName = personNames[0];
            SecondName = personNames.Length > 1 ? personNames[1] : string.Empty;
            FamilyName = personNames.Length > 2 ? personNames[2] : string.Empty;


            if (string.IsNullOrEmpty(FamilyName) && !string.IsNullOrEmpty(SecondName))
            {
                FamilyName = SecondName;
                SecondName = string.Empty;
            }
        }

         

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
