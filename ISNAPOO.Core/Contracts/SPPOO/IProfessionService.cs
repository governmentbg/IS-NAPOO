using ISNAPOO.Core.ViewModels.SPPOO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.SPPOO
{
    public interface IProfessionService : IBaseService
    {
        Task<string> CreateProfessionAsync(ProfessionVM professionVM);

        Task<string> DeleteProfessionAsync(int id);

        Task<ProfessionVM> GetProfessionByIdAsync(ProfessionVM professionVM);

        Task<string> UpdateProfessionAsync(ProfessionVM professionVM);

        Task<IEnumerable<ProfessionVM>> GetAllAsync(ProfessionVM professionVM);

        Task<IEnumerable<ProfessionVM>> GetAllAsync();

        IEnumerable<ProfessionVM> GetAll(ProfessionVM professionVM);

        Task<IEnumerable<ProfessionalDirectionVM>> GetAllProfessionalDirections();

        Task<IEnumerable<ProfessionalDirectionVM>> GetAllActiveProfessionalDirectionsAsync();

        Task<IEnumerable<ProfessionVM>> GetAllActiveProfessionsByProfessionalDirectionIdAsync(ProfessionalDirectionVM professionalDirectionVM);

        Task UpdateProfessionOrdersAsync(ProfessionVM professionVM);

        Task<IEnumerable<ProfessionVM>> GetAllActiveProfessionsAsync();

        Task<IEnumerable<ProfessionVM>> GetProfessionsByIdsAsync(List<int> ids);
        Task<ProfessionVM> GetOnlyProfessionByIdAsync(int idProfession);
    }
}
