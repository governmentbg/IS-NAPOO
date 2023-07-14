using System.Drawing;
using Data.Models.Migrations;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC.NKPD;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
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
using Data.Models.Data.Training;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class ClientCourseIssueDuplicate : BlazorBaseComponent
    {
        private SfComboBox<int?, DocumentSerialNumberVM> docSerialNumbersComboBox = new SfComboBox<int?, DocumentSerialNumberVM>();
        private SubmissionCommentModal submissionCommentModal = new SubmissionCommentModal();
        private DocumentStatusModal documentStatusModal = new DocumentStatusModal();

        private ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM duplicateFinishedModel = new ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM();
        private IEnumerable<KeyValueVM> finishedTypeSource = new List<KeyValueVM>();
        private List<DocumentSerialNumberVM> documentSerialNumbersSource = new List<DocumentSerialNumberVM>();
        private ValidationMessageStore? messageStore;
        public bool isTabRendered = false;
        private IEnumerable<CourseProtocolVM> protocolsSource = new List<CourseProtocolVM>();
        private KeyValueVM kvFinishedWithDoc = new KeyValueVM();
        private KeyValueVM kvIssueOfDuplicate = new KeyValueVM();
        private IEnumerable<KeyValueVM> professionalTrainingTypesSource;
        private KeyValueVM kvDocumentStatusNotSubmitted = new KeyValueVM();
        private KeyValueVM kvDocumentStatusReturned = new KeyValueVM();
        private List<KeyValueVM> courseStatusSource = new List<KeyValueVM>();
        private KeyValueVM kvQualificationLevel = new KeyValueVM();
        private KeyValueVM kvEGN = new KeyValueVM();
        private KeyValueVM kvLNCh = new KeyValueVM();
        private KeyValueVM kvIDN = new KeyValueVM();
        private IEnumerable<KeyValueVM> kvSexSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvNationalitySource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvIndentTypeSource = new List<KeyValueVM>();
        private KeyValueVM kvCourseFinished = new KeyValueVM();
        private KeyValueVM kvPartProfessionValue = new KeyValueVM();
        private IEnumerable<CourseSubjectVM> courseSubjectSource;
        private KeyValueVM kvVocationalQualificationCertificateDuplicate;
        private KeyValueVM kvCertificateOfVocationalTraining;
        private ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM finishedModel;
        private List<DocumentSeriesVM> documentSeriesSource;
        private List<TrainingCurriculumVM> addedCurriculums;
        private IEnumerable<KeyValueVM> professionalTrainingsSource;
        private KeyValueVM kvSPK = new KeyValueVM();
        private PrintDocumentModalMessage printDocumentModalMessage = new PrintDocumentModalMessage();

        [Parameter]
        public ClientCourseVM ClientCourseVM { get; set; }

        [Parameter]
        public CourseVM CourseVM { get; set; }

        [Parameter]
        public bool EntryFromCourseGraduatesList { get; set; }

        [Parameter]
        public EventCallback<ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM> CallbackAfterEditContextValidation { get; set; }

        [Parameter]
        public bool IsEditEnabled { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }


        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public Microsoft.JSInterop.IJSRuntime JS { get; set; }

        [Inject]
        public IDOCService DocService { get; set; }

        [Inject]
        public INKPDService NKPDService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        public ITemplateDocumentService templateDocumentService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.duplicateFinishedModel);
            this.finishedTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseFinishedType");
            this.kvPartProfessionValue = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "PartProfession");
            this.kvIssueOfDuplicate = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type6");
            this.kvEGN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
            this.kvQualificationLevel = await this.DataSourceService.GetKeyValueByIntCodeAsync("QualificationLevel", "WithoutQualification_Update");
            this.kvLNCh = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "LNK");
            this.kvIDN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "IDN");
            this.kvSexSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");
            this.kvNationalitySource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality");
            this.kvFinishedWithDoc = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type1");
            this.kvIndentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            this.professionalTrainingTypesSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            this.FormTitle = "Издаване на дубликат";

            this.isTabRendered = true;



            this.kvDocumentStatusNotSubmitted = await this.DataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "NotSubmitted");
            this.kvDocumentStatusReturned = await this.DataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Returned");

            this.kvSPK = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification");

            await this.LoadProtocolsDataAsync();

            await this.LoadModelDataAsync();

            this.LoadDocumentSerialNumbersData();

            this.editContext.MarkAsUnmodified();

            await this.CallbackAfterEditContextValidation.InvokeAsync(this.duplicateFinishedModel);

        }

        private async void LoadDocumentSerialNumbersData()
        {
            if (this.duplicateFinishedModel.FinishedYear.HasValue)
            {
                if (this.duplicateFinishedModel.FinishedYear.ToString().Length == 4)
                {
                    CandidateProviderVM candidateProvider = new CandidateProviderVM() { IdCandidate_Provider = this.UserProps.IdCandidateProvider };
                    this.documentSerialNumbersSource = this.ProviderDocumentRequestService.GetAllDocumentSerialNumbersByIdCandidateProviderByStatusReceivedByYearAndByIdCourseType(candidateProvider, this.duplicateFinishedModel.FinishedYear.Value, this.CourseVM.IdTrainingCourseType.Value, true).OrderBy(x => x.SerialNumberAsIntForOrderBy).ToList();
                    if (this.duplicateFinishedModel.IdDocumentSerialNumber.HasValue)
                    {
                        if (!this.documentSerialNumbersSource.Any(x => x.IdDocumentSerialNumber == this.duplicateFinishedModel.IdDocumentSerialNumber.Value))
                        {
                            var docSerialNumber = await this.ProviderDocumentRequestService.GetDocumentSerialNumberByIdAndYearAsync(this.duplicateFinishedModel.IdDocumentSerialNumber.Value, this.duplicateFinishedModel.FinishedYear.Value);
                            if (docSerialNumber != null)
                            {
                                this.documentSerialNumbersSource.Add(docSerialNumber);
                            }

                            if (this.documentSerialNumbersSource.Any())
                            {
                                this.documentSerialNumbersSource = this.documentSerialNumbersSource.OrderBy(x => x.SerialNumberAsIntForOrderBy).ToList();

                                await this.docSerialNumbersComboBox.RefreshDataAsync();
                                this.StateHasChanged();
                            }
                        }
                    }
                }
            }
        }

        private void OnFinishedYearValueChanged()
        {
            if (this.duplicateFinishedModel.HasDocumentFabricNumber)
            {
                this.LoadDocumentSerialNumbersData();
            }
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            var file = args.Files[0].Stream;

            var result = await this.UploadFileService.UploadFileAsync<CourseDocumentUploadedFile>(file, args.Files[0].FileInfo.Name, "ClientCourseDocument", this.duplicateFinishedModel.IdCourseDocumentUploadedFile);
            if (!string.IsNullOrEmpty(result))
            {
                this.duplicateFinishedModel.UploadedFileName = result;
            }

            this.StateHasChanged();
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.duplicateFinishedModel.UploadedFileName))
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<CourseDocumentUploadedFile>(this.duplicateFinishedModel.IdCourseDocumentUploadedFile);
                    if (result == 1)
                    {
                        this.duplicateFinishedModel.UploadedFileName = null;
                    }

                    this.StateHasChanged();
                }
            }
        }

        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(this.duplicateFinishedModel.UploadedFileName))
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<CourseDocumentUploadedFile>(this.duplicateFinishedModel.IdCourseDocumentUploadedFile);
                    if (result == 1)
                    {
                        this.duplicateFinishedModel.UploadedFileName = null;
                    }

                    this.StateHasChanged();
                }
            }
        }

        private async Task OnDownloadClick()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!string.IsNullOrEmpty(this.duplicateFinishedModel.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseDocumentUploadedFile>(this.duplicateFinishedModel.IdCourseDocumentUploadedFile);
                    if (hasFile)
                    {
                        var document = await this.UploadFileService.GetUploadedFileAsync<CourseDocumentUploadedFile>(this.duplicateFinishedModel.IdCourseDocumentUploadedFile);
                        if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, this.duplicateFinishedModel.FileName, document.MS!.ToArray());
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
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void OnProtocolSelected(ChangeEventArgs<int?, CourseProtocolVM> args)
        {
            this.SpinnerShow();

            if (args.ItemData is not null)
            {
                this.duplicateFinishedModel.DocumentProtocol = args.ItemData.CourseProtocolNumber;
                this.duplicateFinishedModel.FinalResult = args.ItemData.CourseProtocolGrades.FirstOrDefault().Grade.ToString();
            }
            else
            {
                this.duplicateFinishedModel.DocumentProtocol = null;
                this.duplicateFinishedModel.FinalResult = string.Empty;
            }

            this.SpinnerHide();
        }

        public override async void SubmitHandler()
        {
            if (this.duplicateFinishedModel.IdFinishedType == kvIssueOfDuplicate.IdKeyValue)
            {
                //TODO:
            }

            this.editContext = new EditContext(this.duplicateFinishedModel);
            this.editContext.EnableDataAnnotationsValidation();
            this.messageStore = new ValidationMessageStore(this.editContext);
            //this.editContext.OnValidationRequested += this.ValidateFinishedDate;
            this.editContext.OnValidationRequested += this.ValidateDocumentDate;
            this.editContext.OnValidationRequested += this.ValidateRequiredFabricNumber;
            this.editContext.OnValidationRequested += this.ValidateProtocolSelected;
            this.editContext.OnValidationRequested += this.ValidateRequiredFields;

            this.editContext.Validate();

            await this.CallbackAfterEditContextValidation.InvokeAsync(this.duplicateFinishedModel);
        }

        public async Task LoadProtocolsDataAsync()
        {
            this.protocolsSource = await this.TrainingService.GetAll381BProtocolsWhichHaveGradeAddedByIdCourseAndByIdClientCourseAsync(this.CourseVM.IdCourse, this.ClientCourseVM.IdClientCourse);
            this.StateHasChanged();
        }

        private void ValidateDocumentDate(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.duplicateFinishedModel.IdFinishedType.HasValue && this.duplicateFinishedModel.IdFinishedType.Value == this.kvFinishedWithDoc.IdKeyValue)
            {
                if (this.CourseVM is not null)
                {
                    if (this.CourseVM.StartDate.HasValue)
                    {
                        if (this.duplicateFinishedModel.DocumentDate < this.CourseVM.StartDate)
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.duplicateFinishedModel, "FinishedDate");
                            this.messageStore?.Add(fi, $"Полето 'Дата на издаване' не може да бъде преди {this.CourseVM.StartDate.Value.ToString("dd.MM.yyyy")}г.!");
                            return;
                        }
                    }
                }
            }
        }

        private void ValidateFinishedDate(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.CourseVM is not null)
            {
                if (this.CourseVM.StartDate.HasValue)
                {
                    if (this.duplicateFinishedModel.FinishedDate < this.CourseVM.StartDate)
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.duplicateFinishedModel, "FinishedDate");
                        this.messageStore?.Add(fi, $"Полето 'Дата на завършване на курса' не може да бъде преди {this.CourseVM.StartDate.Value.ToString("dd.MM.yyyy")}г.!");
                        return;
                    }
                }

                if (this.CourseVM.EndDate.HasValue)
                {
                    if (this.duplicateFinishedModel.FinishedDate > this.CourseVM.EndDate)
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.duplicateFinishedModel, "FinishedDate");
                        this.messageStore?.Add(fi, $"Полето 'Дата на завършване на курса' не може да бъде след {this.CourseVM.EndDate.Value.ToString("dd.MM.yyyy")}г.!");
                    }
                }
            }
        }

        private void ValidateRequiredFabricNumber(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.duplicateFinishedModel.IdFinishedType.HasValue && this.duplicateFinishedModel.IdFinishedType.Value == this.kvFinishedWithDoc.IdKeyValue)
            {
                if (this.duplicateFinishedModel.HasDocumentFabricNumber && this.duplicateFinishedModel.IdDocumentSerialNumber is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.duplicateFinishedModel, "IdDocumentSerialNumber");
                    this.messageStore?.Add(fi, "Полето 'Фабричен номер на документа' е задължително!");
                }
            }
        }

        private void ValidateProtocolSelected(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.duplicateFinishedModel.IdFinishedType.HasValue && this.duplicateFinishedModel.IdFinishedType.Value == this.kvFinishedWithDoc.IdKeyValue)
            {
                if (this.duplicateFinishedModel.IdCourseProtocol is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.duplicateFinishedModel, "IdCourseProtocol");
                    this.messageStore?.Add(fi, "Полето 'Протокол' е задължително!");
                }
            }
        }

        private void ValidateRequiredFields(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.duplicateFinishedModel.IdFinishedType.HasValue && this.duplicateFinishedModel.IdFinishedType.Value == this.kvIssueOfDuplicate.IdKeyValue)
            {
                if (this.duplicateFinishedModel.IdDocumentType is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.duplicateFinishedModel, "IdDocumentType");
                    this.messageStore?.Add(fi, "Полето 'Вид на издадения документ' е задължително!");
                }

                if (this.duplicateFinishedModel.FinishedYear is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.duplicateFinishedModel, "FinishedYear");
                    this.messageStore?.Add(fi, "Полето 'Година на завършване' е задължително!");
                }

                if (string.IsNullOrEmpty(this.duplicateFinishedModel.DocumentRegNo))
                {
                    FieldIdentifier fi = new FieldIdentifier(this.duplicateFinishedModel, "DocumentRegNo");
                    this.messageStore?.Add(fi, "Полето 'Регистрационен номер' е задължително!");
                }

                if (this.duplicateFinishedModel.DocumentDate is null)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.duplicateFinishedModel, "DocumentDate");
                    this.messageStore?.Add(fi, "Полето 'Дата на издаване' е задължително!");
                }
            }
        }

        public async Task LoadModelDataAsync()
        {
            if (this.ClientCourseVM is not null)
            {
                this.duplicateFinishedModel = await this.TrainingService.GetClientCourseDuplicateClientCourseDocumentAndCourseDocumentUploadedFileCombinedByIdClientCourseAsync(this.ClientCourseVM.IdClientCourse);
                this.duplicateFinishedModel.IdFinishedType = this.finishedTypeSource.Where(x => x.Name == "Издаване на дубликат").FirstOrDefault()!.IdKeyValue;
                this.finishedModel = await this.TrainingService.GetClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedByIdClientCourseAsync(this.ClientCourseVM.IdClientCourse);

                this.StateHasChanged();
            }
        }

        public void SetEditContextAsUnmodified()
        {
            if (this.editContext is not null)
                this.editContext.MarkAsUnmodified();
        }

        public bool GetIsEditContextModified()
        {
            if (this.editContext is not null)
            {
                return this.editContext.IsModified();
            }

            return false;
        }

        private async Task OpenStatusHistoryBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.documentStatusModal.OpenModal(this.duplicateFinishedModel.IdClientCourseDocument, "Course");
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task FileInForVerificationBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да подадете избраните документи за проверка към НАПОО?");
                if (isConfirmed)
                {
      
                    this.submissionCommentModal.OpenModal(GlobalConstants.RIDPK_OPERATION_FILE_IN, new List<ClientCourseDocumentVM>() { new ClientCourseDocumentVM() { IdClientCourseDocument = this.duplicateFinishedModel.IdClientCourseDocument } });
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
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
                if (this.duplicateFinishedModel.DocumentTypeName.ToLower()
                    .Contains("свидетелство за професионална квалификация"))
                {
                    this.kvVocationalQualificationCertificateDuplicate =
                        await this.DataSourceService.GetKeyValueByIntCodeAsync(
                            "ProcedureDocumentType",
                            "VocationalQualificationCertificateDuplicate");
                    var templateDocuments =
                        (await this.templateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM()))
                        .Where(x =>
                            x.IdApplicationType == this.kvVocationalQualificationCertificateDuplicate.IdKeyValue
                        );
                    var templateDocument = templateDocuments.FirstOrDefault(x =>
                        this.duplicateFinishedModel.DocumentDate == null ||
                        this.duplicateFinishedModel.DocumentDate.Value >= x.DateFrom &&
                        this.duplicateFinishedModel.DocumentDate.Value <= x.DateTo);

                    var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
                    FileStream blueprint = new FileStream($@"{resources_Folder}{templateDocument?.TemplatePath}",
                        FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                    WordDocument document = new WordDocument(blueprint, FormatType.Docx);
                    LocationVM clientLocation = new LocationVM();
                    this.courseSubjectSource =
                        await this.TrainingService.GetAllCourseSubjectsByIdCourseAsync(this.CourseVM.IdCourse);
                    if (this.ClientCourseVM.IdCityOfBirth != null)
                    {
                        clientLocation =
                            await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(
                                this.ClientCourseVM.IdCityOfBirth.Value);
                    }

                    string[] fieldNames = new string[]
                    {
                        "InstitutionName1", "Kati", "MunicipalityName", "Region", "DistrictName",
                        "InstitutionNameDuplicate", "KatiDuplicate", "MunicipalityNameDuplicate", "RegionDuplicate",
                        "DistrictNameDuplicate", "NKR", "EKR", "Director", "RegNum", "Date", "RegNumD", "DateD",
                        "SN", "DocumentNum", "PersonName", "EGN", "Sex", "PersonCity", "MunicipalityPerson",
                        "DistrictPerson", "FID", "OtherIdent", "NationalityPerson", "Year", "InstitutionName2",
                        "FormOfТraining", "Length", "Indicator", "SPK", "Profession", "Speciality", "ChairmanOfPQC",
                        "AssessedSubject", "AssessedGradeValue", "AssessedGradeName", "Proxy"
                    };

                    var protocols =
                        await this.TrainingService
                            .GetAll381BProtocolsWhichHaveGradeAddedByIdCourseAndByIdClientCourseAsync(
                                this.CourseVM.IdCourse, this.ClientCourseVM.IdClientCourse);
                    CourseCommissionMemberVM commissionMember = new CourseCommissionMemberVM();
                    string[] protocolNameAndDate = new string[2];
                    if (protocols.Any())
                    {
                        commissionMember = this.CourseVM.CourseCommissionMembers.FirstOrDefault(commissionMember =>
                            commissionMember.IdCourseCommissionMember ==
                            protocols.FirstOrDefault().IdCourseCommissionMember);
                    }

                    // Create variables for each record and assign values
                    string institutionName1 = "Център за професионално обучение ";

                    if (this.CourseVM.CandidateProvider != null)
                    {
                        if (string.IsNullOrEmpty(this.CourseVM.CandidateProvider.ProviderName))
                        {
                            institutionName1 += "към ";
                        }
                        else
                        {
                            if (this.CourseVM.CandidateProvider.ProviderName.StartsWith("ЦПО към") ||
                                this.CourseVM.CandidateProvider.ProviderName.StartsWith(
                                    "Център за професионално обучение към "))
                            {
                                institutionName1 =
                                    this.CourseVM.CandidateProvider.ProviderName.Replace("ЦПО към",
                                        "Център за професионално обучение към ");
                            }

                            institutionName1 += this.CourseVM.CandidateProvider.ProviderName + " към ";
                        }

                        if (this.CourseVM.CandidateProvider.ProviderOwner != "" ||
                            this.CourseVM.CandidateProvider.ProviderOwner != null)
                        {
                            institutionName1 += this.CourseVM.CandidateProvider.ProviderOwner;
                        }
                    }

                    string separator = string.Concat(Enumerable.Repeat("\n-",
                        3 - BaseHelper.CalculateNumberOfLines(
                            "                                                " + institutionName1, 348.6f,
                            new Font("Calibri", 11f, FontStyle.Regular))));
                    string institutionName2 = institutionName1 + separator;
                    string institutionName3 = institutionName1 + string.Concat(Enumerable.Repeat("\n-",
                        2 - BaseHelper.CalculateNumberOfLines(institutionName1.Trim(), 328f,
                            new Font("Calibri", 12f, FontStyle.Bold))));
                    institutionName1 += string.Concat(Enumerable.Repeat("\n-",
                        3 - BaseHelper.CalculateNumberOfLines(institutionName1.Trim(), 328f,
                            new Font("Calibri", 12f, FontStyle.Bold))));


                    string kati = this.CourseVM.CandidateProvider?.Location?.kati ??
                                  "-" + "\n                                        -";
                    string municipalityName =
                        this.CourseVM.CandidateProvider?.Location?.Municipality?.MunicipalityName ?? "-";
                    string region = this.CourseVM.CandidateProvider?.Location?.Municipality?.Regions
                        ?.FirstOrDefault(region =>
                            region.idMunicipality ==
                            this.CourseVM.CandidateProvider.Location.Municipality.idMunicipality)?.RegionName ?? "-";
                    string districtName =
                        this.CourseVM.CandidateProvider?.Location?.Municipality?.District?.DistrictName ?? "-";
                    string nkr = this.CourseVM.Program.Speciality.IdNKRLevel == 0
                        ? "-"
                        : (await this.DataSourceService.GetKeyValueByIdAsync(
                            this.CourseVM.Program.Speciality.IdNKRLevel))?.Name ?? "-";
                    string ekr = this.CourseVM.Program.Speciality.IdEKRLevel == 0
                        ? "-"
                        : (await this.DataSourceService.GetKeyValueByIdAsync(
                            this.CourseVM.Program.Speciality.IdEKRLevel))?.Name ?? "-";
                    string director = string.IsNullOrEmpty(this.CourseVM.CandidateProvider?.DirectorFullName)
                        ? "-"
                        : this.CourseVM.CandidateProvider?.DirectorFirstName + " " +
                          this.CourseVM.CandidateProvider?.DirectorFamilyName;
                    string regNumD = string.IsNullOrEmpty(this.duplicateFinishedModel.DocumentRegNo)
                        ? "-"
                        : this.duplicateFinishedModel.DocumentRegNo;
                    string regNum = string.IsNullOrEmpty(this.finishedModel.DocumentRegNo)
                        ? "-"
                        : this.finishedModel.DocumentRegNo;
                    string dateD = this.duplicateFinishedModel.DocumentDate == null
                        ? "-"
                        : this.duplicateFinishedModel.DocumentDate.Value.ToString("dd.MM.yyyy");
                    string date = this.finishedModel.DocumentDate == null
                        ? "-"
                        : this.finishedModel.DocumentDate.Value.ToString("dd.MM.yyyy");
                    string personName =
                        $"{this.ClientCourseVM.FirstName} {this.ClientCourseVM.SecondName} {this.ClientCourseVM.FamilyName}";
                    string sex = this.ClientCourseVM.IdSex ==
                                 this.kvSexSource.FirstOrDefault(x => x.Name == "Мъж")?.IdKeyValue
                        ? "-"
                        : "a";
                    string personCity = clientLocation?.kati ?? "-";
                    string municipalityPerson = clientLocation?.Municipality?.MunicipalityName ?? "-";
                    string districtPerson = clientLocation?.Municipality?.District?.DistrictName ?? "-";
                    string egn = "-";
                    string lnch = "-";
                    string idn = "-";
                    string indent = this.ClientCourseVM.Indent ?? "-";
                    if (this.ClientCourseVM.IdIndentType == this.kvEGN.IdKeyValue)
                    {
                        egn = indent;
                    }
                    else if (this.ClientCourseVM.IdIndentType == this.kvLNCh.IdKeyValue)
                    {

                        lnch = indent;

                    }
                    else if (this.ClientCourseVM.IdIndentType == this.kvIDN.IdKeyValue)
                    {
                        idn = indent;
                    }

                    string nationalityPerson = this.ClientCourseVM.IdCountryOfBirth != null
                        ? kvNationalitySource.FirstOrDefault(x => x.IdKeyValue == this.ClientCourseVM.IdCountryOfBirth)
                            ?.Name ?? "-"
                        : "-";
                    string year = this.duplicateFinishedModel.DocumentDate == null
                        ? "-"
                        : this.duplicateFinishedModel.DocumentDate.Value.ToString("yyyy");
                    string formOfTraining = this.CourseVM.FormEducation?.Name?.ToLower() ?? "-";
                    string length = this.CourseVM.StartDate.HasValue && this.CourseVM.EndDate.HasValue
                        ? BaseHelper.GetTotalMonths(this.CourseVM.StartDate.Value, this.CourseVM.EndDate.Value)
                        : "-";
                    string indicator =
                        (await this.TrainingService
                            .GetAll381BProtocolsWhichHaveGradeAddedByIdCourseAndByIdClientCourseAsync(
                                this.CourseVM.IdCourse, this.ClientCourseVM.IdClientCourse)).Any()
                            ? (await this.TrainingService
                                .GetAll381BProtocolsWhichHaveGradeAddedByIdCourseAndByIdClientCourseAsync(
                                    this.CourseVM.IdCourse, this.ClientCourseVM.IdClientCourse)).FirstOrDefault()
                            ?.NameAndDate?.Remove(0, 1)?.Replace("г.", "") ?? "-"
                            : "-";
                    string spk =
                        (await this.DataSourceService.GetKeyValueByIdAsync(this.CourseVM.Program.Speciality.IdVQS))
                        ?.DefaultValue1 ?? "-";
                    string profession =
                        (this.CourseVM.Program == null || this.CourseVM.Program.Speciality == null ||
                         this.CourseVM.Program.Speciality.Profession == null)
                            ? "-"
                            : this.CourseVM.Program.Speciality.Profession.Name + string.Concat(Enumerable.Repeat("\n-",
                                2 - BaseHelper.CalculateNumberOfLines(
                                    "                  " +
                                    ((this.CourseVM.Program == null || this.CourseVM.Program.Speciality == null ||
                                      this.CourseVM.Program.Speciality.Profession == null)
                                        ? "-"
                                        : this.CourseVM.Program.Speciality.Profession.Name), 343f,
                                    new Font("Calibri", 11f, FontStyle.Regular))));
                    string speciality = (this.CourseVM.Program == null || this.CourseVM.Program.Speciality == null)
                        ? "-"
                        : this.CourseVM.Program.Speciality.Name + string.Concat(Enumerable.Repeat("\n-",
                            2 - BaseHelper.CalculateNumberOfLines(
                                "                   " +
                                ((this.CourseVM.Program == null || this.CourseVM.Program.Speciality == null)
                                    ? "-"
                                    : this.CourseVM.Program.Speciality.Name), 343f,
                                new Font("Calibri", 11f, FontStyle.Regular))));
                    string chairmanOfPQC =
                        this.CourseVM.CourseCommissionMembers.Any() && protocols.Any() && commissionMember != null
                            ? commissionMember.FullName
                            : "-";
                    string assessedSubject = "Теория и практика";
                    string assessedGradeValue = !string.IsNullOrEmpty(this.duplicateFinishedModel.FinalResult)
                        ? Double.Parse(this.duplicateFinishedModel.FinalResult)
                            .ToString("f2", System.Globalization.CultureInfo.InvariantCulture)
                        : "-";
                    string assessedGradeName = !string.IsNullOrEmpty(this.duplicateFinishedModel.FinalResult)
                        ? BaseHelper.GetGradeName(Convert.ToDouble(this.duplicateFinishedModel.FinalResult))
                        : "-";
                    string proxy = "-";

                    string vocationalCertificateSeriesNumber =
                        this.finishedModel.DocumentSerialNumber?.SerialNumber ?? "-";
                    this.documentSeriesSource =
                        (await this.ProviderDocumentRequestService.GetAllDocumentSeriesAsync()).ToList();
                    string vocationalCertificateSeries = this.documentSeriesSource.FirstOrDefault(x =>
                                                             x.IdTypeOfRequestedDocument == this.finishedModel
                                                                 .IdDocumentType)?.SeriesName ??
                                                         "-";
                    string?[] fieldValues = new string[]
                    {
                        //InstitutionName1
                        institutionName3,
                        //Kati
                        kati,
                        //MunicipalityName
                        municipalityName,
                        //Region
                        region,
                        //DistrictName
                        districtName,
                        //InstitutionNameDuplicate
                        institutionName1,
                        //KatiDuplicate
                        kati,
                        //MunicipalityNameDuplicate
                        municipalityName,
                        //RegionDuplicate
                        region,
                        //DistrictNameDuplicate
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
                        // RegNumD
                        regNumD,
                        // DateD
                        dateD,
                        // SN
                        vocationalCertificateSeries,
                        // DocumentNum
                        vocationalCertificateSeriesNumber,
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
                        // Year
                        year,
                        //InstitutionName2
                        institutionName2,
                        // FormOfТraining
                        formOfTraining,
                        // Length
                        length,
                        // Indicator
                        indicator,
                        // SPK
                        spk,
                        // Profession
                        profession,
                        // Speciality
                        speciality,
                        // ChairmanOfPQC
                        chairmanOfPQC,
                        // AssessedSubject
                        assessedSubject,
                        // AssessedGradeValue
                        assessedGradeValue,
                        // AssessedGradeName
                        assessedGradeName,
                        // Proxy
                        proxy
                    };

                    document.MailMerge.Execute(fieldNames,
                        fieldValues.Select(s => string.IsNullOrEmpty(s) ? "-" : s).ToArray());

                    await CreateGradeListTable(document, 34, templateDocument.DateFrom.Value.CompareTo(new DateTime(2023,5,1)) == 0 ? 30 : 32);

                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, FormatType.Docx);
                    blueprint.Close();
                    document.Close();
                    await FileUtils.SaveAs(JS,
                        BaseHelper.ConvertCyrToLatin("Свидетелство_" + this.ClientCourseVM.FirstName + "_" +
                                                     this.ClientCourseVM.FamilyName) + ".docx", stream.ToArray());


                }
                else
                {

                    this.kvCertificateOfVocationalTraining = await this.DataSourceService.GetKeyValueByIntCodeAsync(
                        "ProcedureDocumentType",
                        "CertificateOfVocationalTraining");
                    var templateDocuments =
                        (await this.templateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM()))
                        .Where(x => x.IdApplicationType == kvCertificateOfVocationalTraining.IdKeyValue);
                    var templateDocument = templateDocuments.FirstOrDefault(x =>
                        this.duplicateFinishedModel.DocumentDate == null || this.duplicateFinishedModel.DocumentDate.Value >= x.DateFrom &&
                        this.duplicateFinishedModel.DocumentDate.Value <= x.DateTo);

                    var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
                    FileStream blueprint = new FileStream($@"{resources_Folder}{templateDocument?.TemplatePath}",
                        FileMode.Open, FileAccess.Read, FileShare.ReadWrite);


                    WordDocument document = new WordDocument(blueprint, FormatType.Docx);
                    LocationVM clientLocation = new LocationVM();
                    this.courseSubjectSource =
                        await this.TrainingService.GetAllCourseSubjectsByIdCourseAsync(this.CourseVM.IdCourse);
                    if (this.ClientCourseVM.IdCityOfBirth != null)
                    {
                        clientLocation =
                            await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(
                                this.ClientCourseVM.IdCityOfBirth.Value);
                    }

                    var protocols =
                        await this.TrainingService
                            .GetAll381BProtocolsWhichHaveGradeAddedByIdCourseAndByIdClientCourseAsync(
                                this.CourseVM.IdCourse, this.ClientCourseVM.IdClientCourse);
                    CourseCommissionMemberVM courseCommissionMember = new CourseCommissionMemberVM();
                    string[] protocolNameAndDate = new string[2];
                    IEnumerable<CourseProtocolVM> courseProtocolVms =
                        protocols as CourseProtocolVM[] ?? protocols.ToArray();
                    if (courseProtocolVms.Any())
                    {
                        courseCommissionMember = this.CourseVM.CourseCommissionMembers.FirstOrDefault(
                            commissionMember => commissionMember.IdCourseCommissionMember ==
                                                courseProtocolVms.FirstOrDefault().IdCourseCommissionMember);

                        var firstProtocol = courseProtocolVms.FirstOrDefault();
                        var nameAndDate = firstProtocol?.NameAndDate;
                        protocolNameAndDate = nameAndDate.Remove(0, 1).Replace("г.", "").Split("/");

                    }

                    string[] fieldNames = new string[]
                    {
                        "IsDuplicate", "InstitutionName1", "Kati", "MunicipalityName", "Region", "DistrictName",
                        "RegistrationNumber", "Date", "PersonName", "EGN", "Sex", "PersonCity",
                        "MunicipalityPerson", "DistrictPerson", "FID", "OtherIdent", "NationalityPerson", "Year",
                        "Program", "QualificationLevel", "CourseName", "Profession", "Speciality",
                        "InstitutionName2", "FormOfТraining", "Length", "DocumentProtocol", "DocumentDate",
                        "AssessedGrade", "ChairmanОfТheExaminationBoard", "Director", "NKR", "EKR"
                    };


                    this.addedCurriculums =
                        (await this.TrainingService.GetTrainingCurriculumByIdCourseAsync(this.CourseVM.IdCourse))
                        .ToList();
                    this.professionalTrainingsSource =
                        await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");


                    string institutionName1 = "Център за професионално обучение ";
                    if (this.CourseVM.CandidateProvider != null)
                    {
                        if (string.IsNullOrEmpty(this.CourseVM.CandidateProvider.ProviderName))
                        {
                            institutionName1 += "към ";
                        }
                        else
                        {
                            if (this.CourseVM.CandidateProvider.ProviderName.StartsWith("ЦПО към") ||
                                this.CourseVM.CandidateProvider.ProviderName.StartsWith(
                                    "Център за професионално обучение към "))
                            {
                                institutionName1 =
                                    this.CourseVM.CandidateProvider.ProviderName.Replace("ЦПО към",
                                        "Център за професионално обучение към ");
                            }

                            institutionName1 += this.CourseVM.CandidateProvider.ProviderName + " към ";
                        }

                        if (this.CourseVM.CandidateProvider.ProviderOwner != "" ||
                            this.CourseVM.CandidateProvider.ProviderOwner != null)
                        {
                            institutionName1 += this.CourseVM.CandidateProvider.ProviderOwner;
                        }
                    }

                    string institutionName2 = institutionName1;
                    institutionName1 += string.Concat(Enumerable.Repeat("\n-",
                        2 - BaseHelper.CalculateNumberOfLines(institutionName1.Trim(), 415.4f,
                            new Font("Calibri", 11f, FontStyle.Bold))));
                    institutionName2 += string.Concat(Enumerable.Repeat("\n-",
                        3 - BaseHelper.CalculateNumberOfLines("               " + institutionName2.Trim(), 415.3f,
                            new Font("Calibri", 11f))));

                    string duplicate = "ДУБЛИКАТ";
                    string kati = this.CourseVM.CandidateProvider?.Location?.kati ?? "-";
                    string municipalityName =
                        this.CourseVM.CandidateProvider?.Location?.Municipality?.MunicipalityName ?? "-";
                    string regionName = this.CourseVM.CandidateProvider?.Location?.Municipality?.Regions?.Any() == true
                        ? this.CourseVM.CandidateProvider?.Location?.Municipality?.Regions?.FirstOrDefault(region =>
                            region.idMunicipality ==
                            this.CourseVM.CandidateProvider.Location.Municipality.idMunicipality)?.RegionName ?? "-"
                        : "-";
                    string districtName =
                        this.CourseVM.CandidateProvider?.Location?.Municipality?.District?.DistrictName ?? "-";
                    string documentRegNo =
                        string.IsNullOrEmpty(this.duplicateFinishedModel.DocumentRegNo) ? "-" : this.duplicateFinishedModel.DocumentRegNo;
                    string documentDate = this.duplicateFinishedModel.DocumentDate == null
                        ? "-"
                        : this.duplicateFinishedModel.DocumentDate.Value.ToString("dd.MM.yyyy");
                    string clientName =
                        $"{this.ClientCourseVM.FirstName} {this.ClientCourseVM.SecondName} {this.ClientCourseVM.FamilyName}";
                    string egn = "-";
                    string lnch = "-";
                    string idn = "-";
                    string indent = this.ClientCourseVM.Indent ?? "-";
                    if (this.ClientCourseVM.IdIndentType == this.kvEGN.IdKeyValue)
                    {
                        egn = indent;
                    }
                    else if (this.ClientCourseVM.IdIndentType == this.kvLNCh.IdKeyValue)
                    {

                        lnch = indent;

                    }
                    else if (this.ClientCourseVM.IdIndentType == this.kvIDN.IdKeyValue)
                    {
                        idn = indent;
                    }

                    string sex = this.ClientCourseVM.IdSex ==
                                 this.kvSexSource.FirstOrDefault(x => x.Name == "Мъж")?.IdKeyValue
                        ? "-"
                        : "a";
                    string clientLocationKati = clientLocation?.kati ?? "-";
                    string clientMunicipality = clientLocation?.Municipality == null || clientLocation?.kati == null
                        ? "-"
                        : clientLocation.Municipality.MunicipalityName;
                    string clientDistrictName = clientLocation?.Municipality?.District == null
                        ? "-"
                        : clientLocation.Municipality.District.DistrictName;
                    string countryOfBirth = this.ClientCourseVM.IdCountryOfBirth != null
                        ? kvNationalitySource.FirstOrDefault(x => x.IdKeyValue == this.ClientCourseVM.IdCountryOfBirth)
                            ?.Name ?? "-"
                        : "-";
                    string documentYear = this.duplicateFinishedModel.DocumentDate == null
                        ? "-"
                        : this.duplicateFinishedModel.DocumentDate.Value.ToString("yyyy");
                    string frameworkProgramName = this.CourseVM.Program.FrameworkProgram?.Name ?? "-";
                    string qualificationLevel =
                        this.CourseVM.Program.FrameworkProgram?.IdQualificationLevel ==
                        this.kvQualificationLevel.IdKeyValue
                            ? "актуализиране, разширяване на професионалната квалификация"
                            : "част от професия";
                    string courseName = this.CourseVM.CourseName + string.Concat(Enumerable.Repeat("\n-",
                        2 - BaseHelper.CalculateNumberOfLines(this.CourseVM.CourseName.Trim(), 430.4f,
                            new Font("Calibri", 11f))));
                    string professionName = this.CourseVM.Program.Speciality?.Profession?.Name ?? "-";
                    string specialityName = this.CourseVM.Program.Speciality?.Name ?? "-";
                    string formEducationName = this.CourseVM.FormEducation?.Name?.ToLower() ?? "-";
                    string mandatoryHours =
                        this.CourseVM.MandatoryHours.HasValue && this.CourseVM.SelectableHours.HasValue
                            ? (this.CourseVM.MandatoryHours + this.CourseVM.SelectableHours) + ""
                            : "-";
                    string protocolName = protocolNameAndDate?[0] + "/";
                    string documentDate2 = protocolNameAndDate?[1] ?? "-";
                    string assessedGrade = !string.IsNullOrEmpty(this.duplicateFinishedModel.FinalResult)
                        ? BaseHelper.GetGradeName(Convert.ToDouble(this.duplicateFinishedModel.FinalResult)) + " " + Double
                            .Parse(this.duplicateFinishedModel.FinalResult)
                            .ToString("f2", System.Globalization.CultureInfo.InvariantCulture)
                        : "-";
                    string commissionMember =
                        this.CourseVM.CourseCommissionMembers.Any() && courseProtocolVms.Any() &&
                        courseCommissionMember != null
                            ? courseCommissionMember.FullName
                            : "-";
                    string directorFullName = this.CourseVM.CandidateProvider?.DirectorFullName == null
                        ? "-"
                        : this.CourseVM.CandidateProvider.DirectorFirstName + " " +
                          this.CourseVM.CandidateProvider.DirectorFamilyName;
                    string NKRLevel = this.CourseVM.Program.Speciality?.IdNKRLevel == 0
                        ? "-"
                        : (await this.DataSourceService.GetKeyValueByIdAsync(
                            this.CourseVM.Program.Speciality.IdNKRLevel))?.Name ?? "-";
                    string EKRLevel =
                        (await this.DataSourceService.GetKeyValueByIdAsync(this.CourseVM.Program.Speciality.IdEKRLevel))
                        ?.Name ?? "-";

                    string[] fieldValues = new string[]
                    {
                        // IsDuplicate
                        duplicate,
                        // InstitutionName1
                        institutionName1,
                        // Kati
                        kati,
                        // MunicipalityName
                        municipalityName,
                        // Region
                        regionName,
                        // DistrictName
                        districtName,
                        // RegistrationNumber
                        documentRegNo,
                        // Date
                        documentDate,
                        // PersonName
                        clientName,
                        // EGN
                        egn,
                        // Sex
                        sex,
                        // PersonCity
                        clientLocationKati,
                        // MunicipalityPerson
                        clientMunicipality,
                        // DistrictPerson
                        clientDistrictName,
                        // FID
                        lnch,
                        // OtherIdent
                        idn,
                        // NationalityPerson
                        countryOfBirth,
                        // Year
                        documentYear,
                        // Program
                        frameworkProgramName,
                        // QualificationLevel
                        qualificationLevel,
                        // CourseName
                        courseName,
                        // Profession
                        professionName,
                        // Speciality
                        specialityName,
                        // InstitutionName2
                        institutionName2,
                        // FormOfТraining
                        formEducationName.ToLower(),
                        // Length
                        mandatoryHours,
                        // DocumentProtocol
                        protocolName,
                        // DocumentDate
                        documentDate2,
                        // AssessedGrade
                        assessedGrade,
                        // ChairmanОfТheExaminationBoard
                        commissionMember,
                        // Director 
                        directorFullName,
                        // NKR 
                        NKRLevel,
                        // EKR 
                        EKRLevel
                    };


                    await CreateSubjectListTable(document);

                    fieldValues = fieldValues.Select(s => string.IsNullOrEmpty(s) ? "-" : s).ToArray();
                    fieldValues[0] = "";
                    document.MailMerge.Execute(fieldNames, fieldValues);

                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, FormatType.Docx);
                    blueprint.Close();
                    document.Close();
                    await FileUtils.SaveAs(JS,
                        BaseHelper.ConvertCyrToLatin("Удостоверение_" + this.ClientCourseVM.FirstName + "_" +
                                                     this.ClientCourseVM.FamilyName) + ".docx", stream.ToArray());
                }
            }
            catch
            {
                await this.ShowErrorAsync("Неуспешно генериране на документ!");
            }

          
        }
        private async Task CreateGradeListTable(WordDocument document, int firstTableLines, int secondTableLines, float subjectCellWidth = 210f, bool FitToWindow = false)
        {
            Font font = new Font("Calibri", 9f);
            Font fontBold = new Font("Calibri", 9f, FontStyle.Bold);

            BookmarksNavigator bookNav = new BookmarksNavigator(document);

            bookNav.MoveToBookmark("Grades1Table", true, false);

            IWTable table1 = new WTable(document, true);

            if (FitToWindow)
            {
                table1.AutoFit(AutoFitType.FitToWindow);
            }
            else
            {
                table1.AutoFit(AutoFitType.FixedColumnWidth);
            }

            var idKeyValueB = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "B").FirstOrDefault().IdKeyValue;
            var idKeyValueA1 = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "A1").FirstOrDefault().IdKeyValue;
            var idKeyValueA2 = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "A2").FirstOrDefault().IdKeyValue;
            var idKeyValueA3 = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "A3").FirstOrDefault().IdKeyValue;

            var trainingCurriculum = await this.TrainingService.GetTrainingCurriculumByIdCourseAsync(CourseVM.IdCourse);

            List<object> gradesList = new List<object>();

            gradesList.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA1).Where(x => x.TheoryHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));
            gradesList.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA2).Where(x => x.TheoryHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

            gradesList.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA3).Where(x => x.TheoryHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));


            if (courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA2).Any() || courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA3).Any())
            {
                gradesList.Add("Учебна практика по:");
                gradesList.AddRange(courseSubjectSource
                    .Where(x => x.IdProfessionalTraining == idKeyValueA2)
                    .Where(x => x.PracticeHours != 0).Where(x => !x.Subject.ToLower().Contains("производствена практика")).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

                gradesList.AddRange(courseSubjectSource
                    .Where(x => x.IdProfessionalTraining == idKeyValueA3)
                    .Where(x => x.PracticeHours != 0).Where(x => !x.Subject.ToLower().Contains("производствена практика")).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

                gradesList.AddRange(courseSubjectSource
                    .Where(x => x.IdProfessionalTraining == idKeyValueA2)
                    .Where(x => x.PracticeHours != 0).Where(x => x.Subject.ToLower().Contains("производствена практика")).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

                gradesList.AddRange(courseSubjectSource
                    .Where(x => x.IdProfessionalTraining == idKeyValueA3)
                    .Where(x => x.PracticeHours != 0).Where(x => x.Subject.ToLower().Contains("производствена практика")).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

            }

            if (courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueB).Any())
            {
                gradesList.Add("Разширена професионална подготовка:");
                gradesList.Add("Теория:");
                gradesList.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueB && x.TheoryHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));
                gradesList.Add("Практика:");
                gradesList.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueB && x.PracticeHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

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
                if (record is CourseSubjectVM courseSubject)
                {
                    // Get the course subject grade
                    var courseSubjectGrade =
                        await this.TrainingService
                            .GetClientCourseSubjectGradeByClientCourseIdAndByIdCourseSubjectAsync(
                                this.ClientCourseVM.IdClientCourse,
                                courseSubject.IdCourseSubject);

                    // Get the subject text
                    string subject = courseSubject.Subject == null
                        ? "-"
                        : (isSubListItem && !courseSubject.Subject.ToLower().Contains("производствена практика") ? "– " : "") + courseSubject.Subject;


                    // Create the string with "-" for empty lines
                    string fillEmptyLines = string.Concat(
                            Enumerable.Repeat("-\n", BaseHelper.CalculateNumberOfLines(subject == null
                                ? "-"
                                : subject, subjectCellWidth - cellPadding, font) - 1));

                    // Add a new cell for the subject
                    var cell = row.AddCell();
                    cell.AddParagraph().AppendText(subject);
                    cell.Width = subjectCellWidth;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    tableHeight += BaseHelper.CalculateNumberOfLines(subject == null
                        ? "-"
                        : subject, cell.Width - cellPadding, font);

                    // Add a new cell for the grade name
                    cell = row.AddCell();
                    string gradeText = "-";
                    if (courseSubjectGrade != null)
                    {
                        var grade = !isPractice ? courseSubjectGrade.TheoryGrade : courseSubjectGrade.PracticeGrade;
                        if (grade != null)
                        {
                            gradeText = BaseHelper.GetGradeName(grade.Value);
                        }
                    }
                    cell.AddParagraph().AppendText(fillEmptyLines + gradeText);
                    cell.Width = 78;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;

                    // Add a new cell for the grade value
                    cell = row.AddCell();
                    string gradeValueText = "-";
                    if (courseSubjectGrade != null)
                    {
                        var gradeValue = !isPractice ? courseSubjectGrade.TheoryGrade : courseSubjectGrade.PracticeGrade;
                        if (gradeValue != null)
                        {
                            gradeValueText = gradeValue.Value.ToString("f2", System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    cell.AddParagraph().AppendText(fillEmptyLines + gradeValueText);
                    cell.Width = 36;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;

                    // Add a new cell for the hours

                    cell = row.AddCell();
                    double hours = !isPractice ? courseSubject.TheoryHours : courseSubject.PracticeHours;
                    string hoursText = hours != 0 ? hours + "" : "-";
                    IWParagraph cellHours = cell.AddParagraph();
                    cell.Width = 36;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    cellHours.AppendText(fillEmptyLines + hoursText);
                    cellHours.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                }
                else if (record is string category)
                {

                    if (category.Equals("Учебна практика по:"))
                    {
                        isPractice = true;
                        isSubListItem = true;
                    }
                    else if (category.Equals("Теория:"))
                    {
                        isPractice = false;
                        isSubListItem = true;
                    }
                    else if (category.Equals("Практика:"))
                    {
                        isPractice = true;
                        isSubListItem = true;
                    }
                    else
                    {
                        isSubListItem = false;
                    }

                    if (!(category.Equals("Теория:") || category.Equals("Практика:")))
                    {
                        // Create the string with "-" for empty lines

                        string fillEmptyLines = string.Concat(
                            Enumerable.Repeat("-\n",
                                BaseHelper.CalculateNumberOfLines(category, subjectCellWidth - cellPadding,
                                    fontBold) - 1));

                        // Add a new cell for the category
                        var cell = row.AddCell();
                        cell.AddParagraph().AppendText(category);
                        cell.Width = subjectCellWidth;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                        tableHeight += BaseHelper.CalculateNumberOfLines(category, cell.Width - cellPadding,
                            fontBold);

                        // Add new cells with "-" text
                        for (int j = 0; j < 3; j++)
                        {
                            cell = row.AddCell();
                            IWParagraph cellParagraph = cell.AddParagraph();
                            cellParagraph.AppendText("-" + fillEmptyLines);
                            if (j == 2)
                            {
                                cellParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                            }
                            cell.Width = j == 0 ? 78 : 36;
                            cell.CellFormat.TextWrap = true;
                            cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;

                        }
                    }

                }
            }

            for (int i = 0; i < gradesList.Count(); i++)
            {

                // Add a new row to the table
                var row = table1.AddRow(true, false);
                row.HeightType = TableRowHeightType.Exactly;

                // Fill the row with data
                await FillRow(row, gradesList[i]);


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
                    for (int j = 0; j < 4; j++)
                    {
                        var cell = row.AddCell();
                        IWParagraph cellParagraph = cell.AddParagraph();
                        cellParagraph.AppendText("-");
                        cell.Width = j == 1 ? 78 : j == 0 ? subjectCellWidth : 36;
                        if (j == 3)
                        {
                            cellParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        }
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    }
                }
            }

            table1.TableFormat.Borders.BorderType = BorderStyle.Cleared;
            bookNav.InsertTable(table1);

            bookNav.MoveToBookmark("Grades2Table", true, false);
            IWTable table2 = new WTable(document, true);
            tableHeight = 0;

            if (goToNewPage)
            {
                for (int i = num; i < gradesList.Count(); i++)
                {
                    // Add a new row to the table
                    var row = table2.AddRow(true, false);
                    row.HeightType = TableRowHeightType.Exactly;

                    // Fill the row with data
                    await FillRow(row, gradesList[i]);
                }
            }

            for (int i = 0; i < secondTableLines - tableHeight; i++)
            {
                // Add a new row to the table
                var row = table2.AddRow(true, false);

                // Add new cells with "-" text
                for (int j = 0; j < 4; j++)
                {
                    var cell = row.AddCell();
                    IWParagraph cellParagraph = cell.AddParagraph();
                    cellParagraph.AppendText("-");
                    cell.Width = j == 1 ? 78 : j == 0 ? subjectCellWidth : 36;
                    if (j == 3)
                    {
                        cellParagraph.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                }
            }

            table2.TableFormat.Borders.BorderType = BorderStyle.Cleared;

            bookNav.InsertTable(table2);

        }

        private async Task CreateSubjectListTable(WordDocument document, int firstTableLines = 38, float subjectCellWidth = 480f, bool FitToWindow = false)
        {
            BookmarksNavigator bookNav = new BookmarksNavigator(document);

            bookNav.MoveToBookmark("SubjectsTable", true, false);

            IWTable table1 = new WTable(document, true);

            if (FitToWindow)
            {
                table1.AutoFit(AutoFitType.FitToWindow);
            }
            else
            {
                table1.AutoFit(AutoFitType.FixedColumnWidth);
            }

            var idKeyValueB = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "B").FirstOrDefault().IdKeyValue;
            var idKeyValueA1 = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "A1").FirstOrDefault().IdKeyValue;
            var idKeyValueA2 = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "A2").FirstOrDefault().IdKeyValue;
            var idKeyValueA3 = professionalTrainingTypesSource.Where(x => x.KeyValueIntCode == "A3").FirstOrDefault().IdKeyValue;
            List<object> gradesList = new List<object>();

            var trainingCurriculum = await this.TrainingService.GetTrainingCurriculumByIdCourseAsync(CourseVM.IdCourse);



            gradesList.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA1).Where(x => x.TheoryHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));
            gradesList.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA2).Where(x => x.TheoryHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

            gradesList.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA3).Where(x => x.TheoryHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));


            if (courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA2).Any() || courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueA3).Any())
            {
                gradesList.Add("Учебна практика по:");
                gradesList.AddRange(courseSubjectSource
                    .Where(x => x.IdProfessionalTraining == idKeyValueA2)
                    .Where(x => x.PracticeHours != 0).Where(x => !x.Subject.ToLower().Contains("производствена практика")).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

                gradesList.AddRange(courseSubjectSource
                    .Where(x => x.IdProfessionalTraining == idKeyValueA3)
                    .Where(x => x.PracticeHours != 0).Where(x => !x.Subject.ToLower().Contains("производствена практика")).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

                gradesList.AddRange(courseSubjectSource
                    .Where(x => x.IdProfessionalTraining == idKeyValueA2)
                    .Where(x => x.PracticeHours != 0).Where(x => x.Subject.ToLower().Contains("производствена практика")).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

                gradesList.AddRange(courseSubjectSource
                    .Where(x => x.IdProfessionalTraining == idKeyValueA3)
                    .Where(x => x.PracticeHours != 0).Where(x => x.Subject.ToLower().Contains("производствена практика")).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

            }

            if (courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueB).Any())
            {
                gradesList.Add("Разширена професионална подготовка:");
                gradesList.Add("Теория:");
                gradesList.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueB && x.TheoryHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));
                gradesList.Add("Практика:");
                gradesList.AddRange(courseSubjectSource.Where(x => x.IdProfessionalTraining == idKeyValueB && x.PracticeHours != 0).OrderBy(x => trainingCurriculum.Where(x => x.IdProfessionalTraining == x.IdProfessionalTraining && x.Subject == x.Subject).First()?.Order ?? 0));

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
                if (record is CourseSubjectVM courseSubject)
                {
                    // Get the course subject grade
                    var courseSubjectGrade =
                        await this.TrainingService
                            .GetClientCourseSubjectGradeByClientCourseIdAndByIdCourseSubjectAsync(
                                this.ClientCourseVM.IdClientCourse,
                                courseSubject.IdCourseSubject);

                    // Get the subject text
                    string subject = courseSubject.Subject == null
                        ? "-"
                        : (isSubListItem && !courseSubject.Subject.ToLower().Contains("производствена практика") ? "– " : "") + courseSubject.Subject;

                    // Create the string with "-" for empty lines
                    string fillEmptyLines = string.Concat(
                            Enumerable.Repeat("-\n", BaseHelper.CalculateNumberOfLines(subject == null
                                ? "-"
                                : subject, subjectCellWidth - cellPadding, new Font("Calibri", 11f)) - 1));

                    // Add a new cell for the subject
                    var cell = row.AddCell();
                    cell.AddParagraph().AppendText(subject);
                    cell.Width = subjectCellWidth;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    tableHeight += BaseHelper.CalculateNumberOfLines(subject == null
                        ? "-"
                        : subject, cell.Width - cellPadding, new Font("Calibri", 11f));



                    // Add a new cell for the hours
                    cell = row.AddCell();
                    double hours = !isPractice ? courseSubject.TheoryHours : courseSubject.PracticeHours;
                    string hoursText = hours != 0 ? hours + "" : "-";
                    IWParagraph cellHours = cell.AddParagraph();

                    cell.Width = 36;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                    cellHours.AppendText(fillEmptyLines + hoursText);
                    cellHours.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;


                }
                else if (record is string category)
                {

                    // Set isPractice to true
                    if (category.Equals("Учебна практика по:"))
                    {
                        isPractice = true;
                        isSubListItem = true;
                    }
                    else if (category.Equals("Теория:"))
                    {
                        isPractice = false;
                        isSubListItem = true;
                    }
                    else if (category.Equals("Практика:"))
                    {
                        isPractice = true;
                        isSubListItem = true;
                    }
                    else
                    {
                        isSubListItem = false;
                    }

                    if (!(category.Equals("Теория:") || category.Equals("Практика:")))
                    {
                        // Create the string with "-" for empty lines
                        string fillEmptyLines = string.Concat(
                            Enumerable.Repeat("-\n",
                                BaseHelper.CalculateNumberOfLines(category, subjectCellWidth - cellPadding,
                                    new Font("Calibri", 11f, FontStyle.Bold)) - 1));

                        // Add a new cell for the category
                        var cell = row.AddCell();
                        cell.AddParagraph().AppendText(category);
                        cell.Width = subjectCellWidth;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                        tableHeight += BaseHelper.CalculateNumberOfLines(category, cell.Width - cellPadding,
                            new Font("Calibri", 11f, FontStyle.Bold));

                        // Add new cells with "-" text
                        cell = row.AddCell();
                        IWParagraph cellHours = cell.AddParagraph();
                        cell.Width = 36;
                        cellHours.AppendText("-" + fillEmptyLines);
                        cellHours.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                        cell.CellFormat.TextWrap = true;
                        cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;

                    }
                }
            }

            for (int i = 0; i < gradesList.Count(); i++)
            {

                // Add a new row to the table
                var row = table1.AddRow(true, false);
                row.HeightType = TableRowHeightType.Exactly;

                // Fill the row with data
                await FillRow(row, gradesList[i]);


            }

            if (tableHeight < firstTableLines)
            {
                for (int i = 0; i < firstTableLines - tableHeight; i++)
                {
                    // Add a new row to the table
                    var row = table1.AddRow(true, false);

                    // Add new cells with "-" text
                    var cell = row.AddCell();
                    cell.AddParagraph().AppendText("-");
                    cell.Width = subjectCellWidth;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;

                    // Add new cells with "-" text
                    cell = row.AddCell();
                    IWParagraph cellHours = cell.AddParagraph();
                    cell.Width = 36;
                    cellHours.AppendText("-");
                    cellHours.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Center;
                    cell.CellFormat.TextWrap = true;
                    cell.CellFormat.Borders.BorderType = BorderStyle.Cleared;
                }
            }

            table1.TableFormat.Borders.BorderType = BorderStyle.Cleared;
            bookNav.InsertTable(table1);


        }


    }
}
