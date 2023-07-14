using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using Microsoft.JSInterop;
using ISNAPOO.WebSystem.Resources;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.AspNetCore.Components.Forms;
using ISNAPOO.Common.Framework;
using Data.Models.Data.Training;
using ISNAPOO.Core.HelperClasses;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    partial class TrainingValidationClientOrderModal : BlazorBaseComponent
    {
        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        #region Inject
        [Inject]
        public ITrainingService TrainingService  { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }
        #endregion

        private ToastMsg toast;
        private ValidationOrderVM model = new ValidationOrderVM();
        private string CreationDateStr = string.Empty;
        private string ModifyDateStr = string.Empty;
        private bool showDeleteConfirmDialog;
        private string fileNameForDeletion;

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(ValidationOrderVM validationOrderVM)
        {
            if (validationOrderVM.IdValidationClient != 0)
            {
                this.model = validationOrderVM;
            }
            else
            {
                this.model = validationOrderVM;
                this.CreationDateStr = string.Empty;
                this.ModifyDateStr = string.Empty;
                this.model.CreatePersonName = string.Empty;
                this.model.ModifyPersonName = string.Empty;
            }
            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Save()
        {
            this.SpinnerShow();
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
                    ResultContext<ValidationOrderVM> result = new ResultContext<ValidationOrderVM>();
                    result.ResultContextObject = this.model;
                    if (result.ResultContextObject.IdValidationOrder == 0)
                    {
                        result = await this.TrainingService.CreateValidationOrderAsync(result);
                    }
                    else
                    {
                        result = await this.TrainingService.UpdateValidationOrderAsync(result);
                    }
                    this.model = await this.TrainingService.GetValidationOrderByIdAsync(result.ResultContextObject.IdValidationOrder);

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
                    await this.SetFileNameAsync();
                    await CallbackAfterSubmit.InvokeAsync();
                }
                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }


        private async Task OnRemoveClick(string fileName)
        {
            if (this.model.IdValidationOrder > 0)
            {        
                    this.showDeleteConfirmDialog = !this.showDeleteConfirmDialog;
                    this.fileNameForDeletion = fileName;
                    this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
                }
            }
        
            public async void ConfirmDeleteCallback()
            {

            var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationOrder>(this.model.IdValidationOrder);

            if (result == 1)
            {
                this.model = await this.TrainingService.GetValidationOrderByIdAsync(this.model.IdValidationOrder);

                await this.SetFileNameAsync();
            }
            await this.CallbackAfterSubmit.InvokeAsync();
            this.StateHasChanged();
            this.showDeleteConfirmDialog = false;
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

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ValidationOrder>(this.model.IdValidationOrder, fileName);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<ValidationOrder>(this.model.IdValidationOrder, fileName);

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
        private async Task OnChange(UploadChangeEventArgs args)
        {
            if (args.Files.Count >= 1)
            {
                var fileNames = args.Files.Select(x => $"{x.FileInfo.Name} ");
                var fileName = string.Join(Environment.NewLine, fileNames).Trim();
                var fileStreams = args.Files.Select(x => x.Stream).ToArray();

                var result = await this.UploadFileService.UploadFileAsync<ValidationOrder>(fileStreams.FirstOrDefault(), fileName, "ValidationOrderDocument", this.model.IdValidationOrder);

                this.model = await this.TrainingService.GetValidationOrderByIdAsync(this.model.IdValidationOrder);

                await this.SetFileNameAsync();

                this.StateHasChanged();

                await Save();

                this.editContext = new EditContext(this.model);

            }
        }
        private async Task OnRemove(RemovingEventArgs args)
        {
            if (args.FilesData.Count == 1)
            {
                if (this.model.IdValidationOrder > 0)
                {
                    bool isConfirmed = true;

                    if (isConfirmed)
                    {
                        var result = await this.UploadFileService.RemoveFileByIdAsync<ValidationOrder>(this.model.IdValidationOrder);

                        if (result == 1)
                        {
                            this.model = await this.TrainingService.GetValidationOrderByIdAsync(this.model.IdValidationOrder);

                            this.StateHasChanged();
                        }
                    }
                }
            }
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
        }
    }
}
