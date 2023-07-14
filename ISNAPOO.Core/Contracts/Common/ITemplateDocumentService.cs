namespace ISNAPOO.Core.Contracts.Common
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using ISNAPOO.Core.ViewModels.Common;

    public interface ITemplateDocumentService : IBaseService
    {
        Task<int> CreateTemplateDocument(TemplateDocumentVM model);

        Task<IEnumerable<TemplateDocumentVM>> GetAllTemplateDocumentsWithoutDeletedAsync(TemplateDocumentVM filter);

        Task<bool> CheckIfExistUploadedFileAsync(TemplateDocumentVM model);

        Task<int> UpdateTemplateDocumentsAsync(TemplateDocumentVM model);

        Task<TemplateDocumentVM> GetTemplateDocumentByIdAsync(int id);

        Task<int> RemoveFileAsync(string fileName, TemplateDocumentVM model);

        Task<int> UploadFileAsync(MemoryStream file, string fileName, TemplateDocumentVM model);

        MemoryStream GetUploadedFile(TemplateDocumentVM model);

        Task RemoveTemplateDocument(TemplateDocumentVM templateIdList);
        Task<IEnumerable<TemplateDocumentVM>> GetAllTemplateDocumentsAsync(TemplateDocumentVM filter);
    }
}
