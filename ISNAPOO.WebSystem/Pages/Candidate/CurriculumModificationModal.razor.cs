using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CurriculumModificationModal : BlazorBaseComponent
    {
        private SfGrid<CandidateCurriculumVM> curriculumsGrid = new SfGrid<CandidateCurriculumVM>();
        private CandidateCurriculumModal candidateCurriculumModal = new CandidateCurriculumModal();
        private DOCERUModal docEruModal = new DOCERUModal();
        private ImportCandidateCurriculumFromModificationModal importCandidateCurriculumModal = new ImportCandidateCurriculumFromModificationModal();

        private List<CandidateCurriculumVM> addedCurriculums = new List<CandidateCurriculumVM>();
        private CandidateCurriculumVM candidateCurriculumToDelete = new CandidateCurriculumVM();
        private List<CandidateCurriculumVM> selectedCandidateCurriculums = new List<CandidateCurriculumVM>();
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
        private CandidateProviderSpecialityVM candidateProviderSpecialityVM = new CandidateProviderSpecialityVM();
        private double? totalPracticeHours = 0;
        private double? totalTheoryHours = 0;
        private int idCandidateCurriculumModification = 0;
        private FrameworkProgramVM frameworkProgramVM = new FrameworkProgramVM();
        private SpecialityVM specialityVM = new SpecialityVM();
        private bool curriculumSelected = false;
        private bool isCurriculumGridButtonClicked = false;
        private bool isCurriculumValid = false;
        private string title = string.Empty;
        private string changedSpeciality = string.Empty;

        [Parameter]
        public bool HideAllActions { get; set; }

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

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

        [Inject]
        public ICandidateCurriculumService CandidateCurriculumService { get; set; }

        public async Task OpenModal(int idCandidateCurriculumModification, CandidateProviderSpecialityVM candidateProviderSpeciality, SpecialityVM speciality)
        {
            this.idCandidateCurriculumModification = idCandidateCurriculumModification;
            this.candidateProviderSpecialityVM = candidateProviderSpeciality;
            this.specialityVM = speciality;

            await this.LoadCurriculumDataAsync();

            this.title = this.HideAllActions ? $"Данни за учебен план и учебни програми за специалност <span style=\"color: #ffffff;\">{this.specialityVM.CodeAndName}</span>" : $"Промяна на учебен план и учебни програми за специалност <span style=\"color: #ffffff;\">{this.specialityVM.CodeAndName}</span>";

            this.isVisible = true;
            this.StateHasChanged();
        }

        public async Task LoadCurriculumDataAsync()
        {
            this.professionalTrainingsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            this.kvBType = this.professionalTrainingsSource.FirstOrDefault(x => x.KeyValueIntCode == "B");
            this.minimumLevelEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MinimumLevelEducation");
            this.trainingPeriodSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingPeriod");
            this.erusSource = this.DataSourceService.GetAllERUsList();
            this.addedCurriculums = (await this.CandidateProviderService.GetCandidateCurriculumsByIdCandidateCurriculumModificationAsync(this.idCandidateCurriculumModification)).ToList();
            foreach (var candidateCurriculum in this.addedCurriculums)
            {
                var value = this.professionalTrainingsSource.FirstOrDefault(x => x.IdKeyValue == candidateCurriculum.IdProfessionalTraining).DefaultValue1;
                candidateCurriculum.ProfessionalTraining = value;

                var erus = this.erusSource.Where(x => candidateCurriculum.CandidateCurriculumERUs.Select(x => x.IdERU).ToList().Contains(x.IdERU));
                candidateCurriculum.SelectedERUs.AddRange(erus);
            }

            if (this.candidateProviderSpecialityVM.IdFrameworkProgram.HasValue)
            {
                this.frameworkProgramVM = await this.FrameworkProgramService.GetFrameworkProgramByIdAsync(this.candidateProviderSpecialityVM.IdFrameworkProgram!.Value);

                var lvEdu = this.minimumLevelEducationSource.FirstOrDefault(x => x.IdKeyValue == this.frameworkProgramVM.IdMinimumLevelEducation);
                if (lvEdu != null)
                    this.frameworkProgramVM.MinimumLevelEducationName = lvEdu.Name;

                this.frameworkProgramVM.TrainingPeriodName = this.trainingPeriodSource.FirstOrDefault(x => x.IdKeyValue == this.frameworkProgramVM.IdTrainingPeriod).Name;
            }

            await this.curriculumsGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task FinishModificationBtn()
        {
            string msg = "Сигурни ли сте, че искате да приключите въвеждането на нов учебен план и учебни програми за специалност " + this.changedSpeciality + "? След като потвърдите, учебният план става окончателен и няма да можете да правите промени.";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
            if (isConfirmed)
            {
                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    this.SpinnerShow();

                    this.changedSpeciality = $"{this.specialityVM.Code} {this.specialityVM.Name}";

                    this.loading = false;
                    await this.ValidateCurriculum(false);

                    if (this.isCurriculumValid)
                    {
                        var result = await this.CandidateProviderService.FinishCandidateCurriculumModificationAsync(this.idCandidateCurriculumModification);
                        if (result.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                        }
                        else
                        {
                            await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                            this.isVisible = false;

                            await this.CallbackAfterSubmit.InvokeAsync();
                        }
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task CancelModificationBtn()
        {
            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да откажете въведените промени по учебния план и учебните програми?");
            if (isConfirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;



                    var result = await this.CandidateProviderService.CancelCandidateCurriculumModificationAsync(this.idCandidateCurriculumModification);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

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

        private async Task CurriculumTemplateDownloadHandler()
        {
            var documentStream = await this.UploadService.GetCurriculumTemplate();
            var fileName = "Uchebna-programa-CPO.xlsx";

            await FileUtils.SaveAs(this.JsRuntime, fileName, documentStream.ToArray());
        }

        private async Task UpdateAfterExcelImport()
        {
            await this.LoadCurriculumDataAsync();
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

                await this.importCandidateCurriculumModal.OpenModal(this.candidateProviderSpecialityVM.IdCandidateProviderSpeciality);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task EditSchedule(CandidateCurriculumVM candidateCurriculum)
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

                await this.candidateCurriculumModal.OpenModal(candidateCurriculum, this.specialityVM, this.addedCurriculums, this.candidateProviderSpecialityVM);
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

                await this.candidateCurriculumModal.OpenModal(new CandidateCurriculumVM() { IdCandidateCurriculumModification = this.idCandidateCurriculumModification }, this.specialityVM, this.addedCurriculums, this.candidateProviderSpecialityVM);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteCurriculum(CandidateCurriculumVM candidateCurriculum)
        {
            this.candidateCurriculumToDelete = candidateCurriculum;

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (isConfirmed)
            {
                var result = await this.CandidateCurriculumService.DeleteCandidateCurriculumAsync(this.candidateCurriculumToDelete.IdCandidateCurriculum);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    return;
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                }

                this.addedCurriculums.RemoveAll(x => x.IdCandidateCurriculum == this.candidateCurriculumToDelete.IdCandidateCurriculum);

                await this.curriculumsGrid.DeleteRecordAsync("IdCandidateCurriculum", this.candidateCurriculumToDelete);

                await this.curriculumsGrid.Refresh();

                await this.curriculumsGrid.CallStateHasChangedAsync();
            }
        }

        private async Task OpenDOCERUModal()
        {
            if (this.selectedCandidateCurriculums.Count == 1)
            {
                var curriculum = this.selectedCandidateCurriculums.FirstOrDefault();
                if (curriculum!.ProfessionalTraining == "Б")
                {
                    await this.ShowErrorAsync("Не можете да въведете съответствие с ДОС за тема с вид професионална подтготовка от раздел 'Б'!");
                    return;
                }
            }

            if (this.specialityVM.IdDOC is null)
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
                foreach (var selectedCurriculum in this.selectedCandidateCurriculums)
                {
                    if (this.selectedCandidateCurriculums.Any(x => x.IdProfessionalTraining != selectedCurriculum.IdProfessionalTraining))
                    {
                        await this.ShowErrorAsync("Моля, изберете теми от учебната програма от един същи вид професионална подготовка!");

                        return;
                    }
                }

                await this.docEruModal.OpenModal(this.specialityVM, this.selectedCandidateCurriculums);
            }
        }

        private async Task ValidateCurriculum(bool showSuccessToast)
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

                this.isCurriculumValid = false;

                if (!this.addedCurriculums.Any())
                {
                    await this.ShowErrorAsync("Моля, добавете учебен план и учебни програми!");
                }
                else
                {
                    CandidateCurriculumExcelVM candidateCurriculumExcelVM = new CandidateCurriculumExcelVM();
                    var professionalTrainingId = (await this.DataSourceService.GetKeyValueByIntCodeAsync("ProfessionalTraining", "B")).IdKeyValue;
                    IEnumerable<ERUVM> erusFromDoc = new List<ERUVM>();

                    if (this.specialityVM.IdDOC != null)
                    {
                        erusFromDoc = await this.DocService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = this.specialityVM.IdDOC ?? default });
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

                    if (this.specialityVM.IdDOC.HasValue)
                    {
                        var docSource = await this.DocService.GetAllActiveDocAsync();
                        var doc = docSource.FirstOrDefault(x => x.IdDOC == this.specialityVM.IdDOC.Value);
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
                        this.isCurriculumValid = true;
                        if (showSuccessToast)
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
                    var isCurriculumInvalid = await this.IsCurriculumInvalidAsync();
                    var candidateProvider = await this.CandidateProviderService.GetCandidateProviderWithoutAnythingIncludedByIdAsync(this.UserProps.IdCandidateProvider);
                    var result = await this.CandidateProviderService.PrintCurriculumAsync(this.frameworkProgramVM, this.specialityVM,
                        this.totalHours, this.compulsoryHours, this.nonCompulsoryHours, candidateProvider.ProviderOwner, isCurriculumInvalid, this.candidateProviderSpecialityVM, candidateProvider, this.addedCurriculums, null, false);
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

            if (this.specialityVM.IdDOC != null)
            {
                erusFromDoc = await this.DocService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = this.specialityVM.IdDOC ?? default });
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

            if (this.specialityVM.IdDOC.HasValue)
            {
                var docSource = await this.DocService.GetAllActiveDocAsync();
                var doc = docSource.FirstOrDefault(x => x.IdDOC == this.specialityVM.IdDOC.Value);
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

        private void UpdateAfterSubmitCandidateCurriculumModal(ResultContext<CandidateCurriculumVM> resultContext)
        {
            if (resultContext.ListMessages.Any())
            {
                this.addedCurriculums.RemoveAll(x => x.IdCandidateCurriculum == resultContext.ResultContextObject.IdCandidateCurriculum);
                var value = this.professionalTrainingsSource.FirstOrDefault(x => x.IdKeyValue == resultContext.ResultContextObject.IdProfessionalTraining).DefaultValue1;
                resultContext.ResultContextObject.ProfessionalTraining = value;

                this.addedCurriculums.Add(resultContext.ResultContextObject);

                this.addedCurriculums = this.addedCurriculums.OrderBy(x => x.ProfessionalTraining).ThenBy(x => x.IdCandidateCurriculum).ToList();

                this.StateHasChanged();

            }
        }

        private void CustomizeCellHours(QueryCellInfoEventArgs<CandidateCurriculumVM> args)
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
            foreach (var selectedCurriculum in this.selectedCandidateCurriculums)
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
        private void CurriculumRecordClickHandler(RecordClickEventArgs<CandidateCurriculumVM> args)
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

        private async Task CurriculumDeselectedHandler(RowDeselectEventArgs<CandidateCurriculumVM> args)
        {
            if (this.isCurriculumGridButtonClicked)
            {
                args.Cancel = true;
            }
            else
            {
                this.selectedCandidateCurriculums.Clear();
                this.selectedCandidateCurriculums = await this.curriculumsGrid.GetSelectedRecordsAsync();
                if (!this.selectedCandidateCurriculums.Any())
                {
                    this.curriculumSelected = false;
                }
            }
        }

        private async Task CurriculumSelectedHandler(RowSelectEventArgs<CandidateCurriculumVM> args)
        {
            if (this.isCurriculumGridButtonClicked)
            {
                args.Cancel = true;
            }
            else
            {
                this.curriculumSelected = true;
                this.selectedCandidateCurriculums.Clear();
                this.selectedCandidateCurriculums = await this.curriculumsGrid.GetSelectedRecordsAsync();
            }
        }

        // превентва селектирането на ред, ако е натистнат бутон от учебна програма грида
        private void CurriculumSelectingHandler(RowSelectingEventArgs<CandidateCurriculumVM> args)
        {
            if (this.isCurriculumGridButtonClicked)
            {
                args.Cancel = true;
            }
        }

        // превентва деселектирането на ред, ако е натистнат бутон от учебна програма грида
        private void CurriculumDeselectingHandler(RowDeselectEventArgs<CandidateCurriculumVM> args)
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

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избраните записи?");
            if (isConfirmed)
            {
                var result = await this.CandidateCurriculumService.DeleteListCandidateCurriculumAsync(selectedCurriculums);
                if (!result.HasErrorMessages)
                {
                    foreach (var curriculum in selectedCurriculums)
                    {
                        this.addedCurriculums.RemoveAll(x => x.IdCandidateCurriculum == curriculum.IdCandidateCurriculum);

                        await this.curriculumsGrid.DeleteRecordAsync("IdCandidateCurriculum", curriculum);
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
    }
}
