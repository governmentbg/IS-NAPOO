using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Rating;
using Data.Models.Data.Training;
using Data.Models.DB;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Mailing;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.Services.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Rating;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using Syncfusion.DocIO.DLS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ISNAPOO.Core.Services.Rating
{
    public class RatingService : BaseService, IRatingService
    {
        private readonly IRepository repository;
        private readonly ILogger<CandidateProviderService> _logger;
        private readonly IKeyValueService keyValueService;
        private readonly ISettingService settingService;
        private readonly IDataSourceService dataSourceService;
        private readonly ICandidateProviderService candidateProviderService;
        private readonly ApplicationDbContext contex;
        public RatingService(IRepository repository, ILogger<CandidateProviderService> logger, IKeyValueService keyValueService,
            ISettingService settingService, IDataSourceService dataSourceService, ICandidateProviderService candidateProviderService, ApplicationDbContext applicationDbContext) : base(repository)
        {
            this.repository = repository;
            this._logger = logger;
            this.keyValueService = keyValueService;
            this.settingService = settingService;
            this.dataSourceService = dataSourceService;
            this.candidateProviderService = candidateProviderService;
            this.contex = applicationDbContext;
        }

        #region Indicators
        public async Task<ResultContext<IndicatorVM>> CreateIndicatorAsync(ResultContext<IndicatorVM> resultContext)
        {
            try
            {
                resultContext.ResultContextObject.IdIndicator = 0;

                var entity = resultContext.ResultContextObject.To<Indicator>();

                await this.repository.AddAsync(entity);

                var result = await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject.IdIndicator = entity.IdIndicator;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }
        public async Task<ResultContext<IndicatorVM>> DeleteIndicatorAsync(ResultContext<IndicatorVM> resultContext)
        {
            try
            {
                var entity = resultContext.ResultContextObject.To<Indicator>();

                await this.repository.HardDeleteAsync<Indicator>(entity.IdIndicator);
                this.repository.Detach(entity);

                var result = await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject = new IndicatorVM();

                resultContext.AddMessage("Успешно изтриване");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }
        public async Task<ResultContext<IndicatorVM>> UpdateIndicatorAsync(ResultContext<IndicatorVM> resultContext)
        {
            try
            {
                var entity = resultContext.ResultContextObject.To<Indicator>();

                this.repository.Update<Indicator>(entity);
                var result = await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject = new IndicatorVM();

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }
            return resultContext;
        }
        public async Task<IEnumerable<IndicatorVM>> GetAllIndicatorsAsync(IndicatorVM model)
        {
            var kvIndicators = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RatingIndicatorType", false);

            IQueryable<Indicator> data = this.repository.AllReadonly<Indicator>(FilterIndicator(model));

            var result = await data.To<IndicatorVM>().ToListAsync();

            foreach (var indicator in result)
            {
                if (indicator.IdIndicatorType != null)
                {
                    indicator.IndicatorDetails = kvIndicators.First(x => x.IdKeyValue == indicator.IdIndicatorType);
                }
            }

            return result;
        }

        protected Expression<Func<Indicator, bool>> FilterIndicator(IndicatorVM model)
        {
            var predicate = PredicateBuilder.True<Indicator>();

            if (model.Year != 0)
            {
                predicate = predicate.And(p => p.Year == model.Year);
            }

            if (model.IdIndicatorType != 0)
            {
                predicate = predicate.And(p => p.IdIndicatorType == model.IdIndicatorType);
            }

            return predicate;
        }

        public async Task<IndicatorVM> GetIndicatorByIdAsync(int id)
        {
            var kvIndicators = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RatingIndicatorType", false);

            Indicator data = await this.repository.GetByIdAsync<Indicator>(id);

            var result = data.To<IndicatorVM>();

            if (result.IdIndicatorType != null)
            {
                result.IndicatorDetails = kvIndicators.First(x => x.IdKeyValue == result.IdIndicatorType);
            }

            return result;
        }

        public async Task<List<IndicatorVM>> GetIndicatorsByYearAsync(int Year)
        {
            var kvIndicators = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RatingIndicatorType", false);

            IQueryable<Indicator> data = this.repository.All<Indicator>(x => x.Year == Year);

            List<IndicatorVM> resultList = await data.To<IndicatorVM>().ToListAsync();

            foreach (var result in resultList)
            {
                if (result.IdIndicatorType != null)
                {
                    result.IndicatorDetails = kvIndicators.First(x => x.IdKeyValue == result.IdIndicatorType);
                }
            }

            return resultList;
        }

        public async Task<List<IndicatorVM>> GetIndicatorsByYearAndTypeIdKeyValueAsync(int Year, int IdKeyValue)
        {
            var kvIndicators = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RatingIndicatorType", false);

            IQueryable<Indicator> data = this.repository.All<Indicator>(x => x.Year == Year && x.IdIndicatorType == IdKeyValue);

            List<IndicatorVM> resultList = await data.To<IndicatorVM>().ToListAsync();

            foreach (var result in resultList)
            {
                if (result.IdIndicatorType != null)
                {
                    result.IndicatorDetails = kvIndicators.First(x => x.IdKeyValue == result.IdIndicatorType);
                }
            }

            return resultList;
        }

        public async Task<List<KeyValueVM>> GetValidIndicatorsTypesByYearAsync(int Year)
        {
            var kvIndicators = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RatingIndicatorType", false);

            var data = this.repository.All<Indicator>(x => x.Year == Year).ToList();

            List<KeyValueVM> result = kvIndicators.Where(x => data.Any(y => y.IdIndicatorType == x.IdKeyValue)).ToList();

            return result;
        }
        #endregion

        #region ResultIndicators
        public async Task CreateResultRangeAsync(List<CandidateProviderIndicatorVM> context)
        {
            try
            {
                var entity = context.To<List<CandidateProviderIndicator>>();

                await this.repository.AddRangeAsync<CandidateProviderIndicator>(entity);

                var result = await this.repository.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task CreateResultAsync(CandidateProviderIndicatorVM context)
        {
            try
            {
                var entity = context.To<CandidateProviderIndicator>();

                await this.repository.AddAsync<CandidateProviderIndicator>(entity);

                var result = await this.repository.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task UpdateResultAsync(CandidateProviderIndicatorVM candidateProviderIndicatorVM)
        {
            try
            {
                var entity = candidateProviderIndicatorVM.To<CandidateProviderIndicator>();

                this.repository.Detach(entity);
                this.repository.Update<CandidateProviderIndicator>(entity);
                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task<IEnumerable<CandidateProviderIndicatorVM>> GetAllResultsAsync()
        {
            var kvIndicators = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RatingIndicatorType", false);

            IQueryable<CandidateProviderIndicator> data = this.repository.AllReadonly<CandidateProviderIndicator>();

            var result = await data.To<CandidateProviderIndicatorVM>(
                p => p.CandidateProvider,
                p => p.Indicator).ToListAsync();

            foreach (var indicator in result)
            {
                if (indicator.IdIndicatorType != null)
                {
                    indicator.IndicatorDetails = kvIndicators.First(x => x.IdKeyValue == indicator.IdIndicatorType);
                }
            }

            return result;
        }

        public async Task<IEnumerable<CandidateProviderIndicatorVM>> GetAllResultsNoTrackAsync()
        {
            var kvIndicators = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RatingIndicatorType", false);

            IQueryable<CandidateProviderIndicator> data = this.repository.AllReadonly<CandidateProviderIndicator>().AsNoTracking<CandidateProviderIndicator>();

            var result = await data.To<CandidateProviderIndicatorVM>(
                p => p.CandidateProvider,
                p => p.Indicator).ToListAsync();

            foreach (var indicator in result)
            {
                if (indicator.IdIndicatorType != null)
                {
                    indicator.IndicatorDetails = kvIndicators.First(x => x.IdKeyValue == indicator.IdIndicatorType);
                }
            }

            return result;
        }
        #endregion

        #region CalculatingPoints

        public async Task CalculateRating(List<CalculateRatingVM> list, KeyValueVM IndicatorType, int year)
        {
            List<IndicatorVM> indicatorList = (await this.GetIndicatorsByYearAndTypeIdKeyValueAsync(year, IndicatorType.IdKeyValue)).OrderByDescending(x => x.RangeTo).ToList();
            List<CandidateProviderIndicatorVM> AllCandidateProviderIndicators = new List<CandidateProviderIndicatorVM>();
            AllCandidateProviderIndicators = (await this.GetAllResultsNoTrackAsync()).ToList();
            if (indicatorList.Any())
            {
                if (list.Count != 0)
                {
                    foreach (var count in list.Where(x => x.Year == year).ToList())
                    {
                        if (indicatorList.Any(x => x.RangeFrom <= count.Count && x.RangeTo >= count.Count))
                        {
                            var SelectedIndicator = indicatorList.Where(x => x.RangeFrom <= count.Count && x.RangeTo >= count.Count).FirstOrDefault();
                            if (SelectedIndicator != null && !AllCandidateProviderIndicators.Any(y => y.IdCandidate_Provider == count.IdCandidate_Provider && y.Year == year && y.IdIndicatorType == IndicatorType.IdKeyValue))
                            {
                                CandidateProviderIndicatorVM tempResult = new CandidateProviderIndicatorVM()
                                {
                                    IdCandidate_Provider = count.IdCandidate_Provider,
                                    IdIndicator = SelectedIndicator.IdIndicator,
                                    IdIndicatorType = SelectedIndicator.IdIndicatorType,
                                    Year = count.Year,
                                    Points = SelectedIndicator.Points
                                };
                                await this.CreateResultAsync(tempResult);
                            }
                            else if (SelectedIndicator != null && AllCandidateProviderIndicators.Any(y => y.IdCandidate_Provider == count.IdCandidate_Provider && y.Year == year && y.IdIndicatorType == IndicatorType.IdKeyValue))
                            {

                                CandidateProviderIndicatorVM tempResult = AllCandidateProviderIndicators.First(y => y.IdCandidate_Provider == count.IdCandidate_Provider && y.Year == year && y.IdIndicatorType == IndicatorType.IdKeyValue);
                                tempResult.IdIndicator = SelectedIndicator.IdIndicator;
                                tempResult.Points = SelectedIndicator.Points;

                                await this.UpdateResultAsync(tempResult);
                            }
                        }
                        else if (AllCandidateProviderIndicators.Any(y => y.IdCandidate_Provider == count.IdCandidate_Provider && y.Year == year && y.IdIndicatorType == IndicatorType.IdKeyValue))
                        {
                            CandidateProviderIndicatorVM tempResult = AllCandidateProviderIndicators.First(y => y.IdCandidate_Provider == count.IdCandidate_Provider && y.Year == year && y.IdIndicatorType == IndicatorType.IdKeyValue);
                            tempResult.IdIndicator = null;
                            tempResult.Points = 0;

                            await this.UpdateResultAsync(tempResult);
                        }
                        else
                        {
                            CandidateProviderIndicatorVM tempResult = new CandidateProviderIndicatorVM()
                            {
                                IdCandidate_Provider = count.IdCandidate_Provider,
                                IdIndicatorType = IndicatorType.IdKeyValue,
                                Year = count.Year,
                                Points = 0
                            };
                            await this.CreateResultAsync(tempResult);
                        }
                    }
                }
            }
        }
        public async Task StartCalculation(List<KeyValueVM> IndicatorTypes, int year)
        {
            var candidateProviders = await this.candidateProviderService.GetAllActiveCandidateProvidersAsync("LicensingCPO");

            foreach (var IndicatorType in IndicatorTypes)
            {
                List<CalculateRatingVM> list = new List<CalculateRatingVM>();
                if (IndicatorType.KeyValueIntCode == "CountCertifiedMen")
                {
                    var temp = await this.CalculateCPOResultByCountClientPerYear(year);
                    list.AddRange(temp);
                }
                else if (IndicatorType.KeyValueIntCode == "CountDegreeProfessionalQualification")
                {
                    var temp = await this.GetListCountDegreeProfessionalQualificationAsync(year);
                    list.AddRange(temp);
                }
                else if (IndicatorType.KeyValueIntCode == "CountProfessionalQualificationPartProfession")
                {

                    var temp = await this.GetListCountProfessionalQualificationPartProfession(year);
                    list.AddRange(temp);

                }
                else if (IndicatorType.KeyValueIntCode == "CountDegreeProfessionalQualificationValidation")
                {
                    var temp = await this.GetListCountDegreeProfessionalQualificationValidation(year);
                    list.AddRange(temp);

                }
                else if (IndicatorType.KeyValueIntCode == "CountProfessionalQualificationPartProfessionValidation")
                {

                    var temp = await this.GetListCountProfessionalQualificationPartProfessionValidation(year);
                    list.AddRange(temp);

                }
                else if (IndicatorType.KeyValueIntCode == "CountProfessionalEducationFollowingNextYear")
                {
                    var temp = await this.GetListCountProfessionalEducationFollowingNextYear(year);
                    list.AddRange(temp);

                }
                else if (IndicatorType.KeyValueIntCode == "CountAbandonedTrainingStarted")
                {
                    var temp = await this.GetListCountAbandonedTrainingStarted(year);
                    list.AddRange(temp);

                }
                await this.CalculateRating(list, IndicatorType, year);
            }
        }
        public async Task StartCalculationCIPO(List<KeyValueVM> IndicatorTypes, int year)
        {
            var candidateProviders = await this.candidateProviderService.GetAllActiveCandidateProvidersAsync("LicensingCIPO");

            foreach (var IndicatorType in IndicatorTypes)
            {
                List<CalculateRatingVM> list = new List<CalculateRatingVM>();
                if (IndicatorType.KeyValueIntCode == "CountCertifiedMenCIPO")
                {
                    var temp = await this.CalculateCIPOResultByCountClientPerYear(year);
                    list.AddRange(temp);
                }

                await this.CalculateRating(list, IndicatorType, year);
            }
        }
        #endregion

        #region CalculatingMethods
        public async Task<List<CalculateRatingVM>> CalculateCPOResultByCountClientPerYear(int year)
        {
            var list = (from cp in this.contex.CandidateProviders

                        join c in this.contex.Courses on cp.IdCandidate_Provider equals c.IdCandidateProvider 
                        join cc in this.contex.ClientCourses on c.IdCourse equals cc.IdCourse                     

                        join kvLicencese in this.contex.KeyValues on cp.IdTypeLicense equals kvLicencese.IdKeyValue
                        join ccd in this.contex.ClientCourseDocuments on cc.IdClientCourse equals ccd.IdClientCourse
                        join kvd in this.contex.KeyValues on ccd.IdDocumentType equals kvd.IdKeyValue
                        join kvf in this.contex.KeyValues on cc.IdFinishedType equals kvf.IdKeyValue
                        join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                        where cp.IsActive == true 
                        && (cc.CourseJoinDate.HasValue ? cc.CourseJoinDate.Value.Year == year : false)
                        && kvLicencese.KeyValueIntCode == "LicensingCPO"     
                        && kvf.KeyValueIntCode == "Type1" 
                        && (kvs.KeyValueIntCode == "CourseStatusFinished" || kvs.KeyValueIntCode == "CourseStatusNow")

                        group cc by new
                        {
                            IdCandidate_Provider = cp.IdCandidate_Provider,
                            Year = cc != null ? cc.CourseJoinDate.HasValue ? cc.CourseJoinDate.Value.Year : year : year,
                        }
                         into grp
                        select new CalculateRatingVM
                        {
                            IdCandidate_Provider = grp.Key.IdCandidate_Provider,
                            Year = grp.Key.Year,
                            Count = grp.Where(x => x.CourseJoinDate.Value.Year == year).ToList().Count,
                        }).ToList();


            return list;
        }
        public async Task<List<CalculateRatingVM>> GetListCountDegreeProfessionalQualificationAsync(int year)
        {
            /*
             * select COUNT(*)
	   
                  from Training_Course c 
                  join Training_ClientCourse tcc on tcc.IdCourse=c.IdCourse
                  join Training_ClientCourseDocument tccd on tcc.IdClientCourse=tccd.IdClientCourse
                  join KeyValue kvd on tccd.IdDocumentType=kvd.IdKeyValue
                  join KeyValue kvf on tcc.IdFinishedType=kvf.IdKeyValue
                  join KeyValue kvs on c.IdStatus=kvs.IdKeyValue
                  join KeyValue kvft on tcc.IdFinishedType=kvft.IdKeyValue
   
                 where c.IdCandidateProvider=1041
                   and kvf.KeyValueIntCode='Type1' -- завършил с документ, KeyTypeIntCode='CourseFinishedType'
                   and kvd.KeyValueIntCode='ProfessionalQualification' -- Професионално обучение за придобиване на СПК, KeyTypeIntCode='TypeFrameworkProgram'
                   and c.EndDate like '2023%'---- датата на документа трябва да е от текущата година
                   and  kvs.KeyValueIntCode in ('CourseStatusFinished', 'CourseStatusNow')
             */

            var list = (from cp in this.contex.CandidateProviders
                        join c in this.contex.Courses on cp.IdCandidate_Provider equals c.IdCandidateProvider
                        join cc in this.contex.ClientCourses on c.IdCourse equals cc.IdCourse
                        join ccd in this.contex.ClientCourseDocuments on cc.IdClientCourse equals ccd.IdClientCourse
                        join kvd in this.contex.KeyValues on ccd.IdDocumentType equals kvd.IdKeyValue
                        join kvf in this.contex.KeyValues on cc.IdFinishedType equals kvf.IdKeyValue
                        join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                        where
                        cp.IsActive == true &&
                        ccd.DocumentDate.HasValue && ccd.DocumentDate.Value.Year == year &&
                        kvf.KeyValueIntCode == "Type1" && //завършил с документ
                        kvd.KeyValueIntCode == "ProfessionalQualification" &&
                        (kvs.KeyValueIntCode == "CourseStatusFinished" || kvs.KeyValueIntCode == "CourseStatusNow")

                        group cc by new
                        {
                            IdCandidateProvider = cp.IdCandidate_Provider,
                            Year = ccd.DocumentDate.HasValue ? ccd.DocumentDate.Value.Year : year
                        } into grp
                        select new CalculateRatingVM
                        {
                            IdCandidate_Provider = grp.Key.IdCandidateProvider,
                            Year = grp.Key.Year,
                            Count = grp.Count()
                        }).ToList();

            return list;
        }
        public async Task<List<CalculateRatingVM>> GetListCountProfessionalQualificationPartProfession(int year)
        {
            var list = (from cp in this.contex.CandidateProviders
                        join c in this.contex.Courses on cp.IdCandidate_Provider equals c.IdCandidateProvider
                        join cc in this.contex.ClientCourses on c.IdCourse equals cc.IdCourse
                        join ccd in this.contex.ClientCourseDocuments on cc.IdClientCourse equals ccd.IdClientCourse
                        join kvd in this.contex.KeyValues on ccd.IdDocumentType equals kvd.IdKeyValue
                        join kvf in this.contex.KeyValues on cc.IdFinishedType equals kvf.IdKeyValue
                        join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                        where
                        cp.IsActive == true &&
                        ccd.DocumentDate.HasValue && ccd.DocumentDate.Value.Year == year &&
                        kvf.KeyValueIntCode == "Type1" && //завършил с документ, KeyTypeIntCode = 'CourseFinishedType'
                        kvd.KeyValueIntCode == "PartProfession" && //-- Професионално обучение по част от професия, KeyTypeIntCode = 'TypeFrameworkProgram'
                        (kvs.KeyValueIntCode == "CourseStatusFinished" || kvs.KeyValueIntCode == "CourseStatusNow")

                        group cc by new
                        {
                            IdCandidateProvider = cp.IdCandidate_Provider,
                            Year = ccd.DocumentDate.HasValue ? ccd.DocumentDate.Value.Year : year
                        } into grp
                        select new CalculateRatingVM
                        {
                            IdCandidate_Provider = grp.Key.IdCandidateProvider,
                            Year = grp.Key.Year,
                            Count = grp.Count()
                        }).ToList();

            return list;
        }
        public async Task<List<CalculateRatingVM>> GetListCountDegreeProfessionalQualificationValidation(int year)
        {
            var list = (from cp in this.contex.CandidateProviders
                        join c in this.contex.Courses on cp.IdCandidate_Provider equals c.IdCandidateProvider
                        join cc in this.contex.ClientCourses on c.IdCourse equals cc.IdCourse
                        join ccd in this.contex.ClientCourseDocuments on cc.IdClientCourse equals ccd.IdClientCourse
                        join kvd in this.contex.KeyValues on ccd.IdDocumentType equals kvd.IdKeyValue
                        join kvf in this.contex.KeyValues on cc.IdFinishedType equals kvf.IdKeyValue
                        join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                        where
                        cp.IsActive == true &&
                        ccd.DocumentDate.HasValue && ccd.DocumentDate.Value.Year == year &&
                        kvf.KeyValueIntCode == "Type1" && //завършил с документ, KeyTypeIntCode = 'CourseFinishedType'
                        kvd.KeyValueIntCode == "ValidationOfProfessionalQualifications" && //-- Професионално обучение по част от професия, KeyTypeIntCode = 'TypeFrameworkProgram'
                        (kvs.KeyValueIntCode == "CourseStatusFinished" || kvs.KeyValueIntCode == "CourseStatusNow")

                        group cc by new
                        {
                            IdCandidateProvider = cp.IdCandidate_Provider,
                            Year = ccd.DocumentDate.HasValue ? ccd.DocumentDate.Value.Year : year
                        } into grp
                        select new CalculateRatingVM
                        {
                            IdCandidate_Provider = grp.Key.IdCandidateProvider,
                            Year = grp.Key.Year,
                            Count = grp.Count()
                        }).ToList();


            return list;
        }
        public async Task<List<CalculateRatingVM>> GetListCountProfessionalQualificationPartProfessionValidation(int year)
        {
            var list = (from cp in this.contex.CandidateProviders
                        join c in this.contex.Courses on cp.IdCandidate_Provider equals c.IdCandidateProvider
                        join cc in this.contex.ClientCourses on c.IdCourse equals cc.IdCourse
                        join ccd in this.contex.ClientCourseDocuments on cc.IdClientCourse equals ccd.IdClientCourse
                        join kvd in this.contex.KeyValues on ccd.IdDocumentType equals kvd.IdKeyValue
                        join kvf in this.contex.KeyValues on cc.IdFinishedType equals kvf.IdKeyValue
                        join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                        where
                        cp.IsActive == true &&
                        ccd.DocumentDate.HasValue && ccd.DocumentDate.Value.Year == year &&
                        kvf.KeyValueIntCode == "Type1" && //завършил с документ, KeyTypeIntCode = 'CourseFinishedType'
                        kvd.KeyValueIntCode == "ValidationOfPartOfProfession" && //-- Професионално обучение по част от професия, KeyTypeIntCode = 'TypeFrameworkProgram'
                        (kvs.KeyValueIntCode == "CourseStatusFinished" || kvs.KeyValueIntCode == "CourseStatusNow")

                        group cc by new
                        {
                            IdCandidateProvider = cp.IdCandidate_Provider,
                            Year = ccd.DocumentDate.HasValue ? ccd.DocumentDate.Value.Year : year
                        } into grp
                        select new CalculateRatingVM
                        {
                            IdCandidate_Provider = grp.Key.IdCandidateProvider,
                            Year = grp.Key.Year,
                            Count = grp.Count()
                        }).ToList();

            return list;
        }
        public async Task<List<CalculateRatingVM>> GetListCountProfessionalEducationFollowingNextYear(int year)
        {
            year++; //Следваща година

            var list = (from cp in this.contex.CandidateProviders
                        join c in this.contex.Courses on cp.IdCandidate_Provider equals c.IdCandidateProvider
                        join cc in this.contex.ClientCourses on c.IdCourse equals cc.IdCourse
                        join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                        where
                        cp.IsActive == true &&
                        c.EndDate.HasValue && c.EndDate.Value.Year == year &&
                        kvs.KeyValueIntCode != "CourseStatusFinished" &&
                        cc.IdFinishedType == null // --KeyTypeIntCode = CourseFinishedType, да не включва курсисти със статус "Прекъснал по уважителни причини" и "Прекъснал по неуважителни причини"

                        group cc by new
                        {
                            IdCandidateProvider = cp.IdCandidate_Provider,
                            Year = year
                        } into grp
                        select new CalculateRatingVM
                        {
                            IdCandidate_Provider = grp.Key.IdCandidateProvider,
                            Year = grp.Key.Year,
                            Count = grp.Count()
                        }).ToList();


            return list;
        }
        public async Task<List<CalculateRatingVM>> GetListCountAbandonedTrainingStarted(int year)
        {
            var list = (from cp in this.contex.CandidateProviders
                        join tc in this.contex.Clients on cp.IdCandidate_Provider equals tc.IdCandidateProvider
                        join tcc in this.contex.ClientCourses on tc.IdClient equals tcc.IdClient
                        join tco in this.contex.Courses on tcc.IdCourse equals tco.IdCourse
                        join kvc in this.contex.KeyValues on tco.IdStatus equals kvc.IdKeyValue

                        join kvf in this.contex.KeyValues on tcc.IdFinishedType equals kvf.IdKeyValue into grFinishedType
                        from subFinishedType in grFinishedType.DefaultIfEmpty()


                        where cp.IsActive == true &&
                              tco.EndDate.HasValue &&
                              tco.EndDate.Value.Year == year && //-- дата на приключване на курса да е текущата календарна година
                              subFinishedType != null &&
                              (subFinishedType.KeyValueIntCode == "Type2" || subFinishedType.KeyValueIntCode == "Type3")//  --KeyTypeIntCode = CourseFinishedType, да включва курсисти със статус "Прекъснал по уважителни причини" и "Прекъснал по неуважителни причини"

                        group tc by new
                        {
                            IdCandidateProvider = cp.IdCandidate_Provider,
                            Year = year
                        } into grp
                        select new CalculateRatingVM
                        {
                            IdCandidate_Provider = grp.Key.IdCandidateProvider,
                            Year = grp.Key.Year,
                            Count = grp.Count()
                        }).ToList();


            return list;
        }
        public async Task<List<CalculateRatingVM>> CalculateCIPOResultByCountClientPerYear(int year)
        {
            var list = (from cp in this.contex.CandidateProviders

                        join cc in this.contex.ConsultingClients on cp.IdCandidate_Provider equals cc.IdCandidateProvider
                        join c in this.contex.Consultings on cc.IdConsultingClient equals c.IdConsultingClient

                        join kvLicencese in this.contex.KeyValues on cp.IdTypeLicense equals kvLicencese.IdKeyValue

                        //join kvd in this.contex.KeyValues on ccd.IdDocumentType equals kvd.IdKeyValue
                        join kvf in this.contex.KeyValues on cc.IdFinishedType equals kvf.IdKeyValue
                        //join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                        where cp.IsActive == true
                        && (cc.StartDate.HasValue ? cc.StartDate.Value.Year == year : false)
                        && kvLicencese.KeyValueIntCode == "LicensingCIPO"
                        && (kvf.KeyValueIntCode == "Type9" || kvf.KeyValueIntCode == "Type10")  
                        //&& (kvs.KeyValueIntCode == "CourseStatusFinished" || kvs.KeyValueIntCode == "CourseStatusNow")

                        group cc by new
                        {
                            IdCandidate_Provider = cp.IdCandidate_Provider,
                            Year = cc != null ? cc.StartDate.HasValue ? cc.StartDate.Value.Year : year : year,
                        }
                         into grp
                        select new CalculateRatingVM
                        {
                            IdCandidate_Provider = grp.Key.IdCandidate_Provider,
                            Year = grp.Key.Year,
                            Count = grp.Where(x => x.StartDate.Value.Year == year).ToList().Count,
                        }).ToList();


            return list;
        }
        #endregion

        #region SelfAssessment

        /// <summary>
        /// Придобили степен на професионална квалификация
        /// </summary>
        /// <param name="idCandidateProvider"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<int> CountDegreeProfessionalQualificationAsync(int idCandidateProvider, int year)
        {
            int count = 0;


            /*
             * select COUNT(*)
	   
                  from Training_Course c 
                  join Training_ClientCourse tcc on tcc.IdCourse=c.IdCourse
                  join Training_ClientCourseDocument tccd on tcc.IdClientCourse=tccd.IdClientCourse
                  join KeyValue kvd on tccd.IdDocumentType=kvd.IdKeyValue
                  join KeyValue kvf on tcc.IdFinishedType=kvf.IdKeyValue
                  join KeyValue kvs on c.IdStatus=kvs.IdKeyValue
                  join KeyValue kvft on tcc.IdFinishedType=kvft.IdKeyValue
   
                 where c.IdCandidateProvider=1041
                   and kvf.KeyValueIntCode='Type1' -- завършил с документ, KeyTypeIntCode='CourseFinishedType'
                   and kvd.KeyValueIntCode='ProfessionalQualification' -- Професионално обучение за придобиване на СПК, KeyTypeIntCode='TypeFrameworkProgram'
                   and c.EndDate like '2023%'---- датата на документа трябва да е от текущата година
                   and  kvs.KeyValueIntCode in ('CourseStatusFinished', 'CourseStatusNow')
             */

            count = (from c in this.contex.Courses
                     join cc in this.contex.ClientCourses on c.IdCourse equals cc.IdCourse
                     join ccd in this.contex.ClientCourseDocuments on cc.IdClientCourse equals ccd.IdClientCourse
                     join kvd in this.contex.KeyValues on ccd.IdDocumentType equals kvd.IdKeyValue
                     join kvf in this.contex.KeyValues on cc.IdFinishedType equals kvf.IdKeyValue
                     join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                     where
                     c.IdCandidateProvider == idCandidateProvider &&
                     ccd.DocumentDate.HasValue && ccd.DocumentDate.Value.Year == year &&
                     kvf.KeyValueIntCode == "Type1" && //завършил с документ
                     kvd.KeyValueIntCode == "ProfessionalQualification" &&
                     (kvs.KeyValueIntCode == "CourseStatusFinished" || kvs.KeyValueIntCode == "CourseStatusNow")

                     select c
                     ).Count();

            return count;
        }


        /// <summary>
        /// Придобили професионална квалификация по част от професията
        /// </summary>
        /// <param name="idCandidateProvider"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<int> CountProfessionalQualificationPartProfession(int idCandidateProvider, int year)
        {
            int count = 0;





            count = (from c in this.contex.Courses
                     join cc in this.contex.ClientCourses on c.IdCourse equals cc.IdCourse
                     join ccd in this.contex.ClientCourseDocuments on cc.IdClientCourse equals ccd.IdClientCourse
                     join kvd in this.contex.KeyValues on ccd.IdDocumentType equals kvd.IdKeyValue
                     join kvf in this.contex.KeyValues on cc.IdFinishedType equals kvf.IdKeyValue
                     join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                     where
                     c.IdCandidateProvider == idCandidateProvider &&
                     ccd.DocumentDate.HasValue && ccd.DocumentDate.Value.Year == year &&
                     kvf.KeyValueIntCode == "Type1" && //завършил с документ, KeyTypeIntCode = 'CourseFinishedType'
                     kvd.KeyValueIntCode == "PartProfession" && //-- Професионално обучение по част от професия, KeyTypeIntCode = 'TypeFrameworkProgram'
                     (kvs.KeyValueIntCode == "CourseStatusFinished" || kvs.KeyValueIntCode == "CourseStatusNow")

                     select c
                     ).Count();


            return count;
        }



        /// <summary>
        /// Придобили степен на професионална квалификация чрез валидиране
        /// </summary>
        /// <param name="idCandidateProvider"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<int> CountDegreeProfessionalQualificationValidation(int idCandidateProvider, int year)
        {
            int count = 0;





            count = (from c in this.contex.Courses
                     join cc in this.contex.ClientCourses on c.IdCourse equals cc.IdCourse
                     join ccd in this.contex.ClientCourseDocuments on cc.IdClientCourse equals ccd.IdClientCourse
                     join kvd in this.contex.KeyValues on ccd.IdDocumentType equals kvd.IdKeyValue
                     join kvf in this.contex.KeyValues on cc.IdFinishedType equals kvf.IdKeyValue
                     join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                     where
                     c.IdCandidateProvider == idCandidateProvider &&
                     ccd.DocumentDate.HasValue && ccd.DocumentDate.Value.Year == year &&
                     kvf.KeyValueIntCode == "Type1" && //завършил с документ, KeyTypeIntCode = 'CourseFinishedType'
                     kvd.KeyValueIntCode == "ValidationOfProfessionalQualifications" && //-- Валидиране на степен на професионална квалификация, KeyTypeIntCode = 'TypeFrameworkProgram'
                     (kvs.KeyValueIntCode == "CourseStatusFinished" || kvs.KeyValueIntCode == "CourseStatusNow")

                     select c
                    ).Count();


            return count;
        }


        /// <summary>
        /// Придобили професионална квалификация по част от професията чрез валидиране
        /// </summary>
        /// <param name="idCandidateProvider"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<int> CountProfessionalQualificationPartProfessionValidation(int idCandidateProvider, int year)
        {
            int count = 0;


            count = (from c in this.contex.Courses
                     join cc in this.contex.ClientCourses on c.IdCourse equals cc.IdCourse
                     join ccd in this.contex.ClientCourseDocuments on cc.IdClientCourse equals ccd.IdClientCourse
                     join kvd in this.contex.KeyValues on ccd.IdDocumentType equals kvd.IdKeyValue
                     join kvf in this.contex.KeyValues on cc.IdFinishedType equals kvf.IdKeyValue
                     join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                     where
                     c.IdCandidateProvider == idCandidateProvider &&
                     ccd.DocumentDate.HasValue && ccd.DocumentDate.Value.Year == year &&
                     kvf.KeyValueIntCode == "Type1" && //завършил с документ, KeyTypeIntCode = 'CourseFinishedType'
                     kvd.KeyValueIntCode == "ValidationOfPartOfProfession" && //-- Валидиране на част от професия, KeyTypeIntCode = 'TypeFrameworkProgram'
                     (kvs.KeyValueIntCode == "CourseStatusFinished" || kvs.KeyValueIntCode == "CourseStatusNow")

                     select c
                   ).Count();


            return count;
        }

        /// <summary>
        /// Получили правоспособност	
        /// </summary>
        /// <param name="idCandidateProvider"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<int> CountCourseCompetenceUnderRegulationsOneAndSeven(int idCandidateProvider, int year)
        {
            int count = 0;
                  
            count = (from c in this.contex.Courses
                     join cc in this.contex.ClientCourses on c.IdCourse equals cc.IdCourse
                     join ccd in this.contex.ClientCourseDocuments on cc.IdClientCourse equals ccd.IdClientCourse
                     join kvd in this.contex.KeyValues on ccd.IdDocumentType equals kvd.IdKeyValue
                     join kvf in this.contex.KeyValues on cc.IdFinishedType equals kvf.IdKeyValue
                     join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                     where
                     c.IdCandidateProvider == idCandidateProvider &&
                     ccd.DocumentDate.HasValue && ccd.DocumentDate.Value.Year == year &&
                     kvf.KeyValueIntCode == "Type1" && //завършил с документ, KeyTypeIntCode = 'CourseFinishedType'
                     (kvd.KeyValueIntCode == "CourseRegulation1And7" || kvd.KeyValueIntCode == "CourseRegulation1" || kvd.KeyValueIntCode == "CourseRegulation7") && //-- Курс за правоспособност по Наредба № 1 и Наредба № 7, KeyTypeIntCode = 'TypeFrameworkProgram'
                     (kvs.KeyValueIntCode == "CourseStatusFinished" || kvs.KeyValueIntCode == "CourseStatusNow")

                     select c
                   ).Count();


            return count;
        }

        /// <summary>
        /// Продължаващи професионално обучение през следващата година
        /// </summary>
        /// <param name="idCandidateProvider"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<int> CountProfessionalEducationFollowingNextYear(int idCandidateProvider, int year)
        {
            int count = 0;




            year++; //Следваща година

            count = (from c in this.contex.Courses
                     join cc in this.contex.ClientCourses on c.IdCourse equals cc.IdCourse
                     join kvs in this.contex.KeyValues on c.IdStatus equals kvs.IdKeyValue
                     where
                     c.IdCandidateProvider == idCandidateProvider &&
                     c.EndDate.HasValue && c.EndDate.Value.Year == year &&
                     kvs.KeyValueIntCode != "CourseStatusFinished" &&
                     cc.IdFinishedType == null // --KeyTypeIntCode = CourseFinishedType, да не включва курсисти със статус "Прекъснал по уважителни причини" и "Прекъснал по неуважителни причини"

                     select c
                  ).Count();


            return count;
        }

        /// <summary>
        /// Незавършили успешно професионалното обучение
        /// </summary>
        /// <param name="idCandidateProvider"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<int> CountNonSuccessfulVocationalTraining(int idCandidateProvider, int year)
        {
            int count = 0;

            count = (from tc in this.contex.Clients
                     join tcc in this.contex.ClientCourses on tc.IdClient equals tcc.IdClient
                     join tco in this.contex.Courses on tcc.IdCourse equals tco.IdCourse
                     join kvc in this.contex.KeyValues on tco.IdStatus equals kvc.IdKeyValue
                     join kvf in this.contex.KeyValues on tcc.IdFinishedType equals kvf.IdKeyValue                    
                     where
                     tc.IdCandidateProvider == idCandidateProvider &&
                     tcc.CourseJoinDate != null &&       // курсистът е започнал курса
                     tco.EndDate.Value.Year == year &&   // дата на приключване на курса да е текущата календарна година
                     kvf.KeyValueIntCode == "Type4"      // KeyTypeIntCode=CourseFinishedType, да включва курсисти със статус "Завършил курса, но не положил изпита" 

                     select tc
                   ).Count();


            return count;
        }
        /// <summary>
        /// Отказали се от започнатото професионално обучение
        /// </summary>
        /// <param name="idCandidateProvider"></param>
        /// <param name="year"></param>
        /// <returns></returns>

        public async Task<int> CountAbandonedTrainingStarted(int idCandidateProvider, int year)
        {
            int count = 0;


            count = (from tc in this.contex.Clients
                     join tcc in this.contex.ClientCourses on tc.IdClient equals tcc.IdClient
                     join tco in this.contex.Courses on tcc.IdCourse equals tco.IdCourse
                     join kvc in this.contex.KeyValues on tco.IdStatus equals kvc.IdKeyValue

                     join kvf in this.contex.KeyValues on tcc.IdFinishedType equals kvf.IdKeyValue into grFinishedType
                     from subFinishedType in grFinishedType.DefaultIfEmpty()


                     where tc.IdCandidateProvider == idCandidateProvider &&
                           tco.EndDate.HasValue &&
                           tco.EndDate.Value.Year == year && //-- дата на приключване на курса да е текущата календарна година
                           subFinishedType != null &&
                           (subFinishedType.KeyValueIntCode == "Type2" || subFinishedType.KeyValueIntCode == "Type3")//  --KeyTypeIntCode = CourseFinishedType, да включва курсисти със статус "Прекъснал по уважителни причини" и "Прекъснал по неуважителни причини"
                     select tc
                     ).Count();


            return count;
        }


        #endregion
    }

    public class CalculateRatingVM
    {
        public int IdCandidate_Provider { get; set; } = 0;
        public int Year { get; set; } = 0;
        public int Count { get; set; } = 0;
    }

}
