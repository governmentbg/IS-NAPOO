using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.CPO.ProviderData
{
    public class ProcedureExpertCommissionVM : IMapFrom<ProcedureExpertCommission>, IMapTo<ProcedureExpertCommission>
    {
        [Key]
        public int IdProcedureExpertCommission { get; set; }

        [Display(Name = "Връзка с  Данни за Процедура за лицензиране")]
        public int IdStartedProcedure { get; set; }
        public StartedProcedureVM StartedProcedure { get; set; }


        [Display(Name = "Връзка с  номенклатура Eкспертни комисии")]
        public int IdExpertCommission { get; set; }//[KeyTypeIntCode] = ExpertCommission
    }
}
