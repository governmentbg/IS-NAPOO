using ISNAPOO.Core.ViewModels.EKATTE;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.EKATTE
{
    public interface ILocationService : IBaseService
    {
        Task<IEnumerable<LocationVM>> GetAllLocationsAsync();

        Task<LocationVM> GetLocationByIdAsync(int id);

        Task<LocationVM> GetLocationByLocationIdAsync(int? id);

        Task<int> UpdateLocationAsync(LocationVM locationVM);

        Task<int> CreateLocationAsync(LocationVM locationVM);

        Task<int> DeleteLocationAsync(int id);

        Task<IEnumerable<LocationVM>> GetAllLocationsByMunicipalityIdAsync(int? id);

        Task<IEnumerable<LocationVM>> GetAllLocationsJoinedAsync(LocationVM locationVM);

        Task<IEnumerable<LocationVM>> GetAllLocationsAsync(LocationVM filterModel);

        Task<IEnumerable<LocationVM>> GetAllLocationsByKatiAsync(string kati);

        Task<LocationVM> GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(int id);

        Task<IEnumerable<LocationVM>> GetAllLocationsByDistrictIdAsync(int idDistrict);
    }
}
