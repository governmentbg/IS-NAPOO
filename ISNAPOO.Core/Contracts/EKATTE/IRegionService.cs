using ISNAPOO.Core.ViewModels.EKATTE;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.EKATTE
{
    public interface IRegionService : IBaseService
    {
        Task<IEnumerable<RegionVM>> GetAllRegionsAsync();

        Task<RegionVM> GetRegionByIdAsync(int id);

        Task<IEnumerable<RegionVM>> GetAllRegionsByIdMunicipalityAsync(int idMunicipality);

        Task<int> UpdateRegionAsync(RegionVM regionVM);

        Task<int> CreateRegionAsync(RegionVM regionVM);

        Task<int> DeleteRegionAsync(int id);
    }
}
