using Data.Models.Data.Training;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Registers.DocumentsFromCPO
{
    public partial class DocumentsFromCPOFilter : BlazorBaseComponent
    {
        private SfAutoComplete<int?, CandidateProviderVM> cpAutoComplete = new SfAutoComplete<int?, CandidateProviderVM>();
        private SfAutoComplete<int?, LocationVM> locationAutoComplete = new SfAutoComplete<int?, LocationVM>();
        private SfAutoComplete<int?, ProfessionVM> professionAutoComplete = new SfAutoComplete<int?, ProfessionVM>();
        private SfAutoComplete<int?, SpecialityVM> specialityAutoComplete = new SfAutoComplete<int?, SpecialityVM>();

        private TrainedPeopleFilterVM model = new TrainedPeopleFilterVM();
        private List<CandidateProviderVM> candidateProvidersSource = new List<CandidateProviderVM>();
        private List<LocationVM> locationSource = new List<LocationVM>();
        private List<LocationVM> originalLocationSource = new List<LocationVM>();
        private List<DistrictVM> districtSource = new List<DistrictVM>();
        private List<DistrictVM> originalDistrictSource = new List<DistrictVM>();
        private List<MunicipalityVM> municipalitySource = new List<MunicipalityVM>();
        private List<MunicipalityVM> originalMunicipalitySource = new List<MunicipalityVM>();
        private List<ProfessionVM> professionSource = new List<ProfessionVM>();
        private List<ProfessionVM> originalProfessionSource = new List<ProfessionVM>();
        private List<SpecialityVM> specialitySource = new List<SpecialityVM>();
        private List<SpecialityVM> originalSpecialitySource = new List<SpecialityVM>();
        private List<KeyValueVM> nationalitiesTypesSource = new List<KeyValueVM>();
        private List<KeyValueVM> sexTypesSource = new List<KeyValueVM>();
        private List<KeyValueVM> courseTypesSource = new List<KeyValueVM>();
        private List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsSource = new List<TypeOfRequestedDocumentVM>();

        [Parameter]
        public EventCallback<List<ClientCourseVM>> CallbackAfterSubmit { get; set; }

        [Parameter]
        public EventCallback<TrainedPeopleFilterVM> CallbackAfterSubmitOnProfessionalCertificateList { get; set; }

        [Parameter]
        public EventCallback<List<DocumentsFromCPORegisterVM>> CallbackAfterSubmitOnDocumentsFromCPOList { get; set; }

        [Inject]
        public ICandidateProviderService candidateProviderService { get; set; }

        [Inject]
        public ILocationService locationService { get; set; }

        [Inject]
        public IMunicipalityService municipalityService { get; set; }

        [Inject]
        public IDistrictService districtService { get; set; }

        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        [Inject]
        public ISpecialityService specialityService { get; set; }

        [Inject]
        public IProfessionService professionService { get; set; }

        [Inject]
        public IProviderDocumentRequestService providerDocumentRequestService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        private string PageFrom;

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(string pageFrom)
        {
            this.PageFrom = pageFrom;
            this.editContext = new EditContext(this.model);

            if (!this.candidateProvidersSource.Any())
            {
                this.candidateProvidersSource = (await this.candidateProviderService.GetAllCandidateProvidersForAutoComplete("LicensingCPO")).ToList();
            }

            if (!this.originalMunicipalitySource.Any())
            {
                this.originalMunicipalitySource = (await municipalityService.GetAllMunicipalitiesAsync()).ToList();
                this.municipalitySource = originalMunicipalitySource.ToList();
            }

            if (!this.originalDistrictSource.Any())
            {
                this.originalDistrictSource = (await districtService.GetAllDistrictsAsync()).ToList();
                this.districtSource = this.originalDistrictSource.ToList();
            }

            if (!this.originalProfessionSource.Any())
            {
                this.originalProfessionSource = (await this.professionService.GetAllActiveProfessionsAsync()).ToList();
                this.professionSource = this.originalProfessionSource.ToList();
            }
            
            if (!this.originalSpecialitySource.Any())
            {
                this.originalSpecialitySource = (await this.specialityService.GetAllActiveSpecialitiesAsync()).ToList();
                this.specialitySource = this.originalSpecialitySource.ToList();
            }

            if (!this.originalLocationSource.Any())
            {
                this.originalLocationSource = (await this.locationService.GetAllLocationsAsync()).ToList();
                this.locationSource = this.originalLocationSource.ToList();
            }
            
            this.nationalitiesTypesSource = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality")).ToList();
            this.sexTypesSource = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex")).ToList();
            this.courseTypesSource = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MeasureType")).ToList();

            if (!this.typeOfRequestedDocumentsSource.Any())
            {
                this.typeOfRequestedDocumentsSource = (await providerDocumentRequestService.GetAllValidTypesOfRequestedDocumentAsync()).ToList();
                if (this.PageFrom == "DocumentsFromCPOList")
                {
                    this.typeOfRequestedDocumentsSource = this.typeOfRequestedDocumentsSource.Where(x => x.DocTypeOfficialNumber == "3-54" || x.DocTypeOfficialNumber == "3-54В"
                    || x.DocTypeOfficialNumber == "3-54а" || x.DocTypeOfficialNumber == "3-54аВ").ToList();
                }
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        public void OnProfessionValueChangeHandler(ChangeEventArgs<int?, ProfessionVM> args)
        {
            this.SpinnerShow();

            if (args is not null && args.ItemData is not null)
            {
                this.specialitySource = this.originalSpecialitySource.Where(x => x.IdProfession == args.ItemData.IdProfession).ToList();
            }
            else
            {
                this.specialitySource = this.originalSpecialitySource.ToList();
            }

            this.SpinnerHide();
        }

        public void OnSpecialityValueChangeHandler(ChangeEventArgs<int?, SpecialityVM> args)
        {
            this.SpinnerShow();

            if (args is not null && args.ItemData is not null)
            {
                this.professionSource = this.originalProfessionSource.Where(x => x.IdProfession == args.ItemData.IdProfession).ToList();
            }
            else
            {
                this.professionSource = this.originalProfessionSource.ToList();
            }

            this.SpinnerHide();
        }

        public void ClearFilter()
        {
            this.model = new TrainedPeopleFilterVM();
        }

        public async Task Save()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!this.model.IdCandidateProvider.HasValue && string.IsNullOrEmpty(this.model.LicenceNumber) && !this.model.IdCourseLocation.HasValue
                    && !this.model.IdCourseDistrict.HasValue && !this.model.IdCourseMunicipality.HasValue && string.IsNullOrEmpty(this.model.FirstName)
                    && string.IsNullOrEmpty(this.model.FamilyName) && string.IsNullOrEmpty(this.model.Indent) && !this.model.IdNationality.HasValue
                    && !this.model.IdSex.HasValue && !this.model.IdMeasureType.HasValue && string.IsNullOrEmpty(this.model.CourseName) 
                    && !this.model.IdProfession.HasValue && !this.model.IdSpeciality.HasValue && !this.model.CourseStartFrom.HasValue
                    && !this.model.CourseStartTo.HasValue && !this.model.CourseEndFrom.HasValue && !this.model.CourseEndTo.HasValue
                    && string.IsNullOrEmpty(this.model.DocumentRegNo) && !this.model.IdTypeOfRequestedDocument.HasValue && !this.model.DocumentDateFrom.HasValue && !this.model.DocumentDateTo.HasValue)
                {
                    await this.ShowErrorAsync("Моля, изберете поне един критерий, по който да филтрирате данни за обучени лица!");
                    this.loading = false;
                    this.SpinnerHide();
                    return;
                }

                if (this.PageFrom == "ProfessionalCertificateList")
                {
                    await this.CallbackAfterSubmitOnProfessionalCertificateList.InvokeAsync(this.model);
                }
                else if (this.PageFrom == "DocumentsFromCPOList")
                {
                    var documents = await this.TrainingService.GetRIDPKDocumentsForRegisterAsync(this.model);

                    await this.CallbackAfterSubmitOnDocumentsFromCPOList.InvokeAsync(documents);
                }
                else
                {
                    var clients = await this.TrainingService.GetAllTrainedPeopleFilterAsync(this.model);

                    await this.CallbackAfterSubmit.InvokeAsync(clients);
                }

                this.isVisible = false;
                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public void OnDistrictChangeHandler(ChangeEventArgs<int?, DistrictVM> args)
        {
            this.SpinnerShow();

            if (args is not null && args.ItemData is not null)
            {
                this.municipalitySource = this.originalMunicipalitySource.Where(x => x.idDistrict == args.ItemData.idDistrict).ToList();
                this.locationSource = this.originalLocationSource.Where(x => this.municipalitySource.Select(x => x.idMunicipality).Contains(x.idMunicipality!.Value)).ToList();
            }
            else
            {
                this.municipalitySource = this.originalMunicipalitySource.ToList();
                this.locationSource = this.originalLocationSource.ToList();
            }

            this.SpinnerHide();
        }

        public void OnMunicipalityChangeHandler(ChangeEventArgs<int?, MunicipalityVM> args)
        {
            this.SpinnerShow();

            if (args is not null && args.ItemData is not null)
            {
                this.districtSource = this.originalDistrictSource.Where(x => x.idDistrict == args.ItemData.idDistrict).ToList();
                this.locationSource = this.originalLocationSource.Where(x => x.idMunicipality == args.ItemData.idMunicipality).ToList();
            }
            else
            {
                this.districtSource = this.originalDistrictSource.ToList();
                this.locationSource = this.originalLocationSource.ToList();
            }

            this.SpinnerHide();
        }

        public void OnLocationChangeHandler(ChangeEventArgs<int?, LocationVM> args)
        {
            this.SpinnerShow();

            if (args.ItemData != null)
            {
                this.municipalitySource = this.originalMunicipalitySource.Where(x => x.idMunicipality == args.ItemData.idMunicipality!.Value).ToList();
                this.districtSource = this.originalDistrictSource.Where(x => this.municipalitySource.Select(x => x.idDistrict).Contains(x.idDistrict)).ToList();
            }
            else
            {
                this.municipalitySource = this.originalMunicipalitySource.ToList();
                this.districtSource = this.originalDistrictSource.ToList();
            }

            this.SpinnerHide();
        }

        private async Task OnFilterCandidateProviderHandler(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            var query = new Query().Where(new WhereFilter() { Field = "ProviderJoinedInformation", Operator = "contains", value = args.Text, IgnoreCase = true });

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await this.cpAutoComplete.FilterAsync(this.candidateProvidersSource, query);
        }

        private async Task OnFilterLocationHandler(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            var query = new Query().Where(new WhereFilter() { Field = "kati", Operator = "contains", value = args.Text, IgnoreCase = true });

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await this.locationAutoComplete.FilterAsync(this.locationSource, query);
        }

        private async Task OnFilterProfessionHandler(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            var query = new Query().Where(new WhereFilter() { Field = "ComboBoxName", Operator = "contains", value = args.Text, IgnoreCase = true });

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await this.professionAutoComplete.FilterAsync(this.professionSource, query);
        }

        private async Task OnFilterSpecialityHandler(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            var query = new Query().Where(new WhereFilter() { Field = "CodeAndName", Operator = "contains", value = args.Text, IgnoreCase = true });

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await this.specialityAutoComplete.FilterAsync(this.specialitySource, query);
        }
    }
}

