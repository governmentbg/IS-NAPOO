
using AutoMapper;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.DOC.NKPD;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.DOC
{
    public class DocVM
    {
        public DocVM()
        {
            this.StartDate = DateTime.Now;
            this.Specialities = new HashSet<SpecialityVM>();
            this.DOCNKPDs = new HashSet<DOCNKPDVM>();
            this.ERUs = new HashSet<ERUVM>();
            this.IdsNkpd = new List<int>();

            this.UploadedFileName = "#";
        }
        public int IdDOC { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на документа, съдържащ ДОС' е задължително!")]
        [Display(Name = "Наименование на документа, съдържащ ДОС")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Полето 'Наредба' е задължително!")]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Наредба' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Наредба")]
        public string Regulation { get; set; }

        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Наредба' не може да съдържа повече от 20 символа.")]
        [Display(Name = "Държавен вестник (брой)")]
        public string? NewspaperNumber { get; set; }

        [Display(Name = "Дата на обнародване")]
        public DateTime? PublicationDate { get; set; }

        [Required(ErrorMessage = "Полето 'В сила от' е задължително!")]
        public DateTime? StartDate { get; set; }

        public DateOnly StartDateOnly { get { return DateOnly.FromDateTime( StartDate.Value.Date); } }

        [Required]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Професия' е задължително!")]
        [Display(Name = "Професия")]        
        public int IdProfession { get; set; }

        public ProfessionVM Profession { get; set; }


        [Required(ErrorMessage = "Полето 'Изисквания към кандидатите' е задължително!")]
        [Display(Name = "Изисквания към кандидатите")]
        public string RequirementsCandidates { get; set; }


        [Required(ErrorMessage = "Полето 'Описание на професията' е задължително!")]
        [Display(Name = "Описание на професията")]
        public string DescriptionProfession { get; set; }


        [Required(ErrorMessage = "Полето 'Изисквания към материалната база' е задължително!")]
        [Display(Name = "Изисквания към материалната база")]
        public string RequirementsMaterialBase { get; set; }

        [Required(ErrorMessage = "Полето 'Изисквания към обучаващите' е задължително!")]
        [Display(Name = "Изисквания към обучаващите")]
        public string RequirementsТrainers { get; set; }

        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int IdModifyUser { get; set; }

        [Required]
        public DateTime ModifyDate { get; set; }

        public virtual ICollection<SpecialityVM> Specialities { get; set; }

        public virtual ICollection<DOCNKPDVM> DOCNKPDs { get; set; }
        public virtual ICollection<ERUVM> ERUs { get; set; }



        public List<int> IdsNkpd { get; set; }

        public string SpecialitiesJoin { get; set; }

        public bool IsDOI { get; set; }

        public DateTime? EndDate { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Статус' е задължително!")]
        public int IdStatus { get; set; }//KeyType:StatusSPPOO

        public string StatusName { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }

        public string UploadedFileName { get; set; }
        public MemoryStream UploadedFileStream { get; set; }

        public int EditButtonClick { get; set; }

        public string FileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    var arrNameParts = this.UploadedFileName.Split(new string[2] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);

                    return (arrNameParts.Length > 0 ? arrNameParts.Last() : string.Empty);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public bool HasUploadedFile
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


    }
}
