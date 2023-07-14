using ISNAPOO.Common.Constants;
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
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class ValidationCurriculumModal : BlazorBaseComponent
    {
        private SfGrid<ValidationCurriculumVM> curriculumsGrid = new SfGrid<ValidationCurriculumVM>();
        private ValidationCurriculum validationCurriculumModal = new ValidationCurriculum();
        private DocValidationERUModal docEruModal = new DocValidationERUModal();
        private ImportCandidateValidationCurriculumModal importCandidateCurriculumModal = new ImportCandidateValidationCurriculumModal();
        private CurrentCourseTrainingCurriculumUploadedFilesModal uploadedFilesModal = new CurrentCourseTrainingCurriculumUploadedFilesModal();
        private UploadCourseCurriculumFileModal uploadCourseCurriculumFileModal = new UploadCourseCurriculumFileModal();
        private List<ValidationCurriculumVM> addedCurriculums = new List<ValidationCurriculumVM>();       
        private bool isCurriculumGridButtonClicked = false;
        private bool curriculumSelected = false;
        private ValidationCurriculumVM validationCurriculumToDelete = new ValidationCurriculumVM();
        private List<ValidationCurriculumVM> selectedValidationCurriculums = new List<ValidationCurriculumVM>();
        private double totalHours = 0;
        private double compulsoryHours = 0;
        private double nonCompulsoryHours = 0;
        private double generalProfessionTrainingHours = 0;
        private double industryProfessionTrainingHours = 0;
        private double specificProfessionTrainingHours = 0;
        private double extendedProfessionTrainingHours = 0;
        private double practiceHours = 0;
        private double theoryHours = 0;
        private IEnumerable<KeyValueVM> professionalTrainingsSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> minimumLevelEducationSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> trainingPeriodSource = new List<KeyValueVM>();
        private KeyValueVM kvBType = new KeyValueVM();
        private List<ERUVM> erusSource = new List<ERUVM>();
        private FrameworkProgramVM frameworkProgramVM = new FrameworkProgramVM();
        private CandidateProviderSpecialityVM candidateProviderSpecialityVM = new CandidateProviderSpecialityVM(); 
        private double? totalPracticeHours = 0;
        private double? totalTheoryHours = 0;
        private KeyValueVM kvCourseStatusCurrent = new KeyValueVM();

        [Parameter]
        public ValidationClientVM validationClientVM { get; set; }

        [Parameter]
        public EventCallback CallbackAfterCurriculumModalSubmit { get; set; }

        [Parameter]
        public bool HideBtnsWhenSPK { get; set; }

        [Parameter]
        public bool IsEditable { get; set; } = true;

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

        [Inject]
        public IUploadFileService UploadService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.kvCourseStatusCurrent = await this.DataSourceService.GetKeyValueByIntCodeAsync("CourseStatus", "CourseStatusNow");

            if (this.validationClientVM.IdValidationClient != 0)
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
            this.addedCurriculums = (await this.TrainingService.GetValidationCurriculumByIdValidationClientAsync(this.validationClientVM.IdValidationClient)).ToList();
            foreach (var validationCurriculum in this.addedCurriculums)
            {
                var value = this.professionalTrainingsSource.FirstOrDefault(x => x.IdKeyValue == validationCurriculum.IdProfessionalTraining).DefaultValue1;
                validationCurriculum.ProfessionalTraining = value;

                var erus = this.erusSource.Where(x => validationCurriculum.ValidationCurriculumERUs.Select(x => x.IdERU).ToList().Contains(x.IdERU)).ToList();
                validationCurriculum.SelectedERUs.AddRange(erus);
            }

            if (this.validationClientVM.FrameworkProgram is not null)
            {
                this.frameworkProgramVM = this.validationClientVM.FrameworkProgram;

                if (this.frameworkProgramVM.IdMinimumLevelEducation != 0)
                {
                    var lvEdu = this.minimumLevelEducationSource.FirstOrDefault(x => x.IdKeyValue == this.frameworkProgramVM.IdMinimumLevelEducation);
                    if (lvEdu != null)
                        this.frameworkProgramVM.MinimumLevelEducationName = lvEdu.Name;
                }
                
                if (this.frameworkProgramVM.IdTrainingPeriod != 0)
                {
                    this.frameworkProgramVM.TrainingPeriodName = this.trainingPeriodSource.FirstOrDefault(x => x.IdKeyValue == this.frameworkProgramVM.IdTrainingPeriod).Name;
                }
            }

            this.candidateProviderSpecialityVM = await this.TrainingService.GetCandidateProviderSpecialityByIdCandidateProviderAndIdSpecialityAsync(this.validationClientVM.IdCandidateProvider, this.validationClientVM.IdSpeciality.Value);

            await this.curriculumsGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task LoadFrameworkAndSpecialityDataAsync()
        {
            if (this.validationClientVM.FrameworkProgram is null && this.validationClientVM.IdFrameworkProgram.HasValue)
            {
                this.validationClientVM.FrameworkProgram = await this.FrameworkProgramService.GetFrameworkProgramByIdAsync(this.validationClientVM.IdFrameworkProgram.Value);
            }

            if (this.validationClientVM.Speciality is null && this.validationClientVM.IdSpeciality.HasValue)
            {
                this.validationClientVM.Speciality = await this.SpecialityService.GetSpecialityWithProfessionIncludedByIdSpecialityAsync(this.validationClientVM.IdSpeciality.Value);
            }
        }

        private async Task CurriculumTemplateDownloadHandler()
        {
            var documentStream = await this.UploadService.GetCurriculumTemplate();
            var fileName = "Uchebna-programa-CPO.xlsx";

            await FileUtils.SaveAs(this.JsRuntime, fileName, documentStream.ToArray());
        }

        private async Task UpdateAfterExcelImport()
        {
            await this.LoadCurriculumDataAsync();
            //await this.UpdateProgramHoursAfterCurriculumSubmit();
        }

        private async Task OpenImportCurriculumModal()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.importCandidateCurriculumModal.OpenModal(0, this.validationClientVM.IdValidationClient);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task EditSchedule(ValidationCurriculumVM validationCurriculum)
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

                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(validationCurriculum.IdValidationCurriculum, "TrainingProgramCurriculum");
                if (concurrencyInfoValue == null)
                {
                    await this.AddEntityIdAsCurrentlyOpened(validationCurriculum.IdValidationCurriculum, "TrainingProgramCurriculum");
                }

                var type = this.HideBtnsWhenSPK ? "SPK" : "PP";
                await this.validationCurriculumModal.OpenModal(validationCurriculum, this.addedCurriculums, this.candidateProviderSpecialityVM.IdCandidateProviderSpeciality, this.validationClientVM.Speciality, type, concurrencyInfoValue);
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

                await this.validationCurriculumModal.OpenModal(new ValidationCurriculumVM() { IdValidationClient = this.validationClientVM.IdValidationClient }, this.addedCurriculums, this.candidateProviderSpecialityVM.IdCandidateProviderSpeciality, this.validationClientVM.Speciality);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteCurriculum(ValidationCurriculumVM validationCurriculum)
        {
            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
            if (isConfirmed)
            {
                var result = await this.TrainingService.DeleteValidationCurriculumAsync(this.validationCurriculumToDelete.IdValidationCurriculum);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    return;
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                    //await this.UpdateProgramHoursAfterCurriculumSubmit();
                }

                this.addedCurriculums.RemoveAll(x => x.IdValidationCurriculum == this.validationCurriculumToDelete.IdValidationCurriculum);

                await this.curriculumsGrid.DeleteRecordAsync("IdTrainingCurriculum", this.validationCurriculumToDelete);

                await this.curriculumsGrid.Refresh();

                await this.curriculumsGrid.CallStateHasChangedAsync();
            }
        }

        private async Task OpenDOCERUModal()
        {
            if (this.selectedValidationCurriculums.Count == 1)
            {
                var curriculum = this.selectedValidationCurriculums.FirstOrDefault();
                if (curriculum!.ProfessionalTraining == "Б")
                {
                    await this.ShowErrorAsync("Не можете да въведете съответствие с ДОС за тема с вид професионална подтготовка от раздел 'Б'!");
                    return;
                }
            }

            if (this.validationClientVM.Speciality.IdDOC is null)
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
                foreach (var selectedCurriculum in this.selectedValidationCurriculums)
                {
                    if (this.selectedValidationCurriculums.Any(x => x.IdProfessionalTraining != selectedCurriculum.IdProfessionalTraining))
                    {
                        await this.ShowErrorAsync("Моля, изберете теми от учебната програма от един същи вид професионална подготовка!");

                        return;
                    }
                }

                await this.docEruModal.OpenModal(this.validationClientVM.Speciality, null, this.selectedValidationCurriculums);
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
                    CandidateCurriculumExcelVM candidateCurriculumExcelVM = new CandidateCurriculumExcelVM();
                    var professionalTrainingId = (await this.DataSourceService.GetKeyValueByIntCodeAsync("ProfessionalTraining", "B")).IdKeyValue;
                    IEnumerable<ERUVM> erusFromDoc = new List<ERUVM>();

                    if (this.validationClientVM.Speciality.IdDOC != null)
                    {
                        erusFromDoc = await this.DocService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = this.validationClientVM.Speciality.IdDOC ?? default });
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

                    if (this.validationClientVM.Speciality.IdDOC.HasValue)
                    {
                        var docSource = await this.DocService.GetAllActiveDocAsync();
                        var doc = docSource.FirstOrDefault(x => x.IdDOC == this.validationClientVM.Speciality.IdDOC.Value);
                        if (doc is not null)
                        {
                            foreach (var curriculum in this.addedCurriculums)
                            {
                                if (curriculum.IdProfessionalTraining != professionalTrainingId)
                                {
                                    if (!doc.IsDOI && !curriculum.SelectedERUs.Any())
                                    {
                                        candidateCurriculumExcelVM.MissingTopicERUs.Add($"Няма добавена Единица резултат от учене (ЕРУ) от ДОС към темата!->Тема: {curriculum.Topic}");
                                    }
                                }
                            }

                            if (erus.Count != erusFromDoc.Count())
                            {
                                var missingErus = erusFromDoc.Where(x => erus.All(y => y.IdERU != x.IdERU)).ToList();

                                foreach (var missingEru in missingErus)
                                {
                                    if (!doc.IsDOI)
                                    {
                                        candidateCurriculumExcelVM.MissingDOCERUs.Add($"Единицата резултат от учене (ЕРУ) от ДОС не е добавена към нито една тема!->ЕРУ: {missingEru.Code}");
                                    }
                                }
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
                    var isCurriculumInvalid = await this.IsCurriculumInvalidAsync();
                    var result = await this.CandidateProviderService.PrintValidationCurriculumAsync(this.frameworkProgramVM, this.validationClientVM.Speciality,
                        this.totalHours, this.compulsoryHours, this.nonCompulsoryHours, this.validationClientVM.CandidateProvider.ProviderOwner, isCurriculumInvalid, this.candidateProviderSpecialityVM, this.validationClientVM.CandidateProvider, null, this.addedCurriculums, false);
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

            if (this.validationClientVM.Speciality.IdDOC != null)
            {
                erusFromDoc = await this.DocService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = this.validationClientVM.Speciality.IdDOC ?? default });
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

            if (this.validationClientVM.Speciality.IdDOC.HasValue)
            {
                var docSource = await this.DocService.GetAllActiveDocAsync();
                var doc = docSource.FirstOrDefault(x => x.IdDOC == this.validationClientVM.Speciality.IdDOC.Value);
                if (doc is not null)
                {
                    foreach (var curriculum in this.addedCurriculums)
                    {
                        if (curriculum.IdProfessionalTraining != professionalTrainingId)
                        {
                            if (!doc.IsDOI && !curriculum.SelectedERUs.Any())
                            {
                                candidateCurriculumExcelVM.MissingTopicERUs.Add($"Няма добавена Единица резултат от учене (ЕРУ) от ДОС към темата!->Тема: {curriculum.Topic}");
                            }
                        }
                    }

                    if (erus.Count != erusFromDoc.Count())
                    {
                        var missingErus = erusFromDoc.Where(x => erus.All(y => y.IdERU != x.IdERU)).ToList();

                        foreach (var missingEru in missingErus)
                        {
                            if (!doc.IsDOI)
                            {
                                candidateCurriculumExcelVM.MissingDOCERUs.Add($"Единицата резултат от учене (ЕРУ) от ДОС не е добавена към нито една тема!->ЕРУ: {missingEru.Code}");
                            }
                        }
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

        private async void UpdateAfterSubmitTrainingCurriculumModal(ResultContext<ValidationCurriculumVM> resultContext)
        {
            if (resultContext.ListMessages.Any())
            {
                this.addedCurriculums.RemoveAll(x => x.IdValidationCurriculum == resultContext.ResultContextObject.IdValidationCurriculum);
                var value = this.professionalTrainingsSource.FirstOrDefault(x => x.IdKeyValue == resultContext.ResultContextObject.IdProfessionalTraining).DefaultValue1;
                resultContext.ResultContextObject.ProfessionalTraining = value;

                this.addedCurriculums.Add(resultContext.ResultContextObject);

                this.addedCurriculums = this.addedCurriculums.OrderBy(x => x.ProfessionalTraining).ThenBy(x => x.IdValidationCurriculum).ToList();

                this.StateHasChanged();

                //await this.UpdateProgramHoursAfterCurriculumSubmit();

                await this.CallbackAfterCurriculumModalSubmit.InvokeAsync();
            }
        }

        //private async Task UpdateProgramHoursAfterCurriculumSubmit()
        //{
        //    var kvBType = this.professionalTrainingsSource.FirstOrDefault(x => x.KeyValueIntCode == "B").IdKeyValue;

        //    this.ProgramVM.SelectableHours = 0;
        //    this.ProgramVM.MandatoryHours = 0;
        //    foreach (var curriculum in this.addedCurriculums)
        //    {
        //        if (curriculum.IdProfessionalTraining == kvBType)
        //        {
        //            this.ProgramVM.SelectableHours += (int)((curriculum.Theory.HasValue ? curriculum.Theory.Value : 0) + (curriculum.Practice.HasValue ? curriculum.Practice.Value : 0));
        //        }
        //        else
        //        {
        //            this.ProgramVM.MandatoryHours += (int)((curriculum.Theory.HasValue ? curriculum.Theory.Value : 0) + (curriculum.Practice.HasValue ? curriculum.Practice.Value : 0));
        //        }
        //    }

        //    await this.TrainingService.UpdateTrainingProgramHoursByIdProgramAsync(this.ProgramVM.IdProgram, this.ProgramVM.MandatoryHours, this.ProgramVM.SelectableHours);
        //}

        private void CustomizeCellHours(QueryCellInfoEventArgs<ValidationCurriculumVM> args)
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

        private async Task AfterSelectedERUHandler(List<ERUVM> erus)
        {
            foreach (var selectedCurriculum in this.selectedValidationCurriculums)
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
        private void CurriculumRecordClickHandler(RecordClickEventArgs<ValidationCurriculumVM> args)
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

        private async Task CurriculumDeselectedHandler(RowDeselectEventArgs<ValidationCurriculumVM> args)
        {
            if (this.isCurriculumGridButtonClicked)
            {
                args.Cancel = true;
            }
            else
            {
                this.selectedValidationCurriculums.Clear();
                this.selectedValidationCurriculums = await this.curriculumsGrid.GetSelectedRecordsAsync();
                if (!this.selectedValidationCurriculums.Any())
                {
                    this.curriculumSelected = false;
                }
            }
        }

        private async Task CurriculumSelectedHandler(RowSelectEventArgs<ValidationCurriculumVM> args)
        {
            if (this.isCurriculumGridButtonClicked)
            {
                args.Cancel = true;
            }
            else
            {
                this.curriculumSelected = true;
                this.selectedValidationCurriculums.Clear();
                this.selectedValidationCurriculums = await this.curriculumsGrid.GetSelectedRecordsAsync();
            }
        }

        // превентва селектирането на ред, ако е натистнат бутон от учебна програма грида
        private void CurriculumSelectingHandler(RowSelectingEventArgs<ValidationCurriculumVM> args)
        {
            if (this.isCurriculumGridButtonClicked)
            {
                args.Cancel = true;
            }
        }

        // превентва деселектирането на ред, ако е натистнат бутон от учебна програма грида
        private void CurriculumDeselectingHandler(RowDeselectEventArgs<ValidationCurriculumVM> args)
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

        private async Task DeleteSelectedCurriculumsBtn()
        {
            var selectedCurriculums = await this.curriculumsGrid.GetSelectedRecordsAsync();
            if (!selectedCurriculums.Any())
            {
                await this.ShowErrorAsync("Моля, изберете поне един ред от учебната програма за изтриване!");
                return;
            }
            string msg = "Сигурни ли сте, че искате да изтриете избраните записи?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
            if (isConfirmed)
            {
                var result = await this.TrainingService.DeleteListCandidateValidationCurriculumAsync(selectedCurriculums);
                if (!result.HasErrorMessages)
                {
                    foreach (var curriculum in selectedCurriculums)
                    {
                        this.addedCurriculums.RemoveAll(x => x.IdValidationCurriculum == curriculum.IdValidationCurriculum);

                        await this.curriculumsGrid.DeleteRecordAsync("IdTrainingCurriculum", curriculum);
                    }

                    await this.curriculumsGrid.Refresh();
                    await this.curriculumsGrid.CallStateHasChangedAsync();

                    await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                }
                else
                {
                    await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                }
            }

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

                await this.uploadedFilesModal.OpenModal(null, this.validationClientVM);
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

                if (!string.IsNullOrEmpty(this.validationClientVM.UploadedCurriculumFileName))
                {
                    this.SpinnerHide();
                    string msg = "Вече има прикачен файл с учебен план и учебна програма. В случай, че желаете да прикачите нов, старият ще бъде презаписан. Сигурни ли сте, че искате да продължите?";
                    bool confirmed = await this.ShowConfirmDialogAsync(msg);
                    if (confirmed)
                    {
                        this.SpinnerShow();

                        await this.DeleteUploadedCurriculumFileAsync();

                        await this.uploadCourseCurriculumFileModal.OpenModal(this.validationClientVM.IdValidationClient, this.validationClientVM.FullName, false);
                    }
                }
                else
                {
                    await this.uploadCourseCurriculumFileModal.OpenModal(this.validationClientVM.IdValidationClient, this.validationClientVM.FullName, false);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void UpdateValidationClientCurriculumUploadedFileNameAfterUploadedFile(string uploadedFileName)
        {
            this.validationClientVM.UploadedCurriculumFileName = uploadedFileName;
        }

        private async Task DeleteUploadedCurriculumFileAsync()
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var filePath = settingResource + "\\" + this.validationClientVM.UploadedCurriculumFileName;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);

                this.validationClientVM.UploadedCurriculumFileName = string.Empty;
                await this.TrainingService.UpdateValidationClientCurriculumFileNameAsync(this.validationClientVM.IdValidationClient);
            }
        }
    }
}
