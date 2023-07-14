using ISNAPOO.Common.HelperClasses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IConcurrencyService : IBaseService
    {
        List<ConcurrencyInfo> GetAllCurrentlyOpenedModals();

        void RemoveEntityIdAsCurrentlyOpened(int idEntity, string type);

        Task AddEntityIdAsCurrentlyOpened(int idEntity, string type);
    }
}
