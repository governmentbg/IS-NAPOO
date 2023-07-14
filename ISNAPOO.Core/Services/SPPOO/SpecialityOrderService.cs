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
    public class SpecialityOrderService : BaseService, ISpecialityOrderService
    {
        private readonly IRepository repository;

        public SpecialityOrderService(IRepository repository)
            : base(repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<SpecialityOrderVM>> GetAllSpecialityOrdersAsync()
        {
            IQueryable<SpecialityOrder> data = this.repository.AllReadonly<SpecialityOrder>();

            return await data.To<SpecialityOrderVM>(x => x.SPPOOOrder).OrderByDescending(x => x.SPPOOOrder.OrderDate).ToListAsync();
        }
    }
}
