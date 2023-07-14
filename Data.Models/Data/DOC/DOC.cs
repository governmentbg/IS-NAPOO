using Data.Models.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.DOC
{
    /// <summary>
    /// Държавен образователен стандарт
    /// </summary>
    [Table("DOC_DOC")]
    [Display(Name = "DOC")]
    public class DOC : AbstractUploadFile, IEntity, IModifiable
    {
        public DOC()
        {
            this.Specialities = new HashSet<Speciality>();
            this.ERUs = new HashSet<ERU>();
            this.docNkpds = new HashSet<DOC_DOC_NKPD>();
        }

        [Key]
        public int IdDOC { get; set; }
        public int IdEntity => IdDOC;

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Наименование на документа, съдържащ ДОС")]
        public string Name { get; set; }


        [Comment("В сила от")]
        public DateTime StartDate { get; set; }


        [Comment("В сила до")]
        public DateTime? EndDate { get; set; }

         

        [Comment("Статус")]
        public int IdStatus { get; set; }//KeyType:StatusSPPOO


        [Required]
        [Display(Name = "Професия")]
        [ForeignKey(nameof(Profession))]
        public int IdProfession { get; set; }
        public virtual Profession Profession { get; set; }

        [Column(TypeName = "ntext")]        
        [Display(Name = "Изискванията към кандидатите")]
        public string RequirementsCandidates { get; set; }

        [Column(TypeName = "ntext")]
        [Display(Name = "Описание на професията")]
        public string DescriptionProfession { get; set; }

        [Column(TypeName = "ntext")]
        [Display(Name = "Изисквания към материалната база")]
        public string RequirementsMaterialBase { get; set; }

        [Column(TypeName = "ntext")]
        [Display(Name = "Изисквания към обучаващите")]
        public string RequirementsТrainers { get; set; }

        [StringLength(DBStringLength.StringLength20)]
        [Display(Name = "Държавен вестник (брой)")]
        public string? NewspaperNumber { get; set; }

        [Display(Name = "Дата на обнародване")]
        public DateTime? PublicationDate { get; set; }

        //public List<DocSpeciality> DocSpecialities { get; set; }

        public virtual ICollection<Speciality> Specialities { get; set; }

        public ICollection<ERU> ERUs { get; set; }
        public virtual ICollection<DOC_DOC_NKPD> docNkpds { get; set; }


        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Път до заповед")]
        public override string UploadedFileName { get; set; }


        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Наредба")]
        public string Regulation { get; set; }


        [Comment("Държавни образователни изисквания")]
        public bool IsDOI { get; set; }//Държавни образователни изисквания, трябва да различим старите стандарти


        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public override int IdModifyUser { get; set; }

        [Required]
        public override DateTime ModifyDate { get; set; }
        #endregion
        #region IDataMigration
        public int? OldId { get; set; }

        public override string? MigrationNote { get; set; }
        #endregion
    }
}
