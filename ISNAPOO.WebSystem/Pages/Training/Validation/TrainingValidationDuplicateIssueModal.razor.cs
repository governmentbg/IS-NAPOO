using Data.Models.Data.DOC;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class TrainingValidationDuplicateIssueModal : BlazorBaseComponent
    {
        private SubmissionCommentModal submissionCommentModal = new SubmissionCommentModal();
        private DocumentStatusModal documentStatusModal = new DocumentStatusModal();

        private string title = string.Empty;
        private DuplicateIssueVM duplicateIssueVM = new DuplicateIssueVM();
        private IEnumerable<ValidationClientVM> clientsSource = new List<ValidationClientVM>();
        private KeyValueVM kvFinishedWithDoc = new KeyValueVM();
        private IEnumerable<ValidationProtocolVM> protocolsSource = new List<ValidationProtocolVM>();
        private int idCourseType = 0;
        private string type = string.Empty;
        private List<DocumentSerialNumberVM> documentSerialNumbersSource = new List<DocumentSerialNumberVM>();
        private ValidationMessageStore? messageStore;
        private List<ValidationClientDocumentVM> validationClientDocuments = new List<ValidationClientDocumentVM>();

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.duplicateIssueVM);
        }

        public async Task OpenModal(string courseType, int idCourseType)
        {
            this.duplicateIssueVM = new DuplicateIssueVM();
            this.editContext = new EditContext(this.duplicateIssueVM);

            this.type = courseType;
            this.idCourseType = idCourseType;

            this.SetDocumentTypeName();

            if (this.type == GlobalConstants.VALIDATION_DUPLICATES_SPK)
            {
                this.kvFinishedWithDoc = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type5");
            }
            else
            {
                this.kvFinishedWithDoc = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type8");
            }

            this.clientsSource = await this.TrainingService.GetAllArchivedAndFinishedValidationsByIdCandidateProviderByIdCourseTypeAndByIdFinishedTypeAsync(this.UserProps.IdCandidateProvider, this.idCourseType, this.kvFinishedWithDoc.IdKeyValue);

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

                this.duplicateIssueVM.IdClientCourse = 1;
                this.duplicateIssueVM.IdCourse = 1;

                if (this.editContext.Validate())
                {
                    this.duplicateIssueVM.IdClientCourse = null;
                    this.duplicateIssueVM.IdCourse = null;

                    var inputContext = new ResultContext<DuplicateIssueVM>();
                    inputContext.ResultContextObject = this.duplicateIssueVM;
                    var result = new ResultContext<NoResult>();
                    if (this.duplicateIssueVM.IdClientCourseDocument == 0)
                    {
                        result = await this.TrainingService.CreateValidationDuplicateDocumentAsync(inputContext);
                    }
                    else
                    {
                        result = await this.TrainingService.UpdateValidationDuplicateDocumentAsync(inputContext);
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
            if (this.type == GlobalConstants.VALIDATION_DUPLICATES_SPK)
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

            var result = await this.UploadFileService.UploadFileAsync<ValidationDocumentUploadedFile>(file, args.Files[0].FileInfo.Name, "ValidationDocument", this.duplicateIssueVM.IdValidationDocumentUploadedFile);
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
                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли си сте, че искате да изтриете прикачения файл?");
                if (isConfirmed)
                {
                    this.SpinnerShow();
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationDocumentUploadedFile>(this.duplicateIssueVM.IdValidationDocumentUploadedFile);
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
                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли си сте, че искате да изтриете прикачения файл?");
                if (isConfirmed)
                {
                    this.SpinnerShow();
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationDocumentUploadedFile>(this.duplicateIssueVM.IdValidationDocumentUploadedFile);
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
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ValidationDocumentUploadedFile>(this.duplicateIssueVM.IdValidationDocumentUploadedFile);
                    if (hasFile)
                    {
                        var document = await this.UploadFileService.GetUploadedFileAsync<ValidationDocumentUploadedFile>(this.duplicateIssueVM.IdValidationDocumentUploadedFile);
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
                if (this.duplicateIssueVM.FinishedYear.ToString().Length == 4)
                {
                    CandidateProviderVM candidateProvider = new CandidateProviderVM() { IdCandidate_Provider = this.UserProps.IdCandidateProvider };
                    this.documentSerialNumbersSource = this.ProviderDocumentRequestService.GetAllDocumentSerialNumbersByIdCandidateProviderByStatusReceivedByYearAndByIdCourseType(candidateProvider, this.duplicateIssueVM.FinishedYear.Value, this.idCourseType, false, true).OrderBy(x => x.SerialNumberAsIntForOrderBy).ToList();
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
            if (this.type == GlobalConstants.VALIDATION_DUPLICATES_SPK)
            {
                this.duplicateIssueVM.DocumentTypeName = "Дубликат на свидетелство за валидиране на професионална квалификация";
                this.duplicateIssueVM.HasDocumentFabricNumber = true;
            }
            else
            {
                this.duplicateIssueVM.DocumentTypeName = "Удостоверение за професионално обучение";
            }
        }

        private async Task OnValidationClientSelectedEventHandlerAsync(ChangeEventArgs<int?, ValidationClientVM> args)
        {
            this.SpinnerShow();

            if (args is not null && args.ItemData is not null && this.duplicateIssueVM.IdValidationClient.HasValue)
            {
                this.duplicateIssueVM.ValidationClient = args.ItemData;
                this.title = $"Данни за издаване на дубликат на <span style=\"color: #ffffff;\">{args.ItemData.FullName}</span>";
                this.protocolsSource = await this.TrainingService.GetValidationProtocol381BByIdValidationClientAsync(args.ItemData.IdValidationClient);
                if (this.protocolsSource.Any())
                {
                    this.duplicateIssueVM.IdValidationProtocol = this.protocolsSource.FirstOrDefault()!.IdValidationProtocol;
                    this.duplicateIssueVM.FinalResult = this.protocolsSource.FirstOrDefault()!.ValidationProtocolGrades.FirstOrDefault()!.Grade.ToString();
                    this.duplicateIssueVM.DocumentProtocol = this.protocolsSource.FirstOrDefault()!.ValidationProtocolNumber;
                }
                else
                {
                    this.duplicateIssueVM.IdValidationProtocol = 0;
                    this.duplicateIssueVM.FinalResult = string.Empty;
                    this.duplicateIssueVM.DocumentProtocol = string.Empty;
                }

                this.StateHasChanged();
            }
            else
            {
                this.title = "Данни за издаване на дубликат";
                this.protocolsSource = new List<ValidationProtocolVM>();
                this.duplicateIssueVM.IdValidationProtocol = 0;
                this.duplicateIssueVM.FinalResult = string.Empty;
                this.duplicateIssueVM.DocumentProtocol = string.Empty;
                this.duplicateIssueVM.ValidationClient = null;
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

                await this.documentStatusModal.OpenModal(this.duplicateIssueVM.IdValidationClientDocument, "Validation");
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

                    this.validationClientDocuments = new List<ValidationClientDocumentVM>() { new ValidationClientDocumentVM() { IdValidationClientDocument = this.duplicateIssueVM.IdValidationClientDocument } }; 
                    this.submissionCommentModal.OpenModal(GlobalConstants.RIDPK_OPERATION_FILE_IN);
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
            //TODO: да се направи
        }

        private void ValidateDocumentDate(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.duplicateIssueVM.IdValidationClient != 0 && this.duplicateIssueVM.DocumentDate.HasValue)
            {
                if (this.duplicateIssueVM.ValidationClient.StartDate.HasValue)
                {
                    if (this.duplicateIssueVM.DocumentDate < this.duplicateIssueVM.ValidationClient.StartDate)
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.duplicateIssueVM, "DocumentDate");
                        this.messageStore?.Add(fi, $"Полето 'Дата на издаване' не може да бъде преди {this.duplicateIssueVM.ValidationClient.StartDate.Value.ToString("dd.MM.yyyy")} г.!");
                        return;
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

            if (string.IsNullOrEmpty(this.duplicateIssueVM.DocumentRegNo))
            {
                FieldIdentifier fi = new FieldIdentifier(this.duplicateIssueVM, "DocumentRegNo");
                this.messageStore?.Add(fi, "Полето 'Регистрационен номер' е задължително!");
            }

            if (this.duplicateIssueVM.IdValidationProtocol == 0 && this.duplicateIssueVM.IdValidationClient.HasValue)
            {
                FieldIdentifier fi = new FieldIdentifier(this.duplicateIssueVM, "IdValidationProtocol");
                this.messageStore?.Add(fi, $"Няма данни за протокол за '3-81В', в който да е вписано валидирано лице '{this.duplicateIssueVM.ValidationClient.FullName}'!");
            }
        }
    }
}
