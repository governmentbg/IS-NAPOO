using System.Drawing;
using System.Globalization;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
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

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;

public partial class TrainingCourseDuplicateIssueModal : BlazorBaseComponent
{
    private ClientCourseVM ClientCourseVM;
    private IEnumerable<ClientCourseVM> clientsSource = new List<ClientCourseVM>();
    private IEnumerable<CourseVM> coursesSource = new List<CourseVM>();
    private List<KeyValueVM> courseStatusSource = new();
    private IEnumerable<CourseSubjectVM> courseSubjectSource;
    private CourseVM CourseVM;
    private List<DocumentSerialNumberVM> documentSerialNumbersSource = new();
    private DocumentStatusModal documentStatusModal = new();
    private DuplicateIssueVM duplicateIssueVM = new();
    private int idCourseType = 0;
    private KeyValueVM kvCertificateOfVocationalTraining;
    private KeyValueVM kvEGN = new();
    private KeyValueVM kvFinishedWithDoc = new();
    private KeyValueVM kvIDN = new();
    private IEnumerable<KeyValueVM> kvIndentTypeSource = new List<KeyValueVM>();
    private KeyValueVM kvIssueOfDuplicate = new();
    private KeyValueVM kvLNCh = new();
    private IEnumerable<KeyValueVM> kvNationalitySource = new List<KeyValueVM>();
    private KeyValueVM kvPartProfessionValue = new();
    private KeyValueVM kvQualificationLevel = new();
    private IEnumerable<KeyValueVM> kvSexSource = new List<KeyValueVM>();
    private KeyValueVM kvVocationalQualificationCertificateDuplicate;
    private ValidationMessageStore? messageStore;
    private IEnumerable<KeyValueVM> professionalTrainingTypesSource;
    private IEnumerable<CourseProtocolVM> protocolsSource = new List<CourseProtocolVM>();
    private SubmissionCommentModal submissionCommentModal = new();
    private IEnumerable<KeyValueVM> finishedTypeSource = new List<KeyValueVM>();
    private string title = string.Empty;
    private string type = string.Empty;
    private ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM duplicateFinishedModel;
    private ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM finishedModel;
    private List<DocumentSeriesVM> documentSeriesSource;
    private List<TrainingCurriculumVM> addedCurriculums;
    private IEnumerable<KeyValueVM> professionalTrainingsSource;
    private PrintDocumentModalMessage printDocumentModalMessage = new PrintDocumentModalMessage();

    public override bool IsContextModified => this.editContext.IsModified();

    [Parameter] public EventCallback CallbackAfterSubmit { get; set; }

    [Inject] public ITrainingService TrainingService { get; set; }

    [Inject] public ILocationService LocationService { get; set; }

    [Inject] public IApplicationUserService ApplicationUserService { get; set; }

    [Inject] public IJSRuntime JsRuntime { get; set; }

    [Inject] public IDataSourceService DataSourceService { get; set; }

    [Inject] public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

    [Inject] public IUploadFileService UploadFileService { get; set; }

    [Inject] public ITemplateDocumentService TemplateDocumentService { get; set; }

    [Inject] public ILocService LocService { get; set; }

