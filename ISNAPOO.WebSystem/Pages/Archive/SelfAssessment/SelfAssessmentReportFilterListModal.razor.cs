using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Register;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.Archive.SelfAssessment
{
    public partial class SelfAssessmentReportFilterListModal :  BlazorBaseComponent
    {
        private SelfAssessmentReportListFilterVM selfAssessmentReportListFilterVM = new SelfAssessmentReportListFilterVM();
        private List<SelfAssessmentReportVM> selfAssessmentReportsList = new List<SelfAssessmentReportVM>();
        ValidationMessageStore? messageStore;

        [Parameter]
        public EventCallback<SelfAssessmentReportListFilterVM> CallbackAfterSubmit { get; set; }
        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.selfAssessmentReportListFilterVM);
            this.messageStore = new ValidationMessageStore(this.editContext);
        }
        public async void OpenModal(int year)
        {
            this.editContext = new EditContext(this.selfAssessmentReportListFilterVM);

            if (selfAssessmentReportListFilterVM.Year == 0)
            {
                this.selfAssessmentReportListFilterVM.Year = year;
            }
            
            this.isVisible = true;
            this.StateHasChanged();
        }
        public void ClearBtn()
        {
            this.selfAssessmentReportListFilterVM = new SelfAssessmentReportListFilterVM();
        }
        private async Task SearchBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
             

                if (this.selfAssessmentReportListFilterVM.Year == null && this.selfAssessmentReportListFilterVM.FillingDateFrom == null
                   && this.selfAssessmentReportListFilterVM.FillingDateTo == null) 
                {
                    await this.ShowErrorAsync("Моля, изберете поне един критерий, по който да филтрирате данни за докладите за самооценка!");
                    this.loading = false;
                    this.SpinnerHide();
                    return;
                }

                if (this.selfAssessmentReportListFilterVM.Year != null && this.selfAssessmentReportListFilterVM.Year.ToString().Length != 4)
                {
                    await this.ShowErrorAsync("Моля, въведете валидна стойност в полето 'Година'!");
                    return;
                }
                await this.CallbackAfterSubmit.InvokeAsync(this.selfAssessmentReportListFilterVM);

                this.isVisible = false;
                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
  
        }
    }
}
