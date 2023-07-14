using ISNAPOO.Core.ViewModels.SPPOO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.SPPOO
{
    public interface IProfessionOrderService : IBaseService
    {
        Task<IEnumerable<ProfessionOrderVM>> GetAllProfessionOrdersAsync();
    }
}
