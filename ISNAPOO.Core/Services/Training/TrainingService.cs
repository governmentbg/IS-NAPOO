using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.SPPOO;
using Data.Models.Data.SqlView.Reports;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Syncfusion.XlsIO;
using ISNAPOO.Core.ViewModels.Request;
using Data.Models.Data.Request;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ISNAPOO.Core.Contracts.EKATTE;
using System.Text.RegularExpressions;
using ISNAPOO.Common.HelperClasses;
using System.Xml.Serialization;
using ISNAPOO.Core.XML.Course;
using System.Xml;
using ISNAPOO.Core.Contracts.SPPOO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using ISNAPOO.Core.Contracts.Candidate;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.Archive;
using DocuWorkService;
using DocuServiceReference;
using Syncfusion.Compression.Zip;
using System.Collections;
using Data.Models.Data.Common;
using ISNAPOO.Core.HelperClasses;
using System.Linq.Expressions;
using ISNAPOO.Core.ViewModels.Register;
using System.IO.Compression;
using Data.Models.DB;

namespace ISNAPOO.Core.Services.Training
{
    public class TrainingService : BaseService, ITrainingService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        private readonly ILocationService locationService;
        private readonly IMunicipalityService municipalityService;
        private readonly ISpecialityService specialityService;
        private readonly IApplicationUserService applicationUserService;
        private readonly INotificationService notificationService;
        private readonly ICandidateProviderService candidateProviderService;
        private readonly IUploadFileService uploadFileService;
        private readonly ILogger<TrainingService> _logger;
        private readonly IDocuService docuService;
        private readonly ApplicationDbContext applicationDbContext;

        #region Course status KV

        private IEnumerable<KeyValueVM> courseTypesSource;
        private KeyValueVM kvUpcomingCourse;
        private KeyValueVM kvCurrentCourse;
        private KeyValueVM kvCompletedCourse;

        #endregion Course status KV

        #region Candidate provider premises KV

        private IEnumerable<KeyValueVM> mtbStatusSource;
        private KeyValueVM kvMTBActive;

        #endregion Candidate provider premises KV

        #region Index import Curriculum
        private int professionalTrainingIndex = 0;
        private int subjectIndex = 1;
        private int topicIndex = 2;
        private int theoryIndex = 3;
        private int practiceIndex = 4;

        #endregion Index import Curriculum

        private IEnumerable<KeyValueVM> trainingCourseTypeSource;
        private KeyValueVM kvProfessionalTrainingForAcquiringTheSPK;
        private KeyValueVM kvProfessionalValidationForAcquiringTheSPK;
        private KeyValueVM kvVocationalTrainingInPartOfProfession;
        private KeyValueVM kvVocationalValidationInPartOfProfession;

        public TrainingService(IRepository repository,
            IDataSourceService dataSourceService,
            ILogger<TrainingService> logger,
            AuthenticationStateProvider authStateProvider,
            ILocationService locationService,
             ISpecialityService specialityService,
            IMunicipalityService municipalityService,
            INotificationService notificationService,
            ICandidateProviderService candidateProviderService,
            IUploadFileService uploadFileService,
            IDocuService docuService,
            IApplicationUserService applicationUserService,
            ApplicationDbContext applicationDbContext)
            : base(repository, authStateProvider)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
            this._logger = logger;
            this.locationService = locationService;
            this.municipalityService = municipalityService;
            this.specialityService = specialityService;
            this.notificationService = notificationService;
            this.candidateProviderService = candidateProviderService;
            this.applicationUserService = applicationUserService;
            this.uploadFileService = uploadFileService;
            this.docuService = docuService;
            this.applicationDbContext = applicationDbContext;
            this.LoadKVSources();
        }

        // зарежда данни за номенклатури
        private void LoadKVSources()
        {
            this.courseTypesSource = this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseStatus").Result;
            this.kvUpcomingCourse = this.courseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "CourseStatusUpcoming");
            this.kvCurrentCourse = this.courseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "CourseStatusNow");
            this.kvCompletedCourse = this.courseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "CourseStatusFinished");

            this.mtbStatusSource = this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseStatus").Result;
            this.kvMTBActive = this.mtbStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Active");

            this.trainingCourseTypeSource = this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram").Result;
            this.kvProfessionalTrainingForAcquiringTheSPK = this.trainingCourseTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "ProfessionalQualification");
            this.kvProfessionalValidationForAcquiringTheSPK = this.trainingCourseTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfProfessionalQualifications");
            this.kvVocationalTrainingInPartOfProfession = this.trainingCourseTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "PartProfession");
            this.kvVocationalValidationInPartOfProfession = this.trainingCourseTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfPartOfProfession");
        }

        #region Training program

        public async Task<IEnumerable<ProgramVM>> GetAllActiveProgramsByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<Program>(x => x.IdCandidateProvider == idCandidateProvider && !x.IsDeleted && !x.IsService);
            var dataAsVM = await data.To<ProgramVM>(x => x.FrameworkProgram.FrameworkProgramFormEducations, x => x.Speciality.Profession, x => x.CandidateProvider, x => x.TrainingCurriculums.Where(x => x.IdCourse == null)).ToListAsync();

            return dataAsVM.OrderBy(x => x.OldId).ThenBy(x => x.Speciality.CodeAsIntForOrderBy).ToList();
        }

        public async Task<IEnumerable<ProgramVM>> GetAllActiveLegalCapacityProgramsByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var kvRegulation = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "CourseRegulation1And7");
            var data = this.repository.AllReadonly<Program>(x => x.IdCandidateProvider == idCandidateProvider && !x.IsDeleted && x.IdCourseType == kvRegulation.IdKeyValue && !x.IsService);
            var dataAsVM = await data.To<ProgramVM>(x => x.FrameworkProgram, x => x.Speciality.Profession, x => x.CandidateProvider, x => x.TrainingCurriculums.Where(x => x.IdCourse == null)).ToListAsync();

            return dataAsVM.OrderBy(x => x.OldId).ThenBy(x => x.Speciality.CodeAsIntForOrderBy).ToList();
        }

        public async Task<ResultContext<NoResult>> MarkProgramAsDeletedByIdProgramAsync(int idProgram)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var data = this.repository.AllReadonly<Course>(x => x.IdProgram == idProgram);

                if (data != null && data.Count() > 0)
                {
                    resultContext.AddErrorMessage("Не можете да изтриете програма за обучение, по която има стартиран курс!");
                }
                else
                {
                    var entryFromDb = await this.repository.GetByIdAsync<Program>(idProgram);
                    if (entryFromDb is not null)
                    {
                        entryFromDb.IsDeleted = true;

                        this.repository.Update<Program>(entryFromDb);
                        await this.repository.SaveChangesAsync();

                        resultContext.AddMessage("Записът е изтрит успешно!");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<IEnumerable<CandidateProviderSpecialityVM>> GetCandidateProviderSpecialitiesByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == idCandidateProvider);

            return await data.To<CandidateProviderSpecialityVM>().ToListAsync();
        }

        public async Task<IEnumerable<FrameworkProgramVM>> GetFrameworkProgramByIdVQSAndByIdTypeFrameworkProgramAsync(int idVQS, int idTypeFrameworkProgram)
        {
            var data = this.repository.AllReadonly<FrameworkProgram>(x => x.IdVQS == idVQS && x.IdTypeFrameworkProgram == idTypeFrameworkProgram);

            return await data.To<FrameworkProgramVM>().ToListAsync();
        }

        public async Task<ResultContext<ProgramVM>> CreateTrainingProgramAsync(ResultContext<ProgramVM> inputContext, int? idCourseType = null)
        {
            var model = inputContext.ResultContextObject;
            try
            {
                if (model.FrameworkProgram is not null)
                {
                    model.IdMinimumLevelEducation = model.FrameworkProgram.IdMinimumLevelEducation;
                }

                model.IdCourseType = idCourseType != null ? idCourseType.Value : model.FrameworkProgram.IdTypeFrameworkProgram;
                var entryForDb = model.To<Program>();
                entryForDb.FrameworkProgram = null;
                entryForDb.Courses = null;
                entryForDb.CandidateProvider = null;
                entryForDb.Speciality = null;
                entryForDb.TrainingCurriculums = null;

                await this.repository.AddAsync<Program>(entryForDb);
                await this.repository.SaveChangesAsync();

                model.IdProgram = entryForDb.IdProgram;
                model.ModifyDate = entryForDb.ModifyDate;
                model.CreationDate = entryForDb.CreationDate;
                model.IdModifyUser = entryForDb.IdModifyUser;
                model.IdCreateUser = entryForDb.IdCreateUser;

                inputContext.AddMessage("Записът е успешен!");

                // копира учебната програма, само ако програмата за обучение не е от модула за правоспособност
                if (!idCourseType.HasValue)
                {
                    await this.CopyCurriculumAsync(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                inputContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return inputContext;
        }

        public async Task<ResultContext<ProgramVM>> UpdateTrainingProgramAsync(ResultContext<ProgramVM> inputContext)
        {
            var model = inputContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<Program>(model.IdProgram);
                if (entryFromDb is not null)
                {
                    model.IdCreateUser = entryFromDb.IdCreateUser;
                    model.CreationDate = entryFromDb.CreationDate;
                    entryFromDb = model.To<Program>();
                    entryFromDb.Speciality = null;
                    entryFromDb.FrameworkProgram = null;
                    entryFromDb.Courses = null;
                    entryFromDb.CandidateProvider = null;
                    entryFromDb.TrainingCurriculums = null;

                    this.repository.Update<Program>(entryFromDb);
                    await this.repository.SaveChangesAsync();

                    model.ModifyDate = entryFromDb.ModifyDate;
                    model.CreationDate = entryFromDb.CreationDate;
                    model.IdModifyUser = entryFromDb.IdModifyUser;
                    model.IdCreateUser = entryFromDb.IdCreateUser;

                    inputContext.AddMessage("Записът е успешен!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                inputContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return inputContext;
        }

        public async Task<ProgramVM> GetTrainingProgramByIdAsync(int idTrainingProgram)
        {
            var data = this.repository.AllReadonly<Program>(x => x.IdProgram == idTrainingProgram);

            return await data.To<ProgramVM>(x => x.FrameworkProgram.FrameworkProgramFormEducations, x => x.Speciality.Profession, x => x.CandidateProvider, x => x.TrainingCurriculums.Where(x => x.IdCourse == null)).FirstOrDefaultAsync();
        }

        public async Task<ProgramVM> GetTrainingProgramByFrameworkProgramIdAsync(int idFrameworkProgram)
        {
            var data = this.repository.AllReadonly<Program>(x => x.IdFrameworkProgram == idFrameworkProgram);

            return await data.To<ProgramVM>(x => x.FrameworkProgram, x => x.Speciality.Profession, x => x.CandidateProvider, x => x.TrainingCurriculums.Where(x => x.IdCourse == null)).FirstOrDefaultAsync();
        }

        public async Task<ProgramVM> GetTrainingProgramByIdWithoutAnythingIncludedAsync(int idTrainingProgram)
        {
            var data = this.repository.AllReadonly<Program>(x => x.IdProgram == idTrainingProgram);

            return await data.To<ProgramVM>().FirstOrDefaultAsync();
        }

        public async Task UpdateTrainingProgramHoursByIdProgramAsync(int idProgram, int mandatoryHours, int selectableHours)
        {
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<Program>(idProgram);
                if (entryFromDb is not null)
                {
                    entryFromDb.MandatoryHours = mandatoryHours;
                    entryFromDb.SelectableHours = selectableHours;

                    this.repository.Update<Program>(entryFromDb);
                    await this.repository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task<bool> DoesProgramWithSameNumberFrameworkProgramAndSpecialityExistAsync(ProgramVM program)
        {
            var programs = await this.repository.AllReadonly<Program>(x => x.IdCandidateProvider == program.IdCandidateProvider).Where(x => !string.IsNullOrEmpty(x.ProgramNumber)).ToListAsync();

            return programs.Any(x => x.ProgramNumber == program.ProgramNumber && x.IdFrameworkProgram == program.IdFrameworkProgram && x.IdSpeciality == program.IdSpeciality && x.IdProgram != program.IdProgram);
        }

        #endregion Training program

        #region Training curriculum

        public async Task<TrainingCurriculumVM> GetTrainingCurriculumByIdAsync(int idTrainingCurriculum)
        {
            var data = this.repository.AllReadonly<TrainingCurriculum>(x => x.IdTrainingCurriculum == idTrainingCurriculum);

            return await data.To<TrainingCurriculumVM>().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TrainingCurriculumVM>> GetTrainingCurriculumByIdProgramAsync(int idProgram)
        {
            var data = this.repository.AllReadonly<TrainingCurriculum>(x => x.IdProgram == idProgram && x.IdCourse == null);

            return await data.OrderBy(x => x.IdProfessionalTraining).ThenBy(x => x.Subject).ThenBy(x => x.Topic).To<TrainingCurriculumVM>(x => x.TrainingCurriculumERUs).ToListAsync();
        }

        public async Task<IEnumerable<TrainingCurriculumVM>> GetTrainingCurriculumByIdCourseAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == idCourse);

            return await data.OrderBy(x => x.IdProfessionalTraining).ThenBy(x => x.OldId).ThenBy(x => x.Subject).ThenBy(x => x.Topic).ThenBy(x => x.Theory).ThenBy(x => x.Practice).To<TrainingCurriculumVM>(x => x.TrainingCurriculumERUs).ToListAsync();
        }

        public async Task<IEnumerable<TrainingCurriculumVM>> GetTrainingCurriculumsWithoutAnythingIncludedByIdCourseAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == idCourse);
            var datAsVM = await data.To<TrainingCurriculumVM>().ToListAsync();
            var professionalTrainingTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            foreach (var curriculum in datAsVM)
            {
                var professionalTrainingValue = professionalTrainingTypesSource.FirstOrDefault(x => x.IdKeyValue == curriculum.IdProfessionalTraining);
                if (professionalTrainingValue is not null)
                {
                    curriculum.ProfessionalTraining = professionalTrainingValue.DefaultValue1;
                }
            }

            return datAsVM;
        }

        public async Task<IEnumerable<TrainingCurriculumVM>> GetTrainingCurriculumWithoutAnythingIncludedByIdCourseAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == idCourse);

            return await data.OrderBy(x => x.IdProfessionalTraining).ThenBy(x => x.Subject).ThenBy(x => x.Topic).To<TrainingCurriculumVM>().ToListAsync();
        }

        public async Task<TrainingCurriculumCombinedVM> GetTrainingCurriculumCombinedByIdCourseAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == idCourse);
            var professionalTrainingTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            TrainingCurriculumCombinedVM trainingCurriculumCombinedVM = new TrainingCurriculumCombinedVM();

            var idKeyValueB = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "B").FirstOrDefault().IdKeyValue;
            var idKeyValueА1 = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "A1").FirstOrDefault().IdKeyValue;

            trainingCurriculumCombinedVM.theory = data.Where(x => x.IdProfessionalTraining != idKeyValueB).Where(x => x.Theory.HasValue ? x.Theory != 0 : false).To<TrainingCurriculumVM>().ToList();
            trainingCurriculumCombinedVM.educationalPractice = data.Where(x => x.IdProfessionalTraining != idKeyValueB && x.IdProfessionalTraining != idKeyValueА1).Where(x => x.Practice.HasValue ? x.Practice != 0 : false).To<TrainingCurriculumVM>().ToList();
            trainingCurriculumCombinedVM.productionPractice = data.Where(x => x.IdProfessionalTraining == idKeyValueB).To<TrainingCurriculumVM>().ToList();

            return trainingCurriculumCombinedVM;
        }
        private async Task<IEnumerable<CandidateCurriculumVM>> GetCandidateCurriculumByIdCandidateProviderAndIdSpecialityAsync(int idCandidateProvider, int idSpeciality)
        {
            var candidateProviderSpeciality = await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == idCandidateProvider && x.IdSpeciality == idSpeciality).FirstOrDefaultAsync();
            var data = this.repository.AllReadonly<CandidateCurriculum>(x => x.IdCandidateProviderSpeciality == candidateProviderSpeciality.IdCandidateProviderSpeciality);

            return await data.To<CandidateCurriculumVM>(x => x.CandidateCurriculumERUs).ToListAsync();
        }

        private async Task<IEnumerable<CandidateCurriculumVM>> GetActualCandidateCurriculumByIdCandidateProviderAndIdSpecialityAsync(int idCandidateProvider, int idSpeciality)
        {
            var kvModificationStatusFinalValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");
            var candidateProviderSpeciality = await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == idCandidateProvider && x.IdSpeciality == idSpeciality).FirstOrDefaultAsync();
            if (candidateProviderSpeciality is not null)
            {
                var candidateCurriculumModifications = this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == candidateProviderSpeciality.IdCandidateProviderSpeciality
                    && x.IdModificationStatus == kvModificationStatusFinalValue.IdKeyValue && x.ValidFromDate.HasValue ? x.ValidFromDate!.Value.Date <= DateTime.Now.Date : x.OldId.HasValue)
                        .OrderByDescending(x => x.ValidFromDate!.Value.Date)
                        .ThenByDescending(x => x.OldId);
                if (candidateCurriculumModifications.Any())
                {
                    var candidateCurriculumModificationAsVM = await candidateCurriculumModifications.To<CandidateCurriculumModificationVM>(x => x.CandidateCurriculums.Select(y => y.CandidateCurriculumERUs)).FirstOrDefaultAsync();
                    return candidateCurriculumModificationAsVM.CandidateCurriculums.ToList();
                }
            }

            return new List<CandidateCurriculumVM>();
        }

        private async Task CopyCurriculumAsync(ProgramVM program)
        {
            var candidateCurriculumList = (await this.GetActualCandidateCurriculumByIdCandidateProviderAndIdSpecialityAsync(program.IdCandidateProvider, program.IdSpeciality)).ToList();
            foreach (var candidateCurriculum in candidateCurriculumList)
            {
                TrainingCurriculum trainingCurriculum = new TrainingCurriculum()
                {
                    IdCandidateCurriculum = candidateCurriculum.IdCandidateCurriculum,
                    IdCandidateProviderSpeciality = candidateCurriculum.IdCandidateProviderSpeciality,
                    IdProgram = program.IdProgram,
                    IdProfessionalTraining = candidateCurriculum.IdProfessionalTraining,
                    Subject = candidateCurriculum.Subject,
                    Topic = candidateCurriculum.Topic,
                    Theory = candidateCurriculum.Theory,
                    Practice = candidateCurriculum.Practice,
                };

                await this.repository.AddAsync<TrainingCurriculum>(trainingCurriculum);
                await this.repository.SaveChangesAsync();

                var professionalTrainingsSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
                var kvBType = professionalTrainingsSource.FirstOrDefault(x => x.KeyValueIntCode == "B").IdKeyValue;
                if (candidateCurriculum.IdProfessionalTraining == kvBType)
                {
                    program.SelectableHours += (int)((candidateCurriculum.Theory.HasValue ? candidateCurriculum.Theory.Value : 0) + (candidateCurriculum.Practice.HasValue ? candidateCurriculum.Practice.Value : 0));
                }
                else
                {
                    program.MandatoryHours += (int)((candidateCurriculum.Theory.HasValue ? candidateCurriculum.Theory.Value : 0) + (candidateCurriculum.Practice.HasValue ? candidateCurriculum.Practice.Value : 0));
                }

                await this.UpdateTrainingProgramHoursByIdProgramAsync(program.IdProgram, program.MandatoryHours, program.SelectableHours);

                if (candidateCurriculum.CandidateCurriculumERUs.Any())
                {
                    foreach (var candidateCurriculumERU in candidateCurriculum.CandidateCurriculumERUs)
                    {
                        TrainingCurriculumERU trainingCurriculumERU = new TrainingCurriculumERU()
                        {
                            IdTrainingCurriculum = trainingCurriculum.IdTrainingCurriculum,
                            IdERU = candidateCurriculumERU.IdERU
                        };

                        await this.repository.AddAsync<TrainingCurriculumERU>(trainingCurriculumERU);
                    }

                    await this.repository.SaveChangesAsync();
                }
            }
        }

        public async Task<ResultContext<NoResult>> DeleteTrainingCurriculumAsync(int idTrainingCurriculum)
        {
            var resultContext = new ResultContext<NoResult>();

            try
            {
                var data = this.repository.AllReadonly<TrainingCurriculum>(x => x.IdTrainingCurriculum == idTrainingCurriculum)
                    .Include(x => x.TrainingCurriculumERUs).AsNoTracking()
                    .FirstOrDefault();

                if (data is not null)
                {
                    var course = await this.repository.AllReadonly<Course>(x => x.IdCourse == data.IdCourse).FirstOrDefaultAsync();
                    if (course is not null)
                    {
                        var courseSubjects = this.repository.AllReadonly<CourseSubject>(x => x.IdCourse == course.IdCourse && x.IdProfessionalTraining == data.IdProfessionalTraining)
                            .Include(x => x.CourseSubjectGrades);
                        if (courseSubjects.Any(x => x.CourseSubjectGrades.Any(x => x.TheoryGrade.HasValue || x.PracticeGrade.HasValue)))
                        {
                            resultContext.AddErrorMessage("Темата от учебната програма не може да бъде изтрита, тъй като има въведени оценки на курсисти!");
                        }
                        else
                        {
                            var courseSubject = courseSubjects.FirstOrDefault(x => x.IdProfessionalTraining == data.IdProfessionalTraining
                                && x.Subject == data.Subject);
                            if (courseSubject is not null)
                            {
                                var theoryHours = data.Theory.HasValue ? data.Theory.Value : 0;
                                var practiceHours = data.Practice.HasValue ? data.Practice.Value : 0;
                                if (courseSubject.TheoryHours - theoryHours <= 0 && courseSubject.PracticeHours - practiceHours <= 0)
                                {
                                    if (courseSubject.CourseSubjectGrades.Any())
                                    {
                                        this.repository.HardDeleteRange<CourseSubjectGrade>(courseSubject.CourseSubjectGrades);
                                        await this.repository.SaveChangesAsync();
                                    }

                                    await this.repository.HardDeleteAsync<CourseSubject>(courseSubject.IdCourseSubject);
                                    await this.repository.SaveChangesAsync();
                                }
                                else
                                {
                                    courseSubject.TheoryHours -= theoryHours;
                                    courseSubject.PracticeHours -= practiceHours;

                                    this.repository.Update<CourseSubject>(courseSubject);
                                    await this.repository.SaveChangesAsync();
                                }
                            }

                            if (data.TrainingCurriculumERUs.Any())
                            {
                                this.repository.HardDeleteRange<TrainingCurriculumERU>(data.TrainingCurriculumERUs);
                                await this.repository.SaveChangesAsync();
                            }

                            await this.repository.HardDeleteAsync<TrainingCurriculum>(data.IdTrainingCurriculum);
                            await this.repository.SaveChangesAsync();

                            resultContext.AddMessage("Записът е изтрит успешно!");
                        }
                    }
                    else
                    {
                        if (data.TrainingCurriculumERUs.Any())
                        {
                            this.repository.HardDeleteRange<TrainingCurriculumERU>(data.TrainingCurriculumERUs);
                            await this.repository.SaveChangesAsync();
                        }

                        await this.repository.HardDeleteAsync<TrainingCurriculum>(data.IdTrainingCurriculum);
                        await this.repository.SaveChangesAsync();

                        resultContext.AddMessage("Записът е изтрит успешно!");
                    }
                }
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

        public async Task<ResultContext<TrainingCurriculumVM>> AddTrainingCurriculumAsync(ResultContext<TrainingCurriculumVM> inputContext, bool ignoreERUs = false)
        {
            ResultContext<TrainingCurriculumVM> resultContext = new ResultContext<TrainingCurriculumVM>();

            try
            {
                var keyValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");

                resultContext.ResultContextObject = inputContext.ResultContextObject;
                var professionalTraining = keyValues.FirstOrDefault(x => x.IdKeyValue == resultContext.ResultContextObject.IdProfessionalTraining).DefaultValue1;
                resultContext.ResultContextObject.ProfessionalTraining = professionalTraining;

                var candidateCurriculumForDb = inputContext.ResultContextObject.To<TrainingCurriculum>();
                candidateCurriculumForDb.CandidateProviderSpeciality = null;
                candidateCurriculumForDb.TrainingCurriculumERUs = null;
                candidateCurriculumForDb.CandidateCurriculum = null;
                candidateCurriculumForDb.Program = null;
                candidateCurriculumForDb.Course = null;

                await this.repository.AddAsync<TrainingCurriculum>(candidateCurriculumForDb);
                await this.repository.SaveChangesAsync();

                inputContext.ResultContextObject.IdTrainingCurriculum = candidateCurriculumForDb.IdTrainingCurriculum;
                inputContext.ResultContextObject.CreationDate = candidateCurriculumForDb.CreationDate;
                inputContext.ResultContextObject.ModifyDate = candidateCurriculumForDb.ModifyDate;

                if (!ignoreERUs)
                {
                    await this.HandleCurriculumERUsAsync(inputContext.ResultContextObject.SelectedERUs, candidateCurriculumForDb.IdTrainingCurriculum);
                }

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

        public async Task<ResultContext<TrainingCurriculumVM>> UpdateTrainingCurriculumAsync(ResultContext<TrainingCurriculumVM> inputContext)
        {
            ResultContext<TrainingCurriculumVM> resultContext = new ResultContext<TrainingCurriculumVM>();

            try
            {
                var keyValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");

                resultContext.ResultContextObject = inputContext.ResultContextObject;
                var professionalTraining = keyValues.FirstOrDefault(x => x.IdKeyValue == resultContext.ResultContextObject.IdProfessionalTraining).DefaultValue1;
                resultContext.ResultContextObject.ProfessionalTraining = professionalTraining;

                var trainingCurriculumFromDb = await this.repository.GetByIdAsync<TrainingCurriculum>(inputContext.ResultContextObject.IdTrainingCurriculum);
                inputContext.ResultContextObject.IdCreateUser = trainingCurriculumFromDb.IdCreateUser;
                inputContext.ResultContextObject.CreationDate = trainingCurriculumFromDb.CreationDate;
                trainingCurriculumFromDb = inputContext.ResultContextObject.To<TrainingCurriculum>();
                trainingCurriculumFromDb.CandidateProviderSpeciality = null;
                trainingCurriculumFromDb.TrainingCurriculumERUs = null;
                trainingCurriculumFromDb.Program = null;
                trainingCurriculumFromDb.Course = null;
                trainingCurriculumFromDb.CandidateCurriculum = null;

                this.repository.Update<TrainingCurriculum>(trainingCurriculumFromDb);
                await this.repository.SaveChangesAsync();

                await this.HandleCurriculumERUsAsync(inputContext.ResultContextObject.SelectedERUs, trainingCurriculumFromDb.IdTrainingCurriculum);

                resultContext.ResultContextObject = trainingCurriculumFromDb.To<TrainingCurriculumVM>();
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

        public TrainingCurriculumERUVM GetTrainingCurriculumERUByIdTrainingCurriculumAndIdERU(int idTrainingCurriculum, int idEru)
        {
            try
            {
                var data = this.repository.AllReadonly<TrainingCurriculumERU>(x => x.IdTrainingCurriculum == idTrainingCurriculum && x.IdERU == idEru).FirstOrDefault();

                if (data == null)
                {
                    return null;
                }
                else
                {
                    return data.To<TrainingCurriculumERUVM>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                return null;
            }
        }

        public async Task<ResultContext<NoResult>> DeleteTrainingCurriculumERUAsync(int idTrainingCurriculumERU)
        {
            var resultContext = new ResultContext<NoResult>();

            try
            {
                await this.repository.HardDeleteAsync<TrainingCurriculumERU>(idTrainingCurriculumERU);
                await this.repository.SaveChangesAsync();
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

        public async Task<CandidateProviderSpecialityVM> GetCandidateProviderSpecialityByIdCandidateProviderAndIdSpecialityAsync(int idCandidateProvider, int idSpeciality)
        {
            var data = this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == idCandidateProvider && x.IdSpeciality == idSpeciality);

            return await data.To<CandidateProviderSpecialityVM>().FirstOrDefaultAsync();
        }

        public async Task<ResultContext<NoResult>> AddERUsToCurriculumListAsync(List<ERUVM> erus, List<TrainingCurriculumVM> curriculums)
        {
            var output = new ResultContext<NoResult>();

            try
            {
                foreach (var curriculum in curriculums)
                {
                    foreach (var eru in erus)
                    {
                        var trainingCurriculumERU = this.repository.AllReadonly<TrainingCurriculumERU>(x => x.IdTrainingCurriculum == curriculum.IdTrainingCurriculum && x.IdERU == eru.IdERU).FirstOrDefault();
                        if (trainingCurriculumERU is null)
                        {
                            TrainingCurriculumERU curriculumERU = new TrainingCurriculumERU()
                            {
                                IdTrainingCurriculum = curriculum.IdTrainingCurriculum,
                                IdERU = eru.IdERU,
                            };

                            await this.repository.AddAsync<TrainingCurriculumERU>(curriculumERU);
                        }
                    }
                }

                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                output.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return output;
        }

        public async Task<ResultContext<List<TrainingCurriculumVM>>> ImportCurriculumAsync(MemoryStream file, string fileName)
        {
            ResultContext<List<TrainingCurriculumVM>> resultContext = new ResultContext<List<TrainingCurriculumVM>>();

            List<TrainingCurriculumVM> trainingCurriculumVMs = new List<TrainingCurriculumVM>();

            try
            {
                int counter = GlobalConstants.INVALID_ID_ZERO;

                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\Temp\\ImportTrainingCurriculum";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var path = @"" + filePath + "\\" + fileName;

                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file.WriteTo(filestream);
                    filestream.Close();
                    file.Close();
                }

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        IApplication app = excelEngine.Excel;

                        IWorkbook workbook = app.Workbooks.Open(fileStream, ExcelOpenType.Automatic);

                        IWorksheet worksheet = workbook.Worksheets[0];
                        if (string.IsNullOrEmpty(worksheet.Rows[0].Columns[0].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[1].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[2].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[3].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[4].Text))
                        {
                            resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на учебна програма!");
                            return resultContext;
                        }

                        var firstHeader = worksheet.Rows[0].Columns[0].Text.Trim();
                        var secondHeader = worksheet.Rows[0].Columns[1].Text.Trim();
                        var thirdHeader = worksheet.Rows[0].Columns[2].Text.Trim();
                        var fourthHeader = worksheet.Rows[0].Columns[3].Text.Trim();
                        var fifthHeader = worksheet.Rows[0].Columns[4].Text.Trim();
                        bool skipFirstRow = true;

                        //Проверка по 1 клетка за да се провери дали файла за импорт на учебна програма
                        if (firstHeader != "Раздел" || secondHeader != "Предмет" || thirdHeader != "Тема" || fourthHeader != "Теория" || fifthHeader != "Практика")
                        {
                            resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на учебна програма!");
                            return resultContext;
                        }

                        var rowCounter = 2;
                        foreach (var row in worksheet.Rows)
                        {
                            //Ако сме пропуснали 5 реда приемаме че документа е приключил и са останали празни редове
                            if (counter == 5)
                            {
                                break;
                            }

                            //Пропуска 1 ред който е с хедърите
                            if (skipFirstRow || string.IsNullOrEmpty(row.Cells[professionalTrainingIndex].Value))
                            {
                                skipFirstRow = false;
                                counter++;
                                continue;
                            }

                            var professionalTraining = row.Cells[professionalTrainingIndex].Value.Trim();
                            if (professionalTraining[0] == 'A' || professionalTraining[0] == 'a' || professionalTraining[0] == 'А' || professionalTraining[0] == 'а')
                            {
                                if (!int.TryParse(professionalTraining[1].ToString(), out int value) || int.Parse(professionalTraining[1].ToString()) < 1 || int.Parse(professionalTraining[1].ToString()) > 3)
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} не отговаря на изискванията за вид на професионална подготовка!");
                                }
                                else
                                {
                                    professionalTraining = "A" + professionalTraining[1];
                                }
                            }
                            else if (professionalTraining == "Б" || professionalTraining == "б")
                            {
                                professionalTraining = "B";
                            }
                            else
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter} не отговаря на изискванията за вид на професионална подготовка!");
                            }

                            var subject = row.Cells[subjectIndex].Value.Trim();
                            var topic = row.Cells[topicIndex].Value.Trim();
                            var theory = row.Cells[theoryIndex].Value.Trim();
                            var practice = row.Cells[practiceIndex].Value.Trim();

                            var trainingCurriculum = new TrainingCurriculumVM();

                            trainingCurriculum.UploadedFileName = "#";

                            var keyValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ProfessionalTraining", professionalTraining);
                            if (keyValue != null)
                            {
                                trainingCurriculum.IdProfessionalTraining = keyValue.IdKeyValue;
                                trainingCurriculum.ProfessionalTraining = keyValue.DefaultValue1;
                            }

                            trainingCurriculum.Subject = subject;
                            if (string.IsNullOrEmpty(trainingCurriculum.Subject))
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: полето 'Предмет' е задължително!");
                            }
                            else if (trainingCurriculum.Subject.Length > 1000)
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: полето 'Предмет' не може да съдържа повече от 1000 символа!");
                            }

                            trainingCurriculum.Topic = topic;
                            if (string.IsNullOrEmpty(trainingCurriculum.Topic))
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: полето 'Тема' е задължително!");
                            }
                            else if (trainingCurriculum.Topic.Length > 4000)
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: полето 'Тема' не може да съдържа повече от 4000 символа!");
                            }

                            if (string.IsNullOrEmpty(theory) && string.IsNullOrEmpty(practice))
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: при едно от полетата 'Теория' или 'Практика' трябва да има поне една въведена стойност!");
                            }

                            if (!string.IsNullOrEmpty(theory))
                            {
                                if (double.TryParse(theory, out double value))
                                {
                                    trainingCurriculum.Theory = double.Parse(theory);
                                    if (trainingCurriculum.Theory < 0)
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Теория' може да бъде само положително число!");
                                    }
                                    else if (trainingCurriculum.Theory % 1 != 0)
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Теория' може да бъде само цяло число!");
                                    }
                                }
                                else
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Теория' може да бъде само цяло число!");
                                }
                            }

                            if (!string.IsNullOrEmpty(practice))
                            {
                                if (double.TryParse(practice, out double value))
                                {
                                    trainingCurriculum.Practice = double.Parse(practice);
                                    if (trainingCurriculum.Practice < 0)
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Практика' може да бъде само положително число!");
                                    }
                                    else if (trainingCurriculum.Practice % 1 != 0)
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Практика' може да бъде само цяло число!");
                                    }
                                }
                                else
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Практика' може да бъде само цяло число!");
                                }
                            }

                            trainingCurriculumVMs.Add(trainingCurriculum);

                            rowCounter++;
                        }
                    }

                    if (trainingCurriculumVMs.Any())
                    {
                        resultContext.AddMessage("Импортът приключи успешно!");
                    }
                    else
                    {
                        resultContext.AddErrorMessage("Не може да бъде добавен празен шаблон за учебна програма!");
                    }

                    resultContext.ResultContextObject = trainingCurriculumVMs;
                }
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public MemoryStream CreateExcelWithErrors(ResultContext<List<TrainingCurriculumVM>> resultContext)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 50;
                sheet.Range[$"A1"].Text = "Вид на грешките:";
                //sheet.Range[$"B1"].Text = "Позиция във файла";

                var rowCounter = 2;
                foreach (var item in resultContext.ListErrorMessages)
                {
                    //var splitMsg = item.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    //var msg = splitMsg[0].Trim();
                    //var cell = splitMsg[1].Trim();

                    //sheet.Range[$"A{rowCounter}"].Text = msg;
                    sheet.Range[$"A{rowCounter}"].Text = item;
                    //sheet.Range[$"B{rowCounter}"].Text = cell;

                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }

        public async Task<int> GetIdCandidateProviderSpecialityByIdSpecialityAndIdCandidateProviderAsync(int idSpeciality, int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdSpeciality == idSpeciality && x.IdCandidate_Provider == idCandidateProvider);
            var candidateProviderSpeciality = await data.FirstOrDefaultAsync();
            if (candidateProviderSpeciality is not null)
            {
                return candidateProviderSpeciality.IdCandidateProviderSpeciality;
            }

            return 0;
        }

        public async Task<ResultContext<NoResult>> DeleteListCandidateCurriculumAsync(List<TrainingCurriculumVM> trainingCurriculums)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                foreach (var training in trainingCurriculums)
                {
                    var trainingCurriculumERUs = await this.repository.AllReadonly<TrainingCurriculumERU>(x => x.IdTrainingCurriculum == training.IdTrainingCurriculum).ToListAsync();
                    if (trainingCurriculumERUs.Any())
                    {
                        this.repository.HardDeleteRange<TrainingCurriculumERU>(trainingCurriculumERUs);
                        await this.repository.SaveChangesAsync();
                    }

                    await this.repository.HardDeleteAsync<TrainingCurriculum>(training.IdTrainingCurriculum);
                    await this.repository.SaveChangesAsync();
                }

                var msg = trainingCurriculums.Count == 1 ? "Записът е изтрит успешно!" : "Записите са изтрити успешно!";
                resultContext.AddMessage(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<IEnumerable<TrainingCurriculumUploadedFileVM>> GetTrainingCurriculumUploadedFilesForOldCoursesByIdCourseAsync(int idCourse)
        {
            var dataList = new List<TrainingCurriculumUploadedFileVM>();
            try
            {
                var courseModifications = await this.repository.AllReadonly<CourseCandidateCurriculumModification>(x => x.IdCourse == idCourse)
                    .Include(x => x.CandidateCurriculumModification).OrderByDescending(x => x.IdCourseCandidateCurriculumModification).ToListAsync();
                var counter = 1;
                foreach (var courseMod in courseModifications)
                {
                    if (courseMod.CandidateCurriculumModification is not null && !string.IsNullOrEmpty(courseMod.CandidateCurriculumModification.UploadedFileName))
                    {
                        dataList.Add(new TrainingCurriculumUploadedFileVM()
                        {
                            IdGrid = counter++,
                            UploadedFileName = courseMod.CandidateCurriculumModification.UploadedFileName,
                            IdEntity = courseMod.CandidateCurriculumModification.IdCandidateCurriculumModification
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return dataList;
        }

        private async Task HandleCurriculumERUsAsync(List<ERUVM> erus, int idTrainingCurriculum)
        {
            foreach (var eru in erus)
            {
                var candidateCurriculumERU = this.repository.AllReadonly<TrainingCurriculumERU>(x => x.IdTrainingCurriculum == idTrainingCurriculum && x.IdERU == eru.IdERU).FirstOrDefault();
                if (candidateCurriculumERU is null)
                {
                    TrainingCurriculumERU curriculumERU = new TrainingCurriculumERU()
                    {
                        IdTrainingCurriculum = idTrainingCurriculum,
                        IdERU = eru.IdERU,
                    };

                    await this.repository.AddAsync<TrainingCurriculumERU>(curriculumERU);
                    await this.repository.SaveChangesAsync();
                }
            }
        }

        #endregion Training curriculum

        #region Training course

        public async Task<CourseVM> GetTrainingCourseByIdAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<Course>(x => x.IdCourse == idCourse);
            var dataAsVM = await data.To<CourseVM>(x => x.Location, x => x.Program.FrameworkProgram.FrameworkProgramFormEducations, x => x.Program.Speciality.Profession, x => x.CandidateProviderPremises.Location.Municipality.District, x => x.CandidateProvider.Location, x => x.CandidateProvider.Location.Municipality, x => x.CandidateProvider.Location.Municipality.Regions, x => x.CandidateProvider, x => x.CourseCommissionMembers).FirstOrDefaultAsync();
            var trainingCourseType = this.trainingCourseTypeSource.FirstOrDefault(x => x.IdKeyValue == dataAsVM.IdTrainingCourseType);
            if (trainingCourseType is not null)
            {
                dataAsVM.TrainingCourseTypeName = trainingCourseType.Name;
            }

            if (dataAsVM.IdStatus != null && dataAsVM.IdStatus != 0)
            {
                dataAsVM.Status = await dataSourceService.GetKeyValueByIdAsync(dataAsVM.IdStatus);
            }

            if (dataAsVM.IdMeasureType != null && dataAsVM.IdMeasureType != 0)
            {
                dataAsVM.MeasureType = await dataSourceService.GetKeyValueByIdAsync(dataAsVM.IdMeasureType);
            }
            if (dataAsVM.IdAssignType != null && dataAsVM.IdAssignType != 0)
            {
                dataAsVM.AssignType = await dataSourceService.GetKeyValueByIdAsync(dataAsVM.IdAssignType);
            }
            if (dataAsVM.IdFormEducation != null && dataAsVM.IdFormEducation != 0)
            {
                dataAsVM.FormEducation = await dataSourceService.GetKeyValueByIdAsync(dataAsVM.IdFormEducation);
            }
            return dataAsVM;
        }

        public async Task<CourseVM> GetTrainingCourseWithoutAnythingIncludedByIdAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<Course>(x => x.IdCourse == idCourse);

            return await data.To<CourseVM>().FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CourseVM>> GetAllUpcomingTrainingCoursesByIdCandidateProviderAsync(int idCandidateProvider, int? idCourseType = null)
        {
            IQueryable<Course> data;
            if (idCourseType.HasValue)
            {
                data = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider && x.IdTrainingCourseType == idCourseType && x.IdStatus == this.kvUpcomingCourse.IdKeyValue && !x.IsArchived);
            }
            else
            {
                data = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider && x.IdStatus == this.kvUpcomingCourse.IdKeyValue && !x.IsArchived);
            }

            return await data.To<CourseVM>(x => x.Location, x => x.Program.Speciality.Profession).ToListAsync();
        }

        public async Task<IEnumerable<CourseVM>> GetAllCurrentTrainingCoursesByIdCandidateProviderAsync(int idCandidateProvider, int? idCourseType = null)
        {
            IQueryable<Course> data;
            if (idCourseType.HasValue)
            {
                data = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider && x.IdTrainingCourseType == idCourseType && x.IdStatus == this.kvCurrentCourse.IdKeyValue && !x.IsArchived);
            }
            else
            {
                data = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider && x.IdStatus == this.kvCurrentCourse.IdKeyValue && !x.IsArchived);
            }

            var dataAsVM = await data.To<CourseVM>(x => x.Location, x => x.Program.Speciality.Profession).ToListAsync();
            foreach (var course in dataAsVM)
            {
                course.TrainingCourseTypeName = course.IdTrainingCourseType == this.kvVocationalTrainingInPartOfProfession.IdKeyValue ? this.kvVocationalTrainingInPartOfProfession.Name : this.kvProfessionalTrainingForAcquiringTheSPK.Name;
            }

            return dataAsVM.OrderByDescending(x => x.StartDate).ThenBy(x => x.CourseName).ToList();
        }

        public async Task<IEnumerable<CourseVM>> GetAllCompletedTrainingCoursesByIdCandidateProviderAsync(int idCandidateProvider, int? idCourseType = null)
        {
            IQueryable<Course> data;
            if (idCourseType.HasValue)
            {
                data = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider && x.IdTrainingCourseType == idCourseType && x.IdStatus == this.kvCompletedCourse.IdKeyValue && !x.IsArchived);
            }
            else
            {
                data = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider && x.IdStatus == this.kvCompletedCourse.IdKeyValue && !x.IsArchived);
            }

            return await data.To<CourseVM>(x => x.Location, x => x.Program.Speciality.Profession).OrderByDescending(x => x.EndDate).ThenBy(x => x.CourseName).ToListAsync();
        }

        public async Task<IEnumerable<CourseVM>> GetAllArchivedTrainingCoursesByIdCandidateProviderAsync(int idCandidateProvider, int? idCourseType = null)
        {
            IQueryable<Course> data;
            if (idCourseType.HasValue)
            {
                data = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider && x.IdTrainingCourseType == idCourseType && x.IsArchived);
            }
            else
            {
                data = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider && x.IsArchived);
            }

            return await data.To<CourseVM>(x => x.Location, x => x.Program.Speciality.Profession).OrderByDescending(x => x.EndDate).ThenBy(x => x.CourseName).ToListAsync();
        }

        public async Task<IEnumerable<CourseVM>> GetAllCoursesWhichAreNotOnStatusUpcomingByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider && x.IdStatus != this.kvUpcomingCourse.IdKeyValue);

            return await data.To<CourseVM>(x => x.Program.Speciality.Profession).OrderBy(x => x.CourseName).ToListAsync();
        }

        public async Task<ResultContext<NoResult>> DeleteTrainingCourseByIdAsync(int idCourse)
        {
            var result = new ResultContext<NoResult>();

            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<Course>(idCourse);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<Course>(entryFromDb.IdCourse);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return result;
        }

        public async Task<IEnumerable<CandidateProviderPremisesVM>> GetAllActiveCandidateProviderPremisesByIdCandidateProviderAndIdSpecialityAsync(int idCandidateProvider, int idSpeciality)
        {
            var data = this.repository.AllReadonly<CandidateProviderPremises>(x => x.IdCandidate_Provider == idCandidateProvider && x.IdStatus == this.kvMTBActive.IdKeyValue);
            var premisesAsVM = await data.To<CandidateProviderPremisesVM>(x => x.Location.Municipality.District, x => x.CandidateProviderPremisesSpecialities).ToListAsync();
            var premisesAsList = new List<CandidateProviderPremisesVM>();
            foreach (var premises in premisesAsVM)
            {
                if (premises.CandidateProviderPremisesSpecialities.Any(x => x.IdSpeciality == idSpeciality))
                {
                    premisesAsList.Add(premises);
                }
            }

            return premisesAsList.ToList();
        }

        public async Task<ResultContext<CourseVM>> CreateTrainingCourseAsync(ResultContext<CourseVM> inputContext)
        {
            var model = inputContext.ResultContextObject;
            try
            {
                model.IdTrainingCourseType = model.Program.IdCourseType;
                model.IdProgram = model.Program.IdProgram;
                model.IdLocation = model.CandidateProviderPremises.IdLocation;
                model.IdStatus = this.kvUpcomingCourse.IdKeyValue;
                model.MandatoryHours = model.Program.MandatoryHours;
                model.SelectableHours = model.Program.SelectableHours;
                model.IdLegalCapacityOrdinanceType = model.Program.IdLegalCapacityOrdinanceType;
                model.CourseNameEN = BaseHelper.ConvertCyrToLatin(model.CourseName);
                var entryForDb = model.To<Course>();
                entryForDb.CandidateProvider = null;
                entryForDb.CandidateProviderPremises = null;
                entryForDb.ClientCourses = null;
                entryForDb.Location = null;
                entryForDb.Program = null;

                await this.repository.AddAsync<Course>(entryForDb);
                await this.repository.SaveChangesAsync();

                model.IdCourse = entryForDb.IdCourse;
                model.IdCreateUser = entryForDb.IdCreateUser;
                model.IdModifyUser = entryForDb.IdModifyUser;
                model.CreationDate = entryForDb.CreationDate;
                model.ModifyDate = entryForDb.ModifyDate;

                inputContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                inputContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return inputContext;
        }

        public async Task<ResultContext<CourseVM>> UpdateTrainingCourseAsync(ResultContext<CourseVM> inputContext)
        {
            var model = inputContext.ResultContextObject;
            try
            {
                var entryForDb = await this.repository.GetByIdAsync<Course>(model.IdCourse);
                if (entryForDb is not null)
                {
                    model.IdTrainingCourseType = model.Program.IdCourseType;
                    model.IdProgram = model.Program.IdProgram;
                    model.IdLocation = model.CandidateProviderPremises.IdLocation;
                    model.MandatoryHours = model.Program.MandatoryHours;
                    model.SelectableHours = model.Program.SelectableHours;
                    model.IdCreateUser = entryForDb.IdCreateUser;
                    model.CreationDate = entryForDb.CreationDate;
                    model.CourseNameEN = BaseHelper.ConvertCyrToLatin(model.CourseName);
                    entryForDb = model.To<Course>();
                    entryForDb.CandidateProvider = null;
                    entryForDb.CandidateProviderPremises = null;
                    entryForDb.ClientCourses = null;
                    entryForDb.Location = null;
                    entryForDb.Program = null;

                    this.repository.Update<Course>(entryForDb);
                    await this.repository.SaveChangesAsync();

                    model.IdModifyUser = entryForDb.IdModifyUser;
                    model.ModifyDate = entryForDb.ModifyDate;

                    inputContext.AddMessage("Записът е успешен!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                inputContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return inputContext;
        }

        public async Task<ResultContext<CourseVM>> StartUpcomingTrainingCourseAsync(ResultContext<CourseVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<Course>(model.IdCourse);
                if (entryFromDb is not null)
                {
                    entryFromDb.IdStatus = this.kvCurrentCourse.IdKeyValue;

                    this.repository.Update<Course>(entryFromDb);
                    await this.repository.SaveChangesAsync();

                    model.IdStatus = entryFromDb.IdStatus;

                    await this.CopyTrainingCurriculumToTrainingCourseAsync(model);
                    await this.CopySubjectsToStartedCourseAsync(model);

                    resultContext.AddMessage("Курсът е стартиран успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<CourseVM>> CompleteCurrentTrainingCourseAsync(ResultContext<CourseVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<Course>(model.IdCourse);
                if (entryFromDb is not null)
                {
                    entryFromDb.IdStatus = this.kvCompletedCourse.IdKeyValue;

                    this.repository.Update<Course>(entryFromDb);
                    await this.repository.SaveChangesAsync();

                    model.IdStatus = entryFromDb.IdStatus;

                    resultContext.AddMessage("Курсът е приключен успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        private async Task CopyTrainingCurriculumToTrainingCourseAsync(CourseVM course)
        {
            try
            {
                var trainingCurriculums = await this.repository.AllReadonly<TrainingCurriculum>(x => x.IdProgram == course.IdProgram && x.IdCourse == null)
                    .Include(x => x.TrainingCurriculumERUs)
                        .AsNoTracking()
                    .ToListAsync();
                foreach (var curriculum in trainingCurriculums)
                {
                    TrainingCurriculum trainingCurriculum = new TrainingCurriculum()
                    {
                        IdCandidateProviderSpeciality = curriculum.IdCandidateProviderSpeciality,
                        IdProgram = curriculum.IdProgram,
                        IdCourse = course.IdCourse,
                        IdProfessionalTraining = curriculum.IdProfessionalTraining,
                        Subject = curriculum.Subject,
                        Topic = curriculum.Topic,
                        Theory = curriculum.Theory,
                        Practice = curriculum.Practice
                    };

                    await this.repository.AddAsync<TrainingCurriculum>(trainingCurriculum);
                    await this.repository.SaveChangesAsync();

                    if (curriculum.TrainingCurriculumERUs.Any())
                    {
                        foreach (var eru in curriculum.TrainingCurriculumERUs)
                        {
                            TrainingCurriculumERU trainingCurriculumERU = new TrainingCurriculumERU()
                            {
                                IdTrainingCurriculum = trainingCurriculum.IdTrainingCurriculum,
                                IdERU = eru.IdERU
                            };

                            await this.repository.AddAsync<TrainingCurriculumERU>(trainingCurriculumERU);
                        }

                        await this.repository.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task UpdateTrainingCourseHoursByIdCourseAsync(int idCourse, int mandatoryHours, int selectableHours)
        {
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<Course>(idCourse);
                if (entryFromDb is not null)
                {
                    entryFromDb.MandatoryHours = mandatoryHours;
                    entryFromDb.SelectableHours = selectableHours;

                    this.repository.Update<Course>(entryFromDb);
                    await this.repository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task<int> GetTrainingCourseIdTrainingTypeByIdCourseAsync(int idCourse)
        {
            var data = await this.repository.AllReadonly<Course>(x => x.IdCourse == idCourse).FirstOrDefaultAsync();

            if (data is not null)
            {
                if (data.IdTrainingCourseType.HasValue)
                {
                    return data.IdTrainingCourseType.Value;
                }
            }

            return 0;
        }

        public async Task<IEnumerable<CourseVM>> GetAllArchivedAndOutOfOrderTrainingCoursesByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var kvTypeFrameworkProgram = await this.repository.AllReadonly<KeyType>(x => x.KeyTypeIntCode == "TypeFrameworkProgram").FirstOrDefaultAsync();
            var kvSource = await this.repository.AllReadonly<KeyValue>(x => x.IdKeyType == kvTypeFrameworkProgram!.IdKeyType).ToListAsync();
            var listIds = kvSource.Where(x => x.DefaultValue4 != null).Select(x => x.IdKeyValue).ToList();
            var data = await this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider).Include(x => x.Location).AsNoTracking().ToListAsync();
            var source = new List<CourseVM>();
            foreach (var course in data)
            {
                if (!course.IdTrainingCourseType.HasValue)
                {
                    source.Add(course.To<CourseVM>());
                }
                else
                {
                    if (listIds.Contains(course.IdTrainingCourseType.Value))
                    {
                        source.Add(course.To<CourseVM>());
                    }
                }
            }

            return source;
        }

        public MemoryStream CreateExcelWithXMLImportValidationErrors(ResultContext<CourseCollection> resultContext)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 50;
                sheet.Range[$"A1"].Text = "Вид на грешките:";
                //sheet.Range[$"B1"].Text = "Позиция във файла";

                var rowCounter = 2;
                foreach (var item in resultContext.ListErrorMessages)
                {
                    //var splitMsg = item.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    //var msg = splitMsg[0].Trim();
                    //var cell = splitMsg[1].Trim();

                    //sheet.Range[$"A{rowCounter}"].Text = msg;
                    sheet.Range[$"A{rowCounter}"].Text = item;
                    //sheet.Range[$"B{rowCounter}"].Text = cell;

                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }

        public async Task<ResultContext<CourseCollection>> ImportCourseAsync(MemoryStream file, string fileName)
        {
            ResultContext<CourseCollection> resultContext = new ResultContext<CourseCollection>();

            try
            {
                int counter = GlobalConstants.INVALID_ID_ZERO;

                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\Temp\\ImportCourse";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var path = @"" + filePath + "\\" + fileName;

                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file.WriteTo(filestream);
                    filestream.Close();
                    file.Close();
                }

                CourseCollection courseCollection = this.DeserializeCourseXML(path);

                if (courseCollection.GeneralInfo is not null)
                {
                    var generalInfo = courseCollection.GeneralInfo;
                    await this.ValidateXMLGeneralInfoAsync(resultContext, generalInfo);
                }

                if (courseCollection.CourseGroup is not null)
                {
                    var courseGroup = courseCollection.CourseGroup;
                    this.ValidateXMLCourseGroup(resultContext, courseGroup);
                }

                if (courseCollection.CurriculumGroup is not null)
                {
                    var curriculumGroup = courseCollection.CurriculumGroup;
                    if (curriculumGroup.Modules is not null && curriculumGroup.Modules.Any())
                    {
                        var modules = curriculumGroup.Modules;
                        this.ValidateXMLModule(resultContext, modules);
                    }
                }

                if (courseCollection.Students is not null)
                {
                    var students = courseCollection.Students;
                    if (students.StudentList is not null && students.StudentList.Any())
                    {
                        var studentsList = students.StudentList;
                        this.ValidateXMLStudent(resultContext, studentsList);
                    }
                }

                if (courseCollection.Trainers is not null)
                {
                    var trainers = courseCollection.Trainers;
                    if (trainers.TrainerList is not null && trainers.TrainerList.Any())
                    {
                        var trainersList = trainers.TrainerList;
                        await this.ValidateXMLTrainerAsync(resultContext, trainersList);
                    }
                }

                //if (courseCollection.GeneralInfo is not null || courseCollection.CourseGroup is not null || courseCollection.CurriculumGroup is not null || courseCollection.Students is not null || courseCollection.Trainers is not null)
                //{
                //    resultContext.AddMessage("Импортът приключи успешно!");
                //}
                //else
                //{
                //    resultContext.AddErrorMessage("Не може да бъде добавен празен шаблон за курс!");
                //}

                resultContext.ResultContextObject = courseCollection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на курс!");
            }

            return resultContext;
        }

        public async Task<ResultContext<CourseCollection>> ImportXMLCourseIntoDBAsync(ResultContext<CourseCollection> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var programId = await this.HandleProgramAddFromXMLImportAsync(model);
                var courseId = await this.HandleCourseAddToDbFromXMLImportAsync(model, programId);
                if (model.Students is not null & model.Students.StudentList.Any())
                {
                    await this.HandleClientsAddFromXMLImportAsync(model, courseId, programId);
                }

                if (model.Trainers is not null & model.Trainers.TrainerList.Any())
                {
                    await this.HandleTrainersAddFromXMLImportAsync(model, courseId);
                }

                resultContext.AddMessage("Импортът приключи успешно!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<bool> IsAnnualReportSubmittedOrApprovedByIdCandidateProviderAndYearAsync(int idCandidateProvider, int year)
        {
            var kvAnnualReportStatusSubmittedValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("AnnualInfoStatusType", "Submitted");
            var kvAnnualReportStatusApprovedValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("AnnualInfoStatusType", "Approved");
            return (await this.repository.AllReadonly<AnnualInfo>(x => x.IdCandidateProvider == idCandidateProvider && x.Year == year && (x.IdStatus == kvAnnualReportStatusApprovedValue.IdKeyValue || x.IdStatus == kvAnnualReportStatusSubmittedValue.IdKeyValue)).ToListAsync()).Any();
        }

        public async Task<bool> IsAnyClientWithDocumentPresentByIdCourseAsync(int idCourse)
        {
            var kvEnteredInRegsiterStatusValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "EnteredInTheRegister");
            var kvSubmittedStatusValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Submitted");
            var courseClients = await this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == idCourse)
                .Include(x => x.ClientCourseDocuments)
                    .AsNoTracking()
                        .ToListAsync();
            foreach (var client in courseClients)
            {
                foreach (var doc in client.ClientCourseDocuments)
                {
                    if (doc.IdDocumentStatus is not null && (doc.IdDocumentStatus == kvEnteredInRegsiterStatusValue.IdKeyValue || doc.IdDocumentStatus == kvSubmittedStatusValue.IdKeyValue))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<bool> IsSpecialityFromSPPOORemovedWithOrderBeforeCourseStartDateAsync(int idSpeciality, DateTime courseStartDate)
        {
            var speciality = await this.repository.AllReadonly<Speciality>(x => x.IdSpeciality == idSpeciality)
                .Include(x => x.SpecialityOrders.Where(y => y.IdTypeChange == this.dataSourceService.GetOrderRemoveTypechangeID()).OrderByDescending(x => x.IdSpecialityOrder))
                .ThenInclude(x => x.SPPOOOrder)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (speciality is not null && speciality.SpecialityOrders.Any())
            {
                var order = speciality.SpecialityOrders.FirstOrDefault();
                if (order is not null && order.SPPOOOrder is not null)
                {
                    if (courseStartDate > order.SPPOOOrder.OrderDate)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task<IEnumerable<CourseVM>> GetAllArchivedAndFinishedCoursesByIdCandidateProviderAndByIdCourseTypeAsync(int idCandidateProvider, int idCourseType)
        {
            var kvCourseStatusFinished = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");
            var courses = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider && (x.IsArchived || x.IdStatus == kvCourseStatusFinished.IdKeyValue) && x.IdTrainingCourseType == idCourseType);

            return await courses.To<CourseVM>().OrderByDescending(x => x.EndDate).ToListAsync();
        }

        public async Task<string> AreDOSChangesWithoutActualizationOfCurriculumsAsync(CourseVM course)
        {
            var providerSpec = this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == course.IdCandidateProvider && x.IdSpeciality == course.Program.IdSpeciality);
            var providerSpecAsVM = await providerSpec.To<CandidateProviderSpecialityVM>(x => x.Speciality).FirstOrDefaultAsync();
            var kvDOCActiveStatusValue = this.dataSourceService.GetActiveStatusID();
            var kvModificationStatusFinalValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");
            var kvModificationReasonDOSValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationReasonType", "DOSChange");
            if (providerSpecAsVM is not null)
            {
                if (providerSpecAsVM.Speciality is not null && providerSpecAsVM.Speciality.IdDOC.HasValue && providerSpecAsVM.Speciality.IdStatus == kvDOCActiveStatusValue)
                {
                    var activeDOS = await this.repository.AllReadonly<Data.Models.Data.DOC.DOC>(x => x.IdDOC == providerSpecAsVM.Speciality.IdDOC.Value && x.IdStatus == kvDOCActiveStatusValue).FirstOrDefaultAsync();
                    if (activeDOS is not null)
                    {
                        var modification = await this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == providerSpecAsVM.IdCandidateProviderSpeciality && x.IdModificationReason == kvModificationReasonDOSValue.IdKeyValue && x.IdModificationStatus == kvModificationStatusFinalValue.IdKeyValue).OrderByDescending(x => x.ValidFromDate!.Value.Date).ThenByDescending(x => x.OldId).FirstOrDefaultAsync();
                        if (modification is not null && modification.ValidFromDate!.Value.Date < activeDOS.StartDate.Date)
                        {
                            if (modification.ValidFromDate!.Value.Date < activeDOS.StartDate.Date && course.StartDate!.Value.Date >= activeDOS.StartDate.Date)
                            {
                                return $"Моля, актуализирайте в профила на ЦПО учебния план и учебните програми за специалност '{providerSpecAsVM.Speciality.CodeAndName}' в съответствие с промените в държавния образователен стандарт в сила от {activeDOS.StartDate.Date.ToString(GlobalConstants.DATE_FORMAT)} г.!";
                            }
                        }
                        else
                        {
                            var modificationChangeHistory = await this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == providerSpecAsVM.IdCandidateProviderSpeciality && x.IdModificationStatus == kvModificationStatusFinalValue.IdKeyValue).OrderByDescending(x => x.ValidFromDate!.Value.Date).ThenByDescending(x => x.OldId).FirstOrDefaultAsync();
                            if (modificationChangeHistory is null)
                            {
                                return $"Моля, въведете в профила на ЦПО актуален учебния план и учебните програми за специалност '{providerSpecAsVM.Speciality.CodeAndName}'!";
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        public async Task SetRIDPKCountAsync(List<CourseVM> courses)
        {
            try
            {
                var kvFinishedWithDoc = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type1");
                var kvFinishedWithDocDuplicate = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type6");
                var kvDocSPK = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification");
                var kvDocSPKDuplicate = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "IssueOfDuplicate");
                var courseIds = courses.Select(x => x.IdCourse).ToList();
                var clients = await this.repository.AllReadonly<ClientCourse>(x => x.IdFinishedType.HasValue && (x.IdFinishedType == kvFinishedWithDoc.IdKeyValue || x.IdFinishedType == kvFinishedWithDocDuplicate.IdKeyValue) && courseIds.Contains(x.IdCourse))
                    .Include(x => x.ClientCourseDocuments.Where(x => x.IdDocumentStatus.HasValue && x.IdDocumentType.HasValue && (x.IdDocumentType == kvDocSPK.IdKeyValue || x.IdDocumentType == kvDocSPKDuplicate.IdKeyValue)))
                        .AsNoTracking().ToListAsync();

                var kvDocStatusNotSubmitted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "NotSubmitted");
                var kvDocStatusSubmitted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Submitted");
                var kvDocStatusReturned = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Returned");
                var kvDocStatusEnteredInRegister = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "EnteredInTheRegister");
                var kvDocStatusDeclined = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Rejected");
                foreach (var course in courses)
                {
                    var clientsFromCourse = clients.Where(x => x.IdCourse == course.IdCourse).ToList();
                    if (clientsFromCourse.Any())
                    {
                        var clientsWithDocs = clientsFromCourse.Where(x => x.ClientCourseDocuments.Any()).ToList();
                        if (clientsWithDocs.Any())
                        {
                            var docs = new List<ClientCourseDocument>();
                            foreach (var clientWithDoc in clientsWithDocs)
                            {
                                docs.AddRange(clientWithDoc.ClientCourseDocuments);
                            }

                            if (docs.Any())
                            {
                                course.RIDPKCountNotSubmitted = docs.Where(x => x.IdDocumentStatus == kvDocStatusNotSubmitted.IdKeyValue).Count();
                                if (course.RIDPKCountNotSubmitted == 0)
                                {
                                    course.RIDPKCountNotSubmitted = null;
                                }

                                course.RIDPKCountSubmitted = docs.Where(x => x.IdDocumentStatus == kvDocStatusSubmitted.IdKeyValue).Count();
                                if (course.RIDPKCountSubmitted == 0)
                                {
                                    course.RIDPKCountSubmitted = null;
                                }

                                course.RIDPKCountReturned = docs.Where(x => x.IdDocumentStatus == kvDocStatusReturned.IdKeyValue).Count();
                                if (course.RIDPKCountReturned == 0)
                                {
                                    course.RIDPKCountReturned = null;
                                }

                                course.RIDPKCountEnteredInRegister = docs.Where(x => x.IdDocumentStatus == kvDocStatusEnteredInRegister.IdKeyValue).Count();
                                if (course.RIDPKCountEnteredInRegister == 0)
                                {
                                    course.RIDPKCountEnteredInRegister = null;
                                }

                                course.RIDPKCountDeclined = docs.Where(x => x.IdDocumentStatus == kvDocStatusDeclined.IdKeyValue).Count();
                                if (course.RIDPKCountDeclined == 0)
                                {
                                    course.RIDPKCountDeclined = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task UpdateCourseFileNameAsync(int idCourse)
        {
            try
            {
                var courseFromDb = await this.repository.GetByIdAsync<Course>(idCourse);
                if (courseFromDb is not null)
                {
                    courseFromDb.UploadedFileName = null;

                    this.repository.Update<Course>(courseFromDb);
                    await this.repository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        // проверява дали ЦПО-то е с отнета лицензия към момента на започване на курса
        public async Task<bool> IsCandidateProviderLicenceSuspendedAsync(int idCandidateProvider, DateTime courseStartDate)
        {
            try
            {
                var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(idCandidateProvider);
                if (candidateProviderFromDb is not null)
                {
                    var kvLicenceSuspendedIds = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicenseStatus"))
                        .Where(x => x.KeyValueIntCode == "SuspendedFor3Months" || x.KeyValueIntCode == "SuspendedFor6Months" || x.KeyValueIntCode == "SuspendedFor4Months")
                        .Select(x => x.IdKeyValue)
                        .ToList();
                    if (candidateProviderFromDb.IdLicenceStatus.HasValue)
                    {
                        if (kvLicenceSuspendedIds.Contains(candidateProviderFromDb.IdLicenceStatus.Value))
                        {
                            var licenceChange = await this.repository.AllReadonly<CandidateProviderLicenceChange>(x => x.IdCandidate_Provider == idCandidateProvider && kvLicenceSuspendedIds.Contains(x.IdStatus))
                                .OrderByDescending(x => x.ChangeDate)
                                .FirstOrDefaultAsync();
                            if (licenceChange is not null)
                            {
                                return courseStartDate.Date >= licenceChange.ChangeDate.Date;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return false;
        }

        private async Task HandleTrainersAddFromXMLImportAsync(CourseCollection model, int idCourse)
        {
            var trainingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
            var kvTheory = trainingTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "TheoryTraining").IdKeyValue;
            var kvPractice = trainingTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "PracticalTraining").IdKeyValue;
            var kvPracticeAndTheory = trainingTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "TrainingInTheoryAndPractice").IdKeyValue;
            var kvIdentTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            var kvEGN = kvIdentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "EGN").IdKeyValue;
            var kvIDN = kvIdentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "IDN").IdKeyValue;
            foreach (var trainer in model.Trainers.TrainerList)
            {
                var idIdentType = trainer.TrainerIdType == 0 ? kvEGN : kvIDN;
                var candidateProviderTrainer = await this.repository.AllReadonly<CandidateProviderTrainer>(x => x.IdCandidate_Provider == this.UserProps.IdCandidateProvider
                    && x.IdIndentType == idIdentType && x.Indent == trainer.TrainerId.Trim()).FirstOrDefaultAsync();
                if (candidateProviderTrainer is not null)
                {
                    int idTrainingType;
                    if (trainer.TrainerIsTheory == "1" && trainer.TrainerIsPractice == "0")
                    {
                        idTrainingType = kvTheory;
                    }
                    else if (trainer.TrainerIsPractice == "1" && trainer.TrainerIsTheory == "0")
                    {
                        idTrainingType = kvPractice;
                    }
                    else
                    {
                        idTrainingType = kvPracticeAndTheory;
                    }

                    var trainerCourse = new TrainerCourse()
                    {
                        IdTrainer = candidateProviderTrainer.IdCandidateProviderTrainer,
                        IdCourse = idCourse,
                        IdТraininType = idTrainingType
                    };

                    await this.repository.AddAsync<TrainerCourse>(trainerCourse);
                    await this.repository.SaveChangesAsync();
                }
            }
        }

        private async Task<int> HandleCourseAddToDbFromXMLImportAsync(CourseCollection model, int idProgram)
        {
            var program = await this.repository.GetByIdAsync<Program>(idProgram);
            var courseGroup = model.CourseGroup;
            var course = new Course()
            {
                CourseName = courseGroup.GroupName.Trim(),
                IdTrainingCourseType = this.GetIdCourseTypeFromXMLCourseImport(courseGroup.CourseType),
                SubscribeDate = !string.IsNullOrEmpty(courseGroup.GroupSubscribeDate) ? DateTime.Parse(courseGroup.GroupSubscribeDate) : null,
                IdStatus = this.GetIdStatusFromXMLCourseImport(courseGroup.GroupStatus),
                IdMeasureType = !string.IsNullOrEmpty(courseGroup.GroupMeasureType) ? await this.GetIdMeasureTypeFromXMLCourseImport(courseGroup._GroupMeasureType.Value) : null,
                IdAssignType = await this.GetIdAssignTypeFromXMLCourseImport(courseGroup.GroupAssignType),
                IdFormEducation = await this.GetIdFormEducationFromXMLCourseImport(courseGroup.GroupEducationForm),
                DurationHours = program.SelectableHours + program.MandatoryHours,
                Cost = courseGroup.GroupCost,
                StartDate = courseGroup._GroupStartDate,
                EndDate = courseGroup._GroupEndDate,
                ExamTheoryDate = !string.IsNullOrEmpty(courseGroup.GroupExamTheoryDate) ? DateTime.Parse(courseGroup.GroupExamTheoryDate) : null,
                ExamPracticeDate = !string.IsNullOrEmpty(courseGroup.GroupExamPracticeDate) ? DateTime.Parse(courseGroup.GroupExamPracticeDate) : null,
                IdCandidateProvider = this.UserProps.IdCandidateProvider,
                IdProgram = idProgram,
                SelectableHours = program.SelectableHours,
                MandatoryHours = program.MandatoryHours
            };

            await this.repository.AddAsync<Course>(course);
            await this.repository.SaveChangesAsync();

            await this.CopyTrainingCurriculumToTrainingCourseAsync(new CourseVM() { IdCourse = course.IdCourse, IdProgram = idProgram });
            await this.CopySubjectsToStartedCourseAsync(new CourseVM() { IdCourse = course.IdCourse });

            return course.IdCourse;
        }

        private async Task<int> HandleProgramAddFromXMLImportAsync(CourseCollection model)
        {
            var courseGroup = model.CourseGroup;
            var specialities = this.dataSourceService.GetAllSpecialitiesList();
            var speciality = specialities.FirstOrDefault(x => x.Code == courseGroup.SpecialityId.ToString().Trim());
            var program = new Program()
            {
                ProgramName = courseGroup.CourseName,
                ProgramNote = !string.IsNullOrEmpty(courseGroup.CourseNotes) ? courseGroup.CourseNotes : null,
                IdCandidateProvider = this.UserProps.IdCandidateProvider,
                IdSpeciality = speciality.IdSpeciality,
                IdFrameworkProgram = !string.IsNullOrEmpty(courseGroup.CourseFrameProgram) ? await this.GetIdFrameworkProgramFromXMCourseImport(courseGroup._CourseFrameProgram.Value) : null,
                IdCourseType = this.GetIdCourseTypeFromXMLCourseImport(courseGroup.CourseType),
                IsDeleted = false,
                IdMinimumLevelEducation = !string.IsNullOrEmpty(courseGroup.CourseEducationalRequirements) ? await this.GetIdMinumumLevelEducationXMLCourseImport(courseGroup._CourseEducationalRequirements.Value) : 0
            };

            await this.repository.AddAsync<Program>(program);
            await this.repository.SaveChangesAsync();

            await this.CopyCurriculumAsync(new ProgramVM() { IdProgram = program.IdProgram, IdCandidateProvider = this.UserProps.IdCandidateProvider, IdSpeciality = speciality.IdSpeciality });

            return program.IdProgram;
        }

        private async Task<int> HandleClientsAddFromXMLImportAsync(CourseCollection model, int idCourse, int idProgram)
        {
            var program = await this.repository.GetByIdAsync<Program>(idProgram);
            var kvIdentTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            var kvEGN = kvIdentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "EGN").IdKeyValue;
            var kvIDN = kvIdentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "IDN").IdKeyValue;
            var kvSexSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");
            var kvMale = kvSexSource.FirstOrDefault(x => x.KeyValueIntCode == "Man").IdKeyValue;
            var kvFemale = kvSexSource.FirstOrDefault(x => x.KeyValueIntCode == "Woman").IdKeyValue;
            foreach (var client in model.Students.StudentList)
            {
                var idClient = 0;
                var idIdentType = client.StudentIdType == 0 ? kvEGN : kvIDN;
                var clientFromDb = await this.repository.AllReadonly<Client>(x => x.IdIndentType == idIdentType && x.IdCandidateProvider == this.UserProps.IdCandidateProvider && x.Indent == client.StudentId.Trim()).FirstOrDefaultAsync();
                if (clientFromDb is null)
                {
                    var clientForDb = new Client()
                    {
                        FirstName = client.StudentFirstName.Trim(),
                        SecondName = !string.IsNullOrEmpty(client.StudentMiddleName) ? client.StudentMiddleName.Trim() : null,
                        FamilyName = client.StudentLastName.Trim(),
                        FirstNameEN = BaseHelper.ConvertCyrToLatin(client.StudentFirstName.Trim()),
                        SecondNameEN = !string.IsNullOrEmpty(client.StudentMiddleName) ? BaseHelper.ConvertCyrToLatin(client.StudentMiddleName.Trim()) : null,
                        FamilyNameEN = BaseHelper.ConvertCyrToLatin(client.StudentLastName.Trim()),
                        IdCandidateProvider = this.UserProps.IdCandidateProvider,
                        IdSex = client.StudentGender == "1" ? kvMale : kvFemale,
                        IdIndentType = client.StudentIdType == 0 ? kvEGN : kvIDN,
                        Indent = client.StudentId.Trim(),
                        BirthDate = DateTime.Parse(client.StudentBirthDate),
                        IdNationality = !string.IsNullOrEmpty(client.StudentNationality) ? await this.GetIdNationalityFromXMLCourseImport(int.Parse(client.StudentNationality)) : null
                    };

                    await this.repository.AddAsync<Client>(clientForDb);
                    await this.repository.SaveChangesAsync();

                    idClient = clientForDb.IdClient;
                }

                var clientCourseFromDb = await this.repository.AllReadonly<ClientCourse>(x => x.IdIndentType == idIdentType && x.IdCourse == idCourse && x.Indent == client.StudentId.Trim()).FirstOrDefaultAsync();
                if (clientCourseFromDb == null)
                {
                    var clientCourse = new ClientCourse()
                    {
                        IdClient = clientFromDb is not null ? clientFromDb.IdClient : idClient,
                        IdCourse = idCourse,
                        FirstName = client.StudentFirstName.Trim(),
                        SecondName = !string.IsNullOrEmpty(client.StudentMiddleName) ? client.StudentMiddleName.Trim() : null,
                        FamilyName = client.StudentLastName.Trim(),
                        IdSex = client.StudentGender == "1" ? kvMale : kvFemale,
                        IdIndentType = client.StudentIdType == 0 ? kvEGN : kvIDN,
                        Indent = client.StudentId.Trim(),
                        BirthDate = DateTime.Parse(client.StudentBirthDate),
                        IdNationality = !string.IsNullOrEmpty(client.StudentNationality) ? await this.GetIdNationalityFromXMLCourseImport(int.Parse(client.StudentNationality)) : null,
                        IdSpeciality = program.IdSpeciality,
                        IdAssignType = !string.IsNullOrEmpty(client.StudentAssingType) ? await this.GetIdAssignTypeFromXMLCourseImport(client._StudentAssingType.Value) : null,
                        IdFinishedType = !string.IsNullOrEmpty(client.StudentGraduationStatus) ? await this.GetIdGraduationStatusFromXMCourseImport(client._StudentGraduationStatus.Value) : null,
                        FinishedDate = !string.IsNullOrEmpty(client.StudentEndDate) ? DateTime.Parse(client.StudentEndDate) : null,
                    };

                    await this.repository.AddAsync<ClientCourse>(clientCourse);
                    await this.repository.SaveChangesAsync();

                    var clientCourseDocumentFromDb = await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourse == clientCourse.IdClientCourse).FirstOrDefaultAsync();
                    if (clientCourseDocumentFromDb is null)
                    {
                        var clientCourseDocument = new ClientCourseDocument()
                        {
                            IdClientCourse = clientCourse.IdClientCourse,
                            IdDocumentType = !string.IsNullOrEmpty(client.DocumentType) && client._DocumentType.Value <= 3 ? await this.GetIdDocumentTypeFromXMCourseImport(client._DocumentType.Value) : null,
                            FinishedYear = !string.IsNullOrEmpty(client.DocumentGraduationYear) ? client._DocumentGraduationYear.Value : null,
                            DocumentSerNo = !string.IsNullOrEmpty(client.DocumentSeries) ? client.DocumentSeries : null,
                            DocumentPrnNo = !string.IsNullOrEmpty(client.DocumentSerialNumber) ? client.DocumentSerialNumber : null,
                            IdDocumentSerialNumber = !string.IsNullOrEmpty(client.DocumentSerialNumber) && !string.IsNullOrEmpty(client.DocumentGraduationYear) ? await this.GetDocumentSerialNumberByIdCandidateProviderByYearAndBySerialNumber(client.DocumentSerialNumber.ToString(), client._DocumentGraduationYear.Value) : null,
                            DocumentRegNo = !string.IsNullOrEmpty(client.DocumentRegistrationNumber) ? client.DocumentRegistrationNumber : null,
                            DocumentDate = !string.IsNullOrEmpty(client.DocumentPublishingDate) ? DateTime.Parse(client.DocumentPublishingDate) : null,
                            DocumentProtocol = !string.IsNullOrEmpty(client.DocumentProtocolNumber) ? client.DocumentProtocolNumber : null,
                            TheoryResult = !string.IsNullOrEmpty(client.DocumentTheoryGrade) ? (decimal)client._DocumentTheoryGrade.Value : null,
                            PracticeResult = !string.IsNullOrEmpty(client.DocumentPracticeGrade) ? (decimal)client._DocumentPracticeGrade.Value : null
                        };

                        await this.repository.AddAsync<ClientCourseDocument>(clientCourseDocument);
                        await this.repository.SaveChangesAsync();

                        if (clientCourseDocument.IdDocumentSerialNumber is not null)
                        {
                            await this.HandleDocumentSerialNumberAndRequestDocManagementFromXMLImportAsync(client, clientCourseDocument);
                        }
                    }
                }
            }

            return 0;
        }

        private async Task HandleDocumentSerialNumberAndRequestDocManagementFromXMLImportAsync(Student client, ClientCourseDocument clientCourseDocument)
        {
            var docStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
            var kvDocPrinted = docStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Printed");

            RequestDocumentManagement requestDocumentManagement = new RequestDocumentManagement()
            {
                IdCandidateProvider = this.UserProps.IdCandidateProvider,
                IdTypeOfRequestedDocument = clientCourseDocument.IdDocumentType.Value,
                DocumentCount = 1,
                DocumentDate = clientCourseDocument.DocumentDate.Value,
                IdDocumentOperation = kvDocPrinted.IdKeyValue,
                ReceiveDocumentYear = client._DocumentGraduationYear.Value
            };

            await this.repository.AddAsync<RequestDocumentManagement>(requestDocumentManagement);
            await this.repository.SaveChangesAsync();

            DocumentSerialNumber documentSerialNumber = new DocumentSerialNumber()
            {
                IdRequestDocumentManagement = requestDocumentManagement.IdRequestDocumentManagement,
                IdCandidateProvider = this.UserProps.IdCandidateProvider,
                IdTypeOfRequestedDocument = clientCourseDocument.IdDocumentType.Value,
                DocumentDate = clientCourseDocument.DocumentDate.Value,
                SerialNumber = clientCourseDocument.DocumentPrnNo,
                IdDocumentOperation = kvDocPrinted.IdKeyValue,
                ReceiveDocumentYear = client._DocumentGraduationYear.Value
            };

            await this.repository.AddAsync<DocumentSerialNumber>(documentSerialNumber);
            await this.repository.SaveChangesAsync();

            var docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == clientCourseDocument.IdDocumentType.Value && x.Year == clientCourseDocument.FinishedYear).FirstOrDefaultAsync();

            clientCourseDocument.IdDocumentSerialNumber = documentSerialNumber.IdDocumentSerialNumber;
            clientCourseDocument.DocumentPrnNo = $"{docSeries?.SeriesName}/{documentSerialNumber.SerialNumber}";
            clientCourseDocument.DocumentSerNo = docSeries?.SeriesName;
            this.repository.Update<ClientCourseDocument>(clientCourseDocument);
            await this.repository.SaveChangesAsync();
        }

        private async Task<int> GetDocumentSerialNumberByIdCandidateProviderByYearAndBySerialNumber(string serialNumber, int year)
        {
            var data = await this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdCandidateProvider == this.UserProps.IdCandidateProvider && x.SerialNumber == serialNumber && x.ReceiveDocumentYear == year).ToListAsync();
            if (data.Count == 1)
            {
                return data.FirstOrDefault().IdDocumentSerialNumber;
            }

            return 0;
        }

        private int GetIdCourseTypeFromXMLCourseImport(int courseType)
        {
            return this.trainingCourseTypeSource.FirstOrDefault(x => !string.IsNullOrEmpty(x.DefaultValue5) && x.DefaultValue5 == courseType.ToString()).IdKeyValue;
        }

        private async Task<int> GetIdMinumumLevelEducationXMLCourseImport(int courseEducationRequirements)
        {
            var educationTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Education");
            return educationTypeSource.FirstOrDefault(x => !string.IsNullOrEmpty(x.DefaultValue5) && x.DefaultValue5 == courseEducationRequirements.ToString()).IdKeyValue;
        }

        private int GetIdStatusFromXMLCourseImport(int groupStatus)
        {
            //return this.courseTypesSource.FirstOrDefault(x => int.Parse(x.DefaultValue1) == groupStatus).IdKeyValue;
            return this.kvCurrentCourse.IdKeyValue;
        }

        private async Task<int> GetIdMeasureTypeFromXMLCourseImport(int groupMeasureType)
        {
            var kvMeasureTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MeasureType");
            return kvMeasureTypeSource.FirstOrDefault(x => x.DefaultValue1 == groupMeasureType.ToString()).IdKeyValue;
        }

        private async Task<int> GetIdAssignTypeFromXMLCourseImport(int groupAssingType)
        {
            var kvAssingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType", false, true);
            return kvAssingTypeSource.FirstOrDefault(x => !string.IsNullOrEmpty(x.DefaultValue5) && x.DefaultValue5 == groupAssingType.ToString()).IdKeyValue;
        }

        private async Task<int> GetIdFormEducationFromXMLCourseImport(int groupEducationForm)
        {
            var kvFormEducationSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation");
            return kvFormEducationSource.FirstOrDefault(x => !string.IsNullOrEmpty(x.DefaultValue5) && x.DefaultValue5 == groupEducationForm.ToString()).IdKeyValue;
        }

        private async Task<int> GetIdFrameworkProgramFromXMCourseImport(int courseFrameProgram)
        {
            var frameworkProgramSource = await this.repository.AllReadonly<FrameworkProgram>().ToListAsync();
            return frameworkProgramSource.FirstOrDefault(x => !string.IsNullOrEmpty(x.DefaultValue1) && x.DefaultValue1 == courseFrameProgram.ToString()).IdFrameworkProgram;
        }

        private async Task<int> GetIdDocumentTypeFromXMCourseImport(int documentType)
        {
            var typeOfRequestedDocumentSource = await this.repository.AllReadonly<TypeOfRequestedDocument>().ToListAsync();
            return typeOfRequestedDocumentSource.FirstOrDefault(x => x.DefaultValue1 == documentType.ToString()).IdTypeOfRequestedDocument;
        }

        private async Task<int> GetIdGraduationStatusFromXMCourseImport(int studentGraduationStatus)
        {
            var courseFinishTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseFinishedType");
            return courseFinishTypeSource.FirstOrDefault(x => x.DefaultValue2 == studentGraduationStatus.ToString()).IdKeyValue;
        }

        private async Task<int> GetIdNationalityFromXMLCourseImport(int idNationality)
        {
            var kvNationalitySource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality");
            return kvNationalitySource.FirstOrDefault(x => x.DefaultValue1 == idNationality.ToString()).IdKeyValue;
        }

        private async Task ValidateXMLTrainerAsync(ResultContext<CourseCollection> resultContext, Trainer[] trainersList)
        {
            var identTypes = new List<int>()
                            {
                                0, 1
                            };
            var nationalityList = new List<int>()
                        {
                            1,
                            2,
                            3,
                            4,
                            5,
                            6,
                            7,
                            8,
                            9,
                            10,
                            11,
                            12,
                            13,
                            14,
                            15,
                            16,
                            17,
                            18,
                            19,
                            20,
                            21,
                            22,
                            23,
                            24,
                            25,
                            26,
                            27,
                            28,
                            29,
                            30,
                            31,
                            32,
                            34,
                            35,
                            36,
                            37,
                            38,
                            39,
                            40,
                            41,
                            42,
                            43,
                            44,
                            45,
                            46,
                            47,
                            48,
                            49,
                            50,
                            51,
                            52,
                            53,
                            54,
                            55,
                            56,
                            57,
                            58,
                            59,
                            60,
                            61,
                            62,
                            63,
                            64,
                            65,
                            66,
                            67,
                            68,
                            69,
                            70,
                            71,
                            72,
                            73,
                            74,
                            75,
                            76,
                            77,
                            78,
                            79,
                            80,
                            81,
                            82,
                            83,
                            84,
                            85,
                            86,
                            87,
                            88,
                            89,
                            90,
                            91,
                            92,
                            93,
                            94,
                            95,
                            96,
                            97,
                            98,
                            99,
                            100,
                            101,
                            102,
                            103,
                            104,
                            105,
                            106,
                            107,
                            108,
                            109,
                            110,
                            111,
                            112,
                            113,
                            114,
                            115,
                            116,
                            117,
                            118,
                            119,
                            120,
                            121,
                            122,
                            123,
                            124,
                            125,
                            126,
                            127,
                            128,
                            129,
                            130,
                            131,
                            132,
                            133,
                            134,
                            135,
                            136,
                            137,
                            138,
                            139,
                            140,
                            141,
                            142,
                            143,
                            144,
                            145,
                            146,
                            147,
                            148,
                            149,
                            150,
                            151,
                            152,
                            153,
                            154,
                            155,
                            156,
                            157,
                            158,
                            159,
                            160,
                            161,
                            162,
                            163,
                            164,
                            165,
                            166,
                            167,
                            168,
                            169,
                            170,
                            171,
                            172,
                            173,
                            174,
                            175,
                            176,
                            177,
                            178,
                            179,
                            180,
                            181,
                            182,
                            183,
                            184,
                            185,
                            186,
                            187,
                            188,
                            189,
                            190,
                            191,
                            192,
                            193,
                            194,
                            195,
                            196,
                            197,
                            198,
                            199,
                            200,
                            201,
                            202,
                            203,
                            204,
                            205,
                            206,
                            207,
                            208,
                            209,
                            210,
                            211,
                            212,
                            213,
                            214,
                            215,
                            216,
                            217,
                            218,
                            219,
                            220,
                            221,
                            222,
                            223,
                            224,
                            225,
                            226,
                            227,
                            228,
                            229,
                            230,
                            231,
                            232,
                            233,
                            234,
                            235,
                            236,
                            237,
                            238,
                            239,
                            240,
                            241,
                            242,
                            243,
                            244,
                            245,
                            246,
                            999
                        };
            List<CandidateProviderTrainer> trainersFromDb = null;
            if (trainersList.Any())
            {
                trainersFromDb = await this.repository.AllReadonly<CandidateProviderTrainer>(x => x.IdCandidate_Provider == this.UserProps.IdCandidateProvider).ToListAsync();
            }

            foreach (var trainer in trainersList)
            {
                if (!identTypes.Any(x => x == trainer.TrainerIdType))
                {
                    resultContext.AddErrorMessage("Не е въведена валидна стойност за вид на идентификатора на преподавателя!");
                }
                else
                {
                    var egn = 0;
                    var identType = trainer.TrainerIdType == 0 ? "ЕГН" : "ИДН";
                    if (trainer.TrainerId.Length != 10)
                    {
                        resultContext.AddErrorMessage($"Не е въведена валидна стойност за {identType} на преподавателя!");
                    }
                    else
                    {
                        if (!trainersFromDb!.Any(x => x.Indent != null && x.Indent.Trim() == trainer.TrainerId.Trim()))
                        {
                            resultContext.AddErrorMessage($"Не е намерен преподавател с {identType} {trainer.TrainerId} в базата данни. Моля, въведете данните за преподавателя в 'Профил ЦПО' преди да импортирате!");
                            continue;
                        }

                        if (trainer.TrainerIdType == egn)
                        {
                            var checkEGN = new BasicEGNValidation(trainer.TrainerId);
                            if (!checkEGN.Validate())
                            {
                                resultContext.AddErrorMessage($"Не е въведена валидна стойност за ЕГН!");
                            }
                            else
                            {
                                trainer.TrainerGender = trainer.TrainerId[8] % 2 == 0 ? "1" : "2";

                                DateTime birthDate = this.GetBirthDateFromEGNValue(trainer.TrainerId);
                                trainer.TrainerBirthYear = birthDate.ToString();
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(trainer.TrainerFirstName))
                    {
                        resultContext.AddErrorMessage("Не е въведена стойност за първо име на преподавател!");
                    }
                    else
                    {
                        if (!Regex.IsMatch(trainer.TrainerFirstName, @"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*"))
                        {
                            resultContext.AddErrorMessage($"Име на преподавател може да съдържа само символи на кирилица!");
                        }
                    }

                    if (!string.IsNullOrEmpty(trainer.TrainerMiddleName))
                    {
                        if (!Regex.IsMatch(trainer.TrainerMiddleName, @"\p{IsCyrillic}*\s*-*"))
                        {
                            resultContext.AddErrorMessage($"Презиме на преподавател може да съдържа само символи на кирилица!");
                        }
                    }

                    if (string.IsNullOrEmpty(trainer.TrainerLastName))
                    {
                        resultContext.AddErrorMessage("Не е въведена стойност за фамилия на преподавател!");
                    }
                    else
                    {
                        if (!Regex.IsMatch(trainer.TrainerLastName, @"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*"))
                        {
                            resultContext.AddErrorMessage($"Фамилия на преподавател може да съдържа само символи на кирилица!");
                        }
                    }

                    if (trainer._TrainerNationality.HasValue)
                    {
                        if (!nationalityList.Any(x => x == trainer._TrainerNationality.Value))
                        {
                            resultContext.AddErrorMessage($"Не е въведена валидна стойност за гражданство на преподавател!");
                        }
                    }

                    if (trainer._TrainerIsTheory.HasValue)
                    {
                        if (trainer._TrainerIsTheory.Value != 0 && trainer._TrainerIsTheory != 1)
                        {
                            resultContext.AddErrorMessage($"Не е въведена валидна стойност дали преподавател преподава по теория!");
                        }
                    }

                    if (trainer._TrainerIsPractice.HasValue)
                    {
                        if (trainer._TrainerIsPractice.Value != 0 && trainer._TrainerIsPractice != 1)
                        {
                            resultContext.AddErrorMessage($"Не е въведена валидна стойност дали преподавател преподава по практика!");
                        }
                    }
                }
            }
        }

        private void ValidateXMLStudent(ResultContext<CourseCollection> resultContext, Student[] studentsList)
        {
            var nationalityList = new List<int>()
                        {
                            1,
                            2,
                            3,
                            4,
                            5,
                            6,
                            7,
                            8,
                            9,
                            10,
                            11,
                            12,
                            13,
                            14,
                            15,
                            16,
                            17,
                            18,
                            19,
                            20,
                            21,
                            22,
                            23,
                            24,
                            25,
                            26,
                            27,
                            28,
                            29,
                            30,
                            31,
                            32,
                            34,
                            35,
                            36,
                            37,
                            38,
                            39,
                            40,
                            41,
                            42,
                            43,
                            44,
                            45,
                            46,
                            47,
                            48,
                            49,
                            50,
                            51,
                            52,
                            53,
                            54,
                            55,
                            56,
                            57,
                            58,
                            59,
                            60,
                            61,
                            62,
                            63,
                            64,
                            65,
                            66,
                            67,
                            68,
                            69,
                            70,
                            71,
                            72,
                            73,
                            74,
                            75,
                            76,
                            77,
                            78,
                            79,
                            80,
                            81,
                            82,
                            83,
                            84,
                            85,
                            86,
                            87,
                            88,
                            89,
                            90,
                            91,
                            92,
                            93,
                            94,
                            95,
                            96,
                            97,
                            98,
                            99,
                            100,
                            101,
                            102,
                            103,
                            104,
                            105,
                            106,
                            107,
                            108,
                            109,
                            110,
                            111,
                            112,
                            113,
                            114,
                            115,
                            116,
                            117,
                            118,
                            119,
                            120,
                            121,
                            122,
                            123,
                            124,
                            125,
                            126,
                            127,
                            128,
                            129,
                            130,
                            131,
                            132,
                            133,
                            134,
                            135,
                            136,
                            137,
                            138,
                            139,
                            140,
                            141,
                            142,
                            143,
                            144,
                            145,
                            146,
                            147,
                            148,
                            149,
                            150,
                            151,
                            152,
                            153,
                            154,
                            155,
                            156,
                            157,
                            158,
                            159,
                            160,
                            161,
                            162,
                            163,
                            164,
                            165,
                            166,
                            167,
                            168,
                            169,
                            170,
                            171,
                            172,
                            173,
                            174,
                            175,
                            176,
                            177,
                            178,
                            179,
                            180,
                            181,
                            182,
                            183,
                            184,
                            185,
                            186,
                            187,
                            188,
                            189,
                            190,
                            191,
                            192,
                            193,
                            194,
                            195,
                            196,
                            197,
                            198,
                            199,
                            200,
                            201,
                            202,
                            203,
                            204,
                            205,
                            206,
                            207,
                            208,
                            209,
                            210,
                            211,
                            212,
                            213,
                            214,
                            215,
                            216,
                            217,
                            218,
                            219,
                            220,
                            221,
                            222,
                            223,
                            224,
                            225,
                            226,
                            227,
                            228,
                            229,
                            230,
                            231,
                            232,
                            233,
                            234,
                            235,
                            236,
                            237,
                            238,
                            239,
                            240,
                            241,
                            242,
                            243,
                            244,
                            245,
                            246,
                            999
                        };
            var statusList = new List<int>()
                        {
                            1, 2, 3, 4, 5
                        };
            var assignTypeList = new List<int>()
                        {
                            1, 2, 3, 4, 5, 6, 7, 8, 9, 10
                        };
            var documentTypeList = new List<int>()
                        {
                            1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12
                        };
            var identTypes = new List<int>()
                            {
                                0, 1
                            };
            foreach (var student in studentsList)
            {
                if (!identTypes.Any(x => x == student.StudentIdType))
                {
                    resultContext.AddErrorMessage("Не е въведена валидна стойност за вид на идентификатора на курсиста!");
                }
                else
                {
                    var egn = 0;
                    if (student.StudentId.Length != 10)
                    {
                        var identType = student.StudentIdType == 0 ? "ЕГН" : "ИДН";
                        resultContext.AddErrorMessage($"Не е въведена валидна стойност за {identType} на курсиста!");
                    }
                    else
                    {
                        if (student.StudentIdType == egn)
                        {
                            var checkEGN = new BasicEGNValidation(student.StudentId);
                            if (!checkEGN.Validate())
                            {
                                resultContext.AddErrorMessage($"Не е въведена валидна стойност за ЕГН!");
                            }
                            else
                            {
                                student.StudentGender = student.StudentId[8] % 2 == 0 ? "1" : "2";

                                DateTime birthDate = this.GetBirthDateFromEGNValue(student.StudentId);
                                student.StudentBirthDate = birthDate.ToString();
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(student.StudentFirstName))
                {
                    resultContext.AddErrorMessage("Не е въведена стойност за първо име на курсист!");
                }
                else
                {
                    if (!Regex.IsMatch(student.StudentFirstName, @"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*"))
                    {
                        resultContext.AddErrorMessage($"Име на курсист може да съдържа само символи на кирилица!");
                    }
                }

                if (!string.IsNullOrEmpty(student.StudentMiddleName))
                {
                    if (!Regex.IsMatch(student.StudentMiddleName, @"\p{IsCyrillic}*\s*-*"))
                    {
                        resultContext.AddErrorMessage($"Презиме на курсист може да съдържа само символи на кирилица!");
                    }
                }

                if (string.IsNullOrEmpty(student.StudentLastName))
                {
                    resultContext.AddErrorMessage("Не е въведена стойност за фамилия на курсист!");
                }
                else
                {
                    if (!Regex.IsMatch(student.StudentLastName, @"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*"))
                    {
                        resultContext.AddErrorMessage($"Фамилия на курсист може да съдържа само символи на кирилица!");
                    }
                }

                if (student._StudentNationality.HasValue)
                {
                    if (!nationalityList.Any(x => x == student._StudentNationality.Value))
                    {
                        resultContext.AddErrorMessage($"Не е въведена валидна стойност за гражданство на курсист!");
                    }
                }

                if (student._StudentGraduationStatus.HasValue)
                {
                    if (!statusList.Any(x => x == student._StudentGraduationStatus.Value))
                    {
                        resultContext.AddErrorMessage($"Не е въведена валидна стойност за допълнителни данни за завършване на курсист!");
                    }
                }

                if (student._StudentEndDate.HasValue)
                {
                    if (!DateTime.TryParse(student.StudentEndDate, out DateTime endDate))
                    {
                        resultContext.AddErrorMessage($"Не е въведена валидна стойност за дата на приключване на курса от курсист!");
                    }
                }

                if (student._StudentAssingType.HasValue)
                {
                    if (!assignTypeList.Any(x => x == student._StudentAssingType.Value))
                    {
                        resultContext.AddErrorMessage($"Не е въведена валидна стойност за вид на финансиране на курсиста!");
                    }
                }

                if (student._DocumentType.HasValue)
                {
                    if (!documentTypeList.Any(x => x == student._DocumentType.Value))
                    {
                        resultContext.AddErrorMessage($"Не е въведена валидна стойност за вид на документа на курсиста!");
                    }
                }

                if (student._DocumentGraduationYear.HasValue)
                {
                    if (student._DocumentGraduationYear.Value.ToString().Length != 4)
                    {
                        resultContext.AddErrorMessage($"Не е въведена валидна стойност за година на завършване на курсиста!");
                    }
                }

                if (student._DocumentPublishingDate.HasValue)
                {
                    if (!DateTime.TryParse(student.DocumentPublishingDate, out DateTime publishingDate))
                    {
                        resultContext.AddErrorMessage($"Не е въведена валидна стойност за дата на издаване на документа на курсиста!");
                    }
                }

                if (student._DocumentTheoryGrade.HasValue)
                {
                    double theory;
                    if (!double.TryParse(student.DocumentTheoryGrade, out theory))
                    {
                        resultContext.AddErrorMessage($"Не е въведена валидна стойност за оценка по теория!");
                    }
                    else
                    {
                        if (theory < 2 || theory > 6)
                        {
                            resultContext.AddErrorMessage($"Не е въведена валидна стойност за оценка по теория!");
                        }
                    }
                }

                if (student._DocumentPracticeGrade.HasValue)
                {
                    double practice;
                    if (!double.TryParse(student.DocumentPracticeGrade, out practice))
                    {
                        resultContext.AddErrorMessage($"Не е въведена валидна стойност за оценка по практика!");
                    }
                    else
                    {
                        if (practice < 2 || practice > 6)
                        {
                            resultContext.AddErrorMessage($"Не е въведена валидна стойност за оценка по практика!");
                        }
                    }
                }
            }
        }

        private DateTime GetBirthDateFromEGNValue(string egn)
        {
            int year = int.Parse(egn.Substring(0, 2));
            int month = int.Parse(egn.Substring(2, 2));
            int day = int.Parse(egn.Substring(4, 2));
            if (month < 13)
            {
                year += 1900;
            }
            else if (month > 20 && month < 33)
            {
                year += 1800;
                month -= 20;
            }
            else if (month > 40 && month < 53)
            {
                year += 2000;
                month -= 40;
            }

            var birthDate = new DateTime(year, month, day);
            return birthDate;
        }

        private void ValidateXMLModule(ResultContext<CourseCollection> resultContext, Module[] modules)
        {
            var curricTypes = new List<int>()
            {
                3, 4, 5, 6, 7, 8
            };

            foreach (var module in modules)
            {
                if (!curricTypes.Any(x => x == module.CurricType))
                {
                    resultContext.AddErrorMessage($"Към предмет {module.Name} не е въведен валиден код за вид на предмет за обучение!");
                }

                if (!int.TryParse(module.Duration.ToString(), out int duration))
                {
                    resultContext.AddErrorMessage($"Към предмет {module.Name} не е въведена валидна стойност за продължителност в часове!");
                }
            }
        }

        private void ValidateXMLCourseGroup(ResultContext<CourseCollection> resultContext, CourseGroup courseGroup)
        {
            if (string.IsNullOrEmpty(courseGroup.CourseName))
            {
                resultContext.AddErrorMessage("Не е въведено име на курс!");
            }

            var specialities = this.dataSourceService.GetAllSpecialitiesList();
            var speciality = specialities.FirstOrDefault(x => x.Code == courseGroup.SpecialityId.ToString());
            if (speciality is null)
            {
                resultContext.AddErrorMessage("Не е въведен валиден код на специалността по СППОО!");
            }

            var courseTypes = new List<int>()
                        {
                            1, 2, 3, 4, 5, 6, 7, 8, 21, 22, 99
                        };

            if (!courseTypes.Any(x => x == courseGroup.CourseType))
            {
                resultContext.AddErrorMessage("Не е въведен валиден вид на курса!");
            }

            if (courseGroup._CourseFrameProgram.HasValue)
            {
                var framePrograms = new List<int>()
                            {
                                1,
                                2,
                                3,
                                4,
                                5,
                                11,
                                20,
                                21,
                                22,
                                30,
                                31,
                                32,
                                33,
                                34,
                                35,
                                40,
                                41,
                                42,
                                43,
                                44,
                                45,
                                50,
                                51,
                                52,
                                53,
                                54,
                                55,
                                56,
                                57,
                                58,
                                59,
                                60,
                                61,
                                62,
                            };

                if (!framePrograms.Any(x => x == courseGroup._CourseFrameProgram.Value))
                {
                    resultContext.AddErrorMessage("Не е въведен валиден код за рамкова програма за курса!");
                }
            }

            if (courseGroup._CourseEducationalRequirements.HasValue)
            {
                var educationRequirements = new List<int>()
                            {
                                1,
                                2,
                                3,
                                11,
                                12,
                                21,
                                22,
                                23,
                                24,
                                25,
                                26,
                                99
                            };

                if (!educationRequirements.Any(x => x == courseGroup._CourseEducationalRequirements.Value))
                {
                    resultContext.AddErrorMessage("Не е въведен валиден код за минимални образователни изисквания!");
                }
            }

            if (string.IsNullOrEmpty(courseGroup.GroupName))
            {
                resultContext.AddErrorMessage("Не е въведена стойност за наименование на групата!");
            }

            if (!DateTime.TryParse(courseGroup.GroupStartDate.ToString(), out DateTime startDate))
            {
                resultContext.AddErrorMessage("Не е въведена валидна стойност за дата на започване на курса!");
            }

            if (!DateTime.TryParse(courseGroup.GroupEndDate.ToString(), out DateTime endDate))
            {
                resultContext.AddErrorMessage("Не е въведена валидна стойност за дата на приключване на курса!");
            }

            if (courseGroup._GroupSubscribeDate.HasValue)
            {
                if (!DateTime.TryParse(courseGroup._GroupSubscribeDate.ToString(), out DateTime subscribeDate))
                {
                    resultContext.AddErrorMessage("Не е въведена валидна стойност за крайна дата на записване в курса!");
                }
            }

            var groupStatuses = new List<int>()
                        {
                            1, 2, 3
                        };

            if (!groupStatuses.Any(x => x == courseGroup.GroupStatus))
            {
                resultContext.AddErrorMessage("Не е въведен валиден код за статус на групата!");
            }

            var groupEducationForms = new List<int>()
                        {
                            1,
                            2,
                            3,
                            4,
                            5,
                            6,
                            7,
                            8,
                            9,
                            10
                        };

            if (!groupEducationForms.Any(x => x == courseGroup.GroupEducationForm))
            {
                resultContext.AddErrorMessage("Не е въведен валиден код за форма на обучение!");
            }

            if (courseGroup._GroupMeasureType.HasValue)
            {
                var groupMeasureTypes = new List<int>()
                            {
                                1, 2
                            };

                if (!groupMeasureTypes.Any(x => x == courseGroup._GroupMeasureType.Value))
                {
                    resultContext.AddErrorMessage("Не е въведен валиден код за вид!");
                }
            }

            var groupAssignTypes = new List<int>()
                        {
                            1,
                            2,
                            3,
                            4,
                            5,
                            6,
                            7,
                            8,
                            9,
                            10
                        };

            if (!groupAssignTypes.Any(x => x == courseGroup.GroupAssignType))
            {
                resultContext.AddErrorMessage("Не е въведен валиден код за основен източник на финансиране!");
            }

            if (!decimal.TryParse(courseGroup.GroupCost.ToString(), out decimal price))
            {
                resultContext.AddErrorMessage("Не е въведена валидна стойност за цена в лева!");
            }

            if (!int.TryParse(courseGroup.GroupDurationInHours.ToString(), out int duration))
            {
                resultContext.AddErrorMessage("Не е въведена валидна стойност за продължителност в часове на курса!");
            }

            if (courseGroup._GroupExamTheoryDate.HasValue)
            {
                if (!DateTime.TryParse(courseGroup._GroupExamTheoryDate.ToString(), out DateTime theoryDate))
                {
                    resultContext.AddErrorMessage("Не е въведена валидна стойност за дата на провеждане на изпита по теория!");
                }
            }

            if (courseGroup._GroupExamPracticeDate.HasValue)
            {
                if (!DateTime.TryParse(courseGroup._GroupExamPracticeDate.ToString(), out DateTime theoryDate))
                {
                    resultContext.AddErrorMessage("Не е въведена валидна стойност за дата на провеждане на изпита по практика!");
                }
            }
        }

        private async Task ValidateXMLGeneralInfoAsync(ResultContext<CourseCollection> resultContext, GeneralInfo generalInfo)
        {
            var candidateProvider = await this.repository.GetByIdAsync<CandidateProvider>(this.UserProps.IdCandidateProvider);
            if (generalInfo.CompanyId.ToString() != candidateProvider.LicenceNumber)
            {
                resultContext.AddErrorMessage("Не е въведен правилен номер на лицензия!");
            }

            if (generalInfo.CompanyName.Trim().ToLower() != candidateProvider.ProviderOwner.ToLower())
            {
                resultContext.AddErrorMessage("Не е въведено правилно наименование на юридическото лице!");
            }

            if (generalInfo.CompanyBulstat != candidateProvider.PoviderBulstat)
            {
                resultContext.AddErrorMessage("Не е въведен правилен булстат на юридическото лице!");
            }
        }

        private CourseCollection DeserializeCourseXML(string path)
        {
            CourseCollection courseCollection;
            XmlSerializer serializer = new XmlSerializer(typeof(CourseCollection));
            using (XmlReader reader = XmlReader.Create(path))
            {
                courseCollection = (CourseCollection)serializer.Deserialize(reader);
            }

            return courseCollection;
        }

        private async Task CopySubjectsToStartedCourseAsync(CourseVM model)
        {
            var curriculums = await this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == model.IdCourse).ToListAsync();
            if (curriculums.Any())
            {
                var dict = new Dictionary<string, List<double>>();
                var kvProfessionalTrainingSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
                var kvA1 = kvProfessionalTrainingSource.FirstOrDefault(x => x.KeyValueIntCode == "A1");
                var kvA2 = kvProfessionalTrainingSource.FirstOrDefault(x => x.KeyValueIntCode == "A2");
                var kvA3 = kvProfessionalTrainingSource.FirstOrDefault(x => x.KeyValueIntCode == "A3");
                var kvB = kvProfessionalTrainingSource.FirstOrDefault(x => x.KeyValueIntCode == "B");
                foreach (var curriculum in curriculums)
                {
                    var practice = curriculum.Practice.HasValue ? (double)curriculum.Practice.Value : 0;
                    var theory = curriculum.Theory.HasValue ? (double)curriculum.Theory.Value : 0;
                    var subject = curriculum.Subject;
                    if (curriculum.IdProfessionalTraining == kvA1.IdKeyValue)
                    {
                        var key = $"A1->{subject}";
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add($"A1->{subject}", new List<double>());
                        }

                        if (dict[key].Count == 0)
                        {
                            dict[key].Add(theory);
                        }
                        else
                        {
                            dict[key][0] += theory;
                        }

                        if (dict[key].Count == 1)
                        {
                            dict[key].Add(practice);
                        }
                        else
                        {
                            dict[key][1] += practice;
                        }
                    }
                    else if (curriculum.IdProfessionalTraining == kvA2.IdKeyValue)
                    {
                        var key = $"A2->{subject}";
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add($"A2->{subject}", new List<double>());
                        }

                        if (dict[key].Count == 0)
                        {
                            dict[key].Add(theory);
                        }
                        else
                        {
                            dict[key][0] += theory;
                        }

                        if (dict[key].Count == 1)
                        {
                            dict[key].Add(practice);
                        }
                        else
                        {
                            dict[key][1] += practice;
                        }
                    }
                    else if (curriculum.IdProfessionalTraining == kvA3.IdKeyValue)
                    {
                        var key = $"A3->{subject}";
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add($"A3->{subject}", new List<double>());
                        }

                        if (dict[key].Count == 0)
                        {
                            dict[key].Add(theory);
                        }
                        else
                        {
                            dict[key][0] += theory;
                        }

                        if (dict[key].Count == 1)
                        {
                            dict[key].Add(practice);
                        }
                        else
                        {
                            dict[key][1] += practice;
                        }
                    }
                    else
                    {
                        var key = $"B->{subject}";
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add($"B->{subject}", new List<double>());
                        }

                        if (dict[key].Count == 0)
                        {
                            dict[key].Add(theory);
                        }
                        else
                        {
                            dict[key][0] += theory;
                        }

                        if (dict[key].Count == 1)
                        {
                            dict[key].Add(practice);
                        }
                        else
                        {
                            dict[key][1] += practice;
                        }
                    }
                }

                foreach (var entry in dict.OrderBy(x => x.Key))
                {
                    var subject = entry.Key.Split("->")[1];
                    int idProfessionalTraining = 0;
                    if (entry.Key.Contains("A1"))
                    {
                        idProfessionalTraining = kvA1.IdKeyValue;
                    }
                    else if (entry.Key.Contains("A2"))
                    {
                        idProfessionalTraining = kvA2.IdKeyValue;
                    }
                    else if (entry.Key.Contains("A3"))
                    {
                        idProfessionalTraining = kvA3.IdKeyValue;
                    }
                    else
                    {
                        idProfessionalTraining = kvB.IdKeyValue;
                    }

                    CourseSubject courseSubject = new CourseSubject()
                    {
                        IdCourse = model.IdCourse,
                        Subject = subject,
                        TheoryHours = entry.Value[0],
                        PracticeHours = entry.Value[1],
                        IdProfessionalTraining = idProfessionalTraining
                    };

                    await this.repository.AddAsync<CourseSubject>(courseSubject);
                }

                await this.repository.SaveChangesAsync();
            }
        }

        #endregion Training course

        #region  Training course checking
        public async Task<List<CourseCheckingVM>> GetAllActiveCourseCheckingsAsync(int IdCourse)
        {
            var result = this.repository.AllReadonly<CourseChecking>(x => x.IdCourse == IdCourse);

            return result.To<CourseCheckingVM>().ToList();
        }
        public async Task<List<CourseCheckingVM>> GetAllActiveCourseCheckingsByIdFollowUpControlAsync(int IdFollowUpControl)
        {
            var result = this.repository.AllReadonly<CourseChecking>(x => x.IdFollowUpControl == IdFollowUpControl);
            var data = result.To<CourseCheckingVM>(x => x.FollowUpControl, x => x.Course.Program.Speciality.Profession, x => x.Course.FormEducation, x => x.Course.Program.FrameworkProgram, x => x.Course.CandidateProviderPremises.Location, x => x.Course.ClientCourses).ToList();
            var kvCourseTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram");
            foreach (var item in data)
            {
                item.Course.Program.CourseTypeName = kvCourseTypes.FirstOrDefault(x => x.IdKeyValue == item.Course.Program.IdCourseType).Name;
            }
            return data;
        }
        public async Task<ResultContext<CourseCheckingVM>> CreateCourseCheckingAsync(ResultContext<CourseCheckingVM> resultContext)
        {
            try
            {
                var entryForDb = resultContext.ResultContextObject.To<CourseChecking>();
                entryForDb.Course = null;

                await this.repository.AddAsync<CourseChecking>(entryForDb);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");

                resultContext.ResultContextObject.IdCourseChecking = entryForDb.IdCourseChecking;
                resultContext.ResultContextObject.IdCreateUser = entryForDb.IdCreateUser;
                resultContext.ResultContextObject.IdModifyUser = entryForDb.IdModifyUser;
                resultContext.ResultContextObject.CreationDate = entryForDb.CreationDate;
                resultContext.ResultContextObject.ModifyDate = entryForDb.ModifyDate;


                this.repository.Detach<CourseChecking>(entryForDb);
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<ResultContext<CourseCheckingVM>> DeleteCourseCheckingAsync(CourseCheckingVM courseCheckingVM)
        {
            var entity = await this.repository.GetByIdAsync<CourseChecking>(courseCheckingVM.IdCourseChecking);
            this.repository.Detach<CourseChecking>(entity);

            ResultContext<CourseCheckingVM> resultContext = new ResultContext<CourseCheckingVM>();

            try
            {
                this.repository.HardDelete<CourseChecking>(entity);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Проверката беше изтрита успешно!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }
        #endregion

        #region Training premises course

        public async Task<IEnumerable<PremisesCourseVM>> GetAllPremisesCoursesByIdCourseAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<PremisesCourse>(x => x.IdCourse == idCourse);
            var dataAsVM = await data.To<PremisesCourseVM>(x => x.CandidateProviderPremises.Location, x => x.CandidateProviderPremises.CandidateProviderPremisesRooms).ToListAsync();

            var trainingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
            var ownershipTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseOwnership");
            var roomTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RoomType");

            foreach (var premisesCourse in dataAsVM)
            {
                var type = trainingTypeSource.FirstOrDefault(x => x.IdKeyValue == premisesCourse.IdТraininType);
                if (type is not null)
                {
                    premisesCourse.TrainingTypeName = type.Name;
                }

                if (premisesCourse.CandidateProviderPremises is not null)
                {
                    var ownershipValue = ownershipTypeSource.FirstOrDefault(x => x.IdKeyValue == premisesCourse.CandidateProviderPremises.IdOwnership);
                    if (ownershipValue is not null)
                    {
                        premisesCourse.CandidateProviderPremises.OwnershipValue = ownershipValue.Name;
                    }

                    var counterForIdGrid = 1;
                    foreach (var room in premisesCourse.CandidateProviderPremises.CandidateProviderPremisesRooms)
                    {
                        room.IdForGrid = counterForIdGrid++;

                        var roomTypeValue = roomTypesSource.FirstOrDefault(x => x.IdKeyValue == room.IdPremisesType);
                        if (roomTypeValue is not null)
                        {
                            room.PremisesTypeName = roomTypeValue.Name;
                        }
                    }
                }
            }

            return dataAsVM.OrderBy(x => x.IdТraininType).ThenBy(x => x.CandidateProviderPremises.PremisesName).ToList();
        }

        public async Task<IEnumerable<CandidateProviderPremisesVM>> GetAllActiveCandidateProviderPremisesByIdTrainingTypeByIdSpecialityAndByIdCandidateProviderAsync(int idCandidateProvider, int idSpeciality, int idTrainingType, int idTrainingAndTheoryTypeId)
        {
            var mtbStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseStatus");
            var activeKV = mtbStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Active").IdKeyValue;
            var candidateProviderPremises = this.repository.AllReadonly<CandidateProviderPremises>(x => x.IdCandidate_Provider == idCandidateProvider && x.IdStatus == activeKV)
                .Include(x => x.CandidateProviderPremisesSpecialities)
                    .AsNoTracking();

            var dataList = new List<CandidateProviderPremisesVM>();
            if (idTrainingType != idTrainingAndTheoryTypeId)
            {
                var candidateProviderPremisesForBothTheoryAndPractice = candidateProviderPremises.Where(x => x.CandidateProviderPremisesSpecialities.Any(x => x.IdUsage == idTrainingAndTheoryTypeId && x.IdSpeciality == idSpeciality));
                var dataAsVM = await candidateProviderPremisesForBothTheoryAndPractice.To<CandidateProviderPremisesVM>(x => x.Location).ToListAsync();
                dataList.AddRange(dataAsVM);
            }

            candidateProviderPremises = candidateProviderPremises.Where(x => x.CandidateProviderPremisesSpecialities.Any(x => x.IdUsage == idTrainingType && x.IdSpeciality == idSpeciality));
            var dataAsVM2 = await candidateProviderPremises.To<CandidateProviderPremisesVM>(x => x.Location).ToListAsync();
            dataList.AddRange(dataAsVM2);
            dataList = dataList.OrderBy(x => x.PremisesName).ToList();

            return dataList;
        }

        public async Task<ResultContext<List<CandidateProviderPremisesVM>>> CreateTrainingCoursePremisesByListCandidateProviderPremisesVMAsync(ResultContext<List<CandidateProviderPremisesVM>> resultContext, int idCourse, int idTrainingType)
        {
            var list = resultContext.ResultContextObject;
            try
            {
                foreach (var premises in list)
                {
                    PremisesCourse premisesCourse = new PremisesCourse()
                    {
                        IdPremises = premises.IdCandidateProviderPremises,
                        IdCourse = idCourse,
                        IdТraininType = idTrainingType
                    };

                    await this.repository.AddAsync<PremisesCourse>(premisesCourse);
                }

                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<PremisesCourseVM>> DeletePremisesCourseAsync(ResultContext<PremisesCourseVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<PremisesCourse>(model.IdPremisesCourse);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<PremisesCourse>(entryFromDb.IdPremisesCourse);
                    await this.repository.SaveChangesAsync();

                    resultContext.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        #endregion Training premises course

        #region Training trainer course

        public async Task<IEnumerable<TrainerCourseVM>> GetAllTrainerCoursesByIdCourseAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<TrainerCourse>(x => x.IdCourse == idCourse);
            var dataAsVM = await data.To<TrainerCourseVM>(x => x.CandidateProviderTrainer.CandidateProviderTrainerDocuments).OrderBy(x => x.IdТraininType).ThenBy(x => x.CandidateProviderTrainer.FirstName).ThenBy(x => x.CandidateProviderTrainer.SecondName).ThenBy(x => x.CandidateProviderTrainer.FamilyName).ToListAsync();

            var trainingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
            var documentTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerDocumentType", false, true);
            var educationTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Education");
            foreach (var trainerCourse in dataAsVM)
            {
                var type = trainingTypeSource.FirstOrDefault(x => x.IdKeyValue == trainerCourse.IdТraininType);
                if (type is not null)
                {
                    trainerCourse.TrainingTypeName = type.Name;
                }

                var educationType = educationTypesSource.FirstOrDefault(x => x.IdKeyValue == trainerCourse.CandidateProviderTrainer.IdEducation);
                if (educationType is not null)
                {
                    trainerCourse.CandidateProviderTrainer.EducationValue = educationType.Name;
                }

                trainerCourse.TrainerDocuments = this.GetTrainerUploadedDocumentsVM(trainerCourse.CandidateProviderTrainer.CandidateProviderTrainerDocuments, documentTypesSource);
            }

            return dataAsVM;
        }

        public async Task<IEnumerable<TrainerCourseVM>> GetAllTrainerCoursesWithoutIncludesByIdCourseAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<TrainerCourse>(x => x.IdCourse == idCourse);
            var dataAsVM = await data.To<TrainerCourseVM>(x => x.CandidateProviderTrainer).ToListAsync();

            return dataAsVM.OrderBy(x => x.CandidateProviderTrainer.FirstName).ThenBy(x => x.CandidateProviderTrainer.FamilyName).ToList();
        }

        public async Task<ResultContext<TrainerCourseVM>> DeleteTrainerCourseAsync(ResultContext<TrainerCourseVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<TrainerCourse>(model.IdTrainerCourse);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<TrainerCourse>(entryFromDb.IdTrainerCourse);
                    await this.repository.SaveChangesAsync();

                    resultContext.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<List<CandidateProviderTrainerVM>>> CreateTrainingCourseTrainerByListCandidateProviderTrainerVMAsync(ResultContext<List<CandidateProviderTrainerVM>> resultContext, int idCourse, int idTrainingType)
        {
            var list = resultContext.ResultContextObject;
            try
            {
                foreach (var trainer in list)
                {
                    TrainerCourse premisesCourse = new TrainerCourse()
                    {
                        IdTrainer = trainer.IdCandidateProviderTrainer,
                        IdCourse = idCourse,
                        IdТraininType = idTrainingType
                    };

                    await this.repository.AddAsync<TrainerCourse>(premisesCourse);
                }

                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<IEnumerable<CandidateProviderTrainerVM>> GetAllActiveCandidateProviderTrainersByIdTrainingTypeByIdSpecialityAndByIdCandidateProviderAsync(int idCandidateProvider, int idSpeciality, int idTrainingType, int idTrainingAndTheoryTypeId)
        {
            var trainerStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderTrainerStatus");
            var activeKV = trainerStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Active")!.IdKeyValue;
            var candidateProviderTrainers = this.repository.AllReadonly<CandidateProviderTrainer>(x => x.IdCandidate_Provider == idCandidateProvider && x.IdStatus == activeKV)
                .Include(x => x.CandidateProviderTrainerSpecialities)
                    .AsNoTracking();

            var dataList = new List<CandidateProviderTrainerVM>();
            if (idTrainingAndTheoryTypeId != idTrainingType)
            {
                var trainersForBoth = candidateProviderTrainers.Where(x => x.CandidateProviderTrainerSpecialities.Any(x => x.IdUsage == idTrainingAndTheoryTypeId && x.IdSpeciality == idSpeciality));
                var dataAsVM = await trainersForBoth.To<CandidateProviderTrainerVM>().ToListAsync();
                dataList.AddRange(dataAsVM);
            }

            candidateProviderTrainers = candidateProviderTrainers.Where(x => x.CandidateProviderTrainerSpecialities.Any(x => x.IdUsage == idTrainingType && x.IdSpeciality == idSpeciality));
            var dataAsVM2 = await candidateProviderTrainers.To<CandidateProviderTrainerVM>().ToListAsync();
            dataList.AddRange(dataAsVM2);
            dataList = dataList.OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ToList();

            var educationTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Education");
            foreach (var trainer in dataList)
            {
                var educationType = educationTypesSource.FirstOrDefault(x => x.IdKeyValue == trainer.IdEducation);
                if (educationType is not null)
                {
                    trainer.EducationValue = educationType.Name;
                }
            }

            return dataList;
        }

        #endregion Training trainer course

        #region Training client course

        public async Task<IEnumerable<ClientCourseVM>> GetCourseClientsByIdCourseAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == idCourse);

            return await data.To<ClientCourseVM>(x => x.ClientRequiredDocuments).OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ToListAsync();
        }

        public async Task<IEnumerable<ClientCourseVM>> GetCourseClientsByIdCourseAndByIdCourseFinishedTypeAsync(int idCourse, int idCourseFinishedType)
        {
            var data = this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == idCourse && x.IdFinishedType.HasValue && x.IdFinishedType == idCourseFinishedType);

            return await data.To<ClientCourseVM>().OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ToListAsync();
        }

        public async Task<IEnumerable<ClientCourseVM>> GetCourseClientsWithProtocolsAndDocsForDownloadByIdCourseAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == idCourse);
            var dataAsVM = await data.To<ClientCourseVM>(x => x.ClientCourseDocuments.Select(y => y.CourseDocumentUploadedFiles), x => x.ClientRequiredDocuments, x => x.CourseProtocolGrades.Select(y => y.CourseProtocol)).OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ToListAsync();
            var documentTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType", false, true);
            var protocolTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType");
            foreach (var client in dataAsVM)
            {
                client.ClientDocuments = this.GetClientUploadedDocumentsVM(client.ClientRequiredDocuments, documentTypesSource);
                client.CourseProtocolsWithGrades = this.GetCourseProtocolsWithGradesVM(client.CourseProtocolGrades, protocolTypesSource);
            }

            return dataAsVM;
        }

        public async Task<IEnumerable<ClientCourseVM>> GetCourseClientsByIdCourseWithoutIncludesAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == idCourse);
            var dataAsVM = await data.To<ClientCourseVM>().OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ToListAsync();

            return dataAsVM;
        }

        public async Task<IEnumerable<ClientCourseVM>> GetCourseClientsByListIdsAsync(List<int> ids)
        {
            var data = this.repository.AllReadonly<ClientCourse>(x => ids.Contains(x.IdClientCourse));
            var dataAsVM = await data.To<ClientCourseVM>(x => x.ClientCourseDocuments.Select(y => y.CourseDocumentUploadedFiles), x => x.ClientRequiredDocuments, x => x.CourseProtocolGrades.Select(y => y.CourseProtocol)).OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ToListAsync();
            var documentTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType", false, true);
            var protocolTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType");
            foreach (var client in dataAsVM)
            {
                client.ClientDocuments = this.GetClientUploadedDocumentsVM(client.ClientRequiredDocuments, documentTypesSource);
                client.CourseProtocolsWithGrades = this.GetCourseProtocolsWithGradesVM(client.CourseProtocolGrades.Where(x => x.Grade != null).ToList(), protocolTypesSource);
            }

            return dataAsVM;
        }

        public async Task<IEnumerable<ClientCourseVM>> GetAllClientCourseFromCoursesByIdCourseTypeAndIdCandidateProviderAsync(int idCandidateProvider, int idCourseType)
        {
            var courses = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider && x.IdTrainingCourseType == idCourseType);
            var dataAsVM = await courses.To<CourseVM>(x => x.ClientCourses, x => x.Program.Speciality.Profession).ToListAsync();
            var clientsList = new List<ClientCourseVM>();
            var finishedTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseFinishedType");
            foreach (var course in dataAsVM)
            {
                foreach (var client in course.ClientCourses)
                {
                    client.Course = course;
                    client.CoursePeriod = $"{course.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)}г. - {course.EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)}г.";
                    if (client.IdFinishedType.HasValue)
                    {
                        var finishedType = finishedTypeSource.FirstOrDefault(x => x.IdKeyValue == client.IdFinishedType);
                        if (finishedType is not null)
                        {
                            client.FinishedTypeName = finishedType.Name;
                        }
                    }

                    clientsList.Add(client);
                }
            }

            return clientsList.OrderByDescending(x => x.Course.EndDate.Value).ToList();
        }

        public async Task<ResultContext<NoResult>> DeleteTrainingClientCourseByIdAsync(int idClientCourse)
        {
            var result = new ResultContext<NoResult>();

            try
            {
                var clientCourseSubjectGradeFromDb = await this.repository.AllReadonly<CourseSubjectGrade>(x => x.IdClientCourse == idClientCourse).FirstOrDefaultAsync();
                if (clientCourseSubjectGradeFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<CourseSubjectGrade>(clientCourseSubjectGradeFromDb.IdCourseSubjectGrade);
                    await this.repository.SaveChangesAsync();
                }

                var entryFromDb = await this.repository.GetByIdAsync<ClientCourse>(idClientCourse);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<ClientCourse>(entryFromDb.IdClientCourse);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return result;
        }

        public async Task<ClientCourseVM> GetTrainingClientCourseByIdAsync(int idClientCourse)
        {
            var data = this.repository.AllReadonly<ClientCourse>(x => x.IdClientCourse == idClientCourse);

            return await data.To<ClientCourseVM>(x => x.Course, x => x.ClientRequiredDocuments).FirstOrDefaultAsync();
        }

        public async Task<int?> GetTrainingClientCourseIdCityOfBirthByIdAsync(int idClientCourse)
        {
            return (await this.repository.GetByIdAsync<ClientCourse>(idClientCourse)).IdCityOfBirth;
        }

        public async Task<ResultContext<ClientCourseVM>> CreateTrainingClientCourseAsync(ResultContext<ClientCourseVM> resultContext, int idCandidateProvider, int idTrainingCourseType)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var kvPartProfessionValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "PartProfession");

                model.IdClient = await this.HandleClientAsync(model, idCandidateProvider);
                model.FirstName = model.FirstName.Trim();
                model.SecondName = !string.IsNullOrEmpty(model.SecondName) ? model.SecondName.Trim() : null;
                model.FamilyName = model.FamilyName.Trim();
                model.Indent = model.Indent.Trim();

                var clientCourseForDb = model.To<ClientCourse>();
                clientCourseForDb.Course = null;

                await this.repository.AddAsync<ClientCourse>(clientCourseForDb);
                await this.repository.SaveChangesAsync();

                if (idTrainingCourseType != kvPartProfessionValue.IdKeyValue)
                {
                    await this.AddClientCourseSubjectGradeEntriesAsync(clientCourseForDb);
                }

                model.IdClientCourse = clientCourseForDb.IdClientCourse;
                model.IdCreateUser = clientCourseForDb.IdCreateUser;
                model.IdModifyUser = clientCourseForDb.IdModifyUser;
                model.CreationDate = clientCourseForDb.CreationDate;
                model.ModifyDate = clientCourseForDb.ModifyDate;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        private async Task AddClientCourseSubjectGradeEntriesAsync(ClientCourse clientCourseForDb)
        {
            var subjects = await this.repository.AllReadonly<CourseSubject>(x => x.IdCourse == clientCourseForDb.IdCourse).ToListAsync();
            foreach (var subject in subjects)
            {
                CourseSubjectGrade clientCourseSubjectGrade = new CourseSubjectGrade()
                {
                    IdClientCourse = clientCourseForDb.IdClientCourse,
                    IdCourseSubject = subject.IdCourseSubject
                };

                await this.repository.AddAsync<CourseSubjectGrade>(clientCourseSubjectGrade);
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task<ResultContext<ClientCourseVM>> UpdateTrainingClientCourseAsync(ResultContext<ClientCourseVM> resultContext, int idCandidateProvider, ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM = null, ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM = null, ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM = null)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var clientCourseFromDb = await this.repository.GetByIdAsync<ClientCourse>(model.IdClientCourse);
                if (clientCourseFromDb is not null)
                {
                    model.IdClient = await this.HandleClientAsync(model, idCandidateProvider);
                    model.FirstName = model.FirstName.Trim();
                    model.SecondName = !string.IsNullOrEmpty(model.SecondName) ? model.SecondName.Trim() : null;
                    model.FamilyName = model.FamilyName.Trim();
                    model.Indent = model.Indent.Trim();
                    model.IdCreateUser = clientCourseFromDb.IdCreateUser;
                    model.CreationDate = clientCourseFromDb.CreationDate;

                    var kvDocumentStatusNotSubmitted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "NotSubmitted");
                    if (clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourse != 0)
                    {
                        //if (model.Course.IdCourse == this.kvCompletedCourse.IdKeyValue)
                        //{
                        var docStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
                        var kvDocPrinted = docStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Printed");
                        model.FinishedDate = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedDate;
                        model.IdFinishedType = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdFinishedType;

                        var docSerialNumberReceivedFromNewEntity = await this.repository.GetByIdAsync<DocumentSerialNumber>(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdDocumentSerialNumber);
                        if (clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourseDocument == 0)
                        {
                            if (docSerialNumberReceivedFromNewEntity is not null)
                            {
                                RequestDocumentManagement requestDocumentManagement = new RequestDocumentManagement()
                                {
                                    IdCandidateProvider = this.UserProps.IdCandidateProvider,
                                    IdTypeOfRequestedDocument = docSerialNumberReceivedFromNewEntity.IdTypeOfRequestedDocument,
                                    DocumentCount = 1,
                                    DocumentDate = DateTime.Now,
                                    IdDocumentOperation = kvDocPrinted.IdKeyValue,
                                    ReceiveDocumentYear = docSerialNumberReceivedFromNewEntity.ReceiveDocumentYear
                                };

                                await this.repository.AddAsync<RequestDocumentManagement>(requestDocumentManagement);
                                await this.repository.SaveChangesAsync();

                                DocumentSerialNumber documentSerialNumber = new DocumentSerialNumber()
                                {
                                    IdRequestDocumentManagement = requestDocumentManagement.IdRequestDocumentManagement,
                                    IdCandidateProvider = this.UserProps.IdCandidateProvider,
                                    IdTypeOfRequestedDocument = docSerialNumberReceivedFromNewEntity.IdTypeOfRequestedDocument,
                                    DocumentDate = DateTime.Now,
                                    SerialNumber = docSerialNumberReceivedFromNewEntity.SerialNumber,
                                    IdDocumentOperation = kvDocPrinted.IdKeyValue,
                                    ReceiveDocumentYear = docSerialNumberReceivedFromNewEntity.ReceiveDocumentYear,
                                };

                                await this.repository.AddAsync<DocumentSerialNumber>(documentSerialNumber);
                                await this.repository.SaveChangesAsync();

                                var typeOfReqDoc = await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.IdCourseType == model.Course.IdTrainingCourseType).FirstOrDefaultAsync();
                                var docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == typeOfReqDoc.IdTypeOfRequestedDocument && x.Year == clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedYear).FirstOrDefaultAsync();
                                ClientCourseDocument clientCourseDocument = new ClientCourseDocument()
                                {
                                    IdClientCourse = model.IdClientCourse,
                                    IdDocumentType = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdDocumentType,
                                    FinishedYear = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedYear,
                                    DocumentRegNo = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentRegNo,
                                    IdDocumentSerialNumber = documentSerialNumber.IdDocumentSerialNumber,
                                    DocumentDate = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentDate,
                                    DocumentPrnNo = $"{docSeries?.SeriesName}/{documentSerialNumber.SerialNumber}",
                                    DocumentProtocol = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentProtocol,
                                    TheoryResult = !string.IsNullOrEmpty(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.TheoryResult) ? decimal.Parse(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.TheoryResult) : null,
                                    PracticeResult = !string.IsNullOrEmpty(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.PracticeResult) ? decimal.Parse(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.PracticeResult) : null,
                                    FinalResult = !string.IsNullOrEmpty(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinalResult) ? decimal.Parse(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinalResult) : null,
                                    IdCourseProtocol = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdCourseProtocol,
                                    IdTypeOfRequestedDocument = typeOfReqDoc!.IdTypeOfRequestedDocument,
                                    IdDocumentStatus = kvDocumentStatusNotSubmitted.IdKeyValue,
                                    DocumentSerNo = docSeries?.SeriesName
                                };

                                await this.repository.AddAsync<ClientCourseDocument>(clientCourseDocument);
                                await this.repository.SaveChangesAsync();

                                clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourseDocument = clientCourseDocument.IdClientCourseDocument;

                                await this.AddClientCourseDocumentStatusAsync(clientCourseDocument.IdClientCourseDocument, kvDocumentStatusNotSubmitted.IdKeyValue);
                            }
                            else if (docSerialNumberReceivedFromNewEntity is null && clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdDocumentType == (await dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "PartProfession")).IdKeyValue)
                            {
                                var typeOfReqDoc = await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.IdCourseType == model.Course.IdTrainingCourseType).FirstOrDefaultAsync();
                                ClientCourseDocument clientCourseDocument = new ClientCourseDocument()
                                {
                                    IdClientCourse = model.IdClientCourse,
                                    IdDocumentType = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdDocumentType,
                                    FinishedYear = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedYear,
                                    DocumentRegNo = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentRegNo,
                                    IdDocumentSerialNumber = null,
                                    DocumentDate = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentDate,
                                    DocumentPrnNo = null,
                                    DocumentProtocol = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentProtocol,
                                    TheoryResult = !string.IsNullOrEmpty(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.TheoryResult) ? decimal.Parse(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.TheoryResult) : null,
                                    PracticeResult = !string.IsNullOrEmpty(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.PracticeResult) ? decimal.Parse(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.PracticeResult) : null,
                                    FinalResult = !string.IsNullOrEmpty(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinalResult) ? decimal.Parse(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinalResult) : null,
                                    IdCourseProtocol = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdCourseProtocol,
                                    IdTypeOfRequestedDocument = typeOfReqDoc!.IdTypeOfRequestedDocument,
                                    IdDocumentStatus = kvDocumentStatusNotSubmitted.IdKeyValue,
                                };

                                await this.repository.AddAsync<ClientCourseDocument>(clientCourseDocument);
                                await this.repository.SaveChangesAsync();

                                await this.AddClientCourseDocumentStatusAsync(clientCourseDocument.IdClientCourseDocument, kvDocumentStatusNotSubmitted.IdKeyValue);
                            }
                        }
                        else
                        {
                            var clientCourseDocumentFromDb = await this.repository.GetByIdAsync<ClientCourseDocument>(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourseDocument);
                            if (clientCourseDocumentFromDb is not null)
                            {
                                if (clientCourseDocumentFromDb.IdDocumentSerialNumber != clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdDocumentSerialNumber)
                                {
                                    var docSerialNumberReceivedFromSavedEntity = await this.repository.GetByIdAsync<DocumentSerialNumber>(clientCourseDocumentFromDb.IdDocumentSerialNumber);
                                    var docSerialNumberSubmitted = await this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdCandidateProvider == this.UserProps.IdCandidateProvider && x.IdTypeOfRequestedDocument == docSerialNumberReceivedFromSavedEntity.IdTypeOfRequestedDocument && x.SerialNumber == docSerialNumberReceivedFromSavedEntity.SerialNumber && x.IdDocumentOperation == kvDocPrinted.IdKeyValue).FirstOrDefaultAsync();
                                    if (docSerialNumberSubmitted is not null)
                                    {
                                        docSerialNumberSubmitted.SerialNumber = docSerialNumberReceivedFromNewEntity.SerialNumber;

                                        this.repository.Update<DocumentSerialNumber>(docSerialNumberSubmitted);
                                        await this.repository.SaveChangesAsync();
                                    }
                                }

                                if (clientCourseDocumentFromDb.DocumentSerialNumber is not null)
                                {
                                    var docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == clientCourseDocumentFromDb.DocumentSerialNumber.IdTypeOfRequestedDocument && x.Year == clientCourseDocumentFromDb.DocumentSerialNumber.ReceiveDocumentYear).FirstOrDefaultAsync();
                                    clientCourseDocumentFromDb.DocumentPrnNo = $"{docSeries?.SeriesName}/{clientCourseDocumentFromDb.DocumentSerialNumber.SerialNumber}";
                                    clientCourseDocumentFromDb.DocumentSerNo = docSeries?.SeriesName;
                                }

                                clientCourseDocumentFromDb.FinishedYear = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedYear;
                                clientCourseDocumentFromDb.DocumentRegNo = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentRegNo;
                                clientCourseDocumentFromDb.IdDocumentSerialNumber = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdDocumentSerialNumber;
                                clientCourseDocumentFromDb.DocumentDate = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentDate;
                                clientCourseDocumentFromDb.DocumentProtocol = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentProtocol;
                                clientCourseDocumentFromDb.TheoryResult = !string.IsNullOrEmpty(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.TheoryResult) ? decimal.Parse(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.TheoryResult) : null;
                                clientCourseDocumentFromDb.PracticeResult = !string.IsNullOrEmpty(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.PracticeResult) ? decimal.Parse(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.PracticeResult) : null;
                                clientCourseDocumentFromDb.FinalResult = !string.IsNullOrEmpty(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinalResult) ? decimal.Parse(clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinalResult) : null;
                                clientCourseDocumentFromDb.IdCourseProtocol = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdCourseProtocol;

                                this.repository.Update<ClientCourseDocument>(clientCourseDocumentFromDb);
                                await this.repository.SaveChangesAsync();
                            }
                        }

                        if (clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdCourseDocumentUploadedFile == 0)
                        {
                            if (clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourseDocument != 0)
                            {
                                CourseDocumentUploadedFile courseDocumentUploadedFile = new CourseDocumentUploadedFile()
                                {
                                    IdClientCourseDocument = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourseDocument,
                                    UploadedFileName = string.Empty
                                };

                                await this.repository.AddAsync<CourseDocumentUploadedFile>(courseDocumentUploadedFile);
                                await this.repository.SaveChangesAsync();

                                clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdCourseDocumentUploadedFile = courseDocumentUploadedFile.IdCourseDocumentUploadedFile;
                            }
                        }
                        //}
                    }

                    if (duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM is not null && duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourse != 0)
                    {
                        //if (model.Course.IdCourse == this.kvCompletedCourse.IdKeyValue)
                        //{
                        var docStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
                        var kvDocPrinted = docStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Printed");
                        model.FinishedDate = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedDate;
                        model.IdFinishedType = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdFinishedType;

                        var docSerialNumberReceivedFromNewEntity = await this.repository.GetByIdAsync<DocumentSerialNumber>(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdDocumentSerialNumber);
                        if (duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourseDocument == 0)
                        {
                            if (docSerialNumberReceivedFromNewEntity is not null)
                            {
                                RequestDocumentManagement requestDocumentManagement = new RequestDocumentManagement()
                                {
                                    IdCandidateProvider = this.UserProps.IdCandidateProvider,
                                    IdTypeOfRequestedDocument = docSerialNumberReceivedFromNewEntity.IdTypeOfRequestedDocument,
                                    DocumentCount = 1,
                                    DocumentDate = DateTime.Now,
                                    IdDocumentOperation = kvDocPrinted.IdKeyValue,
                                    ReceiveDocumentYear = docSerialNumberReceivedFromNewEntity.ReceiveDocumentYear
                                };

                                await this.repository.AddAsync<RequestDocumentManagement>(requestDocumentManagement);
                                await this.repository.SaveChangesAsync();

                                DocumentSerialNumber documentSerialNumber = new DocumentSerialNumber()
                                {
                                    IdRequestDocumentManagement = requestDocumentManagement.IdRequestDocumentManagement,
                                    IdCandidateProvider = this.UserProps.IdCandidateProvider,
                                    IdTypeOfRequestedDocument = docSerialNumberReceivedFromNewEntity.IdTypeOfRequestedDocument,
                                    DocumentDate = DateTime.Now,
                                    SerialNumber = docSerialNumberReceivedFromNewEntity.SerialNumber,
                                    IdDocumentOperation = kvDocPrinted.IdKeyValue,
                                    ReceiveDocumentYear = docSerialNumberReceivedFromNewEntity.ReceiveDocumentYear
                                };

                                await this.repository.AddAsync<DocumentSerialNumber>(documentSerialNumber);
                                await this.repository.SaveChangesAsync();

                                var typeOfReqDoc = await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == "3-54a" && x.IsValid).FirstOrDefaultAsync();
                                var docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == typeOfReqDoc.IdTypeOfRequestedDocument && x.Year == clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedYear).FirstOrDefaultAsync();
                                ClientCourseDocument clientCourseDocument = new ClientCourseDocument()
                                {
                                    IdClientCourse = model.IdClientCourse,
                                    IdDocumentType = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdDocumentType,
                                    FinishedYear = clientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedYear,
                                    DocumentRegNo = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentRegNo,
                                    IdDocumentSerialNumber = documentSerialNumber.IdDocumentSerialNumber,
                                    DocumentDate = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentDate,
                                    DocumentPrnNo = $"{docSeries?.SeriesName}/{documentSerialNumber.SerialNumber}",
                                    DocumentProtocol = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentProtocol,
                                    TheoryResult = !string.IsNullOrEmpty(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.TheoryResult) ? decimal.Parse(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.TheoryResult) : null,
                                    PracticeResult = !string.IsNullOrEmpty(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.PracticeResult) ? decimal.Parse(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.PracticeResult) : null,
                                    FinalResult = !string.IsNullOrEmpty(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinalResult) ? decimal.Parse(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinalResult) : null,
                                    IdCourseProtocol = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdCourseProtocol,
                                    IdTypeOfRequestedDocument = typeOfReqDoc!.IdTypeOfRequestedDocument,
                                    IdDocumentStatus = kvDocumentStatusNotSubmitted.IdKeyValue,
                                    DocumentSerNo = docSeries?.SeriesName
                                };

                                await this.repository.AddAsync<ClientCourseDocument>(clientCourseDocument);
                                await this.repository.SaveChangesAsync();

                                duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourseDocument = clientCourseDocument.IdClientCourseDocument;

                                await this.AddClientCourseDocumentStatusAsync(clientCourseDocument.IdClientCourseDocument, kvDocumentStatusNotSubmitted.IdKeyValue);
                            }
                            else
                            {
                                var typeOfReqDoc = await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == "3-37" && x.IsValid).FirstOrDefaultAsync();
                                ClientCourseDocument clientCourseDocument = new ClientCourseDocument()
                                {
                                    IdClientCourse = model.IdClientCourse,
                                    IdDocumentType = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdDocumentType,
                                    FinishedYear = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedYear,
                                    DocumentRegNo = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentRegNo,
                                    DocumentDate = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentDate,
                                    DocumentProtocol = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentProtocol,
                                    TheoryResult = !string.IsNullOrEmpty(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.TheoryResult) ? decimal.Parse(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.TheoryResult) : null,
                                    PracticeResult = !string.IsNullOrEmpty(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.PracticeResult) ? decimal.Parse(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.PracticeResult) : null,
                                    FinalResult = !string.IsNullOrEmpty(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinalResult) ? decimal.Parse(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinalResult) : null,
                                    IdCourseProtocol = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdCourseProtocol,
                                    IdTypeOfRequestedDocument = typeOfReqDoc!.IdTypeOfRequestedDocument,
                                    IdDocumentStatus = kvDocumentStatusNotSubmitted.IdKeyValue
                                };

                                await this.repository.AddAsync<ClientCourseDocument>(clientCourseDocument);
                                await this.repository.SaveChangesAsync();

                                duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourseDocument = clientCourseDocument.IdClientCourseDocument;

                                await this.AddClientCourseDocumentStatusAsync(clientCourseDocument.IdClientCourseDocument, kvDocumentStatusNotSubmitted.IdKeyValue);
                            }
                        }
                        else
                        {
                            var clientCourseDocumentFromDb = await this.repository.GetByIdAsync<ClientCourseDocument>(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourseDocument);
                            if (clientCourseDocumentFromDb is not null)
                            {
                                if (clientCourseDocumentFromDb.IdDocumentSerialNumber != duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdDocumentSerialNumber)
                                {
                                    var docSerialNumberReceivedFromSavedEntity = await this.repository.GetByIdAsync<DocumentSerialNumber>(clientCourseDocumentFromDb.IdDocumentSerialNumber);
                                    var docSerialNumberSubmitted = await this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdCandidateProvider == this.UserProps.IdCandidateProvider && x.IdTypeOfRequestedDocument == docSerialNumberReceivedFromSavedEntity.IdTypeOfRequestedDocument && x.SerialNumber == docSerialNumberReceivedFromSavedEntity.SerialNumber && x.IdDocumentOperation == kvDocPrinted.IdKeyValue).FirstOrDefaultAsync();
                                    if (docSerialNumberSubmitted is not null)
                                    {
                                        docSerialNumberSubmitted.SerialNumber = docSerialNumberReceivedFromNewEntity.SerialNumber;

                                        this.repository.Update<DocumentSerialNumber>(docSerialNumberSubmitted);
                                        await this.repository.SaveChangesAsync();
                                    }
                                }

                                if (clientCourseDocumentFromDb.DocumentSerialNumber is not null)
                                {
                                    var docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == clientCourseDocumentFromDb.DocumentSerialNumber.IdTypeOfRequestedDocument && x.Year == clientCourseDocumentFromDb.DocumentSerialNumber.ReceiveDocumentYear).FirstOrDefaultAsync();
                                    clientCourseDocumentFromDb.DocumentPrnNo = $"{docSeries?.SeriesName}/{clientCourseDocumentFromDb.DocumentSerialNumber.SerialNumber}";
                                    clientCourseDocumentFromDb.DocumentSerNo = docSeries?.SeriesName;
                                }

                                clientCourseDocumentFromDb.FinishedYear = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedYear;
                                clientCourseDocumentFromDb.DocumentRegNo = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentRegNo;
                                clientCourseDocumentFromDb.IdDocumentSerialNumber = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdDocumentSerialNumber;
                                clientCourseDocumentFromDb.DocumentDate = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentDate;
                                clientCourseDocumentFromDb.DocumentProtocol = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.DocumentProtocol;
                                clientCourseDocumentFromDb.TheoryResult = !string.IsNullOrEmpty(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.TheoryResult) ? decimal.Parse(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.TheoryResult) : null;
                                clientCourseDocumentFromDb.PracticeResult = !string.IsNullOrEmpty(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.PracticeResult) ? decimal.Parse(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.PracticeResult) : null;
                                clientCourseDocumentFromDb.FinalResult = !string.IsNullOrEmpty(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinalResult) ? decimal.Parse(duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinalResult) : null;
                                clientCourseDocumentFromDb.IdCourseProtocol = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdCourseProtocol;

                                this.repository.Update<ClientCourseDocument>(clientCourseDocumentFromDb);
                                await this.repository.SaveChangesAsync();
                            }
                        }

                        if (duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdCourseDocumentUploadedFile == 0)
                        {
                            if (duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourseDocument != 0)
                            {
                                CourseDocumentUploadedFile courseDocumentUploadedFile = new CourseDocumentUploadedFile()
                                {
                                    IdClientCourseDocument = duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourseDocument,
                                    UploadedFileName = string.Empty
                                };

                                await this.repository.AddAsync<CourseDocumentUploadedFile>(courseDocumentUploadedFile);
                                await this.repository.SaveChangesAsync();

                                duplicateClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdCourseDocumentUploadedFile = courseDocumentUploadedFile.IdCourseDocumentUploadedFile;
                            }
                        }
                        //}
                    }

                    if (legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM is not null && legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourse != 0)
                    {
                        var clientCourseDocumentFromDb = await this.repository.GetByIdAsync<ClientCourseDocument>(legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityClientCourseDocument);
                        if (clientCourseDocumentFromDb is not null)
                        {
                            clientCourseDocumentFromDb.FinishedYear = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedYear;
                            clientCourseDocumentFromDb.DocumentRegNo = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.LegalCapacityDocumentRegNo;
                            clientCourseDocumentFromDb.DocumentDate = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.LegalCapacityDocumentDate;
                            clientCourseDocumentFromDb.IdTypeOfRequestedDocument = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityTypeOfRequestedDocument;

                            this.repository.Update<ClientCourseDocument>(clientCourseDocumentFromDb);
                            await this.repository.SaveChangesAsync();
                        }
                        else
                        {
                            ClientCourseDocument clientCourseDocument = new ClientCourseDocument()
                            {
                                IdClientCourse = model.IdClientCourse,
                                IdDocumentType = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityDocumentType,
                                FinishedYear = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedYear,
                                DocumentRegNo = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.LegalCapacityDocumentRegNo,
                                DocumentDate = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.LegalCapacityDocumentDate,
                                IdTypeOfRequestedDocument = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityTypeOfRequestedDocument
                            };

                            await this.repository.AddAsync<ClientCourseDocument>(clientCourseDocument);
                            await this.repository.SaveChangesAsync();

                            legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityClientCourseDocument = clientCourseDocument.IdClientCourseDocument;
                        }

                        if (legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityCourseDocumentUploadedFile == 0)
                        {
                            if (legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityClientCourseDocument != 0)
                            {
                                CourseDocumentUploadedFile courseDocumentUploadedFile = new CourseDocumentUploadedFile()
                                {
                                    IdClientCourseDocument = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityClientCourseDocument,
                                    UploadedFileName = string.Empty
                                };

                                await this.repository.AddAsync<CourseDocumentUploadedFile>(courseDocumentUploadedFile);
                                await this.repository.SaveChangesAsync();

                                legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityCourseDocumentUploadedFile = courseDocumentUploadedFile.IdCourseDocumentUploadedFile;
                            }
                        }
                    }

                    if (this.IsClientCourseModified(model, clientCourseFromDb))
                    {
                        clientCourseFromDb = model.To<ClientCourse>();
                        clientCourseFromDb.Course = null;
                        clientCourseFromDb.Client = null;
                        clientCourseFromDb.Speciality = null;
                        clientCourseFromDb.ProfessionalDirection = null;
                        clientCourseFromDb.ClientCourseDocuments = null;
                        clientCourseFromDb.CourseSubjectGrades = null;
                        clientCourseFromDb.CourseProtocolGrades = null;
                        clientCourseFromDb.ClientCourseStatuses = null;
                        clientCourseFromDb.ClientRequiredDocuments = null;
                        clientCourseFromDb.ProfessionalDirection = null;

                        this.repository.Update<ClientCourse>(clientCourseFromDb);
                        await this.repository.SaveChangesAsync();

                        if (clientCourseFromDb.IdFinishedType.HasValue)
                        {
                            await this.HandleClientCourseStatusAsync(clientCourseFromDb);
                        }
                    }

                    model.IdModifyUser = clientCourseFromDb.IdModifyUser;
                    model.ModifyDate = clientCourseFromDb.ModifyDate;

                    resultContext.AddMessage("Записът е успешен!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<ClientCourseVM>> UpdateLegalCapacityTrainingClientCourseAsync(ResultContext<ClientCourseVM> resultContext, int idCandidateProvider, ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM = null)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var clientCourseFromDb = await this.repository.GetByIdAsync<ClientCourse>(model.IdClientCourse);
                if (clientCourseFromDb is not null)
                {
                    model.IdClient = await this.HandleClientAsync(model, idCandidateProvider);
                    model.FirstName = model.FirstName.Trim();
                    model.SecondName = !string.IsNullOrEmpty(model.SecondName) ? model.SecondName.Trim() : null;
                    model.FamilyName = model.FamilyName.Trim();
                    model.Indent = model.Indent.Trim();
                    model.IdCreateUser = clientCourseFromDb.IdCreateUser;
                    model.CreationDate = clientCourseFromDb.CreationDate;

                    if (legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM is not null && legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdClientCourse != 0)
                    {
                        model.IdFinishedType = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdFinishedType;

                        var clientCourseDocumentFromDb = await this.repository.GetByIdAsync<ClientCourseDocument>(legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityClientCourseDocument);
                        if (clientCourseDocumentFromDb is not null)
                        {
                            clientCourseDocumentFromDb.FinishedYear = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedYear;
                            clientCourseDocumentFromDb.DocumentRegNo = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.LegalCapacityDocumentRegNo;
                            clientCourseDocumentFromDb.DocumentDate = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.LegalCapacityDocumentDate;
                            clientCourseDocumentFromDb.IdTypeOfRequestedDocument = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityTypeOfRequestedDocument;

                            this.repository.Update<ClientCourseDocument>(clientCourseDocumentFromDb);
                            await this.repository.SaveChangesAsync();
                        }
                        else
                        {
                            ClientCourseDocument clientCourseDocument = new ClientCourseDocument()
                            {
                                IdClientCourse = model.IdClientCourse,
                                IdDocumentType = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityDocumentType,
                                FinishedYear = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.FinishedYear,
                                DocumentRegNo = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.LegalCapacityDocumentRegNo,
                                DocumentDate = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.LegalCapacityDocumentDate,
                                IdTypeOfRequestedDocument = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityTypeOfRequestedDocument
                            };

                            await this.repository.AddAsync<ClientCourseDocument>(clientCourseDocument);
                            await this.repository.SaveChangesAsync();

                            legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityClientCourseDocument = clientCourseDocument.IdClientCourseDocument;
                        }

                        if (legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityCourseDocumentUploadedFile == 0)
                        {
                            if (legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityClientCourseDocument != 0)
                            {
                                CourseDocumentUploadedFile courseDocumentUploadedFile = new CourseDocumentUploadedFile()
                                {
                                    IdClientCourseDocument = legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityClientCourseDocument,
                                    UploadedFileName = string.Empty
                                };

                                await this.repository.AddAsync<CourseDocumentUploadedFile>(courseDocumentUploadedFile);
                                await this.repository.SaveChangesAsync();

                                legalCapacityOrdinanceClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM.IdLegalCapacityCourseDocumentUploadedFile = courseDocumentUploadedFile.IdCourseDocumentUploadedFile;
                            }
                        }
                    }

                    if (this.IsClientCourseModified(model, clientCourseFromDb))
                    {
                        clientCourseFromDb = model.To<ClientCourse>();
                        clientCourseFromDb.Course = null;
                        clientCourseFromDb.Client = null;
                        clientCourseFromDb.Speciality = null;
                        clientCourseFromDb.ProfessionalDirection = null;
                        clientCourseFromDb.ClientCourseDocuments = null;

                        this.repository.Update<ClientCourse>(clientCourseFromDb);
                        await this.repository.SaveChangesAsync();

                        if (clientCourseFromDb.IdFinishedType.HasValue)
                        {
                            await this.HandleClientCourseStatusAsync(clientCourseFromDb);
                        }
                    }

                    model.IdModifyUser = clientCourseFromDb.IdModifyUser;
                    model.ModifyDate = clientCourseFromDb.ModifyDate;

                    resultContext.AddMessage("Записът е успешен!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        private async Task HandleClientCourseStatusAsync(ClientCourse clientCourse)
        {
            var entryFromDb = await this.repository.AllReadonly<ClientCourseStatus>(x => x.IdClientCourse == clientCourse.IdClientCourse).OrderBy(x => x.IdClientCourseStatus).LastOrDefaultAsync();
            if ((entryFromDb is not null && entryFromDb.IdFinishedType != clientCourse.IdFinishedType) || entryFromDb is null)
            {
                ClientCourseStatus clientCourseStatus = new ClientCourseStatus()
                {
                    IdClientCourse = clientCourse.IdClientCourse,
                    IdFinishedType = clientCourse.IdFinishedType.Value
                };

                await this.repository.AddAsync<ClientCourseStatus>(clientCourseStatus);
                await this.repository.SaveChangesAsync();
            }
        }

        public async Task<ResultContext<ClientVM>> GetClientByIdIndentTypeByIndentAndByIdCandidateProviderAsync(int idIndentType, string indent, int idCandidateProvider)
        {
            var result = new ResultContext<ClientVM>();
            try
            {
                var data = this.repository.AllReadonly<Client>(x => x.IdIndentType == idIndentType && x.Indent == indent.Trim() && x.IdCandidateProvider == idCandidateProvider);
                if (data.Any())
                {
                    var clientVM = await data.To<ClientVM>().FirstOrDefaultAsync();
                    result.ResultContextObject = clientVM;
                }
                else
                {
                    result.AddErrorMessage("Няма намерен курсист в базата данни!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return result;
        }

        public async Task<ResultContext<ClientCourseVM>> UpdateClientCoursesListFinishedDataAsync(ResultContext<ClientCourseVM> resultContext, List<ClientCourseVM> clientCourses)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                foreach (var clientCourse in clientCourses)
                {
                    var clientCourseFromDb = await this.repository.GetByIdAsync<ClientCourse>(clientCourse.IdClientCourse);
                    if (clientCourseFromDb is not null)
                    {
                        clientCourseFromDb.IdFinishedType = model.IdFinishedType;
                        clientCourseFromDb.FinishedDate = model.FinishedDate;

                        this.repository.Update<ClientCourse>(clientCourseFromDb);
                        await this.repository.SaveChangesAsync();
                    }
                }

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        private async Task<int> HandleClientAsync(ClientCourseVM model, int idCandidateProvider)
        {
            var idClient = 0;
            var clientFromDb = await this.repository.AllReadonly<Client>(x => x.IdIndentType == model.IdIndentType && x.Indent == model.Indent.Trim() && x.IdCandidateProvider == idCandidateProvider).FirstOrDefaultAsync();
            if (clientFromDb is null)
            {
                Client client = new Client()
                {
                    FirstName = model.FirstName.Trim(),
                    SecondName = !string.IsNullOrEmpty(model.SecondName) ? model.SecondName.Trim() : null,
                    FamilyName = model.FamilyName.Trim(),
                    FirstNameEN = BaseHelper.ConvertCyrToLatin(model.FirstName),
                    SecondNameEN = !string.IsNullOrEmpty(model.SecondName) ? BaseHelper.ConvertCyrToLatin(model.SecondName) : null,
                    FamilyNameEN = BaseHelper.ConvertCyrToLatin(model.FamilyName),
                    IdCandidateProvider = idCandidateProvider,
                    IdSex = model.IdSex,
                    IdIndentType = model.IdIndentType,
                    Indent = model.Indent.ToString().Trim(),
                    BirthDate = model.BirthDate,
                    IdNationality = model.IdNationality,
                    IdEducation = model.IdEducation,
                    IdCountryOfBirth = model.IdCountryOfBirth,
                    IdCityOfBirth = model.IdCityOfBirth
                };

                await this.repository.AddAsync<Client>(client);
                await this.repository.SaveChangesAsync();

                idClient = client.IdClient;
            }
            else
            {
                ClientVM clientVM = new ClientVM()
                {
                    IdClient = clientFromDb.IdClient,
                    FirstName = model.FirstName.Trim(),
                    SecondName = !string.IsNullOrEmpty(model.SecondName) ? model.SecondName.Trim() : null,
                    FamilyName = model.FamilyName.Trim(),
                    FirstNameEN = BaseHelper.ConvertCyrToLatin(model.FirstName),
                    SecondNameEN = !string.IsNullOrEmpty(model.SecondName) ? BaseHelper.ConvertCyrToLatin(model.SecondName) : null,
                    FamilyNameEN = BaseHelper.ConvertCyrToLatin(model.FamilyName),
                    IdCandidateProvider = idCandidateProvider,
                    IdSex = model.IdSex,
                    IdIndentType = model.IdIndentType,
                    Indent = model.Indent.ToString().Trim(),
                    BirthDate = model.BirthDate,
                    IdNationality = model.IdNationality,
                    IdEducation = model.IdEducation,
                    IdCountryOfBirth = model.IdCountryOfBirth,
                    IdCityOfBirth = model.IdCityOfBirth
                };

                if (this.IsClientModified(clientVM, clientFromDb))
                {
                    clientVM.IdCreateUser = clientFromDb.IdCreateUser;
                    clientVM.CreationDate = clientFromDb.CreationDate;

                    clientFromDb = clientVM.To<Client>();
                    this.repository.Update<Client>(clientFromDb);
                    await this.repository.SaveChangesAsync();
                }

                idClient = clientFromDb.IdClient;
            }

            return idClient;
        }

        public MemoryStream GetCourseClientsTemplate()
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\CPO\Training";
            var fileFullName = $"{resources_Folder}\\Kursisti-CPO.xlsx";

            MemoryStream ms = new MemoryStream();
            if (File.Exists(fileFullName))
            {
                using (ms = new MemoryStream())
                {
                    using (FileStream file = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);
                        ms.Write(bytes, 0, (int)file.Length);
                    }
                }
            }

            return ms;
        }

        public async Task<ResultContext<List<ClientCourseVM>>> ImportCourseClientsAsync(MemoryStream file, string fileName, CourseVM course, List<ClientCourseVM> addedClients)
        {
            ResultContext<List<ClientCourseVM>> resultContext = new ResultContext<List<ClientCourseVM>>();

            List<ClientCourseVM> courseClients = new List<ClientCourseVM>();

            try
            {
                int counter = GlobalConstants.INVALID_ID_ZERO;

                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\Temp\\ImportCourseClients";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var path = @"" + filePath + "\\" + fileName;

                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file.WriteTo(filestream);
                    filestream.Close();
                    file.Close();
                }

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        IApplication app = excelEngine.Excel;

                        IWorkbook workbook = app.Workbooks.Open(fileStream, ExcelOpenType.Automatic);

                        IWorksheet worksheet = workbook.Worksheets[0];
                        if (string.IsNullOrEmpty(worksheet.Rows[0].Columns[0].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[1].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[2].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[3].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[4].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[5].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[6].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[7].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[8].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[9].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[10].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[11].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[12].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[13].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[14].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[15].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[16].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[17].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[18].Text))
                        {
                            resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на курсисти!");
                            return resultContext;
                        }

                        var firstHeader = worksheet.Rows[0].Columns[0].Text.Trim();
                        var secondHeader = worksheet.Rows[0].Columns[1].Text.Trim();
                        var thirdHeader = worksheet.Rows[0].Columns[2].Text.Trim();
                        var fourthHeader = worksheet.Rows[0].Columns[3].Text.Trim();
                        var fifthHeader = worksheet.Rows[0].Columns[4].Text.Trim();
                        var sixthHeader = worksheet.Rows[0].Columns[5].Text.Trim();
                        var seventhHeader = worksheet.Rows[0].Columns[6].Text.Trim();
                        var eighthHeader = worksheet.Rows[0].Columns[7].Text.Trim();
                        var ninthHeader = worksheet.Rows[0].Columns[8].Text.Trim();
                        var tenthHeader = worksheet.Rows[0].Columns[9].Text.Trim();
                        var eleventhHeader = worksheet.Rows[0].Columns[10].Text.Trim();
                        var twelvthHeader = worksheet.Rows[0].Columns[11].Text.Trim();
                        var thirteenthHeader = worksheet.Rows[0].Columns[12].Text.Trim();
                        var fourteenthHeader = worksheet.Rows[0].Columns[13].Text.Trim();
                        var fifteenthHeader = worksheet.Rows[0].Columns[14].Text.Trim();
                        var sixteenthHeader = worksheet.Rows[0].Columns[15].Text.Trim();
                        var seventeenthHeader = worksheet.Rows[0].Columns[16].Text.Trim();
                        var eighteenthHeader = worksheet.Rows[0].Columns[17].Text.Trim();
                        var nineteenthHeader = worksheet.Rows[0].Columns[18].Text.Trim();
                        bool skipFirstRow = true;

                        //Проверка по 1 клетка за да се провери дали файла за импорт на учебна програма
                        if (firstHeader != "Име *" || secondHeader != "Презиме *" || thirdHeader != "Фамилия *" || fourthHeader != "Вид на идентификатора *" || fifthHeader != "ЕГН/ЛНЧ/ИДН *"
                            || sixthHeader != "Дата на раждане *" || seventhHeader != "Пол *" || eighthHeader != "Гражданство *" || ninthHeader != "Месторождение (държава) *"
                            || tenthHeader != "Месторождение (община) *" || eleventhHeader != "Месторождение (населено място) *" || twelvthHeader != "Адрес" || thirteenthHeader != "Телефон"
                            || fourteenthHeader != "E-mail" || fifteenthHeader != "Дата на включване в курса *" || sixteenthHeader != "Финансиране *"
                            || seventeenthHeader != "Съгласие за използване на информацията за контакт от НАПОО *" || eighteenthHeader != "Лице с увреждания" || nineteenthHeader != "Лице в неравностойно положение")
                        {
                            resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на курсисти!");
                            return resultContext;
                        }

                        var rowCounter = 2;
                        foreach (var row in worksheet.Rows)
                        {
                            //Ако сме пропуснали 5 реда приемаме че документа е приключил и са останали празни редове
                            if (counter == 5)
                            {
                                break;
                            }

                            var firstName = row.Cells[0].Value.Trim();

                            //Пропуска 1 ред който е с хедърите
                            if (skipFirstRow || string.IsNullOrEmpty(firstName))
                            {
                                skipFirstRow = false;
                                counter++;
                                continue;
                            }

                            var locations = await this.locationService.GetAllLocationsAsync();
                            var municipalities = await this.municipalityService.GetAllMunicipalitiesAsync();
                            var kvIdentSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
                            var kvEGN = kvIdentSource.FirstOrDefault(x => x.KeyValueIntCode == "EGN");
                            var kvLNCh = kvIdentSource.FirstOrDefault(x => x.KeyValueIntCode == "LNK");
                            var kvIDN = kvIdentSource.FirstOrDefault(x => x.KeyValueIntCode == "IDN");
                            var identList = new List<string>()
                            {
                                { kvEGN.Name.Trim().ToLower() },
                                { kvLNCh.Name.Trim().ToLower() },
                                { kvIDN.Name.Trim().ToLower() }
                            };

                            var kvSexSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");
                            var kvMale = kvSexSource.FirstOrDefault(x => x.KeyValueIntCode == "Man");
                            var kvFemale = kvSexSource.FirstOrDefault(x => x.KeyValueIntCode == "Woman");
                            var sexList = new List<string>()
                            {
                                { kvMale.DefaultValue1.Trim().ToLower() },
                                { kvFemale.DefaultValue1.Trim().ToLower() }
                            };

                            var kvAssingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType");
                            var kvProjects = kvAssingTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "HumanResourcesDevelopmentProjects");
                            var kvVouchers = kvAssingTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "HumanResourcesDevelopmentVouchers");
                            var kvType3 = kvAssingTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Type3");
                            var kvType4 = kvAssingTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Type4");
                            var kvType5 = kvAssingTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Type5");
                            var kvType10 = kvAssingTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Type10");
                            var assingTypeList = new List<string>()
                            {
                                { kvProjects.Name.Trim().ToLower() },
                                { kvVouchers.Name.Trim().ToLower() },
                                { kvType3.Name.Trim().ToLower() },
                                { kvType4.Name.Trim().ToLower() },
                                { kvType5.Name.Trim().ToLower() },
                                { kvType10.Name.Trim().ToLower() }
                            };

                            var kvNationalitySource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality");

                            var clientCourse = new ClientCourseVM();
                            clientCourse.UploadedFileName = "#";
                            clientCourse.IdCourse = course.IdCourse;
                            clientCourse.IdSpeciality = course.Program.IdSpeciality;
                            clientCourse.IdProfessionalDirection = course.Program.Speciality.Profession.IdProfessionalDirection;

                            clientCourse.FirstName = firstName;
                            if (string.IsNullOrEmpty(firstName))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} не е въведено Име на курсист!");
                            }
                            else
                            {
                                if (!Regex.IsMatch(firstName, @"^\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*$"))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} Име на курсист може да съдържа само символи на кирилица!");
                                }
                            }

                            var secondName = row.Cells[1].Value.Trim();
                            clientCourse.SecondName = secondName;
                            if (string.IsNullOrEmpty(secondName))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} не е въведено Презиме на курсист! Ако курсистът няма презиме, въведете '-' за стойност!");
                            }
                            else
                            {
                                if (!Regex.IsMatch(secondName, @"^\p{IsCyrillic}*\s*-*$"))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} Презиме на курсист може да съдържа само символи на кирилица!");
                                }
                            }

                            var familyName = row.Cells[2].Value.Trim();
                            clientCourse.FamilyName = familyName;
                            if (string.IsNullOrEmpty(familyName))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} не е въведена Фамилия на курсист!");
                            }
                            else
                            {
                                if (!Regex.IsMatch(familyName, @"^\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*$"))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} Фамилия на курсист може да съдържа само символи на кирилица!");
                                }
                            }

                            var identType = row.Cells[3].Value.Trim().ToLower();
                            if (string.IsNullOrEmpty(identType))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведен Вид на идентификатор!");
                            }
                            else
                            {
                                if (!identList.Any(x => x == identType))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма въведенa валидна стойност за Вид на идентификатор!");
                                }
                                else
                                {
                                    var kvIdent = kvIdentSource.FirstOrDefault(x => x.Name.ToLower() == identType.ToLower().Trim());
                                    if (kvIdent != null)
                                    {
                                        clientCourse.IdIndentType = kvIdent.IdKeyValue;
                                    }
                                }
                            }

                            var ident = row.Cells[4].Value.Trim();
                            clientCourse.Indent = ident;
                            if (string.IsNullOrEmpty(ident))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена стойност за ЕГН/ЛНЧ/ИДН!");
                            }
                            else
                            {
                                if (ident.Length != 10)
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} въведената стойност за ЕГН/ЛНЧ/ИДН не може да съдържа по-малко или повече от 10 символа!");
                                }
                                else
                                {
                                    if (identType == kvEGN.Name.Trim().ToLower())
                                    {
                                        var checkEGN = new BasicEGNValidation(ident);
                                        if (!checkEGN.Validate())
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} въведената стойност за ЕГН е невалидна!");
                                        }
                                    }
                                }
                            }

                            var birthDate = row.Cells[5].Value.Trim();
                            if (string.IsNullOrEmpty(birthDate))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена стойност за Дата на раждане!");
                            }
                            else
                            {
                                DateTime date;
                                if (!DateTime.TryParse(birthDate, out date))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за Дата на раждане!");
                                }
                                else
                                {
                                    clientCourse.BirthDate = date;
                                    if (course.StartDate.HasValue)
                                    {
                                        var difference = course.StartDate.Value.Year - clientCourse.BirthDate.Value.Year;
                                        if (difference == 16)
                                        {
                                            var startDate = new DateTime(DateTime.Now.Year, course.StartDate.Value.Month, course.StartDate.Value.Day);
                                            var compareBirthDate = new DateTime(DateTime.Now.Year, clientCourse.BirthDate.Value.Month, clientCourse.BirthDate.Value.Day);
                                            if (startDate < compareBirthDate)
                                            {
                                                resultContext.AddErrorMessage($"На ред {rowCounter} не може да запишете за курса лице, което няма навършени 16 години към датата на започване на курса!");
                                            }
                                        }

                                        if (difference < 16)
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} не може да запишете за курса лице, което няма навършени 16 години към датата на започване на курса!");
                                        }
                                    }
                                }
                            }

                            var sex = row.Cells[6].Value.Trim().ToLower();
                            if (string.IsNullOrEmpty(sex))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена стойност за Пол!");
                            }
                            else
                            {
                                if (!sexList.Any(x => x == sex))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за Пол!");
                                }
                                else
                                {
                                    var kvSex = kvSexSource.FirstOrDefault(x => x.DefaultValue1!.ToLower() == sex.ToLower().Trim());
                                    if (kvSex != null)
                                    {
                                        clientCourse.IdSex = kvSex.IdKeyValue;
                                    }
                                }
                            }

                            var nationality = row.Cells[7].Value.Trim().ToLower();
                            if (string.IsNullOrEmpty(nationality))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена стойност за Гражданство!");
                            }
                            else
                            {
                                if (!kvNationalitySource.Any(x => x.Name.Trim().ToLower() == nationality))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за Гражданство!");
                                }
                                else
                                {
                                    var kvNationality = kvNationalitySource.FirstOrDefault(x => x.Name.ToLower() == nationality);
                                    if (kvNationality != null)
                                    {
                                        clientCourse.IdNationality = kvNationality.IdKeyValue;
                                    }
                                }
                            }

                            var countryOfBirth = row.Cells[8].Value.Trim().ToLower();
                            if (string.IsNullOrEmpty(countryOfBirth))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена стойност за Месторождение (държава)!");
                            }
                            else
                            {
                                if (!kvNationalitySource.Any(x => x.Name.Trim().ToLower() == countryOfBirth))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за Месторождение (държава)!");
                                }
                                else
                                {
                                    var kvNationality = kvNationalitySource.FirstOrDefault(x => x.Name.ToLower() == countryOfBirth);
                                    if (kvNationality != null)
                                    {
                                        clientCourse.IdCountryOfBirth = kvNationality.IdKeyValue;
                                    }

                                    if (countryOfBirth == "българия")
                                    {
                                        var municipality = row.Cells[9].Value.Trim().ToLower();
                                        int idMunicipality = 0;
                                        if (string.IsNullOrEmpty(municipality))
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена стойност за Месторождение (община)!");
                                        }
                                        else
                                        {
                                            if (!municipalities.Any(x => x.MunicipalityName.Trim().ToLower() == municipality))
                                            {
                                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за Месторождение (община)!");
                                            }
                                            else
                                            {
                                                var muni = municipalities.FirstOrDefault(x => x.MunicipalityName.ToLower() == municipality);
                                                if (muni != null)
                                                {
                                                    idMunicipality = muni.idMunicipality;
                                                }
                                            }
                                        }

                                        var location = row.Cells[10].Value.Trim().ToLower();
                                        if (string.IsNullOrEmpty(location))
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена стойност за Месторождение (населено място)!");
                                        }
                                        else
                                        {
                                            string locationAfterRemove = location;
                                            if (location.StartsWith("гр."))
                                            {
                                                locationAfterRemove = location.Remove(0, 3);
                                            }

                                            if (location.StartsWith("гр. "))
                                            {
                                                locationAfterRemove = location.Remove(0, 4);
                                            }

                                            if (location.StartsWith("гр "))
                                            {
                                                locationAfterRemove = location.Remove(0, 3);
                                            }

                                            if (location.StartsWith("град "))
                                            {
                                                locationAfterRemove = location.Remove(0, 5);
                                            }

                                            if (location.StartsWith("с."))
                                            {
                                                locationAfterRemove = location.Remove(0, 2);
                                            }

                                            if (location.StartsWith("с. "))
                                            {
                                                locationAfterRemove = location.Remove(0, 3);
                                            }

                                            if (location.StartsWith("с "))
                                            {
                                                locationAfterRemove = location.Remove(0, 2);
                                            }

                                            if (location.StartsWith("село "))
                                            {
                                                locationAfterRemove = location.Remove(0, 5);
                                            }

                                            if (!locations.Any(x => x.kati.Trim().ToLower() == locationAfterRemove))
                                            {
                                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за Месторождение (населено място)!");
                                            }
                                            else
                                            {
                                                if (!string.IsNullOrEmpty(municipality))
                                                {
                                                    if (municipalities.Any(x => x.MunicipalityName.Trim().ToLower() == municipality))
                                                    {
                                                        //var mun = municipalities.FirstOrDefault(x => x.MunicipalityName.Trim().ToLower() == municipality);
                                                        //var loc = locations.FirstOrDefault(x => x.kati.Trim().ToLower() == locationAfterRemove);
                                                        //if (mun is not null && loc is not null)
                                                        //{
                                                        //    var locationsInMunicipality = locations.Where(x => x.idMunicipality == mun.idMunicipality);
                                                        //    if (!locationsInMunicipality.Any(x => x.idMunicipality == loc.idMunicipality))
                                                        //    {
                                                        //        resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за Месторождение (населено място), което да се намира във въведената стойност на Месторождение (община)!");
                                                        //    }
                                                        //    else
                                                        //    {
                                                        //        clientCourse.IdCityOfBirth = loc.idLocation;
                                                        //    }
                                                        //}

                                                        var mun = municipalities.FirstOrDefault(x => x.MunicipalityName.Trim().ToLower() == municipality);
                                                        if (mun is not null)
                                                        {
                                                            var loc = locations.FirstOrDefault(x => x.kati.Trim().ToLower() == locationAfterRemove && x.idMunicipality == mun.idMunicipality);
                                                            if (loc is not null)
                                                            {
                                                                clientCourse.IdCityOfBirth = loc.idLocation;
                                                            }
                                                            else
                                                            {
                                                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за Месторождение (населено място), което да се намира във въведената стойност на Месторождение (община)!");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            var address = row.Cells[11].Value.Trim();
                            clientCourse.Address = address;

                            var phone = row.Cells[12].Value.Trim();
                            clientCourse.Phone = phone;

                            var email = row.Cells[13].Value.Trim();
                            if (!string.IsNullOrEmpty(email))
                            {
                                var pattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";
                                if (Regex.IsMatch(email, pattern))
                                {
                                    clientCourse.EmailAddress = email;
                                }
                                else
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за имейл адрес!");
                                }
                            }

                            var courseJoinDate = row.Cells[14].Value.Trim();
                            if (string.IsNullOrEmpty(courseJoinDate))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена стойност за Дата на включване в курса!");
                            }
                            else
                            {
                                DateTime date;
                                if (!DateTime.TryParse(courseJoinDate, out date))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за Дата на включване в курса!");
                                }
                                else
                                {
                                    clientCourse.CourseJoinDate = date;

                                    if (date > course.EndDate.Value)
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} въведената стойност за Дата на включване в курса не може да бъде след {course.EndDate.Value.ToString("dd.MM.yyyy")}г.!");
                                    }
                                }
                            }

                            var sponsoring = row.Cells[15].Value.Trim().ToLower();
                            if (string.IsNullOrEmpty(sponsoring))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена стойност за Финансиране!");
                            }
                            else
                            {
                                if (!assingTypeList.Any(x => x == sponsoring))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за Финансиране!");
                                }
                                else
                                {
                                    var kvAssing = kvAssingTypeSource.FirstOrDefault(x => x.Name.ToLower() == sponsoring.ToLower().Trim());
                                    if (kvAssing != null)
                                    {
                                        clientCourse.IdAssignType = kvAssing.IdKeyValue;
                                    }
                                }
                            }

                            var repeatedClientsFromImport = courseClients.FirstOrDefault(x => x.IdIndentType == clientCourse.IdIndentType && x.Indent == clientCourse.Indent);
                            var repeatedClientsFromAddedClients = addedClients.FirstOrDefault(x => x.IdIndentType == clientCourse.IdIndentType && x.Indent == clientCourse.Indent);
                            if (repeatedClientsFromImport is not null || repeatedClientsFromAddedClients is not null)
                            {
                                if (repeatedClientsFromImport is not null)
                                {
                                    resultContext.AddErrorMessage($"В шаблона за импорт вече има въведен курсист с идентификатор {clientCourse.Indent}!");
                                }

                                if (repeatedClientsFromAddedClients is not null)
                                {
                                    resultContext.AddErrorMessage($"В списъка с добавени курсисти вече има въведен курсист с идентификатор {clientCourse.Indent}!");
                                }
                            }
                            else
                            {
                                courseClients.Add(clientCourse);
                            }

                            var isContactAllowed = row.Cells[16].Value.Trim();
                            if (string.IsNullOrEmpty(isContactAllowed))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена валидна стойност за съгласие за използване на информацията за контакт от НАПОО!");
                            }
                            else
                            {
                                clientCourse.IsContactAllowed = isContactAllowed == "Да";
                            }

                            if (!string.IsNullOrEmpty(row.Cells[17].Value))
                            {
                                var isDisabledPerson = row.Cells[17].Value.Trim();
                                clientCourse.IsDisabledPerson = isDisabledPerson == "Да";
                            }

                            if (!string.IsNullOrEmpty(row.Cells[18].Value))
                            {
                                var isDisadvantagedPerson = row.Cells[18].Value.Trim();
                                clientCourse.IsDisadvantagedPerson = isDisadvantagedPerson == "Да";
                            }

                            rowCounter++;
                        }
                    }

                    if (courseClients.Any())
                    {
                        resultContext.AddMessage("Импортът приключи успешно!");
                    }
                    else
                    {
                        resultContext.AddErrorMessage("Не може да бъде добавен празен шаблон за курсисти!");
                    }

                    resultContext.ResultContextObject = courseClients;
                }
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public MemoryStream CreateClientCourseExcelWithErrors(ResultContext<List<ClientCourseVM>> resultContext)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 50;
                sheet.Range[$"A1"].Text = "Вид на грешките:";
                //sheet.Range[$"B1"].Text = "Позиция във файла";

                var rowCounter = 2;
                foreach (var item in resultContext.ListErrorMessages)
                {
                    //var splitMsg = item.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    //var msg = splitMsg[0].Trim();
                    //var cell = splitMsg[1].Trim();

                    //sheet.Range[$"A{rowCounter}"].Text = msg;
                    sheet.Range[$"A{rowCounter}"].Text = item;
                    //sheet.Range[$"B{rowCounter}"].Text = cell;

                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }

        public async Task<bool> IsDuplicateIssuedByIdClientCourseAsync(int idClientCourse)
        {
            var isDuplicateIssued = false;
            try
            {
                var kvDocDuplicate = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "IssueOfDuplicate");
                isDuplicateIssued = await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourse == idClientCourse && x.IdDocumentType == kvDocDuplicate.IdKeyValue).AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return isDuplicateIssued;
        }

        private bool IsClientModified(ClientVM viewModel, Client entity)
        {
            return viewModel.FirstName != entity.FirstName || viewModel.SecondName != entity.SecondName || viewModel.FamilyName != entity.FamilyName
                || viewModel.IdSex != entity.IdSex || viewModel.IdIndentType != entity.IdIndentType || viewModel.Indent != entity.Indent
                || viewModel.BirthDate != entity.BirthDate || viewModel.IdNationality != entity.IdNationality || viewModel.IdEducation != entity.IdEducation
                || viewModel.IdCityOfBirth != entity.IdCityOfBirth || viewModel.IdCountryOfBirth != entity.IdCountryOfBirth;
        }

        private bool IsClientCourseModified(ClientCourseVM viewModel, ClientCourse entity)
        {
            return viewModel.FirstName != entity.FirstName || viewModel.SecondName != entity.SecondName || viewModel.FamilyName != entity.FamilyName
               || viewModel.IdSex != entity.IdSex || viewModel.IdIndentType != entity.IdIndentType || viewModel.Indent != entity.Indent || viewModel.CourseJoinDate != entity.CourseJoinDate
               || viewModel.BirthDate != entity.BirthDate || viewModel.IdNationality != entity.IdNationality || viewModel.IdEducation != entity.IdEducation
               || viewModel.IdAssignType != entity.IdAssignType || viewModel.IdFinishedType != entity.IdFinishedType || viewModel.FinishedDate != entity.FinishedDate
               || viewModel.IdQualificationLevel != entity.IdQualificationLevel || viewModel.IdCityOfBirth != entity.IdCityOfBirth || viewModel.IdCountryOfBirth != entity.IdCountryOfBirth
               || viewModel.Address != entity.Address || viewModel.Phone != entity.Phone || viewModel.EmailAddress != entity.EmailAddress || viewModel.IsContactAllowed != entity.IsContactAllowed
               || viewModel.IsDisadvantagedPerson != entity.IsDisadvantagedPerson || viewModel.IsDisabledPerson != entity.IsDisabledPerson;
        }

        private bool IsConsultingClientModified(ConsultingClientVM viewModel, ConsultingClient entity)
        {
            return viewModel.FirstName != entity.FirstName || viewModel.SecondName != entity.SecondName || viewModel.FamilyName != entity.FamilyName
               || viewModel.IdSex != entity.IdSex || viewModel.IdIndentType != entity.IdIndentType || viewModel.Indent != entity.Indent
               || viewModel.BirthDate != entity.BirthDate || viewModel.IdNationality != entity.IdNationality
               || viewModel.IdAssignType != entity.IdAssignType || viewModel.IdFinishedType != entity.IdFinishedType
               || viewModel.IsContactAllowed != entity.IsContactAllowed || viewModel.IsStudent != entity.IsStudent || viewModel.IsEmployedPerson != entity.IsEmployedPerson || viewModel.IdAimAtCIPOServicesType != entity.IdAimAtCIPOServicesType || viewModel.IdRegistrationAtLabourOfficeType != entity.IdRegistrationAtLabourOfficeType
               || viewModel.StartDate != entity.StartDate || viewModel.EndDate != entity.EndDate || viewModel.IsDisabledPerson != entity.IsDisabledPerson || viewModel.IsDisadvantagedPerson != entity.IsDisadvantagedPerson;
        }

        public async Task<ResultContext<ClientRequiredDocumentVM>> CreateClientRequiredDocumentAsync(ResultContext<ClientRequiredDocumentVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var modelForDb = model.To<ClientRequiredDocument>();
                if (string.IsNullOrEmpty(modelForDb.UploadedFileName))
                {
                    modelForDb.UploadedFileName = string.Empty;
                }
                await this.repository.AddAsync<ClientRequiredDocument>(modelForDb);
                await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject.IdClientRequiredDocument = modelForDb.IdClientRequiredDocument;
                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }
        public async Task<ResultContext<ClientRequiredDocumentVM>> DeleteClientRequiredDocumentAsync(ClientRequiredDocumentVM clientRequiredDocumentVM)
        {
            var entity = await this.repository.GetByIdAsync<ClientRequiredDocument>(clientRequiredDocumentVM.IdClientRequiredDocument);
            this.repository.Detach<ClientRequiredDocument>(entity);

            ResultContext<ClientRequiredDocumentVM> resultContext = new ResultContext<ClientRequiredDocumentVM>();

            try
            {
                this.repository.HardDelete<ClientRequiredDocument>(entity);
                await this.repository.SaveChangesAsync();

                var settingsFolder = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var pathToFile = settingsFolder + "\\" + entity.UploadedFileName;
                if (!string.IsNullOrEmpty(entity.UploadedFileName))
                {
                    if (Directory.Exists(pathToFile))
                    {
                        Directory.Delete(pathToFile, true);
                    }
                }

                resultContext.AddMessage("Документът беше изтрит успешно!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }
        public async Task<ResultContext<ClientRequiredDocumentVM>> UpdateClientRequiredDocumentAsync(ResultContext<ClientRequiredDocumentVM> resultContext)
        {
            try
            {
                var trainingCurriculumFromDb = resultContext.ResultContextObject.To<ClientRequiredDocument>();

                this.repository.Update<ClientRequiredDocument>(trainingCurriculumFromDb);
                await this.repository.SaveChangesAsync();


                resultContext.ResultContextObject = trainingCurriculumFromDb.To<ClientRequiredDocumentVM>();
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
        public async Task<ClientRequiredDocumentVM> GetClientRequiredDocumentById(int id)
        {
            var data = await this.repository.GetByIdAsync<ClientRequiredDocument>(id);
            return data.To<ClientRequiredDocumentVM>();
        }
        public async Task<IEnumerable<ClientRequiredDocumentVM>> GetAllClientRequiredDocumentsByIdClientCourse(int id)
        {
            var kvCoureRequiredTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType", false, true);
            var data = this.repository.AllReadonly<ClientRequiredDocument>(x => x.IdClientCourse == id);
            var dataVM = await data.To<ClientRequiredDocumentVM>().ToListAsync();

            foreach (var document in dataVM)
            {
                document.CourseRequiredDocumentTypeName = kvCoureRequiredTypes.FirstOrDefault(x => x.IdKeyValue == document.IdCourseRequiredDocumentType)?.Name;
                document.CreatePersonName = await this.applicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);
            }
            return dataVM;
        }

        public async Task<bool> IsDocumentPresentAsync(int idClientCourse)
        {
            bool isDocPresent = false;
            var clientCourseDoc = await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourse == idClientCourse).FirstOrDefaultAsync();
            if (clientCourseDoc is not null)
            {
                var docStatuses = await this.repository.AllReadonly<ClientCourseDocumentStatus>(x => x.IdClientCourseDocument == clientCourseDoc!.IdClientCourseDocument).ToListAsync();
                if (docStatuses.Any())
                {
                    var lastDocStatus = docStatuses.LastOrDefault();
                    var kvEnteredInRegsiterStatusValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "EnteredInTheRegister");
                    var kvSubmittedStatusValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Submitted");
                    if (lastDocStatus is not null && (lastDocStatus.IdClientDocumentStatus == kvEnteredInRegsiterStatusValue.IdKeyValue || lastDocStatus.IdClientDocumentStatus == kvSubmittedStatusValue.IdKeyValue))
                    {
                        isDocPresent = true;
                    }
                }
            }

            return isDocPresent;
        }

        public MemoryStream CreateExcelWithMissingRequiredDocumentsForClients(Dictionary<string, string> clientsDict)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 50;
                sheet.Range[$"A1"].Text = "Име на курсист";
                sheet.Range[$"A1"].CellStyle.Font.Bold = true;
                sheet.Range[$"B1"].Text = "Липсващ документ";
                sheet.Range["B1"].ColumnWidth = 50;
                sheet.Range[$"B1"].CellStyle.Font.Bold = true;

                var rowCounter = 2;
                foreach (var entry in clientsDict.Keys.OrderBy(x => x))
                {
                    var clientFullName = entry.Split(".", StringSplitOptions.RemoveEmptyEntries)[1];
                    sheet.Range[$"A{rowCounter}"].Text = clientFullName;
                    sheet.Range[$"B{rowCounter}"].Text = clientsDict[entry];

                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }

        public async Task AddClientCourseDocumentStatusAsync(int idClientCourseDocument, int idStatus, string? comment = null)
        {
            ClientCourseDocumentStatus clientCourseDocumentStatus = new ClientCourseDocumentStatus()
            {
                IdClientCourseDocument = idClientCourseDocument,
                IdClientDocumentStatus = idStatus,
                SubmissionComment = comment
            };

            await this.repository.AddAsync<ClientCourseDocumentStatus>(clientCourseDocumentStatus);
            await this.repository.SaveChangesAsync();
        }

        public async Task AddValidationClientDocumentStatusAsync(int idValidationClientDocument, int idStatus, string? comment = null)
        {
            ValidationClientDocumentStatus validationClientDocumentStatus = new ValidationClientDocumentStatus()
            {
                IdValidationClientDocument = idValidationClientDocument,
                IdClientDocumentStatus = idStatus,
                SubmissionComment = comment
            };

            await this.repository.AddAsync<ValidationClientDocumentStatus>(validationClientDocumentStatus);
            await this.repository.SaveChangesAsync();
        }
        #endregion Training client course 

        #region Training course exam

        public async Task<IEnumerable<CourseCommissionMemberVM>> GetAllCourseCommissionMembersByIdCourseAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<CourseCommissionMember>(x => x.IdCourse == idCourse);

            return await data.To<CourseCommissionMemberVM>().ToListAsync();
        }

        public async Task<ResultContext<CourseCommissionMemberVM>> CreateCourseCommissionMemberAsync(ResultContext<CourseCommissionMemberVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var modelForDb = model.To<CourseCommissionMember>();

                await this.repository.AddAsync<CourseCommissionMember>(modelForDb);
                await this.repository.SaveChangesAsync();

                model.IdCourseCommissionMember = modelForDb.IdCourseCommissionMember;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> DeleteCourseCommissionMemberByIdAsync(int idCourseCommissionMember)
        {
            var result = new ResultContext<NoResult>();
            try
            {
                var entity = await this.repository.GetByIdAsync<CourseCommissionMember>(idCourseCommissionMember);
                if (entity is not null)
                {
                    await this.repository.HardDeleteAsync<CourseCommissionMember>(entity.IdCourseCommissionMember);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешнo!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при изтриване от базата данни!");
            }

            return result;
        }

        public async Task<IEnumerable<CourseCommissionMemberVM>> GetAllCourseCommissionChairmansByIdCourseAsync(int idCourse)
        {
            var courseCommissionMembers = this.repository.AllReadonly<CourseCommissionMember>(x => x.IdCourse == idCourse && x.IsChairman);

            return await courseCommissionMembers.To<CourseCommissionMemberVM>().ToListAsync();
        }

        public async Task<bool> IsChairmanAlreadyInProtocolAddedAsync(int idCourseCommissionMember, int idCourse)
        {
            return (await this.repository.AllReadonly<CourseProtocol>(x => x.IdCourse == idCourse && x.IdCourseCommissionMember == idCourseCommissionMember).ToListAsync()).Any();
        }

        #endregion Training course exam

        #region Training course client document

        public async Task<ClientCourseDocumentVM> GetClientCourseDocumentByIdClientCourseAsync(int idClientCourse)
        {
            var clientCourseDocuments = this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourse == idClientCourse);
            if (clientCourseDocuments.Count() > 1)
            {
                var kvTypeOrdinance = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "CourseRegulation1And7");
                return await clientCourseDocuments.To<ClientCourseDocumentVM>().FirstOrDefaultAsync(x => x.IdDocumentType != kvTypeOrdinance.IdKeyValue);
            }
            else
            {
                return await clientCourseDocuments.To<ClientCourseDocumentVM>().FirstOrDefaultAsync();
            }
        }
        //public async Task<ResultContext<ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM>> CreateClientCourseDocumentAsync(ResultContext<ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM> inputContext)
        //{
        //    var model = new ClientCourseDocument();
        //    var model2 = inputContext.ResultContextObject;


        //    try
        //    {
        //        model.IdClientCourse = model2.IdClientCourse;
        //        model.IdDocumentType = model2.IdDocumentType;
        //        model.FinishedYear = model2.FinishedYear;
        //        model.DocumentPrnNo = "111";
        //        model.DocumentDate = model2.DocumentDate;
        //        model.DocumentProtocol = model2.DocumentProtocol;
        //        // model.TheoryResult = model2.TheoryResult;
        //        //model.PracticeResult = model2.PracticeResult;
        //        //model.QualificationName;
        //        //    model.QualificationLevel;
        //        model.DocumentSerialNumber = model2.DocumentSerialNumber.To<DocumentSerialNumber>();
        //        //model.FinalResult = model2.FinalResult;

        //        model.IdClientCourse = model.ClientCourse.IdClientCourse;
        //        model.IdTypeOfRequestedDocument = model.TypeOfRequestedDocument.IdTypeOfRequestedDocument;
        //        model.IdDocumentSerialNumber = model.DocumentSerialNumber.IdDocumentSerialNumber;

        //        var entryForDb = model.To<ClientCourseDocument>();


        //        await this.repository.AddAsync<ClientCourseDocument>(entryForDb);
        //        await this.repository.SaveChangesAsync();

        //        model.IdClientCourse = entryForDb.IdClientCourse;
        //        model.IdCreateUser = entryForDb.IdCreateUser;
        //        model.IdModifyUser = entryForDb.IdModifyUser;
        //        model.CreationDate = entryForDb.CreationDate;
        //        model.ModifyDate = entryForDb.ModifyDate;

        //        inputContext.AddMessage("Записът е успешен!");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex.Message);
        //        _logger.LogError(ex.InnerException?.Message);
        //        _logger.LogError(ex.StackTrace);
        //        inputContext.AddErrorMessage("Грешка при запис в базата данни!");
        //    }

        //    return inputContext;
        //}

        public async Task<ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM> GetClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedByIdClientCourseAsync(int idClientCourse)
        {
            var model = new ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM();

            var clientCourseFromDb = await this.repository.AllReadonly<ClientCourse>(x => x.IdClientCourse == idClientCourse).Include(x => x.Course).FirstOrDefaultAsync();
            if (clientCourseFromDb is not null)
            {
                var clientCourseDocumentFromDb = await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourse == clientCourseFromDb.IdClientCourse)
                    .Include(x => x.DocumentSerialNumber)
                        .ThenInclude(x => x.TypeOfRequestedDocument)
                            .AsNoTracking()
                    .FirstOrDefaultAsync();

                var courseDocumentUploadedFileFromDb = new List<CourseDocumentUploadedFileVM>();
                if (clientCourseDocumentFromDb is not null)
                {
                    courseDocumentUploadedFileFromDb = await this.repository.AllReadonly<CourseDocumentUploadedFile>(x => x.IdClientCourseDocument == clientCourseDocumentFromDb.IdClientCourseDocument).To<CourseDocumentUploadedFileVM>().ToListAsync();
                }

                model.IdClientCourse = clientCourseFromDb.IdClientCourse;
                model.FinishedDate = clientCourseFromDb.FinishedDate;
                model.IdFinishedType = clientCourseFromDb.IdFinishedType;
                model.IdDocumentType = clientCourseFromDb.Course.IdTrainingCourseType;

                var documentType = this.trainingCourseTypeSource.FirstOrDefault(x => x.IdKeyValue == clientCourseFromDb.Course.IdTrainingCourseType);
                if (documentType is not null)
                {
                    var typeOfReqDoc = await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.IdCourseType == documentType.IdKeyValue).FirstOrDefaultAsync();
                    if (typeOfReqDoc is not null)
                    {
                        model.DocumentTypeName = typeOfReqDoc.DocTypeName!;
                        if (model.DocumentTypeName.Contains("заваряване"))
                        {
                            model.DocumentTypeName = "Свидетелство за правоспособност";
                        }
                    }
                }

                var typeOfRequestedDoc = await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.IdCourseType == clientCourseFromDb.Course.IdTrainingCourseType && x.IsValid).FirstOrDefaultAsync();
                if (typeOfRequestedDoc is not null)
                {
                    model.HasDocumentFabricNumber = typeOfRequestedDoc.HasSerialNumber;
                }

                if (clientCourseDocumentFromDb is not null)
                {
                    model.TheoryResult = clientCourseDocumentFromDb.TheoryResult.HasValue ? clientCourseDocumentFromDb.TheoryResult.Value.ToString("f2") : string.Empty;
                    model.PracticeResult = clientCourseDocumentFromDb.PracticeResult.HasValue ? clientCourseDocumentFromDb.PracticeResult.Value.ToString("f2") : string.Empty;
                    model.IdDocumentStatus = clientCourseDocumentFromDb.IdDocumentStatus;
                    model.IdClientCourseDocument = clientCourseDocumentFromDb.IdClientCourseDocument;
                    if (clientCourseDocumentFromDb.DocumentSerialNumber is not null && clientCourseDocumentFromDb.DocumentSerialNumber.TypeOfRequestedDocument is not null)
                    {
                        model.IdDocumentType = clientCourseDocumentFromDb.DocumentSerialNumber.TypeOfRequestedDocument.IdTypeOfRequestedDocument;
                    }

                    model.FinishedYear = clientCourseDocumentFromDb.FinishedYear;
                    model.IdDocumentSerialNumber = clientCourseDocumentFromDb.IdDocumentSerialNumber;
                    if (clientCourseDocumentFromDb.DocumentSerialNumber is not null)
                    {
                        model.DocumentSerialNumber = clientCourseDocumentFromDb.DocumentSerialNumber.To<DocumentSerialNumberVM>();
                    }

                    model.DocumentRegNo = clientCourseDocumentFromDb.DocumentRegNo;
                    model.DocumentDate = clientCourseDocumentFromDb.DocumentDate;
                    model.TheoryResult = clientCourseDocumentFromDb.TheoryResult.HasValue ? clientCourseDocumentFromDb.TheoryResult.Value.ToString() : string.Empty;
                    model.PracticeResult = clientCourseDocumentFromDb.PracticeResult.HasValue ? clientCourseDocumentFromDb.PracticeResult.Value.ToString() : string.Empty;
                    model.FinalResult = clientCourseDocumentFromDb.FinalResult.HasValue ? clientCourseDocumentFromDb.FinalResult.Value.ToString() : string.Empty;
                    model.IdCourseProtocol = clientCourseDocumentFromDb.IdCourseProtocol;
                    model.DocumentProtocol = clientCourseDocumentFromDb.DocumentProtocol;
                }

                if (courseDocumentUploadedFileFromDb is not null && courseDocumentUploadedFileFromDb.Any())
                {
                        model.IdCourseDocumentUploadedFile = courseDocumentUploadedFileFromDb.First().IdCourseDocumentUploadedFile;
                        model.UploadedFileName = courseDocumentUploadedFileFromDb.First().UploadedFileName;
                    
                    model.courseDocumentUploadedFiles.AddRange(courseDocumentUploadedFileFromDb);
                }
            }

            return model;
        }

        public async Task<ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM> GetLegalCapacityClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedByIdClientCourseAsync(int idClientCourse)
        {
            var model = new ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM();

            var clientCourseFromDb = await this.repository.AllReadonly<ClientCourse>(x => x.IdClientCourse == idClientCourse).Include(x => x.Course).AsNoTracking().FirstOrDefaultAsync();
            if (clientCourseFromDb is not null)
            {
                var clientCourseDocumentFromDb = await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourse == clientCourseFromDb.IdClientCourse)
                    .Include(x => x.TypeOfRequestedDocument)
                    .AsNoTracking()
                    .ToListAsync();

                model.IdClientCourse = clientCourseFromDb.IdClientCourse;
                model.FinishedDate = clientCourseFromDb.FinishedDate;
                model.IdFinishedType = clientCourseFromDb.IdFinishedType;
                model.DocumentTypeName = "Свидетелство за професионална квалификация";
                var kvRegulation = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "CourseRegulation1And7");
                var clientCourseDocumentLegalCapacity = clientCourseDocumentFromDb.FirstOrDefault(x => x.IdDocumentType == kvRegulation.IdKeyValue);
                var clientCourseSPK = clientCourseDocumentFromDb.FirstOrDefault(x => x.IdDocumentType != kvRegulation.IdKeyValue);
                if (clientCourseSPK is not null)
                {
                    model.IdDocumentType = clientCourseSPK.IdDocumentType;

                    CourseDocumentUploadedFile courseDocumentUploadedFileFromDb = new CourseDocumentUploadedFile();
                    if (clientCourseDocumentFromDb.Any())
                    {
                        courseDocumentUploadedFileFromDb = await this.repository.AllReadonly<CourseDocumentUploadedFile>(x => x.IdClientCourseDocument == clientCourseSPK.IdClientCourseDocument).FirstOrDefaultAsync();
                    }

                    if (clientCourseDocumentFromDb is not null)
                    {
                        model.IdClientCourseDocument = clientCourseSPK.IdClientCourseDocument;
                        model.FinishedYear = clientCourseSPK.FinishedYear;
                        model.DocumentRegNo = clientCourseSPK.DocumentRegNo;
                        model.DocumentDate = clientCourseSPK.DocumentDate;
                        model.TheoryResult = clientCourseSPK.TheoryResult.HasValue ? clientCourseSPK.TheoryResult.Value.ToString() : string.Empty;
                        model.PracticeResult = clientCourseSPK.PracticeResult.HasValue ? clientCourseSPK.PracticeResult.Value.ToString() : string.Empty;
                        model.FinalResult = clientCourseSPK.FinalResult.HasValue ? clientCourseSPK.FinalResult.Value.ToString() : string.Empty;
                        model.IdCourseProtocol = clientCourseSPK.IdCourseProtocol;
                        model.DocumentProtocol = clientCourseSPK.DocumentProtocol;
                        model.IdTypeOfRequestedDocument = clientCourseSPK.IdTypeOfRequestedDocument;
                    }

                    if (courseDocumentUploadedFileFromDb is not null)
                    {
                        model.IdCourseDocumentUploadedFile = courseDocumentUploadedFileFromDb.IdCourseDocumentUploadedFile;
                        model.UploadedFileName = courseDocumentUploadedFileFromDb.UploadedFileName;
                    }
                }

                if (clientCourseDocumentLegalCapacity is not null)
                {
                    model.IdLegalCapacityDocumentType = clientCourseDocumentLegalCapacity.IdDocumentType;

                    var courseDocumentUploadedFileFromDb = new List<CourseDocumentUploadedFileVM>();
                    if (clientCourseDocumentFromDb.Any())
                    {
                        courseDocumentUploadedFileFromDb = await this.repository.AllReadonly<CourseDocumentUploadedFile>(x => x.IdClientCourseDocument == clientCourseDocumentLegalCapacity.IdClientCourseDocument).To<CourseDocumentUploadedFileVM>().ToListAsync();
                    }

                    model.IdClientCourse = clientCourseFromDb.IdClientCourse;
                    model.FinishedDate = clientCourseFromDb.FinishedDate;
                    model.IdFinishedType = clientCourseFromDb.IdFinishedType;
                    model.IdDocumentType = clientCourseFromDb.Course.IdTrainingCourseType;

                    if (clientCourseDocumentFromDb is not null)
                    {
                        model.IdLegalCapacityClientCourseDocument = clientCourseDocumentLegalCapacity.IdClientCourseDocument;
                        model.LegalCapacityDocumentRegNo = clientCourseDocumentLegalCapacity.DocumentRegNo;
                        model.LegalCapacityDocumentDate = clientCourseDocumentLegalCapacity.DocumentDate;
                        model.TheoryResult = clientCourseDocumentLegalCapacity.TheoryResult.HasValue ? clientCourseDocumentLegalCapacity.TheoryResult.Value.ToString() : string.Empty;
                        model.PracticeResult = clientCourseDocumentLegalCapacity.PracticeResult.HasValue ? clientCourseDocumentLegalCapacity.PracticeResult.Value.ToString() : string.Empty;
                        model.FinalResult = clientCourseDocumentLegalCapacity.FinalResult.HasValue ? clientCourseDocumentLegalCapacity.FinalResult.Value.ToString() : string.Empty;
                        model.IdCourseProtocol = clientCourseDocumentLegalCapacity.IdCourseProtocol;
                        model.DocumentProtocol = clientCourseDocumentLegalCapacity.DocumentProtocol;
                        model.IdLegalCapacityTypeOfRequestedDocument = clientCourseDocumentLegalCapacity.IdTypeOfRequestedDocument;
                    }

                    if (courseDocumentUploadedFileFromDb.Any())
                    {
                        model.IdLegalCapacityCourseDocumentUploadedFile = courseDocumentUploadedFileFromDb.First().IdCourseDocumentUploadedFile;
                        model.LegalCapacityUploadedFileName = courseDocumentUploadedFileFromDb.First().UploadedFileName;

                        model.LegalCapacityCourseDocumentUploadedFiles.AddRange(courseDocumentUploadedFileFromDb);
                    }
                }
            }

            return model;
        }

        public async Task<ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM> GetClientCourseDuplicateClientCourseDocumentAndCourseDocumentUploadedFileCombinedByIdClientCourseAsync(int idClientCourse)
        {
            var model = new ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM();

            var clientCourseFromDb = await this.repository.AllReadonly<ClientCourse>(x => x.IdClientCourse == idClientCourse).Include(x => x.Course).AsNoTracking().FirstOrDefaultAsync();
            if (clientCourseFromDb is not null)
            {
                var kvIssueOfDuplicate = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "IssueOfDuplicate");
                var kvPartProfessionType = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "PartProfession");
                var clientCourseDocumentFromDb = await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourse == clientCourseFromDb.IdClientCourse && x.IdDocumentType == kvIssueOfDuplicate.IdKeyValue)
                    .Include(x => x.DocumentSerialNumber)
                        .ThenInclude(x => x.TypeOfRequestedDocument)
                            .AsNoTracking()
                    .FirstOrDefaultAsync();

                var firstClientCourseDocumentFromDb = await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourse == clientCourseFromDb.IdClientCourse && x.IdDocumentType == clientCourseFromDb.Course.IdTrainingCourseType)
                    .FirstOrDefaultAsync();

                CourseDocumentUploadedFile courseDocumentUploadedFileFromDb = null;
                if (clientCourseDocumentFromDb is not null)
                {
                    courseDocumentUploadedFileFromDb = await this.repository.AllReadonly<CourseDocumentUploadedFile>(x => x.IdClientCourseDocument == clientCourseDocumentFromDb.IdClientCourseDocument).FirstOrDefaultAsync();
                }

                model.IdClientCourse = clientCourseFromDb.IdClientCourse;
                model.FinishedDate = clientCourseFromDb.FinishedDate;
                model.IdFinishedType = clientCourseFromDb.IdFinishedType;
                model.IdDocumentType = kvIssueOfDuplicate.IdKeyValue;

                var typeOfReqDoc = clientCourseFromDb.Course.IdTrainingCourseType != kvPartProfessionType.IdKeyValue
                    ? await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == "3-54а" && x.IsValid).FirstOrDefaultAsync()
                    : await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == "3-37" && x.IsValid).FirstOrDefaultAsync();
                if (typeOfReqDoc is not null)
                {
                    model.DocumentTypeName = typeOfReqDoc.DocTypeName!;
                    model.HasDocumentFabricNumber = typeOfReqDoc.HasSerialNumber;
                }

                if (clientCourseDocumentFromDb is null && firstClientCourseDocumentFromDb is not null)
                {
                    model.TheoryResult = firstClientCourseDocumentFromDb.TheoryResult.HasValue ? firstClientCourseDocumentFromDb.TheoryResult.Value.ToString() : string.Empty;
                    model.PracticeResult = firstClientCourseDocumentFromDb.PracticeResult.HasValue ? firstClientCourseDocumentFromDb.PracticeResult.Value.ToString() : string.Empty;
                    model.FinalResult = firstClientCourseDocumentFromDb.FinalResult.HasValue ? firstClientCourseDocumentFromDb.FinalResult.Value.ToString() : string.Empty;
                    model.IdCourseProtocol = firstClientCourseDocumentFromDb.IdCourseProtocol;
                    model.DocumentProtocol = firstClientCourseDocumentFromDb.DocumentProtocol;
                }

                if (clientCourseDocumentFromDb is not null)
                {
                    model.IdClientCourseDocument = clientCourseDocumentFromDb.IdClientCourseDocument;
                    model.IdDuplicateDocumentStatus = clientCourseDocumentFromDb.IdDocumentStatus;
                    if (clientCourseDocumentFromDb.DocumentSerialNumber is not null && clientCourseDocumentFromDb.DocumentSerialNumber.TypeOfRequestedDocument is not null)
                    {
                        model.IdDocumentType = clientCourseDocumentFromDb.DocumentSerialNumber.TypeOfRequestedDocument.IdTypeOfRequestedDocument;
                    }

                    model.FinishedYear = clientCourseDocumentFromDb.FinishedYear;
                    model.IdDocumentSerialNumber = clientCourseDocumentFromDb.IdDocumentSerialNumber;
                    if (clientCourseDocumentFromDb.DocumentSerialNumber is not null)
                    {
                        model.DocumentSerialNumber = clientCourseDocumentFromDb.DocumentSerialNumber.To<DocumentSerialNumberVM>();
                    }

                    model.DocumentRegNo = clientCourseDocumentFromDb.DocumentRegNo;
                    model.DocumentDate = clientCourseDocumentFromDb.DocumentDate;
                    model.TheoryResult = clientCourseDocumentFromDb.TheoryResult.HasValue ? clientCourseDocumentFromDb.TheoryResult.Value.ToString() : string.Empty;
                    model.PracticeResult = clientCourseDocumentFromDb.PracticeResult.HasValue ? clientCourseDocumentFromDb.PracticeResult.Value.ToString() : string.Empty;
                    model.FinalResult = clientCourseDocumentFromDb.FinalResult.HasValue ? clientCourseDocumentFromDb.FinalResult.Value.ToString() : string.Empty;
                    model.IdCourseProtocol = clientCourseDocumentFromDb.IdCourseProtocol;
                    model.DocumentProtocol = clientCourseDocumentFromDb.DocumentProtocol;
                }

                if (courseDocumentUploadedFileFromDb is not null)
                {
                    model.IdCourseDocumentUploadedFile = courseDocumentUploadedFileFromDb.IdCourseDocumentUploadedFile;
                    model.UploadedFileName = courseDocumentUploadedFileFromDb.UploadedFileName;
                }
            }

            return model;
        }

        public async Task<IEnumerable<ClientCourseDocumentVM>> GetClientCourseDocumentsByIdCourseAsync(int idCourse)
        {
            var courseDocuments = new List<ClientCourseDocumentVM>();
            var kvFinishedWithDoc = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type1");
            var kvDuplicateIssue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type6");
            var clientCourses = this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == idCourse && (x.IdFinishedType == kvFinishedWithDoc.IdKeyValue || x.IdFinishedType == kvDuplicateIssue.IdKeyValue));
            if (clientCourses.Any())
            {
                var kvSPKDoc = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification");
                var clientCoursesAsVM = await clientCourses.To<ClientCourseVM>(x => x.ClientCourseDocuments.Select(y => y.TypeOfRequestedDocument),
                    x => x.ClientCourseDocuments.Select(y => y.DocumentSerialNumber),
                    x => x.ClientCourseDocuments.Select(y => y.CourseDocumentUploadedFiles)).ToListAsync();

                var kvClientDocumentStatusTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientDocumentStatusType");
                foreach (var client in clientCoursesAsVM)
                {
                    foreach (var doc in client.ClientCourseDocuments.Where(y => y.IdDocumentType == kvSPKDoc.IdKeyValue))
                    {
                        var docType = kvClientDocumentStatusTypeSource.FirstOrDefault(x => x.IdKeyValue == doc.IdDocumentStatus);
                        if (docType is not null)
                        {
                            doc.DocumentStatusValue = docType.Name;
                        }

                        doc.ClientCourse = client;

                        courseDocuments.Add(doc);
                    }
                }
            }

            return courseDocuments.OrderBy(x => x.ClientCourse.FirstName).ThenBy(x => x.ClientCourse.FamilyName).ToList();
        }

        public async Task<ResultContext<NoResult>> SendDocumentsForVerificationAsync(List<ClientCourseDocumentVM> documents, string? comment)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var kvDocumentStatusSubmitted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Submitted");
                foreach (var doc in documents)
                {
                    await this.UpdateClientCourseDocumentStatusAsync(doc.IdClientCourseDocument, kvDocumentStatusSubmitted.IdKeyValue);

                    await this.AddClientCourseDocumentStatusAsync(doc.IdClientCourseDocument, kvDocumentStatusSubmitted.IdKeyValue, comment);
                }

                var msg = documents.Count == 1 ? "Документът е подаден успешно за проверка към НАПОО!" : "Документите са подадени успешно за проверка към НАПОО!";
                resultContext.AddMessage(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task UpdateClientCourseDocumentStatusAsync(int idClientCourseDocument, int idDocumentStatus)
        {
            var clientCourseDocument = await this.repository.GetByIdAsync<ClientCourseDocument>(idClientCourseDocument);
            if (clientCourseDocument is not null)
            {
                clientCourseDocument.IdDocumentStatus = idDocumentStatus;

                this.repository.Update<ClientCourseDocument>(clientCourseDocument);
                await this.repository.SaveChangesAsync();
            }
        }

        public async Task UpdateValidationClientDocumentStatusAsync(int idValidationClientDocument, int idDocumentStatus)
        {
            var validationClientDocument = await this.repository.GetByIdAsync<ValidationClientDocument>(idValidationClientDocument);
            if (validationClientDocument is not null)
            {
                validationClientDocument.IdDocumentStatus = idDocumentStatus;

                this.repository.Update<ValidationClientDocument>(validationClientDocument);
                await this.repository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<ClientCourseDocumentVM>> GetAllIssuedDuplicatesFromCoursesByIdCandidateProviderAndByIdCourseTypeAsync(int idCandidateProvider, int idCourseType)
        {
            var kvCourseStatusFinished = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");
            var kvIssueDuplicate = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "IssueOfDuplicate");
            var courses = this.repository.AllReadonly<Course>(x => x.IdTrainingCourseType == idCourseType && x.IdCandidateProvider == idCandidateProvider && (x.IdStatus == kvCourseStatusFinished.IdKeyValue || x.IsArchived));
            var coursesAsVM = await courses.To<CourseVM>(x => x.ClientCourses.Select(y => y.ClientCourseDocuments.Where(z => z.IdDocumentType == kvIssueDuplicate.IdKeyValue && z.DocumentDate.HasValue && z.DocumentRegNo != null))).ToListAsync();
            var clientCourseDocumentsList = new List<ClientCourseDocumentVM>();
            foreach (var course in coursesAsVM)
            {
                foreach (var client in course.ClientCourses)
                {
                    var clientCourseDocs = client.ClientCourseDocuments.Where(z => z.IdDocumentType == kvIssueDuplicate.IdKeyValue && z.DocumentDate.HasValue && z.DocumentRegNo != null).ToList();
                    if (clientCourseDocs.Any())
                    {
                        foreach (var doc in clientCourseDocs)
                        {
                            var docFromDb = this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourseDocument == doc.IdClientCourseDocument);
                            var docFromDbAsVM = await docFromDb.To<ClientCourseDocumentVM>(x => x.ClientCourse.Course.Program.Speciality.Profession).FirstOrDefaultAsync();
                            clientCourseDocumentsList.Add(docFromDbAsVM);
                        }
                    }
                }
            }

            return clientCourseDocumentsList.OrderByDescending(x => x.DocumentDate).ToList();
        }

        public async Task<ResultContext<NoResult>> CreateDuplicateDocumentAsync(ResultContext<DuplicateIssueVM> inputContext)
        {
            var model = inputContext.ResultContextObject;
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var kvIssueDuplicate = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "IssueOfDuplicate");
                var kvDocumentStatusNotSubmitted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "NotSubmitted");
                var typeOfReqDoc = model.CourseTypeFromToken == GlobalConstants.COURSE_DUPLICATES_SPK
                    ? await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == "3-54a" && x.IsValid).FirstOrDefaultAsync()
                    : await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == "3-37" && x.IsValid).FirstOrDefaultAsync();
                var kvCourseType = model.CourseTypeFromToken == GlobalConstants.COURSE_DUPLICATES_SPK
                    ? await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification")
                    : await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "PartProfession");
                var originalClientCourseDocument = await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourse == model.IdClientCourse && x.IdDocumentType == kvCourseType.IdKeyValue).FirstOrDefaultAsync();
                ClientCourseDocument clientCourseDocument = new ClientCourseDocument()
                {
                    IdClientCourse = model.IdClientCourse!.Value,
                    IdDocumentType = kvIssueDuplicate.IdKeyValue,
                    FinishedYear = model.FinishedYear,
                    DocumentRegNo = model.DocumentRegNo,
                    DocumentDate = model.DocumentDate,
                    DocumentProtocol = model.DocumentProtocol,
                    IdCourseProtocol = model.IdCourseProtocol,
                    IdDocumentSerialNumber = model.IdDocumentSerialNumber,
                    FinalResult = decimal.Parse(model.FinalResult),
                    IdTypeOfRequestedDocument = typeOfReqDoc?.IdTypeOfRequestedDocument,
                    IdOriginalClientCourseDocument = originalClientCourseDocument?.IdOriginalClientCourseDocument
                };

                if (model.IdDocumentSerialNumber.HasValue)
                {
                    clientCourseDocument.IdDocumentStatus = kvDocumentStatusNotSubmitted.IdKeyValue;
                }

                await this.repository.AddAsync<ClientCourseDocument>(clientCourseDocument);
                await this.repository.SaveChangesAsync();

                model.IdClientCourseDocument = clientCourseDocument.IdClientCourseDocument;
                model.IdCreateUser = clientCourseDocument.IdCreateUser;
                model.IdModifyUser = clientCourseDocument.IdModifyUser;
                model.CreationDate = clientCourseDocument.CreationDate;
                model.ModifyDate = clientCourseDocument.ModifyDate;

                if (model.IdDocumentSerialNumber.HasValue)
                {
                    await this.AddClientCourseDocumentStatusAsync(clientCourseDocument.IdClientCourseDocument, kvDocumentStatusNotSubmitted.IdKeyValue);
                }

                CourseDocumentUploadedFile courseDocumentUploadedFile = new CourseDocumentUploadedFile()
                {
                    IdClientCourseDocument = clientCourseDocument.IdClientCourseDocument,
                    UploadedFileName = string.Empty
                };

                await this.repository.AddAsync<CourseDocumentUploadedFile>(courseDocumentUploadedFile);
                await this.repository.SaveChangesAsync();

                model.IdCourseDocumentUploadedFile = courseDocumentUploadedFile.IdCourseDocumentUploadedFile;

                if (model.IdDocumentSerialNumber.HasValue)
                {
                    var docSerialNumberReceivedFromNewEntity = await this.repository.GetByIdAsync<DocumentSerialNumber>(model.IdDocumentSerialNumber);
                    if (docSerialNumberReceivedFromNewEntity is not null)
                    {
                        var docStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
                        var kvDocPrinted = docStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Printed");
                        RequestDocumentManagement requestDocumentManagement = new RequestDocumentManagement()
                        {
                            IdCandidateProvider = this.UserProps.IdCandidateProvider,
                            IdTypeOfRequestedDocument = docSerialNumberReceivedFromNewEntity.IdTypeOfRequestedDocument,
                            DocumentCount = 1,
                            DocumentDate = DateTime.Now,
                            IdDocumentOperation = kvDocPrinted.IdKeyValue,
                            ReceiveDocumentYear = docSerialNumberReceivedFromNewEntity.ReceiveDocumentYear
                        };

                        await this.repository.AddAsync<RequestDocumentManagement>(requestDocumentManagement);
                        await this.repository.SaveChangesAsync();

                        DocumentSerialNumber documentSerialNumber = new DocumentSerialNumber()
                        {
                            IdRequestDocumentManagement = requestDocumentManagement.IdRequestDocumentManagement,
                            IdCandidateProvider = this.UserProps.IdCandidateProvider,
                            IdTypeOfRequestedDocument = docSerialNumberReceivedFromNewEntity.IdTypeOfRequestedDocument,
                            DocumentDate = DateTime.Now,
                            SerialNumber = docSerialNumberReceivedFromNewEntity.SerialNumber,
                            IdDocumentOperation = kvDocPrinted.IdKeyValue,
                            ReceiveDocumentYear = docSerialNumberReceivedFromNewEntity.ReceiveDocumentYear,
                        };

                        await this.repository.AddAsync<DocumentSerialNumber>(documentSerialNumber);
                        await this.repository.SaveChangesAsync();
                    }
                }

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> UpdateDuplicateDocumentAsync(ResultContext<DuplicateIssueVM> inputContext)
        {
            var model = inputContext.ResultContextObject;
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var docFromDb = await this.repository.GetByIdAsync<ClientCourseDocument>(model.IdClientCourseDocument);
                if (docFromDb is not null)
                {
                    var docSerialNumber = await this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdDocumentSerialNumber == model.IdDocumentSerialNumber).FirstOrDefaultAsync();
                    var docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument && x.Year == docSerialNumber.ReceiveDocumentYear).FirstOrDefaultAsync();
                    docFromDb.DocumentPrnNo = $"{docSeries?.SeriesName}/{docSerialNumber?.SerialNumber}";
                    docFromDb.DocumentSerNo = docSeries?.SeriesName;
                    docFromDb.DocumentRegNo = model.DocumentRegNo;
                    docFromDb.DocumentDate = model.DocumentDate;

                    this.repository.Update<ClientCourseDocument>(docFromDb);
                    await this.repository.SaveChangesAsync();

                    model.CreationDate = docFromDb.CreationDate;
                    model.ModifyDate = docFromDb.ModifyDate;

                    resultContext.AddMessage("Записът е успешен!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<IEnumerable<CourseDocumentUploadedFileVM>> GetAllClientCourseDocumentFilesAsync(int idClientCourse)
        {
            return await this.repository.AllReadonly<CourseDocumentUploadedFile>(x => x.ClientCourseDocument.IdClientCourse == idClientCourse).To<CourseDocumentUploadedFileVM>().ToListAsync();
        }

        public async Task<ClientCourseDocumentVM> GetClientCourseDocumentWithUploadedFilesByIdAsync(int idClientCourseDocument)
        {
            return await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourseDocument == idClientCourseDocument).To<ClientCourseDocumentVM>(x => x.CourseDocumentUploadedFiles).FirstOrDefaultAsync();
        }

        #endregion Training course client document

        #region Training course subject

        public async Task<IEnumerable<CourseSubjectVM>> GetAllCourseSubjectsByIdCourseAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<CourseSubject>(x => x.IdCourse == idCourse);
            var dataAsVM = await data.To<CourseSubjectVM>().ToListAsync();
            foreach (var subject in dataAsVM)
            {
                if (subject.TheoryHours != 0)
                {
                    subject.EnteredTheoryGradesCount = await this.GetCourseSubjectEnteredTheoryGradesAsync(subject.IdCourseSubject);
                }

                if (subject.PracticeHours != 0)
                {
                    subject.EnteredPracticeGradesCount = await this.GetCourseSubjectEnteredPracticeGradesAsync(subject.IdCourseSubject);
                }
            }

            return dataAsVM.OrderBy(x => x.IdProfessionalTraining).ThenBy(x => x.Subject).ToList();
        }

        public async Task UpdateSubjectGradeAfterUpdateTrainingCurriculumAsync(int idCourse)
        {
            var curriculums = await this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == idCourse).ToListAsync();
            if (curriculums.Any())
            {
                var dict = new Dictionary<string, List<double>>();
                var kvProfessionalTrainingSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
                var kvA1 = kvProfessionalTrainingSource.FirstOrDefault(x => x.KeyValueIntCode == "A1");
                var kvA2 = kvProfessionalTrainingSource.FirstOrDefault(x => x.KeyValueIntCode == "A2");
                var kvA3 = kvProfessionalTrainingSource.FirstOrDefault(x => x.KeyValueIntCode == "A3");
                var kvB = kvProfessionalTrainingSource.FirstOrDefault(x => x.KeyValueIntCode == "B");
                foreach (var curriculum in curriculums)
                {
                    var practice = curriculum.Practice.HasValue ? (double)curriculum.Practice.Value : 0;
                    var theory = curriculum.Theory.HasValue ? (double)curriculum.Theory.Value : 0;
                    var subject = curriculum.Subject;
                    if (curriculum.IdProfessionalTraining == kvA1.IdKeyValue)
                    {
                        var key = $"A1->{subject}";
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add($"A1->{subject}", new List<double>());
                        }

                        dict[key].Add(theory);
                        dict[key].Add(practice);
                    }
                    else if (curriculum.IdProfessionalTraining == kvA2.IdKeyValue)
                    {
                        var key = $"A2->{subject}";
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add($"A2->{subject}", new List<double>());
                        }

                        dict[key].Add(theory);
                        dict[key].Add(practice);
                    }
                    else if (curriculum.IdProfessionalTraining == kvA3.IdKeyValue)
                    {
                        var key = $"A3->{subject}";
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add($"A3->{subject}", new List<double>());
                        }

                        dict[key].Add(theory);
                        dict[key].Add(practice);
                    }
                    else
                    {
                        var key = $"B->{subject}";
                        if (!dict.ContainsKey(key))
                        {
                            dict.Add($"B->{subject}", new List<double>());
                        }

                        dict[key].Add(theory);
                        dict[key].Add(practice);
                    }
                }

                var subjectsFromDb = await this.repository.AllReadonly<CourseSubject>(x => x.IdCourse == idCourse).ToListAsync();
                foreach (var entry in dict.OrderBy(x => x.Key))
                {
                    var subject = entry.Key.Split("->")[1];
                    int idProfessionalTraining = 0;
                    if (entry.Key.Contains("A1"))
                    {
                        idProfessionalTraining = kvA1.IdKeyValue;
                    }
                    else if (entry.Key.Contains("A2"))
                    {
                        idProfessionalTraining = kvA2.IdKeyValue;
                    }
                    else if (entry.Key.Contains("A3"))
                    {
                        idProfessionalTraining = kvA3.IdKeyValue;
                    }
                    else
                    {
                        idProfessionalTraining = kvB.IdKeyValue;
                    }

                    var subjectFromDb = subjectsFromDb.FirstOrDefault(x => x.Subject.ToLower().Trim() == subject.ToLower().Trim());
                    if (subjectFromDb is not null)
                    {
                        subjectFromDb.TheoryHours = entry.Value[0];
                        subjectFromDb.PracticeHours = entry.Value[1];
                        subjectFromDb.IdProfessionalTraining = idProfessionalTraining;

                        this.repository.Update<CourseSubject>(subjectFromDb);
                        await this.repository.SaveChangesAsync();
                    }
                    else
                    {
                        CourseSubject courseSubject = new CourseSubject()
                        {
                            IdCourse = idCourse,
                            Subject = subject,
                            TheoryHours = entry.Value[0],
                            PracticeHours = entry.Value[1],
                            IdProfessionalTraining = idProfessionalTraining
                        };

                        await this.repository.AddAsync<CourseSubject>(courseSubject);
                        await this.repository.SaveChangesAsync();


                        var clients = await this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == idCourse).ToListAsync();
                        if (clients.Any())
                        {
                            foreach (var client in clients)
                            {
                                CourseSubjectGrade courseSubjectGrade = new CourseSubjectGrade()
                                {
                                    IdClientCourse = client.IdClientCourse,
                                    IdCourseSubject = courseSubject.IdCourseSubject
                                };

                                await this.repository.AddAsync<CourseSubjectGrade>(courseSubjectGrade);
                            }

                            await this.repository.SaveChangesAsync();
                        }
                    }
                }

                foreach (var subjectFromDb in subjectsFromDb)
                {
                    if (!curriculums.Any(x => x.Subject.ToLower().Trim() == subjectFromDb.Subject.ToLower().Trim()))
                    {
                        var subjectGrades = await this.repository.AllReadonly<CourseSubjectGrade>(x => x.IdCourseSubject == subjectFromDb.IdCourseSubject).ToListAsync();
                        if (subjectGrades.Any())
                        {
                            this.repository.HardDeleteRange<CourseSubjectGrade>(subjectGrades);
                            await this.repository.SaveChangesAsync();
                        }

                        await this.repository.HardDeleteAsync<CourseSubject>(subjectFromDb.IdCourseSubject);
                        await this.repository.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<int> GetCourseSubjectEnteredTheoryGradesAsync(int idCourseSubject)
        {
            return await this.repository.AllReadonly<CourseSubjectGrade>(x => x.IdCourseSubject == idCourseSubject && x.TheoryGrade != null).CountAsync();
        }

        public async Task<int> GetCourseSubjectEnteredPracticeGradesAsync(int idCourseSubject)
        {
            return await this.repository.AllReadonly<CourseSubjectGrade>(x => x.IdCourseSubject == idCourseSubject && x.PracticeGrade != null).CountAsync();
        }

        #endregion Training course subject

        #region Training client course subject grade

        public async Task<IEnumerable<CourseSubjectGradeVM>> GetClientCourseSubjectGradeListByClientCourseListIdsAndByIdCourseSubjectAsync(List<int> ids, int idCourseSubject)
        {
            var data = this.repository.AllReadonly<CourseSubjectGrade>(x => ids.Contains(x.IdClientCourse) && x.IdCourseSubject == idCourseSubject);

            return await data.To<CourseSubjectGradeVM>(x => x.ClientCourse).OrderBy(x => x.ClientCourse.FirstName).ThenBy(x => x.ClientCourse.FamilyName).ToListAsync();
        }

        public async Task<CourseSubjectGradeVM> GetClientCourseSubjectGradeByClientCourseIdAndByIdCourseSubjectAsync(int id, int idCourseSubject)
        {
            var data = this.repository.AllReadonly<CourseSubjectGrade>(x => x.IdClientCourse == id && x.IdCourseSubject == idCourseSubject);

            return await data.To<CourseSubjectGradeVM>(x => x.ClientCourse).OrderBy(x => x.ClientCourse.FirstName).ThenBy(x => x.ClientCourse.FamilyName).FirstOrDefaultAsync();
        }

        public async Task<ResultContext<CourseSubjectGradeVM>> UpdateClientCourseSubjectGradeAsync(ResultContext<CourseSubjectGradeVM> resultContext, bool isTheory)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.AllReadonly<CourseSubjectGrade>(x => x.IdCourseSubjectGrade == model.IdCourseSubjectGrade).FirstOrDefaultAsync();
                if (entryFromDb is not null)
                {
                    if (isTheory)
                    {
                        entryFromDb.TheoryGrade = model.TheoryGrade;
                    }
                    else
                    {
                        entryFromDb.PracticeGrade = model.PracticeGrade;
                    }

                    this.repository.Update<CourseSubjectGrade>(entryFromDb);
                    await this.repository.SaveChangesAsync();

                    resultContext.AddMessage("Записът е успешен!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<bool> AreAllSubjectGradesForClientCourseAlreadyAddedByIdClientCourseAsync(int idClientCourse)
        {
            var clientCourseSubjectGrades = await this.repository.AllReadonly<CourseSubjectGrade>(x => x.IdClientCourse == idClientCourse).Include(x => x.CourseSubject).AsNoTracking().ToListAsync();
            bool areGradesAdded = true;
            foreach (var grade in clientCourseSubjectGrades)
            {
                if (grade.CourseSubject.TheoryHours != 0)
                {
                    areGradesAdded = !clientCourseSubjectGrades.Where(x => x.IdCourseSubject == grade.CourseSubject.IdCourseSubject).Any(x => x.TheoryGrade == null);
                }

                if (!areGradesAdded)
                {
                    return areGradesAdded;
                }

                if (grade.CourseSubject.PracticeHours != 0)
                {
                    areGradesAdded = !clientCourseSubjectGrades.Where(x => x.IdCourseSubject == grade.CourseSubject.IdCourseSubject).Any(x => x.PracticeGrade == null);
                }

                if (!areGradesAdded)
                {
                    return areGradesAdded;
                }
            }

            return true;
        }

        #endregion Training client course subject grade     

        #region Training course protocol
        public async Task<IEnumerable<CourseProtocolVM>> GetAllCourseProtocolsByIdCourseAsync(int idCourse)
        {
            var kvEnteredInRegsiterStatusValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "EnteredInTheRegister");
            var kvSubmittedStatusValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Submitted");
            var data = this.repository.AllReadonly<CourseProtocol>(x => x.IdCourse == idCourse);
            var dataAsVM = await data.To<CourseProtocolVM>(x => x.ClientCourseDocuments.Where(z => z.IdDocumentStatus != null)).ToListAsync();
            if (dataAsVM.Any())
            {
                var courseProtocolTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType",false,true);
                foreach (var protocol in dataAsVM)
                {
                    var type = courseProtocolTypeSource.FirstOrDefault(x => x.IdKeyValue == protocol.IdCourseProtocolType);
                    if (type is not null)
                    {
                        protocol.CourseProtocolTypeName = type.Name;
                        protocol.Order = type.Order;
                    }

                    if (protocol.ClientCourseDocuments.Any(y => y.IdDocumentStatus == kvEnteredInRegsiterStatusValue.IdKeyValue || y.IdDocumentStatus == kvSubmittedStatusValue.IdKeyValue))
                    {
                        protocol.IsClientWithDocumentPresent = true;
                    }
                }
            }

            return dataAsVM.OrderByDescending(x => x.CourseProtocolDate.Value).ThenBy(x => x.Order).ToList();
        }

        public async Task<CourseProtocolVM> GetCourseProtocolByIdAsync(int idCourseProtocol)
        {
            var data = this.repository.AllReadonly<CourseProtocol>(x => x.IdCourseProtocol == idCourseProtocol);

            return await data.To<CourseProtocolVM>(x => x.Course).FirstOrDefaultAsync();
        }

        public async Task<CourseProtocolVM> GetCourseProtocolWithoutIncludesByIdAsync(int idCourseProtocol)
        {
            var data = this.repository.AllReadonly<CourseProtocol>(x => x.IdCourseProtocol == idCourseProtocol);

            return await data.To<CourseProtocolVM>().FirstOrDefaultAsync();
        }

        public async Task<ResultContext<CourseProtocolVM>> CreateCourseProtocolAsync(ResultContext<CourseProtocolVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                model.UploadedFileName = string.Empty;
                model.IdCandidateProvider = this.UserProps.IdCandidateProvider;
                var entryForDb = model.To<CourseProtocol>();
                entryForDb.Course = null;
                entryForDb.CourseProtocolGrades = null;

                await this.repository.AddAsync<CourseProtocol>(entryForDb);
                await this.repository.SaveChangesAsync();

                model.IdCreateUser = entryForDb.IdCreateUser;
                model.IdModifyUser = entryForDb.IdModifyUser;
                model.CreationDate = entryForDb.CreationDate;
                model.ModifyDate = entryForDb.ModifyDate;
                model.IdCourseProtocol = entryForDb.IdCourseProtocol;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<CourseProtocolVM>> UpdateCourseProtocolAsync(ResultContext<CourseProtocolVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.AllReadonly<CourseProtocol>(x => x.IdCourseProtocol == model.IdCourseProtocol).FirstOrDefaultAsync();
                entryFromDb = model.To<CourseProtocol>();
                entryFromDb.Course = null;
                entryFromDb.CourseProtocolGrades = null;
                entryFromDb.CourseCommissionMember = null;
                entryFromDb.CandidateProvider = null;

                this.repository.Update<CourseProtocol>(entryFromDb);
                await this.repository.SaveChangesAsync();

                model.IdCreateUser = entryFromDb.IdCreateUser;
                model.IdModifyUser = entryFromDb.IdModifyUser;
                model.CreationDate = entryFromDb.CreationDate;
                model.ModifyDate = entryFromDb.ModifyDate;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<IEnumerable<ClientCourseVM>> GetAllClientsWhichAreNotAddedToProtocolByIdCourseAsync(int idCourse, List<CourseProtocolGradeVM> courseProtocolGrades)
        {
            var data = new List<ClientCourseVM>();

            var clients = await this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == idCourse).ToListAsync();
            foreach (var client in clients)
            {
                if (!courseProtocolGrades.Any(x => x.IdClientCourse == client.IdClientCourse))
                {
                    data.Add(client.To<ClientCourseVM>());
                }
            }

            return data.OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ToList();
        }

        public async Task<bool> AreAllCourseClientsAlreadyAddedToCourseProtocolGradeByIdCourseAndByIdProtocolAsync(int idCourse, int idProtocol)
        {
            var courseClients = await this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == idCourse).ToListAsync();
            var grades = await this.repository.AllReadonly<CourseProtocolGrade>(x => x.IdCourseProtocol == idProtocol).ToListAsync();

            return courseClients.Count == grades.Count;
        }

        public async Task<ResultContext<NoResult>> DeleteCourseProtocolByIdAsync(int idCourseProtocol)
        {
            var result = new ResultContext<NoResult>();

            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<CourseProtocol>(idCourseProtocol);
                if (entryFromDb is not null)
                {
                    var protocolGrades = await this.repository.AllReadonly<CourseProtocolGrade>(x => x.IdCourseProtocol == entryFromDb.IdCourseProtocol).ToListAsync();
                    if (protocolGrades.Any())
                    {
                        this.repository.HardDeleteRange<CourseProtocolGrade>(protocolGrades);
                        await this.repository.SaveChangesAsync();
                    }

                    await this.repository.HardDeleteAsync<CourseProtocol>(entryFromDb.IdCourseProtocol);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return result;
        }

        public async Task<IEnumerable<CourseProtocolVM>> GetAllCourseProtocolsByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var kvEnteredInRegsiterStatusValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "EnteredInTheRegister");
            var kvSubmittedStatusValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Submitted");
            var data = this.repository.AllReadonly<CourseProtocol>(x => x.IdCandidateProvider == idCandidateProvider);
            var dataAsVM = await data.To<CourseProtocolVM>(x => x.Course.Location, x => x.ClientCourseDocuments.Where(z => z.IdDocumentStatus != null)).ToListAsync();
            var formEducationSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation");
            var courseProtocolTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType");
            foreach (var protocol in dataAsVM)
            {
                if (protocol.Course is not null)
                {
                    var formEducation = formEducationSource.FirstOrDefault(x => x.IdKeyValue == protocol.Course.IdFormEducation);
                    if (formEducation is not null)
                    {
                        protocol.Course.FormEducationName = formEducation.Name;
                    }

                    protocol.CoursePeriod = $"{protocol.Course.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)}г. - {protocol.Course.EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)}г.";
                }

                var type = courseProtocolTypeSource.FirstOrDefault(x => x.IdKeyValue == protocol.IdCourseProtocolType);
                if (type is not null)
                {
                    protocol.CourseProtocolTypeName = type.Name;
                }

                if (protocol.ClientCourseDocuments.Any(y => y.IdDocumentStatus == kvEnteredInRegsiterStatusValue.IdKeyValue || y.IdDocumentStatus == kvSubmittedStatusValue.IdKeyValue))
                {
                    protocol.IsClientWithDocumentPresent = true;
                }
            }

            return dataAsVM.OrderByDescending(x => x.CourseProtocolDate.Value).ToList();
        }

        public async Task<IEnumerable<CourseProtocolVM>> GetAll381BProtocolsWhichHaveGradeAddedByIdCourseAndByIdClientCourseAsync(int idCourse, int idClientCourse)
        {
            var kvProtocol381 = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseProtocolType", "3-81B");
            var data = await this.repository.AllReadonly<CourseProtocol>(x => x.IdCourse == idCourse && x.IdCourseProtocolType == kvProtocol381.IdKeyValue)
                .Include(x => x.CourseProtocolGrades.Where(x => x.IdClientCourse == idClientCourse && x.Grade.HasValue)).AsNoTracking()
                .ToListAsync();

            var dataList = new List<CourseProtocolVM>();
            foreach (var protocol in data)
            {
                if (protocol.CourseProtocolGrades.Any())
                {
                    var protocolAsVM = protocol.To<CourseProtocolVM>();
                    dataList.Add(protocolAsVM);
                }
            }

            return dataList.OrderByDescending(x => x.CourseProtocolDate.Value).ToList();
        }

        public async Task<bool> AreProtocols380TAnd380PAlreadyAddedByIdCourseAsync(int idCourse)
        {
            var courseProtocols = await this.repository.AllReadonly<CourseProtocol>(x => x.IdCourse == idCourse).ToListAsync();
            var kv380tProtocol = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseProtocolType", "3-80t");
            var kv380pProtocol = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseProtocolType", "3-80p");

            return courseProtocols.FirstOrDefault(x => x.IdCourseProtocolType == kv380pProtocol.IdKeyValue) != null && courseProtocols.FirstOrDefault(x => x.IdCourseProtocolType == kv380tProtocol.IdKeyValue) != null;
        }

        public async Task<IEnumerable<CourseProtocolVM>> GetCourseProtocol381BByIdClientCourseAsync(int idClientCourse)
        {
            var kvCourseProtocol381B = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseProtocolType", "3-81B");
            var courseProtocolGrades = await this.repository.AllReadonly<CourseProtocolGrade>(x => x.IdClientCourse == idClientCourse && x.Grade.HasValue)
                .Include(x => x.CourseProtocol)
                    .AsNoTracking()
                        .ToListAsync();

            var protocolsSource = new List<CourseProtocolVM>();
            foreach (var protocolGrade in courseProtocolGrades)
            {
                if (protocolGrade.CourseProtocol is not null && protocolGrade.CourseProtocol.IdCourseProtocolType == kvCourseProtocol381B.IdKeyValue)
                {
                    protocolGrade.CourseProtocol.CourseProtocolGrades.Add(protocolGrade);
                    protocolsSource.Add(protocolGrade.CourseProtocol.To<CourseProtocolVM>());
                }
            }

            return protocolsSource;
        }

        #endregion

        #region Training course protocol grade
        public async Task<ResultContext<CourseProtocolVM>> AddAllCourseClientsToCourseProtocolGradeAsync(ResultContext<CourseProtocolVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var clientsCourse = await this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == model.IdCourse).ToListAsync();
                if (clientsCourse.Any())
                {
                    var courseProtocolGrades = await this.repository.AllReadonly<CourseProtocolGrade>(x => x.IdCourseProtocol == model.IdCourseProtocol).ToListAsync();
                    foreach (var client in clientsCourse)
                    {
                        if (!courseProtocolGrades.Any(x => x.IdClientCourse == client.IdClientCourse))
                        {
                            CourseProtocolGrade courseProtocolGrade = new CourseProtocolGrade()
                            {
                                IdCourseProtocol = model.IdCourseProtocol,
                                IdClientCourse = client.IdClientCourse
                            };

                            await this.repository.AddAsync<CourseProtocolGrade>(courseProtocolGrade);
                        }
                    }

                    await this.repository.SaveChangesAsync();

                    resultContext.AddMessage("Всички курсисти са добавени успешно!");
                }
                else
                {
                    resultContext.AddErrorMessage("Към курса за обучение няма въведени курсисти!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<IEnumerable<CourseProtocolGradeVM>> GetAllCourseProtocolGradesByIdProtocolAsync(int idProtocol)
        {
            var data = this.repository.AllReadonly<CourseProtocolGrade>(x => x.IdCourseProtocol == idProtocol);

            return await data.To<CourseProtocolGradeVM>(x => x.ClientCourse).OrderBy(x => x.ClientCourse.FirstName).ThenBy(x => x.ClientCourse.FamilyName).ToListAsync();
        }

        public async Task<ResultContext<CourseProtocolGradeVM>> UpdateCourseProtocolGradeAsync(ResultContext<CourseProtocolGradeVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.AllReadonly<CourseProtocolGrade>(x => x.IdCourseProtocolGrade == model.IdCourseProtocolGrade).FirstOrDefaultAsync();
                if (entryFromDb is not null)
                {
                    entryFromDb.Grade = model.Grade;

                    this.repository.Update<CourseProtocolGrade>(entryFromDb);
                    await this.repository.SaveChangesAsync();

                    await this.UpdateClientCourseGradeAfterCourseProtocolGradeUpdateAsync(model, entryFromDb);

                    resultContext.AddMessage("Записът е успешен!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> DeleteCourseProtocolGradeByIdAsync(int idCourseProtocolGrade)
        {
            var result = new ResultContext<NoResult>();

            var data = await this.repository.GetByIdAsync<CourseProtocolGrade>(idCourseProtocolGrade);
            if (data is not null)
            {
                try
                {
                    await this.SetClientCourseDocumentGradeToNullAfterCourseProtocolDeleteAsync(data);

                    await this.repository.HardDeleteAsync<CourseProtocolGrade>(data.IdCourseProtocolGrade);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException?.Message);
                    _logger.LogError(ex.StackTrace);
                    result.AddErrorMessage("Грешка при запис в базата данни!");
                }
            }

            return result;
        }

        public async Task<ResultContext<NoResult>> AddCourseClientToCourseProtocolGradeAsync(int idClientCourse, int idCourseProtocol)
        {
            var resultContext = new ResultContext<NoResult>();

            try
            {
                CourseProtocolGrade courseProtocolGrade = new CourseProtocolGrade()
                {
                    IdClientCourse = idClientCourse,
                    IdCourseProtocol = idCourseProtocol
                };

                await this.repository.AddAsync<CourseProtocolGrade>(courseProtocolGrade);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Курсистът е добавен успешно!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<List<string>> GetTheoryAndPracticeGradesFromCourseProtocols380ByIdCourseAndIdCourseClient(int idCourse, int idClientCourse)
        {
            var emptyList = new List<string>() { string.Empty, string.Empty };
            var kvProtocol380t = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseProtocolType", "3-80t");
            var kvProtocol380p = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseProtocolType", "3-80p");
            var protocols = this.repository.AllReadonly<CourseProtocol>(x => x.IdCourse == idCourse && (x.IdCourseProtocolType == kvProtocol380t.IdKeyValue || x.IdCourseProtocolType == kvProtocol380p.IdKeyValue));
            var protocolsAsVM = await protocols.To<CourseProtocolVM>(x => x.CourseProtocolGrades).ToListAsync();
            var filteredProtocols = new List<CourseProtocolVM>();
            foreach (var protocol in protocolsAsVM)
            {
                if (protocol.CourseProtocolGrades.Any(x => x.IdClientCourse == idClientCourse && x.Grade.HasValue))
                {
                    protocol.CourseProtocolGrades = protocol.CourseProtocolGrades.Where(x => x.IdClientCourse == idClientCourse).ToList();
                    filteredProtocols.Add(protocol);
                }
            }

            if (filteredProtocols.Any())
            {
                var theoryGrade = string.Empty;
                var practiceGrade = string.Empty;
                var theoryProtocol = filteredProtocols.FirstOrDefault(x => x.IdCourseProtocolType == kvProtocol380t.IdKeyValue);
                var practiceProtocol = filteredProtocols.FirstOrDefault(x => x.IdCourseProtocolType == kvProtocol380p.IdKeyValue);
                if (theoryProtocol is not null && practiceProtocol is not null)
                {
                    var gradeTheory = theoryProtocol.CourseProtocolGrades.FirstOrDefault();
                    var gradePractice = practiceProtocol.CourseProtocolGrades.FirstOrDefault();
                    if (gradeTheory is not null && gradePractice is not null)
                    {
                        if (gradeTheory.Grade.HasValue && gradePractice.Grade.HasValue)
                        {
                            theoryGrade = gradeTheory.Grade.Value.ToString();
                            practiceGrade = gradePractice.Grade.Value.ToString();

                            return new List<string>() { theoryGrade, practiceGrade };
                        }
                    }
                }
            }

            return emptyList;
        }

        private async Task UpdateClientCourseGradeAfterCourseProtocolGradeUpdateAsync(CourseProtocolGradeVM model, CourseProtocolGrade? entryFromDb)
        {
            var clientCourseDocument = await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourse == model.IdClientCourse).FirstOrDefaultAsync();
            if (clientCourseDocument is not null && (clientCourseDocument.TheoryResult.HasValue || clientCourseDocument.PracticeResult.HasValue || clientCourseDocument.FinalResult.HasValue))
            {
                var courseProtocol = await this.repository.GetByIdAsync<CourseProtocol>(model.IdCourseProtocol);
                if (courseProtocol is not null)
                {
                    var kvCourseProtocolTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType");
                    var courseProtocolType = kvCourseProtocolTypeSource.FirstOrDefault(x => x.IdKeyValue == courseProtocol.IdCourseProtocolType);
                    if (courseProtocolType is not null)
                    {
                        switch (courseProtocolType.KeyValueIntCode)
                        {
                            case "3-80p":
                                if (clientCourseDocument.PracticeResult.HasValue && clientCourseDocument.PracticeResult.Value != (decimal)entryFromDb.Grade!.Value)
                                {
                                    clientCourseDocument.PracticeResult = (decimal)entryFromDb.Grade!.Value;
                                }
                                break;
                            case "3-80t":
                                if (clientCourseDocument.TheoryResult.HasValue && clientCourseDocument.TheoryResult.Value != (decimal)entryFromDb.Grade!.Value)
                                {
                                    clientCourseDocument.TheoryResult = (decimal)entryFromDb.Grade!.Value;
                                }
                                break;
                            case "3-81B":
                                if (clientCourseDocument.FinalResult.HasValue && clientCourseDocument.FinalResult.Value != (decimal)entryFromDb.Grade!.Value)
                                {
                                    clientCourseDocument.FinalResult = (decimal)entryFromDb.Grade!.Value;
                                }
                                break;
                        }

                        this.repository.Update<ClientCourseDocument>(clientCourseDocument);
                        await this.repository.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task SetClientCourseDocumentGradeToNullAfterCourseProtocolDeleteAsync(CourseProtocolGrade? data)
        {
            var courseProtocol = await this.repository.GetByIdAsync<CourseProtocol>(data.IdCourseProtocol);
            var clientCourseDoc = await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdClientCourse == data.IdClientCourse).FirstOrDefaultAsync();
            if (courseProtocol is not null && clientCourseDoc is not null)
            {
                var kvCourseProtocolTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType");
                var courseProtocolType = kvCourseProtocolTypeSource.FirstOrDefault(x => x.IdKeyValue == courseProtocol.IdCourseProtocolType);
                if (courseProtocolType is not null)
                {
                    switch (courseProtocolType.KeyValueIntCode)
                    {
                        case "3-80p":
                            if (clientCourseDoc.PracticeResult.HasValue)
                            {
                                clientCourseDoc.PracticeResult = null;
                            }
                            break;
                        case "3-80t":
                            if (clientCourseDoc.TheoryResult.HasValue)
                            {
                                clientCourseDoc.TheoryResult = null;
                            }
                            break;
                        case "3-81B":
                            if (clientCourseDoc.FinalResult.HasValue)
                            {
                                clientCourseDoc.FinalResult = null;
                            }
                            break;
                    }
                }
            }
        }

        #endregion

        #region Training course order
        public async Task<IEnumerable<CourseOrderVM>> GetAllCourseOrdersByIdCourseAsync(int idCourse)
        {
            var data = this.repository.AllReadonly<CourseOrder>(x => x.IdCourse == idCourse);
            var dataAsVM = await data.To<CourseOrderVM>().ToListAsync();

            return dataAsVM;
        }
        public async Task<ResultContext<CourseOrderVM>> CreateCourseOrderAsync(ResultContext<CourseOrderVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                model.UploadedFileName = string.Empty;
                var entryForDb = model.To<CourseOrder>();
                entryForDb.Course = null;

                await this.repository.AddAsync<CourseOrder>(entryForDb);
                await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject.IdCreateUser = entryForDb.IdCreateUser;
                resultContext.ResultContextObject.IdModifyUser = entryForDb.IdModifyUser;
                resultContext.ResultContextObject.CreationDate = entryForDb.CreationDate;
                resultContext.ResultContextObject.ModifyDate = entryForDb.ModifyDate;
                resultContext.ResultContextObject.IdCourseOrder = entryForDb.IdCourseOrder;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }
        public async Task<ResultContext<CourseOrderVM>> UpdateCourseOrderAsync(ResultContext<CourseOrderVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.AllReadonly<CourseOrder>(x => x.IdCourseOrder == model.IdCourseOrder).FirstOrDefaultAsync();
                entryFromDb.Course = null;

                this.repository.Update<CourseOrder>(entryFromDb);
                await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject.IdCreateUser = entryFromDb.IdCreateUser;
                resultContext.ResultContextObject.IdModifyUser = entryFromDb.IdModifyUser;
                resultContext.ResultContextObject.CreationDate = entryFromDb.CreationDate;
                resultContext.ResultContextObject.ModifyDate = entryFromDb.ModifyDate;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }
        public async Task<CourseOrderVM> GetCourseOrderByIdAsync(int idCourseOrder)
        {
            var data = this.repository.AllReadonly<CourseOrder>(x => x.IdCourseOrder == idCourseOrder);

            return await data.To<CourseOrderVM>(x => x.Course).FirstOrDefaultAsync();
        }
        public async Task<ResultContext<CourseOrderVM>> DeleteCourseOrderAsync(ResultContext<CourseOrderVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<CourseOrder>(model.IdCourseOrder);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<CourseOrder>(entryFromDb.IdCourseOrder);
                    await this.repository.SaveChangesAsync();

                    resultContext.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }
        #endregion

        #region Consulting
        public async Task<IEnumerable<ConsultingVM>> GetAllConsultingByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<Consulting>();
            var dataAsVM = await data.To<ConsultingVM>(x => x.ConsultingClient).ToListAsync();
            var result = dataAsVM.Where(x => x.ConsultingClient.IdCandidateProvider == idCandidateProvider).ToList();

            return result;
        }
        #endregion

        #region Consulting client
        public async Task<IEnumerable<ConsultingClientVM>> GetAllConsultingClientsByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<ConsultingClient>(x => x.IdCandidateProvider == idCandidateProvider);
            var dataAsVM = await data.To<ConsultingClientVM>(x => x.Consultings).ToListAsync();
            var indentTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            foreach (var client in dataAsVM)
            {
                var indent = indentTypeSource.FirstOrDefault(x => x.IdKeyValue == client.IdIndentType);
                if (indent is not null)
                {
                    client.IndentType = indent.Name;
                }
            }

            return dataAsVM;
        }

        public async Task<ConsultingClientVM> GetConsultingClientByIdClientAsync(int idClient, int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<ConsultingClient>(x => x.IdClient == idClient && x.IdCandidateProvider == idCandidateProvider);
            var client = await data.To<ConsultingClientVM>().FirstOrDefaultAsync();
            var indentTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            if (client != null)
            {
                var indent = indentTypeSource.FirstOrDefault(x => x.IdKeyValue == client.IdIndentType);
                if (indent is not null)
                {
                    client.IndentType = indent.Name;
                }
            }

            return client;
        }
        public async Task<IEnumerable<ConsultingClientVM>> FilterConsultingClients(ConsultingClientVM filter, int idCandidateProvider)
        {
            List<ConsultingClientVM> filteredConsultingClients = (await GetAllConsultingClientsByIdCandidateProviderAsync(idCandidateProvider)).ToList();

            if (filter.IdRegistrationAtLabourOfficeType != 0 && filter.IdRegistrationAtLabourOfficeType != null)
            {
                filteredConsultingClients = filteredConsultingClients.Where(x => x.IdRegistrationAtLabourOfficeType == filter.IdRegistrationAtLabourOfficeType).ToList();
            }
            if (filter.IdAssignType != 0 && filter.IdAssignType != null)
            {
                filteredConsultingClients = filteredConsultingClients.Where(x => x.IdAssignType == filter.IdAssignType).ToList();
            }
            if (filter.IdAimAtCIPOServicesType != 0 && filter.IdAimAtCIPOServicesType != null)
            {
                filteredConsultingClients = filteredConsultingClients.Where(x => x.IdAimAtCIPOServicesType == filter.IdAimAtCIPOServicesType).ToList();
            }

            if (filter.IsStudent != null)
            {
                filteredConsultingClients = filteredConsultingClients
                    .Where(x => x.IsStudent == filter.IsStudent).ToList();
            }

            if (filter.IsEmployedPerson != null)
            {
                filteredConsultingClients = filteredConsultingClients.Where(x => x.IsEmployedPerson == filter.IsEmployedPerson).ToList();
            }

            if (filter.IdConsultingReceiveType != null && filter.IdConsultingReceiveType != 0)
            {
                filteredConsultingClients = filteredConsultingClients.Where(x => x.Consultings.Any(y => y.IdConsultingReceiveType == filter.IdConsultingReceiveType)).ToList();
            }
            if (filter.IdConsultingType != null && filter.IdConsultingType != 0)
            {
                filteredConsultingClients = filteredConsultingClients.Where(x => x.Consultings.Any(y => y.IdConsultingType == filter.IdConsultingType)).ToList();
            }
            if (filter.Consultings != null && filter.Consultings.Count != 0)
            {
                filteredConsultingClients = filteredConsultingClients.Where(x => filter.Consultings.Any(y => y.IdConsultingType == x.IdConsultingType)).ToList();
            }

            return filteredConsultingClients;
        }
        public async Task<ResultContext<NoResult>> DeleteConsultingClientByIdAsync(int idConsultingClient)
        {
            var result = new ResultContext<NoResult>();

            try
            {
                var consultingDocumentUploadedFileFromDb = (this.repository.AllReadonly<ConsultingDocumentUploadedFile>(x => x.IdConsultingClient == idConsultingClient)).ToList();
                if (consultingDocumentUploadedFileFromDb.Count > 0)
                {
                    await this.repository.HardDeleteAsync<ConsultingDocumentUploadedFile>(consultingDocumentUploadedFileFromDb.First().IdConsultingDocumentUploadedFile);
                    await this.repository.SaveChangesAsync();
                }
                var entryFromDb = await this.repository.GetByIdAsync<ConsultingClient>(idConsultingClient);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<ConsultingClient>(entryFromDb.IdConsultingClient);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return result;
        }

        public async Task<ConsultingClientVM> GetConsultingClientByIdAsync(int idClientConsulting)
        {
            var data = this.repository.AllReadonly<ConsultingClient>(x => x.IdConsultingClient == idClientConsulting);

            return await data.To<ConsultingClientVM>().FirstOrDefaultAsync();
        }

        public async Task<ConsultingFinishedDataVM> GetConsultingClientFinishedDataByIdConsultingClientAsync(int idConsultingClient)
        {
            var consultingFinishedDataVM = new ConsultingFinishedDataVM();
            var consultingClient = await this.repository.AllReadonly<ConsultingClient>(x => x.IdConsultingClient == idConsultingClient)
                .Include(x => x.ConsultingDocumentUploadedFiles).AsNoTracking()
                .FirstOrDefaultAsync();

            if (consultingClient is not null)
            {
                consultingFinishedDataVM.IdFinishedType = consultingClient.IdFinishedType;
                var docUploadedFile = consultingClient.ConsultingDocumentUploadedFiles.FirstOrDefault();
                if (docUploadedFile is not null)
                {
                    consultingFinishedDataVM.IdConsultingDocumentUploadedFile = docUploadedFile.IdConsultingDocumentUploadedFile;
                    consultingFinishedDataVM.UploadedFileName = docUploadedFile.UploadedFileName;
                }
            }

            return consultingFinishedDataVM;
        }

        public async Task<ResultContext<ConsultingClientVM>> CreateConsultingClientAsync(ResultContext<ConsultingClientVM> resultContext, int idCandidateProvider)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                model.IdClient = await this.HandleClientAsync(model, idCandidateProvider);
                model.FirstName = model.FirstName.Trim();
                model.SecondName = !string.IsNullOrEmpty(model.SecondName) ? model.SecondName.Trim() : null;
                model.FamilyName = model.FamilyName.Trim();
                model.Indent = model.Indent.Trim();
                model.IdCandidateProvider = this.UserProps.IdCandidateProvider;
                var consultingClientForDb = model.To<ConsultingClient>();
                consultingClientForDb.Consultings = null;
                consultingClientForDb.Client = null;
                consultingClientForDb.ConsultingDocumentUploadedFiles = null;
                consultingClientForDb.ConsultingTrainers = null;
                consultingClientForDb.ConsultingPremises = null;

                await this.repository.AddAsync<ConsultingClient>(consultingClientForDb);
                await this.repository.SaveChangesAsync();

                model.IdCreateUser = consultingClientForDb.IdCreateUser;
                model.IdModifyUser = consultingClientForDb.IdModifyUser;
                model.CreationDate = consultingClientForDb.CreationDate;
                model.ModifyDate = consultingClientForDb.ModifyDate;
                resultContext.ResultContextObject = consultingClientForDb.To<ConsultingClientVM>();
                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<ConsultingClientVM>> UpdateConsultingClientAsync(ResultContext<ConsultingClientVM> resultContext, int idCandidateProvider, ConsultingFinishedDataVM consultingFinishedDataVM = null)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var consultingClientFromDb = await this.repository.GetByIdAsync<ConsultingClient>(model.IdConsultingClient);
                if (consultingClientFromDb is not null)
                {
                    model.IdClient = await this.HandleClientAsync(model, idCandidateProvider);
                    model.FirstName = model.FirstName.Trim();
                    model.SecondName = !string.IsNullOrEmpty(model.SecondName) ? model.SecondName.Trim() : null;
                    model.FamilyName = model.FamilyName.Trim();
                    model.Indent = model.Indent.Trim();
                    model.IdCreateUser = consultingClientFromDb.IdCreateUser;
                    model.CreationDate = consultingClientFromDb.CreationDate;

                    if (consultingFinishedDataVM is not null && consultingFinishedDataVM.IdConsultingClient != 0)
                    {
                        model.IdFinishedType = consultingFinishedDataVM.IdFinishedType;
                    }
                }
                if (consultingFinishedDataVM is not null)
                {
                    var consultingDocumentUploadedFileFromDb = (this.repository.AllReadonly<ConsultingDocumentUploadedFile>(x => x.IdConsultingClient == model.IdConsultingClient)).ToList();
                    if (consultingDocumentUploadedFileFromDb.Count == 0 && consultingFinishedDataVM.IdFinishedType == ((await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type9")).IdKeyValue))
                    {
                        ConsultingDocumentUploadedFile consultingDocumentUploadedFile = new ConsultingDocumentUploadedFile()
                        {
                            IdConsultingClient = model.IdConsultingClient,
                            UploadedFileName = (model.UploadedFileName != null ? model.UploadedFileName : string.Empty)
                        };

                        await this.repository.AddAsync<ConsultingDocumentUploadedFile>(consultingDocumentUploadedFile);
                        await this.repository.SaveChangesAsync();
                    }

                }

                if (this.IsConsultingClientModified(model, consultingClientFromDb))
                {
                    consultingClientFromDb = model.To<ConsultingClient>();
                    consultingClientFromDb.ConsultingPremises = null;
                    consultingClientFromDb.Client = null;
                    consultingClientFromDb.Consultings = null;
                    consultingClientFromDb.ConsultingDocumentUploadedFiles = null;
                    consultingClientFromDb.ConsultingPremises = null;
                    consultingClientFromDb.CandidateProvider = null;

                    this.repository.Update<ConsultingClient>(consultingClientFromDb);
                    await this.repository.SaveChangesAsync();
                }

                model.IdModifyUser = consultingClientFromDb.IdModifyUser;
                model.ModifyDate = consultingClientFromDb.ModifyDate;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        private async Task<int> CreateConsultingAsync(int idCandidateProvider)
        {
            //var consulting = new Consulting()
            //{
            //    IdCandidateProvider = idCandidateProvider
            //};

            //await this.repository.AddAsync<Consulting>(consulting);
            //await this.repository.SaveChangesAsync();

            //return consulting.IdConsulting;

            return 0;
        }

        private async Task<int> HandleClientAsync(ConsultingClientVM model, int idCandidateProvider)
        {
            var idClient = 0;
            var clientFromDb = await this.repository.AllReadonly<Client>(x => x.IdIndentType == model.IdIndentType && x.Indent == model.Indent.Trim() && x.IdCandidateProvider == idCandidateProvider).FirstOrDefaultAsync();
            if (clientFromDb is null)
            {
                Client client = new Client()
                {
                    FirstName = model.FirstName.Trim(),
                    SecondName = !string.IsNullOrEmpty(model.SecondName) ? model.SecondName.Trim() : null,
                    FamilyName = model.FamilyName.Trim(),
                    IdCandidateProvider = idCandidateProvider,
                    IdSex = model.IdSex,
                    IdIndentType = model.IdIndentType,
                    Indent = model.Indent.ToString().Trim(),
                    BirthDate = model.BirthDate,
                    IdNationality = model.IdNationality,
                    FirstNameEN = BaseHelper.ConvertCyrToLatin(model.FirstName.Trim()),
                    SecondNameEN = !string.IsNullOrEmpty(model.SecondName) ? BaseHelper.ConvertCyrToLatin(model.SecondName.Trim()) : null,
                    FamilyNameEN = BaseHelper.ConvertCyrToLatin(model.FamilyName.Trim())
                };

                await this.repository.AddAsync<Client>(client);
                await this.repository.SaveChangesAsync();

                idClient = client.IdClient;
            }
            else
            {
                ClientVM clientVM = new ClientVM()
                {
                    IdClient = clientFromDb.IdClient,
                    FirstName = model.FirstName.Trim(),
                    SecondName = !string.IsNullOrEmpty(model.SecondName) ? model.SecondName.Trim() : null,
                    FamilyName = model.FamilyName.Trim(),
                    IdCandidateProvider = idCandidateProvider,
                    IdSex = model.IdSex,
                    IdIndentType = model.IdIndentType,
                    Indent = model.Indent.ToString().Trim(),
                    BirthDate = model.BirthDate,
                    IdNationality = model.IdNationality,
                    FirstNameEN = BaseHelper.ConvertCyrToLatin(model.FirstName.Trim()),
                    SecondNameEN = !string.IsNullOrEmpty(model.SecondName) ? BaseHelper.ConvertCyrToLatin(model.SecondName.Trim()) : null,
                    FamilyNameEN = BaseHelper.ConvertCyrToLatin(model.FamilyName.Trim())
                };

                if (this.IsClientModified(clientVM, clientFromDb))
                {
                    clientVM.IdCreateUser = clientFromDb.IdCreateUser;
                    clientVM.CreationDate = clientFromDb.CreationDate;

                    clientFromDb = clientVM.To<Client>();
                    this.repository.Update<Client>(clientFromDb);
                    await this.repository.SaveChangesAsync();
                }

                idClient = clientFromDb.IdClient;
            }

            return idClient;
        }
        public async Task<IEnumerable<ConsultingClientRequiredDocumentVM>> GetAllConsultingClientRequiredDocumentsByIdConsultingClientAsync(int id)
        {
            var kvCoureRequiredTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType");
            var data = this.repository.AllReadonly<ConsultingClientRequiredDocument>(x => x.IdConsultingClient == id);
            var dataVM = await data.To<ConsultingClientRequiredDocumentVM>().ToListAsync();

            foreach (var document in dataVM)
            {
                document.ConsultingRequiredDocumentNameType = kvCoureRequiredTypes.FirstOrDefault(x => x.IdKeyValue == document.IdConsultingRequiredDocumentType).Name;
                document.CreatePersonName = await this.applicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);
            }
            return dataVM;
        }
        public async Task<ResultContext<ConsultingClientRequiredDocumentVM>> CreateConsultingClientRequiredDocumentAsync(ResultContext<ConsultingClientRequiredDocumentVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var modelForDb = model.To<ConsultingClientRequiredDocument>();
                if (string.IsNullOrEmpty(modelForDb.UploadedFileName))
                {
                    modelForDb.UploadedFileName = string.Empty;
                }
                await this.repository.AddAsync<ConsultingClientRequiredDocument>(modelForDb);
                await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject.IdConsultingClientRequiredDocument = modelForDb.IdConsultingClientRequiredDocument;
                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }
        public async Task<ResultContext<ConsultingClientRequiredDocumentVM>> UpdateConsultingClientRequiredDocumentAsync(ResultContext<ConsultingClientRequiredDocumentVM> resultContext)
        {
            try
            {
                var trainingCurriculumFromDb = resultContext.ResultContextObject.To<ConsultingClientRequiredDocument>();

                this.repository.Update<ConsultingClientRequiredDocument>(trainingCurriculumFromDb);
                await this.repository.SaveChangesAsync();


                resultContext.ResultContextObject = trainingCurriculumFromDb.To<ConsultingClientRequiredDocumentVM>();
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
        public async Task<ConsultingClientRequiredDocumentVM> GetConsultingClientRequiredDocumentById(int id)
        {
            var data = await this.repository.GetByIdAsync<ConsultingClientRequiredDocument>(id);
            return data.To<ConsultingClientRequiredDocumentVM>();
        }

        public async Task<List<ConsultingDocumentUploadedFileVM>> GetConsultingClientDocumentUploadedFileById(int id)
        {
            var data = this.repository.All<ConsultingDocumentUploadedFile>().Where(x => x.IdConsultingClient == id);
            var result = await data.To<ConsultingDocumentUploadedFileVM>().ToListAsync();
            return result;
        }
        public async Task<ResultContext<ConsultingClientRequiredDocumentVM>> DeleteConsultingClientRequiredDocumentAsync(int idConsultingClient, int idConsultingDocumentUploadedFile)
        {

            var result = new ResultContext<ConsultingClientRequiredDocumentVM>();

            try
            {
                var consultingDocumentUploadedFileFromDb = (this.repository.AllReadonly<ConsultingClientRequiredDocument>(x => x.IdConsultingClient == idConsultingClient)).ToList();
                if (consultingDocumentUploadedFileFromDb.Count > 0)
                {

                    await this.repository.HardDeleteAsync<ConsultingClientRequiredDocument>(idConsultingDocumentUploadedFile);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }
            result.ResultContextObject = result.ResultContextObject.To<ConsultingClientRequiredDocumentVM>();
            return result;
        }
        #endregion

        #region Consulting client premises
        public async Task<IEnumerable<ConsultingPremisesVM>> GetAllConsultingPremisesByIdConsultingClientAsync(int idConsultingClient)
        {
            var data = this.repository.AllReadonly<ConsultingPremises>(x => x.IdConsultingClient == idConsultingClient);
            var dataAsVM = await data.To<ConsultingPremisesVM>(x => x.CandidateProviderPremises.Location).ToListAsync();

            return dataAsVM.OrderBy(x => x.CandidateProviderPremises.PremisesName).ToList();
        }

        public async Task<ResultContext<ConsultingPremisesVM>> DeleteConsultingPremisesAsync(ResultContext<ConsultingPremisesVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<ConsultingPremises>(model.IdConsultingPremises);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<ConsultingPremises>(entryFromDb.IdConsultingPremises);
                    await this.repository.SaveChangesAsync();

                    resultContext.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<List<CandidateProviderPremisesVM>>> CreateConsultingPremisesByListCandidateProviderPremisesVMAsync(ResultContext<List<CandidateProviderPremisesVM>> resultContext, int idConsultingClient)
        {
            var list = resultContext.ResultContextObject;
            try
            {
                foreach (var premises in list)
                {
                    ConsultingPremises consultingPremises = new ConsultingPremises()
                    {
                        IdPremises = premises.IdCandidateProviderPremises,
                        IdConsultingClient = idConsultingClient
                    };

                    await this.repository.AddAsync<ConsultingPremises>(consultingPremises);
                }

                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        #endregion

        #region Consulting client trainer
        public async Task<IEnumerable<ConsultingTrainerVM>> GetAllConsultingTrainersByIdConsultingClientAsync(int idConsultingClient)
        {
            var data = this.repository.AllReadonly<ConsultingTrainer>(x => x.IdConsultingClient == idConsultingClient);
            var dataAsVM = await data.To<ConsultingTrainerVM>(x => x.CandidateProviderTrainer).ToListAsync();

            return dataAsVM.OrderBy(x => x.CandidateProviderTrainer.FirstName).ThenBy(x => x.CandidateProviderTrainer.FamilyName).ToList();
        }

        public async Task<ResultContext<ConsultingTrainerVM>> DeleteConsultingTrainerAsync(ResultContext<ConsultingTrainerVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<ConsultingTrainer>(model.IdConsultingTrainer);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<ConsultingTrainer>(entryFromDb.IdConsultingTrainer);
                    await this.repository.SaveChangesAsync();

                    resultContext.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<List<CandidateProviderTrainerVM>>> CreateConsultingTrainersByListCandidateProviderTrainerVMAsync(ResultContext<List<CandidateProviderTrainerVM>> resultContext, int idConsultingClient)
        {
            var list = resultContext.ResultContextObject;
            try
            {
                foreach (var trainer in list)
                {
                    ConsultingTrainer consultingTrainer = new ConsultingTrainer()
                    {
                        IdTrainer = trainer.IdCandidateProviderTrainer,
                        IdConsultingClient = idConsultingClient
                    };

                    await this.repository.AddAsync<ConsultingTrainer>(consultingTrainer);
                }

                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }
        #endregion

        #region Training course schedule
        public async Task<IEnumerable<CourseScheduleVM>> GetCourseSchedulesBydIdCourseAsync(int idCourse)
        {
            var trainingCurriculums = this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == idCourse);
            var courseScheduleList = new List<CourseScheduleVM>();
            if (trainingCurriculums.Any())
            {
                var trainingCurriculumsAsVM = await trainingCurriculums.To<TrainingCurriculumVM>(x => x.CourseSchedules.Select(y => y.CandidateProviderPremises), x => x.CourseSchedules.Select(y => y.CandidateProviderTrainer)).ToListAsync();
                var kvProfessionalTrainingSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
                var kvScheduleTrainingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingScheduleType");
                foreach (var trainingCurr in trainingCurriculumsAsVM)
                {
                    if (trainingCurr.CourseSchedules.Any())
                    {
                        foreach (var schedule in trainingCurr.CourseSchedules)
                        {
                            schedule.TrainingCurriculum = trainingCurr.To<TrainingCurriculumVM>();
                            if (schedule.TrainingCurriculum is not null)
                            {
                                var type = kvProfessionalTrainingSource.FirstOrDefault(x => x.IdKeyValue == schedule.TrainingCurriculum.IdProfessionalTraining);
                                if (type is not null)
                                {
                                    schedule.TrainingCurriculum.ProfessionalTraining = type.DefaultValue1;
                                }
                            }

                            var typeSchedule = kvScheduleTrainingTypeSource.FirstOrDefault(x => x.IdKeyValue == schedule.IdTrainingScheduleType);
                            if (typeSchedule is not null)
                            {
                                schedule.TrainingScheduleType = typeSchedule.Name;
                            }
                        }

                        courseScheduleList.AddRange(trainingCurr.CourseSchedules);
                    }
                }
            }

            if (courseScheduleList.All(x => x.TrainingCurriculum != null))
            {
                return courseScheduleList.OrderBy(x => x.ScheduleDate).ThenBy(x => x.TrainingCurriculum.ProfessionalTraining).ThenBy(x => x.TrainingCurriculum.Subject).ThenBy(x => x.TrainingCurriculum.Topic).ToList();
            }

            return courseScheduleList;
        }

        public async Task<CourseScheduleVM> GetCourseScheduleByIdAsync(int idCourseSchedule)
        {
            var data = this.repository.AllReadonly<CourseSchedule>(x => x.IdCourseSchedule == idCourseSchedule);
            var dataAsVM = await data.To<CourseScheduleVM>(x => x.TrainingCurriculum).FirstOrDefaultAsync();
            var professionalTrainingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            var type = professionalTrainingTypeSource.FirstOrDefault(x => x.IdKeyValue == dataAsVM!.TrainingCurriculum.IdProfessionalTraining);
            if (type is not null)
            {
                dataAsVM!.TrainingCurriculum.ProfessionalTraining = type.DefaultValue1!;
            }

            return dataAsVM!;
        }

        public async Task<ResultContext<NoResult>> DeleteCourseScheduleByIdAsync(int idCourseSchedule)
        {
            var result = new ResultContext<NoResult>();

            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<CourseSchedule>(idCourseSchedule);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<CourseSchedule>(entryFromDb.IdCourseSchedule);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return result;
        }

        public async Task<IEnumerable<CandidateProviderTrainerVM>> GetTrainersByIdTrainingTypeByIdCourseAsync(int idTrainingType, int idCourse)
        {
            var kvTrainingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingScheduleType");
            var trainingType = kvTrainingTypeSource.FirstOrDefault(x => x.IdKeyValue == idTrainingType).Name;
            var kvTrainerTheory = await this.dataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TheoryTraining");
            var kvTrainerPractice = await this.dataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "PracticalTraining");
            var trainersCourse = this.repository.AllReadonly<TrainerCourse>(x => x.IdCourse == idCourse);
            if (trainingType == "Теория")
            {
                trainersCourse = trainersCourse.Where(x => x.IdТraininType != kvTrainerPractice.IdKeyValue);
            }
            else
            {
                trainersCourse = trainersCourse.Where(x => x.IdТraininType != kvTrainerTheory.IdKeyValue);
            }

            var dataAsVM = await trainersCourse.To<TrainerCourseVM>(x => x.CandidateProviderTrainer).ToListAsync();
            var trainers = dataAsVM.Select(x => x.CandidateProviderTrainer).OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ToList();

            return trainers;
        }

        public async Task<IEnumerable<CandidateProviderPremisesVM>> GetPremisesByIdTrainingTypeByIdCourseAsync(int idTrainingType, int idCourse)
        {
            var kvTrainingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingScheduleType");
            var trainingType = kvTrainingTypeSource.FirstOrDefault(x => x.IdKeyValue == idTrainingType).Name;
            var kvMTBTheory = await this.dataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TheoryTraining");
            var kvMTBPractice = await this.dataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "PracticalTraining");
            var premisesCourse = this.repository.AllReadonly<PremisesCourse>(x => x.IdCourse == idCourse);
            if (trainingType == "Теория")
            {
                premisesCourse = premisesCourse.Where(x => x.IdТraininType != kvMTBPractice.IdKeyValue);
            }
            else
            {
                premisesCourse = premisesCourse.Where(x => x.IdТraininType != kvMTBTheory.IdKeyValue);
            }

            var dataAsVM = await premisesCourse.To<PremisesCourseVM>(x => x.CandidateProviderPremises).ToListAsync();
            var premises = dataAsVM.Select(x => x.CandidateProviderPremises).OrderBy(x => x.PremisesName).ToList();

            return premises;
        }

        public async Task<ResultContext<CourseScheduleVM>> CreateCourseScheduleAsync(ResultContext<CourseScheduleVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryForDb = model.To<CourseSchedule>();
                entryForDb.TrainingCurriculum = null;
                entryForDb.CandidateProviderPremises = null;
                entryForDb.CandidateProviderTrainer = null;

                await this.repository.AddAsync<CourseSchedule>(entryForDb);
                await this.repository.SaveChangesAsync();

                model.IdCreateUser = entryForDb.IdCreateUser;
                model.IdModifyUser = entryForDb.IdModifyUser;
                model.CreationDate = entryForDb.CreationDate;
                model.ModifyDate = entryForDb.ModifyDate;

                resultContext.AddMessage("Записът е успешен!");

                model.IdCourseSchedule = entryForDb.IdCourseSchedule;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<CourseScheduleVM>> UpdateCourseScheduleAsync(ResultContext<CourseScheduleVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<CourseSchedule>(model.IdCourseSchedule);
                entryFromDb = model.To<CourseSchedule>();
                entryFromDb.TrainingCurriculum = null;
                entryFromDb.CandidateProviderPremises = null;
                entryFromDb.CandidateProviderTrainer = null;

                this.repository.Update<CourseSchedule>(entryFromDb);
                await this.repository.SaveChangesAsync();

                model.IdModifyUser = entryFromDb.IdModifyUser;
                model.ModifyDate = entryFromDb.ModifyDate;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<IEnumerable<TrainingCurriculumVM>> GetAllTrainingCurriculumsByIdCourseAndByHoursTypeAsync(int idCourse, string hoursType)
        {
            var data = this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == idCourse);
            if (hoursType == "Теория")
            {
                data = data.Where(x => x.Theory != null & x.Theory != 0);
            }
            else
            {
                data = data.Where(x => x.Practice != null && x.Practice != 0);
            }

            var dataAsVM = await data.To<TrainingCurriculumVM>().ToListAsync();
            var professionalTrainingSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            foreach (var curr in dataAsVM)
            {
                var type = professionalTrainingSource.FirstOrDefault(x => x.IdKeyValue == curr.IdProfessionalTraining);
                if (type is not null)
                {
                    curr.ProfessionalTraining = type.DefaultValue1!;
                }

                curr.Hours = hoursType == "Теория" ? curr.Theory!.Value : curr.Practice!.Value;
            }

            return dataAsVM.OrderBy(x => x.ProfessionalTraining).ThenBy(x => x.Subject).ThenBy(x => x.Topic).ToList();
        }

        public async Task<ResultContext<NoResult>> AddPremisesToListCourseSchedulesAsync(int idCandidateProviderPremises, List<int> scheduleListIds)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                foreach (var id in scheduleListIds)
                {
                    var scheduleFromDb = await this.repository.GetByIdAsync<CourseSchedule>(id);
                    if (scheduleFromDb is not null)
                    {
                        scheduleFromDb.IdCandidateProviderPremises = idCandidateProviderPremises;

                        this.repository.Update(scheduleFromDb);
                        await this.repository.SaveChangesAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> AddTrainerToListCourseSchedulesAsync(int idCandidateProviderTrainer, List<int> scheduleListIds)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                foreach (var id in scheduleListIds)
                {
                    var scheduleFromDb = await this.repository.GetByIdAsync<CourseSchedule>(id);
                    if (scheduleFromDb is not null)
                    {
                        scheduleFromDb.IdCandidateProviderTrainer = idCandidateProviderTrainer;

                        this.repository.Update(scheduleFromDb);
                        await this.repository.SaveChangesAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<MemoryStream> PrintSchedulePlanAsync(List<CourseScheduleVM> addedSchedules, CandidateProviderVM candidateProvider, List<ClientCourseVM> clients, int idSpeciality, DateTime? courseStartDate)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\CPO\Training";

            FileStream template = new FileStream($@"{resources_Folder}\Schedule_Plan.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Doc);
            WSection section = document.Sections[0];

            WTable tableClients = section.Tables[0] as WTable;
            var kvSexSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");
            var kvNationalitySource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality");
            var counter = 3;
            foreach (var client in clients)
            {
                WTableRow row = tableClients!.Rows[2].Clone();

                WTableCell firstCell = row.Cells[0];
                firstCell.Paragraphs[0].Text = client.Indent;

                WTableCell secondCell = row.Cells[1];
                secondCell.Paragraphs[0].Text = client.FullName;

                WTableCell thirdCell = row.Cells[2];
                thirdCell.Paragraphs[0].Text = $"{client.BirthDate!.Value.ToString(GlobalConstants.DATE_FORMAT)}г.";

                var sex = kvSexSource.FirstOrDefault(x => x.IdKeyValue == client.IdSex)!.Name;
                WTableCell fourthCell = row.Cells[3];
                fourthCell.Paragraphs[0].Text = sex;

                var nationality = kvNationalitySource.FirstOrDefault(x => x.IdKeyValue == client.IdNationality)!.Name;
                WTableCell fifthCell = row.Cells[4];
                fifthCell.Paragraphs[0].Text = nationality;

                WTableCell sixthCell = row.Cells[5];
                sixthCell.Paragraphs[0].Text = client.Address != null ? client.Address : string.Empty;

                WTableCell seventhCell = row.Cells[6];
                seventhCell.Paragraphs[0].Text = client.Phone != null ? client.Phone : string.Empty;

                tableClients.Rows.Insert(counter++, row);
            }

            tableClients!.Rows.RemoveAt(2);

            WTable tableSchedule = section.Tables[1] as WTable;
            var kvTrainingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingScheduleType");
            counter = 4;
            foreach (var schedule in addedSchedules)
            {
                var curriculum = schedule.TrainingCurriculum;

                WTableRow row = tableSchedule!.Rows[3].Clone();

                WTableCell firstCell = row.Cells[0];
                firstCell.Paragraphs[0].Text = $"{schedule.ScheduleDate!.Value.ToString(GlobalConstants.DATE_FORMAT)}г.";

                WTableCell secondCell = row.Cells[1];
                secondCell.Paragraphs[0].Text = curriculum.Subject;

                WTableCell thirdCell = row.Cells[2];
                thirdCell.Paragraphs[0].Text = curriculum.Topic;

                WTableCell fourthCell = row.Cells[3];
                fourthCell.Paragraphs[0].Text = schedule.Hours!.Value.ToString();

                WTableCell fifthCell = row.Cells[4];
                fifthCell.Paragraphs[0].Text = schedule.Period;

                var trainingType = kvTrainingTypeSource.FirstOrDefault(x => x.IdKeyValue == schedule.IdTrainingScheduleType)!.Name;
                if (trainingType == "Теория")
                {
                    if (schedule.CandidateProviderPremises is not null)
                    {
                        WTableCell sixthCell = row.Cells[5];
                        sixthCell.Paragraphs[0].Text = schedule.CandidateProviderPremises.PremisesName;
                    }

                    if (schedule.CandidateProviderTrainer is not null)
                    {
                        WTableCell ninthCell = row.Cells[8];
                        ninthCell.Paragraphs[0].Text = schedule.CandidateProviderTrainer.FullName;
                    }
                }
                else
                {
                    if (schedule.CandidateProviderPremises is not null)
                    {
                        WTableCell seventhCell = row.Cells[6];
                        seventhCell.Paragraphs[0].Text = schedule.CandidateProviderPremises.PremisesName;
                    }

                    if (schedule.CandidateProviderTrainer is not null)
                    {
                        WTableCell tenthCell = row.Cells[9];
                        tenthCell.Paragraphs[0].Text = schedule.CandidateProviderTrainer.FullName;
                    }
                }

                tableSchedule.Rows.Insert(counter++, row);
            }

            tableSchedule!.Rows.RemoveAt(3);

            string[] fieldNames = new string[]
            {
                "PROVIDERNAME", "ProviderLocation", "ProfessionInfo", "SpecialityInfo", "Year"
            };

            var location = await this.locationService.GetLocationByLocationIdAsync(candidateProvider.IdLocationCorrespondence!.Value);
            var specialities = this.dataSourceService.GetAllSpecialitiesList();
            var speciality = specialities.FirstOrDefault(x => x.IdSpeciality == idSpeciality)!;
            var professions = this.dataSourceService.GetAllProfessionsList();
            var profession = professions.FirstOrDefault(x => x.IdProfession == speciality.IdProfession)!;
            string[] fieldValues = new string[]
            {
                candidateProvider.ProviderOwner,
                location.LocationName,
                $"{profession.Code}, {profession.Name}",
                $"{speciality.Code}, {speciality.Name}",
                $"{courseStartDate!.Value.Year.ToString()}г."
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            document.Dispose();
            template.Close();

            return stream;
        }

        public async Task<MemoryStream> PrintScheduleProfessionalTrainingAsync(CourseVM course)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\CPO\Training";

            FileStream template = new FileStream($@"{resources_Folder}\Schedule_ProfessionalTraining.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Doc);
            WSection section = document.Sections[0];

            string[] fieldNames = new string[]
            {
                "ProviderName", "ProviderLocation", "ProfessionInfo", "SpecialityInfo", "StartDate", "EndDate", "TheoryExamDate", "PracticeExamDate"
            };

            var location = await this.locationService.GetLocationByLocationIdAsync(course.CandidateProvider.IdLocationCorrespondence!.Value);
            var specialities = this.dataSourceService.GetAllSpecialitiesList();
            var speciality = specialities.FirstOrDefault(x => x.IdSpeciality == course.Program.IdSpeciality)!;
            var professions = this.dataSourceService.GetAllProfessionsList();
            var profession = professions.FirstOrDefault(x => x.IdProfession == speciality.IdProfession)!;
            string[] fieldValues = new string[]
            {
                course.CandidateProvider.ProviderOwner,
                location.LocationName,
                $"{profession.Code}, {profession.Name}",
                $"{speciality.Code}, {speciality.Name}",
                $"{course.StartDate!.Value.ToString(GlobalConstants.DATE_FORMAT)}г.",
                $"{course.EndDate!.Value.ToString(GlobalConstants.DATE_FORMAT)}г.",
                course.ExamTheoryDate.HasValue ? $"{course.ExamTheoryDate!.Value.ToString(GlobalConstants.DATE_FORMAT)}г." : string.Empty,
                course.ExamPracticeDate.HasValue ? $"{course.ExamPracticeDate!.Value.ToString(GlobalConstants.DATE_FORMAT)}г." : string.Empty,
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();

            document.Save(stream, FormatType.Docx);
            document.Dispose();
            template.Close();

            return stream;
        }

        public async Task<MemoryStream> GetCourseScheduleTemplateWithCurriculumsFilledByIdCourseAsync(int idCourse)
        {
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\CPO\Training";
            var fileFullName = $"{resources_Folder}\\Dnevnik_ucheben_plan.xlsx";

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                using (FileStream fileStream = new FileStream(fileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    IApplication app = excelEngine.Excel;

                    IWorkbook workbook = app.Workbooks.Open(fileStream, ExcelOpenType.Automatic);

                    IWorksheet worksheet = workbook.Worksheets[0];

                    var trainingCurriculums = this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == idCourse);
                    var trainingCurriculumsAsVM = await trainingCurriculums.To<TrainingCurriculumVM>().OrderBy(x => x.IdProfessionalTraining).ThenBy(x => x.Subject).ThenBy(x => x.Topic).ToListAsync();
                    var professionalTrainingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");

                    worksheet.Range[$"A2:A5000"].NumberFormat = "dd.MM.yyyy";
                    worksheet.Range[$"G2:G5000"].NumberFormat = "hh:mm";
                    worksheet.Range[$"H2:H5000"].NumberFormat = "hh:mm";
                    worksheet.Range[$"F2:F5000"].NumberFormat = "#";

                    IDataValidation professionalTrainingTypeValidation = worksheet.Range["B2:B5000"].DataValidation;
                    professionalTrainingTypeValidation.AllowType = ExcelDataType.User;
                    professionalTrainingTypeValidation.ListOfValues = professionalTrainingTypeSource.Select(x => x.DefaultValue1).ToArray();

                    IDataValidation hoursTypeValidation = worksheet.Range["E2:E5000"].DataValidation;
                    hoursTypeValidation.AllowType = ExcelDataType.User;
                    hoursTypeValidation.ListOfValues = new string[] { "Теория", "Практика" };

                    var rowCounter = 2;
                    foreach (var curriculum in trainingCurriculumsAsVM)
                    {
                        var professionalTrainingTypeValue = professionalTrainingTypeSource.FirstOrDefault(x => x.IdKeyValue == curriculum.IdProfessionalTraining);
                        if (professionalTrainingTypeValue is not null)
                        {
                            curriculum.ProfessionalTraining = professionalTrainingTypeValue.DefaultValue1!;
                        }

                        if (curriculum.Theory.HasValue)
                        {
                            worksheet.Range[$"B{rowCounter}"].Text = curriculum.ProfessionalTraining;
                            worksheet.Range[$"C{rowCounter}"].Text = curriculum.Subject;
                            worksheet.Range[$"D{rowCounter}"].Text = curriculum.Topic;
                            worksheet.Range[$"E{rowCounter}"].Text = "Теория";
                            worksheet.Range[$"F{rowCounter++}"].Text = curriculum.Theory.Value.ToString();
                        }

                        if (curriculum.Practice.HasValue)
                        {
                            worksheet.Range[$"B{rowCounter}"].Text = curriculum.ProfessionalTraining;
                            worksheet.Range[$"C{rowCounter}"].Text = curriculum.Subject;
                            worksheet.Range[$"D{rowCounter}"].Text = curriculum.Topic;
                            worksheet.Range[$"E{rowCounter}"].Text = "Практика";
                            worksheet.Range[$"F{rowCounter++}"].Text = curriculum.Practice.Value.ToString();
                        }
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        return stream;
                    }
                }
            }
        }

        public async Task<ResultContext<List<CourseScheduleVM>>> ImportCourseScheduleAsync(MemoryStream file, string fileName, CourseVM course)
        {
            var resultContext = new ResultContext<List<CourseScheduleVM>>();
            try
            {
                int counter = GlobalConstants.INVALID_ID_ZERO;

                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\Temp\\ImportCourseSchedule";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var path = @"" + filePath + "\\" + fileName;

                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file.WriteTo(filestream);
                    filestream.Close();
                    file.Close();
                }

                var courseScheduleList = new List<CourseScheduleVM>();
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        IApplication app = excelEngine.Excel;

                        IWorkbook workbook = app.Workbooks.Open(fileStream, ExcelOpenType.Automatic);

                        IWorksheet worksheet = workbook.Worksheets[0];
                        if (string.IsNullOrEmpty(worksheet.Rows[0].Columns[0].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[1].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[2].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[3].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[4].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[5].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[6].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[7].Text))
                        {
                            resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на курсисти!");
                            return resultContext;
                        }

                        var firstHeader = worksheet.Rows[0].Columns[0].Text.Trim();
                        var secondHeader = worksheet.Rows[0].Columns[1].Text.Trim();
                        var thirdHeader = worksheet.Rows[0].Columns[2].Text.Trim();
                        var fourthHeader = worksheet.Rows[0].Columns[3].Text.Trim();
                        var fifthHeader = worksheet.Rows[0].Columns[4].Text.Trim();
                        var sixthHeader = worksheet.Rows[0].Columns[5].Text.Trim();
                        var seventhHeader = worksheet.Rows[0].Columns[6].Text.Trim();
                        var eighthHeader = worksheet.Rows[0].Columns[7].Text.Trim();
                        bool skipFirstRow = true;

                        //Проверка по 1 клетка за да се провери дали файла за импорт на учебна програма
                        if (firstHeader != "Дата *" || secondHeader != "Вид професионална подготовка *" || thirdHeader != "Предмет *" || fourthHeader != "Тема *" || fifthHeader != "Вид на обучението *"
                            || sixthHeader != "Часове *" || seventhHeader != "Продължителност от" || eighthHeader != "Продължителност до")
                        {
                            resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на дневник на провеждано обучение!");
                            return resultContext;
                        }

                        var rowCounter = 2;
                        var trainingCurriculums = await this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == course.IdCourse).ToListAsync();
                        var addedCourseSchedulesDict = new Dictionary<string, Dictionary<double, double>>();
                        foreach (var row in worksheet.Rows)
                        {
                            var dateCell = row.Cells[0].Value.Trim();

                            //Пропуска 1 ред който е с хедърите
                            if (skipFirstRow || string.IsNullOrEmpty(dateCell))
                            {
                                skipFirstRow = false;
                                counter++;
                                continue;
                            }

                            //Ако сме пропуснали 5 реда приемаме че документа е приключил и са останали празни редове
                            if (counter == 5)
                            {
                                break;
                            }

                            var model = new CourseScheduleVM();
                            var trainingCurriculumModel = new TrainingCurriculumVM();

                            if (string.IsNullOrEmpty(dateCell))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} не е въведена стойност в поле 'Дата'!");
                            }
                            else
                            {
                                DateTime validDate;
                                if (!DateTime.TryParse(dateCell, out validDate))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма попълнена валидна стойност в поле 'Дата'!");
                                }
                                else
                                {
                                    var dateCellAsDateTime = DateTime.Parse(dateCell);
                                    if (course.StartDate.HasValue && course.EndDate.HasValue)
                                    {
                                        if (dateCellAsDateTime < course.StartDate.Value)
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} 'Дата' не може да бъде преди {course.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г.!");
                                        }

                                        if (dateCellAsDateTime > course.EndDate.Value)
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} 'Дата' не може да бъде след {course.EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г.!");
                                        }
                                    }

                                    model.ScheduleDate = dateCellAsDateTime;
                                }
                            }

                            var professionalTrainingTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
                            var kvA1 = professionalTrainingTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "A1");
                            var kvA2 = professionalTrainingTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "A2");
                            var kvA3 = professionalTrainingTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "A3");
                            var kvB = professionalTrainingTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "B");
                            var ptList = new List<string>()
                            {
                                { kvA1.DefaultValue1!.Trim() },
                                { kvA2.DefaultValue1!.Trim() },
                                { kvA3.DefaultValue1.Trim() },
                                { kvB.DefaultValue1.Trim() }
                            };

                            var trainingScheduleTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingScheduleType");
                            var kvPractice = trainingScheduleTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "Practice");
                            var kvTheory = trainingScheduleTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "Theory");
                            var trainingScheduleTypesList = new List<string>()
                            {
                                { kvPractice.Name.Trim() },
                                { kvTheory.Name.Trim() }
                            };

                            var professionalTrainingCell = row.Cells[1].Value.Trim();
                            if (string.IsNullOrEmpty(professionalTrainingCell))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} не е въведена стойност в поле 'Вид професионална подготовка'!");
                            }
                            else
                            {
                                if (!ptList.Any(x => x == professionalTrainingCell))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} не е въведена валидна стойност в поле 'Вид професионална подготовка'!");
                                }
                                else
                                {
                                    trainingCurriculumModel.IdProfessionalTraining = professionalTrainingTypesSource.FirstOrDefault(x => x.DefaultValue1 == professionalTrainingCell).IdKeyValue;
                                }
                            }

                            var subjectCell = row.Cells[2].Value.Trim();
                            if (string.IsNullOrEmpty(subjectCell))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} не е въведена стойност в поле 'Предмет'!");
                            }
                            else
                            {
                                trainingCurriculumModel.Subject = subjectCell;
                            }

                            var topicCell = row.Cells[3].Value.Trim();
                            if (string.IsNullOrEmpty(topicCell))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} не е въведена стойност в поле 'Тема'!");
                            }
                            else
                            {
                                trainingCurriculumModel.Topic = topicCell;
                            }

                            var trainingScheduleTypeCell = row.Cells[4].Value.Trim();
                            if (string.IsNullOrEmpty(trainingScheduleTypeCell))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} не е въведена стойност в поле 'Вид на обучението'!");
                            }
                            else
                            {
                                if (!trainingScheduleTypesList.Any(x => x == trainingScheduleTypeCell))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} не е въведена валидна стойност в поле 'Вид на обучението'!");
                                }
                                else
                                {
                                    model.IdTrainingScheduleType = trainingScheduleTypesSource.FirstOrDefault(x => x.Name == trainingScheduleTypeCell).IdKeyValue;
                                }
                            }

                            var hoursCell = row.Cells[5].Value.Trim();
                            if (string.IsNullOrEmpty(hoursCell))
                            {
                                resultContext.AddErrorMessage($"На ред {rowCounter} не е въведена стойност в поле 'Часове'!");
                            }
                            else
                            {
                                int validHours;
                                if (!int.TryParse(hoursCell, out validHours))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма попълнена валидна стойност в поле 'Часове'!");
                                }
                                else
                                {
                                    if (validHours < 0)
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} не може да има отрицателна стойност в поле 'Часове'!");
                                    }
                                    else if (validHours == 0)
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} не може да има стойност '0' в поле 'Часове'!");
                                    }
                                    else
                                    {
                                        if ((validHours % 1) != 0)
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} може да има стойност само цяло число в поле 'Часове'!");
                                        }
                                        else
                                        {
                                            model.Hours = validHours;
                                        }
                                    }
                                }
                            }

                            var timeFrom = row.Cells[6].Value.Trim();
                            if (!string.IsNullOrEmpty(timeFrom))
                            {
                                DateTime validTimeFrom;
                                if (!DateTime.TryParse(timeFrom, out validTimeFrom))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} не е въведена валидна стойност в поле 'Продължителност от'!");
                                }
                                else
                                {
                                    TimeSpan validTimeFromTS = new TimeSpan(7, 0, 0);
                                    TimeSpan validTimeToTS = new TimeSpan(22, 0, 0);
                                    if (validTimeFrom.TimeOfDay < validTimeFromTS)
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} не може да бъде въведена стойност по-малка от '07:00' в поле 'Продължителност от'!");
                                    }
                                    else if (validTimeFrom.TimeOfDay >= validTimeToTS)
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} не може да бъде въведена стойност по-голяма от '22:00' в поле 'Продължителност от'!");
                                    }
                                    else
                                    {
                                        model.TimeFrom = validTimeFrom;
                                    }
                                }
                            }

                            var timeTo = row.Cells[7].Value.Trim();
                            if (!string.IsNullOrEmpty(timeTo))
                            {
                                DateTime validTimeTo;
                                if (!DateTime.TryParse(timeTo, out validTimeTo))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} не е въведена валидна стойност в поле 'Продължителност до'!");
                                }
                                else
                                {
                                    TimeSpan validTimeFromTS = new TimeSpan(7, 0, 0);
                                    TimeSpan validTimeToTS = new TimeSpan(22, 0, 0);
                                    if (validTimeTo.TimeOfDay < validTimeFromTS)
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} не може да бъде въведена стойност по-малка от '07:00' в поле 'Продължителност до'!");
                                    }
                                    else if (validTimeTo.TimeOfDay >= validTimeToTS)
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} не може да бъде въведена стойност по-голяма от '22:00' в поле 'Продължителност до'!");
                                    }
                                    else
                                    {
                                        model.TimeTo = validTimeTo;
                                    }
                                }
                            }

                            if (model.TimeFrom.HasValue && model.TimeTo.HasValue)
                            {
                                if (model.TimeFrom.Value > model.TimeTo.Value)
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} стойността в поле 'Продължителност от' не може да бъде по-голяма от стойността в поле 'Продължителност до'!");
                                }
                            }

                            if (model.IdTrainingScheduleType == kvPractice.IdKeyValue)
                            {
                                var trainingCurriculumFromDb = trainingCurriculums.FirstOrDefault(x => x.IdProfessionalTraining == trainingCurriculumModel.IdProfessionalTraining
                                    && x.Subject == trainingCurriculumModel.Subject
                                    && x.Topic == trainingCurriculumModel.Topic
                                    && x.Practice.HasValue);
                                if (trainingCurriculumFromDb is null)
                                {
                                    if (trainingCurriculumModel.IdProfessionalTraining != 0)
                                    {
                                        var ptValue = professionalTrainingTypesSource.FirstOrDefault(x => x.IdKeyValue == trainingCurriculumModel.IdProfessionalTraining);
                                        resultContext.AddErrorMessage($"Не е открита информация за предмет '{trainingCurriculumModel.Subject}', тема '{trainingCurriculumModel.Topic}, вид на професионална подготовка {ptValue!.Name}, който да има вид на обучение 'Практика'!");
                                    }
                                }
                                else
                                {
                                    if (trainingCurriculumModel.IdProfessionalTraining != 0)
                                    {
                                        var ptValue = professionalTrainingTypesSource.FirstOrDefault(x => x.IdKeyValue == trainingCurriculumModel.IdProfessionalTraining);
                                        var dictKey = $"{ptValue!.Name}{trainingCurriculumModel.Subject}{trainingCurriculumModel.Topic}Практика";
                                        if (!addedCourseSchedulesDict.ContainsKey(dictKey))
                                        {
                                            addedCourseSchedulesDict.Add(dictKey, new Dictionary<double, double>());
                                        }

                                        if (!addedCourseSchedulesDict[dictKey].ContainsKey(trainingCurriculumFromDb.Practice!.Value))
                                        {
                                            addedCourseSchedulesDict[dictKey].Add(trainingCurriculumFromDb.Practice.Value, 0);
                                        }

                                        if (addedCourseSchedulesDict[dictKey][trainingCurriculumFromDb.Practice.Value] + model.Hours > addedCourseSchedulesDict[dictKey].Keys.FirstOrDefault())
                                        {
                                            resultContext.AddErrorMessage($"Не можете да добавите към дневника предмет '{trainingCurriculumModel.Subject}', тема '{trainingCurriculumModel.Topic}, вид на професионална подготовка {ptValue!.Name}, защото въведените часове 'Практика' надхвърлят общия брой налични часове!");
                                        }
                                        else
                                        {
                                            addedCourseSchedulesDict[dictKey][trainingCurriculumFromDb.Practice!.Value] += model.Hours!.Value;
                                            model.IdTrainingCurriculum = trainingCurriculumFromDb.IdTrainingCurriculum;

                                            courseScheduleList.Add(model);
                                        }
                                    }
                                }
                            }
                            else if (model.IdTrainingScheduleType == kvTheory.IdKeyValue)
                            {
                                var trainingCurriculumFromDb = trainingCurriculums.FirstOrDefault(x => x.IdProfessionalTraining == trainingCurriculumModel.IdProfessionalTraining
                                   && x.Subject == trainingCurriculumModel.Subject
                                   && x.Topic == trainingCurriculumModel.Topic
                                   && x.Theory.HasValue);
                                if (trainingCurriculumFromDb is null)
                                {
                                    if (trainingCurriculumModel.IdProfessionalTraining != 0)
                                    {
                                        var ptValue = professionalTrainingTypesSource.FirstOrDefault(x => x.IdKeyValue == trainingCurriculumModel.IdProfessionalTraining);
                                        resultContext.AddErrorMessage($"Не е открита информация за предмет '{trainingCurriculumModel.Subject}', тема '{trainingCurriculumModel.Topic}, вид на професионална подготовка {ptValue!.Name}, който да има вид на обучение 'Теория'!");
                                    }
                                }
                                else
                                {
                                    if (trainingCurriculumModel.IdProfessionalTraining != 0)
                                    {
                                        var ptValue = professionalTrainingTypesSource.FirstOrDefault(x => x.IdKeyValue == trainingCurriculumModel.IdProfessionalTraining);
                                        var dictKey = $"{ptValue!.Name}{trainingCurriculumModel.Subject}{trainingCurriculumModel.Topic}Теория";
                                        if (!addedCourseSchedulesDict.ContainsKey(dictKey))
                                        {
                                            addedCourseSchedulesDict.Add(dictKey, new Dictionary<double, double>());
                                        }

                                        if (!addedCourseSchedulesDict[dictKey].ContainsKey(trainingCurriculumFromDb.Theory!.Value))
                                        {
                                            addedCourseSchedulesDict[dictKey].Add(trainingCurriculumFromDb.Theory!.Value, 0);
                                        }

                                        if (addedCourseSchedulesDict[dictKey][trainingCurriculumFromDb.Theory!.Value] + model.Hours > addedCourseSchedulesDict[dictKey].Keys.FirstOrDefault())
                                        {
                                            resultContext.AddErrorMessage($"Не можете да добавите към дневника предмет '{trainingCurriculumModel.Subject}', тема '{trainingCurriculumModel.Topic}, вид на професионална подготовка {ptValue!.Name}, защото въведените часове 'Теория' надхвърлят общия брой налични часове!");
                                        }
                                        else
                                        {
                                            addedCourseSchedulesDict[dictKey][trainingCurriculumFromDb.Theory!.Value] += model.Hours!.Value;
                                            model.IdTrainingCurriculum = trainingCurriculumFromDb.IdTrainingCurriculum;

                                            courseScheduleList.Add(model);
                                        }
                                    }
                                }
                            }


                            rowCounter++;
                        }
                    }

                    if (courseScheduleList.Any())
                    {
                        resultContext.AddMessage("Импортът приключи успешно!");
                    }
                    else
                    {
                        resultContext.AddErrorMessage("Не може да бъде добавен празен шаблон за дневник!");
                    }

                    resultContext.ResultContextObject = courseScheduleList;
                }
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public MemoryStream CreateCourseScheduleExcelWithErrors(ResultContext<List<CourseScheduleVM>> resultContext)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 50;
                sheet.Range[$"A1"].Text = "Вид на грешките:";
                //sheet.Range[$"B1"].Text = "Позиция във файла";

                var rowCounter = 2;
                foreach (var item in resultContext.ListErrorMessages)
                {
                    //var splitMsg = item.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    //var msg = splitMsg[0].Trim();
                    //var cell = splitMsg[1].Trim();

                    //sheet.Range[$"A{rowCounter}"].Text = msg;
                    sheet.Range[$"A{rowCounter}"].Text = item;
                    //sheet.Range[$"B{rowCounter}"].Text = cell;

                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }

        public async Task CreateCourseSchedulesFromListAsync(List<CourseScheduleVM> schedules, int idCourse)
        {
            try
            {
                bool areCourseSchedulesAlreadyAdded = this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == idCourse).Include(x => x.CourseSchedules).AsNoTracking().Any(x => x.CourseSchedules.Any());
                if (areCourseSchedulesAlreadyAdded)
                {
                    var curriculums = await this.repository.AllReadonly<TrainingCurriculum>(x => x.IdCourse == idCourse).Include(x => x.CourseSchedules).AsNoTracking().ToListAsync();
                    foreach (var curriculum in curriculums)
                    {
                        if (curriculum.CourseSchedules.Any())
                        {
                            this.repository.HardDeleteRange<CourseSchedule>(curriculum.CourseSchedules);
                            await this.repository.SaveChangesAsync();
                        }
                    }
                }

                foreach (var schedule in schedules)
                {
                    var scheduleForDb = schedule.To<CourseSchedule>();
                    scheduleForDb.TrainingCurriculum = null;
                    scheduleForDb.CandidateProviderPremises = null;
                    scheduleForDb.CandidateProviderTrainer = null;

                    await this.repository.AddAsync<CourseSchedule>(scheduleForDb);
                }

                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task<ResultContext<NoResult>> DeleteListCurriculumSchedulesAsync(List<CourseScheduleVM> schedules)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var listScheduleIds = schedules.Select(x => x.IdCourseSchedule).ToList();
                var schedulesFromDb = await this.repository.AllReadonly<CourseSchedule>(x => listScheduleIds.Contains(x.IdCourseSchedule)).ToListAsync();

                this.repository.HardDeleteRange<CourseSchedule>(schedulesFromDb);
                await this.repository.SaveChangesAsync();

                var msg = schedules.Count > 1
                    ? "Записите са изтрити успешно!"
                    : "Записът е изтрит успешно!";
                resultContext.AddMessage(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        #endregion

        #region RIDPK
        public async Task<IEnumerable<RIDPKVM>> GetListRIDPKVMOfSubmittedDocumentsForControlFromCPOAsync(string type)
        {
            var ridpkList = new List<RIDPKVM>();
            var kvDocSubmittedValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Submitted");
            var vqsSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
            var formEducationsSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation");
            var trainingPeriodsSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingPeriod");
            if (type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE)
            {
                var kvSPKValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification");
                var kvCourseRegulationOneAndSeven = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "CourseRegulation1And7");
                var kvIssueOfDuplicate = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "IssueOfDuplicate");
                var clientCourseDocs = await this.repository.AllReadonly<ClientCourseDocument>(x => x.IdDocumentStatus == kvDocSubmittedValue.IdKeyValue && (x.IdDocumentType == kvSPKValue.IdKeyValue || x.IdDocumentType == kvCourseRegulationOneAndSeven.IdKeyValue || x.IdDocumentType == kvIssueOfDuplicate.IdKeyValue))
                .Include(x => x.ClientCourse)
                    .ThenInclude(x => x.Course.CandidateProvider)
                            .AsNoTracking()
                .Include(x => x.ClientCourse)
                    .ThenInclude(x => x.Course)
                .Include(x => x.ClientCourseDocumentStatuses.OrderByDescending(y => y.IdClientCourseDocumentStatus))
                    .AsNoTracking()
                .Include(x => x.ClientCourse.Course.Program.FrameworkProgram)
                    .AsNoTracking()
                .Include(x => x.ClientCourse.Course.Program.Speciality)
                .ToListAsync();

                if (clientCourseDocs.Any())
                {
                    var idCounter = 1;
                    foreach (var doc in clientCourseDocs)
                    {
                        var lastDocStatus = doc.ClientCourseDocumentStatuses.FirstOrDefault(x => x.IdClientDocumentStatus == kvDocSubmittedValue.IdKeyValue);
                        if (lastDocStatus is not null)
                        {
                            var submissionDate = lastDocStatus.CreationDate.Date;
                            if (!ridpkList.Any(x => x.SubmissionDate == submissionDate && x.Course.IdCourse == doc.ClientCourse.IdCourse))
                            {
                                var speciality = doc.ClientCourse.Course.Program.Speciality.To<SpecialityVM>();
                                var vqsValue = vqsSource.FirstOrDefault(x => x.IdKeyValue == speciality.IdVQS);
                                if (vqsValue is not null)
                                {
                                    speciality.VQS_Name = vqsValue.Name;
                                }

                                var course = doc.ClientCourse.Course.To<CourseVM>();
                                var formEducationValue = formEducationsSource.FirstOrDefault(x => x.IdKeyValue == course.IdFormEducation.Value);
                                if (formEducationValue is not null)
                                {
                                    course.FormEducationName = formEducationValue.Name;
                                }

                                var frameworkProgram = doc.ClientCourse.Course.Program.FrameworkProgram.To<FrameworkProgramVM>();
                                var trainingPeriodValue = trainingPeriodsSource.FirstOrDefault(x => x.IdKeyValue == frameworkProgram.IdTrainingPeriod);
                                if (trainingPeriodValue is not null)
                                {
                                    frameworkProgram.TrainingPeriodName = trainingPeriodValue.Name;
                                }

                                RIDPKVM model = new RIDPKVM()
                                {
                                    IdRIDPK = idCounter++,
                                    IdClientCourseDocument = doc.IdClientCourseDocument,
                                    CandidateProvider = doc.ClientCourse.Course.CandidateProvider.To<CandidateProviderVM>(),
                                    Course = course,
                                    SubmissionDate = submissionDate,
                                    FrameworkProgram = frameworkProgram,
                                    Speciality = speciality
                                };

                                ridpkList.Add(model);
                            }

                            var docFromRIDPK = ridpkList.FirstOrDefault(x => x.IdRIDPK == idCounter - 1);
                            if (docFromRIDPK is not null)
                            {
                                docFromRIDPK.SubmittedDocumentCount += 1;
                            }
                        }
                    }
                }
            }
            else
            {
                var kvIssueOfDuplicate = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "IssueOfDuplicate");
                var kvSPKValidation = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ValidationOfProfessionalQualifications");
                var validationClientDocs = await this.repository.AllReadonly<ValidationClientDocument>(x => x.IdDocumentStatus == kvDocSubmittedValue.IdKeyValue && (x.IdDocumentType == kvSPKValidation.IdKeyValue || x.IdDocumentType == kvIssueOfDuplicate.IdKeyValue))
                .Include(x => x.ValidationClient.CandidateProvider).AsNoTracking()
                .Include(x => x.ValidationClient.Speciality).AsNoTracking()
                .Include(x => x.ValidationClient.FrameworkProgram).AsNoTracking()
                .Include(x => x.ValidationClientDocumentStatuses.OrderByDescending(y => y.IdValidationClientDocumentStatus)).AsNoTracking()
                .ToListAsync();

                if (validationClientDocs.Any())
                {
                    var idCounter = 1;
                    foreach (var doc in validationClientDocs)
                    {
                        var lastDocStatus = doc.ValidationClientDocumentStatuses.FirstOrDefault(x => x.IdClientDocumentStatus == kvDocSubmittedValue.IdKeyValue);
                        if (lastDocStatus is not null)
                        {
                            var submissionDate = lastDocStatus.CreationDate.Date;
                            if (!ridpkList.Any(x => x.SubmissionDate == submissionDate && x.IdValidationClientDocument == doc.IdValidationClient))
                            {
                                var speciality = doc.ValidationClient.Speciality.To<SpecialityVM>();
                                var vqsValue = vqsSource.FirstOrDefault(x => x.IdKeyValue == speciality.IdVQS);
                                if (vqsValue is not null)
                                {
                                    speciality.VQS_Name = vqsValue.Name;
                                }

                                var frameworkProgram = doc.ValidationClient.FrameworkProgram.To<FrameworkProgramVM>();
                                var trainingPeriodValue = trainingPeriodsSource.FirstOrDefault(x => x.IdKeyValue == frameworkProgram.IdTrainingPeriod);
                                if (trainingPeriodValue is not null)
                                {
                                    frameworkProgram.TrainingPeriodName = trainingPeriodValue.Name;
                                }

                                RIDPKVM model = new RIDPKVM()
                                {
                                    IdRIDPK = idCounter++,
                                    IdValidationClientDocument = doc.IdValidationClientDocument,
                                    CandidateProvider = doc.ValidationClient.CandidateProvider.To<CandidateProviderVM>(),
                                    ValidationClient = doc.ValidationClient.To<ValidationClientVM>(),
                                    SubmissionDate = submissionDate,
                                    FrameworkProgram = frameworkProgram,
                                    Speciality = speciality
                                };

                                ridpkList.Add(model);
                            }

                            var docFromRIDPK = ridpkList.FirstOrDefault(x => x.IdRIDPK == idCounter - 1);
                            if (docFromRIDPK is not null)
                            {
                                docFromRIDPK.SubmittedDocumentCount += 1;
                            }
                        }
                    }
                }
            }

            return ridpkList.OrderByDescending(x => x.SubmissionDate).ThenBy(x => x.CandidateProvider.CPONameOwnerGrid).ToList();
        }

        public async Task<IEnumerable<RIDPKDocumentVM>> GetRIDPKDocumentsDataAsync(string type, RIDPKVM model)
        {
            var ridpkDocumentList = new List<RIDPKDocumentVM>();
            try
            {
                var documentTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType", false, true);
                var protocolTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType");
                if (type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE)
                {
                    var kvFinishedWithDocValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type1");
                    var kvDuplicateIssueValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type6");
                    var kvDocSubmittedValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Submitted");
                    var clientsFromCourse = await this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == model.Course.IdCourse && (x.IdFinishedType == kvFinishedWithDocValue.IdKeyValue || x.IdFinishedType == kvDuplicateIssueValue.IdKeyValue))
                        .Include(x => x.ClientCourseDocuments.Where(x => x.IdDocumentStatus == kvDocSubmittedValue.IdKeyValue))
                            .ThenInclude(x => x.TypeOfRequestedDocument)
                                .AsNoTracking()
                        .Include(x => x.ClientCourseDocuments.Where(x => x.IdDocumentStatus == kvDocSubmittedValue.IdKeyValue))
                            .ThenInclude(x => x.DocumentSerialNumber)
                                .AsNoTracking()
                        .Include(x => x.ClientRequiredDocuments)
                            .AsNoTracking()
                        .Include(x => x.CourseProtocolGrades)
                            .ThenInclude(x => x.CourseProtocol)
                                .AsNoTracking()
                                .ToListAsync();

                    foreach (var client in clientsFromCourse)
                    {
                        foreach (var doc in client.ClientCourseDocuments)
                        {
                            var docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == doc.IdTypeOfRequestedDocument && x.Year == doc.DocumentSerialNumber.ReceiveDocumentYear).FirstOrDefaultAsync();
                            var kvDocumentStatusValue = await this.dataSourceService.GetKeyValueByIdAsync(doc.IdDocumentStatus);
                            RIDPKDocumentVM rIDPKDocumentVM = new RIDPKDocumentVM()
                            {
                                IdEntity = doc.IdClientCourseDocument,
                                IdClientCourse = client.IdClientCourse,
                                DocumentRegNo = doc.DocumentRegNo!,
                                DocumentDate = $"{doc.DocumentDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.",
                                DocumentSerialNumber = $"{docSeries?.SeriesName}/{doc.DocumentSerialNumber.SerialNumber}",
                                DocumentTypeName = doc.TypeOfRequestedDocument.DocTypeName,
                                ClientFirstName = client.FirstName,
                                ClientSecondName = client.SecondName,
                                ClientFamilyName = client.FamilyName,
                                ClientDocuments = this.GetClientUploadedDocuments(client.ClientRequiredDocuments, documentTypesSource),
                                CourseProtocolsWithGrades = this.GetCourseProtocolsWithGrades(client.CourseProtocolGrades, protocolTypesSource)
                            };

                            if (!rIDPKDocumentVM.CourseProtocolsWithGrades.Any())
                            {
                                var theoryGrade = doc.TheoryResult.HasValue 
                                    ? $"Теория: {doc.TheoryResult.Value.ToString("f2")}"
                                    : string.Empty;
                                rIDPKDocumentVM.GradesFromOldIS.Add(theoryGrade);

                                var practiceResult = doc.PracticeResult.HasValue
                                    ? $"Практика: {doc.PracticeResult.Value.ToString("f2")}"
                                    : string.Empty;
                                rIDPKDocumentVM.GradesFromOldIS.Add(practiceResult);

                                var finalResult = doc.FinalResult.HasValue
                                    ? $"Обща оценка: {doc.FinalResult.Value.ToString("f2")}"
                                    : string.Empty;
                                rIDPKDocumentVM.GradesFromOldIS.Add(finalResult);
                            }

                            ridpkDocumentList.Add(rIDPKDocumentVM);
                        }
                    }
                }
                else
                {
                    var kvFinishedWithDocValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type5");
                    var kvDuplicateIssueValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type6");
                    var kvDocSubmittedValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Submitted");
                    var validationClient = await this.repository.AllReadonly<ValidationClient>(x => x.IdValidationClient == model.ValidationClient.IdValidationClient && (x.IdFinishedType == kvFinishedWithDocValue.IdKeyValue || x.IdFinishedType == kvDuplicateIssueValue.IdKeyValue))
                        .Include(x => x.ValidationClientDocuments.Where(x => x.IdDocumentStatus == kvDocSubmittedValue.IdKeyValue))
                        .ThenInclude(x => x.TypeOfRequestedDocument)
                            .AsNoTracking()
                        .Include(x => x.ValidationClientDocuments.Where(x => x.IdDocumentStatus == kvDocSubmittedValue.IdKeyValue))
                            .ThenInclude(x => x.DocumentSerialNumber)
                                .AsNoTracking()
                        .Include(x => x.ValidationClientRequiredDocuments)
                            .AsNoTracking()
                        .Include(x => x.ValidationProtocolGrades.Where(y => y.Grade != null))
                            .ThenInclude(x => x.ValidationProtocol)
                                .AsNoTracking()
                                .ToListAsync();

                    foreach (var client in validationClient)
                    {
                        foreach (var doc in client.ValidationClientDocuments)
                        {
                            DocumentSeries docSeries = null;
                            if (doc.DocumentSerialNumber is not null)
                            {
                                docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == doc.IdTypeOfRequestedDocument && x.Year == doc.DocumentSerialNumber.ReceiveDocumentYear).FirstOrDefaultAsync();
                            }

                            var kvDocumentStatusValue = await this.dataSourceService.GetKeyValueByIdAsync(doc.IdDocumentStatus);
                            RIDPKDocumentVM rIDPKDocumentVM = new RIDPKDocumentVM()
                            {
                                IdEntity = doc.IdValidationClientDocument,
                                DocumentRegNo = doc.DocumentRegNo!,
                                DocumentDate = $"{doc.DocumentDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.",
                                DocumentSerialNumber = $"{docSeries?.SeriesName}/{doc.DocumentSerialNumber?.SerialNumber}",
                                DocumentTypeName = doc.TypeOfRequestedDocument is not null ? doc.TypeOfRequestedDocument.DocTypeName : string.Empty,
                                ClientFirstName = client.FirstName,
                                ClientSecondName = client.SecondName,
                                ClientFamilyName = client.FamilyName,
                                ClientDocuments = this.GetValidationClientUploadedDocuments(client.ValidationClientRequiredDocuments, documentTypesSource),
                                CourseProtocolsWithGrades = this.GetValidationProtocolsWithGrades(client.ValidationProtocolGrades, protocolTypesSource)
                            };

                            if (!rIDPKDocumentVM.CourseProtocolsWithGrades.Any())
                            {
                                var theoryGrade = doc.TheoryResult.HasValue
                                    ? $"Теория: {doc.TheoryResult.Value.ToString("f2")}"
                                    : string.Empty;
                                rIDPKDocumentVM.GradesFromOldIS.Add(theoryGrade);

                                var practiceResult = doc.PracticeResult.HasValue
                                    ? $"Практика: {doc.PracticeResult.Value.ToString("f2")}"
                                    : string.Empty;
                                rIDPKDocumentVM.GradesFromOldIS.Add(practiceResult);

                                var finalResult = doc.FinalResult.HasValue
                                    ? $"Обща оценка: {doc.FinalResult.Value.ToString("f2")}"
                                    : string.Empty;
                                rIDPKDocumentVM.GradesFromOldIS.Add(finalResult);
                            }

                            ridpkDocumentList.Add(rIDPKDocumentVM);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return ridpkDocumentList.OrderBy(x => x.DocumentDate).ThenBy(x => x.ClientFirstName).ThenBy(x => x.ClientFamilyName).ToList();
        }

        public async Task<bool> IsRIDPKDocumentAlreadyReturnedAsync(RIDPKDocumentVM document)
        {
            var kvDocStatusReturnedValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Returned");
            if (document.IdClientCourse != 0)
            {
                return (await this.repository.AllReadonly<ClientCourseDocumentStatus>(x => x.IdClientCourseDocument == document.IdEntity).ToListAsync())
                    .Any(x => x.IdClientDocumentStatus == kvDocStatusReturnedValue.IdKeyValue);
            }
            else
            {
                return (await this.repository.AllReadonly<ValidationClientDocumentStatus>(x => x.IdValidationClientDocument == document.IdEntity).ToListAsync())
                    .Any(x => x.IdClientDocumentStatus == kvDocStatusReturnedValue.IdKeyValue);
            }
        }

        public async Task<ResultContext<NoResult>> ApproveRIDPKDocumentsAsync(string type, List<RIDPKDocumentVM> documents, string? comment)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var kvDocumentStatusApproved = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "EnteredInTheRegister");
                foreach (var doc in documents)
                {
                    if (type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE)
                    {
                        await this.UpdateClientCourseDocumentStatusAsync(doc.IdEntity, kvDocumentStatusApproved.IdKeyValue);

                        await this.AddClientCourseDocumentStatusAsync(doc.IdEntity, kvDocumentStatusApproved.IdKeyValue, comment);
                    }
                    else
                    {
                        await this.UpdateValidationClientDocumentStatusAsync(doc.IdEntity, kvDocumentStatusApproved.IdKeyValue);

                        await this.AddValidationClientDocumentStatusAsync(doc.IdEntity, kvDocumentStatusApproved.IdKeyValue, comment);
                    }
                }

                var cmt = !string.IsNullOrEmpty(comment) ? comment : "няма";
                await this.SetDataForNotificationAsync(documents, cmt, "РИДПК - вписан в Регистъра");

                var msg = documents.Count == 1 ? "Документът е вписан успешно в регистъра!" : "Документите са успешно вписани в регистъра!";
                resultContext.AddMessage(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> ReturnRIDPKDocumentsAsync(string type, List<RIDPKDocumentVM> documents, string? comment)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var kvDocumentStatusReturned = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Returned");
                foreach (var doc in documents)
                {
                    if (type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE)
                    {
                        await this.UpdateClientCourseDocumentStatusAsync(doc.IdEntity, kvDocumentStatusReturned.IdKeyValue);

                        await this.AddClientCourseDocumentStatusAsync(doc.IdEntity, kvDocumentStatusReturned.IdKeyValue, comment);
                    }
                    else
                    {
                        await this.UpdateValidationClientDocumentStatusAsync(doc.IdEntity, kvDocumentStatusReturned.IdKeyValue);

                        await this.AddValidationClientDocumentStatusAsync(doc.IdEntity, kvDocumentStatusReturned.IdKeyValue, comment);
                    }
                }

                var cmt = !string.IsNullOrEmpty(comment) ? comment : "няма";
                await this.SetDataForNotificationAsync(documents, cmt, "РИДПК - връщане за корекция");

                var msg = documents.Count == 1 ? "Документът е върнат за корекция към ЦПО!" : "Документите са върнати за корекция към ЦПО!";
                resultContext.AddMessage(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> RejectRIDPKDocumentsAsync(string type, List<RIDPKDocumentVM> documents, string? comment)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var kvDocumentStatusRejected = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Rejected");
                foreach (var doc in documents)
                {
                    if (type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE)
                    {
                        await this.UpdateClientCourseDocumentStatusAsync(doc.IdEntity, kvDocumentStatusRejected.IdKeyValue);

                        await this.AddClientCourseDocumentStatusAsync(doc.IdEntity, kvDocumentStatusRejected.IdKeyValue, comment);
                    }
                    else
                    {
                        await this.UpdateValidationClientDocumentStatusAsync(doc.IdEntity, kvDocumentStatusRejected.IdKeyValue);

                        await this.AddValidationClientDocumentStatusAsync(doc.IdEntity, kvDocumentStatusRejected.IdKeyValue, comment);
                    }
                }

                var cmt = !string.IsNullOrEmpty(comment) ? comment : "няма";
                await this.SetDataForNotificationAsync(documents, cmt, "РИДПК - отказан за вписване в регистъра");

                var msg = documents.Count == 1 ? "Документът е отказан за вписване в регистъра!" : "Документите са отказани за вписване в регистъра!";
                resultContext.AddMessage(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<IEnumerable<CourseDocumentUploadedFileVM>> GetCourseDocumentUploadedFileByIdClientCourseDocumentAsync(int idClientCourseDocument)
        {
            return await this.repository.AllReadonly<CourseDocumentUploadedFile>(x => x.IdClientCourseDocument == idClientCourseDocument).To<CourseDocumentUploadedFileVM>().ToListAsync();
        }

        public async Task<IEnumerable<ValidationDocumentUploadedFileVM>> GetValidationDocumentUploadedFileByIdValidationClientDocumentAsync(int idValidationClientDocument)
        {
            return await this.repository.AllReadonly<ValidationDocumentUploadedFile>(x => x.IdValidationClientDocument == idValidationClientDocument).To<ValidationDocumentUploadedFileVM>().ToListAsync();
        }

        public async Task<IEnumerable<DocumentStatusVM>> GetDocumentStatusesByIdAsync(int idEntity, string type)
        {
            var documentStatusList = new List<DocumentStatusVM>();
            var kvDocStatusesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientDocumentStatusType");
            if (type == "Course")
            {
                var clientCourseDocumentStatuses = await this.repository.AllReadonly<ClientCourseDocumentStatus>(x => x.IdClientCourseDocument == idEntity).ToListAsync();
                var idCounter = 1;
                foreach (var status in clientCourseDocumentStatuses.OrderBy(x => x.CreationDate))
                {
                    var kvStatus = kvDocStatusesSource.FirstOrDefault(x => x.IdKeyValue == status.IdClientDocumentStatus);
                    DocumentStatusVM model = new DocumentStatusVM()
                    {
                        Id = idCounter++,
                        StatusValue = kvStatus is not null ? kvStatus.Name : string.Empty,
                        StatusDate = $"{status.CreationDate.ToString(GlobalConstants.DATE_FORMAT)} г. {status.CreationDate.ToString("HH:mm")} ч.",
                        PersonName = await this.applicationUserService.GetApplicationUsersPersonNameAsync(status.IdCreateUser),
                        StatusComment = status.SubmissionComment
                    };

                    documentStatusList.Add(model);
                }
            }
            else
            {
                var validationClientDocumentStatuses = await this.repository.AllReadonly<ValidationClientDocumentStatus>(x => x.IdValidationClientDocument == idEntity).ToListAsync();
                var idCounter = 1;
                foreach (var status in validationClientDocumentStatuses.OrderBy(x => x.CreationDate))
                {
                    var kvStatus = kvDocStatusesSource.FirstOrDefault(x => x.IdKeyValue == status.IdClientDocumentStatus);
                    DocumentStatusVM model = new DocumentStatusVM()
                    {
                        Id = idCounter++,
                        StatusValue = kvStatus is not null ? kvStatus.Name : string.Empty,
                        StatusDate = $"{status.CreationDate.ToString(GlobalConstants.DATE_FORMAT)} г. {status.CreationDate.ToString("HH:mm")} ч.",
                        PersonName = await this.applicationUserService.GetApplicationUsersPersonNameAsync(status.IdCreateUser),
                        StatusComment = status.SubmissionComment
                    };

                    documentStatusList.Add(model);
                }
            }

            return documentStatusList;
        }

        public async Task<ResultContext<NoResult>> ChangeRIDPKStatusForListClientCourseDocumentIdsAsync(List<int> documentIds, int idDocumentStatus, string? comment)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                foreach (var id in documentIds)
                {
                    var docFromDb = await this.repository.GetByIdAsync<ClientCourseDocument>(id);
                    if (docFromDb is not null)
                    {
                        docFromDb.IdDocumentStatus = idDocumentStatus;

                        this.repository.Update<ClientCourseDocument>(docFromDb);
                        await this.repository.SaveChangesAsync();

                        await this.AddClientCourseDocumentStatusAsync(docFromDb.IdClientCourseDocument, idDocumentStatus, comment);
                    }
                }

                var msg = documentIds.Count == 1
                    ? "Статусът на документа е променен успешно!"
                    : "Статусите на документите са променени успешно!";

                resultContext.AddMessage(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> ChangeRIDPKStatusForListValidationClientDocumentIdsAsync(List<int> documentIds, int idDocumentStatus, string? comment)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                foreach (var id in documentIds)
                {
                    var docFromDb = await this.repository.GetByIdAsync<ValidationClientDocument>(id);
                    if (docFromDb is not null)
                    {
                        docFromDb.IdDocumentStatus = idDocumentStatus;

                        this.repository.Update<ValidationClientDocument>(docFromDb);
                        await this.repository.SaveChangesAsync();

                        await this.AddValidationClientDocumentStatusAsync(docFromDb.IdValidationClientDocument, idDocumentStatus, comment);
                    }
                }

                var msg = documentIds.Count == 1
                    ? "Статусът на документа е променен успешно!"
                    : "Статусите на документите са променени успешно!";

                resultContext.AddMessage(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        private List<KeyValuePair<int, string>> GetCourseProtocolsWithGrades(ICollection<CourseProtocolGrade> protocolGrades, IEnumerable<KeyValueVM> protocolTypesSource)
        {
            var listKVP = new List<KeyValuePair<int, string>>();
            foreach (var protocolGrade in protocolGrades)
            {
                if (protocolGrade.CourseProtocol is not null)
                {
                    var protocolTypeValue = protocolTypesSource.FirstOrDefault(x => x.IdKeyValue == protocolGrade.CourseProtocol.IdCourseProtocolType);
                    if (protocolTypeValue is not null)
                    {
                        var alreadyAddedProtocolType = listKVP.FirstOrDefault(x => x.Value == protocolTypeValue.Name);
                        if (alreadyAddedProtocolType.Key != 0)
                        {
                            listKVP.Remove(alreadyAddedProtocolType);
                        }

                        listKVP.Add(new KeyValuePair<int, string>(protocolGrade.CourseProtocol.IdCourseProtocol, protocolTypeValue.Name));
                        if (protocolGrade.Grade.HasValue)
                        {
                            listKVP.Remove(listKVP.LastOrDefault());
                            listKVP.Add(new KeyValuePair<int, string>(protocolGrade.CourseProtocol.IdCourseProtocol, $"{protocolTypeValue.Name} - {protocolGrade.Grade!.Value.ToString("f2")}"));
                        }
                    }
                }
            }

            return listKVP;
        }

        private List<KeyValuePair<int, List<string>>> GetClientUploadedDocuments(ICollection<ClientRequiredDocument> documents, IEnumerable<KeyValueVM> documentTypesSource)
        {
            var listKVP = new List<KeyValuePair<int, List<string>>>();
            var docAsVMList = new List<ClientRequiredDocumentVM>();
            foreach (var doc in documents)
            {
                var docAsVM = doc.To<ClientRequiredDocumentVM>();
                var docTypeValue = documentTypesSource.FirstOrDefault(x => x.IdKeyValue == doc.IdCourseRequiredDocumentType);
                if (docTypeValue is not null && !string.IsNullOrEmpty(docTypeValue.DefaultValue4))
                {
                    docAsVM.Order = docTypeValue.Order;
                }

                docAsVMList.Add(docAsVM);
            }

            foreach (var doc in docAsVMList.OrderBy(x => x.Order))
            {
                var docTypeValue = documentTypesSource.FirstOrDefault(x => x.IdKeyValue == doc.IdCourseRequiredDocumentType);
                if (docTypeValue is not null && !string.IsNullOrEmpty(docTypeValue.DefaultValue4))
                {
                    listKVP.Add(new KeyValuePair<int, List<string>>(doc.IdClientRequiredDocument, new List<string>() { docTypeValue.Name }));
                    if (!string.IsNullOrEmpty(doc.Description))
                    {
                        var lastAddedKVP = listKVP.LastOrDefault();
                        listKVP.Remove(lastAddedKVP);
                        listKVP.Add(new KeyValuePair<int, List<string>>(doc.IdClientRequiredDocument, new List<string>() { docTypeValue.Name, doc.Description }));
                    }
                }
            }

            return listKVP;
        }

        private List<KeyValuePair<int, string>> GetValidationProtocolsWithGrades(ICollection<ValidationProtocolGrade> protocolGrades, IEnumerable<KeyValueVM> protocolTypesSource)
        {
            var listKVP = new List<KeyValuePair<int, string>>();
            foreach (var protocolGrade in protocolGrades)
            {
                if (protocolGrade.ValidationProtocol is not null)
                {
                    var protocolTypeValue = protocolTypesSource.FirstOrDefault(x => x.IdKeyValue == protocolGrade.ValidationProtocol.IdValidationProtocolType);
                    if (protocolTypeValue is not null)
                    {
                        var alreadyAddedProtocolType = listKVP.FirstOrDefault(x => x.Value == protocolTypeValue.Name);
                        if (alreadyAddedProtocolType.Key != 0)
                        {
                            listKVP.Remove(alreadyAddedProtocolType);
                        }

                        listKVP.Add(new KeyValuePair<int, string>(protocolGrade.ValidationProtocol.IdValidationProtocol, protocolTypeValue.Name));
                        if (protocolGrade.Grade.HasValue)
                        {
                            listKVP.Remove(listKVP.LastOrDefault());
                            listKVP.Add(new KeyValuePair<int, string>(protocolGrade.ValidationProtocol.IdValidationProtocol, $"{protocolTypeValue.Name} - {protocolGrade.Grade!.Value.ToString("f2")}"));
                        }
                    }
                }
            }

            return listKVP;
        }

        private List<KeyValuePair<int, List<string>>> GetValidationClientUploadedDocuments(ICollection<ValidationClientRequiredDocument> documents, IEnumerable<KeyValueVM> documentTypesSource)
        {
            var listKVP = new List<KeyValuePair<int, List<string>>>();
            var docAsVMList = new List<ValidationClientRequiredDocumentVM>();
            foreach (var doc in documents)
            {
                var docAsVM = doc.To<ValidationClientRequiredDocumentVM>();
                var docTypeValue = documentTypesSource.FirstOrDefault(x => x.IdKeyValue == doc.IdCourseRequiredDocumentType);
                if (docTypeValue is not null && !string.IsNullOrEmpty(docTypeValue.DefaultValue4))
                {
                    docAsVM.Order = docTypeValue.Order;
                }

                docAsVMList.Add(docAsVM);
            }

            foreach (var doc in docAsVMList.OrderBy(x => x.Order))
            {
                var docTypeValue = documentTypesSource.FirstOrDefault(x => x.IdKeyValue == doc.IdCourseRequiredDocumentType);
                if (docTypeValue is not null && !string.IsNullOrEmpty(docTypeValue.DefaultValue4))
                {
                    listKVP.Add(new KeyValuePair<int, List<string>>(doc.IdValidationClientRequiredDocument, new List<string>() { docTypeValue.Name }));
                    if (!string.IsNullOrEmpty(doc.Description))
                    {
                        var lastAddedKVP = listKVP.LastOrDefault();
                        listKVP.Remove(lastAddedKVP);
                        listKVP.Add(new KeyValuePair<int, List<string>>(doc.IdValidationClientRequiredDocument, new List<string>() { docTypeValue.Name, doc.Description }));
                    }
                }
            }

            return listKVP;
        }

        private List<KeyValuePair<int, string>> GetCourseProtocolsWithGradesVM(ICollection<CourseProtocolGradeVM> protocolGrades, IEnumerable<KeyValueVM> protocolTypesSource)
        {
            var listKVP = new List<KeyValuePair<int, string>>();
            foreach (var protocolGrade in protocolGrades)
            {
                if (protocolGrade.CourseProtocol is not null)
                {
                    var protocolTypeValue = protocolTypesSource.FirstOrDefault(x => x.IdKeyValue == protocolGrade.CourseProtocol.IdCourseProtocolType);
                    if (protocolTypeValue is not null)
                    {
                        listKVP.Add(new KeyValuePair<int, string>(protocolGrade.CourseProtocol.IdCourseProtocol, protocolTypeValue.Name));
                        if (protocolGrade.Grade.HasValue)
                        {
                            listKVP.Remove(listKVP.LastOrDefault());
                            listKVP.Add(new KeyValuePair<int, string>(protocolGrade.CourseProtocol.IdCourseProtocol, $"{protocolTypeValue.Name} - {protocolGrade.Grade!.Value.ToString("f2")}"));
                        }
                    }
                }
            }

            return listKVP;
        }

        private List<KeyValuePair<int, List<string>>> GetClientUploadedDocumentsVM(ICollection<ClientRequiredDocumentVM> documents, IEnumerable<KeyValueVM> documentTypesSource)
        {
            var listKVP = new List<KeyValuePair<int, List<string>>>();
            foreach (var doc in documents)
            {
                var docTypeValue = documentTypesSource.FirstOrDefault(x => x.IdKeyValue == doc.IdCourseRequiredDocumentType);
                if (docTypeValue is not null)
                {
                    doc.Order = docTypeValue.Order;
                }
            }

            foreach (var doc in documents.OrderBy(x => x.Order))
            {
                var docTypeValue = documentTypesSource.FirstOrDefault(x => x.IdKeyValue == doc.IdCourseRequiredDocumentType);
                if (docTypeValue is not null)
                {
                    listKVP.Add(new KeyValuePair<int, List<string>>(doc.IdClientRequiredDocument, new List<string>() { docTypeValue.Name }));
                    if (!string.IsNullOrEmpty(doc.Description))
                    {
                        var lastAddedKVP = listKVP.LastOrDefault();
                        listKVP.Remove(lastAddedKVP);
                        listKVP.Add(new KeyValuePair<int, List<string>>(doc.IdClientRequiredDocument, new List<string>() { docTypeValue.Name, doc.Description }));
                    }
                }
            }

            return listKVP;
        }

        private List<KeyValuePair<int, List<string>>> GetTrainerUploadedDocumentsVM(ICollection<CandidateProviderTrainerDocumentVM> documents, IEnumerable<KeyValueVM> documentTypesSource)
        {
            var kvDocTypeMigratedFromOldIS = documentTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "MigratedTrainerDocumentType");
            var listKVP = new List<KeyValuePair<int, List<string>>>();
            foreach (var doc in documents)
            {
                var docTypeValue = documentTypesSource.FirstOrDefault(x => x.IdKeyValue == doc.IdDocumentType);
                if (docTypeValue is not null)
                {
                    if (docTypeValue.IdKeyValue == kvDocTypeMigratedFromOldIS!.IdKeyValue)
                    {
                        listKVP.Add(new KeyValuePair<int, List<string>>(doc.IdCandidateProviderTrainerDocument, new List<string>() { doc.DocumentTitle }));
                        if (!string.IsNullOrEmpty(doc.DocumentTitle))
                        {
                            var lastAddedKVP = listKVP.LastOrDefault();
                            listKVP.Remove(lastAddedKVP);
                            listKVP.Add(new KeyValuePair<int, List<string>>(doc.IdCandidateProviderTrainerDocument, new List<string>() { doc.DocumentTitle, docTypeValue.Name }));
                        }
                    }
                    else
                    {
                        listKVP.Add(new KeyValuePair<int, List<string>>(doc.IdCandidateProviderTrainerDocument, new List<string>() { docTypeValue.Name }));
                        if (!string.IsNullOrEmpty(doc.DocumentTitle))
                        {
                            var lastAddedKVP = listKVP.LastOrDefault();
                            listKVP.Remove(lastAddedKVP);
                            listKVP.Add(new KeyValuePair<int, List<string>>(doc.IdCandidateProviderTrainerDocument, new List<string>() { docTypeValue.Name, doc.DocumentTitle }));
                        }
                    }
                }
            }

            return listKVP;
        }

        private string GetAgeFromBirthDate(DateTime birthDate)
        {
            var days = (DateTime.Now - birthDate).TotalDays;
            var years = Math.Floor(days / 365);

            return $"{years} г.";
        }

        private async Task SetDataForNotificationAsync(List<RIDPKDocumentVM> documents, string? comment, string about)
        {
            var document = documents.FirstOrDefault();
            var clientCourse = (await this.repository.AllReadonly<ClientCourse>(x => x.IdClientCourse == document!.IdClientCourse)
                .Include(x => x.Course)
                    .AsNoTracking()
                .FirstOrDefaultAsync());
            if (clientCourse is not null)
            {
                var persons = await this.candidateProviderService.GetAllCandidateProviderPersonsAllowedForNotificationsByIdCandidateProviderAsync(clientCourse.Course.IdCandidateProvider!.Value);
                var listPersonIds = persons.Select(y => y.IdPerson).ToList();
                listPersonIds.Add(this.UserProps.IdPerson);
                var expert = await this.repository.AllReadonly<Expert>(x => x.IdPerson == this.UserProps.IdPerson)
                    .Include(x => x.Person)
                        .AsNoTracking()
                    .FirstOrDefaultAsync();
                if (expert is not null)
                {
                    var napooExpert = await this.repository.AllReadonly<ExpertNapoo>(x => x.IdExpert == expert.IdExpert).FirstOrDefaultAsync();
                    if (napooExpert is not null)
                    {
                        string message = string.Empty;
                        if (about.Contains("корекция"))
                        {
                            var period = (await this.dataSourceService.GetSettingByIntCodeAsync("RIDPKCorrectionPeriod")).SettingValue;
                            if (!string.IsNullOrEmpty(period))
                            {
                                var periodAsInt = int.Parse(period);
                                message = $"Подаването на документи за вписване в РИДПК за курс {clientCourse.Course.CourseName}, завършил на {clientCourse.Course.EndDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г. бе обработено с резултат РИДПК - Върнат.\r\nКоментар за резултата от обработката: {comment} .\r\n\r\nВ срок до {periodAsInt} дни от датата на връщане на документите, следва да бъдат отстранени констатираните технически несъответствия и да подадете документите в ИС на НАПОО за втора проверка и вписване в регистъра. При неспазване на срока документите не се вписват в регистъра.\r\n{expert.Person.FirstName} {expert.Person.FamilyName}, {napooExpert.Occupation}, {expert.Person.Phone}";
                            }
                        }
                        else if (about.Contains("вписан"))
                        {
                            message = $"Подаването на документи за вписване в РИДПК за курс {clientCourse.Course.CourseName}, завършил на {clientCourse.Course.EndDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г. бе обработено с резултат РИДПК - Вписан в Регистъра.\r\nКоментар за резултата от обработката: {comment}";
                        }
                        else
                        {
                            message = $"Подаването на документи за вписване в РИДПК за курс {clientCourse.Course.CourseName}, завършил на {clientCourse.Course.EndDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г. бе обработено с резултат РИДПК - Отказан.\r\nКоментар за резултата от обработката: {comment}.\r\n{expert.Person.FirstName} {expert.Person.FamilyName}, {napooExpert.Occupation}, {expert.Person.Phone}";
                        }

                        if (!string.IsNullOrEmpty(message))
                        {
                            await this.notificationService.CreateNotificationsByIdPersonFromAsync(listPersonIds, this.UserProps.IdPerson, about, message);
                        }
                    }
                }
            }
        }
        #endregion

        #region Reports

        public async Task<List<ClientCourseVM>> GetAllTrainedPeopleFilterAsync(TrainedPeopleFilterVM filterModel)
        {
            List<ClientCourseVM> clients = new List<ClientCourseVM>();
            try
            {
                var allowedCourseTypeIds = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram"))
                    .Where(x => x.KeyValueIntCode == "ProfessionalQualification" || x.KeyValueIntCode == "CourseRegulation1And7" || x.KeyValueIntCode == "PartProfession")
                    .Select(x => x.IdKeyValue)
                    .ToList();

                var filter = PredicateBuilder.True<ClientCourseDocument>();

                filter = filter.And(x => x.ClientCourse.Course.IdTrainingCourseType.HasValue && allowedCourseTypeIds.Contains(x.ClientCourse.Course.IdTrainingCourseType.Value));

                if (filterModel.IdCandidateProvider.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdCandidateProvider == filterModel.IdCandidateProvider.Value);
                }

                if (!string.IsNullOrEmpty(filterModel.LicenceNumber))
                {
                    filter = filter.And(x => !string.IsNullOrEmpty(x.ClientCourse.Course.CandidateProvider.LicenceNumber));
                    filter = filter.And(x => x.ClientCourse.Course.CandidateProvider.LicenceNumber!.Contains(filterModel.LicenceNumber.Trim()));
                }

                if (filterModel.IdCourseLocation.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdLocation.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.IdLocation == filterModel.IdCourseLocation);
                }

                if (filterModel.IdCourseDistrict.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdLocation.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.Location.Municipality.idDistrict == filterModel.IdCourseDistrict);
                }

                if (filterModel.IdCourseMunicipality.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdLocation.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.Location.Municipality.idDistrict == filterModel.IdCourseDistrict);
                }

                if (!string.IsNullOrEmpty(filterModel.FirstName))
                {
                    filter = filter.And(x => x.ClientCourse.FirstName.ToLower().Contains(filterModel.FirstName.Trim().ToLower()));
                }

                if (!string.IsNullOrEmpty(filterModel.FamilyName))
                {
                    filter = filter.And(x => x.ClientCourse.FamilyName.ToLower().Contains(filterModel.FamilyName.Trim().ToLower()));
                }

                if (!string.IsNullOrEmpty(filterModel.Indent))
                {
                    filter = filter.And(x => !string.IsNullOrEmpty(x.ClientCourse.Indent));
                    filter = filter.And(x => x.ClientCourse.Indent!.Contains(filterModel.Indent.Trim()));
                }

                if (filterModel.IdNationality.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.IdNationality.HasValue);
                    filter = filter.And(x => x.ClientCourse.IdNationality == filterModel.IdNationality);
                }

                if (filterModel.IdSex.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.IdSex.HasValue);
                    filter = filter.And(x => x.ClientCourse.IdSex == filterModel.IdSex);
                }

                if (filterModel.IdMeasureType.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdMeasureType.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.IdMeasureType == filterModel.IdMeasureType);
                }

                if (!string.IsNullOrEmpty(filterModel.CourseName))
                {
                    filter = filter.And(x => x.ClientCourse.Course.CourseName.ToLower().Contains(filterModel.CourseName.Trim().ToLower()));
                }

                if (filterModel.IdProfession.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdProgram.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.Program.IdSpeciality != 0);
                    filter = filter.And(x => x.ClientCourse.Course.Program.Speciality.IdProfession == filterModel.IdProfession);
                }

                if (filterModel.IdSpeciality.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdProgram.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.Program.IdSpeciality == filterModel.IdSpeciality);
                }

                if (filterModel.CourseStartFrom.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.StartDate.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.StartDate!.Value.Date >= filterModel.CourseStartFrom.Value.Date);
                }

                if (filterModel.CourseStartTo.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.StartDate.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.StartDate!.Value.Date <= filterModel.CourseStartTo.Value.Date);
                }

                if (filterModel.CourseEndFrom.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.EndDate.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.EndDate!.Value.Date >= filterModel.CourseEndFrom.Value.Date);
                }

                if (filterModel.CourseEndTo.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.EndDate.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.EndDate!.Value.Date <= filterModel.CourseEndTo.Value.Date);
                }

                if (!string.IsNullOrEmpty(filterModel.DocumentRegNo))
                {
                    filter = filter.And(x => !string.IsNullOrEmpty(filterModel.DocumentRegNo));
                    filter = filter.And(x => x.DocumentRegNo!.ToLower().Contains(filterModel.DocumentRegNo.Trim().ToLower()));
                }

                if (filterModel.IdTypeOfRequestedDocument.HasValue)
                {
                    filter = filter.And(x => x.IdTypeOfRequestedDocument.HasValue);
                    filter = filter.And(x => x.IdTypeOfRequestedDocument == filterModel.IdTypeOfRequestedDocument);
                }

                if (filterModel.DocumentDateFrom.HasValue)
                {
                    filter = filter.And(x => x.DocumentDate.HasValue);
                    filter = filter.And(x => x.DocumentDate!.Value.Date >= filterModel.DocumentDateFrom.Value.Date);
                }

                if (filterModel.DocumentDateTo.HasValue)
                {
                    filter = filter.And(x => x.DocumentDate.HasValue);
                    filter = filter.And(x => x.DocumentDate!.Value.Date <= filterModel.DocumentDateTo.Value.Date);
                }

                var clientCourseDocuments = await this.repository.AllReadonly<ClientCourseDocument>(filter)
                    .To<ClientCourseDocumentVM>(x => x.ClientCourse.Course.Program.Speciality.Profession,
                        x => x.ClientCourse.Course.CandidateProvider,
                        x => x.ClientCourse.Course.Location)
                    .ToListAsync();

                clients = clientCourseDocuments.DistinctBy(x => x.IdClientCourse).Select(x => x.ClientCourse).ToList();
                var courseTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram", false, true);
                foreach (var client in clients)
                {
                    if (client.Course.IdTrainingCourseType.HasValue)
                    {
                        client.Course.TrainingCourseTypeName = courseTypesSource.FirstOrDefault(x => x.IdKeyValue == client.Course.IdTrainingCourseType.Value)?.Name ?? string.Empty;
                    }
                }
            }
            catch (Exception ex) { }

            return clients.OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ToList();
        }

        public async Task<List<CourseVM>> GetAllCoursesAsync(CoursesFilterVM filterModel)
        {
            try
            {
                //if (filterModel.IdCandidateProvider != 0 || !string.IsNullOrEmpty(filterModel.LicenceNumber))
                //{
                //    var filterCP = PredicateBuilder.True<CandidateProvider>();
                //    if (filterModel.IdCandidateProvider != 0)
                //    {
                //        filterCP.And(x => x.IdCandidate_Provider == filterModel.IdCandidateProvider);
                //    }

                //    if (!string.IsNullOrEmpty(filterModel.LicenceNumber))
                //    {
                //        filterCP.And(x => x.LicenceNumber == filterModel.LicenceNumber.Trim());
                //    }

                //    var candidateProviders = await this.repository.AllReadonly<CandidateProvider>(filterCP).To<CandidateProviderVM>(x => x.Courses).ToListAsync();
                //    candidateProviders.ForEach(x =>
                //    {
                //        courses.AddRange(x.Courses);
                //    });
                //}

                var filterCourses = PredicateBuilder.True<Course>();
                if (filterModel.IdCandidateProvider != 0)
                {
                    filterCourses = filterCourses.And(x => x.CandidateProvider.IdCandidate_Provider == filterModel.IdCandidateProvider);
                }

                if (!string.IsNullOrEmpty(filterModel.LicenceNumber))
                {
                    filterCourses = filterCourses.And(x => x.CandidateProvider.LicenceNumber == filterModel.LicenceNumber.Trim());
                }

                if (filterModel.IdLocation != 0)
                {
                    filterCourses = filterCourses.And(x => x.IdLocation == filterModel.IdLocation);
                }

                if (filterModel.IdMunicipality != 0)
                {
                    var locationsInMunicipality = await this.repository.AllReadonly<Location>(x => x.idMunicipality == filterModel.IdMunicipality).Select(x => x.idLocation).ToListAsync();
                    filterCourses = filterCourses.And(x => x.IdLocation.HasValue);
                    filterCourses = filterCourses.And(x => locationsInMunicipality.Contains(x.IdLocation!.Value));
                }

                if (filterModel.IdDistrict != 0)
                {
                    var municipalitiesInDistrict = await this.repository.AllReadonly<Municipality>(x => x.idDistrict == filterModel.IdDistrict).Select(x => x.idMunicipality).ToListAsync();
                    var locationsInMunicipality = await this.repository.AllReadonly<Location>(x => municipalitiesInDistrict.Contains(x.idMunicipality)).Select(x => x.idLocation).ToListAsync();
                    filterCourses = filterCourses.And(x => x.IdLocation.HasValue);
                    filterCourses = filterCourses.And(x => locationsInMunicipality.Contains(x.IdLocation!.Value));
                }

                if (!string.IsNullOrEmpty(filterModel.CourseName))
                {
                    filterCourses = filterCourses.And(x => x.CourseName.ToLower().Contains(filterModel.CourseName.Trim().ToLower()));
                }

                if (filterModel.IdCourseType.HasValue)
                {
                    filterCourses = filterCourses.And(x => x.IdTrainingCourseType.HasValue);
                    filterCourses = filterCourses.And(x => x.IdTrainingCourseType == filterModel.IdCourseType.Value);
                }

                if (filterModel.IdStatus.HasValue)
                {
                    filterCourses = filterCourses.And(x => x.IdStatus.HasValue);
                    filterCourses = filterCourses.And(x => x.IdStatus == filterModel.IdStatus.Value);
                }

                if (filterModel.IdAssignType.HasValue)
                {
                    filterCourses = filterCourses.And(x => x.IdAssignType.HasValue);
                    filterCourses = filterCourses.And(x => x.IdAssignType == filterModel.IdAssignType.Value);
                }

                if (filterModel.IdProfession != 0)
                {
                    filterCourses = filterCourses.And(x => x.Program.Speciality.IdProfession == filterModel.IdProfession);
                }

                if (filterModel.IdSpeciality != 0)
                {
                    filterCourses = filterCourses.And(x => x.Program.IdSpeciality == filterModel.IdSpeciality);
                }

                filterCourses = filterCourses.And(x => x.IsArchived == filterModel.IsArchived);

                if (filterModel.StartFrom.HasValue)
                {
                    filterCourses = filterCourses.And(x => x.StartDate.HasValue);
                    filterCourses = filterCourses.And(x => x.StartDate!.Value.Date >= filterModel.StartFrom.Value.Date);
                }

                if (filterModel.StartTo.HasValue)
                {
                    filterCourses = filterCourses.And(x => x.StartDate.HasValue);
                    filterCourses = filterCourses.And(x => x.StartDate!.Value.Date <= filterModel.StartTo.Value.Date);
                }

                if (filterModel.EndFrom.HasValue)
                {
                    filterCourses = filterCourses.And(x => x.EndDate.HasValue);
                    filterCourses = filterCourses.And(x => x.EndDate!.Value.Date >= filterModel.EndFrom.Value.Date);
                }

                if (filterModel.EndTo.HasValue)
                {
                    filterCourses = filterCourses.And(x => x.EndDate.HasValue);
                    filterCourses = filterCourses.And(x => x.EndDate!.Value.Date <= filterModel.EndTo.Value.Date);
                }

                var coursesFromDb = await this.repository.AllReadonly<Course>(filterCourses).To<CourseVM>(x => x.CandidateProvider, x => x.Program.Speciality.Profession, x => x.Location).ToListAsync();
                var assignTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType", false, true);;
                var formEducationTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation", false, true);
                var courseStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseStatus", false, true);
                var courseTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram", false, true);
                coursesFromDb.ForEach(x =>
                {
                    if (x.IdAssignType.HasValue)
                    {
                        var assignType = assignTypesSource.FirstOrDefault(y => y.IdKeyValue == x.IdAssignType.Value);
                        x.AssignTypeName = assignType?.Name;
                    }

                    if (x.IdFormEducation.HasValue)
                    {
                        var formEducationType = formEducationTypesSource.FirstOrDefault(y => y.IdKeyValue == x.IdFormEducation.Value);
                        x.FormEducationName = formEducationType?.Name;
                    }

                    if (x.IdStatus.HasValue)
                    {
                        var statusType = courseStatusSource.FirstOrDefault(y => y.IdKeyValue == x.IdStatus.Value);
                        x.StatusName = statusType?.Name;
                    }

                    if (x.IdTrainingCourseType.HasValue)
                    {
                        var courseType = courseTypes.FirstOrDefault(y => y.IdKeyValue == x.IdTrainingCourseType.Value);
                        x.TrainingCourseTypeName = courseType?.Name;
                    }
                });

                return coursesFromDb.OrderBy(x => x.CourseName).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return null;
        }

        public async Task<List<DocumentsFromCPORegisterVM>> GetRIDPKDocumentsForRegisterAsync(TrainedPeopleFilterVM filterModel)
        {
            List<DocumentsFromCPORegisterVM> documents = new List<DocumentsFromCPORegisterVM>();
            try
            {
                var courseTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram", false, true);
                var documentStatusTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientDocumentStatusType", false, true);
                var kvCourseSPK = courseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ProfessionalQualification");
                var kvValidationSPK = courseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfProfessionalQualifications");

                var filter = PredicateBuilder.True<ClientCourseDocument>();
                var filterValidation = PredicateBuilder.True<ValidationClientDocument>();
                filter = filter.And(x => x.ClientCourse.Course.IdTrainingCourseType.HasValue && x.ClientCourse.Course.IdTrainingCourseType == kvCourseSPK!.IdKeyValue);
                filterValidation = filterValidation.And(x => x.ValidationClient.IdCourseType == kvValidationSPK!.IdKeyValue);

                if (filterModel.IdCandidateProvider.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdCandidateProvider == filterModel.IdCandidateProvider.Value);
                    filterValidation = filterValidation.And(x => x.ValidationClient.IdCandidateProvider == filterModel.IdCandidateProvider.Value);
                }

                if (!string.IsNullOrEmpty(filterModel.LicenceNumber))
                {
                    filter = filter.And(x => !string.IsNullOrEmpty(x.ClientCourse.Course.CandidateProvider.LicenceNumber));
                    filter = filter.And(x => x.ClientCourse.Course.CandidateProvider.LicenceNumber!.Contains(filterModel.LicenceNumber.Trim()));
                    filterValidation = filterValidation.And(x => !string.IsNullOrEmpty(x.ValidationClient.CandidateProvider.LicenceNumber));
                    filterValidation = filterValidation.And(x => x.ValidationClient.CandidateProvider.LicenceNumber!.Contains(filterModel.LicenceNumber.Trim()));
                }

                if (filterModel.IdCourseLocation.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdLocation.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.IdLocation == filterModel.IdCourseLocation);
                    filterValidation = filterValidation.And(x => x.ValidationClient.IdCityOfBirth.HasValue);
                    filterValidation = filterValidation.And(x => x.ValidationClient.IdCityOfBirth == filterModel.IdCourseLocation);
                }

                if (filterModel.IdCourseDistrict.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdLocation.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.Location.Municipality.idDistrict == filterModel.IdCourseDistrict);
                    filterValidation = filterValidation.And(x => x.ValidationClient.IdCityOfBirth.HasValue);
                    filterValidation = filterValidation.And(x => x.ValidationClient.CityOfBirth.Municipality.idDistrict == filterModel.IdCourseDistrict);
                }

                if (filterModel.IdCourseMunicipality.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdLocation.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.Location.Municipality.idDistrict == filterModel.IdCourseDistrict);
                    filterValidation = filterValidation.And(x => x.ValidationClient.IdCityOfBirth.HasValue);
                    filterValidation = filterValidation.And(x => x.ValidationClient.CityOfBirth.Municipality.idDistrict == filterModel.IdCourseDistrict);
                }

                if (!string.IsNullOrEmpty(filterModel.FirstName))
                {
                    filter = filter.And(x => x.ClientCourse.FirstName.ToLower().Contains(filterModel.FirstName.Trim().ToLower()));
                    filterValidation = filterValidation.And(x => x.ValidationClient.FirstName.ToLower().Contains(filterModel.FirstName.Trim().ToLower()));
                }

                if (!string.IsNullOrEmpty(filterModel.FamilyName))
                {
                    filter = filter.And(x => x.ClientCourse.FamilyName.ToLower().Contains(filterModel.FamilyName.Trim().ToLower()));
                    filterValidation = filterValidation.And(x => x.ValidationClient.FamilyName.ToLower().Contains(filterModel.FamilyName.Trim().ToLower()));
                }

                if (!string.IsNullOrEmpty(filterModel.Indent))
                {
                    filter = filter.And(x => !string.IsNullOrEmpty(x.ClientCourse.Indent));
                    filter = filter.And(x => x.ClientCourse.Indent!.Contains(filterModel.Indent.Trim()));
                    filterValidation = filterValidation.And(x => !string.IsNullOrEmpty(x.ValidationClient.Indent));
                    filterValidation = filterValidation.And(x => x.ValidationClient.Indent!.Contains(filterModel.Indent.Trim()));
                }

                if (filterModel.IdNationality.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.IdNationality.HasValue);
                    filter = filter.And(x => x.ClientCourse.IdNationality == filterModel.IdNationality);
                    filterValidation = filterValidation.And(x => x.ValidationClient.IdNationality.HasValue);
                    filterValidation = filterValidation.And(x => x.ValidationClient.IdNationality == filterModel.IdNationality);
                }

                if (filterModel.IdSex.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.IdSex.HasValue);
                    filter = filter.And(x => x.ClientCourse.IdSex == filterModel.IdSex);
                    filterValidation = filterValidation.And(x => x.ValidationClient.IdSex.HasValue);
                    filterValidation = filterValidation.And(x => x.ValidationClient.IdSex == filterModel.IdSex);
                }

                if (filterModel.IdMeasureType.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdMeasureType.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.IdMeasureType == filterModel.IdMeasureType);
                }

                if (!string.IsNullOrEmpty(filterModel.CourseName))
                {
                    filter = filter.And(x => x.ClientCourse.Course.CourseName.ToLower().Contains(filterModel.CourseName.Trim().ToLower()));
                }

                if (filterModel.IdProfession.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdProgram.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.Program.IdSpeciality != 0);
                    filter = filter.And(x => x.ClientCourse.Course.Program.Speciality.IdProfession == filterModel.IdProfession);
                    filterValidation = filterValidation.And(x => x.ValidationClient.IdSpeciality.HasValue);
                    filterValidation = filterValidation.And(x => x.ValidationClient.Speciality.IdProfession == filterModel.IdProfession);
                }

                if (filterModel.IdSpeciality.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.IdProgram.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.Program.IdSpeciality == filterModel.IdSpeciality);
                    filterValidation = filterValidation.And(x => x.ValidationClient.IdSpeciality.HasValue);
                    filterValidation = filterValidation.And(x => x.ValidationClient.IdSpeciality == filterModel.IdSpeciality);
                }

                if (filterModel.CourseStartFrom.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.StartDate.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.StartDate!.Value.Date >= filterModel.CourseStartFrom.Value.Date);
                    filterValidation = filterValidation.And(x => x.ValidationClient.StartDate.HasValue);
                    filterValidation = filterValidation.And(x => x.ValidationClient.StartDate!.Value.Date >= filterModel.CourseStartFrom.Value.Date);
                }

                if (filterModel.CourseStartTo.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.StartDate.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.StartDate!.Value.Date <= filterModel.CourseStartTo.Value.Date);
                    filterValidation = filterValidation.And(x => x.ValidationClient.StartDate.HasValue);
                    filterValidation = filterValidation.And(x => x.ValidationClient.StartDate!.Value.Date <= filterModel.CourseStartTo.Value.Date);
                }

                if (filterModel.CourseEndFrom.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.EndDate.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.EndDate!.Value.Date >= filterModel.CourseEndFrom.Value.Date);
                    filterValidation = filterValidation.And(x => x.ValidationClient.EndDate.HasValue);
                    filterValidation = filterValidation.And(x => x.ValidationClient.EndDate!.Value.Date >= filterModel.CourseEndFrom.Value.Date);
                }

                if (filterModel.CourseEndTo.HasValue)
                {
                    filter = filter.And(x => x.ClientCourse.Course.EndDate.HasValue);
                    filter = filter.And(x => x.ClientCourse.Course.EndDate!.Value.Date <= filterModel.CourseEndTo.Value.Date);
                    filterValidation = filterValidation.And(x => x.ValidationClient.EndDate.HasValue);
                    filterValidation = filterValidation.And(x => x.ValidationClient.EndDate!.Value.Date <= filterModel.CourseEndTo.Value.Date);
                }

                if (!string.IsNullOrEmpty(filterModel.DocumentRegNo))
                {
                    filter = filter.And(x => !string.IsNullOrEmpty(filterModel.DocumentRegNo));
                    filter = filter.And(x => x.DocumentRegNo!.ToLower().Contains(filterModel.DocumentRegNo.Trim().ToLower()));
                    filterValidation = filterValidation.And(x => !string.IsNullOrEmpty(filterModel.DocumentRegNo));
                    filterValidation = filterValidation.And(x => x.DocumentRegNo!.ToLower().Contains(filterModel.DocumentRegNo.Trim().ToLower()));
                }

                if (filterModel.IdTypeOfRequestedDocument.HasValue)
                {
                    filter = filter.And(x => x.IdTypeOfRequestedDocument.HasValue);
                    filter = filter.And(x => x.IdTypeOfRequestedDocument == filterModel.IdTypeOfRequestedDocument);
                    filterValidation = filterValidation.And(x => x.IdTypeOfRequestedDocument.HasValue);
                    filterValidation = filterValidation.And(x => x.IdTypeOfRequestedDocument == filterModel.IdTypeOfRequestedDocument);
                }

                if (filterModel.DocumentDateFrom.HasValue)
                {
                    filter = filter.And(x => x.DocumentDate.HasValue);
                    filter = filter.And(x => x.DocumentDate!.Value.Date >= filterModel.DocumentDateFrom.Value.Date);
                    filterValidation = filterValidation.And(x => x.DocumentDate.HasValue);
                    filterValidation = filterValidation.And(x => x.DocumentDate!.Value.Date >= filterModel.DocumentDateFrom.Value.Date);
                }

                if (filterModel.DocumentDateTo.HasValue)
                {
                    filter = filter.And(x => x.DocumentDate.HasValue);
                    filter = filter.And(x => x.DocumentDate!.Value.Date <= filterModel.DocumentDateTo.Value.Date);
                    filterValidation = filterValidation.And(x => x.DocumentDate.HasValue);
                    filterValidation = filterValidation.And(x => x.DocumentDate!.Value.Date <= filterModel.DocumentDateTo.Value.Date);
                }

                var clientCourseDocuments = await this.repository.AllReadonly<ClientCourseDocument>(filter)
                    .To<ClientCourseDocumentVM>(x => x.ClientCourse.Course.Program.Speciality.Profession,
                        x => x.ClientCourse.Course.CandidateProvider,
                        x => x.ClientCourse.Course.Location)
                    .ToListAsync();

                foreach (var clientCourseDocument in clientCourseDocuments)
                {
                    documents.Add(new DocumentsFromCPORegisterVM()
                    {
                        IdEntity = clientCourseDocument.IdClientCourseDocument,
                        IsCourse = true,
                        LicenceNumber = clientCourseDocument.ClientCourse.Course.CandidateProvider.LicenceNumber ?? string.Empty,
                        CPONameOwnerGrid = !string.IsNullOrEmpty(clientCourseDocument.ClientCourse.Course.CandidateProvider.ProviderName) ? $"ЦПО {clientCourseDocument.ClientCourse.Course.CandidateProvider.ProviderName} към {clientCourseDocument.ClientCourse.Course.CandidateProvider.ProviderOwner}" : $"ЦПО към {clientCourseDocument.ClientCourse.Course.CandidateProvider.ProviderOwner}",
                        FullName = clientCourseDocument.ClientCourse.FullName,
                        Profession = clientCourseDocument.ClientCourse.Course.Program.Speciality.Profession.CodeAndName,
                        Speciality = clientCourseDocument.ClientCourse.Course.Program.Speciality.CodeAndName,
                        CourseName = clientCourseDocument.ClientCourse.Course.CourseName,
                        Period = clientCourseDocument.ClientCourse.Course.StartDate.HasValue && clientCourseDocument.ClientCourse.Course.EndDate.HasValue ? clientCourseDocument.ClientCourse.Course.Period : string.Empty,
                        Location = clientCourseDocument.ClientCourse.Course.Location.LocationName,
                        TrainingTypeName = clientCourseDocument.ClientCourse.Course.IdTrainingCourseType.HasValue ? courseTypesSource.FirstOrDefault(x => x.IdKeyValue == clientCourseDocument.ClientCourse.Course.IdTrainingCourseType.Value)?.Name ?? string.Empty : string.Empty,
                        Status = clientCourseDocument.IdDocumentStatus.HasValue ? documentStatusTypesSource.FirstOrDefault(x => x.IdKeyValue == clientCourseDocument.IdDocumentStatus.Value)?.Name ?? string.Empty : string.Empty,
                        FirstName = clientCourseDocument.ClientCourse.FirstName,
                        SecondName = clientCourseDocument.ClientCourse.SecondName ?? string.Empty,
                        FamilyName = clientCourseDocument.ClientCourse.FamilyName,
                        ProviderPerson = clientCourseDocument.ClientCourse.Course.CandidateProvider.ProviderAddressCorrespondence ?? string.Empty,
                        ProviderPhone = clientCourseDocument.ClientCourse.Course.CandidateProvider.ProviderPhoneCorrespondence ?? string.Empty,
                        ProviderEmail = clientCourseDocument.ClientCourse.Course.CandidateProvider.ProviderEmailCorrespondence ?? string.Empty,
                        ProviderAddress = clientCourseDocument.ClientCourse.Course.CandidateProvider.ProviderAddressCorrespondence ?? string.Empty,
                        Indent = clientCourseDocument.ClientCourse.Indent ?? string.Empty,
                        DocumentRegNo = clientCourseDocument.DocumentRegNo ?? string.Empty,
                        DocumentDateAsStr = clientCourseDocument.DocumentDateAsStr,
                        IdSex = clientCourseDocument.ClientCourse.IdSex,
                        IdNationality = clientCourseDocument.ClientCourse.IdNationality,
                        IdEducation = clientCourseDocument.ClientCourse.IdEducation,
                        IdDocumentType = clientCourseDocument.IdDocumentType
                    });
                }

                var validationClientDocuments = await this.repository.AllReadonly<ValidationClientDocument>(filterValidation)
                    .To<ValidationClientDocumentVM>(x => x.ValidationClient.Speciality.Profession,
                        x => x.ValidationClient.CandidateProvider,
                        x => x.ValidationClient.CityOfBirth)
                    .ToListAsync();

                foreach (var validationClientDocument in validationClientDocuments)
                {
                    documents.Add(new DocumentsFromCPORegisterVM()
                    {
                        IdEntity = validationClientDocument.IdValidationClientDocument,
                        IsCourse = false,
                        LicenceNumber = validationClientDocument.ValidationClient.CandidateProvider.LicenceNumber ?? string.Empty,
                        CPONameOwnerGrid = !string.IsNullOrEmpty(validationClientDocument.ValidationClient.CandidateProvider.ProviderName) ? $"ЦПО {validationClientDocument.ValidationClient.CandidateProvider.ProviderName} към {validationClientDocument.ValidationClient.CandidateProvider.ProviderOwner}" : $"ЦПО към {validationClientDocument.ValidationClient.CandidateProvider.ProviderOwner}",
                        FullName = validationClientDocument.ValidationClient.FullName,
                        Profession = validationClientDocument.ValidationClient.Speciality.Profession.CodeAndName,
                        Speciality = validationClientDocument.ValidationClient.Speciality.CodeAndName,
                        Period = validationClientDocument.ValidationClient.StartDate.HasValue && validationClientDocument.ValidationClient.EndDate.HasValue ? validationClientDocument.ValidationClient.Period : string.Empty,
                        TrainingTypeName = validationClientDocument.ValidationClient.IdCourseType != 0 ? courseTypesSource.FirstOrDefault(x => x.IdKeyValue == validationClientDocument.ValidationClient.IdCourseType)?.Name ?? string.Empty : string.Empty,
                        Status = validationClientDocument.IdDocumentStatus.HasValue ? documentStatusTypesSource.FirstOrDefault(x => x.IdKeyValue == validationClientDocument.IdDocumentStatus.Value)?.Name ?? string.Empty : string.Empty,
                        FirstName = validationClientDocument.ValidationClient.FirstName,
                        SecondName = validationClientDocument.ValidationClient.SecondName ?? string.Empty,
                        FamilyName = validationClientDocument.ValidationClient.FamilyName,
                        ProviderPerson = validationClientDocument.ValidationClient.CandidateProvider.ProviderAddressCorrespondence ?? string.Empty,
                        ProviderPhone = validationClientDocument.ValidationClient.CandidateProvider.ProviderEmailCorrespondence ?? string.Empty,
                        ProviderAddress = validationClientDocument.ValidationClient.CandidateProvider.ProviderAddressCorrespondence ?? string.Empty,
                        Indent = validationClientDocument.ValidationClient.Indent ?? string.Empty,
                        DocumentRegNo = validationClientDocument.DocumentRegNo ?? string.Empty,
                        DocumentDateAsStr = validationClientDocument.DocumentDateAsStr,
                        IdSex = validationClientDocument.ValidationClient.IdSex,
                        IdNationality = validationClientDocument.ValidationClient.IdNationality,
                        IdDocumentType = validationClientDocument.IdDocumentType
                    });
                }
            }
            catch (Exception ex) { }

            return documents.OrderBy(x => x.FullName).ToList();
        }

        protected Expression<Func<ClientCourseDocument, bool>> FilterProfessionalCertificateList(TrainedPeopleFilterVM model)
        {
            var predicate = PredicateBuilder.True<ClientCourseDocument>();

            //Само активни ЦПО/ЦПИО
            predicate = predicate.And(p => p.ClientCourse.Course.CandidateProvider.IsActive == true);

            if (model.IdCandidateProvider.HasValue && model.IdCandidateProvider != 0)
            {
                predicate = predicate.And(p => p.ClientCourse.Course.IdCandidateProvider == model.IdCandidateProvider);
            }


            if (model.LicenceNumber != null)
            {
                predicate = predicate.And(x => x.ClientCourse.Course.CandidateProvider.LicenceNumber.Contains(model.LicenceNumber));
            }

            if (model.IdCourseLocation.HasValue && model.IdCourseLocation != 0)
            {
                predicate = predicate.And(x => x.ClientCourse.Course.CandidateProvider.IdLocation == model.IdCourseLocation);
            }

            if (model.IdCourseMunicipality.HasValue && model.IdCourseMunicipality != 0)
            {
                predicate = predicate.And(x => x.ClientCourse.Course.CandidateProvider.Location.Municipality.idMunicipality == model.IdCourseMunicipality);
            }

            if (model.IdCourseDistrict.HasValue && model.IdCourseDistrict != 0)
            {
                predicate = predicate.And(x => x.ClientCourse.Course.CandidateProvider.Location.Municipality.District.idDistrict == model.IdCourseDistrict);
            }


            if (!string.IsNullOrEmpty(model.FirstName))
            {
                predicate = predicate.And(x => x.ClientCourse.FirstName.ToLower().Contains(model.FirstName.ToLower()));
            }

            if (!string.IsNullOrEmpty(model.FamilyName))
            {
                predicate = predicate.And(x => x.ClientCourse.FamilyName.ToLower().Contains(model.FamilyName.ToLower()));
            }

            if (!string.IsNullOrEmpty(model.Indent))
            {
                predicate = predicate.And(x => x.ClientCourse.Indent.Contains(model.Indent));
            }

            if (model.IdNationality.HasValue && model.IdNationality != 0)
            {
                predicate = predicate.And(x => x.ClientCourse.IdNationality == model.IdNationality);
            }

            if (model.IdSex.HasValue && model.IdSex != 0)
            {
                predicate = predicate.And(x => x.ClientCourse.IdSex == model.IdSex);

            }

            if (!string.IsNullOrEmpty(model.CourseName))
            {
                predicate = predicate.And(x => x.ClientCourse.Course.CourseName.ToLower().Contains(model.CourseName.ToLower()));
            }

            if (model.IdMeasureType.HasValue && model.IdMeasureType != 0)
            {
                predicate = predicate.And(x => x.ClientCourse.Course.IdMeasureType == model.IdMeasureType);
            }

            if (model.IdSpeciality.HasValue && model.IdSpeciality != 0)
            {
                predicate = predicate.And(x => x.ClientCourse.Speciality.IdSpeciality == model.IdSpeciality);
            }

            if (model.IdProfession.HasValue && model.IdProfession != 0)
            {
                predicate = predicate.And(x => x.ClientCourse.Speciality.Profession.IdProfession == model.IdProfession);
            }

            if (model.CourseStartFrom != null && model.CourseStartTo != null)
            {
                predicate = predicate.And(x =>
                    x.ClientCourse.Course.StartDate >= model.CourseStartFrom &&
                    x.ClientCourse.Course.StartDate <= model.CourseStartTo);
            }

            if (model.CourseEndFrom != null && model.CourseEndTo != null)
            {
                predicate = predicate.And(x =>
                    x.ClientCourse.Course.EndDate >= model.CourseEndFrom &&
                    x.ClientCourse.Course.EndDate <= model.CourseEndTo);
            }

            if (!string.IsNullOrEmpty(model.DocumentRegNo))
            {
                predicate = predicate.And(x => x.DocumentRegNo.Contains(model.DocumentRegNo));
            }

            if (model.IdTypeOfRequestedDocument.HasValue && model.IdTypeOfRequestedDocument != 0)
            {
                predicate = predicate.And(x => x.IdDocumentType == model.IdTypeOfRequestedDocument);
            }

            if (model.IdTypeOfRequestedDocument.HasValue && model.IdTypeOfRequestedDocument != 0)
            {
                predicate = predicate.And(x => x.IdTypeOfRequestedDocument == model.IdTypeOfRequestedDocument);
            }

            if (model.DocumentDateTo != null && model.DocumentDateFrom != null)
            {
                predicate = predicate.And(x => x.DocumentDate >= model.DocumentDateFrom && x.DocumentDate <= model.DocumentDateTo);
            }

            if(model.TypeOfRequestDocuments is not null)
            {
                predicate = predicate.And(x => model.TypeOfRequestDocuments.Contains(x.IdTypeOfRequestedDocument)); 
            }

            return predicate;
        }

        public async Task<List<ClientCourseDocumentVM>> GetAllDocumentsByIdTypeOfRequestedDocument(int idTypeOfRequestedDocument)
        {
            var data = this.repository
                .All<ClientCourseDocument>(x => x.IdTypeOfRequestedDocument == idTypeOfRequestedDocument)
                .To<ClientCourseDocumentVM>(x => x.ClientCourse.Course.Program.Speciality.Profession,
                x => x.ClientCourse.Speciality.Profession,
                x => x.ClientCourse.Client.CandidateProvider.Location,
                x => x.ClientCourse.Course.Location.Municipality.District)
                .ToList();

            var typesFrameworkProgram = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram");


            foreach (var entry in data)
            {
                entry.ClientCourse.Course.Program.CourseType = await dataSourceService.GetKeyValueByIdAsync(entry.ClientCourse.Course.Program.IdCourseType);
                entry.ClientCourse.Course.Program.Speciality.CodeAndNameAndVQS = $"{entry.ClientCourse.Course.Program.Speciality.CodeAndName} - {dataSourceService.GetKeyValueByIdAsync(entry.ClientCourse.Course.Program.Speciality.IdVQS).Result.Name}";
                if (entry.ClientCourse.Client.CandidateProvider.LicenceNumber == null)
                    entry.ClientCourse.Client.CandidateProvider.LicenceNumber = "";
                if (entry.ClientCourse.Client.CandidateProvider.LicenceDate == null)
                {
                    entry.ClientCourse.Client.CandidateProvider.LicenceDate = DateTime.MinValue;
                }
            }

            return data;
        }

        public async Task<List<ClientCourseDocumentVM>> GetAllDocumentsByIdTypeOfRequestedDocument(int?[] idTypesOfRequestedDocument)
        {

            var data = this.repository
                .All<ClientCourseDocument>(x => idTypesOfRequestedDocument.ToList().Contains(x.IdTypeOfRequestedDocument))
                .To<ClientCourseDocumentVM>(
                    x => x.ClientCourse.Course.Program.Speciality.Profession,
                    x => x.ClientCourse.Speciality.Profession,
                    x => x.ClientCourse.Client.CandidateProvider.Location,
                    x => x.ClientCourse.Course.Location.Municipality.District)
                .ToList();
            var typesFrameworkProgram = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram");


            foreach (var entry in data)
            {
                entry.ClientCourse.Course.Program.CourseType = await dataSourceService.GetKeyValueByIdAsync(entry.ClientCourse.Course.Program.IdCourseType);
                entry.ClientCourse.Course.Program.Speciality.CodeAndNameAndVQS = $"{entry.ClientCourse.Course.Program.Speciality.CodeAndName} - {dataSourceService.GetKeyValueByIdAsync(entry.ClientCourse.Course.Program.Speciality.IdVQS).Result.Name}";
                if (entry.ClientCourse.Client.CandidateProvider.LicenceNumber == null)
                    entry.ClientCourse.Client.CandidateProvider.LicenceNumber = "";
                if (entry.ClientCourse.Client.CandidateProvider.LicenceDate == null)
                {
                    entry.ClientCourse.Client.CandidateProvider.LicenceDate = DateTime.MinValue;
                }
            }

            return data;
        }

        public List<ClientCourseDocumentVM> GetDocumentsByCandidateProvder(CandidateProviderVM candidate)
        {
            var docs = this.repository
                .All<ClientCourseDocument>()
                .To<ClientCourseDocumentVM>(x => x.ClientCourse.Course.Program.FrameworkProgram,
                x => x.ClientCourse.Client)
                .Where(x => x.ClientCourse.Course.Program.IdCandidateProvider == candidate.IdCandidate_Provider)
                .ToList();

            return docs;
        }

        public List<CourseDocumentUploadedFileVM> getCourseDocumentUploadedFile(int idClientCourseDocument)
        {
            IQueryable docs = this.repository.All<CourseDocumentUploadedFile>().Where(x => x.IdClientCourseDocument == idClientCourseDocument);

            return docs.To<CourseDocumentUploadedFileVM>().ToList();
        }

        public async Task<List<CourseVM>> getAllCourses(StateExaminationInfoFilterListVM modelFilterStateExam, string isFromStateExaminationPage)
        {
            List<CourseVM> data = new List<CourseVM>();

            if (isFromStateExaminationPage is not null)
            {
                data = this.repository
                   .All<Course>(FilterTrainingCourses(modelFilterStateExam))
                   .To<CourseVM>(x => x.Program.Speciality.Profession, x => x.Location, x => x.Program.CandidateProvider, x => x.IdCourse, x => x.Program, x => x.FormEducation, x => x.CandidateProvider)
                   .ToList();
            }
            else
            {
                data = this.repository
                    .All<Course>()
                    .To<CourseVM>(x => x.Program.Speciality.Profession, x => x.Location, x => x.Program.CandidateProvider, x => x.IdCourse, x => x.Program, x => x.FormEducation, x => x.CandidateProvider)
                    .ToList();
            }

            foreach (var course in data)
            {
                if (course.Program.IdCourseType != null && course.Program.IdCourseType != 0)
                {
                    course.Program.CourseType = await dataSourceService.GetKeyValueByIdAsync(course.Program.IdCourseType);
                }
                if (course.IdAssignType != null && course.IdAssignType != 0)
                {
                    course.AssignType = await dataSourceService.GetKeyValueByIdAsync(course.IdAssignType);
                }
                if (course.IdFormEducation != null && course.IdFormEducation != 0)
                {
                    course.FormEducation = await dataSourceService.GetKeyValueByIdAsync(course.IdFormEducation);
                }
                if (course.IdStatus != null && course.IdStatus != 0)
                {
                    course.Status = await dataSourceService.GetKeyValueByIdAsync(course.IdStatus);
                }
                course.Program.Speciality.CodeAndNameAndVQS = $"{course.Program.Speciality.CodeAndName} - {dataSourceService.GetKeyValueByIdAsync(course.Program.Speciality.IdVQS).Result.Name}";
            }

            return data;
        }

        protected Expression<Func<Course, bool>> FilterTrainingCourses(StateExaminationInfoFilterListVM modelFilterStateExam)
        {           
            var predicate = PredicateBuilder.True<Course>();
            
            predicate = predicate.And(p => p.CandidateProvider.IsActive == true);

            if (modelFilterStateExam.IdCandidateProvider.HasValue && modelFilterStateExam.IdCandidateProvider != 0)
            {
                predicate = predicate.And(p => p.IdCandidateProvider == modelFilterStateExam.IdCandidateProvider);
            }

            if (modelFilterStateExam.LicenceNumber != null)
            {
                predicate = predicate.And(x => x.CandidateProvider.LicenceNumber.Contains(modelFilterStateExam.LicenceNumber));
            }

            if (!string.IsNullOrEmpty(modelFilterStateExam.CourseName))
            {
                predicate = predicate.And(x => x.CourseName.ToLower().Contains(modelFilterStateExam.CourseName.ToLower()));
            }

            if (modelFilterStateExam.TrainingTypeIntCode == "Theory")
            {
                predicate = predicate.And(x => x.ExamTheoryDate.HasValue);
                if (modelFilterStateExam.ExamDateFrom.HasValue)
                {
                    predicate = predicate.And(x => x.ExamTheoryDate.Value.Date >= modelFilterStateExam.ExamDateFrom.Value.Date);
                }

                if (modelFilterStateExam.ExamDateTo.HasValue)
                {
                    predicate = predicate.And(x => x.ExamTheoryDate.Value.Date <= modelFilterStateExam.ExamDateTo.Value.Date);
                }
            }
            else if (modelFilterStateExam.TrainingTypeIntCode == "Practice")
            {
                predicate = predicate.And(x => x.ExamPracticeDate.HasValue);

                if (modelFilterStateExam.ExamDateFrom.HasValue)
                {
                    predicate = predicate.And(x => x.ExamPracticeDate.Value.Date >= modelFilterStateExam.ExamDateFrom.Value.Date);
                }

                if (modelFilterStateExam.ExamDateTo.HasValue)
                {
                    predicate = predicate.And(x => x.ExamPracticeDate.Value.Date <= modelFilterStateExam.ExamDateTo.Value.Date);
                }
            }
            else
            {
                if (modelFilterStateExam.ExamDateFrom.HasValue)
                {
                    predicate = predicate.And(x => x.ExamPracticeDate.Value.Date >= modelFilterStateExam.ExamDateFrom.Value.Date && x.ExamTheoryDate.Value.Date >= modelFilterStateExam.ExamDateFrom.Value.Date);
                }

                if (modelFilterStateExam.ExamDateTo.HasValue)
                {
                    predicate = predicate.And(x => x.ExamPracticeDate.Value.Date <= modelFilterStateExam.ExamDateTo.Value.Date && x.ExamTheoryDate.Value.Date <= modelFilterStateExam.ExamDateTo.Value.Date);
                }
            }                
            return predicate;
        }

        public async Task<List<ClientVM>> GetAllCIPOClients()
        {
            var key = await dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCIPO");
            return (this.repository
                .All<Client>()
                .To<ClientVM>(x => x.CandidateProvider, x => x.CandidateProvider.Location)
                .Where(x => x.CandidateProvider.IdTypeLicense == key.IdKeyValue))
                .ToList();
        }

        public async Task<MemoryStream> GenerateExcelReportForCoursesAndClients(string year)
        {
            try
            {
                var courses = this.repository
            .All<ClientCourse>()
            .To<ClientCourseVM>(x => x.Course.Program.CandidateProvider, x => x.Client, x => x.Course.Program.Speciality, x => x.Course.Location.Municipality.District)
            .Where(x => x.CreationDate.Year.ToString().Equals(year))
            .ToList();

                var qualificationLevel = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MinimumQualificationLevel", false, true)).ToList();

                var educationType = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Education", false, true)).ToList();

                var assignType = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType", false, true)).ToList();

                var vqs = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS", false, true)).ToList();

                var sex = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex", false, true)).ToList();

                var nationality = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality", false, true)).ToList();

                var docType = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram", false, true)).ToList();

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    int row = 1;

                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Xlsx;

                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];



                    foreach (var course in courses)
                    {
                        if (course.IdClientCourse == 89)
                        {

                        }

                        if (course.IdAssignType != null)
                        {
                            course.Course.AssignType = assignType.Where(x => x.IdKeyValue == course.IdAssignType).First();
                        }
                        if (course.Course.Program.Speciality.IdVQS != null)
                        {
                            course.Course.Program.Speciality.VQS = vqs.Where(x => x.IdKeyValue == course.Course.Program.Speciality.IdVQS).First();
                        }
                        if (course.Client.IdSex != null)
                        {
                            course.Client.Sex = sex.Where(x => x.IdKeyValue == course.Client.IdSex).First();
                        }
                        if (course.Client.IdNationality != null)
                        {
                            course.Client.Nationality = nationality.Where(x => x.IdKeyValue == course.Client.IdNationality).First();
                        }
                        if (course.IdQualificationLevel != null)
                        {
                            course.QualificationLevel = qualificationLevel.Where(x => x.IdKeyValue == course.IdQualificationLevel).First();
                        }
                        if (course.IdEducation != null)
                        {
                            course.Education = educationType.Where(x => x.IdKeyValue == course.IdEducation).First();
                        }
                        if (course.ClientCourseDocuments.Count() > 0 && course.ClientCourseDocuments.First().IdDocumentType != null)
                        {
                            course.ClientCourseDocuments.First().DocumentType = docType.Where(x => x.IdKeyValue == course.ClientCourseDocuments.First().IdDocumentType).First();
                        }


                        object[] excelRow = new object[19]
                        {
                       row,
                       year,
                       course.Course.Program.CandidateProvider.PoviderBulstat,
                       course.Course.Program.CandidateProvider.LicenceNumber,
                       course.QualificationLevel == null ? "" : course.QualificationLevel.DefaultValue2,
                       "0",
                       course.Course.Program.Speciality.Code,
                       course.Course.Program.Speciality.VQS.DefaultValue1,
                       course.Course.Location.Municipality.District.DistrictCode,
                       course.Client.Sex.DefaultValue2,
                       course.Client.BirthDate.ToString(),
                       course.Client.Nationality.Order,
                       course.Course.DurationHours,
                       course.Course.Cost,
                       course.Course.AssignType.DefaultValue2,
                       course.Education == null ? "" : course.Education.DefaultValue2,
                       course.ClientCourseDocuments.Count() > 0 ?  course.ClientCourseDocuments.First().DocumentType.DefaultValue2 : "0",
                       "2",
                        course.ClientCourseDocuments.Count() > 0 ?  "1" : "0"
                        };

                        worksheet.ImportArray(excelRow, row, 1, false);
                        row++;
                    }
                    if (courses.Count() > 0)
                    {
                        worksheet.Range[$"A1:O{courses.Count()}"].AutofitColumns();
                        worksheet.Range[$"A1:O{courses.Count()}"].BorderInside(ExcelLineStyle.Medium);
                        worksheet.Range[$"A1:O{courses.Count()}"].BorderAround(ExcelLineStyle.Medium);
                    }
                    MemoryStream stream = new MemoryStream();

                    workbook.SaveAs(stream);
                    return stream;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                return null;
            }
        }


        public async Task<List<ClientCourseDocumentVM>> GetAllProfessionalCertificateDocuments(TrainedPeopleFilterVM model)
        {

            var data = await this.repository
                .AllReadonly<ClientCourseDocument>(FilterProfessionalCertificateList(model))
                .To<ClientCourseDocumentVM>(
                x => x.ClientCourse.Course.Program.Speciality.Profession,
                x => x.ClientCourse.Course.CandidateProvider.Location.Municipality.District)
                .ToListAsync();

            var typesFrameworkProgram = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram");

            foreach (var entry in data)
            {
                entry.ClientCourse.Course.TrainingCourseTypeName = typesFrameworkProgram.FirstOrDefault(x => x.IdKeyValue == entry.ClientCourse.Course.IdTrainingCourseType.Value)?.Name;
                entry.ClientCourse.Course.Program.Speciality.CodeAndNameAndVQS = $"{entry.ClientCourse.Course.Program.Speciality.CodeAndName} - {dataSourceService.GetKeyValueByIdAsync(entry.ClientCourse.Course.Program.Speciality.IdVQS).Result.Name}";

            }

            return data;
        }



        public async Task<List<ClientCourseDocumentVM>> GetAllDocuments()
        {

            var data = await this.repository
                .AllReadonly<ClientCourseDocument>()
                .To<ClientCourseDocumentVM>(
                x => x.ClientCourse.Course.Program.Speciality.Profession,
                x => x.ClientCourse.Client.CandidateProvider,
                x => x.ClientCourse.Speciality.Profession,
                x => x.ClientCourse.Course.CandidateProvider.Location,
                x => x.ClientCourse.Course.Location.Municipality.District,
                x => x.CourseDocumentUploadedFiles)
                .ToListAsync();

            var typesFrameworkProgram = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram");
            var documentStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientDocumentStatusType");

            foreach (var entry in data)
            {
                entry.ClientCourse.Course.TrainingCourseTypeName = typesFrameworkProgram.FirstOrDefault(x => x.IdKeyValue == entry.ClientCourse.Course.IdTrainingCourseType.Value)?.Name;
                entry.ClientCourse.Course.Program.Speciality.CodeAndNameAndVQS = $"{entry.ClientCourse.Course.Program.Speciality.CodeAndName} - {dataSourceService.GetKeyValueByIdAsync(entry.ClientCourse.Course.Program.Speciality.IdVQS).Result.Name}";
                if (entry.ClientCourse.Client.CandidateProvider.LicenceNumber == null)
                    entry.ClientCourse.Client.CandidateProvider.LicenceNumber = "";
                if (entry.ClientCourse.Client.CandidateProvider.LicenceDate == null)
                {
                    entry.ClientCourse.Client.CandidateProvider.LicenceDate = DateTime.MinValue;
                }

                if (entry.IdDocumentStatus.HasValue)
                {
                    var status = documentStatusSource.FirstOrDefault(x => x.IdKeyValue == entry.IdDocumentStatus.Value);
                    if (status is not null)
                    {
                        entry.DocumentStatusValue = status.Name;
                    }
                }
            }

            return data;
        }

        public List<ClientCourseDocumentVM> FilterClientCourseDocuments(ClientCourseDocumentVM model)
        {
            var filteredDocs = GetAllDocuments().Result;

            if (model.ClientCourse.Client.IdCandidateProvider != 0)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Client.IdCandidateProvider == model.ClientCourse.Client.IdCandidateProvider)
                    .ToList();
            }
            if (model.ClientCourse.Client.CandidateProvider.LicenceNumber != null)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Client.CandidateProvider.LicenceNumber
                    .Contains(model.ClientCourse.Client.CandidateProvider.LicenceNumber)).ToList();
            }
            if (model.ClientCourse.Course.Location.idLocation != 0)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Course.Location.idLocation == model.ClientCourse.Course.Location.idLocation)
                    .ToList();
            }
            if (model.ClientCourse.Course.Location.Municipality.idMunicipality != 0)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Course.Location.Municipality.idMunicipality == model.ClientCourse.Course.Location.Municipality.idMunicipality)
                    .ToList();
            }
            if (model.ClientCourse.Course.Location.Municipality.District.idDistrict != 0)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Course.Location.Municipality.District.idDistrict == model.ClientCourse.Course.Location.Municipality.District.idDistrict)
                    .ToList();
            }
            if (model.ClientCourse.Client.FirstName != null && !model.ClientCourse.Client.FirstName.Equals(""))
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Client.FirstName.ToLower().Contains(model.ClientCourse.Client.FirstName.ToLower()))
                    .ToList();
            }
            if (model.ClientCourse.Client.FamilyName != null && !model.ClientCourse.Client.FamilyName.Equals(""))
            {
                filteredDocs = filteredDocs
                  .Where(x => x.ClientCourse.Client.FamilyName.ToLower().Contains(model.ClientCourse.Client.FamilyName.ToLower()))
                  .ToList();
            }
            if (model.ClientCourse.Client.Indent != null && model.ClientCourse.Client.Indent != "")
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Indent.Contains(model.ClientCourse.Indent))
                    .ToList();
            }
            if (model.ClientCourse.Client.IdNationality != null)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Client.IdNationality == model.ClientCourse.Client.IdNationality)
                    .ToList();
            }
            if (model.ClientCourse.Client.IdSex != null)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Client.IdSex == model.ClientCourse.Client.IdSex)
                    .ToList();
            }
            if (model.ClientCourse.Course.CourseName != null && !model.ClientCourse.Course.CourseName.Equals(""))
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Course.CourseName.ToLower().Contains(model.ClientCourse.Course.CourseName.ToLower()))
                    .ToList();
            }
            if (model.ClientCourse.Course.IdMeasureType != null)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Course.IdMeasureType == model.ClientCourse.Course.IdMeasureType)
                    .ToList();
            }
            if (model.ClientCourse.Speciality.IdSpeciality != 0)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Speciality.IdSpeciality == model.ClientCourse.Speciality.IdSpeciality)
                    .ToList();
            }

            if (model.ClientCourse.Speciality.Profession.IdProfession != 0)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Speciality.Profession.IdProfession == model.ClientCourse.Speciality.Profession.IdProfession)
                    .ToList();
            }
            if (model.ClientCourse.Course.startCourseFrom != null && model.ClientCourse.Course.startCourseTo != null)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.ClientCourse.Course.StartDate >= model.ClientCourse.Course.startCourseFrom && x.ClientCourse.Course.StartDate <= model.ClientCourse.Course.startCourseTo)
                    .ToList();
            }
            if (model.ClientCourse.Course.endCourseFrom != null && model.ClientCourse.Course.endtCourseTo != null)
            {
                filteredDocs = filteredDocs
                           .Where(x => x.ClientCourse.Course.EndDate >= model.ClientCourse.Course.endCourseFrom && x.ClientCourse.Course.EndDate <= model.ClientCourse.Course.endtCourseTo)
                           .ToList();
            }
            if (model.DocumentRegNo != null && model.DocumentRegNo != "")
            {
                filteredDocs = filteredDocs
                    .Where(x => x.DocumentRegNo.Contains(model.DocumentRegNo))
                    .ToList();
            }
            if (model.IdDocumentType != null)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.IdDocumentType == model.IdDocumentType)
                    .ToList();
            }
            if (model.IdTypeOfRequestedDocument != null)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.IdTypeOfRequestedDocument == model.IdTypeOfRequestedDocument)
                    .ToList();
            }
            if (model.docDateTo != null && model.docDateFrom != null)
            {
                filteredDocs = filteredDocs
                    .Where(x => x.DocumentDate >= model.docDateFrom && x.DocumentDate <= model.docDateTo)
                    .ToList();
            }

            return filteredDocs;
        }

        public async Task<List<CourseVM>> filterCourses(CourseVM filter)
        {
            List<CourseVM> filteredCourses = await getAllCourses(null, null);

            if (filter.Program.CandidateProvider.IdCandidate_Provider != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Program.CandidateProvider.IdCandidate_Provider == filter.Program.CandidateProvider.IdCandidate_Provider).ToList();
            }
            if (filter.Program.CandidateProvider.LicenceNumber != null && filter.Program.CandidateProvider.LicenceNumber != "")
            {
                filteredCourses = filteredCourses.Where(x => x.Program.CandidateProvider.LicenceNumber != null ? x.Program.CandidateProvider.LicenceNumber.Contains(filter.Program.CandidateProvider.LicenceNumber) : true).ToList();
            }
            if (filter.Location.idLocation != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Location.idLocation == filter.Location.idLocation).ToList();
            }
            if (filter.Location.Municipality.idMunicipality != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Location.Municipality.idMunicipality == filter.Location.Municipality.idMunicipality).ToList();
            }
            if (filter.Location.Municipality.District.idDistrict != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Location.Municipality.District.idDistrict == filter.Location.Municipality.District.idDistrict).ToList();
            }
            if (filter.CourseName != null && filter.CourseName != "")
            {
                filteredCourses = filteredCourses.Where(x => x.CourseName.ToLower().Contains(filter.CourseName.ToLower())).ToList();
            }
            if (filter.IdMeasureType != null)
            {
                filteredCourses = filteredCourses.Where(x => x.IdMeasureType == filter.IdMeasureType).ToList();
            }
            if (filter.IdStatus != null)
            {
                filteredCourses = filteredCourses.Where(x => x.IdStatus == filter.IdStatus).ToList();
            }
            if (filter.IdFormEducation != null)
            {
                filteredCourses = filteredCourses.Where(x => x.IdFormEducation == filter.IdFormEducation).ToList();
            }
            if (filter.IdAssignType != null)
            {
                filteredCourses = filteredCourses.Where(x => x.IdAssignType == filter.IdAssignType).ToList();
            }
            if (filter.IdTrainingCourseType != null)
            {
                filteredCourses = filteredCourses.Where(x => x.IdTrainingCourseType == filter.IdTrainingCourseType).ToList();
            }
            if (filter.Program.Speciality.IdSpeciality != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Program.Speciality.IdSpeciality == filter.Program.Speciality.IdSpeciality).ToList();
            }
            if (filter.Program.Speciality.Profession.IdProfession != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Program.Speciality.Profession.IdProfession == filter.Program.Speciality.Profession.IdProfession).ToList();
            }
            if (filter.Program.Speciality.IdVQS != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Program.Speciality.IdVQS == filter.Program.Speciality.IdVQS).ToList();
            }
            if (filter.Program.IdFrameworkProgram != null && filter.Program.IdFrameworkProgram != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Program.IdFrameworkProgram == filter.Program.IdFrameworkProgram).ToList();
            }
            if (filter.Program.ProgramName != null && filter.Program.ProgramName != "")
            {
                filteredCourses = filteredCourses.Where(x => x.Program.ProgramName.ToLower().Contains(filter.Program.ProgramName.ToLower())).ToList();
            }
            if (filter.CandidateProviderPremises != null && filter.CandidateProviderPremises.PremisesName != null && filter.CandidateProviderPremises.PremisesName != "")
            {
                filteredCourses = filteredCourses.Where(x => x.CandidateProviderPremises.PremisesName.ToLower().Contains(filter.CandidateProviderPremises.PremisesName.ToLower())).ToList();
            }

            if (filter.startCourseFrom != null && filter.startCourseTo != null)
            {
                filteredCourses = filteredCourses
                    .Where(x => x.StartDate >= filter.startCourseFrom && x.StartDate <= filter.startCourseTo.Value.AddDays(1))
                    .ToList();
            }
            if (filter.endCourseFrom != null && filter.endtCourseTo != null)
            {
                filteredCourses = filteredCourses
                           .Where(x => x.EndDate >= filter.endCourseFrom && x.EndDate <= filter.endtCourseTo.Value.AddDays(1))
                           .ToList();
            }
            if (filter.subscribeDateFrom != null && filter.subscribeDateTo != null)
            {
                filteredCourses = filteredCourses
                           .Where(x => x.SubscribeDate >= filter.subscribeDateFrom && x.SubscribeDate <= filter.subscribeDateTo)
                           .ToList();
            }
            if (filter.examTheoryDateFrom != null && filter.examTheoryDateTo != null)
            {
                filteredCourses = filteredCourses
                           .Where(x => x.ExamTheoryDate >= filter.examTheoryDateFrom && x.ExamTheoryDate <= filter.examTheoryDateTo)
                           .ToList();
            }
            if (filter.examPracticeDateFrom != null && filter.examPracticeDateTo != null)
            {
                filteredCourses = filteredCourses
                           .Where(x => x.ExamPracticeDate >= filter.examPracticeDateFrom && x.ExamPracticeDate <= filter.examPracticeDateTo)
                           .ToList();
            }

            return filteredCourses;
        }

        public async Task<List<CourseVM>> filterCourses(CourseVM filter, String type, int IdCandidateProvider)
        {
            List<CourseVM> courses = new List<CourseVM>();
            switch (type)
            {
                case GlobalConstants.CURRENT_COURSES_PP:
                    courses = (await GetAllCurrentTrainingCoursesByIdCandidateProviderAsync(IdCandidateProvider)).ToList();

                    break;
                case GlobalConstants.CURRENT_COURSES_LC:
                    courses = (await GetAllCurrentTrainingCoursesByIdCandidateProviderAsync(IdCandidateProvider)).ToList();

                    break;

                case GlobalConstants.UPCOMING_COURSES_LC:
                    courses = (await GetAllUpcomingTrainingCoursesByIdCandidateProviderAsync(IdCandidateProvider)).ToList();
                    break;
                case GlobalConstants.UPCOMING_COURSES_SPK:

                    courses = (await GetAllUpcomingTrainingCoursesByIdCandidateProviderAsync(IdCandidateProvider)).ToList();
                    break;
                case GlobalConstants.UPCOMING_COURSES_PP:
                    courses = (await GetAllUpcomingTrainingCoursesByIdCandidateProviderAsync(IdCandidateProvider)).ToList();
                    break;
                case GlobalConstants.CURRENT_COURSES_SPK:
                    courses = (await GetAllCurrentTrainingCoursesByIdCandidateProviderAsync(IdCandidateProvider)).ToList();

                    break;

                case GlobalConstants.COMPLETED_COURSES_SPK:
                    courses = (await GetAllCompletedTrainingCoursesByIdCandidateProviderAsync(IdCandidateProvider)).ToList();
                    break;
                case GlobalConstants.COMPLETED_COURSES_PP:
                    courses = (await GetAllCompletedTrainingCoursesByIdCandidateProviderAsync(IdCandidateProvider)).ToList();
                    break;
                case GlobalConstants.COMPLETED_COURSES_LC:
                    courses = (await GetAllCompletedTrainingCoursesByIdCandidateProviderAsync(IdCandidateProvider)).ToList();
                    break;

                case GlobalConstants.ARCHIVED_COURSES_SPK:
                    break;
            }
            List<CourseVM> filteredCourses = new List<CourseVM>();

            if (filter.clientCourseFirstName != null && filter.clientCourseFirstName != "" || filter.clientCourseLastName != null && filter.clientCourseLastName != "" || filter.clientCourseIndent != null && filter.clientCourseIndent != "")
            {
                foreach (var course in courses)
                {
                    course.ClientCourses = (await GetCourseClientsByIdCourseAsync(course.IdCourse)).ToList();
                    var data = course.ClientCourses.AsEnumerable();
                    if (!string.IsNullOrEmpty(filter.clientCourseFirstName))
                    {
                        data = data.Where(x => x.FirstName == filter.clientCourseFirstName);
                    }
                    if (!string.IsNullOrEmpty(filter.clientCourseLastName))
                    {
                        data = data.Where(x => x.FamilyName == filter.clientCourseLastName);
                    }
                    if (!string.IsNullOrEmpty(filter.clientCourseIndent))
                    {
                        data = data.Where(x => x.Indent == filter.clientCourseIndent);
                    }

                    if (data.Any())
                    {
                        filteredCourses.Add(course);
                    }
                }

            }
            else
            {
                filteredCourses = courses;
            }

            if (filter.Program.CandidateProvider.IdCandidate_Provider != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Program.CandidateProvider.IdCandidate_Provider == filter.Program.CandidateProvider.IdCandidate_Provider).ToList();
            }
            if (filter.Program.CandidateProvider.LicenceNumber != null && filter.Program.CandidateProvider.LicenceNumber != "")
            {
                filteredCourses = filteredCourses.Where(x => x.Program.CandidateProvider.LicenceNumber.Contains(filter.Program.CandidateProvider.LicenceNumber)).ToList();
            }
            if (filter.Location.idLocation != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Location.idLocation == filter.Location.idLocation).ToList();
            }
            if (filter.Location.Municipality.idMunicipality != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Location.Municipality.idMunicipality == filter.Location.Municipality.idMunicipality).ToList();
            }
            if (filter.Location.Municipality.District.idDistrict != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Location.Municipality.District.idDistrict == filter.Location.Municipality.District.idDistrict).ToList();
            }
            if (filter.CourseName != null && filter.CourseName != "")
            {
                filteredCourses = filteredCourses.Where(x => x.CourseName.ToLower().Contains(filter.CourseName.ToLower())).ToList();
            }
            if (filter.IdMeasureType != null)
            {
                filteredCourses = filteredCourses.Where(x => x.IdMeasureType == filter.IdMeasureType).ToList();
            }
            if (filter.IdStatus != null)
            {
                filteredCourses = filteredCourses.Where(x => x.IdStatus == filter.IdStatus).ToList();
            }
            if (filter.IdFormEducation != null)
            {
                filteredCourses = filteredCourses.Where(x => x.IdFormEducation == filter.IdFormEducation).ToList();
            }
            if (filter.IdAssignType != null)
            {
                filteredCourses = filteredCourses.Where(x => x.IdAssignType == filter.IdAssignType).ToList();
            }
            if (filter.IdTrainingCourseType != null)
            {
                filteredCourses = filteredCourses.Where(x => x.IdTrainingCourseType == filter.IdTrainingCourseType).ToList();
            }
            if (filter.Program.Speciality.IdSpeciality != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Program.Speciality.IdSpeciality == filter.Program.Speciality.IdSpeciality).ToList();
            }
            if (filter.Program.Speciality.Profession.IdProfession != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Program != null ? x.Program.Speciality.Profession.IdProfession == filter.Program.Speciality.Profession.IdProfession : false).ToList();
            }
            if (filter.Program.Speciality.IdVQS != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Program.Speciality.IdVQS == filter.Program.Speciality.IdVQS).ToList();
            }
            if (filter.Program.IdFrameworkProgram != null && filter.Program.IdFrameworkProgram != 0)
            {
                filteredCourses = filteredCourses.Where(x => x.Program.IdFrameworkProgram == filter.Program.IdFrameworkProgram).ToList();
            }
            if (filter.Program.ProgramName != null && filter.Program.ProgramName != "")
            {
                filteredCourses = filteredCourses.Where(x => x.Program.ProgramName.ToLower().Contains(filter.Program.ProgramName.ToLower())).ToList();
            }
            if (filter.CandidateProviderPremises != null && filter.CandidateProviderPremises.PremisesName != null && filter.CandidateProviderPremises.PremisesName != "")
            {
                filteredCourses = filteredCourses.Where(x => x.CandidateProviderPremises.PremisesName.ToLower().Contains(filter.CandidateProviderPremises.PremisesName.ToLower())).ToList();
            }

            if (filter.startCourseFrom != null && filter.startCourseTo != null)
            {
                filteredCourses = filteredCourses
                    .Where(x => x.StartDate >= filter.startCourseFrom && x.StartDate <= filter.startCourseTo.Value.AddDays(1))
                    .ToList();
            }
            if (filter.endCourseFrom != null && filter.endtCourseTo != null)
            {
                filteredCourses = filteredCourses
                           .Where(x => x.EndDate >= filter.endCourseFrom && x.EndDate <= filter.endtCourseTo.Value.AddDays(1))
                           .ToList();
            }
            if (filter.subscribeDateFrom != null && filter.subscribeDateTo != null)
            {
                filteredCourses = filteredCourses
                           .Where(x => x.SubscribeDate >= filter.subscribeDateFrom && x.SubscribeDate <= filter.subscribeDateTo)
                           .ToList();
            }
            if (filter.examTheoryDateFrom != null && filter.examTheoryDateTo != null)
            {
                filteredCourses = filteredCourses
                           .Where(x => x.ExamTheoryDate >= filter.examTheoryDateFrom && x.ExamTheoryDate <= filter.examTheoryDateTo)
                           .ToList();
            }
            if (filter.examPracticeDateFrom != null && filter.examPracticeDateTo != null)
            {
                filteredCourses = filteredCourses
                           .Where(x => x.ExamPracticeDate >= filter.examPracticeDateFrom && x.ExamPracticeDate <= filter.examPracticeDateTo)
                           .ToList();
            }

            return filteredCourses;
        }

        public async Task<List<ClientVM>> FilterClients(ClientFilterVM filterModel)
        {

            var filter = PredicateBuilder.True<Client>();

            //List<ClientVM> filteredClients = (this.repository
            //     .All<Client>()
            // .To<ClientVM>(x => x.CandidateProvider.Location.Municipality.District))
            //     .ToList();

            if (filterModel.IdCandidate_Provider != 0)
            {
                filter = filter
                    .And(x => x.CandidateProvider.IdCandidate_Provider == filterModel.IdCandidate_Provider);
            }

            if ( !string.IsNullOrEmpty(filterModel.LicenceNumber))
            {
                filter = filter
                    .And(x => x.CandidateProvider.LicenceNumber.Contains(filterModel.LicenceNumber));
            }

            if (!string.IsNullOrEmpty(filterModel.FirstName))
            {
                filter = filter
                    .And(x => x.FirstName.ToLower().Contains(filterModel.FirstName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filterModel.FamilyName))
            {
                filter = filter
                    .And(x => x.FamilyName.ToLower().Contains(filterModel.FamilyName.ToLower()));
            }

            if (filterModel.idLocation != 0)
            {
                filter = filter
                    .And(x => x.CandidateProvider.Location.idLocation == filterModel.idLocation);
            }

            if (filterModel.idMunicipality != 0)
            {
                filter = filter
                    .And(x => x.CandidateProvider.Location.Municipality.idMunicipality == filterModel.idMunicipality);
            }

            if (filterModel.idDistrict != 0)
            {
                filter = filter
                    .And(x => x.CandidateProvider.Location.Municipality.District.idDistrict == filterModel.idDistrict);
            }

            if (!string.IsNullOrEmpty(filterModel.Indent))
            {
                filter = filter
                    .And(x => x.Indent.Contains(filterModel.Indent));
            }

            if (filterModel.IdNationality != null)
            {
                filter = filter
                    .And(x => x.IdNationality == filterModel.IdNationality);
            }

            if (filterModel.IdSex != null)
            {
                filter = filter
                    .And(x => x.IdSex == filterModel.IdSex);
            }

            if(filterModel.IdIndentType.HasValue)
            {
                filter = filter
                    .And(x => x.IdIndentType == filterModel.IdIndentType);
            }

            if (filterModel.IdTypeLicense.HasValue)
            {
                filter = filter
                    .And(x => x.CandidateProvider.IdTypeLicense == filterModel.IdTypeLicense);
            }

            return await this.repository
                .AllReadonly<Client>(filter)
                .To<ClientVM>(x => x.CandidateProvider.Location.Municipality.District)
                .ToListAsync();

        }

        public async Task<MemoryStream> getCoursesReportStream(CoursesFilterVM filter)
        {
            var courses = await GetAllCoursesAsync(filter);

            var CPOWithoutCourse = await GetCPOWithOutCourseByPeriod(filter);

            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                int row = 1;

                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Xlsx;

                IWorkbook workbook = application.Workbooks.Create(2);
                IWorksheet courseSheet = workbook.Worksheets[0];
                IWorksheet specialitySheet = workbook.Worksheets[1];

                courseSheet.Name = "Проведени обучения";
                specialitySheet.Name = "Специалности, без обучения";

                if (filter.IdCandidateProvider != 0)
                {
                    CPOWithoutCourse = CPOWithoutCourse.Where(x => x.IdCandidate_Provider == filter.IdCandidateProvider).ToList();
                }
                if (courses.Count() > 0)
                    courseSheet.Range[$"A1:F{courses.Count()}"].ColumnWidth = 45;

                object[] firstCourseSheetRow = new object[6]
                {
                    "Юридическо лице",
                    "Пофесия",
                    "Специалност",
                    "Курс",
                    "Начална дата на провеждане",
                    "Крайна дата на провеждане"
                };
                courseSheet.ImportArray(firstCourseSheetRow.ToArray(), row, 1, false);
                row++;
                foreach (var course in courses)
                {
                    object[] courseSheetRow = new object[6]
                    {
                        course.CandidateProvider.ProviderOwner,
                        course.Program.Speciality.Profession.CodeAndName,
                        course.Program.Speciality.CodeAndName,
                        course.CourseName,
                        course.StartDate.ToString(),
                        course.EndDate.ToString()
                    };

                    courseSheet.ImportArray(courseSheetRow.ToArray(), row, 1, false);
                    row++;
                }

                if (CPOWithoutCourse.Count() > 0)
                    specialitySheet.Range[$"A1:C{CPOWithoutCourse.Count()}"].ColumnWidth = 25;

                row = 1;

                object[] firstSpecialitySheetRow = new object[3]
                  {
                       "Юридическо лице",
                       "Професия",
                       "Специалност"
                  };
                specialitySheet.ImportArray(firstSpecialitySheetRow.ToArray(), row, 1, false);
                row++;

                foreach (var course in CPOWithoutCourse)
                {
                    object[] specialitySheetRow = new object[3]
                    {
                        course.ProviderName,
                        course.ProfessionName,
                        course.SpecialityName
                    };

                    specialitySheet.ImportArray(specialitySheetRow.ToArray(), row, 1, false);
                    row++;
                }

                if (courses.Count() > 0)
                {
                    courseSheet.Range[$"A1:F{courses.Count()}"].AutofitColumns();
                    courseSheet.Range[$"A1:F{courses.Count()}"].BorderInside(ExcelLineStyle.Medium);
                    courseSheet.Range[$"A1:F{courses.Count()}"].BorderAround(ExcelLineStyle.Medium);
                }
                if (courses.Count() > 0)
                {
                    specialitySheet.Range[$"A1:C{CPOWithoutCourse.Count()}"].AutofitColumns();
                    specialitySheet.Range[$"A1:C{CPOWithoutCourse.Count()}"].BorderInside(ExcelLineStyle.Medium);
                    specialitySheet.Range[$"A1:C{CPOWithoutCourse.Count()}"].BorderAround(ExcelLineStyle.Medium);
                }

                MemoryStream stream = new MemoryStream();

                workbook.SaveAs(stream);

                return stream;
            }
        }

        public async Task<List<GetCPOWithOutCourseByPeriod>> GetCPOWithOutCourseByPeriod(CoursesFilterVM filter)
        {
            //var data = this.repository.ExecuteSQL<GetCPOWithOutCourseByPeriod>(
            //        "EXECUTE GetCPOWithOutCourseByPeriod {0}, {1}, {2}, {3}",
            //        new object[4] { startDate, startDateEnd, endDate, endDateEnd }).ToList();

            var professions = this.dataSourceService.GetAllProfessionsList();

            var filterSpecialities = PredicateBuilder.True<CandidateProviderSpeciality>();

            if (filter.IdCandidateProvider != 0)
            {
                filterSpecialities = filterSpecialities.And(x => x.IdCandidate_Provider == filter.IdCandidateProvider);
            }

            if (!string.IsNullOrEmpty(filter.LicenceNumber))
            {
                filterSpecialities = filterSpecialities.And(x => !string.IsNullOrEmpty(x.CandidateProvider.LicenceNumber));
                filterSpecialities = filterSpecialities.And(x => x.CandidateProvider.LicenceNumber!.Contains(filter.LicenceNumber));
            }

            filterSpecialities = filterSpecialities.And(x => !x.Speciality.Programs.Any(p => p.Courses.Any(c => c.StartDate >= filter.StartFrom && c.StartDate <= filter.StartTo && c.EndDate >= filter.EndFrom && c.StartDate <= filter.EndTo)));

            var specialities = this.repository.AllReadonly<CandidateProviderSpeciality>(filterSpecialities)
                .To<CandidateProviderSpecialityVM>(x => x.Speciality.Profession, x => x.CandidateProvider).ToList();

            var data = new List<GetCPOWithOutCourseByPeriod>();

            foreach (var speciality in specialities)
            {
                    var CPOWithoutCourse = new GetCPOWithOutCourseByPeriod();

                    CPOWithoutCourse.IdSpeciality = speciality.IdSpeciality;

                    CPOWithoutCourse.ProviderName = speciality.CandidateProvider.ProviderOwner;

                    CPOWithoutCourse.ProfessionName = speciality.Speciality.Profession.Name;

                    CPOWithoutCourse.SpecialityName = speciality.Speciality.Name;

                    data.Add(CPOWithoutCourse);
            }

            return data;
        }

        public async Task<List<ClientCourseVM>> GetCourseClientsByIdCourseStatisticsAsync(int idCourse)
        {
            var data = await this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == idCourse).To<ClientCourseVM>(x => x.Course.Program.FrameworkProgram, x => x.Client).ToListAsync();

            var male = await this.dataSourceService.GetKeyValueByIntCodeAsync("Sex", "Man");

            var bulgarianCitizenship = await this.dataSourceService.GetKeyValueByIntCodeAsync("Nationality", "Bulgaria");

            var statistic = new ClientCourseVM();

            foreach (var cc in data)
            {
                if (cc.BirthDate != null)
                {
                    var year = DateTime.Today.Year - cc.BirthDate.Value.Year;

                    if (year < 16)
                        statistic.to16 = 1;
                    else if (year >= 16 && year <= 29)
                        statistic.between16and29 += 1;
                    else if (year >= 30 && year <= 45)
                        statistic.between30and45 += 1;
                    else if (year >= 46 && year <= 60)
                        statistic.between46and60 += 1;
                    else if (year >= 61 && year <= 80)
                        statistic.between61and80 += 1;
                    else
                        statistic.moreThan80 += 1;

                    if (male.IdKeyValue == cc.IdSex)
                        statistic.male += 1;
                    else
                        statistic.female += 1;
                }

                if (cc.Client.IdNationality == bulgarianCitizenship.IdKeyValue)
                    statistic.bulgarian += 1;
                else
                    statistic.otherNation += 1;

                if (cc.IsDisadvantagedPerson)
                    statistic.disabled += 1;
            }

            if (data.Any())
            {
                if (data.First().IdSpeciality.HasValue)
                {
                    statistic.Speciality = specialityService.GetSpecialityById(data.First().IdSpeciality.Value);
                }

                if (statistic.Speciality is not null)
                {
                    var vqs = await dataSourceService.GetKeyValueByIdAsync(statistic.Speciality.IdVQS);
                    if (vqs is not null)
                    {
                        statistic.Speciality.VQS = vqs;
                    }
                }
                
                statistic.Course = data.First().Course;

                var trainingCourseTypeName = await dataSourceService.GetKeyValueByIdAsync(data.First().Course.IdTrainingCourseType);
                if (trainingCourseTypeName is not null)
                {
                    statistic.Course.TrainingCourseTypeName = trainingCourseTypeName.Name;
                }
            }

            return new List<ClientCourseVM>() { statistic };
        }

        public async Task SetDocumentStatusForListDocumentsFromCPORegisterVMAsync(IEnumerable<DocumentsFromCPORegisterVM> docs)
        {
            var documentStatusTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientDocumentStatusType", false, true);
            foreach (var doc in docs)
            {
                if (doc.IsCourse)
                {
                    var docFromDb = await this.repository.GetByIdAsync<ClientCourseDocument>(doc.IdEntity);
                    if (docFromDb is not null && docFromDb.IdDocumentStatus.HasValue)
                    {
                        doc.Status = documentStatusTypesSource.FirstOrDefault(x => x.IdKeyValue == docFromDb.IdDocumentStatus.Value)?.Name ?? string.Empty;
                    }
                }
                else
                {
                    var docFromDb = await this.repository.GetByIdAsync<ValidationClientDocument>(doc.IdEntity);
                    if (docFromDb is not null && docFromDb.IdDocumentStatus.HasValue)
                    {
                        doc.Status = documentStatusTypesSource.FirstOrDefault(x => x.IdKeyValue == docFromDb.IdDocumentStatus.Value)?.Name ?? string.Empty;
                    }
                }
            }
        }
        #endregion Reports

        #region Validation
        public async Task<List<ValidationClientVM>> getAllValidationClients()
        {
            var clients = this.repository.All<ValidationClient>()
                //.To<ValidationClientVM>(x => x.FrameworkProgram, x => x.CandidateProvider.Location.Municipality.Regions, x => x.CandidateProvider, x => x.Speciality.Profession, x => x.Client, x => x.ValidationClientDocuments)
                .To<ValidationClientVM>(x => x.Speciality.Profession, x => x.Client)
                .ToList();
            var kvSource = dataSourceService.GetAllKeyValueList();
            foreach (var client in clients)
            {
                if (client.IdCourseType != 0)
                {
                    client.CourseType = await dataSourceService.GetKeyValueByIdAsync(client.IdCourseType);
                    client.FinishType = await dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);

                }

                if (client.IdSpeciality != 0 && client.IdSpeciality != null)
                {
                    string VQS_Name = kvSource.FirstOrDefault(c => c.IdKeyValue == client.Speciality.IdVQS).Name;
                    client.Speciality.CodeAndNameAndVQS = client.Speciality.CodeAndName + " - " + VQS_Name;
                }
            }

            return clients.OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ToList();
        }

        public async Task<ValidationClientVM> CreateValidationClient(ValidationClientVM clientVM)
        {
            try
            {
                var ClientExist = await FilterClients(new ClientFilterVM() { Indent = clientVM.Indent, IdIndentType = clientVM.IdIndentType, IdCandidate_Provider = clientVM.IdCandidateProvider });

                if (ClientExist.Count == 0)
                {
                    Client client = new Client()
                    {
                        IdCandidateProvider = clientVM.IdCandidateProvider,
                        FirstName = clientVM.FirstName,
                        SecondName = clientVM.SecondName,
                        FamilyName = clientVM.FamilyName,
                        IdSex = clientVM.IdSex,
                        IdIndentType = clientVM.IdIndentType,
                        Indent = clientVM.Indent,
                        BirthDate = clientVM.BirthDate,
                        IdNationality = clientVM.IdNationality,
                        IdCountryOfBirth = clientVM.IdCountryOfBirth,
                        IdCityOfBirth = clientVM.IdCityOfBirth,
                        FamilyNameEN = BaseHelper.ConvertCyrToLatin(clientVM.FamilyName),
                        FirstNameEN = BaseHelper.ConvertCyrToLatin(clientVM.FirstName)
                    };
                    clientVM.UploadedFileName = String.Empty;
                    await this.repository.AddAsync(client);
                    await this.repository.SaveChangesAsync();
                    clientVM.IdClient = client.IdClient;
                }
                else
                    clientVM.IdClient = ClientExist.First().IdClient;

                clientVM.IdStatus = this.kvCurrentCourse.IdKeyValue;
                var savedClient = clientVM.To<ValidationClient>();

                await this.repository.AddAsync(savedClient);

                await this.repository.SaveChangesAsync();

                return savedClient.To<ValidationClientVM>();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<ValidationClientVM> GetValidationClientByIdAsync(int id)
        {

            return this.repository.All<ValidationClient>()
                .Where(x => x.IdValidationClient == id)
                .To<ValidationClientVM>(x => x.FrameworkProgram,
                x => x.Speciality,
                x => x.Speciality.Profession.DocVMList,
                x => x.CandidateProvider.Location.Municipality.Regions,
                x => x.CandidateProvider,
                x => x.Client,
                x => x.ValidationCommissionMembers,
                x => x.ValidationClientDocuments).FirstOrDefault();
        }

        public async Task<ValidationClientVM> GetValidationClientWithoutIncludesByIdAsync(int idValidationClient)
        {
            return await this.repository.AllReadonly<ValidationClient>(x => x.IdValidationClient == idValidationClient).To<ValidationClientVM>().FirstOrDefaultAsync();
        }

        public async Task<ValidationClientVM> UpdateValidationClientAsync(ValidationClientVM clientVM)
        {
            var updateClient = clientVM.To<ValidationClient>();

            updateClient.Speciality = null;
            updateClient.FrameworkProgram = null;
            updateClient.CandidateProvider = null;
            this.repository.Update(updateClient);

            await this.repository.SaveChangesAsync();

            var CandidateProviderSpeciality = this.repository.All<CandidateProviderSpeciality>()
                .Where(x => x.IdCandidate_Provider == this.UserProps.IdCandidateProvider && x.IdSpeciality == clientVM.IdSpeciality)
                .To<CandidateProviderSpecialityVM>().First();


            if (!this.repository.All<ValidationCurriculum>().Where(x => x.IdValidationClient == clientVM.IdValidationClient).Any())
            {
                var CandidateCurrics = await GetActualCandidateCurriculumByIdCandidateProviderAndIdSpecialityAsync(clientVM.IdCandidateProvider, clientVM.IdSpeciality.Value);

                var curricList = new List<ValidationCurriculum>();

                foreach (var cc in CandidateCurrics)
                {
                    ValidationCurriculum validationCurriculum = new ValidationCurriculum()
                    {
                        IdCandidateProviderSpeciality = CandidateProviderSpeciality.IdCandidateProviderSpeciality,
                        IdValidationClient = clientVM.IdValidationClient,
                        IdProfessionalTraining = cc.IdProfessionalTraining,
                        Subject = cc.Subject,
                        Topic = cc.Topic,
                        Practice = cc.Practice,
                        Theory = cc.Theory
                    };

                    foreach (var ERU in cc.CandidateCurriculumERUs)
                    {
                        validationCurriculum.ValidationCurriculumERUs.Add(new ValidationCurriculumERU()
                        {
                            IdERU = ERU.IdERU
                        });
                    }

                    curricList.Add(validationCurriculum);

                }

                await this.repository.AddRangeAsync(curricList);
                await this.repository.SaveChangesAsync();
            }
            return updateClient.To<ValidationClientVM>();
        }

        public async Task<IEnumerable<ValidationCommissionMemberVM>> GetAllValidationCommissionMembersByClient(int idValidationClient)
        {
            return await this.repository.All<ValidationCommissionMember>()
                .Where(x => x.IdValidationClient == idValidationClient)
                .AsNoTracking()
                .To<ValidationCommissionMemberVM>()
                .ToListAsync();
        }

        public async Task<IEnumerable<ValidationCommissionMemberVM>> GetAllValidationCommissionChairmansByClient(int idValidationClient)
        {
            return await this.repository.AllReadonly<ValidationCommissionMember>()
                .Where(x => x.IdValidationClient == idValidationClient && x.IsChairman)
                .AsNoTracking()
                .To<ValidationCommissionMemberVM>()
                .ToListAsync();
        }

        public async Task<ResultContext<ValidationCommissionMemberVM>> CreateValidationCommissionMemberAsync(ResultContext<ValidationCommissionMemberVM> result)
        {
            var model = result.ResultContextObject;
            try
            {
                var modelForDb = model.To<ValidationCommissionMember>();

                await this.repository.AddAsync<ValidationCommissionMember>(modelForDb);
                await this.repository.SaveChangesAsync();

                model.IdValidationCommissionMember = modelForDb.IdValidationCommissionMember;

                result.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return result;
        }

        public async Task<bool> DeleteValidationClientCommissionMemberByIdAsync(int id)
        {
            try
            {
                var entity = await this.repository.GetByIdAsync<ValidationCommissionMember>(id);
                if (entity is not null)
                {
                    await this.repository.HardDeleteAsync<ValidationCommissionMember>(entity.IdValidationCommissionMember);
                    await this.repository.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task<IEnumerable<ValidationTrainerVM>> GetAllValidationTrainersByIdValidationClientAsync(int idValidationClient)
        {
            var data = this.repository.AllReadonly<ValidationTrainer>(x => x.IdValidationClient == idValidationClient);
            var dataAsVM = await data.To<ValidationTrainerVM>(x => x.CandidateProviderTrainer).ToListAsync();

            var trainingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
            foreach (var validationTrainer in dataAsVM)
            {
                var type = trainingTypeSource.FirstOrDefault(x => x.IdKeyValue == validationTrainer.IdТrainingType);
                if (type is not null)
                {
                    validationTrainer.TrainingTypeName = type.Name;
                }
            }

            return dataAsVM.OrderBy(x => x.IdТrainingType).ThenBy(x => x.CandidateProviderTrainer.FirstName).ThenBy(x => x.CandidateProviderTrainer.FamilyName).ToList();
        }

        public async Task<ResultContext<ValidationTrainerVM>> DeleteValidationTrainerAsync(ResultContext<ValidationTrainerVM> result)
        {
            var model = result.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<ValidationTrainer>(model.IdValidationTrainer);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<ValidationTrainer>(entryFromDb.IdValidationTrainer);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return result;
        }

        public async Task<ResultContext<List<CandidateProviderTrainerVM>>> CreateValidationTrainerByListCandidateProviderTrainerVMAsync(ResultContext<List<CandidateProviderTrainerVM>> resultContext, int idValidationClient, int idTrainingType)
        {
            var list = resultContext.ResultContextObject;
            try
            {
                foreach (var trainer in list)
                {
                    ValidationTrainer premisesCourse = new ValidationTrainer()
                    {
                        IdTrainer = trainer.IdCandidateProviderTrainer,
                        IdValidationClient = idValidationClient,
                        IdТrainingType = idTrainingType
                    };

                    await this.repository.AddAsync<ValidationTrainer>(premisesCourse);
                }

                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<List<ValidationPremisesVM>> GetAllValidationPremisiesByIdCourseAsync(int idValidationClient)
        {
            var data = this.repository.AllReadonly<ValidationPremises>(x => x.IdValidationClient == idValidationClient);
            var dataAsVM = data.To<ValidationPremisesVM>(x => x.CandidateProviderPremises.Location).ToList();

            var trainingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
            foreach (var validationPremises in dataAsVM)
            {
                var type = trainingTypeSource.FirstOrDefault(x => x.IdKeyValue == validationPremises.IdТrainingType);
                if (type is not null)
                {
                    validationPremises.TrainingTypeName = type.Name;
                }
            }

            return dataAsVM.OrderBy(x => x.IdТrainingType).ThenBy(x => x.CandidateProviderPremises.PremisesName).ToList();

        }

        public async Task<ResultContext<ValidationPremisesVM>> DeleteValidationPremisesAsync(ResultContext<ValidationPremisesVM> result)
        {
            var model = result.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<ValidationPremises>(model.IdValidationPremises);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<ValidationPremises>(entryFromDb.IdValidationPremises);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return result;
        }

        public async Task<ResultContext<List<CandidateProviderPremisesVM>>> CreateTrainingValidationPremisesByListCandidateProviderPremisesVMAsync(ResultContext<List<CandidateProviderPremisesVM>> resultContext, int idValidationClient, int idTrainingType)
        {
            var list = resultContext.ResultContextObject;
            try
            {
                foreach (var premises in list)
                {
                    ValidationPremises validationPremises = new ValidationPremises()
                    {
                        IdPremises = premises.IdCandidateProviderPremises,
                        IdValidationClient = idValidationClient,
                        IdТrainingType = idTrainingType
                    };

                    await this.repository.AddAsync<ValidationPremises>(validationPremises);
                }

                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ValidationClientCombinedVM> GetValidationClientCombinedVMByIdClientCourseAsync(int idValidationClient)
        {
            var model = new ValidationClientCombinedVM();

            var validationClientFromDb = await this.repository.AllReadonly<ValidationClient>(x => x.IdValidationClient == idValidationClient).AsNoTracking().FirstOrDefaultAsync();
            if (validationClientFromDb is not null)
            {
                var validationClientDocumentFromDb = await this.repository.AllReadonly<ValidationClientDocument>(x => x.IdValidationClient == validationClientFromDb.IdValidationClient)
                    .Include(x => x.DocumentSerialNumber)
                        .ThenInclude(x => x.TypeOfRequestedDocument)
                            .AsNoTracking()
                                .FirstOrDefaultAsync();

                var validationDocumentUploadedFileFromDb = new List<ValidationDocumentUploadedFileVM>();
                if (validationClientDocumentFromDb is not null)
                {
                    validationDocumentUploadedFileFromDb = await this.repository.AllReadonly<ValidationDocumentUploadedFile>(x => x.IdValidationClientDocument == validationClientDocumentFromDb.IdValidationClientDocument).AsNoTracking().To<ValidationDocumentUploadedFileVM>().ToListAsync();
                }

                model.IdValidationClient = validationClientFromDb.IdValidationClient;
                model.FinishedDate = validationClientFromDb.EndDate;
                model.IdFinishedType = validationClientFromDb.IdFinishedType;
                model.IdDocumentType = validationClientFromDb.IdCourseType;

                model.DocumentTypeName = validationClientFromDb.IdCourseType != this.kvProfessionalValidationForAcquiringTheSPK.IdKeyValue ? "Удостоверение за валидиране на професионална квалификация по част от професия" : "Свидетелство за валидиране на професионална квалификация";
                var typeOfRequestedDoc = await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.IdCourseType == validationClientFromDb.IdCourseType && x.IsValid).AsNoTracking().FirstOrDefaultAsync();
                if (typeOfRequestedDoc is not null)
                {
                    model.HasDocumentFabricNumber = typeOfRequestedDoc.HasSerialNumber;
                }

                if (validationClientDocumentFromDb is not null)
                {
                    model.IdDocumentStatus = validationClientDocumentFromDb.IdDocumentStatus;
                    model.IdValidationClientDocument = validationClientDocumentFromDb.IdValidationClientDocument;
                    model.IdDocumentType = validationClientDocumentFromDb.DocumentSerialNumber != null ? validationClientDocumentFromDb.DocumentSerialNumber.TypeOfRequestedDocument.IdTypeOfRequestedDocument : null;
                    model.FinishedYear = validationClientDocumentFromDb.FinishedYear;
                    model.IdDocumentSerialNumber = validationClientDocumentFromDb.IdDocumentSerialNumber;
                    model.DocumentSerialNumber = validationClientDocumentFromDb.DocumentSerialNumber != null ? validationClientDocumentFromDb.DocumentSerialNumber.To<DocumentSerialNumberVM>() : null;
                    model.DocumentRegNo = validationClientDocumentFromDb.DocumentRegNo;
                    model.DocumentDate = validationClientDocumentFromDb.DocumentDate;
                    //model.TheoryResult = validationClientDocumentFromDb.TheoryResult.HasValue ? validationClientDocumentFromDb.TheoryResult.Value.ToString() : string.Empty;
                    //model.PracticeResult = validationClientDocumentFromDb.PracticeResult.HasValue ? validationClientDocumentFromDb.PracticeResult.Value.ToString() : string.Empty;
                    model.FinalResult = validationClientDocumentFromDb.FinalResult.HasValue ? validationClientDocumentFromDb.FinalResult.Value.ToString() : string.Empty;
                    model.IdValidationProtocol = validationClientDocumentFromDb.IdValidationProtocol;
                    model.DocumentProtocol = validationClientDocumentFromDb.DocumentProtocol;
                }

                if (validationDocumentUploadedFileFromDb.Any())
                {
                    model.IdValidationDocumentUploadedFile = validationDocumentUploadedFileFromDb.First().IdValidationDocumentUploadedFile;
                    model.UploadedFileName = validationDocumentUploadedFileFromDb.First().UploadedFileName;

                    model.DocumentUploadedFiles.AddRange(validationDocumentUploadedFileFromDb);
                }
            }

            return model;
        }

        public async Task<ValidationClientCombinedVM> GetValidationClientDuplicateCombinedVMByIdClientCourseAsync(int idValidationClient)
        {
            var model = new ValidationClientCombinedVM();

            var clientCourseFromDb = await this.repository.AllReadonly<ValidationClient>(x => x.IdValidationClient == idValidationClient).FirstOrDefaultAsync();
            if (clientCourseFromDb is not null)
            {
                var kvIssueOfDuplicate = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "IssueOfDuplicate");
                var kvPartProfessionType = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ValidationOfPartOfProfession");
                var clientCourseDocumentFromDb = await this.repository.AllReadonly<ValidationClientDocument>(x => x.IdValidationClient == clientCourseFromDb.IdValidationClient && x.IdDocumentType == kvIssueOfDuplicate.IdKeyValue)
                    .Include(x => x.DocumentSerialNumber)
                        .ThenInclude(x => x.TypeOfRequestedDocument)
                            .AsNoTracking()
                    .FirstOrDefaultAsync();

                var firstClientCourseDocumentFromDb = await this.repository.AllReadonly<ValidationClientDocument>(x => x.IdValidationClient == clientCourseFromDb.IdValidationClient && x.IdDocumentType == clientCourseFromDb.IdCourseType)
                    .FirstOrDefaultAsync();

                ValidationDocumentUploadedFile courseDocumentUploadedFileFromDb = null;
                if (clientCourseDocumentFromDb is not null)
                {
                    courseDocumentUploadedFileFromDb = await this.repository.AllReadonly<ValidationDocumentUploadedFile>(x => x.IdValidationClientDocument == clientCourseDocumentFromDb.IdValidationClientDocument).FirstOrDefaultAsync();
                }

                model.IdValidationClient = clientCourseFromDb.IdValidationClient;
                model.FinishedDate = clientCourseFromDb.EndDate;
                model.IdFinishedType = clientCourseFromDb.IdFinishedType;
                model.IdDocumentType = kvIssueOfDuplicate.IdKeyValue;

                var typeOfReqDoc = clientCourseFromDb.IdCourseType == kvPartProfessionType.IdKeyValue
                    ? await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == "3-37В" && x.IsValid).FirstOrDefaultAsync()
                    : await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == "3-54аВ" && x.IsValid).FirstOrDefaultAsync();
                if (typeOfReqDoc is not null)
                {
                    model.DocumentTypeName = typeOfReqDoc.DocTypeName!;
                    model.HasDocumentFabricNumber = typeOfReqDoc.HasSerialNumber;
                }

                if (clientCourseDocumentFromDb is null && firstClientCourseDocumentFromDb is not null)
                {
                    //model.TheoryResult = firstClientCourseDocumentFromDb.TheoryResult.HasValue ? firstClientCourseDocumentFromDb.TheoryResult.Value.ToString() : string.Empty;
                    //model.PracticeResult = firstClientCourseDocumentFromDb.PracticeResult.HasValue ? firstClientCourseDocumentFromDb.PracticeResult.Value.ToString() : string.Empty;
                    model.FinalResult = firstClientCourseDocumentFromDb.FinalResult.HasValue ? firstClientCourseDocumentFromDb.FinalResult.Value.ToString() : string.Empty;
                    model.IdValidationProtocol = firstClientCourseDocumentFromDb.IdValidationProtocol;
                    model.DocumentProtocol = firstClientCourseDocumentFromDb.DocumentProtocol;
                }

                if (clientCourseDocumentFromDb is not null)
                {
                    model.IdValidationClientDocument = clientCourseDocumentFromDb.IdValidationClientDocument;
                    model.IdDuplicateDocumentStatus = clientCourseDocumentFromDb.IdDocumentStatus;
                    if (clientCourseDocumentFromDb.DocumentSerialNumber is not null && clientCourseDocumentFromDb.DocumentSerialNumber.TypeOfRequestedDocument is not null)
                    {
                        model.IdDocumentType = clientCourseDocumentFromDb.DocumentSerialNumber.TypeOfRequestedDocument.IdTypeOfRequestedDocument;
                    }

                    model.FinishedYear = clientCourseDocumentFromDb.FinishedYear;
                    model.IdDocumentSerialNumber = clientCourseDocumentFromDb.IdDocumentSerialNumber;
                    if (clientCourseDocumentFromDb.DocumentSerialNumber is not null)
                    {
                        model.DocumentSerialNumber = clientCourseDocumentFromDb.DocumentSerialNumber.To<DocumentSerialNumberVM>();
                    }

                    model.DocumentRegNo = clientCourseDocumentFromDb.DocumentRegNo;
                    model.DocumentDate = clientCourseDocumentFromDb.DocumentDate;
                    //model.TheoryResult = clientCourseDocumentFromDb.TheoryResult.HasValue ? clientCourseDocumentFromDb.TheoryResult.Value.ToString() : string.Empty;
                    //model.PracticeResult = clientCourseDocumentFromDb.PracticeResult.HasValue ? clientCourseDocumentFromDb.PracticeResult.Value.ToString() : string.Empty;
                    model.FinalResult = clientCourseDocumentFromDb.FinalResult.HasValue ? clientCourseDocumentFromDb.FinalResult.Value.ToString() : string.Empty;
                    model.IdValidationProtocol = clientCourseDocumentFromDb.IdValidationProtocol;
                    model.DocumentProtocol = clientCourseDocumentFromDb.DocumentProtocol;
                }

                if (courseDocumentUploadedFileFromDb is not null)
                {
                    model.IdValidationDocumentUploadedFile = courseDocumentUploadedFileFromDb.IdValidationDocumentUploadedFile;
                    model.UploadedFileName = courseDocumentUploadedFileFromDb.UploadedFileName;
                }
            }

            return model;
        }

        public async Task<IEnumerable<ValidationProtocolVM>> GetAll381BProtocolsAddedByIdValidationClientAsync(int idValidationClient)
        {

            var kvProtocol381 = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseProtocolType", "3-81B");
            var data = await this.repository.AllReadonly<ValidationProtocol>(x => x.IdValidationClient == idValidationClient && x.IdValidationProtocolType == kvProtocol381.IdKeyValue)
                .AsNoTracking()
                .To<ValidationProtocolVM>()
                .ToListAsync();

            return data.OrderByDescending(x => x.ValidationProtocolDate.Value);
        }

        public async Task<int> UpdateValidationDuplicateDocumentProtocolAsync(ValidationClientCombinedVM ValidationClientModel)
        {
            try
            {
                var docStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
                var kvDocPrinted = docStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Printed");
                var kvDocumentStatusNotSubmitted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "NotSubmitted");
                var validationClient = await this.repository.GetByIdAsync<ValidationClient>(ValidationClientModel.IdValidationClient);
                var kvPartOfProfession = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ValidationOfPartOfProfession");

                var docSerialNumberReceivedFromNewEntity = await this.repository.GetByIdAsync<DocumentSerialNumber>(ValidationClientModel.IdDocumentSerialNumber);
                if (ValidationClientModel.IdValidationClientDocument == 0)
                {
                    if (docSerialNumberReceivedFromNewEntity is not null)
                    {
                        RequestDocumentManagement requestDocumentManagement = new RequestDocumentManagement()
                        {
                            IdCandidateProvider = this.UserProps.IdCandidateProvider,
                            IdTypeOfRequestedDocument = docSerialNumberReceivedFromNewEntity.IdTypeOfRequestedDocument,
                            DocumentCount = 1,
                            DocumentDate = DateTime.Now,
                            IdDocumentOperation = kvDocPrinted.IdKeyValue,
                            ReceiveDocumentYear = docSerialNumberReceivedFromNewEntity.ReceiveDocumentYear
                        };

                        await this.repository.AddAsync<RequestDocumentManagement>(requestDocumentManagement);
                        await this.repository.SaveChangesAsync();

                        DocumentSerialNumber documentSerialNumber = new DocumentSerialNumber()
                        {
                            IdRequestDocumentManagement = requestDocumentManagement.IdRequestDocumentManagement,
                            IdCandidateProvider = this.UserProps.IdCandidateProvider,
                            IdTypeOfRequestedDocument = docSerialNumberReceivedFromNewEntity.IdTypeOfRequestedDocument,
                            DocumentDate = DateTime.Now,
                            SerialNumber = docSerialNumberReceivedFromNewEntity.SerialNumber,
                            IdDocumentOperation = kvDocPrinted.IdKeyValue,
                            ReceiveDocumentYear = docSerialNumberReceivedFromNewEntity.ReceiveDocumentYear
                        };

                        await this.repository.AddAsync<DocumentSerialNumber>(documentSerialNumber);
                        await this.repository.SaveChangesAsync();

                        var typeOfReqDoc = await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.IdCourseType == validationClient.IdCourseType && x.IsValid).FirstOrDefaultAsync();
                        var docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == typeOfReqDoc.IdTypeOfRequestedDocument && x.Year == ValidationClientModel.FinishedYear).FirstOrDefaultAsync();
                        ValidationClientDocument clientCourseDocument = new ValidationClientDocument()
                        {
                            IdValidationClient = ValidationClientModel.IdValidationClient,
                            IdDocumentType = ValidationClientModel.IdDocumentType,
                            FinishedYear = ValidationClientModel.FinishedYear,
                            DocumentRegNo = ValidationClientModel.DocumentRegNo,
                            IdDocumentSerialNumber = documentSerialNumber.IdDocumentSerialNumber,
                            DocumentDate = ValidationClientModel.DocumentDate,
                            DocumentProtocol = ValidationClientModel.DocumentProtocol,
                            //TheoryResult = !string.IsNullOrEmpty(ValidationClientModel.TheoryResult) ? decimal.Parse(ValidationClientModel.TheoryResult) : null,
                            //PracticeResult = !string.IsNullOrEmpty(ValidationClientModel.PracticeResult) ? decimal.Parse(ValidationClientModel.PracticeResult) : null,
                            FinalResult = !string.IsNullOrEmpty(ValidationClientModel.FinalResult) ? decimal.Parse(ValidationClientModel.FinalResult) : null,
                            IdValidationProtocol = ValidationClientModel.IdValidationProtocol,
                            IdTypeOfRequestedDocument = typeOfReqDoc!.IdTypeOfRequestedDocument,
                            IdDocumentStatus = kvDocumentStatusNotSubmitted.IdKeyValue,
                            DocumentPrnNo = $"{docSeries?.SeriesName}/{documentSerialNumber.SerialNumber}",
                            DocumentSerNo = docSeries?.SeriesName
                        };

                        await this.repository.AddAsync<ValidationClientDocument>(clientCourseDocument);
                        await this.repository.SaveChangesAsync();

                        ValidationClientModel.IdValidationClientDocument = clientCourseDocument.IdValidationClientDocument;

                        await this.AddValidationClientDocumentStatusAsync(clientCourseDocument.IdValidationClientDocument, kvDocumentStatusNotSubmitted.IdKeyValue);
                    }
                    else
                    {
                        var typeOfReqDoc = await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.IdCourseType == validationClient.IdCourseType && x.IsValid).FirstOrDefaultAsync();
                        ValidationClientDocument clientCourseDocument = new ValidationClientDocument()
                        {
                            IdValidationClient = ValidationClientModel.IdValidationClient,
                            IdDocumentType = ValidationClientModel.IdDocumentType,
                            FinishedYear = ValidationClientModel.FinishedYear,
                            DocumentRegNo = ValidationClientModel.DocumentRegNo,
                            DocumentDate = ValidationClientModel.DocumentDate,
                            DocumentProtocol = ValidationClientModel.DocumentProtocol,
                            //TheoryResult = !string.IsNullOrEmpty(ValidationClientModel.TheoryResult) ? decimal.Parse(ValidationClientModel.TheoryResult) : null,
                            //PracticeResult = !string.IsNullOrEmpty(ValidationClientModel.PracticeResult) ? decimal.Parse(ValidationClientModel.PracticeResult) : null,
                            FinalResult = !string.IsNullOrEmpty(ValidationClientModel.FinalResult) ? decimal.Parse(ValidationClientModel.FinalResult) : null,
                            IdValidationProtocol = ValidationClientModel.IdValidationProtocol,
                            IdTypeOfRequestedDocument = typeOfReqDoc!.IdTypeOfRequestedDocument,
                            IdDocumentStatus = kvDocumentStatusNotSubmitted.IdKeyValue
                        };

                        await this.repository.AddAsync<ValidationClientDocument>(clientCourseDocument);
                        await this.repository.SaveChangesAsync();

                        ValidationClientModel.IdValidationClientDocument = clientCourseDocument.IdValidationClientDocument;

                        await this.AddValidationClientDocumentStatusAsync(clientCourseDocument.IdValidationClientDocument, kvDocumentStatusNotSubmitted.IdKeyValue);
                    }
                }
                else
                {
                    var clientCourseDocumentFromDb = await this.repository.GetByIdAsync<ValidationClientDocument>(ValidationClientModel.IdValidationClientDocument);
                    if (clientCourseDocumentFromDb is not null)
                    {
                        if (clientCourseDocumentFromDb.IdDocumentSerialNumber != ValidationClientModel.IdDocumentSerialNumber)
                        {
                            var docSerialNumberReceivedFromSavedEntity = await this.repository.GetByIdAsync<DocumentSerialNumber>(clientCourseDocumentFromDb.IdDocumentSerialNumber);
                            var docSerialNumberSubmitted = await this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdCandidateProvider == this.UserProps.IdCandidateProvider && x.IdTypeOfRequestedDocument == docSerialNumberReceivedFromSavedEntity.IdTypeOfRequestedDocument && x.SerialNumber == docSerialNumberReceivedFromSavedEntity.SerialNumber && x.IdDocumentOperation == kvDocPrinted.IdKeyValue).FirstOrDefaultAsync();
                            if (docSerialNumberSubmitted is not null)
                            {
                                docSerialNumberSubmitted.SerialNumber = docSerialNumberReceivedFromNewEntity.SerialNumber;

                                this.repository.Update<DocumentSerialNumber>(docSerialNumberSubmitted);
                                await this.repository.SaveChangesAsync();
                            }
                        }

                        clientCourseDocumentFromDb.FinishedYear = ValidationClientModel.FinishedYear;
                        clientCourseDocumentFromDb.DocumentRegNo = ValidationClientModel.DocumentRegNo;
                        clientCourseDocumentFromDb.IdDocumentSerialNumber = ValidationClientModel.IdDocumentSerialNumber;
                        clientCourseDocumentFromDb.DocumentDate = ValidationClientModel.DocumentDate;
                        clientCourseDocumentFromDb.DocumentProtocol = ValidationClientModel.DocumentProtocol;
                        //clientCourseDocumentFromDb.TheoryResult = !string.IsNullOrEmpty(ValidationClientModel.TheoryResult) ? decimal.Parse(ValidationClientModel.TheoryResult) : null;
                        //clientCourseDocumentFromDb.PracticeResult = !string.IsNullOrEmpty(ValidationClientModel.PracticeResult) ? decimal.Parse(ValidationClientModel.PracticeResult) : null;
                        //clientCourseDocumentFromDb.FinalResult = !string.IsNullOrEmpty(ValidationClientModel.FinalResult) ? decimal.Parse(ValidationClientModel.FinalResult) : null;
                        clientCourseDocumentFromDb.IdValidationProtocol = ValidationClientModel.IdValidationProtocol;

                        this.repository.Update<ValidationClientDocument>(clientCourseDocumentFromDb);
                        await this.repository.SaveChangesAsync();
                    }
                }

                if (ValidationClientModel.IdValidationDocumentUploadedFile == 0)
                {
                    if (ValidationClientModel.IdValidationClientDocument != 0)
                    {
                        ValidationDocumentUploadedFile courseDocumentUploadedFile = new ValidationDocumentUploadedFile()
                        {
                            IdValidationClientDocument = ValidationClientModel.IdValidationClientDocument,
                            UploadedFileName = string.Empty
                        };

                        await this.repository.AddAsync<ValidationDocumentUploadedFile>(courseDocumentUploadedFile);
                        await this.repository.SaveChangesAsync();

                        ValidationClientModel.IdValidationDocumentUploadedFile = courseDocumentUploadedFile.IdValidationDocumentUploadedFile;
                    }
                }

                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return 0;
        }

        public async Task<int> UpdateValidationDocumentProtocolAsync(ValidationClientCombinedVM ValidationClientModel)
        {
            try
            {
                var kvDocumentStatusNotSubmittedValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "NotSubmitted");
                var typeOfReqDoc = await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == "3-54В" && x.IsValid).FirstOrDefaultAsync();
                var ValidationClierntDocumentFromDb = await this.repository.GetByIdAsync<ValidationClientDocument>(ValidationClientModel.IdValidationClientDocument);
                var docStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
                var kvDocPrinted = docStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Printed");

                var docSerialNumberReceivedFromNewEntity = await this.repository.GetByIdAsync<DocumentSerialNumber>(ValidationClientModel.IdDocumentSerialNumber);

                var validationClient = await this.repository.GetByIdAsync<ValidationClient>(ValidationClientModel.IdValidationClient);

                if (validationClient is not null)
                {
                    validationClient.IdFinishedType = ValidationClientModel.IdFinishedType;

                    this.repository.Update(validationClient);
                    await this.repository.SaveChangesAsync();
                }

                if (ValidationClierntDocumentFromDb.IdDocumentSerialNumber != ValidationClientModel.IdDocumentSerialNumber)
                {
                    var docSerialNumberReceivedFromSavedEntity = await this.repository.GetByIdAsync<DocumentSerialNumber>(ValidationClientModel.IdDocumentSerialNumber);
                    var docSerialNumberSubmitted = await this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdCandidateProvider == this.UserProps.IdCandidateProvider && x.IdTypeOfRequestedDocument == docSerialNumberReceivedFromSavedEntity.IdTypeOfRequestedDocument && x.SerialNumber == docSerialNumberReceivedFromSavedEntity.SerialNumber && x.IdDocumentOperation == kvDocPrinted.IdKeyValue).FirstOrDefaultAsync();
                    if (docSerialNumberSubmitted is not null)
                    {
                        docSerialNumberSubmitted.SerialNumber = docSerialNumberReceivedFromNewEntity.SerialNumber;

                        this.repository.Update<DocumentSerialNumber>(docSerialNumberSubmitted);
                        await this.repository.SaveChangesAsync();
                    }
                }

                ValidationClierntDocumentFromDb.IdTypeOfRequestedDocument = typeOfReqDoc?.IdTypeOfRequestedDocument;
                ValidationClierntDocumentFromDb.IdDocumentStatus = kvDocumentStatusNotSubmittedValue.IdKeyValue;
                ValidationClierntDocumentFromDb.FinishedYear = ValidationClientModel.FinishedYear;
                ValidationClierntDocumentFromDb.DocumentRegNo = ValidationClientModel.DocumentRegNo;
                ValidationClierntDocumentFromDb.IdDocumentSerialNumber = ValidationClientModel.IdDocumentSerialNumber;
                ValidationClierntDocumentFromDb.DocumentDate = ValidationClientModel.DocumentDate;
                ValidationClierntDocumentFromDb.DocumentProtocol = ValidationClientModel.DocumentProtocol;
                //ValidationClierntDocumentFromDb.TheoryResult = !string.IsNullOrEmpty(ValidationClientModel.TheoryResult) ? decimal.Parse(ValidationClientModel.TheoryResult) : null;
                //ValidationClierntDocumentFromDb.PracticeResult = !string.IsNullOrEmpty(ValidationClientModel.PracticeResult) ? decimal.Parse(ValidationClientModel.PracticeResult) : null;
                ValidationClierntDocumentFromDb.FinalResult = !string.IsNullOrEmpty(ValidationClientModel.FinalResult) ? decimal.Parse(ValidationClientModel.FinalResult) : null;
                ValidationClierntDocumentFromDb.IdValidationProtocol = ValidationClientModel.IdValidationProtocol;

                this.repository.Update(ValidationClierntDocumentFromDb);
                await this.repository.SaveChangesAsync();

                return 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
            return 0;
        }

        public async Task<ValidationClientCombinedVM> CreateValidationDocumentProtocolAsync(ValidationClientCombinedVM ValidationClientModel)
        {
            try
            {
                var kvDocumentStatusNotSubmittedValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "NotSubmitted");
                var typeOfReqDoc = await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == "3-54В" && x.IsValid).FirstOrDefaultAsync();
                var ValidationClientFromDb = await this.repository.GetByIdAsync<ValidationClient>(ValidationClientModel.IdValidationClient);
                var docStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
                var kvDocPrinted = docStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Printed");

                var docSerialNumberReceivedFromNewEntity = await this.repository.GetByIdAsync<DocumentSerialNumber>(ValidationClientModel.IdDocumentSerialNumber);

                ValidationClientFromDb.IdFinishedType = ValidationClientModel.IdFinishedType;
                this.repository.Update(ValidationClientFromDb);
                await this.repository.SaveChangesAsync();
                if (docSerialNumberReceivedFromNewEntity is not null)
                {
                    RequestDocumentManagement requestDocumentManagement = new RequestDocumentManagement()
                    {
                        IdCandidateProvider = this.UserProps.IdCandidateProvider,
                        IdTypeOfRequestedDocument = docSerialNumberReceivedFromNewEntity.IdTypeOfRequestedDocument,
                        DocumentCount = 1,
                        DocumentDate = DateTime.Now,
                        IdDocumentOperation = kvDocPrinted.IdKeyValue,
                        ReceiveDocumentYear = docSerialNumberReceivedFromNewEntity.ReceiveDocumentYear
                    };
                    await this.repository.AddAsync(requestDocumentManagement);
                    await this.repository.SaveChangesAsync();

                    DocumentSerialNumber documentSerialNumber = new DocumentSerialNumber()
                    {
                        IdRequestDocumentManagement = requestDocumentManagement.IdRequestDocumentManagement,
                        IdCandidateProvider = this.UserProps.IdCandidateProvider,
                        IdTypeOfRequestedDocument = docSerialNumberReceivedFromNewEntity.IdTypeOfRequestedDocument,
                        DocumentDate = DateTime.Now,
                        SerialNumber = docSerialNumberReceivedFromNewEntity.SerialNumber,
                        IdDocumentOperation = kvDocPrinted.IdKeyValue,
                        ReceiveDocumentYear = docSerialNumberReceivedFromNewEntity.ReceiveDocumentYear
                    };
                    await this.repository.AddAsync(documentSerialNumber);
                    await this.repository.SaveChangesAsync();
                }

                var DoesDocExist = this.repository.All<ValidationClientDocument>().Where(x => x.IdValidationClient == ValidationClientModel.IdValidationClient).FirstOrDefault();
                if (DoesDocExist is null)
                {
                    ValidationDocumentUploadedFile file = new ValidationDocumentUploadedFile();

                    ValidationClientDocument validationClientDocument = new ValidationClientDocument()
                    {
                        IdValidationClient = ValidationClientModel.IdValidationClient,
                        IdDocumentType = ValidationClientModel.IdDocumentType,
                        FinishedYear = ValidationClientModel.FinishedYear,
                        DocumentRegNo = ValidationClientModel.DocumentRegNo,
                        IdDocumentSerialNumber = ValidationClientModel.IdDocumentSerialNumber.HasValue ? ValidationClientModel.IdDocumentSerialNumber : null,
                        DocumentDate = ValidationClientModel.DocumentDate,
                        DocumentProtocol = ValidationClientModel.DocumentProtocol,
                        //TheoryResult = !string.IsNullOrEmpty(ValidationClientModel.TheoryResult) ? decimal.Parse(ValidationClientModel.TheoryResult) : null,
                        //PracticeResult = !string.IsNullOrEmpty(ValidationClientModel.PracticeResult) ? decimal.Parse(ValidationClientModel.PracticeResult) : null,
                        FinalResult = !string.IsNullOrEmpty(ValidationClientModel.FinalResult) ? decimal.Parse(ValidationClientModel.FinalResult) : null,
                        IdValidationProtocol = ValidationClientModel.IdValidationProtocol,
                        IdTypeOfRequestedDocument = typeOfReqDoc?.IdTypeOfRequestedDocument,
                        IdDocumentStatus = kvDocumentStatusNotSubmittedValue.IdKeyValue
                    };

                    file.ValidationClientDocument = validationClientDocument;
                    file.UploadedFileName = "";

                    await this.repository.AddAsync(file);
                    await this.repository.SaveChangesAsync();

                    ValidationClientModel.IdValidationClientDocument = validationClientDocument.IdValidationClientDocument;

                    await this.AddValidationClientDocumentStatusAsync(validationClientDocument.IdValidationClientDocument, kvDocumentStatusNotSubmittedValue.IdKeyValue);
                }
                else
                {
                    ValidationClientModel.IdValidationClientDocument = DoesDocExist.IdValidationClientDocument;
                }

                return ValidationClientModel;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

            }
            return null;
        }

        public async Task<List<string>> GetTheoryAndPracticeGradesFromValidationProtocols380ByIdCourseAndIdCourseClient(int idValidationClient)
        {
            var emptyList = new List<string>() { string.Empty, string.Empty };
            var kvProtocol380t = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseProtocolType", "3-80t");
            var kvProtocol380p = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseProtocolType", "3-80p");
            var protocols = this.repository.AllReadonly<ValidationProtocol>(x => x.IdValidationClient == idValidationClient && (x.IdValidationProtocolType == kvProtocol380t.IdKeyValue || x.IdValidationProtocolType == kvProtocol380p.IdKeyValue));
            var protocolsAsVM = await protocols.To<ValidationProtocolVM>(x => x.ValidationProtocolGrades).ToListAsync();

            if (protocolsAsVM.Any())
            {
                var theoryGrade = string.Empty;
                var practiceGrade = string.Empty;
                var theoryProtocol = protocolsAsVM.FirstOrDefault(x => x.IdValidationProtocolType == kvProtocol380t.IdKeyValue);
                var practiceProtocol = protocolsAsVM.FirstOrDefault(x => x.IdValidationProtocolType == kvProtocol380p.IdKeyValue);
                if (theoryProtocol is not null && practiceProtocol is not null)
                {
                    var gradeTheory = theoryProtocol.ValidationProtocolGrades.FirstOrDefault();
                    var gradePractice = practiceProtocol.ValidationProtocolGrades.FirstOrDefault();
                    if (gradeTheory is not null && gradePractice is not null)
                    {
                        if (gradeTheory.Grade.HasValue && gradePractice.Grade.HasValue)
                        {
                            theoryGrade = gradeTheory.Grade.Value.ToString();
                            practiceGrade = gradePractice.Grade.Value.ToString();

                            return new List<string>() { theoryGrade, practiceGrade };
                        }
                    }
                }
            }

            return emptyList;
        }

        public async Task<ResultContext<ValidationProtocolVM>> UpdateValidationProtocolAsync(ResultContext<ValidationProtocolVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<ValidationProtocol>(model.IdValidationClient);
                entryFromDb = model.To<ValidationProtocol>();
                entryFromDb.ValidationClient = null;
                entryFromDb.ValidationProtocolGrades = null;

                this.repository.Update<ValidationProtocol>(entryFromDb);
                await this.repository.SaveChangesAsync();

                model.IdCreateUser = entryFromDb.IdCreateUser;
                model.IdModifyUser = entryFromDb.IdModifyUser;
                model.CreationDate = entryFromDb.CreationDate;
                model.ModifyDate = entryFromDb.ModifyDate;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<ValidationProtocolVM>> CreateValidationProtocolAsync(ResultContext<ValidationProtocolVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                model.UploadedFileName = string.Empty;
                model.IdCandidateProvider = this.UserProps.IdCandidateProvider;
                var entryForDb = model.To<ValidationProtocol>();
                entryForDb.ValidationClient = null;
                entryForDb.ValidationProtocolGrades = null;

                await this.repository.AddAsync<ValidationProtocol>(entryForDb);
                await this.repository.SaveChangesAsync();

                model.IdCreateUser = entryForDb.IdCreateUser;
                model.IdModifyUser = entryForDb.IdModifyUser;
                model.CreationDate = entryForDb.CreationDate;
                model.ModifyDate = entryForDb.ModifyDate;
                model.IdValidationProtocol = entryForDb.IdValidationProtocol;

                ValidationProtocolGrade protocolGrade = new ValidationProtocolGrade()
                {
                    IdValidationProtocol = entryForDb.IdValidationProtocol,
                    IdValidationClient = entryForDb.IdValidationClient
                };

                await this.repository.AddAsync(protocolGrade);
                await this.repository.SaveChangesAsync();

                model.ValidationProtocolGrades = new List<ValidationProtocolGradeVM>() { protocolGrade.To<ValidationProtocolGradeVM>() };

                resultContext.ResultContextObject = model;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<ValidationProtocolGradeVM>> UpdateValidationProtocolGradeAsync(ResultContext<ValidationProtocolGradeVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.AllReadonly<ValidationProtocolGrade>(x => x.IdValidationProtocol == model.IdValidationProtocol).FirstOrDefaultAsync();
                if (entryFromDb is not null)
                {
                    entryFromDb.Grade = model.Grade;

                    this.repository.Update<ValidationProtocolGrade>(entryFromDb);
                    await this.repository.SaveChangesAsync();

                    await this.UpdateValidationClientGradeAfterValidationProtocolGradeUpdateAsync(model, entryFromDb);

                    resultContext.AddMessage("Записът е успешен!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        private async Task UpdateValidationClientGradeAfterValidationProtocolGradeUpdateAsync(ValidationProtocolGradeVM model, ValidationProtocolGrade? entryFromDb)
        {
            var clientCourseDocument = await this.repository.AllReadonly<ValidationClientDocument>(x => x.IdValidationClient == model.IdValidationClient).FirstOrDefaultAsync();
            if (clientCourseDocument is not null && (clientCourseDocument.TheoryResult.HasValue || clientCourseDocument.PracticeResult.HasValue || clientCourseDocument.FinalResult.HasValue))
            {
                var courseProtocol = await this.repository.GetByIdAsync<CourseProtocol>(model.IdValidationProtocol);
                if (courseProtocol is not null)
                {
                    var kvCourseProtocolTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType");
                    var courseProtocolType = kvCourseProtocolTypeSource.FirstOrDefault(x => x.IdKeyValue == courseProtocol.IdCourseProtocolType);
                    if (courseProtocolType is not null)
                    {
                        switch (courseProtocolType.KeyValueIntCode)
                        {
                            case "3-80p":
                                if (clientCourseDocument.PracticeResult.HasValue && clientCourseDocument.PracticeResult.Value != (decimal)entryFromDb.Grade!.Value)
                                {
                                    clientCourseDocument.PracticeResult = (decimal)entryFromDb.Grade!.Value;
                                }
                                break;
                            case "3-80t":
                                if (clientCourseDocument.TheoryResult.HasValue && clientCourseDocument.TheoryResult.Value != (decimal)entryFromDb.Grade!.Value)
                                {
                                    clientCourseDocument.TheoryResult = (decimal)entryFromDb.Grade!.Value;
                                }
                                break;
                            case "3-81B":
                                if (clientCourseDocument.FinalResult.HasValue && clientCourseDocument.FinalResult.Value != (decimal)entryFromDb.Grade!.Value)
                                {
                                    clientCourseDocument.FinalResult = (decimal)entryFromDb.Grade!.Value;
                                }
                                break;
                        }

                        this.repository.Update<ValidationClientDocument>(clientCourseDocument);
                        await this.repository.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<IEnumerable<ValidationProtocolVM>> GetAllValidationProtocolsByValidationClientId(int idValidationClient)
        {
            var data = this.repository.AllReadonly<ValidationProtocol>(x => x.IdValidationClient == idValidationClient);
            var dataAsVM = await data.To<ValidationProtocolVM>().ToListAsync();
            if (dataAsVM.Any())
            {
                var courseProtocolTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseProtocolType",false,true);
                foreach (var protocol in dataAsVM)
                {
                    var type = courseProtocolTypeSource.FirstOrDefault(x => x.IdKeyValue == protocol.IdValidationProtocolType);
                    if (type is not null)
                    {
                        protocol.ValidationProtocolTypeName = type.Name;
                    }
                }
            }

            return dataAsVM.OrderByDescending(x => x.ValidationProtocolDate.Value).ToList();
        }

        public async Task<ValidationProtocolVM> GetValidationProtocolByIdAsync(int idValidationProtocol)
        {
            var data = await this.repository.AllReadonly<ValidationProtocol>(x => x.IdValidationProtocol == idValidationProtocol).AsNoTracking().To<ValidationProtocolVM>(x => x.ValidationClient).FirstOrDefaultAsync();

            var grade = this.repository.All<ValidationProtocolGrade>().Where(x => x.IdValidationProtocol == idValidationProtocol).AsNoTracking().FirstOrDefault();
            if (grade is not null)
                data.ValidationProtocolGrades.Add(grade.To<ValidationProtocolGradeVM>());

            return data;
        }

        public async Task<ResultContext<NoResult>> DeleteValidationProtocolByIdAsync(int idValidationProtocol)
        {
            var result = new ResultContext<NoResult>();

            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<ValidationProtocol>(idValidationProtocol);
                if (entryFromDb is not null)
                {
                    var protocolGrades = await this.repository.AllReadonly<ValidationProtocolGrade>(x => x.IdValidationProtocol == entryFromDb.IdValidationProtocol).ToListAsync();
                    if (protocolGrades.Any())
                    {
                        this.repository.HardDeleteRange<ValidationProtocolGrade>(protocolGrades);
                        await this.repository.SaveChangesAsync();
                    }

                    await this.repository.HardDeleteAsync<ValidationProtocol>(entryFromDb.IdValidationProtocol);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return result;
        }

        public async Task<ResultContext<ValidationClientRequiredDocumentVM>> CreateValidationRequiredDocumentAsync(ResultContext<ValidationClientRequiredDocumentVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var modelForDb = model.To<ValidationClientRequiredDocument>();
                if (string.IsNullOrEmpty(modelForDb.UploadedFileName))
                {
                    modelForDb.UploadedFileName = string.Empty;
                }
                await this.repository.AddAsync<ValidationClientRequiredDocument>(modelForDb);
                await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject.IdValidationClientRequiredDocument = modelForDb.IdValidationClientRequiredDocument;
                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<ValidationClientRequiredDocumentVM>> UpdateValidationRequiredDocumentAsync(ResultContext<ValidationClientRequiredDocumentVM> resultContext)
        {
            try
            {
                var trainingCurriculumFromDb = resultContext.ResultContextObject.To<ValidationClientRequiredDocument>();
                if (trainingCurriculumFromDb.UploadedFileName is null)
                {
                    trainingCurriculumFromDb.UploadedFileName = "";
                }
                this.repository.Update<ValidationClientRequiredDocument>(trainingCurriculumFromDb);
                await this.repository.SaveChangesAsync();


                resultContext.ResultContextObject = trainingCurriculumFromDb.To<ValidationClientRequiredDocumentVM>();
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

        public async Task<ValidationClientRequiredDocumentVM> GetValidationRequiredDocumentById(int idValidationClientRequiredDocument)
        {
            var data = await this.repository.GetByIdAsync<ValidationClientRequiredDocument>(idValidationClientRequiredDocument);
            return data.To<ValidationClientRequiredDocumentVM>();
        }

        public async Task<IEnumerable<ValidationClientRequiredDocumentVM>> GetAllValidationRequiredDocumentsByIdClient(int idValidationClient)
        {
            var kvCoureRequiredTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType", false, true);
            var data = this.repository.AllReadonly<ValidationClientRequiredDocument>(x => x.IdValidationClient == idValidationClient).OrderBy(x => x.DocumentDate);
            var dataVM = await data.To<ValidationClientRequiredDocumentVM>().ToListAsync();

            foreach (var document in dataVM)
            {   var kv = kvCoureRequiredTypes.FirstOrDefault(x => x.IdKeyValue == document.IdCourseRequiredDocumentType);
                document.CourseRequiredDocumentTypeName = kv.Name;
                document.Order = kv.Order;
                document.CreatePersonName = await this.applicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);
            }
            return dataVM.OrderBy(x => x.DocumentDate).ThenBy(x => x.Order).ToList();
        }

        public async Task<bool> UpdateValidationCompetencyAsync(ValidationCompetencyVM validationCompetencyVM)
        {
            try
            {
                this.repository.Update(validationCompetencyVM.To<ValidationCompetency>());
                await this.repository.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                return false;
            }
        }

        public async Task<ValidationCompetencyVM> CreateValidationCompetencyAsync(ValidationCompetencyVM validationCompetencyVM)
        {
            try
            {
                var validationCompetency = validationCompetencyVM.To<ValidationCompetency>();

                await this.repository.AddAsync(validationCompetency);
                await this.repository.SaveChangesAsync();

                return validationCompetency.To<ValidationCompetencyVM>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                return validationCompetencyVM;
            }
        }

        public async Task<bool> DeleteValidationCompetencies(List<ValidationCompetencyVM> selected)
        {
            try
            {
                foreach (var competency in selected)
                {
                    await this.repository.HardDeleteAsync<ValidationCompetency>(competency.IdValidationCompetency);
                    await this.repository.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                return false;
            }
        }

        public async Task<IEnumerable<ValidationOrderVM>> GetAllValidationOrdersByIdValidationClient(int idValidationClient)
        {
            var data = this.repository.AllReadonly<ValidationOrder>(x => x.IdValidationClient == idValidationClient);
            return data.To<ValidationOrderVM>();
        }
        public async Task<ResultContext<ValidationOrderVM>> CreateValidationOrderAsync(ResultContext<ValidationOrderVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                model.UploadedFileName = string.Empty;
                var entryForDb = model.To<ValidationOrder>();
                entryForDb.ValidationClient = null;

                await this.repository.AddAsync<ValidationOrder>(entryForDb);
                await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject.IdCreateUser = entryForDb.IdCreateUser;
                resultContext.ResultContextObject.IdModifyUser = entryForDb.IdModifyUser;
                resultContext.ResultContextObject.CreationDate = entryForDb.CreationDate;
                resultContext.ResultContextObject.ModifyDate = entryForDb.ModifyDate;
                resultContext.ResultContextObject.IdValidationOrder = entryForDb.IdValidationOrder;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }
        public async Task<ResultContext<ValidationOrderVM>> UpdateValidationOrderAsync(ResultContext<ValidationOrderVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.AllReadonly<ValidationOrder>(x => x.IdValidationOrder == model.IdValidationOrder).FirstOrDefaultAsync();
                entryFromDb.ValidationClient = null;
                entryFromDb = model.To<ValidationOrder>();

                this.repository.Update<ValidationOrder>(entryFromDb);
                await this.repository.SaveChangesAsync();

                resultContext.ResultContextObject.IdCreateUser = entryFromDb.IdCreateUser;
                resultContext.ResultContextObject.IdModifyUser = entryFromDb.IdModifyUser;
                resultContext.ResultContextObject.CreationDate = entryFromDb.CreationDate;
                resultContext.ResultContextObject.ModifyDate = entryFromDb.ModifyDate;

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }
        public async Task<ValidationOrderVM> GetValidationOrderByIdAsync(int idValidationOrder)
        {
            var data = this.repository.AllReadonly<ValidationOrder>(x => x.IdValidationOrder == idValidationOrder);

            return await data.To<ValidationOrderVM>(x => x.ValidationClient).FirstOrDefaultAsync();
        }
        public async Task<ResultContext<ValidationOrderVM>> DeleteValidationOrderAsync(ResultContext<ValidationOrderVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<ValidationOrder>(model.IdValidationOrder);
                if (entryFromDb is not null)
                {
                    await this.repository.HardDeleteAsync<ValidationOrder>(entryFromDb.IdValidationOrder);
                    await this.repository.SaveChangesAsync();

                    resultContext.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ValidationCurriculumVM> GetValidationCurriculumByIdAsync(int idValidationCurriculum)
        {
            try
            {
                var data = this.repository.AllReadonly<ValidationCurriculum>(x => x.IdValidationCurriculum == idValidationCurriculum);

                return await data.To<ValidationCurriculumVM>().FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                return null;
            }
        }

        public async Task<ResultContext<ValidationCurriculumVM>> AddValidationCurriculumAsync(ResultContext<ValidationCurriculumVM> inputContext, bool ignoreERUs = false)
        {
            ResultContext<ValidationCurriculumVM> resultContext = new ResultContext<ValidationCurriculumVM>();

            try
            {
                var keyValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");

                resultContext.ResultContextObject = inputContext.ResultContextObject;
                var professionalTraining = keyValues.FirstOrDefault(x => x.IdKeyValue == resultContext.ResultContextObject.IdProfessionalTraining).DefaultValue1;
                resultContext.ResultContextObject.ProfessionalTraining = professionalTraining;

                var candidateCurriculumForDb = inputContext.ResultContextObject.To<ValidationCurriculum>();
                candidateCurriculumForDb.CandidateProviderSpeciality = null;
                candidateCurriculumForDb.ValidationCurriculumERUs = null;

                await this.repository.AddAsync<ValidationCurriculum>(candidateCurriculumForDb);
                await this.repository.SaveChangesAsync();

                inputContext.ResultContextObject.IdValidationCurriculum = candidateCurriculumForDb.IdValidationCurriculum;
                inputContext.ResultContextObject.CreationDate = candidateCurriculumForDb.CreationDate;
                inputContext.ResultContextObject.ModifyDate = candidateCurriculumForDb.ModifyDate;

                if (!ignoreERUs)
                {
                    await this.HandleValidationCurriculumERUsAsync(inputContext.ResultContextObject.SelectedERUs, candidateCurriculumForDb.IdValidationCurriculum);
                }

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

        public async Task<ResultContext<ValidationCurriculumVM>> UpdateValidationCurriculumAsync(ResultContext<ValidationCurriculumVM> inputContext)
        {
            ResultContext<ValidationCurriculumVM> resultContext = new ResultContext<ValidationCurriculumVM>();

            try
            {
                var keyValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");

                resultContext.ResultContextObject = inputContext.ResultContextObject;
                resultContext.ResultContextObject = inputContext.ResultContextObject;
                var professionalTraining = keyValues.FirstOrDefault(x => x.IdKeyValue == resultContext.ResultContextObject.IdProfessionalTraining).DefaultValue1;
                resultContext.ResultContextObject.ProfessionalTraining = professionalTraining;

                var trainingCurriculumFromDb = await this.repository.GetByIdAsync<ValidationCurriculum>(inputContext.ResultContextObject.IdValidationCurriculum);
                inputContext.ResultContextObject.IdCreateUser = trainingCurriculumFromDb.IdCreateUser;
                inputContext.ResultContextObject.CreationDate = trainingCurriculumFromDb.CreationDate;
                trainingCurriculumFromDb = inputContext.ResultContextObject.To<ValidationCurriculum>();
                trainingCurriculumFromDb.CandidateProviderSpeciality = null;
                trainingCurriculumFromDb.ValidationCurriculumERUs = null;

                this.repository.Update<ValidationCurriculum>(trainingCurriculumFromDb);
                await this.repository.SaveChangesAsync();

                await this.HandleValidationCurriculumERUsAsync(inputContext.ResultContextObject.SelectedERUs, trainingCurriculumFromDb.IdValidationCurriculum);

                resultContext.ResultContextObject = trainingCurriculumFromDb.To<ValidationCurriculumVM>();
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

        private async Task HandleValidationCurriculumERUsAsync(List<ERUVM> erus, int idValidationCurriculum)
        {
            foreach (var eru in erus)
            {
                var candidateCurriculumERU = this.repository.AllReadonly<ValidationCurriculumERU>(x => x.IdValidationCurriculum == idValidationCurriculum && x.IdERU == eru.IdERU).FirstOrDefault();
                if (candidateCurriculumERU is null)
                {
                    ValidationCurriculumERU curriculumERU = new ValidationCurriculumERU()
                    {
                        IdValidationCurriculum = idValidationCurriculum,
                        IdERU = eru.IdERU,
                    };

                    await this.repository.AddAsync<ValidationCurriculumERU>(curriculumERU);
                    await this.repository.SaveChangesAsync();
                }
            }
        }

        public ValidationCurriculumERUVM GetValidationCurriculumERUByIdTrainingCurriculumAndIdERU(int idValidationCurriculum, int idEru)
        {
            try
            {
                var data = this.repository.AllReadonly<ValidationCurriculumERU>(x => x.IdValidationCurriculum == idValidationCurriculum && x.IdERU == idEru).FirstOrDefault();

                if (data == null)
                {
                    return null;
                }
                else
                {
                    return data.To<ValidationCurriculumERUVM>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                return null;
            }
        }

        public async Task<ResultContext<NoResult>> DeleteValidationCurriculumERUAsync(int idValidationCurriculumERU)
        {
            var resultContext = new ResultContext<NoResult>();

            try
            {
                await this.repository.HardDeleteAsync<ValidationCurriculumERU>(idValidationCurriculumERU);
                await this.repository.SaveChangesAsync();
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

        public async Task<ResultContext<NoResult>> AddERUsToValidationCurriculumListAsync(List<ERUVM> selectedERUs, List<ValidationCurriculumVM> validationCurriculums)
        {
            var output = new ResultContext<NoResult>();

            try
            {
                foreach (var curriculum in validationCurriculums)
                {
                    foreach (var eru in selectedERUs)
                    {
                        var trainingCurriculumERU = this.repository.AllReadonly<ValidationCurriculumERU>(x => x.IdValidationCurriculum == curriculum.IdValidationCurriculum && x.IdERU == eru.IdERU).FirstOrDefault();
                        if (trainingCurriculumERU is null)
                        {
                            ValidationCurriculumERU curriculumERU = new ValidationCurriculumERU()
                            {
                                IdValidationCurriculum = curriculum.IdValidationCurriculum,
                                IdERU = eru.IdERU,
                            };

                            await this.repository.AddAsync<ValidationCurriculumERU>(curriculumERU);
                        }
                    }
                }

                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                output.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return output;
        }

        public async Task<ResultContext<List<ValidationCurriculumVM>>> ImportValidationCurriculumAsync(MemoryStream file, string fileName)
        {
            ResultContext<List<ValidationCurriculumVM>> resultContext = new ResultContext<List<ValidationCurriculumVM>>();

            List<ValidationCurriculumVM> validationCurriculumVMs = new List<ValidationCurriculumVM>();

            try
            {
                int counter = GlobalConstants.INVALID_ID_ZERO;

                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\Temp\\ImportTrainingCurriculum";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var path = @"" + filePath + "\\" + fileName;

                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file.WriteTo(filestream);
                    filestream.Close();
                    file.Close();
                }

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        IApplication app = excelEngine.Excel;

                        IWorkbook workbook = app.Workbooks.Open(fileStream, ExcelOpenType.Automatic);

                        IWorksheet worksheet = workbook.Worksheets[0];
                        if (string.IsNullOrEmpty(worksheet.Rows[0].Columns[0].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[1].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[2].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[3].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[4].Text))
                        {
                            resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на учебна програма!");
                            return resultContext;
                        }

                        var firstHeader = worksheet.Rows[0].Columns[0].Text.Trim();
                        var secondHeader = worksheet.Rows[0].Columns[1].Text.Trim();
                        var thirdHeader = worksheet.Rows[0].Columns[2].Text.Trim();
                        var fourthHeader = worksheet.Rows[0].Columns[3].Text.Trim();
                        var fifthHeader = worksheet.Rows[0].Columns[4].Text.Trim();
                        bool skipFirstRow = true;

                        //Проверка по 1 клетка за да се провери дали файла за импорт на учебна програма
                        if (firstHeader != "Раздел" || secondHeader != "Предмет" || thirdHeader != "Тема" || fourthHeader != "Теория" || fifthHeader != "Практика")
                        {
                            resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на учебна програма!");
                            return resultContext;
                        }

                        var rowCounter = 2;
                        foreach (var row in worksheet.Rows)
                        {
                            //Ако сме пропуснали 5 реда приемаме че документа е приключил и са останали празни редове
                            if (counter == 5)
                            {
                                break;
                            }

                            //Пропуска 1 ред който е с хедърите
                            if (skipFirstRow || string.IsNullOrEmpty(row.Cells[professionalTrainingIndex].Value))
                            {
                                skipFirstRow = false;
                                counter++;
                                continue;
                            }

                            var professionalTraining = row.Cells[professionalTrainingIndex].Value.Trim();
                            if (professionalTraining[0] == 'A' || professionalTraining[0] == 'a' || professionalTraining[0] == 'А' || professionalTraining[0] == 'а')
                            {
                                if (!int.TryParse(professionalTraining[1].ToString(), out int value) || int.Parse(professionalTraining[1].ToString()) < 1 || int.Parse(professionalTraining[1].ToString()) > 3)
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} не отговаря на изискванията за вид на професионална подготовка!");
                                }
                                else
                                {
                                    professionalTraining = "A" + professionalTraining[1];
                                }
                            }
                            else if (professionalTraining == "Б" || professionalTraining == "б")
                            {
                                professionalTraining = "B";
                            }
                            else
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter} не отговаря на изискванията за вид на професионална подготовка!");
                            }

                            var subject = row.Cells[subjectIndex].Value.Trim();
                            var topic = row.Cells[topicIndex].Value.Trim();
                            var theory = row.Cells[theoryIndex].Value.Trim();
                            var practice = row.Cells[practiceIndex].Value.Trim();

                            var validationCurriculum = new ValidationCurriculumVM();

                            validationCurriculum.UploadedFileName = "#";

                            var keyValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ProfessionalTraining", professionalTraining);
                            if (keyValue != null)
                            {
                                validationCurriculum.IdProfessionalTraining = keyValue.IdKeyValue;
                                validationCurriculum.ProfessionalTraining = keyValue.DefaultValue1;
                            }

                            validationCurriculum.Subject = subject;
                            if (string.IsNullOrEmpty(validationCurriculum.Subject))
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: полето 'Предмет' е задължително!");
                            }
                            else if (validationCurriculum.Subject.Length > 1000)
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: полето 'Предмет' не може да съдържа повече от 1000 символа!");
                            }

                            validationCurriculum.Topic = topic;
                            if (string.IsNullOrEmpty(validationCurriculum.Topic))
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: полето 'Тема' е задължително!");
                            }
                            else if (validationCurriculum.Topic.Length > 4000)
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: полето 'Тема' не може да съдържа повече от 4000 символа!");
                            }

                            if (string.IsNullOrEmpty(theory) && string.IsNullOrEmpty(practice))
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: при едно от полетата 'Теория' или 'Практика' трябва да има поне една въведена стойност!");
                            }

                            if (!string.IsNullOrEmpty(theory))
                            {
                                if (double.TryParse(theory, out double value))
                                {
                                    validationCurriculum.Theory = double.Parse(theory);
                                    if (validationCurriculum.Theory < 0)
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Теория' може да бъде само положително число!");
                                    }
                                    else if (validationCurriculum.Theory % 1 != 0)
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Теория' може да бъде само цяло число!");
                                    }
                                }
                                else
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Теория' може да бъде само цяло число!");
                                }
                            }

                            if (!string.IsNullOrEmpty(practice))
                            {
                                if (double.TryParse(practice, out double value))
                                {
                                    validationCurriculum.Practice = double.Parse(practice);
                                    if (validationCurriculum.Practice < 0)
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Практика' може да бъде само положително число!");
                                    }
                                    else if (validationCurriculum.Practice % 1 != 0)
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Практика' може да бъде само цяло число!");
                                    }
                                }
                                else
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Практика' може да бъде само цяло число!");
                                }
                            }

                            validationCurriculumVMs.Add(validationCurriculum);

                            rowCounter++;
                        }
                    }

                    if (validationCurriculumVMs.Any())
                    {
                        resultContext.AddMessage("Импортът приключи успешно!");
                    }
                    else
                    {
                        resultContext.AddErrorMessage("Не може да бъде добавен празен шаблон за учебна програма!");
                    }

                    resultContext.ResultContextObject = validationCurriculumVMs;
                }
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public MemoryStream CreateValidationExcelWithErrors(ResultContext<List<ValidationCurriculumVM>> resultContext)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 50;
                sheet.Range[$"A1"].Text = "Вид на грешките:";
                //sheet.Range[$"B1"].Text = "Позиция във файла";

                var rowCounter = 2;
                foreach (var item in resultContext.ListErrorMessages)
                {
                    //var splitMsg = item.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    //var msg = splitMsg[0].Trim();
                    //var cell = splitMsg[1].Trim();

                    //sheet.Range[$"A{rowCounter}"].Text = msg;
                    sheet.Range[$"A{rowCounter}"].Text = item;
                    //sheet.Range[$"B{rowCounter}"].Text = cell;

                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }

        public async Task<IEnumerable<ValidationCurriculumVM>> GetValidationCurriculumByIdValidationClientAsync(int idValidationClient)
        {
            var data = this.repository.AllReadonly<ValidationCurriculum>(x => x.IdValidationClient == idValidationClient);

            return await data.OrderBy(x => x.IdProfessionalTraining).ThenBy(x => x.OldId).ThenBy(x => x.Subject).ThenBy(x => x.Topic).To<ValidationCurriculumVM>(x => x.ValidationCurriculumERUs).ToListAsync();

        }

        public async Task<ResultContext<NoResult>> DeleteValidationCurriculumAsync(int idValidationCurriculum)
        {
            var resultContext = new ResultContext<NoResult>();

            try
            {
                var data = this.repository.AllReadonly<ValidationCurriculum>(x => x.IdValidationCurriculum == idValidationCurriculum)
                    .Include(x => x.ValidationCurriculumERUs).AsNoTracking()
                    .FirstOrDefault();

                if (data is not null)
                {

                    if (data.ValidationCurriculumERUs.Any())
                    {
                        this.repository.HardDeleteRange<ValidationCurriculumERU>(data.ValidationCurriculumERUs);
                        await this.repository.SaveChangesAsync();
                    }

                    await this.repository.HardDeleteAsync<ValidationCurriculum>(data.IdValidationCurriculum);
                    await this.repository.SaveChangesAsync();

                    resultContext.AddMessage("Записът е изтрит успешно!");
                }

                else
                {
                    if (data.ValidationCurriculumERUs.Any())
                    {
                        this.repository.HardDeleteRange<ValidationCurriculumERU>(data.ValidationCurriculumERUs);
                        await this.repository.SaveChangesAsync();
                    }

                    await this.repository.HardDeleteAsync<ValidationCurriculum>(data.IdValidationCurriculum);
                    await this.repository.SaveChangesAsync();

                    resultContext.AddMessage("Записът е изтрит успешно!");
                }

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

        public async Task<ResultContext<NoResult>> DeleteListCandidateValidationCurriculumAsync(List<ValidationCurriculumVM> selectedCurriculums)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                foreach (var training in selectedCurriculums)
                {
                    var trainingCurriculumERUs = await this.repository.AllReadonly<ValidationCurriculumERU>(x => x.IdValidationCurriculum == training.IdValidationCurriculum).ToListAsync();
                    if (trainingCurriculumERUs.Any())
                    {
                        this.repository.HardDeleteRange<ValidationCurriculumERU>(trainingCurriculumERUs);
                        await this.repository.SaveChangesAsync();
                    }

                    await this.repository.HardDeleteAsync<ValidationCurriculum>(training.IdValidationCurriculum);
                    await this.repository.SaveChangesAsync();
                }

                var msg = selectedCurriculums.Count == 1 ? "Записът е изтрит успешно!" : "Записите са изтрити успешно!";
                resultContext.AddMessage(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<IEnumerable<ValidationClientDocumentVM>> GetValidationClientDocumentsByIdValidationClientAsync(int idValidationClient)
        {
            var validationDocuments = new List<ValidationClientDocumentVM>();
            var kvFinishedWithDoc = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type5");
            var kvDuplicateIssue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type6");
            var validationClient = this.repository.AllReadonly<ValidationClient>(x => x.IdValidationClient == idValidationClient && (x.IdFinishedType == kvFinishedWithDoc.IdKeyValue || x.IdFinishedType == kvDuplicateIssue.IdKeyValue));
            if (validationClient.Any())
            {
                var kvValidationSPKDoc = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ValidationOfProfessionalQualifications");
                var validationClientsAsVM = await validationClient.To<ValidationClientVM>(x => x.ValidationClientDocuments.Select(y => y.TypeOfRequestedDocument), x => x.ValidationClientDocuments.Select(y => y.DocumentSerialNumber), x => x.ValidationClientDocuments.Select(y => y.ValidationDocumentUploadedFiles)).ToListAsync();
                var kvClientDocumentStatusTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientDocumentStatusType");
                foreach (var client in validationClientsAsVM)
                {
                    foreach (var doc in client.ValidationClientDocuments)
                    {
                        var docType = kvClientDocumentStatusTypeSource.FirstOrDefault(x => x.IdKeyValue == doc.IdDocumentStatus);
                        if (docType is not null)
                        {
                            doc.DocumentStatusValue = docType.Name;
                        }

                        doc.ValidationClient = client;

                        validationDocuments.Add(doc);
                    }
                }
            }

            return validationDocuments.OrderBy(x => x.ValidationClient.FirstName).ThenBy(x => x.ValidationClient.FamilyName).ToList();
        }

        public async Task<ResultContext<NoResult>> SendValidationDocumentsForVerificationAsync(List<ValidationClientDocumentVM> documents, string? comment)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var kvDocumentStatusSubmitted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Submitted");
                foreach (var doc in documents)
                {
                    await this.UpdateValidationClientDocumentStatusAsync(doc.IdValidationClientDocument, kvDocumentStatusSubmitted.IdKeyValue);

                    await this.AddValidationClientDocumentStatusAsync(doc.IdValidationClientDocument, kvDocumentStatusSubmitted.IdKeyValue, comment);
                }

                var msg = documents.Count == 1 ? "Документът е подаден успешно за проверка към НАПОО!" : "Документите са подадени успешно за проверка към НАПОО!";
                resultContext.AddMessage(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<ValidationClientVM>> CompleteValidationAsync(ResultContext<ValidationClientVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<ValidationClient>(model.IdValidationClient);
                if (entryFromDb is not null)
                {
                    entryFromDb.IdStatus = this.kvCompletedCourse.IdKeyValue;

                    this.repository.Update<ValidationClient>(entryFromDb);
                    await this.repository.SaveChangesAsync();

                    model.IdStatus = entryFromDb.IdStatus;

                    resultContext.AddMessage("Валидирането е приключено успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<bool> IsRIDPKDocumentSubmittedOrEnteredInRegisterByIdValidationClientAsync(int idValidationClient)
        {
            try
            {
                var kvDocStatusSubmitted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Submitted");
                var kvDocStatusEnteredInRegister = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "EnteredInTheRegister");
                var validationClientDocuments = await this.repository.AllReadonly<ValidationClientDocument>(x => x.IdValidationClient == idValidationClient).ToListAsync();
                if (validationClientDocuments.Any(x => x.IdDocumentStatus.HasValue && (x.IdDocumentStatus == kvDocStatusEnteredInRegister.IdKeyValue || x.IdDocumentStatus == kvDocStatusSubmitted.IdKeyValue)))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return false;
        }

        public async Task<bool> IsDuplicateIssuedByIdValidationClientAsync(int idValidationClient)
        {
            var isDuplicateIssued = false;
            try
            {
                var kvDocDuplicate = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "IssueOfDuplicate");
                isDuplicateIssued = await this.repository.AllReadonly<ValidationClientDocument>(x => x.IdValidationClient == idValidationClient && x.IdDocumentType == kvDocDuplicate.IdKeyValue).AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return isDuplicateIssued;
        }

        public async Task<IEnumerable<ValidationClientDocumentVM>> GetAllIssuedDuplicatesFromValidationsByIdCandidateProviderAndByIdCourseTypeAsync(int idCandidateProvider, int idCourseType)
        {
            var kvCourseStatusFinished = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");
            var kvIssueDuplicate = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "IssueOfDuplicate");
            var validationClients = this.repository.AllReadonly<ValidationClient>(x => x.IdCourseType == idCourseType && x.IdCandidateProvider == idCandidateProvider && (x.IdStatus == kvCourseStatusFinished.IdKeyValue || x.IsArchived));
            var validationClientsAsVM = await validationClients.To<ValidationClientVM>(x => x.ValidationClientDocuments.Where(z => z.IdDocumentType == kvIssueDuplicate.IdKeyValue && z.DocumentDate.HasValue && z.DocumentRegNo != null)).ToListAsync();
            var validationClientDocumentsList = new List<ValidationClientDocumentVM>();
            foreach (var client in validationClientsAsVM)
            {
                var clientDocs = client.ValidationClientDocuments.Where(z => z.IdDocumentType == kvIssueDuplicate.IdKeyValue && z.DocumentDate.HasValue && z.DocumentRegNo != null).ToList();
                if (clientDocs.Any())
                {
                    foreach (var doc in clientDocs)
                    {
                        var docFromDb = this.repository.AllReadonly<ValidationClientDocument>(x => x.IdValidationClientDocument == doc.IdValidationClientDocument);
                        var docFromDbAsVM = await docFromDb.To<ValidationClientDocumentVM>(x => x.ValidationClient.Speciality.Profession).FirstOrDefaultAsync();
                        validationClientDocumentsList.Add(docFromDbAsVM);
                    }
                }
            }

            return validationClientDocumentsList.OrderByDescending(x => x.DocumentDate).ToList();
        }

        public async Task<IEnumerable<ValidationClientVM>> GetAllArchivedAndFinishedValidationsByIdCandidateProviderByIdCourseTypeAndByIdFinishedTypeAsync(int idCandidateProvider, int idCourseType, int idFinishedType)
        {
            var kvCourseStatusFinished = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");
            var courses = this.repository.AllReadonly<ValidationClient>(x => x.IdCandidateProvider == idCandidateProvider && (x.IsArchived || x.IdStatus == kvCourseStatusFinished.IdKeyValue) && x.IdCourseType == idCourseType && x.IdFinishedType.HasValue && x.IdFinishedType == idFinishedType);

            return await courses.To<ValidationClientVM>().OrderByDescending(x => x.EndDate).ToListAsync();
        }

        public async Task<ResultContext<NoResult>> CreateValidationDuplicateDocumentAsync(ResultContext<DuplicateIssueVM> inputContext)
        {
            var model = inputContext.ResultContextObject;
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var kvIssueDuplicate = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "IssueOfDuplicate");
                var kvDocumentStatusNotSubmitted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "NotSubmitted");
                var typeOfReqDoc = model.CourseTypeFromToken == GlobalConstants.VALIDATION_DUPLICATES_SPK
                    ? await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == "3-54aB" && x.IsValid).FirstOrDefaultAsync()
                    : await this.repository.AllReadonly<TypeOfRequestedDocument>(x => x.DocTypeOfficialNumber == "3-37" && x.IsValid).FirstOrDefaultAsync();
                var kvCourseType = model.CourseTypeFromToken == GlobalConstants.VALIDATION_DUPLICATES_SPK
                    ? await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ValidationOfProfessionalQualifications")
                    : await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ValidationOfPartOfProfession");
                var originalValidationClientDocument = await this.repository.AllReadonly<ValidationClientDocument>(x => x.IdValidationClient == model.IdValidationClient && x.IdDocumentType == kvCourseType.IdKeyValue).FirstOrDefaultAsync();
                ValidationClientDocument validationClientDocument = new ValidationClientDocument()
                {
                    IdValidationClient = model.IdValidationClient!.Value,
                    IdDocumentType = kvIssueDuplicate.IdKeyValue,
                    FinishedYear = model.FinishedYear,
                    DocumentRegNo = model.DocumentRegNo,
                    DocumentDate = model.DocumentDate,
                    DocumentProtocol = model.DocumentProtocol,
                    IdValidationProtocol = model.IdValidationProtocol,
                    IdDocumentSerialNumber = model.IdDocumentSerialNumber,
                    FinalResult = decimal.Parse(model.FinalResult),
                    IdTypeOfRequestedDocument = typeOfReqDoc?.IdTypeOfRequestedDocument,
                    IdOriginalValidationClientDocument = originalValidationClientDocument?.IdOriginalValidationClientDocument
                };

                if (model.IdDocumentSerialNumber.HasValue)
                {
                    validationClientDocument.IdDocumentStatus = kvDocumentStatusNotSubmitted.IdKeyValue;
                }

                await this.repository.AddAsync<ValidationClientDocument>(validationClientDocument);
                await this.repository.SaveChangesAsync();

                model.IdValidationClientDocument = validationClientDocument.IdValidationClientDocument;
                model.IdCreateUser = validationClientDocument.IdCreateUser;
                model.IdModifyUser = validationClientDocument.IdModifyUser;
                model.CreationDate = validationClientDocument.CreationDate;
                model.ModifyDate = validationClientDocument.ModifyDate;

                if (model.IdDocumentSerialNumber.HasValue)
                {
                    await this.AddValidationClientDocumentStatusAsync(validationClientDocument.IdValidationClientDocument, kvDocumentStatusNotSubmitted.IdKeyValue);
                }

                ValidationDocumentUploadedFile validationDocumentUploadedFile = new ValidationDocumentUploadedFile()
                {
                    IdValidationClientDocument = validationClientDocument.IdValidationClientDocument,
                    UploadedFileName = string.Empty
                };

                await this.repository.AddAsync<ValidationDocumentUploadedFile>(validationDocumentUploadedFile);
                await this.repository.SaveChangesAsync();

                model.IdValidationDocumentUploadedFile = validationDocumentUploadedFile.IdValidationDocumentUploadedFile;

                if (model.IdDocumentSerialNumber.HasValue)
                {
                    var docSerialNumberReceivedFromNewEntity = await this.repository.GetByIdAsync<DocumentSerialNumber>(model.IdDocumentSerialNumber);
                    if (docSerialNumberReceivedFromNewEntity is not null)
                    {
                        var docStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
                        var kvDocPrinted = docStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Printed");
                        RequestDocumentManagement requestDocumentManagement = new RequestDocumentManagement()
                        {
                            IdCandidateProvider = this.UserProps.IdCandidateProvider,
                            IdTypeOfRequestedDocument = docSerialNumberReceivedFromNewEntity.IdTypeOfRequestedDocument,
                            DocumentCount = 1,
                            DocumentDate = DateTime.Now,
                            IdDocumentOperation = kvDocPrinted.IdKeyValue,
                            ReceiveDocumentYear = docSerialNumberReceivedFromNewEntity.ReceiveDocumentYear
                        };

                        await this.repository.AddAsync<RequestDocumentManagement>(requestDocumentManagement);
                        await this.repository.SaveChangesAsync();

                        DocumentSerialNumber documentSerialNumber = new DocumentSerialNumber()
                        {
                            IdRequestDocumentManagement = requestDocumentManagement.IdRequestDocumentManagement,
                            IdCandidateProvider = this.UserProps.IdCandidateProvider,
                            IdTypeOfRequestedDocument = docSerialNumberReceivedFromNewEntity.IdTypeOfRequestedDocument,
                            DocumentDate = DateTime.Now,
                            SerialNumber = docSerialNumberReceivedFromNewEntity.SerialNumber,
                            IdDocumentOperation = kvDocPrinted.IdKeyValue,
                            ReceiveDocumentYear = docSerialNumberReceivedFromNewEntity.ReceiveDocumentYear,
                        };

                        await this.repository.AddAsync<DocumentSerialNumber>(documentSerialNumber);
                        await this.repository.SaveChangesAsync();
                    }
                }

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> UpdateValidationDuplicateDocumentAsync(ResultContext<DuplicateIssueVM> inputContext)
        {
            var model = inputContext.ResultContextObject;
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var docFromDb = await this.repository.GetByIdAsync<ValidationClientDocument>(model.IdValidationClientDocument);
                if (docFromDb is not null)
                {
                    var docSerialNumber = await this.repository.AllReadonly<DocumentSerialNumber>(x => x.IdDocumentSerialNumber == model.IdDocumentSerialNumber).FirstOrDefaultAsync();
                    var docSeries = await this.repository.AllReadonly<DocumentSeries>(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument && x.Year == docSerialNumber.ReceiveDocumentYear).FirstOrDefaultAsync();
                    docFromDb.DocumentPrnNo = $"{docSeries?.SeriesName}/{docSerialNumber?.SerialNumber}";
                    docFromDb.DocumentSerNo = docSeries?.SeriesName;
                    docFromDb.DocumentRegNo = model.DocumentRegNo;
                    docFromDb.DocumentDate = model.DocumentDate;

                    this.repository.Update<ValidationClientDocument>(docFromDb);
                    await this.repository.SaveChangesAsync();

                    model.CreationDate = docFromDb.CreationDate;
                    model.ModifyDate = docFromDb.ModifyDate;

                    resultContext.AddMessage("Записът е успешен!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        public async Task<IEnumerable<ValidationProtocolVM>> GetValidationProtocol381BByIdValidationClientAsync(int idValidationClient)
        {
            var kvCourseProtocol381B = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseProtocolType", "3-81B");
            var validationProtocolGrades = await this.repository.AllReadonly<ValidationProtocolGrade>(x => x.IdValidationClient == idValidationClient && x.Grade.HasValue)
                .Include(x => x.ValidationProtocol)
                    .AsNoTracking()
                        .ToListAsync();

            var protocolsSource = new List<ValidationProtocolVM>();
            foreach (var protocolGrade in validationProtocolGrades)
            {
                if (protocolGrade.ValidationProtocol is not null && protocolGrade.ValidationProtocol.IdValidationProtocolType == kvCourseProtocol381B.IdKeyValue)
                {
                    protocolGrade.ValidationProtocol.ValidationProtocolGrades.Add(protocolGrade);
                    protocolsSource.Add(protocolGrade.ValidationProtocol.To<ValidationProtocolVM>());
                }
            }

            return protocolsSource;
        }

        public async Task UpdateValidationClientCurriculumFileNameAsync(int idValidationClient)
        {
            try
            {
                var validationClientFromDb = await this.repository.GetByIdAsync<ValidationClient>(idValidationClient);
                if (validationClientFromDb is not null)
                {
                    validationClientFromDb.UploadedCurriculumFileName = null;

                    this.repository.Update<ValidationClient>(validationClientFromDb);
                    await this.repository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task<IEnumerable<TrainingCurriculumUploadedFileVM>> GetValidationClientCurriculumUploadedFilesForOldValidationClientsByIdValidationClientAsync(int idValidationClient)
        {
            var dataList = new List<TrainingCurriculumUploadedFileVM>();
            try
            {
                var validationModifications = await this.repository.AllReadonly<ValidationClientCandidateCurriculumModification>(x => x.IdValidationClient == idValidationClient)
                    .Include(x => x.CandidateCurriculumModification).OrderByDescending(x => x.IdValidationClientCandidateCurriculumModification).ToListAsync();
                var counter = 1;
                foreach (var validationMod in validationModifications)
                {
                    if (validationMod.CandidateCurriculumModification is not null && !string.IsNullOrEmpty(validationMod.CandidateCurriculumModification.UploadedFileName))
                    {
                        dataList.Add(new TrainingCurriculumUploadedFileVM()
                        {
                            IdGrid = counter++,
                            UploadedFileName = validationMod.CandidateCurriculumModification.UploadedFileName,
                            IdEntity = validationMod.CandidateCurriculumModification.IdCandidateCurriculumModification
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return dataList;
        }

        public async Task<ValidationClientDocumentVM> GetValidationClientDocumentWithUploadedFilesByIdAsync(int idValidationClientDocument)
        {
            return await this.repository.AllReadonly<ValidationClientDocument>(x => x.IdValidationClientDocument == idValidationClientDocument).To<ValidationClientDocumentVM>(x => x.ValidationDocumentUploadedFiles).FirstOrDefaultAsync();
        }

        public async Task<List<ValidationClientVM>> FilterValidationClients(ValidationClientFIlterVM model)
        {
            var filter = PredicateBuilder.True<ValidationClient>();

            if (model.IdCandidateProvider != 0)
            {
               filter = filter.And(x => x.IdCandidateProvider == model.IdCandidateProvider);
            }

            if(!string.IsNullOrEmpty(model.LicenceNumber))
            {
                filter = filter.And(x => !string.IsNullOrEmpty(x.CandidateProvider.LicenceNumber));
               filter =  filter.And(x => x.CandidateProvider.LicenceNumber.Contains(model.LicenceNumber));
            }

            if(!string.IsNullOrEmpty(model.FirstName))
            {
               filter = filter.And(x => x.FirstName.Contains(model.FirstName));
            }

            if(!string.IsNullOrEmpty(model.FamilyName))
            {
                filter = filter.And(x => x.FamilyName.Contains(model.FamilyName));
            }

            if(!string.IsNullOrEmpty(model.Indent))
            {
                filter = filter.And(x => !string.IsNullOrEmpty(x.Indent));
                filter = filter.And(x => x.Indent.Contains(model.Indent));
            }

            if(model.IdNationality.HasValue)
            {
               filter = filter.And(x => x.IdNationality == model.IdNationality);
            }

            if(model.IdSex.HasValue)
            {
                filter = filter.And(x => x.IdSex == model.IdSex);
            }

            if(model.idLocation != 0)
            {
                filter = filter.And(x => x.CandidateProvider.Location.idLocation == model.idLocation);
            }

            if(model.idMunicipality != 0)
            {
                filter = filter.And(x => x.CandidateProvider.Location.idMunicipality == model.idMunicipality);
            }

            if(model.idDistrict !=0)
            {
                filter = filter.And(x => x.CandidateProvider.Location.Municipality.idDistrict == model.idDistrict);
            }

            //if(model.IdMeasureType.HasValue)
            //{
                
            //}
            if(model.IdProfession != 0)
            {
                filter = filter.And(x => x.Speciality.IdProfession == model.IdProfession);
            }

            if(model.IdSpeciality != 0)
            {
                filter = filter.And(x => x.IdSpeciality == model.IdSpeciality);
            }

            if(model.CourseStartFrom.HasValue)
            {
                filter = filter.And(x => x.StartDate.HasValue);
                filter = filter.And(x => x.StartDate >= model.CourseStartFrom);
            }

            if(model.CourseStartTo.HasValue)
            {
                filter = filter.And(x => x.StartDate.HasValue);
                filter = filter.And(x => x.StartDate <= model.CourseStartTo);
            }

            if(model.CourseEndFrom.HasValue)
            {
                filter = filter.And(x => x.EndDate.HasValue);
                filter = filter.And(x => x.EndDate >= model.CourseEndFrom);
            }

            if (model.CourseEndTo.HasValue)
            {
                filter = filter.And(x => x.EndDate.HasValue);
                filter = filter.And(x => x.EndDate <= model.CourseEndTo);
            }

            if(!string.IsNullOrEmpty(model.DocumentRegNo))
            {
                filter = filter.And(x => !string.IsNullOrEmpty(x.ValidationClientDocuments.First().DocumentRegNo));
                filter = filter.And(x => x.ValidationClientDocuments.First().DocumentRegNo.Contains(model.DocumentRegNo));
            }

            if(model.IdTypeOfRequestedDocument.HasValue)
            {
                filter = filter.And(x => x.ValidationClientDocuments.First().IdTypeOfRequestedDocument.HasValue);
                filter = filter.And(x => x.ValidationClientDocuments.First().IdTypeOfRequestedDocument == model.IdTypeOfRequestedDocument);
            }

            if(model.DocumentDateFrom.HasValue)
            {
                filter = filter.And(x => x.ValidationClientDocuments.First().DocumentDate.HasValue);
                filter = filter.And(x => x.ValidationClientDocuments.First().DocumentDate >= model.DocumentDateFrom);
            }

            if(model.DocumentDateTo.HasValue)
            {
                filter = filter.And(x => x.ValidationClientDocuments.First().DocumentDate.HasValue);
                filter = filter.And(x => x.ValidationClientDocuments.First().DocumentDate <= model.DocumentDateTo);
            }

            var filteredValidationClient = await this.repository.AllReadonly<ValidationClient>(filter)
                .To<ValidationClientVM>(x => x.CandidateProvider.Location.Municipality.District,
                x => x.Speciality.Profession, x => x.Client, x => x.ValidationClientDocuments).ToListAsync();

            var kvSource = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS")).ToList();
            foreach (var client in filteredValidationClient)
            {
                if (client.IdCourseType != 0)
                {
                    client.CourseType = await dataSourceService.GetKeyValueByIdAsync(client.IdCourseType);
                    client.FinishType = await dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);

                }

                if (client.IdSpeciality != 0 && client.IdSpeciality != null)
                {
                    string VQS_Name = kvSource.FirstOrDefault(c => c.IdKeyValue == client.Speciality.IdVQS).Name;
                    client.Speciality.CodeAndNameAndVQS = client.Speciality.CodeAndName + " - " + VQS_Name;
                }
            }

            return filteredValidationClient; 
        }

        public async Task<ResultContext<ValidationClientRequiredDocumentVM>> DeleteValidationClientRequiredDocumentAsync(ValidationClientRequiredDocumentVM ValidationClientRequiredDocumentVM)
        {
            var entity = await this.repository.GetByIdAsync<ValidationClientRequiredDocument>(ValidationClientRequiredDocumentVM.IdValidationClientRequiredDocument);
            this.repository.Detach<ValidationClientRequiredDocument>(entity);

            ResultContext<ValidationClientRequiredDocumentVM> resultContext = new ResultContext<ValidationClientRequiredDocumentVM>();

            try
            {
                this.repository.HardDelete<ValidationClientRequiredDocument>(entity);
                await this.repository.SaveChangesAsync();

                var settingsFolder = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var pathToFile = settingsFolder + "\\" + entity.UploadedFileName;
                if (!string.IsNullOrEmpty(entity.UploadedFileName))
                {
                    if (Directory.Exists(pathToFile))
                    {
                        Directory.Delete(pathToFile, true);
                    }
                }

                resultContext.AddMessage("Документът беше изтрит успешно!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<IEnumerable<ValidationCurriculumVM>> GetValidationCurriculumsWithoutAnythingIncludedByIdCourseAsync(int idValidationClient)
        {
            var data = this.repository.AllReadonly<ValidationCurriculum>(x => x.IdValidationClient == idValidationClient);
            var datAsVM = await data.To<ValidationCurriculumVM>().ToListAsync();
            var professionalTrainingTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            foreach (var curriculum in datAsVM)
            {
                var professionalTrainingValue = professionalTrainingTypesSource.FirstOrDefault(x => x.IdKeyValue == curriculum.IdProfessionalTraining);
                if (professionalTrainingValue is not null)
                {
                    curriculum.ProfessionalTraining = professionalTrainingValue.DefaultValue1;
                }
            }

            return datAsVM;
        }
         
        public async Task<IEnumerable<ValidationDocumentUploadedFileVM>> GetValidationDocumentUploadedFilesByIdValidationClientAsync(int idValidationClient)
        {
            return await this.repository.AllReadonly<ValidationDocumentUploadedFile>(x => x.ValidationClientDocument.IdValidationClient == idValidationClient).To<ValidationDocumentUploadedFileVM>().ToListAsync();
        }

        #endregion Validation

        #region ValidationClientChecking
        public async Task<List<ValidationClientCheckingVM>> GetAllActiveValidationClientCheckingsAsync(int IdValidationClient)
        {
            var result = this.repository.AllReadonly<ValidationClientChecking>(x => x.IdValidationClient == IdValidationClient);

            return result.To<ValidationClientCheckingVM>().ToList();
        }
        public async Task<List<ValidationClientCheckingVM>> GetAllActiveValidationClientCheckingsByIdFollowUpControlAsync(int IdFollowUpControl)
        {
            var result = this.repository.AllReadonly<ValidationClientChecking>(x => x.IdFollowUpControl == IdFollowUpControl);
            var data = result.To<ValidationClientCheckingVM>(x => x.ValidationClient.Speciality.Profession, x => x.ValidationClient.FrameworkProgram, x => x.ValidationClient.CandidateProvider.Location).ToList();
            return data;
        }
        public async Task<ResultContext<ValidationClientCheckingVM>> CreateValidationClientCheckingAsync(ResultContext<ValidationClientCheckingVM> resultContext)
        {
            try
            {
                var entryForDb = resultContext.ResultContextObject.To<ValidationClientChecking>();
                entryForDb.ValidationClient = null;

                await this.repository.AddAsync<ValidationClientChecking>(entryForDb);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");

                resultContext.ResultContextObject.IdValidationClientChecking = entryForDb.IdValidationClientChecking;
                resultContext.ResultContextObject.IdCreateUser = entryForDb.IdCreateUser;
                resultContext.ResultContextObject.IdModifyUser = entryForDb.IdModifyUser;
                resultContext.ResultContextObject.CreationDate = entryForDb.CreationDate;
                resultContext.ResultContextObject.ModifyDate = entryForDb.ModifyDate;


                this.repository.Detach<ValidationClientChecking>(entryForDb);
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<ResultContext<ValidationClientCheckingVM>> DeleteValidationClientCheckingAsync(ValidationClientCheckingVM validationClientCheckingVM)
        {
            var entity = await this.repository.GetByIdAsync<ValidationClientChecking>(validationClientCheckingVM.IdValidationClientChecking);
            this.repository.Detach<ValidationClientChecking>(entity);

            ResultContext<ValidationClientCheckingVM> resultContext = new ResultContext<ValidationClientCheckingVM>();

            try
            {
                this.repository.HardDelete<ValidationClientChecking>(entity);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Проверката беше изтрита успешно!");
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> AddValidationDocumentToIS(ValidationClientVM client)
        {
            var resultContext = new ResultContext<NoResult>();

            try
            {
                var doc = await this.uploadFileService.GetUploadedFileAsync<ValidationClient>(client.IdValidationClient);

                var indexUser = await this.dataSourceService.GetSettingByIntCodeAsync("IndexUserId");

                var kvNotificationValidation = await this.dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "NotificationForValidationClient");

                FileData[] files = new FileData[]
                           {
                            new FileData()
                            {
                                BinaryContent = doc.MS.ToArray(),
                                Filename = client.UploadedFileName
                            }
                           };

                RegisterDocumentParams registerDocumentParams = new RegisterDocumentParams()
                {
                    ExternalCode = kvNotificationValidation.DefaultValue2,
                    RegisterUser = int.Parse(indexUser.SettingValue),
                    RegisterUserSpecified = true
                };

                CorrespData corresp = new CorrespData()
                {
                    Names = client.CandidateProvider.ProviderName,
                    EIK = client.CandidateProvider.PoviderBulstat,
                    Phone = client.CandidateProvider.ProviderPhone,
                    Email = client.CandidateProvider.ProviderEmail
                };

                DocData docs = new DocData()
                {
                    Otnosno = kvNotificationValidation.Description,
                    Corresp = corresp,
                    File = files,

                };

                var registerResult = await this.docuService.RegisterDocumentAsync(registerDocumentParams, docs);

                if (registerResult.HasErrorMessages)
                {
                    resultContext.ListErrorMessages = registerResult.ListErrorMessages;
                    return resultContext;
                }
                var documentResponse = registerResult.ResultContextObject;

                client.DS_OFFICIAL_ID = documentResponse.Doc.DocID;
                client.DS_OFFICIAL_GUID = documentResponse.Doc.GUID;
                //client.MimeType = documentResponse.Doc.File[0].ContentType;
                client.DS_OFFICIAL_DATE = documentResponse.Doc.DocDate;
                client.DS_OFFICIAL_DocNumber = documentResponse.Doc.DocNumber;

                var cl = client.To<ValidationClient>();
                cl.CandidateProvider = null;
                cl.FrameworkProgram = null;
                cl.Speciality = null;
                this.repository.Update(cl);
                await this.repository.SaveChangesAsync();

                return resultContext;
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage("Проблем при записване в база данни");
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                return resultContext;

            }
        }



        #endregion
    }
}