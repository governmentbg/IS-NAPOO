using Data.Models.Common;
using Data.Models.Data.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ISNAPOO.Core.Contracts.Archive;
using Microsoft.AspNetCore.Components.Authorization;
using ISNAPOO.Core.ViewModels.Archive;
using Data.Models.Data.Archive;
using ISNAPOO.Core.HelperClasses;
using System.Linq.Expressions;


using Data.Models.Data.SqlView.Archive;
using Data.Models.Data.Training;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using System.IO;
using ISNAPOO.Core.XML.Course;
using Syncfusion.DocIO.DLS;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.ViewModels.Common;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using RegiX.Class.AVTR.GetActualStateV2;
using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Components;
using ISNAPOO.Core.ViewModels.Register;

namespace ISNAPOO.Core.Services.Archive
{
    public class ArchiveService : BaseService, IArchiveService
    {
        private readonly IRepository repository;
        private readonly ILogger<ArchiveService> _logger;
        private readonly IDataSourceService dataSourceService;
        private readonly IUploadFileService uploadFileService;
        public ArchiveService(
            IRepository repository,
            ILogger<ArchiveService> logger,
            IDataSourceService dataSourceService,
            AuthenticationStateProvider authenticationStateProvider, IUploadFileService uploadFileService) : base(repository, authenticationStateProvider)
        {
            this.repository = repository;
            this._logger = logger;
            this.dataSourceService = dataSourceService;
            this.uploadFileService = uploadFileService;
        }


        public async Task<List<AnnualInfoVM>> GetAllАnnualInfoForCandidateProviderAsync(int idCandidateProvider)
        {
            IQueryable<AnnualInfo> annualInfo = this.repository.AllReadonly<AnnualInfo>(x => x.IdCandidateProvider == idCandidateProvider).OrderByDescending(x => x.Year);

            List<AnnualInfoVM> result = await annualInfo.To<AnnualInfoVM>(x => x.CandidateProvider, x => x.AnnualInfoStatuses).ToListAsync();

            return result;

        }
        public async Task<List<CandidateProviderVM>> GetAllАnnualInfoAsync(CandidateProviderVM candidateProviderVM, int idCandidateProvider, int year)
        {
            var kvAnualInfoStatusWorking = await this.dataSourceService.GetKeyValueByIntCodeAsync("AnnualInfoStatusType", "Working");
            var kvAnualInfoStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AnnualInfoStatusType");

            if (await this.HasClaim("ShowAllAnnualInfoList"))
            {
                candidateProviderVM.IdCandidate_Provider = 0;
            }
            else
            {
                candidateProviderVM.IdCandidate_Provider = idCandidateProvider;
            }
            var kvLicenseStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicenseStatus", "active");
            IQueryable<CandidateProvider> candidateProvider = this.repository.AllReadonly<CandidateProvider>(FilterCandidateProviderAnnualInfo(candidateProviderVM)).Where(
                c => c.IsActive && c.IdLicenceStatus == kvLicenseStatus.IdKeyValue);

            var candidateProviderList = await candidateProvider.
                    To<CandidateProviderVM>(
                x => x.LocationCorrespondence,
                x => x.AnnualInfos
                ).ToListAsync();

            //IQueryable<AnnualInfo> аnnualInfo = this.repository.AllReadonly<AnnualInfo>(FilterAnnualInfo(annualInfoVM));

            //var result = аnnualInfo.To<AnnualInfoVM>(x=>x.CandidateProvider.LocationCorrespondence);

            foreach (var candidate in candidateProviderList)
            {
                var infoStatus = candidate.AnnualInfos.FirstOrDefault(x => x.Year == year && x.IdStatus != kvAnualInfoStatusWorking.IdKeyValue);
                if (infoStatus != null)
                {
                    if (infoStatus.HasCoursePerYear is null)
                    {
                        candidate.HaveFinishedCourses = "";
                    }
                    else if(infoStatus.HasCoursePerYear == true)
                    {
                        candidate.HaveFinishedCourses = "Не";
                    }
                    else if (infoStatus.HasCoursePerYear == false)
                    {
                        candidate.HaveFinishedCourses = "Да";
                    }
            
                    var kvAnualInfoStatus = kvAnualInfoStatusSource.FirstOrDefault(x => x.IdKeyValue == infoStatus.IdStatus);
                    if (kvAnualInfoStatus != null)
                    {                         
                        candidate.AnnualInfoStatusName = kvAnualInfoStatus.Name;
                        candidate.AnnualInfoStatusIntCode = kvAnualInfoStatus.KeyValueIntCode;
                        candidate.AnnualInfoDate = infoStatus.ModifyDate;
                    }
                }
            }
            candidateProviderList = candidateProviderList.OrderByDescending(x => x.AnnualInfoDate).ToList();
            return candidateProviderList;

        }

