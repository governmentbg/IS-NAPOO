using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.SPPOO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.SPPOO
{
    public interface IFrameworkProgramService
    {
        Task<IEnumerable<FrameworkProgramVM>> GetAllFrameworkProgramsAsync(FrameworkProgramVM filter);

        Task<ResultContext<List<FrameworkProgramVM>>> GetAllFrameworkProgramsAsync(ResultContext<FrameworkProgramVM> callContext);

        Task<int> CreateFrameworkProgram(FrameworkProgramVM model);

        Task<int> UpdateFrameworkProgramAsync(FrameworkProgramVM model);

        Task<FrameworkProgramVM> GetFrameworkProgramByIdAsync(int id);

        Task RemoveFrameworkProgram(FrameworkProgramVM frameworkProgram);

        Task<IEnumerable<FrameworkProgramVM>> GetAllFrameworkProgramsBySpecialityVQSIdAsync(int specialityVQSId);
        Task<IEnumerable<FrameworkProgramVM>> GetAllFrameworkProgramsBySpecialityVQSIdAndIdTypeOfFrameworkAsync(int specialityVQSId, int idTypeOfFramework);

        Task<FrameworkProgramVM> GetFrameworkPgoramByIdWithFormEducationsIncludedAsync(FrameworkProgramVM frameworkProgramVM);
        List<FrameworkProgramVM> getAllFrameworkProgramsWithoutAsync();
    }
}
