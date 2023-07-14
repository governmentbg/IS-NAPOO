using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.ExternalExpertCommission
{
    public interface IExpertService : IBaseService
    {
        
        Task<IEnumerable<ExpertVM>> GetAllExpertsAsync(ExpertVM filterExpertVM);
        Task<IEnumerable<ExpertVM>> GetAllExpertsByIdProfessionalDirectionAsync(ExpertVM filterExpertVM);
        Task<IEnumerable<ExpertDOCVM>> GetAllExpertExpertDOCsAsync(int expertId);
        Task<IEnumerable<ExpertNapooVM>> GetAllExpertsNAPOOAsync(int expertId);
        Task<IEnumerable<ExpertExpertCommissionVM>> GetAllExpertExpertCommissionsAsync(ExpertExpertCommissionVM filterVM);
        Task<ExpertVM> GetExpertByIdAsync(int id);
        Task<ExpertVM> GetExpertByIdPersonAsync(int id);
        Task<ExpertDOCVM> GetExpertDOCByIdAsync(int id);
        Task<ExpertNapooVM> GetExpertNAPOOByIdAsync(int id);
        Task<ExpertExpertCommissionVM> GetExpertExpertCommissionByIdAsync(int id);
        Task<IEnumerable<KeyValueVM>> GetExpertExpertCommissionByExpertIdAsync(int id);
        Task<ResultContext<PersonVM>> UpdateExpertAsync(ResultContext<PersonVM> resultContext);
        Task<ResultContext<PersonVM>> UpdatePersonEmailSentDateAsync(ResultContext<PersonVM> resultContext);
        Task<int> UpdateExpertDOCAsync(ExpertDOCVM model);
        Task<ResultContext<ExpertNapooVM>> UpdateExpertNAPOOAsync(ExpertNapooVM model);
        Task<ResultContext<ExpertExpertCommissionVM>> SaveExpertExpertCommissionAsync(ResultContext<ExpertExpertCommissionVM> resultContext);
        Task ChangeExpertExternalOrCommissionAsync(int idExpert);
        Task ChangeExpertDOCAsync(int idExpert);
        Task<int> DeleteExpertAsync(List<ExpertVM> models);

        Task<IEnumerable<ExpertVM>> GetAllNAPOOExpertsWithPersonIncludedAsync();
    }
}
