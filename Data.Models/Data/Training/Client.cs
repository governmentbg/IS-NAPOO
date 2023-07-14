using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{


    /// <summary>
    /// Получател на услугата(обучаем) От стара таблица tb_clients
    /// </summary>
    [Table("Training_Client")]
    [Comment("Получател на услугата(обучаем)")]
    public class Client : IEntity, IModifiable, IDataMigration
    {
        public Client()
        {
            this.ConsultingClients = new HashSet<ConsultingClient>();
        }

        [Key]
        public int IdClient { get; set; }
        public int IdEntity => IdClient;

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Име")]
        public string FirstName { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Име на Латиница")]
        public string FirstNameEN { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment(  "Презиме")]
        public string? SecondName { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Презиме на Латиница")]
        public string? SecondNameEN { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment( "Фамилия")]
        public string FamilyName { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Фамилия на Латиница")]
        public string FamilyNameEN { get; set; }

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }

        [Comment( "Пол")]
        public int? IdSex { get; set; }


        [Comment("Вид на идентификатора")]//ЕГН/ЛНЧ/ИДН
        public int? IdIndentType { get; set; }

        [StringLength(DBStringLength.StringLength20)]
        [Comment( "ЕГН/ЛНЧ/ИДН")]
        public string? Indent { get; set; }


        [Comment("Дата на раждане")]
        public DateTime? BirthDate { get; set; }

        [Comment( "Гражданство")]
        public int? IdNationality { get; set; }



        [Display(Name = "Професионално направление")]
        [ForeignKey(nameof(ProfessionalDirection))]
        public int? IdProfessionalDirection { get; set; }
        public ProfessionalDirection ProfessionalDirection { get; set; }


        [Comment("Образование")]
        public int? IdEducation { get; set; }//Таблица 'code_education', висше - бакалавър, висше - магистър, висше - професионален бакалавър, основно, придобито право за явяване на държавни зрелостни изпити за завършване на средно образование

        [Comment("Месторождение (държава)")]
        public int? IdCountryOfBirth { get; set; }

        [Comment("Месторождение (населено място)")]
        public int? IdCityOfBirth { get; set; }

        public ICollection<ConsultingClient> ConsultingClients { get; set; }

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


