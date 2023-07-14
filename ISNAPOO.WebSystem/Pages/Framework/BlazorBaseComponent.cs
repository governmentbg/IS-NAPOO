using System.Linq;
using System.Security.Claims;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Data.Models.Framework;
using Data.Models.Migrations;
using DocuServiceReference;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Candidate;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.JSInterop;
using RegiX.Class.AVTR.GetActualState;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using static ISNAPOO.WebSystem.Extensions.BlazorCookieLoginMiddleware;
using CallContext = RegiXServiceReference.CallContext;

namespace ISNAPOO.WebSystem.Pages.Framework
{
    public class BlazorBaseComponent : ComponentBase, IDisposable
    {


        [Inject]
        protected ILocalStorageService _localStorage { get; set; }


        [Inject]
        protected ISessionStorageService _sessionStorage { get; set; }


        [Inject]
        public AuthenticationStateProvider authenticationStateProvider { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }


        [Inject]
        private IDataSourceService DataSourceService { get; set; }

        [Inject]
        private IConcurrencyService ConcurrencyService { get; set; }

        [Inject]
        private ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        private ITrainingService TrainingService { get; set; }

        [Inject]
        private INotificationService NotificationService { get; set; }

        [Inject]
        private IPersonService PersonService { get; set; }

        [Inject]
        private IRegiXLogRequestService RegiXLogRequestService { get; set; }

        [Inject]
        IJSRuntime JS { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }

        [Inject]
        public ILogger<BlazorBaseComponent> Logger { get; set; }

        [Inject]
        public SfDialogService DialogService { get; set; }


        [Parameter]
        [SupplyParameterFromQuery]
        public string? Token { get; set; }


        [CascadingParameter]
        public Task<AuthenticationState> authStateCascadingParameter { get; set; }

        #region Concurrency Ids
        protected int IdCandidateProvider { get; set; }

        protected int IdProgram { get; set; }

        protected int IdTrainingProgramCurriculum { get; set; }

        protected int IdTrainingCourse { get; set; }

        protected int IdCourseSchedule { get; set; }

        protected int IdClientCourse { get; set; }

        protected int IdConsultingClient { get; set; }

        protected int IdSelfAssessmentReport { get; set; }

        protected int IdSurvey { get; set; }

        protected int IdQuestion { get; set; }
        #endregion

        protected EditContext editContext;
        protected DialogEffect AnimationEffect = DialogEffect.Zoom;
        protected string dialogClass = "";

        protected bool isSubmitClicked = false;
        protected bool showConfirmDialog = false;
        protected bool closeConfirmed = false;
        protected bool isVisible = false;
        protected bool loading = false;

        protected List<int> personIds = new List<int>();

        protected bool showConcurrencyDialog = false;
        protected string personFullName = string.Empty;

        private List<ConcurrencyInfo> CurrentlyOpenedModalsIdList
        {
            get
            {
                return this.ConcurrencyService.GetAllCurrentlyOpenedModals();
            }
        }

        protected double MinFileSize
        {
            get
            {
                var fileSetting = this.DataSourceService.GetSettingByIntCodeAsync("MinSizeFileUpload").Result;

                if (fileSetting == null) { return 1; }
                return double.Parse(fileSetting.SettingValue);
            }
        }

        protected double MaxFileSize
        {
            get
            {
                var fileSetting = this.DataSourceService.GetSettingByIntCodeAsync("MaxSizeFileUpload").Result;
                if(fileSetting == null) { return GlobalConstants.MAX_DEFAULT_FILE_SIZE; }// 5 MB 
                return double.Parse(fileSetting.SettingValue);
            }
        }

        #region RegiX Law Reason KV
        private IEnumerable<KeyValueVM> RegiXLawReasonsSource => this.GetRegiXLawReasonKVSourceAsync().Result;
        /// <summary>
        /// Справка по код на БУЛСТАТ или по фирмено дело за актуално състояние на субект на БУЛСТАТ – чл. 22 от ЗПОО
        /// </summary>
        protected string BulstatCheckKV => this.RegiXLawReasonsSource.FirstOrDefault(x => x.KeyValueIntCode == "BulstatCheck").Name;

