using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.Contracts;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOTrainingInstitution : BlazorBaseComponent
    {
        private SfAutoComplete<int?, LocationVM> sfAutoCompleteLocationCorrespondence = new SfAutoComplete<int?, LocationVM>();

        private IEnumerable<KeyValueVM> providerRegisteredAtValues = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> providerOwnershipValues = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> providerStatusValues = new List<KeyValueVM>();
        private IEnumerable<RegionVM> regionSource = new List<RegionVM>();
        private List<LocationVM> locationCorrespondenceSource = new List<LocationVM>();
        private LocationVM locationVM = new LocationVM();
        private List<PersonVM> personsFromCPOSource = new List<PersonVM>();
        private ValidationMessageStore? messageStore;
        private List<int> municipalitiIds = new List<int>();
        private LocationVM selectedLocation = new LocationVM();

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool DisableAllFields { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

        [Parameter]
        public bool DisableWhenProcedureIsCompleted { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IPersonService PersonService { get; set; }

        [Inject]
        public IRegionService RegionService { get; set; }

        [Inject]
        public IMunicipalityService MunicipalityService { get; set; }

        [Inject]
        public IProviderService ProviderService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.SpinnerShow();

            this.editContext = new EditContext(this.CandidateProviderVM);
            this.FormTitle = "Обучаваща институция";

            this.municipalitiIds = (await this.MunicipalityService.GetAllMunicipalitiesWithRegionsAsync()).ToList();

            this.providerRegisteredAtValues = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProviderRegisteredAt")).OrderBy(x => x.Order).ToList();
            this.providerOwnershipValues = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProviderOwnership");
            this.providerStatusValues = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProviderStatus");
            this.providerStatusValues = this.providerStatusValues.Where(p => p.KeyValueIntCode.Contains("CIPO"));
            this.locationVM = await this.LocationService.GetLocationByIdAsync(this.CandidateProviderVM.IdLocation ?? default);

            this.CandidateProviderVM.PersonsForNotifications =
                (await this.ProviderService
                .GetAllPersonsForNotificationByCandidateProviderIdAsync(this.CandidateProviderVM.IdCandidate_Provider)).ToList();

            if (this.CandidateProviderVM.IdLocationCorrespondence != null)
            {
                LocationVM locationCorrespondence = await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.CandidateProviderVM.IdLocationCorrespondence ?? default);
                this.locationCorrespondenceSource.Add(locationCorrespondence);

                this.selectedLocation = locationCorrespondence;

                await this.HandleRegionLogicAsync(locationCorrespondence);
            }

            var personsIds = (await this.CandidateProviderService.GetAllActiveCandidateProviderPersonsByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider)).ToList().Select(x => x.IdPerson).ToList();
            if (personsIds.Any())
            {
                this.personsFromCPOSource = (await this.PersonService.GetPersonsByIdsAsync(personsIds)).ToList();
            }

            this.SpinnerHide();
        }

        private async Task HandleRegionLogicAsync(LocationVM location)
        {
            if (this.municipalitiIds.Any(x => x == location.idMunicipality))
            {
                this.regionSource = await this.RegionService.GetAllRegionsByIdMunicipalityAsync(location.idMunicipality.Value);
            }
        }

        // филтрира на всеки символ след въвеждане на 4 символа при LocationCorrespondence
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

        public override void SubmitHandler()
        {
            this.editContext = new EditContext(this.CandidateProviderVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.OnValidationRequested += this.ValidatePersonsForNotifications;
            this.editContext.OnValidationRequested += this.ValidateRegion;

            this.editContext.Validate();
        }

        private async Task SetZIPCodeAndRegionDataAsync(ChangeEventArgs<int?, LocationVM> args)
        {
            if (args.Value != null)
            {
                this.CandidateProviderVM.ZipCodeCorrespondence = args.ItemData.PostCode.ToString();

                this.selectedLocation = args.ItemData;

                this.regionSource = await this.RegionService.GetAllRegionsByIdMunicipalityAsync(this.selectedLocation.idMunicipality.Value);
            }
            else
            {
                this.CandidateProviderVM.ZipCodeCorrespondence = string.Empty;

                this.selectedLocation = new LocationVM();

                this.regionSource = new List<RegionVM>();
            }
        }

        private void ValidatePersonsForNotifications(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (!this.CandidateProviderVM.PersonsForNotifications.Any())
            {
                FieldIdentifier fi = new FieldIdentifier(this.CandidateProviderVM, "PersonsForNotifications");
                this.messageStore?.Add(fi, "Полето 'Потребители за получаване на известия/уведомления' трябва да има поне 1 въведена стойност!");
            }
        }

        private void ValidateRegion(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.municipalitiIds.Any(x => x == this.selectedLocation.idMunicipality))
            {
                if (!this.CandidateProviderVM.IdRegionCorrespondence.HasValue)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.CandidateProviderVM, "IdRegionCorrespondence");
                    this.messageStore?.Add(fi, "Полето 'Район' е задължително!");
                }
            }
        }
    }
}
