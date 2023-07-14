using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.CPO.ProviderData
{
    public class StartedProcedureProgressVM : IMapFrom<StartedProcedureProgress>, IMapTo<StartedProcedureProgress>
    {
        [Key]
        public int IdStartedProcedureProgress { get; set; }

        [Display(Name = "Връзка с  Данни за Процедура за лицензиране")]
        public int IdStartedProcedure { get; set; }
        public StartedProcedureVM StartedProcedure { get; set; }

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
