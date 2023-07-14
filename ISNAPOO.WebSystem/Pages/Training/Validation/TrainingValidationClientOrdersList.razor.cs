using Data.Models.Data.Training;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    partial class TrainingValidationClientOrdersList : BlazorBaseComponent
    {
        [Parameter]
        public ValidationClientVM ClientVM { get; set; }

        [Parameter]
        public bool IsEditable { get; set; } = true;

        #region Inject
        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }
        #endregion

        private ToastMsg toast;
        private TrainingValidationClientOrderModal trainingValidationClientOrderModal = new TrainingValidationClientOrderModal();
        private SfGrid<ValidationOrderVM> sfGrid = new SfGrid<ValidationOrderVM>();
        private IEnumerable<ValidationOrderVM> validationOrdersSource = new List<ValidationOrderVM>();
        private ValidationOrderVM orderToDelete;


        protected override async Task OnInitializedAsync()
        {
            this.validationOrdersSource = (await TrainingService.GetAllValidationOrdersByIdValidationClient(ClientVM.IdValidationClient)).ToList();
            foreach (var validationOrder in this.validationOrdersSource)
            {

                if (validationOrder.HasUploadedFile)
                {
                    await SetFileNameAsync(validationOrder);
                }
            }
        }
        private void AddValidationOrderBtn()
        {
            this.trainingValidationClientOrderModal.OpenModal(new ValidationOrderVM() { IdValidationClient = ClientVM.IdValidationClient });
        }

        private async Task UpdateAfterSave()
        {
            this.validationOrdersSource = (await TrainingService.GetAllValidationOrdersByIdValidationClient(ClientVM.IdValidationClient)).ToList();
            foreach (var validationOrder in this.validationOrdersSource)
            {

                if (validationOrder.HasUploadedFile)
                {
                    await SetFileNameAsync(validationOrder);
                }
            }
        }

        private async Task EditValidationOrderBtn(ValidationOrderVM model)
        {
            this.trainingValidationClientOrderModal.OpenModal(model);
        }

        private async Task SetFileNameAsync(ValidationOrderVM courseOrder)
        {
            var settingResource = (await DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var fileFullName = settingResource + "\\" + courseOrder.UploadedFileName;
            if (Directory.Exists(fileFullName))
            {
                var files = Directory.GetFiles(fileFullName);
                files = files.Select(x => x.Split(($"\\{courseOrder.IdValidationOrder}\\"), StringSplitOptions.RemoveEmptyEntries).LastOrDefault()).ToArray();
                courseOrder.FileName = string.Join(Environment.NewLine, files);
            }
        }
        private async Task OnDownloadClick(string fileName, ValidationOrderVM courseOrder)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await UploadFileService.CheckIfExistUploadedFileAsync<ValidationOrder>(courseOrder.IdValidationOrder, fileName);
                if (hasFile)
                {
                    var documentStream = await UploadFileService.GetUploadedFileAsync<ValidationOrder>(courseOrder.IdValidationOrder, fileName);

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
                    var msg = LocService.GetLocalizedHtmlString("NotExistingFileForDownload");

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
        private async Task DeleteSelected(ValidationOrderVM _model)
        {
            bool hasPermission = await CheckUserActionPermission("ManageOrderData", false);
            if (!hasPermission) { return; }
            this.orderToDelete = _model;
            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (isConfirmed)
            {
                var resultContext = new ResultContext<ValidationOrderVM>();
                resultContext.ResultContextObject = this.orderToDelete;
                var files = this.orderToDelete.FileName.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToArray();
                foreach (var item in files)
                {
                    await UploadFileService.RemoveFileByIdAsync<ValidationOrder>(this.orderToDelete.IdValidationOrder);
                }
                await TrainingService.DeleteValidationOrderAsync(resultContext);
                var result =
                this.toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
                await this.toast.sfSuccessToast.ShowAsync();
                this.validationOrdersSource = (await TrainingService.GetAllValidationOrdersByIdValidationClient(ClientVM.IdValidationClient)).ToList();
                foreach (var validationOrder in this.validationOrdersSource)
                {

                    if (validationOrder.HasUploadedFile)
                    {
                        await SetFileNameAsync(validationOrder);
                    }
                }
                StateHasChanged();
            }
        }

    }
}
