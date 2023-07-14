using ISNAPOO.Core.Services.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.EGovPayment
{
    public partial class PaymentFeeListModal : BlazorBaseComponent
    {
        private SfDialog sfDialog;
        private CandidateProviderVM candidateProviderVM = new CandidateProviderVM();

        public async Task openPaymentFeeList(CandidateProviderVM CandProv)
        {
            this.candidateProviderVM = CandProv;           
            this.isVisible = true;
            this.StateHasChanged();
        }
    }
}
