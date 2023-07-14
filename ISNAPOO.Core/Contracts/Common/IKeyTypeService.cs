using ISNAPOO.Core.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IKeyTypeService
    {
        Task<string> CreateKeyType(KeyTypeVM model);

        Task<IEnumerable<KeyTypeVM>> GetAllKeyTypesAsync();
        Task<IEnumerable<KeyTypeVM>> GetAllKeyTypesIncludeKeyValuesAsync();

        Task<string> UpdateKeyTypeAsync(KeyTypeVM model);

        Task<string> DeleteKeyTypeAsync(KeyTypeVM model);
    }
}
