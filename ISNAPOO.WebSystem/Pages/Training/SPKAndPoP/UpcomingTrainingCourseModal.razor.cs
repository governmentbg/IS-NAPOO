using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class UpcomingTrainingCourseModal : BlazorBaseComponent, IConcurrencyCheck<CourseVM>
    {
        private SelectTrainingProgramModal selectTrainingProgramModal = new SelectTrainingProgramModal();
        private CurrentTrainingCourseModal currentTrainingCourseModal = new CurrentTrainingCourseModal();

        private CourseVM courseVM = new CourseVM();
        private IEnumerable<CandidateProviderPremisesVM> premisesSource = new List<CandidateProviderPremisesVM>();
        private IEnumerable<KeyValueVM> formEducationSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> originalFormEducationSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> typeFrameworkProgramSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> professionalTrainingSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> courseStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> measureTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> assignTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> originalAssignTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> vqsSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> minimumLevelEducationSource = new List<KeyValueVM>();
        private string title = string.Empty;
        private List<string> validationMessages = new List<string>();
        private KeyValueVM kvUpcomingCourse = new KeyValueVM();
        private KeyValueVM kvPartProfession = new KeyValueVM();
        private ProgramVM selectedProgram = new ProgramVM();
        private string emptyPremises = string.Empty;
        private bool hideBtnsConcurrentModal = false;
        private ValidationMessageStore? messageStore;
        private int idCourseType = 0;
        private bool isTrainingProgramSelected = false;

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.courseVM);
        }

        public async Task OpenModal(CourseVM course, IEnumerable<KeyValueVM> formEducationSource, int idCourseType, ConcurrencyInfo concurrencyInfo = null)
        {
            await this.LoadKVDataAsync();

            this.courseVM = course;
            if (this.courseVM.IdCourse != 0)
            {
                this.courseVM = await this.TrainingService.GetTrainingCourseByIdAsync(course.IdCourse);

                this.IdTrainingCourse = this.courseVM.IdCourse;

                this.FilterAssignTypeSource();
            }

            this.formEducationSource = formEducationSource;
            this.originalFormEducationSource = formEducationSource.ToList();

            if (this.courseVM.Program is not null)
            {
                this.isTrainingProgramSelected = true;
                this.selectedProgram = this.courseVM.Program;
                this.SetFrameworkData();
                await this.LoadPremisesDataAsync();
            }
            else
            {
                this.selectedProgram = new ProgramVM();
                this.isTrainingProgramSelected = false;
                this.premisesSource = new List<CandidateProviderPremisesVM>();
            }

            this.courseVM.ProgramName = this.selectedProgram.ProgramName;

            this.idCourseType = idCourseType;

            this.SetTitle();

            this.validationMessages.Clear();

            await this.SetCreateAndModifyInfoAsync();

            this.isVisible = true;
            this.StateHasChanged();

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

        private async Task LoadPremisesDataAsync()
        {
            this.premisesSource = await this.TrainingService.GetAllActiveCandidateProviderPremisesByIdCandidateProviderAndIdSpecialityAsync(this.courseVM.IdCandidateProvider.Value, this.courseVM.Program.IdSpeciality);
        }

        private async Task LoadKVDataAsync()
        {
            this.minimumLevelEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MinimumLevelEducation");

            this.typeFrameworkProgramSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram");
            this.kvPartProfession = this.typeFrameworkProgramSource.FirstOrDefault(x => x.KeyValueIntCode == "PartProfession");

            this.professionalTrainingSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            this.measureTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MeasureType");
            this.assignTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType");
            this.originalAssignTypeSource = this.assignTypeSource.ToList();
            this.vqsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");

            this.courseStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseStatus");
            this.kvUpcomingCourse = this.courseStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "CourseStatusUpcoming");
        }

        private async Task SubmitBtn(bool showToast)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.courseVM);
                this.editContext.EnableDataAnnotationsValidation();
                this.messageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidateProgramSelected;
                this.editContext.OnValidationRequested += this.ValidateCourseEndDate;
                this.validationMessages.Clear();

                if (this.editContext.Validate())
                {
                    if (await this.TrainingService.IsCandidateProviderLicenceSuspendedAsync(this.courseVM.IdCandidateProvider!.Value, this.courseVM.StartDate!.Value))
                    {
                        await this.ShowErrorAsync("Не можете да създадете предстоящ курс на обучение, тъй като в момента сте с временно отнета лицензия!");
                        this.SpinnerHide();
                        return;
                    }

                    if (await this.IsAnnualReportSubmittedOrApprovedAsync())
                    {
                        await this.ShowErrorAsync($"Не можете да запишете предстоящ курс с 'Дата на завършване на курса' {this.courseVM.EndDate!.Value.Year} г., защото има данни за подаден годишен отчет за {this.courseVM.EndDate!.Value.Year} г.!");
                        this.SpinnerHide();
                        return;
                    }

                    if (await this.IsSpecialityFromSPPOORemovedWithOrderBeforeCourseStartDateAsync())
                    {
                        await this.ShowErrorAsync($"Не можете да създадете предстоящ курс на обучение за специалност, която е отпаднала от Списъка на професиите за професионално образование и обучение!");
                        this.SpinnerHide();
                        return;
                    }

                    var dosMsg = await this.CheckForDOSChangesWithoutActualizationOfCurriculumsAsync();
                    if (!string.IsNullOrEmpty(dosMsg))
                    {
                        await this.ShowErrorAsync(dosMsg);
                        this.SpinnerHide();
                        return;
                    }

                    var result = new ResultContext<CourseVM>();
                    this.courseVM.Program = this.selectedProgram;
                    result.ResultContextObject = this.courseVM;
                    var addForConcurrencyCheck = false;
                    if (this.courseVM.IdCourse == 0)
                    {
                        result = await this.TrainingService.CreateTrainingCourseAsync(result);
                        addForConcurrencyCheck = true;
                    }
                    else
                    {
                        result = await this.TrainingService.UpdateTrainingCourseAsync(result);
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        if (showToast)
                        {
                            await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                        }

                        await this.SetCreateAndModifyInfoAsync();

                        this.SetTitle();

                        if (addForConcurrencyCheck)
                        {
                            await this.AddEntityIdAsCurrentlyOpened(this.courseVM.IdCourse, "TrainingCourse");
                            this.IdTrainingCourse = this.courseVM.IdCourse;
                        }

                        await this.CallbackAfterSubmit.InvokeAsync();
                    }
                }
                else
                {
                    this.validationMessages.AddRange(this.editContext.GetValidationMessages());
                    await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenSelectProgramModal()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.selectTrainingProgramModal.OpenModal(this.courseVM, this.professionalTrainingSource, this.typeFrameworkProgramSource, this.vqsSource, this.idCourseType);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task GetSelectedProgramAfterModalSubmit(ProgramVM program)
        {
            this.selectedProgram = program;
            this.courseVM.Program = this.selectedProgram;
            this.courseVM.ProgramName = this.selectedProgram.ProgramName;
            this.courseVM.IdProgram = this.selectedProgram.IdProgram;
            this.courseVM.CourseName = this.selectedProgram.ProgramName;
            this.courseVM.SelectableHours = this.selectedProgram.SelectableHours;
            this.courseVM.MandatoryHours = this.selectedProgram.MandatoryHours;

            this.SetFrameworkData();

            await this.LoadPremisesDataAsync();

            this.isTrainingProgramSelected = true;

            await this.ShowSuccessAsync("Програмата за обучение е избрана успешно!");
        }

        public async Task SetProgramDataFromTrainingModal(ProgramVM program)
        {
            this.selectedProgram = program;
            this.courseVM.Program = this.selectedProgram;
            this.courseVM.ProgramName = this.selectedProgram.ProgramName;
            this.courseVM.IdProgram = this.selectedProgram.IdProgram;
            this.courseVM.SelectableHours = this.selectedProgram.SelectableHours;
            this.courseVM.MandatoryHours = this.selectedProgram.MandatoryHours;
            this.isTrainingProgramSelected = true;

            this.SetFrameworkData();

            await this.LoadPremisesDataAsync();
        }

        private void SetFrameworkData()
        {
            if (this.selectedProgram.FrameworkProgram is not null)
            {
                var kvMinimumEducationLevel = this.minimumLevelEducationSource.FirstOrDefault(x => x.IdKeyValue == this.selectedProgram.FrameworkProgram.IdMinimumLevelEducation);
                if (kvMinimumEducationLevel is not null)
                {
                    this.selectedProgram.FrameworkProgram.MinimumLevelEducationName = kvMinimumEducationLevel.Name;
                }

                if (this.selectedProgram.FrameworkProgram.FrameworkProgramFormEducations is not null && this.selectedProgram.FrameworkProgram.FrameworkProgramFormEducations.Any())
                {
                    var ids = this.selectedProgram.FrameworkProgram.FrameworkProgramFormEducations.Select(x => x.IdFormEducation).ToList();
                    var forms = this.originalFormEducationSource.Where(x => ids.Contains(x.IdKeyValue)).ToList();
                    this.selectedProgram.FrameworkProgram.FormEducationNames = string.Join(", ", forms.Select(x => x.Name).ToList());

                    this.formEducationSource = this.originalFormEducationSource.Where(x => forms.Contains(x)).ToList();
                }
            }
        }

        private async Task StartTrainingCourseBtn()
        {
            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;

                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да стартирате предстоящия курс?");
                if (isConfirmed)
                {
                    await this.SubmitBtn(false);

                    if (!this.editContext.GetValidationMessages().Any())
                    {
                        if (await this.IsSpecialityFromSPPOORemovedWithOrderBeforeCourseStartDateAsync())
                        {
                            await this.ShowErrorAsync($"Не можете да стартирате курс на обучение за специалност, която е отпаднала от Списъка на професиите за професионално образование и обучение!");
                            this.SpinnerHide();
                            return;
                        }

                        var dosMsg = await this.CheckForDOSChangesWithoutActualizationOfCurriculumsAsync();
                        if (!string.IsNullOrEmpty(dosMsg))
                        {
                            await this.ShowErrorAsync(dosMsg);
                            this.SpinnerHide();
                            return;
                        }

                        var inputContext = new ResultContext<CourseVM>();
                        inputContext.ResultContextObject = this.courseVM;
                        inputContext = await this.TrainingService.StartUpcomingTrainingCourseAsync(inputContext);
                        if (inputContext.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join(Environment.NewLine, inputContext.ListErrorMessages));
                        }
                        else
                        {
                            //this.isVisible = false;

                            await this.ShowSuccessAsync(string.Join(Environment.NewLine, inputContext.ListMessages));

                            await this.CallbackAfterSubmit.InvokeAsync();

                            this.SpinnerShow();

                            var courseFromDb = await this.TrainingService.GetTrainingCourseByIdAsync(this.courseVM.IdCourse);
                            await this.currentTrainingCourseModal.OpenModal(courseFromDb);

                            this.SpinnerHide();
                        }
                    }
                }
            }
            finally
            {
                this.loading = false;
            }
        }

        private void PremisesSelectedHandler(ChangeEventArgs<int?, CandidateProviderPremisesVM> args)
        {
            if (args.ItemData is not null)
            {
                this.courseVM.CandidateProviderPremises = args.ItemData;
            }
            else
            {
                this.courseVM.CandidateProviderPremises = null;
            }
        }

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.courseVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.courseVM.IdModifyUser);
            this.courseVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.courseVM.IdCreateUser);
        }

        private void SetTitle()
        {
            if (this.courseVM.IdCourse != 0)
            {
                this.title = $"Данни за предстоящ курс <span style=\"color: #ffffff;\">{this.courseVM.CourseName}</span>";
            }
            else
            {
                this.title = "Данни за предстоящ курс";
            }
        }

        private void OnMeasureTypeSelected(ChangeEventArgs<int?, KeyValueVM> args)
        {
            if (args.ItemData is not null)
            {
                if (args.ItemData.KeyValueIntCode == "EmploymentProgrammesAndMeasures")
                {
                    this.assignTypeSource = this.originalAssignTypeSource.Where(x => x.DefaultValue3 != null);
                }
                else
                {
                    this.assignTypeSource = this.originalAssignTypeSource.Where(x => x.DefaultValue3 == null);
                }
            }
            else
            {
                this.courseVM.IdAssignType = null;
            }
        }

        private void FilterAssignTypeSource()
        {
            if (this.courseVM.IdMeasureType is not null)
            {
                var kvMeasureType = this.measureTypeSource.FirstOrDefault(x => x.IdKeyValue == this.courseVM.IdMeasureType.Value);
                if (kvMeasureType is not null)
                {
                    if (kvMeasureType.KeyValueIntCode == "EmploymentProgrammesAndMeasures")
                    {
                        this.assignTypeSource = this.originalAssignTypeSource.Where(x => x.DefaultValue3 != null);
                    }
                    else
                    {
                        this.assignTypeSource = this.originalAssignTypeSource.Where(x => x.DefaultValue3 == null);
                    }
                }
            }
        }

        private async Task<bool> IsAnnualReportSubmittedOrApprovedAsync()
        {
            return await this.TrainingService.IsAnnualReportSubmittedOrApprovedByIdCandidateProviderAndYearAsync(this.courseVM.IdCandidateProvider!.Value, this.courseVM.EndDate!.Value.Year);
        }

        private async Task<bool> IsSpecialityFromSPPOORemovedWithOrderBeforeCourseStartDateAsync()
        {
            return await this.TrainingService.IsSpecialityFromSPPOORemovedWithOrderBeforeCourseStartDateAsync(this.courseVM.Program.IdSpeciality, this.courseVM.StartDate!.Value);
        }

        // Проверява за промени в ДОС-а и неактулизарани учебни планове
        private async Task<string> CheckForDOSChangesWithoutActualizationOfCurriculumsAsync()
        {
            return await this.TrainingService.AreDOSChangesWithoutActualizationOfCurriculumsAsync(this.courseVM);
        }

        private DateTime GetCourseMinimalEndDate()
        {
            var totalHours = (this.courseVM.SelectableHours.HasValue ? this.courseVM.SelectableHours.Value : 0) + (this.courseVM.MandatoryHours.HasValue ? this.courseVM.MandatoryHours.Value : 0);
            var workingDays = Math.Ceiling(totalHours / 8d);
            var additionalDaysFromWeek = (int)(workingDays / 6);
            var totalDaysForCourse = workingDays + additionalDaysFromWeek;
            var minimalDate = this.courseVM.StartDate!.Value.Date.AddDays(totalDaysForCourse);
            return minimalDate;
        }

        private void ValidateProgramSelected(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.selectedProgram.IdProgram == 0)
            {
                FieldIdentifier fi = new FieldIdentifier(this.courseVM, "ProgramName");
                this.messageStore?.Add(fi, "Полето 'Програма за обучение' е задължително!");
            }
        }

        private void ValidateCourseEndDate(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.courseVM.StartDate.HasValue && this.courseVM.EndDate.HasValue)
            {
                if (this.courseVM.StartDate.Value.Date > this.courseVM.EndDate.Value.Date)
                {
                    FieldIdentifier field = new FieldIdentifier(this.courseVM, "EndDate");
                    this.messageStore?.Add(field, $"Полето 'Дата за завършване на курса' не може да бъде преди {this.courseVM.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г.!");
                    return;
                }

                var minimalDate = this.GetCourseMinimalEndDate();
                if (minimalDate > this.courseVM.EndDate.Value.Date)
                {
                    FieldIdentifier field = new FieldIdentifier(this.courseVM, "EndDate");
                    this.messageStore?.Add(field, $"Полето 'Дата за завършване на курса' не може да бъде преди {minimalDate.ToString(GlobalConstants.DATE_FORMAT)} г.! Датата на завършване трябва да е съобразена с въведените часове в учебния план.");
                }
            }
        }
    }
}
