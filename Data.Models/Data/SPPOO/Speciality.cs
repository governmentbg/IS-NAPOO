using Data.Models.Data.Candidate;
using Data.Models.Data.DOC;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Data.Models.Data.SPPOO
{
    /// <summary>
    /// Специалност
    /// </summary>
    [Table("SPPOO_Speciality")]
    [Display(Name = "Специалност")]
    public class Speciality : IEntity, IModifiable, IDataMigration
    {
        public Speciality()
        {
            this.SpecialityNKPDs = new HashSet<SpecialityNKPD>();
            this.SpecialityOrders = new HashSet<SpecialityOrder>();
            this.ERUSpecialities = new HashSet<ERUSpeciality>();
            this.CandidateProviderPremisesSpecialities = new HashSet<CandidateProviderPremisesSpeciality>();
            this.CandidateProviderTrainerSpecialities = new HashSet<CandidateProviderTrainerSpeciality>();
            this.Programs = new HashSet<Program>();
        }

        [Key]
        public int IdSpeciality { get; set; }
        public int IdEntity => IdSpeciality;

        [Required]
        [ForeignKey(nameof(Profession))]
        public int IdProfession { get; set; }

        public virtual Profession Profession{get;set;}

        [Required]
        [StringLength(DBStringLength.StringLength100)]
        [Display(Name = "Шифър на Специалност")]
        public string Code { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Наименование на Специалност")]
        public string Name { get; set; }


        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Наименование на Специалност - англйски")]
        public string NameEN { get; set; }


        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Линк към Националните изпитни програми")]
        public string LinkNIP { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Display(Name = "Линк към Учебни планове и учебни програми на МОН")]
        public string LinkMON { get; set; }

        // public virtual ICollection<DocSpeciality> DocSpecialities { get; set; }

        [Display(Name = "Статус на валидност")]
        public int IdStatus { get; set; }

        [Display(Name = "Степен на професионална квалификация")]
        public int IdVQS { get; set; }//code_speciality_vqs: I СПК, II СПК....V СПК

        public virtual ICollection<SpecialityNKPD> SpecialityNKPDs { get; set; }

        public virtual ICollection<SpecialityOrder> SpecialityOrders { get; set; }

        [Display(Name = "Нива по Националната квалификационна рамка (НКР)")]
        public int IdNKRLevel { get; set; }//1,2,3,4,5

        [Display(Name = "Нива по Европейската квалификационна рамка (ЕКР)")]
        public int IdEKRLevel { get; set; }//1,2,3,4,5

        [Display(Name = "Включване в „Списък със защитените от държавата специалности от професии“")]
        public bool IsStateProtectedSpecialties { get; set; }//(Да/Не)

        [Display(Name = "Включване в „Списък със специалности от професии, по които е налице очакван недостиг от специалисти на пазара на труда“")]
        public bool IsShortageSpecialistsLaborMarket { get; set; }//(Да/Не)

        [Display(Name = "Допустимост  за обучение на ученици")]
        public bool IsTrainingStudents { get; set; }//(Да/Не)

        [Display(Name = "Допустимост  за обучение на възрастни")]
        public bool IsAdultEducation { get; set; }//(Да/Не)

        [Display(Name = "Допустимост  за обучение по част от професия")]
        public bool IsTrainingPartProfession { get; set; }//(Да/Не)

        [Display(Name = "Допустимост  за дистанционно обучение")]
        public bool IsDistanceLearning { get; set; }//(Да/Не)
        
        [ForeignKey(nameof(Models.Data.DOC.DOC))]
        public int? IdDOC { get; set; }
        public virtual Models.Data.DOC.DOC DOC { get; set; }


        public virtual ICollection<ERUSpeciality> ERUSpecialities { get; set; }

        public virtual ICollection<CandidateProviderPremisesSpeciality> CandidateProviderPremisesSpecialities { get; set; }

        public virtual ICollection<CandidateProviderTrainerSpeciality> CandidateProviderTrainerSpecialities { get; set; }
        public virtual ICollection<Program> Programs { get; set; }

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

