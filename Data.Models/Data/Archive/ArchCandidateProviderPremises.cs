using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Archive
{
    /// <summary>
    /// АРХИВ - Метериална техническа база връзка с CPO,CIPO - Обучаваща институция
    /// </summary>
    [Table("Arch_Candidate_ProviderPremises")]
    [Display(Name = "Метериална техническа база връзка с CPO, CIPO - Обучаваща институция")]
    public class ArchCandidateProviderPremises : IEntity, IModifiable, IDataMigration
    {
        public ArchCandidateProviderPremises()
        {
          
            this.CandidateProviderPremisesSpecialities = new HashSet<ArchCandidateProviderPremisesSpeciality>();
            
        } 

        [Key]
        public int IdArchCandidateProviderPremises { get; set; }

        public int IdEntity => IdArchCandidateProviderPremises;

        public int? IdCandidateProviderPremises { get; set; }


        [Display(Name = "АРХИВ Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(ArchCandidateProvider))]
        public int IdArchCandidateProvider { get; set; }
        public ArchCandidateProvider ArchCandidateProvider { get; set; }

        [Required]
        [Comment("CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }


        [Required]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Наименование на материално-техническата база' не може да съдържа повече от 512 символа.")]
        [Comment("Наименование на материално-техническата база")]
        public string PremisesName { get; set; }

         
        
        [Column(TypeName = "ntext")]
        [Comment("Кратко описание")]
        public string? PremisesNote { get; set; }

        [Comment( "Населено място")]
        [ForeignKey(nameof(Location))]
        public int? IdLocation { get; set; }
        public Location Location { get; set; }

        [Comment("Адрес")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес' не може да съдържа повече от 255 символа.")]
        public string ProviderAddress { get; set; }

        [Comment("Пощенски код")]
        [StringLength(DBStringLength.StringLength4, ErrorMessage = "Полето 'Пощенски код' не може да съдържа повече от 4 символа.")]
        public string ZipCode { get; set; }

        [Comment("Телефон")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Телефон' не може да съдържа повече от 255 символа.")]
        public string? Phone { get; set; }

        /// <summary>
        /// "собствена"
        /// "под наем"
        /// </summary>

        [Comment("Форма на собственост")]
        public int IdOwnership { get; set; }


        /// <summary>
        /// "активен"
        /// "неактивен"
        /// </summary>
        [Comment("Статус")]
        public int IdStatus { get; set; }

        [Comment("Дата на деактивиране на базата")]
        [Display(Name = "Дата на деактивиране на базата")]
        public DateTime? InactiveDate { get; set; }


       
        public virtual ICollection<ArchCandidateProviderPremisesSpeciality> CandidateProviderPremisesSpecialities { get; set; }
       
        

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



