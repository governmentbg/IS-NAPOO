using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.ExternalExpertCommission.Modals.Expert
{
    public partial class ExpertModal : BlazorBaseComponent
    {
        public override bool IsContextModified => this.editContext.IsModified();

        #region Inject Services
        [Inject]
        public Microsoft.JSInterop.IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IExpertService ExpertService { get; set; }
        [Inject]
        public IExpertProfessionalDirectionService ExpertProfessionalDirectionService { get; set; }
        [Inject]
        public IExpertDocumentService ExpertDocumentService { get; set; }
        [Inject]
        public IKeyValueService KeyValueService { get; set; }
        [Inject]
        public IDataSourceService DataSourceService { get; set; }
        [Inject]
        public ISettingService SettingService { get; set; }
        [Inject]
        public ILocationService LocationService { get; set; }
        [Inject]
        public IPersonService personService { get; set; }
        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }
        #endregion

        [Parameter]
        public bool IsRegister { get; set; } = false;

        [Parameter]
        public bool IsEditable { get; set; } = true;

        ToastMsg toast;
        public Query LocalDataQuery = new Query().Take(100);
        private DialogEffect AnimationEffect = DialogEffect.Zoom;
        private ValidationMessageStore? messageStore;

        SfDialog sfDialog = new SfDialog();

        private SfAutoComplete<int?, LocationVM> sfAutoCompleteLocationCorrespondence = new SfAutoComplete<int?, LocationVM>();
        private IEnumerable<LocationVM> locationSource = new List<LocationVM>();
        List<string> validationMessages = new List<string>();
        ResultContext<PersonVM> resultContextPerson;
        public bool Disabled { get; set; } = false;
        public bool IsMailDisable { get; set; } = true;
        public string Name { get; set; }


        private string dialogClass = "";
        private string CreationDateStr = "";
        private string ModifyDateStr = "";
        private string Nickname = "";
        private string identType = "";
        private string LicensingType = "";
        private bool isSubmitClicked = false;
        private bool IsNewModal = false;

        ExpertVM model = new ExpertVM();

        KeyValueVM kvIndentTypeFilterVM;
        KeyValueVM kvSexFilterVM;

        ApplicationUserVM appUser = new ApplicationUserVM();

        IEnumerable<KeyValueVM> kvIndentTypeSource;
        IEnumerable<KeyValueVM> kvSexSource;
        IEnumerable<PersonVM> people;

        public Query localDataQuery = new Query().Take(100);


        ExpertProfessionalDirectionsList expertProfessionalDirectionsList = new ExpertProfessionalDirectionsList();
        ExpertDocumentsList expertDocumentsList = new ExpertDocumentsList();
        ParticipationCommisionList participationInCommisionList = new ParticipationCommisionList();
        ExpertDOCList expertDOCList = new ExpertDOCList();
        ExpertNAPOOList expertNAPOOList = new ExpertNAPOOList();


        int selectedTab = 0;

        [Parameter]
        public EventCallback<ExpertVM> CallbackAfterSave { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.resultContextPerson = new ResultContext<PersonVM>();
            Name = " ";
            this.editContext = new EditContext(this.resultContextPerson.ResultContextObject);

        }

        private async void IsEGNValid()
        {
            string EGN = "";
            BasicEGNValidation validation;
            if (!string.IsNullOrEmpty(this.resultContextPerson.ResultContextObject.Indent))
            {
                this.resultContextPerson.ResultContextObject.Indent = this.resultContextPerson.ResultContextObject.Indent.Trim();
                EGN = this.resultContextPerson.ResultContextObject.Indent;
                validation = new BasicEGNValidation(EGN);
                if (validation.Validate())
                {
                    char charLastDigit = EGN[EGN.Length - 2];
                    int lastDigit = Convert.ToInt32(new string(charLastDigit, 1));
                    int year = int.Parse(EGN.Substring(0, 2));
                    int month = int.Parse(EGN.Substring(2, 2));
                    int day = int.Parse(EGN.Substring(4, 2));
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
                    var dt = new DateTime(year, month, day);
                    this.resultContextPerson.ResultContextObject.BirthDate = dt;
                    if (lastDigit % 2 == 0)
                    {
                        this.resultContextPerson.ResultContextObject.IdSex = DataSourceService.GetKeyValueByIntCodeAsync("Sex", "Man").Result.IdKeyValue;
                    }
                    else
                    {
                        this.resultContextPerson.ResultContextObject.IdSex = DataSourceService.GetKeyValueByIntCodeAsync("Sex", "Woman").Result.IdKeyValue;
                    }
                }
                else
                {
                    var indentKeyValue = await DataSourceService.GetKeyValueByIdAsync(this.resultContextPerson.ResultContextObject.IdIndentType);
                    if (indentKeyValue != null)
                    {
                        if (indentKeyValue.Name == "ЕГН")
                        {
                            ShowErrorAsync(validation.ErrorMessage);
                        }
                    }
                }
            }
        }
        private async void UpdateAfterChange()
        {
            await ExpertService.UpdateExpertAsync(this.resultContextPerson);
            await CallbackAfterSave.InvokeAsync(this.model);
        }

        public async Task OpenModal(ExpertVM _model, string tokenExpertType, string licensingType, bool isNew = false,
            bool isEditable = true)
        {
            this.kvIndentTypeFilterVM = new KeyValueVM();
            this.LicensingType = licensingType;
            this.IsNewModal = isNew;
            IsEditable = isEditable;
            this.kvIndentTypeSource = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            this.people = await personService.GetAllPersonsAsync();
            this.kvSexFilterVM = new KeyValueVM();
            this.kvSexSource = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");
            IsMailDisable = true;
            var users = await ApplicationUserService.GetAllApplicationUserAsync(this.appUser);
            if (users.Any(u => u.IdPerson == _model.IdPerson))
            {
                IsMailDisable = false;
            }
            this.resultContextPerson = new ResultContext<PersonVM>();
            this.validationMessages.Clear();
            if (_model.IdExpert == 0)
            {
                this.CreationDateStr = "";
                this.ModifyDateStr = "";
                _model.CreatePersonName = "";
                _model.ModifyPersonName = "";
                Disabled = true;
            }
            else
            {
                Disabled = false;
            }


            if (_model.IdPerson.HasValue && _model.IdPerson != 0)
            {
                this.model = _model;
                //this.model = await this.ExpertService.GetExpertByIdAsync(_model.IdExpert);
                //this.resultContext.ResultContextObject = model.Person;
                this.resultContextPerson.ResultContextObject = await personService.GetPersonByIdAsync(_model.IdPerson.Value);
                this.CreationDateStr = _model.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = _model.ModifyDate.ToString("dd.MM.yyyy");
                this.model.ModifyPersonName = await ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                this.model.CreatePersonName = await ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
            }
            else
            {
                this.resultContextPerson.ResultContextObject = new PersonVM();
                this.model = _model;
            }
            if (this.resultContextPerson.ResultContextObject.IdIndentType != null)
            {
                this.identType = this.kvIndentTypeSource.FirstOrDefault(i => i.IdKeyValue == this.resultContextPerson.ResultContextObject.IdIndentType).Name;
            }
            else
            {
                this.identType = "ЕГН/ЛНЧ/ИДН";
            }

            this.resultContextPerson.ResultContextObject.TokenExpertType = tokenExpertType;


            List<LocationVM> locationVMs = new List<LocationVM>();

            if (this.resultContextPerson.ResultContextObject.IdLocation != null)
            {
                LocationVM locationAdmin = await LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.resultContextPerson.ResultContextObject.IdLocation ?? default);
                locationVMs.Add(locationAdmin);
            }

            if (locationVMs.Any())
            {
                this.locationSource = locationVMs;
            }

            this.expertProfessionalDirectionsList = new ExpertProfessionalDirectionsList();
            this.expertDocumentsList = new ExpertDocumentsList();

            this.selectedTab = 0;
            Name = this.resultContextPerson.ResultContextObject.FullName;
            if (Name == " ")
            {
                Name = "нов експерт";
            }
            this.resultContextPerson.ResultContextObject.IdExpert = _model.IdExpert;
            this.editContext = new EditContext(this.resultContextPerson.ResultContextObject);
            this.isVisible = true;
            StateHasChanged();
        }

        private async Task SelectedEventHandler()
        {
            SpinnerShow();
            if (!this.IsNewModal)
            {
                if (this.selectedTab == 1)
                {
                    IEnumerable<ExpertNapooVM> expertsNAPOO = await ExpertService.GetAllExpertsNAPOOAsync(this.model.IdExpert);
                    await this.expertNAPOOList.OpenList(expertsNAPOO, this.model.IdExpert);
                }
                else if (this.selectedTab == 2)
                {
                    IEnumerable<ExpertProfessionalDirectionVM> expertProfessionalDirectionsawait = await ExpertProfessionalDirectionService.GetExpertProfessionalDirectionsByExpertIdAsync(this.model.IdExpert);
                    await this.expertProfessionalDirectionsList.OpenList(expertProfessionalDirectionsawait, this.model.IdExpert);
                }
                else if (this.selectedTab == 3)
                {
                    await this.participationInCommisionList.OpenList(this.model.IdExpert);
                }
                else if (this.selectedTab == 4)
                {
                    IEnumerable<ExpertDOCVM> expertDOCs = await ExpertService.GetAllExpertExpertDOCsAsync(this.model.IdExpert);
                    await this.expertDOCList.OpenList(expertDOCs, this.model.IdExpert);

                }
                else if (this.selectedTab == 5)
                {
                    IEnumerable<ExpertDocumentVM> expertDocuments = await ExpertDocumentService.GetExpertDocumentsByExpertIdAsync(this.model.IdExpert);
                    await this.expertDocumentsList.OpenList(expertDocuments, this.model.IdExpert);
                }
            }
            else
            {
                if (this.selectedTab == 1)
                {
                    if (this.IsNewModal && this.LicensingType == "NapooEmployees")
                    {
                        IEnumerable<ExpertNapooVM> expertsNAPOO = await ExpertService.GetAllExpertsNAPOOAsync(this.model.IdExpert);
                        await this.expertNAPOOList.OpenList(expertsNAPOO, this.model.IdExpert);
                    }
                    else if (this.IsNewModal && this.LicensingType == "ExternalExperts")
                    {
                        IEnumerable<ExpertProfessionalDirectionVM> expertProfessionalDirectionsawait = await ExpertProfessionalDirectionService.GetExpertProfessionalDirectionsByExpertIdAsync(this.model.IdExpert);
                        await this.expertProfessionalDirectionsList.OpenList(expertProfessionalDirectionsawait, this.model.IdExpert);
                    }
                    else if (this.IsNewModal && this.LicensingType == "ExpertCommissions")
                    {

                        await this.participationInCommisionList.OpenList(this.model.IdExpert);
                    }
                    else if (this.IsNewModal && this.LicensingType == "DocWorkGroup")
                    {
                        IEnumerable<ExpertDOCVM> expertDOCs = await ExpertService.GetAllExpertExpertDOCsAsync(this.model.IdExpert);
                        await this.expertDOCList.OpenList(expertDOCs, this.model.IdExpert);
                    }
                }
                else if (this.selectedTab == 2)
                {
                    IEnumerable<ExpertDocumentVM> expertDocuments = await ExpertDocumentService.GetExpertDocumentsByExpertIdAsync(this.model.IdExpert);
                    await this.expertDocumentsList.OpenList(expertDocuments, this.model.IdExpert);
                }

            }
            SpinnerHide();
        }

        private void Select(SelectingEventArgs args)
        {
            if (args.IsSwiped)
            {
                args.Cancel = true;
            }
        }


        private async Task Save()
        {
            bool hasPermission = await CheckUserActionPermission("ManageExpertsData", false);
            if (!hasPermission) { return; }
            SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.resultContextPerson.ResultContextObject);

                var kvEGN = await DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");


                if (this.resultContextPerson.ResultContextObject.IdIndentType == null
                    && !string.IsNullOrEmpty(this.resultContextPerson.ResultContextObject.Indent))
                {
                    await ShowErrorAsync("Моля, изберете Вид на идентификатора");
                    return;
                }

                //Ако е избрано ЕГН проверяваме дали е валидно
                if (this.resultContextPerson.ResultContextObject.IdIndentType.HasValue && kvEGN.IdKeyValue == this.resultContextPerson.ResultContextObject.IdIndentType)
                {
                    this.editContext.OnValidationRequested += ValidateEGN;
                    if (!string.IsNullOrEmpty(this.resultContextPerson.ResultContextObject.Indent))
                    {
                        this.editContext.OnValidationRequested += CheckForOtherExpertWithSameIndent;
                        //this.messageStore = new ValidationMessageStore(this.editContext);
                    }
                    this.messageStore = new ValidationMessageStore(this.editContext);
                }


                this.editContext.EnableDataAnnotationsValidation();


                this.isSubmitClicked = true;

                this.validationMessages.Clear();
                bool isValid = this.editContext.Validate();
                this.validationMessages.AddRange(this.editContext.GetValidationMessages());
                if (isValid)
                {
                    this.resultContextPerson.ListErrorMessages.Clear();
                    this.resultContextPerson.ListMessages.Clear();
                    this.resultContextPerson = await ExpertService.UpdateExpertAsync(this.resultContextPerson);
                    this.model = await ExpertService.GetExpertByIdAsync(this.resultContextPerson.ResultContextObject.IdExpert);
                    if (this.resultContextPerson.HasMessages)
                    {
                        await ShowSuccessAsync(string.Join(Environment.NewLine, this.resultContextPerson.ListMessages));
                        this.resultContextPerson.ListMessages.Clear();
                    }
                    else
                    {
                        await ShowErrorAsync(string.Join(Environment.NewLine, this.resultContextPerson.ListErrorMessages));
                        this.resultContextPerson.ListErrorMessages.Clear();
                    }
                    Disabled = false;
                    if (IsMailDisable)
                    {
                        IsMailDisable = false;
                    }
                    this.CreationDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                    this.ModifyDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                    this.model.ModifyPersonName = await ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                    this.model.CreatePersonName = await ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
                    Name = this.resultContextPerson.ResultContextObject.FullName;
                }
                StateHasChanged();
                await CallbackAfterSave.InvokeAsync(this.model);
                this.sfDialog.RefreshPositionAsync();
                this.isSubmitClicked = false;
            }
            finally
            {
                SpinnerHide();
                this.loading = false;
            }
        }


        private async void FilteringKeyValueIndentType(FilteringEventArgs args)
        {
            //Използваме кода да вземем от базата филтрираните редове
            this.kvIndentTypeFilterVM.Name = args.Text;
            this.kvIndentTypeSource = new List<KeyValueVM>();
            this.kvIndentTypeSource = await KeyValueService.GetAllAsync(this.kvIndentTypeFilterVM);
            StateHasChanged();
        }

        private async void FilteringKeyValueSex(FilteringEventArgs args)
        {
            //Използваме кода да вземем от базата филтрираните редове
            this.kvSexFilterVM.Name = args.Text;
            this.kvSexSource = new List<KeyValueVM>();
            this.kvSexSource = await KeyValueService.GetAllAsync(this.kvSexFilterVM);
            StateHasChanged();
        }

        private void AutoCompleteLocationCorrespondenceValueChanged(ChangeEventArgs<int?, LocationVM> args)
        {
            if (args.Value == null)
            {
                this.resultContextPerson.ResultContextObject.IdLocation = null;
            }
            if (args.Value != null)
            {
                this.resultContextPerson.ResultContextObject.PostCode = args.ItemData.PostCode.ToString();
            }
            else
            {
                this.resultContextPerson.ResultContextObject.PostCode = null;
            }
        }

        private async Task OnFilterLocationCorrespondence(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    this.locationSource = await LocationService.GetAllLocationsByKatiAsync(args.Text);
                }
                catch (Exception) { }

                var query = new Query().Where(new WhereFilter() { Field = "kati", Operator = "contains", value = args.Text, IgnoreCase = true });

                query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                await this.sfAutoCompleteLocationCorrespondence.FilterAsync(this.locationSource, query);
            }
        }
        public async Task SendMail()
        {
            bool hasPermission = await CheckUserActionPermission("ManageExpertsData", false);
            if (!hasPermission) { return; }
           
            try
            {
                string msg = "Сигурни ли сте, че искате да генерирате парола и да я изпратите на електронната поща на експерта?";
                bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
                if (isConfirmed)
                {
                    SpinnerShow();

                    if (string.IsNullOrEmpty(this.resultContextPerson.ResultContextObject.Email))
                    {
                        throw new Exception();
                    }
                    var users = await ApplicationUserService.GetAllApplicationUserAsync(this.appUser);
                    var userId = users.FirstOrDefault(u => u.IdPerson == this.resultContextPerson.ResultContextObject.IdPerson).Id;
                    var userUserName = users.FirstOrDefault(u => u.IdPerson == this.resultContextPerson.ResultContextObject.IdPerson).UserName;
                    var user = await ExpertService.UpdatePersonEmailSentDateAsync(new ResultContext<PersonVM>() { ResultContextObject = new PersonVM() { IdPerson = this.resultContextPerson.ResultContextObject.IdPerson } });
                    await ApplicationUserService.SendPassword(userId);
                    await CallbackAfterSave.InvokeAsync(this.model);
                    SpinnerHide();
                    await ShowSuccessAsync("Информация за потребителското име и паролата е изпратена успешно на електронната поща на експерта!");

                }
            }
            catch (Exception)
            {
                await ShowErrorAsync("Неуспешно изпратен имейл.");
            }
        }
        private void ValidateEGN(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.resultContextPerson.ResultContextObject.Indent != null)
            {
                this.resultContextPerson.ResultContextObject.Indent = this.resultContextPerson.ResultContextObject.Indent;

                if (this.resultContextPerson.ResultContextObject.Indent.Length > 10)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.resultContextPerson.ResultContextObject, "Indent");
                    this.messageStore?.Add(fi, "Полето 'ЕГН/ЛНЧ/ИДН' трябва да съдържа 10 символа!");
                }
                else
                {
                    var checkEGN = new BasicEGNValidation(this.resultContextPerson.ResultContextObject.Indent);

                    if (!checkEGN.Validate())
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.resultContextPerson.ResultContextObject, "Indent");
                        this.messageStore?.Add(fi, checkEGN.ErrorMessage);
                    }
                }
            }
            else
            {
                FieldIdentifier fi = new FieldIdentifier(this.resultContextPerson.ResultContextObject, "Indent");
                this.messageStore?.Add(fi, $"Полето '{this.identType}' е задължително!");
            }
        }
        private void CheckForOtherExpertWithSameIndent(object? sender, ValidationRequestedEventArgs args)
        {

            var indentType = this.kvIndentTypeSource.FirstOrDefault(x => x.IdKeyValue == this.resultContextPerson.ResultContextObject.IdIndentType);
            if (this.people.Any(x => x.Indent == this.resultContextPerson.ResultContextObject.Indent && x.IdPerson != this.resultContextPerson.ResultContextObject.IdPerson))
            {
                FieldIdentifier fi = new FieldIdentifier(this.resultContextPerson.ResultContextObject, "Indent");
                this.messageStore.Add(fi, $"Експерт с това {indentType.Name} вече е въведен в информационната система!!");
            }

        }
        private async void IdentValueChangedHandler(ChangeEventArgs<int?, KeyValueVM> args)
        {
            var kvEGN = await DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
            var kvLNCh = await DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "LNK");
            if (args.Value.HasValue)
            {
                if (args.Value == kvEGN.IdKeyValue)
                {
                    this.identType = "ЕГН";
                }
                else if (args.Value == kvLNCh.IdKeyValue)
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

    }
}
