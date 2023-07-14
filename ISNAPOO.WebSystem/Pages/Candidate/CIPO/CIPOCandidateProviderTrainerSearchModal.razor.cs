using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOCandidateProviderTrainerSearchModal
    {
        private CandidateProviderSearchVM currentFilter = new();
        private IEnumerable<KeyValueVM> kvDocumentType;
        private IEnumerable<KeyValueVM> kvEducation;
        private IEnumerable<KeyValueVM> kvStatus;
        private readonly List<CandidateProviderSearchVM> searchData = new();
        private SfDialog sfDialog = new();
        private List<SpecialityVM> specialities = new();

        private List<SpecialityVM> specialitiesSource = new();
        private List<CandidateProviderTrainerVM> trainers = new();

        [Parameter] public EventCallback<List<CandidateProviderTrainerVM>> CallBackRefreshGrid { get; set; }

        [Inject] public IDataSourceService DataSourceService { get; set; }

        [Inject] public ISpecialityService SpecialityService { get; set; }

        [Inject] public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject] public IJSRuntime JsRuntime { get; set; }

        [Inject] public IProfessionService ProfessionService { get; set; }

        [Inject] public IProfessionService ProfessionalDirectionService { get; set; }


        public async Task OpenModal(CandidateProviderVM CandidateProviderVM,
            List<CandidateProviderTrainerVM> candidateProviderTrainerVM)
        {
            searchData.Clear();
            specialities.Clear();
            this.trainers.Clear();
            this.trainers =
                (await CandidateProviderService.GetAllActiveCandidateProvidersTrainersByIdCandidateProviderAsync(
                    CandidateProviderVM.IdCandidate_Provider)).FirstOrDefault().CandidateProviderTrainers.ToList();
            (await ProfessionService.GetAllAsync()).ToList();
            (await ProfessionalDirectionService.GetAllProfessionalDirections()).ToList();
            await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
            kvDocumentType = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerContractType");
            kvStatus = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderTrainerStatus");
            await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ComplianceDOC");
            await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
            kvEducation = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Education");

            foreach (var CandidateProviderSpeciality in CandidateProviderVM.CandidateProviderSpecialities)
            {
                specialities.Add(await SpecialityService.GetSpecialityByIdAsync(CandidateProviderSpeciality.IdSpeciality));
            }

            foreach (var trainers in this.trainers)
            {
                foreach (var speciality in trainers.CandidateProviderTrainerSpecialities)
                {
                    if (!searchData.Any(x => x.id == speciality.IdSpeciality))
                    {
                        var temp = new CandidateProviderSearchVM
                        {
                            id = speciality.IdSpeciality,
                            Name = SpecialityService.GetSpecialityById(speciality.IdSpeciality).CodeAndName
                        };
                        searchData.Add(temp);
                    }
                }
            }

            specialitiesSource = specialities;
            isVisible = true;
            StateHasChanged();
        }

        public async Task ClearFilter()
        {
            currentFilter = new CandidateProviderSearchVM();
            specialities = specialitiesSource;
        }

        public async Task SumbitFilter()
        {
            trainers = trainers.Where(d =>
                (!String.IsNullOrEmpty(currentFilter.Name)
                    ? d.FirstName.Trim().ToLower().ToString().Contains(currentFilter.Name.Trim().ToLower())
                    : true)
                && (!String.IsNullOrEmpty(currentFilter.MiddleName)
                    ? d.SecondName.Trim().ToLower().ToString().Contains(currentFilter.MiddleName.Trim().ToLower())
                    : true)
                && (!String.IsNullOrEmpty(currentFilter.FamilyName)
                    ? d.FamilyName.Trim().ToLower().ToString().Contains(currentFilter.FamilyName.Trim().ToLower())
                    : true)
                && (!String.IsNullOrEmpty(currentFilter.Indent) ? d.Indent.Contains(currentFilter.Indent) : true)
                && (!String.IsNullOrEmpty(currentFilter.EducationCertificateNotes)
                    ? d.FirstName.Contains(currentFilter.EducationCertificateNotes)
                    : true)
                && (!String.IsNullOrEmpty(currentFilter.EducationSpecialityNotes)
                    ? d.FirstName.Contains(currentFilter.EducationSpecialityNotes)
                    : true)
                && (currentFilter.kvPracticeOrTheory is not null && currentFilter.kvPracticeOrTheory != 0
                    ? d.CandidateProviderTrainerSpecialities.Any(x => x.IdUsage == currentFilter.kvPracticeOrTheory)
                    : true)
                && (currentFilter.IdComplianceDOC is not null && currentFilter.IdComplianceDOC != 0
                    ? d.CandidateProviderTrainerSpecialities.Any(x => x.IdComplianceDOC == currentFilter.IdComplianceDOC)
                    : true)
                && (currentFilter.IdStatus != 0 ? d.IdStatus == currentFilter.IdStatus : true)
                && (currentFilter.IdContractType is not null && currentFilter.IdContractType != 0
                    ? d.IdContractType == currentFilter.IdContractType
                    : true)
                && (currentFilter.IdEducation != 0 ? d.IdEducation == currentFilter.IdEducation : true)
                && (currentFilter.Specialities is not null && currentFilter.Specialities.Count != 0
                    ? d.CandidateProviderTrainerSpecialities.Any(y =>
                        currentFilter.Specialities.All(x => x.IdSpeciality == y.IdSpeciality))
                    : true)
                && ((currentFilter.Specialities is null || currentFilter.Specialities.Count == 0) &&
                    currentFilter.IdProfession != 0
                    ? d.CandidateProviderTrainerSpecialities.Any(y =>
                        y.Speciality.IdProfession == currentFilter.IdProfession)
                    : true)
                && (currentFilter.IdProfessionalDirection != 0 &&
                    (currentFilter.Specialities is null || currentFilter.Specialities.Count == 0) &&
                    currentFilter.IdProfession == 0
                    ? d.CandidateProviderTrainerSpecialities.Any(y =>
                        y.Speciality.Profession.IdProfessionalDirection == currentFilter.IdProfessionalDirection)
                    : true)
            ).ToList();
            await CallBackRefreshGrid.InvokeAsync(trainers.ToList());
            isVisible = false;
        }
    }
}
