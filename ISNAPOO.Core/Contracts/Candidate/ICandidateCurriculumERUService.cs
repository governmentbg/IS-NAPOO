using Data.Models.Data.Candidate;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.DOC;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Candidate
{
    public interface ICandidateCurriculumERUService : IBaseService
    {
        Task<IEnumerable<CandidateCurriculumERUVM>> GetAllCandidateCurriculumERUByCandidateCurriculumIdAsync(CandidateCurriculumVM candidateCurriculum);

        CandidateCurriculumERUVM GetCandidateCurriculumERUByIdCandidateCurriculumAndIdERU(int idCandidateCurriculum, int idEru);

        Task<ResultContext<NoResult>> DeleteCandidateCurriculumERUAsync(int idCandidateCurriculumERU);

        Task<ResultContext<NoResult>> AddERUsToCurriculumListAsync(List<ERUVM> erus, List<CandidateCurriculumVM> curriculums);
    }
}
