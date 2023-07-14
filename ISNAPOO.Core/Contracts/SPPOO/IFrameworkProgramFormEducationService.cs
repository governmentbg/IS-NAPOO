using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.SPPOO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.SPPOO
{
    public interface IFrameworkProgramFormEducationService
    {
        Task<IEnumerable<FrameworkProgramFormEducationVM>> GetAllAsync();
        Task<IEnumerable<FrameworkProgramFormEducationVM>> GetAllFrameworkProgramsFormEducationsByIdAsync(int idFrameworkProgram);
        Task<int> CreateFrameworkProgramFormEducation(int idFramworkProgram, int[] formEducationList);
        Task<int> UpdateFrameworkProgramFormEducationAsync(int idFrameworkProgram, int[] formEducationIds);
        Task RemoveFrameworkProgramFormEducation(List<FrameworkProgramFormEducationVM> frameworkProgramsFormEducationsList);
    }
}
