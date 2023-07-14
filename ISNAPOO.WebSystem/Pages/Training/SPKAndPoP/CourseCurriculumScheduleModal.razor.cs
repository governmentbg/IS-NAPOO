using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.DropDowns;
using ISNAPOO.Common.Constants;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CourseCurriculumScheduleModal : BlazorBaseComponent
    {
        private SelectTopicModal selectTopicModal = new SelectTopicModal();

        private CourseScheduleVM courseScheduleVM = new CourseScheduleVM();
        private IEnumerable<KeyValueVM> curriculumScheduleTypeSource = new List<KeyValueVM>();
        private KeyValueVM kvTheory = new KeyValueVM();
        private bool hideBtnsConcurrentModal = false;
        private List<CourseScheduleVM> addedSchedulesSource = new List<CourseScheduleVM>();
        private List<CandidateProviderTrainerVM> trainerSource = new List<CandidateProviderTrainerVM>();
        private List<CandidateProviderPremisesVM> premisesSource = new List<CandidateProviderPremisesVM>();
        private string title = string.Empty;
        private List<string> validationMessages = new List<string>();
        private int idCourse = 0;
        private int idSpeciality = 0;
        private ValidationMessageStore? messageStore;
        private DateTime timeToMin;
        private bool timeFromEntered = false;
        private DateTime timeFromMin;
        private DateTime timeToMax;
        private CourseVM courseVM = new CourseVM();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Parameter] public bool IsEditable { get; set; } = true;


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
            this.editContext = new EditContext(this.courseScheduleVM);
        }

        public async Task OpenModal(CourseScheduleVM model, List<CourseScheduleVM> addedSchedulesSource, CourseVM course, int idSpeciality, ConcurrencyInfo concurrencyInfo = null)
        {
            this.editContext = new EditContext(this.courseScheduleVM);

            this.validationMessages.Clear();

            this.courseScheduleVM = model;
            this.addedSchedulesSource = addedSchedulesSource.ToList();
            this.idCourse = course.IdCourse;
            this.idSpeciality = idSpeciality;
            this.courseVM = course;

            if (this.courseScheduleVM.TimeFrom.HasValue)
            {
                this.timeFromEntered = true;
            }
            else
            {
                this.timeFromEntered = false;
            }

            this.timeFromMin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 7, 0, 0);
            this.timeToMax = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 22, 0, 0);

            await this.LoadNomenclaturesDataAsync();

            if (this.courseScheduleVM.IdCourseSchedule != 0)
            {
                await this.LoadPremisesDataAsync();
                await this.LoadTrainersDataAsync();
            }

            await this.SetCreateAndModifyInfoAsync();

            this.SetTitle();

            if (concurrencyInfo is not null && concurrencyInfo.IdPerson != this.UserProps.IdPerson)
            {
                this.hideBtnsConcurrentModal = true;
                this.SetPersonFullNameAndVisibilityOfDialog(concurrencyInfo);
            }
            else
            {
                this.hideBtnsConcurrentModal = false;
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task LoadNomenclaturesDataAsync()
        {
            this.curriculumScheduleTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingScheduleType");
            this.kvTheory = this.curriculumScheduleTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Theory");
        }

        private async Task LoadTrainersDataAsync()
        {
            this.trainerSource = (await this.TrainingService.GetTrainersByIdTrainingTypeByIdCourseAsync(this.courseScheduleVM.IdTrainingScheduleType, this.idCourse)).ToList();
        }

        private async Task LoadPremisesDataAsync()
        {
            this.premisesSource = (await this.TrainingService.GetPremisesByIdTrainingTypeByIdCourseAsync(this.courseScheduleVM.IdTrainingScheduleType, this.idCourse)).ToList();
        }

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.courseScheduleVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.courseScheduleVM.IdModifyUser);
            this.courseScheduleVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.courseScheduleVM.IdCreateUser);
        }

        private void SetTitle()
        {
            if (this.courseScheduleVM.IdCourseSchedule != 0)
            {
                this.title = $"Данни за график към тема <span style=\"color: #ffffff;\">{this.courseScheduleVM.TrainingCurriculum.Topic}</span>";
            }
            else
            {
                this.title = "Данни за график";
            }
        }

        private async Task ScheduleTypeSelectedHandler(ChangeEventArgs<int, KeyValueVM> args)
        {
            if (args.ItemData is not null)
            {
                await this.LoadTrainersDataAsync();
                await this.LoadPremisesDataAsync();
            }
            else
            {
                this.courseScheduleVM.IdCandidateProviderTrainer = null;
                this.courseScheduleVM.IdCandidateProviderPremises = null;
                this.trainerSource.Clear();
                this.premisesSource.Clear();
            }

            this.StateHasChanged();
        }

        private async Task SubmitAndContinueBtn()
        {
            await this.SubmitBtn(true);

            if (!this.validationMessages.Any())
            {
                this.courseScheduleVM = new CourseScheduleVM()
                {
                    TrainingCurriculum = new TrainingCurriculumVM()
                };

                this.SetTitle();
            }
        }

        private async Task SubmitBtn(bool saveAndContinueClicked = false)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.courseScheduleVM);
                this.editContext.EnableDataAnnotationsValidation();
                this.messageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidateTopicHours;
                this.editContext.OnValidationRequested += this.ValidateTimeInputs;
                this.editContext.OnValidationRequested += this.ValidateDate;
                this.validationMessages.Clear();

                if (this.editContext.Validate())
                {
                    var result = new ResultContext<CourseScheduleVM>();
                    result.ResultContextObject = this.courseScheduleVM;
                    var addForConcurrencyCheck = false;
                    if (this.courseScheduleVM.IdCourseSchedule == 0)
                    {
                        result = await this.TrainingService.CreateCourseScheduleAsync(result);
                        addForConcurrencyCheck = true;
                    }
                    else
                    {
                        result = await this.TrainingService.UpdateCourseScheduleAsync(result);
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                        await this.SetCreateAndModifyInfoAsync();

                        this.SetTitle();

                        if (addForConcurrencyCheck && !saveAndContinueClicked)
                        {
                            await this.AddEntityIdAsCurrentlyOpened(this.courseScheduleVM.IdCourseSchedule, "CourseSchedule");
                            this.IdCourseSchedule = this.courseScheduleVM.IdCourseSchedule;
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

        private async Task OpenSelectTopicModal()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var scheduleType = this.curriculumScheduleTypeSource.FirstOrDefault(x => x.IdKeyValue == this.courseScheduleVM.IdTrainingScheduleType)!.Name;
                var trainingCurriculums = (await this.TrainingService.GetAllTrainingCurriculumsByIdCourseAndByHoursTypeAsync(this.idCourse, scheduleType)).ToList();
                
                await this.HandleAlreadyAddedTopicsWithoutRemainingHoursAsync(trainingCurriculums);

                await this.selectTopicModal.OpenModal(trainingCurriculums, scheduleType, this.courseScheduleVM);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task HandleAlreadyAddedTopicsWithoutRemainingHoursAsync(List<TrainingCurriculumVM> trainingCurriculums)
        {
            this.addedSchedulesSource = (await this.TrainingService.GetCourseSchedulesBydIdCourseAsync(this.idCourse)).ToList();
            var addedCurriculumsFromSameScheduleType = this.addedSchedulesSource.Where(x => x.IdTrainingScheduleType == this.courseScheduleVM.IdTrainingScheduleType).ToList();
            var dict = new Dictionary<string, Dictionary<string, double>>();
            foreach (var schedule in addedCurriculumsFromSameScheduleType)
            {
                double hours = schedule.Hours.HasValue ? schedule.Hours.Value : 0;
                //if (schedule.IdTrainingScheduleType == this.kvTheory.IdKeyValue)
                //{
                //    hours = schedule.TrainingCurriculum.Theory.Value;
                //}
                //else
                //{
                //    hours = schedule.TrainingCurriculum.Practice.Value;
                //}

                var training = schedule.TrainingCurriculum;
                if (!dict.ContainsKey(training.Topic))
                {
                    dict.Add(training.Topic, new Dictionary<string, double>());
                }

                if (!dict[training.Topic].ContainsKey(training.ProfessionalTraining))
                {
                    dict[training.Topic].Add(training.ProfessionalTraining, hours);
                }
                else
                {
                    dict[training.Topic][training.ProfessionalTraining] += hours;
                }
            }

            for (int i = trainingCurriculums.Count - 1; i > -1; i--)
            {
                var trainingCurr = trainingCurriculums[i];
                var trainingCurrFromDict = dict.FirstOrDefault(x => x.Key == trainingCurr.Topic && x.Value.FirstOrDefault().Key == trainingCurr.ProfessionalTraining);
                if (!trainingCurrFromDict.Equals(new KeyValuePair<string, Dictionary<string, double>>()))
                {
                    var hours = trainingCurrFromDict.Value.FirstOrDefault().Value;
                    double hoursFromTrainingCurr;
                    if (this.courseScheduleVM.IdTrainingScheduleType == this.kvTheory.IdKeyValue)
                    {
                        hoursFromTrainingCurr = trainingCurr.Theory.Value;
                    }
                    else
                    {
                        hoursFromTrainingCurr = trainingCurr.Practice.Value;
                    }

                    if (hours >= hoursFromTrainingCurr)
                    {
                        trainingCurriculums.RemoveAll(x => x.IdTrainingCurriculum == trainingCurr.IdTrainingCurriculum);
                    }
                }
            }
        }

        private async Task UpdateAfterTopicSelected(TrainingCurriculumVM trainingCurriculum)
        {
            this.courseScheduleVM.TrainingCurriculum = trainingCurriculum;
            this.courseScheduleVM.IdTrainingCurriculum = trainingCurriculum.IdTrainingCurriculum;
            if (!this.addedSchedulesSource.Any(x => x.IdTrainingCurriculum == trainingCurriculum.IdTrainingCurriculum))
            {
                var type = this.curriculumScheduleTypeSource.FirstOrDefault(x => x.IdKeyValue == this.courseScheduleVM.IdTrainingScheduleType)!;
                if (type.Name == "Теория")
                {
                    this.courseScheduleVM.Hours = trainingCurriculum.Theory!.Value;
                }
                else
                {
                    this.courseScheduleVM.Hours = trainingCurriculum.Practice!.Value;
                }
            }

            await this.ShowSuccessAsync("Темата е избрана успешно!");
        }

        private void SetMinValueForTimeTo()
        {
            if (this.courseScheduleVM.TimeFrom is not null)
            {
                this.timeFromEntered = true;
                this.timeToMin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, this.courseScheduleVM.TimeFrom.Value.Hour, this.courseScheduleVM.TimeFrom.Value.Minute, this.courseScheduleVM.TimeFrom.Value.Second).AddMinutes(5);
            }
            else
            {
                this.timeFromEntered = false;
                this.courseScheduleVM.TimeTo = null;
            }
        }

        private void ValidateTopicHours(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.courseScheduleVM.TrainingCurriculum is not null && this.courseScheduleVM.IdTrainingScheduleType != 0)
            {
                var trainingType = this.curriculumScheduleTypeSource.FirstOrDefault(x => x.IdKeyValue == this.courseScheduleVM.IdTrainingScheduleType)!.Name;
                if (trainingType == "Теория")
                {
                    if (this.courseScheduleVM.TrainingCurriculum.Theory.HasValue)
                    {
                        if (this.courseScheduleVM.Hours > this.courseScheduleVM.TrainingCurriculum.Theory!.Value)
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.courseScheduleVM, "Hours");
                            this.messageStore?.Add(fi, "Въведените часове за обучение по теория не могат да надвишават общия брой часове по теория!");
                            return;
                        }
                    }
                }
                else
                {
                    if (this.courseScheduleVM.TrainingCurriculum.Practice.HasValue)
                    {
                        if (this.courseScheduleVM.Hours > this.courseScheduleVM.TrainingCurriculum.Practice!.Value)
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.courseScheduleVM, "Hours");
                            this.messageStore?.Add(fi, "Въведените часове за обучение по практика не могат да надвишават общия брой часове по практика!");
                            return;
                        }
                    }
                }

                var totalHoursTaught = this.addedSchedulesSource.Where(x => x.IdTrainingCurriculum == this.courseScheduleVM.IdTrainingCurriculum && x.IdTrainingScheduleType == this.courseScheduleVM.IdTrainingScheduleType).Sum(x => x.Hours);
                if (trainingType == "Теория")
                {
                    if (this.courseScheduleVM.TrainingCurriculum.Theory.HasValue)
                    {
                        if (this.courseScheduleVM.Hours + totalHoursTaught > this.courseScheduleVM.TrainingCurriculum.Theory!.Value)
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.courseScheduleVM, "Hours");
                            this.messageStore?.Add(fi, "Въведените часове за обучение по теория не могат да надвишават общия брой часове по теория!");
                            return;
                        }
                    }
                }
                else
                {
                    if (this.courseScheduleVM.TrainingCurriculum.Practice.HasValue)
                    {
                        if (this.courseScheduleVM.Hours + totalHoursTaught > this.courseScheduleVM.TrainingCurriculum.Practice!.Value)
                        {
                            FieldIdentifier fi = new FieldIdentifier(this.courseScheduleVM, "Hours");
                            this.messageStore?.Add(fi, "Въведените часове за обучение по практика не могат да надвишават общия брой часове по практика!");
                            return;
                        }
                    }
                }
            }
        }

        private void ValidateTimeInputs(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.courseScheduleVM.TimeFrom.HasValue && this.courseScheduleVM.TimeTo.HasValue)
            {
                if (this.courseScheduleVM.TimeFrom > this.courseScheduleVM.TimeTo)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.courseScheduleVM, "TimeFrom");
                    this.messageStore?.Add(fi, "Полето 'Продължителност от' не може да бъде по-късно от полето 'Продължителност до'!");
                }

                if (this.courseScheduleVM.TimeTo < this.courseScheduleVM.TimeFrom)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.courseScheduleVM, "TimeTo");
                    this.messageStore?.Add(fi, "Полето 'Продължителност до' не може да бъде по-рано от полето 'Продължителност от'!");
                }
            }
        }

        private void ValidateDate(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.courseScheduleVM.ScheduleDate.HasValue && this.courseVM.StartDate.HasValue && this.courseVM.EndDate.HasValue)
            {
                if (this.courseScheduleVM.ScheduleDate.Value < this.courseVM.StartDate.Value)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.courseScheduleVM, "ScheduleDate");
                    this.messageStore?.Add(fi, $"Полето 'Дата' не може да бъде преди {this.courseVM.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г.!");
                }

                if (this.courseScheduleVM.ScheduleDate.Value > this.courseVM.EndDate.Value)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.courseScheduleVM, "ScheduleDate");
                    this.messageStore?.Add(fi, $"Полето 'Дата' не може да бъде след {this.courseVM.EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г.!");
                }
            }
        }
    }
}
