using Data.Models.Data.Training;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Training.Consulting
{
    public partial class ConsultingClientDocumentModal : BlazorBaseComponent
    {
        [Parameter]
        public EventCallback CallBackAfterSave { get; set; }
        private ConsultingClientRequiredDocumentVM model = new ConsultingClientRequiredDocumentVM();
        private SfUploader uploader = new SfUploader();
        private IEnumerable<KeyValueVM> requiredDocumentTypesSource = new List<KeyValueVM>();
        private string CreationDateStr = string.Empty;
        private string ModifyDateStr = string.Empty;
        private string fileName = "";

        protected override async Task OnInitializedAsync()
        {
            this.requiredDocumentTypesSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType")).Where(x => x.DefaultValue1 != null && x.DefaultValue3.Contains("CIPO")).ToList();
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(ConsultingClientRequiredDocumentVM model)
        {
            this.model = model;
            if (model.IdConsultingClientRequiredDocument != 0)
            {
                this.CreationDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
            }
            else
            {
                this.CreationDateStr = string.Empty;
                this.ModifyDateStr = string.Empty;
            }
            this.editContext = new EditContext(this.model);
            this.isVisible = true;
            this.StateHasChanged();
        }

        public async Task Save()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                this.editContext = new EditContext(this.model);
                this.editContext.EnableDataAnnotationsValidation();

                bool isValid = this.editContext.Validate();
                if (isValid)
                {
                    var result = new ResultContext<ConsultingClientRequiredDocumentVM>();
                    result.ResultContextObject = this.model;
                    if (result.ResultContextObject.IdConsultingClientRequiredDocument == 0)
                    {
                        result = await this.TrainingService.CreateConsultingClientRequiredDocumentAsync(result);
                    }
                    else
                    {
                        result = await this.TrainingService.UpdateConsultingClientRequiredDocumentAsync(result);
                    }
                    this.model = await this.TrainingService.GetConsultingClientRequiredDocumentById(result.ResultContextObject.IdConsultingClientRequiredDocument);
                    await this.SetFileNameAsync();
                    this.CreationDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                    this.ModifyDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                    this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                    this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                    }
                    await CallBackAfterSave.InvokeAsync();
                }
                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            var fileName = args.Files[0].FileInfo.Name;
            var fileStream = args.Files[0].Stream;

            var result = await this.UploadFileService.UploadFileAsync<ConsultingClientRequiredDocument>(fileStream, fileName, "ConsultingClientRequiredDocuments", this.model.IdConsultingClientRequiredDocument);
            if (!string.IsNullOrEmpty(result))
            {
                this.model.UploadedFileName = result;
            }

            await this.SetFileNameAsync();

            await CallBackAfterSave.InvokeAsync();

            await this.uploader.ClearAllAsync();

            this.StateHasChanged();
        }

        private async Task SetFileNameAsync()
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var fileFullName = settingResource + "\\" + this.model.UploadedFileName;
            if (Directory.Exists(fileFullName))
            {
                var files = Directory.GetFiles(fileFullName);
                this.model.FileName = string.Join(Environment.NewLine, files);
            }
            else
            {
                model.FileName = string.Empty;
            }
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете прикачения файл?");
                if (confirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ConsultingClientRequiredDocument>(this.model.IdConsultingClientRequiredDocument);

                    await this.SetFileNameAsync();

                    await CallBackAfterSave.InvokeAsync();

                    this.StateHasChanged();
                }
            }
        }

        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                this.fileName = fileName;
                bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете прикачения файл?");
                if (confirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ConsultingClientRequiredDocument>(this.model.IdConsultingClientRequiredDocument);

                    await this.SetFileNameAsync();

                    await CallBackAfterSave.InvokeAsync();

                    this.StateHasChanged();
                }
            }
            this.fileName = "";
        }

        private async Task OnDownloadClick(string fileName)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ConsultingClientRequiredDocument>(this.model.IdConsultingClientRequiredDocument, fileName);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<ConsultingClientRequiredDocument>(this.model.IdConsultingClientRequiredDocument, fileName);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, fileName, documentStream.MS!.ToArray());
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
    }
}
