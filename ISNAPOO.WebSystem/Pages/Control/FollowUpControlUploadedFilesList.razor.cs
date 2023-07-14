using Data.Models.Data.Control;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Control
{
    partial class FollowUpControlUploadedFilesList : BlazorBaseComponent
    {
        [Parameter]
        public bool IsEditable { get; set; }

        [Parameter]
        public FollowUpControlVM Model { get; set; }

        #region Inject
        [Inject]

        public IControlService ControlService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        #endregion


        private ToastMsg toast;
        private SfGrid<FollowUpControlUploadedFileVM> sfGrid = new SfGrid<FollowUpControlUploadedFileVM>();
        private FollowUpControlUploadedFileModal followUpControlUploadedFileModal = new FollowUpControlUploadedFileModal();
        private IEnumerable<FollowUpControlUploadedFileVM> documentsSource = new List<FollowUpControlUploadedFileVM>();
        private FollowUpControlUploadedFileVM documentToDelete = new FollowUpControlUploadedFileVM();

        protected override async Task OnInitializedAsync()
        {
        }

        //public async Task OpenModal(FollowUpControlVM _model)
        //{
        //    if (_model.IdFollowUpControl != 0)
        //    {
        //        this.followUpControl = _model;
        //    }

        //    this.documentsSource = await this.ControlService.GetAllUploadedFilesByIdFollowUpControl(this.followUpControl.IdFollowUpControl);
        //    this.StateHasChanged();
        //}

        public async Task LoadData()
        {
            if (this.Model.IdFollowUpControl != 0)
            {
                this.documentsSource = await this.ControlService.GetAllUploadedFilesByIdFollowUpControl(this.Model.IdFollowUpControl);
            }

            this.StateHasChanged();
            this.LoadData();
        }

        private async Task AddNewDocument()
        {
            await this.followUpControlUploadedFileModal.OpenModal(new FollowUpControlUploadedFileVM() { IdFollowUpControl = this.Model.IdFollowUpControl });
        }

        private async Task GetDocument(FollowUpControlUploadedFileVM model)
        {
            await this.followUpControlUploadedFileModal.OpenModal(model);
        }

        private async Task CallBackAfterSave()
        {
            this.documentsSource = await this.ControlService.GetAllUploadedFilesByIdFollowUpControl(this.Model.IdFollowUpControl);
        }

        private async Task OnDownloadClick(FollowUpControlUploadedFileVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!string.IsNullOrEmpty(model.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<FollowUpControlUploadedFile>(model.IdFollowUpControlUploadedFile);
                    if (hasFile)
                    {
                        var document = await this.UploadFileService.GetUploadedFileAsync<FollowUpControlUploadedFile>(model.IdFollowUpControlUploadedFile);

                        if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, model.FileName, document.MS!.ToArray());
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

        private async Task DeleteDocument(FollowUpControlUploadedFileVM model)
        {

            this.documentToDelete = model;
            this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
        }

        public async void ConfirmDeleteCallback()
        {
            var resultContext = await this.ControlService.DeleteFollowUpControlUploadedFileAsync(this.documentToDelete);

            if (resultContext.HasErrorMessages)
            {
                this.toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                await this.toast.sfErrorToast.ShowAsync();
            }
            else
            {
                this.toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                await this.toast.sfSuccessToast.ShowAsync();

                this.documentsSource = await this.ControlService.GetAllUploadedFilesByIdFollowUpControl(this.Model.IdFollowUpControl);


                this.StateHasChanged();
            }
        }
    }
}
