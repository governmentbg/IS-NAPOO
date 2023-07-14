using ISNAPOO.Core.ViewModels.SPPOO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.SPPOO
{
    public interface ISpecialityOrderService : IBaseService
    {
        Task<IEnumerable<SpecialityOrderVM>> GetAllSpecialityOrdersAsync();
    }
}
