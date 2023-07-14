using Data.Models.Data.Request;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class CPODocumentDestructionDocumentModal : BlazorBaseComponent
    {
        SfDialog sfDialog = new SfDialog();
        ToastMsg toast = new ToastMsg();

        private ReportUploadedDocVM reportUploadedDocVM = new ReportUploadedDocVM();
        private IEnumerable<KeyValueVM> kvDocumentTypeSource = new List<KeyValueVM>();

        [Parameter]
        public EventCallback<ReportUploadedDocVM> CallbackAfterModalSubmit { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.reportUploadedDocVM);
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

                this.editContext = new EditContext(this.reportUploadedDocVM);
                this.editContext.EnableDataAnnotationsValidation();

                if (string.IsNullOrEmpty(this.reportUploadedDocVM.Description))
                {
                    this.reportUploadedDocVM.Description = string.Empty;
                }

                if (this.editContext.Validate())
                {
                    ResultContext<ReportUploadedDocVM> resultContext = new ResultContext<ReportUploadedDocVM>();

                    if (this.reportUploadedDocVM.IdReportUploadedDoc != 0)
                    {
                        resultContext = await this.ProviderDocumentRequestService.UpdateReportUploadedDocumentAsync(this.reportUploadedDocVM);
                    }
                    else
                    {
                        resultContext = await this.ProviderDocumentRequestService.CreateReportUploadedDocumentAsync(this.reportUploadedDocVM);
                    }

                    if (resultContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                        await this.CallbackAfterModalSubmit.InvokeAsync(this.reportUploadedDocVM);
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public async Task OpenModal(ReportUploadedDocVM reportUploadedDocVM, IEnumerable<KeyValueVM> kvDocumentTypeSource)
        {
            if (reportUploadedDocVM.IdReportUploadedDoc != 0)
            {
                this.reportUploadedDocVM = await this.ProviderDocumentRequestService.GetReportUploadedDocumentByIdAsync(reportUploadedDocVM.IdReportUploadedDoc);
            }
            else
            {
                this.reportUploadedDocVM = reportUploadedDocVM;
            }



            this.kvDocumentTypeSource = kvDocumentTypeSource;

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task OnRemoveClick()
        {
            if (this.reportUploadedDocVM.IdReportUploadedDoc > 0)
            {
                bool isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?");
                if (isConfirmed)
                {
                    var result = await this.UploadFileService.RemoveFileByIdAsync<ReportUploadedDoc>(this.reportUploadedDocVM.IdReportUploadedDoc);

                    if (result == 1)
                    {
                        this.reportUploadedDocVM = await this.ProviderDocumentRequestService.GetReportUploadedDocumentByIdAsync(this.reportUploadedDocVM.IdReportUploadedDoc);

                        this.StateHasChanged();
                    }
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

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ReportUploadedDoc>(this.reportUploadedDocVM.IdReportUploadedDoc);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<ReportUploadedDoc>(this.reportUploadedDocVM.IdReportUploadedDoc);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, this.reportUploadedDocVM.FileName, documentStream.MS!.ToArray());
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

        private async Task OnRemove(RemovingEventArgs args)
        {
            if (args.FilesData.Count == 1)
            {
                if (this.reportUploadedDocVM.IdReportUploadedDoc > 0)
                {
                    bool isConfirmed = true;
                    if (args.FilesData[0].Name == this.reportUploadedDocVM.FileName)
                    {
                        isConfirmed = await this.JsRuntime.InvokeAsync<bool>("confirm", "Сигурни ли си сте, че искате да изтриете прикачения файл?");
                    }

                    if (isConfirmed)
                    {
                        var result = await this.UploadFileService.RemoveFileByIdAsync<ReportUploadedDoc>(this.reportUploadedDocVM.IdReportUploadedDoc);

                        if (result == 1)
                        {
                            this.reportUploadedDocVM = await this.ProviderDocumentRequestService.GetReportUploadedDocumentByIdAsync(this.reportUploadedDocVM.IdReportUploadedDoc);

                            this.StateHasChanged();
                        }
                    }
                }
            }
        }

        private async Task OnChange(UploadChangeEventArgs args)
        {
            if (args.Files.Count == 1)
            {
                var fileName = args.Files[0].FileInfo.Name;

                var result = await this.UploadFileService.UploadFileAsync<ReportUploadedDoc>(args.Files[0].Stream, fileName, "RequestReportDocument", this.reportUploadedDocVM.IdReportUploadedDoc);
                if (!string.IsNullOrEmpty(result))
                {
                    this.reportUploadedDocVM.UploadedFileName = result;
                }

                this.StateHasChanged();

                this.editContext = new EditContext(this.reportUploadedDocVM);
            }
        }
    }
}
