using Data.Models.Data.Request;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class UploadRequestProtocolModal : BlazorBaseComponent
    {
        private SfUploader uploader = new SfUploader();

        private List<RequestDocumentManagementVM> requestDocumentManagements = new List<RequestDocumentManagementVM>();
        private RequestDocumentManagementUploadedFileVM model = new RequestDocumentManagementUploadedFileVM();
        private bool showDeleteConfirmDialog = false;

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(List<RequestDocumentManagementVM> requestDocumentManagements = null, RequestDocumentManagementUploadedFileVM requestDocumentManagementUploadedFile = null)
        {
            this.editContext = new EditContext(this.model);

            await this.uploader.ClearAllAsync();

            if (requestDocumentManagements is not null)
            {
                this.requestDocumentManagements = requestDocumentManagements.ToList();
            }
            else
            {
                this.requestDocumentManagements = null;
            }

            if (requestDocumentManagementUploadedFile is not null)
            {
                this.model = requestDocumentManagementUploadedFile;
            }
            else
            {
                this.model = new RequestDocumentManagementUploadedFileVM();
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        public override async void SubmitHandler()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var result = new ResultContext<NoResult>();
                if (this.requestDocumentManagements is not null)
                {
                    result = await this.ProviderDocumentRequestService.CreateRequestDocumentManagementUploadedFileByListRequestDocumentManagementAsync(this.requestDocumentManagements, this.model);
                }
                else
                {
                    result = await this.ProviderDocumentRequestService.CreateRequestDocumentManagementUploadedAsync(this.model);
                }

                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join("", result.ListMessages));
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
            var file = args.Files[0].Stream;

            string result = string.Empty;
            if (this.requestDocumentManagements is not null)
            {
                foreach (var docManagement in this.requestDocumentManagements)
                {
                    var managementUploadedFileFromDb = await this.ProviderDocumentRequestService.GetManagementUploadedFileByIdRequestDocumentManagementAsync(docManagement.IdRequestDocumentManagement);
                    result = await this.UploadFileService.UploadFileAsync<RequestDocumentManagementUploadedFile>(file, args.Files[0].FileInfo.Name, "RequestDocumentManagementUploadedFile", managementUploadedFileFromDb.IdRequestDocumentManagementUploadedFile);
                }
            }
            else
            {
                var managementUploadedFileFromDb = await this.ProviderDocumentRequestService.GetManagementUploadedFileByIdRequestDocumentManagementAsync(this.model.IdRequestDocumentManagement);
                result = await this.UploadFileService.UploadFileAsync<RequestDocumentManagementUploadedFile>(file, args.Files[0].FileInfo.Name, "RequestDocumentManagementUploadedFile", managementUploadedFileFromDb.IdRequestDocumentManagementUploadedFile);
            }

            if (!string.IsNullOrEmpty(result))
            {
                this.model.UploadedFileName = result;

                await this.CallbackAfterSubmit.InvokeAsync();
            }

            this.StateHasChanged();
        }

        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                this.showDeleteConfirmDialog = !this.showDeleteConfirmDialog;
                this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
            }
        }

        private async Task OnRemove(string fileName)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                this.showDeleteConfirmDialog = !this.showDeleteConfirmDialog;
                this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
            }
        }

        private async void OnDownloadClick()
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
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<RequestDocumentManagementUploadedFile>(this.model.IdRequestDocumentManagementUploadedFile);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<RequestDocumentManagementUploadedFile>(this.model.IdRequestDocumentManagementUploadedFile);
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
        public async void ConfirmDeleteCallback()
        {
            this.showDeleteConfirmDialog = false;

            int result = 0;
            if (this.requestDocumentManagements is not null)
            {
                foreach (var docManagement in this.requestDocumentManagements)
                {
                    var docManagementUploadedFile = await this.ProviderDocumentRequestService.GetManagementUploadedFileByIdRequestDocumentManagementAsync(docManagement.IdRequestDocumentManagement);
                    result = await this.UploadFileService.RemoveFileByIdAsync<RequestDocumentManagementUploadedFile>(docManagementUploadedFile.IdRequestDocumentManagementUploadedFile);
                }
            }
            else
            {
                result = await this.UploadFileService.RemoveFileByIdAsync<RequestDocumentManagementUploadedFile>(this.model.IdRequestDocumentManagementUploadedFile);
            }

            if (result == 1)
            {
                this.model.UploadedFileName = null;

                await this.CallbackAfterSubmit.InvokeAsync();
            }

            this.StateHasChanged();

        }
    }

}
