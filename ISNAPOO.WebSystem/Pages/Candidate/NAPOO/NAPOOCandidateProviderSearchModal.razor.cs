using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.Services.Candidate;
using ISNAPOO.Core.Contracts.Common;

namespace ISNAPOO.WebSystem.Pages.Candidate.NAPOO
{
    public partial class NAPOOCandidateProviderSearchModal : BlazorBaseComponent
    {
        private SfAutoComplete<int, LocationVM> providersAdminAutoComplete = new SfAutoComplete<int, LocationVM>();
        private SfAutoComplete<int, LocationVM> providersCorrespondenceAutoComplete = new SfAutoComplete<int, LocationVM>();
        private SfAutoComplete<int?, CandidateProviderVM> cpAutoComplete = new SfAutoComplete<int?, CandidateProviderVM>();
        private NAPOOCandidateProviderFilterVM napooCandidateProviderFilterVM = new NAPOOCandidateProviderFilterVM();
        private List<LocationVM> locationsSource = new List<LocationVM>();
        private List<LocationVM> locationsAdminSource = new List<LocationVM>();
        private List<LocationVM> locationsCorrespondenceSource = new List<LocationVM>();
        private List<SpecialityVM> specialitiesSource = new List<SpecialityVM>();
        private IEnumerable<CandidateProviderVM> candidateProvidersSource = new List<CandidateProviderVM>();
        private IEnumerable<ProfessionVM> professionsSource = new List<ProfessionVM>();        
        private string title = string.Empty;
        private bool licenceDeactivated = false;
        private int kvSPPOOActiveStatus = 0;

        [Parameter]
        public EventCallback<List<CandidateProviderVM>> CallbackAfterSubmit { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.napooCandidateProviderFilterVM);
        }

        public async Task OpenModal(string searchType, bool licenceDeactivated)
        {
            this.title = searchType;
            
            if (this.title == "ЦПО")
            {
                this.kvSPPOOActiveStatus = this.DataSourceService.GetActiveStatusID();

                if (!licenceDeactivated)
                {
                    this.candidateProvidersSource = (await CandidateProviderService.GetAllActiveCandidateProvidersWithoutIncludesAsync("LicensingCPO")).ToList();
                }
                else
                {
                    this.candidateProvidersSource = (await CandidateProviderService.GetAllCandidateProvidersWithLicenseDeactivatedAsync("LicensingCPO")).ToList();
                }
                
                this.specialitiesSource = (await this.SpecialityService.GetAllActiveSpecialitiesAsync()).ToList();
            }
            else
            {
                if (!licenceDeactivated)
                {
                    this.candidateProvidersSource = (await CandidateProviderService.GetAllActiveCandidateProvidersWithoutIncludesAsync("LicensingCIPO")).ToList();
                }
                else
                {
                    this.candidateProvidersSource = (await CandidateProviderService.GetAllCandidateProvidersWithLicenseDeactivatedAsync("LicensingCIPO")).ToList();
                }
            }

            this.licenceDeactivated = licenceDeactivated;

            this.professionsSource = this.DataSourceService.GetAllProfessionsList().Where(x => x.IdStatus == kvSPPOOActiveStatus).OrderBy(x => x.Code).ToList();
            this.locationsSource = (await this.LocationService.GetAllLocationsAsync()).ToList();
            this.editContext = new EditContext(this.napooCandidateProviderFilterVM);

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void ClearBtn()
        {
            this.SpinnerShow();

            this.napooCandidateProviderFilterVM = new NAPOOCandidateProviderFilterVM();

            this.SpinnerHide();
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

                this.editContext = new EditContext(this.napooCandidateProviderFilterVM);
                this.editContext.EnableDataAnnotationsValidation();

                if (this.editContext.Validate())
                {
                    var filterType = this.title == "ЦПО" ? "LicensingCPO" : "LicensingCIPO";
                    var providers = (await this.CandidateProviderService.FilterCandidateProvidersAsync(this.napooCandidateProviderFilterVM, filterType, this.licenceDeactivated)).ToList();
                    
                    this.isVisible = false;

                    await this.CallbackAfterSubmit.InvokeAsync(providers);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnFilterAdminProvider(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    this.locationsAdminSource = this.locationsSource.ToList();

                    var query = new Query().Where(new WhereFilter() { Field = "LocationName", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.providersAdminAutoComplete.FilterAsync(this.locationsAdminSource, query);
                }
                catch { }
            }
        }

        private async Task OnFilterCorrespondenceProvider(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    this.locationsCorrespondenceSource = this.locationsSource.ToList();

                    var query = new Query().Where(new WhereFilter() { Field = "LocationName", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.providersCorrespondenceAutoComplete.FilterAsync(this.locationsCorrespondenceSource, query);
                }
                catch { }
            }
        }

        private async Task OnFilterCandidateProviderHandler(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            var query = new Query();
            if (this.title == "ЦПО")
            {
                query = query.Where(new WhereFilter() { Field = "CPONameAndOwner", Operator = "contains", value = args.Text, IgnoreCase = true });
            }
            else
            {
                query = query.Where(new WhereFilter() { Field = "CIPONameAndOwner", Operator = "contains", value = args.Text, IgnoreCase = true });
            }

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await this.cpAutoComplete.FilterAsync(this.candidateProvidersSource, query);
        }

        private void OnProfessionSelectedHandler(ChangeEventArgs<int?, ProfessionVM> args)
        {
            if (args.Value.HasValue)
            {
                this.professionsSource = this.DataSourceService.GetAllProfessionsList().Where(x => x.IdStatus == this.kvSPPOOActiveStatus && x.IdProfession == args.Value.Value).OrderBy(x => x.Code).ToList();
                this.napooCandidateProviderFilterVM.Profession = this.professionsSource.Select(x => x.CodeAndName).FirstOrDefault();
            }
            else
            {
                this.professionsSource = this.DataSourceService.GetAllProfessionsList().Where(x => x.IdStatus == kvSPPOOActiveStatus).OrderBy(x => x.Code).ToList();
            }

            this.StateHasChanged();
        }
    }
}
