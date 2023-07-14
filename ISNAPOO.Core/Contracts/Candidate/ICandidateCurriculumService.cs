using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Candidate
{
    public interface ICandidateCurriculumService : IBaseService
    {
        Task<ResultContext<List<CandidateCurriculumVM>>> ImportCurriculumAsync(MemoryStream file, string fileName, int idCandidateCurriculumModification);

        MemoryStream CreateExcelWithErrors(ResultContext<List<CandidateCurriculumVM>> resultContext);

        Task<IEnumerable<CandidateCurriculumVM>> GetAllCandidateCurriculumsAsync();

        Task<ResultContext<CandidateCurriculumVM>> AddCandidateCurriculumAsync(ResultContext<CandidateCurriculumVM> inputContext, bool ignoreErus = false, bool callFromImportModal = false);

        Task<ResultContext<CandidateCurriculumVM>> UpdateCandidateCurriculumAsync(ResultContext<CandidateCurriculumVM> inputContext);

        Task<ResultContext<NoResult>> DeleteCandidateCurriculumAsync(int idCandidateCurriculum);

        Task<IEnumerable<CandidateCurriculumVM>> GetAllCurriculumsByIdCandidateProviderSpecialityAsync(int idCandidateProviderSpeciality);

        Task<ResultContext<NoResult>> DeleteListCandidateCurriculumAsync(List<CandidateCurriculumVM> candidateCurriculums);
    }
}
