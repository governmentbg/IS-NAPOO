using ISNAPOO.Core.ViewModels.SPPOO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.SPPOO
{
    public interface IProfessionalDirectionService : IBaseService
    {
        Task<ProfessionalDirectionVM> GetProfessionalDirectionByIdAsync(ProfessionalDirectionVM professionalDirectionVM);

        Task<string> UpdateProfessionalDirectionAsync(ProfessionalDirectionVM proffesionalDirectionVM);

        Task<string> CreateProfessionalDirectionAsync(ProfessionalDirectionVM proffesionalDirectionVM);

        Task<IEnumerable<ProfessionalDirectionVM>> GetAllProfessionalDirectionsAsync(ProfessionalDirectionVM model);
        Task<IEnumerable<ProfessionalDirectionVM>> GetCIPOProfessionalDirectionsAsync(ProfessionalDirectionVM modelFilter);

        Task<string> DeleteProfessionalDirectionAsync(int id);

        Task<IEnumerable<ProfessionalDirectionVM>> GetAllActiveProfessionalDirectionsAsync();

        Task<IEnumerable<ProfessionalDirectionVM>> GetAllProfessionalDirectionsByCandidateProviderIdAsync(int idCandidateProvider);

        Task<IEnumerable<ProfessionalDirectionVM>> GetProfessionalDirectionsByIdsAsync(List<int> ids);
    }
}
