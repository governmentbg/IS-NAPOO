using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.SPPOO.Modals.LegalCapacityOrdinance
{
    partial class LegalCapacityOrdinanceModal : BlazorBaseComponent
    {
        [Parameter]
        public EventCallback CallBackAfterSave { get; set; }

        #region Inject
        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public ILegalCapacityOrdinanceService LegalCapacityOrdinanceService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        #endregion

        private IEnumerable<KeyValueVM> kvLegalCapacityOrdinanceTypeSource = new List<KeyValueVM>();
        private LegalCapacityOrdinanceUploadedFileVM model = new LegalCapacityOrdinanceUploadedFileVM();
        private SfUploader uploader = new SfUploader();
        private ToastMsg toast;
        private string CreationDateStr = string.Empty;
        private string ModifyDateStr = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.model);
            this.kvLegalCapacityOrdinanceTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LegalCapacityOrdinanceType");
        }

        public async Task OpenModal(LegalCapacityOrdinanceUploadedFileVM _model)
        {
            if (_model.IdLegalCapacityOrdinanceUploadedFile != 0)
            {
                this.model = _model;
                CreationDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                ModifyDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);
            }
            else
            {
                this.model = _model;
                ModifyDateStr = "";
                CreationDateStr = "";
                this.model.CreatePersonName = "";
                this.model.ModifyPersonName = "";
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
                    int result = 0;
                    if (model.IdLegalCapacityOrdinanceUploadedFile == 0)
                    {
                        result = await this.LegalCapacityOrdinanceService.CreateOrdinance(model);
                    }
                    else
                    {
                        result = await this.LegalCapacityOrdinanceService.UpdateOrdinanceAsync(model);
                    }

                    if (result > 0)
                    {
                        this.model = await this.LegalCapacityOrdinanceService.GetOrdinanceByIdAsync(result);

                        this.CreationDateStr = this.model.ModifyDate.ToString("dd.MM.yyyy");
                        this.ModifyDateStr = this.model.CreationDate.ToString("dd.MM.yyyy");
                        this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
                        this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);

                        this.toast.sfSuccessToast.Content = "Записът е успешен!";
                        await toast.sfSuccessToast.ShowAsync();
                    }
                    else
                    {

                        this.toast.sfErrorToast.Content = "В системата вече има въведена информация за същата наредба за правоспособност!";
                        await toast.sfErrorToast.ShowAsync();
                    }


                    await CallBackAfterSave.InvokeAsync();
                }

                this.StateHasChanged();
            }
            finally
            {
                this.SpinnerHide();
                this.loading = false;
            }
        }
        private async Task OnChange(UploadChangeEventArgs args)
        {
            var file = args.Files[0].Stream;

            var result = await this.UploadFileService.UploadFileAsync<LegalCapacityOrdinanceUploadedFile>(file, args.Files[0].FileInfo.Name, "LegalCapacityOrdinances", this.model.IdLegalCapacityOrdinanceUploadedFile);
            if (!string.IsNullOrEmpty(result))
            {
                this.model.UploadedFileName = result;
            }

            await CallBackAfterSave.InvokeAsync();

            await this.uploader.ClearAllAsync();

            this.StateHasChanged();
        }
        private async Task OnRemoveClick(RemovingEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.model.UploadedFileName))
            {
                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли си сте, че искате да изтриете прикачения файл?");

                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<LegalCapacityOrdinanceUploadedFile>(this.model.IdLegalCapacityOrdinanceUploadedFile);
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
                    var result = await this.UploadFileService.RemoveFileByIdAsync<LegalCapacityOrdinanceUploadedFile>(this.model.IdLegalCapacityOrdinanceUploadedFile);
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
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<LegalCapacityOrdinanceUploadedFile>(this.model.IdLegalCapacityOrdinanceUploadedFile);
                    if (hasFile)
                    {
                        var document = await this.UploadFileService.GetUploadedFileAsync<LegalCapacityOrdinanceUploadedFile>(this.model.IdLegalCapacityOrdinanceUploadedFile);
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
