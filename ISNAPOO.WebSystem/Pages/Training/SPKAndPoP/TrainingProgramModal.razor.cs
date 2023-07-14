using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Navigations;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class TrainingProgramModal : BlazorBaseComponent, IConcurrencyCheck<ProgramVM>
    {
        private TrainingProgram trainingProgram = new TrainingProgram();
        private TrainingCurriculum trainingCurriculum = new TrainingCurriculum();
        private UpcomingTrainingCourseModal upcomingTrainingCourseModal = new UpcomingTrainingCourseModal();

        private string title = string.Empty;
        private List<string> validationMessages = new List<string>();
        private int selectedTab = 0;
        private int idCourseType = 0;
        private ProgramVM programVM = new ProgramVM();
        private bool hideBtnsConcurrentModal = false;
        private bool hideBtnsWhenSPK = false;
        private KeyValueVM kvActiveStatus = new KeyValueVM();

        public override bool IsContextModified => this.trainingProgram.IsEditContextModified();

        [Parameter]
        public EventCallback CallBackAfterProgramSubmit { get; set; }

        [Parameter]
        public EventCallback<int> CallbackAfterCurriculumSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override void OnInitialized()
        {
            this.SetEditContext();
        }

        public async Task OpenModal(ProgramVM program, string type, int idCourseType, ConcurrencyInfo concurrencyInfo = null)
        {
            this.SpinnerShow();

            this.selectedTab = 0;
            this.idCourseType = idCourseType;

            if (type.Contains("SPK"))
            {
                this.hideBtnsWhenSPK = true;
            }

            this.programVM = program;
            if (this.programVM.IdProgram != 0)
            {
                this.programVM = await this.TrainingService.GetTrainingProgramByIdAsync(program.IdProgram);
                await this.SetCreateAndModifyInfoAsync();
                this.IdProgram = this.programVM.IdProgram;
            }

            this.kvActiveStatus = await this.DataSourceService.GetKeyValueByIntCodeAsync("StatusTemplate", "Active");

            this.SetModalTitle();

            this.validationMessages.Clear();

            this.SetEditContext();

            this.isVisible = true;
            this.StateHasChanged();

            this.SpinnerHide();

            if (concurrencyInfo is not null && concurrencyInfo.IdPerson != this.UserProps.IdPerson)
            {
                this.hideBtnsConcurrentModal = true;
                this.SetPersonFullNameAndVisibilityOfDialog(concurrencyInfo);
            }
            else
            {
                this.hideBtnsConcurrentModal = false;
            }
        }

        private void SetModalTitle()
        {
            this.title = string.IsNullOrEmpty(this.programVM.ProgramName) ? "Данни за програма за обучение" : $"Данни за програма за обучение <span style=\"color: #ffffff;\">{this.programVM.ProgramName}</span>";
        }

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.programVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.programVM.IdModifyUser);
            this.programVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.programVM.IdCreateUser);
        }

        private void SetEditContext()
        {
            this.editContext = new EditContext(this.programVM);
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

                this.validationMessages.Clear();
                this.trainingProgram.SubmitHandler();
                this.validationMessages.AddRange(this.trainingProgram.GetValidationMessages());

                if (this.programVM.IdSpeciality != 0 && this.programVM.IdFrameworkProgram.HasValue && !string.IsNullOrWhiteSpace(this.programVM.ProgramNumber))
                {
                    var doesProgramWithSameNumberFrameworkProgramAndSpecialityExist = await this.DoesProgramWithSameNumberFrameworkProgramAndSpecialityExistAsync();
                    if (doesProgramWithSameNumberFrameworkProgramAndSpecialityExist)
                    {
                        var msg = $"Вече има въведена програма за обучение с номер '{this.programVM.ProgramNumber}', за същата рамкова програма и същата специалност! Моля, въведете уникален номер на програма! (Програма за обучение)";
                        this.validationMessages.Add(msg);
                    }
                }

                if (this.programVM.IdFrameworkProgram.HasValue && this.programVM.FrameworkProgram is not null && this.programVM.FrameworkProgram.IdStatus.HasValue && this.programVM.FrameworkProgram.IdStatus.Value != this.kvActiveStatus.IdKeyValue)
                {
                    this.validationMessages.Add("Не можете да запишете програма за обучение за стара рамкова програма! (Програма за обучение)");
                }

                if (!this.validationMessages.Any())
                {
                    var inputContext = new ResultContext<ProgramVM>();
                    inputContext.ResultContextObject = this.programVM;
                    var result = new ResultContext<ProgramVM>();
                    var addForConcurrentCheck = false;
                    if (this.programVM.IdProgram == 0)
                    {
                        result = await this.TrainingService.CreateTrainingProgramAsync(inputContext);
                        addForConcurrentCheck = true;
                    }
                    else
                    {
                        result = await this.TrainingService.UpdateTrainingProgramAsync(inputContext);
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                        await this.SetCreateAndModifyInfoAsync();
                        this.SetModalTitle();

                        if (addForConcurrentCheck)
                        {
                            this.IdProgram = this.programVM.IdProgram;
                            await this.AddEntityIdAsCurrentlyOpened(this.programVM.IdProgram, "TrainingProgram");
                        }

                        await this.CallBackAfterProgramSubmit.InvokeAsync();
                    }
                }
                else
                {
                    await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task<bool> DoesProgramWithSameNumberFrameworkProgramAndSpecialityExistAsync()
        {
            return await this.TrainingService.DoesProgramWithSameNumberFrameworkProgramAndSpecialityExistAsync(this.programVM);
        }

        private void Select(SelectingEventArgs args)
        {
            if (args.IsSwiped)
            {
                args.Cancel = true;
            }
        }

        private async Task UpdateAfterCurriculumSubmit()
        {
            await this.CallbackAfterCurriculumSubmit.InvokeAsync();
        }

        private async Task CreateUpcomingCourseBtn()
        {
            var speciality = this.DataSourceService.GetAllSpecialitiesList().FirstOrDefault(x => x.IdSpeciality == this.programVM.IdSpeciality);
            if (speciality is not null && speciality.IdStatus != this.DataSourceService.GetActiveStatusID())
            {
                await this.ShowErrorAsync("Не можете да създадете предстоящ курс за обучение за специалност, която е отпаднала от СППОО!");
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

                if (this.programVM.IdFrameworkProgram.HasValue && this.programVM.FrameworkProgram is not null && this.programVM.FrameworkProgram.IdStatus.HasValue && this.programVM.FrameworkProgram.IdStatus.Value != this.kvActiveStatus.IdKeyValue)
                {
                    await this.ShowErrorAsync("Не можете да създадете курс за стара рамкова програма!");
                }
                else
                {
                    var formEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation");
                    await this.upcomingTrainingCourseModal.OpenModal(new CourseVM() { IdCandidateProvider = this.UserProps.IdCandidateProvider, CourseName = this.programVM.ProgramName }, formEducationSource, this.idCourseType);
                    await this.upcomingTrainingCourseModal.SetProgramDataFromTrainingModal(this.programVM);

                    this.isVisible = false;
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
    }
}
