using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using NuGet.DependencyResolver;

using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;


namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CandidateProviderPremisesSpecialityModal : BlazorBaseComponent
    {
        private SfGrid<SpecialityVM> specialitiesGrid = new SfGrid<SpecialityVM>();

        private List<CandidateProviderPremisesVM> candidateProviderPremises = new List<CandidateProviderPremisesVM>();
        private List<SpecialityVM> specialitiesSource = new List<SpecialityVM>();
        private List<SpecialityVM> selectedSpecialties = new List<SpecialityVM>();
        private IEnumerable<ProfessionVM> professionSource = new List<ProfessionVM>();
        private IEnumerable<ProfessionalDirectionVM> professionalDirectionsSource = new List<ProfessionalDirectionVM>();
        private bool specialitySelected = false;
        List<string> TrainingTypes = new List<string>() { "Практика", "Теория", "Теория и Практика" };
        private string currentTrainingType { get; set; } = "Изберете вид";

        List<string> DocTypes = new List<string>() { "Напълно", "Не съответства", "Частично" };
        private string currentDocType { get; set; } = "Изберете";
        KeyValueVM kvTheory;
        KeyValueVM kvPractice;
        KeyValueVM kvPracticeAndTheory;

        KeyValueVM kvDocComplianceYes;
        KeyValueVM kvDocComplianceNo;
        KeyValueVM kvDocCompliancePartial;

        [Parameter]
        public EventCallback<int> CallbackAfterSpecialitiesSelected { get; set; }

        [Inject]
        public IDOCService DOCService { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        public async Task OpenModal(List<CandidateProviderPremisesVM> candidateProviderPremises, ICollection<CandidateProviderSpecialityVM> selectedSpecialities, IEnumerable<ProfessionVM> professions, IEnumerable<ProfessionalDirectionVM> professionalDirections)
        {
            this.professionSource = professions;
            this.professionalDirectionsSource = professionalDirections;
            this.specialitySelected = false;
            this.selectedSpecialties.Clear();
            var docs = await this.DOCService.GetAllDocAsync();
            this.candidateProviderPremises = candidateProviderPremises;
            kvTheory = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TheoryTraining");
            kvPractice = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "PracticalTraining");
            kvPracticeAndTheory = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TrainingInTheoryAndPractice");
            kvDocComplianceYes = await DataSourceService.GetKeyValueByIntCodeAsync("ComplianceDOC", "Does");
            kvDocComplianceNo = await DataSourceService.GetKeyValueByIntCodeAsync("ComplianceDOC", "Doesnt");
            kvDocCompliancePartial = await DataSourceService.GetKeyValueByIntCodeAsync("ComplianceDOC", "Partial");
            this.currentDocType = "";
            this.currentTrainingType = "";

            var listSpecialityIds = selectedSpecialities.Select(x => x.IdSpeciality).ToList();
            this.specialitiesSource = (await this.SpecialityService.GetSpecialitiesByListIdsAsync(listSpecialityIds)).ToList();
            foreach (var speciality in this.candidateProviderPremises.FirstOrDefault().CandidateProviderPremisesSpecialities.Where(x => x.IdUsage == this.kvPracticeAndTheory.IdKeyValue))
            {
                if (this.specialitiesSource.Any(x => x.IdSpeciality == speciality.IdSpeciality))
                {
                    this.specialitiesSource.Remove(this.specialitiesSource.First(x => x.IdSpeciality == speciality.IdSpeciality));
                }
            }

            foreach (var speciality in this.specialitiesSource)
            {
                if (speciality.IdDOC is not null)
                {
                    speciality.DOCMTBRequirements = docs.FirstOrDefault(x => x.IdDOC == speciality.IdDOC).RequirementsMaterialBase;
                }
            }

            this.specialitiesSource = this.specialitiesSource.OrderBy(x => x.Code).ToList();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task AddSelectedSpecialitiesHandler()
        {
            if (!this.specialitySelected || currentTrainingType == null || currentDocType == null)
            {
                await this.ShowErrorAsync("Моля, изберете специалност(и) от списъка, Вид на провежданото обучение и Съответствие с ДОС!");
            }
            else
            {
                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    var premises = this.candidateProviderPremises[0];
                    var listAddedSpecialities = new List<CandidateProviderPremisesSpecialityVM>();
                    foreach (var speciality in this.selectedSpecialties)
                    {
                        var idUsage = 0;
                        if (currentTrainingType == "Теория и Практика")
                        {
                            idUsage = kvPracticeAndTheory.IdKeyValue;
                        }
                        else if (currentTrainingType == "Практика")
                        {
                            idUsage = kvPractice.IdKeyValue;
                        }
                        else if (currentTrainingType == "Теория")
                        {
                            idUsage = kvTheory.IdKeyValue;
                        }

                        var idComplianceDOC = 0;
                        if (currentDocType == "Частично")
                        {
                            idComplianceDOC = kvDocCompliancePartial.IdKeyValue;
                        }
                        else if (currentDocType == "Напълно")
                        {
                            idComplianceDOC = kvDocComplianceYes.IdKeyValue;
                        }
                        else if (currentDocType == "Не съответства")
                        {
                            idComplianceDOC = kvDocComplianceNo.IdKeyValue;
                        }

                        if (idUsage == this.kvTheory.IdKeyValue && premises.CandidateProviderPremisesSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality && x.IdCandidateProviderPremises == premises.IdCandidateProviderPremises && (x.IdUsage == this.kvTheory.IdKeyValue || x.IdUsage == this.kvPracticeAndTheory.IdKeyValue)))
                        {
                            await this.ShowErrorAsync($"Специалността '{speciality.Name}' съществува. \n Не може да добавяте специалност/специалности, които вече съществуват!");
                            this.SpinnerHide();
                            this.loading = false;
                            return;
                        }

                        if (idUsage == this.kvPractice.IdKeyValue && premises.CandidateProviderPremisesSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality && x.IdCandidateProviderPremises == premises.IdCandidateProviderPremises && (x.IdUsage == this.kvPractice.IdKeyValue || x.IdUsage == this.kvPracticeAndTheory.IdKeyValue)))
                        {
                            await this.ShowErrorAsync($"Специалността '{speciality.Name}' съществува. \n Не може да добавяте специалност/специалности, които вече съществуват!");
                            this.SpinnerHide();
                            this.loading = false;
                            return;
                        }

                        if (idUsage == this.kvPracticeAndTheory.IdKeyValue && premises.CandidateProviderPremisesSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality && x.IdCandidateProviderPremises == premises.IdCandidateProviderPremises && x.IdUsage == this.kvPracticeAndTheory.IdKeyValue))
                        {
                            await this.ShowErrorAsync($"Специалността '{speciality.Name}' съществува. \n Не може да добавяте специалност/специалности, които вече съществуват!");
                            this.SpinnerHide();
                            this.loading = false;
                            return;
                        }

                        var premisesSpeciality = new CandidateProviderPremisesSpecialityVM()
                        {
                            IdSpeciality = speciality.IdSpeciality,
                            IdCandidateProviderPremises = premises.IdCandidateProviderPremises,
                            IdComplianceDOC = idComplianceDOC,
                            IdUsage = idUsage
                        };

                        listAddedSpecialities.Add(premisesSpeciality);
                    }

                    var result = await this.CandidateProviderService.AddSpecialitiesToPremisesByListSpecialitiesAsync(listAddedSpecialities);
                    if (!result.HasErrorMessages)
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                        this.isVisible = false;

                        await this.CallbackAfterSpecialitiesSelected.InvokeAsync(premises.IdCandidateProviderPremises);
                    }
                    else
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task SpecialityDeselectedHandler(RowDeselectEventArgs<SpecialityVM> args)
        {
            this.selectedSpecialties.Clear();
            this.selectedSpecialties = await this.specialitiesGrid.GetSelectedRecordsAsync();

            if (!this.selectedSpecialties.Any())
            {
                this.specialitySelected = false;
            }
        }

        private async Task SpecialitySelectedHandler(RowSelectEventArgs<SpecialityVM> args)
        {
            this.specialitySelected = true;
            this.selectedSpecialties.Clear();
            this.selectedSpecialties = await this.specialitiesGrid.GetSelectedRecordsAsync();
        }
    }
}
