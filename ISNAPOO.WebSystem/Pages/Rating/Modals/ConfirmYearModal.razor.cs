using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Services.Rating;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Rating;
using Microsoft.AspNetCore.Components;

namespace ISNAPOO.WebSystem.Pages.Rating.Modals
{
    public partial class ConfirmYearModal
    {
        [Inject]
        IRatingService ratingService { get; set; }
        List<KeyValueVM> ValidIndicatorTypes { get;set; }
        private int Year { get; set; } = DateTime.Now.Year;
        private string LicensingType { get; set; }
        protected override async Task OnInitializedAsync()
        {
            this.isVisible = false;
            this.StateHasChanged();
        }

        public async Task OpenModal(string LicensingType)
        {
            this.LicensingType = LicensingType;
            this.isVisible = true;
            this.StateHasChanged();
        }
        public async Task CalculateData()
        {
            this.SpinnerShow();
            this.ValidIndicatorTypes = (await this.ratingService.GetValidIndicatorsTypesByYearAsync(this.Year)).Where(x => x.DefaultValue2.Contains(this.LicensingType)).ToList();
            if (ValidIndicatorTypes.Any())
            {
                if (LicensingType == "CPO")
                {
                    await this.ratingService.StartCalculation(this.ValidIndicatorTypes, Year);
                }
                else
                {
                    await this.ratingService.StartCalculationCIPO(this.ValidIndicatorTypes, Year);
                }
                
            }          
            this.SpinnerHide();
            this.isVisible = false;
            this.StateHasChanged();
        }
    }
}
