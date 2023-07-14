using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Request;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Request
{
    public interface IProviderDocumentRequestService : IBaseService
    {
        #region Provider request document
        Task<IEnumerable<ProviderRequestDocumentVM>> GetAllDocumentRequestsByCandidateProviderIdAsync(int idCandidateProvider);

        Task LoadDataForDocumentTypesAsync(List<ProviderRequestDocumentVM> documents);

        Task<IEnumerable<ProviderRequestDocumentVM>> GetAllDocumentRequestsAsync();

        Task<IEnumerable<ProviderRequestDocumentVM>> GetAllDocumentRequestsWhereStatusIsSummarizedAndByIdCandidateProviderAsync(int idCandidateProvider);

        Task<ResultContext<ProviderRequestDocumentVM>> CreateProviderRequestDocumentAsync(ResultContext<ProviderRequestDocumentVM> inputContext);

        Task<ProviderRequestDocumentVM> GetProviderRequestDocumentByIdAsync(ProviderRequestDocumentVM providerRequestDocumentVM);

        Task<ResultContext<ProviderRequestDocumentVM>> DeleteProviderRequestDocumentByIdAsync(ProviderRequestDocumentVM providerRequestDocumentVM);

        Task<ResultContext<ProviderRequestDocumentVM>> FileInProviderRequestDocumentAsync(ResultContext<ProviderRequestDocumentVM> inputContext);

        Task<MemoryStream> PrintRequestDocumentAsync(ProviderRequestDocumentVM providerRequestDocumentVM, IEnumerable<TypeOfRequestedDocumentVM> typeOfRequestedDocuments, CandidateProviderVM providerVM);

        Task<MemoryStream> GeneratePrintingTemplateAsync(List<TypeOfRequestedDocumentVM> typeOfRequestedDocuments, NAPOORequestDocVM nAPOORequestDocVM, bool printInPDF);

        Task UpdateProviderRequestDocumentUploadedFileNameAsync(int idProviderRequestDocument, string fileName);
        #endregion

        #region Type of requested document
        Task<IEnumerable<TypeOfRequestedDocumentVM>> GetAllValidTypesOfRequestedDocumentAsync();

        Task<IEnumerable<TypeOfRequestedDocumentVM>> GetAllTypesOfRequestedDocumentAsync();

        Task<TypeOfRequestedDocumentVM> GetTypeOfRequestedDocumentByIdAsync(TypeOfRequestedDocumentVM typeOfRequestedDocumentVM);

        Task<IEnumerable<TypeOfRequestedDocumentVM>> GetTypesOfRequestedDocumentByListIdsAsync(List<int> ids);

        Task<IEnumerable<TypeOfRequestedDocumentVM>> GetTypesOfRequestedDocumentsByRequestNumberAsync(List<ProviderRequestDocumentVM> providerRequestDocumentsSource, long? requestNumber);

        Task<ResultContext<TypeOfRequestedDocumentVM>> UpdateTypeOfRequestedDocumentAsync(ResultContext<TypeOfRequestedDocumentVM> inputContext);

        Task<ResultContext<TypeOfRequestedDocumentVM>> CreateTypeOfRequestedDocumentAsync(ResultContext<TypeOfRequestedDocumentVM> inputContext);

        Task<IEnumerable<TypeOfRequestedDocumentVM>> GetAllTypeOfRequestedDocsForLegalCapacityCourse();
        #endregion

        #region Request document type
        Task<RequestDocumentTypeVM> GetRequestDocumentTypeByIdTypeOfRequestDocumentAndIdProviderRequestDocumentAsync(RequestDocumentTypeVM requestDocumentTypeVM);

        Task<ResultContext<RequestDocumentTypeVM>> DeleteRequestDocumentTypeByIdAsync(RequestDocumentTypeVM requestDocumentTypeVM);

        Task UpdateRequestDocumentTypeAsync(RequestDocumentTypeVM requestDocumentTypeVM);
        #endregion

        #region Request document status
        Task CreateRequestDocumentStatusAsync(RequestDocumentStatusVM requestDocumentStatusVM);

        Task<IEnumerable<RequestDocumentStatusVM>> GetRequestDocumentStatusesByRequestDocumentIdAsync(int idRequestDocument);
        #endregion

        #region NAPOO request document
        Task<IEnumerable<NAPOORequestDocVM>> GetAllNAPOORequestDocumentsAsync();

        Task<ResultContext<NAPOORequestDocVM>> CreateNAPOORequestDocumentAsync(ResultContext<NAPOORequestDocVM> inputContext);

        Task<ResultContext<NAPOORequestDocVM>> GenerateFileForMONPrinting(ResultContext<NAPOORequestDocVM> inputContext);

        Task<NAPOORequestDocVM> GetNAPOORequestDocumentByIdAsync(NAPOORequestDocVM nAPOORequestDocVM);

        Task<IEnumerable<PrintingHouseReportVM>> GetPrintingHouseReportDataAsync();

        Task UpdateNotificationSentStatusForNAPOORequestDocAsync(NAPOORequestDocVM nAPOORequestDoc);

        Task<NAPOORequestDocVM> GetNAPOORequestDocDataByIdNAPOORequestDocAsync(int idNAPOORequestDoc);
        #endregion

        #region DocumentOffer

        Task<IEnumerable<ProviderDocumentOfferVM>> GetAllProviderDocumentOffersAsync(ProviderDocumentOfferVM fiterModel);
        Task<ProviderDocumentOfferVM> GetProviderDocumentOfferByIdAsync(ProviderDocumentOfferVM model);
        Task<ResultContext<ProviderDocumentOfferVM>> SaveProviderDocumentOfferAsync(ResultContext<ProviderDocumentOfferVM> resultContext);

        #endregion

        #region Request document management
        Task<ResultContext<NoResult>> AddDocumentSerialNumberToRequestDocumentManagementAsync(DocumentSerialNumberVM documentSerialNumber);

        Task<IEnumerable<RequestDocumentManagementControlModel>> GetDocumentsControlDataAsync(int idCandidateProvider);

        Task<IEnumerable<RequestDocumentManagementVM>> GetAllRequestDocumentManagementsByIdCandidateProviderAndDocumentOperationReceivedAsync(int idCandidateProvider);

        Task<ResultContext<RequestDocumentManagementVM>> CreateRequestDocumentManagementAsync(ResultContext<RequestDocumentManagementVM> inputContext);

        Task<ResultContext<RequestDocumentManagementVM>> UpdateRequestDocumentManagementAsync(ResultContext<RequestDocumentManagementVM> inputContext, bool alwaysAddNewDocumentSerialNumber = false, bool updateAfterReceiveFromOtherCPO = false);

        Task<IEnumerable<RequestDocumentManagementVM>> GetAllRequestDocumentManagementsByDocumentOperationSubmittedAndByIdCandidateProviderAsync(int idCandidateProvider);

        Task<IEnumerable<RequestDocumentManagementVM>> GetAllRequestDocumentManagementsByDocumentOperationReceivedAndByIdCandidateProviderAsync(int idCandidateProvider);

        int GetDocumentCountByDocumentOperationReceivedByProviderIdByTypeOfRequestedDocumentIdAndReceiveYear(RequestDocumentManagementVM requestDocumentManagementVM);

        Task<RequestDocumentManagementVM> GetRequestDocumentManagementByIdAsync(RequestDocumentManagementVM requestDocumentManagementVM);

        Task<IEnumerable<RequestDocumentManagementVM>> GetAllRequestDocumentManagementsByIdProviderAndByDocumentOperationAwaitingConfirmationAsync(RequestDocumentManagementVM requestDocumentManagementVM);

        Task<RequestDocumentManagementVM> GetRequestDocumentManagementByProviderIdByProviderPartnerIdByIdTypeOfRequestedDocumentAndByDocumentOperationAwaitingConfirmationAsync(RequestDocumentManagementVM requestDocumentManagementVM);

        Task<int> UpdateDocumentCountAfterDocumentSerialNumberDeletionByIdRequestDocumentManagementAsync(int idRequestDocumentManagement);

        Task<MemoryStream> PrintHandingOverProtocolAsync(List<RequestDocumentManagementVM> requestDocumentManagements, List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsSource, List<DocumentSeriesVM> documentSeries);

        Task<RequestDocumentManagementVM> GetRequestDocumentManagementByIdAsync(int idRequestDocumentManagement);
        #endregion

        #region Request document management uploaded file
        Task<ResultContext<NoResult>> CreateRequestDocumentManagementUploadedFileByListRequestDocumentManagementAsync(List<RequestDocumentManagementVM> requestDocumentManagements, RequestDocumentManagementUploadedFileVM requestDocumentManagementUploadedFile);

        Task<ResultContext<NoResult>> CreateRequestDocumentManagementUploadedAsync(RequestDocumentManagementUploadedFileVM requestDocumentManagementUploadedFile);

        Task<RequestDocumentManagementUploadedFileVM> GetManagementUploadedFileByIdRequestDocumentManagementAsync(int idRequestDocumentManagement);
        #endregion

        #region Document series
        Task<IEnumerable<DocumentSeriesVM>> GetAllDocumentSeriesAsync();

        Task<IEnumerable<DocumentSeriesVM>> GetAllDocumentSeriesIncludeAsync();
        void SaveDocumentSeries(DocumentSeriesVM model);

        #endregion

        #region Document serial number
        Task<IEnumerable<DocumentSerialNumberVM>> GetDocumentSerialNumbersWithOperationReceivedAndByTypeOfRequestedDocumentIdAndByIdCandidateProviderAsync(TypeOfRequestedDocumentVM typeOfRequestedDocument, int idCandidateProvider);

        Task<IEnumerable<DocumentSerialNumberVM>> GetDocumentSerialNumbersWithOperationSubmittedAndOperationAwaitingConfirmationAndSerialNumberAsync(DocumentSerialNumberVM documentSerialNumberVM, int providerPartnerId);

        Task<IEnumerable<DocumentSerialNumberVM>> GetDocumentSerialNumbersWithOperationSubmittedByRequestDocumentManagementIdAndProviderIdAsync(DocumentSerialNumberVM documentSerialNumberVM);

        Task<ResultContext<DocumentSerialNumberVM>> DeleteDocumentSerialNumberAsync(ResultContext<DocumentSerialNumberVM> inputContext);

        Task<ResultContext<DocumentSerialNumberVM>> DeleteDocumentSerialNumbersByListIdsAsync(List<int> ids);

        Task<DocumentSerialNumberVM> GetDocumentSerialNumberByIdOperationAndIdDocumentSerialNumber(int idOperation, int idDocumentSerialNumber);

        Task<IEnumerable<DocumentSerialNumberVM>> FilterDocumentSerialNumbersAsync(NAPOODocumentSerialNumberFilterVM nAPOODocumentSerialNumberFilterVM);

        Task<string> GetClientNameByIdDocumentSerialNumberAsync(int idDocumentSerialNumber);

        Task<IEnumerable<DocumentSerialNumberVM>> GetAllDocumentSerialNumbersByIdCandidateProviderAndStatusReceivedAndYear(CandidateProviderVM candidateProviderVM, int year);

        IEnumerable<DocumentSerialNumberVM> GetAllDocumentSerialNumbersByIdCandidateProviderByStatusReceivedByYearAndByIdCourseType(CandidateProviderVM candidateProviderVM, int year, int idCourseType, bool isDuplicate = false, bool isDuplicateValidation = false);

        Task<DocumentSerialNumberVM> GetDocumentSerialNumberByIdAndYearAsync(int idDocumentSerialNumber, int year);

        Task<IEnumerable<DocumentSerialNumberVM>> GetAllDocumentSerialNumbersByIdRequestDocumentManagementAsync(int idRequestDocumentManagement);

        Task<ResultContext<NoResult>> AddConsecutiveDocumentSerialNumbersAsync(List<DocumentSerialNumberVM> documentSerials);

        Task<IEnumerable<DocumentSerialNumberVM>> GetAllDocumentSerialNumbersByIdCandidateProviderAndStatusSubmittedAndYear(CandidateProviderVM candidateProviderVM, int year);

        Task<IEnumerable<DocumentSerialNumberVM>> GetAllDocumentSerialNumbersByIdCandidateProviderAndStatusPrintedAndYear(CandidateProviderVM candidateProviderVM, int year);
        #endregion

        #region Request report
        Task<IEnumerable<RequestReportVM>> GetAllRequestReportsByCandidateProviderIdAsync(CandidateProviderVM candidateProviderVM);

        Task<IEnumerable<RequestReportVM>> GetAllRequestReportsAsync();

        Task<RequestReportVM> GetRequestReportByIdAsync(RequestReportVM requestReportVM);

        Task<ResultContext<RequestReportVM>> CreateRequestReportAsync(ResultContext<RequestReportVM> inputContext);

        Task<ResultContext<RequestReportVM>> UpdateRequestReportAsync(ResultContext<RequestReportVM> inputContext);

        Task<ResultContext<RequestReportVM>> FileInProviderRequestReportAsync(ResultContext<RequestReportVM> inputContext);

        MemoryStream PrintProtocol(RequestReportVM requestReportVM, List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsSource);

        Task<MemoryStream> PrintReportAsync(RequestReportVM requestReportVM, List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsSource);

        Task<ResultContext<NoResult>> DeleteRequestReportByIdAsync(int idRequestReport);

        Task<bool> DoesRequestReportForSameYearAlreadyExistAsync(int idCandidateProvider, int year);
        #endregion

        #region Report uploaded document
        Task<ReportUploadedDocVM> GetReportUploadedDocumentByIdAsync(int idReportUploadedDoc);

        Task<ResultContext<ReportUploadedDocVM>> CreateReportUploadedDocumentAsync(ReportUploadedDocVM reportUploadedDocVM);

        Task<ResultContext<ReportUploadedDocVM>> UpdateReportUploadedDocumentAsync(ReportUploadedDocVM reportUploadedDocVM);

        Task<ResultContext<ReportUploadedDocVM>> DeleteReportUploadedDocumentAsync(ReportUploadedDocVM reportUploadedDocVM);

        Task<IEnumerable<ReportUploadedDocVM>> GetAllReportUploadedDocumentsByRequestReportIdAsync(int idRequestReport);
        Task<DocumentSeriesVM> GetDocumenSeriestByTypeAndYear(int idTypeOfRequestedDocument, int? year);
        void UpdateDocumentSeries(DocumentSeriesVM model);
        Task<TypeOfRequestedDocumentVM> GetTypeOfRequestedDocumentAsyncByDocTypeOfficialNumber(string docTypeOfficialNumber);
        #endregion
    }
}