    protected override async Task OnInitializedAsync()
    {
        this.editContext = new EditContext(this.duplicateIssueVM);
        this.finishedTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseFinishedType");
        this.kvEGN = await DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
        this.kvQualificationLevel =
            await DataSourceService.GetKeyValueByIntCodeAsync("QualificationLevel", "WithoutQualification_Update");
        this.kvLNCh = await DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "LNK");
        this.kvIDN = await DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "IDN");
        this.kvSexSource = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");
        this.kvNationalitySource = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality");
        this.kvFinishedWithDoc = await DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type1");
        this.kvIndentTypeSource = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
        this.kvPartProfessionValue =
            await DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "PartProfession");
        this.kvIssueOfDuplicate = await DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type6");
        this.professionalTrainingTypesSource =
            await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
    }

    public async Task OpenModal(string courseType, int idCourseType)
    {
        this.duplicateIssueVM = new DuplicateIssueVM();
        this.editContext = new EditContext(this.duplicateIssueVM);

        this.type = courseType;
        this.idCourseType = idCourseType;

        this.coursesSource = await this.TrainingService.GetAllArchivedAndFinishedCoursesByIdCandidateProviderAndByIdCourseTypeAsync(this.UserProps.IdCandidateProvider, this.idCourseType);

        this.SetDocumentTypeName();

        this.kvFinishedWithDoc = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type1");

        this.title = "Данни за издаване на дубликат";

        this.duplicateIssueVM.CourseTypeFromToken = this.type;

        this.isVisible = true;
        this.StateHasChanged();
    }

    private async Task SubmitBtn()
    {
        this.SpinnerShow();

        if (this.loading)
        {
            return;
        }
        try
        {
            this.loading = true;

            this.editContext = new EditContext(this.duplicateIssueVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.OnValidationRequested += this.ValidateDocumentDate;
            this.editContext.OnValidationRequested += this.ValidateRequiredFabricNumber;
            this.editContext.OnValidationRequested += this.ValidateRequiredFields;

            this.duplicateIssueVM.IdValidationClient = 1;

            if (this.editContext.Validate())
            {
                this.duplicateIssueVM.IdValidationClient = null;

                var inputContext = new ResultContext<DuplicateIssueVM>();
                inputContext.ResultContextObject = this.duplicateIssueVM;
                var result = new ResultContext<NoResult>();
                if (this.duplicateIssueVM.IdClientCourseDocument == 0)
                {
                    result = await this.TrainingService.CreateDuplicateDocumentAsync(inputContext);
                }
                else
                {
                    result = await this.TrainingService.UpdateDuplicateDocumentAsync(inputContext);
                }

                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                    await this.SetCreateAndModifyInfoAsync();

                    await this.CallbackAfterSubmit.InvokeAsync();
                }
            }
            else
            {
                await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
            }
        }
        finally
        {
            this.loading = false;
        }

        this.SpinnerHide();
    }

    private void OnFinishedYearValueChanged()
    {
        if (this.type == GlobalConstants.COURSE_DUPLICATES_SPK)
        {
            this.SpinnerShow();
            this.LoadDocumentSerialNumbersData();
            this.SpinnerHide();
        }
    }

    private async Task OnChange(UploadChangeEventArgs args)
    {
        this.SpinnerShow();

        var file = args.Files[0].Stream;

        var result = await this.UploadFileService.UploadFileAsync<CourseDocumentUploadedFile>(file, args.Files[0].FileInfo.Name, "ClientCourseDocument", this.duplicateIssueVM.IdCourseDocumentUploadedFile);
        if (!string.IsNullOrEmpty(result))
        {
            this.duplicateIssueVM.UploadedFileName = result;
        }

        this.StateHasChanged();

        this.SpinnerHide();
    }

    private async Task OnRemoveClick(RemovingEventArgs args)
    {
        if (!string.IsNullOrEmpty(this.duplicateIssueVM.UploadedFileName))
        {
            bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

            if (isConfirmed)
            {
                this.SpinnerShow();
                var result = await this.UploadFileService.RemoveFileByIdAsync<CourseDocumentUploadedFile>(this.duplicateIssueVM.IdCourseDocumentUploadedFile);
                if (result == 1)
                {
                    this.duplicateIssueVM.UploadedFileName = null;
                }

                this.StateHasChanged();

                this.SpinnerHide();
            }
        }
    }

    private async Task OnRemove(string fileName)
    {
        if (!string.IsNullOrEmpty(this.duplicateIssueVM.UploadedFileName))
        {
            bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

            if (isConfirmed)
            {
                this.SpinnerShow();
                var result = await this.UploadFileService.RemoveFileByIdAsync<CourseDocumentUploadedFile>(this.duplicateIssueVM.IdCourseDocumentUploadedFile);
                if (result == 1)
                {
                    this.duplicateIssueVM.UploadedFileName = null;
                }

                this.StateHasChanged();
                this.SpinnerHide();
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

            if (!string.IsNullOrEmpty(this.duplicateIssueVM.UploadedFileName))
            {
                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseDocumentUploadedFile>(this.duplicateIssueVM.IdCourseDocumentUploadedFile);
                if (hasFile)
                {
                    var document = await this.UploadFileService.GetUploadedFileAsync<CourseDocumentUploadedFile>(this.duplicateIssueVM.IdCourseDocumentUploadedFile);
                    if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, this.duplicateIssueVM.FileName, document.MS!.ToArray());
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

    private void LoadDocumentSerialNumbersData()
    {
        if (this.duplicateIssueVM.FinishedYear.HasValue)
        {
            if (this.duplicateIssueVM.FinishedYear.ToString().Length == 4 && (this.duplicateIssueVM.FinishedYear.Value == DateTime.Now.Year || this.duplicateIssueVM.FinishedYear.Value == DateTime.Now.Year - 1))
            {
                CandidateProviderVM candidateProvider = new CandidateProviderVM() { IdCandidate_Provider = this.UserProps.IdCandidateProvider };
                this.documentSerialNumbersSource = this.ProviderDocumentRequestService.GetAllDocumentSerialNumbersByIdCandidateProviderByStatusReceivedByYearAndByIdCourseType(candidateProvider, this.duplicateIssueVM.FinishedYear.Value, this.idCourseType, true).OrderBy(x => x.SerialNumberAsIntForOrderBy).ToList();
                this.StateHasChanged();
            }
            else
            {
                this.documentSerialNumbersSource = new List<DocumentSerialNumberVM>();
                this.StateHasChanged();
            }
        }
        else
        {
            this.documentSerialNumbersSource = new List<DocumentSerialNumberVM>();
            this.StateHasChanged();
        }
    }

    private void SetDocumentTypeName()
    {
        if (this.type == GlobalConstants.COURSE_DUPLICATES_SPK)
        {
            this.duplicateIssueVM.DocumentTypeName = "Дубликат на свидетелство за професионална квалификация (универсален образец)";
            this.duplicateIssueVM.HasDocumentFabricNumber = true;
        }
        else
        {
            this.duplicateIssueVM.DocumentTypeName = "Удостоверение за професионално обучение";
        }
    }

    private async Task OnCourseSelectedEventHandlerAsync(ChangeEventArgs<int?, CourseVM> args)
    {
        this.SpinnerShow();

        if (args is not null && args.ItemData is not null)
        {
            this.duplicateIssueVM.Course = args.ItemData;
            this.clientsSource = await this.TrainingService.GetCourseClientsByIdCourseAndByIdCourseFinishedTypeAsync(args.ItemData.IdCourse, this.kvFinishedWithDoc.IdKeyValue);
            this.StateHasChanged();
        }
        else
        {
            this.duplicateIssueVM.Course = null;
            this.duplicateIssueVM.IdCourse = null;
            this.duplicateIssueVM.IdClientCourse = null;
        }

        this.SpinnerHide();
    }

    private async Task OnClientCourseSelectedEventHandlerAsync(ChangeEventArgs<int?, ClientCourseVM> args)
    {
        this.SpinnerShow();

        if (args is not null && args.ItemData is not null && this.duplicateIssueVM.IdCourse.HasValue)
        {
            this.title = $"Данни за издаване на дубликат на <span style=\"color: #ffffff;\">{args.ItemData.FullName}</span> от <span style=\"color: #ffffff;\">{this.duplicateIssueVM.Course.CourseNameAndPeriod}</span>";
            this.protocolsSource = await this.TrainingService.GetCourseProtocol381BByIdClientCourseAsync(args.ItemData.IdClientCourse);
            if (this.protocolsSource.Any())
            {
                this.duplicateIssueVM.IdCourseProtocol = this.protocolsSource.FirstOrDefault()!.IdCourseProtocol;
                this.duplicateIssueVM.FinalResult = this.protocolsSource.FirstOrDefault().CourseProtocolGrades.FirstOrDefault().Grade.ToString();
                this.duplicateIssueVM.DocumentProtocol = this.protocolsSource.FirstOrDefault()!.CourseProtocolNumber;
            }
            else
            {
                this.duplicateIssueVM.IdCourseProtocol = 0;
                this.duplicateIssueVM.FinalResult = string.Empty;
                this.duplicateIssueVM.DocumentProtocol = string.Empty;
            }

            this.StateHasChanged();
        }
        else
        {
            this.title = "Данни за издаване на дубликат";
            this.protocolsSource = new List<CourseProtocolVM>();
            this.duplicateIssueVM.IdCourseProtocol = 0;
            this.duplicateIssueVM.FinalResult = string.Empty;
            this.duplicateIssueVM.DocumentProtocol = string.Empty;
            this.StateHasChanged();
        }

        this.SpinnerHide();
    }

    private async Task SetCreateAndModifyInfoAsync()
    {
        this.duplicateIssueVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.duplicateIssueVM.IdModifyUser);
        this.duplicateIssueVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.duplicateIssueVM.IdCreateUser);
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

            await this.documentStatusModal.OpenModal(this.duplicateIssueVM.IdClientCourseDocument, "Course");
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

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да подадете дубликата на документа за проверка към НАПОО?");
            if (isConfirmed)
            {

                this.submissionCommentModal.OpenModal(GlobalConstants.RIDPK_OPERATION_FILE_IN, new List<ClientCourseDocumentVM>() { new ClientCourseDocumentVM() { IdClientCourseDocument = this.duplicateIssueVM.IdClientCourseDocument } });
            }
        }
        finally
        {
            this.loading = false;
        }

        this.SpinnerHide();
    }

    private void SetDocumentRIDPKStatusAsSent()
    {
        this.duplicateIssueVM.IsRIDPKDocumentSubmitted = true;
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
            if (duplicateIssueVM.IdCourse != null && duplicateIssueVM.IdClientCourse != null)
            {
                CourseVM = await TrainingService.GetTrainingCourseByIdAsync(duplicateIssueVM.IdCourse.Value);
                this.duplicateFinishedModel =
                    await this.TrainingService
                        .GetClientCourseDuplicateClientCourseDocumentAndCourseDocumentUploadedFileCombinedByIdClientCourseAsync(
                            this.ClientCourseVM.IdClientCourse);
                this.duplicateFinishedModel.IdFinishedType =
                    this.finishedTypeSource.Where(x => x.Name == "Издаване на дубликат").FirstOrDefault()!.IdKeyValue;
                this.finishedModel =
                    await this.TrainingService
                        .GetClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedByIdClientCourseAsync(
                            this.ClientCourseVM.IdClientCourse);


                ClientCourseVM =
                    await TrainingService.GetTrainingClientCourseByIdAsync(duplicateIssueVM.IdClientCourse.Value);
                if (this.duplicateFinishedModel.DocumentTypeName.ToLower()
                    .Contains("свидетелство за професионална квалификация"))
                {
                    this.kvVocationalQualificationCertificateDuplicate =
                        await this.DataSourceService.GetKeyValueByIntCodeAsync(
                            "ProcedureDocumentType",
                            "VocationalQualificationCertificateDuplicate");
                    var templateDocuments =
                        (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM()))
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
                    string formOfTraining = this.CourseVM.FormEducation.Name.ToLower();
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

                    await CreateGradeListTable(document, 34, templateDocument.DateFrom.Value.CompareTo(new DateTime(2023, 5, 1)) == 0 ? 30 : 32);


                    MemoryStream stream = new MemoryStream();
                    document.Save(stream, FormatType.Docx);
                    blueprint.Close();
                    document.Close();
                    await FileUtils.SaveAs(JsRuntime,
                        BaseHelper.ConvertCyrToLatin("Свидетелство_" + this.ClientCourseVM.FirstName + "_" +
                                                     this.ClientCourseVM.FamilyName) + ".docx", stream.ToArray());


                }
                else
                {

                    this.kvCertificateOfVocationalTraining = await this.DataSourceService.GetKeyValueByIntCodeAsync(
                        "ProcedureDocumentType",
                        "CertificateOfVocationalTraining");
                    var templateDocuments =
                        (await this.TemplateDocumentService.GetAllTemplateDocumentsAsync(new TemplateDocumentVM()))
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
                    await FileUtils.SaveAs(JsRuntime,
                        BaseHelper.ConvertCyrToLatin("Удостоверение_" + this.ClientCourseVM.FirstName + "_" +
                                                     this.ClientCourseVM.FamilyName) + ".docx", stream.ToArray());
                }
            }
        }
        catch
        {
            await this.ShowErrorAsync("Неуспешно генериране на документ!");
        }
    }
    private void ValidateDocumentDate(object? sender, ValidationRequestedEventArgs args)
    {
        this.messageStore?.Clear();

        if (this.duplicateIssueVM.IdCourse != 0 && this.duplicateIssueVM.DocumentDate.HasValue)
        {
            if (this.duplicateIssueVM.Course is not null)
            {
                if (this.duplicateIssueVM.Course.StartDate.HasValue)
                {
                    if (this.duplicateIssueVM.DocumentDate < this.duplicateIssueVM.Course.StartDate)
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.duplicateIssueVM, "DocumentDate");
                        this.messageStore?.Add(fi, $"Полето 'Дата на издаване' не може да бъде преди {this.duplicateIssueVM.Course.StartDate.Value.ToString("dd.MM.yyyy")}г.!");
                        return;
                    }
                }
            }
        }
    }
    private void ValidateRequiredFabricNumber(object? sender, ValidationRequestedEventArgs args)
    {
        if (this.duplicateIssueVM.HasDocumentFabricNumber && this.duplicateIssueVM.IdDocumentSerialNumber is null)
        {
            FieldIdentifier fi = new FieldIdentifier(this.duplicateIssueVM, "IdDocumentSerialNumber");
            this.messageStore?.Add(fi, "Полето 'Фабричен номер на документа' е задължително!");
        }
    }

    private void ValidateRequiredFields(object? sender, ValidationRequestedEventArgs args)
    {
        if (this.duplicateIssueVM.FinishedYear is null)
        {
            FieldIdentifier fi = new FieldIdentifier(this.duplicateIssueVM, "FinishedYear");
            this.messageStore?.Add(fi, "Полето 'Година' е задължително!");
        }
        else
        {
            int validYear;
            if (!int.TryParse(this.duplicateIssueVM.FinishedYear.Value.ToString(), out validYear))
            {
                FieldIdentifier fi = new FieldIdentifier(this.duplicateIssueVM, "FinishedYear");
                this.messageStore?.Add(fi, "Полето 'Година' може да съдържа само цифри!");
            }
            else
            {
                if (validYear.ToString().Length != 4)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.duplicateIssueVM, "FinishedYear");
                    this.messageStore?.Add(fi, $"Полето 'Година' може да съдържа само валидна стойност за година (напр. '{DateTime.Now.Year}')!");
                }
                else
                {
                    if (validYear < 0)
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.duplicateIssueVM, "FinishedYear");
                        this.messageStore?.Add(fi, $"Полето 'Година' може да съдържа само валидна стойност за година (напр. '{DateTime.Now.Year}')!");
                    }
                }
            }
        }

        if (string.IsNullOrEmpty(duplicateIssueVM.DocumentRegNo))
        {
            var fi = new FieldIdentifier(duplicateIssueVM, "DocumentRegNo");
            messageStore?.Add(fi, "Полето 'Регистрационен номер' е задължително!");
        }

        if (this.duplicateIssueVM.IdCourseProtocol == 0 && this.duplicateIssueVM.IdClientCourse.HasValue)
        {
            FieldIdentifier fi = new FieldIdentifier(this.duplicateIssueVM, "IdCourseProtocol");
            this.messageStore?.Add(fi, $"Няма данни за протокол за '3-81В', в който да е вписан избраният курсист!");
        }
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

}
