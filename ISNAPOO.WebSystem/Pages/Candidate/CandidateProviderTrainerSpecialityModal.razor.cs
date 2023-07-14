using Data.Models.Data.SPPOO;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;

using Microsoft.AspNetCore.Components;

using NuGet.DependencyResolver;

using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.RichTextEditor;
using Syncfusion.Blazor.SplitButtons;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CandidateProviderTrainerSpecialityModal : BlazorBaseComponent
    {
        private SfGrid<SpecialityVM> specialitiesGrid = new SfGrid<SpecialityVM>();

        private CandidateProviderTrainerVM candidateProviderTrainer = new CandidateProviderTrainerVM();
        private List<SpecialityVM> specialitiesSource = new List<SpecialityVM>();
        private List<SpecialityVM> selectedSpecialties = new List<SpecialityVM>();
        private IEnumerable<ProfessionVM> professionSource = new List<ProfessionVM>();
        private IEnumerable<ProfessionalDirectionVM> professionalDirectionsSource = new List<ProfessionalDirectionVM>();
        private bool specialitySelected = false;
        List<string> TrainingTypes = new List<string>() { "Практика", "Теория", "Теория и Практика" };
        private string currentTrainingType { get; set; } = "Изберете вид";

        List<string> DocTypes = new List<string>() { "Напълно", "Не съответства", "Частично" };
        private string currentDocType { get; set; } = "Изберете";

        [Parameter]
        public EventCallback<int> CallbackAfterSpecialitiesSelected { get; set; }

        [Parameter]
        public EventCallback<List<CandidateProviderTrainerSpecialityVM>> CallbackAfterCPSSpecialitiesSelected { get; set; }

        [Inject]
        public IDOCService DOCService { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }
        [Inject]
        public IProfessionService professionService { get; set; }
        [Inject]
        public IProfessionalDirectionService professionalDirectionService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        KeyValueVM kvTheory;
        KeyValueVM kvPractice;
        KeyValueVM kvPracticeAndTheory;

        KeyValueVM kvDocComplianceYes;
        KeyValueVM kvDocComplianceNo;
        KeyValueVM kvDocCompliancePartial;


        public async Task OpenModal(CandidateProviderTrainerVM candidateProviderTrainer, List<CandidateProviderSpecialityVM> candidateProviderSpecialities)
        {
            this.candidateProviderTrainer = candidateProviderTrainer;
            kvTheory = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TheoryTraining");
            kvPractice = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "PracticalTraining");
            kvPracticeAndTheory = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TrainingInTheoryAndPractice");
            kvDocComplianceYes = await DataSourceService.GetKeyValueByIntCodeAsync("ComplianceDOC", "Does");
            kvDocComplianceNo = await DataSourceService.GetKeyValueByIntCodeAsync("ComplianceDOC", "Doesnt");
            kvDocCompliancePartial = await DataSourceService.GetKeyValueByIntCodeAsync("ComplianceDOC", "Partial");
            this.professionSource = await this.professionService.GetAllActiveProfessionsAsync();
            this.professionalDirectionsSource = await this.professionalDirectionService.GetAllActiveProfessionalDirectionsAsync();
            this.specialitySelected = false;
            this.selectedSpecialties.Clear();
            var docs = await this.DOCService.GetAllDocAsync();
            this.currentDocType = "";
            this.currentTrainingType = "";

            var listSpecialityIds = candidateProviderSpecialities.Select(x => x.IdSpeciality).ToList();
            this.specialitiesSource = (await this.SpecialityService.GetSpecialitiesByListIdsAsync(listSpecialityIds)).ToList();

            if (this.candidateProviderTrainer.IdCandidateProviderTrainer != 0)
            {
                var profDirIdsFromTrainerProfile = this.candidateProviderTrainer.CandidateProviderTrainerProfiles.Select(x => x.IdProfessionalDirection).ToList();
                if (profDirIdsFromTrainerProfile.Any())
                {
                    this.specialitiesSource = this.specialitiesSource.Where(x => profDirIdsFromTrainerProfile.Contains(x.Profession.IdProfessionalDirection)).ToList();
                }
                else
                {
                    this.specialitiesSource.Clear();
                }
            }

            foreach (var speciality in this.candidateProviderTrainer.CandidateProviderTrainerSpecialities.Where(x => x.IdUsage == this.kvPracticeAndTheory.IdKeyValue))
            {
                if (this.specialitiesSource.Any(x => x.IdSpeciality == speciality.IdSpeciality))
                {
                    this.specialitiesSource.Remove(this.specialitiesSource.First(x => x.IdSpeciality == speciality.IdSpeciality));
                }
            }

            this.specialitiesSource = this.specialitiesSource.OrderBy(x => x.Code).ToList();

            foreach (var speciality in this.specialitiesSource)
            {
                if (speciality.IdDOC is not null)
                {
                    speciality.DOCTrainerRequirements = docs.FirstOrDefault(x => x.IdDOC == speciality.IdDOC).RequirementsТrainers;
                }
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task AddSelectedSpecialitiesHandler()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!this.specialitySelected || currentTrainingType == null || currentDocType == null)
                {
                    await this.ShowErrorAsync("Моля, изберете специалност(и) от списъка, Вид на провежданото обучение и Съответствие с ДОС!");
                }
                else
                {
                    if (!this.candidateProviderTrainer.CandidateProviderTrainerProfiles.Any())
                    {
                        await this.ShowErrorWhenProfDirIsNotPresent();
                        this.SpinnerHide();
                        this.loading = false;
                        return;
                    }

                    var listAddedSpecialities = new List<CandidateProviderTrainerSpecialityVM>();
                    foreach (var speciality in this.selectedSpecialties)
                    {
                        if (currentTrainingType == "Теория и Практика")
                        {
                            speciality.IdUsage = kvPracticeAndTheory.IdKeyValue;
                        }
                        else
                        {
                            if (currentTrainingType == "Практика")
                            {
                                speciality.IdUsage = kvPractice.IdKeyValue;
                            }
                            if (currentTrainingType == "Теория")
                            {
                                speciality.IdUsage = kvTheory.IdKeyValue;
                            }
                        }
                        if (currentDocType == "Частично")
                        {
                            speciality.IdComplianceDOC = kvDocCompliancePartial.IdKeyValue;
                        }
                        else
                        {
                            if (currentDocType == "Напълно")
                            {
                                speciality.IdComplianceDOC = kvDocComplianceYes.IdKeyValue;
                            }
                            if (currentDocType == "Не съответства")
                            {
                                speciality.IdComplianceDOC = kvDocComplianceNo.IdKeyValue;
                            }
                        }

                        if (speciality.IdUsage == this.kvTheory.IdKeyValue && this.candidateProviderTrainer.CandidateProviderTrainerSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality && x.IdCandidateProviderTrainer == this.candidateProviderTrainer.IdCandidateProviderTrainer && (x.IdUsage == this.kvTheory.IdKeyValue || x.IdUsage == this.kvPracticeAndTheory.IdKeyValue)))
                        {
                            await this.ShowErrorAsync($"Специалността '{speciality.Name}' съществува. \n Не може да добавяте специалност/специалности, които вече съществуват!");
                            this.SpinnerHide();
                            this.loading = false;
                            return;
                        }

                        if (speciality.IdUsage == this.kvPractice.IdKeyValue && this.candidateProviderTrainer.CandidateProviderTrainerSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality && x.IdCandidateProviderTrainer == this.candidateProviderTrainer.IdCandidateProviderTrainer && (x.IdUsage == this.kvPractice.IdKeyValue || x.IdUsage == this.kvPracticeAndTheory.IdKeyValue)))
                        {
                            await this.ShowErrorAsync($"Специалността '{speciality.Name}' съществува. \n Не може да добавяте специалност/специалности, които вече съществуват!");
                            this.SpinnerHide();
                            this.loading = false;
                            return;
                        }

                        if (speciality.IdUsage == this.kvPracticeAndTheory.IdKeyValue && this.candidateProviderTrainer.CandidateProviderTrainerSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality && x.IdCandidateProviderTrainer == this.candidateProviderTrainer.IdCandidateProviderTrainer && x.IdUsage == this.kvPracticeAndTheory.IdKeyValue))
                        {
                            await this.ShowErrorAsync($"Специалността '{speciality.Name}' съществува. \n Не може да добавяте специалност/специалности, които вече съществуват!");
                            this.SpinnerHide();
                            this.loading = false;
                            return;
                        }

                        var professions = this.professionSource.Where(x => x.IdProfession == speciality.IdProfession).ToList();
                        var listIds = professions.Select(x => x.IdProfessionalDirection).ToList();
                        var professionalDirections = this.professionalDirectionsSource.Where(x => listIds.Contains(x.IdProfessionalDirection)).FirstOrDefault();
                        if (professionalDirections != null)
                        {
                            if (!this.candidateProviderTrainer.CandidateProviderTrainerProfiles.Any(x => x.IdProfessionalDirection == professionalDirections.IdProfessionalDirection))
                            {
                                await this.ShowErrorWhenProfDirIsNotPresent();
                                this.SpinnerHide();
                                this.loading = false;
                                return;
                            }
                        }
                        else
                        {
                            await this.ShowErrorWhenProfDirIsNotPresent();
                            this.SpinnerHide();
                            this.loading = false;
                            return;
                        }

                        if (this.candidateProviderTrainer.CandidateProviderTrainerProfiles.Any(x => x.IdProfessionalDirection == speciality.Profession.ProfessionalDirection.IdProfessionalDirection && !string.IsNullOrEmpty(x.Usage) && x.Usage.Contains(currentTrainingType)))
                        {
                            CandidateProviderTrainerSpecialityVM temp = new CandidateProviderTrainerSpecialityVM()
                            {
                                IdSpeciality = speciality.IdSpeciality,
                                IdCandidateProviderTrainer = this.candidateProviderTrainer.IdCandidateProviderTrainer,
                                IdComplianceDOC = speciality.IdComplianceDOC,
                                IdUsage = speciality.IdUsage
                            };

                            listAddedSpecialities.Add(temp);
                        }
                        else
                        {
                            await this.ShowErrorAsync("Избраната специалност и вид на проведеното обучение не отговарят на въведената информация за професионално направление в секция 'Преподавателска дейност' на преподавателя.");
                            this.SpinnerHide();
                            this.loading = false;
                            return;
                        }
                    }

                    var result = await this.CandidateProviderService.AddSpecialitiesToTrainerByListSpecialitiesAsync(listAddedSpecialities);
                    if (!result.HasErrorMessages)
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                        this.isVisible = false;

                        await this.CallbackAfterSpecialitiesSelected.InvokeAsync(this.candidateProviderTrainer.IdCandidateProviderTrainer);
                    }
                    else
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task ShowErrorWhenProfDirIsNotPresent()
        {
            await this.ShowErrorAsync("Не можете да добавяте специалност към преподавател, който не преподава по избраното професионално направление!");
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
            //var temp = 
            this.selectedSpecialties = await this.specialitiesGrid.GetSelectedRecordsAsync();//(await this.SpecialityService.GetSpecialitiesByListIdsAsync(temp.Select(x => x.IdSpeciality).ToList())).ToList();
        }
    }
}
