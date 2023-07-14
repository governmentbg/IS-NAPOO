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
    public class ProfessionOrderService : BaseService, IProfessionOrderService
    {
        private readonly IRepository repository;

        public ProfessionOrderService(IRepository repository)
            : base(repository)
        {
            this.repository = repository;
        }

        public async Task<IEnumerable<ProfessionOrderVM>> GetAllProfessionOrdersAsync()
        {
            IQueryable<ProfessionOrder> data = this.repository.AllReadonly<ProfessionOrder>();

            return await data.To<ProfessionOrderVM>(x => x.SPPOOOrder).OrderByDescending(x => x.SPPOOOrder.OrderDate).ToListAsync();
        }
    }
}
