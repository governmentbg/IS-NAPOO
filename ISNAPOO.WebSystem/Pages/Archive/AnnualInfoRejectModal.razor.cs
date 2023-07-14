using System.Security.AccessControl;
using Data.Models.Data.Archive;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Archive
{
    public partial class AnnualInfoRejectModal : BlazorBaseComponent
    {
        
        [Inject]
        public IArchiveService ArchiveService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Parameter]
        public EventCallback<bool> CallbackAfterSubmit { get; set; }

        private AnnualInfoStatus annualInfoStatus = new AnnualInfoStatus();
        private AnnualInfoVM annualInfoVM = new AnnualInfoVM();
        private ValidationMessageStore? validationMessageStore;
        private List<CandidateProviderVM> candidateProviders = new List<CandidateProviderVM>();   
        private List<string> validationMessages = new List<string>();
        private SfDialog sfDialog;
        private int year;
        public override bool IsContextModified => this.editContext.IsModified();
        public async Task OpenModal(List<CandidateProviderVM> candidateProviderVM, int year)
        {
            this.validationMessages.Clear();
            this.isVisible = true;
            candidateProviders = candidateProviderVM;
            this.year = year;
            annualInfoStatus = new AnnualInfoStatus();
            this.editContext = new EditContext(annualInfoStatus);
            this.editContext.EnableDataAnnotationsValidation();            
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
                var kvReturned = await this.DataSourceService.GetKeyValueByIntCodeAsync("AnnualInfoStatusType", "Returned");
                var kvSubmitted = await this.DataSourceService.GetKeyValueByIntCodeAsync("AnnualInfoStatusType", "Submitted");
                var kvCipoLicense = await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCIPO");
                var result = new ResultContext<NoResult>();
                var isArchiveOrNot = "MakeIsArchiveFalse";

                if (this.editContext.Validate())
                {
                    this.SpinnerShow();

                    var res = new ResultContext<AnnualInfoStatus>();
                    res.ResultContextObject = this.annualInfoStatus;
                    
                    foreach (var candidateProvAnnualInfo in candidateProviders)
                    {
                        annualInfoVM = this.ArchiveService.GetAnnualInfoByCandProvIdYearAndKeySubmittedIntCode(candidateProvAnnualInfo.IdCandidate_Provider, year, kvSubmitted.IdKeyValue);
                        
                        if (annualInfoVM != null)
                        {
                           
                            result = await this.ArchiveService.SaveArchAnnualInfoStatus(annualInfoVM.IdAnnualInfo, kvReturned.IdKeyValue, this.annualInfoStatus.Comment);
                            annualInfoVM.IdStatus = kvReturned.IdKeyValue;
                            result = await this.ArchiveService.UpdateAnnualInfo(annualInfoVM, "");
                            result = await this.ArchiveService.UpdateFinishedCoursesStatusIsArchive(candidateProvAnnualInfo.IdCandidate_Provider, isArchiveOrNot, year);
                            if (candidateProvAnnualInfo.IdTypeLicense == kvCipoLicense.IdKeyValue)
                            {
                                result = await this.ArchiveService.UpdateTrainingConsultingClientStatusIsArchive(candidateProvAnnualInfo.IdCandidate_Provider, isArchiveOrNot);
                            }
                            result = await this.ArchiveService.UpdateFinishedValidationClientCoursesStatusIsArchive(candidateProvAnnualInfo.IdCandidate_Provider,isArchiveOrNot, year);
                            result = await this.ArchiveService.DeleteArchiveCandidateProviderData(candidateProvAnnualInfo.IdCandidate_Provider, year);
                        }
                    }
                    
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, "Годишните отчети са върнати успешно."));
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
        }

        private void ValidateCode(object? sender, ValidationRequestedEventArgs e)
        {
            this.validationMessageStore?.Clear();

            var annInfoStatusComment = this.annualInfoStatus.Comment;

            FieldIdentifier fi = new FieldIdentifier();

            if (annInfoStatusComment == null)
            {
                fi = new FieldIdentifier(this.annualInfoStatus, "Comment");
                this.validationMessageStore?.Add(fi, "Полето 'Коментар' е задължително!");
            }
        }
    }
}
