using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Popups;
using ISNAPOO.Core.Contracts.Candidate;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.Contracts.EKATTE;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CandidateProviderPremisesSearchModal : BlazorBaseComponent
    {
        private SfAutoComplete<int?, LocationVM> sfAutoCompleteLocationCorrespondence = new SfAutoComplete<int?, LocationVM>();
        private SfAutoComplete<int?, ProfessionVM> sfAutoCompleteProfession = new SfAutoComplete<int?, ProfessionVM>();
        private SfMultiSelect<List<ProfessionVM>, ProfessionVM> multiSelectProfession = new SfMultiSelect<List<ProfessionVM>, ProfessionVM>();
        private SfMultiSelect<List<SpecialityVM>, SpecialityVM> multiSelect = new SfMultiSelect<List<SpecialityVM>, SpecialityVM>();

        private IEnumerable<CandidateProviderPremisesVM> premisess;

        private List<SpecialityVM> specialitiesSource = new List<SpecialityVM>();

        private List<SpecialityVM> specialities = new List<SpecialityVM>();

        public List<LocationVM> locationSource = new List<LocationVM>();

        public List<LocationVM> locationFiltered = new List<LocationVM>();

        public List<DistrictVM> districtSource = new List<DistrictVM>();

        public List<MunicipalityVM> municipalitySource = new List<MunicipalityVM>();

        public List<MunicipalityVM> municipalityFiltered = new List<MunicipalityVM>();

        public List<ProfessionVM> professionSource = new List<ProfessionVM>();

        private List<SpecialityVM> FilteSspecialities = new List<SpecialityVM>();
        private CandidateProviderPremisesSearchVM FilterPremises = new CandidateProviderPremisesSearchVM();
        private CandidateProviderPremisesVM candidateProviderPremisesVM = new CandidateProviderPremisesVM();
        private CandidateProviderVM CandidateProviderVM = new CandidateProviderVM();
        private bool isCPO = true;

        [Parameter]
        public EventCallback<List<CandidateProviderPremisesVM>> CallBackRefreshGrid { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IMunicipalityService municipalityService { get; set; }
        [Inject]
        public IDistrictService districtService { get; set; }
        [Inject]
        public IProfessionService ProfessionService { get; set; }


        private IEnumerable<KeyValueVM> kvOwnership;
        private IEnumerable<KeyValueVM> kvStatus;
        private IEnumerable<KeyValueVM> kvTypeOfEducation;
        private IEnumerable<KeyValueVM> kvComplianceDOC;


        protected override async void OnInitialized()
        {
            this.locationSource = (await this.LocationService.GetAllLocationsAsync()).ToList();
            this.municipalitySource = (await municipalityService.GetAllMunicipalitiesAsync()).ToList();
            this.municipalityFiltered = municipalitySource;
            this.districtSource = (await districtService.GetAllDistrictsAsync()).ToList();
            this.specialities = (await this.SpecialityService.GetAllActiveSpecialitiesAsync()).ToList();
            if (this.isCPO)
            {
                this.professionSource = (await ProfessionService.GetAllActiveProfessionsAsync()).ToList();
                this.kvStatus = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseStatus", false);
                this.kvOwnership = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseOwnership", false);
                this.kvTypeOfEducation = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType", false);
                this.kvComplianceDOC = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ComplianceDOC", false);
            }
        }
        public async Task OpenModal(CandidateProviderVM CandidateProviderVM, List<CandidateProviderPremisesVM> candidateProviderPremisesVM, bool isCPO)
        {
            this.isCPO = isCPO;
            this.CandidateProviderVM = CandidateProviderVM;
            this.premisess = await this.CandidateProviderService.GetCandidateProviderPremisesByIdCandidateProviderAsync(this.CandidateProviderVM);
            if (this.isCPO)
            {
                foreach (var CandidateProviderSpeciality in CandidateProviderVM.CandidateProviderSpecialities)
                {
                    if (!this.specialities.Any(x => x.IdSpeciality == CandidateProviderSpeciality.IdSpeciality))
                    {
                        this.specialities.Add(await this.SpecialityService.GetSpecialityByIdAsync(CandidateProviderSpeciality.IdSpeciality));
                    }
                }

                this.specialitiesSource = this.specialities;
            }
            
            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task OnFilterSpeciality(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                    try
                    {
                        if (FilterPremises.IdProfession is not null && FilterPremises.IdProfession != 0)
                        {
                            this.specialities = (List<SpecialityVM>)this.specialitiesSource.Where(x => x.CodeAndAreaForAutoCompleteSearch.Contains(args.Text) && x.IdProfession == FilterPremises.IdProfession).ToList();
                        }
                        else
                        {
                            this.specialities = (List<SpecialityVM>)this.specialitiesSource.Where(x => x.CodeAndAreaForAutoCompleteSearch.Contains(args.Text)).ToList();
                        }

                    }
                    catch (Exception ex) { }

                    var query = new Query().Where(new WhereFilter() { Field = "CodeAndAreaForAutoCompleteSearch", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.multiSelect.FilterAsync(this.specialities, query);               
            }
        }

        private void OnFocusSpeciality()
        {
            if (FilterPremises.IdProfession != null && FilterPremises.IdProfession != 0)
            {
                this.specialities = (List<SpecialityVM>)this.specialitiesSource.Where(x => x.IdProfession == FilterPremises.IdProfession).ToList();
            }
            else
            {
                this.specialities = specialitiesSource;
            }
        }

        private void OnProfessionSelect()
        {
            if (this.FilterPremises != null && this.FilterPremises.IdProfession != null && this.FilteSspecialities != null)
            {
                this.FilteSspecialities = this.FilteSspecialities.Where(x => x.IdProfession ==  this.FilterPremises.IdProfession).ToList();
            }
        }

        private async Task OnFilterLocationCorrespondence(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    if (this.FilterPremises.idMunicipality != 0)
                    {
                        this.locationSource = (await this.LocationService.GetAllLocationsByMunicipalityIdAsync(this.FilterPremises.idMunicipality)).ToList();
                        locationSource = locationSource.Where(x => x.kati.ToLower().Contains(args.Text.ToLower())).ToList();
                    }
                    else if (this.FilterPremises.idDistrict != 0)
                    {
                        this.locationSource = (await this.LocationService.GetAllLocationsByDistrictIdAsync(this.FilterPremises.idDistrict)).ToList();
                        locationSource = locationSource.Where(x => x.kati.ToLower().Contains(args.Text.ToLower())).ToList();
                    }
                    else
                    {
                        this.locationSource = (await this.LocationService.GetAllLocationsByKatiAsync(args.Text)).ToList();

                    }
                    this.locationFiltered = locationSource;

                    var query = new Query().Where(new WhereFilter() { Field = "kati", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.sfAutoCompleteLocationCorrespondence.FilterAsync(this.locationFiltered, query);
                }
                catch (Exception ex) { }


            }
        }

        private void OnLocationSelect(SelectEventArgs<LocationVM> args)
        {
            var temp = this.municipalitySource.First(x => x.idMunicipality == args.ItemData.idMunicipality);
            this.FilterPremises.idMunicipality = temp.idMunicipality;
            this.FilterPremises.idDistrict = temp.idDistrict;
        }

        public void DistrictChangeHandler(ChangeEventArgs<int, DistrictVM> args)
        {
            if (args.Value != 0)
                municipalityFiltered = municipalitySource.Where(x => x.idDistrict == args.Value).ToList();
            else
            {
                municipalityFiltered = municipalitySource;

                FilterPremises.idMunicipality = 0;
                FilterPremises.idDistrict = 0;
            }
        }

        public async Task MunicipalityChangeHandler(ChangeEventArgs<int, MunicipalityVM> args)
        {
            if (args.Value != 0)
            {
                FilterPremises.idDistrict = municipalitySource.Where(x => x.idMunicipality == args.Value).First().idDistrict;

                locationFiltered = (await LocationService.GetAllLocationsByMunicipalityIdAsync(FilterPremises.idMunicipality)).ToList();
            }
            else
            {
                locationFiltered = locationSource;
                FilterPremises.IdLocation = 0;
            }
        }

        public async Task SumbitFilter()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var temp = this.GetFilteredPremises();
                await this.CallBackRefreshGrid.InvokeAsync(temp);
                this.isVisible = false;
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public void ClearFilter()
        {
            this.FilterPremises.PremisesName = "";
            this.FilterPremises.IdSpeciality = 0;
            this.FilterPremises.IdKvStatus = 0;
            this.FilterPremises.IdLocation = 0;
            this.FilterPremises.idDistrict = 0;
            this.FilterPremises.idMunicipality = 0;
            this.FilterPremises.IdOwnerShip = 0;
            this.FilteSspecialities = new List<SpecialityVM>();
            this.FilterPremises.IdProfession = 0;
            this.FilterPremises.IdComplianceDOC = 0;
            this.FilterPremises.IdTypeOfEducation = 0;
        }

        public List<CandidateProviderPremisesVM> GetFilteredPremises()
        {

            premisess = premisess.Where(
            d => 
            (!string.IsNullOrEmpty(this.FilterPremises.PremisesName) ? d.PremisesName.Trim().ToLower().ToString().Contains(this.FilterPremises.PremisesName.Trim().ToLower().ToString()) : true)
            && ((this.FilterPremises.IdLocation is not null && this.FilterPremises.IdLocation != 0) ? d.IdLocation == this.FilterPremises.IdLocation : true)
            && ((this.FilterPremises.idMunicipality != 0 && (this.FilterPremises.IdLocation is null || this.FilterPremises.IdLocation == 0)) ? this.locationSource.FirstOrDefault(y => y.idLocation == d.IdLocation).idMunicipality == this.FilterPremises.idMunicipality : true)
            && ((this.FilterPremises.idDistrict != 0 && (this.FilterPremises.idMunicipality == 0 && (this.FilterPremises.IdLocation is  null || this.FilterPremises.IdLocation == 0))) ? this.municipalitySource.FirstOrDefault(u => u.idMunicipality == this.locationSource.FirstOrDefault(y => y.idLocation == d.IdLocation).idMunicipality).idDistrict == this.FilterPremises.idDistrict : true)
            && ((this.FilterPremises.IdOwnerShip is not  null &&  this.FilterPremises.IdOwnerShip != 0) ? d.IdOwnership == this.FilterPremises.IdOwnerShip : true)
            && ((this.FilterPremises.IdKvStatus is  not null  && this.FilterPremises.IdKvStatus != 0) ? d.IdStatus == this.FilterPremises.IdKvStatus : true)         
            && ((FilteSspecialities != null && FilteSspecialities.Count != 0) ? d.CandidateProviderPremisesSpecialities.Any(x => FilteSspecialities.Any(y => x.IdSpeciality == y.IdSpeciality)) : true)
            && ((this.FilterPremises.IdProfession is not null && this.FilterPremises.IdProfession != 0 && (FilteSspecialities.Count == 0 || FilteSspecialities == null)) ? d.CandidateProviderPremisesSpecialities.Any(x => x.Speciality.IdProfession == this.FilterPremises.IdProfession) : true)
            && ((this.FilterPremises.IdComplianceDOC is not null && this.FilterPremises.IdComplianceDOC != 0) ? d.CandidateProviderPremisesSpecialities.Any(x => x.IdComplianceDOC == this.FilterPremises.IdComplianceDOC) : true)
            && ((this.FilterPremises.IdTypeOfEducation is not null && this.FilterPremises.IdTypeOfEducation != 0) ? d.CandidateProviderPremisesSpecialities.Any(x => x.IdUsage == this.FilterPremises.IdTypeOfEducation) : true));

            return premisess.ToList();
        }
    }
}