        /// <summary>
        /// Справка по физическо лице за участие в търговски дружества - чл. 49а, ал. 4, т. 1 от ЗПОО
        /// </summary>
        protected string IndividualCheckKV => this.RegiXLawReasonsSource.FirstOrDefault(x => x.KeyValueIntCode == "IndividualCheck").Name;

        /// <summary>
        /// Справка за актуално състояние (v1) – чл. 49а, ал. 4, т. 1 и чл. 49б, ал. 2, т. 2 от ЗПОО
        /// </summary>
        protected string CurrentStatusCheckKV => this.RegiXLawReasonsSource.FirstOrDefault(x => x.KeyValueIntCode == "CurrentStatusCheck").Name;

        /// <summary>
        /// Справка за валидност на ЕИК номер - чл. 49а, ал. 4, т. 1 от ЗПОО
        /// </summary>
        protected string BulstatValidityCheckKV => this.RegiXLawReasonsSource.FirstOrDefault(x => x.KeyValueIntCode == "BulstatValidityCheck").Name;

        /// <summary>
        /// Справка за актуално състояние за всички вписани обстоятелства (v2) - чл. 49а, ал. 4 ,т. 1 от ЗПОО
        /// </summary>
        protected string CurrentStatusRegisteredCircumstancesCheckKV => this.RegiXLawReasonsSource.FirstOrDefault(x => x.KeyValueIntCode == "CurrentStatusRegisteredCircumstancesCheck").Name;

        /// <summary>
        /// Справка за диплома за средно образование - чл.14, ал. 4  и чл. 38 от ЗПОО
        /// </summary>
        protected string SecondarySchoolDiplomaCheckKV => this.RegiXLawReasonsSource.FirstOrDefault(x => x.KeyValueIntCode == "SecondarySchoolDiplomaCheck").Name;

        /// <summary>
        /// Справка за валидност на физическо лице – чл. 38 от ЗПОО
        /// </summary>
        protected string IndividualValidityCheckKV => this.RegiXLawReasonsSource.FirstOrDefault(x => x.KeyValueIntCode == "IndividualValidityCheck").Name;

        /// <summary>
        /// НКПД - чл. 1, ал. 1, т. 2 от ЗПОО
        /// </summary>
        protected string NKPDCheckKV => this.RegiXLawReasonsSource.FirstOrDefault(x => x.KeyValueIntCode == "NKPDCheck").Name;
        #endregion

        public int RowCount = 0;

        [CascadingParameter]
        public MainLayout MainLayoutObj { get; set; }

        public string FormTitle { get; set; }

        public virtual ConfirmDialog ConfirmDialog { get; set; }

        public virtual ConcurrencyDialog ConcurrencyDialog { get; set; }

        public virtual bool IsContextModified { get; set; }

        protected async Task AddEntityIdAsCurrentlyOpened(int idEntity, string type)
        {
            await this.ConcurrencyService.AddEntityIdAsCurrentlyOpened(idEntity, type);
        }

        protected void RemoveEntityIdAsCurrentlyOpened(int idEntity, string type)
        {
            this.ConcurrencyService.RemoveEntityIdAsCurrentlyOpened(idEntity, type);
        }

        protected ConcurrencyInfo GetAllCurrentlyOpenedModalsConcurrencyInfoValue(int idEntity, string type)
        {
            return this.CurrentlyOpenedModalsIdList.FirstOrDefault(x => x.IdEntity == idEntity && x.EntityType == type);
        }

        protected void SetPersonFullNameAndVisibilityOfDialog(ConcurrencyInfo concurrencyInfo)
        {
            this.MainLayoutObj.ConcurrencyShow(concurrencyInfo);
        }

        // проверява дали потребителят е администратор на профила на ЦПО/ЦИПО
        protected async Task<bool> IsPersonProfileAdministratorAsync()
        {
            if (this.UserProps.IdCandidateProvider != 0)
            {
                return await this.CandidateProviderService.IsCandidateProviderPersonProfileAdministratorByIdPersonAsync(this.UserProps.IdPerson, this.UserProps.IdCandidateProvider);
            }

            return false;
        }

        // метод за показване/скриване на модал при незапазени промени
        protected void SetConfirmDialogVisibility()
        {
            if (this.IsContextModified)
            {
                if (this.closeConfirmed)
                {
                    this.closeConfirmed = false;
                    this.isVisible = false;
                }
                else
                {
                    this.closeConfirmed = false;
                }
            }
            else
            {
                this.isVisible = false;
            }

            this.RemoveDataAccordingToSourceType();
        }

