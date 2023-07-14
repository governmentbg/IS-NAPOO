using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class SelectTrainingProgramModal : BlazorBaseComponent
    {
        private SfGrid<ProgramVM> programsGrid = new SfGrid<ProgramVM>();

        private IEnumerable<ProgramVM> programsSource = new List<ProgramVM>();
        private CourseVM courseVM = new CourseVM();
        private IEnumerable<KeyValueVM> professionalTrainingSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> typeFrameworkProgramSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> vqsSource = new List<KeyValueVM>();
        private int idCourseType = 0;
        private string providerOwner = string.Empty;
        private string providerName = string.Empty;
        private bool isOpenedFromLegalCapacity = false;

        [Parameter]
        public EventCallback<ProgramVM> CallbackAfterSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        public async Task OpenModal(CourseVM course, IEnumerable<KeyValueVM> professionalTrainingSource, IEnumerable<KeyValueVM> typeFrameworkProgramSource, IEnumerable<KeyValueVM> vqsSource, int idCourseType, bool isOpenedFromLegalCapacity = false)
        {
            var candidateProvider = await this.CandidateProviderService.GetCandidateProviderWithoutAnythingIncludedByIdAsync(this.UserProps.IdCandidateProvider);
            this.providerOwner = candidateProvider.ProviderOwner;
            this.providerName = candidateProvider.ProviderName;
            this.idCourseType = idCourseType;
            this.isOpenedFromLegalCapacity = isOpenedFromLegalCapacity;
            this.courseVM = course;
            this.professionalTrainingSource = professionalTrainingSource;
            this.typeFrameworkProgramSource = typeFrameworkProgramSource;
            this.vqsSource = vqsSource;

            await this.LoadProgramsDataAsync();

            this.isVisible = true;
            this.StateHasChanged();

            await this.HandleRowSelectionAsync();
        }

        private async Task LoadProgramsDataAsync()
        {
            var kvFrameworkInactiveValue = await this.DataSourceService.GetKeyValueByIntCodeAsync("StatusTemplate", "Inactive");
            var kvBType = this.professionalTrainingSource.FirstOrDefault(x => x.KeyValueIntCode == "B").IdKeyValue;
            var activeSpecialities = this.DataSourceService.GetAllSpecialitiesList().Where(x => x.IdStatus == this.DataSourceService.GetActiveStatusID()).Select(x => x.IdSpeciality).ToList();
            if (!this.isOpenedFromLegalCapacity)
            {
                this.programsSource = await this.TrainingService.GetAllActiveProgramsByIdCandidateProviderAsync(this.courseVM.IdCandidateProvider.Value);
                this.programsSource = this.programsSource.Where(x => x.IdCourseType == this.idCourseType && x.FrameworkProgram.IdStatus != kvFrameworkInactiveValue.IdKeyValue && x.IdSpeciality != 0 && activeSpecialities.Contains(x.IdSpeciality)).ToList();
            }
            else
            {
                this.programsSource = (await this.TrainingService.GetAllActiveLegalCapacityProgramsByIdCandidateProviderAsync(this.courseVM.IdCandidateProvider.Value)).Where(x => x.FrameworkProgram.IdStatus != kvFrameworkInactiveValue.IdKeyValue && x.IdSpeciality != 0 && activeSpecialities.Contains(x.IdSpeciality)).ToList();
            }

            if (!this.isOpenedFromLegalCapacity)
            {
                foreach (var program in this.programsSource)
                {
                    if (program.FrameworkProgram is not null)
                    {
                        var type = this.typeFrameworkProgramSource.FirstOrDefault(x => x.IdKeyValue == program.FrameworkProgram.IdTypeFrameworkProgram);
                        if (type is not null)
                        {
                            program.CourseTypeName = type.Name;
                        }

                        var vqs = this.vqsSource.FirstOrDefault(x => x.IdKeyValue == program.Speciality.IdVQS);
                        if (vqs is not null)
                        {
                            program.Speciality.VQS_Name = vqs.Name;
                        }
                    }
                }
            }
        }

        private async Task SubmitBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var selectedPrograms = await this.programsGrid.GetSelectedRecordsAsync();
                if (!selectedPrograms.Any())
                {
                    await this.ShowErrorAsync("Моля, изберете учебна програма за обучение от списъка!");
                    return;
                }

                var selectedProgram = selectedPrograms.FirstOrDefault();

                this.isVisible = false;

                await this.CallbackAfterSubmit.InvokeAsync(selectedProgram);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task HandleRowSelectionAsync()
        {
            if (this.courseVM.IdProgram != 0)
            {
                var idx = await this.programsGrid.GetRowIndexByPrimaryKeyAsync(this.courseVM.Program.IdProgram);
                if (idx != -1)
                {
                    await this.programsGrid.SelectRowAsync(idx);
                }
            }
        }

        private async Task RowSelectingHandler()
        {
            var selectedRows = await this.programsGrid.GetSelectedRecordsAsync();
            if (selectedRows.Any())
            {
                await this.programsGrid.ClearRowSelectionAsync();
            }
        }
    }
}
