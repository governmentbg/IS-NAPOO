using Data.Models.Common;
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

namespace Data.Models.Data.Training
{


    /// <summary>
    /// Издадени документи на курсисти tb_clients_courses_documents
    /// </summary>
    [Table("Training_ClientRequiredDocument")]
    [Comment("Издадени документи на курсисти")]
    public class ClientRequiredDocument : AbstractUploadFile, IEntity, IModifiable, IDataMigration
    {
        public ClientRequiredDocument()
        {
        }

        [Key]
        public int IdClientRequiredDocument { get; set; }
        public int IdEntity => IdClientRequiredDocument;

        [Comment("Връзка с Курс за обучение, предлагани от ЦПО")]
        [ForeignKey(nameof(Course))]
        public int IdCourse { get; set; }
        public Course Course { get; set; }


        [Comment("Връзка с курс/обучаем")]
        [ForeignKey(nameof(ClientCourse))]
        public int? IdClientCourse { get; set; }
        public ClientCourse ClientCourse { get; set; }


        [Comment("Тип задължителни документи за курс,курсист")]        
        public int IdCourseRequiredDocumentType { get; set; }
        //int_code_course_group_required_documents_type_id, Таблица 'code_course_group_required_documents_type' със стойст 
        //bool_for_client = true
        //Стойности: Заявление,Други документи,Протокол, Медицински документ,Информационна карта,Индивидуален план-график


        [Comment("Образование:KeyType код - Education")]
        public int? IdEducation { get; set; }//Таблица 'code_education' висше - бакалавър,висше - магистър,висше - професионален бакалавър,завършен VII клас (за лица с увреждания),завършен Х клас по реда на отменените ЗНП и ЗСООМУП (или по-високо),завършен клас от средно образование,завършен курс за ограмотяване по реда на ЗНЗ или на ЗПУО ,завършен начален етап или курс за ограмотяване,завършен начален етап на основното образование (или по-високо),завършен първи гимназиален етап (X клас),завършен първи гимназиален етап на основание ЗПУО (или по-високо),завършено средно образование (или по-високо),основно,придобито право за явяване на държавни зрелостни изпити за завършване на средно образование,придобито право за явяване на държавни зрелостни изпити за завършване на средно образование,средно общо,средно със степен на професионална квалификация,валидирани компетентности за начален етап на основно образование по чл. 167, ал. 1, т. 4 от ЗПУО,друго



        [StringLength(DBStringLength.StringLength100)]
        [Comment("Фабричен номер")]
        public string? DocumentPrnNo { get; set; }//vc_document_prn_no


        [StringLength(DBStringLength.StringLength100)]
        [Comment("Регистрационен номер")]
        public string? DocumentRegNo { get; set; }//vc_document_reg_no

        [Comment("Дата на регистрационен документ")]
        public DateTime? DocumentDate { get; set; }//dt_document_date


        [Comment("Официална дата на документ")]
        public DateTime? DocumentOfficialDate { get; set; }//dt_document_official_date


        [StringLength(DBStringLength.StringLength255)]
        [Comment("Описание")]
        public string? Description { get; set; }//vc_desciption


        [Comment("Документа е валиден")]
        public bool IsValid { get; set; }//is_valid

        [Comment("Документа e след 2007 година")]
        public bool IsBeforeDate { get; set; }//bool_before_date



        [Comment("Квалификационно ниво")]
        public int? IdMinimumQualificationLevel { get; set; } // номенклатура: KeyTypeIntCode - "MinimumQualificationLevel"


        [Comment("Дата на издаване на документа")]
        public DateTime? IssueDocumentDate { get; set; }


        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string UploadedFileName { get; set; }


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