        public void ConfirmDialogCallback(bool closeConfirmed)
        {
            this.closeConfirmed = closeConfirmed;
            this.SetConfirmDialogVisibility();
            this.StateHasChanged();
        }


        // показва модал(диалог) за потвърждение при действие
        public async Task<bool> ShowConfirmDialogAsync(string message)
        {
            return await this.DialogService.ConfirmAsync(message, "Внимание!", new DialogOptions()
            {
                CssClass = "predefinedDialog ",
                AnimationSettings = new DialogAnimationSettings() { Effect = DialogEffect.Zoom, Duration = 500 },
                PrimaryButtonOptions = new DialogButtonOptions() { Content = "Да" },
                CancelButtonOptions = new DialogButtonOptions() { Content = "Отказ" },
            });
        }

        // показва модал(диалог) за известяване при действие
        public async void ShowAlertDialog(string message)
        {
            await this.DialogService.AlertAsync(message, "Внимание!", new DialogOptions()
            {
                CssClass = "predefinedDialog ",
                AnimationSettings = new DialogAnimationSettings() { Effect = DialogEffect.Zoom, Duration = 500 },
                PrimaryButtonOptions = new DialogButtonOptions() { Content = "Затвори" }
            });
        }

        // методът се закача за @onclick на бутон Отказ в модал
        public void CancelClickedHandler()
        {
            this.showConfirmDialog = this.IsContextModified;

            if (this.showConfirmDialog)
            {
                this.ConfirmDialog.showConfirmDialog = this.showConfirmDialog;
                this.StateHasChanged();
            }
            else
            {
                this.SetConfirmDialogVisibility();
            }
        }

        // методът се закача за @onclick на бутон Запиши в модал
        public void SaveClickedHandler()
        {
            this.showConfirmDialog = this.IsContextModified;

            if (this.showConfirmDialog)
            {
                this.ConfirmDialog.showConfirmDialog = this.showConfirmDialog;
                this.StateHasChanged();
            }
            else
            {
                this.SetConfirmDialogVisibility();
            }
        }

        // методът се закача за OnClose event на SfDialog
        protected virtual void OnXClickHandler(BeforeCloseEventArgs args)
        {
            if (args.ClosedBy == "Close Icon")
            {
                args.Cancel = true;

                this.CancelClickedHandler();
            }
        }

        public virtual void SubmitHandler()
        {
            throw new Exception("Not implement SubmitHandler.");
        }

        public IEnumerable<string> GetValidationMessages()
        {
            IEnumerable<string> res = new List<string>();

            res = this.editContext.GetValidationMessages();

            return res.Select(c => $"{c} ({FormTitle})");
        }

        public async Task<string> GetRowNumber<T>(SfGrid<T> sfGrid, int key)
        {
            var page = sfGrid.PageSettings.CurrentPage - 1;
            var pageLength = sfGrid.PageSettings.PageSize;
            var index = await sfGrid.GetRowIndexByPrimaryKeyAsync(key);
            var num = page * pageLength + index;

            return $"{num + 1}.";
        }

        public bool IsEditContextModified()
        {
            if (this.editContext is not null)
            {
                return this.editContext.IsModified();
            }
            else
            {
                return false;
            }
        }

        protected async Task<bool> IsInRole(string role)
        {
            var authenticationState = await this.authenticationStateProvider.GetAuthenticationStateAsync();

            return authenticationState.User.IsInRole(role);
        }

