using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EGovPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IEventLogService : IBaseService
    {
        Task<List<EventLogVM>> GetAllEventLogsAsync();
        Task<List<EventLogVM>> GetEventLogsFromToDatePersonNameIPAsync(EventLogListFilterVM eventLogListFilterVM);

    }
}
