using ISNAPOO.Core.ViewModels.EKATTE;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.EKATTE
{
    public interface IDistrictService : IBaseService
    {
        Task<IEnumerable<DistrictVM>> GetAllDistrictsAsync();

        Task<DistrictVM> GetDistrictByIdAsync(int id);

        Task<int> UpdateDistrictAsync(DistrictVM districtVM);

        Task<int> CreateDistrictAsync(DistrictVM districtVM);

        Task<int> DeleteDistrictAsync(int id);
    }
}
