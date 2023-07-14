using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Data.Models.Common;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Role;
using Data.Models.DB;
using DocuWorkService;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Assessment;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.DOC.NKPD;
using ISNAPOO.Core.Contracts.EGovPayment;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.Contracts.Licensing;
using ISNAPOO.Core.Contracts.Mailing;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.Services;
using ISNAPOO.Core.Services.Archive;
using ISNAPOO.Core.Services.Assessment;
using ISNAPOO.Core.Services.Candidate;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.Services.Control;
using ISNAPOO.Core.Services.DOC;
using ISNAPOO.Core.Services.DOC.NKPD;
using ISNAPOO.Core.Services.EGovPayment;
using ISNAPOO.Core.Services.EKATTE;
using ISNAPOO.Core.Services.ExternalExpertCommission;
using ISNAPOO.Core.Services.Licensing;
using ISNAPOO.Core.Services.Mailing;
using ISNAPOO.Core.Services.Rating;
using ISNAPOO.Core.Services.Request;
using ISNAPOO.Core.Services.SPPOO;
using ISNAPOO.Core.Services.Training;
using ISNAPOO.Core.UserContext;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.BISS;
using ISNAPOO.WebSystem.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegiX;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;
using Syncfusion.Licensing;
using static ISNAPOO.WebSystem.Pages.DOC.NKPD.NkpdSelectorModal;

namespace ISNAPOO.WebSystem.Extensions
{
    /// <summary>
    /// Описва услугите и контекстите на приложението
    /// </summary>
    public static class IOWebAppServiceCollectionExtension
    {
        /// <summary>
        /// Регистрира услугите на приложението в  IoC контейнера
        /// </summary>
        /// <param name="services">Регистрирани услуги</param>
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddHttpContextAccessor();
            services.AddBlazoredLocalStorage();
            services.AddBlazoredSessionStorage();

            services.AddScoped<IUserContext, UserContext>();

            services.AddScoped<SfDialogService>();

            services.AddTransient<IProviderService, ProviderService>();               
            services.AddTransient<IKeyTypeService, KeyTypeService>();
            services.AddTransient<IKeyValueService, KeyValueService>();
            services.AddTransient<IRepository, Repository>();
            services.AddTransient<ISettingService, SettingService>();
            services.AddTransient<IAreaService, AreaService>();
            services.AddTransient<IDOCService, DOCService>();
            services.AddTransient<IERUSpecialityService, ERUSpecialityService>();
            services.AddTransient<INotificationService, NotificationService>();
            services.AddTransient<IProfessionalDirectionService, ProfessionalDirectionService>();
            services.AddTransient<IProfessionService, ProfessionService>();
            services.AddTransient<ISpecialityService, SpecialityService>();
            services.AddTransient<INKPDService, NKPDService>();
            services.AddTransient<IMenuNodeService, MenuNodeService>();
            services.AddTransient<IDataSourceService, DataSourceService>();
            services.AddTransient<IConcurrencyService, ConcurrencyService>();
            services.AddTransient<IFrameworkProgramService, FrameworkProgramService>();
            services.AddTransient<IProfessionalDirectionOrderService, ProfessionalDirectionOrderService>();
            services.AddTransient<IProfessionOrderService, ProfessionOrderService>();
            services.AddTransient<ISpecialityOrderService, SpecialityOrderService>();
            services.AddTransient<IFrameworkProgramFormEducationService, FrameworkProgramFormEducationService>();
            services.AddTransient<ISpecialityNKPDService, SpecialityNKPDService>();
            services.AddTransient<IUploadFileService, UploadFileService>();
            services.AddTransient<IManagementDeadlineProcedureService, ManagementDeadlineProcedureService>();

            services.AddTransient<IRegisterService, RegisterService>();



            services.AddTransient<IMailService, MailService>();

            services.AddTransient<IDistrictService, DistrictService>();
            services.AddTransient<IMunicipalityService, MunicipalityService>();
            services.AddTransient<ILocationService, LocationService>();
            services.AddTransient<IRegionService, RegionService>();

            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<ILegalCapacityOrdinanceService, LegalCapacityOrdinanceService>();

            services.AddTransient<IExpertService, ExpertService>();
            services.AddTransient<IExpertProfessionalDirectionService, ExpertProfessionalDirectionService>();
            services.AddTransient<IExpertDocumentService, ExpertDocumentService>();

            services.AddTransient<IPersonService, PersonService>();

            services.AddTransient<ILicensingProcedureDocumentCPOService, LicensingProcedureDocumentCPOService>();
            services.AddTransient<ILicensingChangeProcedureCPOService, LicensingChangeProcedureCPOService>();

