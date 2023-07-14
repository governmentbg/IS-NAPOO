using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore.Metadata;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Action = System.Action;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class TrainingCurriculumModal : BlazorBaseComponent, IConcurrencyCheck<TrainingCurriculumVM>
    {
        private SfGrid<ERUVM> erusGrid = new SfGrid<ERUVM>();

        private TrainingCurriculumVM trainingCurriculumVM = new TrainingCurriculumVM();
        private IEnumerable<KeyValueVM> professionalTrainingSource = new List<KeyValueVM>();
        private List<ERUVM> addedErus = new List<ERUVM>();
        private int idEru = 0;
        private List<ERUVM> eruDataSourceForComboBox = new List<ERUVM>();
        private SpecialityVM speciality = new SpecialityVM();
        private DocVM doc = new DocVM();
        private List<ERUVM> allErus = new List<ERUVM>();
        private List<TrainingCurriculumVM> addedCurriculums = new List<TrainingCurriculumVM>();
        private int idCandidateProviderSpeciality = 0;
        private int nextId = 0;
        private int previousId = 0;
        private string CreationDateStr = "";
        private string ModifyDateStr = "";
        private List<string> validationMessages = new List<string>();
        private bool disableNextBtn = false;
        private bool disablePreviousBtn = false;
        private bool hideBtnsConcurrentModal = false;
        private bool disableBtnsWhenEnteredFromEdit = false;
        private KeyValueVM kvBType = new KeyValueVM();
        private ValidationMessageStore? messageStore;
        private CourseVM courseForValidation = new CourseVM();
        private TrainingCurriculumVM trainingCurrForValidation = new TrainingCurriculumVM();
        private string type = string.Empty;
        private List<ERUVM> erusSource;


        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback<ResultContext<TrainingCurriculumVM>> CallbackAfterSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IDOCService DOCService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IERUSpecialityService ERUSpecialityService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.trainingCurriculumVM);
            this.professionalTrainingSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            this.kvBType = this.professionalTrainingSource.FirstOrDefault(x => x.KeyValueIntCode == "B");

            this.eruDataSourceForComboBox = new List<ERUVM>();
        }

        public async Task OpenModal(TrainingCurriculumVM trainingCurriculumVM, List<TrainingCurriculumVM> addedCurriculums, int idCandidateProviderSpeciality, SpecialityVM speciality, string type = null, ConcurrencyInfo concurrencyInfo = null)
        {
            this.idCandidateProviderSpeciality = idCandidateProviderSpeciality;
            this.allErus.Clear();
            this.validationMessages.Clear();
            this.addedCurriculums = addedCurriculums;
            this.disableBtnsWhenEnteredFromEdit = false;

            if (!string.IsNullOrEmpty(type))
            {
                this.type = type;
            }
            else
            {
                this.type = string.Empty;
            }

            this.idEru = 0;
            this.trainingCurriculumVM = trainingCurriculumVM;
            await UpdateCurriculumData();
            if (this.trainingCurriculumVM.IdTrainingCurriculum != 0)
            {
                this.IdTrainingProgramCurriculum = this.trainingCurriculumVM.IdTrainingCurriculum;
                this.CreationDateStr = this.trainingCurriculumVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.trainingCurriculumVM.ModifyDate.ToString("dd.MM.yyyy");
                this.trainingCurriculumVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.trainingCurriculumVM.IdModifyUser);
                this.trainingCurriculumVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.trainingCurriculumVM.IdCreateUser);

                if (this.trainingCurriculumVM.IdProfessionalTraining != this.kvBType.IdKeyValue)
                {
                    this.disableBtnsWhenEnteredFromEdit = true;
                }
            }
            else
            {
                this.trainingCurriculumVM.IdProfessionalTraining = this.kvBType.IdKeyValue;
            }

            this.editContext = new EditContext(this.trainingCurriculumVM);
            this.speciality = speciality;

            if (this.speciality.IdDOC != null)
            {
                this.doc = await this.DOCService.GetDOCByIdAsync(new DocVM() { IdDOC = this.speciality.IdDOC ?? default });
            }

            var candidateCurriculumEruIds = this.trainingCurriculumVM.SelectedERUs.Select(x => x.IdERU).ToList();
            this.addedErus = (await this.DOCService.GetERUsByIdsAsync(candidateCurriculumEruIds)).ToList();
            this.eruDataSourceForComboBox.Clear();
            this.erusSource = this.DataSourceService.GetAllERUsList();
            this.GetERUsData();

            await this.SetERUsData();

            this.SetButtonsState();

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

        private void GetERUsData()
        {
            if (this.trainingCurriculumVM.IdProfessionalTraining != 0)
            {
                this.eruDataSourceForComboBox.Clear();
                this.eruDataSourceForComboBox = this.allErus.Where(x => x.IdProfessionalTraining == this.trainingCurriculumVM.IdProfessionalTraining).OrderBy(x => x.ERUIntCodeSplit).ToList();
            }
        }

        private async Task UpdateCurriculumData()
        {
            if (this.trainingCurriculumVM.IdCourse != null)
            {
                this.addedCurriculums =
                    (await this.TrainingService.GetTrainingCurriculumByIdCourseAsync(this.trainingCurriculumVM
                        .IdCourse.Value)).ToList();
                foreach (var trainingCurriculum in this.addedCurriculums)
                {
                    var value = this.professionalTrainingSource
                        .FirstOrDefault(x => x.IdKeyValue == trainingCurriculum.IdProfessionalTraining)
                        ?.DefaultValue1;
                    trainingCurriculum.ProfessionalTraining = value;

                    var erus = this.addedErus.Where(x =>
                        trainingCurriculum.TrainingCurriculumERUs.Select(x => x.IdERU).ToList()
                            .Contains(x.IdERU));
                    trainingCurriculum.SelectedERUs.AddRange(erus);
                }
                this.trainingCurriculumVM = this.addedCurriculums.FirstOrDefault(x =>
                    x.IdTrainingCurriculum == this.trainingCurriculumVM.IdTrainingCurriculum) ?? this.trainingCurriculumVM;
            }
        }
        private async Task Submit()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.trainingCurriculumVM);
                this.editContext.EnableDataAnnotationsValidation();
                this.messageStore = new ValidationMessageStore(this.editContext);
                if (this.trainingCurriculumVM.IdCourse is not null)
                {
                    this.courseForValidation = await this.TrainingService.GetTrainingCourseWithoutAnythingIncludedByIdAsync(this.trainingCurriculumVM.IdCourse.Value);
                }

                if (this.trainingCurriculumVM.IdTrainingCurriculum != 0)
                {
                    this.trainingCurrForValidation = await this.TrainingService.GetTrainingCurriculumByIdAsync(this.trainingCurriculumVM.IdTrainingCurriculum);
                }

                this.editContext.OnValidationRequested += this.ValidateCourseEndDateWithSummaryHours;

                if (this.editContext.Validate())
                {
                    this.validationMessages.Clear();

                    ResultContext<TrainingCurriculumVM> inputContext = new ResultContext<TrainingCurriculumVM>();
                    this.trainingCurriculumVM.SelectedERUs = this.addedErus;
                    inputContext.ResultContextObject = this.trainingCurriculumVM;
                    inputContext.ResultContextObject.IdCandidateProviderSpeciality = this.idCandidateProviderSpeciality;
                    var addForConcurrentCheck = false;
                    if (this.trainingCurriculumVM.IdTrainingCurriculum == 0)
                    {
                        inputContext = await this.TrainingService.AddTrainingCurriculumAsync(inputContext);
                        addForConcurrentCheck = true;
                    }
                    else
                    {
                        inputContext = await this.TrainingService.UpdateTrainingCurriculumAsync(inputContext);
                    }

                    if (inputContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, inputContext.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, inputContext.ListMessages));
                        this.editContext.MarkAsUnmodified();
                        inputContext.ResultContextObject.SelectedERUs = this.addedErus;

                        this.CreationDateStr = inputContext.ResultContextObject.CreationDate.ToString("dd.MM.yyyy");
                        this.ModifyDateStr = inputContext.ResultContextObject.ModifyDate.ToString("dd.MM.yyyy");
                        this.trainingCurriculumVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(inputContext.ResultContextObject.IdModifyUser);
                        this.trainingCurriculumVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(inputContext.ResultContextObject.IdCreateUser);

                        if (addForConcurrentCheck)
                        {
                            await this.AddEntityIdAsCurrentlyOpened(this.trainingCurriculumVM.IdTrainingCurriculum, "TrainingProgramCurriculum");
                            this.IdTrainingProgramCurriculum = this.trainingCurriculumVM.IdTrainingCurriculum;
                        }
                    }

                    this.SetButtonsState();

                    await this.CallbackAfterSubmit.InvokeAsync(inputContext);
                }
                else
                {
                    this.validationMessages.Clear();
                    foreach (var msg in this.editContext.GetValidationMessages())
                    {
                        if (!this.validationMessages.Contains(msg))
                        {
                            this.validationMessages.Add(msg);
                        }
                    }

                    await UpdateCurriculumData();

                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task SubmitAndContinueBtn()
        {
            await this.Submit();

            if (!this.validationMessages.Any())
            {
                var idProgram = this.trainingCurriculumVM.IdProgram;
                var idCourse = this.trainingCurriculumVM.IdCourse;
                var trainingCurr = this.addedCurriculums.FirstOrDefault(x => x.IdTrainingCurriculum == this.trainingCurriculumVM.IdTrainingCurriculum);
                if (trainingCurr is not null)
                {
                    trainingCurr.SelectedERUs = this.trainingCurriculumVM.SelectedERUs.ToList();
                    trainingCurr.IdTrainingCurriculum = this.trainingCurriculumVM.IdTrainingCurriculum;
                }

                this.trainingCurriculumVM = new TrainingCurriculumVM()
                {
                    IdProgram = idProgram,
                    IdCourse = idCourse,
                    CreatePersonName = string.Empty,
                    ModifyPersonName = string.Empty,
                    IdProfessionalTraining = this.kvBType.IdKeyValue
                };

                this.CreationDateStr = string.Empty;
                this.ModifyDateStr = string.Empty;

                this.disableBtnsWhenEnteredFromEdit = false;

                this.nextId = this.addedCurriculums.Count;
                this.disableNextBtn = true;
                this.previousId = this.addedCurriculums.Count - 1;
                this.disablePreviousBtn = false;

                this.addedErus.Clear();

                this.StateHasChanged();
            }
        }

        private async Task DeleteEru(ERUVM eru)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.addedErus.Remove(eru);
                await this.erusGrid.Refresh();

                var trainingCurriculumERUVM = this.TrainingService.GetTrainingCurriculumERUByIdTrainingCurriculumAndIdERU(this.trainingCurriculumVM.IdTrainingCurriculum, eru.IdERU);
                if (trainingCurriculumERUVM is not null)
                {
                    var result = await this.TrainingService.DeleteTrainingCurriculumERUAsync(trainingCurriculumERUVM.IdTrainingCurriculumERU);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                        return;
                    }
                    else
                    {
                        this.trainingCurriculumVM.SelectedERUs = this.addedErus.ToList();
                        var resultContext = new ResultContext<TrainingCurriculumVM>();
                        resultContext.ResultContextObject = this.trainingCurriculumVM;
                        await this.CallbackAfterSubmit.InvokeAsync(resultContext);
                    }
                }

                await this.ShowSuccessAsync("Записът е изтрит успешно!");
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task OnAddERUClickHandler()
        {
            if (this.idEru == 0)
            {
                await this.ShowErrorAsync("Моля, изберете ЕРУ!");
            }
            else
            {
                ERUVM eruVM = new ERUVM() { IdERU = this.idEru };
                var eru = await this.DOCService.GetERUByIdAsync(eruVM);

                if (eru is not null)
                {
                    if (!this.addedErus.Any(x => x.IdERU == eru.IdERU))
                    {
                        this.addedErus.Add(eru);
                        this.erusGrid.Refresh();
                        this.idEru = 0;
                    }
                    else
                    {
                        await this.ShowErrorAsync("ЕРУ, с тази стойност, е вече добавено!");
                    }
                }
            }
        }

        private void OnProfessionalTrainingChangeHandler(ChangeEventArgs<int, KeyValueVM> args)
        {
            if (this.trainingCurriculumVM.IdProfessionalTraining != this.kvBType.IdKeyValue)
            {
                this.eruDataSourceForComboBox.Clear();
                this.eruDataSourceForComboBox = this.allErus.Where(x => x.IdProfessionalTraining == this.trainingCurriculumVM.IdProfessionalTraining).OrderBy(x => x.ERUIntCodeSplit).ToList();
            }
        }

        private async Task SetERUsData()
        {
            IEnumerable<ERUVM> docErus = new List<ERUVM>();

            if (this.doc is not null)
            {
                docErus = await this.DOCService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = doc.IdDOC });
            }

            IEnumerable<ERUSpecialityVM> eRUSpecialities = await this.ERUSpecialityService.GetAllERUsBySpecialityIdAsync(new ERUSpecialityVM() { IdSpeciality = this.speciality.IdSpeciality });
            List<int> eruIds = eRUSpecialities.Select(x => x.IdERU).ToList();
            IEnumerable<ERUVM> erus = await this.DOCService.GetERUsByIdsAsync(eruIds);
            this.allErus.AddRange(erus);
            docErus = docErus.Where(x => x.ERUSpecialities.Count == 0);
            this.allErus.AddRange(docErus);
            this.eruDataSourceForComboBox = this.allErus.Where(x => x.IdProfessionalTraining == this.trainingCurriculumVM.IdProfessionalTraining).OrderBy(x => x.ERUIntCodeSplit).ToList();
        }

        private async void NextCurriculum()
        {
            if (this.editContext.IsModified())
            {
                string msg = "Имате незапазени промени! Сигурни ли сте, че искате да смените темата?";
                bool confirmed = await this.ShowConfirmDialogAsync(msg);
                if (confirmed)
                {
                    this.editContext.MarkAsUnmodified();
                    this.nextId = this.addedCurriculums.IndexOf(this.trainingCurriculumVM) + 1;
                    if (this.nextId < this.addedCurriculums.Count)
                    {
                        this.RemoveEntityIdAsCurrentlyOpened(this.trainingCurriculumVM.IdTrainingCurriculum, "TrainingProgramCurriculum");

                        this.trainingCurriculumVM = this.addedCurriculums[this.nextId];

                        this.SetButtonsState();
                        await this.SetCreateAndModifyInfoAsync();
                        await this.SetERUSDataAsync();
                        await this.SetConcurrencyInfoAsync();
                    }
                }
            }
            else
            {
                this.nextId = this.addedCurriculums.IndexOf(this.trainingCurriculumVM) + 1;
                if (this.nextId < this.addedCurriculums.Count)
                {
                    this.RemoveEntityIdAsCurrentlyOpened(this.trainingCurriculumVM.IdTrainingCurriculum, "TrainingProgramCurriculum");

                    this.trainingCurriculumVM = this.addedCurriculums[this.nextId];

                    this.SetButtonsState();
                    await this.SetCreateAndModifyInfoAsync();
                    await this.SetERUSDataAsync();
                    await this.SetConcurrencyInfoAsync();
                }
            }

            this.GetERUsData();
        }

        private async Task SetConcurrencyInfoAsync()
        {
            var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(this.trainingCurriculumVM.IdTrainingCurriculum, "TrainingProgramCurriculum");
            if (concurrencyInfoValue == null)
            {
                this.hideBtnsConcurrentModal = false;
                await this.AddEntityIdAsCurrentlyOpened(this.trainingCurriculumVM.IdTrainingCurriculum, "TrainingProgramCurriculum");
            }
            else if (concurrencyInfoValue != null && concurrencyInfoValue.IdPerson != this.UserProps.IdPerson)
            {
                this.hideBtnsConcurrentModal = true;
                this.SetPersonFullNameAndVisibilityOfDialog(concurrencyInfoValue);
            }
        }

        private async Task SetERUSDataAsync()
        {
            var candidateCurriculumEruIds = this.trainingCurriculumVM.SelectedERUs.Select(x => x.IdERU).ToList();
            this.addedErus = (List<ERUVM>)await this.DOCService.GetERUsByIdsAsync(candidateCurriculumEruIds);

            this.StateHasChanged();
        }

        private async void PreviousCurriculum()
        {
            if (this.editContext.IsModified())
            {
                string msg = "Имате незапазени промени! Сигурни ли сте, че искате да смените темата?";
                bool confirmed = await this.ShowConfirmDialogAsync(msg);
                if (confirmed)
                {
                    this.editContext.MarkAsUnmodified();
                    this.previousId = this.addedCurriculums.IndexOf(this.trainingCurriculumVM) - 1;
                    if (this.previousId >= 0)
                    {
                        this.trainingCurriculumVM = this.addedCurriculums[this.previousId];

                        this.SetButtonsState();
                        await this.SetERUSDataAsync();
                        await this.SetCreateAndModifyInfoAsync();
                        await this.SetConcurrencyInfoAsync();
                    }
                }
            }
            else
            {
                this.previousId = this.addedCurriculums.IndexOf(this.trainingCurriculumVM) - 1;
                if (this.previousId <= -1)
                {
                    this.previousId = this.addedCurriculums.Count - 1;
                }

                if (this.previousId >= 0)
                {
                    this.trainingCurriculumVM = this.addedCurriculums[this.previousId];

                    this.SetButtonsState();
                    await this.SetERUSDataAsync();
                    await this.SetCreateAndModifyInfoAsync();
                    await this.SetConcurrencyInfoAsync();
                }
            }

            this.GetERUsData();
        }

        private void SetButtonsState()
        {
            this.nextId = this.addedCurriculums.IndexOf(this.trainingCurriculumVM) + 1;
            this.previousId = this.addedCurriculums.IndexOf(this.trainingCurriculumVM) - 1;

            if (this.nextId == this.addedCurriculums.Count)
            {
                this.disableNextBtn = true;
            }
            else
            {
                this.disableNextBtn = false;
            }

            if (this.previousId == -1)
            {
                this.disablePreviousBtn = true;
            }
            else
            {
                this.disablePreviousBtn = false;
            }
        }

        private async Task SetCreateAndModifyInfoAsync()
        {
            this.trainingCurriculumVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.trainingCurriculumVM.IdModifyUser);
            this.trainingCurriculumVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.trainingCurriculumVM.IdCreateUser);
            this.CreationDateStr = this.trainingCurriculumVM.CreationDate.ToString("dd.MM.yyyy");
            this.ModifyDateStr = this.trainingCurriculumVM.ModifyDate.ToString("dd.MM.yyyy");
        }

        private void ValidateCourseEndDateWithSummaryHours(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.trainingCurriculumVM.IdCourse is not null)
            {
                if (this.courseForValidation.StartDate.HasValue && this.courseForValidation.EndDate.HasValue)
                {
                    var courseHours = (this.courseForValidation.SelectableHours.HasValue ? this.courseForValidation.SelectableHours.Value : 0) + (this.courseForValidation.MandatoryHours.HasValue ? this.courseForValidation.MandatoryHours.Value : 0);
                    if (this.trainingCurriculumVM.IdTrainingCurriculum != 0)
                    {
                        if (this.trainingCurrForValidation.Theory != this.trainingCurriculumVM.Theory || this.trainingCurrForValidation.Practice != this.trainingCurriculumVM.Practice)
                        {
                            int hoursDifference = 0;
                            var vmTheory = this.trainingCurriculumVM.Theory.HasValue ? this.trainingCurriculumVM.Theory.Value : 0;
                            var vmPractice = this.trainingCurriculumVM.Practice.HasValue ? this.trainingCurriculumVM.Practice.Value : 0;
                            var entityTheory = this.trainingCurrForValidation.Theory.HasValue ? this.trainingCurrForValidation.Theory.Value : 0;
                            var entityPractice = this.trainingCurrForValidation.Practice.HasValue ? this.trainingCurrForValidation.Practice.Value : 0;

                            if (vmTheory > entityTheory)
                            {
                                hoursDifference += (int)(vmTheory - entityTheory);
                            }
                            else if (entityTheory > vmTheory)
                            {
                                hoursDifference -= (int)(entityTheory - vmTheory);
                            }

                            if (vmPractice > entityPractice)
                            {
                                hoursDifference += (int)(vmPractice - entityPractice);
                            }
                            else if (entityPractice > vmPractice)
                            {
                                hoursDifference -= (int)(entityPractice - vmPractice);
                            }

                            courseHours += hoursDifference;

                            var minimalDate = this.GetCourseMinimalEndDate(courseHours, this.courseForValidation);
                            if (minimalDate > this.courseForValidation.EndDate.Value.Date)
                            {
                                FieldIdentifier field = new FieldIdentifier(this.courseForValidation, "EndDate");
                                this.messageStore?.Add(field, $"За да промените учебните часове е необходимо преди това да удължите общата продължителност на курса! Дата на завършване следва да е след {minimalDate.ToString(GlobalConstants.DATE_FORMAT)} г.!");
                            }
                        }
                    }
                    else
                    {
                        var vmTheory = this.trainingCurriculumVM.Theory.HasValue ? this.trainingCurriculumVM.Theory.Value : 0;
                        var vmPractice = this.trainingCurriculumVM.Practice.HasValue ? this.trainingCurriculumVM.Practice.Value : 0;
                        var hoursDifference = (int)(vmTheory + vmPractice);
                        courseHours += hoursDifference;
                        var minimalDate = this.GetCourseMinimalEndDate(courseHours, this.courseForValidation);
                        if (minimalDate > this.courseForValidation.EndDate.Value.Date)
                        {
                            FieldIdentifier field = new FieldIdentifier(this.courseForValidation, "EndDate");
                            this.messageStore?.Add(field, $"За да промените учебните часове е необходимо преди това да удължите общата продължителност на курса! Дата на завършване следва да е след {minimalDate.ToString(GlobalConstants.DATE_FORMAT)} г.!");
                        }
                    }
                }
            }
        }

        private DateTime GetCourseMinimalEndDate(int courseHours, CourseVM course)
        {
            var workingDays = Math.Ceiling(courseHours / 8d);
            var additionalDaysFromWeek = (int)(workingDays / 6);
            var totalDaysForCourse = workingDays + additionalDaysFromWeek;
            var minimalDate = course.StartDate!.Value.Date.AddDays(totalDaysForCourse);
            return minimalDate;
        }
    }
}
