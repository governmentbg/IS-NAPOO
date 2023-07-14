using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Candidate;
using Syncfusion.Blazor.Popups;
using ISNAPOO.WebSystem.Pages.Common;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Syncfusion.Blazor.Data;
using Syncfusion.DocIORenderer;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIO;
using Microsoft.JSInterop;
using ISNAPOO.Core.HelperClasses;
using DocuServiceReference;
using DocuWorkService;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Pages.Candidate;
using ISNAPOO.WebSystem.Pages.Candidate.CIPO;
using Syncfusion.Blazor.Grids;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.Common.Constants;

namespace ISNAPOO.WebSystem.Pages.Control
{
    partial class FollowUpControlInformation : BlazorBaseComponent
    {
        [Parameter]

        public bool IsCPO { get; set; }

        [Parameter]

        public bool IsEditable { get; set; } = true;

        [Parameter]

        public EventCallback<FollowUpControlVM> CallBackAfterCreation { get; set; }

        [Parameter]

        public EventCallback<string> CallBackChangeHeader { get; set; }

        [Parameter]

        public FollowUpControlVM Model { get; set; }

        #region Inject
        [Inject]
        public IDataSourceService DataSourceService { get; set; }
        [Inject]
        public IControlService ControlService { get; set; }

        [Inject]
        public IExpertService ExpertService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDocuService DocuService { get; set; }

        [Inject]
        public ITemplateDocumentService TemplateDocumentService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }
        #endregion

        private ToastMsg toast;
        private int idExpert = 0;
        private SfGrid<FollowUpControlExpertVM> sfGrid = new SfGrid<FollowUpControlExpertVM>();
        private ValidationMessageStore? messageStore;
        private SfAutoComplete<int?, CandidateProviderVM> sfAutoComplete = new SfAutoComplete<int?, CandidateProviderVM>();
        private string CPOorCIPO = string.Empty;
        private string CPOorCIPONameAndOwner = string.Empty;
        private bool IsDateValid = true;
        private bool isBlankGenerate = false;
        private bool isProtocolGenerate = false;
        private bool isLetterGenerate = false;
        private bool isReportGenerate = false;
        private FollowUpControlExpertVM toBeDelete = new FollowUpControlExpertVM();
        private ApplicationModal applicationModal = new ApplicationModal();
        private CIPOApplicationModal cipoApplicationModal = new CIPOApplicationModal();

