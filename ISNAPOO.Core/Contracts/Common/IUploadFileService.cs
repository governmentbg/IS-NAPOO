using System.IO;
using System.Threading.Tasks;
using Data.Models.Common;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IUploadFileService : IBaseService
    {
        #region Base CRUD methods
        Task<bool> CheckIfExistUploadedFileAsync<T>(int idEntity, string? fileName = null) where T : AbstractUploadFile;

        Task<string> UploadFileAsync<T>(MemoryStream file, string fileName, string folderName, int idEntity) where T : AbstractUploadFile;

        Task<int> RemoveFileByIdAsync<T>(int idEntity) where T : AbstractUploadFile;

        Task<int> RemoveFileByIdAndFileNameAsync<T>(int idEntity, string fileName) where T : AbstractUploadFile;

        Task<(MemoryStream? MS, string FileNameFromOldIS)> GetUploadedFileAsync<T>(int idEntity, string? fileName = null) where T : AbstractUploadFile;
        #endregion

        #region Candidate Provider
        Task<ResultContext<CandidateProviderTrainerDocumentVM>> UploadFileCandidateProviderTrainerDocumentAsync(MemoryStream[] files, string fileName, string folderName, int idEntity);

        Task<ResultContext<CandidateProviderPremisesDocumentVM>> UploadFileCandidateProviderPremisesDocumentAsync(MemoryStream[] files, string fileName, string folderName, int idEntity);

        Task<ResultContext<CandidateProviderDocumentVM>> UploadFileCandidateProviderDocumentAsync(MemoryStream[] files, string fileName, string folderName, int idEntity);

        Task UploadFormularCandidateProviderDocumentAsync(MemoryStream file, string fileName, string folderName, int idEntity, int IdType);

        Task<int> UploadFileESignedApplicationAsync(MemoryStream file, CandidateProviderVM candidateProvider);

        Task<int> RemoveESignedApplicationFileByNameAsync(int idEntity, string fileName);

        Task<MemoryStream> GetUploadedFileESignedApplicationAsync(CandidateProviderVM candidateProvider);

        Task<MemoryStream> GetCurriculumTemplate();
        #endregion

        #region Validation
        Task<MemoryStream> GetValidationClientCurriculumUploadedFileAsync(int idValidationClient);
        #endregion

        #region Timestamp
        Task UploadTimeStampFilesAsync(MemoryStream timeStampResponse, string timeStampResponseName, string notificationTextName, string notificationText, int idNotification);
        #endregion

        #region Procedure
        Task<ResultContext<ProcedureExternalExpertVM>> UploadFileProcedureExternalExpertAsync(MemoryStream file, string fileName, string folderName, int idEntity);
        #endregion

        #region NSI Report
        Task<MemoryStream> GetReportNSIZipFile(int year);

        Task UploadFileReportNSI(AnnualReportNSIVM annualReport);
        #endregion

        #region FollowUpControl
        Task<ResultContext<FollowUpControlDocumentVM>> UploadFileFollowUpControlAdditionalDocumentAsync(MemoryStream[] files, string fileName, string folderName, int idEntity);
        #endregion
    }
}
