using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.DOC
{
    public interface IDOCService
    {
        Task<IEnumerable<DocVM>> GetAllDocAsync(DocVM filterDocVM);
        Task<IEnumerable<DocVM>> GetAllDOCByStatus(string statusName);
        Task<List<DocVM>> GetAllActiveDocAsync();

        Task<IEnumerable<DocVM>> GetAllDocAsync();

        Task<DocVM> GetDOCByIdAsync(DocVM model);

        Task<string> UpdateDOCAsync(DocVM model);

        Task<string> CreateDOCAsync(DocVM model);

        Task<DocVM> GetActiveDocByProfessionIdAsync(ProfessionVM professionVM);

        Task<ResultContext<DocVM>> ImportDOCAsync(ResultContext<DocVM> resultContext, MemoryStream file, string fileName);

        Task<MemoryStream> CreateExcelWithErrors(ResultContext<DocVM> resultContext);

        Task<int> RemoveDocFromSpecialityById(int idSpeciality);
        Task<int> DelteNKPDFromDocById(int idNKPD, int idDoc);


        Task<IEnumerable<ERUVM>> GetAllERUsByDocIdAsync(ERUVM filterERUVM, bool includeSpecialities = true);
        Task<IEnumerable<ERUVM>> GetAllERUsByActiveDOCsAsync();

        Task<string> UpdateERUAsync(ERUVM model);

        Task<string> CreateERUAsync(ERUVM model);

        Task<IEnumerable<ERUVM>> GetERUsByIdsAsync(List<int> ids);

        Task<ERUVM> GetERUByIdAsync(ERUVM eru);
        Task<bool> CheckForActiveDocWithSameProfession(DocVM model);
        Task<bool> CheckForActiveDocWithSameSpeciality(DocVM model);
        Task<int> DelteSpecialityFromERUById(int idSpeciality, int idERU);
        Task<int> DelteERUById(int idEru);
        Task<int> DelteDocById(int idDoc);

        Task<IEnumerable<ERUVM>> GetAllERUsByIdSpecialityAsync(int idSpeciality);
    }
}
