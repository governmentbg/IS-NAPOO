using Data.Models.Data.Control;
using Data.Models.Data.Training;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Control
{
    partial class FollowUpControlUploadedFileModal : BlazorBaseComponent
    {
        [Parameter]
        public EventCallback CallBackAfterSave { get; set; }

        #region Inject
        [Inject]
        public IControlService ControlService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }
        #endregion

        private SfUploader uploader = new SfUploader();
        private FollowUpControlUploadedFileVM model = new FollowUpControlUploadedFileVM();
        private MemoryStream file = new MemoryStream();
        private string CreationDateStr = string.Empty;
        private string ModifyDateStr = string.Empty;
        private string UploadedFileName = string.Empty;

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(FollowUpControlUploadedFileVM _model)
        {
            this.file = new MemoryStream();
            if (_model.IdFollowUpControlUploadedFile != 0)
            {
                this.model = _model;
                this.CreationDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
            }
            else
            {
                this.model = _model;
                this.CreationDateStr = string.Empty;
                this.ModifyDateStr = string.Empty;
            }

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
                    var result = new ResultContext<FollowUpControlUploadedFileVM>();
                    if (model.IdFollowUpControlUploadedFile == 0)
                    {
                        result = await this.ControlService.CreateFollowUpControlUploadedFileAsync(this.model);
                    }
                    else
                    {
                        result = await this.ControlService.UpdateFollowUpControlUploadedFileAsync(this.model);
                    }

                    this.model = await this.ControlService.GetFollowUpControlUploadedFileById(result.ResultContextObject.IdFollowUpControlUploadedFile);

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
            this.file = args.Files[0].Stream;

            var result = await this.UploadFileService.UploadFileAsync<FollowUpControlUploadedFile>(this.file, args.Files[0].FileInfo.Name, "FollowUpControlUploadedFile", this.model.IdFollowUpControlUploadedFile);
            if (!string.IsNullOrEmpty(result))
            {
                this.UploadedFileName = result;
                this.model.UploadedFileName = result;
            }

            await this.uploader.ClearAllAsync();

            this.StateHasChanged();
        }
        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<FollowUpControlUploadedFile>(this.model.IdFollowUpControlUploadedFile);
                    if (result == 1)
                    {
                        this.model.UploadedFileName = null;
                    }

                    await CallBackAfterSave.InvokeAsync();

                    this.StateHasChanged();
                }
            }
        }
        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?"); ;

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<FollowUpControlUploadedFile>(this.model.IdFollowUpControlUploadedFile);
                    if (result == 1)
                    {
                        this.model.UploadedFileName = null;
                    }
                    await CallBackAfterSave.InvokeAsync();

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
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<FollowUpControlUploadedFile>(this.model.IdFollowUpControlUploadedFile);
                    if (hasFile)
                    {
                        var document = await this.UploadFileService.GetUploadedFileAsync<FollowUpControlUploadedFile>(this.model.IdFollowUpControlUploadedFile);
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
