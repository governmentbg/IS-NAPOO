﻿using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CurrentCourseTrainingCurriculum : BlazorBaseComponent
    {
        private SfGrid<TrainingCurriculumVM> curriculumsGrid = new SfGrid<TrainingCurriculumVM>();
        private TrainingCurriculumModal trainingCurriculumModal = new TrainingCurriculumModal();
        private DOCERUModal docEruModal = new DOCERUModal();
        private CurrentCourseTrainingCurriculumUploadedFilesModal uploadedFilesModal = new CurrentCourseTrainingCurriculumUploadedFilesModal();
        private UploadCourseCurriculumFileModal uploadCourseCurriculumFileModal = new UploadCourseCurriculumFileModal();

        private List<TrainingCurriculumVM> addedCurriculums = new List<TrainingCurriculumVM>();
        private bool isCurriculumGridButtonClicked = false;
        private bool curriculumSelected = false;
        private TrainingCurriculumVM trainingCurriculumToDelete = new TrainingCurriculumVM();
        private List<TrainingCurriculumVM> selectedTrainingCurriculums = new List<TrainingCurriculumVM>();
        private double totalHours = 0;
        private double compulsoryHours = 0;
        private double nonCompulsoryHours = 0;
        private double generalProfessionTrainingHours = 0;
        private double industryProfessionTrainingHours = 0;
        private double specificProfessionTrainingHours = 0;
        private double extendedProfessionTrainingHours = 0;
        private double practiceHours = 0;
        private double theoryHours = 0;
        private double? totalPracticeHours = 0;
        private double? totalTheoryHours = 0;
        private IEnumerable<KeyValueVM> professionalTrainingsSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> minimumLevelEducationSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> trainingPeriodSource = new List<KeyValueVM>();
        private List<ERUVM> erusSource = new List<ERUVM>();
        private FrameworkProgramVM frameworkProgramVM = new FrameworkProgramVM();
        private CandidateProviderSpecialityVM candidateProviderSpecialityVM = new CandidateProviderSpecialityVM();
        private KeyValueVM kvBType = new KeyValueVM();
        private KeyValueVM kvCourseStatusCurrent = new KeyValueVM();

        [Parameter]
        public bool HideBtnsWhenSPK { get; set; }

        [Parameter]
        public bool IsEditable { get; set; } = true;

        [Parameter]
        public CourseVM CourseVM { get; set; }

        [Parameter]
        public EventCallback CallbackAfterCurriculumModalSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDOCService DocService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IFrameworkProgramService FrameworkProgramService { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.kvCourseStatusCurrent = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusNow");

            if (this.CourseVM.IdProgram != 0)
            {
                await this.LoadCurriculumDataAsync();
            }
        }

        public async Task LoadCurriculumDataAsync()
        {
            await this.LoadFrameworkAndSpecialityDataAsync();

            this.professionalTrainingsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            this.kvBType = this.professionalTrainingsSource.FirstOrDefault(x => x.KeyValueIntCode == "B");
            this.minimumLevelEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MinimumLevelEducation");
            this.trainingPeriodSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingPeriod");
            this.erusSource = this.DataSourceService.GetAllERUsList();
            this.addedCurriculums = (await this.TrainingService.GetTrainingCurriculumByIdCourseAsync(this.CourseVM.IdCourse)).ToList();
            foreach (var trainingCurriculum in this.addedCurriculums)
            {
                var value = this.professionalTrainingsSource.FirstOrDefault(x => x.IdKeyValue == trainingCurriculum.IdProfessionalTraining).DefaultValue1;
                trainingCurriculum.ProfessionalTraining = value;

                var erus = this.erusSource.Where(x => trainingCurriculum.TrainingCurriculumERUs.Select(x => x.IdERU).ToList().Contains(x.IdERU));
                trainingCurriculum.SelectedERUs.AddRange(erus);
            }

            this.frameworkProgramVM = this.CourseVM.Program.FrameworkProgram;
            if (this.frameworkProgramVM is not null)
            {
                var minimumLevelEducationValue = this.minimumLevelEducationSource.FirstOrDefault(x => x.IdKeyValue == this.frameworkProgramVM.IdMinimumLevelEducation);
                if (minimumLevelEducationValue is not null)
                {
                    this.frameworkProgramVM.MinimumLevelEducationName = minimumLevelEducationValue.Name;
                }

                var trainingPeriodValue = this.trainingPeriodSource.FirstOrDefault(x => x.IdKeyValue == this.frameworkProgramVM.IdTrainingPeriod);
                if (trainingPeriodValue is not null)
                {
                    this.frameworkProgramVM.TrainingPeriodName = trainingPeriodValue.Name;
                }
            }
            else
            {
                this.frameworkProgramVM = new FrameworkProgramVM();
            }

            this.candidateProviderSpecialityVM = await this.TrainingService.GetCandidateProviderSpecialityByIdCandidateProviderAndIdSpecialityAsync(this.CourseVM.Program.IdCandidateProvider, this.CourseVM.Program.IdSpeciality);

            await this.curriculumsGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task LoadFrameworkAndSpecialityDataAsync()
        {
            if (this.CourseVM.Program.FrameworkProgram is null && this.CourseVM.Program.IdFrameworkProgram.HasValue)
            {
                this.CourseVM.Program.FrameworkProgram = await this.FrameworkProgramService.GetFrameworkProgramByIdAsync(this.CourseVM.Program.IdFrameworkProgram.Value);
            }

            if (this.CourseVM.Program.Speciality is null && this.CourseVM.Program.IdSpeciality != 0)
            {
                this.CourseVM.Program.Speciality = await this.SpecialityService.GetSpecialityWithProfessionIncludedByIdSpecialityAsync(this.CourseVM.Program.IdSpeciality);
            }
        }

        private async Task EditSchedule(TrainingCurriculumVM trainingCurriculum)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;

                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(trainingCurriculum.IdTrainingCurriculum, "TrainingProgramCurriculum");
                if (concurrencyInfoValue == null)
                {
                    await this.AddEntityIdAsCurrentlyOpened(trainingCurriculum.IdTrainingCurriculum, "TrainingProgramCurriculum");
                }

                var type = this.HideBtnsWhenSPK ? "SPK" : "PP";
                await this.trainingCurriculumModal.OpenModal(trainingCurriculum, this.addedCurriculums, this.candidateProviderSpecialityVM.IdCandidateProviderSpeciality, this.CourseVM.Program.Speciality, type);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenCandidateCurriculumModal()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var type = this.HideBtnsWhenSPK ? "SPK" : "PP";
                await this.trainingCurriculumModal.OpenModal(new TrainingCurriculumVM() { IdProgram = this.CourseVM.IdProgram, IdCourse = this.CourseVM.IdCourse }, this.addedCurriculums, this.candidateProviderSpecialityVM.IdCandidateProviderSpeciality, this.CourseVM.Program.Speciality, type);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteSelectedCurriculumsBtn()
        {
            var selectedCurriculums = await this.curriculumsGrid.GetSelectedRecordsAsync();
            if (!selectedCurriculums.Any())
            {
                await this.ShowErrorAsync("Моля, изберете поне един ред от учебната програма за изтриване!");
                return;
            }

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избраните записи?");
            if (isConfirmed)
            {
                var result = await this.TrainingService.DeleteListCandidateCurriculumAsync(selectedCurriculums);
                if (!result.HasErrorMessages)
                {
                    foreach (var curriculum in selectedCurriculums)
                    {
                        this.addedCurriculums.RemoveAll(x => x.IdTrainingCurriculum == curriculum.IdTrainingCurriculum);

                        await this.curriculumsGrid.DeleteRecordAsync("IdTrainingCurriculum", curriculum);
                    }

                    await this.curriculumsGrid.Refresh();
                    await this.curriculumsGrid.CallStateHasChangedAsync();

                    await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                    await this.UpdateProgramHoursAfterCurriculumSubmit();
                }
                else
                {
                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                }
            }

        }

        private async Task DeleteCurriculum(TrainingCurriculumVM trainingCurriculum)
        {
            this.trainingCurriculumToDelete = trainingCurriculum;

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (isConfirmed)
            {
                var result = await this.TrainingService.DeleteTrainingCurriculumAsync(this.trainingCurriculumToDelete.IdTrainingCurriculum);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    return;
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                    this.addedCurriculums.RemoveAll(x => x.IdTrainingCurriculum == this.trainingCurriculumToDelete.IdTrainingCurriculum);

                    await this.curriculumsGrid.DeleteRecordAsync("IdTrainingCurriculum", this.trainingCurriculumToDelete);

                    await this.curriculumsGrid.Refresh();

                    await this.curriculumsGrid.CallStateHasChangedAsync();

                    await this.UpdateProgramHoursAfterCurriculumSubmit();
                    await this.UpdateSubjectGradeTableAsync();
                }
            }
        }

        private async Task OpenDOCERUModal()
        {
            if (this.selectedTrainingCurriculums.Count == 1)
            {
                var curriculum = this.selectedTrainingCurriculums.FirstOrDefault();
                if (curriculum!.ProfessionalTraining == "Б")
                {
                    await this.ShowErrorAsync("Не можете да въведете съответствие с ДОС за тема с вид професионална подтготовка от раздел 'Б'!");
                    return;
                }
            }

            if (this.CourseVM.Program.Speciality.IdDOC is null)
            {
                await this.ShowErrorAsync("Не можете да въведете съответствие с ДОС, защото за избраната специалност няма въведен ДОС в системата!");
                return;
            }

            if (!this.curriculumSelected)
            {
                await this.ShowErrorAsync("Моля, изберете тема/теми от учебната програма!");
            }
            else
            {
                foreach (var selectedCurriculum in this.selectedTrainingCurriculums)
                {
                    if (this.selectedTrainingCurriculums.Any(x => x.IdProfessionalTraining != selectedCurriculum.IdProfessionalTraining))
                    {
                        await this.ShowErrorAsync("Моля, изберете теми от учебната програма от един същи вид професионална подготовка!");

                        return;
                    }
                }

                await this.docEruModal.OpenModal(this.CourseVM.Program.Speciality, null, this.selectedTrainingCurriculums);
            }
        }

        private async Task ValidateCurriculum()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.SpinnerShow();

                if (!this.addedCurriculums.Any())
                {
                    await this.ShowErrorAsync("Моля, добавете учебен план и учебни програми!");
                }
                else
                {
                    if (this.HideBtnsWhenSPK)
                    {
                        CandidateCurriculumExcelVM candidateCurriculumExcelVM = new CandidateCurriculumExcelVM();
                        var professionalTrainingId = (await this.DataSourceService.GetKeyValueByIntCodeAsync("ProfessionalTraining", "B")).IdKeyValue;
                        IEnumerable<ERUVM> erusFromDoc = new List<ERUVM>();

                        if (this.CourseVM.Program.Speciality.IdDOC != null)
                        {
                            erusFromDoc = await this.DocService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = this.CourseVM.Program.Speciality.IdDOC ?? default });
                        }

                        var erus = new HashSet<ERUVM>();
                        if (erusFromDoc.Any())
                        {
                            foreach (var curriculum in this.addedCurriculums)
                            {
                                foreach (var eru in curriculum.SelectedERUs)
                                {
                                    if (!erus.Any(x => x.IdERU == eru.IdERU))
                                    {
                                        erus.Add(eru);
                                    }
                                }
                            }
                        }

                        if (erus.Count != erusFromDoc.Count())
                        {
                            var missingErus = erusFromDoc.Where(x => erus.All(y => y.IdERU != x.IdERU)).ToList();

                            foreach (var missingEru in missingErus)
                            {
                                candidateCurriculumExcelVM.MissingDOCERUs.Add($"Единицата резултат от учене (ЕРУ) от ДОС не е добавена към нито една тема!->ЕРУ: {missingEru.Code}");
                            }
                        }

                        foreach (var curriculum in this.addedCurriculums)
                        {
                            if (curriculum.IdProfessionalTraining != professionalTrainingId)
                            {
                                if (!curriculum.SelectedERUs.Any())
                                {
                                    candidateCurriculumExcelVM.MissingTopicERUs.Add($"Няма добавена Единица резултат от учене (ЕРУ) от ДОС към темата!->Тема: {curriculum.Topic}");
                                }
                            }
                        }

                        if (this.frameworkProgramVM.SectionА > this.compulsoryHours)
                        {
                            candidateCurriculumExcelVM.MinimumCompulsoryHoursNotReached = true;
                        }

                        if (this.frameworkProgramVM.SectionB > this.nonCompulsoryHours)
                        {
                            candidateCurriculumExcelVM.MinimumChoosableHoursNotReached = true;
                        }

                        var percentCompulsoryHours = (this.generalProfessionTrainingHours / this.compulsoryHours) * 100;
                        if (this.frameworkProgramVM.SectionА1 < percentCompulsoryHours)
                        {
                            candidateCurriculumExcelVM.MaximumPercentNotReached = true;
                        }

                        var percentSpecificTraining = (this.practiceHours / (this.industryProfessionTrainingHours + this.specificProfessionTrainingHours)) * 100;
                        if (this.frameworkProgramVM.Practice > percentSpecificTraining)
                        {
                            candidateCurriculumExcelVM.MinimumPercentNotReached = true;
                        }

                        if (candidateCurriculumExcelVM.MinimumPercentNotReached
                            || candidateCurriculumExcelVM.MaximumPercentNotReached
                            || candidateCurriculumExcelVM.MinimumChoosableHoursNotReached
                            || candidateCurriculumExcelVM.MinimumCompulsoryHoursNotReached
                            || candidateCurriculumExcelVM.MissingDOCERUs.Any()
                            || candidateCurriculumExcelVM.MissingTopicERUs.Any())
                        {
                            var resultObject = new ResultContext<CandidateCurriculumExcelVM>();
                            resultObject.ResultContextObject = candidateCurriculumExcelVM;
                            var result = this.CandidateProviderService.CreateExcelCurriculumValidationErrors(resultObject, this.compulsoryHours, this.nonCompulsoryHours, percentCompulsoryHours, percentSpecificTraining);
                            await this.JsRuntime.SaveAs($"Errors_Curriculum_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", result.ToArray());

                            await this.ShowErrorAsync("Избраната учебна програма не отговаря на заложените минимални изисквания! Моля, отстранете грешките във файла! Валидирането е неуспешно!");
                        }
                        else
                        {
                            await this.ShowSuccessAsync("Избраната учебна програма отговаря на минималните изисквания и е валидирана успешно!");
                        }
                    }
                    else
                    {
                        CandidateCurriculumExcelVM candidateCurriculumExcelVM = new CandidateCurriculumExcelVM();
                        var professionalTrainingId = (await this.DataSourceService.GetKeyValueByIntCodeAsync("ProfessionalTraining", "B")).IdKeyValue;
                        foreach (var curriculum in this.addedCurriculums)
                        {
                            if (curriculum.IdProfessionalTraining != professionalTrainingId)
                            {
                                if (!curriculum.SelectedERUs.Any())
                                {
                                    candidateCurriculumExcelVM.MissingTopicERUs.Add($"Няма добавена Единица резултат от учене (ЕРУ) от ДОС към темата!->Тема: {curriculum.Topic}");
                                }
                            }
                        }

                        var percentCompulsoryHours = (this.generalProfessionTrainingHours / this.compulsoryHours) * 100;
                        if (this.frameworkProgramVM.SectionА1 < percentCompulsoryHours)
                        {
                            candidateCurriculumExcelVM.MaximumPercentNotReached = true;
                        }

                        var percentSpecificTraining = (this.practiceHours / (this.industryProfessionTrainingHours + this.specificProfessionTrainingHours)) * 100;
                        if (this.frameworkProgramVM.Practice > percentSpecificTraining)
                        {
                            candidateCurriculumExcelVM.MinimumPercentNotReached = true;
                        }

                        if (candidateCurriculumExcelVM.MinimumPercentNotReached
                            || candidateCurriculumExcelVM.MaximumPercentNotReached
                            || candidateCurriculumExcelVM.MissingTopicERUs.Any())
                        {
                            var resultObject = new ResultContext<CandidateCurriculumExcelVM>();
                            resultObject.ResultContextObject = candidateCurriculumExcelVM;
                            var result = this.CandidateProviderService.CreateExcelCurriculumValidationErrors(resultObject, this.compulsoryHours, this.nonCompulsoryHours, percentCompulsoryHours, percentSpecificTraining);
                            await this.JsRuntime.SaveAs($"Errors_Curriculum_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", result.ToArray());

                            await this.ShowErrorAsync("Избраната учебна програма не отговаря на заложените минимални изисквания! Моля, отстранете грешките във файла! Валидирането е неуспешно!");
                        }
                        else
                        {
                            await this.ShowSuccessAsync("Избраната учебна програма отговаря на минималните изисквания и е валидирана успешно!");
                        }
                    }
                }
            }
            finally
            {
                this.loading = false;
                this.SpinnerHide();
            }
        }

        private async Task PrintCurriculum()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.SpinnerShow();

                if (this.addedCurriculums.Any())
                {
                    this.CourseVM.Program.Speciality.IdFormEducation = this.CourseVM.IdFormEducation;
                    var isCurriculumInvalid = await this.IsCurriculumInvalidAsync();
                    var result = await this.CandidateProviderService.PrintCurriculumAsync(this.frameworkProgramVM, this.CourseVM.Program.Speciality,
                        this.totalHours, this.compulsoryHours, this.nonCompulsoryHours, this.CourseVM.CandidateProvider.ProviderOwner, isCurriculumInvalid, this.candidateProviderSpecialityVM, this.CourseVM.CandidateProvider, null, this.addedCurriculums, false);
                    await FileUtils.SaveAs(this.JsRuntime, "Ucheben-plan-CPO.docx", result.ToArray());
                }
                else
                {
                    await this.ShowErrorAsync("Моля, добавете учебен план и учебни програми!");
                }
            }
            finally
            {
                this.loading = false;
                this.SpinnerHide();
            }
        }

        private async Task<bool> IsCurriculumInvalidAsync()
        {
            CandidateCurriculumExcelVM candidateCurriculumExcelVM = new CandidateCurriculumExcelVM();
            var professionalTrainingId = (await this.DataSourceService.GetKeyValueByIntCodeAsync("ProfessionalTraining", "B")).IdKeyValue;
            IEnumerable<ERUVM> erusFromDoc = new List<ERUVM>();

            if (this.CourseVM.Program.Speciality.IdDOC != null)
            {
                erusFromDoc = await this.DocService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = this.CourseVM.Program.Speciality.IdDOC ?? default });
            }

            var erus = new HashSet<ERUVM>();
            if (erusFromDoc.Any())
            {
                foreach (var curriculum in this.addedCurriculums)
                {
                    foreach (var eru in curriculum.SelectedERUs)
                    {
                        if (!erus.Any(x => x.IdERU == eru.IdERU))
                        {
                            erus.Add(eru);
                        }
                    }
                }
            }

            if (erus.Count != erusFromDoc.Count())
            {
                var missingErus = erusFromDoc.Where(x => erus.All(y => y.IdERU != x.IdERU)).ToList();

                foreach (var missingEru in missingErus)
                {
                    candidateCurriculumExcelVM.MissingDOCERUs.Add($"Единицата резултат от учене (ЕРУ) от ДОС не е добавена към нито една тема!->ЕРУ: {missingEru.Code}");
                }
            }

            foreach (var curriculum in this.addedCurriculums)
            {
                if (curriculum.IdProfessionalTraining != professionalTrainingId)
                {
                    if (!curriculum.SelectedERUs.Any())
                    {
                        candidateCurriculumExcelVM.MissingTopicERUs.Add($"Няма добавена Единица резултат от учене (ЕРУ) от ДОС към темата!->Тема: {curriculum.Topic}");
                    }
                }
            }

            if (this.frameworkProgramVM.SectionА > this.compulsoryHours)
            {
                candidateCurriculumExcelVM.MinimumCompulsoryHoursNotReached = true;
            }

            if (this.frameworkProgramVM.SectionB > this.nonCompulsoryHours)
            {
                candidateCurriculumExcelVM.MinimumChoosableHoursNotReached = true;
            }

            var percentCompulsoryHours = (this.generalProfessionTrainingHours / this.compulsoryHours) * 100;
            if (this.frameworkProgramVM.SectionА1 < percentCompulsoryHours)
            {
                candidateCurriculumExcelVM.MaximumPercentNotReached = true;
            }

            var percentSpecificTraining = (this.practiceHours / (this.industryProfessionTrainingHours + this.specificProfessionTrainingHours)) * 100;
            if (this.frameworkProgramVM.Practice > percentSpecificTraining)
            {
                candidateCurriculumExcelVM.MinimumPercentNotReached = true;
            }

            return candidateCurriculumExcelVM.MinimumPercentNotReached
                || candidateCurriculumExcelVM.MaximumPercentNotReached
                || candidateCurriculumExcelVM.MinimumChoosableHoursNotReached
                || candidateCurriculumExcelVM.MinimumCompulsoryHoursNotReached
                || candidateCurriculumExcelVM.MissingDOCERUs.Any()
                || candidateCurriculumExcelVM.MissingTopicERUs.Any();
        }

        private async void UpdateAfterSubmitTrainingCurriculumModal(ResultContext<TrainingCurriculumVM> resultContext)
        {
            if (resultContext.ListMessages.Any())
            {
                this.addedCurriculums.RemoveAll(x => x.IdTrainingCurriculum == resultContext.ResultContextObject.IdTrainingCurriculum);
                var value = this.professionalTrainingsSource.FirstOrDefault(x => x.IdKeyValue == resultContext.ResultContextObject.IdProfessionalTraining).DefaultValue1;
                resultContext.ResultContextObject.ProfessionalTraining = value;

                this.addedCurriculums.Add(resultContext.ResultContextObject);

                this.addedCurriculums = this.addedCurriculums.OrderBy(x => x.ProfessionalTraining).ThenBy(x => x.IdTrainingCurriculum).ToList();

                this.StateHasChanged();

                await this.UpdateProgramHoursAfterCurriculumSubmit();

                await this.UpdateSubjectGradeTableAsync();

                await this.CallbackAfterCurriculumModalSubmit.InvokeAsync();
            }
        }

        private async Task UpdateSubjectGradeTableAsync()
        {
            //await this.TrainingService.UpdateSubjectGradeAfterUpdateTrainingCurriculumAsync(this.CourseVM.IdCourse);
        }

        private async Task UpdateProgramHoursAfterCurriculumSubmit()
        {
            var kvBType = this.professionalTrainingsSource.FirstOrDefault(x => x.KeyValueIntCode == "B").IdKeyValue;

            this.CourseVM.SelectableHours = 0;
            this.CourseVM.MandatoryHours = 0;
            foreach (var curriculum in this.addedCurriculums)
            {
                if (curriculum.IdProfessionalTraining == kvBType)
                {
                    this.CourseVM.SelectableHours += (int)((curriculum.Theory.HasValue ? curriculum.Theory.Value : 0) + (curriculum.Practice.HasValue ? curriculum.Practice.Value : 0));
                }
                else
                {
                    this.CourseVM.MandatoryHours += (int)((curriculum.Theory.HasValue ? curriculum.Theory.Value : 0) + (curriculum.Practice.HasValue ? curriculum.Practice.Value : 0));
                }
            }

            await this.TrainingService.UpdateTrainingCourseHoursByIdCourseAsync(this.CourseVM.IdCourse, this.CourseVM.MandatoryHours.Value, this.CourseVM.SelectableHours.Value);

            await this.CallbackAfterCurriculumModalSubmit.InvokeAsync();
        }

        private async Task AfterSelectedERUHandler(List<ERUVM> erus)
        {
            foreach (var selectedCurriculum in this.selectedTrainingCurriculums)
            {
                var curriculum = this.addedCurriculums.FirstOrDefault(x => x.Subject == selectedCurriculum.Subject && x.IdProfessionalTraining == selectedCurriculum.IdProfessionalTraining && x.Topic == selectedCurriculum.Topic);

                foreach (var eru in erus)
                {
                    if (!curriculum.SelectedERUs.Any(x => x.Code == eru.Code))
                    {
                        curriculum.SelectedERUs.Add(eru);
                    }
                }
            }

            await this.curriculumsGrid.Refresh();
            this.StateHasChanged();
        }

        // проверява дали е натиснат бутон от учебна програма грида, за да не се маркира целият ред
        private void CurriculumRecordClickHandler(RecordClickEventArgs<TrainingCurriculumVM> args)
        {
            if (args.Column.HeaderText == " " || args.Column.HeaderText == "Съответствие с ЕРУ от ДОС")
            {
                this.isCurriculumGridButtonClicked = true;
            }
            else
            {
                this.isCurriculumGridButtonClicked = false;
            }
        }

        private async Task CurriculumDeselectedHandler(RowDeselectEventArgs<TrainingCurriculumVM> args)
        {
            if (this.isCurriculumGridButtonClicked)
            {
                args.Cancel = true;
            }
            else
            {
                this.selectedTrainingCurriculums.Clear();
                this.selectedTrainingCurriculums = await this.curriculumsGrid.GetSelectedRecordsAsync();
                if (!this.selectedTrainingCurriculums.Any())
                {
                    this.curriculumSelected = false;
                }
            }
        }

        private async Task CurriculumSelectedHandler(RowSelectEventArgs<TrainingCurriculumVM> args)
        {
            if (this.isCurriculumGridButtonClicked)
            {
                args.Cancel = true;
            }
            else
            {
                this.curriculumSelected = true;
                this.selectedTrainingCurriculums.Clear();
                this.selectedTrainingCurriculums = await this.curriculumsGrid.GetSelectedRecordsAsync();
            }
        }

        // превентва селектирането на ред, ако е натистнат бутон от учебна програма грида
        private void CurriculumSelectingHandler(RowSelectingEventArgs<TrainingCurriculumVM> args)
        {
            if (this.isCurriculumGridButtonClicked)
            {
                args.Cancel = true;
            }
        }

        // превентва деселектирането на ред, ако е натистнат бутон от учебна програма грида
        private void CurriculumDeselectingHandler(RowDeselectEventArgs<TrainingCurriculumVM> args)
        {
            if (this.isCurriculumGridButtonClicked)
            {
                args.Cancel = true;
            }
        }

        // пресмята общият брой часове за учебна програма
        private void CalculateCurriculumHours()
        {
            this.ResetHours();

            foreach (var curriculum in this.addedCurriculums)
            {
                if (curriculum.Theory.HasValue)
                {
                    this.theoryHours = curriculum.Theory.Value;
                    this.totalTheoryHours += curriculum.Theory.Value;
                }
                else
                {
                    this.theoryHours = 0;
                }

                if (curriculum.Practice.HasValue)
                {
                    this.totalPracticeHours += curriculum.Practice.Value;
                }

                if (curriculum.ProfessionalTraining != "Б")
                {
                    if (curriculum.Practice.HasValue)
                    {
                        this.practiceHours += curriculum.Practice.Value;
                    }
                    else
                    {
                        this.practiceHours += 0;
                    }
                }

                if (curriculum.ProfessionalTraining == "Б")
                {
                    var bPracticeHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                    var bTheoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                    this.extendedProfessionTrainingHours += (bPracticeHours + bTheoryHours);
                }

                if (curriculum.ProfessionalTraining == "А1")
                {
                    var a1PracticeHours = curriculum.Practice.HasValue ? curriculum.Practice.Value : 0;
                    var a2TheoryHours = curriculum.Theory.HasValue ? curriculum.Theory.Value : 0;
                    this.generalProfessionTrainingHours += (a1PracticeHours + a2TheoryHours);
                }

                if (curriculum.ProfessionalTraining == "А2")
                {
                    double a2PracticeHours = 0;
                    if (curriculum.Practice.HasValue)
                    {
                        a2PracticeHours = curriculum.Practice.Value;
                    }
                    else
                    {
                        a2PracticeHours = 0;
                    }

                    this.industryProfessionTrainingHours += (a2PracticeHours + this.theoryHours);
                }

                if (curriculum.ProfessionalTraining == "А3")
                {
                    double a3PracticeHours = 0;
                    if (curriculum.Practice.HasValue)
                    {
                        a3PracticeHours = curriculum.Practice.Value;
                    }
                    else
                    {
                        a3PracticeHours = 0;
                    }

                    this.specificProfessionTrainingHours += (a3PracticeHours + this.theoryHours);
                }
            }

            this.totalHours += this.extendedProfessionTrainingHours + this.generalProfessionTrainingHours + industryProfessionTrainingHours + specificProfessionTrainingHours;
            this.nonCompulsoryHours = this.extendedProfessionTrainingHours;
            this.compulsoryHours = this.totalHours - this.nonCompulsoryHours;
        }

        // ресетва бройката с часовете от учебната програма
        private void ResetHours()
        {
            this.totalHours = 0;
            this.compulsoryHours = 0;
            this.nonCompulsoryHours = 0;
            this.generalProfessionTrainingHours = 0;
            this.industryProfessionTrainingHours = 0;
            this.specificProfessionTrainingHours = 0;
            this.extendedProfessionTrainingHours = 0;
            this.practiceHours = 0;
            this.theoryHours = 0;
            this.totalTheoryHours = 0;
            this.totalPracticeHours = 0;
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                ExportProperties.IncludeTemplateColumn = true;

                PdfTheme Theme = new PdfTheme();
                PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Regular, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };

                PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Bold, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };
                Theme.Record = RecordThemeStyle;
                Theme.Header = HeaderThemeStyle;

                ExportProperties.Theme = Theme;
                ExportProperties.FileName = $"Curriculum_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.curriculumsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Curriculum_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.Columns = this.SetGridColumnsForExport();
                ExportProperties.IncludeTemplateColumn = true;
                await this.curriculumsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "ProfessionalTraining", HeaderText = "Раздел", TextAlign = TextAlign.Center });
            ExportColumns.Add(new GridColumn() { Field = "Subject", HeaderText = "Предмет", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Topic", HeaderText = "Тема", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Theory", HeaderText = "Теория", TextAlign = TextAlign.Center });
            ExportColumns.Add(new GridColumn() { Field = "Practice", HeaderText = "Практика", TextAlign = TextAlign.Center });
            ExportColumns.Add(new GridColumn() { Field = "ERUsForExport", HeaderText = "ЕРУ", TextAlign = TextAlign.Center, });

            return ExportColumns;
        }

        private void CustomizeCellHours(QueryCellInfoEventArgs<TrainingCurriculumVM> args)
        {
            if (args.Column.Field == "Theory")
            {
                args.Cell.AddClass(new string[] { "cell-orange" });
            }

            if (args.Column.Field == "Practice")
            {
                args.Cell.AddClass(new string[] { "cell-bluegreen" });
            }
        }

        private async Task OpenCurriculumFilesModalBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.uploadedFilesModal.OpenModal(this.CourseVM);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenUploadCurriculumUploadedFileModalBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!string.IsNullOrEmpty(this.CourseVM.UploadedFileName))
                {
                    this.SpinnerHide();

                    bool confirmed = await this.ShowConfirmDialogAsync("Вече има прикачен файл с учебен план и учебна програма. В случай, че желаете да прикачите нов, старият ще бъде презаписан. Сигурни ли сте, че искате да продължите?");
                    if (confirmed)
                    {
                        this.SpinnerShow();

                        await this.DeleteUploadedCurriculumFileAsync();

                        await this.uploadCourseCurriculumFileModal.OpenModal(this.CourseVM.IdCourse, this.CourseVM.CourseName, true);
                    }
                }
                else
                {
                    await this.uploadCourseCurriculumFileModal.OpenModal(this.CourseVM.IdCourse, this.CourseVM.CourseName, true);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void UpdateCourseUploadedFileNameAfterUploadedFile(string uploadedFileName)
        {
            this.CourseVM.UploadedFileName = uploadedFileName;
        }

        private async Task DeleteUploadedCurriculumFileAsync()
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var filePath = settingResource + "\\" + this.CourseVM.UploadedFileName;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);

                this.CourseVM.UploadedFileName = string.Empty;
                await this.TrainingService.UpdateCourseFileNameAsync(this.CourseVM.IdCourse);
            }
        }
    }
}
