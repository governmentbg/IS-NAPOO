using ISNAPOO.Core.ViewModels.SPPOO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.SPPOO
{
    public interface ISpecialityService : IBaseService
    {
        Task<IEnumerable<SpecialityVM>> GetAllSpecialitiesAsync(SpecialityVM specialityVM);

        Task<List<SpecialityVM>> GetSpecialitiesByDocIdAsync(int idDoc);
        Task<List<SpecialityVM>> GetSpecialitiesByERUIdAsync(int idERU);
        Task<List<SpecialityVM>> GetAllSpecialitiesIncludeAsync(SpecialityVM specialityVM);

        IEnumerable<SpecialityVM> GetAllSpecialities(SpecialityVM specialityVM);

        Task<SpecialityVM> GetSpecialityByIdAsync(SpecialityVM specialityVM);

        Task<SpecialityVM> GetSpecialityByIdAsync(int id);

        Task<string> UpdateSpecialityAsync(SpecialityVM specialityVM);

        Task<string> CreateSpecialityAsync(SpecialityVM specialityVM);

        Task<string> DeleteSpecialityAsync(int id);

        Task<IEnumerable<SpecialityVM>> GetAllActiveSpecialitiesByProfessionIdAsync(ProfessionVM professionVM);

        Task UpdateSpecialityOrdersAsync(SpecialityVM specialityVM);

        Task<IEnumerable<SpecialityVM>> GetAllActiveSpecialitiesAsync();

        Task<IEnumerable<SpecialityVM>> GetSpecialitiesByListIdsAsync(List<int> ids);

        SpecialityVM GetSpecialityById(int id);

        List<int> GetSpecialitiesIdsByIdArea(int idArea);

        List<int> GetSpecialitiesIdsByIdProfession(int idProfession);

        List<int> GetSpecialitiesIdsByIdProfessionalDirection(int idProfessionalDirection);

        List<int> GetSpecialitiesIdsByIdDOC(int idDOC);

        Task<SpecialityVM> GetSpecialityWithProfessionIncludedByIdSpecialityAsync(int idSpeciality);
    }
}
