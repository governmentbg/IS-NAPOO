using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.SPPOO
{
    public interface IAreaService : IBaseService
    {
        Task<IEnumerable<AreaVM>> GetAllAreasAsync();

        Task<AreaVM> GetAreaByIdAsync(int id);

        Task<string> UpdateAreaAsync(AreaVM areaVM);

        Task<string> CreateAreaAsync(AreaVM areaVM);

        Task<string> DeleteAreaAsync(int id);

        Task<List<SPPOOTreeGridData>> LoadSPPOOData(
            List<int> areaList,
            List<int> professionalDirectionList,
            List<int> professionList,
            List<int> specialityList);

        MemoryStream GenerateSPPOOReport(List<SPPOOTreeGridData> SPPOOSource, IEnumerable<KeyValueVM> spkValues);
    }
}
