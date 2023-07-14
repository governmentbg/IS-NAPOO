using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.ExternalExpertCommission
{
    public interface IExpertDocumentService : IBaseService
    {
        Task<int> CreateExpertDocumentAndUpload(ExpertDocumentVM model, MemoryStream file, string fileName);

        Task<IEnumerable<ExpertDocumentVM>> GetAllExpertDocumentsAsync(ExpertDocumentVM filterModel);

        Task<IEnumerable<ExpertDocumentVM>> GetAllExpertDocumentsAsync();

        Task<IEnumerable<ExpertDocumentVM>> GetExpertDocumentsByIdsAsync(List<int> ids);

        Task<ExpertDocumentVM> GetExpertDocumentByIdAsync(int id);

        Task<IEnumerable<ExpertDocumentVM>> GetExpertDocumentsByExpertIdAsync(int idExpert);

        Task<int> UpdateExpertDocumentAndUploadAsync(ExpertDocumentVM model, MemoryStream file, string fileName);

        Task<int> UpdateExpertDocumentAsync(ExpertDocumentVM model);

        Task<int> DeleteExpertDocumentAsync(ExpertDocumentVM model);
    }
}