        private IEnumerable<KeyValueVM> kvFollowUpControlTypeSource;
        private IEnumerable<KeyValueVM> kvControlTypeSource;
        private IEnumerable<KeyValueVM> kvControlStatusesSource;
        private IEnumerable<KeyValueVM> kvDeadlineTypes;
        private IEnumerable<KeyValueVM> kvTypeFrameworkProgram = new List<KeyValueVM>();
        private IEnumerable<FollowUpControlExpertVM> followUpControlExperts;
        private List<ExpertVM> experts = new List<ExpertVM>();
        private IEnumerable<CandidateProviderVM> candidateProviders = new List<CandidateProviderVM>();

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.Model);

            this.kvFollowUpControlTypeSource = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FollowUpControlType");
            this.kvControlTypeSource = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ControlType");
            this.kvControlStatusesSource = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FollowUpControlStatus");
            this.kvDeadlineTypes = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ControlDeadlinePeriodType");
            this.kvTypeFrameworkProgram = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).ToList().Where(k => k.KeyValueIntCode == "ProfessionalQualification" || k.KeyValueIntCode == "PartProfession").ToList();
            this.FormTitle = "Данни за последващ контрол";


            if (IsCPO)
            {
                //this.candidateProviders = await CandidateProviderService.GetAllActiveCandidateProvidersAsync("LicensingCPO", "All");

                this.candidateProviders = await this.CandidateProviderService.GetAllCandidateProvidersForAutoComplete("LicensingCPO");
            }
            else
            {
                //this.candidateProviders = await CandidateProviderService.GetAllActiveCandidateProvidersAsync("LicensingCIPO", "All");
                this.candidateProviders = await this.CandidateProviderService.GetAllCandidateProvidersForAutoComplete("LicensingCIPO");
            }

            if (IsCPO)
            {
                CPOorCIPO = "ЦПО";
            }
            else
            {
                CPOorCIPO = "ЦИПО";
            }
            this.LoadData();
        }
        public async Task LoadData()
        {
            this.SpinnerShow();
            this.idExpert = 0;
            this.experts = (await ExpertService.GetAllExpertsAsync(new ExpertVM() { IsNapooExpert = true })).ToList();



            foreach (var item in this.experts)
            {
                var napooExperts = (await ExpertService.GetAllExpertsNAPOOAsync(item.IdExpert)).ToList();
                if (napooExperts.Count > 0)
                {
                    item.FullNameAndOccupation = item.Person.FullName + " - " + string.Join(", ", napooExperts.Select(x => x.Occupation).ToList());
                }
                else
                {
                    item.FullNameAndOccupation = item.Person.FullName;
                }
            }
            if (this.Model.IdFollowUpControl != 0)
            {
                this.followUpControlExperts = await this.ControlService.GetAllControlExpertsByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                isBlankGenerate = (await this.ControlService.GetAllDocumentsAsync(this.Model.IdFollowUpControl)).Any(d => d.DocumentTypeName == "Заповед за осъществяване на последващ контрол");
                isProtocolGenerate = (await this.ControlService.GetAllDocumentsAsync(this.Model.IdFollowUpControl)).Any(d => d.DocumentTypeName == "Констативен протокол от извършен последващ контрол");
                isLetterGenerate = (await this.ControlService.GetAllDocumentsAsync(this.Model.IdFollowUpControl)).Any(d => d.DocumentTypeName == "Уведомително писмо, съпровождащо констативния протокол");
                isReportGenerate = (await this.ControlService.GetAllDocumentsAsync(this.Model.IdFollowUpControl)).Any(d => d.DocumentTypeName == "Доклад на длъжностното лице за отстранените нередовности");
                if (IsCPO)
                {
                    this.CPOorCIPONameAndOwner = this.Model.IdFollowUpControl != 0 ? this.Model.CandidateProvider.CPONameOwnerGrid : string.Empty;
                }
                else
                {
                    this.CPOorCIPONameAndOwner = this.Model.IdFollowUpControl != 0 ? this.Model.CandidateProvider.CIPONameOwnerGrid : string.Empty;
                }
                await this.CallBackChangeHeader.InvokeAsync(this.CPOorCIPONameAndOwner);
            }
            else
            {
                this.followUpControlExperts = new List<FollowUpControlExpertVM>();
                this.Model = new FollowUpControlVM();
                isBlankGenerate = false;
                isProtocolGenerate = false;
                isLetterGenerate = false;
                isReportGenerate = false;
            }
            this.StateHasChanged();
            this.editContext.MarkAsUnmodified();
            this.SpinnerHide();
        }
        private async Task ValueChangeHandler(ChangeEventArgs<int?, CandidateProviderVM> args)
        {
            // Here, you can customize your code.

            if (IsCPO)
            {
                await this.CallBackChangeHeader.InvokeAsync(args.ItemData.CPONameOwnerGrid);
            }
            else
            {
                await this.CallBackChangeHeader.InvokeAsync(args.ItemData.CIPONameOwnerGrid);
            }
        }

        private async Task OpenProfileBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.applicationModal.OpenModal(new CandidateProviderVM() { IdCandidate_Provider = this.Model.IdCandidateProvider!.Value }, false, false);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public async Task SubmitHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageFollowUpControlData", false);
            if (!hasPermission) { return; }
            this.SpinnerShow();
            this.editContext = new EditContext(this.Model);


            this.editContext.OnValidationRequested += this.ValidateCandidateProvider;
            this.messageStore = new ValidationMessageStore(this.editContext);

            if (this.Model.IsFollowUpControlOnsite)
            {
                this.editContext.OnValidationRequested += this.ValidateOnsiteControlDate;
                this.messageStore = new ValidationMessageStore(this.editContext);
            }
            else if (!this.Model.IsFollowUpControlOnsite && this.Model.OnsiteControlDateFrom.HasValue)
            {
                this.Model.OnsiteControlDateFrom = null;
            }

            // Дата на проверк до
            if (!this.Model.IsFollowUpControlOnsite && this.Model.OnsiteControlDateTo.HasValue)
            {
                this.Model.OnsiteControlDateTo = null;
            }

            this.editContext.EnableDataAnnotationsValidation();

            var result = 0;

            bool isValid = this.editContext.Validate();

            if (IsDateValid)
            {

                if (isValid)
                {
                    ResultContext<FollowUpControlVM> resultContext = new ResultContext<FollowUpControlVM>();
                    resultContext.ResultContextObject = Model;
                    resultContext = await this.ControlService.UpdateFollowUpControlAsync(resultContext);
                    if (resultContext.HasMessages)
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                        resultContext.ListMessages.Clear();
                    }
                    else
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                        resultContext.ListErrorMessages.Clear();
                    }
                    this.Model.IdFollowUpControl = resultContext.ResultContextObject.IdFollowUpControl;
                    await this.CallBackAfterCreation.InvokeAsync(this.Model);
                }

            }
            else
            {
                toast.sfErrorToast.Content = "Въведената дата в полето 'Срок за проверката от' не може да е след 'Срок за проверката до'!";
                await toast.sfErrorToast.ShowAsync();
            }

            if (IsCPO)
            {
                this.CPOorCIPONameAndOwner = this.Model.IdFollowUpControl != 0 ? this.Model.CandidateProvider.CPONameOwnerGrid : string.Empty;
            }
            else
            {
                this.CPOorCIPONameAndOwner = this.Model.IdFollowUpControl != 0 ? this.Model.CandidateProvider.CIPONameOwnerGrid : string.Empty;
            }
            this.SpinnerHide();
            this.StateHasChanged();
        }
        private void ValidateOnsiteControlDate(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (!this.Model.OnsiteControlDateFrom.HasValue)
            {
                FieldIdentifier fi = new FieldIdentifier(this.Model, "OnsiteControlDateFrom");
                this.messageStore?.Add(fi, $"Полето 'Дата на проверка от' е задължително!");
            }

            if (!this.Model.OnsiteControlDateTo.HasValue)
            {
                FieldIdentifier fi = new FieldIdentifier(this.Model, "OnsiteControlDateTo");
                this.messageStore?.Add(fi, $"Полето 'Дата на проверка до' е задължително!");
            }
        }

        private void ValidateCandidateProvider(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (!this.Model.IdCandidateProvider.HasValue)
            {
                FieldIdentifier fi = new FieldIdentifier(this.Model, "IdCandidateProvider");
                this.messageStore?.Add(fi, $"Полето '{CPOorCIPO}' е задължително!");
            }
        }

        private async Task AddNewControlExpert()
        {
            this.SpinnerShow();
            if (this.idExpert != 0)
            {
                if (!followUpControlExperts.Any(c => c.IdExpert == this.idExpert))
                {
                    FollowUpControlExpertVM controlExpert = new FollowUpControlExpertVM()
                    {
                        IdFollowUpControl = this.Model.IdFollowUpControl,
                        IdExpert = this.idExpert
                    };
                    var resultContext = await ControlService.AddControlExpertAsync(controlExpert);
                    if (resultContext.HasMessages)
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                        resultContext.ListMessages.Clear();
                    }
                    else
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                        resultContext.ListErrorMessages.Clear();
                    }

                    this.followUpControlExperts = await this.ControlService.GetAllControlExpertsByIdFollowUpControlAsync(Model.IdFollowUpControl);
                    this.idExpert = 0;
                }
                else
                {
                    toast.sfErrorToast.Content = "Това длъжностно лице вече е добавено!";
                    await toast.sfErrorToast.ShowAsync();
                }
            }
            else
            {
                toast.sfErrorToast.Content = "Трябва да попълните поленцето 'Длъжностно лице'!";
                await toast.sfErrorToast.ShowAsync();
            }

            this.SpinnerHide();
        }

        private async Task OnFilter(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    if (CPOorCIPO == "ЦПО")
                    {
                        this.candidateProviders = await this.CandidateProviderService.GetAllCandidateProvidersForAutoComplete("LicensingCPO");

                        this.candidateProviders = this.candidateProviders.Where(
                                x => x.CPONameOwnerGrid.ToLower().Contains(args.Text.ToLower()) ||
                                    (x.LicenceNumber != null ? x.LicenceNumber.Contains(args.Text) : false)).ToList();
                    }
                    else
                    {
                        this.candidateProviders = await this.CandidateProviderService.GetAllCandidateProvidersForAutoComplete("LicensingCIPO");
                        this.candidateProviders = this.candidateProviders.Where(
                                x => x.CIPONameOwnerGrid.ToLower().Contains(args.Text.ToLower()) ||
                                    (x.LicenceNumber != null ? x.LicenceNumber.Contains(args.Text) : false)).ToList();
                    }

                    var query = new Query().Where(new WhereFilter() { Field = "ProviderOwner", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.sfAutoComplete.FilterAsync(this.candidateProviders, query);
                }
                catch (Exception ex) { }
            }
        }
        private void DateValid()
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime(); ;
            if (this.Model.ControlStartDate.HasValue)
            {
                startDate = this.Model.ControlStartDate.Value.Date;
            }
            if (this.Model.ControlEndDate.HasValue)
            {
                endDate = this.Model.ControlEndDate.Value.Date;

            }
            int result = DateTime.Compare(startDate, endDate);

            if (result > 0 && this.Model.ControlEndDate.HasValue && this.Model.ControlStartDate.HasValue)
            {
                this.IsDateValid = false;
            }
            else
            {
                this.IsDateValid = true;
            }
        }
        public async void ConfirmDeleteCallback()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;


                var resultContext = new ResultContext<FollowUpControlExpertVM>();
                resultContext.ResultContextObject = toBeDelete;

                resultContext = await this.ControlService.DeleteControlExpertAsync(resultContext);

                if (resultContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                    await toast.sfSuccessToast.ShowAsync();
                    resultContext.ListMessages.Clear();

                    this.followUpControlExperts = await this.ControlService.GetAllControlExpertsByIdFollowUpControlAsync(Model.IdFollowUpControl);
                }
                else
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    resultContext.ListErrorMessages.Clear();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }
        private async Task DeleteControlExpert(FollowUpControlExpertVM controlExpert)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            toBeDelete = controlExpert;
            this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
        }

        private async Task GenerateBlank()
        {
            if (this.Model.IsFollowUpControlOnsite)
            {
                if (!this.Model.OnsiteControlDateFrom.HasValue || !this.Model.OnsiteControlDateTo.HasValue)
                {
                    this.toast.sfErrorToast.Content = "Не може да се разпечата бланка преди да е въведен период от/до";
                    await this.toast.sfErrorToast.ShowAsync();
                    return;
                }
            }
            //Get resource document
            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
            string documentName = string.Empty;
            string onlineStr = string.Empty;
            string currentApplication = string.Empty;
            if (CPOorCIPO == "ЦПО")
            {
                documentName = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CPO_ApplicationPK1")).Result.IdKeyValue).TemplatePath;
                currentApplication = "CPO_ApplicationPK1";
                if (this.Model.IsFollowUpControlOnsite)
                {
                    var locationName = this.Model.CandidateProvider.LocationCorrespondence != null ? this.Model.CandidateProvider.LocationCorrespondence.LocationName : string.Empty;
                    if (this.Model.OnsiteControlDateFrom.Value == this.Model.OnsiteControlDateTo.Value)
                    {
                        onlineStr = $" На място, в офиса на {this.Model.CandidateProvider.CPONameOwnerGrid}, {locationName}, {this.Model.CandidateProvider.ProviderAddressCorrespondence}, на дата {this.Model.OnsiteControlDateFrom.Value.ToString("dd.MM.yyyy")} г.";
                    }
                    else
                    {
                        onlineStr = $" На място, в офиса на {this.Model.CandidateProvider.CPONameOwnerGrid}, {locationName}, {this.Model.CandidateProvider.ProviderAddressCorrespondence}, в период от {this.Model.OnsiteControlDateFrom.Value.ToString("dd.MM.yyyy")} до {this.Model.OnsiteControlDateTo.Value.ToString("dd.MM.yyyy")} г.";
                    }
                }
            }
            else
            {
                documentName = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", "CIPO_ApplicationPK1")).Result.IdKeyValue).TemplatePath;
                currentApplication = "CIPO_ApplicationPK1";
                if (this.Model.IsFollowUpControlOnsite)
                {
                    if (this.Model.OnsiteControlDateFrom.Value == this.Model.OnsiteControlDateTo.Value)
                    {
                        onlineStr = $" На място, на дата {this.Model.OnsiteControlDateFrom.Value.ToString("dd.MM.yyyy")} г. в офиса на {this.Model.CandidateProvider.CIPONameOwnerGrid} на адрес: {this.Model.CandidateProvider.ProviderAddressCorrespondence} .";
                    }
                    else
                    {
                        onlineStr = $" На място, в период от {this.Model.OnsiteControlDateFrom.Value.ToString("dd.MM.yyyy")} до {this.Model.OnsiteControlDateTo.Value.ToString("dd.MM.yyyy")} г. в офиса на {this.Model.CandidateProvider.CIPONameOwnerGrid} на адрес: {this.Model.CandidateProvider.ProviderAddressCorrespondence} .";
                    }

                }
            }
            FileStream template = new FileStream($@"{resources_Folder}{documentName}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);
            DocIORenderer render = new DocIORenderer();
            var currentCPOorCIPO = this.IsCPO ? this.CPOorCIPONameAndOwner.Replace("ЦПО", "") : this.CPOorCIPONameAndOwner.Replace("ЦИПО", "");
            //Merge fields
            string[] fieldNames = new string[]
            {
                "FollowUpControlTypeName",
                "NameOwnerGrid",
                "LocationCorrespondence",
                "LicenceNumber",
                "ControlTypeName",
                "ControlStartDate",
                "ControlEndDate",
                "DateFrom",
                "DateTo"
            };
            string[] fieldValues = new string[]
            {
                 this.Model.FollowUpControlTypeName.ToLower(),
                 currentCPOorCIPO,
                 this.Model.CandidateProvider.LocationCorrespondence != null ? this.Model.CandidateProvider.LocationCorrespondence.LocationName : string.Empty,
                 this.Model.CandidateProvider.LicenceNumber,
                 this.Model.ControlTypeName.ToLower(),
                 this.Model.ControlStartDate.Value.ToString("dd.MM.yyyy"),
                 this.Model.ControlEndDate.Value.ToString("dd.MM.yyyy"),
                 this.Model.PeriodFrom.HasValue ? this.Model.PeriodFrom.Value.ToString("dd.MM.yyyy") : string.Empty,
                 this.Model.PeriodTo.HasValue ? this.Model.PeriodTo.Value.ToString("dd.MM.yyyy") : string.Empty
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            //Navigate to first bookmar with list of trainers
            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("FollowUpControlExperts", true, false);

            #region Paragraphs

            #region TrainersParagraph
            //Create new paragraph
            IWParagraphStyle paragraphStyle = document.AddParagraphStyle("ControlExpertsParagraph");
            paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
            #endregion

            #endregion

            ListStyle trainersListStyle = document.AddListStyle(ListType.Numbered, "ControlExpertsList");

            WListLevel trainersListLevel = trainersListStyle.Levels[0];
            trainersListLevel.CharacterFormat.FontSize = (float)10;

            #region CharacterFormat

            //CharacterFormat
            WCharacterFormat expertsCharacterFormat = new WCharacterFormat(document);
            expertsCharacterFormat.FontName = "Times New Roman";
            expertsCharacterFormat.FontSize = 12;

            #endregion

            #region DrawListOfControlExperts
            int row = 0;
            foreach (var controlExpert in followUpControlExperts)
            {
                row++;
                IWParagraph membersParagraph = new WParagraph(document);

                bookNav.InsertParagraph(membersParagraph);

                var napooExperts = (await ExpertService.GetAllExpertsNAPOOAsync(controlExpert.IdExpert)).ToList();
                if (napooExperts != null && napooExperts.Count > 0)
                {
                    membersParagraph.AppendText(row.ToString() + ". " + controlExpert.Expert.Person.FullName + ", " + string.Join(", ", napooExperts.Select(x => x.Occupation))).ApplyCharacterFormat(expertsCharacterFormat);
                }
                else
                {
                    membersParagraph.AppendText(row.ToString() + ". " + controlExpert.Expert.Person.FullName).ApplyCharacterFormat(expertsCharacterFormat);
                }
                membersParagraph.ApplyStyle("ControlExpertsParagraph");
            }
            #endregion

            //Navigate to bookmark with information for every trainer and create new paragraph 
            bookNav.MoveToBookmark("FollowUpControlExpertOnlineOrOnsite", true, false);

            trainersListStyle = document.AddListStyle(ListType.Numbered, "Trainers");

            trainersListLevel = trainersListStyle.Levels[0];
            trainersListLevel.FollowCharacter = FollowCharacterType.Space;
            trainersListLevel.CharacterFormat.FontSize = (float)9.5;
            trainersListLevel.TextPosition = 1;

            if (this.Model.IsFollowUpControlOnline && this.Model.IsFollowUpControlOnsite)
            {
                IWParagraph membersParagraph = new WParagraph(document);

                bookNav.InsertParagraph(membersParagraph);
                membersParagraph.AppendCheckBox("OnSite", true).ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.AppendText(" По документи в ИС на НАПОО и въз основа на допълнително изискани документи \n").ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.AppendCheckBox("Online", true).ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.AppendText(onlineStr).ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.ApplyStyle("ControlExpertsParagraph");
            }
            else if (this.Model.IsFollowUpControlOnline && !this.Model.IsFollowUpControlOnsite)
            {
                IWParagraph membersParagraph = new WParagraph(document);

                bookNav.InsertParagraph(membersParagraph);

                membersParagraph.AppendCheckBox("Online", true).ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.AppendText(" По документи в ИС на НАПОО и въз основа на допълнително изискани документи").ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.ApplyStyle("ControlExpertsParagraph");
            }
            else if (!this.Model.IsFollowUpControlOnline && this.Model.IsFollowUpControlOnsite)
            {
                IWParagraph membersParagraph = new WParagraph(document);

                bookNav.InsertParagraph(membersParagraph);

                membersParagraph.AppendCheckBox("OnSite", true).ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.AppendText(onlineStr).ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.ApplyStyle("ControlExpertsParagraph");
            }

            FollowUpControlDocumentVM followUpControlDocumentVM = new FollowUpControlDocumentVM();

            var kvDocumentType = await DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", currentApplication);
            var indexUser = await DataSourceService.GetSettingByIntCodeAsync("IndexUserId");

            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, FormatType.Docx);

                FileData[] files = new FileData[]
                {
                                new FileData()
                                {
                                    BinaryContent = stream.ToArray(),
                                    Filename = "Prilojenie1_zapovedPK.docx"
                                }
                };

                RegisterDocumentParams registerDocumentParams = new RegisterDocumentParams()
                {
                    ExternalCode = kvDocumentType.DefaultValue2,
                    RegisterUser = int.Parse(indexUser.SettingValue),
                    RegisterUserSpecified = true
                };

                CorrespData corresp = new CorrespData()
                {
                    Names = this.CPOorCIPONameAndOwner,
                    EIK = this.Model.CandidateProvider.PoviderBulstat,
                    Phone = this.Model.CandidateProvider.ProviderPhone,
                    Email = this.Model.CandidateProvider.ProviderEmail
                };

                DocData docs = new DocData()
                {
                    Otnosno = "Заповед за осъществяване на последващ контрол",
                    Corresp = corresp,
                    File = files,

                };
                var registerResult = await this.DocuService.RegisterDocumentAsync(registerDocumentParams, docs);

                if (registerResult.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, registerResult.ListErrorMessages));
                    return;
                }
                var documentResponse = registerResult.ResultContextObject;

                followUpControlDocumentVM.IdFollowUpControl = this.Model.IdFollowUpControl;
                followUpControlDocumentVM.IdDocumentType = kvDocumentType.IdKeyValue;
                followUpControlDocumentVM.DS_ID = documentResponse.Doc.DocID;
                followUpControlDocumentVM.DS_GUID = documentResponse.Doc.GUID;
                followUpControlDocumentVM.DS_DATE = documentResponse.Doc.DocDate;
                followUpControlDocumentVM.DS_DocNumber = documentResponse.Doc.DocNumber;

                await this.ControlService.SaveControlDocument(followUpControlDocumentVM);

                await this.JsRuntime.SaveAs("Prilojenie1_zapovedPK" + ".docx", stream.ToArray());
                isBlankGenerate = (await this.ControlService.GetAllDocumentsAsync(this.Model.IdFollowUpControl)).Any(d => d.DocumentTypeName == "Заповед за осъществяване на последващ контрол");
            }
        }
        private async Task GenerateProtocol()
        {
            FollowUpControlDocumentVM followUpControlDocumentVM = new FollowUpControlDocumentVM();
            //Get resource document

            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
            string documentName = string.Empty;
            string documentType = string.Empty;
            string lastDocumentType = string.Empty;
            string onlineStr = string.Empty;

            if (CPOorCIPO == "ЦПО")
            {
                documentType = "CPO_ApplicationPK2";
                lastDocumentType = "CPO_ApplicationPK1";
                if (this.Model.IsFollowUpControlOnsite)
                {
                    var locationName = this.Model.CandidateProvider.LocationCorrespondence != null ? this.Model.CandidateProvider.LocationCorrespondence.LocationName : string.Empty;
                    if (this.Model.OnsiteControlDateFrom.Value == this.Model.OnsiteControlDateTo.Value)
                    {
                        onlineStr = $" На място, в офиса на {this.Model.CandidateProvider.CPONameOwnerGrid}, {locationName}, {this.Model.CandidateProvider.ProviderAddressCorrespondence}, на дата {this.Model.OnsiteControlDateFrom.Value.ToString("dd.MM.yyyy")} г.";
                    }
                    else
                    {
                        onlineStr = $" На място, в офиса на {this.Model.CandidateProvider.CPONameOwnerGrid}, {locationName}, {this.Model.CandidateProvider.ProviderAddressCorrespondence}, в период от {this.Model.OnsiteControlDateFrom.Value.ToString("dd.MM.yyyy")} до {this.Model.OnsiteControlDateTo.Value.ToString("dd.MM.yyyy")} г.";
                    }

                }
            }
            else
            {
                documentType = "CIPO_ApplicationPK2";
                lastDocumentType = "CIPO_ApplicationPK1";
                if (this.Model.IsFollowUpControlOnsite)
                {
                    if (this.Model.OnsiteControlDateFrom.Value == this.Model.OnsiteControlDateTo.Value)
                    {
                        onlineStr = $" На място, на дата {this.Model.OnsiteControlDateFrom.Value.ToString("dd.MM.yyyy")} г. в офиса на {this.Model.CandidateProvider.CIPONameOwnerGrid} на адрес: {this.Model.CandidateProvider.ProviderAddressCorrespondence} .";
                    }
                    else
                    {
                        onlineStr = $" На място, в период от {this.Model.OnsiteControlDateFrom.Value.ToString("dd.MM.yyyy")} до {this.Model.OnsiteControlDateTo.Value.ToString("dd.MM.yyyy")} г. в офиса на {this.Model.CandidateProvider.CIPONameOwnerGrid} на адрес: {this.Model.CandidateProvider.ProviderAddressCorrespondence} .";
                    }

                }
            }

            documentName = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", documentType)).Result.IdKeyValue).TemplatePath;
            FileStream template = new FileStream($@"{resources_Folder}{documentName}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);
            DocIORenderer render = new DocIORenderer();
            var kvDocumentType = await DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", documentType);
            var kvDocumentType1 = await DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", lastDocumentType);
            var indexUser = await DataSourceService.GetSettingByIntCodeAsync("IndexUserId");
            var documentBlank = (await this.ControlService.GetAllDocumentsAsync(this.Model.IdFollowUpControl)).FirstOrDefault(d => d.IdDocumentType == kvDocumentType1.IdKeyValue);
            var currentCPOorCIPO = this.IsCPO ? this.CPOorCIPONameAndOwner.Replace("ЦПО", "") : this.CPOorCIPONameAndOwner.Replace("ЦИПО", "");

            //Merge fields
            string[] fieldNames = new string[]
            {
                "Official_number",
                "Official_date",
                "FollowUpControlTypeName",
                "NameOwnerGrid",
                "LocationCorrespondence",
                "LicenceNumber",
                "ControlTypeName",
                "ControlStartDate",
                "ControlEndDate",
                "DateFrom",
                "DateTo",
                "DeadlinePeriod"
            };
            string[] fieldValues = new string[]
            {
                 documentBlank.DS_OFFICIAL_DocNumber,
                 documentBlank.DS_OFFICIAL_DATE.HasValue ? documentBlank.DS_OFFICIAL_DATE.Value.ToString("dd.MM.yyyy") : string.Empty,
                 this.Model.FollowUpControlTypeName.ToLower(),
                 currentCPOorCIPO,
                 this.Model.CandidateProvider.LocationCorrespondence != null ? this.Model.CandidateProvider.LocationCorrespondence.LocationName : string.Empty,
                 this.Model.CandidateProvider.LicenceNumber,
                 this.Model.ControlTypeName.ToLower(),
                 this.Model.ControlStartDate.Value.ToString("dd.MM.yyyy"),
                 this.Model.ControlEndDate.Value.ToString("dd.MM.yyyy"),
                 this.Model.PeriodFrom.HasValue ? this.Model.PeriodFrom.Value.ToString("dd.MM.yyyy") : string.Empty,
                 this.Model.PeriodTo.HasValue ? this.Model.PeriodTo.Value.ToString("dd.MM.yyyy") : string.Empty,
                 this.Model.IdDeadlinePeriodType.HasValue ?
                 kvDeadlineTypes.FirstOrDefault(x => x.IdKeyValue == this.Model.IdDeadlinePeriodType.Value).Name == "1 месец" ? "1 (един) месец" :
                 kvDeadlineTypes.FirstOrDefault(x => x.IdKeyValue == this.Model.IdDeadlinePeriodType.Value).Name == "2 месеца" ? "2 (два) месеца" :
                 kvDeadlineTypes.FirstOrDefault(x => x.IdKeyValue == this.Model.IdDeadlinePeriodType.Value).Name == "3 месеца" ? "3 (три) месеца" : string.Empty
                 : string.Empty
            };

            document.MailMerge.Execute(fieldNames, fieldValues);

            BookmarksNavigator bookNav = new BookmarksNavigator(document);
            bookNav.MoveToBookmark("FollowUpControlExpertOnlineOrOnsite", true, false);

            #region Paragraphs

            #region TrainersParagraph
            //Create new paragraph
            IWParagraphStyle paragraphStyle = document.AddParagraphStyle("ControlExpertsParagraph");
            paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
            #endregion

            #endregion

            ListStyle trainersListStyle = document.AddListStyle(ListType.Numbered, "ControlExpertsList");

            WListLevel trainersListLevel = trainersListStyle.Levels[0];
            trainersListLevel.CharacterFormat.FontSize = (float)10;

            #region CharacterFormat

            //CharacterFormat
            WCharacterFormat expertsCharacterFormat = new WCharacterFormat(document);
            expertsCharacterFormat.FontName = "Times New Roman";
            expertsCharacterFormat.FontSize = 12;

            #endregion

            trainersListLevel = trainersListStyle.Levels[0];
            trainersListLevel.FollowCharacter = FollowCharacterType.Space;
            trainersListLevel.CharacterFormat.FontSize = (float)9.5;
            trainersListLevel.TextPosition = 1;

            if (this.Model.IsFollowUpControlOnline && this.Model.IsFollowUpControlOnsite)
            {
                IWParagraph membersParagraph = new WParagraph(document);

                bookNav.InsertParagraph(membersParagraph);
                membersParagraph.AppendCheckBox("OnSite", true).ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.AppendText(" По документи в ИС на НАПОО и въз основа на допълнително изискани документи \n").ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.AppendCheckBox("Online", true).ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.AppendText(onlineStr).ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.ApplyStyle("ControlExpertsParagraph");
            }
            else if (this.Model.IsFollowUpControlOnline && !this.Model.IsFollowUpControlOnsite)
            {
                IWParagraph membersParagraph = new WParagraph(document);

                bookNav.InsertParagraph(membersParagraph);

                membersParagraph.AppendCheckBox("Online", true).ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.AppendText(" По документи в ИС на НАПОО и въз основа на допълнително изискани документи").ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.ApplyStyle("ControlExpertsParagraph");
            }
            else if (!this.Model.IsFollowUpControlOnline && this.Model.IsFollowUpControlOnsite)
            {
                IWParagraph membersParagraph = new WParagraph(document);

                bookNav.InsertParagraph(membersParagraph);

                membersParagraph.AppendCheckBox("OnSite", true).ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.AppendText(onlineStr).ApplyCharacterFormat(expertsCharacterFormat);
                membersParagraph.ApplyStyle("ControlExpertsParagraph");
            }


            #region Dynamic tables filling
            WSection section = document.Sections[0];
            var kvCourseStatusFinished = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");
            var kvCourseStatusCurrent = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusNow");
            var courses = await this.TrainingService.GetAllActiveCourseCheckingsByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
            var spkFinishedCourseSource = courses
                .Where(x => x.Course.IdTrainingCourseType == kvTypeFrameworkProgram.First(x => x.KeyValueIntCode == "ProfessionalQualification").IdKeyValue && x.Course.IdStatus == kvCourseStatusFinished.IdKeyValue)
                .ToList();
            var ppFinishedCourseSource = courses
                .Where(x => x.Course.IdTrainingCourseType == kvTypeFrameworkProgram.First(x => x.KeyValueIntCode == "PartProfession").IdKeyValue && x.Course.IdStatus == kvCourseStatusFinished.IdKeyValue)
                .ToList();
            var validationClientsSource = (await this.TrainingService.GetAllActiveValidationClientCheckingsByIdFollowUpControlAsync(this.Model.IdFollowUpControl)).ToList();
            var currentCourseSource = courses
                .Where(x => x.Course.IdStatus == kvCourseStatusCurrent.IdKeyValue)
                .ToList();
            var currentCourseSourceBetweenDates = currentCourseSource
                .Where(x => x.Course.IdStatus == kvCourseStatusCurrent.IdKeyValue && x.Course.ExamPracticeDate.HasValue && x.Course.ExamTheoryDate.HasValue && x.FollowUpControl.PeriodFrom.HasValue && x.FollowUpControl.PeriodTo.HasValue && x.Course.ExamPracticeDate.Value.Date >= x.FollowUpControl.PeriodFrom.Value.Date && x.Course.ExamPracticeDate.Value.Date <= x.FollowUpControl.PeriodTo.Value.Date && x.Course.ExamTheoryDate.Value.Date >= x.FollowUpControl.PeriodFrom.Value.Date && x.Course.ExamTheoryDate.Value.Date <= x.FollowUpControl.PeriodTo.Value.Date)
                .ToList();
            var vqsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
            var trainingTypesSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
            var formEducationsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation");
            var kvTrainingTypeTheory = trainingTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "TheoryTraining");
            var kvTrainingTypePractice = trainingTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "PracticalTraining");
            var kvTrainingAndPracticeTypeTheory = trainingTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "TrainingInTheoryAndPractice");

            // добавя данни в таблица 8.1
            var list81Tables = new List<WTable>();
            foreach (var item in spkFinishedCourseSource)
            {
                WTable table = section.Tables[0] as WTable;
                var coursePremises = await this.TrainingService.GetAllPremisesCoursesByIdCourseAsync(item.IdCourse!.Value);
                var courseTrainers = await this.TrainingService.GetAllTrainerCoursesWithoutIncludesByIdCourseAsync(item.IdCourse!.Value);
                var trainers = await this.TrainingService.GetAllTrainerCoursesByIdCourseAsync(item.IdCourse.Value);
                var trainingTypes = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
                var vqs = vqsSource.FirstOrDefault(x => x.IdKeyValue == item.Course.Program.Speciality.IdVQS);
                var profession = this.DataSourceService.GetAllProfessionsList().FirstOrDefault(x => x.IdProfession == item.Course.Program.Speciality.IdProfession);
                var kvFormEducationTypeValue = formEducationsSource.FirstOrDefault(x => x.IdKeyValue == item.Course.IdFormEducation);
                var theoryPremises = coursePremises.Where(x => x.IdТraininType == kvTrainingTypeTheory!.IdKeyValue || x.IdТraininType == kvTrainingAndPracticeTypeTheory!.IdKeyValue).Select(x => x.CandidateProviderPremises.PremisesName);
                var practicePremises = coursePremises.Where(x => x.IdТraininType == kvTrainingTypePractice!.IdKeyValue || x.IdТraininType == kvTrainingAndPracticeTypeTheory!.IdKeyValue).Select(x => x.CandidateProviderPremises.PremisesName);
                var theoryTrainers = courseTrainers.Where(x => x.IdТraininType == kvTrainingTypeTheory!.IdKeyValue || x.IdТraininType == kvTrainingAndPracticeTypeTheory!.IdKeyValue).Select(x => x.CandidateProviderTrainer.FullName);
                var practiceTrainers = courseTrainers.Where(x => x.IdТraininType == kvTrainingTypePractice!.IdKeyValue || x.IdТraininType == kvTrainingAndPracticeTypeTheory!.IdKeyValue).Select(x => x.CandidateProviderTrainer.FullName);
                var examPracticeDate = item.Course.ExamPracticeDate.HasValue ? $"{item.Course.ExamPracticeDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : "-";
                var examTheoryDate = item.Course.ExamTheoryDate.HasValue ? $"{item.Course.ExamTheoryDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : "-";

                table.Rows[0].Cells[1].Paragraphs[0].Text = vqs?.Name;
                table.Rows[1].Cells[1].Paragraphs[0].Text = $"{profession?.Name}, {profession?.Code}";
                table.Rows[2].Cells[1].Paragraphs[0].Text = $"{item.Course.Program.Speciality?.Name}, {item.Course.Program.Speciality?.Code}";
                table.Rows[3].Cells[1].Paragraphs[0].Text = $"{item.Course.Program.FrameworkProgram.Name}";
                table.Rows[4].Cells[1].Paragraphs[0].Text = $"{item.Course.CandidateProviderPremises.Location.LocationName}";

                if (theoryPremises is not null && theoryPremises.Any())
                {
                    table.Rows[6].Cells[1].Paragraphs[0].Text = string.Join(", ", theoryPremises);
                }
                else
                {
                    table.Rows[6].Cells[1].Paragraphs[0].Text = "-";
                }

                if (practicePremises is not null && practicePremises.Any())
                {
                    table.Rows[7].Cells[1].Paragraphs[0].Text = string.Join(", ", practicePremises);
                }
                else
                {
                    table.Rows[7].Cells[1].Paragraphs[0].Text = "-";
                }

                if (theoryTrainers is not null && theoryTrainers.Any())
                {
                    table.Rows[9].Cells[1].Paragraphs[0].Text = string.Join(", ", theoryTrainers);
                }
                else
                {
                    table.Rows[9].Cells[1].Paragraphs[0].Text = "-";
                }

                if (practiceTrainers is not null && practiceTrainers.Any())
                {
                    table.Rows[10].Cells[1].Paragraphs[0].Text = string.Join(", ", practiceTrainers);
                }
                else
                {
                    table.Rows[10].Cells[1].Paragraphs[0].Text = "-";
                }

                table.Rows[11].Cells[1].Paragraphs[0].Text = $"{kvFormEducationTypeValue?.Name}";
                table.Rows[12].Cells[1].Paragraphs[0].Text = $"{item.Course.SelectableHours!.Value + item.Course.MandatoryHours!.Value}";
                table.Rows[13].Cells[1].Paragraphs[0].Text = $"{item.Course.StartDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
                table.Rows[14].Cells[1].Paragraphs[0].Text = $"{item.Course.EndDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
                table.Rows[15].Cells[1].Paragraphs[0].Text = examTheoryDate;
                table.Rows[16].Cells[1].Paragraphs[0].Text = examPracticeDate;

                list81Tables.Add(table);
            }

            // добавя данни в таблица 8.2
            var list82Tables = new List<WTable>();
            foreach (var item in ppFinishedCourseSource)
            {
                WTable table = section.Tables[1] as WTable;
                var coursePremises = await this.TrainingService.GetAllPremisesCoursesByIdCourseAsync(item.IdCourse!.Value);
                var courseTrainers = await this.TrainingService.GetAllTrainerCoursesWithoutIncludesByIdCourseAsync(item.IdCourse!.Value);
                var trainers = await this.TrainingService.GetAllTrainerCoursesByIdCourseAsync(item.IdCourse.Value);
                var trainingTypes = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
                var vqs = vqsSource.FirstOrDefault(x => x.IdKeyValue == item.Course.Program.Speciality.IdVQS);
                var profession = this.DataSourceService.GetAllProfessionsList().FirstOrDefault(x => x.IdProfession == item.Course.Program.Speciality.IdProfession);
                var kvFormEducationTypeValue = formEducationsSource.FirstOrDefault(x => x.IdKeyValue == item.Course.IdFormEducation);
                var theoryPremises = coursePremises.Where(x => x.IdТraininType == kvTrainingTypeTheory!.IdKeyValue || x.IdТraininType == kvTrainingAndPracticeTypeTheory!.IdKeyValue).Select(x => x.CandidateProviderPremises.PremisesName);
                var practicePremises = coursePremises.Where(x => x.IdТraininType == kvTrainingTypePractice!.IdKeyValue || x.IdТraininType == kvTrainingAndPracticeTypeTheory!.IdKeyValue).Select(x => x.CandidateProviderPremises.PremisesName);
                var theoryTrainers = courseTrainers.Where(x => x.IdТraininType == kvTrainingTypeTheory!.IdKeyValue || x.IdТraininType == kvTrainingAndPracticeTypeTheory!.IdKeyValue).Select(x => x.CandidateProviderTrainer.FullName);
                var practiceTrainers = courseTrainers.Where(x => x.IdТraininType == kvTrainingTypePractice!.IdKeyValue || x.IdТraininType == kvTrainingAndPracticeTypeTheory!.IdKeyValue).Select(x => x.CandidateProviderTrainer.FullName);
                var examPracticeDate = item.Course.ExamPracticeDate.HasValue ? $"{item.Course.ExamPracticeDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : "-";
                var examTheoryDate = item.Course.ExamTheoryDate.HasValue ? $"{item.Course.ExamTheoryDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : "-";

                table.Rows[0].Cells[1].Paragraphs[0].Text = vqs?.Name;
                table.Rows[1].Cells[1].Paragraphs[0].Text = $"{profession?.Name}, {profession?.Code}";
                table.Rows[2].Cells[1].Paragraphs[0].Text = $"{item.Course.Program.Speciality?.Name}, {item.Course.Program.Speciality?.Code}";
                table.Rows[3].Cells[1].Paragraphs[0].Text = $"{item.Course.Program.FrameworkProgram.Name}";
                table.Rows[4].Cells[1].Paragraphs[0].Text = $"{item.Course.CandidateProviderPremises.Location.LocationName}";

                if (theoryPremises is not null && theoryPremises.Any())
                {
                    table.Rows[6].Cells[1].Paragraphs[0].Text = string.Join(", ", theoryPremises);
                }
                else
                {
                    table.Rows[6].Cells[1].Paragraphs[0].Text = "-";
                }

                if (practicePremises is not null && practicePremises.Any())
                {
                    table.Rows[7].Cells[1].Paragraphs[0].Text = string.Join(", ", practicePremises);
                }
                else
                {
                    table.Rows[7].Cells[1].Paragraphs[0].Text = "-";
                }

                if (theoryTrainers is not null && theoryTrainers.Any())
                {
                    table.Rows[9].Cells[1].Paragraphs[0].Text = string.Join(", ", theoryTrainers);
                }
                else
                {
                    table.Rows[9].Cells[1].Paragraphs[0].Text = "-";
                }

                if (practiceTrainers is not null && practiceTrainers.Any())
                {
                    table.Rows[10].Cells[1].Paragraphs[0].Text = string.Join(", ", practiceTrainers);
                }
                else
                {
                    table.Rows[10].Cells[1].Paragraphs[0].Text = "-";
                }

                table.Rows[11].Cells[1].Paragraphs[0].Text = $"{kvFormEducationTypeValue?.Name}";
                table.Rows[12].Cells[1].Paragraphs[0].Text = $"{item.Course.SelectableHours!.Value + item.Course.MandatoryHours!.Value}";
                table.Rows[13].Cells[1].Paragraphs[0].Text = $"{item.Course.StartDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
                table.Rows[14].Cells[1].Paragraphs[0].Text = $"{item.Course.EndDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
                table.Rows[15].Cells[1].Paragraphs[0].Text = examTheoryDate;
                table.Rows[16].Cells[1].Paragraphs[0].Text = examPracticeDate;

                list82Tables.Add(table);
            }

            // добавя данни в таблица 9
            var list9Tables = new List<WTable>();
            foreach (var item in validationClientsSource)
            {
                WTable table = section.Tables[2] as WTable;
                var vqs = vqsSource.FirstOrDefault(x => x.IdKeyValue == item.ValidationClient.Speciality.IdVQS);
                var profession = this.DataSourceService.GetAllProfessionsList().FirstOrDefault(x => x.IdProfession == item.ValidationClient.Speciality.IdProfession);
                var examPracticeDate = item.ValidationClient.ExamPracticeDate.HasValue ? $"{item.ValidationClient.ExamPracticeDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : "-";
                var examTheoryDate = item.ValidationClient.ExamTheoryDate.HasValue ? $"{item.ValidationClient.ExamTheoryDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : "-";

                table.Rows[0].Cells[1].Paragraphs[0].Text = $"{profession?.Name}, {profession?.Code}";
                table.Rows[1].Cells[1].Paragraphs[0].Text = $"{item.ValidationClient.Speciality?.Name}, {item.ValidationClient.Speciality?.Code}";
                table.Rows[2].Cells[1].Paragraphs[0].Text = vqs?.Name;
                table.Rows[3].Cells[1].Paragraphs[0].Text = $"{item.ValidationClient.FrameworkProgram.Name}";
                table.Rows[4].Cells[1].Paragraphs[0].Text = $"{item.ValidationClient.CandidateProvider.Location.LocationName}";
                table.Rows[5].Cells[1].Paragraphs[0].Text = $"{item.ValidationClient.StartDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
                table.Rows[6].Cells[1].Paragraphs[0].Text = $"{item.ValidationClient.EndDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
                table.Rows[7].Cells[1].Paragraphs[0].Text = examTheoryDate;
                table.Rows[8].Cells[1].Paragraphs[0].Text = examPracticeDate;

                list9Tables.Add(table);
            }

            // добавя данни в таблица 10
            var list10Tables = new List<WTable>();
            foreach (var item in currentCourseSource)
            {
                WTable table = section.Tables[3] as WTable;
                var coursePremises = await this.TrainingService.GetAllPremisesCoursesByIdCourseAsync(item.IdCourse!.Value);
                var courseTrainers = await this.TrainingService.GetAllTrainerCoursesWithoutIncludesByIdCourseAsync(item.IdCourse!.Value);
                var trainers = await this.TrainingService.GetAllTrainerCoursesByIdCourseAsync(item.IdCourse.Value);
                var trainingTypes = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
                var vqs = vqsSource.FirstOrDefault(x => x.IdKeyValue == item.Course.Program.Speciality.IdVQS);
                var profession = this.DataSourceService.GetAllProfessionsList().FirstOrDefault(x => x.IdProfession == item.Course.Program.Speciality.IdProfession);
                var kvFormEducationTypeValue = formEducationsSource.FirstOrDefault(x => x.IdKeyValue == item.Course.IdFormEducation);
                var theoryPremises = coursePremises.Where(x => x.IdТraininType == kvTrainingTypeTheory!.IdKeyValue || x.IdТraininType == kvTrainingAndPracticeTypeTheory!.IdKeyValue).Select(x => x.CandidateProviderPremises.PremisesName);
                var practicePremises = coursePremises.Where(x => x.IdТraininType == kvTrainingTypePractice!.IdKeyValue || x.IdТraininType == kvTrainingAndPracticeTypeTheory!.IdKeyValue).Select(x => x.CandidateProviderPremises.PremisesName);
                var theoryTrainers = courseTrainers.Where(x => x.IdТraininType == kvTrainingTypeTheory!.IdKeyValue || x.IdТraininType == kvTrainingAndPracticeTypeTheory!.IdKeyValue).Select(x => x.CandidateProviderTrainer.FullName);
                var practiceTrainers = courseTrainers.Where(x => x.IdТraininType == kvTrainingTypePractice!.IdKeyValue || x.IdТraininType == kvTrainingAndPracticeTypeTheory!.IdKeyValue).Select(x => x.CandidateProviderTrainer.FullName);
                var examPracticeDate = item.Course.ExamPracticeDate.HasValue ? $"{item.Course.ExamPracticeDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : "-";
                var examTheoryDate = item.Course.ExamTheoryDate.HasValue ? $"{item.Course.ExamTheoryDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : "-";

                table.Rows[0].Cells[1].Paragraphs[0].Text = $"{profession?.Name}, {profession?.Code}";
                table.Rows[1].Cells[1].Paragraphs[0].Text = $"{item.Course.Program.Speciality?.Name}, {item.Course.Program.Speciality?.Code}";
                table.Rows[2].Cells[1].Paragraphs[0].Text = vqs?.Name;
                table.Rows[3].Cells[1].Paragraphs[0].Text = $"{item.Course.Program.FrameworkProgram.Name}";
                table.Rows[4].Cells[1].Paragraphs[0].Text = $"{item.Course.CandidateProviderPremises.Location.LocationName}";

                if (theoryPremises is not null && theoryPremises.Any())
                {
                    table.Rows[6].Cells[1].Paragraphs[0].Text = string.Join(", ", theoryPremises);
                }
                else
                {
                    table.Rows[6].Cells[1].Paragraphs[0].Text = "-";
                }

                if (practicePremises is not null && practicePremises.Any())
                {
                    table.Rows[7].Cells[1].Paragraphs[0].Text = string.Join(", ", practicePremises);
                }
                else
                {
                    table.Rows[7].Cells[1].Paragraphs[0].Text = "-";
                }

                if (theoryTrainers is not null && theoryTrainers.Any())
                {
                    table.Rows[9].Cells[1].Paragraphs[0].Text = string.Join(", ", theoryTrainers);
                }
                else
                {
                    table.Rows[9].Cells[1].Paragraphs[0].Text = "-";
                }

                if (practiceTrainers is not null && practiceTrainers.Any())
                {
                    table.Rows[10].Cells[1].Paragraphs[0].Text = string.Join(", ", practiceTrainers);
                }
                else
                {
                    table.Rows[10].Cells[1].Paragraphs[0].Text = "-";
                }

                table.Rows[11].Cells[1].Paragraphs[0].Text = $"{kvFormEducationTypeValue?.Name}";
                table.Rows[12].Cells[1].Paragraphs[0].Text = $"{item.Course.SelectableHours!.Value + item.Course.MandatoryHours!.Value}";
                table.Rows[13].Cells[1].Paragraphs[0].Text = $"{item.Course.StartDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
                table.Rows[14].Cells[1].Paragraphs[0].Text = $"{item.Course.EndDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
                table.Rows[15].Cells[1].Paragraphs[0].Text = item.Course.ClientCourses.Count.ToString();

                list10Tables.Add(table);
            }

            // добавя данни в таблица 11
            var list11Tables = new List<WTable>();
            foreach (var item in currentCourseSourceBetweenDates)
            {
                WTable table = section.Tables[4] as WTable;
                var vqs = vqsSource.FirstOrDefault(x => x.IdKeyValue == item.Course.Program.Speciality.IdVQS);
                var profession = this.DataSourceService.GetAllProfessionsList().FirstOrDefault(x => x.IdProfession == item.Course.Program.Speciality.IdProfession);
                var kvFormEducationTypeValue = formEducationsSource.FirstOrDefault(x => x.IdKeyValue == item.Course.IdFormEducation);
                var examPracticeDate = item.Course.ExamPracticeDate.HasValue ? $"{item.Course.ExamPracticeDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : "-";
                var examTheoryDate = item.Course.ExamTheoryDate.HasValue ? $"{item.Course.ExamTheoryDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : "-";

                table.Rows[0].Cells[1].Paragraphs[0].Text = $"{profession?.Name}, {profession?.Code}";
                table.Rows[1].Cells[1].Paragraphs[0].Text = $"{item.Course.Program.Speciality?.Name}, {item.Course.Program.Speciality?.Code}";
                table.Rows[2].Cells[1].Paragraphs[0].Text = vqs?.Name;
                table.Rows[3].Cells[1].Paragraphs[0].Text = $"{item.Course.Program.FrameworkProgram.Name}";
                table.Rows[4].Cells[1].Paragraphs[0].Text = $"{item.Course.CandidateProviderPremises.Location.LocationName}";

                table.Rows[13].Cells[1].Paragraphs[0].Text = $"{kvFormEducationTypeValue?.Name}";
                table.Rows[14].Cells[1].Paragraphs[0].Text = $"{item.Course.SelectableHours!.Value + item.Course.MandatoryHours!.Value}";
                table.Rows[15].Cells[1].Paragraphs[0].Text = $"{item.Course.StartDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
                table.Rows[16].Cells[1].Paragraphs[0].Text = $"{item.Course.EndDate!.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
                table.Rows[17].Cells[1].Paragraphs[0].Text = $"{examTheoryDate}/{examPracticeDate}";

                list11Tables.Add(table);
            }

            // добавя таблиците за раздел 8.1 към документа
            if (list81Tables.Any())
            {
                bookNav.MoveToBookmark("Bookmark81", true, false);
                list81Tables.ForEach(x =>
                {
                    bookNav.InsertTable(x);
                });
            }

            // добавя таблиците за раздел 8.2 към документа
            if (list82Tables.Any())
            {
                bookNav.MoveToBookmark("Bookmark82", true, false);
                list82Tables.ForEach(x =>
                {
                    bookNav.InsertTable(x);
                });
            }

            // добавя таблиците за раздел 9 към документа
            if (list9Tables.Any())
            {
                bookNav.MoveToBookmark("Bookmark9", true, false);
                list9Tables.ForEach(x =>
                {
                    bookNav.InsertTable(x);
                });
            }

            // добавя таблиците за раздел 10 към документа
            if (list10Tables.Any())
            {
                bookNav.MoveToBookmark("Bookmark10", true, false);
                list10Tables.ForEach(x =>
                {
                    bookNav.InsertTable(x);
                });
            }

            // добавя таблиците за раздел 11 към документа
            if (list11Tables.Any())
            {
                bookNav.MoveToBookmark("Bookmark11", true, false);
                list11Tables.ForEach(x =>
                {
                    bookNav.InsertTable(x);
                });
            }

            // премахва първите 5 таблици в документа
            WTextBody body = section.Body;
            for (int index = 0; index < 5; index++)
            {
                if (body.ChildEntities[index] is WTable)
                {
                    body.ChildEntities.RemoveAt(index);
                    index--;
                }
            }

            // премахва празните парагрифи в началото на документа
            for (int index = 0; index < 2; index++)
            {
                if (body.ChildEntities[index] is WParagraph)
                {
                    body.ChildEntities.RemoveAt(index);
                }
            }

            #endregion

            bookNav.MoveToBookmark("FollowUpControlExperts", true, false);

            #region DrawListOfControlExperts
            int row = 0;
            foreach (var controlExpert in followUpControlExperts)
            {
                row++;
                IWParagraph membersParagraph = new WParagraph(document);

                bookNav.InsertParagraph(membersParagraph);

                var napooExperts = (await ExpertService.GetAllExpertsNAPOOAsync(controlExpert.IdExpert)).ToList();
                if (napooExperts != null && napooExperts.Count > 0)
                {
                    membersParagraph.AppendText("\n" + "\n" + row.ToString() + ". " + controlExpert.Expert.Person.FullName + ", " + string.Join(", ", napooExperts.Select(x => x.Occupation))).ApplyCharacterFormat(expertsCharacterFormat);
                }
                else
                {
                    membersParagraph.AppendText("\n" + "\n" + row.ToString() + ". " + controlExpert.Expert.Person.FullName).ApplyCharacterFormat(expertsCharacterFormat);
                }
                membersParagraph.ApplyStyle("ControlExpertsParagraph");
            }
            #endregion

            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, FormatType.Docx);
                FileData[] files = new FileData[]
                {
                                new FileData()
                                {
                                    BinaryContent = stream.ToArray(),
                                    Filename = "Prilojenie2_konstativen_protokol.docx"
                                }
                };

                RegisterDocumentParams registerDocumentParams = new RegisterDocumentParams()
                {
                    ExternalCode = kvDocumentType.DefaultValue2,
                    RegisterUser = int.Parse(indexUser.SettingValue),
                    RegisterUserSpecified = true
                };

                CorrespData corresp = new CorrespData()
                {
                    Names = this.CPOorCIPONameAndOwner,
                    EIK = this.Model.CandidateProvider.PoviderBulstat,
                    Phone = this.Model.CandidateProvider.ProviderPhone,
                    Email = this.Model.CandidateProvider.ProviderEmail
                };

                DocData docs = new DocData()
                {
                    Otnosno = "Констативен протокол от извършен последващ контрол",
                    Corresp = corresp,
                    File = files,

                };
                var registerResult = await this.DocuService.RegisterDocumentAsync(registerDocumentParams, docs);

                if (registerResult.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, registerResult.ListErrorMessages));
                    return;
                }
                var documentResponse = registerResult.ResultContextObject;
                followUpControlDocumentVM.IdFollowUpControl = this.Model.IdFollowUpControl;
                followUpControlDocumentVM.IdDocumentType = kvDocumentType.IdKeyValue;
                followUpControlDocumentVM.DS_ID = documentResponse.Doc.DocID;
                followUpControlDocumentVM.DS_GUID = documentResponse.Doc.GUID;
                followUpControlDocumentVM.DS_DATE = documentResponse.Doc.DocDate;
                followUpControlDocumentVM.DS_DocNumber = documentResponse.Doc.DocNumber;

                await this.ControlService.SaveControlDocument(followUpControlDocumentVM);

                await this.JsRuntime.SaveAs("Prilojenie2_konstativen_protokol" + ".docx", stream.ToArray());
                isProtocolGenerate = (await this.ControlService.GetAllDocumentsAsync(this.Model.IdFollowUpControl)).Any(d => d.DocumentTypeName == "Констативен протокол от извършен последващ контрол");
            }
        }
        private async Task GenerateLetter()
        {
            FollowUpControlDocumentVM followUpControlDocumentVM = new FollowUpControlDocumentVM();
            //Get resource document

            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
            string documentName = string.Empty;
            string fileName = string.Empty;
            string documentType = string.Empty;
            string lastDocumentType = string.Empty;

            if (CPOorCIPO == "ЦПО")
            {
                documentType = "CPO_ApplicationPK3";
                lastDocumentType = "CPO_ApplicationPK1";
                fileName = "Prilojenie3_pismo_cpo";
            }
            else
            {
                documentType = "CIPO_ApplicationPK3";
                lastDocumentType = "CIPO_ApplicationPK1";
                fileName = "Prilojenie3_pismo_cipo";
            }
            documentName = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", documentType)).Result.IdKeyValue).TemplatePath;
            FileStream template = new FileStream($@"{resources_Folder}{documentName}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);
            DocIORenderer render = new DocIORenderer();
            var kvDocumentType = await DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", documentType);
            var kvDocumentType1 = await DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", lastDocumentType);
            var indexUser = await DataSourceService.GetSettingByIntCodeAsync("IndexUserId");
            var documentBlank = (await this.ControlService.GetAllDocumentsAsync(this.Model.IdFollowUpControl)).FirstOrDefault(d => d.IdDocumentType == kvDocumentType1.IdKeyValue);
            var currentCPOorCIPO = this.IsCPO ? this.CPOorCIPONameAndOwner.Replace("ЦПО", "") : this.CPOorCIPONameAndOwner.Replace("ЦИПО", "");

            //Merge fields
            string[] fieldNames = new string[]
            {
                "DirectorFullName",
                "ProviderAddressCorrespondence",
                "LocationName",
                "Email",
                "CPOorCIPO",
                "LocationName2",
                "LicenceNumber",
                "DirectorFullName2",
                "Official_number",
                "Official_date",
                "CPOorCIPO2",
                "LocationName3",
                "LicenceNumber2",
            };
            string[] fieldValues = new string[]
            {
                 this.CPOorCIPO == "ЦПО" ? this.Model.CandidateProvider.DirectorFullName : this.CPOorCIPONameAndOwner,
                 this.Model.CandidateProvider.ProviderAddressCorrespondence != null ? this.Model.CandidateProvider.ProviderAddressCorrespondence : string.Empty,
                 this.Model.CandidateProvider.LocationCorrespondence != null ? this.Model.CandidateProvider.LocationCorrespondence.LocationName : string.Empty,
                 this.CPOorCIPO == "ЦПО" ? this.Model.CandidateProvider.ProviderEmailCorrespondence : this.Model.CandidateProvider.LocationCorrespondence.PostCode.ToString(),
                 currentCPOorCIPO,
                 this.Model.CandidateProvider.LocationCorrespondence != null ? this.Model.CandidateProvider.LocationCorrespondence.LocationName : string.Empty,
                 this.Model.CandidateProvider.LicenceNumber,
                 this.Model.CandidateProvider.DirectorFamilyName,
                 documentBlank.DS_OFFICIAL_DocNumber,
                 documentBlank.DS_OFFICIAL_DATE.HasValue ? documentBlank.DS_OFFICIAL_DATE.Value.ToString("dd.MM.yyyy") : string.Empty,
                 this.CPOorCIPONameAndOwner,
                 this.Model.CandidateProvider.LocationCorrespondence != null ? this.Model.CandidateProvider.LocationCorrespondence.LocationName : string.Empty,
                 this.CPOorCIPO == "ЦПО" ? this.Model.CandidateProvider.LicenceNumber : string.Empty
            };

            document.MailMerge.Execute(fieldNames, fieldValues);


            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, FormatType.Docx);
                FileData[] files = new FileData[]
                {
                                new FileData()
                                {
                                    BinaryContent = stream.ToArray(),
                                    Filename = fileName + ".docx"
                                }
                };

                RegisterDocumentParams registerDocumentParams = new RegisterDocumentParams()
                {
                    ExternalCode = kvDocumentType.DefaultValue2,
                    RegisterUser = int.Parse(indexUser.SettingValue),
                    RegisterUserSpecified = true
                };

                CorrespData corresp = new CorrespData()
                {
                    Names = this.CPOorCIPONameAndOwner,
                    EIK = this.Model.CandidateProvider.PoviderBulstat,
                    Phone = this.Model.CandidateProvider.ProviderPhone,
                    Email = this.Model.CandidateProvider.ProviderEmail
                };

                DocData docs = new DocData()
                {
                    Otnosno = "Уведомително писмо, съпровождащо констативния протокол",
                    Corresp = corresp,
                    File = files,

                };
                var registerResult = await this.DocuService.RegisterDocumentAsync(registerDocumentParams, docs);

                if (registerResult.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, registerResult.ListErrorMessages));
                    return;
                }
                var documentResponse = registerResult.ResultContextObject;

                followUpControlDocumentVM.IdFollowUpControl = this.Model.IdFollowUpControl;
                followUpControlDocumentVM.IdDocumentType = kvDocumentType.IdKeyValue;
                followUpControlDocumentVM.DS_ID = documentResponse.Doc.DocID;
                followUpControlDocumentVM.DS_GUID = documentResponse.Doc.GUID;
                followUpControlDocumentVM.DS_DATE = documentResponse.Doc.DocDate;
                followUpControlDocumentVM.DS_DocNumber = documentResponse.Doc.DocNumber;

                await this.ControlService.SaveControlDocument(followUpControlDocumentVM);

                await this.JsRuntime.SaveAs(fileName + ".docx", stream.ToArray());
                isLetterGenerate = (await this.ControlService.GetAllDocumentsAsync(this.Model.IdFollowUpControl)).Any(d => d.DocumentTypeName == "Уведомително писмо, съпровождащо констативния протокол");
            }
        }
        private async Task GenerateReport()
        {
            FollowUpControlDocumentVM followUpControlDocumentVM = new FollowUpControlDocumentVM();
            //Get resource document

            var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
            string documentName = string.Empty;
            string documentType = string.Empty;
            string lastDocumentType = string.Empty;
            if (CPOorCIPO == "ЦПО")
            {
                documentType = "CPO_ApplicationPK4";
                lastDocumentType = "CPO_ApplicationPK1";

            }
            else
            {
                documentType = "CIPO_ApplicationPK4";
                lastDocumentType = "CIPO_ApplicationPK1";
            }
            documentName = (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM())).First(x => x.IdApplicationType == (DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", documentType)).Result.IdKeyValue).TemplatePath;
            FileStream template = new FileStream($@"{resources_Folder}{documentName}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            WordDocument document = new WordDocument(template, FormatType.Docx);
            DocIORenderer render = new DocIORenderer();
            var kvDocumentType = await DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", documentType);
            var kvDocumentType1 = await DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType", lastDocumentType);
            var indexUser = await DataSourceService.GetSettingByIntCodeAsync("IndexUserId");
            var documentBlank = (await this.ControlService.GetAllDocumentsAsync(this.Model.IdFollowUpControl)).FirstOrDefault(d => d.IdDocumentType == kvDocumentType1.IdKeyValue);

            //Merge fields
            string[] fieldNames = new string[]
            {
                "Experts",
                "Official_number",
                "Official_date",
                "OnsiteControlDateFrom",
                "OnsiteControlDateTo",
                "Official_number2",
                "ControlTypeName",
                "CPOorCIPO",
                "LocationName",
                "LicenceNumber"

            };
            string[] fieldValues = new string[]
            {
                 followUpControlExperts.ToList().Count == 1 ? followUpControlExperts.ToList().FirstOrDefault().Expert.Person.FullName : string.Empty,
                 documentBlank.DS_OFFICIAL_DocNumber,
                 documentBlank.DS_OFFICIAL_DATE.HasValue ? documentBlank.DS_OFFICIAL_DATE.Value.ToString("dd.MM.yyyy") : string.Empty,
                 this.Model.OnsiteControlDateFrom.HasValue ? this.Model.OnsiteControlDateFrom.Value.ToString("dd.MM.yyyy") : string.Empty,
                 this.Model.OnsiteControlDateTo.HasValue ? this.Model.OnsiteControlDateTo.Value.ToString("dd.MM.yyyy") : string.Empty,
                 documentBlank.DS_OFFICIAL_DocNumber,
                 this.Model.ControlTypeName.ToLower(),
                 this.CPOorCIPONameAndOwner,
                 this.Model.CandidateProvider.LocationCorrespondence != null ? this.Model.CandidateProvider.LocationCorrespondence.LocationName : string.Empty,
                 this.Model.CandidateProvider.LicenceNumber,
            };

            document.MailMerge.Execute(fieldNames, fieldValues);


            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream, FormatType.Docx);
                FileData[] files = new FileData[]
                {
                                new FileData()
                                {
                                    BinaryContent = stream.ToArray(),
                                    Filename = "Prilojenie4_doklad_eksperti.docx"
                                }
                };

                RegisterDocumentParams registerDocumentParams = new RegisterDocumentParams()
                {
                    ExternalCode = kvDocumentType.DefaultValue2,
                    RegisterUser = int.Parse(indexUser.SettingValue),
                    RegisterUserSpecified = true
                };

                CorrespData corresp = new CorrespData()
                {
                    Names = this.CPOorCIPONameAndOwner,
                    EIK = this.Model.CandidateProvider.PoviderBulstat,
                    Phone = this.Model.CandidateProvider.ProviderPhone,
                    Email = this.Model.CandidateProvider.ProviderEmail
                };

                DocData docs = new DocData()
                {
                    Otnosno = "Доклад на длъжностното лице за отстранените нередовности",
                    Corresp = corresp,
                    File = files,

                };
                var registerResult = await this.DocuService.RegisterDocumentAsync(registerDocumentParams, docs);

                if (registerResult.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, registerResult.ListErrorMessages));
                    return;
                }
                var documentResponse = registerResult.ResultContextObject;

                followUpControlDocumentVM.IdFollowUpControl = this.Model.IdFollowUpControl;
                followUpControlDocumentVM.IdDocumentType = kvDocumentType.IdKeyValue;
                followUpControlDocumentVM.DS_ID = documentResponse.Doc.DocID;
                followUpControlDocumentVM.DS_GUID = documentResponse.Doc.GUID;
                followUpControlDocumentVM.DS_DATE = documentResponse.Doc.DocDate;
                followUpControlDocumentVM.DS_DocNumber = documentResponse.Doc.DocNumber;

                await this.ControlService.SaveControlDocument(followUpControlDocumentVM);

                await this.JsRuntime.SaveAs("Prilojenie4_doklad_eksperti" + ".docx", stream.ToArray());
                isReportGenerate = (await this.ControlService.GetAllDocumentsAsync(this.Model.IdFollowUpControl)).Any(d => d.DocumentTypeName == "Доклад на длъжностното лице за отстранените нередовности");
            }
        }

    }
}
