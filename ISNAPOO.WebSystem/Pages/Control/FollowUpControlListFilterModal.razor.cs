using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;

namespace ISNAPOO.WebSystem.Pages.Control
{
    partial class FollowUpControlListFilterModal : BlazorBaseComponent
    {

        [Parameter]

        public EventCallback<List<FollowUpControlVM>> CallbackAfterSubmit { get; set; }

        [Inject]
        public IExpertService ExpertService {get; set;}

        private ToastMsg toast;
        private FollowUpControlFilterVM followUpControlfilter = new FollowUpControlFilterVM();
        private List<FollowUpControlVM> followUpControlSource = new List<FollowUpControlVM>();
        private IEnumerable<ExpertVM> experts = new List<ExpertVM>();
        private bool IsDateValid = true;
        private bool IsDateLimitValid = true;

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.followUpControlfilter);
            this.experts = await ExpertService.GetAllExpertsAsync(new ExpertVM() { IsNapooExpert = true });
        }

        public void OpenModal(List<FollowUpControlVM> followUpControlSource)
        {
            this.editContext = new EditContext(this.followUpControlfilter);

            this.followUpControlSource = followUpControlSource.ToList();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task SearchBtn()
        {
            this.editContext = new EditContext(this.followUpControlfilter);
            this.editContext.EnableDataAnnotationsValidation();
            if (IsDateValid && IsDateLimitValid)
            {
                if (this.editContext.Validate())
                {
                    this.followUpControlSource = this.FilterFollowUpControls(this.followUpControlSource);

                    this.isVisible = false;

                    await this.CallbackAfterSubmit.InvokeAsync(this.followUpControlSource);
                }
            }
        }

        private async void ClearBtn()
        {
            this.followUpControlfilter = new FollowUpControlFilterVM();
            await this.CallbackAfterSubmit.InvokeAsync(this.followUpControlSource);
        }
        private async Task DateValid()
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime(); ;
            if (this.followUpControlfilter.ControlStartDate.HasValue)
            {
                startDate = this.followUpControlfilter.ControlStartDate.Value.Date;
            }
            if (this.followUpControlfilter.ControlEndDate.HasValue)
            {
                endDate = this.followUpControlfilter.ControlEndDate.Value.Date;

            }
            int result = DateTime.Compare(startDate, endDate);

            if (result > 0 && this.followUpControlfilter.ControlEndDate.HasValue && this.followUpControlfilter.ControlStartDate.HasValue)
            {
                this.IsDateValid = false;

                toast.sfErrorToast.Content = $"Въведената дата в полето 'Срок за проверката от (Начало)' не може да е след 'Срок за проверката до (Начало)'!";
                await toast.sfErrorToast.ShowAsync();
            }
            else
            {
                this.IsDateValid = true;
            }
        }
        private async Task DateValidLimit(string mode)
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            if (mode == "LimitStart")
            {
                if (this.followUpControlfilter.ControlStartDate.HasValue)
                {
                    startDate = this.followUpControlfilter.ControlStartDate.Value.Date;
                }
                if (this.followUpControlfilter.ControlStartDateLimit.HasValue)
                {
                    endDate = this.followUpControlfilter.ControlStartDateLimit.Value.Date;
                }
            }
            else
            {
                if (this.followUpControlfilter.ControlEndDate.HasValue)
                {
                    startDate = this.followUpControlfilter.ControlEndDate.Value.Date;
                }
                if (this.followUpControlfilter.ControlEndDateLimit.HasValue)
                {
                    endDate = this.followUpControlfilter.ControlEndDateLimit.Value.Date;
                }
            }

            int result = DateTime.Compare(startDate, endDate);

            if (result > 0)
            {
                this.IsDateLimitValid = false;
                if (mode == "LimitStart")
                {
                    toast.sfErrorToast.Content = $"Въведената дата в полето 'Срок за проверката от (Начало)' не може да е след 'Срок за проверката от (Край)'!";
                    await toast.sfErrorToast.ShowAsync();
                }
                else
                {
                    toast.sfErrorToast.Content = $"Въведената дата в полето 'Срок за проверката до (Начало)' не може да е след 'Срок за проверката до (Край)'!";
                    await toast.sfErrorToast.ShowAsync();
                }
            }
            else
            {
                this.IsDateLimitValid = true;
            }
        }

        private List<FollowUpControlVM> FilterFollowUpControls(List<FollowUpControlVM> source)
        {
            var filteredSource = source;
            if (followUpControlfilter.IdFollowUpControlExpert == 0
            && followUpControlfilter.ControlStartDate is null
            && followUpControlfilter.ControlEndDate is null
            && followUpControlfilter.ControlEndDateLimit is null
            && followUpControlfilter.ControlStartDateLimit is null)
            {
                return filteredSource;
            }

            if (followUpControlfilter.IdFollowUpControlExpert != 0)
            {
                filteredSource = filteredSource.Where(x => x.FollowUpControlExperts.Any(x => x.IdExpert == followUpControlfilter.IdFollowUpControlExpert)).ToList();
            }

            if (followUpControlfilter.ControlStartDate.HasValue && followUpControlfilter.ControlStartDateLimit.HasValue)
            {
                filteredSource = filteredSource.Where(x => x.ControlStartDate.Value >= followUpControlfilter.ControlStartDate.Value && x.ControlStartDate.Value <= followUpControlfilter.ControlStartDateLimit.Value).ToList();
            }
            else if (followUpControlfilter.ControlStartDate.HasValue && !followUpControlfilter.ControlStartDateLimit.HasValue)
            {
                filteredSource = filteredSource.Where(x => x.ControlStartDate.Value >= followUpControlfilter.ControlStartDate.Value).ToList();
            }
            else if (!followUpControlfilter.ControlStartDate.HasValue && followUpControlfilter.ControlStartDateLimit.HasValue)
            {
                filteredSource = filteredSource.Where(x => x.ControlStartDate.Value <= followUpControlfilter.ControlStartDateLimit.Value).ToList();
            }

            if (followUpControlfilter.ControlEndDate.HasValue && followUpControlfilter.ControlEndDateLimit.HasValue)
            {
                filteredSource = filteredSource.Where(x => x.ControlEndDate.Value >= followUpControlfilter.ControlEndDate.Value && x.ControlEndDate.Value <= followUpControlfilter.ControlEndDateLimit.Value).ToList();
            }
            else if (followUpControlfilter.ControlEndDate.HasValue && !followUpControlfilter.ControlEndDateLimit.HasValue)
            {
                filteredSource = filteredSource.Where(x => x.ControlEndDate.Value >= followUpControlfilter.ControlEndDate.Value).ToList();
            }
            else if (!followUpControlfilter.ControlEndDate.HasValue && followUpControlfilter.ControlEndDateLimit.HasValue)
            {
                filteredSource = filteredSource.Where(x => x.ControlEndDate.Value <= followUpControlfilter.ControlEndDateLimit.Value).ToList();
            }
            return filteredSource;
        }
    }
    public class FollowUpControlFilterVM
    {
        public int IdFollowUpControlExpert { get; set; }
        public DateTime? ControlStartDate { get; set; }
        public DateTime? ControlStartDateLimit { get; set; }
        public DateTime? ControlEndDate { get; set; }
        public DateTime? ControlEndDateLimit { get; set; }
    }
}
