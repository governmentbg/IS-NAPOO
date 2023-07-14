using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IManagementDeadlineProcedureService
    {
        Task<IEnumerable<ManagementDeadlineProcedureVM>> GetAllManagementDeadlineProceduresAsync();
        Task<ResultContext<ManagementDeadlineProcedureVM>> SaveManagementDeadlineProcedureAsync(ResultContext<ManagementDeadlineProcedureVM> resultContext);
    }
}
