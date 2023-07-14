namespace ISNAPOO.Core.Services.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Data.Models.Common;
    using Data.Models.Data.Archive;
    using Data.Models.Data.Candidate;
    using Data.Models.Data.Common;
    using Data.Models.Data.Control;
    using Data.Models.Data.ProviderData;
    using Data.Models.Framework;
    using DocuServiceReference;
    using DocuWorkService;
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Common.Framework;
    using ISNAPOO.Core.Contracts;
    using ISNAPOO.Core.Contracts.Candidate;
    using ISNAPOO.Core.Contracts.Common;
    using ISNAPOO.Core.Contracts.EGovPayment;
    using ISNAPOO.Core.Contracts.EKATTE;
    using ISNAPOO.Core.Contracts.ExternalExpertCommission;
    using ISNAPOO.Core.Contracts.Licensing;
    using ISNAPOO.Core.Contracts.Mailing;
    using ISNAPOO.Core.HelperClasses;
    using ISNAPOO.Core.Mapping;
    using ISNAPOO.Core.ViewModels.Archive;
    using ISNAPOO.Core.ViewModels.Candidate;
    using ISNAPOO.Core.ViewModels.Common;
    using ISNAPOO.Core.ViewModels.CPO.LicensingChangeProcedureDoc;
    using ISNAPOO.Core.ViewModels.CPO.LicensingProcedureDoc;
    using ISNAPOO.Core.ViewModels.CPO.ProviderData;
    using ISNAPOO.Core.ViewModels.EKATTE;
    using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
    using ISNAPOO.Core.ViewModels.Identity;
    using ISNAPOO.Core.ViewModels.NAPOOCommon;
    using ISNAPOO.Core.ViewModels.SPPOO;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using vb = Microsoft.VisualBasic;
    public class ProviderService : BaseService, IProviderService
    {
        private readonly IRepository repositorysitory;
        private readonly IDataSourceService dataSourceService;
        private readonly IApplicationUserService applicationUserService;
        private readonly ICandidateProviderService candidateProviderService;
        private readonly IDocuService docuService;
        private readonly ILocationService locationService;
        private readonly IMailService MailService;
        private readonly IExpertService expertService;
        private readonly ITemplateDocumentService templateDocumentService;
        private readonly ILicensingProcedureDocumentCPOService LicensingService;
        private readonly ILicensingChangeProcedureCPOService ChangeLicensingService;
        private readonly ILicensingProcedureDocumentCIPOService LicensingCIPOService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ISettingService settingService;
        private readonly IPersonService personService;
        private readonly IUploadFileService uploadFileService;
        public ProviderService(IRepository _repository, ILogger<ProviderService> _logger,
            IDataSourceService dataSourceService, IApplicationUserService applicationUser,
            ICandidateProviderService candidateProvider, IMailService mailService, AuthenticationStateProvider authenticationStateProvider,
            IDocuService docuService,
            ITemplateDocumentService templateDocumentService,
            ILocationService locationService,
            IExpertService expertService,
            ILicensingProcedureDocumentCPOService LicensingService,
            ILicensingChangeProcedureCPOService ChangeLicensingService,
            ISettingService settingService,
            ILicensingProcedureDocumentCIPOService LicensingCIPOService,
            IPersonService personService,
            IUploadFileService uploadFileService,
            UserManager<ApplicationUser> userManager) : base(_repository, authenticationStateProvider)
        {
            repositorysitory = _repository;
            logger = _logger;

            this.dataSourceService = dataSourceService;
            this.applicationUserService = applicationUser;
            this.candidateProviderService = candidateProvider;
            this.MailService = mailService;
            this.expertService = expertService;
            this.templateDocumentService = templateDocumentService;
            this.locationService = locationService;
            this.docuService = docuService;
            this.userManager = userManager;
            this.LicensingService = LicensingService;
            this.ChangeLicensingService = ChangeLicensingService;
            this.LicensingCIPOService = LicensingCIPOService;
            this.personService = personService;
            this.settingService = settingService;
            this.uploadFileService = uploadFileService;
        }

        public async Task<int> CreateProviderAsync(ProviderVM providerVM)
        {
            Provider provider = providerVM.To<Provider>();
            await this.repository.AddAsync<Provider>(provider);
            return await this.repository.SaveChangesAsync();
        }

        public async Task<int> DeleteProviderAsync(int providerId)
        {
            Provider provider = await this.repository.GetByIdAsync<Provider>(providerId);

            if (provider != null)
            {
                this.repository.Detach<Provider>(provider);
                this.repository.HardDelete<Provider>(provider);

                return await this.repository.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<int> UpdateProviderAsync(ProviderVM model)
        {
            Provider provider = await this.repository.GetByIdAsync<Provider>(model.Id);

            if (provider != null)
            {
                this.repository.Detach<Provider>(provider);
                provider = model.To<Provider>();
                this.repository.Update<Provider>(provider);

                return await this.repository.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<IEnumerable<ProviderVM>> GetAllProvidersAsync()
        {
            IQueryable<Provider> data = this.repository.All<Provider>();
            return await data.To<ProviderVM>().ToListAsync();
        }

        public async Task<ProviderVM> GetProviderByIdAsync(int providerId)
        {
            Provider provider = await this.repository.GetByIdAsync<Provider>(providerId);

            if (provider != null)
            {
                this.repository.Detach<Provider>(provider);
                ProviderVM providerVM = provider.To<ProviderVM>();

                return providerVM;
            }

            return null;
        }

        public async Task<IEnumerable<ProcedureExternalExpertVM>> GetAllProcedureExternalExpertsAsync(ProcedureExternalExpertVM model)
        {
            IQueryable<ProcedureExternalExpert> data = this.repository.All<ProcedureExternalExpert>(FilterProcedureExpert(model));
            return await data.To<ProcedureExternalExpertVM>(x => x.Expert.Person, x => x.ProfessionalDirection).ToListAsync();
        }
        public async Task<IEnumerable<ProcedureExpertCommissionVM>> GetAllProcedureExpertExpertCommissionsAsync()
        {
            IQueryable<ProcedureExpertCommission> data = this.repository.All<ProcedureExpertCommission>();

            return await data.To<ProcedureExpertCommissionVM>(p => p.StartedProcedure).ToListAsync();
        }

        public async Task<IEnumerable<NegativeIssueVM>> GetAllNegativeIssuesByIdStartedProcedureAsync(int idStartedProcedure)
        {
            IQueryable<NegativeIssue> data = this.repository.All<NegativeIssue>(n => n.IdStartedProcedure == idStartedProcedure);
            return await data.To<NegativeIssueVM>().ToListAsync();
        }

        public async Task<IEnumerable<ProcedureDocumentVM>> GetAllProcedureDocumentsAsync(ProcedureDocumentVM model)
        {
            var docTypeValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType",false,true);

            IQueryable<ProcedureDocument> data = this.repository.All<ProcedureDocument>(FilterProcedureDocument(model));
            var list = await data.To<ProcedureDocumentVM>().ToListAsync();

            int counter = 1;
            foreach (var item in list)
            {
                item.gridRowCounter = counter;
                counter++;

                var type = docTypeValues.FirstOrDefault(d => d.IdKeyValue == (item.IdDocumentType.HasValue ? item.IdDocumentType.Value : GlobalConstants.INVALID_ID));
                if (type is not null)
                {
                    item.DocumentTypeNameDescription = type.Name + " - " + type.Description;
                    item.Order = type.Order;
                }
            }

            return list;
        }

        public async Task<ProcedureExpertCommissionVM> GetProcedureExpertCommissionByIdStartProcedureAsync(int idStartedProcedure)
        {
            IQueryable<ProcedureExpertCommission> data = this.repository.All<ProcedureExpertCommission>(x => x.IdStartedProcedure == idStartedProcedure);
            var listVm = await data.To<ProcedureExpertCommissionVM>().ToListAsync();
            return listVm.FirstOrDefault();
        }

        public async Task<IEnumerable<StartedProcedureProgressVM>> GetAllStartedProcedureProgressByIdStartProcedureAsync(int idStartedProcedure)
        {
            IQueryable<StartedProcedureProgress> data = this.repository.All<StartedProcedureProgress>(x => x.IdStartedProcedure == idStartedProcedure);
            var listVm = await data.To<StartedProcedureProgressVM>().ToListAsync();
            return listVm;
        }

        public async Task<ResultContext<ProcedureExternalExpertVM>> DeleteProcedureExternalExpertAsync(ResultContext<ProcedureExternalExpertVM> resultContext)
        {
            try
            {
                //var deleteEnity = await this.GetByIdAsync<ProcedureExternalExpert>(resultContext.ResultContextObject.IdProcedureExternalExpert);
                //this.repository.Detach<ProcedureExternalExpert>(deleteEnity);

                await this.repository.HardDeleteAsync<ProcedureExternalExpert>(resultContext.ResultContextObject.IdProcedureExternalExpert);
                //if (deleteEnity != null)
                //{
                //}

                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    resultContext.AddMessage("Записът е изтрит успешно!");
                }
                else
                {
                    resultContext.AddErrorMessage("Грешка при изтриване в базата!");
                }

                return resultContext;
            }
            catch (Exception ex)
            {
                logger.LogError($"Message - {ex.Message}");
                logger.LogError($"InnerException - {ex.InnerException}");
                logger.LogError($"StackTrace - {ex.StackTrace}");
                resultContext.AddErrorMessage(ex.Message);

                return resultContext;

            }
        }

        public async Task<ResultContext<NegativeIssueVM>> DeleteNegativeIssueAsync(ResultContext<NegativeIssueVM> resultContext)
        {
            try
            {
                await this.repository.HardDeleteAsync<NegativeIssue>(resultContext.ResultContextObject.IdNegativeIssue);

                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    resultContext.AddMessage("Записът е изтрит успешно!");
                }
                else
                {
                    resultContext.AddErrorMessage("Грешка при изтриване в базата!");
                }

                return resultContext;
            }
            catch (Exception ex)
            {
                logger.LogError($"Message - {ex.Message}");
                logger.LogError($"InnerException - {ex.InnerException}");
                logger.LogError($"StackTrace - {ex.StackTrace}");
                resultContext.AddErrorMessage(ex.Message);

                return resultContext;

            }
        }

        public async Task<ResultContext<ProcedureExpertCommissionVM>> SaveProcedureExpertCommissionAsync(ResultContext<ProcedureExpertCommissionVM> resultContext)
        {

            if (resultContext.ResultContextObject.IdProcedureExpertCommission > GlobalConstants.INVALID_ID_ZERO)
            {
                //Update
                var updatedEnity = await this.GetByIdAsync<ProcedureExpertCommission>(resultContext.ResultContextObject.IdProcedureExpertCommission);
                this.repository.Detach<ProcedureExpertCommission>(updatedEnity);
                updatedEnity = resultContext.ResultContextObject.To<ProcedureExpertCommission>();
                this.repository.Update(updatedEnity);
            }
            else
            {
                //Create new
                var data = resultContext.ResultContextObject.To<ProcedureExpertCommission>();
                await this.repository.AddAsync<ProcedureExpertCommission>(data);
            }


            var result = await this.repository.SaveChangesAsync();

            if (result > 0)
            {
                resultContext.AddMessage("Записът e успешeн!");
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }

            return resultContext;
        }

        public async Task<ResultContext<ProcedureExternalExpertVM>> SaveProcedureExternalExpertAsync(ResultContext<ProcedureExternalExpertVM> resultContext)
        {

            if (resultContext.ResultContextObject.IdProcedureExternalExpert > GlobalConstants.INVALID_ID_ZERO)
            {
                //Update
                var updatedEnity = await this.GetByIdAsync<ProcedureExternalExpert>(resultContext.ResultContextObject.IdProcedureExternalExpert);
                this.repository.Detach<ProcedureExternalExpert>(updatedEnity);
                updatedEnity = resultContext.ResultContextObject.To<ProcedureExternalExpert>();
                updatedEnity.Expert = null;
                this.repository.Update(updatedEnity);
            }
            else
            {
                //Create new
                var data = resultContext.ResultContextObject.To<ProcedureExternalExpert>();
                data.IsActive = true;

                data.Expert = null;

                await this.repository.AddAsync<ProcedureExternalExpert>(data);

                //AdministrativeCheck
                var candidate = this.repository.All<CandidateProvider>().Where(x => x.IdStartedProcedure == data.IdStartedProcedure).First();

                candidate.IdApplicationStatus = (await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "AdministrativeCheck")).IdKeyValue;


                this.repository.Update(candidate);
            }


            var result = await this.repository.SaveChangesAsync(false);

            if (result > 0)
            {
                resultContext.AddMessage("Записът e успешeн!");
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }

            return resultContext;
        }

        public async Task<ResultContext<NegativeIssueVM>> SaveNegativeIssueAsync(ResultContext<NegativeIssueVM> resultContext)
        {

            var data = resultContext.ResultContextObject.To<NegativeIssue>();
            var result = await this.repository.SaveChangesAsync();
            await this.repository.AddAsync<NegativeIssue>(data);

            var candidateProvider = await this.repository.AllReadonly<CandidateProvider>(x => x.IdStartedProcedure == data.IdStartedProcedure).FirstOrDefaultAsync();
            if (candidateProvider is not null)
            {
                var kvNegativeStepApplicationStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "LeadingExpertGaveNegativeAssessment");
                candidateProvider.IdApplicationStatus = kvNegativeStepApplicationStatus.IdKeyValue;

                this.repository.Update<CandidateProvider>(candidateProvider);
                await this.repository.SaveChangesAsync(false);
            }


            if (result > 0)
            {
                resultContext.AddMessage("Записът e успешeн!");
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }

            return resultContext;
        }

        public async Task SetApplicationStatusAfterPositiveAssessmentAsync(int idCandidateProvider)
        {
            try
            {
                var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(idCandidateProvider);
                if (candidateProviderFromDb is not null)
                {
                    var kvApplicationStatusPositiveAssessment = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "LeadingExpertGavePositiveAssessment");
                    candidateProviderFromDb.IdApplicationStatus = kvApplicationStatusPositiveAssessment.IdKeyValue;

                    this.repository.Update<CandidateProvider>(candidateProviderFromDb);
                    await this.repository.SaveChangesAsync(false);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public async Task UpdateStartedProcedureNapooDeadlineAsync(int idStartedProcedure, DateTime deadline)
        {

            var updatedEnity = await this.GetByIdAsync<StartedProcedure>(idStartedProcedure);

            updatedEnity.NapooReportDeadline = deadline;

            this.repository.Update(updatedEnity);

            await this.repository.SaveChangesAsync();
        }

        public async Task UpdateStartedProcedureExpertDeadlineAsync(int idStartedProcedure, DateTime deadline)
        {

            var updatedEnity = await this.GetByIdAsync<StartedProcedure>(idStartedProcedure);

            updatedEnity.ExpertReportDeadline = deadline;

            this.repository.Update(updatedEnity);

            await this.repository.SaveChangesAsync();
        }

        public async Task<StartedProcedureVM> GetStartedProcedureByIdAsync(int idStartedProcedure)
        {
            StartedProcedureVM procedureVM = new StartedProcedureVM();
            StartedProcedure procedure = await this.repository.GetByIdAsync<StartedProcedure>(idStartedProcedure);

            if (procedure != null)
            {
                this.repository.Detach<StartedProcedure>(procedure);
                procedureVM = procedure.To<StartedProcedureVM>();

            }

            return procedureVM;
        }
        public async Task<List<ProcedureExpertCommissionVM>> GetStartedProcedureExpertCommissionByIdAsync(int idStartedProcedure)
        {
            var commission = this.repository.All<ProcedureExpertCommission>().Where(x => x.IdStartedProcedure == idStartedProcedure);
            var result = commission.To<ProcedureExpertCommissionVM>();

            return await result.ToListAsync();
        }

        public async Task<StartedProcedureVM> GetStartedProcedureByIdForGenerateDocumentAsync(int idStartedProcedure)
        {
            var procedure = this.repository.All<StartedProcedure>(p => p.IdStartedProcedure == idStartedProcedure);

            var procedureVMList = procedure.To<StartedProcedureVM>(
                p => p.ProcedureExternalExperts.Select(e => e.ProfessionalDirection),
                p => p.ProcedureExternalExperts.Select(e => e.Expert.Person),
                p => p.ProcedureExternalExperts.Select(e => e.Expert.ExpertProfessionalDirections.Select(x => x.ProfessionalDirection)),
                p => p.ProcedureExpertCommissions,
                p => p.NegativeIssues,
                p => p.ProcedureDocuments.Select(e => e.Expert.Person),
                p => p.CandidateProviders.Select(c => c.CandidateProviderSpecialities
                                                 .Select(ps => ps.Speciality.Profession.ProfessionalDirection)));

            return procedureVMList.FirstOrDefault();
        }

        public async Task<ResultContext<StartedProcedureProgressVM>> InsertStartedProcedureProgressAsync(ResultContext<StartedProcedureProgressVM> resultContext)
        {
            var data = resultContext.ResultContextObject.To<StartedProcedureProgress>();
            await this.repository.AddAsync<StartedProcedureProgress>(data);

            var result = await this.repository.SaveChangesAsync();

            if (result > 0)
            {
                resultContext.AddMessage("Записът e успешeн!");
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }

            return resultContext;
        }

        public async Task<ResultContext<ProcedureDocumentVM>> InsertProcedureDocumentAsync(ResultContext<ProcedureDocumentVM> resultContext)
        {
            var data = resultContext.ResultContextObject.To<ProcedureDocument>();
            await this.repository.AddAsync<ProcedureDocument>(data);

            var result = await this.repository.SaveChangesAsync();

            if (result > 0)
            {
                resultContext.AddMessage("Записът e успешeн!");
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }

            return resultContext;
        }

        public async Task<ResultContext<CandidateProviderVM>> StartCIPOProcedureAsync(ResultContext<CandidateProviderVM> resultContext, bool isApplicationSentViaIS)
        {
            try
            {
                //var kvStepSubmited = await dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "RequestedByCPOOrCIPO");
                var kvStepProcedureWasRegisteredInKeepingSystem = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "ProcedureWasRegisteredInKeepingSystem");
                var kvStepRequestedByCPOOrCIPO = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "RequestedByCPOOrCIPO");
                var kvDocTypeRequestLicensing = await this.dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "RequestLicensingCIPO");
                var ProcedureDocumentTypes = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType")).Where(x => !string.IsNullOrEmpty(x.DefaultValue6)).ToList();

                var candidate = await this.repository.GetByIdAsync<CandidateProvider>(resultContext.ResultContextObject.IdCandidate_Provider);

                var startedProcedureProgress = new StartedProcedureProgress();
                startedProcedureProgress.StepDate = DateTime.Now;

                var document = new ProcedureDocument();
                document.IsValid = true;
                document.IdDocumentType = kvDocTypeRequestLicensing.IdKeyValue;

                var startedProcedure = new StartedProcedure();

                startedProcedure.StartedProcedureProgresses.Add(startedProcedureProgress);
                startedProcedure.ProcedureDocuments.Add(document);

                await this.repository.AddAsync<StartedProcedure>(startedProcedure);
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    resultContext.AddMessage("Успешно подадено заявление!");
                    candidate.IdStartedProcedure = startedProcedure.IdStartedProcedure;
                    this.repository.Update(candidate);
                    result = await this.repository.SaveChangesAsync();
                    resultContext.ResultContextObject.IdStartedProcedure = candidate.IdStartedProcedure;
                    var kvApplicationFilingSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationFilingType");
                    var kvReceiveLicenseSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ReceiveLicenseType");
                    var kvVQSSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");

                    var indexUser = await dataSourceService.GetSettingByIntCodeAsync("IndexUserId");

                    if (isApplicationSentViaIS)
                    {
                        //var file = await this.LicensingService.GenerateLicensingApplication(candidate.To<CandidateProviderVM>(), kvApplicationFilingSource, kvReceiveLicenseSource, kvVQSSource);

                        var kvESignedApplication = kvApplicationFilingSource.FirstOrDefault(x => x.KeyValueIntCode == "ThroughESignature").IdKeyValue;

                        var documentStream = await this.uploadFileService.GetUploadedFileESignedApplicationAsync(candidate.To<CandidateProviderVM>());

                        if (resultContext.ResultContextObject.IdApplicationFiling == kvESignedApplication)
                        {
                            FileData[] files = new FileData[]
                            {
                            new FileData()
                            {
                                BinaryContent = documentStream.ToArray(),
                                ContentType = "application/pdf",
                                Filename = "Zaqvlenie-Licenzirane-CIPO.pdf"
                            }
                            };

                            RegisterDocumentParams registerDocumentParams = new RegisterDocumentParams()
                            {
                                ExternalCode = kvDocTypeRequestLicensing.DefaultValue2,
                                RegisterUser = int.Parse(indexUser.SettingValue),
                                RegisterUserSpecified = true
                            };

                            CorrespData corresp = new CorrespData()
                            {
                                Names = candidate.ProviderName,
                                EIK = candidate.PoviderBulstat,
                                Phone = candidate.ProviderPhone,
                                Email = candidate.ProviderEmail
                            };

                            DocData docs = new DocData()
                            {
                                Otnosno = kvDocTypeRequestLicensing.Description,
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

                            document.DS_OFFICIAL_ID = documentResponse.Doc.DocID;
                            document.DS_OFFICIAL_GUID = documentResponse.Doc.GUID;
                            document.MimeType = documentResponse.Doc.File[0].ContentType;
                            document.DS_OFFICIAL_DATE = documentResponse.Doc.DocDate;
                            document.DS_OFFICIAL_DocNumber = documentResponse.Doc.DocNumber;

                            candidate.ApplicationNumber = documentResponse.Doc.DocNumber;
                            candidate.ApplicationDate = documentResponse.Doc.DocDate;

                            candidate.IdApplicationStatus = kvStepRequestedByCPOOrCIPO.IdKeyValue;
                            startedProcedureProgress.IdStep = kvStepRequestedByCPOOrCIPO.IdKeyValue;

                            resultContext.ResultContextObject.ApplicationNumber = candidate.ApplicationNumber;
                            resultContext.ResultContextObject.ApplicationDate = candidate.ApplicationDate;
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(candidate.ApplicationNumber) && candidate.ApplicationDate.HasValue)
                        {
                            var doc = await this.docuService.GetIdentDocument(candidate.ApplicationNumber, 0, candidate.ApplicationDate.Value);

                            var contextResponse = await this.docuService.GetDocumentAsync(doc.DocIdent.First().DocID, doc.DocIdent.First().GUID);

                            if (contextResponse.HasErrorMessages)
                            {
                                resultContext.ListErrorMessages = contextResponse.ListErrorMessages;

                                return resultContext;
                            }

                            var documentResponse = contextResponse.ResultContextObject;

                            document.DS_OFFICIAL_ID = documentResponse.Doc.DocID;
                            document.DS_OFFICIAL_GUID = documentResponse.Doc.GUID;
                            document.MimeType = documentResponse.Doc.File[0].ContentType;
                            document.DS_OFFICIAL_DATE = documentResponse.Doc.DocDate;
                            document.DS_OFFICIAL_DocNumber = documentResponse.Doc.DocNumber;

                            document.IdDocumentType = ProcedureDocumentTypes.Where(x => Int32.Parse(x.DefaultValue6) == documentResponse.Doc.DocVidCode).First().IdKeyValue;

                            candidate.IdApplicationStatus = kvStepRequestedByCPOOrCIPO.IdKeyValue;
                            startedProcedureProgress.IdStep = kvStepRequestedByCPOOrCIPO.IdKeyValue;

                            resultContext.ResultContextObject.ApplicationNumber = candidate.ApplicationNumber;
                            resultContext.ResultContextObject.ApplicationDate = candidate.ApplicationDate;
                        }
                        else
                        {
                            candidate.IdApplicationStatus = kvStepRequestedByCPOOrCIPO.IdKeyValue;
                            startedProcedureProgress.IdStep = kvStepRequestedByCPOOrCIPO.IdKeyValue;
                        }
                    }
                    this.repository.Update(document);
                    this.repository.Update(candidate);
                    this.repository.Update(startedProcedureProgress);
                    await this.repository.SaveChangesAsync(false);
                }
                else
                {
                    resultContext.AddErrorMessage("Грешка при запис в базата!");
                }

                return resultContext;
            }
            catch (Exception ex)
            {
                logger.LogError($"Message - {ex.Message}");
                logger.LogError($"InnerException - {ex.InnerException}");
                logger.LogError($"StackTrace - {ex.StackTrace}");
                resultContext.AddErrorMessage(ex.Message);

                return resultContext;


            }
        }

        public async Task<ResultContext<CandidateProviderVM>> StartProcedureAsync(ResultContext<CandidateProviderVM> resultContext, bool isApplicationSentViaIS, bool isCPO)
        {
            try
            {
                //var kvStepSubmited = await dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "RequestedByCPOOrCIPO");
                var kvStepProcedureWasRegisteredInKeepingSystem = await dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "ProcedureWasRegisteredInKeepingSystem");
                var kvStepRequestedByCPOOrCIPO = await dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "RequestedByCPOOrCIPO");
                var ProcedureDocumentTypes = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType")).Where(x => !string.IsNullOrEmpty(x.DefaultValue6));
                
                var kvDocTypeRequestLicensing = isCPO 
                    ? await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "RequestLicensingCPO")
                    : await dataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "RequestLicensingCIPO");

                var candidate = await this.repository.GetByIdAsync<CandidateProvider>(resultContext.ResultContextObject.IdCandidate_Provider);

                var startedProcedureProgress = new StartedProcedureProgress();
                startedProcedureProgress.StepDate = DateTime.Now;

                var document = new ProcedureDocument();
                document.IsValid = true;
                document.IdDocumentType = kvDocTypeRequestLicensing.IdKeyValue;

                var startedProcedure = new StartedProcedure();

                startedProcedure.StartedProcedureProgresses.Add(startedProcedureProgress);
                startedProcedure.ProcedureDocuments.Add(document);

                await this.repository.AddAsync<StartedProcedure>(startedProcedure);
                var result = await this.repository.SaveChangesAsync();
                if (result > 0)
                {
                    resultContext.AddMessage("Успешно подадено заявление!");
                    candidate.IdStartedProcedure = startedProcedure.IdStartedProcedure;
                    this.repository.Update(candidate);
                    result = await this.repository.SaveChangesAsync();
                    resultContext.ResultContextObject.IdStartedProcedure = candidate.IdStartedProcedure;
                    var kvApplicationFilingSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationFilingType");
                    var kvReceiveLicenseSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ReceiveLicenseType");
                    var kvVQSSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");

                    var indexUser = await dataSourceService.GetSettingByIntCodeAsync("IndexUserId");

                    if (isApplicationSentViaIS)
                    {
                        //var file = isCPO
                        //    ? await this.LicensingService.GenerateLicensingApplication(candidate.To<CandidateProviderVM>(), kvApplicationFilingSource, kvReceiveLicenseSource, kvVQSSource)
                        //    : await this.LicensingCIPOService.GenerateLicensingApplication(candidate.To<CandidateProviderVM>(), kvApplicationFilingSource, kvReceiveLicenseSource, kvVQSSource);

                        var kvESignedApplication = kvApplicationFilingSource.FirstOrDefault(x => x.KeyValueIntCode == "ThroughESignature").IdKeyValue;

                        var documentStream = await this.uploadFileService.GetUploadedFileESignedApplicationAsync(candidate.To<CandidateProviderVM>());

                        if (resultContext.ResultContextObject.IdApplicationFiling == kvESignedApplication)
                        {
                            FileData[] files = new FileData[]
                            {
                            new FileData()
                            {
                                BinaryContent = documentStream.ToArray(),
                                ContentType = "application/pdf",
                                Filename = "Zaqvlenie-Licenzirane-CPO.pdf"
                            }
                            };

                            RegisterDocumentParams registerDocumentParams = new RegisterDocumentParams()
                            {
                                ExternalCode = kvDocTypeRequestLicensing.DefaultValue2,
                                RegisterUser = int.Parse(indexUser.SettingValue),
                                RegisterUserSpecified = true
                            };

                            CorrespData corresp = new CorrespData()
                            {
                                Names = candidate.ProviderName,
                                EIK = candidate.PoviderBulstat,
                                Phone = candidate.ProviderPhone,
                                Email = candidate.ProviderEmail
                            };

                            DocData docs = new DocData()
                            {
                                Otnosno = kvDocTypeRequestLicensing.Description,
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

                            document.DS_OFFICIAL_ID = documentResponse.Doc.DocID;
                            document.DS_OFFICIAL_GUID = documentResponse.Doc.GUID;
                            document.MimeType = documentResponse.Doc.File[0].ContentType;
                            document.DS_OFFICIAL_DATE = documentResponse.Doc.DocDate;
                            document.DS_OFFICIAL_DocNumber = documentResponse.Doc.DocNumber;

                            candidate.ApplicationNumber = documentResponse.Doc.DocNumber;
                            candidate.ApplicationDate = documentResponse.Doc.DocDate;

                            candidate.IdApplicationStatus = kvStepRequestedByCPOOrCIPO.IdKeyValue;
                            startedProcedureProgress.IdStep = kvStepRequestedByCPOOrCIPO.IdKeyValue;

                            document.IdDocumentType = ProcedureDocumentTypes.Where(x => Int32.Parse(x.DefaultValue6) == documentResponse.Doc.DocVidCode).First().IdKeyValue;

                            resultContext.ResultContextObject.ApplicationNumber = candidate.ApplicationNumber;
                            resultContext.ResultContextObject.ApplicationDate = candidate.ApplicationDate;
                        }
                    }
                    else
                    {
                       if(!string.IsNullOrEmpty(candidate.ApplicationNumber) && candidate.ApplicationDate.HasValue)
                        {
                            var doc = await this.docuService.GetIdentDocument(candidate.ApplicationNumber, 0, candidate.ApplicationDate.Value);

                            var contextResponse = await this.docuService.GetDocumentAsync(doc.DocIdent.First().DocID, doc.DocIdent.First().GUID);

                            if (contextResponse.HasErrorMessages)
                            {
                                resultContext.ListErrorMessages = contextResponse.ListErrorMessages;

                                return resultContext;
                            }

                            var documentResponse = contextResponse.ResultContextObject;

                            document.DS_OFFICIAL_ID = documentResponse.Doc.DocID;
                            document.DS_OFFICIAL_GUID = documentResponse.Doc.GUID;
                            document.MimeType = documentResponse.Doc.File[0].ContentType;
                            document.DS_OFFICIAL_DATE = documentResponse.Doc.DocDate;
                            document.DS_OFFICIAL_DocNumber = documentResponse.Doc.DocNumber;

                            candidate.IdApplicationStatus = kvStepRequestedByCPOOrCIPO.IdKeyValue;
                            startedProcedureProgress.IdStep = kvStepRequestedByCPOOrCIPO.IdKeyValue;

                            resultContext.ResultContextObject.ApplicationNumber = candidate.ApplicationNumber;
                            resultContext.ResultContextObject.ApplicationDate = candidate.ApplicationDate;
                        }
                        else
                        {
                            candidate.IdApplicationStatus = kvStepRequestedByCPOOrCIPO.IdKeyValue;
                            startedProcedureProgress.IdStep = kvStepRequestedByCPOOrCIPO.IdKeyValue;
                        }
                    }

                    this.repository.Update(document);
                    this.repository.Update(candidate);
                    this.repository.Update(startedProcedureProgress);
                    await this.repository.SaveChangesAsync(false);
                }
                else
                {
                    resultContext.AddErrorMessage("Грешка при запис в базата!");
                }

                return resultContext;
            }
            catch (Exception ex)
            {
                logger.LogError($"Message - {ex.Message}");
                logger.LogError($"InnerException - {ex.InnerException}");
                logger.LogError($"StackTrace - {ex.StackTrace}");
                resultContext.AddErrorMessage(ex.Message);

                return resultContext;


            }
        }

        protected Expression<Func<ProcedureExternalExpert, bool>> FilterProcedureExpert(ProcedureExternalExpertVM model)
        {
            var predicate = PredicateBuilder.True<ProcedureExternalExpert>();

            if (model.IsLeadingExpert.HasValue && model.IsLeadingExpert.Value)
            {
                predicate = predicate.And(p => p.IdProfessionalDirection == null);
            }
            if (model.IsLeadingExpert.HasValue && !model.IsLeadingExpert.Value)
            {
                predicate = predicate.And(p => p.IdProfessionalDirection != null);
            }
            if (model.IdStartedProcedure > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdStartedProcedure == model.IdStartedProcedure);
            }

            return predicate;
        }

        protected Expression<Func<ProcedureDocument, bool>> FilterProcedureDocument(ProcedureDocumentVM model)
        {
            var predicate = PredicateBuilder.True<ProcedureDocument>();

            if (model.IdStartedProcedure > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdStartedProcedure == model.IdStartedProcedure);
            }

            return predicate;
        }


        public async Task<IEnumerable<ProcedurePriceVM>> GetAllProcedurePricesAsync()
        {
            IQueryable<ProcedurePrice> data = this.repository.All<ProcedurePrice>();
            var priceVMs = await data.To<ProcedurePriceVM>().ToListAsync();

            var applicationTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingFee");
            var applicationStatuses = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");

            foreach (var entry in priceVMs)
            {
                var applicationType = applicationTypes.FirstOrDefault(a => a.IdKeyValue == entry.IdTypeApplication);
                var applicationStatus = applicationStatuses.FirstOrDefault(a => a.IdKeyValue == entry.IdApplicationStatus);

                if (applicationType is not null)
                {
                    entry.TypeApplicationName = applicationType.Name;
                }

                if (applicationStatus is not null)
                {
                    entry.ApplicationStatusName = applicationStatus.Name;
                }
            }

            return priceVMs.OrderBy(x => x.Name).ToList();
        }

        public async Task<ProcedurePriceVM> GetProcedurePriceByIdAsync(ProcedurePriceVM model)
        {
            var data = await this.repository.All<ProcedurePrice>(x => x.IdProcedurePrice == model.IdProcedurePrice).FirstOrDefaultAsync();

            this.repository.Detach<ProcedurePrice>(data);

            ProcedurePriceVM viewModel = data.To<ProcedurePriceVM>();

            return viewModel;

        }
        public async Task<List<ProcedurePriceVM>> GetProcedurePriceWithPredicateAsync(ProcedurePriceVM model)
        {
            //var filter = FilterProcedurePrice(model);

            var data = this.repository.All<ProcedurePrice>(x => x.IdTypeApplication == model.IdTypeApplication);

            var viewModel = await data.To<ProcedurePriceVM>().ToListAsync();

            return viewModel;

        }
        //protected Expression<Func<ProcedurePrice, bool>> FilterProcedurePrice(ProcedurePriceVM model)
        //{
        //    var predicate = PredicateBuilder.True<ProcedurePrice>();

        //    if (model.IdTypeApplication != 0)
        //    {
        //        predicate.And(x => x.IdTypeApplication == model.IdTypeApplication);
        //    }
        //    if (model.IdApplicationStatus != null)
        //    {
        //        predicate.And(x => x.IdApplicationStatus == model.IdApplicationStatus);
        //    }
        //    if (model.CountProfessionsFrom != null)
        //    {
        //        predicate.And(x => x.CountProfessionsFrom >= model.CountProfessionsFrom);
        //    }
        //    if (model.CountProfessionsTo != null)
        //    {
        //        predicate.And(x => x.CountProfessionsTo <= model.CountProfessionsTo);
        //    }

        //    return predicate;
        //}

        public async Task<ResultContext<ProcedurePriceVM>> SaveProcedurePriceAsync(ResultContext<ProcedurePriceVM> resultContext)
        {
            if (resultContext.ResultContextObject.IdProcedurePrice == GlobalConstants.INVALID_ID_ZERO)
            {
                resultContext = await CreateProcedurePriceAsync(resultContext);
            }
            else
            {
                resultContext = await UpdateProcedurePriceAsync(resultContext);
            }

            return resultContext;
        }

        private async Task<ResultContext<ProcedurePriceVM>> CreateProcedurePriceAsync(ResultContext<ProcedurePriceVM> resultContext)
        {
            var newPrice = resultContext.ResultContextObject.To<ProcedurePrice>();



            await this.repository.AddAsync<ProcedurePrice>(newPrice);
            int result = await this.repository.SaveChangesAsync();

            if (result > 0)
            {
                resultContext.AddMessage("Записът e успешeн!");
                resultContext.ResultContextObject.IdProcedurePrice = newPrice.IdProcedurePrice;
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }

            return resultContext;
        }

        private async Task<ResultContext<ProcedurePriceVM>> UpdateProcedurePriceAsync(ResultContext<ProcedurePriceVM> resultContext)
        {
            try
            {
                var updatedEnity = await this.GetByIdAsync<ProcedurePrice>(resultContext.ResultContextObject.IdProcedurePrice);
                this.repository.Detach<ProcedurePrice>(updatedEnity);

                updatedEnity = resultContext.ResultContextObject.To<ProcedurePrice>();


                this.repository.Update(updatedEnity);
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    resultContext.AddMessage("Записът e успешeн!");
                }
                else
                {
                    resultContext.AddErrorMessage("Грешка при запис в базата!");
                }

                return resultContext;
            }
            catch (Exception еx)
            {
                return resultContext;
            }
        }

        public async Task<IEnumerable<ProviderVM>> GetProvidersByListIdsAsync(List<int> ids)
        {
            var data = this.repository.AllReadonly<Provider>(x => ids.Contains(x.Id));

            return await data.To<ProviderVM>().ToListAsync();
        }
        public async Task<ProcedureDocumentVM> GetProcedureDocumentByIdStartedProcedureAndIdDocumentTypeAsync(int idStartedProcedure, int idDocumentType)
        {
            var data = this.repository.AllReadonly<ProcedureDocument>(x => x.IdStartedProcedure == idStartedProcedure && x.IdDocumentType == idDocumentType);

            return await data.To<ProcedureDocumentVM>().FirstOrDefaultAsync();
        }
        public async Task<ProcedureDocumentVM> GetProcedureDocumentByIdStartedProcedureByIdDocumentTypeAndByIdExpertAsync(int idStartedProcedure, int idDocumentType, int idExpert)
        {
            var data = this.repository.AllReadonly<ProcedureDocument>(x => x.IdStartedProcedure == idStartedProcedure && x.IdDocumentType == idDocumentType && x.IdExpert == idExpert);

            return await data.To<ProcedureDocumentVM>().FirstOrDefaultAsync();
        }

        public async Task<ResultContext<NoResult>> SaveNewDocumentByNumberAndDate(ProcedureDocumentVM doc)
        {
            var resultContext = new ResultContext<NoResult>();

            
            var documentData = await docuService.GetIdentDocument(
                    doc.ApplicationNumber, 
                    doc.DeloSerial.HasValue? 0:doc.DeloSerial.Value, 
                    doc.ApplicationDate.Value);

            var contextResponse = await this.docuService.GetDocumentAsync(documentData.DocIdent.First().DocID, documentData.DocIdent.First().GUID);

            var ProcedureDocumentTypes = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType")).Where(x => !string.IsNullOrEmpty(x.DefaultValue6));

            if (contextResponse.HasErrorMessages)
            {
                resultContext.ListErrorMessages = contextResponse.ListErrorMessages;

                return resultContext;
            }

            var documentResponse = contextResponse.ResultContextObject;
            doc.DS_OFFICIAL_ID = documentResponse.Doc.DocID;
            doc.DS_OFFICIAL_GUID = documentResponse.Doc.GUID;
            doc.DS_OFFICIAL_DATE = documentResponse.Doc.DocDate;
            doc.DS_OFFICIAL_DocNumber = documentResponse.Doc.DocNumber;
            doc.IsFromDS = true;

            doc.DeloSerial = documentResponse.Doc.DeloSerial;

            doc.IdDocumentType = ProcedureDocumentTypes.Where(x => Int32.Parse(x.DefaultValue6) == documentResponse.Doc.DocVidCode).First().IdKeyValue;

            await repository.AddAsync(doc.To<ProcedureDocument>());
            await repository.SaveChangesAsync();

            return resultContext;
        }

        #region CandidateProviderPerson
        public async Task<IEnumerable<PersonVM>> GetAllPersonsForNotificationByCandidateProviderIdAsync(int idCandidateProvider)
        {

            List<PersonVM> result = new List<PersonVM>();
            try
            {
                IQueryable<CandidateProviderPerson> data = this.repository.All<CandidateProviderPerson>(x=>x.IdCandidate_Provider == idCandidateProvider);


                result = await data.To<CandidateProviderPersonVM>(x=>x.Person).Where(x=>x.IsAllowedForNotification).Select(x=>x.Person).ToListAsync();
                
            }

            catch (Exception ex)
            {
                logger.LogError("Грешка при изтегляне на CandidateProviderPerson");
                logger.LogError($"Message - {ex.Message}");
                logger.LogError($"StackTrace - {ex.StackTrace}");
                logger.LogError($"InnerException - {ex.InnerException}");
                

                
                
            }

            return result;
        }
        public async Task<IEnumerable<CandidateProviderPersonVM>> GetAllCandidateProviderPersonsAsync(CandidateProviderPersonVM fiterModel)
        {
            try
            {
                IQueryable<CandidateProviderPerson> data = this.repository.All<CandidateProviderPerson>(FilterPersonValue(fiterModel)).Include(x => x.CandidateProvider);
                var viewVMs = await data.To<CandidateProviderPersonVM>(x => x.CandidateProvider, x => x.Person).ToListAsync();

                var kvIndentTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");

                var users = this.repository.AllReadonly<ApplicationUser>();

                foreach (var entry in viewVMs)
                {
                    var user = users.Where(x => x.IdPerson == entry.IdPerson).FirstOrDefault();
                    if (user != null)
                    {
                        entry.Username = user.UserName;
                        entry.Status = await this.dataSourceService.GetKeyValueByIdAsync(user.IdUserStatus.Value);
                        entry.Person.IdApplicationUser = user.Id;
                    }


                    else
                        entry.Username = "N/A";

                    var kvType = kvIndentTypes.FirstOrDefault(x => x.IdKeyValue == entry.Person.IdIndentType);
                    if (kvType is not null)
                    {
                        entry.Person.IndentTypeName = kvType.Name;
                    }
                }

                return viewVMs.OrderBy(v => v.Person.FirstName);

            }
            catch (Exception ex)
            {

                return null;
            }
        }

        protected Expression<Func<CandidateProviderPerson, bool>> FilterPersonValue(CandidateProviderPersonVM model)
        {
            var predicate = PredicateBuilder.True<CandidateProviderPerson>();

            if (model.IdCandidate_Provider > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdCandidate_Provider == model.IdCandidate_Provider);
            }

            return predicate;
        }

        public async Task<CandidateProviderPersonVM> GetCandidateProviderPersonByIdAsync(int candidateProviderPersonId)
        {
            var data = this.repository.AllReadonly<CandidateProviderPerson>(x => x.IdCandidateProviderPerson == candidateProviderPersonId);

            CandidateProviderPersonVM viewModel = await data.To<CandidateProviderPersonVM>(x => x.Person,x => x.CandidateProvider).FirstOrDefaultAsync();
            var user = this.repository.AllReadonly<ApplicationUser>().Where(x => x.IdPerson == viewModel.IdPerson).First();
            viewModel.Status = dataSourceService.GetAllKeyValueList().Where(x => x.IdKeyValue == user.IdUserStatus).First();
            return viewModel;

        }

        public async Task<ResultContext<CandidateProviderPersonVM>> SaveCandidateProviderPersonAsync(ResultContext<CandidateProviderPersonVM> resultContext)
        {
            var isUnique = await personService.CheckPersonIdentIsUniqueForCandidateProvider(resultContext.ResultContextObject.Person.Indent, resultContext.ResultContextObject.Person.IdPerson, resultContext.ResultContextObject.IdCandidate_Provider);

            if (!isUnique)
            {
                resultContext.AddErrorMessage("В базата има записан потребител със същото 'ЕГН/ЛНЧ/ИДН'!");
                return resultContext;
            }


            if (resultContext.ResultContextObject.IdCandidateProviderPerson == GlobalConstants.INVALID_ID_ZERO)
            {
                resultContext = await CreateCandidateProviderPersonAsync(resultContext);
            }
            else
            {
                resultContext = await UpdateCandidateProviderPersonAsync(resultContext);
            }

            return resultContext;
        }

        private async Task<ResultContext<CandidateProviderPersonVM>> CreateCandidateProviderPersonAsync(ResultContext<CandidateProviderPersonVM> resultContext)
        {
            var cp = await this.repository
                .All<CandidateProvider>(cp => cp.IdCandidate_Provider == resultContext.ResultContextObject.IdCandidate_Provider)
                .Include(cp => cp.CandidateProviderPersons).FirstAsync();

            var newCandidatePerson = resultContext.ResultContextObject.To<CandidateProviderPerson>();

            cp.CandidateProviderPersons.Add(newCandidatePerson);

            //await this.repository.AddAsync<CandidateProviderPerson>(newCandidatePerson);
            int result = await this.repository.SaveChangesAsync();

            if (result > 0)
            {
                this.repository.Detach<CandidateProviderPerson>(newCandidatePerson);

                //ApplicationUser
                ResultContext<ApplicationUserVM> resultContextAppUser = new ResultContext<ApplicationUserVM>();
                ApplicationUserVM applicationUser = new ApplicationUserVM();
                var candidateProvider = await this.candidateProviderService.GetCandidateProviderWithoutAnythingIncludedByIdAsync(resultContext.ResultContextObject.IdCandidate_Provider);
                applicationUser.IdPerson = newCandidatePerson.IdPerson;
                applicationUser.Email = newCandidatePerson.Person.Email;
                applicationUser.FirstName = newCandidatePerson.Person.FirstName;
                applicationUser.FamilyName = newCandidatePerson.Person.FamilyName;
                applicationUser.EIK = candidateProvider.PoviderBulstat;
                applicationUser.IdCandidateProvider = candidateProvider.IdCandidate_Provider;
                applicationUser.IdUserStatus = dataSourceService.GetKeyValueByIntCodeAsync("UserStatus", "Active").Result.IdKeyValue;

                resultContextAppUser.ResultContextObject = applicationUser;

                resultContextAppUser = await applicationUserService.CreateApplicationUserAsync(resultContextAppUser);

                //await this.MailService.SendEmailNewRegistrationUserPass(resultContextAppUser);

                resultContext.ResultContextObject = newCandidatePerson.To<CandidateProviderPersonVM>();

                resultContext.AddMessage("Записът e успешeн!");
            }
            else
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }

            return resultContext;
        }

        private async Task<ResultContext<CandidateProviderPersonVM>> UpdateCandidateProviderPersonAsync(ResultContext<CandidateProviderPersonVM> resultContext)
        {
            try
            {
                var updatedEnity = await this.GetByIdAsync<CandidateProviderPerson>(resultContext.ResultContextObject.IdCandidateProviderPerson);

                updatedEnity = resultContext.ResultContextObject.To<CandidateProviderPerson>();

                var user = await this.repository.All<ApplicationUser>().Where(x => x.IdPerson == updatedEnity.IdPerson).FirstAsync();

                user.IdUserStatus = resultContext.ResultContextObject.Status.IdKeyValue;
                user.Email = resultContext.ResultContextObject.Person.Email;
                user.NormalizedEmail = resultContext.ResultContextObject.Person.Email.ToUpper();
                updatedEnity.Person.ModifyDate = DateTime.Now;
                updatedEnity.Person.IdModifyUser = UserProps.UserId;

                this.repository.Update(user);

                this.repository.Update(updatedEnity);
                resultContext.ResultContextObject.Person.IdModifyUser = updatedEnity.Person.IdModifyUser;
                resultContext.ResultContextObject.Person.ModifyDate = updatedEnity.Person.ModifyDate;
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    resultContext.AddMessage("Записът e успешeн!");
                }
                else
                {
                    resultContext.AddErrorMessage("Грешка при запис в базата!");
                }

                return resultContext;
            }
            catch (Exception еx)
            {
                return resultContext;
            }
        }

        #endregion

        public async Task<ResultContext<List<ProcedureDocumentVM>>> InsertProcedureDocumentFromListAsync(ResultContext<List<ProcedureDocumentVM>> resultContext)
        {
            try
            {
                foreach (var item in resultContext.ResultContextObject)
                {
                    var keyValue = dataSourceService.GetAllKeyValueList().Where(x => x.IdKeyValue == item.IdDocumentType).First();

                    var indexUser = await dataSourceService.GetSettingByIntCodeAsync("IndexUserId");

                    var data = item.To<ProcedureDocument>();

                    MemoryStream file = null;

                    if (item.TypeLicensing == GlobalConstants.LICENSING_CPO)
                    {
                        file = await GenerateDocument(item);
                    }
                    else if (item.TypeLicensing == GlobalConstants.CHANCE_LICENSING)
                    {
                        file = await GenerateDocumentChangeLicenzing(item);
                    }
                    else if (item.TypeLicensing == GlobalConstants.LICENSING_CIPO)
                    {
                        file = await GenerateDocumentCipo(item);
                    }


                    if (file == null)
                    {
                        resultContext.AddErrorMessage($"Няма шаблон за {keyValue.Name}");
                        return resultContext;
                    }
                    FileData[] files = new FileData[]
                        {
                        new FileData(){
                        BinaryContent = file.ToArray(),
                        ContentType = "application/docx",
                        Filename = $"{keyValue.KeyValueIntCode}_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx"
                        }
                        };

                    RegisterDocumentParams registerDocumentParams = new RegisterDocumentParams()
                    {
                        ExternalCode = keyValue.DefaultValue2,
                        RegisterUser = int.Parse(indexUser.SettingValue),
                        RegisterUserSpecified = true
                    };
                    DocData docs = new DocData()
                    {
                        Otnosno = keyValue.Description,
                        File = files
                    };

                    var registerResult = await this.docuService.RegisterDocumentAsync(registerDocumentParams, docs);

                    if (registerResult.HasErrorMessages)
                    {
                        resultContext.ListErrorMessages = registerResult.ListErrorMessages;
                        return resultContext;
                    }
                    var documentResponse = registerResult.ResultContextObject;


                    if (keyValue.DefaultValue3.Equals("Unofficial"))
                    {
                        data.DS_ID = documentResponse.Doc.DocID;
                        data.DS_GUID = documentResponse.Doc.GUID;
                        data.DS_DATE = documentResponse.Doc.DocDate;
                        data.DS_DocNumber = documentResponse.Doc.DocNumber;
                    }
                    else if (keyValue.DefaultValue3.Equals("Official"))
                    {
                        data.DS_OFFICIAL_ID = documentResponse.Doc.DocID;
                        data.DS_OFFICIAL_GUID = documentResponse.Doc.GUID;
                        data.DS_OFFICIAL_DATE = documentResponse.Doc.DocDate;
                        data.DS_OFFICIAL_DocNumber = documentResponse.Doc.DocNumber;
                    }
                    await this.repository.AddAsync<ProcedureDocument>(data);

                    logger.LogInformation($"{data.DS_DocNumber} беше записано.");


                }

                await this.repository.SaveChangesAsync();
                resultContext.AddMessage("Записът e успешeн!");

            }
            catch (Exception e)
            {
                resultContext.AddErrorMessage("Грешка при запис в базата!");
            }
            return resultContext;
        }

        public async Task<IEnumerable<ProcedureExternalExpertVM>> GetAllProcedureExternalExpertReportsByIdStartedProcedureAsync(int idStartedProcedure)
        {
            var data = this.repository.AllReadonly<ProcedureExternalExpert>(x => x.IdStartedProcedure == idStartedProcedure && x.IdProfessionalDirection != null);

            return await data.To<ProcedureExternalExpertVM>(x => x.ProfessionalDirection, x => x.Expert.Person, x => x.StartedProcedure).ToListAsync();
        }

        public async Task<int> GetIdExpertByIdPersonAsync(int idPerson)
        {
            var data = await this.repository.AllReadonly<Data.Models.Data.ExternalExpertCommission.Expert>(x => x.IdPerson == idPerson && x.IsExternalExpert).FirstOrDefaultAsync();
            if (data != null)
            {
                return data.IdExpert;
            }
            else
            {
                return 0;
            }
        }

        public async Task<ProcedureExternalExpertVM> GetProcedureExternalExpertByIdAsync(int idProcedureExternalExpert)
        {
            var data = this.repository.AllReadonly<ProcedureExternalExpert>(x => x.IdProcedureExternalExpert == idProcedureExternalExpert);

            return await data.To<ProcedureExternalExpertVM>(x => x.ProfessionalDirection, x => x.Expert.Person, x => x.StartedProcedure).FirstOrDefaultAsync();
        }

        #region Application document generator

        private async Task<MemoryStream> GenerateDocument(ProcedureDocumentVM procedureDocumentVM)
        {
            try
            {
                #region Зареждаме информация от системата
                //Зареждаме статус активен за темплейта
                var kvActiveStatusTemplate = await dataSourceService.GetKeyValueByIntCodeAsync("StatusTemplate", "Active");
                var filterTemplateVM = new TemplateDocumentVM()
                {
                    IdStatus = kvActiveStatusTemplate.IdKeyValue,
                    IdApplicationType = procedureDocumentVM.IdDocumentType.HasValue ? procedureDocumentVM.IdDocumentType.Value : GlobalConstants.INVALID_ID,
                };
                var listTemplates = await this.templateDocumentService.GetAllTemplateDocumentsAsync(filterTemplateVM);
                var template = listTemplates.FirstOrDefault();

                MemoryStream documentStream = new MemoryStream();

                if (template != null && !string.IsNullOrEmpty(template.TemplatePath) && template.TemplatePath != "#")
                {
                    //Зареждаме всички документи за тази процедура
                    var model = new ProcedureDocumentVM();
                    model.IdStartedProcedure = procedureDocumentVM.IdStartedProcedure;
                    var listDocs = await GetAllProcedureDocumentsAsync(model);

                    //Вземаме кей велю на нужните приложения 6 и 7
                    var kvDocTypeList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");
                    var kvDocTypeApplication2 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application2");
                    var kvDocTypeApplication3 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application3");
                    var kvDocTypeApplication6 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application6");
                    var kvDocTypeApplication7 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application7");
                    var kvDocTypeApplication16 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application16");
                    var kvDocTypeApplication17 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application17");
                    var kvDocTypeApplication19 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application19");

                    //Зареждаме данни, тези ще се използват в повече от 1 шаблон
                    var startedProcedureVM = await GetStartedProcedureByIdForGenerateDocumentAsync(procedureDocumentVM.IdStartedProcedure);
                    var kvCommissionList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertCommission");

                    //Зареждане на главен експер и външните експерти
                    var chiefExpert = startedProcedureVM.ProcedureExternalExperts.FirstOrDefault(pe => pe.IdProfessionalDirection == null);
                    var externalExperts = startedProcedureVM.ProcedureExternalExperts.Where(pe => pe.IdProfessionalDirection != null).ToList();

                    //Зареждане на експертна комисия
                    var expertCommision = startedProcedureVM.ProcedureExpertCommissions.FirstOrDefault();
                    var commisionId = GlobalConstants.INVALID_ID_ZERO;
                    var commisionName = string.Empty;
                    if (expertCommision is not null)
                    {
                        commisionName = kvCommissionList.FirstOrDefault(c => c.IdKeyValue == expertCommision.IdExpertCommission).Name;
                        commisionId = expertCommision.IdExpertCommission;
                    }

                    //Зареждаме членовете на експертна комисия
                    //Зареждаме ролята за председател и активен статус
                    var kvRoleCommissionStrainer = await dataSourceService.GetKeyValueByIntCodeAsync("ExpertRoleCommission", "Chairman");
                    var kvRoleCommissionMember = await dataSourceService.GetKeyValueByIntCodeAsync("ExpertRoleCommission", "Member");
                    var kvStatusActive = await dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");

                    //Създаваме си филтър и вземаме данните
                    ExpertExpertCommissionVM filterExpertCommisionVM = new ExpertExpertCommissionVM()
                    {
                        IdExpertCommission = commisionId,
                        IdStatus = kvStatusActive.IdKeyValue,

                    };
                    var expertExpertCommissionList = await this.expertService.GetAllExpertExpertCommissionsAsync(filterExpertCommisionVM);

                    //Отделяме по роля председателя и членовете на комисията
                    var strainerOfExpertCommission = expertExpertCommissionList.FirstOrDefault(e => e.IdRole == kvRoleCommissionStrainer.IdKeyValue);
                    var membersfExpertCommission = expertExpertCommissionList.Where(e => e.IdRole == kvRoleCommissionMember.IdKeyValue);

                    var headOfExpertCommissionName = string.Empty;
                    var headOfExpertCommissionSirName = string.Empty;
                    if (strainerOfExpertCommission is not null)
                    {
                        headOfExpertCommissionName = strainerOfExpertCommission.Expert.Person.FullName;
                        var split = headOfExpertCommissionName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        headOfExpertCommissionSirName = split.Count() > 0 ? split.Last() : string.Empty;
                    }

                    //Зареждаме само имената на членовете на комисията
                    var expertCommissionMembersNames = membersfExpertCommission.Select(e => e.Expert.Person.FullName).ToList();


                    //Зареждаме данните за кореспонденция
                    var candidate = startedProcedureVM.CandidateProviders.FirstOrDefault();

                    LocationVM location = null;
                    if (candidate.IdLocation != null)
                    {
                        location = await this.locationService.GetLocationByIdAsync(candidate.IdLocation.Value);
                    }

                    LocationVM locationCorrespondence = null;
                    if (candidate.IdLocationCorrespondence != null)
                    {
                        locationCorrespondence = await this.locationService.GetLocationByIdAsync(candidate.IdLocationCorrespondence.Value);
                    }

                    var splitNames = candidate.PersonNameCorrespondence.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    var contactData = new ContactPersonData()
                    {
                        FullName = candidate.PersonNameCorrespondence,
                        Sirname = splitNames.Count() > 0 ? splitNames.Last() : "........",
                        CityName = locationCorrespondence != null ? locationCorrespondence.LocationName : "........",
                        PostCode = candidate.ZipCodeCorrespondence,
                        StreetName = candidate.ProviderAddressCorrespondence,
                    };

                    //Попълваме данните за ЦПО
                    var cPOMainData = new CPOMainData
                    {
                        CPOName = candidate.ProviderName,
                        CompanyName = candidate.ProviderOwner,
                        CompanyId = candidate.PoviderBulstat,
                        CityName = location != null ? location.LocationName : string.Empty,
                    };

                    //Взимаме всички видове такси
                    var ProcedurePriceTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingFee", false);
                    var StartedProcedurePrice = ProcedurePriceTypes.FirstOrDefault(x => x.KeyValueIntCode == "StartProcedureCPO");
                    #endregion
                    //Попълване на нужните данни за съответното приложение и подаване към метода за генериране
                    if (template.ApplicationTypeIntCode == "Application1")
                    {
                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }


                        var application1 = new CPOLicensingApplication1
                        {
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,
                            ExpertCommissionName = commisionName,
                            Deadline = startedProcedureVM.NapooReportDeadline.HasValue ? startedProcedureVM.NapooReportDeadline.Value : DateTime.MinValue,
                            CPOMainData = cPOMainData,
                            ProcedureExternalExperts = externalExperts,
                            ProfessionalDirections = directionVMList,
                        };

                        documentStream = await LicensingService.GenerateApplication_1(application1, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application1_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application2")
                    {
                        int profCount = 0;

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                profCount++;
                            }
                        }

                        var application2 = new CPOLicensingApplication2
                        {
                            OrderNumber = ".......",
                            OrderDate = DateTime.UtcNow.AddDays(-5),

                            ProfessionsCount = profCount,
                            SpecialtiesCount = candidate.CandidateProviderSpecialities.Count(),


                            ExpertCommissionName = commisionName,


                            ExpertCommissionReportTerm = startedProcedureVM.NapooReportDeadline.HasValue ? startedProcedureVM.NapooReportDeadline.Value : DateTime.MinValue,

                            ExternalExpertCommissionReportTerm = startedProcedureVM.ExpertReportDeadline.HasValue ? startedProcedureVM.ExpertReportDeadline.Value : DateTime.MinValue,
                            ProcedureExternalExperts = externalExperts,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        documentStream = await LicensingService.GenerateApplication_2(application2, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application2_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application3")
                    {

                        var professions = candidate.CandidateProviderSpecialities.Select(x => x.Speciality.Profession).Distinct().ToList();
                        var taxes = await this.GetProcedurePriceWithPredicateAsync(new ProcedurePriceVM()
                        { IdTypeApplication = StartedProcedurePrice.IdKeyValue });
                        var tax = "";
                        if (taxes != null || taxes.Count != 0)
                        {
                            if (professions.Count > 20)
                            {
                                tax = taxes.Last().PriceAsStaticStr;
                            }
                            else
                            {
                                tax = taxes.FirstOrDefault(x => x.CountProfessionsFrom <= professions.Count && x.CountProfessionsTo >= professions.Count).PriceAsStaticStr;
                            }
                        }
                        var StringTax = tax != null && tax != "" ? Num2Text.num2txt2(tax) : "";

                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var application3 = new CPOLicensingApplication3
                        {
                            //TODO да се взима таксите
                            IntegerTax = tax,
                            StringTax = StringTax,

                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,
                            ContactPerson = contactData,
                            ProcedureExternalExperts = externalExperts,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application3.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application3.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_3(application3, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application3_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application4")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        //Вземаме точния документ за експерта експерта за който е създаден документа за да го генерираме с неговите данни
                        var expert = externalExperts.FirstOrDefault(x => x.IdExpert == procedureDocumentVM.IdExpert);

                        var application4 = new CPOLicensingApplication4
                        {
                            ContractNumber = ".....",

                            DateOfDraft = DateTime.Now,

                            ContractTerm = startedProcedureVM.ExpertReportDeadline.HasValue ? startedProcedureVM.ExpertReportDeadline.Value : DateTime.MinValue,
                            ExpertDataVM = expert.Expert,

                            CPOMainData = cPOMainData,
                        };

                        var strDirectionList = new List<string>();
                        //var strDirection = string.Join(", ", application4.ExpertDataVM.ExpertProfessionalDirections.Select(x => x.ProfessionalDirectionName));

                        foreach (var item in application4.ExpertDataVM.ExpertProfessionalDirections)
                        {
                            strDirectionList.Add($"{item.ProfessionalDirectionCode}: {item.ProfessionalDirectionName}");
                        }

                        application4.ExpertDataVM.ProfessionalDirectionStr = string.Join(", ", strDirectionList);

                        if (app2 is not null)
                        {
                            application4.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application4.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }


                        documentStream = await LicensingService.GenerateApplication_4(application4, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application4_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application5")
                    {
                        var professions = candidate.CandidateProviderSpecialities.Select(x => x.Speciality.Profession).Distinct().ToList();
                        var taxes = await this.GetProcedurePriceWithPredicateAsync(new ProcedurePriceVM()
                        { IdTypeApplication = StartedProcedurePrice.IdKeyValue });
                        var tax = "";
                        if (taxes != null || taxes.Count != 0)
                        {
                            if (professions.Count > 20)
                            {
                                tax = taxes.Last().PriceAsStaticStr;
                            }
                            else
                            {
                                tax = taxes.FirstOrDefault(x => x.CountProfessionsFrom <= professions.Count && x.CountProfessionsTo >= professions.Count).PriceAsStaticStr;
                            }
                        }
                        var StringTax = tax != null && tax != "" ? Num2Text.num2txt2(tax) : "";

                        var app3 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication3.IdKeyValue);

                        var application5 = new CPOLicensingApplication5
                        {
                            IntegerTax = tax,
                            StringTax = StringTax,


                            ContactPersonData = contactData,
                            TelephoneNumber = chiefExpert != null ? chiefExpert.Expert.Person.Phone : string.Empty,
                            EmailAddress = chiefExpert != null ? chiefExpert.Expert.Person.Email : string.Empty,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                        };

                        if (app3 is not null)
                        {
                            application5.OutputNumber = app3.DS_OFFICIAL_DocNumber != null ? app3.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application5.OutputDate = app3.DS_OFFICIAL_DATE.HasValue ? app3.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_5(application5, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application5_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application6")
                    {
                        var issues = startedProcedureVM.NegativeIssues.Select(i => i.NegativeIssueText).ToList();

                        var application6 = new CPOLicensingApplication6
                        {
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,
                            CPOMainData = cPOMainData,
                            Issues = issues,
                        };

                        documentStream = await LicensingService.GenerateApplication_6(application6, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application6_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application7")
                    {


                        var issues = startedProcedureVM.NegativeIssues.Select(i => i.NegativeIssueText).ToList();

                        var application7 = new CPOLicensingApplication7
                        {
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            TelephoneNumber = chiefExpert != null ? chiefExpert.Expert.Person.Phone : string.Empty,
                            EmailAddress = chiefExpert != null ? chiefExpert.Expert.Person.Email : string.Empty,
                            ContactPersonData = contactData,
                            CPOMainData = cPOMainData,
                            Issues = issues,
                        };

                        documentStream = await LicensingService.GenerateApplication_7(application7, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application7_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application8")
                    {

                        var app6 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication6.IdKeyValue);
                        var app7 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication7.IdKeyValue);


                        var application8 = new CPOLicensingApplication8
                        {
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,
                        };

                        if (app6 is not null)
                        {
                            application8.ReportNumber = app6.DS_OFFICIAL_DocNumber != null ? app6.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application8.ReportInputDate = app6.DS_OFFICIAL_DATE.HasValue ? app6.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        if (app7 is not null)
                        {
                            application8.NotificationLetterNumber = app7.DS_OFFICIAL_DocNumber != null ? app7.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application8.NotificationLetterOutputDate = app7.DS_OFFICIAL_DATE.HasValue ? app7.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_8(application8, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application8_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application9")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var application9 = new CPOLicensingApplication9
                        {

                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application9.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application9.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_9(application9, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application9_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application10")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var application10 = new CPOLicensingApplication10
                        {
                            NotificationLetterNumber = "......",
                            NotificationLetterOutputDate = DateTime.UtcNow.AddDays(-10),
                            DueDate = DateTime.UtcNow.AddDays(-10),


                            ContactPersonData = contactData,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application10.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application10.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_10(application10, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application10_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application11")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        //Вземаме точния документ за експерта за който е създаден документа за да го генерираме с неговите данни
                        var expert = externalExperts.FirstOrDefault(x => x.IdExpert == procedureDocumentVM.IdExpert);

                        var application11 = new CPOLicensingApplication11
                        {
                            //За момента не се прави
                            //MeanScore = ".....",
                            PeriodOfOrderCompletion = new PeriodOfOrderCompletion
                            {
                                FromDate = DateTime.MinValue,
                                ToDate = DateTime.MinValue
                            },


                            ExternalExpertDataVM = expert.Expert,

                            ProfessionalDirections = directionVMList,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application11.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application11.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        var strDirection = string.Join(", ", application11.ExternalExpertDataVM.ExpertProfessionalDirections.Select(x => x.ProfessionalDirectionName));
                        application11.ExternalExpertDataVM.ProfessionalDirectionStr = strDirection;

                        documentStream = await LicensingService.GenerateApplication_11(application11, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application11_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application13")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        string dayOfWeek = string.Empty;
                        if (startedProcedureVM.MeetingDate.HasValue)
                        {
                            dayOfWeek = startedProcedureVM.MeetingDate.Value.DayOfWeek.ToString();
                        }

                        var application13 = new CPOLicensingApplication13
                        {
                            DateOfMeeting = startedProcedureVM.MeetingDate,
                            DayOfWeek = dayOfWeek,
                            MeetingHour = startedProcedureVM.MeetingHour,

                            //Прецедател на експертна комисия и членовете
                            HeadOfExpertCommission = headOfExpertCommissionName,
                            ExpertCommissionMembers = expertCommissionMembersNames,

                            ExpertCommissionName = commisionName,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application13.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application13.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_13(application13, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application13_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application14")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var application14 = new CPOLicensingApplication14
                        {
                            //Ще се върнат от деловодството
                            //ContractNumber = "......",
                            DateOfDraft = DateTime.Now,

                            ExpertCommissionName = commisionName,
                            ExpertDataVM = chiefExpert.Expert,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application14.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application14.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_14(application14, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application14_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application15")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        //Вземаме точния документ за експерта за който е създаден документа за да го генерираме с неговите данни
                        var member = membersfExpertCommission.FirstOrDefault(x => x.IdExpert == procedureDocumentVM.IdExpert);

                        var application15 = new CPOLicensingApplication15
                        {
                            //Ще се върнат от деловодството
                            DateOfDraft = DateTime.Now,
                            //ContractNumber = "......",

                            //Член на експертна комисия
                            ExpertDataVM = member.Expert,

                            ExpertCommissionName = commisionName,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application15.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application15.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_15(application15, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application15_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application16")
                    {

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();
                        Dictionary<string, string> expertCommisionScores = new Dictionary<string, string>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            //Професионални направления с оценки (за оценка слагам точки)
                            expertCommisionScores.Add(direction.First().Speciality.Profession.ProfessionalDirection.Name, ".....");

                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var application16 = new CPOLicensingApplication16
                        {
                            //за сега не се попълват
                            //ProtcolNumber = "......",
                            //TotalScore = "......",
                            //ReviewResults = "...........",
                            Protocoler = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            //DistanceConnectionSoftwareName = "......",

                            DateOfDraft = DateTime.Today,
                            MeetingHour = "",

                            ChiefOfExpertCommission = headOfExpertCommissionName,

                            //Всички членове на експертната комисия и прецедател
                            MeetingAttendance = expertExpertCommissionList.Select(e => e.Expert.Person.FullName).ToList(),

                            //Само професионалните направления с оценки
                            //За сега не се попълват
                            ExpertCommissionScores = expertCommisionScores,

                            ProfessionalDirections = directionVMList,
                            ExpertCommissionName = commisionName,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application16.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application16.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_16(application16, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application16_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application17")
                    {
                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();
                        Dictionary<string, string> expertCommisionScores = new Dictionary<string, string>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            //Професионални направления с оценки (за оценка слагам точки)
                            expertCommisionScores.Add(direction.First().Speciality.Profession.ProfessionalDirection.Name, ".....");

                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);
                        var app16 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication16.IdKeyValue);

                        var application17 = new CPOLicensingApplication17
                        {

                            //Председател
                            ExpertCommissionChairman = headOfExpertCommissionName,

                            //Номер на заявление
                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,

                            //За сега не се попълват
                            //TotalScore = ".....",
                            ExpertCommissionScores = expertCommisionScores,

                            ProfessionalDirections = directionVMList,

                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            ExpertCommissionName = commisionName,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application17.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application17.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app16 is not null)
                        {
                            application17.ProtocolNumber = app16.DS_OFFICIAL_DocNumber != null ? app16.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application17.ProtocolInputDate = app16.DS_OFFICIAL_DATE.HasValue ? app16.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_17(application17, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application17_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application18")
                    {
                        int profCount = 0;

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                profCount++;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);
                        var app16 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication16.IdKeyValue);
                        var app17 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication17.IdKeyValue);


                        var application18 = new CPOLicensingApplication18
                        {
                            //Председател
                            ExpertCommissionChairmanFullName = headOfExpertCommissionName,
                            ExpertCommissionChairmanSirname = headOfExpertCommissionSirName,

                            ProfessionsCount = profCount.ToString(),
                            SpecialitiesCount = candidate.CandidateProviderSpecialities.Count().ToString(),

                            ProfessionalDirections = directionVMList,

                            ExpertCommissionName = commisionName,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application18.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application18.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app16 is not null)
                        {
                            application18.ProtocolNumber = app16.DS_OFFICIAL_DocNumber != null ? app16.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application18.ProtocolInputDate = app16.DS_OFFICIAL_DATE.HasValue ? app16.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app17 is not null)
                        {
                            application18.ReportNumber = app17.DS_OFFICIAL_DocNumber != null ? app17.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application18.ReportInputDate = app17.DS_OFFICIAL_DATE.HasValue ? app17.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_18(application18, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application18_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application19")
                    {
                        int profCount = 0;

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                profCount++;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app17 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication17.IdKeyValue);

                        var application19 = new CPOLicensingApplication19
                        {
                            //Не се попълва към момента
                            //OrderNumber = "....",
                            //OrderInputDate = DateTime.UtcNow.AddDays(-10),

                            //Лиценз номер / Дата на лиценз
                            LicenseNumber = startedProcedureVM.LicenseNumber + "/" + (startedProcedureVM.LicenseDate.HasValue ? startedProcedureVM.LicenseDate.Value.ToString(GlobalConstants.DATE_FORMAT) : string.Empty),

                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,

                            ProfessionsCount = profCount.ToString(),
                            SpecialitiesCount = candidate.CandidateProviderSpecialities.Count().ToString(),
                            ProfessionalDirections = directionVMList,

                            ExpertCommissionName = commisionName,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app17 is not null)
                        {
                            application19.ReportNumber = app17.DS_OFFICIAL_DocNumber != null ? app17.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application19.ReportInputDate = app17.DS_OFFICIAL_DATE.HasValue ? app17.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_19(application19, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application19_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application20")
                    {

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var application20 = new CPOLicensingApplication20
                        {
                            ContactPersonData = contactData,
                            ProfessionalDirections = directionVMList,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        documentStream = await LicensingService.GenerateApplication_20(application20, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application20_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application21")
                    {
                        var app16 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication16.IdKeyValue);
                        var app19 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication19.IdKeyValue);

                        var application21 = new CPOLicensingApplication21
                        {
                            //Не се попълва
                            //OrderNumber = "....",
                            //OrderInputDate = DateTime.UtcNow.AddDays(-10),

                            //Председател
                            ExpertCommissionChairman = headOfExpertCommissionName,
                            //Членовете на комисията
                            MemberList = expertCommissionMembersNames,

                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            ExpertCommissionName = commisionName,
                            CPOMainData = cPOMainData,
                        };

                        if (app16 is not null)
                        {
                            application21.ProtocolNumber = app16.DS_OFFICIAL_DocNumber != null ? app16.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application21.ProtocolInputDate = app16.DS_OFFICIAL_DATE.HasValue ? app16.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app19 is not null)
                        {
                            application21.App19OrderNumber = app19.DS_OFFICIAL_DocNumber != null ? app19.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application21.App19OrderInputDate = app19.DS_OFFICIAL_DATE.HasValue ? app19.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingService.GenerateApplication_21(application21, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application21_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }


                }
                return documentStream;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private async Task<MemoryStream> GenerateDocumentChangeLicenzing(ProcedureDocumentVM procedureDocumentVM)
        {
            try
            {
                #region Зареждаме информация
                //Зареждаме статус активен за темплейта
                var kvActiveStatusTemplate = await dataSourceService.GetKeyValueByIntCodeAsync("StatusTemplate", "Active");
                var filterTemplateVM = new TemplateDocumentVM()
                {
                    IdStatus = kvActiveStatusTemplate.IdKeyValue,
                    IdApplicationType = procedureDocumentVM.IdDocumentType.HasValue ? procedureDocumentVM.IdDocumentType.Value : GlobalConstants.INVALID_ID,
                };
                var listTemplates = await this.templateDocumentService.GetAllTemplateDocumentsAsync(filterTemplateVM);
                var template = listTemplates.FirstOrDefault();

                MemoryStream documentStream = new MemoryStream();

                if (template != null && !string.IsNullOrEmpty(template.TemplatePath) && template.TemplatePath != "#")
                {
                    //Зареждаме всички документи за тази процедура
                    var model = new ProcedureDocumentVM();
                    model.IdStartedProcedure = procedureDocumentVM.IdStartedProcedure;
                    var listDocs = await GetAllProcedureDocumentsAsync(model);

                    //Вземаме кей велю на нужните приложения
                    var kvDocTypeList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");
                    var kvDocTypeApplication1 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application1a");
                    var kvDocTypeApplication2 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application2a");
                    var kvDocTypeApplication3 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application3a");
                    var kvDocTypeApplication6 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application6a");
                    var kvDocTypeApplication7 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application7a");
                    var kvDocTypeApplication9 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application9a");
                    var kvDocTypeApplication16 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application16a");
                    var kvDocTypeApplication17 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application17a");
                    var kvDocTypeApplication19 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "Application19a");

                    //Зареждаме данни, тези ще се използват в повече от 1 шаблон
                    var startedProcedureVM = await GetStartedProcedureByIdForGenerateDocumentAsync(procedureDocumentVM.IdStartedProcedure);
                    var kvCommissionList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertCommission");

                    //Зареждане на главен експер и външните експерти
                    var chiefExpert = startedProcedureVM.ProcedureExternalExperts.FirstOrDefault(pe => pe.IdProfessionalDirection == null);
                    var externalExperts = startedProcedureVM.ProcedureExternalExperts.Where(pe => pe.IdProfessionalDirection != null).ToList();

                    //Зареждане на експертна комисия
                    var expertCommision = startedProcedureVM.ProcedureExpertCommissions.FirstOrDefault();
                    var commisionId = GlobalConstants.INVALID_ID_ZERO;
                    var commisionName = string.Empty;
                    if (expertCommision is not null)
                    {
                        commisionName = kvCommissionList.FirstOrDefault(c => c.IdKeyValue == expertCommision.IdExpertCommission).Name;
                        commisionId = expertCommision.IdExpertCommission;
                    }

                    //Зареждаме членовете на експертна комисия
                    //Зареждаме ролята за председател и активен статус
                    var kvRoleCommissionStrainer = await dataSourceService.GetKeyValueByIntCodeAsync("ExpertRoleCommission", "Chairman");
                    var kvRoleCommissionMember = await dataSourceService.GetKeyValueByIntCodeAsync("ExpertRoleCommission", "Member");
                    var kvStatusActive = await dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");

                    //Създаваме си филтър и вземаме данните
                    ExpertExpertCommissionVM filterExpertCommisionVM = new ExpertExpertCommissionVM()
                    {
                        IdExpertCommission = commisionId,
                        IdStatus = kvStatusActive.IdKeyValue,

                    };
                    var expertExpertCommissionList = await this.expertService.GetAllExpertExpertCommissionsAsync(filterExpertCommisionVM);

                    //Отделяме по роля председателя и членовете на комисията
                    var strainerOfExpertCommission = expertExpertCommissionList.FirstOrDefault(e => e.IdRole == kvRoleCommissionStrainer.IdKeyValue);
                    var membersfExpertCommission = expertExpertCommissionList.Where(e => e.IdRole == kvRoleCommissionMember.IdKeyValue);

                    var headOfExpertCommissionName = string.Empty;
                    var headOfExpertCommissionSirName = string.Empty;
                    if (strainerOfExpertCommission is not null)
                    {
                        headOfExpertCommissionName = strainerOfExpertCommission.Expert.Person.FullName;
                        var split = headOfExpertCommissionName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        headOfExpertCommissionSirName = split.Count() > 0 ? split.Last() : string.Empty;
                    }

                    //Зареждаме само имената на членовете на комисията
                    var expertCommissionMembersNames = membersfExpertCommission.Select(e => e.Expert.Person.FullName).ToList();


                    //Зареждаме данните за кореспонденция
                    var candidate = startedProcedureVM.CandidateProviders.FirstOrDefault();

                    LocationVM location = null;
                    if (candidate.IdLocation != null)
                    {
                        location = await this.locationService.GetLocationByIdAsync(candidate.IdLocation.Value);
                    }

                    LocationVM locationCorrespondence = null;
                    if (candidate.IdLocationCorrespondence != null)
                    {
                        locationCorrespondence = await this.locationService.GetLocationByIdAsync(candidate.IdLocationCorrespondence.Value);
                    }

                    var splitNames = candidate.PersonNameCorrespondence.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    var contactData = new ContactPersonData()
                    {
                        FullName = candidate.PersonNameCorrespondence,
                        Sirname = splitNames.Count() > 0 ? splitNames.Last() : "........",
                        CityName = locationCorrespondence != null ? locationCorrespondence.LocationName : "........",
                        PostCode = candidate.ZipCodeCorrespondence,
                        StreetName = candidate.ProviderAddressCorrespondence,
                    };

                    //Попълваме данните за ЦПО
                    var cPOMainData = new CPOMainData
                    {
                        CPOName = candidate.ProviderName,
                        CompanyName = candidate.ProviderOwner,
                        CompanyId = candidate.PoviderBulstat,
                        CityName = location != null ? location.LocationName : string.Empty,
                    };
                    #endregion
                    //Попълване на нужните данни за съответното приложение и подаване към метода за генериране
                    if (template.ApplicationTypeIntCode == "Application1a")
                    {
                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }


                        var application1 = new CPOLicenseChangeApplication1
                        {
                            LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : ""),
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,
                            ExpertCommissionName = commisionName,
                            ExpertCommissionReportTerm = startedProcedureVM.ExpertReportDeadline.HasValue ? startedProcedureVM.ExpertReportDeadline.Value : DateTime.MinValue,
                            CPOMainData = cPOMainData,
                            ProcedureExternalExperts = externalExperts,
                            ProfessionalDirections = directionVMList,
                        };

                        documentStream = await ChangeLicensingService.GenerateApplication_1(application1, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application1_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application2a")
                    {
                        int profCount = 0;

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                profCount++;
                            }
                        }

                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);

                        var application2 = new CPOLicenseChangeApplication2
                        {
                            OrderNumber = ".......",
                            OrderDate = DateTime.Now,

                            ProfessionsCount = profCount,
                            SpecialtiesCount = candidate.CandidateProviderSpecialities.Count(),


                            ExpertCommissionName = commisionName,
                            //ExpertCommissionReportTerm = data.NapooReportDeadline.HasValue ? data.NapooReportDeadline.Value : DateTime.MinValue,
                            //Стефи каза че трябва друга дата
                            ExpertCommissionReportTerm = startedProcedureVM.NapooReportDeadline.HasValue ? startedProcedureVM.NapooReportDeadline.Value : DateTime.MinValue,

                            ExternalExpertCommissionReportTerm = startedProcedureVM.ExpertReportDeadline.HasValue ? startedProcedureVM.ExpertReportDeadline.Value : DateTime.MinValue,
                            ProcedureExternalExperts = externalExperts,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app1 is not null)
                        {
                            application2.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_2(application2, template);

                    }
                    else if (template.ApplicationTypeIntCode == "Application3a")
                    {
                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var application3 = new CPOLicenseChangeApplication3
                        {
                            StringTax = "......",
                            IntegerTax = "......",

                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,
                            ContactPerson = contactData,
                            ContactPersonSirname = contactData.Sirname,
                            ProcedureExternalExperts = externalExperts,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app1 is not null)
                        {
                            application3.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }
                        if (app2 is not null)
                        {
                            application3.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application3.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_3(application3, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application3_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application4a")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        //Вземаме точния документ за експерта експерта за който е създаден документа за да го генерираме с неговите данни
                        var expert = externalExperts.FirstOrDefault(x => x.IdExpert == procedureDocumentVM.IdExpert);

                        var application4 = new CPOLicenseChangeApplication4
                        {
                            ContractNumber = ".....",

                            DateOfDraft = DateTime.Now,

                            ContractTerm = startedProcedureVM.NapooReportDeadline.HasValue ? startedProcedureVM.NapooReportDeadline.Value : DateTime.MinValue,
                            ExpertDataVM = expert.Expert,

                            CPOMainData = cPOMainData,
                        };


                        var strDirection = string.Join(", ", application4.ExpertDataVM.ExpertProfessionalDirections.Select(x => x.ProfessionalDirectionName));
                        application4.ExpertDataVM.ProfessionalDirectionStr = strDirection;

                        if (app2 is not null)
                        {
                            application4.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application4.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }


                        documentStream = await ChangeLicensingService.GenerateApplication_4(application4, template);

                    }
                    else if (template.ApplicationTypeIntCode == "Application6a")
                    {
                        var issues = startedProcedureVM.NegativeIssues.Select(i => i.NegativeIssueText).ToList();

                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);

                        var application6 = new CPOLicenseChangeApplication6
                        {
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,
                            CPOMainData = cPOMainData,
                            Issues = issues,
                        };

                        if (app1 is not null)
                        {
                            application6.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_6(application6, template);

                    }
                    else if (template.ApplicationTypeIntCode == "Application7a")
                    {
                        var issues = startedProcedureVM.NegativeIssues.Select(i => i.NegativeIssueText).ToList();

                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);

                        var application7 = new CPOLicenseChangeApplication7
                        {

                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            TelephoneNumber = chiefExpert != null ? chiefExpert.Expert.Person.Phone : string.Empty,
                            EmailAddress = chiefExpert != null ? chiefExpert.Expert.Person.Email : string.Empty,
                            ContactPersonData = contactData,
                            CPOMainData = cPOMainData,
                            Issues = issues,
                        };

                        if (app1 is not null)
                        {
                            application7.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_7(application7, template);

                    }
                    else if (template.ApplicationTypeIntCode == "Application8a")
                    {
                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);
                        var app6 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication6.IdKeyValue);
                        var app7 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication7.IdKeyValue);


                        var application8 = new CPOLicenseChangeApplication8
                        {
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,
                        };

                        if (app1 is not null)
                        {
                            application8.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }
                        if (app6 is not null)
                        {
                            application8.ReportNumber = app6.DS_OFFICIAL_DocNumber != null ? app6.DS_OFFICIAL_DocNumber.ToString() : "";
                            //application8.ReportInputDate = app6.DS_OFFICIAL_DATE.HasValue ? app6.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app7 is not null)
                        {
                            application8.NotificationLetterNumber = app7.DS_OFFICIAL_DocNumber != null ? app7.DS_OFFICIAL_DocNumber.ToString() : "";
                            application8.DueDate = app7.DS_OFFICIAL_DATE.HasValue ? app7.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_8(application8, template);

                    }
                    else if (template.ApplicationTypeIntCode == "Application9a")
                    {
                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);

                        var application9 = new CPOLicenseChangeApplication9
                        {
                            OrderNumber = ".......",
                            OrderDate = DateTime.Now,

                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app1 is not null)
                        {
                            application9.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_9(application9, template);

                    }
                    else if (template.ApplicationTypeIntCode == "Application10a")
                    {
                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);
                        var app9 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication9.IdKeyValue);

                        var application10 = new CPOLicenseChangeApplication10
                        {
                            NotificationLetterNumber = "......",
                            DueDate = DateTime.UtcNow.AddDays(-10),

                            ContactPersonData = contactData,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app1 is not null)
                        {
                            application10.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }
                        if (app9 is not null)
                        {
                            application10.OrderNumber = app9.DS_OFFICIAL_DocNumber != null ? app9.DS_OFFICIAL_DocNumber.ToString() : "";
                            application10.OrderDate = app9.DS_OFFICIAL_DATE.HasValue ? app9.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_10(application10, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application10_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application11a")
                    {
                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        //Вземаме точния документ за експерта за който е създаден документа за да го генерираме с неговите данни
                        var expert = externalExperts.FirstOrDefault(x => x.IdExpert == procedureDocumentVM.IdExpert);

                        var application11 = new CPOLicenseChangeApplication11
                        {

                            //За момента не се прави
                            //MeanScore = ".....",
                            PeriodOfOrderCompletion = new PeriodOfOrderCompletion
                            {
                                FromDate = DateTime.MinValue,
                                ToDate = DateTime.MinValue
                            },

                            ExternalExpertDataVM = expert.Expert,

                            ProfessionalDirections = directionVMList,
                            CPOMainData = cPOMainData,
                        };

                        if (app1 is not null)
                        {
                            application11.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }
                        if (app2 is not null)
                        {
                            application11.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application11.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        var strDirection = string.Join(", ", application11.ExternalExpertDataVM.ExpertProfessionalDirections.Select(x => x.ProfessionalDirectionName));
                        application11.ExternalExpertDataVM.ProfessionalDirectionStr = strDirection;

                        documentStream = await ChangeLicensingService.GenerateApplication_11(application11, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application11_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application13a")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        string dayOfWeek = string.Empty;
                        if (startedProcedureVM.MeetingDate.HasValue)
                        {
                            dayOfWeek = startedProcedureVM.MeetingDate.Value.DayOfWeek.ToString();
                        }

                        var application13 = new CPOLicenseChangeApplication13
                        {

                            DateOfMeeting = startedProcedureVM.MeetingDate,
                            DayOfWeek = dayOfWeek,
                            MeetingHour = startedProcedureVM.MeetingHour,

                            //Прецедател на експертна комисия и членовете
                            HeadOfExpertCommission = headOfExpertCommissionName,
                            ExpertCommissionMembers = expertCommissionMembersNames,

                            ExpertCommissionName = commisionName,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application13.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application13.OrderDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_13(application13, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application13_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application14a")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var application14 = new CPOLicenseChangeApplication14
                        {

                            //Ще се върнат от деловодството
                            //ContractNumber = "......",
                            DateOfDraft = DateTime.Now,

                            ExpertCommissionName = commisionName,
                            ExpertDataVM = chiefExpert.Expert,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application14.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application14.OrderDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_14(application14, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application14_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application15a")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        //Вземаме точния документ за експерта за който е създаден документа за да го генерираме с неговите данни
                        var member = membersfExpertCommission.FirstOrDefault(x => x.IdExpert == procedureDocumentVM.IdExpert);

                        var application15 = new CPOLicenseChangeApplication15
                        {
                            //Ще се върнат от деловодството
                            DateOfDraft = DateTime.Now,
                            //ContractNumber = "......",

                            //Член на експертна комисия
                            ExpertDataVM = member.Expert,

                            ExpertCommissionName = commisionName,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application15.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application15.OrderDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_15(application15, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application15_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application16a")
                    {

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();
                        Dictionary<string, string> expertCommisionScores = new Dictionary<string, string>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            //Професионални направления с оценки (за оценка слагам точки)
                            expertCommisionScores.Add(direction.First().Speciality.Profession.ProfessionalDirection.Name, ".....");

                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var application16 = new CPOLicenseChangeApplication16
                        {
                            //за сега не се попълват
                            //ProtcolNumber = "......",
                            //TotalScore = "......",
                            //ReviewResults = "...........",
                            Protocoler = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            //DistanceConnectionSoftwareName = "......",

                            DateOfDraft = startedProcedureVM.MeetingDate,
                            MeetingHour = startedProcedureVM.MeetingHour,

                            ChiefOfExpertCommission = headOfExpertCommissionName,

                            //Всички членове на експертната комисия и прецедател
                            MeetingAttendance = expertExpertCommissionList.Select(e => e.Expert.Person.FullName).ToList(),

                            //Само професионалните направления с оценки
                            //За сега не се попълват
                            ExpertCommissionScores = expertCommisionScores,

                            ProfessionalDirections = directionVMList,
                            ExpertCommissionName = commisionName,
                            CPOMainData = cPOMainData,
                        };

                        if (app1 is not null)
                        {
                            application16.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }
                        if (app2 is not null)
                        {
                            application16.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application16.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_16(application16, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application16_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application17a")
                    {

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();
                        Dictionary<string, string> expertCommisionScores = new Dictionary<string, string>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            expertCommisionScores.Add(direction.First().Speciality.Profession.ProfessionalDirection.Name, ".....");

                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);
                        var app16 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication16.IdKeyValue);

                        var application17 = new CPOLicenseChangeApplication17
                        {
                            //Председател
                            ExpertCommissionChairman = headOfExpertCommissionName,

                            //Номер на заявление
                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,

                            //За сега не се попълват
                            //TotalScore = ".....",
                            ExpertCommissionScores = expertCommisionScores,

                            ProfessionalDirections = directionVMList,

                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            ExpertCommissionName = commisionName,
                            CPOMainData = cPOMainData,
                        };

                        if (app1 is not null)
                        {
                            application17.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }
                        if (app2 is not null)
                        {
                            application17.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application17.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app16 is not null)
                        {
                            application17.ProtocolNumber = app16.DS_OFFICIAL_DocNumber != null ? app16.DS_OFFICIAL_DocNumber.ToString() : "";
                            application17.ProtocolInputDate = app16.DS_OFFICIAL_DATE.HasValue ? app16.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_17(application17, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application17_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application18a")
                    {
                        int profCount = 0;

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                profCount++;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);
                        var app16 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication16.IdKeyValue);
                        var app17 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication17.IdKeyValue);


                        var application18 = new CPOLicenseChangeApplication18
                        {
                            //Председател
                            ExpertCommissionChairmanFullName = headOfExpertCommissionName,
                            ExpertCommissionChairmanSirname = headOfExpertCommissionSirName,

                            ProfessionsCount = profCount.ToString(),
                            SpecialitiesCount = candidate.CandidateProviderSpecialities.Count().ToString(),

                            ProfessionalDirections = directionVMList,

                            ExpertCommissionName = commisionName,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app1 is not null)
                        {
                            application18.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }
                        if (app2 is not null)
                        {
                            application18.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application18.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app16 is not null)
                        {
                            application18.ProtocolNumber = app16.DS_OFFICIAL_DocNumber != null ? app16.DS_OFFICIAL_DocNumber.ToString() : "";
                            application18.ProtocolInputDate = app16.DS_OFFICIAL_DATE.HasValue ? app16.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app17 is not null)
                        {
                            application18.ReportNumber = app17.DS_OFFICIAL_DocNumber != null ? app17.DS_OFFICIAL_DocNumber.ToString() : "";
                            application18.ReportInputDate = app17.DS_OFFICIAL_DATE.HasValue ? app17.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_18(application18, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application18_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application19a")
                    {
                        int profCount = 0;

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                profCount++;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app17 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication17.IdKeyValue);

                        var application19 = new CPOLicenseChangeApplication19
                        {
                            //Не се попълва към момента
                            //OrderNumber = "....",
                            //OrderInputDate = DateTime.UtcNow.AddDays(-10),

                            //Лиценз номер / Дата на лиценз
                            LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : ""),

                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,

                            ProfessionsCount = profCount.ToString(),
                            SpecialitiesCount = candidate.CandidateProviderSpecialities.Count().ToString(),
                            ProfessionalDirections = directionVMList,

                            ExpertCommissionName = commisionName,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app17 is not null)
                        {
                            application19.ReportNumber = app17.DS_OFFICIAL_DocNumber != null ? app17.DS_OFFICIAL_DocNumber.ToString() : "";
                            application19.ReportInputDate = app17.DS_OFFICIAL_DATE.HasValue ? app17.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_19(application19, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application19_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application20a")
                    {

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);

                        var application20 = new CPOLicenseChangeApplication20
                        {
                            ContactPersonData = contactData,
                            ProfessionalDirections = directionVMList,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app1 is not null)
                        {
                            application20.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_20(application20, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application20_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "Application21a")
                    {
                        var app1 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication1.IdKeyValue);
                        var app16 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication16.IdKeyValue);
                        var app19 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication19.IdKeyValue);

                        var application21 = new CPOLicenseChangeApplication21
                        {
                            //Не се попълва
                            //OrderNumber = "....",
                            //OrderInputDate = DateTime.UtcNow.AddDays(-10),

                            //Председател
                            ExpertCommissionChairman = headOfExpertCommissionName,
                            //Членовете на комисията
                            MemberList = expertCommissionMembersNames,

                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            ExpertCommissionName = commisionName,
                            CPOMainData = cPOMainData,
                        };

                        if (app1 is not null)
                        {
                            application21.LicenseNumber = candidate.LicenceNumber + "/" + (candidate.LicenceDate.HasValue ? candidate.LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "");
                        }
                        if (app16 is not null)
                        {
                            application21.ProtocolNumber = app16.DS_OFFICIAL_DocNumber != null ? app16.DS_OFFICIAL_DocNumber.ToString() : "";
                            application21.ProtocolInputDate = app16.DS_OFFICIAL_DATE.HasValue ? app16.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app19 is not null)
                        {
                            application21.App19OrderNumber = app19.DS_OFFICIAL_DocNumber != null ? app19.DS_OFFICIAL_DocNumber.ToString() : "";
                            application21.App19OrderInputDate = app19.DS_OFFICIAL_DATE.HasValue ? app19.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await ChangeLicensingService.GenerateApplication_21(application21, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application21_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                }
                return documentStream;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private async Task<MemoryStream> GenerateDocumentCipo(ProcedureDocumentVM procedureDocumentVM)
        {
            try
            {
                #region Зареждаме информация
                //Зареждаме статус активен за темплейта
                var kvActiveStatusTemplate = await dataSourceService.GetKeyValueByIntCodeAsync("StatusTemplate", "Active");
                var filterTemplateVM = new TemplateDocumentVM()
                {
                    IdStatus = kvActiveStatusTemplate.IdKeyValue,
                    IdApplicationType = procedureDocumentVM.IdDocumentType.HasValue ? procedureDocumentVM.IdDocumentType.Value : GlobalConstants.INVALID_ID,
                };
                var listTemplates = await this.templateDocumentService.GetAllTemplateDocumentsAsync(filterTemplateVM);
                var template = listTemplates.FirstOrDefault();

                MemoryStream documentStream = new MemoryStream();

                if (template != null && !string.IsNullOrEmpty(template.TemplatePath) && template.TemplatePath != "#")
                {
                    //Зареждаме всички документи за тази процедура
                    var model = new ProcedureDocumentVM();
                    model.IdStartedProcedure = procedureDocumentVM.IdStartedProcedure;
                    var listDocs = await GetAllProcedureDocumentsAsync(model);

                    //Вземаме кей велю на нужните приложения 6 и 7
                    var kvDocTypeList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");
                    var kvDocTypeApplication2 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application2");
                    var kvDocTypeApplication3 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application3");
                    var kvDocTypeApplication4 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application4");
                    var kvDocTypeApplication5 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application5");
                    var kvDocTypeApplication6 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application6");
                    var kvDocTypeApplication7 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application7");
                    var kvDocTypeApplication16 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application16");
                    var kvDocTypeApplication17 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application17");
                    var kvDocTypeApplication18 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application18");
                    var kvDocTypeApplication19 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application19");
                    var kvDocTypeApplication9 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application9");
                    var kvDocTypeApplication10 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application10");
                    var kvDocTypeApplication11 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application11");
                    var kvDocTypeApplication13 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application13");
                    var kvDocTypeApplication14 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application14");
                    var kvDocTypeApplication15 = kvDocTypeList.FirstOrDefault(kv => kv.KeyValueIntCode == "CIPO_Application15");

                    //Зареждаме данни, тези ще се използват в повече от 1 шаблон
                    var startedProcedureVM = await GetStartedProcedureByIdForGenerateDocumentAsync(procedureDocumentVM.IdStartedProcedure);
                    var kvCommissionList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ExpertCommission");

                    //Зареждане на главен експер и външните експерти
                    var chiefExpert = startedProcedureVM.ProcedureExternalExperts.First(pe => pe.IdProfessionalDirection == null);
                    var externalExperts = startedProcedureVM.ProcedureExternalExperts.Where(pe => pe.IdProfessionalDirection != null).ToList();

                    //Зареждане на експертна комисия
                    var expertCommision = startedProcedureVM.ProcedureExpertCommissions.FirstOrDefault();
                    var commisionId = GlobalConstants.INVALID_ID_ZERO;
                    var commisionName = string.Empty;
                    if (expertCommision is not null)
                    {
                        commisionName = kvCommissionList.FirstOrDefault(c => c.IdKeyValue == expertCommision.IdExpertCommission).Name;
                        commisionId = expertCommision.IdExpertCommission;
                    }

                    //Зареждаме членовете на експертна комисия
                    //Зареждаме ролята за председател и активен статус
                    var kvRoleCommissionStrainer = await dataSourceService.GetKeyValueByIntCodeAsync("ExpertRoleCommission", "Chairman");
                    var kvRoleCommissionMember = await dataSourceService.GetKeyValueByIntCodeAsync("ExpertRoleCommission", "Member");
                    var kvStatusActive = await dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");

                    //Създаваме си филтър и вземаме данните
                    ExpertExpertCommissionVM filterExpertCommisionVM = new ExpertExpertCommissionVM()
                    {
                        IdExpertCommission = commisionId,
                        IdStatus = kvStatusActive.IdKeyValue,

                    };
                    var expertExpertCommissionList = await this.expertService.GetAllExpertExpertCommissionsAsync(filterExpertCommisionVM);

                    //Отделяме по роля председателя и членовете на комисията
                    var strainerOfExpertCommission = expertExpertCommissionList.FirstOrDefault(e => e.IdRole == kvRoleCommissionStrainer.IdKeyValue);
                    var membersfExpertCommission = expertExpertCommissionList.Where(e => e.IdRole == kvRoleCommissionMember.IdKeyValue);

                    var headOfExpertCommissionName = string.Empty;
                    var headOfExpertCommissionSirName = string.Empty;
                    var headOfCommisionVM = new ExpertVM();
                    if (strainerOfExpertCommission is not null)
                    {
                        headOfExpertCommissionName = strainerOfExpertCommission.Expert.Person.FullName;
                        var split = headOfExpertCommissionName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        headOfExpertCommissionSirName = split.Count() > 0 ? split.Last() : string.Empty;
                        headOfCommisionVM = strainerOfExpertCommission.Expert;
                    }

                    //Зареждаме само имената на членовете на комисията
                    var expertCommissionMembersNames = membersfExpertCommission.Select(e => e.Expert.Person.FullName).ToList();


                    //Зареждаме данните за кореспонденция
                    var candidate = startedProcedureVM.CandidateProviders.FirstOrDefault();

                    LocationVM location = null;
                    if (candidate.IdLocation != null)
                    {
                        location = await this.locationService.GetLocationByIdAsync(candidate.IdLocation.Value);
                    }

                    LocationVM locationCorrespondence = null;
                    if (candidate.IdLocationCorrespondence != null)
                    {
                        locationCorrespondence = await this.locationService.GetLocationByIdAsync(candidate.IdLocationCorrespondence.Value);
                    }

                    var splitNames = candidate.PersonNameCorrespondence.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    var contactData = new ContactPersonData()
                    {
                        FullName = candidate.PersonNameCorrespondence,
                        Sirname = splitNames.Count() > 0 ? splitNames.Last() : "........",
                        CityName = locationCorrespondence != null ? locationCorrespondence.LocationName : "........",
                        PostCode = candidate.ZipCodeCorrespondence,
                        StreetName = candidate.ProviderAddressCorrespondence,
                    };

                    //Попълваме данните за ЦИПО
                    var cPOMainData = new CPOMainData
                    {
                        CPOName = candidate.ProviderName,
                        CompanyName = candidate.ProviderOwner,
                        CompanyId = candidate.PoviderBulstat,
                        CityName = location != null ? location.LocationName : string.Empty,
                    };

                    var ProcedurePriceTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingFee", false);
                    var StartedProcedurePrice = ProcedurePriceTypes.FirstOrDefault(x => x.KeyValueIntCode == "StartProcedureCIPO");
                    #endregion
                    //Попълване на нужните данни за съответното приложение и подаване към метода за генериране
                    if (template.ApplicationTypeIntCode == "CIPO_Application1")
                    {
                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }


                        var application1 = new CPOLicensingApplication1
                        {
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,
                            ExpertCommissionName = commisionName,
                            Deadline = startedProcedureVM.NapooReportDeadline.HasValue ? startedProcedureVM.NapooReportDeadline.Value : DateTime.MinValue,
                            CPOMainData = cPOMainData,
                            ProcedureExternalExperts = externalExperts,
                            ProfessionalDirections = directionVMList,
                        };

                        documentStream = await LicensingCIPOService.GenerateApplication_1(application1, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application1_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application2")
                    {
                        int profCount = 0;

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                profCount++;
                            }
                        }

                        var application2 = new CPOLicensingApplication2
                        {
                            OrderNumber = ".......",
                            OrderDate = DateTime.UtcNow.AddDays(-5),

                            ProfessionsCount = profCount,
                            SpecialtiesCount = candidate.CandidateProviderSpecialities.Count(),


                            ExpertCommissionName = commisionName,
                            
                            
                            ExpertCommissionReportTerm = startedProcedureVM.NapooReportDeadline.HasValue ? startedProcedureVM.NapooReportDeadline.Value : DateTime.MinValue,

                            ExternalExpertCommissionReportTerm = startedProcedureVM.ExpertReportDeadline.HasValue ? startedProcedureVM.ExpertReportDeadline.Value : DateTime.MinValue,
                            ProcedureExternalExperts = externalExperts,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        documentStream = await LicensingCIPOService.GenerateApplication_2(application2, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application2_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application3")
                    {

                        var taxes = await this.GetProcedurePriceWithPredicateAsync(new ProcedurePriceVM()
                        { IdTypeApplication = StartedProcedurePrice.IdKeyValue });
                        var tax = taxes != null ? taxes.FirstOrDefault().PriceAsStaticStr : "";
                        var StringTax = tax != null && tax != "" ? Num2Text.num2txt2(tax) : "";

                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var application3 = new CPOLicensingApplication3
                        {

                            IntegerTax = tax,
                            StringTax = StringTax,

                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,
                            ContactPerson = contactData,
                            ProcedureExternalExperts = externalExperts,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application3.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application3.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_3(application3, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application3_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application4")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        //Вземаме точния документ за експерта експерта за който е създаден документа за да го генерираме с неговите данни
                        var expert = externalExperts.FirstOrDefault(x => x.IdExpert == procedureDocumentVM.IdExpert);

                        var application4 = new CPOLicensingApplication4
                        {
                            ContractNumber = ".....",

                            DateOfDraft = DateTime.Now,

                            ContractTerm = startedProcedureVM.ExpertReportDeadline.HasValue ? startedProcedureVM.ExpertReportDeadline.Value : DateTime.MinValue,
                            ExpertDataVM = expert.Expert,

                            CPOMainData = cPOMainData,
                        };


                        var strDirection = string.Join(", ", application4.ExpertDataVM.ExpertProfessionalDirections.Select(x => x.ProfessionalDirectionName));
                        application4.ExpertDataVM.ProfessionalDirectionStr = strDirection;

                        if (app2 is not null)
                        {
                            application4.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application4.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }


                        documentStream = await LicensingCIPOService.GenerateApplication_4(application4, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application4_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application5")
                    {
                        var taxes = await this.GetProcedurePriceWithPredicateAsync(new ProcedurePriceVM()
                        { IdTypeApplication = StartedProcedurePrice.IdKeyValue });
                        var tax = taxes != null ? taxes.FirstOrDefault().PriceAsStaticStr : "";
                        var StringTax = tax != null && tax != "" ? Num2Text.num2txt2(tax) : "";

                        var app3 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication3.IdKeyValue);

                        var application5 = new CPOLicensingApplication5
                        {
                            IntegerTax = tax,
                            StringTax = StringTax,


                            ContactPersonData = contactData,
                            TelephoneNumber = chiefExpert != null ? chiefExpert.Expert.Person.Phone : string.Empty,
                            EmailAddress = chiefExpert != null ? chiefExpert.Expert.Person.Email : string.Empty,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                        };

                        if (app3 is not null)
                        {
                            application5.OutputNumber = app3.DS_OFFICIAL_DocNumber.ToString();
                            application5.OutputDate = app3.DS_OFFICIAL_DATE.HasValue ? app3.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_5(application5, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application5_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application6")
                    {
                        var issues = startedProcedureVM.NegativeIssues.Select(i => i.NegativeIssueText).ToList();

                        var application6 = new CPOLicensingApplication6
                        {
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,
                            CPOMainData = cPOMainData,
                            Issues = issues,
                        };

                        documentStream = await LicensingCIPOService.GenerateApplication_6(application6, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application6_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application7")
                    {


                        var issues = startedProcedureVM.NegativeIssues.Select(i => i.NegativeIssueText).ToList();

                        var application7 = new CPOLicensingApplication7
                        {
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            TelephoneNumber = chiefExpert != null ? chiefExpert.Expert.Person.Phone : string.Empty,
                            EmailAddress = chiefExpert != null ? chiefExpert.Expert.Person.Email : string.Empty,
                            ContactPersonData = contactData,
                            CPOMainData = cPOMainData,
                            Issues = issues,
                        };

                        documentStream = await LicensingCIPOService.GenerateApplication_7(application7, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application7_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application8")
                    {

                        var app6 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication6.IdKeyValue);
                        var app7 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication7.IdKeyValue);


                        var application8 = new CPOLicensingApplication8
                        {
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,
                        };

                        if (app6 is not null)
                        {
                            application8.ReportNumber = app6.DS_OFFICIAL_DocNumber != null ? app6.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application8.ReportInputDate = app6.DS_OFFICIAL_DATE.HasValue ? app6.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        if (app7 is not null)
                        {
                            application8.NotificationLetterNumber = app7.DS_OFFICIAL_DocNumber != null ? app7.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application8.NotificationLetterOutputDate = app7.DS_OFFICIAL_DATE.HasValue ? app7.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_8(application8, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application8_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application9")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication9.IdKeyValue);

                        var application9 = new CPOLicensingApplication9
                        {

                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application9.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application9.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_9(application9, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application9_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application10")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication10.IdKeyValue);

                        var application10 = new CPOLicensingApplication10
                        {
                            NotificationLetterNumber = "......",
                            NotificationLetterOutputDate = DateTime.UtcNow.AddDays(-10),
                            DueDate = DateTime.UtcNow.AddDays(-10),


                            ContactPersonData = contactData,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application10.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : String.Empty;
                            application10.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_10(application10, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application10_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application11")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        //Вземаме точния документ за експерта за който е създаден документа за да го генерираме с неговите данни
                        var expert = externalExperts.FirstOrDefault(x => x.IdExpert == procedureDocumentVM.IdExpert);

                        var application11 = new CPOLicensingApplication11
                        {
                            //За момента не се прави
                            //MeanScore = ".....",
                            PeriodOfOrderCompletion = new PeriodOfOrderCompletion
                            {
                                FromDate = DateTime.UtcNow,
                                ToDate = DateTime.UtcNow
                            },


                            ExternalExpertDataVM = expert.Expert,

                            ProfessionalDirections = directionVMList,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application11.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application11.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        var strDirection = string.Join(", ", application11.ExternalExpertDataVM.ExpertProfessionalDirections.Select(x => x.ProfessionalDirectionName));
                        application11.ExternalExpertDataVM.ProfessionalDirectionStr = strDirection;

                        documentStream = await LicensingCIPOService.GenerateApplication_11(application11, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application11_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application13")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        string dayOfWeek = string.Empty;
                        if (startedProcedureVM.MeetingDate.HasValue)
                        {
                            dayOfWeek = startedProcedureVM.MeetingDate.Value.DayOfWeek.ToString();
                        }

                        var application13 = new CPOLicensingApplication13
                        {
                            DateOfMeeting = startedProcedureVM.MeetingDate,
                            DayOfWeek = dayOfWeek,
                            MeetingHour = startedProcedureVM.MeetingHour,

                            //Прецедател на експертна комисия и членовете
                            HeadOfExpertCommission = headOfExpertCommissionName,
                            ExpertCommissionMembers = expertCommissionMembersNames,

                            ExpertCommissionName = commisionName,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application13.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application13.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_13(application13, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application13_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application14")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var application14 = new CPOLicensingApplication14
                        {
                            //Ще се върнат от деловодството
                            //ContractNumber = "......",
                            DateOfDraft = DateTime.Now,

                            ExpertCommissionName = commisionName,

                            //Данни за прецедател на експерната комисия
                            ExpertDataVM = headOfCommisionVM,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application14.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application14.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_14(application14, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application14_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application15")
                    {
                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        //Вземаме точния документ за експерта за който е създаден документа за да го генерираме с неговите данни
                        var member = membersfExpertCommission.FirstOrDefault(x => x.IdExpert == procedureDocumentVM.IdExpert);

                        var application15 = new CPOLicensingApplication15
                        {
                            //Ще се върнат от деловодството
                            DateOfDraft = DateTime.Now,
                            //ContractNumber = "......",

                            //Член на експертна комисия
                            ExpertDataVM = member.Expert,

                            ExpertCommissionName = commisionName,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application15.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application15.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_15(application15, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application15_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application16")
                    {

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();
                        Dictionary<string, string> expertCommisionScores = new Dictionary<string, string>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            //Професионални направления с оценки (за оценка слагам точки)
                            expertCommisionScores.Add(direction.First().Speciality.Profession.ProfessionalDirection.Name, ".....");

                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);

                        var application16 = new CPOLicensingApplication16
                        {
                            //за сега не се попълват
                            //ProtcolNumber = "......",
                            //TotalScore = "......",
                            //ReviewResults = "...........",
                            Protocoler = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            //DistanceConnectionSoftwareName = "......",

                            DateOfDraft = startedProcedureVM.MeetingDate,
                            MeetingHour = startedProcedureVM.MeetingHour,

                            ChiefOfExpertCommission = headOfExpertCommissionName,

                            //Всички членове на експертната комисия и прецедател
                            MeetingAttendance = expertExpertCommissionList.Select(e => e.Expert.Person.FullName).ToList(),

                            //Само професионалните направления с оценки
                            //За сега не се попълват
                            ExpertCommissionScores = expertCommisionScores,

                            ProfessionalDirections = directionVMList,
                            ExpertCommissionName = commisionName,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application16.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application16.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_16(application16, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application16_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application17")
                    {
                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);
                        var app16 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication16.IdKeyValue);

                        var application17 = new CPOLicensingApplication17
                        {

                            //Председател
                            ExpertCommissionChairman = headOfExpertCommissionName,

                            //Номер на заявление
                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,

                            //За сега не се попълват
                            //TotalScore = ".....",
                            //ExpertCommissionScores = expertCommisionScores

                            ProfessionalDirections = directionVMList,

                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            ExpertCommissionName = commisionName,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application17.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application17.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app16 is not null)
                        {
                            application17.ProtocolNumber = app16.DS_OFFICIAL_DocNumber != null ? app16.DS_OFFICIAL_DocNumber.ToString() : "";
                            application17.ProtocolInputDate = app16.DS_OFFICIAL_DATE.HasValue ? app16.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_17(application17, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application17_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application18")
                    {
                        int profCount = 0;

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                profCount++;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app2 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication2.IdKeyValue);
                        var app16 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication16.IdKeyValue);
                        var app17 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication17.IdKeyValue);


                        var application18 = new CPOLicensingApplication18
                        {
                            //Председател
                            ExpertCommissionChairmanFullName = headOfExpertCommissionName,
                            ExpertCommissionChairmanSirname = headOfExpertCommissionSirName,

                            ProfessionsCount = profCount.ToString(),
                            SpecialitiesCount = candidate.CandidateProviderSpecialities.Count().ToString(),

                            ProfessionalDirections = directionVMList,

                            ExpertCommissionName = commisionName,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app2 is not null)
                        {
                            application18.OrderNumber = app2.DS_OFFICIAL_DocNumber != null ? app2.DS_OFFICIAL_DocNumber.ToString() : "";
                            application18.OrderInputDate = app2.DS_OFFICIAL_DATE.HasValue ? app2.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app16 is not null)
                        {
                            application18.ProtocolNumber = app16.DS_OFFICIAL_DocNumber != null ? app16.DS_OFFICIAL_DocNumber.ToString() : "";
                            application18.ProtocolInputDate = app16.DS_OFFICIAL_DATE.HasValue ? app16.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app17 is not null)
                        {
                            application18.ReportNumber = app17.DS_OFFICIAL_DocNumber != null ? app17.DS_OFFICIAL_DocNumber.ToString() : "";
                            application18.ReportInputDate = app17.DS_OFFICIAL_DATE.HasValue ? app17.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_18(application18, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application18_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application19")
                    {
                        int profCount = 0;

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                profCount++;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var app17 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication17.IdKeyValue);

                        var application19 = new CPOLicensingApplication19
                        {
                            //Не се попълва към момента
                            //OrderNumber = "....",
                            //OrderInputDate = DateTime.UtcNow.AddDays(-10),

                            //Лиценз номер / Дата на лиценз
                            LicenseNumber = startedProcedureVM.LicenseNumber + "/" + startedProcedureVM.LicenseDate.Value.ToString(GlobalConstants.DATE_FORMAT),

                            ApplicationNumber = candidate.ApplicationNumber,
                            ApplicationInputDate = candidate.ApplicationDate.HasValue ? candidate.ApplicationDate.Value : DateTime.MinValue,

                            ProfessionsCount = profCount.ToString(),
                            SpecialitiesCount = candidate.CandidateProviderSpecialities.Count().ToString(),
                            ProfessionalDirections = directionVMList,

                            ExpertCommissionName = commisionName,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        if (app17 is not null)
                        {
                            application19.ReportNumber = app17.DS_OFFICIAL_DocNumber != null ? app17.DS_OFFICIAL_DocNumber.ToString() : "";
                            application19.ReportInputDate = app17.DS_OFFICIAL_DATE.HasValue ? app17.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_19(application19, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application19_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application20")
                    {

                        var kvVQSList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                        var directionVMList = new List<ProfessionalDirectionVM>();

                        var groupedDirection = candidate.CandidateProviderSpecialities.GroupBy(s => s.Speciality.Profession.IdProfessionalDirection);
                        foreach (var direction in groupedDirection)
                        {
                            var directionVM = new ProfessionalDirectionVM();
                            directionVM.Name = direction.First().Speciality.Profession.ProfessionalDirection.Name;
                            directionVM.Code = direction.First().Speciality.Profession.ProfessionalDirection.Code;

                            var groupedProfession = direction.GroupBy(x => x.Speciality.IdProfession);
                            foreach (var profession in groupedProfession)
                            {
                                var professionVM = new ProfessionVM();
                                professionVM.Name = profession.First().Speciality.Profession.Name;
                                professionVM.Code = profession.First().Speciality.Profession.Code;

                                foreach (var speciality in profession)
                                {
                                    var specialityVM = new SpecialityVM();
                                    specialityVM.Name = speciality.Speciality.Name;
                                    specialityVM.Code = speciality.Speciality.Code;
                                    specialityVM.VQS_Name = kvVQSList.FirstOrDefault(kv => kv.IdKeyValue == speciality.Speciality.IdVQS)?.Name ?? "....";


                                    professionVM.Specialities.Add(specialityVM);
                                }

                                directionVM.Professions.Add(professionVM);
                            }

                            directionVMList.Add(directionVM);
                        }

                        var application20 = new CPOLicensingApplication20
                        {
                            ContactPersonData = contactData,
                            ProfessionalDirections = directionVMList,
                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            CPOMainData = cPOMainData,
                        };

                        documentStream = await LicensingCIPOService.GenerateApplication_20(application20, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application20_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }
                    else if (template.ApplicationTypeIntCode == "CIPO_Application21")
                    {
                        var app16 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication16.IdKeyValue);
                        var app19 = listDocs.FirstOrDefault(d => d.IdDocumentType == kvDocTypeApplication19.IdKeyValue);

                        var application21 = new CPOLicensingApplication21
                        {
                            //Не се попълва
                            //OrderNumber = "....",
                            //OrderInputDate = DateTime.UtcNow.AddDays(-10),

                            //Председател
                            ExpertCommissionChairman = headOfExpertCommissionName,
                            //Членовете на комисията
                            MemberList = expertCommissionMembersNames,

                            ChiefExpert = chiefExpert != null ? chiefExpert.Expert.Person.FullName : string.Empty,
                            ExpertCommissionName = commisionName,
                            CPOMainData = cPOMainData,
                        };

                        if (app16 is not null)
                        {
                            application21.ProtocolNumber = app16.DS_OFFICIAL_DocNumber != null ? app16.DS_OFFICIAL_DocNumber.ToString() : "";
                            application21.ProtocolInputDate = app16.DS_OFFICIAL_DATE.HasValue ? app16.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }
                        if (app19 is not null)
                        {
                            application21.App19OrderNumber = app19.DS_OFFICIAL_DocNumber != null ? app19.DS_OFFICIAL_DocNumber.ToString() : "";
                            application21.App19OrderInputDate = app19.DS_OFFICIAL_DATE.HasValue ? app19.DS_OFFICIAL_DATE.Value : DateTime.MinValue;
                        }

                        documentStream = await LicensingCIPOService.GenerateApplication_21(application21, template);
                        //await FileUtils.SaveAs(JsRuntime, $"Application21_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.docx", documentStream.ToArray());
                    }


                }
                return documentStream;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<bool> DeleteProcedureDocument(ProcedureDocumentVM procedureDocumentVM)
        {
            try
            { 
            var procedure = procedureDocumentVM.To<ProcedureDocument>();
            this.repository.HardDelete(procedure);
            this.repository.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                logger.LogError(ex.Message);
                logger.LogError(ex.InnerException?.Message);
                logger.LogError(ex.StackTrace);
            }
        }

        #endregion
    }
    #region NumToTxt
    public class Num2Text
    {
        public static string[] warray2 = new string[] { "", "ед", "дв", "три", "четири", "пет", "шест", "седем", "осем", "девет" };

        string[] warray = new string[10];

        string edin;
        string edna;
        string dwa;
        string dwe;
        string sto;
        string dwesta;
        string trista;
        string stotin;
        string deset;
        string mil;
        string mila;
        string hil;
        string hili;
        string lw;
        string st;
        string i;
        string jn;
        string na;

        int C1;

        private static string w_edin = "";
        private static string w_edna = "";
        private static string w_dwa = "";
        private static string w_dwe = "";
        private static string w_sto = "сто";
        private static string w_dwesta = "двеста";
        private static string w_trista = "триста";
        private static string w_stotin = "стотин";
        private static string w_deset = "десет";
        private static string w_mil = "милион";
        private static string w_mila = " милиона";
        private static string w_hil = "хиляда";
        private static string w_hili = " хиляди";
        private static string w_lw = " лв";
        private static string w_st = " ст.";
        private static string w_i = " и ";
        private static string w_jn = "ин";
        private static string w_na = "на";

        private void myInit()
        {
            warray[0] = "";
            warray[1] = "ед";
            warray[2] = "дв";
            warray[3] = "три";
            warray[4] = "четири";
            warray[5] = "пет";
            warray[6] = "шест";
            warray[7] = "седем";
            warray[8] = "осем";
            warray[9] = "девет";


            jn = "ин";
            na = "на";
            sto = "сто";
            dwesta = "двеста";
            trista = "триста";
            stotin = "стотин";
            deset = "десет";
            mil = "милион";
            mila = " милиона";
            hil = "хиляда";
            hili = " хиляди";
            lw = " лв";
            st = " ст.";
            i = " и ";
        }

        private void myInit2()
        {
            jn = "ин";
            na = "на";
            sto = "сто";
            dwesta = "двеста";
            trista = "триста";
            stotin = "стотин";
            deset = "десет";
            mil = "милион";
            mila = " милиона";
            hil = "хиляда";
            hili = " хиляди";
            lw = " лв";
            st = " ст.";
            i = " и ";
        }

        private string Conv(string sstr)
        {
            string rstr = string.Empty, astrc = string.Empty, bstrc = string.Empty, cstrc = string.Empty;
            int apos, bpos, cpos;

            if (vb.Strings.Len(sstr) == 3)
            {
                apos = vb.Strings.Asc(vb.Strings.Mid(sstr, 3, 1)) - vb.Strings.Asc("0");
                bpos = vb.Strings.Asc(vb.Strings.Mid(sstr, 2, 1)) - vb.Strings.Asc("0");
                cpos = vb.Strings.Asc(vb.Strings.Mid(sstr, 1, 1)) - vb.Strings.Asc("0");
            }
            else
            {
                apos = vb.Strings.Asc(vb.Strings.Mid(sstr, 2, 1)) - vb.Strings.Asc("0");
                bpos = vb.Strings.Asc(vb.Strings.Mid(sstr, 1, 1)) - vb.Strings.Asc("0");
                cpos = 0;
            }

            if (apos == 1)
            {
                if (vb.Strings.Len(sstr) == 3)
                {
                    if (C1 == 1)
                    {
                        if (sstr == "011" | sstr == "111")
                        {
                            astrc = warray[1] + jn;
                        }
                        else
                        {
                            astrc = warray[1] + na;
                        }
                    }
                    else
                    {
                        astrc = warray[1] + jn;
                    }
                }
                else
                {
                    astrc = warray[1] + na;
                }
            }
            else
            {
                if (apos == 2)
                {
                    if (vb.Strings.Len(sstr) == 3)
                    {
                        if ((C1 == 1) & (bpos != 1))
                        {
                            astrc = warray[2] + "е";
                        }
                        else
                        {
                            astrc = warray[2] + "а";
                        }
                    }
                    else
                    {
                        astrc = warray[2] + "e";
                    }
                }
                else
                {
                    if ((apos >= 3) & (apos <= 9))
                        astrc = warray[apos];
                }
            }

            if (bpos == 1)
            {
                if (apos == 1)
                {
                    bstrc = astrc + "а" + deset;
                    astrc = "";
                }
                else
                {
                    if (apos == 0)
                    {
                        bstrc = deset;
                        astrc = "";
                    }
                    else
                    {
                        if ((apos >= 2) & (apos <= 9))
                            bstrc = astrc + na + deset;
                        astrc = "";
                    }
                }
            }
            else
            {
                if (bpos == 2)
                {
                    bstrc = warray[bpos] + "а" + deset;
                }
                else
                {
                    if ((bpos >= 3) & (bpos <= 9))
                        bstrc = warray[bpos] + deset;
                }
            }

            switch (cpos)
            {
                case 1:
                    cstrc = sto;
                    break;
                case 2:
                    cstrc = dwesta;
                    break;
                case 3:
                    cstrc = trista;
                    break;
                default:
                    if ((cpos >= 4) & (cpos <= 9))
                        cstrc = warray[cpos] + stotin;
                    break;

            }

            rstr = astrc;
            if (vb.Strings.Len(cstrc) > 0)
            {
                if (vb.Strings.Len(astrc) > 0)
                {
                    if (vb.Strings.Len(bstrc) > 0)
                    {
                        rstr = cstrc + " " + bstrc + i + rstr;
                    }
                    else
                    {
                        rstr = cstrc + i + rstr;
                    }
                }
                else
                {
                    if (vb.Strings.Len(bstrc) > 0)
                    {
                        rstr = cstrc + i + bstrc;
                    }
                    else
                    {
                        rstr = cstrc;
                    }
                }
            }
            else
            {
                if (vb.Strings.Len(bstrc) > 0)
                {
                    if (vb.Strings.Len(astrc) > 0)
                    {
                        rstr = bstrc + i + rstr;
                    }
                    else
                    {
                        rstr = bstrc;
                    }
                }
            }

            return rstr;

        }

        private static string Conv2(string sstr, int _C2)
        {
            string rstr = string.Empty, astrc = string.Empty, bstrc = string.Empty, cstrc = string.Empty;
            int apos, bpos, cpos;

            if (vb.Strings.Len(sstr) == 3)
            {
                apos = vb.Strings.Asc(vb.Strings.Mid(sstr, 3, 1)) - vb.Strings.Asc("0");
                bpos = vb.Strings.Asc(vb.Strings.Mid(sstr, 2, 1)) - vb.Strings.Asc("0");
                cpos = vb.Strings.Asc(vb.Strings.Mid(sstr, 1, 1)) - vb.Strings.Asc("0");
            }
            else
            {
                apos = vb.Strings.Asc(vb.Strings.Mid(sstr, 2, 1)) - vb.Strings.Asc("0");
                bpos = vb.Strings.Asc(vb.Strings.Mid(sstr, 1, 1)) - vb.Strings.Asc("0");
                cpos = 0;
            }

            if (apos == 1)
            {
                if (vb.Strings.Len(sstr) == 3)
                {
                    if (_C2 == 1)
                    {
                        if (sstr == "011" | sstr == "111")
                        {
                            astrc = warray2[1] + w_jn;
                        }
                        else
                        {
                            astrc = warray2[1] + w_na;
                        }
                    }
                    else
                    {
                        astrc = warray2[1] + w_jn;
                    }
                }
                else
                {
                    astrc = warray2[1] + w_na;
                }
            }
            else
            {
                if (apos == 2)
                {
                    if (vb.Strings.Len(sstr) == 3)
                    {
                        if ((_C2 == 1) & (bpos != 1))
                        {
                            astrc = warray2[2] + "е";
                        }
                        else
                        {
                            astrc = warray2[2] + "а";
                        }
                    }
                    else
                    {
                        astrc = warray2[2] + "e";
                    }
                }
                else
                {
                    if ((apos >= 3) & (apos <= 9))
                        astrc = warray2[apos];
                }
            }

            if (bpos == 1)
            {
                if (apos == 1)
                {
                    bstrc = astrc + "а" + w_deset;
                    astrc = "";
                }
                else
                {
                    if (apos == 0)
                    {
                        bstrc = w_deset;
                        astrc = "";
                    }
                    else
                    {
                        if ((apos >= 2) & (apos <= 9))
                            bstrc = astrc + w_na + w_deset;
                        astrc = "";
                    }
                }
            }
            else
            {
                if (bpos == 2)
                {
                    bstrc = warray2[bpos] + "а" + w_deset;
                }
                else
                {
                    if ((bpos >= 3) & (bpos <= 9))
                        bstrc = warray2[bpos] + w_deset;
                }
            }

            switch (cpos)
            {
                case 1:
                    cstrc = w_sto;
                    break;
                case 2:
                    cstrc = w_dwesta;
                    break;
                case 3:
                    cstrc = w_trista;
                    break;
                default:
                    if ((cpos >= 4) & (cpos <= 9))
                        cstrc = warray2[cpos] + w_stotin;
                    break;

            }

            rstr = astrc;
            if (vb.Strings.Len(cstrc) > 0)
            {
                if (vb.Strings.Len(astrc) > 0)
                {
                    if (vb.Strings.Len(bstrc) > 0)
                    {
                        rstr = cstrc + " " + bstrc + w_i + rstr;
                    }
                    else
                    {
                        rstr = cstrc + w_i + rstr;
                    }
                }
                else
                {
                    if (vb.Strings.Len(bstrc) > 0)
                    {
                        rstr = cstrc + w_i + bstrc;
                    }
                    else
                    {
                        rstr = cstrc;
                    }
                }
            }
            else
            {
                if (vb.Strings.Len(bstrc) > 0)
                {
                    if (vb.Strings.Len(astrc) > 0)
                    {
                        rstr = bstrc + w_i + rstr;
                    }
                    else
                    {
                        rstr = bstrc;
                    }
                }
            }

            return rstr;
        }

        public string numThreeDigitTxt(string instring)
        {
            int LastDelimiter;
            string wstr;
            string fstr = string.Empty;
            string ostring;
            string cstrc;
            string tstr;

            myInit();

            wstr = instring;
            LastDelimiter = vb.Strings.InStr(1, wstr, ",");

            if (LastDelimiter > 0)
            {
                fstr = vb.Strings.Mid(wstr, LastDelimiter + 1, 3);
                wstr = vb.Strings.Mid(wstr, 1, LastDelimiter - 1);
            }

            if (vb.Strings.Len(fstr) < 2)
            {
                fstr = fstr + vb.Strings.Mid("00", 1, 2 - vb.Strings.Len(fstr));
            }

            ostring = "";
            C1 = 0;

            while (wstr != "")
            {
                if (vb.Strings.Len(wstr) < 3)
                {
                    wstr = vb.Strings.Mid("000", 1, 3 - vb.Strings.Len(wstr)) + wstr;
                }
                tstr = vb.Strings.Mid(wstr, vb.Strings.Len(wstr) - 2, 3);
                cstrc = Conv(tstr);

                switch (C1)
                {
                    case 1:
                        if ((tstr == "001"))
                        {
                            cstrc = hil;
                        }

                        else if (cstrc != "")
                        {
                            cstrc = cstrc + hili;
                        }
                        break;
                    case 2:
                        if ((tstr == "001"))
                        {
                            cstrc = warray[1] + jn + " " + mil;
                        }
                        else if (cstrc != "")
                        {
                            cstrc = cstrc + mila;
                        }
                        break;
                }

                if (vb.Strings.Len(wstr) >= 3)
                {
                    wstr = vb.Strings.Mid(wstr, 1, vb.Strings.Len(wstr) - 3);
                }
                else
                {
                    wstr = "";
                }

                if (cstrc != "")
                {
                    if ((C1 == 0))
                    {
                        if ((wstr != "") & (vb.Strings.InStr(1, cstrc, i) == 0))
                        {
                            ostring = i + cstrc;
                        }
                        else
                        {
                            ostring = cstrc;
                        }
                    }
                    else
                    {
                        ostring = cstrc + " " + ostring;
                    }
                }

                //Try
                //    If Len(wstr.Trim) > 0 Then
                //        If CInt(wstr.Trim) <> 11 Then
                //            C1 = C1 + 1
                //        End If
                //    End If
                //Catch ex As Exception
                //    C1 = C1 + 1
                //End Try

                C1 = C1 + 1;

            }

            try
            {
                if (Int32.Parse(fstr) == 0)
                {
                    fstr = "0";
                }
            }
            catch (Exception ex)
            {
            }

            string res = string.Empty;

            if (vb.Strings.Len(fstr) > 0)
            {
                if (fstr.Length == 3)
                {
                    fstr = fstr.Insert(2, ".");
                    if (fstr.First() == '0')
                    {
                        fstr = fstr.Remove(0, 1);
                    }
                }
                //    cstrc = Conv(fstr)
                if (ostring != "")
                {
                    res = ostring + lw + " и " + fstr + st;
                }
                else
                {
                    res = fstr + st;
                }
            }
            else
            {
                res = ostring + lw;
            }

            if (res.StartsWith("един лв"))
            {
                res = res.Replace("лв", "лев");
            }
            else
            {
                res = res.Replace("лв", "лева");
            }

            return res;
        }

        public string num2txt(string instring)
        {
            int LastDelimiter = -1;
            string wstr = string.Empty;
            string fstr = string.Empty;
            string ostring = string.Empty;
            string cstrc = string.Empty;
            string tstr = string.Empty;

            myInit();

            wstr = instring;
            LastDelimiter = vb.Strings.InStr(1, wstr, ",");

            if (LastDelimiter > 0)
            {
                fstr = vb.Strings.Mid(wstr, LastDelimiter + 1, 2);
                wstr = vb.Strings.Mid(wstr, 1, LastDelimiter - 1);
            }

            if (vb.Strings.Len(fstr) < 2)
            {
                fstr = fstr + vb.Strings.Mid("00", 1, 2 - vb.Strings.Len(fstr));
            }

            ostring = "";
            C1 = 0;

            while (wstr != "")
            {
                if (vb.Strings.Len(wstr) < 3)
                {
                    wstr = vb.Strings.Mid("000", 1, 3 - vb.Strings.Len(wstr)) + wstr;
                }
                tstr = vb.Strings.Mid(wstr, vb.Strings.Len(wstr) - 2, 3);
                cstrc = Conv(tstr);

                switch (C1)
                {
                    case 1:
                        if ((tstr == "001"))
                        {
                            cstrc = hil;
                        }

                        else if (cstrc != "")
                        {
                            cstrc = cstrc + hili;
                        }
                        break;
                    case 2:
                        if ((tstr == "001"))
                        {
                            cstrc = warray[1] + jn + " " + mil;
                        }
                        else if (cstrc != "")
                        {
                            cstrc = cstrc + mila;
                        }
                        break;
                }

                if (vb.Strings.Len(wstr) >= 3)
                {
                    wstr = vb.Strings.Mid(wstr, 1, vb.Strings.Len(wstr) - 3);
                }
                else
                {
                    wstr = "";
                }

                if (cstrc != "")
                {
                    if ((C1 == 0))
                    {
                        if ((wstr != "") & (vb.Strings.InStr(1, cstrc, i) == 0))
                        {
                            ostring = i + cstrc;
                        }
                        else
                        {
                            ostring = cstrc;
                        }
                    }
                    else
                    {
                        ostring = cstrc + " " + ostring;
                    }
                }

                //Try
                //    If Len(wstr.Trim) > 0 Then
                //        If CInt(wstr.Trim) <> 11 Then
                //            C1 = C1 + 1
                //        End If
                //    End If
                //Catch ex As Exception
                //    C1 = C1 + 1
                //End Try

                C1 = C1 + 1;
            }

            try
            {
                if (Int32.Parse(fstr) == 0)
                {
                    fstr = "0";
                }
            }
            catch (Exception ex)
            {
            }

            string res = string.Empty;

            if (vb.Strings.Len(fstr) > 0)
            {
                //    cstrc = Conv(fstr)
                if (ostring != "")
                {
                    res = ostring + lw + " и " + fstr + st;
                }
                else
                {
                    res = fstr + st;
                }
            }
            else
            {
                res = ostring + lw;
            }

            if (res.StartsWith("един лв"))
            {
                res = res.Replace("лв", "лев");
            }
            else
            {
                res = res.Replace("лв", "лева");
            }

            return res;
        }

        public static string num2txt2(string instring)
        {
            int LastDelimiter = -1;
            string wstr = string.Empty;
            string fstr = string.Empty;
            string ostring = string.Empty;
            string cstrc = string.Empty;
            string tstr = string.Empty;

            //myInit2();

            int C2 = 0;

            wstr = instring;
            LastDelimiter = vb.Strings.InStr(1, wstr, ",");

            if (LastDelimiter > 0)
            {
                fstr = vb.Strings.Mid(wstr, LastDelimiter + 1, 2);
                wstr = vb.Strings.Mid(wstr, 1, LastDelimiter - 1);
            }

            if (vb.Strings.Len(fstr) < 2)
            {
                fstr = fstr + vb.Strings.Mid("00", 1, 2 - vb.Strings.Len(fstr));
            }

            ostring = "";
            C2 = 0;

            while (wstr != "")
            {
                if (vb.Strings.Len(wstr) < 3)
                {
                    wstr = vb.Strings.Mid("000", 1, 3 - vb.Strings.Len(wstr)) + wstr;
                }
                tstr = vb.Strings.Mid(wstr, vb.Strings.Len(wstr) - 2, 3);
                cstrc = Conv2(tstr, C2);

                switch (C2)
                {
                    case 1:
                        if ((tstr == "001"))
                        {
                            cstrc = w_hil;
                        }

                        else if (cstrc != "")
                        {
                            cstrc = cstrc + w_hili;
                        }
                        break;
                    case 2:
                        if ((tstr == "001"))
                        {
                            cstrc = warray2[1] + w_jn + " " + w_mil;
                        }
                        else if (cstrc != "")
                        {
                            cstrc = cstrc + w_mila;
                        }
                        break;
                }

                if (vb.Strings.Len(wstr) >= 3)
                {
                    wstr = vb.Strings.Mid(wstr, 1, vb.Strings.Len(wstr) - 3);
                }
                else
                {
                    wstr = "";
                }

                if (cstrc != "")
                {
                    if ((C2 == 0))
                    {
                        if ((wstr != "") & (vb.Strings.InStr(1, cstrc, w_i) == 0))
                        {
                            ostring = w_i + cstrc;
                        }
                        else
                        {
                            ostring = cstrc;
                        }
                    }
                    else
                    {
                        ostring = cstrc + " " + ostring;
                    }
                }

                //Try
                //    If Len(wstr.Trim) > 0 Then
                //        If CInt(wstr.Trim) <> 11 Then
                //            C1 = C1 + 1
                //        End If
                //    End If
                //Catch ex As Exception
                //    C1 = C1 + 1
                //End Try

                C2 = C2 + 1;
            }

            try
            {
                if (Int32.Parse(fstr) == 0)
                {
                    fstr = "0";
                }
            }
            catch (Exception ex)
            {
            }

            string res = string.Empty;

            if (vb.Strings.Len(fstr) > 0)
            {
                //    cstrc = Conv(fstr)
                if (ostring != "")
                {
                    res = ostring + w_lw + " и " + fstr + w_st;
                }
                else
                {
                    res = fstr + w_st;
                }
            }
            else
            {
                res = ostring + w_lw;
            }

            if (res.StartsWith("един лв"))
            {
                res = res.Replace("лв", "лев");
            }
            else
            {
                res = res.Replace("лв", "лева");
            }

            return res;
        }
    }
    #endregion
}
