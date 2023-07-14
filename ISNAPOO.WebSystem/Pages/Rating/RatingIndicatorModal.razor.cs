using Data.Models.Data.Rating;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Services.Rating;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Rating;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.Rating
{
    public partial class RatingIndicatorModal : BlazorBaseComponent
    {
        [Parameter]
        public EventCallback<bool> CallBackAfterSumbit { get; set; }

        [Inject]
        IRatingService ratingService { get; set; }

        [Inject]
        IDataSourceService dataSourceService { get; set; }

        List<KeyValueVM> kvTypesOfIndicators = new List<KeyValueVM>();

        IndicatorVM indicator = new IndicatorVM();

        List<string> ValidationMessages = new List<string>();
        public int? TypeOfIndicator { get; set; }
        public bool IsTypeOfIndicatorSelected { get; set; } = false;
        public bool IsValid { get; set; } = true;
        protected override async Task OnInitializedAsync()
        {
            this.kvTypesOfIndicators = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RatingIndicatorType", false)).ToList();
        }
        public async Task OpenModal(IndicatorVM indicator, string TypeLicense)
        {
            ValidationMessages.Clear();
            this.kvTypesOfIndicators = this.kvTypesOfIndicators.Where(x => x.DefaultValue2.Contains(TypeLicense)).ToList();
            this.indicator = indicator;
            this.TypeOfIndicator = indicator.IdIndicatorType;
            editContext = new Microsoft.AspNetCore.Components.Forms.EditContext(this.indicator);
            this.IsTypeOfIndicatorSelected = true;
            this.isVisible = true;
            this.StateHasChanged();
        }

        public async Task OpenModal(string TypeLicense)
        {
            ValidationMessages.Clear();
            this.kvTypesOfIndicators = this.kvTypesOfIndicators.Where(x => x.DefaultValue2.Contains(TypeLicense)).ToList();
            this.IsTypeOfIndicatorSelected = false;
            this.TypeOfIndicator = null;
            this.indicator = new IndicatorVM();
            this.indicator.Year = int.Parse(DateTime.Now.Year.ToString());
            editContext = new Microsoft.AspNetCore.Components.Forms.EditContext(this.indicator);
            this.isVisible = true;
            this.StateHasChanged();
        }

        public void SelectedType()
        {
            IsTypeOfIndicatorSelected = true;
        }

        public async Task CreateNewIndicator()
        {
            this.SpinnerShow();
            this.editContext = new EditContext(this.indicator);
            this.editContext.EnableDataAnnotationsValidation();

            this.ValidationMessages.Clear();
            this.indicator.IndicatorDetails.DefaultValue1 = this.kvTypesOfIndicators.First(y => y.IdKeyValue == TypeOfIndicator).DefaultValue1;
            this.IsValid = this.editContext.Validate();
            this.ValidationMessages.AddRange(this.editContext.GetValidationMessages().ToList());
            if (IsValid)
            {
                this.indicator.IdIndicatorType = (int)this.TypeOfIndicator;
                if (await ValidateIndicator(this.indicator))
                {
                    ResultContext<IndicatorVM> result = new ResultContext<IndicatorVM>();
                    if (this.indicator.IdIndicator == 0)
                    {
                        result.ResultContextObject = this.indicator;
                        result = await this.ratingService.CreateIndicatorAsync(result);
                    }
                    else
                    {
                        result.ResultContextObject = this.indicator;
                        result = await this.ratingService.UpdateIndicatorAsync(result);
                    }
                    if (result.HasErrorMessages)
                    {
                        this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                        await this.CallBackAfterSumbit.InvokeAsync();
                        this.isVisible = false;
                        this.StateHasChanged();
                    }
                }
                else
                {

                }
            }
            this.SpinnerHide();
        }
        private async Task<bool> ValidateIndicator(IndicatorVM _indicator)
        {
            var data = await this.ratingService.GetAllIndicatorsAsync(_indicator);

            if (this.kvTypesOfIndicators.First(y => y.IdKeyValue == _indicator.IdIndicatorType).DefaultValue1 == "Quality")
            {
                if (data.Any())
                {
                    ValidationMessages.Add("Не можете да въведете едновременно два валидни записа за една и съща календарна година!");
                    return false;
                }
            }
            else
            {
                var _IdIndicator = _indicator.IdIndicator;
                foreach (var tempIndicator in data.Where(x => x.IdIndicator != _IdIndicator))
                {
                    if ((_indicator.RangeFrom >= tempIndicator.RangeFrom && _indicator.RangeFrom <= tempIndicator.RangeTo) || (_indicator.RangeTo >= tempIndicator.RangeFrom && _indicator.RangeFrom <= tempIndicator.RangeTo))
                    {
                        ValidationMessages.Add("Не можете да въведете едновременно два валидни записа за една и съща календарна година!");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
