using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ApplicationListFilterModal : BlazorBaseComponent
    {
        private ApplicationListFilterVM applicationListFilterVM = new ApplicationListFilterVM();
        private SfAutoComplete<int?, LocationVM> sfAutoCompleteLocationCorrespondence = new SfAutoComplete<int?, LocationVM>();

        private List<LocationVM> locationCorrespondenceSource = new List<LocationVM>();
        private string title = string.Empty;
        private List<CandidateProviderVM> candidateProvidersSource = new List<CandidateProviderVM>();
        private List<CandidateProviderVM> candProvOrigSource = new List<CandidateProviderVM>();
        private IEnumerable<KeyValueVM> kvTypeApplicationsSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvApplicationStatusesSource = new List<KeyValueVM>();
        private IEnumerable<ExpertVM> napooExpertsSource = new List<ExpertVM>();

        ValidationMessageStore? messageStore;

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback<List<CandidateProviderVM>> CallbackAfterSubmit { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        public IExpertService ExpertService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.applicationListFilterVM);
            this.messageStore = new ValidationMessageStore(this.editContext);
        }

        public async Task OpenModal(string title, List<CandidateProviderVM> candidateProviders, List<CandidateProviderVM> candidateProvidersOrigList, IEnumerable<KeyValueVM> kvTypeApplicationsSource, IEnumerable<KeyValueVM> kvApplicationStatusesSource)
        {
            this.editContext = new EditContext(this.applicationListFilterVM);

            this.kvApplicationStatusesSource = kvApplicationStatusesSource;
            this.kvTypeApplicationsSource = kvTypeApplicationsSource;
            this.napooExpertsSource = await this.ExpertService.GetAllNAPOOExpertsWithPersonIncludedAsync();
            this.title = title;
            this.candidateProvidersSource = candidateProviders.ToList();
            this.candProvOrigSource = candidateProvidersOrigList.ToList(); 

            this.isVisible = true;
            this.StateHasChanged();
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

                this.editContext = new EditContext(this.applicationListFilterVM);
                this.messageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidateUINValue;
                this.editContext.EnableDataAnnotationsValidation();

                if (this.editContext.Validate())
                {
                    this.candidateProvidersSource = this.FilterCandidateProviders(this.candidateProvidersSource).ToList();

                    this.isVisible = false;

                    await this.CallbackAfterSubmit.InvokeAsync(this.candidateProvidersSource);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void ClearBtn()
        {
            this.applicationListFilterVM = new ApplicationListFilterVM();
        }

        private void ValidateUINValue(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (!string.IsNullOrEmpty(this.applicationListFilterVM.UIN))
            {
                if (!int.TryParse(this.applicationListFilterVM.UIN.Trim(), out int uinValue))
                {
                    FieldIdentifier fi = new FieldIdentifier(this.applicationListFilterVM, "UIN");
                    this.messageStore?.Add(fi, "Полето 'УИН' трябва да бъде цяло число!");
                }
            }
        }

        private async Task OnFilterLocationCorrespondence(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    this.locationCorrespondenceSource = (List<LocationVM>)await this.LocationService.GetAllLocationsByKatiAsync(args.Text);
                }
                catch (Exception ex) { }

                var query = new Query().Where(new WhereFilter() { Field = "kati", Operator = "contains", value = args.Text, IgnoreCase = true });

                query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                await this.sfAutoCompleteLocationCorrespondence.FilterAsync(this.locationCorrespondenceSource, query);
            }
        }

        private List<CandidateProviderVM> FilterCandidateProviders(List<CandidateProviderVM> candidateProviders)
        {
            candidateProviders = this.candProvOrigSource;

            if (!string.IsNullOrEmpty(this.applicationListFilterVM.ProviderName))
            {
                candidateProviders = candidateProviders.Where(x => !string.IsNullOrEmpty(x.ProviderName) ? x.ProviderName.ToLower().Contains(this.applicationListFilterVM.ProviderName.Trim().ToLower()) : false).ToList();
            }
            if (!string.IsNullOrEmpty(this.applicationListFilterVM.UIN))
            {
                candidateProviders = candidateProviders.Where(x => x.UIN == long.Parse(this.applicationListFilterVM.UIN.Trim())).ToList();
            }

            if (!string.IsNullOrEmpty(this.applicationListFilterVM.ProviderOwner))
            {
                candidateProviders = candidateProviders.Where(x => x.ProviderOwner.ToLower().Contains(this.applicationListFilterVM.ProviderOwner.Trim().ToLower())).ToList();
            }

            if (this.applicationListFilterVM.IdLocation is not null)
            {
                candidateProviders = candidateProviders.Where(x => x.IdLocationCorrespondence == this.applicationListFilterVM.IdLocation).ToList();
            }

            if (!string.IsNullOrEmpty(this.applicationListFilterVM.ProviderAddressCorrespondence))
            {
                candidateProviders = candidateProviders.Where(x => !string.IsNullOrEmpty(x.ProviderAddressCorrespondence) ? x.ProviderAddressCorrespondence.ToLower().Contains(this.applicationListFilterVM.ProviderAddressCorrespondence.Trim().ToLower()) : false).ToList();
            }

            if (!string.IsNullOrEmpty(this.applicationListFilterVM.PersonNameCorrespondence))
            {
                candidateProviders = candidateProviders.Where(x => !string.IsNullOrEmpty(x.PersonNameCorrespondence) ? x.PersonNameCorrespondence.ToLower().Contains(this.applicationListFilterVM.PersonNameCorrespondence.Trim().ToLower()) : false).ToList();
            }

            if (!string.IsNullOrEmpty(this.applicationListFilterVM.ProviderPhoneCorrespondence))
            {
                candidateProviders = candidateProviders.Where(x => !string.IsNullOrEmpty(x.ProviderPhoneCorrespondence) ? x.ProviderPhoneCorrespondence.ToLower().Contains(this.applicationListFilterVM.ProviderPhoneCorrespondence.Trim().ToLower()) : false).ToList();
            }

            if (!string.IsNullOrEmpty(this.applicationListFilterVM.ProviderEmailCorrespondence))
            {
                candidateProviders = candidateProviders.Where(x => !string.IsNullOrEmpty(x.ProviderEmailCorrespondence) ? x.ProviderEmailCorrespondence.ToLower().Contains(this.applicationListFilterVM.ProviderEmailCorrespondence.Trim().ToLower()) : false).ToList();
            }

            if (!string.IsNullOrEmpty(this.applicationListFilterVM.LicenceNumber))
            {
                candidateProviders = candidateProviders.Where(x => !string.IsNullOrEmpty(x.LicenceNumber) ? x.LicenceNumber.ToLower().Contains(this.applicationListFilterVM.LicenceNumber.Trim().ToLower()) : false).ToList();
            }

            if (this.applicationListFilterVM.LicenceDate is not null)
            {
                candidateProviders = candidateProviders.Where(x => x.LicenceDate.HasValue ? x.LicenceDate.Value.ToString("dd.MM.yyyy") == this.applicationListFilterVM.LicenceDate.Value.ToString("dd.MM.yyyy") : false).ToList();
            }

            if (this.applicationListFilterVM.IdTypeApplication is not null)
            {
                candidateProviders = candidateProviders.Where(x => x.IdTypeApplication == this.applicationListFilterVM.IdTypeApplication).ToList();
            }

            if (this.applicationListFilterVM.IdApplicationStatus is not null)
            {
                candidateProviders = candidateProviders.Where(x => x.IdApplicationStatus == this.applicationListFilterVM.IdApplicationStatus).ToList();
            }

            if (this.applicationListFilterVM.IdNAPOOExpert.HasValue)
            {
                candidateProviders = candidateProviders.Where(x => x.StartedProcedure.ProcedureExternalExperts.FirstOrDefault(y => y.IdExpert == this.applicationListFilterVM.IdNAPOOExpert) != null).ToList();
            }

            return candidateProviders;
        }
    }
}
