using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Registers.VocationalQualificationValidationCertificates
{
    public partial class VocationalQualificationValidationCertificateListFilter
    {
        ValidationClientFIlterVM model = new ValidationClientFIlterVM();


        [Parameter]
        public EventCallback<ValidationClientFIlterVM> CallbackAfterSubmit { get; set; }


        public List<CandidateProviderVM> candidateProviders = new List<CandidateProviderVM>();

        public List<LocationVM> locationSource = new List<LocationVM>();

        public List<LocationVM> locationFiltered = new List<LocationVM>();

        public List<DistrictVM> districtSource = new List<DistrictVM>();

        public List<MunicipalityVM> municipalitySource = new List<MunicipalityVM>();

        public List<MunicipalityVM> municipalityFiltered = new List<MunicipalityVM>();


        public List<ProfessionVM> professionSource = new List<ProfessionVM>();

        public List<SpecialityVM> specialitySource = new List<SpecialityVM>();



        public List<KeyValueVM> nationalities = new List<KeyValueVM>();

        public List<KeyValueVM> sex = new List<KeyValueVM>();

        public List<KeyValueVM> courseType = new List<KeyValueVM>();

        public List<TypeOfRequestedDocumentVM> docType = new List<TypeOfRequestedDocumentVM>();



        private SfAutoComplete<int, CandidateProviderVM> sfAutoCompleteCPO = new SfAutoComplete<int, CandidateProviderVM>();

        private SfAutoComplete<int, LocationVM> sfAutoCompleteLocation = new SfAutoComplete<int, LocationVM>();

        private SfAutoComplete<int, ProfessionVM> sfAutoCompleteProfession = new SfAutoComplete<int, ProfessionVM>();

        private SfAutoComplete<int, SpecialityVM> sfAutoCompleteSpeciality = new SfAutoComplete<int, SpecialityVM>();




        [Inject]
        ICandidateProviderService candidateProviderService { get; set; }
        [Inject]
        ILocationService locationService { get; set; }
        [Inject]
        IMunicipalityService municipalityService { get; set; }
        [Inject]
        IDistrictService districtService { get; set; }
        [Inject]
        IDataSourceService dataSourceService { get; set; }
        [Inject]
        ISpecialityService specialityService { get; set; }
        [Inject]
        IProfessionService professionService { get; set; }
        [Inject]
        IProviderDocumentRequestService providerDocumentRequestService { get; set; }

        protected override void OnInitialized()
        {
            createNewModel();
        }

        public async Task openModal()
        {
            this.isVisible = true;

            candidateProviders = (await this.candidateProviderService.GetAllCandidateProvidersForAutoComplete("LicensingCPO")).ToList();
            this.municipalitySource = (await municipalityService.GetAllMunicipalitiesAsync()).ToList();
            this.municipalityFiltered = municipalitySource;
            this.districtSource = (await districtService.GetAllDistrictsAsync()).ToList();
            this.professionSource = (await professionService.GetAllActiveProfessionsAsync()).ToList();
            this.specialitySource = (await specialityService.GetAllActiveSpecialitiesAsync()).ToList();
            nationalities = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality")).ToList();
            sex = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex")).ToList();
            courseType = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MeasureType")).ToList();
            docType = (await providerDocumentRequestService.GetAllValidTypesOfRequestedDocumentAsync()).ToList();

            this.StateHasChanged();
        }

        public void createNewModel()
        {
            //model = new ClientCourseDocumentVM();
            //model.ClientCourse = new ClientCourseVM();
            //model.ClientCourse.Course = new CourseVM();
            //model.ClientCourse.Client = new ClientVM();
            //model.ClientCourse.Course.Location = new LocationVM();
            //model.ClientCourse.Course.Location.Municipality = new MunicipalityVM();
            //model.ClientCourse.Course.Location.Municipality.District = new DistrictVM();
            //model.ClientCourse.Speciality = new SpecialityVM();
            //model.ClientCourse.Speciality.Profession = new ProfessionVM();
            //model.ClientCourse.Client.CandidateProvider = new CandidateProviderVM();
            model = new ValidationClientFIlterVM();
            this.editContext = new EditContext(model);
        }

        public async Task OnFilterCPO(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {

                try
                {
                    var query = new Query().Where(new WhereFilter() { Field = "ProviderOwner", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.sfAutoCompleteCPO.FilterAsync(this.candidateProviders, query);
                }
                catch (Exception e) { }

            }
        }

        private async Task OnFilterLocationCorrespondence(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    if (this.model.idMunicipality != 0)
                    {
                        this.locationSource = (await this.locationService.GetAllLocationsByMunicipalityIdAsync(this.model.idMunicipality)).ToList();
                        locationSource = locationSource.Where(x => x.kati.ToLower().Contains(args.Text.ToLower())).ToList();
                    }
                    else if (this.model.idDistrict != 0)
                    {
                        this.locationSource = (await this.locationService.GetAllLocationsByDistrictIdAsync(this.model.idDistrict)).ToList();
                        locationSource = locationSource.Where(x => x.kati.ToLower().Contains(args.Text.ToLower())).ToList();
                    }
                    else
                    {
                        this.locationSource = (await this.locationService.GetAllLocationsByKatiAsync(args.Text)).ToList();

                    }
                    this.locationFiltered = locationSource;

                    var query = new Query().Where(new WhereFilter() { Field = "kati", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.sfAutoCompleteLocation.FilterAsync(this.locationFiltered, query);
                }
                catch (Exception ex) { }


            }
        }
        private async Task OnFilterProfession(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    var query = new Query().Where(new WhereFilter() { Field = "CodeAndName", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.sfAutoCompleteProfession.FilterAsync(this.professionSource, query);
                }
                catch (Exception e) { }


            }
        }

        public async Task OnSelectedProfession(ChangeEventArgs<int, ProfessionVM> args)
        {
            if (args.ItemData != null)
            {
                specialitySource = specialityService.GetAllSpecialities(new SpecialityVM { IdProfession = args.ItemData.IdProfession }).ToList();
            }
            else
            {
                this.specialitySource = (await specialityService.GetAllActiveSpecialitiesAsync()).ToList();

                this.model.IdSpeciality = 0;
            }
        }

        private async Task OnFilterSpeciality(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    var query = new Query().Where(new WhereFilter() { Field = "CodeAndName", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.sfAutoCompleteSpeciality.FilterAsync(this.specialitySource, query);
                }
                catch (Exception e) { }


            }
        }
        public async Task OnSelectedSpeciality(ChangeEventArgs<int, SpecialityVM> args)
        {
            if (args.ItemData != null)
            {
                this.model.IdProfession = args.ItemData.IdProfession;
            }
        }

        public async Task ClearFilter()
        {
            createNewModel();

        }

        public async Task Save()
        {
            if (loading) return;

            try
            {
                loading = true;
                if (this.model.IsEmpty())
                {
                    await this.ShowErrorAsync("Моля, изберете поне един критерий, по който да филтрирате данни за обучени лица!");
                    this.loading = false;
                    this.SpinnerHide();
                }
                else
                {
                    this.isVisible = false;
                    await this.CallbackAfterSubmit.InvokeAsync(model);
                    this.StateHasChanged();
                }
            }
            finally
            {
                loading = false;
            }
        }

        public async Task DistrictChangeHandler(ChangeEventArgs<int, DistrictVM> args)
        {
            if (args.Value != 0)
                municipalityFiltered = municipalitySource.Where(x => x.idDistrict == args.Value).ToList();
            else
            {
                municipalityFiltered = municipalitySource;

                model.idMunicipality = 0;
                model.idDistrict = 0;
            }
        }

        public async Task MunicipalityChangeHandler(ChangeEventArgs<int, MunicipalityVM> args)
        {
            if (args.Value != 0)
            {
                model.idDistrict = municipalitySource.Where(x => x.idMunicipality == args.Value).First().idDistrict;

                locationFiltered = (await locationService.GetAllLocationsByMunicipalityIdAsync(model.idMunicipality)).ToList();
            }
            else
            {
                locationFiltered = locationSource;
                this.model.idLocation = 0;
            }
        }

        public async void LocationChanegeHandler(ChangeEventArgs<int, LocationVM> args)
        {
            if (args.ItemData != null)
            {
                if (args.ItemData.Municipality == null)
                {
                    args.ItemData.Municipality = await municipalityService.GetMunicipalityByIdAsync((int)args.ItemData.idMunicipality);
                }
                model.idMunicipality = (int)args.ItemData.idMunicipality;
                model.idDistrict = args.ItemData.Municipality.idDistrict;
            }

        }
    }
}
