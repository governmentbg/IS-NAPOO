using Data.Models.Data.ProviderData;
using Data.Models.Data.Training;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;
using Syncfusion.DocIO.DLS;

namespace ISNAPOO.WebSystem.Pages.Training.Consulting
{
    public partial class ConsultingClientFinishedData : BlazorBaseComponent
    {

        private IEnumerable<KeyValueVM> finishedTypeSource = new List<KeyValueVM>();
        private KeyValueVM kvFinishedWithDoc = new KeyValueVM();
        public bool isTabRendered = false;
        private ConsultingFinishedDataVM model = new ConsultingFinishedDataVM();

        [Parameter]
        public ConsultingClientVM ConsultingClientVM { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.model);
            this.FormTitle = "Данни за завършване";

            this.isTabRendered = true;

            this.finishedTypeSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseFinishedType")).Where(k => k.DefaultValue3 != null ? k.DefaultValue3.Contains("CIPO") : false).ToList();
            this.kvFinishedWithDoc = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseFinishedType", "Type9");

            await this.LoadModelDataAsync();
        }

        private async Task LoadModelDataAsync()
        {
            this.model = await this.TrainingService.GetConsultingClientFinishedDataByIdConsultingClientAsync(this.ConsultingClientVM.IdConsultingClient);
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            var file = args.Files[0].Stream;
            this.model.IdConsultingClient = this.ConsultingClientVM.IdConsultingClient;
            var result = await this.UploadFileService.UploadFileAsync<ConsultingDocumentUploadedFile>(file, args.Files[0].FileInfo.Name, "ConsultingClientDocument", this.model.IdConsultingDocumentUploadedFile);
            if (!string.IsNullOrEmpty(result))
            {
                this.model.UploadedFileName = result;
            }

            this.StateHasChanged();
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                string msg = "Сигурни ли си сте, че искате да изтриете прикачения файл?";
                bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ConsultingDocumentUploadedFile>(this.model.IdConsultingDocumentUploadedFile);
                    if (result == 1)
                    {
                        this.model.UploadedFileName = null;
                    }

                    this.StateHasChanged();
                }
            }
        }

        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                string msg = "Сигурни ли си сте, че искате да изтриете прикачения файл?";
                bool isConfirmed = await this.ShowConfirmDialogAsync(msg);

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ConsultingDocumentUploadedFile>(this.model.IdConsultingDocumentUploadedFile);
                    if (result == 1)
                    {
                        this.model.UploadedFileName = null;
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

                if (!string.IsNullOrEmpty(this.model.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ConsultingDocumentUploadedFile>(this.model.IdConsultingDocumentUploadedFile);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<ConsultingDocumentUploadedFile>(this.model.IdConsultingDocumentUploadedFile);
                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, this.model.FileName, documentStream.MS!.ToArray());
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

        public override void SubmitHandler()
        {
            this.editContext = new EditContext(this.model);
            this.editContext.EnableDataAnnotationsValidation();

            if (this.editContext.Validate())
            {
                this.ConsultingClientVM.IdFinishedType = this.model.IdFinishedType;
                this.ConsultingClientVM.UploadedFileName = this.model.UploadedFileName;
            }
        }
    }
}
