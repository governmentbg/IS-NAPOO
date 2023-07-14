using Data.Models.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.EKATTE;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class ProviderRequestDocumentVM 
    {
        public ProviderRequestDocumentVM()
        {
            this.RequestDocumentStatuses = new HashSet<RequestDocumentStatusVM>();
            this.RequestDocumentTypes = new HashSet<RequestDocumentTypeVM>();
            this.RequestDocumentManagements = new HashSet<RequestDocumentManagementVM>();
        }

        public int IdProviderRequestDocument { get; set; }

        [Display(Name = "Връзка с  CPO,CIPO - Обучаваща институция")]
        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Display(Name = "Връзка с обобщена заявка на документи към печатницата на МОН")]
        public int? IdNAPOORequestDoc { get; set; }

        public virtual NAPOORequestDocVM NAPOORequestDoc { get; set; }

        [Required(ErrorMessage = "Полето 'Година' е задължително!")]
        [Comment("Година на заявка")]
        public int? CurrentYear { get; set; } = DateTime.Now.Year;

        [Comment("Дата на заявка")]
        public DateTime? RequestDate { get; set; }

        [Required(ErrorMessage = "Полето 'Длъжност' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Длъжност' може да съдържа до 100 символа!")]
        [Comment("Длъжност на заявител")]
        public string Position { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Имена на заявител' може да съдържа до 100 символа!")]
        [Comment("Имена на заявител")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Полето 'Адрес' е задължително!")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес' може да съдържа до 100 символа!")]
        [Comment("Адрес")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Полето 'Телефон' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Телефон' може да съдържа до 100 символа!")]
        [Comment("Телефон")]
        public string Telephone { get; set; }

        [Comment("Заявката е изпратена към печатницата")]
        public bool IsSent { get; set; }//Заявката е изпратена към печатницата

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public string UploadedFileName { get; set; }

        [Comment("Ноемер на заявка")]
        public long? RequestNumber { get; set; }

        [Comment("Статус на заявката")]
        public int IdStatus { get; set; }// Номенклатура статус на заявка: RequestDocumetStatus

        public string RequestStatus { get; set; }

        [Comment("Населено място за кореспондениця на ЦПО,ЦИПО")]
        [Required(ErrorMessage = "Полето 'Населено място' е задължително!")]
        public int? IdLocationCorrespondence { get; set; }

        public LocationVM LocationCorrespondence { get; set; }
        public string CreatePersonName { get; set; }
        public string ModifyPersonName { get; set; }

        public string LocationName { get; set; }

        public string FileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    var arrNameParts = this.UploadedFileName.Split(new string[2] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);

                    return (arrNameParts.Length > 0 ? arrNameParts[arrNameParts.Length - 1] : string.Empty);
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

        public string RequestNumberAndDate => $"{this.RequestNumber}/{this.RequestDate.Value.ToString(GlobalConstants.DATE_FORMAT)}г.";

        public virtual ICollection<RequestDocumentStatusVM> RequestDocumentStatuses { get; set; }

        public virtual ICollection<RequestDocumentTypeVM> RequestDocumentTypes { get; set; }

        public virtual ICollection<RequestDocumentManagementVM> RequestDocumentManagements { get; set; }

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
        public int? oid_request_pdf { get; set; } //ОТ Стара систем oid_request_pdf Ще се ползва само за миграцията

        public string? MigrationNote { get; set; }
        #endregion
    }
}
