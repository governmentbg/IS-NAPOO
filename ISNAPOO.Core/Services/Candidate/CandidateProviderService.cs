using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.EGovPayment;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.ProviderData;
using Data.Models.Data.SPPOO;
using Data.Models.Data.Training;
using Data.Models.Migrations;
using DocuWorkService;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Mailing;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.Core.ViewModels.Register;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using CandidateProviderPremisesChecking = Data.Models.Data.Candidate.CandidateProviderPremisesChecking;

namespace ISNAPOO.Core.Services.Candidate
{
    public class CandidateProviderService : BaseService, ICandidateProviderService
    {
        private readonly IRepository repository;
        private readonly ILogger<CandidateProviderService> _logger;
        private readonly IKeyValueService keyValueService;
        private readonly ISettingService settingService;
        private readonly IDataSourceService dataSourceService;
        private readonly IApplicationUserService applicationUserService;
        private readonly IMailService MailService;
        private readonly INotificationService NotificationService;
        private readonly ILocationService LocationService;
        private readonly IProfessionService ProfessionService;
        private readonly IPersonService PersonService;
        private readonly IDOCService DOCService;
        private readonly IERUSpecialityService eRUSpecialityService;
        private readonly IDocuService docuService;
        private readonly UserManager<ApplicationUser> userManager;

        #region Curriculum Print fields
        private int counter = 1;
        #endregion

        #region KV Application type
        IEnumerable<KeyValueVM> kvApplicationTypesSource;
        KeyValueVM kvCPO;
        KeyValueVM kvCIPO;
        #endregion

        #region KV Application status
        IEnumerable<KeyValueVM> kvApplicationStatusSource;
        KeyValueVM kvProcedureCompleted;
        #endregion

        //private readonly IUserEmailStore<ApplicationUser> _emailStore;
        //private readonly IEmailSender _emailSender;

        public CandidateProviderService(IRepository repository, ILogger<CandidateProviderService> logger, IKeyValueService keyValueService,
            ISettingService settingService, IDataSourceService dataSourceService, IApplicationUserService applicationUserService, IMailService MailService,
            AuthenticationStateProvider authenticationStateProvider, INotificationService NotificationService, ILocationService locationService,
            IProfessionService professionService, IPersonService PersonService, IDOCService docService,
            IERUSpecialityService eRUSpecialityService, IDocuService docuService, UserManager<ApplicationUser> userManager) : base(repository, authenticationStateProvider)
        {
            this.repository = repository;
            this._logger = logger;
            this.keyValueService = keyValueService;
            this.settingService = settingService;
            this.dataSourceService = dataSourceService;
            this.applicationUserService = applicationUserService;
            this.MailService = MailService;
            this.NotificationService = NotificationService;
            this.LocationService = locationService;
            this.ProfessionService = professionService;
            this.PersonService = PersonService;
            this.DOCService = docService;
            this.eRUSpecialityService = eRUSpecialityService;
            this.LoadKVSources();
            this.docuService = docuService;
            this.userManager = userManager;
        }

        // зарежда данни за номенклатури
        private void LoadKVSources()
        {
            this.kvApplicationTypesSource = this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingType").Result;
            this.kvCPO = this.kvApplicationTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "LicensingCPO");
            this.kvCIPO = this.kvApplicationTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "LicensingCIPO");

            this.kvApplicationStatusSource = this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus").Result;
            this.kvProcedureCompleted = this.kvApplicationStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "ProcedureCompleted");
        }

        #region Candidate Provider
        public async Task<ResultContext<CandidateProviderVM>> CreateCandidateProvider(ResultContext<CandidateProviderVM> inputContext)
        {
            try
            {
                var userID = await this.dataSourceService.GetSettingByIntCodeAsync("UserIDBindWithSystem");
                var userFromSystem = await this.userManager.FindByIdAsync(userID.SettingValue);

                var candidateProvider = inputContext.ResultContextObject.To<CandidateProvider>();

                var IdStatusEmailValidationExpected = await this.dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderStatus", "EMAIL_VALIDATION_EXPECTED");


                candidateProvider.CandidateProviderStatuses.Add(
                    new CandidateProviderStatus()
                    {
                        IdStatus = IdStatusEmailValidationExpected.IdKeyValue,
                        StatusDate = DateTime.Now
                    }
                );


                candidateProvider.IdRegistrationApplicationStatus = IdStatusEmailValidationExpected.IdKeyValue;
                candidateProvider.IsActive = true;
                candidateProvider.IdCreateUser = userFromSystem.IdUser;
                candidateProvider.IdModifyUser = userFromSystem.IdUser;
                candidateProvider.CreationDate = DateTime.Now;
                candidateProvider.ModifyDate = DateTime.Now;
                await this.repository.AddAsync<CandidateProvider>(candidateProvider);

                var result = await this.repository.SaveChangesAsync(false);

                inputContext.ResultContextObject.IdCandidate_Provider = candidateProvider.IdCandidate_Provider;



            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                inputContext.AddErrorMessage(ex.Message);
            }


            return inputContext;
        }

        public async Task<ResultContext<NoResult>> CreateCandidateProviderUserAsync(ResultContext<TokenVM> inputContext, bool isProfileAdministrator = false)
        {
            ResultContext<NoResult> resultContext = new ResultContext<NoResult>();

            try
            {
                var candidateProviderStatusValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderStatus");

                var IndentTypeEGN = await this.dataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");

                var IdStatusEmailValidationExpected = candidateProviderStatusValues.FirstOrDefault(v => v.KeyValueIntCode == "EMAIL_VALIDATION_EXPECTED"); ;
                var IdStatusSuccessfulEmailValidation = candidateProviderStatusValues.FirstOrDefault(v => v.KeyValueIntCode == "SUCCESSFUL_EMAIL_VALIDATION");
                var IdStatusAwaitingConfirmationNapoo = candidateProviderStatusValues.FirstOrDefault(v => v.KeyValueIntCode == "AWAITING_CONFIRMATION_FROM_NAPOO");
                var IdStatusApprovedApplicationFromNapoo = candidateProviderStatusValues.FirstOrDefault(v => v.KeyValueIntCode == "APPROVED_APPLICATION_FROM_NAPOO");
                var IdStatusAutomaticApproval = candidateProviderStatusValues.FirstOrDefault(v => v.KeyValueIntCode == "AUTOMATIC_APPROVAL");
                var IdStatusCreatedUser = candidateProviderStatusValues.FirstOrDefault(v => v.KeyValueIntCode == "CREATED_USER");


                CandidateProviderVM model = new CandidateProviderVM();
                model.Token = inputContext.ResultContextObject.Token;


                var candidateProviderFromDb = await this.repository
                                              .All<CandidateProvider>(FilterCandidateProvider(model))
                                              .FirstOrDefaultAsync();

                if (candidateProviderFromDb.IdRegistrationApplicationStatus != IdStatusEmailValidationExpected.IdKeyValue &&
                    candidateProviderFromDb.IdRegistrationApplicationStatus != IdStatusApprovedApplicationFromNapoo.IdKeyValue)
                {
                    resultContext.AddErrorMessage("Вашата електронна поща е вече валидирана");
                    return resultContext;
                }






                ///Ако заявката е направена от пълномошник се валидира ел. поща и се слага в статус за одобрение от НАПОО
                if (string.IsNullOrEmpty(candidateProviderFromDb.ManagerName) &&
                    candidateProviderFromDb.IdRegistrationApplicationStatus == IdStatusEmailValidationExpected.IdKeyValue)
                {

                    candidateProviderFromDb.CandidateProviderStatuses.Add(
                        new CandidateProviderStatus()
                        {
                            IdStatus = IdStatusSuccessfulEmailValidation.IdKeyValue,
                            StatusDate = DateTime.Now
                        }
                    );

                    candidateProviderFromDb.CandidateProviderStatuses.Add(
                    new CandidateProviderStatus()
                    {
                        IdStatus = IdStatusAwaitingConfirmationNapoo.IdKeyValue,
                        StatusDate = DateTime.Now
                    });

                    candidateProviderFromDb.IdRegistrationApplicationStatus = IdStatusAwaitingConfirmationNapoo.IdKeyValue;

                    await this.repository.SaveChangesAsync();

                    ///TODO: Да се създаде Извести и да се изпрати E-mail
                    ///
                    ResultContext<CandidateProvider> resultCandidateProvider = new ResultContext<CandidateProvider>();
                    resultCandidateProvider.ResultContextObject = candidateProviderFromDb;
                    ///
                    await this.NotificationService.SendNotificationForAwaitingConfirmationNapoo(resultCandidateProvider);

                    resultContext.AddMessage("Електронната заявка очаква одобрение от НАПОО.");
                    return resultContext;

                }

                //Ako заявката се подава от представител на фирмата се валидра ел. пощата
                if (!string.IsNullOrEmpty(candidateProviderFromDb.ManagerName) &&
                    candidateProviderFromDb.IdRegistrationApplicationStatus == IdStatusEmailValidationExpected.IdKeyValue)
                {
                    candidateProviderFromDb.CandidateProviderStatuses.Add(
                        new CandidateProviderStatus()
                        {
                            IdStatus = IdStatusSuccessfulEmailValidation.IdKeyValue,
                            StatusDate = DateTime.Now
                        }
                    );

                    candidateProviderFromDb.CandidateProviderStatuses.Add(
                        new CandidateProviderStatus()
                        {
                            IdStatus = IdStatusAutomaticApproval.IdKeyValue,
                            StatusDate = DateTime.Now
                        }
                    );
                }



                Person person = new Person();

                person.TaxOffice = string.Empty;

                ///Ако не е попълнено полето ManagerName(Представлявано от) се взема полето AttorneyName пълномощник
                string personFullName = (!string.IsNullOrEmpty(candidateProviderFromDb.ManagerName) ? candidateProviderFromDb.ManagerName : candidateProviderFromDb.AttorneyName);




                person.PreSetPersonName(personFullName);
                person.IdIndentType = IndentTypeEGN.IdKeyValue;
                person.Indent = candidateProviderFromDb.Indent;
                person.Email = candidateProviderFromDb.ProviderEmail;
                person.Position = candidateProviderFromDb.Title;
                person.Title = String.Empty;
                person.CandidateProviderPersons.Add(new CandidateProviderPerson
                {
                    IdCandidate_Provider = candidateProviderFromDb.IdCandidate_Provider,
                    Person = person,
                    IsAdministrator = isProfileAdministrator
                });






                await this.repository.AddAsync<Person>(person);
                await this.repository.SaveChangesAsync();



                ApplicationUserVM createUserVM = new ApplicationUserVM();

                createUserVM.IdPerson = person.IdPerson;
                createUserVM.Email = person.Email;
                createUserVM.EIK = candidateProviderFromDb.PoviderBulstat;
                createUserVM.FirstName = person.FirstName;
                createUserVM.FamilyName = person.FamilyName;
                createUserVM.IdCandidateProvider = candidateProviderFromDb.IdCandidate_Provider;
                createUserVM.IdUserStatus = dataSourceService.GetKeyValueByIntCodeAsync("UserStatus", "Active").Result.IdKeyValue;


                if (UserProps.UserId != 0)
                {
                    createUserVM.IdCreateUser = UserProps.UserId;
                    createUserVM.IdModifyUser = UserProps.UserId;

                }
                else
                {
                    var user = await this.dataSourceService.GetSettingByIntCodeAsync("UserIDBindWithSystem");

                    var appUser = await applicationUserService.GetApplicationUsersById(user.SettingValue);

                    createUserVM.IdCreateUser = appUser.IdUser;
                    createUserVM.IdModifyUser = appUser.IdUser;
                }


                ResultContext<ApplicationUserVM> resultCreateUserVM = new ResultContext<ApplicationUserVM>();
                resultCreateUserVM.ResultContextObject = createUserVM;
                resultCreateUserVM.ResultContextObject.IsFirstReistration = true;

                resultCreateUserVM = await applicationUserService.CreateApplicationUserAsync(resultCreateUserVM);

                candidateProviderFromDb.CandidateProviderStatuses.Add(
                       new CandidateProviderStatus()
                       {
                           IdStatus = IdStatusCreatedUser.IdKeyValue,
                           StatusDate = DateTime.Now
                       }
                   );

                candidateProviderFromDb.IdModifyUser = resultCreateUserVM.ResultContextObject.IdUser;
                candidateProviderFromDb.IdCreateUser = resultCreateUserVM.ResultContextObject.IdUser;

                await this.repository.SaveChangesAsync();

                await this.MailService.SendEmailNewRegistrationUserPass(resultCreateUserVM);


            }
            catch (Exception e)
            {
                resultContext.AddErrorMessage(e.Message);

                _logger.LogError(e.Message);
                _logger.LogError(e.InnerException?.Message);
                _logger.LogError(e.StackTrace);


            }

            return resultContext;
        }

        protected Expression<Func<CandidateProvider, bool>> FilterCandidateProvider(CandidateProviderVM model)
        {
            var predicate = PredicateBuilder.True<CandidateProvider>();

            if (!string.IsNullOrEmpty(model.Token))
            {
                predicate = predicate.And(p => p.Token == model.Token);
            }

            if (model.IdTypeLicense > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdTypeLicense == model.IdTypeLicense);
            }

            if (model.IdCandidate_Provider != 0)
            {
                predicate = predicate.And(p => p.IdCandidate_Provider == model.IdCandidate_Provider);
            }

            if (model.IdTypeApplication is not null)
            {
                predicate = predicate.And(p => p.IdTypeApplication == model.IdTypeApplication);
            }

            if (model.IdCandidateProviderActive is not null)
            {
                predicate = predicate.Or(p => p.IdCandidateProviderActive == model.IdCandidateProviderActive);
            }

            if (model.StartedProcedureIds is not null && model.StartedProcedureIds.Count > 0)
            {
                predicate = predicate.And(p => model.StartedProcedureIds.Contains(p.IdStartedProcedure.Value));
            }

            return predicate;
        }

        public async Task<ResultContext<List<CandidateProviderVM>>> ApproveRegistrationAsync(ResultContext<List<CandidateProviderVM>> inputContext)
        {

            try
            {
                var result = GlobalConstants.INVALID_ID_ZERO;
                var idProviders = inputContext.ResultContextObject.Select(x => x.IdCandidate_Provider);

                IQueryable<CandidateProvider> candidateProviders = this.repository.All<CandidateProvider>(x => idProviders.Contains(x.IdCandidate_Provider));

                var awaitingConform = await dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderStatus", "AWAITING_CONFIRMATION_FROM_NAPOO");
                var approvedFromNAPOO = await dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderStatus", "APPROVED_APPLICATION_FROM_NAPOO");

                foreach (var item in candidateProviders)
                {
                    if (item.IdRegistrationApplicationStatus == awaitingConform.IdKeyValue)
                    {
                        item.IdRegistrationApplicationStatus = approvedFromNAPOO.IdKeyValue;
                        item.Token = inputContext.ResultContextObject.First(x => x.IdCandidate_Provider == item.IdCandidate_Provider).Token;

                        item.CandidateProviderStatuses.Add(new CandidateProviderStatus()
                        {
                            IdStatus = approvedFromNAPOO.IdKeyValue,
                            StatusDate = DateTime.Now
                        });

                    }
                    else
                    {
                        var statusKeyValue = await dataSourceService.GetKeyValueByIdAsync(item.IdRegistrationApplicationStatus);
                        inputContext.AddErrorMessage($"Можете да одобрите електронна регистрация само на заявки на статус 'Очаква се одобрение на регистрацията от НАПОО'! Избраният ред с ЕИК {item.PoviderBulstat} е на статус '{statusKeyValue.Name}'!");
                    }

                }

                if (!inputContext.HasErrorMessages)
                {
                    result = await this.repository.SaveChangesAsync(false);
                }


                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();
                currentContext.ResultContextObject = new TokenVM();



                if (result > GlobalConstants.INVALID_ID_ZERO)
                {
                    //Създаване на потребител и изпращане на мейл за успешното създаване
                    foreach (var item in inputContext.ResultContextObject)
                    {
                        currentContext.ResultContextObject.Token = item.Token;
                        //currentContext = this.CommonService.GetDecodeToken(currentContext);
                        ResultContext<NoResult> currentNoResult = await CreateCandidateProviderUserAsync(currentContext, true);

                    }

                    inputContext.AddMessage("Избраните от Вас форми за регистрация са одобрени!");
                }

                return inputContext;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                inputContext.AddErrorMessage(ex.Message);
            }


            return inputContext;
        }

        public async Task<ResultContext<List<CandidateProviderVM>>> RejectRegistrationAsync(ResultContext<List<CandidateProviderVM>> inputContext, string reason)
        {

            try
            {
                var result = GlobalConstants.INVALID_ID_ZERO;
                var idProviders = inputContext.ResultContextObject.Select(x => x.IdCandidate_Provider);

                IQueryable<CandidateProvider> candidateProviders = this.repository.All<CandidateProvider>(x => idProviders.Contains(x.IdCandidate_Provider));

                var rejectedFromNAPOO = await dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderStatus", "REJECTED_APPLICATION_FORM_NAPOO");

                foreach (var item in candidateProviders)
                {

                    item.IdRegistrationApplicationStatus = rejectedFromNAPOO.IdKeyValue;
                    item.Token = inputContext.ResultContextObject.First(x => x.IdCandidate_Provider == item.IdCandidate_Provider).Token;
                    item.RejectionReason = reason;
                    item.CandidateProviderStatuses.Add(new CandidateProviderStatus()
                    {
                        IdStatus = rejectedFromNAPOO.IdKeyValue,
                        StatusDate = DateTime.Now
                    });


                }

                if (!inputContext.HasErrorMessages)
                {
                    result = await this.repository.SaveChangesAsync(false);
                }


                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();
                currentContext.ResultContextObject = new TokenVM();



                if (result > GlobalConstants.INVALID_ID_ZERO)
                {
                    //Изпращане на мейл за отказване на заявление
                    foreach (var item in idProviders)
                    {
                        var provider = await GetCandidateProviderByIdAsync(new CandidateProviderVM { IdCandidate_Provider = item });
                        ResultContext<CandidateProviderVM> providerContext = new ResultContext<CandidateProviderVM>();
                        providerContext.ResultContextObject = provider;
                        this.MailService.SendEmailRejectRegistration(providerContext);
                    }
                    inputContext.AddMessage("Отказана електронна регистрация в ИС!");
                }

                return inputContext;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                inputContext.AddErrorMessage(ex.Message);
            }


            return inputContext;
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetAllExpertsAsync(CandidateProviderVM filterExpertVM)
        {
            IQueryable<CandidateProvider> candidateProviders = this.repository.All<CandidateProvider>(FilterCandidateProvider(filterExpertVM));

            List<CandidateProviderVM> candidateProviderVMs = await candidateProviders.To<CandidateProviderVM>().Where(x => x.IdRegistrationApplicationStatus != null).ToListAsync();
            var applicationStatuses = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderStatus");

            foreach (var entry in candidateProviderVMs)
            {

                var applicationStatus = applicationStatuses.FirstOrDefault(a => a.IdKeyValue == entry.IdRegistrationApplicationStatus);

                if (applicationStatus is not null)
                {
                    entry.ApplicationStatus = applicationStatus.Name;
                }
            }

            return candidateProviderVMs;
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetAllCandidateProvidersAsync()
        {
            CandidateProviderVM filter = new CandidateProviderVM();

            //if (this.UserProps.IdCandidateProvider != 0) 
            //{
            //    filter.IdCandidate_Provider = this.UserProps.IdCandidateProvider;   
            //}

            try
            {
                var data = this.repository.AllReadonly<CandidateProvider>(FilterCandidateProvider(filter));
                var dataAsVM = await data.To<CandidateProviderVM>().ToListAsync();



                var applicationTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeApplication");
                var applicationStatuses = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");

                foreach (var entry in dataAsVM)
                {
                    var applicationType = applicationTypes.FirstOrDefault(a => a.IdKeyValue == entry.IdTypeApplication);
                    var applicationStatus = applicationStatuses.FirstOrDefault(a => a.IdKeyValue == entry.IdTypeApplication);

                    if (applicationType is not null)
                    {
                        entry.TypeApplication = applicationType.Name;
                    }

                    if (applicationStatus is not null)
                    {
                        entry.ApplicationStatus = applicationStatus.Name;
                    }
                }
                return dataAsVM;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetAllInаctiveCandidateProvidersByIdActiveCandidateProviderAsync(int idCandidateProvider)
        {
            var kvAppStatusPrepDocLicensing = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "PreparationDocumentationLicensing");
            var kvAppStatusProcTerminated = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "ProcedureTerminatedByCenter");
            var data = this.repository.AllReadonly<CandidateProvider>(x => x.IdCandidateProviderActive == idCandidateProvider 
            && x.IdApplicationStatus != kvAppStatusPrepDocLicensing.IdKeyValue && x.IdApplicationStatus != kvAppStatusProcTerminated.IdKeyValue)
                .OrderBy(x => x.ApplicationDate.HasValue).ThenBy(x => x.ApplicationDate);

            var dataAsVM = await data.To<CandidateProviderVM>().ToListAsync();

            var applicationTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeApplication");
            var applicationStatuses = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");

            foreach (var entry in dataAsVM)
            {
                var applicationType = applicationTypes.FirstOrDefault(a => a.IdKeyValue == entry.IdTypeApplication);
                var applicationStatus = applicationStatuses.FirstOrDefault(a => a.IdKeyValue == entry.IdApplicationStatus);

                if (applicationType is not null)
                {
                    entry.TypeApplication = applicationType.Name;
                }

                if (applicationStatus is not null)
                {
                    entry.ApplicationStatus = applicationStatus.Name;
                }
            }

            return dataAsVM;
        }
        public async Task<MemoryStream> GenerateExcelReportForCandidateProviders(string year)
        {
            try
            {
                List<CandidateProviderVM> candidates = this.repository
                    .All<CandidateProvider>()
                    .To<CandidateProviderVM>(x => x.Location.Municipality.District)
                    .Where(x => x.CreationDate.Year.ToString().Equals(year))
                    .ToList();

                List<KeyValueVM> ownershipKeys = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProviderOwnership")).ToList();

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    int row = 1;

                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Xlsx;

                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    foreach (var candidate in candidates)
                    {
                        if (candidate.IdProviderOwnership != 0)
                        {
                            foreach (var key in ownershipKeys)
                            {
                                if (key.IdKeyValue == candidate.IdProviderOwnership)
                                {
                                    candidate.ProviderOwnership = key;
                                    break;
                                }
                            }

                        }
                        else
                        {
                            candidate.ProviderOwnership = new KeyValueVM
                            {
                                DefaultValue2 = ""
                            };
                        }

                        if (candidate.ManagerName == null)
                            candidate.ManagerName = "";

                        if (candidate.ProviderPhone == null)
                            candidate.ProviderPhone = "";

                        if (candidate.ProviderFax == null)
                            candidate.ProviderFax = "";

                        if (candidate.ProviderPhoneCorrespondence == null)
                            candidate.ProviderPhoneCorrespondence = "";

                        if (candidate.LicenceNumber == null)
                            candidate.LicenceNumber = "";

                        if (candidate.ProviderEmail == null)
                            candidate.ProviderEmail = "";

                        object[] arrayRow = new object[15]
                 {
                 row,
                 year,
                 candidate.LicenceNumber,
                 candidate.PoviderBulstat,
                 candidate.ProviderName,
                 candidate.ZipCode,
                 candidate.Location.LocationCode,
                 candidate.Location.Municipality.District.NSICode,
                 candidate.ProviderAddress,
                 candidate.ProviderPhone,
                 candidate.ProviderFax,
                 candidate.ProviderEmail,
                 candidate.ManagerName,
                 candidate.ProviderPhoneCorrespondence,
                 candidate.ProviderOwnership.DefaultValue2
                 };

                        worksheet.ImportArray(arrayRow, row, 1, false);
                        row++;

                    }
                    if (candidates.Count() > 0)
                    {
                        worksheet.Range[$"A1:O{candidates.Count()}"].AutofitColumns();
                        worksheet.Range[$"A1:O{candidates.Count()}"].BorderInside(ExcelLineStyle.Medium);
                        worksheet.Range[$"A1:O{candidates.Count()}"].BorderAround(ExcelLineStyle.Medium);
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

        /// <summary>
        /// зарежда данни за активни ЦПО/ЦИПО за AutoComplete. При showAllTypes = true, зарежда и ЦПО, и ЦИПО
        /// </summary>
        /// <param name="licensingType">LicensingCPO | LicensingCIPO</param>
        /// <param name="showAllTypes">true | false</param>
        /// <returns></returns>
        public async Task<IEnumerable<CandidateProviderVM>> GetAllCandidateProvidersForAutoComplete(string licensingType, bool showAllTypes = false)
        {
            int IdTypeLicense = 0;
            if (!showAllTypes)
            {
                IdTypeLicense = this.kvApplicationTypesSource.FirstOrDefault(x => x.KeyValueIntCode == licensingType).IdKeyValue;
            }

            var data = !showAllTypes 
                ? await this.repository
               .AllReadonly<CandidateProvider>(x => x.IsActive == true && x.IdLicenceStatus != null && x.IdTypeLicense == IdTypeLicense)
               .To<CandidateProviderVM>().ToListAsync()
               : await this.repository
               .AllReadonly<CandidateProvider>(x => x.IsActive == true && x.IdLicenceStatus != null)
               .To<CandidateProviderVM>().ToListAsync();


            return data;
        }



        public async Task<IEnumerable<CandidateProviderVM>> GetAllActiveCandidateProvidersWithoutAnythingIncludedAsync(CandidateProviderVM model)
        {

            var data = await this.repository
               .AllReadonly<CandidateProvider>(FilterActiveCandidateProvidersWithoutAnythingIncluded(model))
               .To<CandidateProviderVM>().ToListAsync();


            return data;
        }

        protected Expression<Func<CandidateProvider, bool>> FilterActiveCandidateProvidersWithoutAnythingIncluded(CandidateProviderVM model)
        {
            var predicate = PredicateBuilder.True<CandidateProvider>();

            if (!model.SkipIsActive)
            {
                predicate = predicate.And(p => p.IsActive == model.IsActive);
            }

            if (model.IdTypeLicense != 0)
            {
                predicate = predicate.And(p => p.IdTypeLicense == model.IdTypeLicense);
            }

            foreach (int idStatus in model.ApplicationStatusFilter_NOT_IN_List)
            {
                predicate = predicate.And(p => p.IdApplicationStatus != idStatus);
            }

            foreach (int idStatus in model.ApplicationStatusFilter_IN_List)
            {
                predicate = predicate.And(p => p.IdApplicationStatus == idStatus);
            }

            return predicate;
        }


        public async Task<IEnumerable<CandidateProviderVM>> GetAllActiveCPOCandidateProvidersWithoutAnythingIncludedAsync()
        {
            var data = this.repository.AllReadonly<CandidateProvider>(x => x.IsActive && x.IdTypeLicense == this.kvCPO.IdKeyValue);
            var dataAsVM = await data.To<CandidateProviderVM>().ToListAsync();

            return dataAsVM;
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetFirstLicencingCandidateProviderByIdCandidateProviderAsync(int idCandidateProvider, bool isCPO = true)
        {
            try
            {
                var kvTypeLicence = isCPO
                    ? (await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "FirstLicenzing"))
                    : (await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "FirstLicensingCIPO"));

                var candidateProviders = await this.repository.AllReadonly<CandidateProvider>(x => (x.IdCandidateProviderActive == idCandidateProvider && x.IdTypeApplication == kvTypeLicence.IdKeyValue) || (x.IdCandidate_Provider == idCandidateProvider && x.IdTypeApplication == kvTypeLicence.IdKeyValue))
                    .To<CandidateProviderVM>(x => x.StartedProcedure.StartedProcedureProgresses.OrderByDescending(y => y.IdStartedProcedureProgress), x => x.Payments)
                    .ToListAsync();

                var applicationStatuses = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");
                foreach (var entry in candidateProviders)
                {
                    entry.TypeApplication = kvTypeLicence.Name;

                    var applicationStatus = applicationStatuses.FirstOrDefault(a => a.IdKeyValue == entry.IdApplicationStatus);
                    if (applicationStatus is not null)
                    {
                        entry.ApplicationStatus = applicationStatus.Name;
                    }
                }

                return candidateProviders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return null;
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetLicencingChangeCandidateProvidersByIdCandidateProviderAsync(int idCandidateProvider)
        {
            try
            {
                var kvChangesSource = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeApplication")).Where(x => !x.KeyValueIntCode.Contains("FirstLicenzing")).ToList();
                var idsKvFirstLicencingSource = kvChangesSource.Select(x => x.IdKeyValue).ToList();
                var candidateProviders = await this.repository.AllReadonly<CandidateProvider>(x => (x.IdCandidateProviderActive == idCandidateProvider && x.IdTypeApplication.HasValue && idsKvFirstLicencingSource.Contains(x.IdTypeApplication.Value)) || (x.IdCandidate_Provider == idCandidateProvider && x.IdTypeApplication.HasValue && idsKvFirstLicencingSource.Contains(x.IdTypeApplication.Value)))
                    .To<CandidateProviderVM>(x => x.StartedProcedure.StartedProcedureProgresses.OrderByDescending(y => y.IdStartedProcedureProgress), x => x.Payments)
                    .ToListAsync();

                var applicationStatuses = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");
                foreach (var entry in candidateProviders)
                {
                    var kvChange = kvChangesSource.FirstOrDefault(x => x.IdKeyValue == entry.IdTypeApplication);
                    if (kvChange is not null)
                    {
                        entry.TypeApplication = kvChange.Name;
                    }

                    var applicationStatus = applicationStatuses.FirstOrDefault(a => a.IdKeyValue == entry.IdApplicationStatus);
                    if (applicationStatus is not null)
                    {
                        entry.ApplicationStatus = applicationStatus.Name;
                    }
                }

                return candidateProviders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return null;
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetAllActiveProceduresForRegisterAsync(bool isCPO = true)
        {
            var applicationStatuses = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");
            var displayApplicationStatusIds = applicationStatuses.Where(x => x.KeyValueIntCode == "ExpertCommissionAssessment"
                || x.KeyValueIntCode == "AdministrativeCheck"
                || x.KeyValueIntCode == "RequestedByCPOOrCIPO"
                || x.KeyValueIntCode == "ProcedureWasRegisteredInKeepingSystem"
                || x.KeyValueIntCode == "LeadingExpertGavePositiveAssessment"
                || x.KeyValueIntCode == "LeadingExpertGaveNegativeAssessment"
                || x.KeyValueIntCode == "LicensingExpertiseStarted"
                || x.KeyValueIntCode == "CorrectionApplication").Select(x => x.IdKeyValue).ToList();

            var filter = PredicateBuilder.True<CandidateProvider>();
            if (isCPO)
            {
                filter = filter.And(x => x.IdTypeLicense == this.kvCPO.IdKeyValue);
            }
            else
            {
                filter = filter.And(x => x.IdTypeLicense == this.kvCIPO.IdKeyValue);
            }

            filter = filter.And(x => x.IdTypeApplication.HasValue);
            filter = filter.And(x => x.IdApplicationStatus.HasValue);
            filter = filter.And(x => displayApplicationStatusIds.Contains(x.IdApplicationStatus!.Value));

            var data = this.repository.AllReadonly<CandidateProvider>(filter);
            var dataAsVM = await data.To<CandidateProviderVM>().ToListAsync();
            foreach (var entry in dataAsVM)
            {
                var applicationStatus = applicationStatuses.FirstOrDefault(a => a.IdKeyValue == entry.IdApplicationStatus);
                if (applicationStatus is not null)
                {
                    entry.ApplicationStatus = applicationStatus.Name;
                }
            }

            return dataAsVM;
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetAllActiveProceduresAsync(bool isCPO = true)
        {
            var applicationStatuses = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");
            var displayApplicationStatusIds = applicationStatuses.Where(x => x.KeyValueIntCode == "ExpertCommissionAssessment"
                || x.KeyValueIntCode == "AdministrativeCheck"
                || x.KeyValueIntCode == "RequestedByCPOOrCIPO"
                || x.KeyValueIntCode == "ProcedureWasRegisteredInKeepingSystem"
                || x.KeyValueIntCode == "LeadingExpertGavePositiveAssessment"
                || x.KeyValueIntCode == "LeadingExpertGaveNegativeAssessment"
                || x.KeyValueIntCode == "LicensingExpertiseStarted"
                || x.KeyValueIntCode == "CorrectionApplication").Select(x => x.IdKeyValue).ToList();

            var filter = PredicateBuilder.True<CandidateProvider>();
            if (isCPO)
            {
                filter = filter.And(x => x.IdTypeLicense == this.kvCPO.IdKeyValue);
            }
            else
            {
                filter = filter.And(x => x.IdTypeLicense == this.kvCIPO.IdKeyValue);
            }

            filter = filter.And(x => x.IdTypeApplication.HasValue);
            filter = filter.And(x => x.IdApplicationStatus.HasValue);
            filter = filter.And(x => displayApplicationStatusIds.Contains(x.IdApplicationStatus!.Value));

            var roles = await this.GetUserRoles();

            #region Филтър ако си външен експерт или член на екперстна комисия
            //Redmine #16874 and #16864
            int idPerson = this.UserProps.IdPerson;

            if (!roles.Contains("NAPOO_Expert") && !roles.Contains("SUPPORT"))
            {
                if (roles.Contains("EXPERT_COMMITTEES"))
                {
                    var kvActiveCommision = await this.dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");

                    //Добавяне невалидно ид, ако си член на комисия но не си добавен никъде да не можеш да видиш нищо
                    filter = filter.And(p => p.IdStartedProcedure == GlobalConstants.INVALID_ID);

                    var expert = await this.repository.AllReadonly<Expert>(e => e.IdPerson == idPerson)
                        .Include(x => x.ExpertExpertCommissions)
                        .FirstOrDefaultAsync();

                    if (expert is not null)
                    {
                        var commissionsIds = expert.ExpertExpertCommissions.Where(c => c.IdStatus == kvActiveCommision.IdKeyValue).Select(c => c.IdExpertCommission).ToList();

                        var listRrocedureCommissions = this.repository.AllReadonly<ProcedureExpertCommission>(e => commissionsIds.Contains(e.IdExpertCommission)).ToList();

                        if (listRrocedureCommissions is not null && listRrocedureCommissions.Count > 0)
                        {
                            var procedureIds = listRrocedureCommissions.Select(c => c.IdStartedProcedure).ToHashSet();

                            foreach (var item in procedureIds)
                            {
                                filter = filter.Or(p => p.IdStartedProcedure == item);
                            }
                        }
                    }
                }

                if (roles.Contains("EXTERNAL_EXPERTS"))
                {
                    var kvActiveCommision = await this.dataSourceService.GetKeyValueByIntCodeAsync("ExpertStatus", "ActiveExpert");

                    //Добавяне невалидно ид, ако си външен експер но не си добавен никъде да не можеш да видиш нищо
                    filter = filter.And(p => p.IdStartedProcedure == GlobalConstants.INVALID_ID);

                    var experDirections = this.repository.AllReadonly<ExpertProfessionalDirection>(e => e.Expert.IdPerson == idPerson && e.IdStatus == kvActiveCommision.IdKeyValue).ToList();
                    var expertIds = experDirections.Select(d => d.IdExpert).ToList();

                    var procedureExternalExperts = this.repository.AllReadonly<ProcedureExternalExpert>(e => expertIds.Contains(e.IdExpert)).ToList();
                    procedureExternalExperts = procedureExternalExperts.Where(p => p.IsActive).ToList();

                    if (procedureExternalExperts is not null && procedureExternalExperts.Count > 0)
                    {
                        var procedureIds = procedureExternalExperts.Select(c => c.IdStartedProcedure).ToHashSet();

                        foreach (var item in procedureIds)
                        {
                            filter = filter.Or(p => p.IdStartedProcedure == item);
                        }
                    }
                }
            }
            #endregion

            var data = this.repository.AllReadonly<CandidateProvider>(filter);
            var dataAsVM = await data.To<CandidateProviderVM>(x => x.StartedProcedure.StartedProcedureProgresses.OrderByDescending(y => y.IdStartedProcedureProgress), x => x.StartedProcedure.ProcedureExternalExperts, x => x.Payments).ToListAsync();
            var applicationTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeApplication");
            foreach (var entry in dataAsVM)
            {
                var applicationType = applicationTypes.FirstOrDefault(a => a.IdKeyValue == entry.IdTypeApplication);
                var applicationStatus = applicationStatuses.FirstOrDefault(a => a.IdKeyValue == entry.IdApplicationStatus);

                if (applicationType is not null)
                {
                    entry.TypeApplication = applicationType.Name;
                }

                if (applicationStatus is not null)
                {
                    entry.ApplicationStatus = applicationStatus.Name;
                }
            }

            return dataAsVM;
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetAllApplicationsByIdCandidateProviderAsync(int idCandidateProvider)
        {
            CandidateProviderVM filter = new CandidateProviderVM();
            //filter.IdCandidateProviderActive = idCandidateProvider;

            if (!(await this.HasClaim("ShowAllCandidateProvider")))
            {
                filter.IdCandidate_Provider = idCandidateProvider;
            }
            var roles = await this.GetUserRoles();

            #region Филтър ако си външен експерт или член на екперстна комисия
            //Redmine #16874 and #16864
            int idPerson = this.UserProps.IdPerson;

            if (!roles.Contains("NAPOO_Expert") && !roles.Contains("SUPPORT"))
            {
                if (roles.Contains("EXPERT_COMMITTEES"))
                {
                    var kvActiveCommision = await this.dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");

                    //Добавяне невалидно ид, ако си член на комисия но не си добавен никъде да не можеш да видиш нищо
                    filter.StartedProcedureIds.Add(GlobalConstants.INVALID_ID);

                    var expert = await this.repository.AllReadonly<Expert>(e => e.IdPerson == idPerson)
                        .Include(x => x.ExpertExpertCommissions)
                        .FirstOrDefaultAsync();

                    if (expert is not null)
                    {
                        var commissionsIds = expert.ExpertExpertCommissions.Where(c => c.IdStatus == kvActiveCommision.IdKeyValue).Select(c => c.IdExpertCommission).ToList();

                        var listRrocedureCommissions = this.repository.AllReadonly<ProcedureExpertCommission>(e => commissionsIds.Contains(e.IdExpertCommission)).ToList();

                        if (listRrocedureCommissions is not null && listRrocedureCommissions.Count > 0)
                        {
                            var procedureIds = listRrocedureCommissions.Select(c => c.IdStartedProcedure).ToHashSet();

                            foreach (var item in procedureIds)
                            {
                                filter.StartedProcedureIds.Add(item);
                            }
                        }
                    }
                }

                if (roles.Contains("EXTERNAL_EXPERTS"))
                {
                    var kvActiveCommision = await this.dataSourceService.GetKeyValueByIntCodeAsync("ExpertStatus", "ActiveExpert");

                    //Добавяне невалидно ид, ако си външен експер но не си добавен никъде да не можеш да видиш нищо
                    filter.StartedProcedureIds.Add(GlobalConstants.INVALID_ID);

                    var experDirections = this.repository.AllReadonly<ExpertProfessionalDirection>(e => e.Expert.IdPerson == idPerson && e.IdStatus == kvActiveCommision.IdKeyValue).ToList();
                    var expertIds = experDirections.Select(d => d.IdExpert).ToList();

                    var procedureExternalExperts = this.repository.AllReadonly<ProcedureExternalExpert>(e => expertIds.Contains(e.IdExpert)).ToList();
                    procedureExternalExperts = procedureExternalExperts.Where(p => p.IsActive).ToList();

                    if (procedureExternalExperts is not null && procedureExternalExperts.Count > 0)
                    {
                        var procedureIds = procedureExternalExperts.Select(c => c.IdStartedProcedure).ToHashSet();

                        foreach (var item in procedureIds)
                        {
                            filter.StartedProcedureIds.Add(item);
                        }
                    }
                }
            }
            #endregion



            var data = this.repository.AllReadonly<CandidateProvider>(FilterCandidateProvider(filter));
            data = data.Where(x => x.IdTypeLicense == this.kvCPO.IdKeyValue);
            var dataAsVM = await data.To<CandidateProviderVM>(x => x.StartedProcedure.StartedProcedureProgresses.OrderByDescending(y => y.IdStartedProcedureProgress)).ToListAsync();

            var applicationTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeApplication");
            var applicationStatuses = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");

            foreach (var entry in dataAsVM)
            {
                var applicationType = applicationTypes.FirstOrDefault(a => a.IdKeyValue == entry.IdTypeApplication);
                var applicationStatus = applicationStatuses.FirstOrDefault(a => a.IdKeyValue == entry.IdApplicationStatus);

                if (applicationType is not null)
                {
                    entry.TypeApplication = applicationType.Name;
                }

                if (applicationStatus is not null)
                {
                    entry.ApplicationStatus = applicationStatus.Name;
                }
            }

            return dataAsVM;
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetAllCIPOApplicationsByIdCandidateProviderAsync(int idCandidateProvider)
        {
            CandidateProviderVM filter = new CandidateProviderVM();

            if (!(await this.HasClaim("ShowAllCandidateProvider")))
            {
                filter.IdCandidate_Provider = idCandidateProvider;
            }
            var roles = await this.GetUserRoles();

            #region Филтър ако си външен експерт или член на екперстна комисия
            //Redmine #16874 and #16864
            int idPerson = this.UserProps.IdPerson;

            if (!roles.Contains("NAPOO_Expert") && !roles.Contains("SUPPORT"))
            {
                if (roles.Contains("EXPERT_COMMITTEES"))
                {
                    var kvActiveCommision = await this.dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");

                    //Добавяне невалидно ид, ако си член на комисия но не си добавен никъде да не можеш да видиш нищо
                    filter.StartedProcedureIds.Add(GlobalConstants.INVALID_ID);

                    var expert = await this.repository.AllReadonly<Expert>(e => e.IdPerson == idPerson)
                        .Include(x => x.ExpertExpertCommissions)
                        .FirstOrDefaultAsync();

                    if (expert is not null)
                    {
                        var commissionsIds = expert.ExpertExpertCommissions.Where(c => c.IdStatus == kvActiveCommision.IdKeyValue).Select(c => c.IdExpertCommission).ToList();

                        var listRrocedureCommissions = this.repository.AllReadonly<ProcedureExpertCommission>(e => commissionsIds.Contains(e.IdExpertCommission)).ToList();

                        if (listRrocedureCommissions is not null && listRrocedureCommissions.Count > 0)
                        {
                            var procedureIds = listRrocedureCommissions.Select(c => c.IdStartedProcedure).ToHashSet();

                            foreach (var item in procedureIds)
                            {
                                filter.StartedProcedureIds.Add(item);
                            }
                        }
                    }
                }

                if (roles.Contains("EXTERNAL_EXPERTS"))
                {
                    var kvActiveCommision = await this.dataSourceService.GetKeyValueByIntCodeAsync("ExpertStatus", "ActiveExpert");

                    //Добавяне невалидно ид, ако си външен експер но не си добавен никъде да не можеш да видиш нищо
                    filter.StartedProcedureIds.Add(GlobalConstants.INVALID_ID);

                    var experDirections = this.repository.AllReadonly<ExpertProfessionalDirection>(e => e.Expert.IdPerson == idPerson && e.IdStatus == kvActiveCommision.IdKeyValue).ToList();
                    var expertIds = experDirections.Select(d => d.IdExpert).ToList();

                    var procedureExternalExperts = this.repository.AllReadonly<ProcedureExternalExpert>(e => expertIds.Contains(e.IdExpert)).ToList();
                    procedureExternalExperts = procedureExternalExperts.Where(p => p.IsActive).ToList();

                    if (procedureExternalExperts is not null && procedureExternalExperts.Count > 0)
                    {
                        var procedureIds = procedureExternalExperts.Select(c => c.IdStartedProcedure).ToHashSet();

                        foreach (var item in procedureIds)
                        {
                            filter.StartedProcedureIds.Add(item);
                        }
                    }
                }
            }
            #endregion

            var data = this.repository.AllReadonly<CandidateProvider>(FilterCandidateProvider(filter));
            data = data.Where(x => x.IdTypeLicense == this.kvCIPO.IdKeyValue);
            var dataAsVM = await data.To<CandidateProviderVM>(x => x.StartedProcedure.StartedProcedureProgresses.OrderByDescending(y => y.IdStartedProcedureProgress)).ToListAsync();

            var applicationTypes = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeApplication");
            var applicationStatuses = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");

            foreach (var entry in dataAsVM)
            {
                var applicationType = applicationTypes.FirstOrDefault(a => a.IdKeyValue == entry.IdTypeApplication);
                var applicationStatus = applicationStatuses.FirstOrDefault(a => a.IdKeyValue == entry.IdTypeApplication);

                if (applicationType is not null)
                {
                    entry.TypeApplication = applicationType.Name;
                }

                if (applicationStatus is not null)
                {
                    entry.ApplicationStatus = applicationStatus.Name;
                }
            }

            return dataAsVM;
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetAllActiveCandidateProvidersSpecialitiesAsync(string keyValueIntCode)
        {
            var keyValue = dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", keyValueIntCode).Result;
            var locations = await LocationService.GetAllLocationsAsync();

            var data = this.repository.AllReadonly<CandidateProvider>(x => x.IsActive == true && x.IdTypeLicense == keyValue.IdKeyValue);
            var dataAsVM = data.To<CandidateProviderVM>(
                y => y.CandidateProviderSpecialities,
                y => y.CandidateProviderSpecialities.Select(v => v.Speciality),
                y => y.CandidateProviderSpecialities.Select(v => v.Speciality.CodeAndArea),
                y => y.CandidateProviderSpecialities.Select(v => v.Speciality.Profession),
                y => y.CandidateProviderSpecialities.Select(v => v.Speciality.Profession.ProfessionalDirection),
                y => y.CandidateProviderSpecialities.Select(v => v.Speciality.Profession.CodeAndArea)
                ).ToListAsync();
            foreach (var entry in await dataAsVM)
            {
                if (entry.IdLocationCorrespondence != null)
                {
                    entry.Location = locations.First(x => x.idLocation == entry.IdLocationCorrespondence);
                }
                if (entry.IdApplicationStatus != null)
                {
                    entry.ApplicationStatus = dataSourceService.GetKeyValueByIdAsync(entry.IdApplicationStatus).Result.Name;
                }
            }
            return dataAsVM.Result;
        }

        public async Task<bool> IsCandidateProviderPersonProfileAdministratorByIdPersonAsync(int idPerson, int idCandidateProvider)
        {
            var candidateProviderPerson = await this.repository.AllReadonly<CandidateProviderPerson>(x => x.IdPerson == idPerson && x.IdCandidate_Provider == idCandidateProvider).FirstOrDefaultAsync();

            if (candidateProviderPerson == null) return false;
            else { return candidateProviderPerson.IsAdministrator; }

        }

        public async Task<bool> IsOnlyOneProfileAdministratorByIdCandidateProviderAsync(int idCandidateProvider, int idPerson)
        {
            var persons = await this.repository.AllReadonly<CandidateProviderPerson>(x => x.IdCandidate_Provider == idCandidateProvider && x.IsAdministrator && x.IdPerson != idPerson).ToListAsync();
            return persons.Count != 0;
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetAllActiveCandidateProvidersWithoutIncludesAsync(string keyValueIntCode)
        {
            var keyValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", keyValueIntCode);
            var kvLicenseStatusesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicenseStatus");
            var kvLicenceStatusActiveValue = kvLicenseStatusesSource.FirstOrDefault(x => x.KeyValueIntCode == "active");

            var data = this.repository.AllReadonly<CandidateProvider>(x => x.IsActive == true && x.IdTypeLicense == keyValue.IdKeyValue
            && x.IdLicenceStatus.HasValue && x.IdLicenceStatus == kvLicenceStatusActiveValue!.IdKeyValue);
            var dataAsVM = await data.To<CandidateProviderVM>(x => x.LocationCorrespondence, x => x.FollowUpControls).ToListAsync();

            foreach (var entry in dataAsVM)
            {
                entry.LicenceStatusName = kvLicenceStatusActiveValue!.Name;
            }

            return dataAsVM;
        }

        public async Task<ResultContext<NoResult>> ChangeProviderToUserAsync(int idCandidateProvider)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var user = await this.userManager.FindByNameAsync("provider");
                var providerPerson = await this.repository.AllReadonly<CandidateProviderPerson>(x => x.IdPerson == user.IdPerson).FirstOrDefaultAsync();

                await this.repository.HardDeleteAsync<CandidateProviderPerson>(providerPerson!.IdCandidateProviderPerson);
                await this.repository.SaveChangesAsync();

                var candidateProviderPerson = new CandidateProviderPerson()
                {
                    IdCandidate_Provider = idCandidateProvider,
                    IdPerson = user.IdPerson!.Value,
                    IsAllowedForNotification = true,
                    IsAdministrator = true
                };

                await this.repository.AddAsync<CandidateProviderPerson>(candidateProviderPerson);
                await this.repository.SaveChangesAsync();

                var claims = await this.userManager.GetClaimsAsync(user);
                var candidateProviderClaim = claims.FirstOrDefault(x => x.Type == GlobalConstants.ID_CANDIDATE_PROVIDER);
                await this.userManager.ReplaceClaimAsync(user, candidateProviderClaim, new System.Security.Claims.Claim(
                                    GlobalConstants.ID_CANDIDATE_PROVIDER,
                                    idCandidateProvider.ToString()));

                var candidateProvider = await this.repository.GetByIdAsync<CandidateProvider>(idCandidateProvider);
                var kvTypeCPO = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");
                var roles = await this.userManager.GetRolesAsync(user);
                if (candidateProvider.IdTypeLicense == kvTypeCPO.IdKeyValue)
                {
                    if (roles.Any(x => x == "CIPO"))
                    {
                        await this.userManager.RemoveFromRoleAsync(user, "CIPO");
                    }

                    if (!roles.Any(x => x == "CPO"))
                    {
                        await this.userManager.AddToRoleAsync(user, "CPO");
                    }
                }
                else
                {
                    if (roles.Any(x => x == "CPO"))
                    {
                        await this.userManager.RemoveFromRoleAsync(user, "CPO");
                    }

                    if (!roles.Any(x => x == "CIPO"))
                    {
                        await this.userManager.AddToRoleAsync(user, "CIPO");
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

        public async Task<IEnumerable<CandidateProviderVM>> GetAllActiveCandidateProvidersAsync(string keyValueIntCode, string? applicationStatus = null)
        {
            var locations = await LocationService.GetAllLocationsAsync();
            IQueryable<CandidateProvider> data = null;
            int IdApplicationStatus = GlobalConstants.INVALID_ID_ZERO;

            if (keyValueIntCode == "All")
            {
                if (applicationStatus is null)
                {
                    IdApplicationStatus = this.kvProcedureCompleted.IdKeyValue;
                }
                else if (applicationStatus == "All")
                {
                    data = this.repository.AllReadonly<CandidateProvider>(x => x.IsActive == true);
                }
                else
                {
                    IdApplicationStatus = this.kvApplicationStatusSource.FirstOrDefault(x => x.KeyValueIntCode == applicationStatus).IdKeyValue;
                }

                if (applicationStatus != "All")
                {
                    data = this.repository.AllReadonly<CandidateProvider>(x => x.IsActive == true && x.IdApplicationStatus == IdApplicationStatus);
                }
            }
            else
            {
                var keyValue = dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", keyValueIntCode).Result;

                if (applicationStatus is null)
                {
                    IdApplicationStatus = this.kvProcedureCompleted.IdKeyValue;
                }
                else if (applicationStatus == "All")
                {
                    data = this.repository.AllReadonly<CandidateProvider>(x => x.IsActive == true && x.IdTypeLicense == keyValue.IdKeyValue);
                }
                else
                {
                    IdApplicationStatus = this.kvApplicationStatusSource.FirstOrDefault(x => x.KeyValueIntCode == applicationStatus).IdKeyValue;
                }

                if (applicationStatus != "All")
                {
                    data = this.repository.AllReadonly<CandidateProvider>(x => x.IsActive == true && x.IdTypeLicense == keyValue.IdKeyValue && x.IdApplicationStatus == IdApplicationStatus);
                }
            }


            var dataAsVM = data.To<CandidateProviderVM>(
                y => y.CandidateProviderTrainers,
                y => y.CandidateProviderTrainers.Select(v => v.CandidateProviderTrainerSpecialities.Select(x => x.Speciality.Profession)),
                y => y.CandidateProviderTrainers.Select(v => v.CandidateProviderTrainerSpecialities.Select(x => x.Speciality.Profession.ProfessionalDirection)),
                y => y.CandidateProviderTrainers.Select(v => v.CandidateProviderTrainerProfiles),
                y => y.CandidateProviderTrainers.Select(v => v.CandidateProviderTrainerProfiles.Select(x => x.ProfessionalDirection)),
                y => y.CandidateProviderTrainers.Select(v => v.CandidateProviderTrainerQualifications),
                y => y.CandidateProviderTrainers.Select(v => v.CandidateProviderTrainerDocuments),
                y => y.CandidateProviderPremises.Select(x => x.Location.Municipality.District),
                y => y.CandidateProviderPremises.Select(x => x.CandidateProviderPremisesRooms),
                y => y.CandidateProviderPremises.Select(x => x.CandidateProviderPremisesDocuments),
                y => y.CandidateProviderPremises.Select(x => x.CandidateProviderPremisesCheckings),
                y => y.CandidateProviderPremises.Select(x => x.CandidateProviderPremisesSpecialities),
                y => y.CandidateProviderPremises.Select(x => x.CandidateProviderPremisesSpecialities.Select(v => v.Speciality)),
                y => y.CandidateProviderPremises,
                y => y.CandidateProviderSpecialities,
                y => y.CandidateProviderSpecialities.Select(x => x.Speciality.Profession.ProfessionalDirection)
                ).ToListAsync();

            foreach (var entry in await dataAsVM)
            {
                if (entry.IdLocationCorrespondence != null)
                {
                    entry.Location = locations.First(x => x.idLocation == entry.IdLocationCorrespondence);
                }
                if (entry.IdApplicationStatus != null)
                {
                    entry.ApplicationStatus = dataSourceService.GetKeyValueByIdAsync(entry.IdApplicationStatus).Result.Name;
                }
            }

            return dataAsVM.Result;
        }

        public async Task<List<CandidateProviderVM>> GetAllActiveCandidateProvidersTrainersByIdCandidateProviderAsync(int IdCanidateProvider)
        {
            IQueryable<CandidateProvider> data = null;
            data = this.repository.AllReadonly<CandidateProvider>(x => x.IdCandidate_Provider == IdCanidateProvider);

            var dataAsVM = data.To<CandidateProviderVM>(
                y => y.CandidateProviderTrainers,
                y => y.CandidateProviderTrainers.Select(v => v.CandidateProviderTrainerSpecialities.Select(x => x.Speciality.Profession)),
                y => y.CandidateProviderTrainers.Select(v => v.CandidateProviderTrainerSpecialities.Select(x => x.Speciality.Profession.ProfessionalDirection)),
                y => y.CandidateProviderTrainers.Select(v => v.CandidateProviderTrainerProfiles),
                y => y.CandidateProviderTrainers.Select(v => v.CandidateProviderTrainerProfiles.Select(x => x.ProfessionalDirection)),
                y => y.CandidateProviderTrainers.Select(v => v.CandidateProviderTrainerQualifications),
                y => y.CandidateProviderTrainers.Select(v => v.CandidateProviderTrainerDocuments)
                ).ToList();

            return dataAsVM;
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetAllCandidateProvidersWithLicenseDeactivatedAsync(string keyValueIntCode)
        {
            var licenceStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicenseStatus");
            var keyValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", keyValueIntCode);
            var kvLicenceActive = licenceStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "active");

            var data = this.repository.AllReadonly<CandidateProvider>(x => x.IsActive == true && x.IdTypeLicense == keyValue.IdKeyValue && x.IdLicenceStatus.HasValue && x.IdLicenceStatus != kvLicenceActive.IdKeyValue);
            var dataAsVM = await data.To<CandidateProviderVM>(x => x.LocationCorrespondence, x => x.FollowUpControls).ToListAsync();
            foreach (var entry in dataAsVM)
            {
                if (entry.IdLicenceStatus.HasValue)
                {
                    var licenceStatusValue = licenceStatusSource.FirstOrDefault(x => x.IdKeyValue == entry.IdLicenceStatus.Value);
                    if (licenceStatusValue is not null)
                    {
                        entry.LicenceStatusName = licenceStatusValue.Name;
                    }
                }
            }

            return dataAsVM;
        }

        public async Task ChangeCandidateProviderApplicationStatusAsync(int idCandidateProvider, int idApplicationStatus)
        {
            try
            {
                var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(idCandidateProvider);
                if (candidateProviderFromDb is not null)
                {
                    candidateProviderFromDb.IdApplicationStatus = idApplicationStatus;
                    this.repository.Update<CandidateProvider>(candidateProviderFromDb);
                    await this.repository.SaveChangesAsync(false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }

        public async Task<CandidateProviderVM> GetOnlyCandidateProviderByIdAsync(int IdCandidateProvider)
        {
            var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(IdCandidateProvider);

            this.repository.Detach<CandidateProvider>(candidateProviderFromDb);

            CandidateProviderVM candidateProviderVM = candidateProviderFromDb.To<CandidateProviderVM>();

            return candidateProviderVM;
        }
        public async Task<CandidateProviderVM> GetCandidateProviderByIdAsync(CandidateProviderVM candidateProviderVM)
        {
            IQueryable<CandidateProvider> candidateProviderFromDb = this.repository.AllReadonly<CandidateProvider>(x => x.IdCandidate_Provider == candidateProviderVM.IdCandidate_Provider);

            if (candidateProviderVM != null)
            {
                var candidateProviderAsVM = candidateProviderFromDb.To<CandidateProviderVM>(
                    y => y.CandidateProviderSpecialities.Select(x => x.CandidateCurriculums.Select(e => e.CandidateCurriculumERUs)),
                    y => y.CandidateProviderSpecialities.Select(x => x.Speciality),
                    y => y.CandidateProviderTrainers.Select(x => x.CandidateProviderTrainerProfiles),
                    y => y.CandidateProviderTrainers.Select(x => x.CandidateProviderTrainerDocuments),
                    y => y.CandidateProviderTrainers.Select(x => x.CandidateProviderTrainerSpecialities),
                    y => y.CandidateProviderTrainers.Select(x => x.CandidateProviderTrainerCheckings),
                    y => y.CandidateProviderPremises.Select(x => x.CandidateProviderPremisesSpecialities),
                    y => y.CandidateProviderPremises.Select(x => x.CandidateProviderPremisesDocuments),
                    y => y.CandidateProviderPremises.Select(x => x.CandidateProviderPremisesCheckings),
                    y => y.CandidateProviderDocuments,
                    y => y.RegionAdmin,
                    y => y.LocationCorrespondence
                    );


                CandidateProviderVM candidate = candidateProviderAsVM.FirstOrDefault();
                if (candidate.IdApplicationStatus.HasValue)
                {
                    var applicationStatusValue = await this.dataSourceService.GetKeyValueByIdAsync(candidate.IdApplicationStatus.Value);
                    if (applicationStatusValue is not null)
                    {
                        candidate.ApplicationStatus = applicationStatusValue.Name;
                    }
                }

                candidate.ModifyPersonName = await applicationUserService.GetApplicationUsersPersonNameAsync(candidate.IdModifyUser);
                candidate.CreatePersonName = await applicationUserService.GetApplicationUsersPersonNameAsync(candidate.IdCreateUser);

                var listCandidateProviderPersonsIds = this.repository.AllReadonly<CandidateProviderPerson>(x => x.IdCandidate_Provider == candidate.IdCandidate_Provider && x.IsAllowedForNotification).Select(x => x.IdPerson).ToList();
                if (listCandidateProviderPersonsIds.Any())
                {
                    candidate.PersonsForNotifications = (await this.PersonService.GetPersonsByIdsAsync(listCandidateProviderPersonsIds)).ToList();
                }

                return candidate;
            }

            return null;
        }

        public async Task<IEnumerable<CandidateProviderSpecialityVM>> GetAllCandidateProviderSpecialitiesWithActualCurriculumsByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var candidateProviderSpecialitiesList = new List<CandidateProviderSpecialityVM>();
            var kvCurriculumModificationStatusFinal = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");
            var candidateProviderSpecialities = await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == idCandidateProvider).Include(x => x.Speciality.Profession.ProfessionalDirection).AsNoTracking()
                    .Include(x => x.CandidateCurriculumModifications.Where(y => y.IdModificationStatus == kvCurriculumModificationStatusFinal.IdKeyValue && y.ValidFromDate.HasValue ? y.ValidFromDate!.Value.Date <= DateTime.Now.Date : y.OldId.HasValue).OrderByDescending(x => x.ValidFromDate!.Value.Date).ThenByDescending(x => x.OldId).Take(1)).AsNoTracking()
                        .ToListAsync();
            foreach (var candidateProviderSpeciality in candidateProviderSpecialities)
            {
                var candidateProviderSpecialityAsVM = candidateProviderSpeciality.To<CandidateProviderSpecialityVM>();
                if (candidateProviderSpeciality.CandidateCurriculumModifications.FirstOrDefault() != null)
                {
                    candidateProviderSpecialityAsVM.CurriculumModificationUploadedFileName = candidateProviderSpeciality.CandidateCurriculumModifications.FirstOrDefault().UploadedFileName;
                }

                candidateProviderSpecialitiesList.Add(candidateProviderSpecialityAsVM);
            }

            return candidateProviderSpecialitiesList.OrderBy(x => x.Speciality.Code).ToList();
        }

        public async Task<IEnumerable<CandidateProviderTrainerVM>> GetCandidateProviderTrainersByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var trainers = this.repository.AllReadonly<CandidateProviderTrainer>(x => x.IdCandidate_Provider == idCandidateProvider);

            return await trainers.To<CandidateProviderTrainerVM>(y => y.CandidateProviderTrainerProfiles,
                    y => y.CandidateProviderTrainerDocuments,
                    y => y.CandidateProviderTrainerSpecialities,
                    y => y.CandidateProviderTrainerCheckings).ToListAsync();
        }

        public async Task<IEnumerable<CandidateProviderPremisesVM>> GetCandidateProviderPremisesWithAllIncludedByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var premises = this.repository.AllReadonly<CandidateProviderPremises>(x => x.IdCandidate_Provider == idCandidateProvider);

            return await premises.To<CandidateProviderPremisesVM>(y => y.CandidateProviderPremisesSpecialities,
                    y => y.CandidateProviderPremisesDocuments,
                    y => y.CandidateProviderPremisesCheckings).ToListAsync();
        }

        public async Task<CandidateProviderVM> GetActiveCandidateProviderByIdAsync(int idCandidateProvider)
        {
            IQueryable<CandidateProvider> candidateProviderFromDb = this.repository.AllReadonly<CandidateProvider>(x => x.IdCandidate_Provider == idCandidateProvider && x.IsActive);

            if (candidateProviderFromDb != null)
            {
                this.repository.Detach<CandidateProvider>(candidateProviderFromDb.FirstOrDefault());

                //var candidateProviderAsVM = candidateProviderFromDb.To<CandidateProviderVM>(
                //    y => y.CandidateProviderSpecialities.Select(x => x.CandidateCurriculums.Select(e => e.CandidateCurriculumERUs)),
                //    y => y.CandidateProviderSpecialities.Select(x => x.Speciality),
                //    y => y.CandidateProviderTrainers.Select(x => x.CandidateProviderTrainerProfiles),
                //    y => y.CandidateProviderTrainers.Select(x => x.CandidateProviderTrainerSpecialities),
                //    y => y.CandidateProviderPremises.Select(x => x.CandidateProviderPremisesSpecialities),
                //    y => y.CandidateProviderDocuments,
                //    y => y.LocationCorrespondence);

                var candidateProviderAsVM = candidateProviderFromDb.To<CandidateProviderVM>(
                    y => y.CandidateProviderSpecialities.Select(x => x.CandidateCurriculums.Select(e => e.CandidateCurriculumERUs)),
                    y => y.CandidateProviderSpecialities.Select(x => x.Speciality),
                    y => y.CandidateProviderTrainers.Select(x => x.CandidateProviderTrainerProfiles),
                    y => y.CandidateProviderTrainers.Select(x => x.CandidateProviderTrainerDocuments),
                    y => y.CandidateProviderTrainers.Select(x => x.CandidateProviderTrainerSpecialities),
                    y => y.CandidateProviderTrainers.Select(x => x.CandidateProviderTrainerCheckings),
                    y => y.CandidateProviderPremises.Select(x => x.CandidateProviderPremisesSpecialities),
                    y => y.CandidateProviderPremises.Select(x => x.CandidateProviderPremisesDocuments),
                    y => y.CandidateProviderPremises.Select(x => x.CandidateProviderPremisesCheckings),
                    y => y.CandidateProviderDocuments,
                    y => y.RegionAdmin,
                    y => y.LocationCorrespondence
                    );

                CandidateProviderVM candidate = await candidateProviderAsVM.FirstOrDefaultAsync();
                candidate.ModifyPersonName = await applicationUserService.GetApplicationUsersPersonNameAsync(candidate.IdModifyUser);
                candidate.CreatePersonName = await applicationUserService.GetApplicationUsersPersonNameAsync(candidate.IdCreateUser);

                return candidate;
            }

            return null;
        }

        public async Task<ResultContext<CandidateProviderVM>> CreateApplicationAsync(ResultContext<CandidateProviderVM> inputContext)
        {
            var candidateProviderFromDb = await this.SetApplicationDisabledFieldsValues(inputContext.ResultContextObject);

            try
            {
                var CandidateProviderSpecialitiesEntries = new List<CandidateProviderSpecialityVM>();

                foreach (var candidateProviderSpecialitiesEntry in inputContext.ResultContextObject.CandidateProviderSpecialities)
                {
                    CandidateProviderSpecialitiesEntries.Add(candidateProviderSpecialitiesEntry);
                }

                inputContext.ResultContextObject.ProviderNameEN = !string.IsNullOrEmpty(inputContext.ResultContextObject.ProviderName) ? BaseHelper.ConvertCyrToLatin(inputContext.ResultContextObject.ProviderName) : null;
                inputContext.ResultContextObject.PersonNameCorrespondenceEN = !string.IsNullOrEmpty(inputContext.ResultContextObject.PersonNameCorrespondence) ? BaseHelper.ConvertCyrToLatin(inputContext.ResultContextObject.PersonNameCorrespondence) : null;
                inputContext.ResultContextObject.ProviderAddressCorrespondenceEN = !string.IsNullOrEmpty(inputContext.ResultContextObject.ProviderAddressCorrespondence) ? BaseHelper.ConvertCyrToLatin(inputContext.ResultContextObject.ProviderAddressCorrespondence) : null;
                inputContext.ResultContextObject.IdCreateUser = candidateProviderFromDb.IdCreateUser;
                inputContext.ResultContextObject.CreationDate = candidateProviderFromDb.CreationDate;
                candidateProviderFromDb = inputContext.ResultContextObject.To<CandidateProvider>();

                await this.HandleCandidateProviderSpecialitiesTabData(CandidateProviderSpecialitiesEntries, candidateProviderFromDb, inputContext.ResultContextObject);
                await this.HandleCandidateProviderTrainers(inputContext, candidateProviderFromDb);
                await this.HandleCandidateProviderPremises(inputContext, candidateProviderFromDb);
                await this.HandlePersonsForNotificationsAsync(inputContext);

                candidateProviderFromDb.CandidateProviderSpecialities = null;
                candidateProviderFromDb.CandidateProviderTrainers = null;
                candidateProviderFromDb.CandidateProviderPremises = null;
                candidateProviderFromDb.Location = null;
                candidateProviderFromDb.LocationCorrespondence = null;

                //if (this.IsCandidateProviderModified(candidateProviderFromDb, inputContext.ResultContextObject))
                //{
                this.repository.Update<CandidateProvider>(candidateProviderFromDb);
                await this.repository.SaveChangesAsync();
                //}

                inputContext.AddMessage("Записът е успешен!");

                inputContext.ResultContextObject.CandidateProviderSpecialities = CandidateProviderSpecialitiesEntries;
                inputContext.ResultContextObject.IdModifyUser = candidateProviderFromDb.IdModifyUser;
                inputContext.ResultContextObject.IdCreateUser = candidateProviderFromDb.IdCreateUser;
                inputContext.ResultContextObject.ModifyDate = candidateProviderFromDb.ModifyDate;
                inputContext.ResultContextObject.CreationDate = candidateProviderFromDb.CreationDate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                inputContext.AddErrorMessage(ex.Message);
                inputContext.AddMessage("Грешка при запис в базата данни!");
            }

            return inputContext;
        }

        public async Task<ResultContext<CandidateProviderVM>> CreateApplicationChangeAsync(ResultContext<CandidateProviderVM> inputContext)
        {
            await this.SetApplicationDisabledFieldsValues(inputContext.ResultContextObject);

            try
            {
                inputContext.ResultContextObject.ProviderNameEN = !string.IsNullOrEmpty(inputContext.ResultContextObject.ProviderName) ? BaseHelper.ConvertCyrToLatin(inputContext.ResultContextObject.ProviderName) : null;
                inputContext.ResultContextObject.PersonNameCorrespondenceEN = !string.IsNullOrEmpty(inputContext.ResultContextObject.PersonNameCorrespondence) ? BaseHelper.ConvertCyrToLatin(inputContext.ResultContextObject.PersonNameCorrespondence) : null;
                inputContext.ResultContextObject.ProviderAddressCorrespondenceEN = !string.IsNullOrEmpty(inputContext.ResultContextObject.ProviderAddressCorrespondence) ? BaseHelper.ConvertCyrToLatin(inputContext.ResultContextObject.ProviderAddressCorrespondence) : null;
                var candidateProviderForDb = inputContext.ResultContextObject.To<CandidateProvider>();
                candidateProviderForDb.CandidateProviderSpecialities = null;
                candidateProviderForDb.CandidateProviderTrainers = null;
                candidateProviderForDb.CandidateProviderPremises = null;
                candidateProviderForDb.Location = null;
                candidateProviderForDb.LocationCorrespondence = null;
                candidateProviderForDb.IsActive = false;
                candidateProviderForDb.RegionAdmin = null;
                candidateProviderForDb.RegionCorrespondence = null;

                var CandidateProviderSpecialitiesEntries = new List<CandidateProviderSpecialityVM>();

                foreach (var candidateProviderSpecialitiesEntry in inputContext.ResultContextObject.CandidateProviderSpecialities)
                {
                    CandidateProviderSpecialitiesEntries.Add(candidateProviderSpecialitiesEntry);
                }

                var entryFromDb = this.repository.AllReadonly<CandidateProvider>(x => !x.IsActive && x.IdCandidate_Provider == candidateProviderForDb.IdCandidate_Provider).FirstOrDefault();

                if (entryFromDb is not null)
                {
                    entryFromDb.ProviderNameEN = !string.IsNullOrEmpty(inputContext.ResultContextObject.ProviderName) ? BaseHelper.ConvertCyrToLatin(inputContext.ResultContextObject.ProviderName) : null;
                    entryFromDb.PersonNameCorrespondenceEN = !string.IsNullOrEmpty(inputContext.ResultContextObject.PersonNameCorrespondence) ? BaseHelper.ConvertCyrToLatin(inputContext.ResultContextObject.PersonNameCorrespondence) : null;
                    entryFromDb.ProviderAddressCorrespondenceEN = !string.IsNullOrEmpty(inputContext.ResultContextObject.ProviderAddressCorrespondence) ? BaseHelper.ConvertCyrToLatin(inputContext.ResultContextObject.ProviderAddressCorrespondence) : null;
                    entryFromDb.IdCreateUser = entryFromDb.IdCreateUser;
                    entryFromDb.CreationDate = entryFromDb.CreationDate;

                    await this.HandleCandidateProviderSpecialitiesTabData(CandidateProviderSpecialitiesEntries, entryFromDb, inputContext.ResultContextObject);
                    await this.HandleCandidateProviderTrainers(inputContext, entryFromDb);
                    await this.HandleCandidateProviderPremises(inputContext, entryFromDb);
                    await this.HandlePersonsForNotificationsAsync(inputContext);

                    candidateProviderForDb.IdCreateUser = entryFromDb.IdCreateUser;
                    candidateProviderForDb.CreationDate = entryFromDb.CreationDate;
                    candidateProviderForDb.IdCandidateProviderActive = entryFromDb.IdCandidateProviderActive;
                    this.repository.Update<CandidateProvider>(candidateProviderForDb);
                    await this.repository.SaveChangesAsync();
                }
                else
                {
                    var kvPreparationDocs = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "PreparationDocumentationLicensing");
                    candidateProviderForDb.ProviderNameEN = !string.IsNullOrEmpty(inputContext.ResultContextObject.ProviderName) ? BaseHelper.ConvertCyrToLatin(inputContext.ResultContextObject.ProviderName) : null;
                    candidateProviderForDb.PersonNameCorrespondenceEN = !string.IsNullOrEmpty(inputContext.ResultContextObject.PersonNameCorrespondence) ? BaseHelper.ConvertCyrToLatin(inputContext.ResultContextObject.PersonNameCorrespondence) : null;
                    candidateProviderForDb.ProviderAddressCorrespondenceEN = !string.IsNullOrEmpty(inputContext.ResultContextObject.ProviderAddressCorrespondence) ? BaseHelper.ConvertCyrToLatin(inputContext.ResultContextObject.ProviderAddressCorrespondence) : null;
                    candidateProviderForDb.IdCandidate_Provider = 0;
                    candidateProviderForDb.IdCandidateProviderActive = inputContext.ResultContextObject.IdCandidate_Provider;
                    candidateProviderForDb.IdApplicationStatus = kvPreparationDocs.IdKeyValue;
                    candidateProviderForDb.UploadedFileName = string.Empty;
                    candidateProviderForDb.ApplicationDate = null;
                    candidateProviderForDb.Indent = null;
                    candidateProviderForDb.IdStartedProcedure = null;
                    candidateProviderForDb.IdApplicationFiling = null;
                    candidateProviderForDb.IdReceiveLicense = null;
                    candidateProviderForDb.CandidateProviderDocuments = null;
                    await this.repository.AddAsync<CandidateProvider>(candidateProviderForDb);
                    await this.repository.SaveChangesAsync();
                    inputContext.ResultContextObject.IdCandidate_Provider = candidateProviderForDb.IdCandidate_Provider;

                    await this.HandleCandidateProviderSpecialitiesTabData(CandidateProviderSpecialitiesEntries, candidateProviderForDb, inputContext.ResultContextObject);
                    await this.HandleCandidateProviderTrainers(inputContext, candidateProviderForDb);
                    await this.HandleCandidateProviderPremises(inputContext, candidateProviderForDb);
                    await this.HandlePersonsForNotificationsAsync(inputContext);
                }

                inputContext.AddMessage("Записът е успешен!");

                inputContext.ResultContextObject.IdModifyUser = candidateProviderForDb.IdModifyUser;
                inputContext.ResultContextObject.IdCreateUser = candidateProviderForDb.IdCreateUser;
                inputContext.ResultContextObject.ModifyDate = candidateProviderForDb.ModifyDate;
                inputContext.ResultContextObject.CreationDate = candidateProviderForDb.CreationDate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                inputContext.AddErrorMessage(ex.Message);
                inputContext.AddMessage("Грешка при запис в базата данни!");
            }

            return inputContext;
        }

        public async Task<int> CreateApplicationChangeCandidateProviderAsync(int idCandidateProvider, int idTypeApplication)
        {
            var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(idCandidateProvider);
            if (candidateProviderFromDb is not null)
            {
                var kvDocPreparationApplicationStatusValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "PreparationDocumentationLicensing");
                var candidateProviderForDb = new CandidateProvider()
                {
                    IdCandidateProviderActive = candidateProviderFromDb.IdCandidate_Provider,
                    IsActive = false,
                    ProviderOwner = candidateProviderFromDb.ProviderOwner,
                    PoviderBulstat = candidateProviderFromDb.PoviderBulstat,
                    ManagerName = candidateProviderFromDb.ManagerName,
                    AttorneyName = candidateProviderFromDb.AttorneyName,
                    IdProviderRegistration = candidateProviderFromDb.IdProviderRegistration,
                    IdProviderOwnership = candidateProviderFromDb.IdProviderOwnership,
                    IdProviderStatus = candidateProviderFromDb.IdProviderStatus,
                    IdLocation = candidateProviderFromDb.IdLocation,
                    ProviderAddress = candidateProviderFromDb.ProviderAddress,
                    ZipCode = candidateProviderFromDb.ZipCode,
                    ProviderName = candidateProviderFromDb.ProviderName,
                    IdTypeLicense = candidateProviderFromDb.IdTypeLicense,
                    ProviderPhone = candidateProviderFromDb.ProviderPhone,
                    ProviderFax = candidateProviderFromDb.ProviderFax,
                    ProviderWeb = candidateProviderFromDb.ProviderWeb,
                    ProviderEmail = candidateProviderFromDb.ProviderEmail,
                    AdditionalInfo = candidateProviderFromDb.AdditionalInfo,
                    AccessibilityInfo = candidateProviderFromDb.AccessibilityInfo,
                    PersonNameCorrespondence = candidateProviderFromDb.PersonNameCorrespondence,
                    IdLocationCorrespondence = candidateProviderFromDb.IdLocationCorrespondence,
                    ProviderAddressCorrespondence = candidateProviderFromDb.ProviderAddressCorrespondence,
                    ZipCodeCorrespondence = candidateProviderFromDb.ZipCodeCorrespondence,
                    ProviderPhoneCorrespondence = candidateProviderFromDb.ProviderPhoneCorrespondence,
                    ProviderFaxCorrespondence = candidateProviderFromDb.ProviderFaxCorrespondence,
                    ProviderEmailCorrespondence = candidateProviderFromDb.ProviderEmailCorrespondence,
                    IdApplicationStatus = kvDocPreparationApplicationStatusValue.IdKeyValue,
                    IdTypeApplication = idTypeApplication,
                    IdRegionCorrespondence = candidateProviderFromDb.IdRegionCorrespondence,
                    DirectorFamilyName = candidateProviderFromDb.DirectorFamilyName,
                    DirectorFirstName = candidateProviderFromDb.DirectorFirstName,
                    DirectorSecondName = candidateProviderFromDb.DirectorSecondName,
                    PersonNameCorrespondenceEN = candidateProviderFromDb.PersonNameCorrespondenceEN,
                    ProviderAddressCorrespondenceEN = candidateProviderFromDb.ProviderAddressCorrespondenceEN,
                    ProviderNameEN = candidateProviderFromDb.ProviderNameEN,
                    ProviderOwnerEN = candidateProviderFromDb.ProviderOwnerEN,
                    IdRegionAdmin = candidateProviderFromDb.IdRegionAdmin,
                    AdditionalDocumentRequested = false
                };

                await this.repository.AddAsync<CandidateProvider>(candidateProviderForDb);
                await this.repository.SaveChangesAsync();

                return candidateProviderForDb.IdCandidate_Provider;
            }

            return 0;
        }

        public async Task<CandidateProviderSpecialityVM> GetCandidateProviderSpecialityBySpecialityIdAndCandidateProviderIdAsync(SpecialityVM specialityVM, CandidateProviderVM candidateProviderVM)
        {
            IQueryable<CandidateProviderSpeciality> candidateProviderSpeciality = this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdSpeciality == specialityVM.IdSpeciality && x.IdCandidate_Provider == candidateProviderVM.IdCandidate_Provider);

            return await candidateProviderSpeciality.To<CandidateProviderSpecialityVM>().FirstOrDefaultAsync();
        }

        public async Task<ResultContext<CandidateProviderVM>> UpdateCandidateProviderApplicationNumber(ResultContext<CandidateProviderVM> candidateProvider)
        {
            var model = candidateProvider.ResultContextObject;
            try
            {
                var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(candidateProvider.ResultContextObject.IdCandidate_Provider);
                candidateProviderFromDb = model.To<CandidateProvider>();
                this.repository.Update<CandidateProvider>(candidateProviderFromDb);
                await this.repository.SaveChangesAsync(false);

                candidateProvider.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                candidateProvider.AddErrorMessage(ex.Message);
                candidateProvider.AddMessage("Грешка при запис в базата данни!");
            }
            return candidateProvider;
        }

        private async Task HandleCandidateProviderPremises(ResultContext<CandidateProviderVM> inputContext, CandidateProvider candidateProviderFromDb)
        {
            var premises = inputContext.ResultContextObject.CandidateProviderPremises;

            foreach (var entry in premises)
            {
                var entryForDb = entry.To<CandidateProviderPremises>();
                //var tempdata = entryForDb.CandidateProviderPremisesSpecialities;
                //entryForDb.CandidateProviderPremisesSpecialities = null;
                entryForDb.CandidateProvider = null;
                entryForDb.IdCandidate_Provider = candidateProviderFromDb.IdCandidate_Provider;


                if (entry.IdCandidateProviderPremises != 0)
                {
                    var premisesFromDb = await this.repository.GetByIdAsync<CandidateProviderPremises>(entry.IdCandidateProviderPremises);
                    if (this.IsPremisesModified(premisesFromDb, entry))
                    {
                        this.repository.Update<CandidateProviderPremises>(entryForDb);
                        await this.repository.SaveChangesAsync();
                    }
                }
                else
                {

                    await this.repository.AddAsync<CandidateProviderPremises>(entryForDb);
                    await this.repository.SaveChangesAsync();

                    entry.IdCandidateProviderPremises = entryForDb.IdCandidateProviderPremises;
                }

                //await this.HandleCandidateProviderPremisesSpecialities(tempdata, entryForDb);
            }
        }

        private async Task HandleCandidateProviderPremisesSpecialities(ICollection<CandidateProviderPremisesSpeciality> entry, CandidateProviderPremises entryForDb)
        {
            if (entry is null)
            {
                entry = new List<CandidateProviderPremisesSpeciality>();
            }
            foreach (var speciality in entry)
            {
                var specialityForDb = this.repository.AllReadonly<CandidateProviderPremisesSpeciality>(x => x.IdSpeciality == speciality.IdSpeciality && x.IdCandidateProviderPremises == entryForDb.IdCandidateProviderPremises).FirstOrDefault();
                if (specialityForDb == null)
                {
                    specialityForDb = speciality.To<CandidateProviderPremisesSpeciality>();
                    await this.repository.AddAsync<CandidateProviderPremisesSpeciality>(specialityForDb);
                    await this.repository.SaveChangesAsync();
                }
                else
                {
                    speciality.IdCandidateProviderPremisesSpeciality = specialityForDb.IdCandidateProviderPremisesSpeciality;
                    specialityForDb = speciality.To<CandidateProviderPremisesSpeciality>();
                    if (specialityForDb.IdUsage == 0 || specialityForDb.IdComplianceDOC == 0)
                    {
                    }
                    else
                    {
                        this.repository.Update<CandidateProviderPremisesSpeciality>(specialityForDb);
                        await this.repository.SaveChangesAsync();
                    }
                }
            }
        }

        // пресетва стойности на полета, които са disabled за редакция
        private async Task<CandidateProvider> SetApplicationDisabledFieldsValues(CandidateProviderVM candidateProviderVM)
        {
            var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(candidateProviderVM.IdCandidate_Provider);

            if (candidateProviderFromDb is not null)
            {
                candidateProviderVM.ProviderOwner = candidateProviderFromDb.ProviderOwner;
                candidateProviderVM.AttorneyName = candidateProviderFromDb.AttorneyName;
                candidateProviderVM.ManagerName = candidateProviderFromDb.ManagerName;
                candidateProviderVM.PoviderBulstat = candidateProviderFromDb.PoviderBulstat;
                candidateProviderVM.ProviderAddress = candidateProviderFromDb.ProviderAddress;
                candidateProviderVM.ZipCode = candidateProviderFromDb.ZipCode;
                candidateProviderVM.IdLocation = candidateProviderFromDb.IdLocation;
            }

            return candidateProviderFromDb;
        }

        private async Task HandleCandidateProviderSpecialitiesTabData(List<CandidateProviderSpecialityVM> CandidateProviderSpecialitiesEntries, CandidateProvider candidateProviderFromDb, CandidateProviderVM candidateProviderVM)
        {
            foreach (var candidateProviderSpeciality in CandidateProviderSpecialitiesEntries)
            {
                if (candidateProviderSpeciality.IdCandidateProviderSpeciality == 0)
                {
                    candidateProviderSpeciality.CandidateProvider = null;

                    CandidateProviderSpeciality newCandidateProviderSpeciality = new CandidateProviderSpeciality
                    {
                        IdSpeciality = candidateProviderSpeciality.IdSpeciality,
                        IdCandidate_Provider = candidateProviderFromDb.IdCandidate_Provider,
                        IdFormEducation = candidateProviderSpeciality.IdFormEducation,
                        IdFrameworkProgram = candidateProviderSpeciality.IdFrameworkProgram
                    };

                    await this.repository.AddAsync<CandidateProviderSpeciality>(newCandidateProviderSpeciality);
                    await this.repository.SaveChangesAsync();

                    candidateProviderSpeciality.IdCandidateProviderSpeciality = newCandidateProviderSpeciality.IdCandidateProviderSpeciality;
                    candidateProviderVM.CandidateProviderSpecialities.FirstOrDefault(x => x.IdSpeciality == candidateProviderSpeciality.IdSpeciality).IdCandidateProviderSpeciality = newCandidateProviderSpeciality.IdCandidateProviderSpeciality;

                    if (newCandidateProviderSpeciality.CandidateProvider != null)
                    {
                        this.repository.Detach<CandidateProvider>(newCandidateProviderSpeciality.CandidateProvider);
                    }
                }
                else
                {
                    var candidateProviderSpecialityFromDb = await this.repository.GetByIdAsync<CandidateProviderSpeciality>(candidateProviderSpeciality.IdCandidateProviderSpeciality);
                    if (candidateProviderSpecialityFromDb.IdFrameworkProgram.HasValue && !candidateProviderSpeciality.IdFrameworkProgram.HasValue)
                    {
                        candidateProviderSpeciality.IdFrameworkProgram = candidateProviderSpecialityFromDb.IdFrameworkProgram;
                    }

                    var isModified = this.IsCandidateProviderSpecialityModified(candidateProviderSpecialityFromDb, candidateProviderSpeciality);
                    if (isModified)
                    {
                        candidateProviderSpeciality.IdCreateUser = candidateProviderSpecialityFromDb.IdCreateUser;
                        candidateProviderSpeciality.CreationDate = candidateProviderSpecialityFromDb.CreationDate;
                        var candidateProviderSpecialityForDb = candidateProviderSpeciality.To<CandidateProviderSpeciality>();
                        candidateProviderSpecialityForDb.LicenceData = candidateProviderSpecialityFromDb.LicenceData;
                        candidateProviderSpecialityForDb.LicenceProtNo = candidateProviderSpecialityFromDb.LicenceProtNo;
                        candidateProviderSpecialityForDb.MigrationNote = candidateProviderSpecialityFromDb.MigrationNote;
                        candidateProviderSpecialityForDb.OldId = candidateProviderSpecialityFromDb.OldId;
                        candidateProviderSpecialityForDb.CandidateCurriculums = null;
                        candidateProviderSpecialityForDb.Speciality = null;
                        candidateProviderSpecialityForDb.CandidateProvider = null;

                        this.repository.Update<CandidateProviderSpeciality>(candidateProviderSpecialityForDb);
                        await this.repository.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task HandleCandidateProviderProfiles(ICollection<CandidateProviderTrainerProfileVM> trainerProfiles, CandidateProviderTrainer trainerForDb)
        {
            var trainerProfilesFromDb = this.repository.AllReadonly<CandidateProviderTrainerProfile>(x => x.IdCandidateProviderTrainer == trainerForDb.IdCandidateProviderTrainer).To<CandidateProviderTrainerProfileVM>().ToList();

            var deletedProfDirs = trainerProfilesFromDb.Where(x => trainerProfiles.All(y => y.IdCandidateProviderTrainerProfile != x.IdCandidateProviderTrainerProfile)).ToList();

            if (deletedProfDirs.Any())
            {
                foreach (var profDir in deletedProfDirs)
                {
                    var entityToDelete = profDir.To<CandidateProviderTrainerProfile>();
                    this.repository.HardDelete<CandidateProviderTrainerProfile>(entityToDelete);
                    await this.repository.SaveChangesAsync();
                }
            }

            if (trainerProfiles != null)
            {
                foreach (var profile in trainerProfiles)
                {
                    var profileForDb = profile.To<CandidateProviderTrainerProfile>();

                    if (profile.IdCandidateProviderTrainerProfile != 0)
                    {
                        var trainerProfileFromDb = await this.repository.GetByIdAsync<CandidateProviderTrainerProfile>(profile.IdCandidateProviderTrainerProfile);
                        if (this.IsTrainerProfileModified(trainerProfileFromDb, profile))
                        {
                            this.repository.Update<CandidateProviderTrainerProfile>(profileForDb);
                            await this.repository.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        profileForDb.IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer;


                        await this.repository.AddAsync<CandidateProviderTrainerProfile>(profileForDb);
                        await this.repository.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task HandleCandidateProviderTrainers(ResultContext<CandidateProviderVM> inputContext, CandidateProvider candidateProviderFromDb)
        {
            var trainers = inputContext.ResultContextObject.CandidateProviderTrainers;

            foreach (var trainer in trainers)
            {
                if (!string.IsNullOrEmpty(trainer.Indent))
                {
                    trainer.Indent = trainer.Indent.Trim();
                }

                if (trainer.Indent != null)
                {
                    trainer.Indent = trainer.Indent.Trim();
                }

                trainer.CandidateProviderTrainerQualifications = null;
                var trainerProfiles = trainer.CandidateProviderTrainerProfiles;
                trainer.CandidateProviderTrainerProfiles = null;
                var trainerSpecialities = trainer.CandidateProviderTrainerSpecialities;
                trainer.CandidateProviderTrainerSpecialities = null;
                var trainerForDb = trainer.To<CandidateProviderTrainer>();
                trainerForDb.CandidateProviderTrainerProfiles = null;
                trainerForDb.CandidateProviderTrainerQualifications = null;
                trainerForDb.CandidateProviderTrainerSpecialities = null;
                trainerForDb.CandidateProvider = null;
                trainerForDb.IdCandidate_Provider = candidateProviderFromDb.IdCandidate_Provider;

                if (trainer.IdCandidateProviderTrainer != 0)
                {
                    var trainerFromDb = await this.repository.GetByIdAsync<CandidateProviderTrainer>(trainer.IdCandidateProviderTrainer);
                    if (this.IsTrainerModified(trainerFromDb, trainer))
                    {
                        this.repository.Update<CandidateProviderTrainer>(trainerForDb);
                        await this.repository.SaveChangesAsync();
                    }
                }
                else
                {
                    await this.repository.AddAsync<CandidateProviderTrainer>(trainerForDb);
                    await this.repository.SaveChangesAsync();

                    trainer.IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer;
                }

                this.repository.Detach<CandidateProviderTrainer>(trainerForDb);

                //await this.HandleCandidateProviderProfiles(trainerProfiles, trainerForDb);
                //await this.HandleCandidateProviderTrainerSpecialities(trainer, trainerSpecialities);
            }
        }

        private async Task HandleCandidateProviderTrainerSpecialities(CandidateProviderTrainerVM trainerForDb, ICollection<CandidateProviderTrainerSpecialityVM> trainerSpecialities)
        {
            if (trainerSpecialities != null)
            {
                foreach (var speciality in trainerSpecialities)
                {
                    var specialityForDb = this.repository.AllReadonly<CandidateProviderTrainerSpeciality>(x => x.IdSpeciality == speciality.IdSpeciality && x.IdCandidateProviderTrainer == speciality.IdCandidateProviderTrainer).FirstOrDefault();
                    if (specialityForDb == null)
                    {
                        specialityForDb = speciality.To<CandidateProviderTrainerSpeciality>();

                        specialityForDb.IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer;

                        await this.repository.AddAsync<CandidateProviderTrainerSpeciality>(specialityForDb);
                        await this.repository.SaveChangesAsync();
                    }
                    else
                    {
                        speciality.IdCandidateProviderTrainerSpeciality = specialityForDb.IdCandidateProviderTrainerSpeciality;
                        specialityForDb = speciality.To<CandidateProviderTrainerSpeciality>();
                        if (specialityForDb.IdUsage == 0 || specialityForDb.IdComplianceDOC == 0)
                        {
                        }
                        else
                        {
                            await this.repository.AddAsync<CandidateProviderTrainerSpeciality>(specialityForDb);
                            await this.repository.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        private async Task HandlePersonsForNotificationsAsync(ResultContext<CandidateProviderVM> inputContext)
        {
            var data = this.repository.AllReadonly<CandidateProviderPerson>(x => x.IdCandidate_Provider == inputContext.ResultContextObject.IdCandidate_Provider).ToList();
            foreach (var entry in data)
            {
                entry.IsAllowedForNotification = false;
                this.repository.Update<CandidateProviderPerson>(entry);
            }

            await this.repository.SaveChangesAsync();

            foreach (var entry in inputContext.ResultContextObject.PersonsForNotifications)
            {
                var person = data.FirstOrDefault(x => x.IdPerson == entry.IdPerson);
                if (person is not null)
                {
                    person.IsAllowedForNotification = true;
                    this.repository.Update<CandidateProviderPerson>(person);
                }
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task<CandidateProviderVM> GetCandidateProviderWithoutAnythingIncludedByIdAsync(int id)
        {
            var data = this.repository.AllReadonly<CandidateProvider>(x => x.IdCandidate_Provider == id);

            return await data.To<CandidateProviderVM>().FirstOrDefaultAsync();
        }

        public async Task<CandidateProviderVM> GetActiveCandidateProviderWithLocationIncludedByIdAsync(int idCnadidateProvider)
        {
            var data = this.repository.AllReadonly<CandidateProvider>(x => x.IdCandidate_Provider == idCnadidateProvider);
            var candidateProvider = await data.To<CandidateProviderVM>(x => x.Location, x => x.LocationCorrespondence.Municipality.District).FirstOrDefaultAsync();

            candidateProvider.ModifyPersonName = await this.applicationUserService.GetApplicationUsersPersonNameAsync(candidateProvider.IdModifyUser);
            candidateProvider.CreatePersonName = await this.applicationUserService.GetApplicationUsersPersonNameAsync(candidateProvider.IdCreateUser);

            return candidateProvider;
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetCandidateProvidersByListIdsAsync(List<int> ids)
        {
            var data = this.repository.AllReadonly<CandidateProvider>(x => ids.Contains(x.IdCandidate_Provider));

            return await data.To<CandidateProviderVM>().ToListAsync();
        }

        public async Task<IEnumerable<CandidateProviderVM>> GetCandidateProvidersByListIdsWithIncludeAsync(List<int> ids)
        {
            var data = this.repository.AllReadonly<CandidateProvider>(x => ids.Contains(x.IdCandidate_Provider));

            return await data.To<CandidateProviderVM>(x => x.CandidateProviderPersons).ToListAsync();
        }

        public async Task<List<CandidateProviderVM>> GetCandidateProvidersAsync(List<int> idPersons)
        {
            var candidateProviderPersons = this.repository.AllReadonly<CandidateProviderPerson>(x => idPersons.Contains(x.IdPerson));
            var idCandidateProvider = candidateProviderPersons.Select(x => x.IdCandidate_Provider).Distinct().ToList();

            var candidateProviders = this.repository.AllReadonly<CandidateProvider>(x => idCandidateProvider.Contains(x.IdCandidate_Provider));
            var dataAsVM = await candidateProviders.To<CandidateProviderVM>().ToListAsync();
            return dataAsVM;
        }
        public async Task<List<CandidateProviderPersonVM>> GetCandidateProviderPerson(int? idCandidateProvider)
        {
            var data = this.repository.AllReadonly<CandidateProviderPerson>(x => x.IdCandidate_Provider == idCandidateProvider);

            return await data.To<CandidateProviderPersonVM>().ToListAsync();
        }

        //public async Task<string> ApproveChangedApplicationAsync(CandidateProviderVM candidateProviderVM)
        //{
        //    string msg = string.Empty;
        //    var activeCandidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(candidateProviderVM.IdCandidateProviderActive);

        //    try
        //    {
        //        candidateProviderVM.IdCreateUser = activeCandidateProviderFromDb.IdCreateUser;
        //        candidateProviderVM.CreationDate = activeCandidateProviderFromDb.CreationDate;
        //        candidateProviderVM.IdTypeApplication = activeCandidateProviderFromDb.IdTypeApplication;
        //        candidateProviderVM.IsActive = activeCandidateProviderFromDb.IsActive;
        //        activeCandidateProviderFromDb = candidateProviderVM.To<CandidateProvider>();
        //        activeCandidateProviderFromDb.IdCandidateProviderActive = null;
        //        activeCandidateProviderFromDb.IdCandidate_Provider = candidateProviderVM.IdCandidateProviderActive.Value;
        //        activeCandidateProviderFromDb.Location = null;
        //        activeCandidateProviderFromDb.LocationCorrespondence = null;
        //        activeCandidateProviderFromDb.CandidateProviderActive = null;
        //        activeCandidateProviderFromDb.CandidateProviderDocuments = null;
        //        activeCandidateProviderFromDb.CandidateProviderPeople = null;
        //        activeCandidateProviderFromDb.CandidateProviderPremises = null;
        //        activeCandidateProviderFromDb.CandidateProviderSpecialities = null;
        //        activeCandidateProviderFromDb.CandidateProviderStatuses = null;
        //        activeCandidateProviderFromDb.CandidateProviderStatuses = null;
        //        activeCandidateProviderFromDb.CandidateProviderTrainers = null;

        //        this.repository.Update<CandidateProvider>(activeCandidateProviderFromDb);
        //        await this.repository.SaveChangesAsync();

        //        var candidateProviderSpecialitiesFromDb = this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == candidateProviderVM.IdCandidate_Provider);
        //        if (candidateProviderSpecialitiesFromDb.Any())
        //        {
        //            foreach (var entry in candidateProviderSpecialitiesFromDb)
        //            {
        //                var candidateCurriculums = this.repository.AllReadonly<CandidateCurriculum>(x => x.IdCandidateProviderSpeciality == entry.IdCandidateProviderSpeciality);

        //                entry.IdCandidate_Provider = activeCandidateProviderFromDb.IdCandidate_Provider;
        //                entry.IdCandidateProviderSpeciality = 0;

        //                await this.repository.AddAsync<CandidateProviderSpeciality>(entry);
        //                await this.repository.SaveChangesAsync();

        //                if (candidateCurriculums.Any())
        //                {
        //                    foreach (var candidateCurriculum in candidateCurriculums)
        //                    {
        //                        candidateCurriculum.IdCandidateProviderSpeciality = entry.IdCandidateProviderSpeciality;

        //                        this.repository.Update<CandidateCurriculum>(candidateCurriculum);
        //                    }

        //                    await this.repository.SaveChangesAsync();
        //                }
        //            }

        //        }

        //        var candidateProviderDocumentsFromDb = this.repository.AllReadonly<CandidateProviderDocument>(x => x.IdCandidateProvider == candidateProviderVM.IdCandidate_Provider);
        //        if (candidateProviderDocumentsFromDb.Any())
        //        {
        //            foreach (var entry in candidateProviderDocumentsFromDb)
        //            {
        //                entry.IdCandidateProvider = activeCandidateProviderFromDb.IdCandidate_Provider;
        //                entry.IdCandidateProviderDocument = 0;

        //                await this.repository.AddAsync<CandidateProviderDocument>(entry);
        //            }

        //            await this.repository.SaveChangesAsync();
        //        }

        //        var candidateProviderPremisesFromDb = this.repository.AllReadonly<CandidateProviderPremises>(x => x.IdCandidate_Provider == candidateProviderVM.IdCandidate_Provider);
        //        if (candidateProviderPremisesFromDb.Any())
        //        {
        //            foreach (var entry in candidateProviderPremisesFromDb)
        //            {
        //                var candidateProviderPremisesDocuments = this.repository.AllReadonly<CandidateProviderPremisesDocument>(x => x.IdCandidateProviderPremises == entry.IdCandidateProviderPremises);
        //                var candidateProviderPremisesRooms = this.repository.AllReadonly<CandidateProviderPremisesRoom>(x => x.IdCandidateProviderPremises == entry.IdCandidateProviderPremises);
        //                var candidateProviderPremisesSpecialities = this.repository.AllReadonly<CandidateProviderPremisesSpeciality>(x => x.IdCandidateProviderPremises == entry.IdCandidateProviderPremises);

        //                entry.IdCandidate_Provider = activeCandidateProviderFromDb.IdCandidate_Provider;
        //                entry.IdCandidateProviderPremises = 0;

        //                await this.repository.AddAsync<CandidateProviderPremises>(entry);
        //                await this.repository.SaveChangesAsync();

        //                if (candidateProviderPremisesDocuments.Any())
        //                {
        //                    foreach (var candidateProviderPremisesDoc in candidateProviderPremisesDocuments)
        //                    {
        //                        candidateProviderPremisesDoc.IdCandidateProviderPremises = entry.IdCandidateProviderPremises;

        //                        this.repository.Update<CandidateProviderPremisesDocument>(candidateProviderPremisesDoc);
        //                    }

        //                    await this.repository.SaveChangesAsync();
        //                }

        //                if (candidateProviderPremisesRooms.Any())
        //                {
        //                    foreach (var candidateProviderPremisesRoom in candidateProviderPremisesRooms)
        //                    {
        //                        candidateProviderPremisesRoom.IdCandidateProviderPremises = entry.IdCandidateProviderPremises;

        //                        this.repository.Update<CandidateProviderPremisesRoom>(candidateProviderPremisesRoom);
        //                    }

        //                    await this.repository.SaveChangesAsync();
        //                }

        //                if (candidateProviderPremisesSpecialities.Any())
        //                {
        //                    foreach (var candidateProviderPremisesSpeciality in candidateProviderPremisesSpecialities)
        //                    {
        //                        candidateProviderPremisesSpeciality.IdCandidateProviderPremises = entry.IdCandidateProviderPremises;

        //                        this.repository.Update<CandidateProviderPremisesSpeciality>(candidateProviderPremisesSpeciality);
        //                    }

        //                    await this.repository.SaveChangesAsync();
        //                }
        //            }
        //        }

        //        var candidateProviderTrainersFromDb = this.repository.AllReadonly<CandidateProviderTrainer>(x => x.IdCandidate_Provider == candidateProviderVM.IdCandidate_Provider);
        //        if (candidateProviderTrainersFromDb.Any())
        //        {
        //            foreach (var entry in candidateProviderTrainersFromDb)
        //            {
        //                var candidateProviderTrainerDocuments = this.repository.AllReadonly<CandidateProviderTrainerDocument>(x => x.IdCandidateProviderTrainer == entry.IdCandidateProviderTrainer);
        //                var candidateProviderTrainerProfiles = this.repository.AllReadonly<CandidateProviderTrainerProfile>(x => x.IdCandidateProviderTrainer == entry.IdCandidateProviderTrainer);
        //                var candidateProviderTrainerQualifications = this.repository.AllReadonly<CandidateProviderTrainerQualification>(x => x.IdCandidateProviderTrainer == entry.IdCandidateProviderTrainer);
        //                var candidateProviderTrainerSpecialities = this.repository.AllReadonly<CandidateProviderTrainerSpeciality>(x => x.IdCandidateProviderTrainer == entry.IdCandidateProviderTrainer);

        //                entry.IdCandidate_Provider = activeCandidateProviderFromDb.IdCandidate_Provider;
        //                entry.IdCandidateProviderTrainer = 0;

        //                await this.repository.AddAsync<CandidateProviderTrainer>(entry);
        //                await this.repository.SaveChangesAsync();

        //                if (candidateProviderTrainerDocuments.Any())
        //                {
        //                    foreach (var candidateProviderTrainerDocument in candidateProviderTrainerDocuments)
        //                    {
        //                        candidateProviderTrainerDocument.IdCandidateProviderTrainer = entry.IdCandidateProviderTrainer;

        //                        this.repository.Update<CandidateProviderTrainerDocument>(candidateProviderTrainerDocument);
        //                    }

        //                    await this.repository.SaveChangesAsync();
        //                }

        //                if (candidateProviderTrainerProfiles.Any())
        //                {
        //                    foreach (var candidateProviderTrainerProfile in candidateProviderTrainerProfiles)
        //                    {
        //                        candidateProviderTrainerProfile.IdCandidateProviderTrainer = entry.IdCandidateProviderTrainer;

        //                        this.repository.Update<CandidateProviderTrainerProfile>(candidateProviderTrainerProfile);
        //                    }

        //                    await this.repository.SaveChangesAsync();
        //                }

        //                if (candidateProviderTrainerQualifications.Any())
        //                {
        //                    foreach (var candidateProviderTrainerQualification in candidateProviderTrainerQualifications)
        //                    {
        //                        candidateProviderTrainerQualification.IdCandidateProviderTrainer = entry.IdCandidateProviderTrainer;

        //                        this.repository.Update<CandidateProviderTrainerQualification>(candidateProviderTrainerQualification);
        //                    }

        //                    await this.repository.SaveChangesAsync();
        //                }

        //                if (candidateProviderTrainerSpecialities.Any())
        //                {
        //                    foreach (var candidateProviderTrainerSpeciality in candidateProviderTrainerSpecialities)
        //                    {
        //                        candidateProviderTrainerSpeciality.IdCandidateProviderTrainer = entry.IdCandidateProviderTrainer;

        //                        this.repository.Update<CandidateProviderTrainerSpeciality>(candidateProviderTrainerSpeciality);
        //                    }

        //                    await this.repository.SaveChangesAsync();
        //                }
        //            }

        //        }

        //        msg = "Записът е успешен!";
        //    }
        //    catch (Exception ex)
        //    {
        //        msg = "Грешка при запис в базата данни!";
        //        _logger.LogError(ex.Message);
        //        _logger.LogError(ex.InnerException?.Message);
        //        _logger.LogError(ex.StackTrace);
        //    }

        //    return msg;
        //}

        public async Task<string> ApproveChangedApplicationAsync(CandidateProviderVM candidateProviderVM, ProcedureDocumentVM procedureDocument)
        {
            string msg = string.Empty;
            var activeCandidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(candidateProviderVM.IdCandidate_Provider);

            try
            {
                var kvLicenseActiveValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicenseStatus", "active");

                var context = await this.CreateCandidateProviderLicenceChangeEntryAfterNAPOOApproveAsync(procedureDocument, activeCandidateProviderFromDb, kvLicenseActiveValue, candidateProviderVM.IdTypeApplication!.Value);

                if (context.HasErrorMessages)
                {
                    msg = string.Join(Environment.NewLine, context.ListErrorMessages);
                    return msg;
                }

                await this.CreateCandidateProviderHistoryEntryAfterNAPOOApproveAsync(activeCandidateProviderFromDb);

                var kvProcedureCompletedValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "ProcedureCompleted");
                candidateProviderVM.IdCreateUser = activeCandidateProviderFromDb.IdCreateUser;
                candidateProviderVM.CreationDate = activeCandidateProviderFromDb.CreationDate;
                candidateProviderVM.IdTypeApplication = activeCandidateProviderFromDb.IdTypeApplication;
                candidateProviderVM.IsActive = true;
                candidateProviderVM.IdStartedProcedure = null;
                candidateProviderVM.IdApplicationStatus = null;
                candidateProviderVM.IdLicenceStatus = kvLicenseActiveValue.IdKeyValue;
                activeCandidateProviderFromDb = candidateProviderVM.To<CandidateProvider>();
                activeCandidateProviderFromDb.IdCandidateProviderActive = null;
                //activeCandidateProviderFromDb.IdCandidate_Provider = candidateProviderVM.IdCandidateProviderActive.Value;
                activeCandidateProviderFromDb.Location = null;
                activeCandidateProviderFromDb.LocationCorrespondence = null;
                activeCandidateProviderFromDb.CandidateProviderActive = null;
                activeCandidateProviderFromDb.CandidateProviderDocuments = null;
                activeCandidateProviderFromDb.CandidateProviderPersons = null;
                activeCandidateProviderFromDb.CandidateProviderPremises = null;
                activeCandidateProviderFromDb.CandidateProviderSpecialities = null;
                activeCandidateProviderFromDb.CandidateProviderStatuses = null;
                activeCandidateProviderFromDb.CandidateProviderStatuses = null;
                activeCandidateProviderFromDb.CandidateProviderTrainers = null;

                this.repository.Update<CandidateProvider>(activeCandidateProviderFromDb);
                await this.repository.SaveChangesAsync(false);

                // добавя данни за дата на лицензиране на специалностите към ЦПО
                var kvLicenceTypeCPOValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");
                if (activeCandidateProviderFromDb.IdTypeLicense == kvLicenceTypeCPOValue.IdKeyValue)
                {
                    await this.SetCandidateProviderSpecialityLicenceDataAsync(activeCandidateProviderFromDb, procedureDocument);
                }

                // слага роли на лицензиран ЦПО/ЦИПО след одобрена процедура по лицензиране
                await this.SetLicensedRoleToCandidateProviderUsersAfterProcedureCompletedAsync(activeCandidateProviderFromDb);

                msg = "Записът е успешен!";
            }
            catch (Exception ex)
            {
                msg = "Грешка при запис в базата данни!";
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return msg;
        }

        // добавя данни за дата на лицензиране на специалностите към ЦПО
        private async Task SetCandidateProviderSpecialityLicenceDataAsync(CandidateProvider activeCandidateProviderFromDb, ProcedureDocumentVM application19)
        {
            var specialities = await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == activeCandidateProviderFromDb.IdCandidate_Provider).ToListAsync();
            foreach (var speciality in specialities)
            {
                speciality.LicenceData = activeCandidateProviderFromDb.LicenceDate;
                if (!string.IsNullOrEmpty(application19.DS_OFFICIAL_DocNumber) && application19.DS_OFFICIAL_DATE.HasValue)
                {
                    speciality.LicenceProtNo = $"№ {application19.DS_OFFICIAL_DocNumber}/{application19.DS_OFFICIAL_DATE.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
                }

                this.repository.Update<CandidateProviderSpeciality>(speciality);
            }

            await this.repository.SaveChangesAsync(false);
        }

        // слага роли на всички потребители на CandidateProvider на лицензиран ЦПО/ЦИПО след одобрена процедура по лицензиране
        private async Task SetLicensedRoleToCandidateProviderUsersAfterProcedureCompletedAsync(CandidateProvider activeCandidateProviderFromDb)
        {
            var kvLicenceTypeCPOValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");
            var personIds = await this.repository.AllReadonly<CandidateProviderPerson>(x => x.IdCandidate_Provider == activeCandidateProviderFromDb.IdCandidate_Provider).Select(x => x.IdPerson).ToListAsync();
            var listCandidateProviderUsernames = await this.repository.AllReadonly<ApplicationUser>(x => personIds.Contains(x.IdPerson!.Value)).ToListAsync();
            var licensedCPORole = (await this.settingService.GetSettingByIntCodeAsync("LicensedCPO")).SettingIntCode;
            var licensedCIPORole = (await this.settingService.GetSettingByIntCodeAsync("LicensedCIPO")).SettingIntCode;
            foreach (var user in listCandidateProviderUsernames)
            {
                if (activeCandidateProviderFromDb.IdTypeLicense == kvLicenceTypeCPOValue.IdKeyValue)
                {
                    await this.userManager.AddToRoleAsync(user, licensedCPORole);
                    continue;
                }

                await this.userManager.AddToRoleAsync(user, licensedCIPORole);
            }
        }

        private async Task CreateCandidateProviderHistoryEntryAfterNAPOOApproveAsync(CandidateProvider activeCandidateProviderFromDb)
        {
            var kvProcedureCompleted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "ProcedureCompleted");
            int idActiveCandidateProvider = activeCandidateProviderFromDb.IdCandidate_Provider;
            var candidateProviderForHistory = activeCandidateProviderFromDb.To<CandidateProvider>();
            candidateProviderForHistory.IsActive = false;
            candidateProviderForHistory.IdCandidateProviderActive = idActiveCandidateProvider;
            candidateProviderForHistory.IdStartedProcedure = activeCandidateProviderFromDb.IdStartedProcedure;
            candidateProviderForHistory.IdCandidate_Provider = 0;
            candidateProviderForHistory.IdApplicationStatus = kvProcedureCompleted.IdKeyValue;

            await this.repository.AddAsync<CandidateProvider>(candidateProviderForHistory);
            await this.repository.SaveChangesAsync();
        }

        private async Task<ResultContext<NoResult>> CreateCandidateProviderLicenceChangeEntryAfterNAPOOApproveAsync(ProcedureDocumentVM procedureDocument, CandidateProvider activeCandidateProviderFromDb, KeyValueVM kvLicenseActiveValue, int idTypeApplication)
        {
            var errorContext = new ResultContext<NoResult>();

            var kvCIPOFirstLicenzing = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "FirstLicensingCIPO");
            var kvCPOFirstLicenzing = await this.dataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "FirstLicenzing");
            var documentData = await this.docuService.GetIdentDocument(procedureDocument.DS_OFFICIAL_DocNumber, 0, procedureDocument.DS_OFFICIAL_DATE.Value);
            
            var resultContext = await this.docuService.GetDocumentAsync(documentData.DocIdent.First().DocID, documentData.DocIdent.First().GUID);
            
            if (resultContext.HasMessages)
            {
                
                errorContext.ListErrorMessages = resultContext.ListErrorMessages;

                return errorContext;
            }

            var documentResponse = resultContext.ResultContextObject;

            var idLicenceStatusDetail = (idTypeApplication == kvCIPOFirstLicenzing.IdKeyValue || idTypeApplication == kvCPOFirstLicenzing.IdKeyValue)
                ? (await this.dataSourceService.GetKeyValueByIntCodeAsync("LicenceStatusDetail", "LicenceStatus1")).IdKeyValue
                : (await this.dataSourceService.GetKeyValueByIntCodeAsync("LicenceStatusDetail", "LicenceStatus2")).IdKeyValue;
            CandidateProviderLicenceChange candidateProviderLicenceChange = new CandidateProviderLicenceChange()
            {
                IdCandidate_Provider = activeCandidateProviderFromDb.IdCandidate_Provider,
                IdStatus = kvLicenseActiveValue.IdKeyValue,
                IdLicenceStatusDetail = idLicenceStatusDetail,
                DS_OFFICIAL_ID = documentResponse.Doc.DocID,
                DS_OFFICIAL_GUID = documentResponse.Doc.GUID,
                DS_OFFICIAL_DATE = documentResponse.Doc.DocDate,
                DS_OFFICIAL_DocNumber = documentResponse.Doc.DocNumber
            };

            await this.repository.AddAsync<CandidateProviderLicenceChange>(candidateProviderLicenceChange);
            await this.repository.SaveChangesAsync();

            return errorContext;
        }

        public async Task<string> SetCandidateProviderUINValueAsync(CandidateProviderVM candidateProviderVM)
        {
            string msg = string.Empty;

            try
            {
                var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(candidateProviderVM.IdCandidate_Provider);

                candidateProviderFromDb.UIN = await this.GetSequenceNextValue("LicenceApplicationUIN");

                this.repository.Update<CandidateProvider>(candidateProviderFromDb);
                await this.repository.SaveChangesAsync(false);

                candidateProviderVM.UIN = candidateProviderFromDb.UIN;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return msg;
        }

        public async Task<IEnumerable<CandidateProviderSpecialityVM>> GetAllCandidateProviderSpecialitiesByIdCandidateProvider(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == idCandidateProvider);
            var dataVM = await data.To<CandidateProviderSpecialityVM>(x => x.Speciality).ToListAsync();
            var kvSource = dataSourceService.GetAllKeyValueList();
            foreach (var item in dataVM)
            {
                string VQS_Name = kvSource.FirstOrDefault(c => c.IdKeyValue == item.Speciality.IdVQS).Name;
                item.Speciality.Profession = await this.ProfessionService.GetProfessionByIdAsync(new ProfessionVM() { IdProfession = item.Speciality.IdProfession });
                item.Speciality.CodeAndNameAndVQS = item.Speciality.CodeAndName + " - " + VQS_Name;
                item.Speciality.CodeAndNameProfession = $"{item.Speciality.Profession.Code} {item.Speciality.Profession.Name}";
                item.CandidateProvider = await this.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = item.IdCandidate_Provider });
            }
            return dataVM.OrderBy(c => c.Speciality.Code).ToList();
        }

        public async Task<IEnumerable<CandidateProviderSpecialityVM>> GetProviderSpecialitiesWithProfessionIncludedByIdCandidateProviderAsync(int idCandidateProvider)
        {
            return await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == idCandidateProvider).To<CandidateProviderSpecialityVM>(x => x.Speciality.Profession).ToListAsync();
        }

        public async Task<IEnumerable<CandidateProviderSpecialityVM>> GetProviderSpecialitiesWithoutIncludesByIdCandidateProviderAsync(int idCandidateProvider)
        {
            return await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == idCandidateProvider).To<CandidateProviderSpecialityVM>().ToListAsync();
        }

        public async Task<IEnumerable<CandidateProviderVM>> FilterCandidateProvidersAsync(NAPOOCandidateProviderFilterVM nAPOOCandidateProviderFilterVM, string filterType, bool licenceDeactivated)
        {
            var keyValue = dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", filterType).Result;
            var kvLicenceActiveValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicenseStatus", "active");
            var candidateProvidersAsVM = licenceDeactivated
                ? await this.repository.AllReadonly<CandidateProvider>(x => x.IdLicenceStatus.HasValue && x.IdLicenceStatus.Value != kvLicenceActiveValue.IdKeyValue && x.IsActive && x.IdTypeLicense == keyValue.IdKeyValue).To<CandidateProviderVM>(x => x.CandidateProviderSpecialities.Select(y => y.Speciality.Profession)).ToListAsync()
                : await this.repository.AllReadonly<CandidateProvider>(x => x.IdLicenceStatus.HasValue && x.IdLicenceStatus.Value == kvLicenceActiveValue.IdKeyValue && x.IsActive && x.IdTypeLicense == keyValue.IdKeyValue).To<CandidateProviderVM>(x => x.CandidateProviderSpecialities.Select(y => y.Speciality.Profession)).ToListAsync();

            if (nAPOOCandidateProviderFilterVM.IdCandidateProvider != 0 && nAPOOCandidateProviderFilterVM.IdCandidateProvider != null)
            {
                candidateProvidersAsVM = candidateProvidersAsVM.Where(x => x.IdCandidate_Provider == nAPOOCandidateProviderFilterVM.IdCandidateProvider).ToList();
            }

            if (!string.IsNullOrEmpty(nAPOOCandidateProviderFilterVM.Bulstat))
            {
                candidateProvidersAsVM = candidateProvidersAsVM.Where(x => x.PoviderBulstat.ToLower().Contains(nAPOOCandidateProviderFilterVM.Bulstat.ToLower())).ToList();
            }

            if (nAPOOCandidateProviderFilterVM.IdLocationAdmin != 0)
            {
                candidateProvidersAsVM = candidateProvidersAsVM.Where(x => x.IdLocation == nAPOOCandidateProviderFilterVM.IdLocationAdmin).ToList();
            }

            if (nAPOOCandidateProviderFilterVM.IdLocationCorrespondence != 0)
            {
                candidateProvidersAsVM = candidateProvidersAsVM.Where(x => x.IdLocationCorrespondence == nAPOOCandidateProviderFilterVM.IdLocationCorrespondence).ToList();
            }

            if (!string.IsNullOrEmpty(nAPOOCandidateProviderFilterVM.LicenceNumber))
            {
                candidateProvidersAsVM = candidateProvidersAsVM.Where(x => !string.IsNullOrEmpty(x.LicenceNumber) && x.LicenceNumber.ToLower().Contains(nAPOOCandidateProviderFilterVM.LicenceNumber.ToLower())).ToList();
            }

            if (nAPOOCandidateProviderFilterVM.LicenceDateFrom.HasValue)
            {
                candidateProvidersAsVM = candidateProvidersAsVM.Where(x => x.LicenceDate.HasValue ? x.LicenceDate.Value >= nAPOOCandidateProviderFilterVM.LicenceDateFrom.Value : x.LicenceDate >= nAPOOCandidateProviderFilterVM.LicenceDateFrom.Value).ToList();
            }

            if (nAPOOCandidateProviderFilterVM.LicenceDateTo.HasValue)
            {
                candidateProvidersAsVM = candidateProvidersAsVM.Where(x => x.LicenceDate.HasValue ? x.LicenceDate.Value <= nAPOOCandidateProviderFilterVM.LicenceDateTo.Value : x.LicenceDate <= nAPOOCandidateProviderFilterVM.LicenceDateTo.Value).ToList();
            }

            if (nAPOOCandidateProviderFilterVM.LicensedSpecialities != null)
            {
                if (nAPOOCandidateProviderFilterVM.LicensedSpecialities.Any())
                {
                    candidateProvidersAsVM = candidateProvidersAsVM.Where(x => nAPOOCandidateProviderFilterVM.LicensedSpecialities.All(y => x.CandidateProviderSpecialities.Any(c => c.IdSpeciality == y.IdSpeciality))).ToList();
                }
            }
            if (nAPOOCandidateProviderFilterVM.IdProfession.HasValue)
            { 
                candidateProvidersAsVM = candidateProvidersAsVM.Where(y => y.CandidateProviderSpecialities.Any(z => z.Speciality.IdProfession == nAPOOCandidateProviderFilterVM.IdProfession.Value)).ToList();
            }

            return candidateProvidersAsVM;
        }

        public async Task UpdateCandidateProvider(CandidateProvider candidateProvider)
        {
            this.repository.Update<CandidateProvider>(candidateProvider);
            await this.repository.SaveChangesAsync(false);

        }

        public async Task<ResultContext<NoResult>> UpdateCandidateProviderAsync(ResultContext<CandidateProviderVM> inputContext)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var model = inputContext.ResultContextObject;
                var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(model.IdCandidate_Provider);
                if (candidateProviderFromDb is not null)
                {
                    candidateProviderFromDb = model.To<CandidateProvider>();
                    candidateProviderFromDb.CandidateProviderActive = null;
                    candidateProviderFromDb.CandidateProviderCIPOStructureAndActivities = null;
                    candidateProviderFromDb.CandidateProviderCPOStructureAndActivities = null;
                    candidateProviderFromDb.CandidateProviderConsultings = null;
                    candidateProviderFromDb.CandidateProviderDocuments = null;
                    candidateProviderFromDb.CandidateProviderPersons = null;
                    candidateProviderFromDb.CandidateProviderPremises = null;
                    candidateProviderFromDb.CandidateProviderSpecialities = null;
                    candidateProviderFromDb.CandidateProviderStatuses = null;
                    candidateProviderFromDb.CandidateProviderTrainers = null;
                    candidateProviderFromDb.Courses = null;
                    candidateProviderFromDb.Location = null;
                    candidateProviderFromDb.LocationCorrespondence = null;
                    candidateProviderFromDb.Programs = null;
                    candidateProviderFromDb.ProviderDocumentOffers = null;
                    candidateProviderFromDb.ProviderRequestDocuments = null;
                    candidateProviderFromDb.SelfAssessmentReports = null;
                    candidateProviderFromDb.StartedProcedure = null;
                    candidateProviderFromDb.ValidationClients = null;
                    candidateProviderFromDb.AnnualInfos = null;
                    candidateProviderFromDb.RegionAdmin = null;
                    candidateProviderFromDb.RegionCorrespondence = null;

                    this.repository.Update<CandidateProvider>(candidateProviderFromDb);
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

        public async Task UpdateCandidateProviderForAdditionalDocumentsRequestedAsync(CandidateProviderVM candidateProvider)
        {
            var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(candidateProvider.IdCandidate_Provider);
            candidateProviderFromDb.AdditionalDocumentRequested = candidateProvider.AdditionalDocumentRequested;
            this.repository.Update<CandidateProvider>(candidateProviderFromDb);
            await this.repository.SaveChangesAsync(false);

        }

        public async Task SetCandidateProviderLicenseNumberAsync(CandidateProviderVM candidateProvider)
        {
            var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(candidateProvider.IdCandidate_Provider);

            if (string.IsNullOrEmpty(candidateProviderFromDb.LicenceNumber))
            {
                try
                {
                    candidateProviderFromDb.LicenceNumber = candidateProvider.LicenceNumber;

                    this.repository.Update<CandidateProvider>(candidateProviderFromDb);
                    await this.repository.SaveChangesAsync(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException?.Message);
                    _logger.LogError(ex.StackTrace);
                }
            }
        }

        public async Task SetCandidateProviderLicenseDateAsync(CandidateProviderVM candidateProvider)
        {
            var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(candidateProvider.IdCandidate_Provider);

            if (candidateProviderFromDb.LicenceDate == null)
            {
                try
                {
                    candidateProviderFromDb.LicenceDate = candidateProvider.LicenceDate;

                    this.repository.Update<CandidateProvider>(candidateProviderFromDb);
                    await this.repository.SaveChangesAsync(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException?.Message);
                    _logger.LogError(ex.StackTrace);
                }
            }
        }

        public async Task<ResultContext<NoResult>> DeleteCandidateProviderSpecialityByIdAsync(int idCandidateProviderSpeciality)
        {
            var resultContext = new ResultContext<NoResult>();

            try
            {
                var modification = await this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == idCandidateProviderSpeciality).FirstOrDefaultAsync();
                if (modification is not null)
                {
                    if (!string.IsNullOrEmpty(modification.UploadedFileName))
                    {
                        var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                        var directory = settingResource + "\\" + "UploadedFiles" + "\\" + "CurriculumModification" + "\\" + modification.IdCandidateCurriculumModification;
                        if (Directory.Exists(directory))
                        {
                            Directory.Delete(directory, true);
                        }
                    }

                    await this.repository.HardDeleteAsync<CandidateCurriculumModification>(modification.IdCandidateCurriculumModification);
                    await this.repository.SaveChangesAsync();
                }

                await this.repository.HardDeleteAsync<CandidateProviderSpeciality>(idCandidateProviderSpeciality);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е изтрит успешно!");
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

        public async Task<bool> DoesApplicationChangeOnStatusDifferentFromProcedureCompletedExistAsync(int idCandidateProvider)
        {
            var applicationStatusesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");
            var appStatusIds = applicationStatusesSource.Where(x => x.KeyValueIntCode == "PreparationDocumentationLicensing"
                || x.KeyValueIntCode == "RequestedByCPOOrCIPO"
                || x.KeyValueIntCode == "ProcedureWasRegisteredInKeepingSystem"
                || x.KeyValueIntCode == "AdministrativeCheck"
                || x.KeyValueIntCode == "LeadingExpertGavePositiveAssessment"
                || x.KeyValueIntCode == "LeadingExpertGaveNegativeAssessment"
                || x.KeyValueIntCode == "CorrectionApplication"
                || x.KeyValueIntCode == "LicensingExpertiseStarted"
                || x.KeyValueIntCode == "ExpertCommissionAssessment")
                    .Select(x => x.IdKeyValue).ToList();
            var data = this.repository.AllReadonly<CandidateProvider>(x => !x.IsActive && x.IdCandidateProviderActive == idCandidateProvider && x.IdApplicationStatus.HasValue && appStatusIds.Contains(x.IdApplicationStatus!.Value));

            return await data.AnyAsync();
        }

        public List<int> GetCandidateProviderIdsBySpecialityIdsAndByIsActive(List<int> specialityIds)
        {
            var providerIds = new List<int>();

            var candidateProviderSpecialities = this.repository.AllReadonly<CandidateProviderSpeciality>(x => specialityIds.Contains(x.IdSpeciality));
            var candidateProviderIds = candidateProviderSpecialities.Select(x => x.IdCandidate_Provider);
            var candidateProvidersActive = this.repository.AllReadonly<CandidateProvider>(x => candidateProviderIds.Contains(x.IdCandidate_Provider) && x.IsActive);
            providerIds.AddRange(candidateProvidersActive.Select(x => x.IdCandidate_Provider));

            return providerIds;
        }

        public MemoryStream CreateExcelApplicationValidationErrors(ResultContext<CandidateProviderExcelVM> inputContext)
        {
            var model = inputContext.ResultContextObject;
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 70;
                sheet.Range["A1"].Text = "Вид на грешката";
                sheet.Range["B1"].ColumnWidth = 170;
                sheet.Range["B1"].Text = "Описание на грешката";

                IRange range = sheet.Range["A1"];
                IRichTextString boldText = range.RichText;
                IFont boldFont = workbook.CreateFont();

                boldFont.Bold = true;
                boldText.SetFont(0, sheet.Range["A1"].Text.Length, boldFont);

                IRange range2 = sheet.Range["B1"];
                IRichTextString boldText2 = range2.RichText;
                IFont boldFont2 = workbook.CreateFont();

                boldFont2.Bold = true;
                boldText2.SetFont(0, sheet.Range["B1"].Text.Length, boldFont2);

                var rowCounter = 2;
                if (!model.ProviderRegistrationHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Обучаваща институция'";
                    sheet.Range[$"B{rowCounter++}"].Text = "В полето 'Регистрирано в' няма попълнена стойност!";
                }

                if (!model.ProviderOwnershipHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Обучаваща институция'";
                    sheet.Range[$"B{rowCounter++}"].Text = "В полето 'Форма на собственост' няма попълнена стойност!";
                }

                if (!model.ProviderEmailHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Обучаваща институция'";
                    sheet.Range[$"B{rowCounter++}"].Text = "В полето 'E-mail адрес' няма попълнена стойност!";
                }

                if (!model.ProviderNameHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Обучаваща институция'";
                    if (model.ProviderType == "CPO")
                    {
                        sheet.Range[$"B{rowCounter++}"].Text = "В полето 'Име на ЦПО' няма попълнена стойност!";
                    }
                    else
                    {
                        sheet.Range[$"B{rowCounter++}"].Text = "В полето 'Име на ЦИПО' няма попълнена стойност!";
                    }
                }

                if (!model.ProviderStatusHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Обучаваща институция'";
                    sheet.Range[$"B{rowCounter++}"].Text = "В полето 'Вид на обучаващата институция' няма попълнена стойност!";
                }

                if (!model.LocationCorrespondenceHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Обучаваща институция'";
                    if (model.ProviderType == "CPO")
                    {
                        sheet.Range[$"B{rowCounter++}"].Text = "В полето 'Населено място на административния офис на ЦПО' няма попълнена стойност!";
                    }
                    else
                    {
                        sheet.Range[$"B{rowCounter++}"].Text = "В полето 'Населено място на административния офис на ЦИПО' няма попълнена стойност!";
                    }
                }

                if (!model.ZipCodeCorrespondenceHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Обучаваща институция'";
                    sheet.Range[$"B{rowCounter++}"].Text = "В полето 'Пощ. код' няма попълнена стойност!";
                }

                if (!model.PersonNameCorrespondenceHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Обучаваща институция'";
                    sheet.Range[$"B{rowCounter++}"].Text = "В полето 'Лице за контакт' няма попълнена стойност!";
                }

                if (!model.PersonsForNotificationsHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Обучаваща институция'";
                    sheet.Range[$"B{rowCounter++}"].Text = "В полето 'Потребители за получаване на известия/уведомления' няма попълнена стойност!";
                }

                if (!model.ProviderPhoneCorrespondenceHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Обучаваща институция'";
                    sheet.Range[$"B{rowCounter++}"].Text = "В полето 'Телефон' няма попълнена стойност!";
                }

                if (!model.ProviderEmailCorrespondenceHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Обучаваща институция'";
                    sheet.Range[$"B{rowCounter++}"].Text = "В полето 'E-mail адрес' няма попълнена стойност!";
                }

                if (model.ProviderType == "CPO")
                {
                    if (!model.CandidateProviderSpecialitiesHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Специалности - учебна програма'";
                        sheet.Range[$"B{rowCounter++}"].Text = "Няма нито една въведена специалност!";
                    }

                    if (model.InactiveSpecialities.Any())
                    {
                        foreach (var entry in model.InactiveSpecialities)
                        {
                            sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Специалности - учебна програма'";
                            sheet.Range[$"B{rowCounter++}"].Text = $"Добавената специалност {entry} е неактивна!";
                        }
                    }

                    if (model.ProviderSpecialities.Any())
                    {
                        foreach (var entry in model.ProviderSpecialities)
                        {
                            if (!entry.FrameworkProgramHasValue)
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Специалности - учебна програма'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към специалност {entry.Speciality} няма въведена рамкова програма!";
                            }

                            if (!entry.EducationFormHasValue)
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Специалности - учебна програма'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към специалност {entry.Speciality} няма въведена форма на обучение!";
                            }

                            if (entry.CurriculumExcelVM.CurriculumNotAdded)
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Специалности - учебна програма'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към специалност {entry.Speciality} няма въведена учебна програма!";
                            }
                            else
                            {
                                if (!entry.IsCurriculumValid)
                                {
                                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Специалности - учебна програма'";
                                    sheet.Range[$"B{rowCounter++}"].Text = $"Към специалност {entry.Speciality} няма въведена валидна учебна програма!";

                                    foreach (var missingTopicEru in entry.CurriculumExcelVM.MissingTopicERUs)
                                    {
                                        var errorArr = missingTopicEru.Split("->").ToArray();
                                        var typeError = errorArr[0];
                                        var descriptionError = errorArr[1];
                                        sheet.Range[$"A{rowCounter}"].Text = typeError;
                                        sheet.Range[$"B{rowCounter++}"].Text = descriptionError;
                                    }

                                    if (entry.CurriculumExcelVM.MissingDOCERUs.Any())
                                    {
                                        sheet.Range[$"A{rowCounter}"].Text = "";
                                        sheet.Range[$"B{rowCounter++}"].Text = "";
                                    }

                                    foreach (var missingEruFromDoc in entry.CurriculumExcelVM.MissingDOCERUs)
                                    {
                                        var errorArr = missingEruFromDoc.Split("->").ToArray();
                                        var typeError = errorArr[0];
                                        var descriptionError = errorArr[1];
                                        sheet.Range[$"A{rowCounter}"].Text = typeError;
                                        sheet.Range[$"B{rowCounter++}"].Text = descriptionError;
                                    }

                                    if (entry.CurriculumExcelVM.MinimumCompulsoryHoursNotReached)
                                    {
                                        sheet.Range[$"A{rowCounter}"].Text = "";
                                        sheet.Range[$"B{rowCounter++}"].Text = "";
                                        sheet.Range[$"A{rowCounter}"].Text = "Въведената учебна програма не отговаря на изискванията за минимален брой задължителни учебни часове!";
                                        sheet.Range[$"B{rowCounter++}"].Text = $"Въведен минимален брой задължителни учебни часове: {entry.CurriculumExcelVM.CompulsoryHours}";
                                    }

                                    if (entry.CurriculumExcelVM.MinimumChoosableHoursNotReached)
                                    {
                                        sheet.Range[$"A{rowCounter}"].Text = "";
                                        sheet.Range[$"B{rowCounter++}"].Text = "";
                                        sheet.Range[$"A{rowCounter}"].Text = "Въведената учебна програма не отговаря на изискванията за минимален брой избираеми учебни часове!";
                                        sheet.Range[$"B{rowCounter++}"].Text = $"Въведен минимален брой избираеми учебни часове: {entry.CurriculumExcelVM.NonCompulsoryHours}";
                                    }

                                    if (entry.CurriculumExcelVM.MaximumPercentNotReached)
                                    {
                                        sheet.Range[$"A{rowCounter}"].Text = "";
                                        sheet.Range[$"B{rowCounter++}"].Text = "";
                                        sheet.Range[$"A{rowCounter}"].Text = "Въведената учебна програма не отговаря на изискванията за максимален % часове обща професионална подготовка спрямо общия брой задължителни часове!";
                                        sheet.Range[$"B{rowCounter++}"].Text = $"Въведен максимален % часове обща професионална подготовка спрямо общия брой задължителни часове: {entry.CurriculumExcelVM.PercentCompulsoryHours.ToString("f2")}";
                                    }

                                    if (entry.CurriculumExcelVM.MaximumPercentNotReached)
                                    {
                                        sheet.Range[$"A{rowCounter}"].Text = "";
                                        sheet.Range[$"B{rowCounter++}"].Text = "";
                                        sheet.Range[$"A{rowCounter}"].Text = "Въведената учебна програма не отговаря на изискванията за минимален % на учебните часове за практическо обучение спрямо общия брой часове за отраслова и специфична професионална подготовка!";
                                        sheet.Range[$"B{rowCounter++}"].Text = $"Въведен минимален % на учебните часове за практическо обучение спрямо общия брой часове за отраслова и специфична професионална подготовка: {entry.CurriculumExcelVM.PercentSpecificTraining.ToString("f2")}";
                                    }
                                }
                            }
                        }
                    }
                }

                if (model.ProviderType == "CPO")
                {
                    if (!model.ManagementHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Управление на центъра' няма въведена стойност!";
                    }

                    if (!model.OrganisationTrainingProcessHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Организация на процеса на обучение' няма въведена стойност!";
                    }

                    if (!model.CompletionCertificationTrainingHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Завършване и удостоверяване на професионалното обучение' няма въведена стойност!";
                    }

                    if (!model.InternalQualitySystemHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Вътрешна система за осигуряване на качеството на обучението, което извършва и прилагането ѝ' няма въведена стойност!";
                    }

                    if (!model.InformationProvisionMaintenanceHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Информационно осигуряване и поддържане на архива на центъра' няма въведена стойност!";
                    }

                    if (!model.TrainingDocumentationHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Актуализиране на учебната документация - учебни планове и учебни програми' няма въведена стойност!";
                    }

                    if (!model.TeachersSelectionHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Подбор на преподаватели' няма въведена стойност!";
                    }

                    if (!model.MTBDescriptionHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Описание на материалната база за провеждане на обучението по теория и по практика в съответствие с изискванията на ДОС за придобиване на квалификация по професията и специалността, за която се кандидатства' няма въведена стойност!";
                    }

                    if (!model.DataMaintenanceHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Поддържане на актуални данни на центъра за професионално обучение и провежданите от него обучения в информационната система на Националната агенция за професионално образование и обучение' няма въведена стойност!";
                    }
                }
                else
                {
                    if (!model.ManagementHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Управление на центъра' няма въведена стойност!";
                    }

                    if (!model.OrganisationInformationProcessHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Организация на дейността по информиране и професионално ориентиране' няма въведена стойност!";
                    }

                    if (!model.InternalQualitySystemHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Вътрешна система за осигуряване на качеството на дейността по информиране и професионално ориентиране' няма въведена стойност!";
                    }

                    if (!model.InformationProvisionMaintenanceHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Информационно осигуряване и поддържането на архива на центъра' няма въведена стойност!";
                    }

                    if (!model.TrainingDocumentationHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Актуализиране на документацията за информиране и професионално ориентиране' няма въведена стойност!";
                    }

                    if (!model.ConsultantsSelectionHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Подбор на лицата, които извършват информиране и професионално ориентиране' няма въведена стойност!";
                    }

                    if (!model.MTBDescriptionHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Осигуряване на материалната база' няма въведена стойност!";
                    }

                    if (!model.DataMaintenanceHasValue)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Устройство и дейност'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"В полето 'Поддържане на актуални данни за центъра и провежданото от него информиране и професионално ориентиране в информационната система на Националната агенция за професионално образование и обучение' няма въведена стойност!";
                    }
                }

                if (!model.AddedTrainersHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Преподаватели'";
                    if (model.ProviderType == "CPO")
                    {
                        sheet.Range[$"B{rowCounter++}"].Text = $"Няма въведена информация за нито един преподавател!";
                    }
                    else
                    {
                        sheet.Range[$"B{rowCounter++}"].Text = $"Няма въведена информация за нито един консултант!";
                    }
                }

                if (model.AddedTrainers.Any())
                {
                    foreach (var trainer in model.AddedTrainers)
                    {
                        if (model.ProviderType == "CPO")
                        {
                            if (!trainer.IsSpecialityAdded)
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Преподаватели'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към преподавател {trainer.FullName} няма въведена специалност!";
                            }
                        }

                        if (!trainer.IsCertificateAdded)
                        {
                            if (model.ProviderType == "CPO")
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Преподаватели'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към преподавател {trainer.FullName} няма добавен документ за свидетелство!";
                            }
                            else
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Консултанти'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към консултант {trainer.FullName} няма добавен документ за свидетелство!";
                            }
                        }

                        if (!trainer.IsAutobiographyAdded)
                        {
                            if (model.ProviderType == "CPO")
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Преподаватели'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към преподавател {trainer.FullName} няма добавен документ за автобиография!";
                            }
                            else
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Консултанти'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към консултант {trainer.FullName} няма добавен документ за автобиография!";
                            }
                        }

                        if (!trainer.IsDeclarationOfConsentAdded)
                        {
                            if (model.ProviderType == "CPO")
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Преподаватели'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към преподавател {trainer.FullName} няма добавен документ за декларация за съгласие!";
                            }
                            else
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Консултанти'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към консултант {trainer.FullName} няма добавен документ за декларация за съгласие!";
                            }
                        }

                        if (!trainer.IsRetrainingDiplomaAdded)
                        {
                            if (model.ProviderType == "CPO")
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Преподаватели'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към преподавател {trainer.FullName} няма добавен документ за диплома за преквалификация!";
                            }
                            else
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Консултанти'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към консултант {trainer.FullName} няма добавен документ за диплома за преквалификация!";
                            }
                        }
                    }

                    if (model.ProviderType == "CPO")
                    {
                        if (model.MissingTrainerSpecialities.Any())
                        {
                            foreach (var entry in model.MissingTrainerSpecialities)
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Преподаватели'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Специалност {entry} не е добавена към нито един преподавател!";
                            }
                        }
                    }
                }

                if (!model.AddedMTBsHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'МТБ'";
                    sheet.Range[$"B{rowCounter++}"].Text = $"Няма въведена информация за нито една МТБ!";
                }

                if (model.ProviderType == "CPO")
                {
                    if (!model.IsDocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfessionAdded)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'МТБ'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"Няма добавен документ за наличие на МТБ съобразно ДОС за придобиване на квалификация по професия - административния офис на центъра!";
                    }
                }

                if (model.AddedMTBs.Any())
                {
                    foreach (var mtb in model.AddedMTBs)
                    {
                        if (model.ProviderType == "CPO")
                        {
                            if (!mtb.IsSpecialityAdded)
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'МТБ'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към МТБ {mtb.Name} няма въведена специалност!";
                            }
                        }

                        if (!mtb.IsDocumentForComplianceWithFireSafetyRulesAndRegulationsAdded)
                        {
                            sheet.Range[$"A{rowCounter}"].Text = "Раздел 'МТБ'";
                            sheet.Range[$"B{rowCounter++}"].Text = $"Към МТБ {mtb.Name} няма добавен документ за съответствие с правилата и нормите за пожарна безопасност!";
                        }

                        if (!mtb.IsDocumentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirementsAdded)
                        {
                            sheet.Range[$"A{rowCounter}"].Text = "Раздел 'МТБ'";
                            sheet.Range[$"B{rowCounter++}"].Text = $"Към МТБ {mtb.Name} няма добавен документ за удостоверяване на издадени от компетентните органи докуемтни за съответствие на МТБ със здравните изисквания!";
                        }

                        if (model.ProviderType == "CPO")
                        {
                            if (!mtb.IsDocumentsForThePresenceOfMTBInAccordanceWithTheDOSAdded)
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'МТБ'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Към МТБ {mtb.Name} няма добавен документ за наличие на МТБ съобразно ДОС за придобиване на квалификация по професия - учебна дейност на центъра!";
                            }
                        }
                    }

                    if (model.ProviderType == "CPO")
                    {
                        if (model.MissingMTBSpecialities.Any())
                        {
                            foreach (var entry in model.MissingMTBSpecialities)
                            {
                                sheet.Range[$"A{rowCounter}"].Text = "Раздел 'МТБ'";
                                sheet.Range[$"B{rowCounter++}"].Text = $"Специалност {entry} не е добавена към нито една МТБ!";
                            }
                        }
                    }
                }

                if (!model.IsRegulationsForTheOrganizationAndOperationOfTheProfessionalTrainingCenterAdded)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Приложени документи'";
                    if (model.ProviderType == "CPO")
                    {
                        sheet.Range[$"B{rowCounter++}"].Text = $"Към заявлението за лицензиране няма добавен документ за правилник за устройството и дейността на центъра за професионално обучение, който съдържа съответните раздели съгласно чл. 23, ал. 3, т. 3, б. от а.) до и.) от ПДНАПОО!";
                    }
                }

                if (model.ProviderType == "CIPO")
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Приложени документи'";
                    sheet.Range[$"B{rowCounter++}"].Text = $"Към заявлението за лицензиране няма добавен документ за процедурата!";
                }

                if (!model.IsFeePaidDocumentAdded)
                {
                    if (model.ProviderType == "CPO")
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Приложени документи'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"Към заявлението за лицензиране няма добавен документ за платена такса!";
                    }
                }

                if (!model.ReceiveLicenseHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Подаване на заявление'";
                    sheet.Range[$"B{rowCounter++}"].Text = $"Към заявлението за лицензиране няма избран начин за получаването на издадения индивидуален административен акт и лицензията!";
                }

                if (!model.ApplicationFilingHasValue)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Подаване на заявление'";
                    sheet.Range[$"B{rowCounter++}"].Text = $"Към заявлението за лицензиране няма избран начин за подаването в НАПОО на разпечатаното от ИС заявление и документ за платена държавна такса, определена в тарифа на Министерски съвет по чл. 60, ал. 2, т. 1 от ЗПОО!";
                }
                else
                {
                    if (model.ESignedApplicationFileIsMissing)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Подаване на заявление'";
                        sheet.Range[$"B{rowCounter++}"].Text = $"Няма прикачен файл с подписано с електронен подпис заявление!";
                    }
                }

                if (!model.IsFormApplicationValid)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "Раздел 'Организация на работата'";
                    sheet.Range[$"B{rowCounter++}"].Text = model.FormApplicationMissingFields;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }

        public async Task<string> UpdateProviderAfterRegixCheckAsync(CandidateProviderVM candidateProvider, string address, string postCode, int idLocation, string providerOwner)
        {
            string msg = string.Empty;

            try
            {
                var candidateProviderFromDb = await this.repository.GetByIdAsync<CandidateProvider>(candidateProvider.IdCandidate_Provider);
                candidateProviderFromDb.IdLocation = idLocation;
                candidateProviderFromDb.ProviderAddress = address;
                candidateProviderFromDb.ZipCode = postCode;
                candidateProviderFromDb.IdLocation = idLocation;
                candidateProviderFromDb.ProviderOwner = providerOwner;

                this.repository.Update<CandidateProvider>(candidateProviderFromDb);
                await this.repository.SaveChangesAsync(false);

                candidateProvider.ProviderOwner = candidateProviderFromDb.ProviderOwner;
                candidateProvider.ProviderAddress = candidateProviderFromDb.ProviderAddress;
                candidateProvider.ZipCode = candidateProviderFromDb.ZipCode;
                candidateProvider.IdLocation = candidateProviderFromDb.IdLocation;

                msg = "Данните в Обучаваща институция са актуализирани спрямо актуалния статус на юридическото лице в Търговския регистър/Регистър БУЛСТАТ!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                msg = "Грешка при запис в базата данни!";
            }

            return msg;
        }

        public async Task<List<int?>> GetCandidateProviderIdApplicationFilingAndIdApplicationReceiveByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var list = new List<int?>();
            var data = await this.repository.GetByIdAsync<CandidateProvider>(idCandidateProvider);
            list.Add(data.IdApplicationFiling);
            list.Add(data.IdReceiveLicense);

            return list;
        }

        public async Task<CandidateProviderSpecialityVM> GetCandidateProviderSpecialityByIdCandidateProviderAndByIdSpecialityAsync(int idCandidateProvider, int idSpeciality)
        {
            var candidateProviderSpeciality = this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == idCandidateProvider && x.IdSpeciality == idSpeciality);

            return await candidateProviderSpeciality.To<CandidateProviderSpecialityVM>(x => x.CandidateCurriculumModifications.Select(y => y.CandidateCurriculums)).FirstOrDefaultAsync();
        }

        public async Task<bool> AreAnyCandidateProviderSpecialitiesByIdCandidateProviderAsync(int idCandidateProvider)
        {
            return await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == idCandidateProvider).AnyAsync();
        }

        public async Task<bool> AreAnyCandidateProviderTrainersByIdCandidateProviderAsync(int idCandidateProvider)
        {
            return await this.repository.AllReadonly<CandidateProviderTrainer>(x => x.IdCandidate_Provider == idCandidateProvider).AnyAsync();
        }

        public async Task<bool> AreAnyCandidateProviderPremisesByIdCandidateProviderAsync(int idCandidateProvider)
        {
            return await this.repository.AllReadonly<CandidateProviderPremises>(x => x.IdCandidate_Provider == idCandidateProvider).AnyAsync();
        }

        public async Task<bool> AreAnyCandidateProviderDocumentsByIdCandidateProviderAsync(int idCandidateProvider)
        {
            return await this.repository.AllReadonly<CandidateProviderDocument>(x => x.IdCandidateProvider == idCandidateProvider).AnyAsync();
        }
        #endregion

        #region Helper methods
        private bool IsTrainerModified(CandidateProviderTrainer trainerFromDb, CandidateProviderTrainerVM trainerVM)
        {
            return trainerFromDb.FirstName != trainerVM.FirstName || trainerFromDb.SecondName != trainerVM.SecondName
                || trainerFromDb.FamilyName != trainerVM.FamilyName || trainerFromDb.IdIndentType != trainerVM.IdIndentType
                || trainerFromDb.Indent != trainerVM.Indent || trainerFromDb.BirthDate != trainerVM.BirthDate
                || trainerFromDb.IdSex != trainerVM.IdSex || trainerFromDb.IdNationality != trainerVM.IdNationality
                || trainerFromDb.Email != trainerVM.Email || trainerFromDb.IdEducation != trainerVM.IdEducation
                || trainerFromDb.EducationSpecialityNotes != trainerVM.EducationSpecialityNotes || trainerFromDb.EducationCertificateNotes != trainerVM.EducationCertificateNotes
                || trainerFromDb.EducationAcademicNotes != trainerVM.EducationAcademicNotes || trainerFromDb.IsAndragog != trainerVM.IsAndragog
                || trainerFromDb.IdContractType != trainerVM.IdContractType || trainerFromDb.ContractDate != trainerVM.ContractDate
                || trainerFromDb.IdStatus != trainerVM.IdStatus
                || (!trainerFromDb.InactiveDate.HasValue && trainerVM.InactiveDate.HasValue)
                || (trainerFromDb.InactiveDate.HasValue && !trainerVM.InactiveDate.HasValue)
                || (trainerFromDb.InactiveDate.HasValue && trainerVM.InactiveDate.HasValue ? trainerFromDb.InactiveDate.Value != trainerVM.InactiveDate.Value : false);
        }

        private bool IsTrainerProfileModified(CandidateProviderTrainerProfile trainerProfileFromDb, CandidateProviderTrainerProfileVM trainerProfileVM)
        {
            return trainerProfileFromDb.IdProfessionalDirection != trainerProfileVM.IdProfessionalDirection || trainerProfileFromDb.IsProfessionalDirectionQualified != trainerProfileVM.IsProfessionalDirectionQualified
                || trainerProfileFromDb.IsTheory != trainerProfileVM.IsTheory || trainerProfileFromDb.IsPractice != trainerProfileVM.IsPractice;
        }

        private bool IsPremisesModified(CandidateProviderPremises premisesFromDb, CandidateProviderPremisesVM premisesVM)
        {
            return premisesFromDb.PremisesName != premisesVM.PremisesName || premisesFromDb.PremisesNote != premisesVM.PremisesNote
                || premisesFromDb.IdLocation != premisesVM.IdLocation || premisesFromDb.ProviderAddress != premisesVM.ProviderAddress
                || premisesFromDb.ZipCode != premisesVM.ZipCode || premisesFromDb.Phone != premisesVM.Phone || premisesFromDb.IdOwnership != premisesVM.IdOwnership
                || premisesFromDb.IdStatus != premisesVM.IdStatus
                || (!premisesFromDb.InactiveDate.HasValue && premisesVM.InactiveDate.HasValue)
                || (premisesFromDb.InactiveDate.HasValue && !premisesVM.InactiveDate.HasValue)
                || (premisesFromDb.InactiveDate.HasValue && premisesVM.InactiveDate.HasValue ? premisesFromDb.InactiveDate.Value != premisesVM.InactiveDate.Value : false);
        }

        private bool IsCandidateProviderModified(CandidateProvider providerFromDb, CandidateProviderVM providerVM)
        {
            return providerFromDb.ProviderOwner != providerVM.ProviderOwner || providerFromDb.PoviderBulstat != providerVM.PoviderBulstat
                || providerFromDb.ManagerName != providerVM.ManagerName || providerFromDb.AttorneyName != providerVM.AttorneyName
                || providerFromDb.Indent != providerVM.Indent || providerFromDb.IdProviderRegistration != providerVM.IdProviderRegistration
                || providerFromDb.IdProviderOwnership != providerVM.IdProviderOwnership || providerFromDb.IdProviderStatus != providerVM.IdProviderStatus
                || providerFromDb.IdLocation != providerVM.IdLocation || providerFromDb.ProviderAddress != providerVM.ProviderAddress
                || providerFromDb.ZipCode != providerVM.ZipCode || providerFromDb.UploadedFileName != providerVM.UploadedFileName
                || providerFromDb.Title != providerVM.Title || providerFromDb.ProviderName != providerVM.ProviderName
                || providerFromDb.IdTypeLicense != providerVM.IdTypeLicense || providerFromDb.ApplicationNumber != providerVM.ApplicationNumber
                || providerFromDb.ApplicationDate != providerVM.ApplicationDate || providerFromDb.LicenceNumber != providerVM.LicenceNumber
                || providerFromDb.LicenceDate != providerVM.LicenceDate || providerFromDb.IdLicenceStatus != providerVM.IdLicenceStatus
                || providerFromDb.ProviderPhone != providerVM.ProviderPhone || providerFromDb.ProviderFax != providerVM.ProviderFax
                || providerFromDb.ProviderWeb != providerVM.ProviderWeb || providerFromDb.ProviderEmail != providerVM.ProviderEmail
                || providerFromDb.AdditionalInfo != providerVM.AdditionalInfo || providerFromDb.AccessibilityInfo != providerVM.AccessibilityInfo
                || providerFromDb.OnlineTrainingInfo != providerVM.OnlineTrainingInfo || providerFromDb.IdTypeApplication != providerVM.IdTypeApplication
                || providerFromDb.IdApplicationStatus != providerVM.IdApplicationStatus || providerFromDb.IdRegistrationApplicationStatus != providerVM.IdRegistrationApplicationStatus
                || providerFromDb.RejectionReason != providerVM.RejectionReason || providerFromDb.DateRequest != providerVM.DateRequest
                || providerFromDb.DueDateRequest != providerVM.DueDateRequest || providerFromDb.DateConfirmRequestNAPOO != providerVM.DateConfirmRequestNAPOO
                || providerFromDb.DateConfirmEMail != providerVM.DateConfirmEMail || providerFromDb.IdReceiveLicense != providerVM.IdReceiveLicense
                || providerFromDb.IdApplicationFiling != providerVM.IdApplicationFiling || providerFromDb.IsActive != providerVM.IsActive
                || providerFromDb.UIN != providerVM.UIN || providerFromDb.PersonNameCorrespondence != providerVM.PersonNameCorrespondence
                || providerFromDb.IdLocationCorrespondence != providerVM.IdLocationCorrespondence || providerFromDb.ProviderAddressCorrespondence != providerVM.ProviderAddressCorrespondence
                || providerFromDb.ZipCodeCorrespondence != providerVM.ZipCodeCorrespondence || providerFromDb.ProviderPhoneCorrespondence != providerVM.ProviderPhoneCorrespondence
                || providerFromDb.ProviderFaxCorrespondence != providerVM.ProviderFaxCorrespondence || providerFromDb.ProviderEmailCorrespondence != providerVM.ProviderEmailCorrespondence
                || providerFromDb.Token != providerVM.Token || providerFromDb.IdStartedProcedure != providerVM.IdStartedProcedure
                || providerFromDb.IdCandidateProviderActive != providerVM.IdCandidateProviderActive;
        }

        private bool IsCandidateProviderSpecialityModified(CandidateProviderSpeciality candidateProviderSpecialityFromDb, CandidateProviderSpecialityVM candidateProviderSpecialityVM)
        {
            if (candidateProviderSpecialityFromDb is not null)
            {
                return candidateProviderSpecialityFromDb.IdFrameworkProgram != candidateProviderSpecialityVM.IdFrameworkProgram || candidateProviderSpecialityFromDb.IdFormEducation != candidateProviderSpecialityVM.IdFormEducation;
            }

            return false;
        }
        #endregion

        #region Curriculum
        public async Task<ResultContext<NoResult>> CreateCandidateProviderSpecialityAsync(int idCandidateProvider, int idSpeciality)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var candidateProviderSpecialityForDb = new CandidateProviderSpeciality()
                {
                    IdCandidate_Provider = idCandidateProvider,
                    IdSpeciality = idSpeciality
                };

                await this.repository.AddAsync<CandidateProviderSpeciality>(candidateProviderSpecialityForDb);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Специалността е добавена успешно!");
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

        public async Task<ResultContext<NoResult>> UpdateCandidateProviderSpecialityIdFrameworkAndIdFormEducationAsync(int idCandidateProviderSpeciality, int? idFrameworkProgram, int? idFormEducation)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var candidateProviderSpecialityFromDb = await this.repository.GetByIdAsync<CandidateProviderSpeciality>(idCandidateProviderSpeciality);
                if (candidateProviderSpecialityFromDb is not null)
                {
                    candidateProviderSpecialityFromDb.IdFormEducation = idFormEducation;
                    candidateProviderSpecialityFromDb.IdFrameworkProgram = idFrameworkProgram;

                    this.repository.Update<CandidateProviderSpeciality>(candidateProviderSpecialityFromDb);
                    await this.repository.SaveChangesAsync();
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

        public MemoryStream CreateExcelCurriculumValidationErrors(ResultContext<CandidateCurriculumExcelVM> resultContext, double compulsoryHours, double nonCompulsoryHours, double percentCompulsoryHours, double percentSpecificTraining)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 120;
                sheet.Range["A1"].Text = "Вид на грешката";
                sheet.Range["B1"].ColumnWidth = 120;
                sheet.Range["B1"].Text = "Описание на грешката";

                IRange range = sheet.Range["A1"];
                IRichTextString boldText = range.RichText;
                IFont boldFont = workbook.CreateFont();

                boldFont.Bold = true;
                boldText.SetFont(0, sheet.Range["A1"].Text.Length, boldFont);

                IRange range2 = sheet.Range["B1"];
                IRichTextString boldText2 = range2.RichText;
                IFont boldFont2 = workbook.CreateFont();

                boldFont2.Bold = true;
                boldText2.SetFont(0, sheet.Range["B1"].Text.Length, boldFont2);


                var rowCounter = 2;
                foreach (var missingTopicEru in resultContext.ResultContextObject.MissingTopicERUs)
                {
                    var errorArr = missingTopicEru.Split("->").ToArray();
                    var typeError = errorArr[0];
                    var descriptionError = errorArr[1];
                    sheet.Range[$"A{rowCounter}"].Text = typeError;
                    sheet.Range[$"B{rowCounter++}"].Text = descriptionError;
                }

                if (resultContext.ResultContextObject.MissingDOCERUs.Any())
                {
                    sheet.Range[$"A{rowCounter}"].Text = "";
                    sheet.Range[$"B{rowCounter++}"].Text = "";
                }

                foreach (var missingEruFromDoc in resultContext.ResultContextObject.MissingDOCERUs)
                {
                    var errorArr = missingEruFromDoc.Split("->").ToArray();
                    var typeError = errorArr[0];
                    var descriptionError = errorArr[1];
                    sheet.Range[$"A{rowCounter}"].Text = typeError;
                    sheet.Range[$"B{rowCounter++}"].Text = descriptionError;
                }

                if (resultContext.ResultContextObject.MinimumCompulsoryHoursNotReached)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "";
                    sheet.Range[$"B{rowCounter++}"].Text = "";
                    sheet.Range[$"A{rowCounter}"].Text = "Въведената учебна програма не отговаря на изискванията за минимален брой задължителни учебни часове!";
                    sheet.Range[$"B{rowCounter++}"].Text = $"Въведен минимален брой задължителни учебни часове: {compulsoryHours}";
                }

                if (resultContext.ResultContextObject.MinimumChoosableHoursNotReached)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "";
                    sheet.Range[$"B{rowCounter++}"].Text = "";
                    sheet.Range[$"A{rowCounter}"].Text = "Въведената учебна програма не отговаря на изискванията за минимален брой избираеми учебни часове!";
                    sheet.Range[$"B{rowCounter++}"].Text = $"Въведен минимален брой избираеми учебни часове: {nonCompulsoryHours}";
                }

                if (resultContext.ResultContextObject.MaximumPercentNotReached)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "";
                    sheet.Range[$"B{rowCounter++}"].Text = "";
                    sheet.Range[$"A{rowCounter}"].Text = "Въведената учебна програма не отговаря на изискванията за максимален % часове обща професионална подготовка спрямо общия брой задължителни часове!";
                    sheet.Range[$"B{rowCounter++}"].Text = $"Въведен максимален % часове обща професионална подготовка спрямо общия брой задължителни часове: {percentCompulsoryHours.ToString("f2")}";
                }

                if (resultContext.ResultContextObject.MinimumPercentNotReached)
                {
                    sheet.Range[$"A{rowCounter}"].Text = "";
                    sheet.Range[$"B{rowCounter++}"].Text = "";
                    sheet.Range[$"A{rowCounter}"].Text = "Въведената учебна програма не отговаря на изискванията за минимален % на учебните часове за практическо обучение спрямо общия брой часове за отраслова и специфична професионална подготовка!";
                    sheet.Range[$"B{rowCounter++}"].Text = $"Въведен минимален % на учебните часове за практическо обучение спрямо общия брой часове за отраслова и специфична професионална подготовка: {percentSpecificTraining.ToString("f2")}";
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }

        public async Task<List<string>> AreDOSChangesWithoutActualizationOfCurriculumsAsync(CandidateProviderVM candidateProvider)
        {
            var msgList = new List<string>();
            var kvDOCActiveStatusValue = this.dataSourceService.GetActiveStatusID();
            var kvModificationStatusFinalValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");
            var kvModificationReasonDOSValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationReasonType", "DOSChange");
            var candidateProviderSpecialities = await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == candidateProvider.IdCandidate_Provider)
                .To<CandidateProviderSpecialityVM>(x => x.Speciality)
                    .ToListAsync();
            foreach (var providerSpec in candidateProviderSpecialities)
            {
                if (providerSpec.Speciality is not null && providerSpec.Speciality.IdDOC.HasValue && providerSpec.Speciality.IdStatus == kvDOCActiveStatusValue)
                {
                    var activeDOS = await this.repository.AllReadonly<Data.Models.Data.DOC.DOC>(x => x.IdDOC == providerSpec.Speciality.IdDOC.Value && x.IdStatus == kvDOCActiveStatusValue).FirstOrDefaultAsync();
                    if (activeDOS is not null)
                    {
                        var modification = await this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == providerSpec.IdCandidateProviderSpeciality && x.IdModificationReason == kvModificationReasonDOSValue.IdKeyValue && x.IdModificationStatus == kvModificationStatusFinalValue.IdKeyValue).OrderByDescending(x => x.ValidFromDate!.Value.Date).FirstOrDefaultAsync();
                        if (modification is not null && modification.ValidFromDate!.Value.Date < activeDOS.StartDate.Date)
                        {
                            msgList.Add(providerSpec.Speciality.CodeAndName);
                            msgList.Add($"{activeDOS.StartDate.Date.ToString(GlobalConstants.DATE_FORMAT)} г.");
                        }
                    }
                }
            }

            return msgList;
        }

        public async Task<MemoryStream> PrintCurriculumAsync(
            FrameworkProgramVM frameworkProgram, SpecialityVM speciality, double totalHours, double ATotalHours, double BTotalHours,
            string providerOwner, bool isInvalid, CandidateProviderSpecialityVM candidateProviderSpeciality, CandidateProviderVM candidateProvider, List<CandidateCurriculumVM> curriculums = null, List<TrainingCurriculumVM> trainingCurriculums = null, bool showInvalidCurriculumText = false)
        {
            var isCandidateCurriculum = curriculums is not null;
            if (isCandidateCurriculum)
            {
                curriculums = curriculums.OrderBy(x => x.IdProfessionalTraining).ToList();
            }
            else
            {
                trainingCurriculums = trainingCurriculums.OrderBy(x => x.IdProfessionalTraining).ToList();
            }

            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\CPO\Application";

            FileStream template = new FileStream($@"{resources_Folder}\Ucheben_plan_template.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Doc);
            WSection section = document.Sections[0];

            WTable table = section.Tables[1] as WTable;
            table.Rows.RemoveAt(5);
            table.Rows.RemoveAt(8);
            table.Rows.RemoveAt(10);
            table.Rows.RemoveAt(15);
            table.Rows.RemoveAt(18);
            table.Rows.RemoveAt(26);

            var insertRowCounter = 5;
            this.HandleA1Curriculums(curriculums, ref table, ref insertRowCounter, trainingCurriculums);
            this.HandleA2TheoryCurriculums(curriculums, ref table, ref insertRowCounter, trainingCurriculums);
            this.HandleA3TheoryCurriculums(curriculums, ref table, ref insertRowCounter, trainingCurriculums);
            this.HandleA2PracticeCurriculums(curriculums, ref table, ref insertRowCounter, trainingCurriculums);
            this.HandleA3PracticeCurriculums(curriculums, ref table, ref insertRowCounter, trainingCurriculums);
            this.HandleBCurriculums(curriculums, ref table, ref insertRowCounter, trainingCurriculums);

            table.Rows.RemoveAt(0);

            if (frameworkProgram.Name.Contains("Д"))
            {
                for (int i = table.Rows.Count - 1; i > -1; i--)
                {
                    var row = table.Rows[i];
                    foreach (var cell in (row as WTableRow).Cells)
                    {
                        foreach (var paragraph in (cell as WTableCell).Paragraphs)
                        {
                            if ((paragraph as WParagraph).Text.Contains("Процент на учебните часове А1 спрямо общия брой задължителни учебни часове за раздел А") || (paragraph as WParagraph).Text.Contains("Процент на учебните часове за практическо обучение спрямо общия брой на учебните часове за раздел А2+раздел А3"))
                            {
                                table.Rows.Remove(row as WTableRow);
                            }
                        }
                    }
                }
            }

            string nkrValue = string.Empty;
            if (speciality.IdNKRLevel != 0)
            {
                var kvNKRValue = await this.dataSourceService.GetKeyValueByIdAsync(speciality.IdNKRLevel);
                if (kvNKRValue is not null)
                {
                    nkrValue = kvNKRValue.Name;
                }
            }

            string ekrValue = string.Empty;
            if (speciality.IdEKRLevel != 0)
            {
                var kvEKRValue = await this.dataSourceService.GetKeyValueByIdAsync(speciality.IdEKRLevel);
                if (kvEKRValue is not null)
                {
                    ekrValue = kvEKRValue.Name;
                }
            }

            string formEducation = string.Empty;
            if (speciality.IdFormEducation != null)
            {
                formEducation = (await this.dataSourceService.GetKeyValueByIdAsync(speciality.IdFormEducation ?? default)).Name;
            }

            var practiceHours = isCandidateCurriculum ? curriculums.Sum(x => x.Practice) : trainingCurriculums.Sum(x => x.Practice);
            var theoryHours = isCandidateCurriculum ? curriculums.Sum(x => x.Theory) : trainingCurriculums.Sum(x => x.Theory);
            var a1TotalHours = isCandidateCurriculum ? curriculums.Where(x => x.ProfessionalTraining == "А1").Sum(x => x.Theory) : trainingCurriculums.Where(x => x.ProfessionalTraining == "А1").Sum(x => x.Theory);
            var a1Percentage = (a1TotalHours / ATotalHours) * 100;
            var a2TheoryTotalHours = isCandidateCurriculum ? curriculums.Where(x => x.ProfessionalTraining == "А2").Sum(x => x.Theory) : trainingCurriculums.Where(x => x.ProfessionalTraining == "А2").Sum(x => x.Theory);
            var a3TheoryTotalHours = isCandidateCurriculum ? curriculums.Where(x => x.ProfessionalTraining == "А3").Sum(x => x.Theory) : trainingCurriculums.Where(x => x.ProfessionalTraining == "А3").Sum(x => x.Theory);
            var a2PracticeTotalHours = isCandidateCurriculum ? curriculums.Where(x => x.ProfessionalTraining == "А2").Sum(x => x.Practice) : trainingCurriculums.Where(x => x.ProfessionalTraining == "А2").Sum(x => x.Practice);
            var a3PracticeTotalHours = isCandidateCurriculum ? curriculums.Where(x => x.ProfessionalTraining == "А3").Sum(x => x.Practice) : trainingCurriculums.Where(x => x.ProfessionalTraining == "А3").Sum(x => x.Practice);
            double a2a3TheoryTotalHours = 0;
            if (a2TheoryTotalHours != null)
            {
                a2a3TheoryTotalHours += a2TheoryTotalHours ?? default;
            }

            if (a3TheoryTotalHours != null)
            {
                a2a3TheoryTotalHours += a3TheoryTotalHours ?? default;
            }

            double a2a3PracticeTotalHours = 0;
            if (a2TheoryTotalHours != null)
            {
                a2a3PracticeTotalHours += a2PracticeTotalHours ?? default;
            }

            if (a3TheoryTotalHours != null)
            {
                a2a3PracticeTotalHours += a3PracticeTotalHours ?? default;
            }

            double a2a3TotalHours = a2a3TheoryTotalHours + a2a3PracticeTotalHours;
            double a2a3Percentage = (a2a3PracticeTotalHours / a2a3TotalHours) * 100;

            var locationAndDate = string.Empty;
            if (candidateProvider.IdLocationCorrespondence.HasValue)
            {
                var location = await this.LocationService.GetLocationByIdAsync(candidateProvider.IdLocationCorrespondence.Value);
                locationAndDate = $"{location.LocationName}, {candidateProviderSpeciality.ModifyDate.ToString("yyyy")}г.";
            }

            var typeFrameworkProgramTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram");
            var partProfessionKV = typeFrameworkProgramTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "PartProfession").IdKeyValue;
            var professionalQualificationKV = typeFrameworkProgramTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ProfessionalQualification").IdKeyValue;
            var vqsSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS", false, true);
            var firstVQS = vqsSource.FirstOrDefault(x => x.KeyValueIntCode == "I_VQS").IdKeyValue;
            var secondVQS = vqsSource.FirstOrDefault(x => x.KeyValueIntCode == "II_VQS").IdKeyValue;
            var thirdVQS = vqsSource.FirstOrDefault(x => x.KeyValueIntCode == "III_VQS").IdKeyValue;
            var fourthVQS = vqsSource.FirstOrDefault(x => x.KeyValueIntCode == "IV_VQS").IdKeyValue;
            speciality.VQS_Name = vqsSource.FirstOrDefault(x => x.IdKeyValue == speciality.IdVQS)!.Name;
            var spkValue = string.Empty;
            if (frameworkProgram.IdVQS == firstVQS)
            {
                spkValue = "Първа степен на професионална квалификация";
            }
            else if (frameworkProgram.IdVQS == secondVQS)
            {
                spkValue = "Втора степен на професионална квалификация";
            }
            else if (frameworkProgram.IdVQS == thirdVQS)
            {
                spkValue = "Трета степен на професионална квалификация";
            }
            else
            {
                spkValue = "Четвърта степен на професионална квалификация";
            }

            var curriculumInformation = string.Empty;
            if (frameworkProgram.IdTypeFrameworkProgram == partProfessionKV)
            {
                curriculumInformation = $"За обучение по рамкова програма \"{frameworkProgram.Name}\" за придобиване на част от професия за {spkValue}";
            }
            else
            {
                curriculumInformation = $"За професионално обучение с придобиване на {spkValue} по рамкова програма \"{frameworkProgram.Name}\"";
            }

            var professionalDirectionSource = this.dataSourceService.GetAllProfessionalDirectionsList();
            var professionsSource = this.dataSourceService.GetAllProfessionsList();
            var profession = professionsSource.FirstOrDefault(x => x.IdProfession == speciality.IdProfession);
            var professionalDirection = professionalDirectionSource.FirstOrDefault(x => x.IdProfessionalDirection == profession.IdProfessionalDirection);
            var doc = await this.DOCService.GetActiveDocByProfessionIdAsync(new ProfessionVM() { IdProfession = profession.IdProfession });
            if (doc is not null)
            {
                var erus = new List<ERUVM>();
                var docErus = await this.DOCService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = doc.IdDOC });
                var eRUSpecialities = await this.eRUSpecialityService.GetAllERUsBySpecialityIdAsync(new ERUSpecialityVM() { IdSpeciality = speciality.IdSpeciality });
                var eruIds = eRUSpecialities.Select(x => x.IdERU).ToList();
                var erusFromDoc = await this.DOCService.GetERUsByIdsAsync(eruIds);
                erus.AddRange(erusFromDoc);
                docErus = docErus.Where(x => x.ERUSpecialities.Count == 0);
                erus.AddRange(docErus);
                if (erus.Any())
                {
                    BookmarksNavigator bookmarksNavigator = new BookmarksNavigator(document);
                    bookmarksNavigator.MoveToBookmark("ERUInformation");

                    foreach (var eru in erus)
                    {
                        IWParagraph paragraph = new WParagraph(document);
                        paragraph.Text = eru.Code + " " + eru.Name;
                        bookmarksNavigator.InsertParagraph(paragraph);

                        IWParagraph paragraph2 = new WParagraph(document);
                        bookmarksNavigator.InsertParagraph(paragraph2);
                        var formatedText = eru.RUText.Replace("<br>", "<br />");
                        paragraph2.AppendHTML(formatedText);
                    }
                }
            }

            //string finishInformation = string.Empty;
            //if (frameworkProgram.Name.Contains("А") || frameworkProgram.Name.Contains("Б"))
            //{
            //    var firstChar = frameworkProgram.Name[0];
            //    finishInformation = $"Професионалното обучение по рамкови програми {firstChar} се завършва след полагане на държавен изпит за придобиване на професионална квалификация – по теория и практика на професията.\r\nОрганизацията и провеждането на държавните изпити се определят с наредба на министъра на образованието и науката.\r\nДържавните изпити за придобиване на първа СПК се провеждат по национални изпитни програми, утвърдени от министъра на образованието и науката.\r\nПридобитата първа СПК се удостоверява със Свидетелство за професионална квалификация. Съдържанието на документа е определено в ДОС за информацията и документите.\r\nПридобилият свидетелство за първа СПК по свое желание може да получи Европейско приложение към Свидетелството за професионална квалификация.\r\nСвидетелството за професионална квалификация дава достъп до пазара на труда и достъп до включване в рамкови програми Е за продължаващо професионално обучение с придобиване на втора степен на професионална квалификация.\r\nСвидетелство за правоспособност се издава за професии, упражняването на които изисква правоспособност. Условията и редът за издаване на свидетелство за правоспособност се определят с наредби на министъра на образованието и науката, освен ако в закон е предвидено друго.";
            //}
            //else if (frameworkProgram.Name.Contains("Е"))
            //{
            //    finishInformation = "Професионалното обучение по рамкови програми Е, за придобиване на степен на професионална квалификация, завършва след полагане на държавен изпит за придобиване на професионална квалификация – по теория и практика на професията.\r\nОрганизацията и провеждането на държавните изпити се определя с наредба на министъра на образованието и науката.\r\nДържавния изпит за придобиване на първа, втора или трета степен на професионална квалификация се провеждат по национални изпитни програми, утвърдени от министъра на образованието и науката.\r\nПридобитата първа, втора или трета степен се удостоверява със Свидетелство за професионална квалификация. Съдържанието на документа е определено в държавния образователен стандарт за информацията и документите.\r\nПрофесионалното обучение по рамкови програми Е, за актуализиране или разширяване на придобита професионална квалификация, завършва след полагане на изпит за актуализиране или разширяване на придобита професионална квалификация по професия – по теория и практика на професията.\r\nИзпитът за актуализиране или разширяване на придобита професионална квалификация се провежда в две части: част по теория на професията и част по практика на професията.\r\nИзпитът за актуализиране или разширяване на придобита професионална квалификация по професия се провежда като писмен изпит по теория и индивидуално задание по практика, определени от обучаващата институция след съгласуване с представителите на работодателите и на работниците и служителите.\r\nЗа завършено професионално обучение за актуализиране или разширяване на придобита професионална квалификация по професия се издава Удостоверение за професионално обучение. Съдържанието на документа е определено в държавния образователен стандарт за информацията и документите.\r\nПридобилият документ за степен на професионална квалификация или за квалификация по част от професия по свое желание може да получи Европейско приложение.\r\nСвидетелството за професионална квалификация или Удостоверението за професионално обучение дава достъп до пазара на труда и достъп до включване в рамкова програма Е за продължаващо професионално обучение с придобиване на следваща степен на професионална квалификация.\r\n\r\n";
            //}
            //else if (frameworkProgram.Name.Contains("Д"))
            //{
            //    finishInformation = "Професионалното обучение по рамкови програми Д се завършва след полагане на изпит за придобиване на квалификация по част от професия – по теория и практика на професията.\r\nИзпитът за придобиване на квалификация по част от професия се провежда в две части: част по теория на професията и част по практика на професията.\r\nИзпитът за придобиване на квалификация по част от професия се провежда като писмен изпит по теория и индивидуално задание по практика, определени от обучаващата институция след съгласуване с представителите на работодателите и на работниците и служителите.\r\nОрганизацията и провеждането на изпита се определя с наредба на министъра на образованието и науката.\r\nЗа завършено професионално обучение за придобиване на квалификация по част от професия се издава Удостоверение за професионално обучение. Съдържанието на документа е определено в държавния образователен стандарт за информацията и документите.\r\nПридобилият Удостоверение за професионално обучение по свое желание може да получи Европейско приложение.\r\nУдостоверението за професионално обучение дава достъп до пазара на труда и достъп до включване в рамкова програма Е за продължаващо професионално обучение с придобиване на съответната степен на професионална квалификация";
            //}

            var invalidText = showInvalidCurriculumText ? "Учебният план не отговаря на изискванията при проверката за съответствие с нормативната уредба!" : string.Empty;
            string[] fieldNames = new string[]
            {
                "CPOName", "CompanyName", "ProfessionalDirectionCodeAndName", "ProfessionCodeAndName", "SpecialityCodeAndName", "SPKValue",
                "NKRValue", "EKRValue", "TrainingPeriod", "TotalHours", "TheoryHours", "PracticeHours", "FormEducation", "MinimumLevelEducation",
                "A1TotalHours", "A1Percentage", "A2TheoryTotalHours", "A3TheoryTotalHours", "A2A3TheoryTotalHours", "A2PracticeTotalHours", "A3PracticeTotalHours",
                "A2A3PracticeTotalHours", "A2A3Percentage", "ATotalHours", "BTotalHours", "ABTotalHours", "ValidationError", "LocationAndDate", "CURRICULUMINFORMATION",
                "DOSOrDOIInformation", "FrameworkProgramName", "SecondFrameworkProgramName", "FinishInformation", "ExplanatoryNotes"
            };

            string[] fieldValues = new string[]
            {
                !string.IsNullOrEmpty(candidateProvider.ProviderName) ? candidateProvider.ProviderName : string.Empty,
                providerOwner,
                professionalDirection.DisplayNameAndCode,
                profession.CodeAndName,
                speciality.CodeAndName,
                speciality.VQS_Name,
                nkrValue,
                ekrValue,
                frameworkProgram.TrainingPeriodName,
                totalHours.ToString(),
                theoryHours.ToString(),
                practiceHours.ToString(),
                formEducation,
                frameworkProgram.MinimumLevelEducationName,
                a1TotalHours.ToString(),
                a1Percentage.Value.ToString("f2"),
                a2TheoryTotalHours.Value != null ? a2TheoryTotalHours.ToString() : string.Empty,
                a3TheoryTotalHours.Value != null ? a3TheoryTotalHours.ToString() : string.Empty,
                a2a3TheoryTotalHours.ToString(),
                a2PracticeTotalHours.Value != null ? a2PracticeTotalHours.ToString() : string.Empty,
                a3PracticeTotalHours.Value != null ? a3PracticeTotalHours.ToString() : string.Empty,
                a2a3PracticeTotalHours.ToString(),
                a2a3Percentage.ToString("f2"),
                ATotalHours.ToString(),
                BTotalHours.ToString(),
                totalHours.ToString(),
                isInvalid ? invalidText : string.Empty,
                locationAndDate,
                curriculumInformation,
                doc is not null ? $"В съответствие с държавния образователен стандарт (ДОС) за придобиване квалификация по професията \"{profession.Name}\", приет с {doc.Regulation}, издадена от МОН" : $"В съответствие с държавни образователни изисквания (ДОИ) за придобиване квалификация по професията \"{profession.Name}\"",
                $"\"{frameworkProgram.Name}\"",
                $"\"{frameworkProgram.Name}\"",
                frameworkProgram.CompletionVocationalTraining,
                frameworkProgram.ExplanatoryNotes
            };

            var professionalTrainingsDict = new Dictionary<string, Dictionary<string, List<string>>>();
            if (isCandidateCurriculum)
            {
                foreach (var curriculum in curriculums.OrderBy(x => x.ProfessionalTraining))
                {
                    var key = curriculum.ProfessionalTraining;
                    if (!professionalTrainingsDict.ContainsKey(key))
                    {
                        professionalTrainingsDict.Add(key, new Dictionary<string, List<string>>());
                    }

                    if (!professionalTrainingsDict[key].ContainsKey(curriculum.Subject))
                    {
                        professionalTrainingsDict[key].Add(curriculum.Subject, new List<string>());
                    }

                    professionalTrainingsDict[key][curriculum.Subject].Add(curriculum.Topic);
                }
            }
            else
            {
                foreach (var curriculum in trainingCurriculums.OrderBy(x => x.ProfessionalTraining))
                {
                    var key = curriculum.ProfessionalTraining;
                    if (!professionalTrainingsDict.ContainsKey(key))
                    {
                        professionalTrainingsDict.Add(key, new Dictionary<string, List<string>>());
                    }

                    if (!professionalTrainingsDict[key].ContainsKey(curriculum.Subject))
                    {
                        professionalTrainingsDict[key].Add(curriculum.Subject, new List<string>());
                    }

                    professionalTrainingsDict[key][curriculum.Subject].Add(curriculum.Topic);
                }
            }

            IWTable thirdTable = section.Tables[2];
            if (professionalTrainingsDict.Any())
            {
                var counterToInsert = 6;
                double aTotalHours = 0;
                double bTotalHours = 0;
                //foreach (var entry in professionalTrainingsDict)
                //{
                //    if (entry.Key == "A1" || entry.Key == "А1")
                //    {
                //        this.HandleRowAggregatedInformation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalPracticeHours, ref aTotalTheoryHours, ref bTotalPracticeHours, ref bTotalTheoryHours, trainingCurriculums);
                //    }
                //    else if (entry.Key == "A2" || entry.Key == "А2")
                //    {
                //        counterToInsert += 1;
                //        this.HandleRowAggregatedInformation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalPracticeHours, ref aTotalTheoryHours, ref bTotalPracticeHours, ref bTotalTheoryHours, trainingCurriculums);
                //    }
                //    else if (entry.Key == "A3" || entry.Key == "А3")
                //    {
                //        counterToInsert += 1;
                //        this.HandleRowAggregatedInformation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalPracticeHours, ref aTotalTheoryHours, ref bTotalPracticeHours, ref bTotalTheoryHours, trainingCurriculums);
                //    }
                //    else
                //    {
                //        counterToInsert += 1;
                //        this.HandleRowAggregatedInformation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalPracticeHours, ref aTotalTheoryHours, ref bTotalPracticeHours, ref bTotalTheoryHours, trainingCurriculums);
                //    }
                //}

                foreach (var entry in professionalTrainingsDict)
                {
                    if (entry.Key == "A1" || entry.Key == "А1")
                    {
                        this.HandleRowAggregatedInformationForTheoryHours(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, trainingCurriculums);
                    }
                    else if (entry.Key == "A2" || entry.Key == "А2")
                    {
                        counterToInsert += 1;
                        this.HandleRowAggregatedInformationForTheoryHours(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, trainingCurriculums);
                    }
                    else if (entry.Key == "A3" || entry.Key == "А3")
                    {
                        counterToInsert += 1;
                        this.HandleRowAggregatedInformationForTheoryHours(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, trainingCurriculums);
                    }
                }

                int a3practiceSubjectCounter = 1;
                foreach (var entry in professionalTrainingsDict)
                {
                    if (entry.Key == "A2" || entry.Key == "А2")
                    {
                        counterToInsert += 3;
                        this.HandleRowAggregatedInformationForA2PracticeHours(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, trainingCurriculums);
                    }
                    else if (entry.Key == "A3" || entry.Key == "А3")
                    {
                        counterToInsert += 2;
                        this.HandleRowAggregatedInformationForA3PracticeHours(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, ref a3practiceSubjectCounter, trainingCurriculums);
                    }
                }

                this.HandleRowAggregatedInformationForA3PracticeHoursInWorkingEnvironment(curriculums, thirdTable, ref counterToInsert, ref a3practiceSubjectCounter, ref aTotalHours, trainingCurriculums);

                foreach (var entry in professionalTrainingsDict)
                {
                    if (entry.Key.StartsWith("Б"))
                    {
                        counterToInsert += 1;
                        this.HandleRowAggregatedInformationForBHours(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, trainingCurriculums);
                    }
                }

                var totalAggregatedHours = aTotalHours + bTotalHours;
                var lastRow = thirdTable.Rows[thirdTable.Rows.Count - 1];
                lastRow.Cells[2].Paragraphs[0].Text = totalAggregatedHours > 0 ? totalAggregatedHours.ToString() : "-";
                //lastRow.Cells[3].Paragraphs[0].Text = aTotalTheoryHours + bTotalTheoryHours > 0 ? (aTotalTheoryHours + bTotalTheoryHours).ToString() : "-";
                //lastRow.Cells[4].Paragraphs[0].Text = aTotalPracticeHours + bTotalPracticeHours > 0 ? (aTotalPracticeHours + bTotalPracticeHours).ToString() : "-";
            }

            thirdTable.Rows.RemoveAt(5);

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            document.Close();
            template.Close();

            return stream;
        }

        private void HandleRowAggregatedInformationForTheoryHours(List<CandidateCurriculumVM> curriculums, Dictionary<string, Dictionary<string, List<string>>> professionalTrainingsDict, IWTable thirdTable, ref int counterToInsert, KeyValuePair<string, Dictionary<string, List<string>>> entry, ref double aTotalHours, ref double bTotalHours, List<TrainingCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = curriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Theory.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalTheoryHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Theory);

                        WTableRow rowA1 = thirdTable.Rows[5].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalTheoryHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalTheoryHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalTheoryHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = curriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Theory.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[5].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Theory.HasValue ? neededTopic!.Theory!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
            else
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = trainingCurriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Theory.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalTheoryHours = trainingCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Theory);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalTheoryHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalTheoryHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalTheoryHours!.Value;
                        }

                        var topicCounter = 1;
                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = trainingCurriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Theory.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Theory.HasValue ? neededTopic!.Theory!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
        }

        private void HandleRowAggregatedInformationForPracticeHours(List<CandidateCurriculumVM> curriculums, Dictionary<string, Dictionary<string, List<string>>> professionalTrainingsDict, IWTable thirdTable, ref int counterToInsert, KeyValuePair<string, Dictionary<string, List<string>>> entry, ref double aTotalHours, ref double bTotalHours, List<TrainingCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = curriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        var topicCounter = 1;
                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = curriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
            else
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = trainingCurriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = trainingCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        var topicCounter = 1;
                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = trainingCurriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
        }

        private void HandleRowAggregatedInformationForBHours(List<CandidateCurriculumVM> curriculums, Dictionary<string, Dictionary<string, List<string>>> professionalTrainingsDict, IWTable thirdTable, ref int counterToInsert, KeyValuePair<string, Dictionary<string, List<string>>> entry, ref double aTotalHours, ref double bTotalHours, List<TrainingCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = curriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);
                        var totalTheoryHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Theory);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = (totalPracticeHours + totalTheoryHours).ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                            bTotalHours += totalTheoryHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = curriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                var bPracticeHours = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value : 0;
                                var bTheoryHours = neededTopic.Theory.HasValue ? neededTopic!.Theory!.Value : 0;

                                thirdCellTopic.Paragraphs[0].Text = (bPracticeHours + bTheoryHours).ToString();

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
            else
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = trainingCurriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = trainingCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);
                        var totalTheoryHours = trainingCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Theory);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = (totalPracticeHours + totalTheoryHours).ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                            bTotalHours += totalTheoryHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = trainingCurriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                var bPracticeHours = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value : 0;
                                var bTheoryHours = neededTopic.Theory.HasValue ? neededTopic!.Theory!.Value : 0;

                                thirdCellTopic.Paragraphs[0].Text = (bPracticeHours + bTheoryHours).ToString();

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
        }

        private void HandleRowAggregatedInformationForA2PracticeHours(List<CandidateCurriculumVM> curriculums, Dictionary<string, Dictionary<string, List<string>>> professionalTrainingsDict, IWTable thirdTable, ref int counterToInsert, KeyValuePair<string, Dictionary<string, List<string>>> entry, ref double aTotalHours, ref double bTotalHours, List<TrainingCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = curriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = curriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
            else
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = trainingCurriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = trainingCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = trainingCurriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
        }

        private void HandleRowAggregatedInformationForA3PracticeHours(List<CandidateCurriculumVM> curriculums, Dictionary<string, Dictionary<string, List<string>>> professionalTrainingsDict, IWTable thirdTable, ref int counterToInsert, KeyValuePair<string, Dictionary<string, List<string>>> entry, ref double aTotalHours, ref double bTotalHours, ref int a3practiceCounter, List<TrainingCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = curriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue && x.Subject.Trim() != "Практическо обучение в реална работна среда" && x.Subject.Trim() != "Производствена практика");
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{a3practiceCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = curriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = trainingCurriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue && x.Subject.Trim() != "Практическо обучение в реална работна среда" && x.Subject.Trim() != "Производствена практика");
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = trainingCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{a3practiceCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = trainingCurriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
        }

        private void HandleRowAggregatedInformationForA3PracticeHoursInWorkingEnvironment(List<CandidateCurriculumVM> curriculums, IWTable thirdTable, ref int counterToInsert, ref int a3PracticeCounter, ref double aTotalHours, List<TrainingCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var curriculumSubjects = curriculums.Where(x => (x.ProfessionalTraining == "A3" || x.ProfessionalTraining == "А3") && x.Practice.HasValue && (x.Subject.Trim() == "Практическо обучение в реална работна среда" || x.Subject.Trim() == "Производствена практика"));
                foreach (var subject in curriculumSubjects)
                {
                    var totalPracticeHours = curriculums.Where(x => x.Subject == subject.Subject && (x.ProfessionalTraining == "A3" || x.ProfessionalTraining == "А3")).Sum(x => x.Practice);
                    aTotalHours += totalPracticeHours.HasValue ? totalPracticeHours.Value : 0;
                    WTableRow rowA1 = thirdTable.Rows[6].Clone();
                    WTableCell secondCell = rowA1.Cells[1];
                    secondCell.Paragraphs[0].Text = subject.Subject;
                    WTableCell thirdCell = rowA1.Cells[2];
                    thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                    thirdTable.Rows.Insert(counterToInsert++, rowA1);

                    var topics = curriculums.Where(x => x.Subject == subject.Subject && (x.ProfessionalTraining == "A3" || x.ProfessionalTraining == "А3") && x.Practice.HasValue);
                    foreach (var topicEntry in topics)
                    {
                        WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                        WTableCell secondCellTopic = rowA1Topic.Cells[1];
                        secondCellTopic.Paragraphs[0].Text = $"{topicEntry.Topic}";
                        WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                        thirdCellTopic.Paragraphs[0].Text = topicEntry!.Practice!.Value.ToString();

                        rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                        thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                    }
                }
            }
            else
            {
                var curriculumSubjects = trainingCurriculums.Where(x => (x.ProfessionalTraining == "A3" || x.ProfessionalTraining == "А3") && x.Practice.HasValue && (x.Subject.Trim() == "Практическо обучение в реална работна среда" || x.Subject.Trim() == "Производствена практика"));
                foreach (var subject in curriculumSubjects)
                {
                    var totalPracticeHours = trainingCurriculums.Where(x => x.Subject == subject.Subject && (x.ProfessionalTraining == "A3" || x.ProfessionalTraining == "А3")).Sum(x => x.Practice);
                    aTotalHours += totalPracticeHours.HasValue ? totalPracticeHours.Value : 0;
                    WTableRow rowA1 = thirdTable.Rows[6].Clone();
                    WTableCell secondCell = rowA1.Cells[1];
                    secondCell.Paragraphs[0].Text = subject.Subject;
                    WTableCell thirdCell = rowA1.Cells[2];
                    thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                    thirdTable.Rows.Insert(counterToInsert++, rowA1);

                    var topics = trainingCurriculums.Where(x => x.Subject == subject.Subject && (x.ProfessionalTraining == "A3" || x.ProfessionalTraining == "А3") && x.Practice.HasValue);
                    foreach (var topicEntry in topics)
                    {
                        WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                        WTableCell secondCellTopic = rowA1Topic.Cells[1];
                        secondCellTopic.Paragraphs[0].Text = $"{topicEntry.Topic}";
                        WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                        thirdCellTopic.Paragraphs[0].Text = topicEntry!.Practice!.Value.ToString();

                        rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                        thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                    }
                }
            }
        }

        public async Task<MemoryStream> GenerateExcelReportCurriculum(string year)
        {
            try
            {
                var Curriculums = this.repository.All<CandidateCurriculum>()
                    .To<CandidateCurriculumVM>(x => x.CandidateProviderSpeciality.CandidateProvider, x => x.CandidateProviderSpeciality.Speciality.Profession)
                    .ToList();
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    int row = 1;

                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Xlsx;

                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    foreach (var curriculum in Curriculums)
                    {
                        object[] excelRow = new object[8]
                        {
                        row,
                        year,
                        curriculum.CandidateProviderSpeciality.CandidateProvider.LicenceNumber,
                        curriculum.CandidateProviderSpeciality.CandidateProvider.PoviderBulstat,
                        curriculum.CandidateProviderSpeciality.Speciality.Profession.Code,
                        curriculum.CandidateProviderSpeciality.Speciality.Code,
                        "2",
                        curriculum.ModifyDate.ToString("dd.MM.yyyy")
                        };

                        worksheet.ImportArray(excelRow, row, 1, false);
                        row++;
                    }

                    if (Curriculums.Count() > 0)
                    {
                        worksheet.Range[$"A1:H{Curriculums.Count()}"].AutofitColumns();
                        worksheet.Range[$"A1:H{Curriculums.Count()}"].BorderInside(ExcelLineStyle.Medium);
                        worksheet.Range[$"A1:H{Curriculums.Count()}"].BorderAround(ExcelLineStyle.Medium);
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

        private void HandleA1Curriculums(List<CandidateCurriculumVM> curriculums, ref WTable table, ref int insertRowCounter, List<TrainingCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var a1Curriculums = curriculums.Where(x => x.ProfessionalTraining == "А1").ToList();
                var a1CurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a1Curriculums)
                {
                    if (!a1CurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a1CurriculumsDict.Add(entry.Subject, entry.Theory ?? default);
                    }
                    else
                    {
                        a1CurriculumsDict[entry.Subject] += entry.Theory ?? default;
                    }
                }

                foreach (var curriculum in a1CurriculumsDict)
                {
                    this.AddRowAndCellInformation(ref table, 0, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
            else
            {
                var a1Curriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "А1").ToList();
                var a1CurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a1Curriculums)
                {
                    if (!a1CurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a1CurriculumsDict.Add(entry.Subject, entry.Theory ?? default);
                    }
                    else
                    {
                        a1CurriculumsDict[entry.Subject] += entry.Theory ?? default;
                    }
                }

                foreach (var curriculum in a1CurriculumsDict)
                {
                    this.AddRowAndCellInformation(ref table, 0, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
        }

        private void HandleA2TheoryCurriculums(List<CandidateCurriculumVM> curriculums, ref WTable table, ref int insertRowCounter, List<TrainingCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var a2TheoryCurriculums = curriculums.Where(x => x.ProfessionalTraining == "А2" && x.Theory != null).ToList();
                this.counter = 1;
                var a2TheoryCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a2TheoryCurriculums)
                {
                    if (!a2TheoryCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a2TheoryCurriculumsDict.Add(entry.Subject, entry.Theory ?? default);
                    }
                    else
                    {
                        a2TheoryCurriculumsDict[entry.Subject] += entry.Theory ?? default;
                    }
                }

                foreach (var curriculum in a2TheoryCurriculumsDict)
                {
                    this.AddRowAndCellInformation(ref table, 3, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
            else
            {
                var a2TheoryCurriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "А2" && x.Theory != null).ToList();
                this.counter = 1;
                var a2TheoryCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a2TheoryCurriculums)
                {
                    if (!a2TheoryCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a2TheoryCurriculumsDict.Add(entry.Subject, entry.Theory ?? default);
                    }
                    else
                    {
                        a2TheoryCurriculumsDict[entry.Subject] += entry.Theory ?? default;
                    }
                }

                foreach (var curriculum in a2TheoryCurriculumsDict)
                {
                    this.AddRowAndCellInformation(ref table, 3, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
        }

        private void HandleA3TheoryCurriculums(List<CandidateCurriculumVM> curriculums, ref WTable table, ref int insertRowCounter, List<TrainingCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var a3TheoryCurriculums = curriculums.Where(x => x.ProfessionalTraining == "А3" && x.Theory != null).ToList();
                this.counter = 1;
                var a3TheoryCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a3TheoryCurriculums)
                {
                    if (!a3TheoryCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a3TheoryCurriculumsDict.Add(entry.Subject, entry.Theory ?? default);
                    }
                    else
                    {
                        a3TheoryCurriculumsDict[entry.Subject] += entry.Theory ?? default;
                    }
                }

                foreach (var curriculum in a3TheoryCurriculumsDict)
                {
                    this.AddRowAndCellInformation(ref table, 5, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
            else
            {
                var a3TheoryCurriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "А3" && x.Theory != null).ToList();
                this.counter = 1;
                var a3TheoryCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a3TheoryCurriculums)
                {
                    if (!a3TheoryCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a3TheoryCurriculumsDict.Add(entry.Subject, entry.Theory ?? default);
                    }
                    else
                    {
                        a3TheoryCurriculumsDict[entry.Subject] += entry.Theory ?? default;
                    }
                }

                foreach (var curriculum in a3TheoryCurriculumsDict)
                {
                    this.AddRowAndCellInformation(ref table, 5, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
        }

        private void HandleA2PracticeCurriculums(List<CandidateCurriculumVM> curriculums, ref WTable table, ref int insertRowCounter, List<TrainingCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                //var a2PracticeCurriculums = curriculums.Where(x => x.ProfessionalTraining == "А2" && x.Practice != null).ToList();
                var a2PracticeCurriculums = curriculums.Where(x => x.ProfessionalTraining == "А2" && x.Practice != null && x.Subject.Trim() != "Практическо обучение в реална работна среда" && x.Subject.Trim() != "Производствена практика").ToList();
                this.counter = 1;
                var a2PracticeCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a2PracticeCurriculums)
                {
                    if (!a2PracticeCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a2PracticeCurriculumsDict.Add(entry.Subject, entry.Practice ?? default);
                    }
                    else
                    {
                        a2PracticeCurriculumsDict[entry.Subject] += entry.Practice ?? default;
                    }
                }

                foreach (var curriculum in a2PracticeCurriculumsDict)
                {
                    //this.AddRowAndCellInformation(ref table, 9, curriculum.Key, curriculum.Value, ref insertRowCounter);
                    this.AddRowAndCellInformation(ref table, 10, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
            else
            {
                //var a2PracticeCurriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "А2" && x.Practice != null).ToList();
                var a2PracticeCurriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "А2" && x.Practice != null && x.Subject.Trim() != "Практическо обучение в реална работна среда" && x.Subject.Trim() != "Производствена практика").ToList();
                this.counter = 1;
                var a2PracticeCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a2PracticeCurriculums)
                {
                    if (!a2PracticeCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a2PracticeCurriculumsDict.Add(entry.Subject, entry.Practice ?? default);
                    }
                    else
                    {
                        a2PracticeCurriculumsDict[entry.Subject] += entry.Practice ?? default;
                    }
                }

                foreach (var curriculum in a2PracticeCurriculumsDict)
                {
                    //this.AddRowAndCellInformation(ref table, 9, curriculum.Key, curriculum.Value, ref insertRowCounter);
                    this.AddRowAndCellInformation(ref table, 10, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
        }

        private void HandleA3PracticeCurriculums(List<CandidateCurriculumVM> curriculums, ref WTable table, ref int insertRowCounter, List<TrainingCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                //var a3PracticeCurriculums = curriculums.Where(x => x.ProfessionalTraining == "А3" && x.Practice != null).ToList();
                var a3PracticeCurriculums = curriculums.Where(x => x.ProfessionalTraining == "А3" && x.Practice != null && x.Subject.Trim() != "Практическо обучение в реална работна среда" && x.Subject.Trim() != "Производствена практика").ToList();
                this.counter = 1;
                var a3PracticeCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a3PracticeCurriculums)
                {
                    if (!a3PracticeCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a3PracticeCurriculumsDict.Add(entry.Subject, entry.Practice ?? default);
                    }
                    else
                    {
                        a3PracticeCurriculumsDict[entry.Subject] += entry.Practice ?? default;
                    }
                }

                foreach (var curriculum in a3PracticeCurriculumsDict)
                {
                    this.AddRowAndCellInformation(ref table, 13, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }

                var a3PracticeCurriculumsInWorkingEnvironment = curriculums.Where(x => x.ProfessionalTraining == "А3" && x.Practice != null && (x.Subject.Trim() == "Практическо обучение в реална работна среда" || x.Subject.Trim() == "Производствена практика")).ToList();
                this.counter = 1;
                var a3PracticeCurriculumsInWorkingEnvironmentDict = new Dictionary<string, double>();
                foreach (var entry in a3PracticeCurriculumsInWorkingEnvironment)
                {
                    if (!a3PracticeCurriculumsInWorkingEnvironmentDict.ContainsKey(entry.Subject))
                    {
                        a3PracticeCurriculumsInWorkingEnvironmentDict.Add(entry.Subject, entry.Practice ?? default);
                    }
                    else
                    {
                        a3PracticeCurriculumsInWorkingEnvironmentDict[entry.Subject] += entry.Practice ?? default;
                    }
                }

                foreach (var curriculum in a3PracticeCurriculumsInWorkingEnvironmentDict)
                {
                    this.AddRowAndCellInformation(ref table, 14, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
            else
            {
                //var a3PracticeCurriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "А3" && x.Practice != null).ToList();
                var a3PracticeCurriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "А3" && x.Practice != null && x.Subject.Trim() != "Практическо обучение в реална работна среда" || x.Subject.Trim() != "Производствена практика").ToList();
                this.counter = 1;
                var a3PracticeCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a3PracticeCurriculums)
                {
                    if (!a3PracticeCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a3PracticeCurriculumsDict.Add(entry.Subject, entry.Practice ?? default);
                    }
                    else
                    {
                        a3PracticeCurriculumsDict[entry.Subject] += entry.Practice ?? default;
                    }
                }

                foreach (var curriculum in a3PracticeCurriculumsDict)
                {
                    this.AddRowAndCellInformation(ref table, 13, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }

                var a3PracticeCurriculumsInWorkingEnvironment = trainingCurriculums.Where(x => x.ProfessionalTraining == "А3" && x.Practice != null && (x.Subject.Trim() == "Практическо обучение в реална работна среда" || x.Subject.Trim() == "Производствена практика")).ToList();
                this.counter = 1;
                var a3PracticeCurriculumsInWorkingEnvironmentDict = new Dictionary<string, double>();
                foreach (var entry in a3PracticeCurriculums)
                {
                    if (!a3PracticeCurriculumsInWorkingEnvironmentDict.ContainsKey(entry.Subject))
                    {
                        a3PracticeCurriculumsInWorkingEnvironmentDict.Add(entry.Subject, entry.Practice ?? default);
                    }
                    else
                    {
                        a3PracticeCurriculumsInWorkingEnvironmentDict[entry.Subject] += entry.Practice ?? default;
                    }
                }

                foreach (var curriculum in a3PracticeCurriculumsInWorkingEnvironmentDict)
                {
                    this.AddRowAndCellInformation(ref table, 14, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
        }

        private void HandleBCurriculums(List<CandidateCurriculumVM> curriculums, ref WTable table, ref int insertRowCounter, List<TrainingCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var bCurriculums = curriculums.Where(x => x.ProfessionalTraining == "Б").ToList();
                this.counter = 1;
                var bCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in bCurriculums)
                {
                    var theory = entry.Theory.HasValue ? entry.Theory.Value : 0;
                    var practice = entry.Practice.HasValue ? entry.Practice.Value : 0;
                    if (!bCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        bCurriculumsDict.Add(entry.Subject, theory + practice);
                    }
                    else
                    {
                        bCurriculumsDict[entry.Subject] += (theory + practice);
                    }
                }

                foreach (var curriculum in bCurriculumsDict)
                {
                    this.AddRowAndCellInformation(ref table, 21, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
            else
            {
                var bCurriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "Б").ToList();
                this.counter = 1;
                var bCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in bCurriculums)
                {
                    var theory = entry.Theory.HasValue ? entry.Theory.Value : 0;
                    var practice = entry.Practice.HasValue ? entry.Practice.Value : 0;
                    if (!bCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        bCurriculumsDict.Add(entry.Subject, theory + practice);
                    }
                    else
                    {
                        bCurriculumsDict[entry.Subject] += (theory + practice);
                    }
                }

                foreach (var curriculum in bCurriculumsDict)
                {
                    this.AddRowAndCellInformation(ref table, 21, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
        }

        private void AddRowAndCellInformation(ref WTable table, int tableCounter, string subject, double hours, ref int insertRowCounter)
        {
            WTableRow row = table.Rows[0].Clone();
            WTableCell firstCell = row.Cells[0];
            firstCell.AddParagraph().AppendText($"{this.counter++}.");
            firstCell.Paragraphs.RemoveAt(0);
            WTableCell secondCell = row.Cells[1];
            secondCell.AddParagraph().AppendText(subject);
            secondCell.Paragraphs.RemoveAt(0);
            WTableCell thirdCell = row.Cells[2];
            thirdCell.AddParagraph().AppendText(hours.ToString());
            thirdCell.Paragraphs.RemoveAt(0);
            thirdCell.Paragraphs[0].ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            table.Rows.Insert((tableCounter + insertRowCounter++), row);
        }

        public async Task<MemoryStream> PrintValidationCurriculumAsync(FrameworkProgramVM frameworkProgram, SpecialityVM speciality, double totalHours, double ATotalHours, double BTotalHours, string providerOwner, bool isInvalid, CandidateProviderSpecialityVM candidateProviderSpeciality, CandidateProviderVM candidateProvider, List<CandidateCurriculumVM> curriculums = null, List<ValidationCurriculumVM> validationCurriculums = null, bool showInvalidCurriculumText = false)
        {
            var isCandidateCurriculum = curriculums is not null;
            if (isCandidateCurriculum)
            {
                curriculums = curriculums.OrderBy(x => x.IdProfessionalTraining).ToList();
            }
            else
            {
                validationCurriculums = validationCurriculums.OrderBy(x => x.IdProfessionalTraining).ToList();
            }

            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\Templates\CPO\Application";

            FileStream template = new FileStream($@"{resources_Folder}\Ucheben_plan_template.docx", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Doc);
            WSection section = document.Sections[0];

            WTable table = section.Tables[1] as WTable;
            table.Rows.RemoveAt(5);
            table.Rows.RemoveAt(8);
            table.Rows.RemoveAt(10);
            table.Rows.RemoveAt(14);
            table.Rows.RemoveAt(16);
            table.Rows.RemoveAt(22);

            var insertRowCounter = 5;
            this.HandleA1ValidationCurriculums(curriculums, ref table, ref insertRowCounter, validationCurriculums);
            this.HandleA2TheoryValidationCurriculums(curriculums, ref table, ref insertRowCounter, validationCurriculums);
            this.HandleA3TheoryValidationCurriculums(curriculums, ref table, ref insertRowCounter, validationCurriculums);
            this.HandleA2PracticeValidationCurriculums(curriculums, ref table, ref insertRowCounter, validationCurriculums);
            this.HandleA3PracticeValidationCurriculums(curriculums, ref table, ref insertRowCounter, validationCurriculums);
            this.HandleBValidationCurriculums(curriculums, ref table, ref insertRowCounter, validationCurriculums);

            table.Rows.RemoveAt(0);

            if (frameworkProgram.Name.Contains("Д"))
            {
                for (int i = table.Rows.Count - 1; i > -1; i--)
                {
                    var row = table.Rows[i];
                    foreach (var cell in (row as WTableRow).Cells)
                    {
                        foreach (var paragraph in (cell as WTableCell).Paragraphs)
                        {
                            if ((paragraph as WParagraph).Text.Contains("Процент на учебните часове А1 спрямо общия брой задължителни учебни часове за раздел А") || (paragraph as WParagraph).Text.Contains("Процент на учебните часове за практическо обучение спрямо общия брой на учебните часове за раздел А2+раздел А3"))
                            {
                                table.Rows.Remove(row as WTableRow);
                            }
                        }
                    }
                }
            }

            string nkrValue = string.Empty;
            if (speciality.IdNKRLevel != 0)
            {
                nkrValue = (await this.dataSourceService.GetKeyValueByIdAsync(speciality.IdNKRLevel)).Name;
            }

            string ekrValue = string.Empty;
            if (speciality.IdEKRLevel != 0)
            {
                ekrValue = (await this.dataSourceService.GetKeyValueByIdAsync(speciality.IdEKRLevel)).Name;
            }

            string formEducation = string.Empty;
            if (speciality.IdFormEducation != null)
            {
                formEducation = (await this.dataSourceService.GetKeyValueByIdAsync(speciality.IdFormEducation ?? default)).Name;
            }

            var practiceHours = isCandidateCurriculum ? curriculums.Sum(x => x.Practice) : validationCurriculums.Sum(x => x.Practice);
            var theoryHours = isCandidateCurriculum ? curriculums.Sum(x => x.Theory) : validationCurriculums.Sum(x => x.Theory);
            var a1TotalHours = isCandidateCurriculum ? curriculums.Where(x => x.ProfessionalTraining == "А1").Sum(x => x.Theory) : validationCurriculums.Where(x => x.ProfessionalTraining == "А1").Sum(x => x.Theory);
            var a1Percentage = (a1TotalHours / ATotalHours) * 100;
            var a2TheoryTotalHours = isCandidateCurriculum ? curriculums.Where(x => x.ProfessionalTraining == "А2").Sum(x => x.Theory) : validationCurriculums.Where(x => x.ProfessionalTraining == "А2").Sum(x => x.Theory);
            var a3TheoryTotalHours = isCandidateCurriculum ? curriculums.Where(x => x.ProfessionalTraining == "А3").Sum(x => x.Theory) : validationCurriculums.Where(x => x.ProfessionalTraining == "А3").Sum(x => x.Theory);
            var a2PracticeTotalHours = isCandidateCurriculum ? curriculums.Where(x => x.ProfessionalTraining == "А2").Sum(x => x.Practice) : validationCurriculums.Where(x => x.ProfessionalTraining == "А2").Sum(x => x.Practice);
            var a3PracticeTotalHours = isCandidateCurriculum ? curriculums.Where(x => x.ProfessionalTraining == "А3").Sum(x => x.Practice) : validationCurriculums.Where(x => x.ProfessionalTraining == "А3").Sum(x => x.Practice);
            double a2a3TheoryTotalHours = 0;
            if (a2TheoryTotalHours != null)
            {
                a2a3TheoryTotalHours += a2TheoryTotalHours ?? default;
            }

            if (a3TheoryTotalHours != null)
            {
                a2a3TheoryTotalHours += a3TheoryTotalHours ?? default;
            }

            double a2a3PracticeTotalHours = 0;
            if (a2TheoryTotalHours != null)
            {
                a2a3PracticeTotalHours += a2PracticeTotalHours ?? default;
            }

            if (a3TheoryTotalHours != null)
            {
                a2a3PracticeTotalHours += a3PracticeTotalHours ?? default;
            }

            double a2a3TotalHours = a2a3TheoryTotalHours + a2a3PracticeTotalHours;
            double a2a3Percentage = (a2a3PracticeTotalHours / a2a3TotalHours) * 100;

            var locationAndDate = string.Empty;
            if (candidateProvider.IdLocationCorrespondence.HasValue)
            {
                var location = await this.LocationService.GetLocationByIdAsync(candidateProvider.IdLocationCorrespondence.Value);
                locationAndDate = $"{location.LocationName}, {candidateProviderSpeciality.ModifyDate.ToString("yyyy")}г.";
            }

            var typeFrameworkProgramTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram");
            var partProfessionKV = typeFrameworkProgramTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "PartProfession").IdKeyValue;
            var professionalQualificationKV = typeFrameworkProgramTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ProfessionalQualification").IdKeyValue;
            var vqsSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
            var firstVQS = vqsSource.FirstOrDefault(x => x.KeyValueIntCode == "I_VQS").IdKeyValue;
            var secondVQS = vqsSource.FirstOrDefault(x => x.KeyValueIntCode == "II_VQS").IdKeyValue;
            var thirdVQS = vqsSource.FirstOrDefault(x => x.KeyValueIntCode == "III_VQS").IdKeyValue;
            var fourthVQS = vqsSource.FirstOrDefault(x => x.KeyValueIntCode == "IV_VQS").IdKeyValue;
            speciality.VQS_Name = vqsSource.FirstOrDefault(x => x.IdKeyValue == speciality.IdVQS)!.Name;
            var spkValue = string.Empty;
            if (frameworkProgram.IdVQS == firstVQS)
            {
                spkValue = "Първа степен на професионална квалификация";
            }
            else if (frameworkProgram.IdVQS == secondVQS)
            {
                spkValue = "Втора степен на професионална квалификация";
            }
            else if (frameworkProgram.IdVQS == thirdVQS)
            {
                spkValue = "Трета степен на професионална квалификация";
            }
            else
            {
                spkValue = "Четвърта степен на професионална квалификация";
            }

            var curriculumInformation = string.Empty;
            if (frameworkProgram.IdTypeFrameworkProgram == partProfessionKV)
            {
                curriculumInformation = $"За обучение по рамкова програма \"{frameworkProgram.Name}\" за придобиване на част от професия за {spkValue}";
            }
            else
            {
                curriculumInformation = $"За професионално обучение с придобиване на {spkValue} по рамкова програма \"{frameworkProgram.Name}\"";
            }

            var professionalDirectionSource = this.dataSourceService.GetAllProfessionalDirectionsList();
            var professionsSource = this.dataSourceService.GetAllProfessionsList();
            var profession = professionsSource.FirstOrDefault(x => x.IdProfession == speciality.IdProfession);
            var professionalDirection = professionalDirectionSource.FirstOrDefault(x => x.IdProfessionalDirection == profession.IdProfessionalDirection);
            var doc = await this.DOCService.GetActiveDocByProfessionIdAsync(new ProfessionVM() { IdProfession = profession.IdProfession });
            if (doc is not null)
            {
                var erus = new List<ERUVM>();
                var docErus = await this.DOCService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = doc.IdDOC });
                var eRUSpecialities = await this.eRUSpecialityService.GetAllERUsBySpecialityIdAsync(new ERUSpecialityVM() { IdSpeciality = speciality.IdSpeciality });
                var eruIds = eRUSpecialities.Select(x => x.IdERU).ToList();
                var erusFromDoc = await this.DOCService.GetERUsByIdsAsync(eruIds);
                erus.AddRange(erusFromDoc);
                docErus = docErus.Where(x => x.ERUSpecialities.Count == 0);
                erus.AddRange(docErus);
                if (erus.Any())
                {
                    BookmarksNavigator bookmarksNavigator = new BookmarksNavigator(document);
                    bookmarksNavigator.MoveToBookmark("ERUInformation");

                    foreach (var eru in erus)
                    {
                        IWParagraph paragraph = new WParagraph(document);
                        paragraph.Text = eru.Code + " " + eru.Name;
                        bookmarksNavigator.InsertParagraph(paragraph);

                        IWParagraph paragraph2 = new WParagraph(document);
                        bookmarksNavigator.InsertParagraph(paragraph2);
                        var formatedText = eru.RUText.Replace("<br>", "<br />");
                        paragraph2.AppendHTML(formatedText);
                    }
                }
            }

            string finishInformation = string.Empty;
            if (frameworkProgram.Name.Contains("А") || frameworkProgram.Name.Contains("Б"))
            {
                var firstChar = frameworkProgram.Name[0];
                finishInformation = $"Професионалното обучение по рамкови програми {firstChar} се завършва след полагане на държавен изпит за придобиване на професионална квалификация – по теория и практика на професията.\r\nОрганизацията и провеждането на държавните изпити се определят с наредба на министъра на образованието и науката.\r\nДържавните изпити за придобиване на първа СПК се провеждат по национални изпитни програми, утвърдени от министъра на образованието и науката.\r\nПридобитата първа СПК се удостоверява със Свидетелство за професионална квалификация. Съдържанието на документа е определено в ДОС за информацията и документите.\r\nПридобилият свидетелство за първа СПК по свое желание може да получи Европейско приложение към Свидетелството за професионална квалификация.\r\nСвидетелството за професионална квалификация дава достъп до пазара на труда и достъп до включване в рамкови програми Е за продължаващо професионално обучение с придобиване на втора степен на професионална квалификация.\r\nСвидетелство за правоспособност се издава за професии, упражняването на които изисква правоспособност. Условията и редът за издаване на свидетелство за правоспособност се определят с наредби на министъра на образованието и науката, освен ако в закон е предвидено друго.";
            }
            else if (frameworkProgram.Name.Contains("Е"))
            {
                finishInformation = "Професионалното обучение по рамкови програми Е, за придобиване на степен на професионална квалификация, завършва след полагане на държавен изпит за придобиване на професионална квалификация – по теория и практика на професията.\r\nОрганизацията и провеждането на държавните изпити се определя с наредба на министъра на образованието и науката.\r\nДържавния изпит за придобиване на първа, втора или трета степен на професионална квалификация се провеждат по национални изпитни програми, утвърдени от министъра на образованието и науката.\r\nПридобитата първа, втора или трета степен се удостоверява със Свидетелство за професионална квалификация. Съдържанието на документа е определено в държавния образователен стандарт за информацията и документите.\r\nПрофесионалното обучение по рамкови програми Е, за актуализиране или разширяване на придобита професионална квалификация, завършва след полагане на изпит за актуализиране или разширяване на придобита професионална квалификация по професия – по теория и практика на професията.\r\nИзпитът за актуализиране или разширяване на придобита професионална квалификация се провежда в две части: част по теория на професията и част по практика на професията.\r\nИзпитът за актуализиране или разширяване на придобита професионална квалификация по професия се провежда като писмен изпит по теория и индивидуално задание по практика, определени от обучаващата институция след съгласуване с представителите на работодателите и на работниците и служителите.\r\nЗа завършено професионално обучение за актуализиране или разширяване на придобита професионална квалификация по професия се издава Удостоверение за професионално обучение. Съдържанието на документа е определено в държавния образователен стандарт за информацията и документите.\r\nПридобилият документ за степен на професионална квалификация или за квалификация по част от професия по свое желание може да получи Европейско приложение.\r\nСвидетелството за професионална квалификация или Удостоверението за професионално обучение дава достъп до пазара на труда и достъп до включване в рамкова програма Е за продължаващо професионално обучение с придобиване на следваща степен на професионална квалификация.\r\n\r\n";
            }
            else if (frameworkProgram.Name.Contains("Д"))
            {
                finishInformation = "Професионалното обучение по рамкови програми Д се завършва след полагане на изпит за придобиване на квалификация по част от професия – по теория и практика на професията.\r\nИзпитът за придобиване на квалификация по част от професия се провежда в две части: част по теория на професията и част по практика на професията.\r\nИзпитът за придобиване на квалификация по част от професия се провежда като писмен изпит по теория и индивидуално задание по практика, определени от обучаващата институция след съгласуване с представителите на работодателите и на работниците и служителите.\r\nОрганизацията и провеждането на изпита се определя с наредба на министъра на образованието и науката.\r\nЗа завършено професионално обучение за придобиване на квалификация по част от професия се издава Удостоверение за професионално обучение. Съдържанието на документа е определено в държавния образователен стандарт за информацията и документите.\r\nПридобилият Удостоверение за професионално обучение по свое желание може да получи Европейско приложение.\r\nУдостоверението за професионално обучение дава достъп до пазара на труда и достъп до включване в рамкова програма Е за продължаващо професионално обучение с придобиване на съответната степен на професионална квалификация";
            }

            var invalidText = showInvalidCurriculumText ? "Учебният план не отговаря на изискванията при проверката за съответствие с нормативната уредба!" : string.Empty;
            string[] fieldNames = new string[]
            {
                "CPOName", "CompanyName", "ProfessionalDirectionCodeAndName", "ProfessionCodeAndName", "SpecialityCodeAndName", "SPKValue",
                "NKRValue", "EKRValue", "TrainingPeriod", "TotalHours", "TheoryHours", "PracticeHours", "FormEducation", "MinimumLevelEducation",
                "A1TotalHours", "A1Percentage", "A2TheoryTotalHours", "A3TheoryTotalHours", "A2A3TheoryTotalHours", "A2PracticeTotalHours", "A3PracticeTotalHours",
                "A2A3PracticeTotalHours", "A2A3Percentage", "ATotalHours", "BTotalHours", "ABTotalHours", "ValidationError", "LocationAndDate", "CURRICULUMINFORMATION",
                "DOSOrDOIInformation", "FrameworkProgramName", "SecondFrameworkProgramName", "FinishInformation"
            };

            string[] fieldValues = new string[]
            {
                !string.IsNullOrEmpty(candidateProvider.ProviderName) ? candidateProvider.ProviderName : string.Empty,
                providerOwner,
                professionalDirection.DisplayNameAndCode,
                profession.CodeAndName,
                speciality.CodeAndName,
                speciality.VQS_Name,
                nkrValue,
                ekrValue,
                frameworkProgram.TrainingPeriodName,
                totalHours.ToString(),
                theoryHours.ToString(),
                practiceHours.ToString(),
                formEducation,
                frameworkProgram.MinimumLevelEducationName,
                a1TotalHours.ToString(),
                a1Percentage.Value.ToString("f2"),
                a2TheoryTotalHours.Value != null ? a2TheoryTotalHours.ToString() : string.Empty,
                a3TheoryTotalHours.Value != null ? a3TheoryTotalHours.ToString() : string.Empty,
                a2a3TheoryTotalHours.ToString(),
                a2PracticeTotalHours.Value != null ? a2PracticeTotalHours.ToString() : string.Empty,
                a3PracticeTotalHours.Value != null ? a3PracticeTotalHours.ToString() : string.Empty,
                a2a3PracticeTotalHours.ToString(),
                a2a3Percentage.ToString("f2"),
                ATotalHours.ToString(),
                BTotalHours.ToString(),
                totalHours.ToString(),
                isInvalid ? invalidText : string.Empty,
                locationAndDate,
                curriculumInformation,
                doc is not null ? $"В съответствие с държавния образователен стандарт (ДОС) за придобиване квалификация по професията \"{profession.Name}\", приет с {doc.Regulation}, издадена от МОН" : $"В съответствие с държавни образователни изисквания (ДОИ) за придобиване квалификация по професията \"{profession.Name}\"",
                $"\"{frameworkProgram.Name}\"",
                $"\"{frameworkProgram.Name}\"",
                finishInformation
            };

            var professionalTrainingsDict = new Dictionary<string, Dictionary<string, List<string>>>();
            if (isCandidateCurriculum)
            {
                foreach (var curriculum in curriculums.OrderBy(x => x.ProfessionalTraining))
                {
                    var key = curriculum.ProfessionalTraining;
                    if (!professionalTrainingsDict.ContainsKey(key))
                    {
                        professionalTrainingsDict.Add(key, new Dictionary<string, List<string>>());
                    }

                    if (!professionalTrainingsDict[key].ContainsKey(curriculum.Subject))
                    {
                        professionalTrainingsDict[key].Add(curriculum.Subject, new List<string>());
                    }

                    professionalTrainingsDict[key][curriculum.Subject].Add(curriculum.Topic);
                }
            }
            else
            {
                foreach (var curriculum in validationCurriculums.OrderBy(x => x.ProfessionalTraining))
                {
                    var key = curriculum.ProfessionalTraining;
                    if (!professionalTrainingsDict.ContainsKey(key))
                    {
                        professionalTrainingsDict.Add(key, new Dictionary<string, List<string>>());
                    }

                    if (!professionalTrainingsDict[key].ContainsKey(curriculum.Subject))
                    {
                        professionalTrainingsDict[key].Add(curriculum.Subject, new List<string>());
                    }

                    professionalTrainingsDict[key][curriculum.Subject].Add(curriculum.Topic);
                }
            }

            IWTable thirdTable = section.Tables[2];
            if (professionalTrainingsDict.Any())
            {
                var counterToInsert = 6;
                double aTotalHours = 0;
                double bTotalHours = 0;

                //foreach (var entry in professionalTrainingsDict)
                //{
                //    if (entry.Key == "A1" || entry.Key == "А1")
                //    {
                //        this.HandleRowAggregatedValidationInformation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalPracticeHours, ref aTotalTheoryHours, ref bTotalPracticeHours, ref bTotalTheoryHours, validationCurriculums);
                //    }
                //    else if (entry.Key == "A2" || entry.Key == "А2")
                //    {
                //        counterToInsert += 1;
                //        this.HandleRowAggregatedValidationInformation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalPracticeHours, ref aTotalTheoryHours, ref bTotalPracticeHours, ref bTotalTheoryHours, validationCurriculums);
                //    }
                //    else if (entry.Key == "A3" || entry.Key == "А3")
                //    {
                //        counterToInsert += 1;
                //        this.HandleRowAggregatedValidationInformation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalPracticeHours, ref aTotalTheoryHours, ref bTotalPracticeHours, ref bTotalTheoryHours, validationCurriculums);
                //    }
                //    else
                //    {
                //        counterToInsert += 1;
                //        this.HandleRowAggregatedValidationInformation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalPracticeHours, ref aTotalTheoryHours, ref bTotalPracticeHours, ref bTotalTheoryHours, validationCurriculums);
                //    }
                //}

                foreach (var entry in professionalTrainingsDict)
                {
                    if (entry.Key == "A1" || entry.Key == "А1")
                    {
                        this.HandleRowAggregatedInformationForTheoryHoursValidation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, validationCurriculums);
                    }
                    else if (entry.Key == "A2" || entry.Key == "А2")
                    {
                        counterToInsert += 1;
                        this.HandleRowAggregatedInformationForTheoryHoursValidation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, validationCurriculums);
                    }
                    else if (entry.Key == "A3" || entry.Key == "А3")
                    {
                        counterToInsert += 1;
                        this.HandleRowAggregatedInformationForTheoryHoursValidation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, validationCurriculums);
                    }
                }

                int a3practiceSubjectCounter = 1;
                foreach (var entry in professionalTrainingsDict)
                {
                    if (entry.Key == "A1" || entry.Key == "А1")
                    {
                        this.HandleRowAggregatedInformationForPracticeHoursValidation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, validationCurriculums);
                    }
                    else if (entry.Key == "A2" || entry.Key == "А2")
                    {
                        counterToInsert += 3;
                        this.HandleRowAggregatedInformationForA2PracticeHoursValidation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, validationCurriculums);
                    }
                    else if (entry.Key == "A3" || entry.Key == "А3")
                    {
                        counterToInsert += 2;
                        this.HandleRowAggregatedInformationForA3PracticeHoursValidation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, ref a3practiceSubjectCounter, validationCurriculums);
                    }
                }

                this.HandleRowAggregatedInformationForA3PracticeHoursInWorkingEnvironmentValidation(curriculums, thirdTable, ref counterToInsert, ref a3practiceSubjectCounter, ref aTotalHours, validationCurriculums);

                foreach (var entry in professionalTrainingsDict)
                {
                    if (entry.Key.StartsWith("Б"))
                    {
                        counterToInsert += 1;
                        this.HandleRowAggregatedInformationForBHoursValidation(curriculums, professionalTrainingsDict, thirdTable, ref counterToInsert, entry, ref aTotalHours, ref bTotalHours, validationCurriculums);
                    }
                }

                var totalAggregatedHours = aTotalHours + bTotalHours;
                var lastRow = thirdTable.Rows[thirdTable.Rows.Count - 1];
                lastRow.Cells[2].Paragraphs[0].Text = totalAggregatedHours > 0 ? totalAggregatedHours.ToString() : "-";
                //lastRow.Cells[3].Paragraphs[0].Text = aTotalTheoryHours + bTotalTheoryHours > 0 ? (aTotalTheoryHours + bTotalTheoryHours).ToString() : "-";
                //lastRow.Cells[4].Paragraphs[0].Text = aTotalPracticeHours + bTotalPracticeHours > 0 ? (aTotalPracticeHours + bTotalPracticeHours).ToString() : "-";
            }

            thirdTable.Rows.RemoveAt(5);

            document.MailMerge.Execute(fieldNames, fieldValues);

            MemoryStream stream = new MemoryStream();
            document.Save(stream, FormatType.Docx);
            document.Close();
            template.Close();

            return stream;
        }

        private void HandleRowAggregatedInformationForTheoryHoursValidation(List<CandidateCurriculumVM> curriculums, Dictionary<string, Dictionary<string, List<string>>> professionalTrainingsDict, IWTable thirdTable, ref int counterToInsert, KeyValuePair<string, Dictionary<string, List<string>>> entry, ref double aTotalHours, ref double bTotalHours, List<ValidationCurriculumVM> validationCurriculums)
        {
            if (curriculums is not null)
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = curriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Theory.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalTheoryHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Theory);

                        WTableRow rowA1 = thirdTable.Rows[5].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalTheoryHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalTheoryHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalTheoryHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = curriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Theory.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[5].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Theory.HasValue ? neededTopic!.Theory!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
            else
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = validationCurriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Theory.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalTheoryHours = validationCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Theory);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalTheoryHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalTheoryHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalTheoryHours!.Value;
                        }

                        var topicCounter = 1;
                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = validationCurriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Theory.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Theory.HasValue ? neededTopic!.Theory!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
        }

        private void HandleRowAggregatedInformationForPracticeHoursValidation(List<CandidateCurriculumVM> curriculums, Dictionary<string, Dictionary<string, List<string>>> professionalTrainingsDict, IWTable thirdTable, ref int counterToInsert, KeyValuePair<string, Dictionary<string, List<string>>> entry, ref double aTotalHours, ref double bTotalHours, List<ValidationCurriculumVM> validationCurriculums)
        {
            if (curriculums is not null)
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = curriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        var topicCounter = 1;
                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = curriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
            else
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = validationCurriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = validationCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        var topicCounter = 1;
                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = validationCurriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
        }

        private void HandleRowAggregatedInformationForA2PracticeHoursValidation(List<CandidateCurriculumVM> curriculums, Dictionary<string, Dictionary<string, List<string>>> professionalTrainingsDict, IWTable thirdTable, ref int counterToInsert, KeyValuePair<string, Dictionary<string, List<string>>> entry, ref double aTotalHours, ref double bTotalHours, List<ValidationCurriculumVM> validationCurriculums)
        {
            if (curriculums is not null)
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = curriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = curriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
            else
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = validationCurriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = validationCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = validationCurriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
        }

        private void HandleRowAggregatedInformationForA3PracticeHoursValidation(List<CandidateCurriculumVM> curriculums, Dictionary<string, Dictionary<string, List<string>>> professionalTrainingsDict, IWTable thirdTable, ref int counterToInsert, KeyValuePair<string, Dictionary<string, List<string>>> entry, ref double aTotalHours, ref double bTotalHours, ref int a3practiceCounter, List<ValidationCurriculumVM> validationCurriculums)
        {
            if (curriculums is not null)
            {
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = curriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue && x.Subject.Trim() != "Практическо обучение в реална работна среда" && x.Subject.Trim() != "Производствена практика");
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{a3practiceCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = curriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = validationCurriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue && x.Subject.Trim() != "Практическо обучение в реална работна среда" && x.Subject.Trim() != "Производствена практика");
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = validationCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{a3practiceCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = validationCurriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                thirdCellTopic.Paragraphs[0].Text = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value.ToString() : "-";

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
        }

        private void HandleRowAggregatedInformationForA3PracticeHoursInWorkingEnvironmentValidation(List<CandidateCurriculumVM> curriculums, IWTable thirdTable, ref int counterToInsert, ref int a3PracticeCounter, ref double aTotalHours, List<ValidationCurriculumVM> validationCurriculums)
        {
            if (curriculums is not null)
            {
                var curriculumSubjects = curriculums.Where(x => (x.ProfessionalTraining == "A3" || x.ProfessionalTraining == "А3") && x.Practice.HasValue && (x.Subject.Trim() == "Практическо обучение в реална работна среда" || x.Subject.Trim() == "Производствена практика"));
                foreach (var subject in curriculumSubjects)
                {
                    var totalPracticeHours = curriculums.Where(x => x.Subject == subject.Subject && (x.ProfessionalTraining == "A3" || x.ProfessionalTraining == "А3")).Sum(x => x.Practice);
                    aTotalHours += totalPracticeHours.HasValue ? totalPracticeHours.Value : 0;
                    WTableRow rowA1 = thirdTable.Rows[6].Clone();
                    WTableCell secondCell = rowA1.Cells[1];
                    secondCell.Paragraphs[0].Text = subject.Subject;
                    WTableCell thirdCell = rowA1.Cells[2];
                    thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                    thirdTable.Rows.Insert(counterToInsert++, rowA1);

                    var topics = curriculums.Where(x => x.Subject == subject.Subject && (x.ProfessionalTraining == "A3" || x.ProfessionalTraining == "А3") && x.Practice.HasValue);
                    foreach (var topicEntry in topics)
                    {
                        WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                        WTableCell secondCellTopic = rowA1Topic.Cells[1];
                        secondCellTopic.Paragraphs[0].Text = $"{topicEntry.Topic}";
                        WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                        thirdCellTopic.Paragraphs[0].Text = topicEntry!.Practice!.Value.ToString();

                        rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                        thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                    }
                }
            }
            else
            {
                var curriculumSubjects = validationCurriculums.Where(x => (x.ProfessionalTraining == "A3" || x.ProfessionalTraining == "А3") && x.Practice.HasValue && (x.Subject.Trim() == "Практическо обучение в реална работна среда" || x.Subject.Trim() == "Производствена практика"));
                foreach (var subject in curriculumSubjects)
                {
                    var totalPracticeHours = validationCurriculums.Where(x => x.Subject == subject.Subject && (x.ProfessionalTraining == "A3" || x.ProfessionalTraining == "А3")).Sum(x => x.Practice);
                    aTotalHours += totalPracticeHours.HasValue ? totalPracticeHours.Value : 0;
                    WTableRow rowA1 = thirdTable.Rows[6].Clone();
                    WTableCell secondCell = rowA1.Cells[1];
                    secondCell.Paragraphs[0].Text = subject.Subject;
                    WTableCell thirdCell = rowA1.Cells[2];
                    thirdCell.Paragraphs[0].Text = totalPracticeHours.ToString();

                    thirdTable.Rows.Insert(counterToInsert++, rowA1);

                    var topics = validationCurriculums.Where(x => x.Subject == subject.Subject && (x.ProfessionalTraining == "A3" || x.ProfessionalTraining == "А3") && x.Practice.HasValue);
                    foreach (var topicEntry in topics)
                    {
                        WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                        WTableCell secondCellTopic = rowA1Topic.Cells[1];
                        secondCellTopic.Paragraphs[0].Text = $"{topicEntry.Topic}";
                        WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                        thirdCellTopic.Paragraphs[0].Text = topicEntry!.Practice!.Value.ToString();

                        rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                        thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                    }
                }
            }
        }

        private void HandleRowAggregatedInformationForBHoursValidation(List<CandidateCurriculumVM> curriculums, Dictionary<string, Dictionary<string, List<string>>> professionalTrainingsDict, IWTable thirdTable, ref int counterToInsert, KeyValuePair<string, Dictionary<string, List<string>>> entry, ref double aTotalHours, ref double bTotalHours, List<ValidationCurriculumVM> validationCurriculums)
        {
            if (curriculums is not null)
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = curriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);
                        var totalTheoryHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Theory);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = (totalPracticeHours + totalTheoryHours).ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                            bTotalHours += totalTheoryHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = curriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                var bPracticeHours = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value : 0;
                                var bTheoryHours = neededTopic.Theory.HasValue ? neededTopic!.Theory!.Value : 0;

                                thirdCellTopic.Paragraphs[0].Text = (bPracticeHours + bTheoryHours).ToString();

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
            else
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = validationCurriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                    if (curriculumSubject is not null)
                    {
                        var totalPracticeHours = validationCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);
                        var totalTheoryHours = validationCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Theory);

                        WTableRow rowA1 = thirdTable.Rows[6].Clone();
                        WTableCell firstCell = rowA1.Cells[0];
                        firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                        WTableCell secondCell = rowA1.Cells[1];
                        secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                        WTableCell thirdCell = rowA1.Cells[2];
                        thirdCell.Paragraphs[0].Text = (totalPracticeHours + totalTheoryHours).ToString();

                        thirdTable.Rows.Insert(counterToInsert++, rowA1);

                        if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                        {
                            bTotalHours += totalPracticeHours!.Value;
                            bTotalHours += totalTheoryHours!.Value;
                        }
                        else
                        {
                            aTotalHours += totalPracticeHours!.Value;
                        }

                        foreach (var topicEntry in subject.Value)
                        {
                            var neededTopic = validationCurriculums.FirstOrDefault(x => x.Topic == topicEntry && x.Subject == curriculumSubject.Subject && x.ProfessionalTraining == entry.Key && x.Practice.HasValue);
                            if (neededTopic is not null)
                            {
                                WTableRow rowA1Topic = thirdTable.Rows[6].Clone();
                                WTableCell secondCellTopic = rowA1Topic.Cells[1];
                                secondCellTopic.Paragraphs[0].Text = $"{topicEntry}";
                                WTableCell thirdCellTopic = rowA1Topic.Cells[2];

                                var bPracticeHours = neededTopic.Practice.HasValue ? neededTopic!.Practice!.Value : 0;
                                var bTheoryHours = neededTopic.Theory.HasValue ? neededTopic!.Theory!.Value : 0;

                                thirdCellTopic.Paragraphs[0].Text = (bPracticeHours + bTheoryHours).ToString();

                                rowA1Topic.Cells[0].Paragraphs[0].Text = string.Empty;

                                thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                            }
                        }
                    }
                }
            }
        }

        private void HandleRowAggregatedValidationInformation(List<CandidateCurriculumVM> curriculums, Dictionary<string, Dictionary<string, List<string>>> professionalTrainingsDict, IWTable thirdTable, ref int counterToInsert, KeyValuePair<string, Dictionary<string, List<string>>> entry, ref double aTotalPracticeHours, ref double aTotalTheoryHours, ref double bTotalPracticeHours, ref double bTotalTheoryHours, List<ValidationCurriculumVM> validationCurriculums)
        {
            if (curriculums is not null)
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = curriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key);
                    var totalPracticeHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);
                    var totalTheoryHours = curriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Theory);


                    WTableRow rowA1 = thirdTable.Rows[5].Clone();
                    WTableCell firstCell = rowA1.Cells[0];
                    firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                    WTableCell secondCell = rowA1.Cells[1];
                    secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                    WTableCell thirdCell = rowA1.Cells[2];
                    var totalSubjectHours = totalPracticeHours + totalTheoryHours;
                    thirdCell.Paragraphs[0].Text = totalSubjectHours > 0 ? totalSubjectHours.Value.ToString() : "-";
                    WTableCell fourthCell = rowA1.Cells[3];
                    fourthCell.Paragraphs[0].Text = totalTheoryHours > 0 ? totalTheoryHours.Value.ToString() : "-";
                    WTableCell fifthCell = rowA1.Cells[4];
                    fifthCell.Paragraphs[0].Text = totalPracticeHours > 0 ? totalPracticeHours.Value.ToString() : "-";

                    thirdTable.Rows.Insert(counterToInsert++, rowA1);

                    if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                    {
                        if (totalPracticeHours.HasValue)
                        {
                            bTotalPracticeHours += totalPracticeHours.Value;
                        }
                        else
                        {
                            bTotalPracticeHours += 0;
                        }

                        if (totalTheoryHours.HasValue)
                        {
                            bTotalTheoryHours += totalTheoryHours.Value;
                        }
                        else
                        {
                            bTotalTheoryHours += 0;
                        }
                    }
                    else
                    {
                        if (totalPracticeHours.HasValue)
                        {
                            aTotalPracticeHours += totalPracticeHours.Value;
                        }
                        else
                        {
                            aTotalPracticeHours += 0;
                        }

                        if (totalTheoryHours.HasValue)
                        {
                            aTotalTheoryHours += totalTheoryHours.Value;
                        }
                        else
                        {
                            aTotalTheoryHours += 0;
                        }
                    }

                    var topicCounter = 1;
                    foreach (var topicEntry in subject.Value)
                    {
                        var neededTopic = curriculums.FirstOrDefault(x => x.Topic == topicEntry && x.ProfessionalTraining == entry.Key);

                        WTableRow rowA1Topic = thirdTable.Rows[5].Clone();
                        WTableCell secondCellTopic = rowA1Topic.Cells[1];
                        secondCellTopic.Paragraphs[0].Text = $"{topicCounter++}. {topicEntry}";
                        WTableCell thirdCellTopic = rowA1Topic.Cells[2];
                        double totalTopicHours = 0;
                        if (neededTopic.Practice.HasValue)
                        {
                            if (neededTopic.Theory.HasValue)
                            {
                                totalTopicHours = neededTopic.Practice.Value + neededTopic.Theory.Value;
                            }
                            else
                            {
                                totalTopicHours = neededTopic.Practice.Value + 0;
                            }
                        }
                        else
                        {
                            if (neededTopic.Theory.HasValue)
                            {
                                totalTopicHours = 0 + neededTopic.Theory.Value;
                            }
                            else
                            {
                                totalTopicHours = 0 + 0;
                            }
                        }

                        thirdCellTopic.Paragraphs[0].Text = totalTopicHours > 0 ? totalTopicHours.ToString() : "-";
                        WTableCell fourthTopicCell = rowA1Topic.Cells[3];
                        fourthTopicCell.Paragraphs[0].Text = neededTopic.Theory > 0 ? neededTopic.Theory.Value.ToString() : "-";
                        WTableCell fifthTopicCell = rowA1Topic.Cells[4];
                        fifthTopicCell.Paragraphs[0].Text = neededTopic.Practice > 0 ? neededTopic.Practice.Value.ToString() : "-";

                        thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                    }
                }
            }
            else
            {
                var subjectCounter = 1;
                foreach (var subject in professionalTrainingsDict[entry.Key])
                {
                    var curriculumSubject = validationCurriculums.FirstOrDefault(x => x.Subject == subject.Key && x.ProfessionalTraining == entry.Key);
                    var totalPracticeHours = validationCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Practice);
                    var totalTheoryHours = validationCurriculums.Where(x => x.Subject == subject.Key).Sum(x => x.Theory);

                    WTableRow rowA1 = thirdTable.Rows[5].Clone();
                    WTableCell firstCell = rowA1.Cells[0];
                    firstCell.Paragraphs[0].Text = $"{subjectCounter++}.";
                    WTableCell secondCell = rowA1.Cells[1];
                    secondCell.Paragraphs[0].Text = curriculumSubject.Subject;
                    WTableCell thirdCell = rowA1.Cells[2];
                    var totalSubjectHours = totalPracticeHours + totalTheoryHours;
                    thirdCell.Paragraphs[0].Text = totalSubjectHours > 0 ? totalSubjectHours.Value.ToString() : "-";
                    //WTableCell fourthCell = rowA1.Cells[3];
                    //fourthCell.Paragraphs[0].Text = totalTheoryHours > 0 ? totalTheoryHours.Value.ToString() : "-";
                    //WTableCell fifthCell = rowA1.Cells[4];
                    //fifthCell.Paragraphs[0].Text = totalPracticeHours > 0 ? totalPracticeHours.Value.ToString() : "-";

                    thirdTable.Rows.Insert(counterToInsert++, rowA1);

                    if (!entry.Key.Contains("A") || !entry.Key.Contains("А"))
                    {
                        if (totalPracticeHours.HasValue)
                        {
                            bTotalPracticeHours += totalPracticeHours.Value;
                        }
                        else
                        {
                            bTotalPracticeHours += 0;
                        }

                        if (totalTheoryHours.HasValue)
                        {
                            bTotalTheoryHours += totalTheoryHours.Value;
                        }
                        else
                        {
                            bTotalTheoryHours += 0;
                        }
                    }
                    else
                    {
                        if (totalPracticeHours.HasValue)
                        {
                            aTotalPracticeHours += totalPracticeHours.Value;
                        }
                        else
                        {
                            aTotalPracticeHours += 0;
                        }

                        if (totalTheoryHours.HasValue)
                        {
                            aTotalTheoryHours += totalTheoryHours.Value;
                        }
                        else
                        {
                            aTotalTheoryHours += 0;
                        }
                    }

                    var topicCounter = 1;
                    foreach (var topicEntry in subject.Value)
                    {
                        var neededTopic = validationCurriculums.FirstOrDefault(x => x.Topic == topicEntry && x.ProfessionalTraining == entry.Key);

                        WTableRow rowA1Topic = thirdTable.Rows[5].Clone();
                        WTableCell secondCellTopic = rowA1Topic.Cells[1];
                        secondCellTopic.Paragraphs[0].Text = $"{topicCounter++}. {topicEntry}";
                        WTableCell thirdCellTopic = rowA1Topic.Cells[2];
                        double totalTopicHours = 0;
                        if (neededTopic.Practice.HasValue)
                        {
                            if (neededTopic.Theory.HasValue)
                            {
                                totalTopicHours = neededTopic.Practice.Value + neededTopic.Theory.Value;
                            }
                            else
                            {
                                totalTopicHours = neededTopic.Practice.Value + 0;
                            }
                        }
                        else
                        {
                            if (neededTopic.Theory.HasValue)
                            {
                                totalTopicHours = 0 + neededTopic.Theory.Value;
                            }
                            else
                            {
                                totalTopicHours = 0 + 0;
                            }
                        }

                        thirdCellTopic.Paragraphs[0].Text = totalTopicHours > 0 ? totalTopicHours.ToString() : "-";
                        WTableCell fourthTopicCell = rowA1Topic.Cells[3];
                        fourthTopicCell.Paragraphs[0].Text = neededTopic.Theory > 0 ? neededTopic.Theory.Value.ToString() : "-";
                        WTableCell fifthTopicCell = rowA1Topic.Cells[4];
                        fifthTopicCell.Paragraphs[0].Text = neededTopic.Practice > 0 ? neededTopic.Practice.Value.ToString() : "-";

                        thirdTable.Rows.Insert(counterToInsert++, rowA1Topic);
                    }
                }
            }
        }

        private void HandleA1ValidationCurriculums(List<CandidateCurriculumVM> curriculums, ref WTable table, ref int insertRowCounter, List<ValidationCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var a1Curriculums = curriculums.Where(x => x.ProfessionalTraining == "А1").ToList();
                var a1CurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a1Curriculums)
                {
                    if (!a1CurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a1CurriculumsDict.Add(entry.Subject, entry.Theory ?? default);
                    }
                    else
                    {
                        a1CurriculumsDict[entry.Subject] += entry.Theory ?? default;
                    }
                }

                foreach (var curriculum in a1CurriculumsDict)
                {
                    this.ValidationAddRowAndCellInformation(ref table, 0, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
            else
            {
                var a1Curriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "А1").ToList();
                var a1CurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a1Curriculums)
                {
                    if (!a1CurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a1CurriculumsDict.Add(entry.Subject, entry.Theory ?? default);
                    }
                    else
                    {
                        a1CurriculumsDict[entry.Subject] += entry.Theory ?? default;
                    }
                }

                foreach (var curriculum in a1CurriculumsDict)
                {
                    this.AddRowAndCellInformation(ref table, 0, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
        }

        private void HandleA2TheoryValidationCurriculums(List<CandidateCurriculumVM> curriculums, ref WTable table, ref int insertRowCounter, List<ValidationCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var a2TheoryCurriculums = curriculums.Where(x => x.ProfessionalTraining == "А2" && x.Theory != null).ToList();
                this.counter = 1;
                var a2TheoryCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a2TheoryCurriculums)
                {
                    if (!a2TheoryCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a2TheoryCurriculumsDict.Add(entry.Subject, entry.Theory ?? default);
                    }
                    else
                    {
                        a2TheoryCurriculumsDict[entry.Subject] += entry.Theory ?? default;
                    }
                }

                foreach (var curriculum in a2TheoryCurriculumsDict)
                {
                    this.ValidationAddRowAndCellInformation(ref table, 3, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
            else
            {
                var a2TheoryCurriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "А2" && x.Theory != null).ToList();
                this.counter = 1;
                var a2TheoryCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a2TheoryCurriculums)
                {
                    if (!a2TheoryCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a2TheoryCurriculumsDict.Add(entry.Subject, entry.Theory ?? default);
                    }
                    else
                    {
                        a2TheoryCurriculumsDict[entry.Subject] += entry.Theory ?? default;
                    }
                }

                foreach (var curriculum in a2TheoryCurriculumsDict)
                {
                    this.ValidationAddRowAndCellInformation(ref table, 3, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
        }

        private void HandleA3TheoryValidationCurriculums(List<CandidateCurriculumVM> curriculums, ref WTable table, ref int insertRowCounter, List<ValidationCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var a3TheoryCurriculums = curriculums.Where(x => x.ProfessionalTraining == "А3" && x.Theory != null).ToList();
                this.counter = 1;
                var a3TheoryCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a3TheoryCurriculums)
                {
                    if (!a3TheoryCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a3TheoryCurriculumsDict.Add(entry.Subject, entry.Theory ?? default);
                    }
                    else
                    {
                        a3TheoryCurriculumsDict[entry.Subject] += entry.Theory ?? default;
                    }
                }

                foreach (var curriculum in a3TheoryCurriculumsDict)
                {
                    this.ValidationAddRowAndCellInformation(ref table, 5, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
            else
            {
                var a3TheoryCurriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "А3" && x.Theory != null).ToList();
                this.counter = 1;
                var a3TheoryCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a3TheoryCurriculums)
                {
                    if (!a3TheoryCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a3TheoryCurriculumsDict.Add(entry.Subject, entry.Theory ?? default);
                    }
                    else
                    {
                        a3TheoryCurriculumsDict[entry.Subject] += entry.Theory ?? default;
                    }
                }

                foreach (var curriculum in a3TheoryCurriculumsDict)
                {
                    this.ValidationAddRowAndCellInformation(ref table, 5, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
        }

        private void HandleA2PracticeValidationCurriculums(List<CandidateCurriculumVM> curriculums, ref WTable table, ref int insertRowCounter, List<ValidationCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var a2PracticeCurriculums = curriculums.Where(x => x.ProfessionalTraining == "А2" && x.Practice != null).ToList();
                this.counter = 1;
                var a2PracticeCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a2PracticeCurriculums)
                {
                    if (!a2PracticeCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a2PracticeCurriculumsDict.Add(entry.Subject, entry.Practice ?? default);
                    }
                    else
                    {
                        a2PracticeCurriculumsDict[entry.Subject] += entry.Practice ?? default;
                    }
                }

                foreach (var curriculum in a2PracticeCurriculumsDict)
                {
                    this.ValidationAddRowAndCellInformation(ref table, 9, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
            else
            {
                var a2PracticeCurriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "А2" && x.Practice != null).ToList();
                this.counter = 1;
                var a2PracticeCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a2PracticeCurriculums)
                {
                    if (!a2PracticeCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a2PracticeCurriculumsDict.Add(entry.Subject, entry.Practice ?? default);
                    }
                    else
                    {
                        a2PracticeCurriculumsDict[entry.Subject] += entry.Practice ?? default;
                    }
                }

                foreach (var curriculum in a2PracticeCurriculumsDict)
                {
                    this.ValidationAddRowAndCellInformation(ref table, 9, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
        }

        private void HandleA3PracticeValidationCurriculums(List<CandidateCurriculumVM> curriculums, ref WTable table, ref int insertRowCounter, List<ValidationCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var a3PracticeCurriculums = curriculums.Where(x => x.ProfessionalTraining == "А3" && x.Practice != null).ToList();
                this.counter = 1;
                var a3PracticeCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a3PracticeCurriculums)
                {
                    if (!a3PracticeCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a3PracticeCurriculumsDict.Add(entry.Subject, entry.Practice ?? default);
                    }
                    else
                    {
                        a3PracticeCurriculumsDict[entry.Subject] += entry.Practice ?? default;
                    }
                }

                foreach (var curriculum in a3PracticeCurriculumsDict)
                {
                    this.ValidationAddRowAndCellInformation(ref table, 11, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
            else
            {
                var a3PracticeCurriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "А3" && x.Practice != null).ToList();
                this.counter = 1;
                var a3PracticeCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in a3PracticeCurriculums)
                {
                    if (!a3PracticeCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        a3PracticeCurriculumsDict.Add(entry.Subject, entry.Practice ?? default);
                    }
                    else
                    {
                        a3PracticeCurriculumsDict[entry.Subject] += entry.Practice ?? default;
                    }
                }

                foreach (var curriculum in a3PracticeCurriculumsDict)
                {
                    this.ValidationAddRowAndCellInformation(ref table, 11, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
        }

        private void HandleBValidationCurriculums(List<CandidateCurriculumVM> curriculums, ref WTable table, ref int insertRowCounter, List<ValidationCurriculumVM> trainingCurriculums)
        {
            if (curriculums is not null)
            {
                var bCurriculums = curriculums.Where(x => x.ProfessionalTraining == "Б").ToList();
                this.counter = 1;
                var bCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in bCurriculums)
                {
                    var theory = entry.Theory.HasValue ? entry.Theory.Value : 0;
                    var practice = entry.Practice.HasValue ? entry.Practice.Value : 0;
                    if (!bCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        bCurriculumsDict.Add(entry.Subject, theory + practice);
                    }
                    else
                    {
                        bCurriculumsDict[entry.Subject] += (theory + practice);
                    }
                }

                foreach (var curriculum in bCurriculumsDict)
                {
                    this.ValidationAddRowAndCellInformation(ref table, 17, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
            else
            {
                var bCurriculums = trainingCurriculums.Where(x => x.ProfessionalTraining == "Б").ToList();
                this.counter = 1;
                var bCurriculumsDict = new Dictionary<string, double>();
                foreach (var entry in bCurriculums)
                {
                    var theory = entry.Theory.HasValue ? entry.Theory.Value : 0;
                    var practice = entry.Practice.HasValue ? entry.Practice.Value : 0;
                    if (!bCurriculumsDict.ContainsKey(entry.Subject))
                    {
                        bCurriculumsDict.Add(entry.Subject, theory + practice);
                    }
                    else
                    {
                        bCurriculumsDict[entry.Subject] += (theory + practice);
                    }
                }

                foreach (var curriculum in bCurriculumsDict)
                {
                    this.ValidationAddRowAndCellInformation(ref table, 17, curriculum.Key, curriculum.Value, ref insertRowCounter);
                }
            }
        }

        private void ValidationAddRowAndCellInformation(ref WTable table, int tableCounter, string subject, double hours, ref int insertRowCounter)
        {
            WTableRow row = table.Rows[0].Clone();
            WTableCell firstCell = row.Cells[0];
            firstCell.AddParagraph().AppendText($"{this.counter++}.");
            firstCell.Paragraphs.RemoveAt(0);
            WTableCell secondCell = row.Cells[1];
            secondCell.AddParagraph().AppendText(subject);
            secondCell.Paragraphs.RemoveAt(0);
            WTableCell thirdCell = row.Cells[2];
            thirdCell.AddParagraph().AppendText(hours.ToString());
            thirdCell.Paragraphs.RemoveAt(0);
            thirdCell.Paragraphs[0].ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Right;
            table.Rows.Insert((tableCounter + insertRowCounter++), row);
        }
        #endregion

        #region Document
        public async Task<CandidateProviderDocumentVM> GetCandidateProviderDocumentByIdAsync(CandidateProviderDocumentVM candidateProviderDocumentVM)
        {
            IQueryable<CandidateProviderDocument> candidateProviderDocuments = this.repository.AllReadonly<CandidateProviderDocument>(x => x.IdCandidateProviderDocument == candidateProviderDocumentVM.IdCandidateProviderDocument);

            return await candidateProviderDocuments.To<CandidateProviderDocumentVM>().FirstOrDefaultAsync();
        }

        public async Task<ResultContext<CandidateProviderDocumentVM>> CreateCandidateProviderDocumentAsync(CandidateProviderDocumentVM candidateProviderDocumentVM, bool isAdditionalDocument = false)
        {
            ResultContext<CandidateProviderDocumentVM> outputContext = new ResultContext<CandidateProviderDocumentVM>();

            try
            {
                candidateProviderDocumentVM.ModifyDate = DateTime.Now;
                candidateProviderDocumentVM.CreationDate = DateTime.Now;

                var entryForDb = candidateProviderDocumentVM.To<CandidateProviderDocument>();
                entryForDb.CandidateProvider = null;
                entryForDb.IsAdditionalDocument = isAdditionalDocument;

                await this.repository.AddAsync<CandidateProviderDocument>(entryForDb);
                await this.repository.SaveChangesAsync();

                outputContext.AddMessage("Записът е успешен!");
                candidateProviderDocumentVM.IdCandidateProviderDocument = entryForDb.IdCandidateProviderDocument;
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<CandidateProviderDocumentVM>> UpdateCandidateProviderDocumentAsync(CandidateProviderDocumentVM candidateProviderDocumentVM, bool isAdditionalDocument = false)
        {
            ResultContext<CandidateProviderDocumentVM> outputContext = new ResultContext<CandidateProviderDocumentVM>();

            try
            {
                var entity = await this.repository.GetByIdAsync<CandidateProviderDocument>(candidateProviderDocumentVM.IdCandidateProviderDocument);
                this.repository.Detach<CandidateProviderDocument>(entity);

                candidateProviderDocumentVM.IdCreateUser = entity.IdCreateUser;
                candidateProviderDocumentVM.CreationDate = entity.CreationDate;
                entity = candidateProviderDocumentVM.To<CandidateProviderDocument>();
                entity.CandidateProvider = null;
                entity.IsAdditionalDocument = isAdditionalDocument;

                this.repository.Update<CandidateProviderDocument>(entity);
                await this.repository.SaveChangesAsync();
                this.repository.Detach<CandidateProviderDocument>(entity);

                outputContext.AddMessage("Записът е успешен!");
                outputContext.ResultContextObject = candidateProviderDocumentVM;
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<CandidateProviderDocumentVM>> DeleteCandidateProviderDocumentAsync(CandidateProviderDocumentVM candidateProviderDocumentVM)
        {
            var entity = await this.repository.GetByIdAsync<CandidateProviderDocument>(candidateProviderDocumentVM.IdCandidateProviderDocument);
            this.repository.Detach<CandidateProviderDocument>(entity);

            ResultContext<CandidateProviderDocumentVM> resultContext = new ResultContext<CandidateProviderDocumentVM>();

            try
            {
                this.repository.HardDelete<CandidateProviderDocument>(entity);
                await this.repository.SaveChangesAsync();

                var settingsFolder = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var pathToFile = settingsFolder + "\\" + entity.UploadedFileName;
                if (entity.UploadedFileName != null)
                {
                    if (Directory.Exists(pathToFile))
                    {
                        Directory.Delete(pathToFile, true);
                    }
                }

                resultContext.AddMessage("Документът е изтрит успешно!");
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

        public async Task<IEnumerable<CandidateProviderDocumentVM>> GetAllCandidateProviderDocumentsByCandidateProviderIdAsync(CandidateProviderDocumentVM candidateProviderDocumentVM)
        {
            IQueryable<CandidateProviderDocument> providerDocuments = this.repository.AllReadonly<CandidateProviderDocument>(x => x.IdCandidateProvider == candidateProviderDocumentVM.IdCandidateProvider);
            var result = providerDocuments.To<CandidateProviderDocumentVM>();

            return await result.ToListAsync();
        }

        public async Task<IEnumerable<CandidateProviderDocumentsGridData>> SetDataForDocumentsGrid(int candidateProviderId, IEnumerable<KeyValueVM> kvProviderDocumentTypeSource,
            IEnumerable<KeyValueVM> kvMTBDocumentTypeSource, IEnumerable<KeyValueVM> kvTrainerDocumentTypeSource, bool isAdditionalDocument = false)
        {
            List<CandidateProviderDocumentsGridData> data = new List<CandidateProviderDocumentsGridData>();
            CandidateProvider candidateProviderFromDb;

            if (isAdditionalDocument)
            {
                candidateProviderFromDb = await this.repository.AllReadonly<CandidateProvider>(x => x.IdCandidate_Provider == candidateProviderId)
                    .Include(x => x.CandidateProviderDocuments.Where(c => c.IsAdditionalDocument == isAdditionalDocument))
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }
            else
            {
                candidateProviderFromDb = await this.repository.AllReadonly<CandidateProvider>(x => x.IdCandidate_Provider == candidateProviderId)
                    .Include(x => x.CandidateProviderTrainers)
                        .ThenInclude(x => x.CandidateProviderTrainerDocuments)
                    .AsNoTracking()
                    .Include(x => x.CandidateProviderPremises)
                        .ThenInclude(x => x.CandidateProviderPremisesDocuments)
                    .AsNoTracking()
                    .Include(x => x.CandidateProviderDocuments)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();
            }

            var applicationUsers = await this.repository.AllReadonly<ApplicationUser>().Include(x => x.Person).AsNoTracking().ToListAsync();
            var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;

            var candidateProviderDocuments = candidateProviderFromDb.CandidateProviderDocuments;
            foreach (var document in candidateProviderDocuments)
            {
                var person = applicationUsers.FirstOrDefault(x => x.IdUser == document.IdCreateUser)?.Person;
                CandidateProviderDocumentsGridData entry = new CandidateProviderDocumentsGridData()
                {
                    EntityId = document.IdCandidateProviderDocument,
                    EntityType = "CandidateProviderDocument",
                    IdDocumentType = document.IdDocumentType,
                    DocumentTypeName = document.IdDocumentType != 0 ? kvProviderDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType)?.Name : string.Empty,
                    DocumentTitle = document.DocumentTitle,
                    UploadedFileName = document.UploadedFileName,
                    UploadedByName = $"{person?.FirstName} {person?.FamilyName}",
                    CreationDate = $"{document.CreationDate.ToString("dd.MM.yyyy")} г.",
                    IsAdditionalDocument = document.IsAdditionalDocument
                };

                this.SetFileName(entry, settingResource);
                data.Add(entry);
            }

            var candidateProviderTrainers = candidateProviderFromDb.CandidateProviderTrainers;
            foreach (var trainer in candidateProviderTrainers)
            {
                foreach (var document in trainer.CandidateProviderTrainerDocuments)
                {
                    var person = applicationUsers.FirstOrDefault(x => x.IdUser == document.IdCreateUser)?.Person;
                    CandidateProviderDocumentsGridData entry = new CandidateProviderDocumentsGridData()
                    {
                        EntityId = document.IdCandidateProviderTrainerDocument,
                        EntityType = "CandidateProviderTrainerDocument",
                        IdDocumentType = document.IdDocumentType,
                        DocumentTypeName = document.IdDocumentType != 0 ? kvTrainerDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType)?.Name : string.Empty,
                        DocumentTitle = document.DocumentTitle,
                        UploadedFileName = document.UploadedFileName,
                        UploadedByName = $"{person?.FirstName} {person?.FamilyName}",
                        CreationDate = $"{document.CreationDate.ToString("dd.MM.yyyy")} г."
                    };

                    this.SetFileName(entry, settingResource);
                    data.Add(entry);
                }
            }

            var candidateProviderPremises = candidateProviderFromDb.CandidateProviderPremises;
            foreach (var premises in candidateProviderPremises)
            {
                foreach (var document in premises.CandidateProviderPremisesDocuments)
                {
                    var person = applicationUsers.FirstOrDefault(x => x.IdUser == document.IdCreateUser)?.Person;
                    CandidateProviderDocumentsGridData entry = new CandidateProviderDocumentsGridData()
                    {
                        EntityId = document.IdCandidateProviderPremisesDocument,
                        EntityType = "CandidateProviderPremisesDocument",
                        IdDocumentType = document.IdDocumentType,
                        DocumentTypeName = document.IdDocumentType != 0 ? kvMTBDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType)?.Name : string.Empty,
                        DocumentTitle = document.DocumentTitle,
                        UploadedFileName = document.UploadedFileName,
                        UploadedByName = $"{person?.FirstName} {person?.FamilyName}",
                        CreationDate = $"{document.CreationDate.ToString("dd.MM.yyyy")} г."
                    };

                    this.SetFileName(entry, settingResource);
                    data.Add(entry);
                }
            }

            return data.OrderByDescending(x => DateTime.Parse(x.CreationDate)).ThenBy(x => x.IdDocumentType).ThenBy(x => x.DocumentTypeName).ToList();
        }

        private void SetFileName(CandidateProviderDocumentsGridData candidateProviderTrainerDocument, string settingResourse)
        {
            if (!string.IsNullOrEmpty(candidateProviderTrainerDocument.UploadedFileName))
            {
                var fileFullName = settingResourse + (candidateProviderTrainerDocument.UploadedFileName.StartsWith("\\")
                ? candidateProviderTrainerDocument.UploadedFileName
                : "\\" + candidateProviderTrainerDocument.UploadedFileName);
                if (Directory.Exists(fileFullName))
                {
                    var files = Directory.GetFiles(fileFullName);
                    files = files.Select(x => x.Split(($"\\{candidateProviderTrainerDocument.EntityId}\\"), StringSplitOptions.RemoveEmptyEntries).LastOrDefault()).ToArray();
                    candidateProviderTrainerDocument.FileName = string.Join(Environment.NewLine, files);
                }
            }
        }
        #endregion

        #region Trainer
        public async Task<CandidateProviderTrainerVM> GetCandidateProviderTrainerByIdAsync(CandidateProviderTrainerVM candidateProviderTrainerVM)
        {
            IQueryable<CandidateProviderTrainer> candidateProviderTrainers = this.repository.AllReadonly<CandidateProviderTrainer>(x => x.IdCandidateProviderTrainer == candidateProviderTrainerVM.IdCandidateProviderTrainer);

            var result = candidateProviderTrainers.To<CandidateProviderTrainerVM>(x =>
            x.CandidateProviderTrainerProfiles,
            x => x.CandidateProviderTrainerQualifications,
            x => x.CandidateProviderTrainerSpecialities.Select(y => y.Speciality),
            x => x.CandidateProviderTrainerDocuments,
            x => x.CandidateProviderTrainerCheckings
            );

            return await result.FirstOrDefaultAsync();
        }

        public async Task<CandidateProviderTrainerVM> GetCandidateProviderTrainerWithDocumentsByIdAsync(CandidateProviderTrainerVM candidateProviderTrainerVM)
        {
            IQueryable<CandidateProviderTrainer> candidateProviderTrainers = this.repository.AllReadonly<CandidateProviderTrainer>(x => x.IdCandidateProviderTrainer == candidateProviderTrainerVM.IdCandidateProviderTrainer);

            var result = candidateProviderTrainers.To<CandidateProviderTrainerVM>(x => x.CandidateProviderTrainerDocuments);

            return await result.FirstOrDefaultAsync();
        }

        public async Task<CandidateProviderTrainerVM> GetCandidateProviderTrainerWithoutIncludesByIdAsync(int idCandidateProviderTrainer)
        {
            return await this.repository.AllReadonly<CandidateProviderTrainer>(x => x.IdCandidateProviderTrainer == idCandidateProviderTrainer).To<CandidateProviderTrainerVM>().FirstOrDefaultAsync();
        }

        public IEnumerable<CandidateProviderTrainerVM> GetCandidateProviderTrainersByCandidateProviderId(CandidateProviderVM candidateProviderVM)
        {
            IQueryable<CandidateProviderTrainer> data = this.repository.All<CandidateProviderTrainer>(z => z.IdCandidate_Provider == candidateProviderVM.IdCandidate_Provider);

            var result = data.To<CandidateProviderTrainerVM>(
                p => p.CandidateProviderTrainerProfiles,
                p => p.CandidateProviderTrainerCheckings,
                p => p.CandidateProviderTrainerSpecialities.Select(z => z.Speciality),
                p => p.CandidateProviderTrainerDocuments).ToList();

            return result;

        }

        public async Task<IEnumerable<CandidateProviderTrainerVM>> GetCandidateProviderTrainersWithStatusByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var trainers = await this.repository.AllReadonly<CandidateProviderTrainer>(x => x.IdCandidate_Provider == idCandidateProvider).To<CandidateProviderTrainerVM>().ToListAsync();
            var statusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderTrainerStatus");
            foreach (var trainer in trainers)
            {
                if (trainer.IdStatus.HasValue)
                {
                    var statusValue = statusSource.FirstOrDefault(x => x.IdKeyValue == trainer.IdStatus.Value);
                    if (statusValue is not null)
                    {
                        trainer.StatusName = statusValue.Name;
                    }
                }
            }

            return trainers.OrderBy(x => x.FirstName).ThenBy(x => x.SecondName).ThenBy(x => x.FamilyName).ToList();
        }

        public async Task<IEnumerable<CandidateProviderTrainerVM>> GetAllActiveCandidateProviderTrainersByCandidateProviderIdWithTrainerSpecialitiesIncludedAsync(int idCandidateProvider)
        {
            var kvActiveStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");

            IQueryable<CandidateProviderTrainer> data = this.repository.AllReadonly<CandidateProviderTrainer>(z => z.IdCandidate_Provider == idCandidateProvider && z.IdStatus == kvActiveStatus.IdKeyValue);

            var result = await data.To<CandidateProviderTrainerVM>(p => p.CandidateProviderTrainerSpecialities).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<CandidateProviderTrainerVM>> GetAllCandidateProviderTrainersByCandidateProviderIdWithTrainerSpecialitiesIncludedAsync(int idCandidateProvider)
        {

            IQueryable<CandidateProviderTrainer> data = this.repository.AllReadonly<CandidateProviderTrainer>(z => z.IdCandidate_Provider == idCandidateProvider);

            var result = await data.To<CandidateProviderTrainerVM>(p => p.CandidateProviderTrainerSpecialities).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<CandidateProviderTrainerVM>> GetAllActiveTrainersByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var kvTrainerActive = await this.dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");
            var data = this.repository.AllReadonly<CandidateProviderTrainer>(x => x.IdCandidate_Provider == idCandidateProvider && x.IdStatus == kvTrainerActive.IdKeyValue);

            return await data.To<CandidateProviderTrainerVM>().OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ToListAsync();
        }

        public async Task<CandidateProviderTrainerVM> GetCandidateProviderTrainerForRegisterByIdAsync(int idCandidateProviderTrainer)
        {
            var trainerFromDb = await this.repository.AllReadonly<CandidateProviderTrainer>(x => x.IdCandidateProviderTrainer == idCandidateProviderTrainer)
                .To<CandidateProviderTrainerVM>(x => x.CandidateProviderTrainerQualifications.Select(y => y.Profession),
                    x => x.CandidateProviderTrainerProfiles.Select(y => y.ProfessionalDirection),
                    x => x.CandidateProviderTrainerDocuments).FirstOrDefaultAsync();

            if (trainerFromDb is not null)
            {
                if (trainerFromDb.IdSex.HasValue)
                {
                    trainerFromDb.SexValue = (await this.dataSourceService.GetKeyValueByIdAsync(trainerFromDb.IdSex.Value))?.Name;
                }

                if (trainerFromDb.IdIndentType.HasValue)
                {
                    trainerFromDb.IndentTypeValue = (await this.dataSourceService.GetKeyValueByIdAsync(trainerFromDb.IdIndentType.Value))?.Name;
                }

                if (trainerFromDb.IdNationality.HasValue)
                {
                    trainerFromDb.NationalityValue = (await this.dataSourceService.GetKeyValueByIdAsync(trainerFromDb.IdNationality.Value))?.Name;
                }

                if (trainerFromDb.IdContractType.HasValue)
                {
                    trainerFromDb.ContractTypeValue = (await this.dataSourceService.GetKeyValueByIdAsync(trainerFromDb.IdContractType.Value))?.Name;
                }

                if (trainerFromDb.CandidateProviderTrainerDocuments.Any())
                {
                    var documentTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerDocumentType", false, true);
                    foreach (var doc in trainerFromDb.CandidateProviderTrainerDocuments)
                    {
                        if (doc.IdDocumentType != 0)
                        {
                            var docTypeValue = documentTypesSource.FirstOrDefault(x => x.IdKeyValue == doc.IdDocumentType);
                            if (docTypeValue is not null)
                            {
                                doc.DocumentTypeName = docTypeValue.Name;
                            }
                        }

                        doc.UploadedByName = await this.applicationUserService.GetApplicationUsersPersonNameAsync(doc.IdCreateUser);
                    }
                }
                if (trainerFromDb.IdEducation > 0)
                {
                    trainerFromDb.EducationValue = (await this.dataSourceService.GetKeyValueByIdAsync(trainerFromDb.IdEducation))?.Name;
                }
            }

            return trainerFromDb;
        }

        public async Task CreateCandidateProviderTrainerAsync(CandidateProviderTrainerVM model)
        {
            var trainerForDb = model.To<CandidateProviderTrainer>();
            trainerForDb.CandidateProvider = null;
            trainerForDb.CandidateProviderTrainerCheckings = null;
            trainerForDb.CandidateProviderTrainerDocuments = null;
            trainerForDb.CandidateProviderTrainerProfiles = null;
            trainerForDb.CandidateProviderTrainerQualifications = null;
            trainerForDb.CandidateProviderTrainerSpecialities = null;

            await this.repository.AddAsync<CandidateProviderTrainer>(trainerForDb);
            await this.repository.SaveChangesAsync();

            model.IdCandidateProviderTrainer = trainerForDb.IdCandidateProviderTrainer;
            model.IdModifyUser = trainerForDb.IdModifyUser;
            model.IdCreateUser = trainerForDb.IdCreateUser;
            model.ModifyDate = trainerForDb.ModifyDate;
            model.CreationDate = trainerForDb.CreationDate;

            if (model.IdStatus.HasValue)
            {
                model.StatusName = (await this.dataSourceService.GetKeyValueByIdAsync(model.IdStatus.Value))?.Name;
            }
        }

        public async Task UpdateCandidateProviderTrainerAsync(CandidateProviderTrainerVM model)
        {
            var trainerFromDb = await this.repository.GetByIdAsync<CandidateProviderTrainer>(model.IdCandidateProviderTrainer);
            if (trainerFromDb is not null)
            {
                trainerFromDb = model.To<CandidateProviderTrainer>();
                trainerFromDb.CandidateProvider = null;
                trainerFromDb.CandidateProviderTrainerCheckings = null;
                trainerFromDb.CandidateProviderTrainerDocuments = null;
                trainerFromDb.CandidateProviderTrainerProfiles = null;
                trainerFromDb.CandidateProviderTrainerQualifications = null;
                trainerFromDb.CandidateProviderTrainerSpecialities = null;

                this.repository.Update<CandidateProviderTrainer>(trainerFromDb);
                await this.repository.SaveChangesAsync();

                if (model.IdStatus.HasValue)
                {
                    model.StatusName = (await this.dataSourceService.GetKeyValueByIdAsync(model.IdStatus.Value))?.Name;
                }
            }
        }
        #endregion

        #region Trainer Qualification
        public async Task<CandidateProviderTrainerQualificationVM> GetCandidateProviderTrainerQualificationByIdAsync(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM)
        {
            IQueryable<CandidateProviderTrainerQualification> providerTrainerQualification = this.repository.AllReadonly<CandidateProviderTrainerQualification>(x => x.IdCandidateProviderTrainerQualification == candidateProviderTrainerQualificationVM.IdCandidateProviderTrainerQualification);
            var result = providerTrainerQualification.To<CandidateProviderTrainerQualificationVM>();

            return await result.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CandidateProviderTrainerQualificationVM>> GetAllCandidateProviderTrainerQualificationsByCandidateProviderTrainerIdAsync(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM)
        {
            IQueryable<CandidateProviderTrainerQualification> providerTrainerQualifications = this.repository.AllReadonly<CandidateProviderTrainerQualification>(x => x.IdCandidateProviderTrainer == candidateProviderTrainerQualificationVM.IdCandidateProviderTrainer);
            var result = providerTrainerQualifications.To<CandidateProviderTrainerQualificationVM>();

            return await result.ToListAsync();
        }

        public async Task<List<CandidateProviderTrainerProfileVM>> GetAllCandidateProviderTrainerProfilesByCandidateProviderTrainerIdAsync(int IdCandidateProviderTrainer)
        {
            IQueryable<CandidateProviderTrainerProfile> providerTrainerProfiles = this.repository.AllReadonly<CandidateProviderTrainerProfile>(x => x.IdCandidateProviderTrainer == IdCandidateProviderTrainer);
            var result = providerTrainerProfiles.To<CandidateProviderTrainerProfileVM>();

            return result.ToList();
        }

        public async Task<ResultContext<CandidateProviderTrainerQualificationVM>> CreateCandidateProviderTrainerQualificationAsync(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM)
        {
            ResultContext<CandidateProviderTrainerQualificationVM> outputContext = new ResultContext<CandidateProviderTrainerQualificationVM>();

            try
            {

                var entryForDb = candidateProviderTrainerQualificationVM.To<CandidateProviderTrainerQualification>();
                entryForDb.Profession = null;
                entryForDb.CandidateProviderTrainer = null;

                await this.repository.AddAsync<CandidateProviderTrainerQualification>(entryForDb);
                await this.repository.SaveChangesAsync();

                outputContext.AddMessage("Записът е успешен!");
                candidateProviderTrainerQualificationVM.IdCandidateProviderTrainerQualification = entryForDb.IdCandidateProviderTrainerQualification;
                outputContext.ResultContextObject = candidateProviderTrainerQualificationVM;

                this.repository.Detach<CandidateProviderTrainerQualification>(entryForDb);
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<CandidateProviderTrainerQualificationVM>> UpdateCandidateProviderTrainerQualificationAsync(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM)
        {
            ResultContext<CandidateProviderTrainerQualificationVM> outputContext = new ResultContext<CandidateProviderTrainerQualificationVM>();

            try
            {
                var entity = await this.repository.GetByIdAsync<CandidateProviderTrainerQualification>(candidateProviderTrainerQualificationVM.IdCandidateProviderTrainerQualification);
                this.repository.Detach<CandidateProviderTrainerQualification>(entity);


                entity = candidateProviderTrainerQualificationVM.To<CandidateProviderTrainerQualification>();
                entity.Profession = null;
                entity.CandidateProviderTrainer = null;

                this.repository.Update<CandidateProviderTrainerQualification>(entity);
                await this.repository.SaveChangesAsync();
                this.repository.Detach<CandidateProviderTrainerQualification>(entity);

                outputContext.AddMessage("Записът е успешен!");
                outputContext.ResultContextObject = candidateProviderTrainerQualificationVM;
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<CandidateProviderTrainerQualificationVM>> DeleteCandidateProviderTrainerQualificationAsync(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM)
        {
            var entity = await this.repository.GetByIdAsync<CandidateProviderTrainerQualification>(candidateProviderTrainerQualificationVM.IdCandidateProviderTrainerQualification);
            this.repository.Detach<CandidateProviderTrainerQualification>(entity);

            ResultContext<CandidateProviderTrainerQualificationVM> resultContext = new ResultContext<CandidateProviderTrainerQualificationVM>();

            try
            {
                this.repository.HardDelete<CandidateProviderTrainerQualification>(entity);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Квалификацията беше изтрита успешно!");
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


        public async Task<MemoryStream> GenerateExcelReportForCandidateProviderTrainerQualification(string year)
        {
            try
            {
                var trainerQualifications = this.repository
                    .All<CandidateProviderTrainerQualification>()
                    .To<CandidateProviderTrainerQualificationVM>(x => x.CandidateProviderTrainer.CandidateProvider, x => x.Profession)
                    .Where(x => x.CreationDate.Year.ToString().Equals(year))
                    .ToList();

                var qualificationType = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("QualificationType")).ToList();

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    int row = 1;

                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Xlsx;

                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    foreach (var trainerq in trainerQualifications)
                    {
                        if (trainerq.IdTrainingQualificationType != 0)
                        {
                            trainerq.TrainingQualificationType = qualificationType.Where(x => x.IdKeyValue == trainerq.IdQualificationType).First();
                        }

                        if (trainerq.QualificationDuration == null)
                        {
                            trainerq.QualificationDuration = 0;
                        }

                        //Pitai reni zashto nqma profesiq
                        if (trainerq.Profession == null)
                        {
                            trainerq.Profession = new ProfessionVM();
                            trainerq.Profession.Code = "";
                        }


                        object[] excelRow = new object[7]
                        {
                        row,
                        year,
                        trainerq.CandidateProviderTrainer.CandidateProvider.LicenceNumber,
                        trainerq.CandidateProviderTrainer.CandidateProvider.PoviderBulstat,
                        trainerq.Profession.Code,
                       trainerq.IdTrainingQualificationType != 0 ? trainerq.TrainingQualificationType.DefaultValue2 : "0",
                        trainerq.QualificationDuration
                        };
                        worksheet.ImportArray(excelRow, row, 1, false);
                        row++;
                    }
                    if (trainerQualifications.Count() > 0)
                    {
                        worksheet.Range[$"A1:G{trainerQualifications.Count()}"].AutofitColumns();
                        worksheet.Range[$"A1:G{trainerQualifications.Count()}"].BorderInside(ExcelLineStyle.Medium);
                        worksheet.Range[$"A1:G{trainerQualifications.Count()}"].BorderAround(ExcelLineStyle.Medium);
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
        #endregion

        #region Trainer Document
        public async Task<CandidateProviderTrainerDocumentVM> GetCandidateProviderTrainerDocumentByIdAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM)
        {
            IQueryable<CandidateProviderTrainerDocument> providerTrainerDocument = this.repository.AllReadonly<CandidateProviderTrainerDocument>(x => x.IdCandidateProviderTrainerDocument == candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument);

            return await providerTrainerDocument.To<CandidateProviderTrainerDocumentVM>().FirstOrDefaultAsync();
        }

        public async Task<ResultContext<CandidateProviderTrainerDocumentVM>> CreateCandidateProviderTrainerDocumentAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM)
        {
            ResultContext<CandidateProviderTrainerDocumentVM> outputContext = new ResultContext<CandidateProviderTrainerDocumentVM>();

            try
            {


                var entryForDb = candidateProviderTrainerDocumentVM.To<CandidateProviderTrainerDocument>();
                entryForDb.CandidateProviderTrainer = null;

                await this.repository.AddAsync<CandidateProviderTrainerDocument>(entryForDb);
                await this.repository.SaveChangesAsync();

                outputContext.AddMessage("Записът е успешен!");
                candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument = entryForDb.IdCandidateProviderTrainerDocument;
                outputContext.ResultContextObject = candidateProviderTrainerDocumentVM;

                this.repository.Detach<CandidateProviderTrainerDocument>(entryForDb);
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<CandidateProviderTrainerDocumentVM>> UpdateCandidateProviderTrainerDocumentAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM)
        {
            ResultContext<CandidateProviderTrainerDocumentVM> outputContext = new ResultContext<CandidateProviderTrainerDocumentVM>();

            try
            {
                var entity = await this.repository.GetByIdAsync<CandidateProviderTrainerDocument>(candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument);
                this.repository.Detach<CandidateProviderTrainerDocument>(entity);

                candidateProviderTrainerDocumentVM.IdCreateUser = entity.IdCreateUser;
                candidateProviderTrainerDocumentVM.CreationDate = entity.CreationDate;
                entity = candidateProviderTrainerDocumentVM.To<CandidateProviderTrainerDocument>();
                entity.CandidateProviderTrainer = null;

                this.repository.Update<CandidateProviderTrainerDocument>(entity);
                await this.repository.SaveChangesAsync();
                this.repository.Detach<CandidateProviderTrainerDocument>(entity);

                outputContext.AddMessage("Записът е успешен!");
                outputContext.ResultContextObject = candidateProviderTrainerDocumentVM;
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<CandidateProviderTrainerDocumentVM>> DeleteCandidateProviderTrainerDocumentAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM)
        {
            var entity = await this.repository.GetByIdAsync<CandidateProviderTrainerDocument>(candidateProviderTrainerDocumentVM.IdCandidateProviderTrainerDocument);

            ResultContext<CandidateProviderTrainerDocumentVM> resultContext = new ResultContext<CandidateProviderTrainerDocumentVM>();

            try
            {
                this.repository.HardDelete<CandidateProviderTrainerDocument>(entity);
                await this.repository.SaveChangesAsync();

                var settingsFolder = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var pathToFile = settingsFolder + "\\" + entity.UploadedFileName;
                if (entity.UploadedFileName != null)
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

        public async Task<IEnumerable<CandidateProviderTrainerDocumentVM>> GetAllCandidateProviderTrainerDocumentsByCandidateProviderTrainerIdAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM)
        {
            IQueryable<CandidateProviderTrainerDocument> providerTrainerDocuments = this.repository.AllReadonly<CandidateProviderTrainerDocument>(x => x.IdCandidateProviderTrainer == candidateProviderTrainerDocumentVM.IdCandidateProviderTrainer);
            var result = providerTrainerDocuments.To<CandidateProviderTrainerDocumentVM>();

            return await result.ToListAsync();
        }
        #endregion

        #region Trainer Speciality
        public async Task<CandidateProviderTrainerSpecialityVM> GetCandidateProviderSpecialityByIdSpecialityAndIdCandidateProviderTrainerAsync(int idSpeciality, int idCandidateProviderTrainer, int idUsage)
        {
            var kvTheoryAndPractice = await this.dataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TrainingInTheoryAndPractice");
            var result = this.repository.AllReadonly<CandidateProviderTrainerSpeciality>(x => x.IdSpeciality == idSpeciality && x.IdCandidateProviderTrainer == idCandidateProviderTrainer && (x.IdUsage == idUsage || x.IdUsage == kvTheoryAndPractice.IdKeyValue));

            return await result.To<CandidateProviderTrainerSpecialityVM>().FirstOrDefaultAsync();
        }

        public List<CandidateProviderTrainerSpecialityVM> GetCandidateProviderTrainerSpecialitiesWithSpecialitiesByIdCandidateProviderTrainer(int idCandidateProviderTrainer)
        {
            var result = this.repository.AllReadonly<CandidateProviderTrainerSpeciality>(x => x.IdCandidateProviderTrainer == idCandidateProviderTrainer);

            return result.To<CandidateProviderTrainerSpecialityVM>(x => x.Speciality).ToList();
        }

        public async Task<ResultContext<NoResult>> AddSpecialitiesToTrainerByListSpecialitiesAsync(List<CandidateProviderTrainerSpecialityVM> trainerSpecialities)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                foreach (var trainerSpeciality in trainerSpecialities)
                {
                    var trainerSpecialityForDb = trainerSpeciality.To<CandidateProviderTrainerSpeciality>();
                    await this.repository.AddAsync<CandidateProviderTrainerSpeciality>(trainerSpecialityForDb);
                }

                await this.repository.SaveChangesAsync();

                var msg = trainerSpecialities.Count == 1
                    ? "Специалността е добавена успешно!"
                    : "Специалностите са добавени успешно!";

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

        public async Task<ResultContext<CandidateProviderTrainerSpecialityVM>> DeleteCandidateProviderTrainerSpecialityAsync(CandidateProviderTrainerSpecialityVM candidateProviderTrainerSpecialityVM, int idUsage)
        {
            var kvTheory = await this.dataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TheoryTraining");
            var kvPractice = await this.dataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "PracticalTraining");
            var entity = await this.repository.GetByIdAsync<CandidateProviderTrainerSpeciality>(candidateProviderTrainerSpecialityVM.IdCandidateProviderTrainerSpeciality);
            ResultContext<CandidateProviderTrainerSpecialityVM> resultContext = new ResultContext<CandidateProviderTrainerSpecialityVM>();
            try
            {
                if (entity is not null)
                {
                    if (entity.IdUsage == idUsage)
                    {
                        await this.repository.HardDeleteAsync<CandidateProviderTrainerSpeciality>(entity.IdCandidateProviderTrainerSpeciality);
                    }
                    else
                    {
                        if (idUsage == kvTheory.IdKeyValue)
                        {
                            entity.IdUsage = kvPractice.IdKeyValue;
                        }
                        else
                        {
                            entity.IdUsage = kvTheory.IdKeyValue;
                        }

                        this.repository.Update<CandidateProviderTrainerSpeciality>(entity);
                    }
                }

                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Специалността е изтрита успешно!");
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

        public async Task<ResultContext<CandidateProviderTrainerCheckingVM>> DeleteCandidateProviderTrainerCheckingAsync(CandidateProviderTrainerCheckingVM candidateProviderTrainerCheckingVM)
        {
            var entity = await this.repository.GetByIdAsync<CandidateProviderTrainerChecking>(candidateProviderTrainerCheckingVM.IdCandidateProviderTrainerChecking);
            this.repository.Detach<CandidateProviderTrainerChecking>(entity);

            ResultContext<CandidateProviderTrainerCheckingVM> resultContext = new ResultContext<CandidateProviderTrainerCheckingVM>();

            try
            {
                this.repository.HardDelete<CandidateProviderTrainerChecking>(entity);
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

        #region Trainer Profile
        public async Task<ResultContext<CandidateProviderTrainerProfileVM>> DeleteCandidateProviderTrainerProfileAsync(CandidateProviderTrainerProfileVM candidateProviderTrainerSpecialityVM)
        {
            var entity = await this.repository.GetByIdAsync<CandidateProviderTrainerProfile>(candidateProviderTrainerSpecialityVM.IdCandidateProviderTrainerProfile);
            this.repository.Detach<CandidateProviderTrainerProfile>(entity);

            ResultContext<CandidateProviderTrainerProfileVM> resultContext = new ResultContext<CandidateProviderTrainerProfileVM>();

            try
            {
                this.repository.HardDelete<CandidateProviderTrainerProfile>(entity);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Професионалното направление е изтрито успешно!");
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

        public async Task<ResultContext<CandidateProviderTrainerProfileVM>> CreateCandidateProviderTrainerProfileAsync(ResultContext<CandidateProviderTrainerProfileVM> inputContext)
        {
            var resultContext = new ResultContext<CandidateProviderTrainerProfileVM>();
            var model = inputContext.ResultContextObject;
            try
            {
                var trainerProfileForDb = model.To<CandidateProviderTrainerProfile>();
                trainerProfileForDb.ProfessionalDirection = null;
                trainerProfileForDb.CandidateProviderTrainer = null;

                await this.repository.AddAsync<CandidateProviderTrainerProfile>(trainerProfileForDb);
                var result = await this.repository.SaveChangesAsync();
                if (result == 1)
                {
                    var temp = this.repository.All<CandidateProviderTrainerProfile>().Where(x => x.IdCandidateProviderTrainer == trainerProfileForDb.IdCandidateProviderTrainer).ToList().Last().To<CandidateProviderTrainerProfileVM>();
                    resultContext.ResultContextObject = temp;
                }

                resultContext.AddMessage("Професионалното направление е добавено успешно!");
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

        #region Premises
        public async Task<CandidateProviderPremisesVM> GetCandidateProviderPremisesByIdAsync(CandidateProviderPremisesVM candidateProviderPremisesVM)
        {
            IQueryable<CandidateProviderPremises> data = this.repository.AllReadonly<CandidateProviderPremises>(x => x.IdCandidateProviderPremises == candidateProviderPremisesVM.IdCandidateProviderPremises);
            var result = data.To<CandidateProviderPremisesVM>(x => x.CandidateProviderPremisesRooms, x => x.CandidateProviderPremisesDocuments, x => x.CandidateProviderPremisesCheckings, x => x.Location, x => x.CandidateProviderPremisesSpecialities.Select(y => y.Speciality));

            return await result.FirstOrDefaultAsync();
        }

        public async Task<CandidateProviderPremisesVM> GetCandidateProviderPremisesWithRoomsAndDocumentsByIdAsync(CandidateProviderPremisesVM candidateProviderPremisesVM)
        {
            IQueryable<CandidateProviderPremises> data = this.repository.AllReadonly<CandidateProviderPremises>(x => x.IdCandidateProviderPremises == candidateProviderPremisesVM.IdCandidateProviderPremises);
            var result = data.To<CandidateProviderPremisesVM>(x => x.CandidateProviderPremisesRooms, x => x.CandidateProviderPremisesDocuments);

            return await result.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CandidateProviderPremisesVM>> GetCandidateProviderPremisesWithStatusByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var premises = await this.repository.AllReadonly<CandidateProviderPremises>(x => x.IdCandidate_Provider == idCandidateProvider).To<CandidateProviderPremisesVM>().ToListAsync();
            var statusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseStatus");
            foreach (var mtb in premises)
            {
                if (mtb.IdStatus != 0)
                {
                    var statusValue = statusSource.FirstOrDefault(x => x.IdKeyValue == mtb.IdStatus);
                    if (statusValue is not null)
                    {
                        mtb.StatusValue = statusValue.Name;
                    }
                }
            }

            return premises.OrderBy(x => x.PremisesName).ToList();
        }

        public async Task CreateCandidateProviderPremisesAsync(CandidateProviderPremisesVM model)
        {
            var premisesForDb = model.To<CandidateProviderPremises>();
            premisesForDb.CandidateProvider = null;
            premisesForDb.CandidateProviderPremisesCheckings = null;
            premisesForDb.CandidateProviderPremisesDocuments = null;
            premisesForDb.CandidateProviderPremisesRooms = null;
            premisesForDb.CandidateProviderPremisesSpecialities = null;
            premisesForDb.Location = null;

            await this.repository.AddAsync<CandidateProviderPremises>(premisesForDb);
            await this.repository.SaveChangesAsync();

            model.IdCandidateProviderPremises = premisesForDb.IdCandidateProviderPremises;
            model.IdModifyUser = premisesForDb.IdModifyUser;
            model.IdCreateUser = premisesForDb.IdCreateUser;
            model.ModifyDate = premisesForDb.ModifyDate;
            model.CreationDate = premisesForDb.CreationDate;

            if (model.IdStatus != 0)
            {
                model.StatusValue = (await this.dataSourceService.GetKeyValueByIdAsync(model.IdStatus))?.Name;
            }
        }

        public async Task UpdateCandidateProviderPremisesAsync(CandidateProviderPremisesVM model)
        {
            var premisesFromDb = await this.repository.GetByIdAsync<CandidateProviderPremises>(model.IdCandidateProviderPremises);
            if (premisesFromDb is not null)
            {
                premisesFromDb = model.To<CandidateProviderPremises>();
                premisesFromDb.CandidateProvider = null;
                premisesFromDb.CandidateProviderPremisesCheckings = null;
                premisesFromDb.CandidateProviderPremisesDocuments = null;
                premisesFromDb.CandidateProviderPremisesRooms = null;
                premisesFromDb.CandidateProviderPremisesSpecialities = null;
                premisesFromDb.Location = null;

                this.repository.Update<CandidateProviderPremises>(premisesFromDb);
                await this.repository.SaveChangesAsync();

                if (model.IdStatus != 0)
                {
                    model.StatusValue = (await this.dataSourceService.GetKeyValueByIdAsync(model.IdStatus))?.Name;
                }
            }
        }

        public async Task<IEnumerable<CandidateProviderPremisesVM>> GetCandidateProviderPremisesByIdCandidateProviderAsync(CandidateProviderVM candidateProviderVM)
        {
            IQueryable<CandidateProviderPremises> data = this.repository.AllReadonly<CandidateProviderPremises>(x => x.IdCandidate_Provider == candidateProviderVM.IdCandidate_Provider);
            var result = data.To<CandidateProviderPremisesVM>(x => x.CandidateProviderPremisesSpecialities.Select(y => y.Speciality));

            return result;
        }

        public async Task<IEnumerable<CandidateProviderPremisesVM>> GetCandidateProviderPremisesByIdAsync(CandidateProviderVM candidateProvider)
        {
            IQueryable<CandidateProviderPremises> data = this.repository.All<CandidateProviderPremises>(x => x.IdCandidate_Provider == candidateProvider.IdCandidate_Provider);
            var result = await data.To<CandidateProviderPremisesVM>(
                p => p.CandidateProviderPremisesSpecialities
                    .Select(x => x.Speciality.Profession)).ToListAsync();

            return result;

        }

        public async Task<MemoryStream> GetExcelReportForCandidateProviderPremisies(string year)
        {
            //var premisies = this.repository
            //            .All<CandidateProviderPremises>()
            //            .Include(x => x.CandidateProvider)
            //            .Include(x => x.CandidateProviderPremisesSpecialities)
            //            .ThenInclude(xx => xx.Speciality.Profession)
            //            .To<CandidateProviderPremisesVM>()
            //            .Where(x => x.CreationDate.Year.ToString().Equals(year))
            //            .ToList();
            try
            {
                var premisies = this.repository.All<CandidateProviderPremises>()
                .To<CandidateProviderPremisesVM>(x => x.CandidateProvider, x => x.CandidateProviderPremisesSpecialities)
                .ToList();

                var ComplianceDOC = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ComplianceDOC")).ToList();

                var MaterialTechnicalBaseOwnership = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseOwnership")).ToList();


                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    int row = 1;

                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Xlsx;

                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet worksheet = workbook.Worksheets[0];

                    foreach (var premis in premisies)
                    {
                        foreach (var speciality in premis.CandidateProviderPremisesSpecialities)
                        {

                            speciality.Speciality = (await this.repository.GetByIdAsync<Speciality>(speciality.IdSpeciality)).To<SpecialityVM>();
                            speciality.Speciality.Profession = (await this.repository.GetByIdAsync<Profession>(speciality.Speciality.IdProfession)).To<ProfessionVM>();
                            if (speciality.IdComplianceDOC != 0)
                            {
                                speciality.ComplianceDOC = ComplianceDOC.Where(x => x.IdKeyValue == speciality.IdComplianceDOC).First();
                            }
                            if (premis.IdStatus != 0)
                            {
                                premis.Status = MaterialTechnicalBaseOwnership.Where(x => x.IdKeyValue == premis.IdOwnership).First();
                            }
                            object[] excelRow = new object[9]
                            {
                                row,
                                year,
                                premis.CandidateProvider.LicenceNumber,
                                premis.CandidateProvider.PoviderBulstat,
                                speciality.Speciality.Profession.Code,
                                speciality.Speciality.Code,
                                speciality.ComplianceDOC != null ? speciality.ComplianceDOC.DefaultValue2 : "",
                                premis.Status.DefaultValue2,
                                premis.PremisesName
                            };

                            worksheet.ImportArray(excelRow, row, 1, false);
                            row++;
                        }
                        if (premisies.Count > 0)
                        {
                            worksheet.Range[$"A1:I{premisies.Count()}"].AutofitColumns();
                            worksheet.Range[$"A1:I{premisies.Count()}"].BorderInside(ExcelLineStyle.Medium);
                            worksheet.Range[$"A1:I{premisies.Count()}"].BorderAround(ExcelLineStyle.Medium);
                        }

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

        public async Task<CandidateProviderPremisesCheckingVM> GetCandidateProviderPremisesCheckingAsync(int IdCandidateProviderPremisesCheckings)
        {
            var result = this.repository.AllReadonly<CandidateProviderPremisesChecking>(x => x.IdCandidateProviderPremisesChecking == IdCandidateProviderPremisesCheckings);

            return await result.To<CandidateProviderPremisesCheckingVM>().FirstOrDefaultAsync();
        }

        public async Task<List<CandidateProviderPremisesCheckingVM>> GetAllActiveCandidateProviderPremisesCheckingAsync(int IdCandidateProviderPremises)
        {
            var result = this.repository.AllReadonly<CandidateProviderPremisesChecking>(x => x.IdCandidateProviderPremises == IdCandidateProviderPremises);

            return result.To<CandidateProviderPremisesCheckingVM>().ToList();
        }

        public async Task<List<CandidateProviderTrainerCheckingVM>> GetAllActiveCandidateProviderTrainerCheckingAsync(int IdCandidateProviderTrainer)
        {
            var result = this.repository.AllReadonly<CandidateProviderTrainerChecking>(x => x.IdCandidateProviderTrainer == IdCandidateProviderTrainer);

            return result.To<CandidateProviderTrainerCheckingVM>().ToList();
        }
        public async Task<List<CandidateProviderPremisesCheckingVM>> GetAllActiveCandidateProviderPremisesCheckingByIdFollowUpControlAsync(int IdFollowUpControl)
        {
            var result = this.repository.AllReadonly<CandidateProviderPremisesChecking>(x => x.IdFollowUpControl == IdFollowUpControl);

            return result.To<CandidateProviderPremisesCheckingVM>(x => x.CandidateProviderPremises.Location).ToList();
        }

        public async Task<List<CandidateProviderTrainerCheckingVM>> GetAllActiveCandidateProviderTrainerCheckingByIdFollowUpControlAsync(int IdFollowUpControl)
        {
            var result = this.repository.AllReadonly<CandidateProviderTrainerChecking>(x => x.IdFollowUpControl == IdFollowUpControl);

            return result.To<CandidateProviderTrainerCheckingVM>(x => x.CandidateProviderTrainer).ToList();
        }

        public IEnumerable<CandidateProviderPremisesVM> GetCandidateProviderPremisesBySpeciality(CandidateProviderSpecialityVM candidateProviderSpeciality)
        {
            IQueryable<CandidateProviderPremises> data = this.repository.All<CandidateProviderPremises>(x => x.IdCandidate_Provider == candidateProviderSpeciality.IdCandidate_Provider);
            var result = data.To<CandidateProviderPremisesVM>(
                p => p.CandidateProviderPremisesSpecialities
                    .Select(x => x.Speciality.Profession)).ToList();

            return result;

        }

        public async Task<IEnumerable<CandidateProviderPremisesVM>> GetAllActivePremisesByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var kvPremisesActive = await this.dataSourceService.GetKeyValueByIntCodeAsync("MaterialTechnicalBaseStatus", "Active");
            var data = this.repository.AllReadonly<CandidateProviderPremises>(x => x.IdCandidate_Provider == idCandidateProvider && x.IdStatus == kvPremisesActive.IdKeyValue);

            return await data.To<CandidateProviderPremisesVM>(x => x.Location).OrderBy(x => x.PremisesName).ToListAsync();
        }

        public async Task<IEnumerable<CandidateProviderPremisesVM>> GetAllActiveCandidateProviderPremisesByCandidateProviderIdWithPremisesSpecialitiesIncludedAsync(int idCandidateProvider)
        {
            var kvActiveStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("MaterialTechnicalBaseStatus", "Active");

            IQueryable<CandidateProviderPremises> data = this.repository.AllReadonly<CandidateProviderPremises>(z => z.IdCandidate_Provider == idCandidateProvider && z.IdStatus == kvActiveStatus.IdKeyValue);

            var result = await data.To<CandidateProviderPremisesVM>(p => p.CandidateProviderPremisesSpecialities,
            p => p.CandidateProviderPremisesSpecialities.Select(x => x.Speciality)).ToListAsync();

            return result;
        }

        public async Task<IEnumerable<CandidateProviderPremisesVM>> GetAllCandidateProviderPremisesByCandidateProviderIdWithPremisesSpecialitiesIncludedAsync(int idCandidateProvider)
        {
            IQueryable<CandidateProviderPremises> data = this.repository.AllReadonly<CandidateProviderPremises>(z => z.IdCandidate_Provider == idCandidateProvider);

            var result = await data.To<CandidateProviderPremisesVM>(p => p.CandidateProviderPremisesSpecialities, p => p.Location).ToListAsync();

            return result;
        }

        #endregion

        #region Premises Room
        public async Task<CandidateProviderPremisesRoomVM> GetCandidateProviderPremisesRoomByIdAsync(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM)
        {
            IQueryable<CandidateProviderPremisesRoom> candidateProviderPremisesRooms = this.repository.AllReadonly<CandidateProviderPremisesRoom>(x => x.IdCandidateProviderPremisesRoom == candidateProviderPremisesRoomVM.IdCandidateProviderPremisesRoom);
            var result = candidateProviderPremisesRooms.To<CandidateProviderPremisesRoomVM>();

            return await result.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CandidateProviderPremisesRoomVM>> GetAllCandidateProviderPremisesRoomsByCandidateProviderPremisesIdAsync(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM)
        {
            IQueryable<CandidateProviderPremisesRoom> candidateProviderPremisesRooms = this.repository.AllReadonly<CandidateProviderPremisesRoom>(x => x.IdCandidateProviderPremises == candidateProviderPremisesRoomVM.IdCandidateProviderPremises);
            var result = candidateProviderPremisesRooms.To<CandidateProviderPremisesRoomVM>();

            return await result.ToListAsync();
        }

        public async Task<ResultContext<CandidateProviderPremisesRoomVM>> UpdateCandidateProviderPremisesRoomAsync(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM)
        {
            ResultContext<CandidateProviderPremisesRoomVM> outputContext = new ResultContext<CandidateProviderPremisesRoomVM>();

            try
            {
                var entity = await this.repository.GetByIdAsync<CandidateProviderPremisesRoom>(candidateProviderPremisesRoomVM.IdCandidateProviderPremisesRoom);
                this.repository.Detach<CandidateProviderPremisesRoom>(entity);


                entity = candidateProviderPremisesRoomVM.To<CandidateProviderPremisesRoom>();
                entity.CandidateProviderPremises = null;

                this.repository.Update<CandidateProviderPremisesRoom>(entity);
                await this.repository.SaveChangesAsync();
                this.repository.Detach<CandidateProviderPremisesRoom>(entity);

                outputContext.AddMessage("Записът е успешен!");
                outputContext.ResultContextObject = candidateProviderPremisesRoomVM;
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<CandidateProviderPremisesRoomVM>> CreateCandidateProviderPremisesRoomAsync(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM)
        {
            ResultContext<CandidateProviderPremisesRoomVM> outputContext = new ResultContext<CandidateProviderPremisesRoomVM>();

            try
            {


                var entryForDb = candidateProviderPremisesRoomVM.To<CandidateProviderPremisesRoom>();
                entryForDb.CandidateProviderPremises = null;

                await this.repository.AddAsync<CandidateProviderPremisesRoom>(entryForDb);
                await this.repository.SaveChangesAsync();

                outputContext.AddMessage("Записът е успешен!");
                candidateProviderPremisesRoomVM.IdCandidateProviderPremisesRoom = entryForDb.IdCandidateProviderPremisesRoom;
                outputContext.ResultContextObject = candidateProviderPremisesRoomVM;

                this.repository.Detach<CandidateProviderPremisesRoom>(entryForDb);
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<CandidateProviderPremisesRoomVM>> DeleteCandidateProviderPremisesRoomAsync(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM)
        {
            var entity = await this.repository.GetByIdAsync<CandidateProviderPremisesRoom>(candidateProviderPremisesRoomVM.IdCandidateProviderPremisesRoom);
            this.repository.Detach<CandidateProviderPremisesRoom>(entity);

            ResultContext<CandidateProviderPremisesRoomVM> resultContext = new ResultContext<CandidateProviderPremisesRoomVM>();

            try
            {
                this.repository.HardDelete<CandidateProviderPremisesRoom>(entity);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Помещението е изтрито успешно!");
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

        #region Premises Speciality
        public async Task<CandidateProviderPremisesSpecialityVM> GetCandidateProviderSpecialityByIdSpecialityAndIdCandidateProviderPremisesAsync(int idSpeciality, int idCandidateProviderPremises, int idUsage)
        {
            var kvTheoryAndPractice = await this.dataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TrainingInTheoryAndPractice");
            var result = this.repository.AllReadonly<CandidateProviderPremisesSpeciality>(x => x.IdSpeciality == idSpeciality && x.IdCandidateProviderPremises == idCandidateProviderPremises && (x.IdUsage == idUsage || x.IdUsage == kvTheoryAndPractice.IdKeyValue));

            return await result.To<CandidateProviderPremisesSpecialityVM>(x => x.Speciality).FirstOrDefaultAsync();
        }

        public List<CandidateProviderPremisesSpecialityVM> GetCandidateProviderPremisesSpecialitiesWithSpecialitiesByIdCandidateProviderPremises(int idCandidateProviderPremises)
        {
            var result = this.repository.AllReadonly<CandidateProviderPremisesSpeciality>(x => x.IdCandidateProviderPremises == idCandidateProviderPremises);

            return result.To<CandidateProviderPremisesSpecialityVM>(x => x.Speciality).ToList();
        }

        public async Task<IEnumerable<CandidateProviderPremisesSpecialityVM>> GetCandidateProviderSpecialityByIdCandidateProviderPremisesAsync(int idCandidateProviderPremises)
        {
            IQueryable<CandidateProviderPremisesSpeciality> result = this.repository.AllReadonly<CandidateProviderPremisesSpeciality>(x => x.IdCandidateProviderPremises == idCandidateProviderPremises);

            return await result.To<CandidateProviderPremisesSpecialityVM>().ToListAsync();
        }

        public async Task<IEnumerable<CandidateProviderSpecialityVM>> GetCandidateProviderAllPremisesSpecialititesByCandidateProviderId(CandidateProviderVM candidateProviderVm)
        {
            IQueryable<CandidateProviderSpeciality> data = this.repository.All<CandidateProviderSpeciality>(x => x.IdCandidate_Provider == candidateProviderVm.IdCandidate_Provider);
            var result = await data.To<CandidateProviderSpecialityVM>(
                p => p.CandidateProvider.CandidateProviderPremises,
                p => p.CandidateProvider.CandidateProviderTrainers,
                p => p.Speciality.Profession).ToListAsync();

            return result;
        }

        public async Task<ResultContext<NoResult>> AddSpecialitiesToPremisesByListSpecialitiesAsync(List<CandidateProviderPremisesSpecialityVM> premisesSpecialities)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                foreach (var premisesSpeciality in premisesSpecialities)
                {
                    var premisesSpecialityForDb = premisesSpeciality.To<CandidateProviderPremisesSpeciality>();
                    await this.repository.AddAsync<CandidateProviderPremisesSpeciality>(premisesSpecialityForDb);
                }

                await this.repository.SaveChangesAsync();

                var msg = premisesSpecialities.Count == 1
                    ? "Специалността е добавена успешно!"
                    : "Специалностите са добавени успешно!";

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

        public async Task<ResultContext<CandidateProviderPremisesSpecialityVM>> DeleteCandidateProviderPremisesSpecialityAsync(CandidateProviderPremisesSpecialityVM candidateProviderPremisesSpecialityVM, int idUsage)
        {
            var kvTheory = await this.dataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TheoryTraining");
            var kvPractice = await this.dataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "PracticalTraining");
            var entity = await this.repository.GetByIdAsync<CandidateProviderPremisesSpeciality>(candidateProviderPremisesSpecialityVM.IdCandidateProviderPremisesSpeciality);
            this.repository.Detach<CandidateProviderPremisesSpeciality>(entity);

            ResultContext<CandidateProviderPremisesSpecialityVM> resultContext = new ResultContext<CandidateProviderPremisesSpecialityVM>();

            try
            {
                if (entity is not null)
                {
                    if (entity.IdUsage == idUsage)
                    {
                        await this.repository.HardDeleteAsync<CandidateProviderPremisesSpeciality>(entity.IdCandidateProviderPremisesSpeciality);
                    }
                    else
                    {
                        if (idUsage == kvTheory.IdKeyValue)
                        {
                            entity.IdUsage = kvPractice.IdKeyValue;
                        }
                        else
                        {
                            entity.IdUsage = kvTheory.IdKeyValue;
                        }

                        this.repository.Update<CandidateProviderPremisesSpeciality>(entity);
                    }
                }

                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Специалността е изтрита успешно!");
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

        public async Task<ResultContext<CandidateProviderPremisesCheckingVM>> DeleteCandidateProviderPremisesCheckingAsync(CandidateProviderPremisesCheckingVM candidateProviderPremisesCheckingVM)
        {
            var entity = await this.repository.GetByIdAsync<CandidateProviderPremisesChecking>(candidateProviderPremisesCheckingVM.IdCandidateProviderPremisesChecking);
            this.repository.Detach<CandidateProviderPremisesChecking>(entity);

            ResultContext<CandidateProviderPremisesCheckingVM> resultContext = new ResultContext<CandidateProviderPremisesCheckingVM>();

            try
            {
                this.repository.HardDelete<CandidateProviderPremisesChecking>(entity);
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

        #region Premises Document
        public async Task<CandidateProviderPremisesDocumentVM> GetCandidateProviderPremisesDocumentByIdAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM)
        {
            IQueryable<CandidateProviderPremisesDocument> providerPremisesDocuments = this.repository.AllReadonly<CandidateProviderPremisesDocument>(x => x.IdCandidateProviderPremisesDocument == candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument);

            return await providerPremisesDocuments.To<CandidateProviderPremisesDocumentVM>().FirstOrDefaultAsync();
        }

        public async Task<ResultContext<CandidateProviderPremisesDocumentVM>> CreateCandidateProviderPremisesDocumentAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM)
        {
            ResultContext<CandidateProviderPremisesDocumentVM> outputContext = new ResultContext<CandidateProviderPremisesDocumentVM>();

            try
            {

                var entryForDb = candidateProviderPremisesDocumentVM.To<CandidateProviderPremisesDocument>();
                entryForDb.CandidateProviderPremises = null;
                await this.repository.AddAsync<CandidateProviderPremisesDocument>(entryForDb);
                await this.repository.SaveChangesAsync();

                outputContext.AddMessage("Записът е успешен!");
                candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument = entryForDb.IdCandidateProviderPremisesDocument;
                outputContext.ResultContextObject = candidateProviderPremisesDocumentVM;

                this.repository.Detach<CandidateProviderPremisesDocument>(entryForDb);
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<CandidateProviderPremisesDocumentVM>> UpdateCandidateProviderPremisesDocumentAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM)
        {
            ResultContext<CandidateProviderPremisesDocumentVM> outputContext = new ResultContext<CandidateProviderPremisesDocumentVM>();

            try
            {
                var entity = await this.repository.GetByIdAsync<CandidateProviderPremisesDocument>(candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument);

                candidateProviderPremisesDocumentVM.IdCreateUser = entity.IdCreateUser;
                candidateProviderPremisesDocumentVM.CreationDate = entity.CreationDate;
                entity = candidateProviderPremisesDocumentVM.To<CandidateProviderPremisesDocument>();
                entity.CandidateProviderPremises = null;

                this.repository.Update<CandidateProviderPremisesDocument>(entity);
                await this.repository.SaveChangesAsync();
                this.repository.Detach<CandidateProviderPremisesDocument>(entity);

                outputContext.AddMessage("Записът е успешен!");
                outputContext.ResultContextObject = candidateProviderPremisesDocumentVM;
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<CandidateProviderPremisesDocumentVM>> DeleteCandidateProviderPremisesDocumentAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM)
        {
            var entity = await this.repository.GetByIdAsync<CandidateProviderPremisesDocument>(candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument);

            ResultContext<CandidateProviderPremisesDocumentVM> resultContext = new ResultContext<CandidateProviderPremisesDocumentVM>();

            try
            {
                this.repository.HardDelete<CandidateProviderPremisesDocument>(entity);
                await this.repository.SaveChangesAsync();

                var settingsFolder = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var pathToFile = settingsFolder + "\\" + entity.UploadedFileName;
                if (entity.UploadedFileName != null)
                {
                    if (Directory.Exists(pathToFile))
                    {
                        Directory.Delete(pathToFile, true);
                    }
                }

                resultContext.AddMessage("Документът е изтрит успешно!");
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

        public async Task<IEnumerable<CandidateProviderPremisesDocumentVM>> GetAllCandidateProviderPremisesDocumentsByCandidateProviderPremisesIdAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM)
        {
            IQueryable<CandidateProviderPremisesDocument> providerPremisesDocuments = this.repository.AllReadonly<CandidateProviderPremisesDocument>(x => x.IdCandidateProviderPremises == candidateProviderPremisesDocumentVM.IdCandidateProviderPremises);
            var result = providerPremisesDocuments.To<CandidateProviderPremisesDocumentVM>();

            return await result.ToListAsync();
        }
        #endregion

        #region Candidate Provider Person
        public async Task<List<CandidateProviderPersonVM>> GetAllCandidateProviderPersonAsync()
        {
            IQueryable<CandidateProviderPerson> providerPeople = this.repository.AllReadonly<CandidateProviderPerson>();
            var result = providerPeople.To<CandidateProviderPersonVM>();

            return await result.ToListAsync();

        }

        public async Task<IEnumerable<CandidateProviderPersonVM>> GetAllActiveCandidateProviderPersonsByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<CandidateProviderPerson>(x => x.IdCandidate_Provider == idCandidateProvider);
            var personIds = data.Select(x => x.IdPerson).ToList();
            var kvUserStatusActiveValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("UserStatus", "Active");
            var users = await this.repository.AllReadonly<ApplicationUser>(x => personIds.Contains(x.IdPerson!.Value) && x.IdUserStatus == kvUserStatusActiveValue.IdKeyValue).ToListAsync();
            var dataAsVM = await data.To<CandidateProviderPersonVM>().ToListAsync();
            for (int i = dataAsVM.Count - 1; i >= 0; i--)
            {
                if (!users.Any(x => x.IdPerson == dataAsVM[i].IdPerson))
                    dataAsVM.Remove(dataAsVM[i]);
            }

            return dataAsVM;
        }

        public async Task<IEnumerable<CandidateProviderPersonVM>> GetAllCandidateProviderPersonsByIdCandidateProviderAsync(int idCandidateProvider)
        {
            return await this.repository.AllReadonly<CandidateProviderPerson>(x => x.IdCandidate_Provider == idCandidateProvider).To<CandidateProviderPersonVM>().ToListAsync();
        }

        public async Task<IEnumerable<CandidateProviderPersonVM>> GetAllCandidateProviderPersonsAllowedForNotificationsByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<CandidateProviderPerson>(x => x.IdCandidate_Provider == idCandidateProvider && x.IsAllowedForNotification);

            return await data.To<CandidateProviderPersonVM>().ToListAsync();
        }
        #endregion

        #region Candidate Provider CPO Structure and Activity
        public async Task<CandidateProviderCPOStructureActivityVM> GetCandidateProviderCPOStructureActivityByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<CandidateProviderCPOStructureActivity>(x => x.IdCandidate_Provider == idCandidateProvider);

            return await data.To<CandidateProviderCPOStructureActivityVM>().FirstOrDefaultAsync();
        }

        public async Task<ResultContext<CandidateProviderCPOStructureActivityVM>> CreateCandidateProviderCPOStructureActivityAsync(ResultContext<CandidateProviderCPOStructureActivityVM> inputContext)
        {
            var model = inputContext.ResultContextObject;

            try
            {
                var entityForDb = model.To<CandidateProviderCPOStructureActivity>();
                entityForDb.CandidateProvider = null;

                await this.repository.AddAsync<CandidateProviderCPOStructureActivity>(entityForDb);
                await this.repository.SaveChangesAsync();

                inputContext.ResultContextObject.IdCandidateProviderCPOStructureActivity = entityForDb.IdCandidateProviderCPOStructureActivity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return inputContext;
        }

        public async Task<ResultContext<CandidateProviderCPOStructureActivityVM>> UpdateCandidateProviderCPOStructureActivityAsync(ResultContext<CandidateProviderCPOStructureActivityVM> inputContext)
        {
            var model = inputContext.ResultContextObject;

            try
            {
                var entityFromDb = await this.repository.GetByIdAsync<CandidateProviderCPOStructureActivity>(model.IdCandidateProviderCPOStructureActivity);

                if (entityFromDb.Management != model.Management
                    || entityFromDb.CompletionCertificationTraining != model.CompletionCertificationTraining
                    || entityFromDb.DataMaintenance != model.DataMaintenance
                    || entityFromDb.InformationProvisionMaintenance != model.InformationProvisionMaintenance
                    || entityFromDb.OrganisationTrainingProcess != model.InformationProvisionMaintenance
                    || entityFromDb.InternalQualitySystem != model.InternalQualitySystem
                    || entityFromDb.MTBDescription != model.MTBDescription
                    || entityFromDb.TeachersSelection != model.TeachersSelection
                    || entityFromDb.TrainingDocumentation != model.TrainingDocumentation)
                {
                    model.IdCreateUser = entityFromDb.IdCreateUser;
                    model.CreationDate = entityFromDb.CreationDate;
                    entityFromDb = model.To<CandidateProviderCPOStructureActivity>();
                    entityFromDb.CandidateProvider = null;

                    this.repository.Update<CandidateProviderCPOStructureActivity>(entityFromDb);
                    await this.repository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return inputContext;
        }
        #endregion

        #region Candidate Provider CIPO Structure and Activity
        public async Task<CandidateProviderCIPOStructureActivityVM> GetCandidateProviderCIPOStructureActivityByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var data = this.repository.AllReadonly<CandidateProviderCIPOStructureActivity>(x => x.IdCandidate_Provider == idCandidateProvider);

            return await data.To<CandidateProviderCIPOStructureActivityVM>().FirstOrDefaultAsync();
        }

        public async Task<ResultContext<CandidateProviderCIPOStructureActivityVM>> CreateCandidateProviderCIPOStructureActivityAsync(ResultContext<CandidateProviderCIPOStructureActivityVM> inputContext)
        {
            var model = inputContext.ResultContextObject;

            try
            {
                var entityForDb = model.To<CandidateProviderCIPOStructureActivity>();
                entityForDb.CandidateProvider = null;

                await this.repository.AddAsync<CandidateProviderCIPOStructureActivity>(entityForDb);
                await this.repository.SaveChangesAsync();

                inputContext.ResultContextObject.IdCandidateProviderCIPOStructureActivity = entityForDb.IdCandidateProviderCIPOStructureActivity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return inputContext;
        }

        public async Task<ResultContext<CandidateProviderCIPOStructureActivityVM>> UpdateCandidateProviderCIPOStructureActivityAsync(ResultContext<CandidateProviderCIPOStructureActivityVM> inputContext)
        {
            var model = inputContext.ResultContextObject;

            try
            {
                var entityFromDb = await this.repository.GetByIdAsync<CandidateProviderCIPOStructureActivity>(model.IdCandidateProviderCIPOStructureActivity);

                if (entityFromDb.Management != model.Management
                    || entityFromDb.OrganisationInformationProcess != model.OrganisationInformationProcess
                    || entityFromDb.DataMaintenance != model.DataMaintenance
                    || entityFromDb.InformationProvisionMaintenance != model.InformationProvisionMaintenance
                    || entityFromDb.InternalQualitySystem != model.InternalQualitySystem
                    || entityFromDb.MTBDescription != model.MTBDescription
                    || entityFromDb.ConsultantsSelection != model.ConsultantsSelection
                    || entityFromDb.TrainingDocumentation != model.TrainingDocumentation)
                {
                    model.IdCreateUser = entityFromDb.IdCreateUser;
                    model.CreationDate = entityFromDb.CreationDate;
                    entityFromDb = model.To<CandidateProviderCIPOStructureActivity>();
                    entityFromDb.CandidateProvider = null;

                    this.repository.Update<CandidateProviderCIPOStructureActivity>(entityFromDb);
                    await this.repository.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return inputContext;
        }
        #endregion

        #region Candidate Provider Change License
        public async Task<CandidateProviderLicenceChangeVM> GetCandidateProviderChangeLicenseByIdAsync(int idCandidateProvider, int IdCandidateProviderLicenceChange)
        {
            var entity = await this.repository.AllReadonly<CandidateProviderLicenceChange>(x => x.IdCandidate_Provider == idCandidateProvider && x.IdCandidateProviderLicenceChange == IdCandidateProviderLicenceChange).FirstOrDefaultAsync();

            if (entity != null)
            {
                var CanditeVM = entity.To<CandidateProviderLicenceChangeVM>();
                return CanditeVM;
            }
            else
            {
                var CanditeVM = new CandidateProviderLicenceChangeVM() { IdCandidate_Provider = idCandidateProvider };
                return CanditeVM;
            }

        }

        public async Task<IEnumerable<CandidateProviderLicenceChangeVM>> GetCandidateProviderLicensesListByIdAsync(int idCandidateProvider)
        {
            var entity = this.repository.AllReadonly<CandidateProviderLicenceChange>(x => x.IdCandidate_Provider == idCandidateProvider).Include(x => x.CandidateProvider);
            var dataAsVM = await entity.To<CandidateProviderLicenceChangeVM>(x => x.CandidateProvider).ToListAsync();
            var kvLicenseChangeStatus = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicenseStatus");
            var kvLicenceStatusDetails = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicenceStatusDetail");
            foreach (var entry in dataAsVM)
            {
                var kvLicenceStatus = kvLicenseChangeStatus.FirstOrDefault(x => x.IdKeyValue == entry.IdStatus);
                entry.LicenceStatusName = kvLicenceStatus?.Name;
                entry.LicenceStatusNameEn = kvLicenceStatus?.KeyValueIntCode;

                var kvLicenceStatusDetail = kvLicenceStatusDetails.FirstOrDefault(x => x.IdKeyValue == entry.IdLicenceStatusDetail);
                entry.LicenceStatusDetailName = kvLicenceStatusDetail?.Name;
            }

            return dataAsVM.OrderBy(x => x.ChangeDate);
        }

        public async Task<ResultContext<CandidateProviderLicenceChangeVM>> CreateCandidateProviderLicenceChangeAsync(CandidateProviderLicenceChangeVM candidateProviderLicenceChangeVM)
        {
            ResultContext<CandidateProviderLicenceChangeVM> outputContext = new ResultContext<CandidateProviderLicenceChangeVM>();

            try
            {
                var candidateProvider = await this.repository.GetByIdAsync<CandidateProvider>(candidateProviderLicenceChangeVM.IdCandidate_Provider);
                candidateProvider.Archive = candidateProviderLicenceChangeVM.Archive;
                candidateProvider.IdLicenceStatus = candidateProviderLicenceChangeVM.IdStatus;

                candidateProviderLicenceChangeVM.ModifyDate = DateTime.Now;
                candidateProviderLicenceChangeVM.CreationDate = DateTime.Now;


                var documentData = await docuService.GetIdentDocument(candidateProviderLicenceChangeVM.NumberCommand, 0, candidateProviderLicenceChangeVM.ChangeDate.Value);
                
                
                if (documentData.DocIdent == null)
                {
                    outputContext.AddErrorMessage("Не можете да запишете промяната в лицензията, защото за въведеният номер на заповед няма намерено съответствие в деловодната система! Моля, коригирайте въведената информация и опитайте пак.");
                    return outputContext;
                }
                var contextResponse = await docuService.GetDocumentAsync(documentData.DocIdent.First().DocID, documentData.DocIdent.First().GUID);

                if (contextResponse.HasErrorMessages)
                {
                    outputContext.ListErrorMessages = contextResponse.ListErrorMessages;

                    return outputContext;
                }

                var documentResponse = contextResponse.ResultContextObject;

                if (documentResponse.Doc == null)
                {
                    outputContext.AddErrorMessage("Не можете да запишете промяната в лицензията, за въведеният номер на заповед няма прикачен файл в деловодната система!");
                    return outputContext;
                }
                candidateProviderLicenceChangeVM.DS_OFFICIAL_ID = documentResponse.Doc.DocID;
                candidateProviderLicenceChangeVM.DS_OFFICIAL_GUID = documentResponse.Doc.GUID;
                candidateProviderLicenceChangeVM.DS_OFFICIAL_DATE = documentResponse.Doc.DocDate;
                candidateProviderLicenceChangeVM.DS_OFFICIAL_DocNumber = documentResponse.Doc.DocNumber;

                var entryForDb = candidateProviderLicenceChangeVM.To<CandidateProviderLicenceChange>();

                entryForDb.CandidateProvider = null;

                await this.repository.AddAsync<CandidateProviderLicenceChange>(entryForDb);
                this.repository.Update<CandidateProvider>(candidateProvider);
                await this.repository.SaveChangesAsync(false);

                outputContext.AddMessage("Записът е успешен!");
                candidateProviderLicenceChangeVM.IdCandidateProviderLicenceChange = entryForDb.IdCandidateProviderLicenceChange;
                outputContext.ResultContextObject = candidateProviderLicenceChangeVM;

                this.repository.Detach<CandidateProviderLicenceChange>(entryForDb);
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage("Има проблем при запис на промяна в лицензията.");
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<CandidateProviderLicenceChangeVM>> UpdateCandidateProviderLicenceChangeAsync(CandidateProviderLicenceChangeVM candidateProviderLicenceChangeVM)
        {
            ResultContext<CandidateProviderLicenceChangeVM> outputContext = new ResultContext<CandidateProviderLicenceChangeVM>();
            //  CandidateProvider candProvider = new CandidateProvider();
            try
            {
                var entity = await this.repository.GetByIdAsync<CandidateProviderLicenceChange>(candidateProviderLicenceChangeVM.IdCandidateProviderLicenceChange);
                this.repository.Detach<CandidateProviderLicenceChange>(entity);

                var candidateProvider = await this.repository.GetByIdAsync<CandidateProvider>(candidateProviderLicenceChangeVM.IdCandidate_Provider);
                candidateProvider.Archive = candidateProviderLicenceChangeVM.Archive;
                candidateProvider.IdLicenceStatus = candidateProviderLicenceChangeVM.IdStatus;

                candidateProviderLicenceChangeVM.IdModifyUser = entity.IdModifyUser;
                candidateProviderLicenceChangeVM.ModifyDate = entity.ModifyDate;

                //candidateProviderLicenceChangeVM.IdCreateUser = entity.IdCreateUser;
                //candidateProviderLicenceChangeVM.CreationDate = entity.CreationDate;

                entity = candidateProviderLicenceChangeVM.To<CandidateProviderLicenceChange>();
                entity.CandidateProvider = null;

                this.repository.Update<CandidateProvider>(candidateProvider);
                this.repository.Update<CandidateProviderLicenceChange>(entity);
                await this.repository.SaveChangesAsync(false);
                this.repository.Detach<CandidateProviderLicenceChange>(entity);

                outputContext.AddMessage("Записът е успешен!");
                outputContext.ResultContextObject = candidateProviderLicenceChangeVM;
            }
            catch (Exception ex)
            {
                outputContext.AddErrorMessage(ex.Message);
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return outputContext;
        }

        public async Task<ResultContext<CandidateProviderTrainerCheckingVM>> CreateCandidateProviderTrainerCheckingAsync(ResultContext<CandidateProviderTrainerCheckingVM> resultContext)
        {


            try
            {


                var entryForDb = resultContext.ResultContextObject.To<CandidateProviderTrainerChecking>();
                entryForDb.CandidateProviderTrainer = null;

                await this.repository.AddAsync<CandidateProviderTrainerChecking>(entryForDb);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");
                resultContext.ResultContextObject.IdCandidateProviderTrainerChecking = entryForDb.IdCandidateProviderTrainerChecking;
                resultContext.ResultContextObject.IdCreateUser = entryForDb.IdCreateUser;
                resultContext.ResultContextObject.IdModifyUser = entryForDb.IdModifyUser;
                resultContext.ResultContextObject.CreationDate = entryForDb.CreationDate;
                resultContext.ResultContextObject.ModifyDate = entryForDb.ModifyDate;


                this.repository.Detach<CandidateProviderTrainerChecking>(entryForDb);
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

        public async Task<ResultContext<CandidateProviderPremisesCheckingVM>> CreateCandidateProviderPremisesCheckingAsync(ResultContext<CandidateProviderPremisesCheckingVM> resultContext)
        {
            try
            {
                var entryForDb = resultContext.ResultContextObject.To<CandidateProviderPremisesChecking>();
                entryForDb.CandidateProviderPremises = null;

                await this.repository.AddAsync<CandidateProviderPremisesChecking>(entryForDb);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Записът е успешен!");

                this.repository.Detach<CandidateProviderPremisesChecking>(entryForDb);

                resultContext.ResultContextObject.IdCandidateProviderPremisesChecking = entryForDb.IdCandidateProviderPremisesChecking;
                resultContext.ResultContextObject.IdCreateUser = entryForDb.IdCreateUser;
                resultContext.ResultContextObject.IdModifyUser = entryForDb.IdModifyUser;
                resultContext.ResultContextObject.CreationDate = entryForDb.CreationDate;
                resultContext.ResultContextObject.ModifyDate = entryForDb.ModifyDate;
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

        #region Candidate Provider Consulting
        public async Task<IEnumerable<CandidateProviderConsultingVM>> GetAllCandidateProviderConsultingsByIdCandidateProviderAsync(int idCandidateProvider)
        {
            var candidateProviderConsultings = this.repository.AllReadonly<CandidateProviderConsulting>(x => x.IdCandidateProvider == idCandidateProvider);
            var candidateProviderConsultingsAsVM = await candidateProviderConsultings.To<CandidateProviderConsultingVM>().ToListAsync();
            if (candidateProviderConsultingsAsVM.Any())
            {
                var kvConsultingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingType");
                foreach (var consulting in candidateProviderConsultingsAsVM)
                {
                    if (consulting.IdConsultingType != 0)
                    {
                        var consultingType = kvConsultingTypeSource.FirstOrDefault(x => x.IdKeyValue == consulting.IdConsultingType);
                        if (consultingType is not null)
                        {
                            consulting.ConsultingTypeValue = consultingType.Name;
                        }
                    }
                }
            }

            return candidateProviderConsultingsAsVM.OrderBy(x => x.ConsultingTypeValue).ToList();
        }

        public async Task<IEnumerable<ConsultingVM>> GetAllConsultingsByIdConsultingClientAsync(int idConsultingClient)
        {
            var candidateProviderConsultings = this.repository.AllReadonly<Consulting>(x => x.IdConsultingClient == idConsultingClient);
            var candidateProviderConsultingsAsVM = await candidateProviderConsultings.To<ConsultingVM>().ToListAsync();
            if (candidateProviderConsultingsAsVM.Any())
            {
                var kvConsultingTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingType");
                var kvConsultingReceiveTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingReceiveType");
                foreach (var consulting in candidateProviderConsultingsAsVM)
                {
                    if (consulting.IdConsultingType is not null)
                    {
                        var consultingType = kvConsultingTypeSource.FirstOrDefault(x => x.IdKeyValue == consulting.IdConsultingType);
                        if (consultingType is not null)
                        {
                            consulting.ConsultingTypeValue = consultingType.Name;
                        }
                    }

                    if (consulting.IdConsultingReceiveType is not null)
                    {
                        var consultingReceiveType = kvConsultingReceiveTypeSource.FirstOrDefault(x => x.IdKeyValue == consulting.IdConsultingReceiveType);
                        if (consultingReceiveType is not null)
                        {
                            consulting.ConsultingReceiveTypeValue = consultingReceiveType.Name;
                        }
                    }
                }
            }

            return candidateProviderConsultingsAsVM.OrderBy(x => x.ConsultingTypeValue).ToList();
        }

        public async Task<ResultContext<NoResult>> CreateCandidateProviderConsultingAsync(CandidateProviderConsultingVM model)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var entryForDb = model.To<CandidateProviderConsulting>();

                await this.repository.AddAsync<CandidateProviderConsulting>(entryForDb);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Услугата е добавена успешно!");
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

        public async Task<ResultContext<NoResult>> CreateConsultingAsync(ConsultingVM model)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var entryForDb = model.To<Consulting>();

                await this.repository.AddAsync<Consulting>(entryForDb);
                await this.repository.SaveChangesAsync();

                resultContext.AddMessage("Услугата е добавена успешно!");
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

        public async Task<ResultContext<NoResult>> DeleteCandidateProviderConsultingByIdAsync(int idCandidateProviderConsulting)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var candidateProviderConsulting = await this.repository.GetByIdAsync<CandidateProviderConsulting>(idCandidateProviderConsulting);
                if (candidateProviderConsulting is not null)
                {
                    var consultedClients = await this.repository.AllReadonly<Consulting>(x => x.ConsultingClient.IdCandidateProvider == candidateProviderConsulting.IdCandidateProvider && x.IdConsultingReceiveType == candidateProviderConsulting.IdConsultingType).AnyAsync();
                    if (consultedClients)
                    {
                        resultContext.AddErrorMessage("Не можете да изтриете избрания запис, защото има данни за консултирани лица по тази услуга!");
                    }
                    else
                    {
                        await this.repository.HardDeleteAsync<CandidateProviderConsulting>(candidateProviderConsulting.IdCandidateProviderConsulting);
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

        public async Task<ResultContext<NoResult>> DeleteConsultingByIdAsync(int idConsulting)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var consulting = await this.repository.GetByIdAsync<Consulting>(idConsulting);
                if (consulting is not null)
                {
                    await this.repository.HardDeleteAsync<Consulting>(consulting.IdConsulting);
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

        #region Curriculum modification
        public async Task<CandidateCurriculumModificationVM> GetCurriculumModificationByIdCandidateProviderSpecialityAndByIdModificationStatusAsync(int idCandidateProviderSpeciality, int idModificationStatus)
        {
            return await this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == idCandidateProviderSpeciality && x.IdModificationStatus == idModificationStatus)
                .To<CandidateCurriculumModificationVM>(x => x.CandidateCurriculums)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CandidateCurriculumVM>> GetCandidateCurriculumsByIdCandidateCurriculumModificationAsync(int idCandidateCurriculumModification)
        {
            return await this.repository.AllReadonly<CandidateCurriculum>(x => x.IdCandidateCurriculumModification == idCandidateCurriculumModification)
                .To<CandidateCurriculumVM>(x => x.CandidateCurriculumERUs)
                .OrderBy(x => x.IdProfessionalTraining)
                    .ThenBy(x => x.Subject)
                        .ThenBy(x => x.Topic)
                .ToListAsync();
        }

        public async Task<CandidateCurriculumModificationVM> GetCandidateCurriculumModificationWhenApplicationByIdCandidateProviderSpecialityAsync(int idCandidateProviderSpeciality)
        {
            var kvModificationStatusFinalValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");
            var kvModificationReasonFirstLicensingValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationReasonType", "FirstLicensing");
            var candidateCurriculumModification = await this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == idCandidateProviderSpeciality && x.IdModificationStatus == kvModificationStatusFinalValue.IdKeyValue && x.IdModificationReason == kvModificationReasonFirstLicensingValue.IdKeyValue).FirstOrDefaultAsync();
            if (candidateCurriculumModification is not null)
            {
                return candidateCurriculumModification.To<CandidateCurriculumModificationVM>();
            }

            return await this.CreateCandidateCurriculumModificationAsync(idCandidateProviderSpeciality, kvModificationStatusFinalValue.IdKeyValue, kvModificationReasonFirstLicensingValue.IdKeyValue);
        }

        public async Task<IEnumerable<CandidateCurriculumModificationVM>> GetAllCurriculumModificationsByIdCandidateProviderSpecialityAsync(int idCandidateProviderSpeciality)
        {
            var modifications = this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == idCandidateProviderSpeciality);
            var kvModificationReasonSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CurriculumModificationReasonType");
            var kvModificationStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CurriculumModificationStatusType");
            var modificationsAsVM = await modifications.To<CandidateCurriculumModificationVM>().ToListAsync();
            foreach (var mod in modificationsAsVM)
            {
                var status = kvModificationStatusSource.FirstOrDefault(x => x.IdKeyValue == mod.IdModificationStatus);
                if (status is not null)
                {
                    mod.ModificationStatusValue = status.Name;
                }

                var reason = kvModificationReasonSource.FirstOrDefault(x => x.IdKeyValue == mod.IdModificationReason);
                if (reason is not null)
                {
                    mod.ModificationReasonValue = reason.Name;
                }
            }

            return modificationsAsVM.OrderBy(x => x.ValidFromDate);
        }

        public async Task<IEnumerable<CandidateCurriculumVM>> GetActualCandidateCurriculumByIdCandidateProviderSpecialityAsync(int idCandidateProviderSpeciality)
        {
            var candidateCurriculumsList = new List<CandidateCurriculumVM>();
            var kvModificationStatusFinalValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");
            //var candidateProviderSpeciality = await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidateProviderSpeciality == idCandidateProviderSpeciality).FirstOrDefaultAsync();
            //if (candidateProviderSpeciality is not null)
            //{
            //    var candidateCurriculumModifications = this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == candidateProviderSpeciality.IdCandidateProviderSpeciality
            //        && x.IdModificationStatus == kvModificationStatusFinalValue.IdKeyValue && x.ValidFromDate!.Value.Date <= DateTime.Now.Date)
            //            .OrderByDescending(x => x.IdCandidateCurriculumModification);
            //    if (candidateCurriculumModifications.Any())
            //    {
            //        var candidateCurriculumModificationAsVM = await candidateCurriculumModifications.To<CandidateCurriculumModificationVM>(x => x.CandidateCurriculums.Select(y => y.CandidateCurriculumERUs.Select(z => z.ERU))).FirstOrDefaultAsync();
            //        return candidateCurriculumModificationAsVM.CandidateCurriculums.ToList();
            //    }
            //}

            //return new List<CandidateCurriculumVM>();

            var candidateProviderSpeciality = await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidateProviderSpeciality == idCandidateProviderSpeciality)
                .Include(x => x.CandidateCurriculumModifications.Where(y => y.IdModificationStatus == kvModificationStatusFinalValue.IdKeyValue && y.ValidFromDate!.Value.Date <= DateTime.Now.Date).OrderByDescending(x => x.IdCandidateCurriculumModification).Take(1))
                    .ThenInclude(x => x.CandidateCurriculums)
                        .ThenInclude(x => x.CandidateCurriculumERUs).FirstOrDefaultAsync();
            if (candidateProviderSpeciality is not null)
            {
                var curriculumModification = candidateProviderSpeciality.CandidateCurriculumModifications.FirstOrDefault();
                if (curriculumModification is not null && curriculumModification.CandidateCurriculums.Any())
                {
                    foreach (var curriculum in curriculumModification.CandidateCurriculums)
                    {
                        var curriculumAsVM = curriculum.To<CandidateCurriculumVM>();
                        candidateCurriculumsList.Add(curriculumAsVM);
                    }
                } 
            }

            return candidateCurriculumsList;
        }

        public async Task<IEnumerable<CandidateCurriculumVM>> GetActualCandidateCurriculumWithERUIncludedByIdCandidateProviderSpecialityAsync(int idCandidateProviderSpeciality)
        {
            var candidateCurriculumsList = new List<CandidateCurriculumVM>();
            var kvModificationStatusFinalValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");
            var candidateProviderSpeciality = await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidateProviderSpeciality == idCandidateProviderSpeciality)
                .Include(x => x.CandidateCurriculumModifications.Where(y => y.IdModificationStatus == kvModificationStatusFinalValue.IdKeyValue && y.ValidFromDate!.Value.Date <= DateTime.Now.Date).OrderByDescending(x => x.IdCandidateCurriculumModification).Take(1))
                    .ThenInclude(x => x.CandidateCurriculums)
                        .ThenInclude(x => x.CandidateCurriculumERUs)
                            .ThenInclude(x => x.ERU).FirstOrDefaultAsync();
            if (candidateProviderSpeciality is not null)
            {
                var curriculumModification = candidateProviderSpeciality.CandidateCurriculumModifications.FirstOrDefault();
                if (curriculumModification is not null && curriculumModification.CandidateCurriculums.Any())
                {
                    foreach (var curriculum in curriculumModification.CandidateCurriculums)
                    {
                        var curriculumAsVM = curriculum.To<CandidateCurriculumVM>();
                        candidateCurriculumsList.Add(curriculumAsVM);
                    }
                }
            }

            return candidateCurriculumsList;
        }

        public async Task<CandidateCurriculumModificationVM> GetActualCandidateCurriculumModificationByIdCandidateProviderSpecialityAsync(int idCandidateProviderSpeciality)
        {
            var providerSpeciality = await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdCandidateProviderSpeciality == idCandidateProviderSpeciality).FirstOrDefaultAsync();
            if (providerSpeciality is not null)
            {
                //var kvModificationStatusFinalValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");
                //var candidateCurriculumModifications = this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == providerSpeciality.IdCandidateProviderSpeciality
                //    && x.IdModificationStatus == kvModificationStatusFinalValue.IdKeyValue && x.ValidFromDate.HasValue ? x.ValidFromDate!.Value.Date <= DateTime.Now.Date : x.OldId.HasValue)
                //        .OrderByDescending(x => x.IdCandidateCurriculumModification).ThenByDescending(x => x.OldId);
                //if (candidateCurriculumModifications.Any())
                //{
                //    return await candidateCurriculumModifications.To<CandidateCurriculumModificationVM>().FirstOrDefaultAsync();
                //}

                var kvModificationStatusFinalValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");
                var candidateCurriculumModifications = this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == providerSpeciality.IdCandidateProviderSpeciality
                    && x.IdModificationStatus == kvModificationStatusFinalValue.IdKeyValue)
                        .OrderByDescending(x => x.IdCandidateCurriculumModification).ThenByDescending(x => x.OldId);
                if (candidateCurriculumModifications.Any())
                {
                    if (candidateCurriculumModifications.Count() == 1)
                    {
                        return await candidateCurriculumModifications.To<CandidateCurriculumModificationVM>().FirstOrDefaultAsync();
                    }
                    else if (candidateCurriculumModifications.Count() > 1)
                    {
                        return await candidateCurriculumModifications.OrderByDescending(x => x.ValidFromDate!.Value.Date <= DateTime.Now.Date).ThenByDescending(x => x.OldId).To<CandidateCurriculumModificationVM>().FirstOrDefaultAsync();
                    }
                }
            }

            return null;
        }

        public async Task<ResultContext<NoResult>> CancelCandidateCurriculumModificationAsync(int idCandidateCurriculumModification)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var candidateCurriculumModification = await this.repository.GetByIdAsync<CandidateCurriculumModification>(idCandidateCurriculumModification);
                if (candidateCurriculumModification is not null)
                {
                    var kvModificationStatusRefusedValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Refused");

                    candidateCurriculumModification.IdModificationStatus = kvModificationStatusRefusedValue.IdKeyValue;

                    this.repository.Update<CandidateCurriculumModification>(candidateCurriculumModification);
                    await this.repository.SaveChangesAsync();
                }

                resultContext.AddMessage("Промените по учебния план и учебните програми са отказани успешно!");
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

        public async Task<ResultContext<NoResult>> FinishCandidateCurriculumModificationAsync(int idCandidateCurriculumModification)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var candidateCurriculumModification = await this.repository.GetByIdAsync<CandidateCurriculumModification>(idCandidateCurriculumModification);
                if (candidateCurriculumModification is not null)
                {
                    var kvModificationStatusFinishedValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");

                    candidateCurriculumModification.IdModificationStatus = kvModificationStatusFinishedValue.IdKeyValue;

                    this.repository.Update<CandidateCurriculumModification>(candidateCurriculumModification);
                    await this.repository.SaveChangesAsync();
                }

                resultContext.AddMessage("Промените по учебната програма са приключени успешно!");
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

        public async Task<ResultContext<NoResult>> CreateCurriculumModificationEntryAsync(int idCandidateProviderSpeciality, int idModificationReason, DateTime validFromDate, bool isDOSChange = false)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var kvCurriculumModificationStatusWorking = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Working");
                CandidateCurriculumModification candidateCurriculumModification = new CandidateCurriculumModification()
                {
                    IdCandidateProviderSpeciality = idCandidateProviderSpeciality,
                    IdModificationReason = idModificationReason,
                    IdModificationStatus = kvCurriculumModificationStatusWorking.IdKeyValue,
                    ValidFromDate = validFromDate
                };

                await this.repository.AddAsync<CandidateCurriculumModification>(candidateCurriculumModification);
                await this.repository.SaveChangesAsync();

                await this.CopyCandidateCurriculumsAsync(idCandidateProviderSpeciality, isDOSChange, candidateCurriculumModification);

                resultContext.NewEntityId = candidateCurriculumModification.IdCandidateCurriculumModification;
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

        // проверява дали актуалната модификация на уч. план на статус различен от окончателен, за да позволи прикачване на файл
        public async Task<bool> IsCurriculumUploadFileAllowedAsync(int idSpeciality, int idCandidateProvider)
        {
            var providerSpeciality = await this.repository.AllReadonly<CandidateProviderSpeciality>(x => x.IdSpeciality == idSpeciality && x.IdCandidate_Provider == idCandidateProvider).FirstOrDefaultAsync();
            if (providerSpeciality is not null)
            {
                var kvModificationStatusFinalValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");
                var candidateCurriculumModifications = this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == providerSpeciality.IdCandidateProviderSpeciality
                    && x.IdModificationStatus == kvModificationStatusFinalValue.IdKeyValue && x.ValidFromDate!.Value.Date <= DateTime.Now.Date)
                        .OrderByDescending(x => x.IdCandidateCurriculumModification);
                if (candidateCurriculumModifications.Any())
                {
                    return (await candidateCurriculumModifications.To<CandidateCurriculumModificationVM>().FirstOrDefaultAsync())?.IdModificationStatus != kvModificationStatusFinalValue.IdKeyValue;
                }
            }

            return true;
        }

        // проверява дали има модификация с дата на валидност от по-нова от тази, която се въвежда в момента
        public async Task<CandidateCurriculumModificationVM> IsNewestValidFromDateByIdCandidateProviderSpecialityAndNewValidFromDateAsync(int idCandidateProviderSpeciality, DateTime validFromDate)
        {
            try
            {
                var kvModificationStatusFinal = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");
                var modifications = await this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == idCandidateProviderSpeciality && x.IdModificationStatus == kvModificationStatusFinal.IdKeyValue).ToListAsync();
                var modification = modifications.FirstOrDefault(x => x.ValidFromDate!.Value >= validFromDate);

                if (modification is not null)
                {
                    return modification.To<CandidateCurriculumModificationVM>();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return null;
        }

        private async Task CopyCandidateCurriculumsAsync(int idCandidateProviderSpeciality, bool isDOSChange, CandidateCurriculumModification candidateCurriculumModification)
        {
            var kvCurriculumModificationStatusFinal = await this.dataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Final");
            var lastFinalCurriculumModification = await this.repository.AllReadonly<CandidateCurriculumModification>(x => x.IdCandidateProviderSpeciality == idCandidateProviderSpeciality
                && x.IdModificationStatus == kvCurriculumModificationStatusFinal.IdKeyValue && x.ValidFromDate!.Value.Date <= DateTime.Now.Date)
                .OrderByDescending(x => x.ValidFromDate!.Value)
                .FirstOrDefaultAsync();
            if (lastFinalCurriculumModification is not null)
            {
                var curriculums = !isDOSChange
                    ? await this.repository.AllReadonly<CandidateCurriculum>(x => x.IdCandidateCurriculumModification == lastFinalCurriculumModification.IdCandidateCurriculumModification)
                        .Include(x => x.CandidateCurriculumERUs)
                            .AsNoTracking()
                                .ToListAsync()
                    : await this.repository.AllReadonly<CandidateCurriculum>(x => x.IdCandidateCurriculumModification == lastFinalCurriculumModification.IdCandidateCurriculumModification).ToListAsync();

                foreach (var curriculum in curriculums)
                {
                    CandidateCurriculum candidateCurriculum = new CandidateCurriculum()
                    {
                        IdCandidateCurriculum = 0,
                        IdCandidateProviderSpeciality = idCandidateProviderSpeciality,
                        IdProfessionalTraining = curriculum.IdProfessionalTraining,
                        Subject = curriculum.Subject,
                        Topic = curriculum.Topic,
                        Theory = curriculum.Theory,
                        Practice = curriculum.Practice,
                        IdCandidateCurriculumModification = candidateCurriculumModification.IdCandidateCurriculumModification
                    };

                    await this.repository.AddAsync<CandidateCurriculum>(candidateCurriculum);
                    await this.repository.SaveChangesAsync();

                    await this.CopyCandidateCurriculumERUsAsync(curriculum, candidateCurriculum);
                }
            }
        }

        private async Task CopyCandidateCurriculumERUsAsync(CandidateCurriculum curriculum, CandidateCurriculum candidateCurriculum)
        {
            foreach (var currERU in curriculum.CandidateCurriculumERUs)
            {
                CandidateCurriculumERU candidateCurriculumERU = new CandidateCurriculumERU()
                {
                    IdCandidateCurriculum = candidateCurriculum.IdCandidateCurriculum,
                    IdERU = currERU.IdERU
                };

                await this.repository.AddAsync<CandidateCurriculumERU>(candidateCurriculumERU);
            }

            await this.repository.SaveChangesAsync();
        }

        private async Task<CandidateCurriculumModificationVM> CreateCandidateCurriculumModificationAsync(int idCandidateProviderSpeciality, int idStatus, int idReason)
        {
            var candidateCurriculumModificationToAdd = new CandidateCurriculumModification()
            {
                IdCandidateProviderSpeciality = idCandidateProviderSpeciality,
                IdModificationReason = idReason,
                IdModificationStatus = idStatus,
                ValidFromDate = DateTime.Now
            };

            await this.repository.AddAsync<CandidateCurriculumModification>(candidateCurriculumModificationToAdd);
            await this.repository.SaveChangesAsync();

            return candidateCurriculumModificationToAdd.To<CandidateCurriculumModificationVM>();
        }
        #endregion

        #region Registers
        public async Task<IEnumerable<RegisterMTBVM>> GetAllMTBsForActiveCandidateProvidersAsync(PremisesFilterVM filterModel)
        {
            var mtbsSource = new List<RegisterMTBVM>();
            var kvPremisesStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseStatus");
            var kvOwnerShipTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseOwnership");
            var kvLicenseTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingType");
            var kvLicenseForCPOValue = kvLicenseTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "LicensingCPO");
            var kvApplicationStatusIssued = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "ProcedureCompleted");
            bool haveCandProvPremisesCheckings = false;
            //var candidateProviders = this.repository.AllReadonly<CandidateProvider>(x => x.IsActive && x.IdApplicationStatus == kvApplicationStatusIssued.IdKeyValue);
            //var candidateProvidersAsVM = await candidateProviders.To<CandidateProviderVM>(x => x.CandidateProviderPremises.Select(y => y.Location.Municipality.District),
            //    x => x.CandidateProviderPremises.Select(y => y.CandidateProviderPremisesSpecialities),
            //    x => x.CandidateProviderPremises.Select(y => y.CandidateProviderPremisesCheckings)).ToListAsync();
            //foreach (var provider in candidateProvidersAsVM)
            //{
            //    var licenseType = kvLicenseTypeSource.FirstOrDefault(x => x.IdKeyValue == provider.IdTypeLicense);
            //    if (licenseType is not null)
            //    {
            //        provider.LicenceTypeValue = licenseType.Name;
            //    }

            //    provider.ProviderNameAndOwnerForRegister = provider.IdTypeLicense == kvLicenseForCPOValue.IdKeyValue ? provider.CPONameOwnerGrid : provider.CIPONameOwnerGrid;

            //    foreach (var mtb in provider.CandidateProviderPremises)
            //    {
            //        var ownershipType = kvOwnerShipTypeSource.FirstOrDefault(x => x.IdKeyValue == mtb.IdOwnership);
            //        if (ownershipType is not null)
            //        {
            //            mtb.OwnershipValue = ownershipType.Name;
            //        }

            //        var statusType = kvPremisesStatusSource.FirstOrDefault(x => x.IdKeyValue == mtb.IdStatus);
            //        if (statusType is not null)
            //        {
            //            mtb.StatusValue = statusType.Name;
            //        }

            //        RegisterMTBVM mtbVM = new RegisterMTBVM()
            //        {
            //            CandidateProvider = provider,
            //            CandidateProviderPremises = mtb
            //        };

            //        mtbsSource.Add(mtbVM);
            //    }
            //}

            //return mtbsSource.OrderBy(x => x.CandidateProvider.IdCandidate_Provider).ThenBy(x => x.CandidateProviderPremises.PremisesName).ToList();

            var filter = PredicateBuilder.True<CandidateProviderPremises>();

            if (filterModel.IdCandidateProvider.HasValue)
            {
                filter = filter.And(x => x.IdCandidate_Provider == filterModel.IdCandidateProvider.Value);
            }

            if (!string.IsNullOrEmpty(filterModel.LicenceNumber))
            {
                filter = filter.And(x => !string.IsNullOrEmpty(x.CandidateProvider.LicenceNumber));
                filter = filter.And(x => x.CandidateProvider.LicenceNumber!.Contains(filterModel.LicenceNumber.Trim()));
            }

            if (filterModel.IdKvStatus.HasValue)
            {
                filter = filter.And(x => x.IdStatus == filterModel.IdKvStatus);
            }

            if (filterModel.IdOwnerShip.HasValue)
            {
                filter = filter.And(x => x.IdOwnership == filterModel.IdOwnerShip);
            }

            if (filterModel.idLocation != 0)
            {
                filter = filter.And(x => x.Location.idLocation == filterModel.idMunicipality);
            }

            if (filterModel.idMunicipality != 0)
            {
                filter = filter.And(x => x.Location.idMunicipality == filterModel.idMunicipality);
            }

            if (filterModel.idDistrict != 0)
            {
                filter = filter.And(x => x.Location.Municipality.idDistrict == filterModel.idDistrict);
            }

            if (filterModel.IdProfession.HasValue)
            {
                filter = filter.And(x => x.CandidateProviderPremisesSpecialities.Any(c => c.Speciality.IdProfession == filterModel.IdProfession));
            }

            if (filterModel.specialities.Any())
            {
                filter = filter.And(t => t.CandidateProviderPremisesSpecialities.Any(c => filterModel.specialities.All(s => s.IdSpeciality == c.IdSpeciality)));
            }

            if (filterModel.IdTypeOfEducation.HasValue)
            {
                filter = filter.And(t => t.CandidateProviderPremisesSpecialities.Any(c => c.IdUsage == filterModel.IdTypeOfEducation));
            }

            if (filterModel.CreationDateFrom.HasValue && filterModel.CreationDateTo.HasValue)
            {
                filter = filter.And(t => (t.CreationDate.ToString() != "1.1.0001 г. 0:00:00" && filterModel.CreationDateFrom.HasValue ? t.CreationDate.Date >= filterModel.CreationDateFrom.Value.Date : false) && (t.CreationDate.ToString() != "1.1.0001 г. 0:00:00" && filterModel.CreationDateTo.HasValue ? t.CreationDate.Date <= filterModel.CreationDateTo.Value.Date : false));
            }
            else if (filterModel.CreationDateFrom.HasValue && !filterModel.CreationDateTo.HasValue) /*Дата на създаване От*/
            {
                filter = filter.And(t => t.CreationDate.ToString() != "1.1.0001 г. 0:00:00" && filterModel.CreationDateFrom.HasValue ? t.CreationDate.Date >= filterModel.CreationDateFrom.Value.Date : false);
            }
            else if (filterModel.CreationDateTo.HasValue && !filterModel.CreationDateFrom.HasValue)/*Дата на създаване До*/
            {
                filter = filter.And(t => t.CreationDate.ToString() != "1.1.0001 г. 0:00:00" && filterModel.CreationDateTo.HasValue ? t.CreationDate.Date <= filterModel.CreationDateTo.Value.Date : false);
            }

            if (filterModel.ModifyDateFrom.HasValue && filterModel.ModifyDateTo.HasValue)
            {
                filter = filter.And(t => (t.ModifyDate.ToString() != "1.1.0001 г. 0:00:00" && filterModel.ModifyDateFrom.HasValue ? t.ModifyDate.Date >= filterModel.ModifyDateFrom.Value.Date : false) && (t.ModifyDate.ToString() != "1.1.0001 г. 0:00:00" && filterModel.ModifyDateTo.HasValue ? t.ModifyDate.Date <= filterModel.ModifyDateTo.Value.Date : false));
            }
            else if (filterModel.ModifyDateFrom.HasValue && !filterModel.ModifyDateTo.HasValue)/*Дата на последна актуализация От*/
            {
                filter = filter.And(t => t.ModifyDate.ToString() != "1.1.0001 г. 0:00:00" && filterModel.ModifyDateFrom.HasValue ? t.ModifyDate.Date >= filterModel.ModifyDateFrom.Value.Date : false);
            }
            else if (filterModel.ModifyDateTo.HasValue && !filterModel.ModifyDateFrom.HasValue)/*Дата на последна актуализация До*/
            {
                filter = filter.And(t => t.ModifyDate.ToString() != "1.1.0001 г. 0:00:00" && filterModel.ModifyDateTo.HasValue ? t.ModifyDate.Date <= filterModel.ModifyDateTo.Value.Date : false);
            }

            if (filterModel.IsNAPOOCheck)
            {
                filter = filter.And(t => t.CandidateProviderPremisesCheckings.Any());
            }

            /*Дата на проверка От/До*/
            if (filterModel.NAPOOCheckDateFrom.HasValue && filterModel.NAPOOCheckDateTo.HasValue)
            {
                filter = filter.And(t => t.CandidateProviderPremisesCheckings.Count > 0 ? t.CandidateProviderPremisesCheckings.Any(c => (c.CheckingDate.HasValue && filterModel.NAPOOCheckDateFrom.HasValue ? c.CheckingDate.Value.Date >= filterModel.NAPOOCheckDateFrom.Value.Date : false) && (c.CheckingDate.HasValue && filterModel.NAPOOCheckDateTo.HasValue ? c.CheckingDate.Value.Date <= filterModel.NAPOOCheckDateTo.Value.Date : false)) : false);
            }
            else if (filterModel.ModifyDateFrom.HasValue && !filterModel.ModifyDateTo.HasValue)/*Дата на проверка От*/
            {
                filter = filter.And(t => t.CandidateProviderPremisesCheckings.Count > 0 ? t.CandidateProviderPremisesCheckings.Any(c => c.CheckingDate.HasValue && filterModel.NAPOOCheckDateFrom.HasValue ? c.CheckingDate.Value.Date >= filterModel.NAPOOCheckDateFrom.Value.Date : false) : false);
            }
            else if (filterModel.ModifyDateTo.HasValue && !filterModel.ModifyDateFrom.HasValue)/*Дата на проверка До*/
            {
                filter = filter.And(t => t.CandidateProviderPremisesCheckings.Count > 0 ? t.CandidateProviderPremisesCheckings.Any(c => c.CheckingDate.HasValue && filterModel.NAPOOCheckDateTo.HasValue ? c.CheckingDate.Value.Date <= filterModel.NAPOOCheckDateTo.Value.Date : false) : false);
            }

            var premises = await this.repository.AllReadonly<CandidateProviderPremises>(filter)
                .To<CandidateProviderPremisesVM>(x => x.CandidateProviderPremisesCheckings,
                x => x.CandidateProviderPremisesSpecialities,
                x => x.Location.Municipality.District,
                x => x.CandidateProvider).ToListAsync();

            foreach (var mtb in premises)
            {
                var provider = mtb.CandidateProvider;
                haveCandProvPremisesCheckings = false;

                var licenseType = kvLicenseTypeSource.FirstOrDefault(x => x.IdKeyValue == provider.IdTypeLicense);
                if (licenseType is not null)
                {
                    provider.LicenceTypeValue = licenseType.Name;
                }

                provider.ProviderNameAndOwnerForRegister = provider.IdTypeLicense == kvLicenseForCPOValue.IdKeyValue ? provider.CPONameOwnerGrid : provider.CIPONameOwnerGrid;

                var ownershipType = kvOwnerShipTypeSource.FirstOrDefault(x => x.IdKeyValue == mtb.IdOwnership);
                if (ownershipType is not null)
                {
                    mtb.OwnershipValue = ownershipType.Name;
                }

                var statusType = kvPremisesStatusSource.FirstOrDefault(x => x.IdKeyValue == mtb.IdStatus);
                if (statusType is not null)
                {
                    mtb.StatusValue = statusType.Name;
                }

                if(mtb.CandidateProviderPremisesCheckings.Any())
                {
                    haveCandProvPremisesCheckings = true;
                }


                RegisterMTBVM mtbVM = new RegisterMTBVM()
                {
                    CandidateProvider = provider,
                    CandidateProviderPremises = mtb,
                    haveCandidateProviderPremisesCheckings = haveCandProvPremisesCheckings
                };

                mtbsSource.Add(mtbVM);
            }

            return mtbsSource;
        }



        public async Task<RegisterMTBVM> GetRegisterMTBVMByIdCandidateProviderPremisesAsync(int idCandidateProviderPremises)
        {
            var mtbsSource = new List<RegisterMTBVM>();
            var kvPremisesStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseStatus");
            var kvOwnerShipTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseOwnership");
            var kvLicenseTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingType");
            var kvLicenseForCPOValue = kvLicenseTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "LicensingCPO");

            var candidateProviderPremises = this.repository.AllReadonly<CandidateProviderPremises>(x => x.IdCandidateProviderPremises == idCandidateProviderPremises);
            var candidateProviderPremisesAsVM = await candidateProviderPremises.To<CandidateProviderPremisesVM>(x => x.Location.Municipality.District,
                x => x.CandidateProvider,
                x => x.CandidateProviderPremisesSpecialities,
                x => x.CandidateProviderPremisesCheckings).ToListAsync();


            foreach (var premises in candidateProviderPremisesAsVM)
            {
                var licenseType = kvLicenseTypeSource.FirstOrDefault(x => x.IdKeyValue == premises.CandidateProvider.IdTypeLicense);
                if (licenseType is not null)
                {
                    premises.CandidateProvider.LicenceTypeValue = licenseType.Name;
                }

                premises.CandidateProvider.ProviderNameAndOwnerForRegister = premises.CandidateProvider.IdTypeLicense == kvLicenseForCPOValue.IdKeyValue ? premises.CandidateProvider.CPONameOwnerGrid : premises.CandidateProvider.CIPONameOwnerGrid;

                var ownershipType = kvOwnerShipTypeSource.FirstOrDefault(x => x.IdKeyValue == premises.IdOwnership);
                if (ownershipType is not null)
                {
                    premises.OwnershipValue = ownershipType.Name;
                }

                var statusType = kvPremisesStatusSource.FirstOrDefault(x => x.IdKeyValue == premises.IdStatus);
                if (statusType is not null)
                {
                    premises.StatusValue = statusType.Name;
                }

                RegisterMTBVM mtbVM = new RegisterMTBVM()
                {
                    CandidateProvider = premises.CandidateProvider,
                    CandidateProviderPremises = premises
                };

                mtbsSource.Add(mtbVM);
            }

            return mtbsSource.FirstOrDefault()!;
        }

        public async Task<IEnumerable<RegisterTrainerVM>> GetCandidateProviderTrainersByFilterModelAsync(RegisterTrainerVM model)
        {
            var trainers = new List<RegisterTrainerVM>();

            var filter = PredicateBuilder.True<CandidateProviderTrainer>();
            if (model.IsCPO)
            {
                filter = filter.And(x => x.CandidateProvider.IdTypeLicense == this.kvCPO.IdKeyValue);
            }
            else
            {
                filter = filter.And(x => x.CandidateProvider.IdTypeLicense == this.kvCIPO.IdKeyValue);
            }

            if (model.IdCandidateProvider.HasValue)
            {
                filter = filter.And(x => x.IdCandidate_Provider == model.IdCandidateProvider.Value);
            }

            if (!string.IsNullOrEmpty(model.LicenseNumber))
            {
                filter = filter.And(x => !string.IsNullOrEmpty(x.CandidateProvider.LicenceNumber) && x.CandidateProvider.LicenceNumber.Contains(model.LicenseNumber.Trim()));
            }

            if (model.LicenseDate.HasValue)
            {
                filter = filter.And(x => x.CandidateProvider.LicenceDate.HasValue && x.CandidateProvider.LicenceDate.Value.Date == model.LicenseDate.Value.Date);
            }

            if (model.IdStatus.HasValue)
            {
                filter = filter.And(x => x.IdStatus == model.IdStatus);
            }

            if (!string.IsNullOrEmpty(model.FirstName))
            {
                filter = filter.And(x => x.FirstName.ToLower().Contains(model.FirstName.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(model.SecondName))
            {
                filter = filter.And(x => !string.IsNullOrEmpty(x.SecondName) && x.SecondName.ToLower().Contains(model.SecondName.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(model.FamilyName))
            {
                filter = filter.And(x => x.FamilyName.ToLower().Contains(model.FamilyName.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(model.Indent))
            {
                filter = filter.And(x => !string.IsNullOrEmpty(x.Indent) && x.Indent.Contains(model.Indent.Trim().ToLower()));
            }

            if (model.IdEducation.HasValue)
            {
                filter = filter.And(x => x.IdEducation == model.IdEducation);
            }

            if (model.IdContractType.HasValue)
            {
                filter = filter.And(x => x.IdContractType == model.IdContractType);
            }

            if (!string.IsNullOrEmpty(model.EducationSpecialityNotes))
            {
                filter = filter.And(x => !string.IsNullOrEmpty(x.EducationSpecialityNotes) && x.EducationSpecialityNotes.Contains(model.EducationSpecialityNotes.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(model.EducationCertificateNotes))
            {
                filter = filter.And(x => !string.IsNullOrEmpty(x.EducationCertificateNotes) && x.EducationCertificateNotes.Contains(model.EducationCertificateNotes.Trim().ToLower()));
            }

            if (model.IdProfessionalDirection.HasValue)
            {
                filter = filter.And(x => x.CandidateProviderTrainerProfiles.Any(y => y.IdProfessionalDirection == model.IdProfessionalDirection.Value));
            }

            if (model.IdProfession.HasValue)
            {
                filter = filter.And(x => x.CandidateProviderTrainerProfiles.Any(y => y.ProfessionalDirection.Professions.Any(z => z.IdProfession == model.IdProfession.Value)));
            }

            if (model.IdSpeciality.HasValue)
            {
                filter = filter.And(x => x.CandidateProviderTrainerProfiles.Any(y => y.ProfessionalDirection.Professions.Any(z => z.Specialities.Any(o => o.IdSpeciality == model.IdSpeciality.Value))));
            }

            if (model.IdTrainingType.HasValue)
            {
                var kvTheory = await this.dataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TheoryTraining");
                var kvPractice = await this.dataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "PracticalTraining");
                if (model.IdTrainingType.Value == kvTheory.IdKeyValue)
                {
                    filter = filter.And(x => x.CandidateProviderTrainerProfiles.Any(y => y.IsTheory));
                }
                else if (model.IdTrainingType == kvPractice.IdKeyValue)
                {
                    filter = filter.And(x => x.CandidateProviderTrainerProfiles.Any(y => y.IsPractice));
                }
                else
                {
                    filter = filter.And(x => x.CandidateProviderTrainerProfiles.Any(y => y.IsPractice && y.IsTheory));
                }
            }

            if (model.CreationDateFrom.HasValue)
            {
                filter = filter.And(x => x.CreationDate.Date >= model.CreationDateFrom.Value.Date);
            }

            if (model.CreationDateTo.HasValue)
            {
                filter = filter.And(x => x.CreationDate.Date <= model.CreationDateTo.Value.Date);
            }

            if (model.ModifyDateFrom.HasValue)
            {
                filter = filter.And(x => x.ModifyDate.Date >= model.ModifyDateFrom.Value.Date);
            }

            if (model.ModifyDateTo.HasValue)
            {
                filter = filter.And(x => x.ModifyDate.Date <= model.ModifyDateTo.Value.Date);
            }

            if (model.IdFilterDataType.HasValue)
            {
                var kvYesValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("FilterDataType", "Yes");
                if (model.IdFilterDataType.Value == kvYesValue.IdKeyValue)
                {
                    filter = filter.And(x => x.CandidateProviderTrainerCheckings.Any());
                }
                else
                {
                    filter = filter.And(x => !x.CandidateProviderTrainerCheckings.Any());
                }
            }

            if (model.NAPOOCheckDateFrom.HasValue)
            {
                filter = filter.And(x => x.CandidateProviderTrainerCheckings.Any(y => y.CheckingDate.HasValue && y.CheckingDate.Value.Date >= model.NAPOOCheckDateFrom.Value.Date));
            }

            if (model.NAPOOCheckDateTo.HasValue)
            {
                filter = filter.And(x => x.CandidateProviderTrainerCheckings.Any(y => y.CheckingDate.HasValue && y.CheckingDate.Value.Date <= model.NAPOOCheckDateTo.Value.Date));
            }

            var candidateProviderTrainers = await this.repository.AllReadonly<CandidateProviderTrainer>(filter)
                .To<CandidateProviderTrainerVM>(x => x.CandidateProvider, x => x.CandidateProviderTrainerProfiles.Select(y => y.ProfessionalDirection))
                .ToListAsync();
            foreach (var trainer in candidateProviderTrainers)
            {
                trainers.Add(new RegisterTrainerVM()
                {
                    IdEntity = trainer.IdCandidateProviderTrainer,
                    FirstName = trainer.FirstName,
                    SecondName = trainer.SecondName,
                    FamilyName = trainer.FamilyName,
                    LicenseNumber = trainer.CandidateProvider.LicenceNumber,
                    OwnerAndProvider = model.IsCPO ? !string.IsNullOrEmpty(trainer.CandidateProvider.ProviderName) ? $"ЦПО {trainer.CandidateProvider.ProviderName} към {trainer.CandidateProvider.ProviderOwner}" : $"ЦПО към {trainer.CandidateProvider.ProviderOwner}" : !string.IsNullOrEmpty(trainer.CandidateProvider.ProviderName) ? $"ЦИПО {trainer.CandidateProvider.ProviderName} към {trainer.CandidateProvider.ProviderOwner}" : $"ЦИПО към {trainer.CandidateProvider.ProviderOwner}",
                    ProfessionalDirections = string.Join("; ", trainer.CandidateProviderTrainerProfiles.Select(x => x.ProfessionalDirection.DisplayNameAndCode).ToList()),
                    StatusValue = trainer.IdStatus.HasValue ? (await this.dataSourceService.GetKeyValueByIdAsync(trainer.IdStatus.Value)).Name : string.Empty
                });
            }

            return trainers.OrderBy(x => x.FirstName).ThenBy(x => x.SecondName).ThenBy(x => x.FamilyName).ToList();
        }
        #endregion
    }
}