        protected async Task<bool> HasClaim(string claimType)
        {
            var policy = await this.DataSourceService.GetPolicyByCode(claimType);

            try
            {
                var LOCAL_LOGIN_KEY = await _localStorage.GetItemAsync<string>("LOCAL_LOGIN_KEY");

                if (!string.IsNullOrEmpty(LOCAL_LOGIN_KEY))
                {

                    LoginInfo loginInfo = null;
                    if (BlazorCookieLoginMiddleware.OnlineUsers.TryGetValue(LOCAL_LOGIN_KEY, out loginInfo))
                    {
                        loginInfo.Activity = policy == null ? "" : policy.PolicyDescription;
                        loginInfo.LastActivity = DateTime.Now;
                    }
                }
            }
            catch { }



           

            var authenticationState = await this.authenticationStateProvider.GetAuthenticationStateAsync();
            this.authenticationStateProvider.AuthenticationStateChanged += AuthenticationStateProvider_AuthenticationStateChanged;

            var runtimeDataValue = authenticationState.User.Identities.Where(x => x.AuthenticationType == "RuntimeData").FirstOrDefault();

            if (runtimeDataValue != null)
            {
                var runtimeData = runtimeDataValue as ClaimsIdentity;
                runtimeData.RemoveClaim(runtimeData.FindFirst("currentAction"));
                runtimeData.AddClaim(new Claim("currentAction", claimType));

                runtimeData.RemoveClaim(runtimeData.FindFirst("currentActionDescription"));
                runtimeData.AddClaim(new Claim("currentActionDescription", policy.PolicyDescription));

               


                try
                {
                    runtimeData.RemoveClaim(runtimeData.FindFirst("currentUrl"));
                    var currentUrl = NavigationManager.Uri.Substring(NavigationManager.BaseUri.Length);
                    runtimeData.AddClaim(new Claim("currentUrl", currentUrl));

                    var urlparts = currentUrl.Split('?');

                    if (urlparts.Length == 1) 
                    {
                        var menuText = this.DataSourceService.GetAllMenuNode().Where(x => x.URL == urlparts[0]).FirstOrDefault();
                        runtimeData.RemoveClaim(runtimeData.FindFirst("currentMenu"));
                        runtimeData.AddClaim(new Claim("currentMenu", menuText?.Name));
                    }

                    else if (urlparts.Length == 2)
                    {
                        var menuText = this.DataSourceService.GetAllMenuNode().Where(
                                x => 
                                    x.URL == urlparts[0] &&
                                    x.QueryParams == "?" + urlparts[1]
                                ).FirstOrDefault();

                        runtimeData.RemoveClaim(runtimeData.FindFirst("currentMenu"));
                        runtimeData.AddClaim(new Claim("currentMenu", menuText?.Name));
                    }
                }
                catch{}
                


                //if (menuText.Any()) {
                //    runtimeData.RemoveClaim(runtimeData.FindFirst("currentMenu"));
                //    runtimeData.AddClaim(new Claim("currentMenu", menuText.First().Name));
                //}

                

            }



            if (await IsInRole("SUPPORT")) { return true; }

            //authenticationState.User.

            if (authenticationState == null)
            {
                return false;
            }



            return authenticationState.User.Claims.Any(c => c.Type == claimType);


        }

        private void AuthenticationStateProvider_AuthenticationStateChanged(Task<AuthenticationState> task)
        {
            
            //this.authStateCascadingParameter = task;
        }

        protected List<string> GetUserRoles()
        {
            //var authenticationState = await authStateCascadingParameter
            var authenticationState = this.authenticationStateProvider.GetAuthenticationStateAsync().Result;
            var roles = ((ClaimsIdentity)authenticationState.User.Identity).Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value);

            return roles.ToList();
        }

        protected async Task<bool> CheckUserActionPermission(string claimType, bool redirect)
        {

            // Logger.LogError("claimType:" + claimType);
            bool result = await HasClaim(claimType);

            if (!result)
            {
                if (redirect)
                {
                    NavigationManager.NavigateTo("/NoPermission");
                }
                else
                {
                    if (this.ConfirmDialog != null)
                    {
                        result = false;
                        this.ConfirmDialog.showPermitedActionDialog = true;
                        this.StateHasChanged();
                    }
                    else
                    {
                        //await JS.InvokeVoidAsync("alert", "Нямате права за това действие");
                    }

                }
            }

            return result;


        }

        public async Task<bool> IsSecondInitAsync(IJSRuntime _jSRuntime)
        {
            bool result = false;
            try
            {
                var res = await _jSRuntime.InvokeAsync<bool>("isPreRendering");
                result = !res;
            }
            catch (Exception)
            {

            }

            return result;
        }

        protected override async void OnAfterRender(bool firstRender)
        {

        }

