using System.Text.RegularExpressions;
using Blazored.LocalStorage;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using static ISNAPOO.WebSystem.Extensions.BlazorCookieLoginMiddleware;

namespace ISNAPOO.WebSystem.Pages.Common.UserManagement
{
    public partial class UserModal : BlazorBaseComponent
    {
        [Parameter]
        public EventCallback<ResultContext<ApplicationUserVM>> CallbackAfterSubmit { get; set; }

        [Inject]
        public IDataSourceService dataSourceService { get; set; }
        [Inject]
        public IProviderService providerService { get; set; }
        [Inject]
        public UserManager<ApplicationUser> userManager { get; set; }

        [Inject]
        public SignInManager<ApplicationUser> signInMgr { get; set; }

        [Inject]
        public ILocalStorageService localStorage { get; set; }

        [Inject]
        public NavigationManager navMng { get; set; }

        ApplicationUserVM applicationUserVM = new ApplicationUserVM();
        RoleSelectorModal roleSelectorModal = new RoleSelectorModal();
        private SfDialog sfDialog = new SfDialog();
        ToastMsg toast;
        IEnumerable<ApplicationRoleVM> roles;
        SfGrid<ApplicationRoleVM> refGrid;
        protected EditContext editContext;
        IEnumerable<KeyValueVM> keys;
        IEnumerable<KeyValueVM> kvIndentTypeSource;
        private ValidationMessageStore? messageStore;
        List<string> validationMessages = new List<string>();
        public string CreationDateStr { get; set; } = "";
        public string ModifyDateStr { get; set; } = "";
        private string identType = "";
        private bool isRoleAdmin = false;
        protected async override void OnInitialized()
        {
            this.editContext = new EditContext(this.applicationUserVM);
            this.kvIndentTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            keys = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("UserStatus");
            this.editContext.EnableDataAnnotationsValidation();
            var roleVMs = applicationUserVM.Roles.ToList();
           

        }


        private async Task GetSelectedRolesVM(ResultContext<List<ApplicationRoleVM>> resultContext)
        {

            foreach (var role in resultContext.ResultContextObject)
            {
                this.applicationUserVM.Roles.Add(new ApplicationRoleVM { Id = role.Id, Name = role.Name, RoleName = role.RoleName });
            }
            refGrid.Refresh();

        }

