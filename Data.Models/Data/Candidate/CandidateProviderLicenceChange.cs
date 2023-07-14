using Data.Models.Data.Framework;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Models.Data.ProviderData;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace Data.Models.Data.Candidate
{


    /// <summary>
    /// Смяна на статуса на лицензия на ЦПО/ЦИПО
    /// </summary>
    [Table("Candidate_ProviderLicenceChange")]
    [Display(Name = " Данни за установени непълноти и неточности")]
    public class CandidateProviderLicenceChange : IEntity, IModifiable
    {


        [Key]
        public int IdCandidateProviderLicenceChange { get; set; }
        public int IdEntity => IdCandidateProviderLicenceChange;

        [Display(Name = "Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }


        /// <summary>
        /// Номенклатура Смяна на лицензията /ProviderLicenceChange 
        /// </summary>
        [Comment("Смяна на лицензията/Статус")]//Стара таблица code_licence_status със стойности: активна, заличена,окончателно отнета, временно отнета за срок от 6 месеца, временно отнета за срок от 3 месеца,прекратена,временно отнета за срок от 4 месеца,
        public int IdStatus { get; set; }

        [Comment("Дата на промяна")]
        public DateTime ChangeDate { get; set; }


        [Comment("Номер на заповедта")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Номер на заповедта' не може да съдържа повече от 255 символа.")]
        public string? NumberCommand { get; set; }


        [Comment("Бележки")]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Бележки' не може да съдържа повече от 512 символа.")]
        public string? Notes { get; set; }


        /// <summary>
        /// Номенклатура Вид на заявлението/TypeApplication
        /// Първоначално лицензиране на център за професионално обучение,
        /// Процедура за изменение на лицензията на Център за професионално обучение, съгласно чл. 22, ал. 7 от Закона за професионалното образование и обучение
        /// Процедура за изменение на наименованията, кодовете и степените на професионална квалификация на професии и специалности във връзка с изменения в списъка на професиите за професионално образование и обучение
        /// Процедура за вписване на промени в обстоятелствата по чл. 49а, ал. 4, т. 1 и т. 2 от Закона за професионално образование и обучение
        /// 
        /// </summary>
        [Comment("Детайли при смяна на лицензията/Статус")]
        public int? IdLicenceStatusDetail { get; set; }// Стара таблица code_licence_status_detail със стойности: първоначално лицензиран, изменение на лицензията, вписване на промени, възстановена

        [StringLength(DBStringLength.StringLength4000)]
        [Comment("Информация къде се съхранява архивът на ЦПО/ЦИПО при отнемане на лицензия")]
        public string? Archive { get; set; }


        
        [Display(Name = "ID на работен документ в деловодната система на НАПОО")]
        public int? DS_ID { get; set; }

        [Display(Name = "Дата на работен документ в деловодната система на НАПОО")]
        public DateTime? DS_DATE { get; set; }

        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "GUID на работен документ в деловодната система на НАПОО")]
        public string? DS_GUID { get; set; }

        [StringLength(DBStringLength.StringLength20)]
        [Display(Name = "Номер на на работен документ в деловодната система на НАПОО")]
        public string? DS_DocNumber { get; set; }

        [Display(Name = "Официален номер на документ в деловодната система на НАПОО")]
        public int? DS_OFFICIAL_ID { get; set; }

        [Display(Name = "Дата на официален документ в деловодната система на НАПОО")]
        public DateTime? DS_OFFICIAL_DATE { get; set; }

        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "GUID на официален документ в деловодната система на НАПОО")]
        public string? DS_OFFICIAL_GUID { get; set; }

        [StringLength(DBStringLength.StringLength20)]
        [Display(Name = "Номер на официален документ в деловодната система на НАПОО")]
        public string? DS_OFFICIAL_DocNumber { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Преписка в деловодната система на НАПОО")]
        public string? DS_PREP { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Връзка към документа  в деловодната система на НАПОО")]
        public string? DS_LINK { get; set; }

        [DefaultValue(false)]
        [Display(Name = "Стойност дали документа е взет от деловодната система на НАПОО")]
        public bool IsFromDS { get; set; }

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