        protected override async Task OnInitializedAsync()
        {
            //var authenticationState = this.authenticationStateProvider.GetAuthenticationStateAsync().Result;

            var authenticationState = await authStateCascadingParameter;

            if (this.NavigationManager.Uri.Contains("/SurveyFilingOut"))
            {
                return;
            }

            if (!authenticationState.User.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/Login?returnUrl=" + System.Net.WebUtility.UrlEncode(new Uri(NavigationManager.Uri).PathAndQuery.Replace("/", "")));
            }


        }

        protected void SpinnerHide()
        {
            this.MainLayoutObj.SpinnerHide();
        }

        protected void SpinnerShow()
        {
            this.MainLayoutObj.SpinnerShow();
        }

        protected async Task ShowErrorAsync(string message)
        {
            this.MainLayoutObj.SpinnerHide();
            this.MainLayoutObj.toast.sfErrorToast.Content = message;
            await this.MainLayoutObj.toast.sfErrorToast.ShowAsync();
        }

        protected async Task ShowSuccessAsync(string message)
        {
            this.MainLayoutObj.SpinnerHide();
            this.MainLayoutObj.toast.sfSuccessToast.Content = message;
            await this.MainLayoutObj.toast.sfSuccessToast.ShowAsync();
        }

        protected UserProps UserProps
        {
            get
            {
                UserProps userProps = new UserProps();

                try
                {
                    AuthenticationState AuthenticationState = this.authenticationStateProvider.GetAuthenticationStateAsync().Result;

                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_CANDIDATE_PROVIDER)))
                    {
                        userProps.IdCandidateProvider = Int32.Parse(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_CANDIDATE_PROVIDER));
                    }

                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(ClaimTypes.NameIdentifier)))
                    {
                        userProps.ID = AuthenticationState.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    }

                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_USER)))
                    {
                        userProps.UserId = Int32.Parse(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_USER));
                    }

                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_PERSON)))
                    {
                        userProps.IdPerson = Int32.Parse(AuthenticationState.User.FindFirstValue(GlobalConstants.ID_PERSON));
                    }

                    if (!string.IsNullOrEmpty(AuthenticationState.User.FindFirstValue(GlobalConstants.PERSON_FULLNAME)))
                    {
                        userProps.PersonName = AuthenticationState.User.FindFirstValue(GlobalConstants.PERSON_FULLNAME);
                    }

                    if (!string.IsNullOrEmpty(AuthenticationState.User.Identity.Name))
                    {
                        userProps.UserName = AuthenticationState.User.Identity.Name;
                    }

                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "remoteIpAddress"))
                    {
                        userProps.IPAddress = AuthenticationState.User.Claims.First(x => x.Type == "remoteIpAddress").Value;
                    }
                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "browserInformation"))
                    {
                        userProps.BrowserInformation = AuthenticationState.User.Claims.First(x => x.Type == "browserInformation").Value;
                    }


                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentAction"))
                    {
                        userProps.CurrentAction = AuthenticationState.User.Claims.First(x => x.Type == "currentAction").Value;
                    }

                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentActionDescription"))
                    {
                        userProps.CurrentActionDescription = AuthenticationState.User.Claims.First(x => x.Type == "currentActionDescription").Value;
                    }

                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentUrl"))
                    {
                        userProps.CurrentUrl = AuthenticationState.User.Claims.First(x => x.Type == "currentUrl").Value;
                    }

                    if (AuthenticationState != null && AuthenticationState.User != null && AuthenticationState.User.Claims.Any(x => x.Type == "currentMenu"))
                    {
                        userProps.CurrentMenu = AuthenticationState.User.Claims.First(x => x.Type == "currentMenu").Value;
                    }
                }
                catch { }


                return userProps;
            }
        }

        // отваря модала за създаване на нотификация
        protected async Task OpenSendNotificationModal(bool isOpenedFromSPPOOModule = false, List<int> personIds = null, List<ProcedureDocumentVM> procedureDocuments = null, List<FollowUpControlDocumentVM> followUpControlDocuments = null)
        {
            await this.MainLayoutObj.OpenNotificationModal(isOpenedFromSPPOOModule, personIds, procedureDocuments, followUpControlDocuments);
        }

        protected async Task LoadDataForPersonsToSendNotificationToAsync(string? entityType = null, int? entityId = null, List<int> idsCandidateProviders = null)
        {
            if (this.personIds != null)
            {
                this.personIds.Clear();
            }

            if (idsCandidateProviders != null)
            {
                foreach (var candidateProviderId in idsCandidateProviders)
                {
                    var list = await this.NotificationService.GetAllPersonIdsByCandidateProviderIdAsync(candidateProviderId);
                    this.personIds.AddRange(list);
                }

                return;
            }
            if (entityType == "FrameworkProgram")
            {

                var trainingProgram = await this.TrainingService.GetTrainingProgramByFrameworkProgramIdAsync(entityId.Value);
                if (trainingProgram != null)
                {
                    if (trainingProgram.IdCandidateProvider != null)
                    {
                        var candidateProviderId = trainingProgram.IdCandidateProvider;

                        var list =
                            await this.NotificationService.GetAllPersonIdsByCandidateProviderIdAsync(
                                candidateProviderId);
                        this.personIds.AddRange(list);
                    }
                }
            }
            else if (entityType != "StartedProcedure")
            {
                var specialityIds = this.GetSpecialityIds(entityType, entityId.Value);
                var candidateProviderIds = this.CandidateProviderService.GetCandidateProviderIdsBySpecialityIdsAndByIsActive(specialityIds);

                if (candidateProviderIds.Any())
                {
                    foreach (var candidateProviderId in candidateProviderIds)
                    {
                        var list = await this.NotificationService.GetAllPersonIdsByCandidateProviderIdAsync(candidateProviderId);
                        this.personIds.AddRange(list);
                    }
                }
            }
            else if (entityType == "CandidateProvider")
            {

            }
            else
            {
                var list = await this.NotificationService.GetAllPersonIdsByCandidateProviderIdAsync(entityId.Value);
                this.personIds.AddRange(list);
            }
        }

        /// <summary>
        /// Създава CallContext за изпращане на заявка към RegiX
        /// </summary>
        /// <param name="lawReason">Контекст на правното основание</param>
        /// <param name="remark">Допълнително поле в свободен текст</param>
        /// <param name="serviceType">Вид на услугата, във връзка с която се извиква операцията</param>
        /// <param name="serviceURI">Идентификатор на инстанцията на административната услуга или процедура в администрацията</param>
        /// <param name="employeeAdditionalIdentifier">Опционален допълнителен идентификатор на служител от администрация – например номер на служебната карта или друг индентификатор от информационната система - клиент</param>
        /// <returns></returns>
        protected async Task<CallContext> GetCallContextAsync(string lawReason, string? remark = null, string? serviceType = null, string? serviceURI = null, string? employeeAdditionalIdentifier = null)
        {
            PersonVM person = new PersonVM();

            if (this.UserProps != null && this.UserProps.IdPerson != 0)
            {
                person = await this.PersonService.GetPersonByIdAsync(this.UserProps.IdPerson);
            }
            else
            {
                person.FirstName = "*FirstName*";
                person.FamilyName = "*FamilyName*";
                person.Position = "*Position*";
            }

            var administrationName = (await this.DataSourceService.GetSettingByIntCodeAsync("AdministrationName")).SettingValue;
            var administrationOId = (await this.DataSourceService.GetSettingByIntCodeAsync("AdministrationOId")).SettingValue;

            CallContext callContext = new CallContext();
            callContext.AdministrationName = administrationName;
            callContext.AdministrationOId = administrationOId;
            callContext.EmployeeIdentifier = this.UserProps.UserName;
            callContext.EmployeeNames = person.FullName;
            callContext.EmployeePosition = person.Position;
            callContext.LawReason = lawReason;
            callContext.Remark = remark;
            callContext.ServiceType = serviceType;
            callContext.ServiceURI = serviceURI;
            callContext.EmployeeAditionalIdentifier = employeeAdditionalIdentifier;

            return callContext;
        }

        /// <summary>
        /// Създава запис в БД за логване на изпратена заявка към RegiX
        /// </summary>
        /// <param name="callContext">CallContext, използван при изпращане на заявката към RegiX</param>
        /// <returns></returns>
        protected async Task LogRegiXRequestAsync(CallContext callContext, bool isRequestValid)
        {
            ResultContext<RegiXLogRequestVM> inputContext = new ResultContext<RegiXLogRequestVM>();
            var model = new RegiXLogRequestVM();
            var msg = !isRequestValid ? $"Грешка при изпълнение на заявката." : "Заявката е изпълнена успешно.";
            model.AdministrationName = callContext.AdministrationName;
            model.AdministrationOId = callContext.AdministrationOId;
            model.EmployeeIdentifier = callContext.EmployeeIdentifier;
            model.EmployeePosition = callContext.EmployeePosition;
            model.EmployeeNames = callContext.EmployeeNames;
            model.ResponsiblePersonIdentifier = callContext.ResponsiblePersonIdentifier;
            model.LawReason = callContext.LawReason;
            model.Remark = callContext.Remark;
            model.ServiceType = callContext.ServiceType;
            model.ServiceURI = callContext.ServiceURI;
            model.ServiceResultStatus = msg;

            inputContext.ResultContextObject = model;

            await this.RegiXLogRequestService.CreateRegiXLogRequestAsync(inputContext);
        }

        private List<int> GetSpecialityIds(string entityType, int entityId)
        {
            var specialityIds = new List<int>();

            switch (entityType)
            {
                case SPPOOTypes.Area:
                    specialityIds = this.SpecialityService.GetSpecialitiesIdsByIdArea(entityId).ToList();
                    break;
                case SPPOOTypes.ProfessionalDirection:
                    specialityIds = this.SpecialityService.GetSpecialitiesIdsByIdProfessionalDirection(entityId).ToList();
                    break;
                case SPPOOTypes.Profession:
                    specialityIds = this.SpecialityService.GetSpecialitiesIdsByIdProfession(entityId).ToList();
                    break;
                case SPPOOTypes.Speciality:
                    specialityIds.Add(entityId);
                    break;
                case "DOC":
                    specialityIds = this.SpecialityService.GetSpecialitiesIdsByIdDOC(entityId).ToList();
                    break;
            }

            return specialityIds;
        }

        // прави проверка по Interface и тогава изтрива от ConcurrencyList
        private void RemoveDataAccordingToSourceType()
        {
            if (this is IConcurrencyCheck<CandidateProviderVM>)
            {
                this.RemoveEntityIdAsCurrentlyOpened(this.IdCandidateProvider, "CandidateProvider");
            }
            else if (this is IConcurrencyCheck<ProgramVM>)
            {
                this.RemoveEntityIdAsCurrentlyOpened(this.IdProgram, "TrainingProgram");
            }
            else if (this is IConcurrencyCheck<TrainingCurriculumVM>)
            {
                this.RemoveEntityIdAsCurrentlyOpened(this.IdTrainingProgramCurriculum, "TrainingProgramCurriculum");
            }
            else if (this is IConcurrencyCheck<CourseVM>)
            {
                this.RemoveEntityIdAsCurrentlyOpened(this.IdTrainingCourse, "TrainingCourse");
            }
            else if (this is IConcurrencyCheck<ClientCourseVM>)
            {
                this.RemoveEntityIdAsCurrentlyOpened(this.IdClientCourse, "TrainingClientCourse");
            }
            else if (this is IConcurrencyCheck<CourseScheduleVM>)
            {
                this.RemoveEntityIdAsCurrentlyOpened(this.IdCourseSchedule, "CourseSchedule");
            }
            else if (this is IConcurrencyCheck<ConsultingClientVM>)
            {
                this.RemoveEntityIdAsCurrentlyOpened(this.IdConsultingClient, "Consulting");
            }
            else if (this is IConcurrencyCheck<SelfAssessmentReportVM>)
            {
                this.RemoveEntityIdAsCurrentlyOpened(this.IdSelfAssessmentReport, "SelfAssessmentReport");
            }
            else if (this is IConcurrencyCheck<SurveyVM>)
            {
                this.RemoveEntityIdAsCurrentlyOpened(this.IdSurvey, "Survey");
            }
            else if (this is IConcurrencyCheck<QuestionVM>)
            {
                this.RemoveEntityIdAsCurrentlyOpened(this.IdQuestion, "Question");
            }
        }

        private async Task<IEnumerable<KeyValueVM>> GetRegiXLawReasonKVSourceAsync()
        {
            return await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RegiXLawReason");
        }

        public void Dispose()
        {
            this.RemoveDataAccordingToSourceType();
        }
    }
}
