using System.Linq;
using Data.Models.Migrations;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Services.Rating;
using ISNAPOO.Core.ViewModels.Rating;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Rating.Modals;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Rating
{
    public partial class RatingIndicatorList : BlazorBaseComponent
    {
        [Inject]
        IRatingService ratingService { get; set; }

        private RatingIndicatorModal ratingIndicatorModal = new RatingIndicatorModal();
        private ConfirmYearModal confirmYearModal = new ConfirmYearModal();

        IEnumerable<IndicatorVM> IndicatorSource { get; set; }

        SfGrid<IndicatorVM> sfGrid = new SfGrid<IndicatorVM>();

        List<IndicatorVM> IndicatorGridSource = new List<IndicatorVM>();
        List<CandidateProviderIndicatorVM> ResultSource = new List<CandidateProviderIndicatorVM>();

        Dictionary<int, List<IndicatorVM>> IndicatorsAsDictionary = new Dictionary<int, List<IndicatorVM>>();

        private string LicensingType { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
            tokenContext.ResultContextObject.Token = Token;
            tokenContext = BaseHelper.GetDecodeToken(tokenContext);
            this.LicensingType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "RatingIndicatorList").Value.ToString();
            this.ResultSource = (await this.ratingService.GetAllResultsAsync()).ToList();
            await ReloadData();
        }

        public async Task CreateIndicatorModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageRatingIndicatorList", false);
            if (!hasPermission) { return; }

            await ratingIndicatorModal.OpenModal(this.LicensingType);
        }
        public async Task DeleteIndicator(IndicatorVM indicatorVM)
        {
            bool hasPermission = await CheckUserActionPermission("ManageRatingIndicatorList", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (!ResultSource.Any(x => x.IdIndicator == indicatorVM.IdIndicator))
            {
                ResultContext<IndicatorVM> resultContext = new ResultContext<IndicatorVM>();
                resultContext.ResultContextObject = indicatorVM;
                resultContext = await this.ratingService.DeleteIndicatorAsync(resultContext);
                if (resultContext.HasErrorMessages)
                {
                    this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                }
                else
                {
                    this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                    await this.ReloadData();
                }
            }
            else
            {
                this.ShowErrorAsync("Не можете да изтриете избрания индикатор, защото вече участва в изчисления за рейтинг в системата!");
            }
            this.SpinnerHide();
        }

        public async Task ReloadData()
        {
            this.IndicatorSource = (await this.ratingService.GetAllIndicatorsAsync(new IndicatorVM())).Where(x => x.IndicatorDetails.DefaultValue2.Contains(this.LicensingType));
            this.IndicatorsAsDictionary = this.IndicatorSource.GroupBy(x => x.Year).ToDictionary(y => y.Key, y => y.ToList());
            IndicatorGridSource = this.IndicatorSource.ToList();
            await this.sfGrid.Refresh();
        }
    }
}
