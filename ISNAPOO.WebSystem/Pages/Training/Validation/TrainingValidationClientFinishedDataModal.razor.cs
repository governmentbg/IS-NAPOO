using System.Drawing.Imaging;
using System.Drawing;
using System.Globalization;
using System.Text.RegularExpressions;
using Data.Models.Data.DOC;
using Data.Models.Data.Training;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Inputs;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class TrainingValidationClientFinishedDataModal : BlazorBaseComponent
    {
        private SfComboBox<int?, DocumentSerialNumberVM> docSerialNumbersComboBox = new SfComboBox<int?, DocumentSerialNumberVM>();

        private ValidationClientCombinedVM model = new ValidationClientCombinedVM();
        private IEnumerable<KeyValueVM> finishedTypeSource = new List<KeyValueVM>();
        private List<DocumentSerialNumberVM> documentSerialNumbersSource = new List<DocumentSerialNumberVM>();
        private ValidationMessageStore? messageStore;
        public bool isTabRendered = false;
        private IEnumerable<ValidationProtocolVM> protocolsSource = new List<ValidationProtocolVM>();
        private KeyValueVM kvFinishedWithDoc = new KeyValueVM();
        private string docRegNoPlaceholder = "например: 5044-1201";
        private KeyValueVM kvPartOfProfession = new KeyValueVM();
        private KeyValueVM kvSPK = new KeyValueVM();
        private List<KeyValueVM> typeFrameworkProgramSource = new List<KeyValueVM>();
        private SfUploader sfUploader;
        DocVM doc = new DocVM();
        public List<string> errorMessages = new List<string>();
        private KeyValueVM kvVocationalQualificationValidationCertificate;
        private List<ValidationCommissionMemberVM> membersSource = new List<ValidationCommissionMemberVM>();
        private IEnumerable<KeyValueVM> kvSexSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvNationalitySource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvIndentTypeSource = new List<KeyValueVM>();
        private KeyValueVM kvCourseFinished = new KeyValueVM();
        private KeyValueVM kvEGN = new KeyValueVM();
        private KeyValueVM kvLNCh = new KeyValueVM();
        private KeyValueVM kvIDN = new KeyValueVM();
        private KeyValueVM kvCourseCompleted = new KeyValueVM();
        private KeyValueVM kvQualificationLevel = new KeyValueVM();
        private KeyValueVM kvCertificateForValidationOfProfessionalQualificationInPartOfAProfession;
        private PrintDocumentModalMessage printDocumentModalMessage = new PrintDocumentModalMessage();

        [Parameter]
        public ValidationClientVM ClientVM { get; set; }
        [Parameter]
        public int PageType { get; set; }
        [Parameter]
        public EventCallback<ValidationClientCombinedVM> CallbackAfterEditContextValidation { get; set; }
        [Parameter]
        public bool IsEditable { get; set; } = true;

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }
        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        public ITemplateDocumentService templateDocumentService { get; set; }

        [Inject]
        public IDOCService DOCService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.model);
            FormTitle = "Данни за завършване";

            this.isTabRendered = true;

            var validationTypeSPK = await DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ValidationOfProfessionalQualifications");
            if (this.ClientVM is not null)
            {
                if (this.ClientVM.Speciality is not null)
                {
                    this.doc = await this.DOCService.GetActiveDocByProfessionIdAsync(this.ClientVM.Speciality.Profession);
                }
                this.membersSource = (await this.TrainingService.GetAllValidationCommissionMembersByClient(this.ClientVM.IdValidationClient)).ToList();
            }
            if (ClientVM.IdCourseType == validationTypeSPK.IdKeyValue)
            {
                this.kvFinishedWithDoc = await DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type5");
                this.finishedTypeSource = (await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseFinishedType")).Where(x => x.DefaultValue4 is not null && x.DefaultValue4!.Contains("validationSPK")).ToList();
            }
            else
            {
                this.finishedTypeSource = (await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseFinishedType")).Where(x => x.DefaultValue4 is not null && x.DefaultValue4!.Contains("validationPP")).ToList();
                this.kvFinishedWithDoc = await DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type8");
            }
            this.kvEGN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
            this.kvQualificationLevel = await this.DataSourceService.GetKeyValueByIntCodeAsync("QualificationLevel", "WithoutQualification_Update");
            this.kvLNCh = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "LNK");
            this.kvIDN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "IDN");
            this.kvSexSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");
            this.kvNationalitySource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality");
            this.kvIndentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            this.kvCourseCompleted = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusFinished");
            this.kvCourseFinished = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type5");


            //this.kvFinishedWithDoc = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type1");

            await LoadProtocolsDataAsync();

            await LoadModelDataAsync();

            LoadDocumentSerialNumbersData();
        }

        private async void LoadDocumentSerialNumbersData()
        {
            if (this.model.FinishedYear.HasValue)
            {

                if (this.model.FinishedYear.ToString().Length == 4 && (this.model.FinishedYear.Value == DateTime.Now.Year || this.model.FinishedYear.Value == DateTime.Now.Year - 1))
                {
                    CandidateProviderVM candidateProvider = new CandidateProviderVM() { IdCandidate_Provider = UserProps.IdCandidateProvider };
                    this.documentSerialNumbersSource = ProviderDocumentRequestService.GetAllDocumentSerialNumbersByIdCandidateProviderByStatusReceivedByYearAndByIdCourseType(candidateProvider, this.model.FinishedYear.Value, ClientVM.IdCourseType).OrderBy(x => x.SerialNumberAsIntForOrderBy).ToList();
                    if (this.model.IdDocumentSerialNumber.HasValue)
                    {
                        if (!this.documentSerialNumbersSource.Any(x => x.IdDocumentSerialNumber == this.model.IdDocumentSerialNumber.Value))
                        {
                            var docSerialNumber = await ProviderDocumentRequestService.GetDocumentSerialNumberByIdAndYearAsync(this.model.IdDocumentSerialNumber.Value, this.model.FinishedYear.Value);
                            if (docSerialNumber != null)
                            {
                                this.documentSerialNumbersSource.Add(docSerialNumber);
                            }

                            if (this.documentSerialNumbersSource.Any())
                            {
                                this.documentSerialNumbersSource = this.documentSerialNumbersSource.OrderBy(x => x.SerialNumberAsIntForOrderBy).ToList();

                                await this.docSerialNumbersComboBox.RefreshDataAsync();
                                StateHasChanged();
                            }
                        }
                    }
                }
            }
        }

        private void OnFinishedYearValueChanged()
        {
            LoadDocumentSerialNumbersData();
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            var file = args.Files[0].Stream;

            var result = await this.UploadFileService.UploadFileAsync<ValidationDocumentUploadedFile>(file, args.Files[0].FileInfo.Name, "ValidationDocument", this.model.IdValidationDocumentUploadedFile);
            if (!string.IsNullOrEmpty(result))
            {
                this.model.UploadedFileName = result;
            }

            await this.sfUploader.ClearAllAsync();
            await LoadModelDataAsync();

            StateHasChanged();
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationDocumentUploadedFile>(this.model.IdValidationDocumentUploadedFile);
                    if (result == 1)
                    {
                        this.model.UploadedFileName = null;
                    }

                    StateHasChanged();
                }
            }
        }

        private async Task OnRemove(string fileName, int IdValidationDocumentUploadedFile)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                bool isConfirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationDocumentUploadedFile>(this.model.IdValidationDocumentUploadedFile);
                    if (result == 1)
                    {
                        this.model.UploadedFileName = null;
                    }

                    await LoadModelDataAsync();

                    StateHasChanged();
                }
            }
        }

        private async Task OnDownloadClick(string fileName, int IdValidationDocumentUploadedFile)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!string.IsNullOrEmpty(this.model.UploadedFileName))
                {
                    this.model.UploadedFileName = fileName;
                    this.model.IdValidationDocumentUploadedFile = IdValidationDocumentUploadedFile;
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ValidationDocumentUploadedFile>(IdValidationDocumentUploadedFile);
                    if (hasFile)
                    {
                        var document = await this.UploadFileService.GetUploadedFileAsync<ValidationDocumentUploadedFile>(IdValidationDocumentUploadedFile);
                        if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, this.model.FileName, document.MS!.ToArray());
                        }
                    }
                    else
                    {
                        var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                        await this.ShowErrorAsync(msg);
                    }
                }
                else
                {
                    var msg = LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnProtocolSelected(ChangeEventArgs<int?, ValidationProtocolVM> args)
        {
            SpinnerShow();

            if (args.ItemData is not null)
            {
                this.model.DocumentProtocol = args.ItemData.ValidationProtocolNumber;
                this.model.FinalResult = args.ItemData.ValidationProtocolGrades.FirstOrDefault().Grade.ToString();
                var grades = await TrainingService.GetTheoryAndPracticeGradesFromValidationProtocols380ByIdCourseAndIdCourseClient(ClientVM.IdValidationClient);
                //this.model.TheoryResult = grades[0];
                //this.model.PracticeResult = grades[1];
            }
            else
            {
                this.model.DocumentProtocol = null;
                this.model.FinalResult = string.Empty;
                //this.model.TheoryResult = string.Empty;
                //this.model.PracticeResult = string.Empty;
            }

            SpinnerHide();
        }

        public override async void SubmitHandler()
        {
            this.errorMessages.Clear();
            if (this.model.IdFinishedType.HasValue && this.model.IdFinishedType != this.kvFinishedWithDoc.IdKeyValue)
            {
                this.model.FinishedYear = null;
            }
            if (kvFinishedWithDoc.IdKeyValue == this.model.IdFinishedType)
            {
                this.editContext = new EditContext(this.model);
                this.editContext.EnableDataAnnotationsValidation();
                this.messageStore = new ValidationMessageStore(this.editContext);
                //this.editContext.OnValidationRequested += this.ValidateFinishedDate;
                this.editContext.OnValidationRequested += ValidateDocumentDate;
                this.editContext.OnValidationRequested += ValidateRequiredFabricNumber;
                this.editContext.OnValidationRequested += ValidateProtocolSelected;
                this.editContext.OnValidationRequested += ValidateRequiredFields;
                this.editContext.OnValidationRequested += ValidateGrades;
            }

            var validate = this.editContext.Validate();

            this.errorMessages.AddRange(this.editContext.GetValidationMessages().Distinct());

            if (validate)
            {
                if (this.model.IdValidationClientDocument != 0)
                {
                    int isValid = await TrainingService.UpdateValidationDocumentProtocolAsync(this.model);
                }
                else
                {
                    this.model = await TrainingService.CreateValidationDocumentProtocolAsync(this.model);
                }

                await LoadModelDataAsync();
                StateHasChanged();
            }
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.model.DocumentRegNo) && string.IsNullOrEmpty(this.model.UploadedFileName) && this.model.IdDocumentSerialNumber is null &&
                !this.model.DocumentDate.HasValue && this.model.IdFinishedType is not null;
        }

        public async Task LoadProtocolsDataAsync()
        {
            this.protocolsSource = await TrainingService.GetAll381BProtocolsAddedByIdValidationClientAsync(ClientVM.IdValidationClient);
            StateHasChanged();
        }

        private void ValidateDocumentDate(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.model.IdFinishedType.HasValue && this.model.IdFinishedType.Value == this.kvFinishedWithDoc.IdKeyValue)
            {
                if (ClientVM is not null)
                {
                    if (ClientVM.StartDate.HasValue)
                    {
                        if (this.model.DocumentDate < ClientVM.StartDate)
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.model, "FinishedDate");
                            this.messageStore?.Add(fi, $"Полето 'Дата на издаване' не може да бъде преди {ClientVM.StartDate.Value.ToString("dd.MM.yyyy")}г.!");
                            return;
                        }
                    }
                }
            }
        }
        private void ValidateGrades(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.model is not null)
            {
                //if (string.IsNullOrEmpty(this.model.TheoryResult))
                //{
                //    FieldIdentifier fi = new FieldIdentifier(this.model, "TheoryResult");
                //    this.messageStore?.Add(fi, "Полето 'Оценка по теория' е задължително!");
                //}
                //else
                //{
                //    var value = BaseHelper.ConvertToFloat(this.model.TheoryResult, 2);
                //    FieldIdentifier fi = new FieldIdentifier(this.model, "TheoryResult");
                //    if (value < 2)
                //    {
                //        this.messageStore?.Add(fi, "Полето 'Оценка по теория' не може да има стойност по-малка от 2.00!");
                //    }

                //    if (value > 6)
                //    {
                //        this.messageStore?.Add(fi, "Полето 'Оценка по теория' не може да има стойност по-голяма от 6.00!");
                //    }

                //    if (value.ToString().Length > 4)
                //    {
                //        this.messageStore?.Add(fi, "Полето 'Оценка по теория' не може да съдържа повече от 2 знака след десетичната запетая!");
                //    }
                //}

                if (string.IsNullOrEmpty(this.model.FinalResult))
                {
                    FieldIdentifier fi = new FieldIdentifier(this.model, "FinalResult");
                    this.messageStore?.Add(fi, "Полето 'Обща оценка' е задължително!");
                }
                else
                {
                    var value = BaseHelper.ConvertToFloat(this.model.FinalResult, 2);
                    FieldIdentifier fi = new FieldIdentifier(this.model, "FinalResult");
                    if (value < 2)
                    {
                        this.messageStore?.Add(fi, "Полето 'Обща оценка' не може да има стойност по-малка от 2.00!");
                    }

                    if (value > 6)
                    {
                        this.messageStore?.Add(fi, "Полето 'Обща оценка' не може да има стойност по-голяма от 6.00!");
                    }

                    if (value.ToString().Length > 4)
                    {
                        this.messageStore?.Add(fi, "Полето 'Обща оценка' не може да съдържа повече от 2 знака след десетичната запетая!");
                    }
                }

                //if (string.IsNullOrEmpty(this.model.PracticeResult))
                //{
                //    FieldIdentifier fi = new FieldIdentifier(this.model, "PracticeResult");
                //    this.messageStore?.Add(fi, "Полето 'Оценка по практика' е задължително!");
                //}
                //else
                //{
                //    var value = BaseHelper.ConvertToFloat(this.model.PracticeResult, 2);
                //    FieldIdentifier fi = new FieldIdentifier(this.model, "PracticeResult");
                //    if (value < 2)
                //    {
                //        this.messageStore?.Add(fi, "Полето 'Оценка по практика' не може да има стойност по-малка от 2.00!");
                //    }

                //    if (value > 6)
                //    {
                //        this.messageStore?.Add(fi, "Полето 'Оценка по практика' не може да има стойност по-голяма от 6.00!");
                //    }

                //    if (value.ToString().Length > 4)
                //    {
                //        this.messageStore?.Add(fi, "Полето 'Оценка по практика' не може да съдържа повече от 2 знака след десетичната запетая!");
                //    }
                //}
            }
        }
        private void ValidateFinishedDate(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (ClientVM is not null)
            {
                if (ClientVM.StartDate.HasValue)
                {
                    if (this.model.FinishedDate < ClientVM.StartDate)
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.model, "FinishedDate");
                        this.messageStore?.Add(fi, $"Полето 'Дата на завършване на курса' не може да бъде преди {ClientVM.StartDate.Value.ToString("dd.MM.yyyy")}г.!");
                        return;
                    }
                }

                if (ClientVM.EndDate.HasValue)
                {
                    if (this.model.FinishedDate > ClientVM.EndDate)
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.model, "FinishedDate");
                        this.messageStore?.Add(fi, $"Полето 'Дата на завършване на курса' не може да бъде след {ClientVM.EndDate.Value.ToString("dd.MM.yyyy")}г.!");
                    }
                }
            }
        }

        private void ValidateRequiredFabricNumber(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.model.IdFinishedType.HasValue && this.model.IdFinishedType.Value == this.kvFinishedWithDoc.IdKeyValue)
            {
                if (this.model.HasDocumentFabricNumber && this.model.IdDocumentSerialNumber is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.model, "IdDocumentSerialNumber");
                    this.messageStore?.Add(fi, "Полето 'Фабричен номер на документа' е задължително!");
                }
            }
        }

        private void ValidateProtocolSelected(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.model.IdFinishedType.HasValue && this.model.IdFinishedType.Value == this.kvFinishedWithDoc.IdKeyValue)
            {
                if (this.model.IdValidationProtocol is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.model, "IdCourseProtocol");
                    this.messageStore?.Add(fi, "Полето 'Протокол' е задължително!");
                }
            }
        }

        private void ValidateRequiredFields(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.model.IdFinishedType.HasValue && this.model.IdFinishedType.Value == this.kvFinishedWithDoc.IdKeyValue)
            {
                if (this.model.FinishedYear is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.model, "FinishedYear");
                    this.messageStore?.Add(fi, "Полето 'Година на завършване' е задължително!");
                }

                if (string.IsNullOrEmpty(this.model.DocumentRegNo))
                {
                    FieldIdentifier fi = new FieldIdentifier(this.model, "DocumentRegNo");
                    this.messageStore?.Add(fi, "Полето 'Регистрационен номер на документа' е задължително!");
                }
                else
                {
                    this.model.DocumentRegNo = this.model.DocumentRegNo.Trim();
                    if (!Regex.IsMatch(this.model.DocumentRegNo, @"^[0-9]+[--]{1}[0-9]+$"))
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.model, "DocumentRegNo");
                        this.messageStore?.Add(fi, "Моля, въведете коректна стойност в полето 'Регистрационен номер на документа'! Регистрационният номер на документа трябва да съдържа тире (-) като разделител между цифрите.");
                    }
                }

                if (this.model.DocumentDate is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.model, "DocumentDate");
                    this.messageStore?.Add(fi, "Полето 'Дата на издаване' е задължително!");
                }
            }
        }

        public async Task LoadModelDataAsync()
        {
            if (ClientVM is not null)
            {
                this.model = await TrainingService.GetValidationClientCombinedVMByIdClientCourseAsync(ClientVM.IdValidationClient);
            }
        }

        private async Task Export()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                this.printDocumentModalMessage.OpenModal();

            }
            finally
            {
                this.loading = false;
            }
            this.SpinnerHide();

            try
            {
                if (this.model.DocumentTypeName.ToLower()
                    .Contains("свидетелство за валидиране на професионална квалификация"))
                {
                    this.kvVocationalQualificationValidationCertificate =
                        await this.DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType",
                            "VocationalQualificationValidationCertificate");
                    var templateDocuments =
                        (await this.templateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM()))
                        .Where(x =>
                            x.IdApplicationType == this.kvVocationalQualificationValidationCertificate.IdKeyValue
                        );
                    var templateDocument = templateDocuments.FirstOrDefault(x =>
                        this.model.DocumentDate == null
                            ? true
                            : this.model.DocumentDate.Value >= x.DateFrom && this.model.DocumentDate.Value <= x.DateTo);

                    var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
                    FileStream blueprint = new FileStream($@"{resources_Folder}{templateDocument.TemplatePath}",
                        FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    WordDocument document = new WordDocument(blueprint, FormatType.Docx);
                    LocationVM clientLocation = new LocationVM();
                    if (this.ClientVM.IdCityOfBirth != null)
                    {
                        clientLocation =
                            await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(
                                this.ClientVM.IdCityOfBirth.Value);
                    }

                    string[] fieldNames = new string[]
                    {
                        "InstitutionName", "Kati", "MunicipalityName", "Region", "DistrictName", "NKR", "EKR",
                        "Director", "RegNum", "Date", "PersonName", "EGN", "PersonCity", "MunicipalityPerson",
                        "DistrictPerson", "FID", "OtherIdent", "NationalityPerson", "SPK", "Profession",
                        "Speciality", "DocumentProtocol", "DocumentDate", "AssessedGrade", "ChairmanOfPQC",
                        "OrderN", "OrderDate", "PaperN", "PaperDate", "CompetenceN", "Competence", "Proxy"
                    };


                    var protocols =
                        await this.TrainingService.GetAll381BProtocolsAddedByIdValidationClientAsync(this.ClientVM
                            .IdValidationClient);
                    ValidationCommissionMemberVM commissionMember = new ValidationCommissionMemberVM();
                    string[] protocolNameAndDate = new string[2];
                    if (protocols.Any())
                    {
                        commissionMember = this.membersSource.FirstOrDefault(commissionMember =>
                            commissionMember.IdValidationCommissionMember ==
                            protocols.FirstOrDefault().IdValidationCommissionMember);

                        var firstProtocol = protocols.FirstOrDefault();
                        var nameAndDate = firstProtocol.NameAndDate;
                        protocolNameAndDate = nameAndDate.Remove(0, 1).Replace("г.", "").Split("/");


                    }

                    string institutionName1 = "Център за професионално обучение ";

                    if (this.ClientVM.CandidateProvider != null)
                    {
                        if (string.IsNullOrEmpty(this.ClientVM.CandidateProvider.ProviderName))
                        {
                            institutionName1 += "към ";
                        }
                        else
                        {
                            if (this.ClientVM.CandidateProvider.ProviderName.StartsWith("ЦПО към") ||
                                this.ClientVM.CandidateProvider.ProviderName.StartsWith(
                                    "Център за професионално обучение към "))
                            {
                                institutionName1 =
                                    this.ClientVM.CandidateProvider.ProviderName.Replace("ЦПО към",
                                        "Център за професионално обучение към ");
                            }

                            institutionName1 += this.ClientVM.CandidateProvider.ProviderName + " към ";
                        }

                        if (this.ClientVM.CandidateProvider.ProviderOwner != "" ||
                            this.ClientVM.CandidateProvider.ProviderOwner != null)
                        {
                            institutionName1 += this.ClientVM.CandidateProvider.ProviderOwner;
                        }
                    }

                    institutionName1 += string.Concat(Enumerable.Repeat("\n-",
                        5 - CalculateNumberOfLines(institutionName1.Trim(), 306,
                            new Font("Calibri", 12f, FontStyle.Bold))));

                    string competence = "-";
                    string competenceN = "-";


                    string kati = this.ClientVM.CandidateProvider?.Location?.kati ??
                                  "-" + "\n                                        -";
                    string municipalityName =
                        this.ClientVM.CandidateProvider?.Location?.Municipality?.MunicipalityName ?? "-";
                    string region = this.ClientVM.CandidateProvider?.Location?.Municipality?.Regions
                        ?.FirstOrDefault(region =>
                            region.idMunicipality ==
                            this.ClientVM.CandidateProvider.Location.Municipality.idMunicipality)?.RegionName ?? "-";
                    string districtName =
                        this.ClientVM.CandidateProvider?.Location?.Municipality?.District?.DistrictName ?? "-";
                    string nkr = this.ClientVM.Speciality.IdNKRLevel == 0
                        ? "-"
                        : (await this.DataSourceService.GetKeyValueByIdAsync(this.ClientVM.Speciality.IdNKRLevel))
                        ?.Name ?? "-";
                    string ekr = this.ClientVM.Speciality.IdEKRLevel == 0
                        ? "-"
                        : (await this.DataSourceService.GetKeyValueByIdAsync(this.ClientVM.Speciality.IdEKRLevel))
                        ?.Name ?? "-";
                    string director = string.IsNullOrEmpty(this.ClientVM.CandidateProvider?.DirectorFullName)
                        ? "-"
                        : this.ClientVM.CandidateProvider?.DirectorFirstName + " " +
                          this.ClientVM.CandidateProvider?.DirectorFamilyName;
                    string regNum = string.IsNullOrEmpty(this.model.DocumentRegNo) ? "-" : this.model.DocumentRegNo;
                    string date = this.model.DocumentDate == null
                        ? "-"
                        : this.model.DocumentDate.Value.ToString("dd.MM.yyyy");
                    string personName =
                        $"{this.ClientVM.FirstName} {this.ClientVM.SecondName} {this.ClientVM.FamilyName}";
                    string personCity = clientLocation?.kati ?? "-";
                    string municipalityPerson = clientLocation?.Municipality?.MunicipalityName ?? "-";
                    string districtPerson = clientLocation?.Municipality?.District?.DistrictName ?? "-";
                    string egn = "-";
                    string lnch = "-";
                    string idn = "-";
                    string indent = this.ClientVM.Indent ?? "-";

                    if (this.ClientVM.IdIndentType == this.kvEGN.IdKeyValue)
                    {
                        egn = indent;
                    }
                    else if (this.ClientVM.IdIndentType == this.kvLNCh.IdKeyValue)
                    {

                        lnch = indent;

                    }
                    else if (this.ClientVM.IdIndentType == this.kvIDN.IdKeyValue)
                    {
                        idn = indent;
                    }

                    string nationalityPerson = this.ClientVM.IdCountryOfBirth != null
                        ? kvNationalitySource.FirstOrDefault(x => x.IdKeyValue == this.ClientVM.IdCountryOfBirth)
                            ?.Name ?? "-"
                        : "-";
                    string spk = (await this.DataSourceService.GetKeyValueByIdAsync(this.ClientVM.Speciality.IdVQS))
                        ?.DefaultValue1 ?? "-";
                    string profession = this.ClientVM?.Speciality?.Profession?.Code == null
                        ? "-"
                        : this.ClientVM.Speciality.Profession.Code + " " + this.ClientVM.Speciality.Profession.Name;
                    profession += string.Concat(Enumerable.Repeat("\n-",
                        2 - BaseHelper.CalculateNumberOfLines("                  " + profession, 343f,
                            new Font("Calibri", 11f, FontStyle.Regular))));
                    string speciality = this.ClientVM?.Speciality?.Code == null
                        ? "-"
                        : this.ClientVM.Speciality.Code + " " + this.ClientVM.Speciality.Name;
                    speciality += string.Concat(Enumerable.Repeat("\n-",
                        2 - BaseHelper.CalculateNumberOfLines("                   " + speciality, 343f,
                            new Font("Calibri", 11f, FontStyle.Regular))));
                    string chairmanOfPQC = this.membersSource.Any() && protocols.Any() && commissionMember != null
                        ? commissionMember.FullName
                        : "-";
                    string protocolName = protocolNameAndDate?[0] ?? "-";
                    string documentDate = protocolNameAndDate?[1] ?? "-";
                    string assessedGradeValue = !string.IsNullOrEmpty(this.model.FinalResult)
                        ? Double.Parse(this.model.FinalResult)
                            .ToString("f2", System.Globalization.CultureInfo.InvariantCulture)
                        : "-";
                    string assessedGradeName = !string.IsNullOrEmpty(this.model.FinalResult)
                        ? BaseHelper.GetGradeName(Convert.ToDouble(this.model.FinalResult))
                        : "-";
                    string orderN = Regex.Match(this.doc?.Regulation ?? "", @"(?<=№\s*)\d+").Value ?? "-";

                    string orderDate = Regex.Match(this.doc?.Regulation?.ToLower() ?? "",
                        @"(?<=от)\s+\d{1,2}\s+[А-Яа-я]+\s+\d{4}").Value.Trim();
                    DateTime orderDateTime;
                    orderDate = DateTime.TryParseExact(orderDate, "d MMMM yyyy", CultureInfo.GetCultureInfo("bg-BG"),
                        DateTimeStyles.None, out orderDateTime)
                        ? orderDateTime.ToString("dd.MM.yyyy")
                        : "-";
                    string paperN = this.doc?.NewspaperNumber ?? "-";
                    string paperDate = this.doc?.PublicationDate?.ToString("dd.MM.yyyy") ?? "-";
                    string proxy = "-";
                    string?[] fieldValues = new string[]
                    {
                        //InstitutionName
                        institutionName1,
                        //Kati
                        kati,
                        //MunicipalityName
                        municipalityName,
                        //Region
                        region,
                        //DistrictName
                        districtName,
                        //NKR
                        nkr,
                        //EKR
                        ekr,
                        //Director
                        director,
                        // RegNum
                        regNum,
                        // Date
                        date,
                        // PersonName
                        personName,
                        // EGN
                        egn,
                        // PersonCity
                        personCity,
                        // MunicipalityPerson
                        municipalityPerson,
                        // DistrictPerson
                        districtPerson,
                        // FID
                        lnch,
                        // OtherIdent
                        idn,
                        // NationalityPerson
                        nationalityPerson,
                        // SPK
                        spk,
                        // Profession
                        profession,
                        // Speciality
                        speciality,
                        // DocumentProtocol
                        protocolName,
                        // DocumentDate
                        documentDate,
                        // AssessedGrade
                        assessedGradeName + " " + assessedGradeValue ?? "-",
                        // ChairmanOfPQC
                        chairmanOfPQC,
                        // OrderN
                        orderN,
                        // OrderDate
                        orderDate,
                        // PaperN
                        paperN,
                        // PaperDate
                        paperDate,
                        // CompetenceN
                        competenceN,
                        // Competence
                        competence,
                        // Proxy
                        proxy
                    };

                    document.MailMerge.Execute(fieldNames,
                        fieldValues.Select(s => string.IsNullOrEmpty(s) ? "-" : s).ToArray());


                    await CreateCompetenciesTable(document);
                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, FormatType.Docx);
                    blueprint.Close();
                    document.Close();
                    await FileUtils.SaveAs(JsRuntime,
                        BaseHelper.ConvertCyrToLatin("Свидетелство_" + this.ClientVM.FirstName + "_" +
                                                     this.ClientVM.FamilyName) + ".docx", stream.ToArray());
                }
                else if (this.model.DocumentTypeName.Equals(
                             "Удостоверение за валидиране на професионална квалификация по част от професия"))
                {
                    this.kvCertificateForValidationOfProfessionalQualificationInPartOfAProfession =
                        await this.DataSourceService.GetKeyValueByIntCodeAsync("ProcedureDocumentType",
                            "CertificateForValidationOfProfessionalQualificationInPartOfAProfession");
                    var templateDocuments =
                        (await this.templateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM()))
                        .Where(x =>
                            x.IdApplicationType ==
                            this.kvCertificateForValidationOfProfessionalQualificationInPartOfAProfession.IdKeyValue
                        );
                    var templateDocument = templateDocuments.FirstOrDefault(x =>
                        this.model.DocumentDate == null
                            ? true
                            : this.model.DocumentDate.Value >= x.DateFrom && this.model.DocumentDate.Value <= x.DateTo);


                    var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
                    FileStream blueprint = new FileStream($@"{resources_Folder}{templateDocument.TemplatePath}",
                        FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    WordDocument document = new WordDocument(blueprint, FormatType.Docx);
                    LocationVM clientLocation = new LocationVM();

                    if (this.ClientVM.IdCityOfBirth != null)
                    {
                        clientLocation =
                            await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(
                                this.ClientVM.IdCityOfBirth.Value);
                    }

                    string[] fieldNames = new string[]
                    {
                        "InstitutionName", "Kati", "MunicipalityName", "Region", "DistrictName", "NKR", "EKR",
                        "Director", "RegNum", "Date", "PersonName", "EGN", "Sex", "PersonCity",
                        "MunicipalityPerson", "DistrictPerson", "FID", "OtherIdent", "NationalityPerson", "Part",
                        "Profession", "Speciality", "DocumentProtocol", "DocumentDate", "AssessedGrade",
                        "ChairmanOfPQC", "OrderN", "OrderDate", "PaperN", "PaperDate", "OrderN1", "OrderDateN1",
                        "OrderN2", "OrderDateN2", "ResultDOC"
                    };


                    var protocols =
                        await this.TrainingService.GetAll381BProtocolsAddedByIdValidationClientAsync(this.ClientVM
                            .IdValidationClient);
                    ValidationCommissionMemberVM commissionMember = new ValidationCommissionMemberVM();
                    string[] protocolNameAndDate = new string[2];
                    if (protocols.Any())
                    {
                        commissionMember = this.membersSource.FirstOrDefault(commissionMember =>
                            commissionMember.IdValidationCommissionMember ==
                            protocols.FirstOrDefault().IdValidationCommissionMember);

                        var firstProtocol = protocols.FirstOrDefault();
                        var nameAndDate = firstProtocol.NameAndDate;
                        protocolNameAndDate = nameAndDate.Remove(0, 1).Replace("г.", "").Split("/");


                    }

                    string institutionName1 = "Център за професионално обучение ";

                    if (this.ClientVM.CandidateProvider != null)
                    {
                        if (string.IsNullOrEmpty(this.ClientVM.CandidateProvider.ProviderName))
                        {
                            institutionName1 += "към ";
                        }
                        else
                        {
                            if (this.ClientVM.CandidateProvider.ProviderName.StartsWith("ЦПО към") ||
                                this.ClientVM.CandidateProvider.ProviderName.StartsWith(
                                    "Център за професионално обучение към "))
                            {
                                institutionName1 =
                                    this.ClientVM.CandidateProvider.ProviderName.Replace("ЦПО към",
                                        "Център за професионално обучение към ");
                            }

                            institutionName1 += this.ClientVM.CandidateProvider.ProviderName + " към ";
                        }

                        if (this.ClientVM.CandidateProvider.ProviderOwner != "" ||
                            this.ClientVM.CandidateProvider.ProviderOwner != null)
                        {
                            institutionName1 += this.ClientVM.CandidateProvider.ProviderOwner;
                        }
                    }

                    string kati = this.ClientVM.CandidateProvider?.Location?.kati ??
                                  "-" + "\n                                        -";
                    string municipalityName =
                        this.ClientVM.CandidateProvider?.Location?.Municipality?.MunicipalityName ?? "-";
                    string region = this.ClientVM.CandidateProvider?.Location?.Municipality?.Regions
                        ?.FirstOrDefault(region =>
                            region.idMunicipality ==
                            this.ClientVM.CandidateProvider.Location.Municipality.idMunicipality)?.RegionName ?? "-";
                    string districtName =
                        this.ClientVM.CandidateProvider?.Location?.Municipality?.District?.DistrictName ?? "-";
                    string nkr = this.ClientVM.Speciality.IdNKRLevel == 0
                        ? "-"
                        : (await this.DataSourceService.GetKeyValueByIdAsync(this.ClientVM.Speciality.IdNKRLevel))
                        ?.Name ?? "-";
                    string ekr = this.ClientVM.Speciality.IdEKRLevel == 0
                        ? "-"
                        : (await this.DataSourceService.GetKeyValueByIdAsync(this.ClientVM.Speciality.IdEKRLevel))
                        ?.Name ?? "-";
                    string director = string.IsNullOrEmpty(this.ClientVM.CandidateProvider?.DirectorFullName)
                        ? "-"
                        : this.ClientVM.CandidateProvider?.DirectorFirstName + " " +
                          this.ClientVM.CandidateProvider?.DirectorFamilyName;
                    string regNum = string.IsNullOrEmpty(this.model.DocumentRegNo) ? "-" : this.model.DocumentRegNo;
                    string date = this.model.DocumentDate == null
                        ? "-"
                        : this.model.DocumentDate.Value.ToString("dd.MM.yyyy");
                    string personName =
                        $"{this.ClientVM.FirstName} {this.ClientVM.SecondName} {this.ClientVM.FamilyName}";
                    string personCity = clientLocation?.kati ?? "-";
                    string sex = this.ClientVM.IdSex ==
                                 this.kvSexSource.FirstOrDefault(x => x.Name == "Мъж")?.IdKeyValue
                        ? "-"
                        : "a";

                    string municipalityPerson = clientLocation?.Municipality?.MunicipalityName ?? "-";
                    string districtPerson = clientLocation?.Municipality?.District?.DistrictName ?? "-";
                    string egn = "-";
                    string lnch = "-";
                    string idn = "-";
                    string indent = this.ClientVM.Indent ?? "-";

                    if (this.ClientVM.IdIndentType == this.kvEGN.IdKeyValue)
                    {
                        egn = indent;
                    }
                    else if (this.ClientVM.IdIndentType == this.kvLNCh.IdKeyValue)
                    {

                        lnch = indent;

                    }
                    else if (this.ClientVM.IdIndentType == this.kvIDN.IdKeyValue)
                    {
                        idn = indent;
                    }

                    string nationalityPerson = this.ClientVM.IdCountryOfBirth != null
                        ? kvNationalitySource.FirstOrDefault(x => x.IdKeyValue == this.ClientVM.IdCountryOfBirth)
                            ?.Name ?? "-"
                        : "-";
                    string profession = this.ClientVM?.Speciality?.Profession?.Code == null
                        ? "-"
                        : this.ClientVM.Speciality.Profession.Code + " " + this.ClientVM.Speciality.Profession.Name;
                    profession += string.Concat(Enumerable.Repeat("\n-",
                        2 - BaseHelper.CalculateNumberOfLines("                  " + profession, 343f,
                            new Font("Calibri", 11f, FontStyle.Regular))));
                    string speciality = this.ClientVM?.Speciality?.Code == null
                        ? "-"
                        : this.ClientVM.Speciality.Code + " " + this.ClientVM.Speciality.Name;
                    speciality += string.Concat(Enumerable.Repeat("\n-",
                        2 - BaseHelper.CalculateNumberOfLines("                   " + speciality, 343f,
                            new Font("Calibri", 11f, FontStyle.Regular))));
                    string chairmanOfPQC = this.membersSource.Any() && protocols.Any() && commissionMember != null
                        ? commissionMember.FullName
                        : "-";
                    string protocolName = protocolNameAndDate?[0] ?? "-";
                    string documentDate = protocolNameAndDate?[1] ?? "-";
                    string assessedGradeValue = !string.IsNullOrEmpty(this.model.FinalResult)
                        ? Double.Parse(this.model.FinalResult)
                            .ToString("f2", System.Globalization.CultureInfo.InvariantCulture)
                        : "-";
                    string assessedGradeName = !string.IsNullOrEmpty(this.model.FinalResult)
                        ? BaseHelper.GetGradeName(Convert.ToDouble(this.model.FinalResult))
                        : "-";
                    string orderN = Regex.Match(this.doc?.Regulation ?? "", @"(?<=№\s*)\d+").Value ?? "-";
                    string orderDate = Regex.Match(this.doc?.Regulation?.ToLower() ?? "",
                        @"(?<=от)\s+\d{1,2}\s+[А-Яа-я]+\s+\d{4}").Value.Trim();
                    DateTime orderDateTime;
                    orderDate = DateTime.TryParseExact(orderDate, "d MMMM yyyy", CultureInfo.GetCultureInfo("bg-BG"),
                        DateTimeStyles.None, out orderDateTime)
                        ? orderDateTime.ToString("dd.MM.yyyy")
                        : "-";
                    string paperN = this.doc?.NewspaperNumber ?? "-";
                    string paperDate = this.doc?.PublicationDate?.ToString("dd.MM.yyyy") ?? "-";

                    string part ="Част от професия " + this.ClientVM?.Speciality?.Profession?.Name ?? "-";
                    part += string.Concat(Enumerable.Repeat("\u000B-",
                        3 - BaseHelper.CalculateNumberOfLines("                                                                                                                      " +part, 444f,
                            new Font("Calibri", 10f))));
                    string?[] fieldValues = new string[]
                    {
                        //InstitutionName
                        institutionName1,
                        //Kati
                        kati,
                        //MunicipalityName
                        municipalityName,
                        //Region
                        region,
                        //DistrictName
                        districtName,
                        //NKR
                        nkr,
                        //EKR
                        ekr,
                        //Director
                        director,
                        // RegNum
                        regNum,
                        // Date
                        date,
                        // PersonName
                        personName,
                        // EGN
                        egn,
                        // Sex
                        sex,
                        // PersonCity
                        personCity,
                        // MunicipalityPerson
                        municipalityPerson,
                        // DistrictPerson
                        districtPerson,
                        // FID
                        lnch,
                        // OtherIdent
                        idn,
                        // NationalityPerson
                        nationalityPerson,
                        // Part
                        part,
                        // Profession
                        profession,
                        // Speciality
                        speciality,
                        // DocumentProtocol
                        protocolName,
                        // DocumentDate
                        documentDate,
                        // AssessedGrade
                        assessedGradeName + " " + assessedGradeValue ?? "-",
                        // ChairmanOfPQC
                        chairmanOfPQC,
                        // OrderN
                        orderN,
                        // OrderDate
                        orderDate,
                        // PaperN
                        paperN,
                        // PaperDate
                        paperDate,
                        // OrderN1
                        orderN,
                        // OrderDateN1
                        orderDate,
                        // OrderN2
                        "-",
                        // OrderDateN2
                        "-",
                        // ResultDOC
                        "-"
                    };

                    document.MailMerge.Execute(fieldNames,
                        fieldValues.Select(s => string.IsNullOrEmpty(s) ? "-" : s).ToArray());
                    await CreateCompetenciesTable(document,40, 184f, 338f);
                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, FormatType.Docx);
                    blueprint.Close();
                    document.Close();
                    await FileUtils.SaveAs(JsRuntime,
                        BaseHelper.ConvertCyrToLatin("Удостоверение_" + this.ClientVM.FirstName + "_" +
                                                     this.ClientVM.FamilyName) + ".docx", stream.ToArray());
                }
            }
            catch
            {
                await this.ShowErrorAsync("Неуспешно генериране на документ!");
            }


        }
        public static int CalculateNumberOfLines(string text, float boxWidthInPoints, Font font)
        {
            var bitmap = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            bitmap.SetResolution(72, 72);



            using (var g = Graphics.FromImage(bitmap))
            {


                var stringFormat = StringFormat.GenericTypographic;
                stringFormat.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
                int charactersFitted;
                int linesFitted;
                g.MeasureString(text, font, new SizeF(boxWidthInPoints, 0), stringFormat, out charactersFitted,
                    out linesFitted);
                if (text.Trim().Length == 0)
                {
                    return linesFitted - 1;
                }
                return linesFitted;
            }

        }
        private async Task CreateCompetenciesTable(WordDocument document, int firstTableLines = 27, float competenciesDescriptionCellWidth = 229f, float competenciesNameCellWidth = 113f, bool FitToWindow = false)
        {
            BookmarksNavigator bookNav = new BookmarksNavigator(document);

            bookNav.MoveToBookmark("CompetenciesTable1", true, false);

            IWTable table1 = new WTable(document, true);

            if (FitToWindow)
            {
                table1.AutoFit(AutoFitType.FitToWindow);
            }
            else
            {
                table1.AutoFit(AutoFitType.FixedColumnWidth);
            }



            List<object> competenciesList = new List<object>();
            if (ClientVM.IdSpeciality != null && this.ClientVM.IdValidationClient != 0 && this.doc is not null)
            {
                if (this.doc.IsDOI)
                {
                    competenciesList.AddRange(this.ClientVM.ValidationCompetencies);
                }
                else
                {
                    var ERUs = (await this.DOCService.GetAllERUsByIdSpecialityAsync(ClientVM.IdSpeciality.Value)).ToList().Where(x => x.IdDOC == this.doc.IdDOC).ToList();
                    competenciesList.AddRange(ERUs);
                }
            }




            int num = 0;
            int cellPadding = 12;
            bool goToNewPage = false;
            float tableHeight = 0;
            bool isPractice = false;
            bool isSubListItem = false;
            // Define a local function to fill a row with data
            async Task FillRow(WTableRow row, object record)
            {
                if (record is ValidationCompetencyVM validationCompetency)
                {



                    string competencyNumber = validationCompetency.CompetencyNumber == null
                        ? "-"
                        : "Компетентност " + validationCompetency.CompetencyNumber;
                    string competencyDescription = validationCompetency.Competency == null
                        ? "-"
                        : validationCompetency.Competency;
                    string lines = string.Concat(
                            Enumerable.Repeat("\u000B-", BaseHelper.CalculateNumberOfLines(competencyDescription, competenciesDescriptionCellWidth - cellPadding, new Font("Calibri", 11f)) - 1));
                    var cell = row.AddCell();
                    cell.AddParagraph().AppendText(competencyNumber + lines);

                    cell.Width = competenciesNameCellWidth;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;

                    cell = row.AddCell();
                    cell.AddParagraph().AppendText(competencyDescription);
                    cell.Width = competenciesDescriptionCellWidth;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    tableHeight += BaseHelper.CalculateNumberOfLines(competencyDescription, cell.Width - cellPadding, new Font("Calibri", 11f));

                }
                else if (record is ERUVM eru)
                {

                    string eruCode = eru.Code == null
                        ? "-"
                        : eru.Code;
                    string eruName = eru.Name == null
                        ? "-"
                        : eru.Name;
                    string lines = string.Concat(
                        Enumerable.Repeat("\u000B-", BaseHelper.CalculateNumberOfLines(eruName, competenciesDescriptionCellWidth - cellPadding, new Font("Calibri", 11f)) - 1));
                    var cell = row.AddCell();
                    var paragraph = cell.AddParagraph().AppendText(eruCode + lines);

                    cell.Width = 54;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;


                    cell = row.AddCell();
                    cell.AddParagraph().AppendText(eruName);
                    cell.Width = competenciesDescriptionCellWidth;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    tableHeight += BaseHelper.CalculateNumberOfLines(eruName, cell.Width - cellPadding, new Font("Calibri", 11f));
                }
            }
            for (int i = 0; i < competenciesList.Count(); i++)
            {

                // Add a new row to the table
                var row = table1.AddRow(true, false);
                row.HeightType = TableRowHeightType.Exactly;

                // Fill the row with data
                await FillRow(row, competenciesList[i]);

                if (tableHeight > firstTableLines)
                {
                    num = i + 1;
                    goToNewPage = true;
                    break;
                }

                goToNewPage = false;

            }
            if (tableHeight < firstTableLines)
            {
                for (int i = 0; i < firstTableLines - tableHeight; i++)
                {
                    // Add a new row to the table
                    var row = table1.AddRow(true, false);

                    // Add new cells with "-" text
                    for (int j = 0; j < 2; j++)
                    {
                        var cell = row.AddCell();
                        cell.AddParagraph().AppendText("-");
                        cell.Width = j == 0 ? competenciesNameCellWidth : competenciesDescriptionCellWidth;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    }
                }
            }
            table1.TableFormat.Borders.BorderType = BorderStyle.Cleared;
            bookNav.InsertTable(table1);
            try
            {
                bookNav.MoveToBookmark("CompetenciesTable2", true, false);
                IWTable table2 = new WTable(document, true);
                tableHeight = 0;

                if (goToNewPage)
                {
                    for (int i = num; i < competenciesList.Count(); i++)
                    {
                        // Add a new row to the table
                        var row = table2.AddRow(true, false);
                        row.HeightType = TableRowHeightType.Exactly;

                        // Fill the row with data
                        await FillRow(row, competenciesList[i]);
                    }
                }

                for (int i = 0; i < 30 - tableHeight; i++)
                {
                    // Add a new row to the table
                    var row = table2.AddRow(true, false);

                    // Add new cells with "-" text
                    for (int j = 0; j < 2; j++)
                    {
                        var cell = row.AddCell();
                        cell.AddParagraph().AppendText("-");
                        cell.Width = j == 0 ? competenciesNameCellWidth : competenciesDescriptionCellWidth;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    }
                }

                table2.TableFormat.Borders.BorderType = BorderStyle.Cleared;

                bookNav.InsertTable(table2);
            }
            catch
            {
             

            }
        }

    }
}
