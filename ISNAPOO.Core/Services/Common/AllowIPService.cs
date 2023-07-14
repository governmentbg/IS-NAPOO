using Data.Models.Common;
using Data.Models.Data.Common;
using Data.Models.Data.EGovPayment;
using Data.Models.Data.Training;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EGovPayment;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.Services.EGovPayment;
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
    public class AllowIPService : BaseService, IAllowIPService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        private readonly ILogger<PaymentService> _logger;
        private readonly IApplicationUserService applicationUserService;
        public AllowIPService(IRepository repository,
            IDataSourceService dataSourceService,
            ILogger<PaymentService> logger,
            IApplicationUserService applicationUserService,
            AuthenticationStateProvider authenticationStateProvider)
            : base(repository)
        {
            this.repository = repository;
            this.applicationUserService = applicationUserService;
            this.dataSourceService = dataSourceService;
            this._logger = logger;
            this.authenticationStateProvider = authenticationStateProvider;

            _logger.LogDebug("Стартиране на AllowIPService.......");
        }

        public async Task<IEnumerable<AllowIPVM>> GetAllAllowIPsAsync()
        { 

            var data = this.repository.AllReadonly<AllowIP>();
            var dataAsVM = await data.To<AllowIPVM>().ToListAsync();

            return dataAsVM;
        }
        public async Task<AllowIPVM> GetAllowIPAsync(int idAllowIP)
        {

            AllowIP data = await this.repository.GetByIdAsync<AllowIP>(idAllowIP);
            var result = data.To<AllowIPVM>();

            return result;
        }
        public async Task<ResultContext<AllowIPVM>> CreateAllowIPAsync(ResultContext<AllowIPVM> resultContext)
        {
            try
            {              
                resultContext.ResultContextObject.idAllowIP = 0;
                var entity = resultContext.ResultContextObject.To<AllowIP>();
                await this.repository.AddAsync(entity);
                var result = await this.repository.SaveChangesAsync();
                resultContext.ResultContextObject.idAllowIP = entity.idAllowIP;

                resultContext.AddMessage("Успешно добавено IP");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }
        public async Task<ResultContext<AllowIPVM>> UpdateAllowIPAsync(ResultContext<AllowIPVM> resultContext)
        {
            try
            {
               var entity = resultContext.ResultContextObject.To<AllowIP>();        
                this.repository.Update<AllowIP>(entity);
                var result = await this.repository.SaveChangesAsync();
              
                resultContext.AddMessage("Успешна промяна");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }
        public async Task<ResultContext<AllowIPVM>> DeleteAllowIPdByIdAsync(int idAllowIP)
        {
            var resultContext = new ResultContext<AllowIPVM>();
            try
            {
                        await this.repository.HardDeleteAsync<AllowIP>(idAllowIP);
                        await this.repository.SaveChangesAsync();

                        resultContext.AddMessage("Записът е изтрит успешно!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

    }
}