        private void ValidateEGN(object? sender, ValidationRequestedEventArgs args)
        {


            if (this.applicationUserVM.IndentTypeName != null)
            {
                this.applicationUserVM.IndentTypeName = this.applicationUserVM.IndentTypeName;

                if (this.applicationUserVM.IndentTypeName.Length > 10)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.applicationUserVM, "IndentTypeName");
                    this.messageStore?.Add(fi, "Полето 'ЕГН/ЛНЧ/ИДН' трябва да съдържа 10 символа!");
                }
                else
                {
                    var checkEGN = new BasicEGNValidation(this.applicationUserVM.IndentTypeName);

                    if (!checkEGN.Validate())
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.applicationUserVM, "IndentTypeName");
                        this.messageStore?.Add(fi, checkEGN.ErrorMessage);
                    }
                }
            }
            else
            {
                FieldIdentifier fi = new FieldIdentifier(this.applicationUserVM, "IndentTypeName");
                this.messageStore?.Add(fi, $"Полето '{this.identType}' е задължително!");
            }

        }

    
        private void ValidateFields(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();
            if (string.IsNullOrEmpty(this.applicationUserVM.Email))
            {
                FieldIdentifier fi = new FieldIdentifier(this.applicationUserVM, "Email");
                this.messageStore?.Add(fi, "Полето 'E-mail' е задължително!");
            }
            else
            {
                Regex regex = new Regex("(?:[a-z0-9!#$%&'*+\\=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+\\=?^_`{|}~-]+)*|\"\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\"\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])");
                Match match = regex.Match(this.applicationUserVM.Email);
                if (!match.Success)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.applicationUserVM, "Email");
                    this.messageStore?.Add(fi, "Въведеният E-mail е невалиден!");
                }
            }
        }
        private async Task SubmitUserHandler()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                bool hasPermission = await CheckUserActionPermission("ManageUserData", false);
                if (!hasPermission) { return; }
                var kvEGN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
                this.editContext = new EditContext(this.applicationUserVM);
                this.validationMessages.Clear();



                if (string.IsNullOrEmpty(this.applicationUserVM.Email) || !string.IsNullOrEmpty(this.applicationUserVM.Email))
                {
                    this.editContext.OnValidationRequested += this.ValidateFields;
                }
                if (this.applicationUserVM.IdIndentType.HasValue && kvEGN.IdKeyValue == this.applicationUserVM.IdIndentType)
                {
                    this.editContext.OnValidationRequested += this.ValidateEGN;
                }

                this.messageStore = new ValidationMessageStore(this.editContext);
                this.editContext.EnableDataAnnotationsValidation();
                bool isValid = this.editContext.Validate();
                this.validationMessages.AddRange(this.editContext.GetValidationMessages());

                if (isValid && this.validationMessages.Count == 0)
                {   
                    ResultContext<ApplicationUserVM> resultContext = new ResultContext<ApplicationUserVM>();

                    resultContext.ResultContextObject = this.applicationUserVM;
             
                    this.SpinnerShow();
                    if (string.IsNullOrEmpty(this.applicationUserVM.Id))
                    {
              
                        resultContext = await this.ApplicationUserService.CreateApplicationUserAsAdminAsync(resultContext);
                        if (resultContext.ResultContextObject.UserName != null)
                        {
                            var user = await userManager.FindByNameAsync(resultContext.ResultContextObject.UserName);
                            applicationUserVM = await this.ApplicationUserService.GetApplicationUsersAndPersonById(user.Id);
                        }
                    }
                    else
                    {
                        resultContext = await this.ApplicationUserService.UpdateApplicationUserAsync(resultContext);
                    }


                    if (resultContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                        applicationUserVM.ModifyUserName = await ApplicationUserService.GetApplicationUsersPersonNameAsync(resultContext.ResultContextObject.IdModifyUser);
                        applicationUserVM.CreateUserName = await ApplicationUserService.GetApplicationUsersPersonNameAsync(resultContext.ResultContextObject.IdCreateUser);
                        applicationUserVM.CreationDate = resultContext.ResultContextObject.CreationDate;
                        applicationUserVM.ModifyDate = resultContext.ResultContextObject.ModifyDate;
                        this.CreationDateStr = applicationUserVM.CreationDate.ToString("dd.MM.yyyy");
                        this.ModifyDateStr = applicationUserVM.ModifyDate.ToString("dd.MM.yyyy");
                        resultContext.ResultContextObject = applicationUserVM;
                    }

                    this.SpinnerHide();
                    await this.CallbackAfterSubmit.InvokeAsync(resultContext);

                }
            }
            finally
            {
                this.loading = false;
            }
        }


       public async Task OpenModal(ApplicationUserVM _applicationUserVM)
       {
            //Чистя го защото когато се отвори втори път се дублират ролите
            this.applicationUserVM.Roles.Clear();

            this.isRoleAdmin = await this.IsInRole("ADMIN");
       
            if (_applicationUserVM.Id == null)
            {
                _applicationUserVM.IdUserStatus = (keys.Where(x => x.KeyValueIntCode.Equals("Active")).First()).IdKeyValue;
                this.CreationDateStr = null;
                this.ModifyDateStr = null;
                _applicationUserVM.ModifyUserName = "";
                _applicationUserVM.CreateUserName = "";
            }
            else
            {

                _applicationUserVM.ModifyUserName = await ApplicationUserService.GetApplicationUsersPersonNameAsync(_applicationUserVM.IdModifyUser);
                _applicationUserVM.CreateUserName = await ApplicationUserService.GetApplicationUsersPersonNameAsync(_applicationUserVM.IdCreateUser);
                this.CreationDateStr = _applicationUserVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = _applicationUserVM.ModifyDate.ToString("dd.MM.yyyy");
            }
            this.applicationUserVM = _applicationUserVM;
            this.validationMessages.Clear();
            this.editContext = new EditContext(this.applicationUserVM);

            if (this.applicationUserVM.Id != null)
            {
                await ApplicationUserService.GetAllApplicationRolePerUserAsync(applicationUserVM);
            }

            if (this.applicationUserVM.IndentTypeName != null && this.applicationUserVM.IdIndentType.HasValue)
            {
                this.identType = kvIndentTypeSource.FirstOrDefault(i => i.IdKeyValue == this.applicationUserVM.IdIndentType).Name;
            }
            else
            {
                this.identType = "ЕГН/ЛНЧ/ИДН";
            }

            this.isVisible = true;
            this.StateHasChanged();

        }
        private async void IdentValueChangedHandler()
        {
            var kvEGN = kvIndentTypeSource.FirstOrDefault(i => i.KeyValueIntCode == "EGN");
            var kvLNCh = kvIndentTypeSource.FirstOrDefault(i => i.KeyValueIntCode == "LNK");
            if (applicationUserVM.IdIndentType.HasValue)
            {
                if (applicationUserVM.IdIndentType == kvEGN.IdKeyValue)
                {
                    this.identType = "ЕГН";
                }
                else if (applicationUserVM.IdIndentType == kvLNCh.IdKeyValue)
                {
                    this.identType = "ЛНЧ";
                }
                else
                {
                    this.identType = "ИДН";
                }
            }
            else
            {
                this.identType = "ЕГН/ЛНЧ/ИДН";
            }
        }

        public async Task OpenRole()
        {
            bool hasPermission = await CheckUserActionPermission("ManageUserData", false);
            if (!hasPermission) { return; }

            roleSelectorModal.OpenModal(this.applicationUserVM);
        }

        public async Task RemoveRole()
        {
            bool hasPermission = await CheckUserActionPermission("ManageUserData", false);
            if (!hasPermission) { return; }

            var removeRoles = await this.refGrid.GetSelectedRecordsAsync();

            ResultContext<ApplicationUserVM> resultContext = new ResultContext<ApplicationUserVM>();

            resultContext.ResultContextObject = this.applicationUserVM;

            resultContext = await ApplicationUserService.RemoveUserRoles(resultContext, removeRoles);

            this.applicationUserVM = resultContext.ResultContextObject;

            //if (applicationRoleVM.Id != null)
            //{
            //    this.applicationRoleVM = await ApplicationUserService.GetApplicationRoleByIdAsync(this.applicationRoleVM);

            //    roleClaims = this.applicationRoleVM.RoleClaims;
            //    refGrid.Refresh();
            //}

            refGrid.Refresh();

            this.StateHasChanged();
        }

        public async Task ChangePassword()
        {
            if (loading) return;

            try
            {
                loading = true;
                this.SpinnerShow();
                await ApplicationUserService.SendPassword(applicationUserVM.Id);
                this.SpinnerHide();
                await this.ShowSuccessAsync("Паролата е нулирана успешно и е изпратен автоматичен e-mail с новата парола до потребителя!");

            }
            finally
            {
                loading = false;
            }
        }
        public async Task LoginLikeUser()
        {

            var user = await userManager.FindByIdAsync(applicationUserVM.Id);

            try
            {
                string key = Guid.NewGuid().ToString();
                BlazorCookieLoginMiddleware.Logins[key] = new LoginInfo { Email = user.UserName, Password = null, ApplicationUser = user };
                navMng.NavigateTo($"/login?key={key}", true);

                await this.localStorage.RemoveItemAsync("menu-id");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
