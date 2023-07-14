using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Models.Data.ProviderData
{


    /// <summary>
    /// Данни за Процедура за лицензиране - прогрес
    /// </summary>
    [Table("Procedure_StartedProcedureProgress")]
    [Display(Name = " Данни за Процедура за лицензиране - прогрес")]
    public class StartedProcedureProgress : IEntity, IModifiable
    {
        public StartedProcedureProgress()
        {
        }

        [Key]
        public int IdStartedProcedureProgress { get; set; }
        public int IdEntity => IdStartedProcedureProgress;

        [Display(Name = "Връзка с  Данни за Процедура за лицензиране")]
        [ForeignKey(nameof(StartedProcedure))]
        public int IdStartedProcedure { get; set; }
        public StartedProcedure StartedProcedure { get; set; }

        /// <summary>
        ///Подадено заявление за лицензиране на нов център
        ///Одобрено заявление за лицензиране на нов център
        ///Отказано заявление за лицензиране на нов център
        ///Подготвяне на документация за лицензиране/изменение на център
        ///Процедурата е прекратена от центъра
        ///Документите са попълнени от кандидата и подадени към НАПОО
        ///Процедурата е заведена в деловодната система и е изпратена към водещ експерт за проверка на документацията
        ///Водещият експерт е дал положителна оценка
        ///Водещият експерт е дал отрицателна оценка
        ///Стартира лицензионна експертиза
        ///Приключване на процедурата - издаване на лицензия
        ///Отказ за издаване на лицензия
        /// </summary>
        [Display(Name = "Стъпки на процедура")]
        public int? IdStep { get; set; }

        [Display(Name = "Дата на стъпката")]
        public DateTime? StepDate { get; set; }

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
