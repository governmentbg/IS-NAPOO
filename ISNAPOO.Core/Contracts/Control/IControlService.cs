using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Control;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Control
{
    public interface IControlService
    {
        #region FollowUpControl
        Task<int> CreateControlAsync(FollowUpControlVM model);
        Task<IEnumerable<FollowUpControlVM>> GetAllControlsAsync(string followUpControlType);
        Task<IEnumerable<FollowUpControlVM>> GetAllControlsByIdCandidateProviderAsync(int idCandidateProvider, string licensingType);
        Task<FollowUpControlVM> GetControlByIdFollowUpControlAsync(int id);
        Task<ResultContext<FollowUpControlVM>> UpdateFollowUpControlAsync(ResultContext<FollowUpControlVM> resultContext);
        #endregion

        #region FollowUpControlExpert
        Task<IEnumerable<FollowUpControlExpertVM>> GetAllControlExpertsByIdFollowUpControlAsync(int id);
        Task<ResultContext<FollowUpControlExpertVM>> AddControlExpertAsync(FollowUpControlExpertVM controlExpert);
        Task<ResultContext<FollowUpControlExpertVM>> DeleteControlExpertAsync(ResultContext<FollowUpControlExpertVM> resultContext);
        #endregion

        #region FollowUpControlDocument
        Task<ResultContext<NoResult>> SaveNewControlDocumentByNumberAndDate(FollowUpControlDocumentVM doc);
        Task<int> SaveControlDocument(FollowUpControlDocumentVM followUpControlDocumentVM);
        Task<int> UpdateControlDocument(FollowUpControlDocumentVM model);
        Task<IEnumerable<FollowUpControlDocumentVM>> GetAllDocumentsAsync(int idFollowUpControl);
        Task<bool> DeleteControlDocumentbyId(int id);
        Task<FollowUpControlDocumentVM> GetFollowUpControlDocumentByIdAsync(FollowUpControlDocumentVM followUpControlDocument);
        #endregion

        #region FollowUpControlUploadedFile
        Task<IEnumerable<FollowUpControlUploadedFileVM>> GetAllUploadedFilesByIdFollowUpControl(int id);
        Task<ResultContext<FollowUpControlUploadedFileVM>> CreateFollowUpControlUploadedFileAsync(FollowUpControlUploadedFileVM uploadedFile);
        Task<ResultContext<FollowUpControlUploadedFileVM>> UpdateFollowUpControlUploadedFileAsync(FollowUpControlUploadedFileVM model);
        Task<FollowUpControlUploadedFileVM> GetFollowUpControlUploadedFileById(int id);
        Task<ResultContext<FollowUpControlUploadedFileVM>> DeleteFollowUpControlUploadedFileAsync(FollowUpControlUploadedFileVM followUpControlUploadedFileVM);
        #endregion
    }
}
