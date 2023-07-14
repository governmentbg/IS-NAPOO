using ISNAPOO.Core.ViewModels.SPPOO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.SPPOO
{
    public interface ILegalCapacityOrdinanceService : IBaseService
    {
        Task<int> CreateOrdinance(LegalCapacityOrdinanceUploadedFileVM model);

        Task<IEnumerable<LegalCapacityOrdinanceUploadedFileVM>> GetAllOrdinancesAsync();

        Task<LegalCapacityOrdinanceUploadedFileVM> GetOrdinanceByIdAsync(int id);

        Task<int> UpdateOrdinanceAsync(LegalCapacityOrdinanceUploadedFileVM model);

        Task<int> DeleteOrdinanceAsync(LegalCapacityOrdinanceUploadedFileVM model);

    }
}
