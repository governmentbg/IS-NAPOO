using Data.Models.Data.Archive;
using Data.Models.Data.Common;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Archive.SelfAssessment
{
    public partial class SelfAssessmentApproveRejectModal : BlazorBaseComponent
    {
        [Inject]
        public IArchiveService ArchiveService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Parameter]
        public EventCallback<bool> CallbackAfterSubmit { get; set; }

        private SelfAssessmentReportVM selfAssessmentReportVM = new SelfAssessmentReportVM();
        private SelfAssessmentReportStatusVM selfAssessmentReportStatusVM = new SelfAssessmentReportStatusVM();
        private List<SelfAssessmentReportVM> selfAssessmentListVM = new List<SelfAssessmentReportVM>();
        private ValidationMessageStore? validationMessageStore;
        private KeyValueVM keyValueAppOrRej = new KeyValueVM();
        private List<string> validationMessages = new List<string>();
        private SfDialog sfDialog;
        private string rejectOrApprove;
        public override bool IsContextModified => this.editContext.IsModified();
        public async Task OpenModal(SelfAssessmentReportVM selfAssessmentVM, string rejOrApp, List<SelfAssessmentReportVM> selfAssessmentList)
        {
            this.validationMessages.Clear();
                   
            this.rejectOrApprove = rejOrApp;
            this.selfAssessmentReportVM = selfAssessmentVM;
            this.selfAssessmentListVM = selfAssessmentList;
            selfAssessmentReportStatusVM = new SelfAssessmentReportStatusVM();
            this.editContext = new EditContext(selfAssessmentReportStatusVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.isVisible = true;
            this.StateHasChanged();
        }
        public async Task Submit()
        {
            this.validationMessages.Clear();
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.validationMessageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidateCode;

                if (this.rejectOrApprove == "Reject")
                {
                    this.keyValueAppOrRej = await this.DataSourceService.GetKeyValueByIntCodeAsync("SelfAssessmentReportStatus", "Returned");
                }
                else if (this.rejectOrApprove == "Approve")
                {
                    this.keyValueAppOrRej = await this.DataSourceService.GetKeyValueByIntCodeAsync("SelfAssessmentReportStatus", "Approved");
                }        

                if (this.editContext.Validate())
                {
                    this.SpinnerShow();

                    var result = new ResultContext<NoResult>();

                    if (selfAssessmentReportVM is not null)
                    {
                        result = await this.ArchiveService.SaveSelfAssessmentReportApproveOrRejectStatusAsync(this.selfAssessmentReportVM.IdSelfAssessmentReport, this.keyValueAppOrRej.IdKeyValue, this.selfAssessmentReportStatusVM.Comment);
                        result = await this.ArchiveService.UpdateSelfAssessmentReportAppOrRej(this.selfAssessmentReportVM, this.keyValueAppOrRej.IdKeyValue);
                    }
                    else if (selfAssessmentReportVM is null && selfAssessmentListVM.Any())
                    {
                        result = await this.ArchiveService.SaveSelfAssessmentReportsApproveOrRejectStatusAsync(this.selfAssessmentListVM, this.keyValueAppOrRej.IdKeyValue, this.selfAssessmentReportStatusVM.Comment);
                       // result = await this.ArchiveService.UpdateSelfAssessmentReportsApproveOrRejectStatuses(this.selfAssessmentListVM);
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        if (this.rejectOrApprove == "Approve")
                        {
                            await this.ShowSuccessAsync(string.Join(Environment.NewLine, "Доклада за самооценка е одобрен успешно."));
                        }
                        else if (this.rejectOrApprove == "Reject")
                        {
                            await this.ShowSuccessAsync(string.Join(Environment.NewLine, "Доклада за самооценка е върнат успешно."));
                        }
                        await this.CallbackAfterSubmit.InvokeAsync();
                    }
                }
                else
                {
                    this.validationMessages.AddRange(this.editContext.GetValidationMessages());
                    await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                }
            }
            finally
            {
                this.loading = false;
            }
            this.SpinnerHide();
            this.isVisible = false;
        }

        private void ValidateCode(object? sender, ValidationRequestedEventArgs e)
        {
            this.validationMessageStore?.Clear();

            var annInfoStatusComment = this.selfAssessmentReportStatusVM.Comment;

            FieldIdentifier fi = new FieldIdentifier();

            if (annInfoStatusComment == null)
            {
                fi = new FieldIdentifier(this.selfAssessmentReportStatusVM, "Comment");
                this.validationMessageStore?.Add(fi, "Полето 'Коментар' е задължително!");
            }
        }
    }
}
