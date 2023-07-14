using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IKeyValueService
    {
        //Task<KeyValueVM> GetKeyValueByIdAsync(int id);

        Task<IEnumerable<KeyValueVM>> GetAllAsync();

        Task<IEnumerable<KeyValueVM>> GetAllAsync(KeyValueVM filter);


        Task<ResultContext<KeyValueVM>> CreateKeyValueAsync(ResultContext<KeyValueVM> inputContext);

        Task<ResultContext<KeyValueVM>> UpdateKeyValueAsync(ResultContext<KeyValueVM> inputContext);

        Task<string> DeleteKeyValueAsync(KeyValueVM model);

        Task<IEnumerable<KeyValueVM>> GetAllNKPDClassValuesViaKeyTypeIntCodeAsync(string keyTypeIntCode);
    }
}
