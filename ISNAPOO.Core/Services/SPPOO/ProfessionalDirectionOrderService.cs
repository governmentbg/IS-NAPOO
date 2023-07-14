using Data.Models.Common;
using Data.Models.Data.SPPOO;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.SPPOO
{
    public class ProfessionalDirectionOrderService : BaseService, IProfessionalDirectionOrderService
    {
        private readonly IRepository repository;

        public ProfessionalDirectionOrderService(IRepository repository)
            : base(repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<ProfessionalDirectionOrderVM>> GetAllProfessionalDirectionOrdersAsync()
        {
            IQueryable<ProfessionalDirectionOrder> data = this.repository.AllReadonly<ProfessionalDirectionOrder>();

            return await data.To<ProfessionalDirectionOrderVM>(x => x.SPPOOOrder).OrderByDescending(x => x.SPPOOOrder.OrderDate).ToListAsync();
        }
    }
}