        protected Expression<Func<CandidateProvider, bool>> FilterCandidateProviderAnnualInfo(CandidateProviderVM model)
        {
            var predicate = PredicateBuilder.True<CandidateProvider>();

            if (model.IdCandidate_Provider != 0)
            {
                predicate = predicate.And(p => p.IdCandidate_Provider == model.IdCandidate_Provider);
            }

            return predicate;
        }
        public async Task<List<CandidateProviderVM>> GetCandidateProviderАnnualInfoAndSelfAssessmentReportsAsync(CandidateProviderVM candidateProviderVM, int idCandidateProvider)
        {
            var kvLicenseStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicenseStatus", "active");

            IQueryable<CandidateProvider> candidateProvider = this.repository.AllReadonly<CandidateProvider>
                (FilterCandidateProviderAnnualInfo(candidateProviderVM))
                .Where(c => c.IsActive && c.IdLicenceStatus == kvLicenseStatus.IdKeyValue);

            var candidateProviderList = await candidateProvider.
                    To<CandidateProviderVM>(
                x => x.LocationCorrespondence,
                x => x.AnnualInfos,              
                x => x.SelfAssessmentReports.Select(x => x.SelfAssessmentReportStatuses)
                ).ToListAsync();

            return candidateProviderList.ToList();

        }
        public async Task<ResultContext<AnnualInfoVM>> CreateAnnualInfo(AnnualInfoVM аnnualInfoVM)
        {
            ResultContext<AnnualInfoVM> outputContext = new ResultContext<AnnualInfoVM>();

            try
            {
                var аnnualInfoFromDb = await this.repository.GetByIdAsync<AnnualInfo>(аnnualInfoVM.IdAnnualInfo);

                аnnualInfoFromDb = аnnualInfoVM.To<AnnualInfo>();
                this.repository.Update<AnnualInfo>(аnnualInfoFromDb);
                await this.repository.SaveChangesAsync();

                this.repository.Detach<AnnualInfo>(аnnualInfoFromDb);

                outputContext.AddMessage("Записът е успешен!");
                outputContext.ResultContextObject = аnnualInfoVM;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<IEnumerable<AnnualMTBVM>> GetAnnualMTBReportAsync(List<CandidateProviderVM> candidateProviders, int year, string licensingType)
        {
            List<AnnualMTBVM> result = new List<AnnualMTBVM>();

            IQueryable<CandidateProvider> candidateProvidersFromDb = Enumerable.Empty<CandidateProvider>().AsQueryable();
            IQueryable<ArchCandidateProvider> archCandidateProvidersFromDb = Enumerable.Empty<ArchCandidateProvider>().AsQueryable();
            IQueryable<CandidateProviderVM> candidateProvidersAfterJoin = Enumerable.Empty<CandidateProviderVM>().AsQueryable();

            try
            {
                if (licensingType == "InfoNAPOOCPO" || licensingType == "InfoNAPOOCIPO")
                {
                    archCandidateProvidersFromDb = this.repository.AllReadonly<ArchCandidateProvider>(x => x.Year == year && candidateProviders.Select(y => y.IdCandidate_Provider).Contains(x.IdCandidate_Provider));
                    candidateProvidersAfterJoin = archCandidateProvidersFromDb.To<CandidateProviderVM>(x => x.CandidateProviderPremises.Select(y => y.CandidateProviderPremisesSpecialities));
                }
                else if (licensingType == "CipoiliCPO")
                {
                    archCandidateProvidersFromDb = this.repository.AllReadonly<ArchCandidateProvider>(x => x.Year == year && candidateProviders.Select(y => y.IdCandidate_Provider).Contains(x.IdCandidate_Provider));
                    if (archCandidateProvidersFromDb.Any())
                    {
                        candidateProvidersAfterJoin = archCandidateProvidersFromDb.To<CandidateProviderVM>(x => x.CandidateProviderPremises.Select(y => y.CandidateProviderPremisesSpecialities));
                    }
                    else
                    {
                        candidateProvidersFromDb = this.repository.AllReadonly<CandidateProvider>(x => candidateProviders.Select(y => y.IdCandidate_Provider).Contains(x.IdCandidate_Provider));
                        candidateProvidersAfterJoin = candidateProvidersFromDb.To<CandidateProviderVM>(x => x.CandidateProviderPremises.Select(y => y.CandidateProviderPremisesSpecialities));
                    }
                }

                var professionsSource = this.dataSourceService.GetAllProfessionsList();
                var specialitiesSource = this.dataSourceService.GetAllSpecialitiesList();
                var kvOwnershipsSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseOwnership");
                var kvDOSCompatibilitySource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ComplianceDOC");
                var VQSSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");

                var counter = 1;
                foreach (var candidateProvider in candidateProvidersAfterJoin.OrderBy(x => x.ProviderOwner))
                {
                    foreach (var mtb in candidateProvider.CandidateProviderPremises)
                    {
                        foreach (var speciality in mtb.CandidateProviderPremisesSpecialities)
                        {
                            var specialityFromDb = specialitiesSource.FirstOrDefault(x => x.IdSpeciality == speciality.IdSpeciality);
                            var profession = professionsSource.FirstOrDefault(x => x.IdProfession == specialityFromDb.IdProfession);
                            var vqsValue = VQSSource.FirstOrDefault(x => x.IdKeyValue == specialityFromDb.IdVQS);
                            var dosCompatibility = kvDOSCompatibilitySource.FirstOrDefault(x => x.IdKeyValue == speciality.IdComplianceDOC);
                            var ownerShipValue = kvOwnershipsSource.FirstOrDefault(x => x.IdKeyValue == mtb.IdStatus);
                            AnnualMTBVM annual = new AnnualMTBVM()
                            {
                                Id = counter++,
                                CPOName = $"ЦПО към {candidateProvider.ProviderOwner}",
                                Profession = $"{profession.Code} {profession.Name}",
                                Speciality = $"{specialityFromDb.Code} {specialityFromDb.Name} - {vqsValue.Name}",
                                DOSCompatibility = dosCompatibility != null ? dosCompatibility.Name : string.Empty,
                                MTBOwnership = ownerShipValue != null ? ownerShipValue.Name : string.Empty,
                                MTBName = mtb.PremisesName
                            };

                            result.Add(annual);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                this._logger.LogError(ex.InnerException?.Message);
                this._logger.LogError(ex.StackTrace);
            }

            result = result.OrderBy(x => x.CPOName).ToList();
            return result;
        }

        public async Task<IEnumerable<AnnualTrainerQualificationsVM>> GetAnnualTrainerQualificationsReportAsync(List<CandidateProviderVM> candidateProviders, int year, string licensingType)
        {
            List<AnnualTrainerQualificationsVM> result = new List<AnnualTrainerQualificationsVM>();
            var dict = new Dictionary<int, AnnualTrainerQualificationsVM>();


            IQueryable<CandidateProvider> candidateProvidersFromDb = Enumerable.Empty<CandidateProvider>().AsQueryable();
            IQueryable<ArchCandidateProvider> archCandidateProvidersFromDb = Enumerable.Empty<ArchCandidateProvider>().AsQueryable();
            IQueryable<CandidateProviderVM> candidateProvidersAfterJoin = Enumerable.Empty<CandidateProviderVM>().AsQueryable();

            try
            {
                if (licensingType == "InfoNAPOOCPO" || licensingType == "InfoNAPOOCIPO")
                {
                    archCandidateProvidersFromDb = this.repository.AllReadonly<ArchCandidateProvider>(x => x.Year == year && candidateProviders.Select(y => y.IdCandidate_Provider).Contains(x.IdCandidate_Provider));
                    candidateProvidersAfterJoin = archCandidateProvidersFromDb.To<CandidateProviderVM>(x => x.CandidateProviderTrainers.Select(y => y.CandidateProviderTrainerQualifications));
                }
                else if (licensingType == "CipoiliCPO")
                {
                    archCandidateProvidersFromDb = this.repository.AllReadonly<ArchCandidateProvider>(x => x.Year == year && candidateProviders.Select(y => y.IdCandidate_Provider).Contains(x.IdCandidate_Provider));
                    if (archCandidateProvidersFromDb.Any())
                    {
                        candidateProvidersAfterJoin = archCandidateProvidersFromDb.To<CandidateProviderVM>(x => x.CandidateProviderTrainers.Select(y => y.CandidateProviderTrainerQualifications));
                    }
                    else
                    {
                        candidateProvidersFromDb = this.repository.AllReadonly<CandidateProvider>(x => candidateProviders.Select(y => y.IdCandidate_Provider).Contains(x.IdCandidate_Provider));
                        candidateProvidersAfterJoin = candidateProvidersFromDb.To<CandidateProviderVM>(x => x.CandidateProviderTrainers.Select(y => y.CandidateProviderTrainerQualifications));
                    }
                }

                var professionsSource = this.dataSourceService.GetAllProfessionsList();
                var kvQualificationTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingQualificationType");
                var kvInternalQualification = kvQualificationTypesSource.FirstOrDefault(x => x.KeyValueIntCode.Contains("InHouseTraining"));
                var kvExternalQualification = kvQualificationTypesSource.FirstOrDefault(x => x.KeyValueIntCode.Contains("ExternalTraining"));

                var counter = 1;
                foreach (var candidateProvider in candidateProvidersAfterJoin)
                {
                    foreach (var trainer in candidateProvider.CandidateProviderTrainers)
                    {
                        foreach (var qualification in trainer.CandidateProviderTrainerQualifications)
                        {
                            if (qualification.IdProfession is null || !qualification.TrainingTo.HasValue)
                            {
                                continue;
                            }

                            DateTime dtEnd = new DateTime(year, 12, 31);
                            DateTime dtStart = new DateTime(year, 1, 1);
                            if (qualification.TrainingTo.Value > dtEnd || qualification.TrainingTo.Value < dtStart)
                            {
                                continue;
                            }

                            if (!dict.ContainsKey(qualification.IdProfession.Value))
                            {
                                var profession = professionsSource.FirstOrDefault(x => x.IdProfession == qualification.IdProfession.Value);
                                dict.Add(qualification.IdProfession.Value,
                                    new AnnualTrainerQualificationsVM()
                                    {
                                        Id = counter++,
                                        Profession = $"{profession.Code} {profession.Name}",
                                    });
                            }

                            var qualificationType = kvQualificationTypesSource.FirstOrDefault(x => x.IdKeyValue == qualification.IdTrainingQualificationType);
                            if (qualificationType != null)
                            {
                                if (qualificationType.IdKeyValue == kvInternalQualification.IdKeyValue)
                                {
                                    if (qualification.QualificationDuration.HasValue)
                                    {
                                        dict[qualification.IdProfession.Value].InternalTrainingHours += qualification.QualificationDuration.Value;
                                    }

                                    dict[qualification.IdProfession.Value].InternalTrainingCount++;
                                }
                                else
                                {
                                    if (qualification.QualificationDuration.HasValue)
                                    {
                                        dict[qualification.IdProfession.Value].ExternalTrainingHours += qualification.QualificationDuration.Value;
                                    }

                                    dict[qualification.IdProfession.Value].ExternalTrainingCount++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                this._logger.LogError(ex.InnerException?.Message);
                this._logger.LogError(ex.StackTrace);
            }

            result.AddRange(dict.Values.ToList());
            result = result.OrderBy(x => x.Profession).ToList();
            return result;
        }

        public async Task<List<AnnualTrainingCourse>> GetTrainingCourseAsync(List<CandidateProviderVM> listCandidateProviderVM, string frameworkProgramValue, string year)
        {
            //var data = this.repository.ExecuteSQL<AnnualTrainingCourse>("EXECUTE GetAnnualTrainingCourse {0} ", new object[1] { string.Join(',', listCandidateProviderVM.Select(c=>c.IdCandidate_Provider) ) }).ToList();

            var candidateProviders = this.repository.AllReadonly<CandidateProvider>(x => listCandidateProviderVM.Select(y => y.IdCandidate_Provider).ToList().Contains(x.IdCandidate_Provider))
                .Include(x => x.Courses)
                    .ThenInclude(x => x.Program)
                        .ThenInclude(x => x.Speciality)
                            .ThenInclude(x => x.Profession).AsNoTracking()
                .Include(x => x.Courses)
                    .ThenInclude(x => x.CandidateProviderPremises)
                        .ThenInclude(x => x.Location)
                            .ThenInclude(x => x.Municipality)
                                .ThenInclude(x => x.District).AsNoTracking()
                .Include(x => x.Courses)
                    .ThenInclude(x => x.ClientCourses)
                        .ThenInclude(x => x.ClientCourseDocuments).AsNoTracking()
                .ToList();

            var isOtherCourse = false;
            var idCourseType = 0;
            //string sourceFunding = string.Empty;
            var kvSPK = (await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification")).IdKeyValue;
            var kvPP = (await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "PartProfession")).IdKeyValue;
            var kvSPKValidation = (await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ValidationOfProfessionalQualifications")).IdKeyValue;
            var kvCourseRegulation1And7 = (await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "CourseRegulation1And7")).IdKeyValue;

            switch (frameworkProgramValue)
            {
                case "SPK":
                    idCourseType = kvSPK;
                    break;
                case "PartProfession":
                    idCourseType = kvPP;
                    break;
                case "CourseRegulation1And7":
                    idCourseType = kvCourseRegulation1And7;
                    break;
                case "OtherCourses":
                    isOtherCourse = true;
                    break;
            }

            var data = new List<AnnualTrainingCourse>();
            var kvVQSSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
            var kvAssingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType");
            var kvMale = await this.dataSourceService.GetKeyValueByIntCodeAsync("Sex", "Man");
            var kvFemale = await this.dataSourceService.GetKeyValueByIntCodeAsync("Sex", "Woman");
            var kvFinishedWithDoc = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type1");
            var kvCourseStatusFinished = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");

            var idCounter = 1;
            foreach (var entry in candidateProviders)
            {
                if (!isOtherCourse)
                {
                    var courses = entry.Courses.Where(x => x.EndDate!.Value.ToString("yyyy") == year
                    && x.IdTrainingCourseType == idCourseType
                    && x.IdStatus == kvCourseStatusFinished.IdKeyValue);

                    foreach (var course in courses)
                    {
                        var menCount = course.ClientCourses.Where(x => x.IdSex == kvMale.IdKeyValue).Count();
                        var womenCount = course.ClientCourses.Where(x => x.IdSex == kvFemale.IdKeyValue).Count();
                        var menClients = course.ClientCourses.Where(x => x.IdSex == kvMale.IdKeyValue).ToList();
                        var womenClients = course.ClientCourses.Where(x => x.IdSex == kvFemale.IdKeyValue).ToList();
                        var menFinishedWithDoc = menClients.Where(x => x.IdFinishedType == kvFinishedWithDoc.IdKeyValue).Count();
                        var womenFinishedWithDoc = womenClients.Where(x => x.IdFinishedType == kvFinishedWithDoc.IdKeyValue).Count();
                        var vqs = kvVQSSource.FirstOrDefault(x => x.IdKeyValue == course.Program.Speciality.IdVQS);
                        var horarium = (course.SelectableHours.HasValue ? course.SelectableHours.Value : 0) + (course.MandatoryHours.HasValue ? course.MandatoryHours.Value : 0);
                        var countDisabledPerson = course.ClientCourses.Where(x => x.IsDisabledPerson == true).Count();
                        var CountDisadvantagedPerson = course.ClientCourses.Where(x => x.IsDisadvantagedPerson == true).Count();
                        //var idAssignType = course.IdAssignType.HasValue ? course.IdAssignType : null;
                        //if (idAssignType != null)
                        //{
                        //    sourceFunding = kvAssingTypeSource.FirstOrDefault(x => x.IdKeyValue == course.IdAssignType.Value)..Name ? : string.Empty;
                        //}
                        var sourceFunding = course.IdAssignType.HasValue ? kvAssingTypeSource.FirstOrDefault(x => x.IdKeyValue == course.IdAssignType.Value) != null ? kvAssingTypeSource.FirstOrDefault(x => x.IdKeyValue == course.IdAssignType.Value)!.Name : string.Empty : string.Empty;

                        AnnualTrainingCourse annualCourse = new AnnualTrainingCourse()
                        {
                            IdAnnualTrainingCourse = idCounter++,
                            ProfessionCode = course.Program.Speciality.Profession.Code,
                            SpecialityCode = course.Program.Speciality.Code,
                            VQSName = vqs!.Name,
                            DistrictName = course.CandidateProviderPremises.Location.Municipality.District.DistrictName,
                            CountIncludedMan = menCount,
                            CountIncludedWomen = womenCount,
                            CountCertificateMan = menFinishedWithDoc,
                            CountCertificateWomen = womenFinishedWithDoc,
                            Horarium = horarium,
                            Cost = course.Cost.HasValue ? course.Cost.Value : 0,
                            CountTestimony = menFinishedWithDoc + womenFinishedWithDoc,
                            SourceFunding = sourceFunding, //course.IdAssignType.HasValue ? kvAssingTypeSource.FirstOrDefault(x => x.IdKeyValue == course.IdAssignType.Value)!.Name : string.Empty,
                            CountDisabledPerson = countDisabledPerson,
                            CountDisadvantagedPerson = CountDisadvantagedPerson
                        };

                        data.Add(annualCourse);
                    }
                }
                else
                {
                    var courses = entry.Courses.Where(x => x.EndDate!.Value.ToString("yyyy") == year && x.IdStatus == kvCourseStatusFinished.IdKeyValue
                    && x.IdTrainingCourseType != kvSPK && x.IdTrainingCourseType != kvPP && x.IdTrainingCourseType != kvSPKValidation);
                    foreach (var course in courses)
                    {
                        var menCount = course.ClientCourses.Where(x => x.IdSex == kvMale.IdKeyValue).Count();
                        var womenCount = course.ClientCourses.Where(x => x.IdSex == kvFemale.IdKeyValue).Count();
                        var menClients = course.ClientCourses.Where(x => x.IdSex == kvMale.IdKeyValue).ToList();
                        var womenClients = course.ClientCourses.Where(x => x.IdSex == kvFemale.IdKeyValue).ToList();
                        var menFinishedWithDoc = menClients.Where(x => x.IdFinishedType == kvFinishedWithDoc.IdKeyValue).Count();
                        var womenFinishedWithDoc = womenClients.Where(x => x.IdFinishedType == kvFinishedWithDoc.IdKeyValue).Count();
                        var vqs = kvVQSSource.FirstOrDefault(x => x.IdKeyValue == course.Program.Speciality.IdVQS);
                        AnnualTrainingCourse annualCourse = new AnnualTrainingCourse()
                        {
                            ProfessionCode = course.Program.Speciality.Profession.Code,
                            SpecialityCode = course.Program.Speciality.Code,
                            VQSName = vqs!.Name,
                            DistrictName = course.CandidateProviderPremises.Location.Municipality.District.DistrictName,
                            CountIncludedMan = menCount,
                            CountIncludedWomen = womenCount,
                            CountCertificateMan = menFinishedWithDoc,
                            CountCertificateWomen = womenFinishedWithDoc,
                            Horarium = course.DurationHours.HasValue ? course.DurationHours.Value : 0,
                            Cost = course.Cost.HasValue ? course.Cost.Value : 0,
                            CountTestimony = menFinishedWithDoc + womenFinishedWithDoc,
                            SourceFunding = course.IdAssignType.HasValue ? kvAssingTypeSource.FirstOrDefault(x => x.IdKeyValue == course.IdAssignType.Value)!.Name : string.Empty
                        };

                        data.Add(annualCourse);
                    }
                }
            }

            return data;
        }
        public async Task<List<AnnualTrainingValidationClientCourse>> GetTrainingValidationClientAsync(List<CandidateProviderVM> listCandidateProviderVM, string year)
        {
            var validClientCourse = new List<AnnualTrainingValidationClientCourse>();
            
            KeyValueVM vqs = new KeyValueVM();
            var candidateProviders = this.repository.AllReadonly<CandidateProvider>(x => listCandidateProviderVM.Select(y => y.IdCandidate_Provider).ToList().Contains(x.IdCandidate_Provider))
                .Include(x => x.ValidationClients)
                    .ThenInclude(x => x.Speciality)
                        .ThenInclude(x =>x.Profession).AsNoTracking()      
                .Include(x => x.ValidationClients)
                    .ThenInclude(x => x.ValidationClientDocuments).AsNoTracking()
                .Include(x => x.ValidationClients)
                    .ThenInclude(x => x.ValidationPremises)
                        .ThenInclude(x => x.CandidateProviderPremises)
                            .ThenInclude(x => x.Location)
                               .ThenInclude(x => x.Municipality)
                                  .ThenInclude(x => x.District).AsNoTracking()
                .Include(x => x.ValidationClients)
                    .ThenInclude(x => x.ValidationCurriculums)
                .ToList();

            try
            {
          
           
            var kvProfessionalQualifications = (await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ValidationOfProfessionalQualifications"));
            var kvValidationOfPartOfProfession = (await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ValidationOfPartOfProfession"));
            var kvVQSSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
            var kvAssingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType");
            var kvCourseStatusFinished = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");
            var professionCode = string.Empty;
            var specialityCode = string.Empty;
            var countValidationPK = 0;
            var countValidPO = 0;
            var idValidClient = 1;

                foreach (var entry in candidateProviders)
                {

                    var validationClients = entry.ValidationClients.Where(x => x.EndDate!.Value.ToString("yyyy") == year
                     && x.IdStatus == kvCourseStatusFinished.IdKeyValue && (x.IdCourseType == kvProfessionalQualifications.IdKeyValue || x.IdCourseType == kvValidationOfPartOfProfession.IdKeyValue));

                    foreach (var validationClient in validationClients)
                    {                   
                    var idVqs = validationClient.Speciality is not null ? validationClient.Speciality : null;
                    if (idVqs is not null)
                       {
                          vqs = kvVQSSource.FirstOrDefault(x => x.IdKeyValue == validationClient.Speciality.IdVQS);
                          specialityCode = validationClient.Speciality.Code;
                          professionCode = validationClient.Speciality.Profession is not null ? validationClient.Speciality.Profession.Code : string.Empty;
                       }
                    var districtName = validationClient.ValidationPremises.FirstOrDefault() is not null
                    ? validationClient.ValidationPremises.FirstOrDefault().CandidateProviderPremises.Location.Municipality.District.DistrictName
                    : string.Empty;

                        var exist = validClientCourse.FirstOrDefault(
                                                      x => x.ProfessionCode == professionCode &&
                                                         x.SpecialityCode == specialityCode &&
                                                         x.DistrictName == districtName &&
                                                         x.VQSName == vqs.Name);

                        if (exist == null)
                        {
                            var valClientDoc = validationClient.ValidationClientDocuments.FirstOrDefault();
                            if (valClientDoc != null)
                            {
                                if (valClientDoc.IdDocumentType == kvProfessionalQualifications.IdKeyValue)
                                {
                                    countValidationPK = 1;
                                    countValidPO = 0;
                                }
                                else if (valClientDoc.IdDocumentType == kvValidationOfPartOfProfession.IdKeyValue)
                                {
                                    countValidPO = 1;
                                    countValidationPK = 0;
                                }
                            }
                                        
                            validClientCourse.Add(new AnnualTrainingValidationClientCourse()
                            {
                                IdAnnualTrainingValidationClient = idValidClient++,
                                ProfessionCode = professionCode,
                                SpecialityCode = specialityCode,
                                DistrictName = districtName,
                                VQSName = vqs.Name,
                                CountValidationPK = countValidationPK,
                                CountValidPO = countValidPO,
                                CountDisadvantagedPerson =  Convert.ToInt32(validationClient.IsDisadvantagedPerson),
                                CountDisabledPerson = Convert.ToInt32(validationClient.IsDisabledPerson)
                            });
                        }
                        else
                        {
                            var valClientDoc = validationClient.ValidationClientDocuments.FirstOrDefault();
                            if (valClientDoc != null)
                            {
                                if (valClientDoc.IdDocumentType == kvProfessionalQualifications.IdKeyValue)
                                {
                                    exist.CountValidationPK++;
                                }
                                else if (valClientDoc.IdDocumentType == kvValidationOfPartOfProfession.IdKeyValue)
                                {
                                    exist.CountValidPO++;
                                }
                            }
                                                     
                            exist.CountDisadvantagedPerson += Convert.ToInt32(validationClient.IsDisadvantagedPerson);
                            exist.CountDisabledPerson += Convert.ToInt32(validationClient.IsDisabledPerson);
                        }
                
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                this._logger.LogError(ex.InnerException?.Message);
                this._logger.LogError(ex.StackTrace);
            }

            return validClientCourse;
        }
        public async Task<List<AnnualStudents>> GetStudentsAsync(List<CandidateProviderVM> listCandidateProviderVM)
        {
            string temp = string.Join(',', listCandidateProviderVM.Select(c => c.IdCandidate_Provider));
            List<AnnualStudents> data = this.repository.ExecuteSQL<AnnualStudents>("EXECUTE GetAnnualStudent {0} ", new object[1] { temp }).ToList();
            List<AnnualStudents> result = new List<AnnualStudents>();

            var smth = data.GroupBy(x => x.BirthDate);
            foreach (var year in smth)
            {
                if (year.Count() > 1)
                {
                    AnnualStudents annualStudent = new AnnualStudents();
                    foreach (var item in year)
                    {
                        annualStudent.CountIncludedWomen_I_VQS += item.CountIncludedWomen_I_VQS;
                        annualStudent.CountIncludedWomen_II_VQS += item.CountIncludedWomen_II_VQS;
                        annualStudent.CountIncludedWomen_III_VQS += item.CountIncludedWomen_III_VQS;
                        annualStudent.CountIncludedWomen_IV_VQS += item.CountIncludedWomen_IV_VQS;
                        annualStudent.CountIncludedMan_I_VQS += item.CountIncludedMan_I_VQS;
                        annualStudent.CountIncludedMan_II_VQS += item.CountIncludedMan_II_VQS;
                        annualStudent.CountIncludedMan_III_VQS += item.CountIncludedMan_III_VQS;
                        annualStudent.CountIncludedMan_IV_VQS += item.CountIncludedMan_IV_VQS;

                        annualStudent.CountCertificateWomen_I_VQS += item.CountCertificateWomen_I_VQS;
                        annualStudent.CountCertificateWomen_II_VQS += item.CountCertificateWomen_II_VQS;
                        annualStudent.CountCertificateWomen_III_VQS += item.CountCertificateWomen_III_VQS;
                        annualStudent.CountCertificateWomen_IV_VQS += item.CountCertificateWomen_IV_VQS;
                        annualStudent.CountCertificateMan_I_VQS += item.CountCertificateMan_I_VQS;
                        annualStudent.CountCertificateMan_II_VQS += item.CountCertificateMan_II_VQS;
                        annualStudent.CountCertificateMan_III_VQS += item.CountCertificateMan_III_VQS;
                        annualStudent.CountCertificateMan_IV_VQS += item.CountCertificateMan_IV_VQS;
                    }
                    annualStudent.BirthDate = year.Key;
                    result.Add(annualStudent);
                }
                else
                {
                    foreach (var item in year)
                    {
                        result.Add(item);
                    }
                }
            }


            return result;

        }

        public async Task<List<AnnualStudentsByNationality>> GetStudentsByNationalityAsync(List<CandidateProviderVM> listCandidateProviderVM)
        {
            try
            {
                string temp = string.Join(',', listCandidateProviderVM.Select(c => c.IdCandidate_Provider));
                var data = this.repository.ExecuteSQL<AnnualStudentsByNationality>("EXECUTE GetAnnualStudentByNationality {0}", new object[1] { temp }).ToList();
                List<AnnualStudentsByNationality> result = new List<AnnualStudentsByNationality>();

                var tempData = data.GroupBy(x => x.Nationality);
                foreach (var country in tempData)
                {
                    if (country.Count() > 1)
                    {
                        AnnualStudentsByNationality tempCountry = new AnnualStudentsByNationality();
                        foreach (var item in country)
                        {
                            tempCountry.CountCertifiedMen += item.CountCertifiedMen;
                            tempCountry.CountIncludedMen += item.CountIncludedMen;
                            tempCountry.CountIncludedPartOfProfessionMen += item.CountIncludedPartOfProfessionMen;
                            tempCountry.CountProfessionallyCertifiedMen += item.CountProfessionallyCertifiedMen;
                        }
                        tempCountry.Nationality = country.Key;
                        tempCountry.IdNationality = country.First().IdNationality;
                        result.Add(tempCountry);
                    }
                    else
                    {
                        AnnualStudentsByNationality tempCountry = new AnnualStudentsByNationality();
                        tempCountry.CountCertifiedMen += country.FirstOrDefault().CountCertifiedMen;
                        tempCountry.CountIncludedMen += country.FirstOrDefault().CountIncludedMen;
                        tempCountry.CountIncludedPartOfProfessionMen += country.FirstOrDefault().CountIncludedPartOfProfessionMen;
                        tempCountry.CountProfessionallyCertifiedMen += country.FirstOrDefault().CountProfessionallyCertifiedMen;                   
                        tempCountry.Nationality = country.Key;
                        tempCountry.IdNationality = country.First().IdNationality;
                        result.Add(tempCountry);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public IEnumerable<AnnualCurriculumsVM> GetAnnualCurriculumsReport(List<CandidateProviderVM> candidateProviders, int year, string licensingType)
        {
            List<AnnualCurriculumsVM> result = new List<AnnualCurriculumsVM>();

            IQueryable<CandidateProvider> candidateProvidersFromDb = Enumerable.Empty<CandidateProvider>().AsQueryable();
            IQueryable<ArchCandidateProvider> archCandidateProvidersFromDb = Enumerable.Empty<ArchCandidateProvider>().AsQueryable();
            IQueryable<CandidateProviderVM> candidateProvidersAfterJoin = Enumerable.Empty<CandidateProviderVM>().AsQueryable();

            try
            {

                if (licensingType == "InfoNAPOOCPO" || licensingType == "InfoNAPOOCIPO")
                {
                    archCandidateProvidersFromDb = this.repository.AllReadonly<ArchCandidateProvider>(x => x.Year == year && candidateProviders.Select(y => y.IdCandidate_Provider).Contains(x.IdCandidate_Provider));
                    candidateProvidersAfterJoin = archCandidateProvidersFromDb.To<CandidateProviderVM>(x => x.CandidateProviderSpecialities.Select(y => y.CandidateCurriculums));
                }
                else if (licensingType == "CipoiliCPO")
                {
                    archCandidateProvidersFromDb = this.repository.AllReadonly<ArchCandidateProvider>(x => x.Year == year && candidateProviders.Select(y => y.IdCandidate_Provider).Contains(x.IdCandidate_Provider));
                    if (archCandidateProvidersFromDb.Any())
                    {
                        candidateProvidersAfterJoin = archCandidateProvidersFromDb.To<CandidateProviderVM>(x => x.CandidateProviderSpecialities.Select(y => y.CandidateCurriculums));
                    }
                    else
                    {
                        candidateProvidersFromDb = this.repository.AllReadonly<CandidateProvider>(x => candidateProviders.Select(y => y.IdCandidate_Provider).Contains(x.IdCandidate_Provider));
                        candidateProvidersAfterJoin = candidateProvidersFromDb.To<CandidateProviderVM>(x => x.CandidateProviderSpecialities.Select(y => y.CandidateCurriculums));
                    }
                }

                var professionsSource = this.dataSourceService.GetAllProfessionsList();
                var specialitiesSource = this.dataSourceService.GetAllSpecialitiesList();

                var counter = 1;
                foreach (var candidateProvider in candidateProvidersAfterJoin.OrderBy(x => x.ProviderOwner))
                {
                    foreach (var providerSpeciality in candidateProvider.CandidateProviderSpecialities)
                    {
                        foreach (var curriculum in providerSpeciality.CandidateCurriculums)
                        {
                            var speciality = specialitiesSource.FirstOrDefault(x => x.IdSpeciality == providerSpeciality.IdSpeciality);
                            var profession = professionsSource.FirstOrDefault(x => x.IdProfession == speciality.IdProfession);

                            if (!result.Any(x => x.Speciality == $"{speciality.Code} {speciality.Name}" && x.CPOName == $"ЦПО към {candidateProvider.ProviderOwner}"))
                            {
                                AnnualCurriculumsVM data = new AnnualCurriculumsVM()
                                {
                                    Id = counter++,
                                    CPOName = $"ЦПО към {candidateProvider.ProviderOwner}",
                                    Profession = $"{profession.Code} {profession.Name}",
                                    Speciality = $"{speciality.Code} {speciality.Name}",
                                    UpdateReason = string.Empty,
                                    Date = curriculum.ModifyDate
                                };

                                result.Add(data);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                this._logger.LogError(ex.InnerException?.Message);
                this._logger.LogError(ex.StackTrace);
            }

            result = result.OrderBy(x => x.CPOName).ToList();
            return result;
        }

        public async Task SaveAnnualInfoAsync(AnnualInfoVM annualInfo)
        {
            await this.repository.AddAsync(annualInfo.To<AnnualInfo>());

            await this.repository.SaveChangesAsync();
        }
        public async Task<ResultContext<NoResult>> CreateAnnualInfoIdStatusAsync(AnnualInfoVM annualInfo)
        {
            var result = new ResultContext<NoResult>();
            try
            {
                var ann = annualInfo.To<AnnualInfo>();
                await this.repository.AddAsync(ann);

                await this.repository.SaveChangesAsync();
                annualInfo.IdAnnualInfo = ann.IdAnnualInfo;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                this._logger.LogError(ex.InnerException?.Message);
                this._logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Има проблем при запис в базата");
            }
            return result;
        }
        public async Task<ResultContext<NoResult>> SaveArchAnnualInfoStatus(int idAnnualInfo, int idKeyValue, string comment)
        {
            var result = new ResultContext<NoResult>();
            AnnualInfoStatus annualInfoStatus = new AnnualInfoStatus();

            annualInfoStatus.IdAnnualInfo = idAnnualInfo;
            annualInfoStatus.IdStatus = idKeyValue;
            annualInfoStatus.Comment = comment;
            annualInfoStatus.IdCreateUser = this.UserProps.UserId;
            annualInfoStatus.CreationDate = DateTime.Now;
            annualInfoStatus.IdModifyUser = this.UserProps.UserId;
            annualInfoStatus.ModifyDate = DateTime.Now;
            annualInfoStatus.UploadedFileName = "";

            try
            {
                await this.repository.AddAsync(annualInfoStatus);
                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                this._logger.LogError(ex.InnerException?.Message);
                this._logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }
            return result;
        }
        public async Task<ResultContext<NoResult>> UpdateAnnualInfo(AnnualInfoVM archAnnualInfoVM, string isSubmite)
        {
            var result = new ResultContext<NoResult>();
            try
            {
                var annualInfoForDb = archAnnualInfoVM.To<AnnualInfo>();
                annualInfoForDb.IdModifyUser = this.UserProps.UserId;
                annualInfoForDb.ModifyDate = DateTime.Now;
                if (isSubmite == "Submite")
                {
                    var kvCourseStatusFinished = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");
                    var finishCourses = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == annualInfoForDb.IdCandidateProvider
                   && x.EndDate.Value.Year == annualInfoForDb.Year && x.IdStatus == kvCourseStatusFinished.IdKeyValue);
                    var finishedValidationClientCourses = this.repository.AllReadonly<ValidationClient>(x => x.IdCandidateProvider == annualInfoForDb.IdCandidateProvider
                   && x.EndDate.Value.Year == annualInfoForDb.Year && x.IdStatus == kvCourseStatusFinished.IdKeyValue);

                    if (finishCourses.Any() || finishedValidationClientCourses.Any())
                    {
                        annualInfoForDb.HasCoursePerYear = true;
                    }
                    else
                    {
                        annualInfoForDb.HasCoursePerYear = false;
                    }
                }
                annualInfoForDb.AnnualInfoStatuses = null;
                annualInfoForDb.CandidateProvider = null;
                this.repository.Update(annualInfoForDb);

                await this.repository.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                this._logger.LogError(ex.InnerException?.Message);
                this._logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис на данни в базата!");
            }
            return result;
        }
        public async Task saveAnnualReportNSIAsync(AnnualReportNSIVM annualReport)
        {
            //ResourcesFolderName
            try
            {
                await this.repository.AddAsync(annualReport.To<AnnualReportNSI>());

                await this.repository.SaveChangesAsync();

                this.uploadFileService.UploadFileReportNSI(annualReport);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                this._logger.LogError(ex.InnerException?.Message);
                this._logger.LogError(ex.StackTrace);
            }
        }
        public async Task<AnnualReportNSIVM> getAnnualReportNSIByYear(string year)
        {
            var entity = await this.repository.All<AnnualReportNSI>().Where(x => x.Year == int.Parse(year)).FirstOrDefaultAsync();

            if (entity != null)
                this.repository.Detach<AnnualReportNSI>(entity);

            return entity == null ? null : entity.To<AnnualReportNSIVM>();
        }

        public async Task UpdateAnnualReportNSI(AnnualReportNSIVM report)
        {
            this.repository.Update(report.To<AnnualReportNSI>());

            await this.repository.SaveChangesAsync();
        }

        public List<AnnualReportNSIVM> getAllAnnualReportNSI()
        {
            var list = this.repository
                .All<AnnualReportNSI>()
                .To<AnnualReportNSIVM>()
                .ToList();

            foreach (var item in list)
            {
                item.Status = dataSourceService.GetKeyValueByIdAsync(item.IdStatus).Result;
            }

            return list;
        }

        public AnnualInfoVM GetAnnualInfoByIdCandProvAndYear(int idCandidateProvider, int year)
        {
            var info = this.repository.All<AnnualInfo>().Where(x => x.Year == year && x.IdCandidateProvider == idCandidateProvider).FirstOrDefault();

            return info == null ? null : info.To<AnnualInfoVM>();
        }

        public AnnualInfoVM GetAnnualInfoByCandProvIdYearAndKeySubmittedIntCode(int idCandProvider, int year, int kvSubmitted)
        {
            var info = this.repository.All<AnnualInfo>().Where(x => x.Year == year && x.IdCandidateProvider == idCandProvider && x.IdStatus == kvSubmitted).FirstOrDefault();

            return info == null ? null : info.To<AnnualInfoVM>();
        }

        #region Self assessment report

        public async Task<IEnumerable<SelfAssessmentReportVM>> GetAllSelfAssessmentReports(string licensingType, SelfAssessmentReportListFilterVM selfAssessmentReportListFilter, string fromWhere)
        {
            IQueryable<SelfAssessmentReport> reports = Enumerable.Empty<SelfAssessmentReport>().AsQueryable();
            var kvSelfAssessmentReportStatus = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("SelfAssessmentReportStatus");
            var kvLicensingType = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", licensingType);

            if (selfAssessmentReportListFilter == null && fromWhere == "FromOnInitialized" || (selfAssessmentReportListFilter?.Year == null && selfAssessmentReportListFilter?.FillingDateFrom == null && selfAssessmentReportListFilter?.FillingDateTo == null))
            {
                DateTime lastYear = DateTime.Now.AddYears(-1);
                reports = this.repository.AllReadonly<SelfAssessmentReport>().Where(x => x.CandidateProvider.IdTypeLicense == kvLicensingType.IdKeyValue && x.Year == lastYear.Year);            
            }
            else 
            {
                reports = this.repository.AllReadonly<SelfAssessmentReport>().Where(FilterSelfAssessmentReport(selfAssessmentReportListFilter, 0, kvLicensingType.IdKeyValue));
            }

            var reportsAsVM = await reports.To<SelfAssessmentReportVM>(x => x.SelfAssessmentReportStatuses, x => x.CandidateProvider).ToListAsync();
            foreach (var report in reportsAsVM)
            {
                        var kvStatus = kvSelfAssessmentReportStatus.FirstOrDefault(x => x.IdKeyValue == report.IdStatus);

                        if (kvStatus != null)
                        {
                            report.Status = kvStatus.Name;                            
                            report.StatusIntCode = kvStatus.KeyValueIntCode;
                        }
                        else
                        {
                            report.Status = string.Empty;
                            report.StatusIntCode = string.Empty;
                         }
                
            }

            return reportsAsVM.OrderByDescending(x => x.FilingDateAsStr).ToList();
        }

        public async Task<IEnumerable<SelfAssessmentReportVM>> GetAllSelfAssessmentReportsByIdCandidateProviderAsync(int idCandidateProvider, SelfAssessmentReportListFilterVM selfAssessmentReportListFilter, string fromWhere)
        {
            IQueryable<SelfAssessmentReport> reports = Enumerable.Empty<SelfAssessmentReport>().AsQueryable();
            var kvSelfAssessmentReportStatus = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("SelfAssessmentReportStatus");
           
            if (selfAssessmentReportListFilter == null && fromWhere == "FromOnInitialized" || (selfAssessmentReportListFilter?.Year == null && selfAssessmentReportListFilter?.FillingDateFrom == null && selfAssessmentReportListFilter?.FillingDateTo == null ))
            {
                reports = this.repository.AllReadonly<SelfAssessmentReport>(x => x.IdCandidateProvider == idCandidateProvider);
            }
            else
            {
                reports = this.repository.AllReadonly<SelfAssessmentReport>(FilterSelfAssessmentReport(selfAssessmentReportListFilter, idCandidateProvider, 0));
            }

            var kvLicenseStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicenseStatus", "active");
            var reportsAsVM = await reports.To<SelfAssessmentReportVM>(x => x.SelfAssessmentReportStatuses, x => x.CandidateProvider).ToListAsync();
            foreach (var report in reportsAsVM)
            {
                if (report.CandidateProvider.IdLicenceStatus == kvLicenseStatus.IdKeyValue)
                {
                        var kvStatus = kvSelfAssessmentReportStatus.FirstOrDefault(x => x.IdKeyValue == report.IdStatus);

                        if (kvStatus != null)
                        {
                            report.Status = kvStatus.Name;
                            report.StatusIntCode = kvStatus.KeyValueIntCode;
                        }
                        else
                        {
                            report.Status = string.Empty;
                            report.StatusIntCode = string.Empty;
                         }
                }
                else
                {
                    report.Status = string.Empty;
                    report.StatusIntCode = string.Empty;
                }
            }

            return reportsAsVM.OrderByDescending(x => x.FilingDate).ToList();
        }
        protected Expression<Func<SelfAssessmentReport, bool>> FilterSelfAssessmentReport(SelfAssessmentReportListFilterVM selfAssessmentReportListFilter, int idCandidateProvider, int IdKeyValue)
        {
            var predicate = PredicateBuilder.True<SelfAssessmentReport>();
        
            if (selfAssessmentReportListFilter.Year != null)
            {
                if (IdKeyValue == 0 && idCandidateProvider != 0)
                {
                    predicate = predicate.And(p => p.Year == selfAssessmentReportListFilter.Year && p.IdCandidateProvider == idCandidateProvider);
                }
                else if (IdKeyValue != 0 && idCandidateProvider == 0) 
                {
                    predicate = predicate.And(p => p.Year == selfAssessmentReportListFilter.Year && p.CandidateProvider.IdTypeLicense == IdKeyValue);
                }                
            }

            if (selfAssessmentReportListFilter.FillingDateFrom != null && selfAssessmentReportListFilter.FillingDateTo != null)
            {
                if (IdKeyValue == 0 && idCandidateProvider != 0)
                {
                    predicate = predicate.And(x => x.IdCandidateProvider == idCandidateProvider &&
                  (x.FilingDate >= selfAssessmentReportListFilter.FillingDateFrom &&
                  x.FilingDate <= selfAssessmentReportListFilter.FillingDateTo));
                }
                else if (IdKeyValue != 0 && idCandidateProvider == 0)
                {
                    predicate = predicate.And(x => x.CandidateProvider.IdTypeLicense == IdKeyValue &&
                  (x.FilingDate >= selfAssessmentReportListFilter.FillingDateFrom &&
                  x.FilingDate <= selfAssessmentReportListFilter.FillingDateTo));
                }             
            }

            if (selfAssessmentReportListFilter.FillingDateFrom != null && selfAssessmentReportListFilter.FillingDateTo == null)
            {
                if (IdKeyValue == 0 && idCandidateProvider != 0)
                {
                    predicate = predicate.And(x => x.IdCandidateProvider == idCandidateProvider &&
                 x.FilingDate >= selfAssessmentReportListFilter.FillingDateFrom);
                }
                else if (IdKeyValue != 0 && idCandidateProvider == 0)
                {
                    predicate = predicate.And(x => x.CandidateProvider.IdTypeLicense == IdKeyValue &&
                    x.FilingDate >= selfAssessmentReportListFilter.FillingDateFrom);
                }
            }

            if (selfAssessmentReportListFilter.FillingDateFrom == null && selfAssessmentReportListFilter.FillingDateTo != null)
            {
                if (IdKeyValue == 0 && idCandidateProvider != 0)
                {
                    predicate = predicate.And(x => x.IdCandidateProvider == idCandidateProvider &&
                 x.FilingDate <= selfAssessmentReportListFilter.FillingDateTo);
                }
                else if (IdKeyValue != 0 && idCandidateProvider == 0)
                {
                    predicate = predicate.And(x => x.CandidateProvider.IdTypeLicense == IdKeyValue &&
                    x.FilingDate <= selfAssessmentReportListFilter.FillingDateTo);
                }
            }

            return predicate;
        }
        public async Task<SelfAssessmentReportVM> GetSelfAssessmentReportByIdAsync(int idSelfAssessmentReport)
        {
            var report = this.repository.AllReadonly<SelfAssessmentReport>(x => x.IdSelfAssessmentReport == idSelfAssessmentReport);
            var reportAsVM = await report.To<SelfAssessmentReportVM>(x => x.SelfAssessmentReportStatuses).FirstOrDefaultAsync();
            if (reportAsVM is not null && reportAsVM.SelfAssessmentReportStatuses.Any())
            {
                var lastStatus = reportAsVM.SelfAssessmentReportStatuses.LastOrDefault();
                var kvSelfAssessmentReportStatus = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("SelfAssessmentReportStatus");
                var lastStatusValue = kvSelfAssessmentReportStatus.FirstOrDefault(x => x.IdKeyValue == lastStatus!.IdStatus);
                if (lastStatusValue != null)
                {
                    reportAsVM.Status = lastStatusValue.Name;
                    reportAsVM.IdStatus = lastStatusValue.IdKeyValue;
                    reportAsVM.StatusIntCode = lastStatusValue.KeyValueIntCode;
                }

            }

            return reportAsVM!;
        }

        private async Task<IEnumerable<SelfAssessmentSummaryProfessionalTrainingVM>> GetSelfAssessmentSummaryProfessionalTrainingVM()
        {
            List<SelfAssessmentSummaryProfessionalTrainingVM> result = new List<SelfAssessmentSummaryProfessionalTrainingVM>();






            return result;
        }

        public async Task<ResultContext<SelfAssessmentReportVM>> CreateSelfAssessmentReportAsync(ResultContext<SelfAssessmentReportVM> inputContext)
        {
            var kvSelfAssessmentReportStatusCreated = await this.dataSourceService.GetKeyValueByIntCodeAsync("SelfAssessmentReportStatus", "Created");
            var model = inputContext.ResultContextObject;
            try
            {
                var entryForDb = model.To<SelfAssessmentReport>();
                entryForDb.CandidateProvider = null;
                entryForDb.SelfAssessmentReportStatuses = null;
                entryForDb.SurveyResult.Survey = null;
                entryForDb.SurveyResult = null;
                entryForDb.IdStatus = kvSelfAssessmentReportStatusCreated.IdKeyValue;

                await this.repository.AddAsync<SelfAssessmentReport>(entryForDb);
                await this.repository.SaveChangesAsync();

                model.IdSelfAssessmentReport = entryForDb.IdSelfAssessmentReport;
                model.IdCreateUser = entryForDb.IdCreateUser;
                model.IdModifyUser = entryForDb.IdModifyUser;
                model.CreationDate = entryForDb.CreationDate;
                model.ModifyDate = entryForDb.ModifyDate;
               
                await this.AddSelfAssessmentReportStatusAsync(model, kvSelfAssessmentReportStatusCreated);

                model.Status = kvSelfAssessmentReportStatusCreated.Name;

                var reportStatuses = this.repository.AllReadonly<SelfAssessmentReportStatus>(x => x.IdSelfAssessmentReport == model.IdSelfAssessmentReport);
                model.SelfAssessmentReportStatuses = await reportStatuses.To<SelfAssessmentReportStatusVM>().ToListAsync();

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

        public async Task<ResultContext<SelfAssessmentReportVM>> UpdateSelfAssessmentReportAsync(ResultContext<SelfAssessmentReportVM> inputContext)
        {
            var model = inputContext.ResultContextObject;
            try
            {
                var entryForDb = await this.repository.GetByIdAsync<SelfAssessmentReport>(model.IdSelfAssessmentReport);
                if (entryForDb is not null)
                {
                    model.IdCreateUser = entryForDb.IdCreateUser;
                    model.CreationDate = entryForDb.CreationDate;
                    entryForDb = model.To<SelfAssessmentReport>();
                    entryForDb.CandidateProvider = null;
                    entryForDb.SelfAssessmentReportStatuses = null;
                    entryForDb.SurveyResult = null;

                    this.repository.Update<SelfAssessmentReport>(entryForDb);
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

        public async Task<ResultContext<SelfAssessmentReportVM>> FileInSelfAssessmentReportAsync(ResultContext<SelfAssessmentReportVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            var kvSelfAssessmentReportSubmitted = await this.dataSourceService.GetKeyValueByIntCodeAsync("SelfAssessmentReportStatus", "Submitted");
            try
            {
                var entryFromDb = await this.repository.GetByIdAsync<SelfAssessmentReport>(model.IdSelfAssessmentReport);
                if (entryFromDb is not null)
                {
                    entryFromDb.FilingDate = DateTime.Now;
                    entryFromDb.ModifyDate = DateTime.Now;
                    entryFromDb.IdModifyUser = this.UserProps.UserId;
                    entryFromDb.IdStatus = kvSelfAssessmentReportSubmitted.IdKeyValue;
                    
                    this.repository.Update<SelfAssessmentReport>(entryFromDb);
                    await this.repository.SaveChangesAsync();

                    model.FilingDate = entryFromDb.FilingDate;

                    
                    await this.AddSelfAssessmentReportStatusAsync(model, kvSelfAssessmentReportSubmitted);

                    resultContext.AddMessage("Докладът за самооценка е подаден успешно!");
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

        private async Task AddSelfAssessmentReportStatusAsync(SelfAssessmentReportVM model, KeyValueVM kvSelfAssessmentReportStatus)
        {
            SelfAssessmentReportStatus selfAssessmentReportStatus = new SelfAssessmentReportStatus()
            {
                IdSelfAssessmentReport = model.IdSelfAssessmentReport,
                IdStatus = kvSelfAssessmentReportStatus.IdKeyValue,
                UploadedFileName = String.Empty
            };

            await this.repository.AddAsync<SelfAssessmentReportStatus>(selfAssessmentReportStatus);
            await this.repository.SaveChangesAsync();
        }

        public async Task<ResultContext<NoResult>> SaveSelfAssessmentReportApproveOrRejectStatusAsync(int idSelfAssessmentReport, int keyValueAppOrRej, string comment)
        {
            var result = new ResultContext<NoResult>();

            SelfAssessmentReportStatus selfAssessmentReportStatus = new SelfAssessmentReportStatus();
            selfAssessmentReportStatus.IdSelfAssessmentReport = idSelfAssessmentReport;
            selfAssessmentReportStatus.IdStatus = keyValueAppOrRej;           
            selfAssessmentReportStatus.IdCreateUser = this.UserProps.UserId;
            selfAssessmentReportStatus.CreationDate = DateTime.Now;
            selfAssessmentReportStatus.IdModifyUser = this.UserProps.UserId;
            selfAssessmentReportStatus.ModifyDate = DateTime.Now;
            selfAssessmentReportStatus.Comment = comment;
            selfAssessmentReportStatus.UploadedFileName = "";
            selfAssessmentReportStatus.SelfAssessmentReport = null;

            try
            {
                await this.repository.AddAsync(selfAssessmentReportStatus);
                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                this._logger.LogError(ex.InnerException?.Message);
                this._logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }
            return result;
        }
        public async Task<ResultContext<NoResult>> UpdateSelfAssessmentReportAppOrRej(SelfAssessmentReportVM selfAssessmentReportVM, int keyValueAppOrRej)
        {
            var result = new ResultContext<NoResult>();
            try
            {         
                selfAssessmentReportVM.IdModifyUser = this.UserProps.UserId;
                selfAssessmentReportVM.ModifyDate = DateTime.Now;
                selfAssessmentReportVM.IdStatus = keyValueAppOrRej;
                selfAssessmentReportVM.SurveyResult = null;
                selfAssessmentReportVM.CandidateProvider = null;
                selfAssessmentReportVM.SelfAssessmentReportStatuses = null;
                this.repository.Update(selfAssessmentReportVM.To<SelfAssessmentReport>());

                await this.repository.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                this._logger.LogError(ex.InnerException?.Message);
                this._logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис на данни в базата!");
            }
            return result;
        }

        public async Task<ResultContext<NoResult>> SaveSelfAssessmentReportsApproveOrRejectStatusAsync(List<SelfAssessmentReportVM> selfAssessmentListVM, int keyValueAppOrRej, string comment)
        {
            KeyValueVM keyValueSubmited = new KeyValueVM();
            List<SelfAssessmentReportVM> selfAssessmentList = new List<SelfAssessmentReportVM>();
            var result = new ResultContext<NoResult>();

            try
            {
               // keyValueSubmited = await this.dataSourceService.GetKeyValueByIntCodeAsync("SelfAssessmentReportStatus", "Submitted");

                 foreach (var report in selfAssessmentListVM)
                 {
                           SelfAssessmentReportStatus selfAssessmentReportStatus = new SelfAssessmentReportStatus();
                           selfAssessmentReportStatus.IdSelfAssessmentReport = report.IdSelfAssessmentReport;
                           selfAssessmentReportStatus.IdStatus = keyValueAppOrRej;
                           selfAssessmentReportStatus.IdCreateUser = this.UserProps.UserId;
                           selfAssessmentReportStatus.CreationDate = DateTime.Now;
                           selfAssessmentReportStatus.IdModifyUser = this.UserProps.UserId;
                           selfAssessmentReportStatus.ModifyDate = DateTime.Now;
                           selfAssessmentReportStatus.Comment = comment;
                           selfAssessmentReportStatus.SelfAssessmentReport = null;
                           selfAssessmentReportStatus.UploadedFileName = "";
                           await this.repository.AddAsync(selfAssessmentReportStatus);
                           selfAssessmentList.Add(report);
                 }                                               
                 await this.repository.SaveChangesAsync();
                 result = await UpdateSelfAssessmentReportsApproveOrRejectStatuses(selfAssessmentList, keyValueAppOrRej);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                this._logger.LogError(ex.InnerException?.Message);
                this._logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }
            return result;
        }
        public async Task<ResultContext<NoResult>> UpdateSelfAssessmentReportsApproveOrRejectStatuses(List<SelfAssessmentReportVM> selfAssessmentListVM, int keyValueAppOrRej)
        {
            var result = new ResultContext<NoResult>();
            try
            {
                foreach (var report in selfAssessmentListVM)
                {
                    report.IdModifyUser = this.UserProps.UserId;
                    report.ModifyDate = DateTime.Now;
                    report.IdStatus = keyValueAppOrRej;
                    report.SurveyResult = null;
                    report.CandidateProvider = null;
                    report.SelfAssessmentReportStatuses = null;
                    this.repository.Update(report.To<SelfAssessmentReport>());
                }
                await this.repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex.Message);
                this._logger.LogError(ex.InnerException?.Message);
                this._logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис на данни в базата!");
            }
            return result;
        }
        public async Task<List<SelfAssessmentReportStatusVM>> GetSelfAssessmentReportStatuses(int idSelfAssessmentReportVM)
        {
            List<SelfAssessmentReportStatusVM> listself = new List<SelfAssessmentReportStatusVM>();

            var reportStatuses = this.repository.AllReadonly<SelfAssessmentReportStatus>(x => x.IdSelfAssessmentReport == idSelfAssessmentReportVM);
           return await reportStatuses.To<SelfAssessmentReportStatusVM>().ToListAsync();
        }
        #endregion

        public async Task<int> GetNoFinishedCourses(int idCandidateProvider, int year)
        {
            int noFinishedCourses = 0;
            try
            {
                var kvCourseStatusFinished = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");
                var unfinishedCourses = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider
                && x.EndDate.Value.Year == year && x.IdStatus != kvCourseStatusFinished.IdKeyValue);

                if (unfinishedCourses.Any())
                {
                    noFinishedCourses = unfinishedCourses.Count();
                }

                var unfinishedValidClientCourses = this.repository.AllReadonly<ValidationClient>(x => x.IdCandidateProvider == idCandidateProvider
                 && x.EndDate.Value.Year == year
                 && x.IdStatus != kvCourseStatusFinished.IdKeyValue);

                if (unfinishedValidClientCourses.Any())
                {
                    noFinishedCourses += unfinishedValidClientCourses.Count();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
          
            return noFinishedCourses;
        }
        public async Task<ResultContext<NoResult>> UpdateFinishedCoursesStatusIsArchive(int idCandidateProvider, string isArchiveOrNot, int year)
        {
            var result = new ResultContext<NoResult>();
            try
            {
                var kvCourseStatusFinished = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");

                var unfinishedCourses = this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == idCandidateProvider
                && x.EndDate.Value.Year == year && x.IdStatus == kvCourseStatusFinished.IdKeyValue);

                if (unfinishedCourses.Any())
                {
                    foreach (var unfinishedCourse in unfinishedCourses)
                    {
                        if (isArchiveOrNot == "MakeIsArchiveTrue")
                        {
                            unfinishedCourse.IsArchived = true;
                        }
                        else if (isArchiveOrNot == "MakeIsArchiveFalse")
                        {
                            unfinishedCourse.IsArchived = false;
                        }
                            this.repository.Update<Course>(unfinishedCourse);
                    }
                    await this.repository.SaveChangesAsync();                 
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
        public async Task<ResultContext<NoResult>> UpdateFinishedValidationClientCoursesStatusIsArchive(int idCandidateProvider, string isArchiveOrNot, int year)
        {
            var result = new ResultContext<NoResult>();
            try
            {
                var kvCourseStatusFinished = await this.dataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");

                var unfinishedValidationClientCourses = this.repository.AllReadonly<ValidationClient>(x => x.IdCandidateProvider == idCandidateProvider
                && x.EndDate.Value.Year == year && x.IdStatus == kvCourseStatusFinished.IdKeyValue);

                if (unfinishedValidationClientCourses.Any())
                {
                    foreach (var unfinishedValidationClientCourse in unfinishedValidationClientCourses)
                    {
                        if (isArchiveOrNot == "MakeIsArchiveTrue")
                        {
                            unfinishedValidationClientCourse.IsArchived = true;
                        }
                        else if (isArchiveOrNot == "MakeIsArchiveFalse")
                        {
                            unfinishedValidationClientCourse.IsArchived = false;
                        }
                     
                        this.repository.Update<ValidationClient>(unfinishedValidationClientCourse);
                    }
                    await this.repository.SaveChangesAsync();
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

        public async Task<ResultContext<NoResult>> UpdateTrainingConsultingClientStatusIsArchive(int idCandidateProvider, string isArchiveOrNot)
        {
            var result = new ResultContext<NoResult>();
            try
            {
             
                var trainingConsultingClients = this.repository.AllReadonly<ConsultingClient>(x => x.IdCandidateProvider == idCandidateProvider);

                if (trainingConsultingClients.Any())
                {
                    foreach (var trainingConsultingClient in trainingConsultingClients)
                    {
                        if (isArchiveOrNot == "MakeIsArchiveTrue")
                        {
                            trainingConsultingClient.IsArchived = true;
                        }
                        else if (isArchiveOrNot == "MakeIsArchiveFalse")
                        {
                            trainingConsultingClient.IsArchived = false;
                        }

                        this.repository.Update<ConsultingClient>(trainingConsultingClient);
                    }
                    await this.repository.SaveChangesAsync();
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
        public async Task<ResultContext<NoResult>> DoAnnualArhiveToCandidateProvider(int idCandidateProvider, int year)
        {
            var result = new ResultContext<NoResult>();
            CandidateProviderVM candidateProviderVM = new CandidateProviderVM();
            int archCandProvId = 0;
            var archiveCandidateProvider = new ArchCandidateProvider();

            try
            {
                var candidatProv = await this.repository.GetByIdAsync<CandidateProvider>(idCandidateProvider);

                if (candidatProv != null)
                {
                    // await DeleteArchiveCandidateProviderData(candidatProv.IdCandidate_Provider, year);

                    candidateProviderVM = candidatProv.To<CandidateProviderVM>();

                    var candidateProviderById = this.repository.AllReadonly<ArchCandidateProvider>(x => x.Year == year && x.IdCandidate_Provider == candidatProv.IdCandidate_Provider).FirstOrDefault();

                    if (candidateProviderById == null)
                    {
                        archiveCandidateProvider = candidateProviderVM.To<ArchCandidateProvider>();
                        archiveCandidateProvider.Year = year;
                        await this.repository.AddAsync(archiveCandidateProvider);
                        await this.repository.SaveChangesAsync();
                        archCandProvId = archiveCandidateProvider.IdArchCandidateProvider;
                    }
                    else
                    {
                        archCandProvId = candidateProviderById.IdArchCandidateProvider;
                    }

                    await InsertArchTrainersAndArchQualificationsTables(candidatProv.IdCandidate_Provider, archCandProvId, year);
                    await InsertArchPremisesAndArchPremisesSpecialitiesTables(candidatProv.IdCandidate_Provider, archCandProvId);
                    await InsertArchSpecialitiesAndArchCurriculumsTables(candidatProv.IdCandidate_Provider, archCandProvId, year);
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

        private async Task InsertArchTrainersAndArchQualificationsTables(int idCandidate_Provider, int archCandProvId, int year)
        {
            CandidateProviderTrainerVM candidateProviderTrainerVM = new CandidateProviderTrainerVM();
            CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM = new CandidateProviderTrainerQualificationVM();

            IQueryable<CandidateProviderTrainerQualification> providerTrainersQ = Enumerable.Empty<CandidateProviderTrainerQualification>().AsQueryable();

            var archiveCandidateProviderTrainer = new ArchCandidateProviderTrainer();
            var archCandidateProviderTrainerQualifications = new ArchCandidateProviderTrainerQualification();

            try
            {
                var candProvTrainers = this.repository.AllReadonly<CandidateProviderTrainer>
                    (x => x.IdCandidate_Provider == idCandidate_Provider);

                // DateTime dateTime = DateTime.Now;

                if (candProvTrainers.Any())
                {
                    foreach (var cProvTrainer in candProvTrainers)
                    {
                        providerTrainersQ = this.repository.AllReadonly<CandidateProviderTrainerQualification>
                           (x => x.IdCandidateProviderTrainer == cProvTrainer.IdCandidateProviderTrainer
                           && (x.TrainingTo.Value.Year == year));

                        if (providerTrainersQ.Any())
                        {
                            var archProviderTrainer = this.repository.AllReadonly<ArchCandidateProviderTrainer>
                                (x => x.IdCandidateProviderTrainer == cProvTrainer.IdCandidateProviderTrainer).FirstOrDefault();

                            if (archProviderTrainer == null)
                            {
                                candidateProviderTrainerVM = cProvTrainer.To<CandidateProviderTrainerVM>();
                                archiveCandidateProviderTrainer = candidateProviderTrainerVM.To<ArchCandidateProviderTrainer>();
                                archiveCandidateProviderTrainer.IdArchCandidateProvider = archCandProvId;
                                //archiveCandidateProviderTrainer.IdEducation = 0;
                                //archiveCandidateProviderTrainer.ArchCandidateProvider = null;
                                archiveCandidateProviderTrainer.CandidateProvider = null;

                                await this.repository.AddAsync<ArchCandidateProviderTrainer>(archiveCandidateProviderTrainer);
                            }
                        }
                    }
                    await this.repository.SaveChangesAsync();

                    foreach (var cProvTrainer in candProvTrainers)
                    {
                        providerTrainersQ = this.repository.AllReadonly<CandidateProviderTrainerQualification>
                           (x => x.IdCandidateProviderTrainer == cProvTrainer.IdCandidateProviderTrainer
                           && (x.TrainingTo.Value.Year == year));

                        if (providerTrainersQ.Any())
                        {
                            foreach (var provTrainerQ in providerTrainersQ)
                            {
                                var archProviderTrainer = this.repository.AllReadonly<ArchCandidateProviderTrainer>
                                    (x => x.IdCandidateProviderTrainer == provTrainerQ.IdCandidateProviderTrainer).FirstOrDefault();
                                if (archProviderTrainer != null)
                                {
                                    var archProviderTrainerQ = this.repository.AllReadonly<ArchCandidateProviderTrainerQualification>
                                            (x => x.IdCandidateProviderTrainerQualification == provTrainerQ.IdCandidateProviderTrainerQualification);

                                    if (!archProviderTrainerQ.Any())
                                    {
                                        candidateProviderTrainerQualificationVM = provTrainerQ.To<CandidateProviderTrainerQualificationVM>();//.ToListAsync();

                                        archCandidateProviderTrainerQualifications = candidateProviderTrainerQualificationVM.To<ArchCandidateProviderTrainerQualification>();
                                        archCandidateProviderTrainerQualifications.Profession = null;
                                        archCandidateProviderTrainerQualifications.ArchCandidateProviderTrainer = null;
                                        archCandidateProviderTrainerQualifications.CandidateProviderTrainer = null;
                                        archCandidateProviderTrainerQualifications.IdArchCandidateProviderTrainer = archProviderTrainer.IdArchCandidateProviderTrainer;
                                        await this.repository.AddAsync<ArchCandidateProviderTrainerQualification>(archCandidateProviderTrainerQualifications);

                                    }
                                }
                            }
                        }
                    }
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

        private async Task InsertArchPremisesAndArchPremisesSpecialitiesTables(int idCandidate_Provider, int archCandProvId)
        {
            CandidateProviderPremisesVM candidateProviderPremisesVM = new CandidateProviderPremisesVM();
            CandidateProviderPremisesSpecialityVM candidateProviderPremisesSpecialityVM = new CandidateProviderPremisesSpecialityVM();

            var archCandidateProviderPremises = new ArchCandidateProviderPremises();
            var archCandidateProviderPremisesSpeciality = new ArchCandidateProviderPremisesSpeciality();

            try
            {
                var candidatProviderPremises = this.repository.AllReadonly<CandidateProviderPremises>
                              (x => x.IdCandidate_Provider == idCandidate_Provider);

                if (candidatProviderPremises.Any())
                {
                    foreach (var candProvPremis in candidatProviderPremises)
                    {
                        var archCandidatProvPremis = this.repository.AllReadonly<ArchCandidateProviderPremises>
                            (x => x.IdCandidate_Provider == candProvPremis.IdCandidate_Provider
                             && x.IdArchCandidateProvider == archCandProvId).FirstOrDefault();

                        if (archCandidatProvPremis == null)
                        {
                            candidateProviderPremisesVM = candProvPremis.To<CandidateProviderPremisesVM>();
                            archCandidateProviderPremises = candidateProviderPremisesVM.To<ArchCandidateProviderPremises>();
                            archCandidateProviderPremises.IdArchCandidateProvider = archCandProvId;
                            archCandidateProviderPremises.CandidateProvider = null;
                            archCandidateProviderPremises.CandidateProviderPremisesSpecialities = null;
                            await this.repository.AddAsync<ArchCandidateProviderPremises>(archCandidateProviderPremises);
                        }
                    }
                    await this.repository.SaveChangesAsync();

                    var archCandidatProviderPremises = this.repository.AllReadonly<ArchCandidateProviderPremises>
                       (x => x.IdArchCandidateProvider == archCandProvId);

                    if (archCandidatProviderPremises.Any())
                    {
                        foreach (var archCandProvPremis in archCandidatProviderPremises)
                        {
                            var candProvPremisesSpecialities = this.repository.AllReadonly<CandidateProviderPremisesSpeciality>
                                (x => x.IdCandidateProviderPremises == archCandProvPremis.IdCandidateProviderPremises);

                            if (candProvPremisesSpecialities.Any())
                            {
                                foreach (var candPrPremSpeciality in candProvPremisesSpecialities)
                                {
                                    candidateProviderPremisesSpecialityVM = candPrPremSpeciality.To<CandidateProviderPremisesSpecialityVM>();
                                    archCandidateProviderPremisesSpeciality = candidateProviderPremisesSpecialityVM.To<ArchCandidateProviderPremisesSpeciality>();
                                    archCandidateProviderPremisesSpeciality.IdArchCandidateProviderPremises = archCandProvPremis.IdArchCandidateProviderPremises;
                                    await this.repository.AddAsync<ArchCandidateProviderPremisesSpeciality>(archCandidateProviderPremisesSpeciality);

                                }
                            }
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

        private async Task InsertArchSpecialitiesAndArchCurriculumsTables(int idCandidate_Provider, int archCandProvId, int year)
        {
            CandidateProviderSpecialityVM candidateProviderSpecialityVM = new CandidateProviderSpecialityVM();
            CandidateCurriculumVM candidateCurriculumVM = new CandidateCurriculumVM();

            //IQueryable<CandidateProviderTrainerQualification> providerTrainersQ = Enumerable.Empty<CandidateProviderTrainerQualification>().AsQueryable();

            var archCandidateProviderSpeciality = new ArchCandidateProviderSpeciality();
            var archCandidateCurriculum = new ArchCandidateCurriculum();

            try
            {
                var candidateProviderSpeciality = this.repository.AllReadonly<CandidateProviderSpeciality>
                            (x => x.IdCandidate_Provider == idCandidate_Provider);

                if (candidateProviderSpeciality.Any())
                {
                    var kvCurriculumModStatusType = this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final").Result.IdKeyValue;

                    foreach (var candProvSpeciality in candidateProviderSpeciality)
                    {
                        var candidateCurriculumModification = this.repository.AllReadonly<CandidateCurriculumModification>
                            (x => x.IdCandidateProviderSpeciality == candProvSpeciality.IdCandidateProviderSpeciality &&
                            x.IdModificationStatus == kvCurriculumModStatusType && x.ValidFromDate.Value.Year == year).FirstOrDefault();

                        if (candidateCurriculumModification != null)
                        {
                            var archCandProvSpeciality = this.repository.AllReadonly<ArchCandidateProviderSpeciality>
                                (x => x.IdCandidateProviderSpeciality == candProvSpeciality.IdCandidateProviderSpeciality &&
                                x.IdArchCandidateProvider == archCandProvId).FirstOrDefault();

                            if (archCandProvSpeciality == null)
                            {
                                candidateProviderSpecialityVM = candProvSpeciality.To<CandidateProviderSpecialityVM>();
                                archCandidateProviderSpeciality = candidateProviderSpecialityVM.To<ArchCandidateProviderSpeciality>();
                                archCandidateProviderSpeciality.CandidateProvider = null;
                                archCandidateProviderSpeciality.CandidateCurriculums = null;
                                archCandidateProviderSpeciality.Speciality = null;
                                archCandidateProviderSpeciality.IdArchCandidateProvider = archCandProvId;
                                await this.repository.AddAsync<ArchCandidateProviderSpeciality>(archCandidateProviderSpeciality);
                            }
                        }
                    }
                    await this.repository.SaveChangesAsync();

                    await InsertArchCurriculumsTable(archCandProvId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        private async Task InsertArchCurriculumsTable(int archCandProvId)
        {
            CandidateCurriculumVM candidateCurriculumVM = new CandidateCurriculumVM();

            var archCandidateCurriculum = new ArchCandidateCurriculum();

            try
            {

                var archCandProvSpecialities = this.repository.AllReadonly<ArchCandidateProviderSpeciality>
                              (x => x.IdArchCandidateProvider == archCandProvId);

                if (archCandProvSpecialities.Any())
                {
                    foreach (var archCandPrSpeciality in archCandProvSpecialities)
                    {
                        var candidateCurriculums = this.repository.AllReadonly<CandidateCurriculum>
                          (x => x.IdCandidateProviderSpeciality == archCandPrSpeciality.IdCandidateProviderSpeciality);

                        if (candidateCurriculums.Any())
                        {
                            foreach (var candCurriculum in candidateCurriculums)
                            {
                                var archCandCurriculum = this.repository.AllReadonly<ArchCandidateCurriculum>
                                   (x => x.IdCandidateCurriculum == candCurriculum.IdCandidateProviderSpeciality &&
                                    x.IdArchCandidateProviderSpeciality == archCandPrSpeciality.IdArchCandidateProviderSpeciality).FirstOrDefault();

                                if (archCandCurriculum == null)
                                {
                                    candidateCurriculumVM = candCurriculum.To<CandidateCurriculumVM>();
                                    archCandidateCurriculum = candidateCurriculumVM.To<ArchCandidateCurriculum>();
                                    archCandidateCurriculum.IdArchCandidateProviderSpeciality = archCandPrSpeciality.IdArchCandidateProviderSpeciality;
                                    await this.repository.AddAsync<ArchCandidateCurriculum>(archCandidateCurriculum);
                                }
                            }
                        }
                    }
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

        public async Task<ResultContext<NoResult>> DeleteArchiveCandidateProviderData(int idCandidateProvider, int year)
        {
            var result = new ResultContext<NoResult>();
            IEnumerable<ArchCandidateProviderTrainer> archCandidateProviderTrainers;
            IEnumerable<ArchCandidateProviderPremises> archCandidateProviderPremises;
            IEnumerable<ArchCandidateProviderSpeciality> archCandidateProviderSpecialities;

            try
            {
                var archCandidateProviderByYear = this.repository.AllReadonly<ArchCandidateProvider>
                    (x => x.Year == year && x.IdCandidate_Provider == idCandidateProvider).FirstOrDefault();

                if (archCandidateProviderByYear != null)
                {
                    archCandidateProviderTrainers = this.repository.AllReadonly<ArchCandidateProviderTrainer>
                       (x => x.IdArchCandidateProvider == archCandidateProviderByYear.IdArchCandidateProvider);

                    if (archCandidateProviderTrainers.Count() > 0)
                    {
                        foreach (var archCandProvTrainer in archCandidateProviderTrainers)
                        {
                            IEnumerable<ArchCandidateProviderTrainerQualification> archCandidateProviderTrainerQ = this.repository.AllReadonly<ArchCandidateProviderTrainerQualification>
                                (x => x.IdArchCandidateProviderTrainer == archCandProvTrainer.IdArchCandidateProviderTrainer);

                            if (archCandidateProviderTrainerQ.Count() > 0)
                            {
                                this.repository.HardDeleteRange<ArchCandidateProviderTrainerQualification>(archCandidateProviderTrainerQ);
                            }
                        }
                        await this.repository.SaveChangesAsync();

                        await DeleteArchCandidateProviderTrainerTable(archCandidateProviderTrainers);
                    }

                    archCandidateProviderPremises = this.repository.AllReadonly<ArchCandidateProviderPremises>
                       (x => x.IdArchCandidateProvider == archCandidateProviderByYear.IdArchCandidateProvider);

                    if (archCandidateProviderPremises.Count() > 0)
                    {
                        foreach (var archCandProvPremise in archCandidateProviderPremises)
                        {
                            IEnumerable<ArchCandidateProviderPremisesSpeciality> archCandidateProviderPremisesSpecialities = this.repository.AllReadonly<ArchCandidateProviderPremisesSpeciality>
                                (x => x.IdArchCandidateProviderPremises == archCandProvPremise.IdArchCandidateProviderPremises);

                            if (archCandidateProviderPremisesSpecialities.Count() > 0)
                            {
                                this.repository.HardDeleteRange<ArchCandidateProviderPremisesSpeciality>(archCandidateProviderPremisesSpecialities);
                            }
                        }
                        await this.repository.SaveChangesAsync();

                        await DeleteArchCandidateProviderPremises(archCandidateProviderPremises);
                    }

                    archCandidateProviderSpecialities = this.repository.AllReadonly<ArchCandidateProviderSpeciality>
                        (x => x.IdArchCandidateProvider == archCandidateProviderByYear.IdArchCandidateProvider);

                    if (archCandidateProviderSpecialities.Count() > 0)
                    {
                        foreach (var archCandidateProviderSpeciality in archCandidateProviderSpecialities)
                        {
                            IEnumerable<ArchCandidateCurriculum> archCandidateCurriculums = this.repository.AllReadonly<ArchCandidateCurriculum>
                              (x => x.IdArchCandidateProviderSpeciality == archCandidateProviderSpeciality.IdArchCandidateProviderSpeciality);

                            if (archCandidateCurriculums.Count() > 0)
                            {
                                this.repository.HardDeleteRange<ArchCandidateCurriculum>(archCandidateCurriculums);
                            }
                        }
                        await this.repository.SaveChangesAsync();

                        await DeleteArchCandidateProviderSpecialities(archCandidateProviderSpecialities);
                    }

                    await DeleteArchCandidateProviderTable(archCandidateProviderByYear);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при изтриване на данни от архивни таблици в базата");
            }

            return result;
        }

        private async Task DeleteArchCandidateProviderSpecialities(IEnumerable<ArchCandidateProviderSpeciality> archCandidateProviderSpecialities)
        {
            this.repository.HardDeleteRange<ArchCandidateProviderSpeciality>(archCandidateProviderSpecialities);
            await this.repository.SaveChangesAsync();
        }

        private async Task DeleteArchCandidateProviderTrainerTable(IEnumerable<ArchCandidateProviderTrainer> archCandidateProviderTrainers)
        {
            this.repository.HardDeleteRange<ArchCandidateProviderTrainer>(archCandidateProviderTrainers);
            await this.repository.SaveChangesAsync();
        }

        private async Task DeleteArchCandidateProviderPremises(IEnumerable<ArchCandidateProviderPremises> archCandidateProviderPremises)
        {
            this.repository.HardDeleteRange<ArchCandidateProviderPremises>(archCandidateProviderPremises);
            await this.repository.SaveChangesAsync();
        }

        private async Task DeleteArchCandidateProviderTable(ArchCandidateProvider archCandidateProvider)
        {
            await this.repository.HardDeleteAsync<ArchCandidateProvider>(archCandidateProvider.IdArchCandidateProvider);
            await this.repository.SaveChangesAsync();
        }
    }
}

