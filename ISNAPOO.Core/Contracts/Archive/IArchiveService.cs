using Data.Models.Data.Archive;
using Data.Models.Data.SqlView.Archive;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Archive
{
    public interface IArchiveService
    {
        Task<List<CandidateProviderVM>> GetAllАnnualInfoAsync(CandidateProviderVM annualInfoVM, int idCandidateProvider, int year);
        Task<List<CandidateProviderVM>> GetCandidateProviderАnnualInfoAndSelfAssessmentReportsAsync(CandidateProviderVM candidateProviderVM, int idCandidateProvider);       
        Task<IEnumerable<AnnualMTBVM>> GetAnnualMTBReportAsync(List<CandidateProviderVM> candidateProviders, int year,string licensingType);
        Task<IEnumerable<AnnualTrainerQualificationsVM>> GetAnnualTrainerQualificationsReportAsync(List<CandidateProviderVM> candidateProviders, int year, string licensingType);

        Task<List<AnnualTrainingCourse>> GetTrainingCourseAsync(List<CandidateProviderVM> listCandidateProviderVM, string frameworkProgramValue, string year);
        Task<List<AnnualTrainingValidationClientCourse>> GetTrainingValidationClientAsync(List<CandidateProviderVM> listCandidateProviderVM, string year);
        Task<List<AnnualStudents>> GetStudentsAsync(List<CandidateProviderVM> listCandidateProviderVM);
        Task<List<AnnualStudentsByNationality>> GetStudentsByNationalityAsync(List<CandidateProviderVM> listCandidateProviderVM);
        IEnumerable<AnnualCurriculumsVM> GetAnnualCurriculumsReport(List<CandidateProviderVM> candidateProviders, int year, string licensingType);
        Task SaveAnnualInfoAsync(AnnualInfoVM annualInfo);
        Task<ResultContext<NoResult>> CreateAnnualInfoIdStatusAsync(AnnualInfoVM annualInfo);
        Task<ResultContext<NoResult>> SaveArchAnnualInfoStatus(int idAnnualInfo, int idKeyValue, string comment);
        Task<ResultContext<NoResult>> UpdateAnnualInfo(AnnualInfoVM archAnnualInfo, string isSubmite);
        Task saveAnnualReportNSIAsync(AnnualReportNSIVM annualReport);
        Task<AnnualReportNSIVM> getAnnualReportNSIByYear(string year);
        Task UpdateAnnualReportNSI(AnnualReportNSIVM report);
        List<AnnualReportNSIVM> getAllAnnualReportNSI();
        AnnualInfoVM GetAnnualInfoByIdCandProvAndYear(int idCandidateProvider, int year);
        AnnualInfoVM GetAnnualInfoByCandProvIdYearAndKeySubmittedIntCode(int idCandProvider, int year, int kvSubmitted);
        Task<List<AnnualInfoVM>> GetAllАnnualInfoForCandidateProviderAsync(int idCandidateProvider);
        Task<int> GetNoFinishedCourses(int idCandidateProvider, int year);
        Task<ResultContext<NoResult>> UpdateFinishedCoursesStatusIsArchive(int idCandidateProvider, string isArchiveOrNot, int year);
        Task<ResultContext<NoResult>> UpdateFinishedValidationClientCoursesStatusIsArchive(int idCandidateProvider, string isArchiveOrNot, int year);
        Task<ResultContext<NoResult>> UpdateTrainingConsultingClientStatusIsArchive(int idCandidateProvider, string isArchiveOrNot);
        Task<ResultContext<NoResult>> DoAnnualArhiveToCandidateProvider(int idCandidateProvider, int year);
        Task<ResultContext<NoResult>> DeleteArchiveCandidateProviderData(int idCandidateProvider, int year);
       

        #region Self assessment report
        Task<IEnumerable<SelfAssessmentReportVM>> GetAllSelfAssessmentReportsByIdCandidateProviderAsync(int idCandidateProvider, SelfAssessmentReportListFilterVM selfAssessmentReportListFilterVM, string fromWhere);
        Task<SelfAssessmentReportVM> GetSelfAssessmentReportByIdAsync(int idSelfAssessmentReport);
        Task<ResultContext<SelfAssessmentReportVM>> CreateSelfAssessmentReportAsync(ResultContext<SelfAssessmentReportVM> inputContext);
        Task<ResultContext<SelfAssessmentReportVM>> UpdateSelfAssessmentReportAsync(ResultContext<SelfAssessmentReportVM> inputContext);
        Task<ResultContext<SelfAssessmentReportVM>> FileInSelfAssessmentReportAsync(ResultContext<SelfAssessmentReportVM> resultContext);     
        Task<IEnumerable<SelfAssessmentReportVM>> GetAllSelfAssessmentReports(string licensingType, SelfAssessmentReportListFilterVM selfAssessmentReportListFilterVM, string fromWhere);
        Task<ResultContext<NoResult>> SaveSelfAssessmentReportApproveOrRejectStatusAsync(int idSelfAssessmentReport, int keyValueAppOrRej, string comment);
        Task<ResultContext<NoResult>> UpdateSelfAssessmentReportAppOrRej(SelfAssessmentReportVM selfAssessmentReportVM, int keyValueAppOrRej);
        Task<ResultContext<NoResult>> SaveSelfAssessmentReportsApproveOrRejectStatusAsync(List<SelfAssessmentReportVM> selfAssessmentListVM, int keyValueAppOrRej, string comment);
        Task<List<SelfAssessmentReportStatusVM>> GetSelfAssessmentReportStatuses(int idselfAssessmentReport);

       // Task<ResultContext<NoResult>> UpdateSelfAssessmentReportsApproveOrRejectStatuses(List<SelfAssessmentReportVM> selfAssessmentListVM);
        #endregion
    }
}
