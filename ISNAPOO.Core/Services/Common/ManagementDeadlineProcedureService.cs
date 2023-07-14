using Data.Models.Common;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{


    public class ManagementDeadlineProcedureService : BaseService, IManagementDeadlineProcedureService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;

        public ManagementDeadlineProcedureService(IRepository repository, IDataSourceService dataSourceService) : base(repository)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
        }

        public async Task<IEnumerable<ManagementDeadlineProcedureVM>> GetAllManagementDeadlineProceduresAsync()
        {
            var data = this.repository.All<ManagementDeadlineProcedure>();

            var dataVM = await data.To<ManagementDeadlineProcedureVM>().ToListAsync(); 

            foreach (var vm in  dataVM) 
            {
                vm.ApplicationStatusName = (await dataSourceService.GetKeyValueByIdAsync(vm.IdApplicationStatus)).Name;
                vm.LicensingTypeName = (await dataSourceService.GetKeyValueByIdAsync(vm.IdLicensingType)).Name;
                vm.TermAsStr = vm.Term.ToString();
            }

            return dataVM;
        }

        public async Task<ResultContext<ManagementDeadlineProcedureVM>> SaveManagementDeadlineProcedureAsync(ResultContext<ManagementDeadlineProcedureVM> resultContext)
        {
            if (resultContext.ResultContextObject.IdManagementDeadlineProcedure == GlobalConstants.INVALID_ID_ZERO)
            {
                resultContext = await CreateManagementDeadlineProcedureAsync(resultContext);
            }
            else
            {
                resultContext = await UpdateManagementDeadlineProcedureAsync(resultContext);
            }

            return resultContext;
        }

        private async Task<ResultContext<ManagementDeadlineProcedureVM>> CreateManagementDeadlineProcedureAsync(ResultContext<ManagementDeadlineProcedureVM> resultContext)
        {
            var managementDeadlineProcedure = resultContext.ResultContextObject.To<ManagementDeadlineProcedure>();



            await this.repository.AddAsync<ManagementDeadlineProcedure>(managementDeadlineProcedure);
            int result = await this.repository.SaveChangesAsync();

            if (result > 0)
            {
                resultContext.AddMessage("Записът e успешeн!");
                resultContext.ResultContextObject.IdManagementDeadlineProcedure = managementDeadlineProcedure.IdManagementDeadlineProcedure;
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }

            return resultContext;
        }

        private async Task<ResultContext<ManagementDeadlineProcedureVM>> UpdateManagementDeadlineProcedureAsync(ResultContext<ManagementDeadlineProcedureVM> resultContext)
        {
            try
            {
                var updatedEnity = await this.GetByIdAsync<ManagementDeadlineProcedure>(resultContext.ResultContextObject.IdManagementDeadlineProcedure);
                this.repository.Detach<ManagementDeadlineProcedure>(updatedEnity);

                updatedEnity = resultContext.ResultContextObject.To<ManagementDeadlineProcedure>();


                this.repository.Update(updatedEnity);
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    resultContext.AddMessage("Записът e успешeн!");
                }
                else
                {
                    resultContext.AddErrorMessage("Грешка при запис в базата!");
                }

                return resultContext;
            }
            catch (Exception еx)
            {
                return resultContext;
            }
        }
    }
}

