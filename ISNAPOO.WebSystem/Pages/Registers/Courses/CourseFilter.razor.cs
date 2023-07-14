using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Registers.Courses
{
    public partial class CourseFilter : BlazorBaseComponent
    {

        private CoursesFilterVM model = new CoursesFilterVM();

        [Parameter]
        public EventCallback<List<CourseVM>> CallbackAfterSubmit { get; set; }

        public List<CandidateProviderVM> candidateProviders = new List<CandidateProviderVM>();

        public List<LocationVM> locationSource = new List<LocationVM>();

        public List<DistrictVM> districtSource = new List<DistrictVM>();

        public List<LocationVM> locationFiltered = new List<LocationVM>();

        public List<MunicipalityVM> municipalitySource = new List<MunicipalityVM>();

        public List<MunicipalityVM> municipalityFiltered = new List<MunicipalityVM>();

        public List<ProfessionVM> professionSource = new List<ProfessionVM>();

        public List<SpecialityVM> specialitySource = new List<SpecialityVM>();




        public List<KeyValueVM> courseType = new List<KeyValueVM>();

        public List<KeyValueVM> assignType = new List<KeyValueVM>();

        public List<KeyValueVM> Status = new List<KeyValueVM>();



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
        public ITrainingService TrainingService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task openModal()
        {
            courseType = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).ToList();
            assignType = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType")).ToList();
            Status = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseStatus")).ToList();

            this.candidateProviders = (await candidateProviderService.GetAllCandidateProvidersForAutoComplete("LicensingCPO")).ToList();
            this.municipalitySource = (await municipalityService.GetAllMunicipalitiesAsync()).ToList();
            this.districtSource = (await districtService.GetAllDistrictsAsync()).ToList();
            this.professionSource = (await professionService.GetAllActiveProfessionsAsync()).ToList();
            this.specialitySource = (await specialityService.GetAllActiveSpecialitiesAsync()).ToList();

            this.municipalityFiltered = municipalitySource;

            this.isVisible = true;
            this.StateHasChanged();
        }

        public async Task Filter()
        {
            if (this.model.IdCandidateProvider == 0 && string.IsNullOrEmpty(this.model.LicenceNumber) && this.model.IdDistrict == 0
                && this.model.IdMunicipality == 0 && this.model.IdLocation == 0 && string.IsNullOrEmpty(this.model.CourseName)
                && !this.model.IdCourseType.HasValue && !this.model.IdAssignType.HasValue && !this.model.IdStatus.HasValue
                && this.model.IdProfession == 0 && this.model.IdSpeciality == 0 && !this.model.StartFrom.HasValue
                && !this.model.StartTo.HasValue && !this.model.EndFrom.HasValue && !this.model.EndTo.HasValue)
            {
                await this.ShowErrorAsync("Моля, въведете поне един критерий, по който да филтрирате курсове!");
                return;
            }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var filteredResults = await this.TrainingService.GetAllCoursesAsync(this.model);

                await this.CallbackAfterSubmit.InvokeAsync(filteredResults);

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
            this.model = new CoursesFilterVM();
        }

        public async Task OnFilterCPO(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                var query = new Query().Where(new WhereFilter() { Field = "CPONameOwnerGrid", Operator = "contains", value = args.Text, IgnoreCase = true });

                query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                await this.sfAutoCompleteCPO.FilterAsync(this.candidateProviders, query);
            }
        }

        private async Task OnFilterLocationCorrespondence(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    if (this.model.IdMunicipality != 0)
                    {
                        this.locationSource = (await this.locationService.GetAllLocationsByMunicipalityIdAsync(this.model.IdMunicipality)).ToList();
                        locationSource = locationSource.Where(x => x.kati.ToLower().Contains(args.Text.ToLower())).ToList();
                    }
                    else if (this.model.IdDistrict != 0)
                    {
                        this.locationSource = (await this.locationService.GetAllLocationsByDistrictIdAsync(this.model.IdDistrict)).ToList();
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

        public void OnSelectedSpeciality(ChangeEventArgs<int, SpecialityVM> args)
        {
            if (args.ItemData != null)
            {
                this.model.IdProfession = args.ItemData.IdProfession;
            }
        }

        public void DistrictChangeHandler(ChangeEventArgs<int, DistrictVM> args)
        {
            if (args.Value != 0)
                municipalityFiltered = municipalitySource.Where(x => x.idDistrict == args.Value).ToList();
            else
            {
                municipalityFiltered = municipalitySource;
                model.IdMunicipality = 0;
                model.IdDistrict = 0;
            }
        }

        public async Task MunicipalityChangeHandler(ChangeEventArgs<int, MunicipalityVM> args)
        {
            if (args.Value != 0)
            {
                model.IdDistrict = municipalitySource.Where(x => x.idMunicipality == args.Value).First().idDistrict;

                locationFiltered = (await locationService.GetAllLocationsByMunicipalityIdAsync(model.IdMunicipality)).ToList();
            }
            else
            {
                locationFiltered = locationSource;
                this.model.IdLocation = 0;
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
                model.IdMunicipality = (int)args.ItemData.idMunicipality;
                model.IdDistrict = args.ItemData.Municipality.idDistrict;
            }

        }
    }
}

