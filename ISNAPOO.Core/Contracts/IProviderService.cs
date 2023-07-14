namespace ISNAPOO.Core.Contracts
{
    using Data.Models.Data.ProviderData;
    using Data.Models.Framework;
    using ISNAPOO.Common.Framework;
    using ISNAPOO.Core.ViewModels.Candidate;
    using ISNAPOO.Core.ViewModels.Common;
    using ISNAPOO.Core.ViewModels.CPO.ProviderData;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IProviderService
    {
        /// <summary>
        /// Запис на данни за ЦПО,ЦИПО
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<int> CreateProviderAsync(ProviderVM providerVM);

        Task<ProviderVM> GetProviderByIdAsync(int providerId);

        Task<IEnumerable<ProviderVM>> GetAllProvidersAsync();

        Task<int> UpdateProviderAsync(ProviderVM model);

        Task<int> DeleteProviderAsync(int providerId);

        Task<ResultContext<CandidateProviderVM>> StartCIPOProcedureAsync(ResultContext<CandidateProviderVM> resultContext, bool isApplicationSentViaIS);

        Task<ResultContext<CandidateProviderVM>> StartProcedureAsync(ResultContext<CandidateProviderVM> resultContext, bool isApplicationSentViaIS, bool isCPO);
        Task<StartedProcedureVM> GetStartedProcedureByIdAsync(int idStartedProcedure);
        Task SetApplicationStatusAfterPositiveAssessmentAsync(int idCandidateProvider);
        Task<List<ProcedureExpertCommissionVM>> GetStartedProcedureExpertCommissionByIdAsync(int idStartedProcedure);
        Task<IEnumerable<ProcedureExternalExpertVM>> GetAllProcedureExternalExpertsAsync(ProcedureExternalExpertVM model);
        Task<ResultContext<ProcedureExternalExpertVM>> SaveProcedureExternalExpertAsync(ResultContext<ProcedureExternalExpertVM> resultContext);
        Task<ResultContext<ProcedureExternalExpertVM>> DeleteProcedureExternalExpertAsync(ResultContext<ProcedureExternalExpertVM> resultContext);
        Task<ResultContext<ProcedureExpertCommissionVM>> SaveProcedureExpertCommissionAsync(ResultContext<ProcedureExpertCommissionVM> resultContext);
        Task<ProcedureExpertCommissionVM> GetProcedureExpertCommissionByIdStartProcedureAsync(int idStartedProcedure);
        Task<ResultContext<StartedProcedureProgressVM>> InsertStartedProcedureProgressAsync(ResultContext<StartedProcedureProgressVM> resultContext);
        Task<IEnumerable<StartedProcedureProgressVM>> GetAllStartedProcedureProgressByIdStartProcedureAsync(int idStartedProcedure);
        Task<ResultContext<ProcedureDocumentVM>> InsertProcedureDocumentAsync(ResultContext<ProcedureDocumentVM> resultContext);
        Task UpdateStartedProcedureNapooDeadlineAsync(int idStartedProcedure, DateTime deadline);
        Task<IEnumerable<ProcedureDocumentVM>> GetAllProcedureDocumentsAsync(ProcedureDocumentVM model);
        Task<IEnumerable<ProcedureExpertCommissionVM>> GetAllProcedureExpertExpertCommissionsAsync();
        Task<StartedProcedureVM> GetStartedProcedureByIdForGenerateDocumentAsync(int idStartedProcedure);
        Task<IEnumerable<NegativeIssueVM>> GetAllNegativeIssuesByIdStartedProcedureAsync(int idStartedProcedure);
        Task<ResultContext<NegativeIssueVM>> DeleteNegativeIssueAsync(ResultContext<NegativeIssueVM> resultContext);
        Task<ResultContext<NegativeIssueVM>> SaveNegativeIssueAsync(ResultContext<NegativeIssueVM> resultContext);
        Task<ResultContext<List<ProcedureDocumentVM>>> InsertProcedureDocumentFromListAsync(ResultContext<List<ProcedureDocumentVM>> resultContext);
        Task UpdateStartedProcedureExpertDeadlineAsync(int idStartedProcedure, DateTime deadline);
        Task<IEnumerable<ProcedurePriceVM>> GetAllProcedurePricesAsync();
        Task<ProcedurePriceVM> GetProcedurePriceByIdAsync(ProcedurePriceVM model);
        Task<List<ProcedurePriceVM>> GetProcedurePriceWithPredicateAsync(ProcedurePriceVM model);
        Task<ResultContext<ProcedurePriceVM>> SaveProcedurePriceAsync(ResultContext<ProcedurePriceVM> resultContext);
        Task<IEnumerable<ProviderVM>> GetProvidersByListIdsAsync(List<int> ids);
        Task<CandidateProviderPersonVM> GetCandidateProviderPersonByIdAsync(int candidateProviderPersonId);
        Task<IEnumerable<CandidateProviderPersonVM>> GetAllCandidateProviderPersonsAsync(CandidateProviderPersonVM fiterModel);
        Task<ResultContext<CandidateProviderPersonVM>> SaveCandidateProviderPersonAsync(ResultContext<CandidateProviderPersonVM> resultContext);

        Task<ProcedureDocumentVM> GetProcedureDocumentByIdStartedProcedureAndIdDocumentTypeAsync(int idStartedProcedure, int idDocumentType);

        Task<IEnumerable<ProcedureExternalExpertVM>> GetAllProcedureExternalExpertReportsByIdStartedProcedureAsync(int idStartedProcedure);

        Task<int> GetIdExpertByIdPersonAsync(int idPerson);

        Task<ProcedureExternalExpertVM> GetProcedureExternalExpertByIdAsync(int idProcedureExternalExpert);

        Task<ProcedureDocumentVM> GetProcedureDocumentByIdStartedProcedureByIdDocumentTypeAndByIdExpertAsync(int idStartedProcedure, int idDocumentType, int idExpert);
        Task<IEnumerable<PersonVM>> GetAllPersonsForNotificationByCandidateProviderIdAsync(int idCandidateProvider);

        Task<ResultContext<NoResult>> SaveNewDocumentByNumberAndDate(ProcedureDocumentVM doc);
        Task<bool> DeleteProcedureDocument(ProcedureDocumentVM procedureDocumentVM);
    }
}
