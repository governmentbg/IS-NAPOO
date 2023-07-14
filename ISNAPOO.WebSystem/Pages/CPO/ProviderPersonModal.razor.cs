using System.Text.RegularExpressions;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.CPO
{
    public partial class ProviderPersonModal : BlazorBaseComponent
    {
        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback<ResultContext<CandidateProviderPersonVM>> CallbackAfterSubmit { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IProviderService providerService { get; set; }
        [Inject]
        public IDataSourceService dataSourceService { get; set; }
        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }
        [Inject]
        public ISettingService settingService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }


        private DialogEffect AnimationEffect = DialogEffect.Zoom;
        private int ComboBoxValue;
        public string ModifyDateStr { get; set; }
        public string CreatedDateStr { get; set; }
        private string type;
        SfDialog sfDialog;
        ResultContext<CandidateProviderPersonVM> resultContext = new ResultContext<CandidateProviderPersonVM>();

        IEnumerable<KeyValueVM> kvIndentTypeSource;
        IEnumerable<KeyValueVM> keys;
        private ValidationMessageStore? messageStore;
        string header = "";
        private bool IsNew = false;
        List<string> validationMessages = new List<string>();
        CandidateProviderVM currentUserProvider = new CandidateProviderVM();
        private bool isUserAdministrator = false;


        protected override async Task OnInitializedAsync()
        {
            type = string.Empty;
            this.editContext = new EditContext(this.resultContext.ResultContextObject);
            resultContext.ResultContextObject.CandidateProvider = new CandidateProviderVM();
            resultContext.ResultContextObject.Person.FirstName = "";
            resultContext.ResultContextObject.Person.FamilyName = "";
            resultContext.ResultContextObject.Person.SecondName = "";
            this.editContext.EnableDataAnnotationsValidation();
            keys = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("UserStatus");
        }


        public async Task OpenModal(CandidateProviderPersonVM _model,string type)
        {
            this.type = type;
            this.kvIndentTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            if(_model.Status!=null)
                ComboBoxValue = _model.Status.IdKeyValue;
            else
                ComboBoxValue = (keys.Where(x => x.KeyValueIntCode == "Active").First()).IdKeyValue;

            if(_model.Person.IdIndentType == null)
            {
                _model.Person.IdIndentType = kvIndentTypeSource.Where(x => x.KeyValueIntCode.Equals("EGN")).First().IdKeyValue;
            }

            this.resultContext.ResultContextObject = _model;

            editContext = new EditContext(this.resultContext.ResultContextObject);
            this.validationMessages.Clear();


            this.resultContext.ResultContextObject.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.IdModifyUser);
            this.resultContext.ResultContextObject.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.IdCreateUser);
            if (_model.IdCandidate_Provider == 0)
            {
                ModifyDateStr = "";
                CreatedDateStr = "";
                this.resultContext.ResultContextObject.CreatePersonName = "";
                this.resultContext.ResultContextObject.ModifyPersonName = "";
            }
            else {
                ModifyDateStr = this.resultContext.ResultContextObject.ModifyDate.ToString("dd.MM.yyyy");
                CreatedDateStr = this.resultContext.ResultContextObject.CreationDate.ToString("dd.MM.yyyy");
            }

            await this.GetUserAdministratorState();

            this.isVisible = true;
          
            this.StateHasChanged();
        }

        private async Task GetUserAdministratorState()
        {
            this.isUserAdministrator = await this.IsPersonProfileAdministratorAsync();
        }

        private async Task Save()
        {
            bool hasPermission = await CheckUserActionPermission("ManageProviderPersonData", false);
            if (!hasPermission) { return; }
            this.SpinnerShow();
            this.editContext = new EditContext(this.resultContext.ResultContextObject);

            this.editContext.OnValidationRequested += this.CheckRequiredFildsAndValidateEGN;
            this.messageStore = new ValidationMessageStore(this.editContext);

            this.validationMessages.AddRange(editContext.GetValidationMessages());

            if (this.resultContext.ResultContextObject.IdCandidateProviderPerson != 0)
            {
                if (!this.resultContext.ResultContextObject.IsAdministrator)
                {
                    if (!(await this.CandidateProviderService.IsOnlyOneProfileAdministratorByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, this.resultContext.ResultContextObject.IdPerson)))
                    {
                        await this.ShowErrorAsync("Не можете да премахнете Администратор на профил, защото няма други потребители, които да са избрани за Администратор на профил!");
                        this.SpinnerHide();
                        return;
                    }
                }
            }

            try
            {
                bool isValid = this.editContext.Validate();

                if (isValid)
                {
                    resultContext.ResultContextObject.IdCandidate_Provider = this.UserProps.IdCandidateProvider;
                    resultContext.ResultContextObject.Status = this.keys.Where(x => x.IdKeyValue == ComboBoxValue).First();
                    var providers = (await providerService.GetAllCandidateProviderPersonsAsync(new CandidateProviderPersonVM() { IdCandidate_Provider = this.UserProps.IdCandidateProvider })).ToList();

                    var MaxActiveUsersSetting = await settingService.GetSettingByIntCodeAsync("MaxNumberOfActiveUsersForCPO");
                    var ActiveStatus = await dataSourceService.GetKeyValueByIntCodeAsync("UserStatus", "Active");

                    var count = providers.Where(x => x.Status != null && x.Status.IdKeyValue == ActiveStatus.IdKeyValue).Count();
                    var maxCount = Int32.Parse(MaxActiveUsersSetting.SettingValue);
                    var provider = providers.Where(x => x.IdCandidateProviderPerson == this.resultContext.ResultContextObject.IdCandidateProviderPerson).FirstOrDefault();

                    if (provider != null && provider.Status.IdKeyValue != this.resultContext.ResultContextObject.Status.IdKeyValue && this.resultContext.ResultContextObject.Status.IdKeyValue == ActiveStatus.IdKeyValue && maxCount <= count)
                    {
                        this.resultContext.AddErrorMessage($"Не можете да поддържате повече от {MaxActiveUsersSetting.SettingValue} активни профила в управление на потребителите!");
                    }
                    else
                    {
                        this.resultContext = await this.providerService.SaveCandidateProviderPersonAsync(this.resultContext);
                        ModifyDateStr = this.resultContext.ResultContextObject.Person.ModifyDate.ToString("dd.MM.yyyy");
                        CreatedDateStr = this.resultContext.ResultContextObject.Person.CreationDate.ToString("dd.MM.yyyy");
                        if (this.resultContext.ResultContextObject.CreatePersonName == null && this.resultContext.ResultContextObject.ModifyPersonName == null)
                        {
                            this.resultContext.ResultContextObject.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.Person.IdModifyUser);
                            this.resultContext.ResultContextObject.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.Person.IdCreateUser);
                        }
                        else
                        {
                            this.resultContext.ResultContextObject.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.resultContext.ResultContextObject.Person.IdModifyUser);
                        }

                        this.editContext = new EditContext(this.resultContext.ResultContextObject);

                        var user = await ApplicationUserService.GetAllApplicationUserAsync(new ApplicationUserVM() { IdPerson = resultContext.ResultContextObject.IdPerson });

                        this.resultContext.ResultContextObject.Person.IdApplicationUser = user.First().Id;
                    }
                    await this.CallbackAfterSubmit.InvokeAsync(resultContext);
                }
               
            }
            catch(Exception e)
            {

            }
            this.SpinnerHide();
        }

        private async void SendPassword()
        {
            if (loading) return;

            try
            {
                loading = true;
                this.SpinnerShow();
                await ApplicationUserService.SendPassword(this.resultContext.ResultContextObject.Person.IdApplicationUser);
                this.SpinnerHide();
                await this.ShowSuccessAsync("Успешно изпращане на парола.");
            }
            finally
            {
                loading = false;
            }
        }

        private void CheckRequiredFildsAndValidateEGN(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();
            var kvEGN = this.kvIndentTypeSource.FirstOrDefault(kv => kv.KeyValueIntCode == "EGN");
            if(this.resultContext.ResultContextObject.Person.Indent != null)
            this.resultContext.ResultContextObject.Person.Indent = this.resultContext.ResultContextObject.Person.Indent.Trim();
            Regex regexCyrillic = new Regex(@"^[\p{IsCyrillic}]+[\s-]*[\p{IsCyrillic}]+\s*$");
            if(this.resultContext.ResultContextObject.Person.SecondName != null && this.resultContext.ResultContextObject.Person.SecondName != "")
            {
                Match matchSecondName = regexCyrillic.Match(this.resultContext.ResultContextObject.Person.SecondName);
                if (!matchSecondName.Success)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "SecondName");
                    this.messageStore?.Add(fi, "Полето 'Презиме' може да съдържа само текст на български език!");
                }
            }
            if (string.IsNullOrEmpty(this.resultContext.ResultContextObject.Person.FirstName))
            {
                FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "FirstName");
                this.messageStore?.Add(fi, "Полето 'Име' е задължително!");
            }
            else
            {
                Match matchFirstName = regexCyrillic.Match(this.resultContext.ResultContextObject.Person.FirstName);
                if (!matchFirstName.Success)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "FirstName");
                    this.messageStore?.Add(fi, "Полето 'Име' може да съдържа само текст на български език!");
                }
            }
            if (string.IsNullOrEmpty(this.resultContext.ResultContextObject.Person.FamilyName))
            {
                FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "FamilyName");
                this.messageStore?.Add(fi, "Полето 'Фамилия' е задължително!");
            }
           else
            {
                Match matchFamilyName = regexCyrillic.Match(this.resultContext.ResultContextObject.Person.FamilyName);
                if (!matchFamilyName.Success)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "FamilyName");
                    this.messageStore?.Add(fi, "Полето 'Фамилия' може да съдържа само текст на български език!");
                }
            }
            if (this.resultContext.ResultContextObject.Person.IdIndentType == null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "IdIndentType");
                this.messageStore?.Add(fi, "Полето 'Вид на идентификатора' е задължително!");
            }
            if (string.IsNullOrEmpty(this.resultContext.ResultContextObject.Person.Indent))
            {
                FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "Indent");
                this.messageStore?.Add(fi, "Полето 'ЕГН/ЛНЧ/ИДН' е задължително!");
            }
            if (string.IsNullOrEmpty(this.resultContext.ResultContextObject.Person.Phone))
            {
                FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "Phone");
                this.messageStore?.Add(fi, "Полето 'Телефон' е задължително!");
            }

            if (string.IsNullOrEmpty(this.resultContext.ResultContextObject.Person.Position))
            {
                FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "Position");
                this.messageStore?.Add(fi, "Полето 'Длъжност' е задължително!");
            }

            if (string.IsNullOrEmpty(this.resultContext.ResultContextObject.Person.Email))
            {
                FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "Email");
                this.messageStore?.Add(fi, "Полето 'E-mail' е задължително!");
            }else
            {
                Regex regex = new Regex("(?:[a-z0-9!#$%&'*+\\=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+\\=?^_`{|}~-]+)*|\"\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\"\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])");
                Match match = regex.Match(this.resultContext.ResultContextObject.Person.Email);
                if (!match.Success)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "Email");
                    this.messageStore?.Add(fi, "Въведеният E-mail е невалиден!");
                }
            }
            //Ако е избрано ЕГН проверяваме дали е валидно
            if (kvEGN.IdKeyValue == this.resultContext.ResultContextObject.Person.IdIndentType)
            {
                if (this.resultContext.ResultContextObject.Person.Indent != null)
                {
                    this.resultContext.ResultContextObject.Person.Indent = this.resultContext.ResultContextObject.Person.Indent;

                    if (this.resultContext.ResultContextObject.Person.Indent.Length > 10)
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "Indent");
                        this.messageStore?.Add(fi, "Полето 'ЕГН/ЛНЧ/ИДН' трябва да съдържа 10 символа!");
                    }
                    else
                    {
                        var checkEGN = new BasicEGNValidation(this.resultContext.ResultContextObject.Person.Indent);

                        if (!checkEGN.Validate())
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "Indent");
                            this.messageStore?.Add(fi, checkEGN.ErrorMessage);
                        }
                    }
                }
            }
        }

        
    }
}
