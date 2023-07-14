using ISNAPOO.Core.ViewModels.EKATTE;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.EKATTE
{
    public interface IMunicipalityService : IBaseService
    {
        Task<IEnumerable<MunicipalityVM>> GetAllMunicipalitiesAsync();

        Task<IEnumerable<int>> GetAllMunicipalitiesWithRegionsAsync();

        Task<MunicipalityVM> GetMunicipalityByIdAsync(int id);

        Task<int> UpdateMunicipalityAsync(MunicipalityVM municipalityVM);

        Task<int> CreateMunicipalityAsync(MunicipalityVM municipalityVM);

        Task<int> DeleteMunicipalityAsync(int id);

        Task<IEnumerable<MunicipalityVM>> GetAllMunicipalitiesByDistrictIdAsync(int? id);
    }
}
