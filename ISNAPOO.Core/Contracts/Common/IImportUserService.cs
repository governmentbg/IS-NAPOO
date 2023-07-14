using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Common;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IImportUserService : IBaseService
    {
        Task<ResultContext<List<PersonVM>>> ImportUsersAsync(MemoryStream file, string fileName, int idImportType);
        Task<ResultContext<List<PersonVM>>> ImportUsersAndLinkToCandidateProviderAsync(MemoryStream file, string fileName, int idImportType);

        MemoryStream CreateExcelWithErrors(ResultContext<List<PersonVM>> resultContext);

        Task<(MemoryStream? MS, bool IsSuccessfull)> CreateUsersAsync(List<PersonVM> persons, ImportUsersVM model);
        Task<(MemoryStream? MS, bool IsSuccessfull)> CreateUsersForCandidateProvidersAsync(List<PersonVM> persons, ImportUsersVM model);

        Task CopyCoursesAsync();
    }
}
