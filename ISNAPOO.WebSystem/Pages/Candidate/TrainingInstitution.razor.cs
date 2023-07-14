using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class TrainingInstitution : BlazorBaseComponent
    {
        private SfAutoComplete<int?, LocationVM> sfAutoCompleteLocationCorrespondence = new SfAutoComplete<int?, LocationVM>();

        private IEnumerable<KeyValueVM> providerRegisteredAtValues = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> providerOwnershipValues = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> providerStatusValues = new List<KeyValueVM>();
        private IEnumerable<RegionVM> regionSource = new List<RegionVM>();
        private List<LocationVM> locationCorrespondenceSource = new List<LocationVM>();
        private List<PersonVM> personsFromCPOSource = new List<PersonVM>();
        private ValidationMessageStore? messageStore;
        private List<int> municipalitiesIds = new List<int>();
        private LocationVM selectedLocation = new LocationVM();

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool DisableFieldsWhenOpenFromProfile { get; set; }

        [Parameter]
        public bool IsCPO { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

        [Parameter]
        public bool DisableFieldsWhenApplicationStatusIsNotDocPreparation { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsNAPOO { get; set; }

        [Parameter]
        public bool DisableFieldsWhenActiveLicenceChange { get; set; }

        [Parameter]
        public bool IsUserProfileAdministrator { get; set; }

        [Parameter]
        public bool IsLicenceChange { get; set; }

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
            this.editContext = new EditContext(this.CandidateProviderVM);
            this.FormTitle = "Обучаваща институция";

            this.providerRegisteredAtValues = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProviderRegisteredAt")).OrderBy(x => x.Order).ToList();
            this.providerOwnershipValues = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProviderOwnership");
            this.providerStatusValues = this.IsCPO
                ? (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProviderStatus")).Where(p => p.KeyValueIntCode.Contains("CPO")).ToList()
                : (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProviderStatus")).Where(p => p.KeyValueIntCode.Contains("CIPO")).ToList();

            if (this.CandidateProviderVM.Location is null)
            {
                this.CandidateProviderVM.Location = new LocationVM();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.SpinnerShow();

                this.municipalitiesIds = (await this.MunicipalityService.GetAllMunicipalitiesWithRegionsAsync()).ToList();

                if (this.CandidateProviderVM.LocationCorrespondence is not null)
                {
                    this.locationCorrespondenceSource.Add(this.CandidateProviderVM.LocationCorrespondence);
                    this.selectedLocation = this.CandidateProviderVM.LocationCorrespondence;

                    await this.HandleRegionLogicAsync(this.CandidateProviderVM.LocationCorrespondence);
                }

                if (this.CandidateProviderVM.IdCandidateProviderActive is not null)
                {
                    this.CandidateProviderVM.PersonsForNotifications =
                    (await this.ProviderService
                    .GetAllPersonsForNotificationByCandidateProviderIdAsync(this.CandidateProviderVM.IdCandidateProviderActive.Value)).ToList();
                }
                else
                {
                    this.CandidateProviderVM.PersonsForNotifications =
                    (await this.ProviderService
                    .GetAllPersonsForNotificationByCandidateProviderIdAsync(this.CandidateProviderVM.IdCandidate_Provider)).ToList();
                }

                List<int> personIds = new List<int>();
                if (this.CandidateProviderVM.IdCandidateProviderActive.HasValue)
                {
                    personIds = (await this.CandidateProviderService.GetAllActiveCandidateProviderPersonsByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidateProviderActive.Value)).ToList().Select(x => x.IdPerson).ToList();
                }
                else
                {
                    personIds = (await this.CandidateProviderService.GetAllActiveCandidateProviderPersonsByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider)).ToList().Select(x => x.IdPerson).ToList();
                }

                if (personIds.Any())
                {
                    this.personsFromCPOSource = (await this.PersonService.GetPersonsByIdsAsync(personIds)).ToList();
                }

                if (this.CandidateProviderVM.Location is null)
                {
                    this.CandidateProviderVM.Location = new LocationVM();
                }

                this.editContext.MarkAsUnmodified();

                this.SpinnerHide();

                this.StateHasChanged();
            }
        }

        private async Task HandleRegionLogicAsync(LocationVM location)
        {
            if (this.municipalitiesIds.Any(x => x == location.idMunicipality))
            {
                this.regionSource = await this.RegionService.GetAllRegionsByIdMunicipalityAsync(location.idMunicipality!.Value);
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

                this.regionSource = await this.RegionService.GetAllRegionsByIdMunicipalityAsync(this.selectedLocation.idMunicipality!.Value);
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
            if (this.municipalitiesIds.Any(x => x == this.selectedLocation.idMunicipality))
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
