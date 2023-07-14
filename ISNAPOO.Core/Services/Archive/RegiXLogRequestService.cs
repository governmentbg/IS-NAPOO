using Data.Models.Common;
using Data.Models.Data.Archive;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Archive;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Archive
{
    public class RegiXLogRequestService : BaseService, IRegiXLogRequestService
    {
        private readonly IRepository repository;
        private readonly ILogger<RegiXLogRequestService> _logger;

        public RegiXLogRequestService(IRepository repository, ILogger<RegiXLogRequestService> logger)
            : base (repository)
        {
            this.repository = repository;
            this._logger = logger;
        }

        public async Task CreateRegiXLogRequestAsync(ResultContext<RegiXLogRequestVM> inputContext)
        {
            var model = inputContext.ResultContextObject;

            try
            {
                var entryForDb = model.To<RegiXLogRequest>();

                await this.repository.AddAsync<RegiXLogRequest>(entryForDb);
                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }
    }
}
