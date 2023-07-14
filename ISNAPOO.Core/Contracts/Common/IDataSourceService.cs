using Data.Models.Data.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IDataSourceService : IBaseService
    {
        List<KeyValueVM> GetAllKeyValueList();
        List<Setting> GetAllSettingList();

        List<AreaVM> GetAllAreasList();

        List<ProfessionalDirectionVM> GetAllProfessionalDirectionsList();

        List<ProfessionVM> GetAllProfessionsList();

        List<SpecialityVM> GetAllSpecialitiesList();

        List<ERUVM> GetAllERUsList();

        List<MenuNodeVM> GetAllMenuNode();

        Task<KeyValueVM> GetKeyValueByIdAsync(int? id);

        Task<KeyValueVM> GetKeyValueByIntCodeAsync(string keyTypeIntCode, string keyValueIntCode);

        Task<IEnumerable<KeyValueVM>> GetKeyValuesByKeyTypeIntCodeAsync(string keyTypeIntCode, bool addDefaultValue = false, bool allKeyValue = false);
        Task<IEnumerable<KeyValueVM>> GetKeyValuesByKeyTypeIntCodeAsync(string keyTypeIntCode, int idKeyValue, bool addDefaultValue = false);

        Task<IEnumerable<KeyValueVM>> GetKeyValuesByListIdsAsync(List<int> ids);

        Task<Setting> GetSettingByIntCodeAsync(string settingIntCode);

        Task<PolicyVM> GetPolicyByCode(string code);

        Task ReloadKeyType();
        Task ReloadKeyValue();
        Task ReloadSettings();

        Task ReloadAreas();

        Task ReloadProfessionalDirections();

        Task ReloadProfessions();

        Task ReloadSpecialities();

        Task ReloadERUs();
        Task ReloadPolicies();

        int GetActiveStatusID();
        int GetRemoveStatusID();
        int GetWorkStatusID();
        int GetOrderAddTypechangeID();
        int GetOrderChangeTypechangeID();
        int GetOrderRemoveTypechangeID();
        
    }
}
