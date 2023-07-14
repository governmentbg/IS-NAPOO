using Data.Models.Common;
using Data.Models.Data.ExternalExpertCommission;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.ExternalExpertCommission
{
    public class ExpertProfessionalDirectionService : BaseService, IExpertProfessionalDirectionService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        private readonly IExpertService expertService;

        public ExpertProfessionalDirectionService(IRepository repository, IDataSourceService dataSourceService, IExpertService expertService) : base(repository)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
            this.expertService = expertService;
        }

        public async Task<ExpertProfessionalDirectionVM> GetExpertProfessionalDirectionByIdAsync(int id)
        {
            ExpertProfessionalDirection expertProfessionalDirection = await this.repository.GetByIdAsync<ExpertProfessionalDirection>(id);
            this.repository.Detach<ExpertProfessionalDirection>(expertProfessionalDirection);

            return expertProfessionalDirection.To<ExpertProfessionalDirectionVM>();
        }

        public async Task<IEnumerable<ExpertProfessionalDirectionVM>> GetExpertProfessionalDirectionsByIdsAsync(List<int> ids)
        {
            IQueryable<ExpertProfessionalDirection> expertProfessionalDirections = this.repository.All<ExpertProfessionalDirection>(this.FilterExpertProfessionalDirectionByIds(ids))
                                                                                                  .Include(epd => epd.ProfessionalDirection);

            var dataVM = await expertProfessionalDirections.To<ExpertProfessionalDirectionVM>(epd => epd.ProfessionalDirection).ToListAsync();

            var dataKvExpertType = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertType");
            var dataKvExpertStatus = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertStatus");

            foreach (ExpertProfessionalDirectionVM expertProfessionalDirectionVM in dataVM)
            {
                var kvExpertType = dataKvExpertType.Where(w => w.IdKeyValue == expertProfessionalDirectionVM.IdExpertType).FirstOrDefault();
                if (kvExpertType != null)
                {
                    expertProfessionalDirectionVM.ExpertTypeName = kvExpertType.Name;
                }
                var kvStatus = dataKvExpertStatus.Where(w => w.IdKeyValue == expertProfessionalDirectionVM.IdStatus).FirstOrDefault();
                if (kvStatus != null)
                {
                    expertProfessionalDirectionVM.StatusName = kvStatus.Name;
                }
            }

            return dataVM;
        }

        public async Task<IEnumerable<ExpertProfessionalDirectionVM>> GetExpertProfessionalDirectionsByExpertIdAsync(int idExpert)
        {
            //var ExpertProfessionalDirection =  this.repository.All<ExpertProfessionalDirection>();

            IQueryable<ExpertProfessionalDirection> expertProfessionalDirections = this.repository.All<ExpertProfessionalDirection>(epd => epd.IdExpert == idExpert)
                                                                                                  .Include(epd => epd.ProfessionalDirection);

            var dataVM = await expertProfessionalDirections.To<ExpertProfessionalDirectionVM>(epd => epd.ProfessionalDirection).ToListAsync();

            var dataKvExpertType = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertType");
            var dataKvExpertStatus = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertStatus");

            foreach (ExpertProfessionalDirectionVM expertProfessionalDirectionVM in dataVM)
            {
                var kvExpertType = dataKvExpertType.Where(w => w.IdKeyValue == expertProfessionalDirectionVM.IdExpertType).FirstOrDefault();
                if (kvExpertType != null)
                {
                    expertProfessionalDirectionVM.ExpertTypeName = kvExpertType.Name;
                }
                var kvStatus = dataKvExpertStatus.Where(w => w.IdKeyValue == expertProfessionalDirectionVM.IdStatus).FirstOrDefault();
                if (kvStatus != null)
                {
                    expertProfessionalDirectionVM.StatusName = kvStatus.Name;
                }
            }

            return dataVM;
        }

        public async Task<int> CreateExpertProfessionalDirection(ExpertProfessionalDirectionVM model)
        {
            var newExpertProfessionalDirection = model.To<ExpertProfessionalDirection>();

      

            await this.repository.AddAsync<ExpertProfessionalDirection>(newExpertProfessionalDirection);
            var result = await this.repository.SaveChangesAsync();

            return result;
        }

        public async Task<IEnumerable<ExpertProfessionalDirectionVM>> GetAllExpertProfessionalDirectionsAsync()
        {
            var data = this.repository.All<ExpertProfessionalDirection>();

            return await data.To<ExpertProfessionalDirectionVM>().ToListAsync();
        }

        public async Task<IEnumerable<ExpertProfessionalDirectionVM>> GetAllExpertProfessionalDirectionsAsync(ExpertProfessionalDirectionVM modelFilter)
        {
            var data = this.repository.All<ExpertProfessionalDirection>(this.FilterExpertProfessionalDirectionValue(modelFilter))
                                      .Include(epd => epd.ProfessionalDirection);

            var dataVM = await data.To<ExpertProfessionalDirectionVM>(epd => epd.ProfessionalDirection).ToListAsync();

            var dataKvExpertType = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertType");
            var dataKvExpertStatus = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertStatus");

            foreach (ExpertProfessionalDirectionVM expertProfessionalDirectionVM in dataVM)
            {
                var kvExpertType = dataKvExpertType.Where(w => w.IdKeyValue == expertProfessionalDirectionVM.IdExpertType).FirstOrDefault();
                if (kvExpertType != null)
                {
                    expertProfessionalDirectionVM.ExpertTypeName = kvExpertType.Name;
                }
                var kvStatus = dataKvExpertStatus.Where(w => w.IdKeyValue == expertProfessionalDirectionVM.IdStatus).FirstOrDefault();
                if (kvStatus != null)
                {
                    expertProfessionalDirectionVM.StatusName = kvStatus.Name;
                }
            }

            return dataVM;
        }

        protected Expression<Func<ExpertProfessionalDirection, bool>> FilterExpertProfessionalDirectionValue(ExpertProfessionalDirectionVM modelFilter)
        {
            var predicate = PredicateBuilder.True<ExpertProfessionalDirection>();

            if (modelFilter.IdExpertType != GlobalConstants.INVALID_ID
                && modelFilter.IdExpertType != GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdExpertType == modelFilter.IdExpertType);
            }
            if (modelFilter.IdExpertProfessionalDirection != GlobalConstants.INVALID_ID
                && modelFilter.IdExpertProfessionalDirection != GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdExpertProfessionalDirection == modelFilter.IdExpertProfessionalDirection);
            }
            if (modelFilter.IdStatus != GlobalConstants.INVALID_ID
                && modelFilter.IdStatus != GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdStatus == modelFilter.IdStatus);
            }
            if (!string.IsNullOrEmpty(modelFilter.Comment))
            {
                predicate = predicate.And(p => p.Comment != null && p.Comment.Contains(modelFilter.Comment));
            }
            if (modelFilter.DateApprovalExternalExpert.HasValue)
            {
                predicate = predicate.And(p => p.DateApprovalExternalExpert.HasValue && p.DateApprovalExternalExpert.Value == modelFilter.DateApprovalExternalExpert.Value);
            }
            if (!string.IsNullOrEmpty(modelFilter.OrderNumber))
            {
                predicate = predicate.And(p => p.OrderNumber != null && p.OrderNumber.Contains(modelFilter.OrderNumber));
            }
            if (modelFilter.DateOrderIncludedExpertCommission.HasValue)
            {
                predicate = predicate.And(p => p.DateOrderIncludedExpertCommission.HasValue && p.DateOrderIncludedExpertCommission.Value == modelFilter.DateOrderIncludedExpertCommission.Value);
            }

            return predicate;
        }

        protected Expression<Func<ExpertProfessionalDirection, bool>> FilterExpertProfessionalDirectionByIds(List<int> ids)
        {
            var predicate = PredicateBuilder.True<ExpertProfessionalDirection>();

            predicate = predicate.And(n => ids.Contains(n.IdExpertProfessionalDirection));

            return predicate;
        }

        public async Task<int> UpdateExpertProfessionalDirectionAsync(ExpertProfessionalDirectionVM model)
        {
            int result = GlobalConstants.INVALID_ID_ZERO;
            var context = new ResultContext<ExpertProfessionalDirectionVM>();

            if (model.IdExpertProfessionalDirection == 0)
            {
                result = await this.CreateExpertProfessionalDirection(model);
            }
            else
            {
                var updatedEnity = await this.GetByIdAsync<ExpertProfessionalDirection>(model.IdExpertProfessionalDirection);
                this.repository.Detach<ExpertProfessionalDirection>(updatedEnity);

                updatedEnity = model.To<ExpertProfessionalDirection>();
 

                this.repository.Update(updatedEnity);
                result = await this.repository.SaveChangesAsync();
            }
            if (result > 0)
            {
                await this.expertService.ChangeExpertExternalOrCommissionAsync(model.IdExpert);
            }

            return result;
        }

        public async Task<int> DeleteExpertProfessionalDirectionAsync(ExpertProfessionalDirectionVM modelToDelete)
        {
            
                await this.repository.HardDeleteAsync<ExpertProfessionalDirection>(modelToDelete.IdExpertProfessionalDirection);
            

            return await this.repository.SaveChangesAsync();
        }
    }
}
