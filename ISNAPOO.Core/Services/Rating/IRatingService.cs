using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Rating;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Rating
{
    public interface IRatingService
    {
        #region Indicators
        Task<ResultContext<IndicatorVM>> CreateIndicatorAsync(ResultContext<IndicatorVM> resultContext);
        Task<IEnumerable<IndicatorVM>> GetAllIndicatorsAsync(IndicatorVM model);
        Task<IndicatorVM> GetIndicatorByIdAsync(int id);
        Task<List<IndicatorVM>> GetIndicatorsByYearAsync(int Year);
        Task<List<IndicatorVM>> GetIndicatorsByYearAndTypeIdKeyValueAsync(int Year, int IdKeyValue);
        Task<ResultContext<IndicatorVM>> DeleteIndicatorAsync(ResultContext<IndicatorVM> resultContext);
        Task<ResultContext<IndicatorVM>> UpdateIndicatorAsync(ResultContext<IndicatorVM> resultContext);
        Task<List<KeyValueVM>> GetValidIndicatorsTypesByYearAsync(int Year);
        #endregion

        Task<IEnumerable<CandidateProviderIndicatorVM>> GetAllResultsAsync();
        Task CreateResultRangeAsync(List<CandidateProviderIndicatorVM> context);
        Task UpdateResultAsync(CandidateProviderIndicatorVM candidateProviderIndicatorVM);
        Task CreateResultAsync(CandidateProviderIndicatorVM context);

        #region StartCalculating
        Task StartCalculation(List<KeyValueVM> IndicatorTypes, int year);
        Task StartCalculationCIPO(List<KeyValueVM> IndicatorTypes, int year);

        #endregion

        #region Calculations
        Task<List<CalculateRatingVM>> CalculateCPOResultByCountClientPerYear(int year);
        Task<int> CountAbandonedTrainingStarted(int idCandidateProvider, int year);
        Task<int> CountProfessionalEducationFollowingNextYear(int idCandidateProvider, int year);
        Task<int> CountProfessionalQualificationPartProfessionValidation(int idCandidateProvider, int year);
        Task<int> CountDegreeProfessionalQualificationValidation(int idCandidateProvider, int year);
        Task<int> CountProfessionalQualificationPartProfession(int idCandidateProvider, int year);
        Task<int> CountDegreeProfessionalQualificationAsync(int idCandidateProvider, int year);
        Task<int> CountCourseCompetenceUnderRegulationsOneAndSeven(int idCandidateProvider, int year);
        Task<int> CountNonSuccessfulVocationalTraining(int idCandidateProvider, int year);
        #endregion
    }
}