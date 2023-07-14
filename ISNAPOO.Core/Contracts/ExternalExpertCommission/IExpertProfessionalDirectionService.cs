using Data.Models.Data.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.ExternalExpertCommission
{
    public interface IExpertProfessionalDirectionService : IBaseService
    {
        Task<int> CreateExpertProfessionalDirection(ExpertProfessionalDirectionVM model);

        Task<IEnumerable<ExpertProfessionalDirectionVM>> GetAllExpertProfessionalDirectionsAsync(ExpertProfessionalDirectionVM filterModel);

        Task<IEnumerable<ExpertProfessionalDirectionVM>> GetAllExpertProfessionalDirectionsAsync();

        Task<IEnumerable<ExpertProfessionalDirectionVM>> GetExpertProfessionalDirectionsByIdsAsync(List<int> ids);

        Task<ExpertProfessionalDirectionVM> GetExpertProfessionalDirectionByIdAsync(int id);

        Task<IEnumerable<ExpertProfessionalDirectionVM>> GetExpertProfessionalDirectionsByExpertIdAsync(int idExpert);

        Task<int> UpdateExpertProfessionalDirectionAsync(ExpertProfessionalDirectionVM model);

        Task<int> DeleteExpertProfessionalDirectionAsync(ExpertProfessionalDirectionVM model);
    }
}
