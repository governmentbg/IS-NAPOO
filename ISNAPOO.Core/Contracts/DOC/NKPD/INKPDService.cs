using ISNAPOO.Core.ViewModels.DOC.NKPD;
using System.Collections.Generic;
using System.Threading.Tasks;
using ISNAPOO.Common.HelperClasses;

namespace ISNAPOO.Core.Contracts.DOC.NKPD
{
    public interface INKPDService : IBaseService
    {
        Task<IEnumerable<NKPDVM>> GetAllNKPDAsync();
        Task<IEnumerable<NKPDVM>> GetAllNKPDOnlyAsync();

        Task<int> CreateNKPDAsync(NKPDVM nKPDVM);

        Task<int> DeleteNKPDAsync(int id);

        Task<NKPDVM> GetNKPDByIdAsync(int id);

        Task<IEnumerable<NKPDVM>> GetNKPDsByIdsAsync(List<int> ids);

        Task<NKPDVM> GetNKPDByCodeAsync(string code);
        Task<List<NKPDTreeGridData>> LoadNKPDDataAsync();
        Task UpdateNKPD(NKPDVM nKPDVM);
        Task<List<string>> UpdateNKPDTableAsync();
    }
}
