using Data.Models.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Request;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Training
{
    //ValidationClient ValidationClientDocument ValidationDocumentUploadFile
    public class ValidationClientCombinedVM
    {

        public ValidationClientCombinedVM()
        {
            DocumentUploadedFiles = new List<ValidationDocumentUploadedFileVM>();
        }
        public int IdValidationClient { get; set; }

        public int IdValidationClientDocument { get; set; }

        public int? IdDocumentStatus { get; set; }

        public int? IdDuplicateDocumentStatus { get; set; }


        public int IdValidationDocumentUploadedFile { get; set; }

        public int? IdValidationProtocol { get; set; }

        public int? IdTypeOfRequestedDocument { get; set; }


        [Comment("Дата на приключване на курса")]
        public DateTime? FinishedDate { get; set; }

        //[Required(ErrorMessage = "Полето 'Статус на завършване' е задължително!")]
        [Comment("Приключване на курс")]
        public int? IdFinishedType { get; set; }//Таблица 'code_cfinished_type' завършил с документ, прекъснал по уважителни причини, прекъснал по неуважителни причини, завършил курса, но не положил успешно изпита, придобил СПК по реда на чл.40 от ЗПОО, издаване на дубликат

        [Comment("Документи за завършено обучение")]
        public int? IdDocumentType { get; set; }//Сочи към ID на TypeOfRequestedDocument; Таблица 'code_document_type' Свидетелство за придобита СПК, Удостоверение за професионално обучение, Свидетелство за правоспособност, Удостоверение за правоспособност
      
        public string DocumentTypeName { get; set; }

        public bool HasDocumentFabricNumber { get; set; }

        [Comment("Година на приключване")]
        public int? FinishedYear { get; set; } = DateTime.Now.Year;

        [Comment("Връзка с фабричен номер на документ от печатница на МОН")]
        public int? IdDocumentSerialNumber { get; set; }

        public virtual DocumentSerialNumberVM DocumentSerialNumber { get; set; }

        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Регистрационен номер на документа' не може да съдържа повече от 50 символа.")]
        [Comment("Регистрационен номер")]
        public string? DocumentRegNo { get; set; }

        [Comment("Дата на регистрационен документ")]
        public DateTime? DocumentDate { get; set; }

        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Протокол' не може да съдържа повече от 50 символа.")]
        [Comment("Протокол")]
        public string? DocumentProtocol { get; set; }

        //[Comment("Оценка по теория")]
        //public string TheoryResult { get; set; }

        //[Comment("Оценка по практика")]
        //public string PracticeResult { get; set; }

        [Comment("Обща оценка от теория и практика")]
        public string FinalResult { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Прикачен файл' не може да съдържа повече от 512 символа.")]
        [Comment("Прикачен файл")]
        public string UploadedFileName { get; set; }

        [Comment("Прикачен файл")]

        public List<ValidationDocumentUploadedFileVM> DocumentUploadedFiles { get; set; }
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

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.DocumentRegNo) && string.IsNullOrEmpty(this.UploadedFileName) && this.IdDocumentSerialNumber is null && this.IdFinishedType is null;
        }
    }
}