            services.AddTransient<ILicensingProcedureDocumentCIPOService, LicensingProcedureDocumentCIPOService>();

            services.AddTransient<ITemplateDocumentService, TemplateDocumentService>();



            if (Configuration.GetSection("AppSettings")["UseRegiXService"].ToString() == "YES")
            {
                services.AddTransient<IRegiXService, RegiXService>();
            }
            else {
                services.AddTransient<IRegiXService, RegiXServiceLocal>();
            }


            //
            

            services.AddTransient<CustomWebApiAdaptor>();

            services.AddSingleton<ILocService, LocService>();
            services.AddTransient<IPolicyService, PolicyService>();

            services.AddTransient<ICandidateProviderService, CandidateProviderService>();
            services.AddTransient<ICandidateCurriculumService, CandidateCurriculumService>();
            services.AddTransient<ICandidateCurriculumERUService, CandidateCurriculumERUService>();
            services.AddSingleton<ICommonService, CommonService>();
            services.AddTransient<IApplicationUserService, ApplicationUserService>();
            services.AddTransient<IProviderDocumentRequestService, ProviderDocumentRequestService>();



            services.AddTransient<IDocuService, DocuService>();

            services.AddTransient<IScheduleService, ScheduleService>();

            services.AddTransient<ITrainingService, TrainingService>();

            services.AddTransient<IArchiveService, ArchiveService>();

            services.AddTransient<IRegiXLogRequestService, RegiXLogRequestService>();

            services.AddTransient<ITimeStampService, TimeStampService>();

            services.AddTransient<IControlService, ControlService>();

            services.AddTransient<IAssessmentService, AssessmentService>();

            services.AddTransient<IRatingService, RatingService>();
            
            services.AddTransient<IPaymentService, PaymentService>();

            services.AddTransient<IAllowIPService, AllowIPService>();

            services.AddTransient<IEventLogService, EventLogService>();

            services.AddTransient<IImportUserService, ImportUserService>();

            //BISS Services


            services.AddTransient<IGetSignerService, GetSignerService>();
            services.AddTransient<IGetVersionService, GetVersionService>();
            services.AddTransient<ISignService, SignByBISS>();



        }

        /// <summary>
        /// Регистрира Syncfusion в апликацията
        /// </summary>
        /// <param name="services">Регистрирани услуги</param>
        public static void AddSyncfusionBlazor(this IServiceCollection services)
        {
            if (File.Exists(System.IO.Directory.GetCurrentDirectory() + "/SyncfusionLicense.txt"))
            {
                string licenseKey = System.IO.File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "/SyncfusionLicense.txt");
                SyncfusionLicenseProvider.RegisterLicense(licenseKey);
            }

            services.AddSyncfusionBlazor(options => { options.IgnoreScriptIsolation = true; });
            services.AddServerSideBlazor().AddHubOptions(options =>
            {
                options.MaximumReceiveMessageSize = 102400000;
            });
            
        }

        /// <summary>
        /// Регистрира контекстите на приложението в IoC контейнера
        /// </summary>
        /// <param name="services">Регистрирани услуги</param>
        /// <param name="Configuration">Настройки на приложението</param>
        public static void AddApplicationDbContext(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), 
                split => split.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)), ServiceLifetime.Transient);
        }

        /// <summary>
        /// Регистрира Identity provider в IoC контейнера
        /// </summary>
        /// <param name="services">Регистрирани услуги</param>
        public static async void AddApplicationIdentity(this IServiceCollection services)
        {

            var settingService = services.BuildServiceProvider().GetService<ISettingService>();
            

            var maxFailedAccessAttempts = await settingService.GetSettingByIntCodeAsync("MaxFailedAccessAttempts");

            var defaultLockoutTimeSpan = await settingService.GetSettingByIntCodeAsync("DefaultLockoutTimeSpan");

            services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
            {
                opt.Lockout.MaxFailedAccessAttempts = Int32.Parse(maxFailedAccessAttempts.SettingValue);
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(Int32.Parse(defaultLockoutTimeSpan.SettingValue));
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI()
            .AddPasswordValidator<PreventPasswordChangeValidator<ApplicationUser>>();


            services.AddAuthorization(options =>
            {  
                foreach(var policy in IPolicyService.GetPolicies())
                {
                    options.AddPolicy(policy.PolicyCode, c => c.RequireClaim(policy.PolicyCode));
                }
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireLowercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequiredLength = 3;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            });

      

            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            services.Configure<DataProtectionTokenProviderOptions>(options =>options.TokenLifespan = TimeSpan.FromHours(1));
           
        }
    }
}
