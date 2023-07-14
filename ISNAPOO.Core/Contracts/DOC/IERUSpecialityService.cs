using ISNAPOO.Core.ViewModels.DOC;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.DOC
{
    public interface IERUSpecialityService : IBaseService
    {
        Task<IEnumerable<ERUSpecialityVM>> GetAllERUsBySpecialityIdAsync(ERUSpecialityVM eruSpeciality);
    }
}
