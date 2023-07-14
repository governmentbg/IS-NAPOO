using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.EGovPayment;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.Services.Archive;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.EGovPayment;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class EventLogService : BaseService, IEventLogService
    {
        private readonly IRepository repository;
        private readonly ILogger<ArchiveService> _logger;
        private readonly IDataSourceService dataSourceService;
        public EventLogService(
            IRepository repository,
            ILogger<ArchiveService> logger,
            IDataSourceService dataSourceService) : base(repository)       
        {
            this.repository = repository;
            this._logger = logger;
            this.dataSourceService = dataSourceService;
        }

        public async Task<List<EventLogVM>> GetAllEventLogsAsync()
        {
         
            var dateOneHourBefore = DateTime.Now.AddHours(-1);
            var data = this.repository.AllReadonly<EventLog>(x => x.EventDate >= dateOneHourBefore).OrderByDescending(x=>x.EventDate);
            var dataAsVM = await data.To<EventLogVM>().ToListAsync();

            return dataAsVM;
        }

        public async Task<List<EventLogVM>> GetEventLogsFromToDatePersonNameIPAsync(EventLogListFilterVM eventLogListFilterVM)
        {
            
            IQueryable<EventLog> eventLog = Enumerable.Empty<EventLog>().AsQueryable(); ;
            if (string.IsNullOrEmpty(eventLogListFilterVM.IP) && string.IsNullOrEmpty(eventLogListFilterVM.PersonName))
            {
                eventLog = this.repository.AllReadonly<EventLog>(x => x.EventDate >= eventLogListFilterVM.EventLogsFrom 
                && x.EventDate <= eventLogListFilterVM.EventLogsTo).OrderByDescending(x => x.EventDate);

            }
            else if (!string.IsNullOrEmpty(eventLogListFilterVM.IP) && string.IsNullOrEmpty(eventLogListFilterVM.PersonName))
            {
                eventLog = this.repository.AllReadonly<EventLog>(x => x.EventDate >= eventLogListFilterVM.EventLogsFrom 
                && x.EventDate <= eventLogListFilterVM.EventLogsTo 
                && x.IP.StartsWith(eventLogListFilterVM.IP)).OrderByDescending(x => x.EventDate);

            }
            else if (string.IsNullOrEmpty(eventLogListFilterVM.IP) && !string.IsNullOrEmpty(eventLogListFilterVM.PersonName))
            {
                eventLog = this.repository.AllReadonly<EventLog>(x => x.EventDate >= eventLogListFilterVM.EventLogsFrom
                && x.EventDate <= eventLogListFilterVM.EventLogsTo
                && x.PersonName.StartsWith(eventLogListFilterVM.PersonName)).OrderByDescending(x => x.EventDate);

            }
            else if (!string.IsNullOrEmpty(eventLogListFilterVM.IP) && !string.IsNullOrEmpty(eventLogListFilterVM.PersonName))
            {
                eventLog = this.repository.AllReadonly<EventLog>(x => x.EventDate >= eventLogListFilterVM.EventLogsFrom
                && x.EventDate <= eventLogListFilterVM.EventLogsTo
                && x.PersonName.StartsWith(eventLogListFilterVM.PersonName)
                && x.IP.StartsWith(eventLogListFilterVM.IP)).OrderByDescending(x => x.EventDate);

            }
            var dataAsVM = await eventLog.To<EventLogVM>().ToListAsync();

            return dataAsVM;
        }
    }
}
