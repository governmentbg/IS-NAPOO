using Data.Models.Data.Training;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class ClientCourseIssueLegalCapacityOrdinance : BlazorBaseComponent
    {
        private ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM model = new ClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedVM();
        private ValidationMessageStore? messageStore;
        public bool isTabRendered = false;
        private KeyValueVM kvFinishedWithDoc = new KeyValueVM();
        private IEnumerable<TypeOfRequestedDocumentVM> typeOfRequestedDocSource = new List<TypeOfRequestedDocumentVM>();
        private int kvLCId = 0;

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

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.model);
            this.FormTitle = "Документ за правоспособност";

            this.isTabRendered = true;

            this.typeOfRequestedDocSource = await this.ProviderDocumentRequestService.GetAllTypeOfRequestedDocsForLegalCapacityCourse();

            this.kvLCId = (await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "CourseRegulation1And7")).IdKeyValue;

            await this.LoadModelDataAsync();
        }

        private async Task OnChangeLegalCapacity(UploadChangeEventArgs args)
        {
            var file = args.Files[0].Stream;
            var result = await this.UploadFileService.UploadFileAsync<CourseDocumentUploadedFile>(file, args.Files[0].FileInfo.Name, "ClientCourseDocument", this.model.IdLegalCapacityCourseDocumentUploadedFile);
            if (!string.IsNullOrEmpty(result))
            {
                this.model.LegalCapacityUploadedFileName = result;
            }

            this.StateHasChanged();
        }

        private async Task OnRemoveClickLegalCapacity(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.model.LegalCapacityUploadedFileName))
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<CourseDocumentUploadedFile>(this.model.IdLegalCapacityCourseDocumentUploadedFile);
                    if (result == 1)
                    {
                        this.model.LegalCapacityUploadedFileName = null;
                    }

                    this.StateHasChanged();
                }
            }
        }

        private async Task OnRemoveLegalCapacity(string fileName)
        {
            if (!string.IsNullOrEmpty(this.model.LegalCapacityUploadedFileName))
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<CourseDocumentUploadedFile>(this.model.IdLegalCapacityCourseDocumentUploadedFile);
                    if (result == 1)
                    {
                        this.model.LegalCapacityUploadedFileName = null;
                    }

                    this.StateHasChanged();
                }
            }
        }

        private async Task OnDownloadClickLegalCapacity()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!string.IsNullOrEmpty(this.model.LegalCapacityUploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseDocumentUploadedFile>(this.model.IdLegalCapacityCourseDocumentUploadedFile);
                    if (hasFile)
                    {
                        var document = await this.UploadFileService.GetUploadedFileAsync<CourseDocumentUploadedFile>(this.model.IdLegalCapacityCourseDocumentUploadedFile);
                        if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, this.model.LegalCapacityFileName, document.MS!.ToArray());
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

        public override async void SubmitHandler()
        {
            this.model.IdFinishedType = 2;

            this.model.IdLegalCapacityDocumentType = this.kvLCId;

            this.editContext = new EditContext(this.model);
            this.editContext.EnableDataAnnotationsValidation();
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.OnValidationRequested += this.ValidateRequiredFields;

            this.editContext.Validate();

            this.model.IdFinishedType = null;

            await this.CallbackAfterEditContextValidation.InvokeAsync(this.model);
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.model.DocumentRegNo) && string.IsNullOrEmpty(this.model.UploadedFileName) && string.IsNullOrEmpty(this.model.LegalCapacityDocumentRegNo) && string.IsNullOrEmpty(this.model.LegalCapacityUploadedFileName) &&
                !this.model.DocumentDate.HasValue && this.model.IdFinishedType is not null && !this.model.LegalCapacityDocumentDate.HasValue;
        }

        public void SetEditContextAsUnmodified()
        {
            if (this.editContext is not null)
                this.editContext.MarkAsUnmodified();
        }

        private void ValidateRequiredFields(object? sender, ValidationRequestedEventArgs args)
        {
            if (string.IsNullOrEmpty(this.model.LegalCapacityDocumentRegNo))
            {
                FieldIdentifier fi = new FieldIdentifier(this.model, "LegalCapacityDocumentRegNo");
                this.messageStore?.Add(fi, "Полето 'Регистрационен номер (документ за правоспособност)' е задължително!");
            }

            if (this.model.LegalCapacityDocumentDate is null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.model, "LegalCapacityDocumentDate");
                this.messageStore?.Add(fi, "Полето 'Дата на издаване (документ за правоспосбност)' е задължително!");
            }

            if (this.model.IdLegalCapacityTypeOfRequestedDocument is null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.model, "IdLegalCapacityTypeOfRequestedDocument");
                this.messageStore?.Add(fi, "Полето 'Вид на издадения документ за правоспособност' е задължително!");
            }
        }

        public async Task LoadModelDataAsync()
        {
            if (this.ClientCourseVM is not null)
            {
                this.model = await this.TrainingService.GetLegalCapacityClientCourseClientCourseDocumentAndCourseDocumentUploadedFileCombinedByIdClientCourseAsync(this.ClientCourseVM.IdClientCourse);
            }
        }

        public bool GetIsEditContextModified()
        {
            if (this.editContext is not null)
            {
                return this.editContext.IsModified();
            }

            return false;
        }
    }
}
