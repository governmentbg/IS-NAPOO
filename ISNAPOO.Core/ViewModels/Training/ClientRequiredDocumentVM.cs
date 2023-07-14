using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ClientRequiredDocumentVM : IMapFrom<ClientRequiredDocument>
    {
        public ClientRequiredDocumentVM()
        {
        }

        public int IdClientRequiredDocument { get; set; }



        [Display(Name = "Връзка с Курс за обучение, предлагани от ЦПО")]
        public int IdCourse { get; set; }
        public Course Course { get; set; }


        [Display(Name = "Връзка с курс/обучаем")]
        public int? IdClientCourse { get; set; }
        public ClientCourse ClientCourse { get; set; }

        [Required(ErrorMessage = "Полето 'Вид на документа' е задължително!")]
        [Display(Name = "Вид на документа")]
        public int? IdCourseRequiredDocumentType { get; set; }
        public string CourseRequiredDocumentTypeName { get; set; }
        //int_code_course_group_required_documents_type_id, Таблица 'code_course_group_required_documents_type' със стойст 
        //bool_for_client = true
        //Стойности: Заявление,Други документи,Протокол, Медицински документ,Информационна карта,Индивидуален план-график


        [Display(Name = "Образование")]//KeyType код - Education
        public int? IdEducation { get; set; }//Таблица 'code_education' висше - бакалавър,висше - магистър,висше - професионален бакалавър,завършен VII клас (за лица с увреждания),завършен Х клас по реда на отменените ЗНП и ЗСООМУП (или по-високо),завършен клас от средно образование,завършен курс за ограмотяване по реда на ЗНЗ или на ЗПУО ,завършен начален етап или курс за ограмотяване,завършен начален етап на основното образование (или по-високо),завършен първи гимназиален етап (X клас),завършен първи гимназиален етап на основание ЗПУО (или по-високо),завършено средно образование (или по-високо),основно,придобито право за явяване на държавни зрелостни изпити за завършване на средно образование,придобито право за явяване на държавни зрелостни изпити за завършване на средно образование,средно общо,средно със степен на професионална квалификация,валидирани компетентности за начален етап на основно образование по чл. 167, ал. 1, т. 4 от ЗПУО,друго



        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "Фабричен номер")]
        public string? DocumentPrnNo { get; set; }//vc_document_prn_no


        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "Регистрационен номер")]
        public string? DocumentRegNo { get; set; }//vc_document_reg_no

        [Display(Name = "Дата на регистрационен документ")]
        public DateTime? DocumentDate { get; set; }//dt_document_date


        [Display(Name = "Официална дата на документ")]
        public DateTime? DocumentOfficialDate { get; set; }//dt_document_official_date


        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Описание")]
        public string? Description { get; set; }//vc_desciption


        [Display(Name = "Документа е валиден")]
        public bool IsValid { get; set; }//is_valid

        [Display(Name = "Документа e след 2007 година")]
        public bool IsBeforeDate { get; set; }//bool_before_date



        [Display(Name = "Квалификационно ниво")]
        public int? IdMinimumQualificationLevel { get; set; } // номенклатура: KeyTypeIntCode - "MinimumQualificationLevel"


        [Display(Name = "Дата на издаване на документа")]
        public DateTime? IssueDocumentDate { get; set; }

        public int Order { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Прикачен файл")]
        public string UploadedFileName { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }

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
