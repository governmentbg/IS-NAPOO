using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ApplicationNumberChangeModal : BlazorBaseComponent
    {
        private CandidateProviderVM candidateProvider = new CandidateProviderVM();
        private ValidationMessageStore? validationMessageStore;
        private List<string> validationMessages = new List<string>();

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.candidateProvider);
        }

        public void OpenModal(CandidateProviderVM candidateProviderVM)
        {         
            this.validationMessages.Clear();

            this.candidateProvider = candidateProviderVM;

            this.editContext = new EditContext(this.candidateProvider);

            this.isVisible = true;
            this.StateHasChanged();
        }

        public async Task Submit()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.validationMessages.Clear();

                this.editContext = new EditContext(this.candidateProvider);
                this.editContext.EnableDataAnnotationsValidation();
                this.validationMessageStore = new ValidationMessageStore(this.editContext);
               // this.editContext.OnValidationRequested += this.ValidateCode;
                if (this.editContext.Validate())
                {
                    var result = new ResultContext<CandidateProviderVM>();

                    result.ResultContextObject = this.candidateProvider;
                    result = await this.CandidateProviderService.UpdateCandidateProviderApplicationNumber(result);

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

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
    }
}
