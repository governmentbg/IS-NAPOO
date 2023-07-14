using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.SPPOO;
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

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class TrainingCourseListFilterModal : BlazorBaseComponent
    {
        //Номенклатури
        public List<KeyValueVM> assignType = new List<KeyValueVM>();
        public List<KeyValueVM> courseType = new List<KeyValueVM>();
        public List<KeyValueVM> formEducation = new List<KeyValueVM>();
        public List<KeyValueVM> trainingCourseType = new List<KeyValueVM>();
        public List<KeyValueVM> VQS = new List<KeyValueVM>();

        
        public List<CandidateProviderVM> candidateProviders = new List<CandidateProviderVM>();
        public List<FrameworkProgramVM> frameworkSource = new List<FrameworkProgramVM>();
        public List<LocationVM> locationFiltered = new List<LocationVM>();
        public List<LocationVM> locationSource = new List<LocationVM>();
        public List<ProfessionVM> professionSource = new List<ProfessionVM>();
        public List<SpecialityVM> specialitySource = new List<SpecialityVM>();
        private CourseVM model = new CourseVM();

        private string type = "";
        private int? nullableVQS = null;

        private SfAutoComplete<int, LocationVM> sfAutoCompleteLocation = new SfAutoComplete<int, LocationVM>();

        private SfAutoComplete<int, ProfessionVM> sfAutoCompleteProfession = new SfAutoComplete<int, ProfessionVM>();

        private SfAutoComplete<int, SpecialityVM> sfAutoCompleteSpeciality = new SfAutoComplete<int, SpecialityVM>();

        [Parameter]
        public EventCallback<CourseVM> CallbackAfterSubmit { get; set; }
        [Inject]
        private ICandidateProviderService candidateProviderService { get; set; }

        [Inject]
        private IDataSourceService dataSourceService { get; set; }

        [Inject]
        private IFrameworkProgramService frameworkProgramService { get; set; }

        [Inject]
        private ILocationService locationService { get; set; }
        [Inject]
        private IProfessionService professionService { get; set; }

        [Inject]
        private ISpecialityService specialityService { get; set; }
        public async Task ClearFilter()
        {
            createNewCourse();
            nullableVQS = null;
            await this.CallbackAfterSubmit.InvokeAsync(model);
        }

        public void createNewCourse()
        {
            model = new CourseVM();
            model.Program = new ProgramVM();
            model.Program.Speciality = new SpecialityVM();
            model.Program.Speciality.Profession = new ProfessionVM();
            model.Program.FrameworkProgram = new FrameworkProgramVM();
            model.Location = new LocationVM();
            model.CandidateProviderPremises = new CandidateProviderPremisesVM();
            model.Program.CandidateProvider = new CandidateProviderVM();
            model.Location.Municipality = new MunicipalityVM();
            model.Location.Municipality.District = new DistrictVM();
            model.CourseName = String.Empty;
        }

        public async Task Filter()
        {
            this.isVisible = false;
            await this.CallbackAfterSubmit.InvokeAsync(model);
            this.StateHasChanged();
        }

        public async void OnSelectedLocation(ChangeEventArgs<int, LocationVM> args)
        {
            if (args.ItemData != null)
            {
                this.model.Location.idLocation = args.ItemData.idLocation;
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

                this.model.Program.Speciality.IdSpeciality = 0;
            }
        }

        public async Task OnSelectedSpeciality(ChangeEventArgs<int, SpecialityVM> args)
        {
            if (args.ItemData != null)
            {
                this.model.Program.Speciality.Profession.IdProfession = args.ItemData.IdProfession;
            }
        }

        public async Task OnSelectedVQS(ChangeEventArgs<int?, KeyValueVM> args)
        {
            if (args.ItemData != null)
            {
                this.nullableVQS = args.ItemData.IdKeyValue;
                this.model.Program.Speciality.IdVQS = (int)this.nullableVQS;
                this.frameworkSource = (await frameworkProgramService.GetAllFrameworkProgramsBySpecialityVQSIdAsync(this.model.Program.Speciality.IdVQS)).ToList();
            }
            else
            {
                this.model.Program.Speciality.IdVQS = 0;
                this.nullableVQS = null;
            }
        }

        public async Task openModal(string type = "")
        {
            this.isVisible = true;
            this.type = type;
            courseType = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MeasureType")).ToList();
            trainingCourseType = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).Where(x => x.DefaultValue1 == null).ToList();
            assignType = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType")).ToList();
            VQS = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS")).ToList();
            formEducation = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation")).ToList();

            this.candidateProviders = (await candidateProviderService.GetAllCandidateProvidersAsync()).ToList();
            this.professionSource = (await professionService.GetAllActiveProfessionsAsync()).ToList();
            this.specialitySource = (await specialityService.GetAllActiveSpecialitiesAsync()).ToList();
            this.frameworkSource = (await frameworkProgramService.GetAllFrameworkProgramsAsync(new FrameworkProgramVM())).ToList();
            this.locationSource = (await locationService.GetAllLocationsAsync()).ToList();
            this.StateHasChanged();
        }

        protected override void OnInitialized()
        {
            createNewCourse();
            nullableVQS = null;
            this.editContext = new EditContext(model);
        }
        private async Task OnFilterLocationCorrespondence(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    var query = new Query().Where(new WhereFilter() { Field = "kati", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.sfAutoCompleteLocation.FilterAsync(this.locationSource, query);
                }
                catch (Exception e) { }
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
    }
}
