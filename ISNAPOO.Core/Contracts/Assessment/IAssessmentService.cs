using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.Core.ViewModels.Common;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Assessment
{
    public interface IAssessmentService : IBaseService
    {
        #region Report
        Task<MemoryStream> CreateExcelSummarizedReportWithSurveyResultsAsync(ResultContext<List<SurveyResultVM>> resultContext);

        Task<MemoryStream> CreateExcelDetailedReportWithSurveyResultsAsync(ResultContext<List<SurveyResultVM>> resultContext);
        Task<MemoryStream> CreateExcelReportWithSurveyResultsAsync(List<SelfAssessmentReportVM> selfAssessmentReportVM, List<SurveyResultVM> listSurveyResultVM, string cpoCipoHeaderTxt, List<List<SelfAssessmentSummaryProfessionalTrainingVM>> selfAssSummTrainingVM, List<List<UserAnswerModel>> userAnswersListModels);
        #endregion

        #region Survey
        Task<IEnumerable<SurveyVM>> GetAllSurveysByIdSurveyTypeAsync(int idSurveyType);

        Task<IEnumerable<SurveyVM>> GetAllSurveysByIdSurveyTypeAndIdCandidateProviderAsync(int idSurveyType, int idCandidateProvider);

        Task<List<SurveyResultVM>> GetSurveyResultsForReportWithIncludesByIdSurveyAsync(int idSurvey, bool isResultForNAPOO = true);

        Task<List<SurveyResultVM>> GetSurveyResultsForReportWithSurveyResultIncludesByIdSurveyAsync(int idSurvey, bool isResultForNAPOO = true);

        Task<SurveyVM> GetSurveyByIdAsync(int idSurvey);

        Task<ResultContext<NoResult>> DeleteSurveyByIdAsync(int idSurvey);

        Task<ResultContext<SurveyVM>> CreateSurveyAsync(ResultContext<SurveyVM> resultContext);

        Task<ResultContext<SurveyVM>> UpdateSurveyAsync(ResultContext<SurveyVM> resultContext);

        Task<ResultContext<SurveyVM>> SendSurveyAsync(ResultContext<SurveyVM> resultContext);

        Task<ResultContext<SurveyVM>> ReSendSurveyAsync(ResultContext<SurveyVM> resultContext);

        Task<(MemoryStream MS, bool IsMacroIncluded)> GetSurveyTemplateWithQuestionsFilledByIdSurveyAsync(int idSurvey);

        Task<ResultContext<List<SurveyResultVM>>> ImportSurveyResultsAsync(MemoryStream file, string fileName, SurveyVM survey);

        MemoryStream CreateSurveyResultsExcelWithErrors(ResultContext<List<SurveyResultVM>> resultContext);

        Task<ResultContext<SurveyResultVM>> CreateSurveyResultFromExcelImportAsync(ResultContext<SurveyResultVM> resultContext);

        Task<ResultContext<NoResult>> CopySurveyByIdSurveyAsync(int idSurvey, int? year = 0);
        #endregion

        #region Question
        Task<int> GetNextQuestionOrderByIdSurveyAsync(int idSurvey);

        Task<ResultContext<QuestionVM>> CreateQuestionAsync(ResultContext<QuestionVM> resultContext);

        Task<ResultContext<QuestionVM>> UpdateQuestionAsync(ResultContext<QuestionVM> resultContext);

        Task<QuestionVM> GetQuestionByIdAsync(int idQuestion);

        Task<IEnumerable<QuestionVM>> GetQuestionsByIdSurveyAsync(int idSurvey);
        #endregion

        #region Answer
        Task<AnswerVM[]> GetAnswersByIdQuestionAsync(int idQuestion);

        Task<bool> CreateAnswerAsync(AnswerVM answer);

        Task<bool> UpdateAnswerAsync(AnswerVM answer);

        Task<ResultContext<NoResult>> DeleteQuestionByIdAsync(int idQuestion);
        #endregion

        #region Survey result
        Task<SurveyResultVM> GetSurveyResultByIdAsync(int idSurveyResult);

        Task SetSurveyResultStartDateAsync(int idSurveyResult);

        Task SetSurveyResultEndDateAndStatusToFiledAsync(SurveyResultVM surveyResultVM);
        #endregion

        #region User answer
        Task<ResultContext<NoResult>> CreateUserAnswersAsync(List<UserAnswerModel> userAnswerModels, SurveyResultVM surveyResult);
        Task<IEnumerable<SurveyVM>> GetAllSelfAssessmentSurveyAsync(string surveyTarget);
        Task<SurveyResultVM> GetSurveyResultWithIncludesUserAnswerByIdAsync(int idSurveyResult);
        Task<SurveyResultVM> GetSurveyResultsWithIncludesUserAnswerByIdAsync(int? idSurveyResult, int idCandidateProvider);
        Task<SurveyVM> GetSurveySelfAssessmentByYear(int year, string surveyTarget);
        Task<SurveyResultVM> CreateSurveyResultForSelfAssessmentAsync(int IdCandidate_Provider, SurveyVM survey, int idSurveyResultStatus);
        Task<ResultContext<NoResult>> CreateUserAnswersSelfAssessmentAsync(List<UserAnswerModel> userAnswerModels, SurveyResultVM surveyResult);
        Task<bool> CheckIsSelfAssessmentReportExist(int idCandidateProvider, int year);
        #endregion
    }
}
