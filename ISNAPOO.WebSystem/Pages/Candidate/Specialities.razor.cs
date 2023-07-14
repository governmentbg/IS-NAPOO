using System.Linq;
using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using NuGet.Packaging;
using RegiX.Class.AVTR.GetActualStateV2;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.ExcelExport;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class Specialities : BlazorBaseComponent
    {
        private SfAutoComplete<int, SpecialityVM> sfAutoCompleteSpecialities = new SfAutoComplete<int, SpecialityVM>();
        public SfGrid<CandidateProviderSpecialityVM> providerSpecialitiesGrid = new SfGrid<CandidateProviderSpecialityVM>();
        private SfGrid<CandidateCurriculumVM> curriculumsGrid = new SfGrid<CandidateCurriculumVM>();
        private ImportCandidateCurriculumModal importCandidateCurriculumModal = new ImportCandidateCurriculumModal();
        private ValidationMessageStore? messageStore;
        private CandidateCurriculumModal candidateCurriculumModal = new CandidateCurriculumModal();
        private DOCERUModal docEruModal = new DOCERUModal();
        private CurriculumModificationReasonModal curriculumModificationReasonModal = new CurriculumModificationReasonModal();
        private CurriculumModificationModal curriculumModificationModal = new CurriculumModificationModal();
        private CurriculumModificationHistoryModal curriculumModificationHistoryModal = new CurriculumModificationHistoryModal();
        private UploadCurriculumFileModal uploadCurriculumFileModal = new UploadCurriculumFileModal();
        private CurriculumReadonlyModal curriculumReadonlyModal = new CurriculumReadonlyModal();

        private int idSpeciality = 0;
        private CandidateProviderSpecialitiesVM candidateProviderSpecialitiesVM = new CandidateProviderSpecialitiesVM();
        private CandidateProviderSpecialityVM selectedCandidateProviderSpeciality = new CandidateProviderSpecialityVM();
        private List<CandidateProviderSpecialityVM> providerSpecialitiesSource = new List<CandidateProviderSpecialityVM>();
        private List<CandidateCurriculumVM> addedCurriculums = new List<CandidateCurriculumVM>();
        private List<SpecialityVM> specialitySource = new List<SpecialityVM>();
        private IEnumerable<KeyValueVM> professionalTrainingsSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvVQSSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvMinimumLevelEducationSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvTrainingPeriodSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvFormEducationSource = new List<KeyValueVM>();
        private List<FrameworkProgramVM> frameworkProgramSource = new List<FrameworkProgramVM>();
        private string selectedSpecialityName = string.Empty;
        private bool isSpecialityGridButtonClicked = false;
        private bool isCurriculumGridButtonClicked = false;
        private bool firstRender = false;
        private bool showUploadCurriculumFileBtn = true;
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
        private bool frameworkProgramSelected = false;
        private FrameworkProgramVM frameworkProgramVM = new FrameworkProgramVM();
        public double selectedRowIdx = 0;
        private List<ERUVM> erusSource = new List<ERUVM>();
        private KeyValueVM kvDailyFormEducation = new KeyValueVM();
        private KeyValueVM kvSPKTypeValue = new KeyValueVM();

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool DisableFieldsWhenOpenFromProfile { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

        [Parameter]
        public bool DisableFieldsWhenApplicationStatusIsNotDocPreparation { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsNAPOO { get; set; }

        [Parameter]
        public bool DisableFieldsWhenActiveLicenceChange { get; set; }

        [Parameter]
        public bool IsUserProfileAdministrator { get; set; }

        [Parameter]
        public bool IsLicenceChange { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IAreaService AreaService { get; set; }

        [Inject]
        public IProfessionalDirectionService ProfessionalDirectionService { get; set; }

        [Inject]
        public IProfessionService ProfessionService { get; set; }

        [Inject]
        public ICandidateCurriculumService CandidateCurriculumService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IDOCService DocService { get; set; }

        [Inject]
        public IUploadFileService UploadService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IFrameworkProgramService FrameworkProgramService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.candidateProviderSpecialitiesVM);
            this.FormTitle = "Специалности - Учебна програма";

            this.editContext.MarkAsUnmodified();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await this.LoadDataAsync();

                await this.providerSpecialitiesGrid.Refresh();

                this.editContext.MarkAsUnmodified();

                this.StateHasChanged();
            }
        }

        private async Task LoadDataAsync()
        {
            this.specialitySource = this.DataSourceService.GetAllSpecialitiesList().Where(x => x.IdStatus == DataSourceService.GetActiveStatusID()).ToList();

            this.erusSource = this.DataSourceService.GetAllERUsList();

            this.professionalTrainingsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            this.kvVQSSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
            this.kvFormEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation");
            this.kvDailyFormEducation = this.kvFormEducationSource.FirstOrDefault(x => x.KeyValueIntCode == "Type1");
            this.kvMinimumLevelEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MinimumLevelEducation");
            this.kvTrainingPeriodSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingPeriod");
            this.kvSPKTypeValue = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification");

            this.providerSpecialitiesSource = (await this.CandidateProviderService.GetAllCandidateProviderSpecialitiesWithActualCurriculumsByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider)).ToList();

            this.RemoveAlreadyAddedSpecialitiesFromSource();
        }

        // премахва от списъка вече добавените специалности към заявлението
        private void RemoveAlreadyAddedSpecialitiesFromSource()
        {
            if (!this.DisableFieldsWhenActiveLicenceChange && !this.DisableFieldsWhenUserIsNAPOO && !this.DisableFieldsWhenUserIsExternalExpertOrCommittee && !this.DisableFieldsWhenApplicationStatusIsNotDocPreparation && !this.DisableFieldsWhenOpenFromProfile)
            {
                foreach (var speciality in this.providerSpecialitiesSource)
                {
                    this.specialitySource.RemoveAll(x => x.IdSpeciality == speciality.IdSpeciality);
                }
            }
        }

        private async Task OpenChangeCurriculumModalBtn()
        {
            var selectedSpecialites = await this.providerSpecialitiesGrid.GetSelectedRecordsAsync();
            if (!selectedSpecialites.Any())
            {
                await this.ShowErrorAsync("Моля, изберете специалност!");
                return;
            }

            var selectedSpeciality = selectedSpecialites.FirstOrDefault()!;

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var kvWorkingStatusValue = await this.DataSourceService.GetKeyValueByIntCodeAsync("CurriculumModificationStatusType", "Working");
                var curriculumModification = await this.CandidateProviderService.GetCurriculumModificationByIdCandidateProviderSpecialityAndByIdModificationStatusAsync(selectedSpeciality.IdCandidateProviderSpeciality, kvWorkingStatusValue.IdKeyValue);
                if (curriculumModification is null)
                {
                    await this.curriculumModificationReasonModal.OpenModal(selectedSpeciality);
                }
                else
                {
                    await this.curriculumModificationModal.OpenModal(curriculumModification.IdCandidateCurriculumModification, selectedSpeciality, selectedSpeciality.Speciality);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenHistoryChangeCurriculumModalBtn()
        {
            var selectedSpecialites = await this.providerSpecialitiesGrid.GetSelectedRecordsAsync();
            if (!selectedSpecialites.Any())
            {
                await this.ShowErrorAsync("Моля, изберете специалност!");
                return;
            }

            var selectedSpeciality = selectedSpecialites.FirstOrDefault()!;

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.curriculumModificationHistoryModal.OpenModal(selectedSpeciality, selectedSpeciality.Speciality);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnFilterSpeciality(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 1)
            {
                var query = new Query().Where(new WhereFilter() { Field = "CodeAndAreaForAutoCompleteSearch", Operator = "contains", value = args.Text, IgnoreCase = true });

                query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                await this.sfAutoCompleteSpecialities.FilterAsync(this.specialitySource, query);
            }
        }

        private async Task DeleteProviderSpecialityBtn(CandidateProviderSpecialityVM candidateProviderSpeciality)
        {
            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);

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

                    bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
                    if (!hasPermission) { return; }

                    var doesConnectionExist = await this.DoesConnectionBetweenSpecialityExistAsync(candidateProviderSpeciality.Speciality);
                    if (doesConnectionExist)
                    {
                        await this.ShowErrorAsync("Не можете да изтриете специалността, защото има въведена свързана информация!");
                        return;
                    }

                    var result = await this.CandidateProviderService.DeleteCandidateProviderSpecialityByIdAsync(candidateProviderSpeciality.IdCandidateProviderSpeciality);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                        this.SpinnerHide();
                        this.loading = false;
                        return;
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                    }

                    this.providerSpecialitiesSource.Remove(candidateProviderSpeciality);
                    this.providerSpecialitiesSource = this.providerSpecialitiesSource.OrderBy(x => x.Speciality.Code).ToList();

                    await this.providerSpecialitiesGrid.Refresh();

                    this.StateHasChanged();
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task<bool> DoesConnectionBetweenSpecialityExistAsync(SpecialityVM speciality)
        {
            var candidateProviderSpeciality = await this.CandidateProviderService.GetCandidateProviderSpecialityByIdCandidateProviderAndByIdSpecialityAsync(this.CandidateProviderVM.IdCandidate_Provider, speciality.IdSpeciality);
            if (candidateProviderSpeciality is not null)
            {
                if (candidateProviderSpeciality.CandidateCurriculums is not null && candidateProviderSpeciality.CandidateCurriculumModifications.Any(x => x.CandidateCurriculums.Any()))
                {
                    return true;
                }
            }

            var trainers = await this.CandidateProviderService.GetAllActiveCandidateProviderTrainersByCandidateProviderIdWithTrainerSpecialitiesIncludedAsync(this.CandidateProviderVM.IdCandidate_Provider);
            var trainerSpecialityExist = trainers.Any(x => x.CandidateProviderTrainerSpecialities.Any(y => y.IdSpeciality == speciality.IdSpeciality));
            if (trainerSpecialityExist)
            {
                return true;
            }

            var premises = await this.CandidateProviderService.GetAllActiveCandidateProviderPremisesByCandidateProviderIdWithPremisesSpecialitiesIncludedAsync(this.CandidateProviderVM.IdCandidate_Provider);
            var mtbSpecialityExist = premises.Any(x => x.CandidateProviderPremisesSpecialities.Any(y => y.IdSpeciality == speciality.IdSpeciality));
            if (mtbSpecialityExist)
            {
                return true;
            }

            return false;
        }

        private async Task DeleteCurriculum(CandidateCurriculumVM candidateCurriculum)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
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

                    var result = await this.CandidateCurriculumService.DeleteCandidateCurriculumAsync(candidateCurriculum.IdCandidateCurriculum);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                        this.SpinnerHide();
                        this.loading = false;
                        return;
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                    }

                    this.addedCurriculums.RemoveAll(x => x.IdCandidateCurriculum == candidateCurriculum.IdCandidateCurriculum);

                    await this.curriculumsGrid.DeleteRecordAsync("IdSpeciality", candidateCurriculum);

                    await this.curriculumsGrid.Refresh();

                    await this.curriculumsGrid.CallStateHasChangedAsync();
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
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

                var candidateCurriculumModification = await this.CandidateProviderService.GetActualCandidateCurriculumModificationByIdCandidateProviderSpecialityAsync(this.selectedCandidateProviderSpeciality.IdSpeciality);
                await this.uploadCurriculumFileModal.OpenModal(candidateCurriculumModification, this.selectedCandidateProviderSpeciality.IdSpeciality);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void ReloadCurriculumFileDataAfterSubmit(List<string> modificationData)
        {
            var fileName = modificationData[0];
            var idSpeciality = int.Parse(modificationData[1]);
            var speciality = this.providerSpecialitiesSource.FirstOrDefault(x => x.IdSpeciality == idSpeciality);
            if (speciality is not null)
            {
                speciality.CurriculumModificationUploadedFileName = fileName;

                this.StateHasChanged();
            }
        }

        private async Task AddSpeciality()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            if (this.idSpeciality == 0)
            {
                var msg = this.LocService.GetLocalizedHtmlString("SpecialityNotChoosed").Value;
                await this.ShowErrorAsync(msg);
            }
            else
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    var result = await this.CandidateProviderService.CreateCandidateProviderSpecialityAsync(this.CandidateProviderVM.IdCandidate_Provider, this.idSpeciality);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        this.specialitySource.RemoveAll(x => x.IdSpeciality == idSpeciality);

                        this.providerSpecialitiesSource = (await this.CandidateProviderService.GetAllCandidateProviderSpecialitiesWithActualCurriculumsByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider)).ToList();

                        this.idSpeciality = 0;

                        await this.providerSpecialitiesGrid.Refresh();
                        this.StateHasChanged();
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task OpenImportCurriculumModalBtn()
        {
            if (this.selectedCandidateProviderSpeciality.IdCandidateProviderSpeciality == 0)
            {
                await this.ShowErrorAsync("Моля, изберете специалност!");
            }
            else
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    var idCandidateProviderSpeciality = this.selectedCandidateProviderSpeciality.IdCandidateProviderSpeciality;
                    await this.importCandidateCurriculumModal.OpenModal(idCandidateProviderSpeciality, 0);
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private void UpdateAfterExcelImport(List<CandidateCurriculumVM> candidateCurriculumSource)
        {
            if (candidateCurriculumSource.Any())
            {
                this.addedCurriculums = candidateCurriculumSource.ToList();
                foreach (var candidateCurriculum in this.addedCurriculums)
                {
                    var value = this.professionalTrainingsSource.FirstOrDefault(x => x.IdKeyValue == candidateCurriculum.IdProfessionalTraining).DefaultValue1;
                    candidateCurriculum.ProfessionalTraining = value;

                    var erus = this.erusSource.Where(x => candidateCurriculum.CandidateCurriculumERUs.Select(x => x.IdERU).ToList().Contains(x.IdERU));
                    candidateCurriculum.SelectedERUs.AddRange(erus);
                }

                this.addedCurriculums = this.addedCurriculums.OrderBy(x => x.ProfessionalTraining).ThenBy(x => x.Subject).ThenBy(x => x.Topic).ToList();
            }
        }

        private async Task SpecialitySelectedHandler(RowSelectEventArgs<CandidateProviderSpecialityVM> args)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (args.Data is not null)
                {
                    this.selectedRowIdx = await this.providerSpecialitiesGrid.GetRowIndexByPrimaryKeyAsync(args.Data.IdCandidateProviderSpeciality);
                    if (this.isSpecialityGridButtonClicked)
                    {
                        args.Cancel = true;
                    }
                    else
                    {
                        this.totalHours = 0;

                        this.selectedCandidateProviderSpeciality = args.Data;

                        this.selectedSpecialityName = $"{args.Data.Speciality.Code} {args.Data.Speciality.Name}";
                        if (this.selectedCandidateProviderSpeciality.Speciality.IdVQS != 0)
                        {
                            this.frameworkProgramSource = (await this.FrameworkProgramService.GetAllFrameworkProgramsBySpecialityVQSIdAsync(this.selectedCandidateProviderSpeciality.Speciality.IdVQS)).ToList();
                            this.FilterFrameworkProgramDataBasedOnSpecialityIdVQS();
                        }

                        // проверява дали промяната на учебния план е на статус Окончателен, ако е - скрива бутона за ъплоуд
                        this.showUploadCurriculumFileBtn = await this.CandidateProviderService.IsCurriculumUploadFileAllowedAsync(this.selectedCandidateProviderSpeciality.IdSpeciality, this.CandidateProviderVM.IdCandidate_Provider);

                        if (this.selectedCandidateProviderSpeciality.IdFrameworkProgram.HasValue)
                        {
                            this.frameworkProgramVM = await this.FrameworkProgramService.GetFrameworkPgoramByIdWithFormEducationsIncludedAsync(new FrameworkProgramVM() { IdFrameworkProgram = this.selectedCandidateProviderSpeciality.IdFrameworkProgram.Value });
                            this.frameworkProgramVM.MinimumLevelEducationName = this.kvMinimumLevelEducationSource.FirstOrDefault(x => x.IdKeyValue == this.frameworkProgramVM.IdMinimumLevelEducation).Name;
                            this.frameworkProgramVM.TrainingPeriodName = this.kvTrainingPeriodSource.FirstOrDefault(x => x.IdKeyValue == this.frameworkProgramVM.IdTrainingPeriod).Name;
                            var listIds = this.frameworkProgramVM.FrameworkProgramFormEducations.Select(x => x.IdFormEducation).ToList();
                            this.kvFormEducationSource = await this.DataSourceService.GetKeyValuesByListIdsAsync(listIds);
                        }

                        this.addedCurriculums = (await this.CandidateProviderService.GetActualCandidateCurriculumByIdCandidateProviderSpecialityAsync(this.selectedCandidateProviderSpeciality.IdCandidateProviderSpeciality)).ToList();
                        foreach (var candidateCurriculum in this.addedCurriculums)
                        {
                            var value = this.professionalTrainingsSource.FirstOrDefault(x => x.IdKeyValue == candidateCurriculum.IdProfessionalTraining).DefaultValue1;
                            candidateCurriculum.ProfessionalTraining = value;

                            var erus = this.erusSource.Where(x => candidateCurriculum.CandidateCurriculumERUs.Select(x => x.IdERU).ToList().Contains(x.IdERU));
                            candidateCurriculum.SelectedERUs.AddRange(erus);
                        }

                        await this.curriculumsGrid.Refresh();
                        this.StateHasChanged();
                    }
                }
                else
                {
                    this.selectedCandidateProviderSpeciality = new CandidateProviderSpecialityVM();
                    this.selectedSpecialityName = string.Empty;
                    this.addedCurriculums.Clear();
                    this.showUploadCurriculumFileBtn = false;
                    this.frameworkProgramVM = new FrameworkProgramVM();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void FilterFrameworkProgramDataBasedOnSpecialityIdVQS()
        {
            for (int i = this.frameworkProgramSource.Count() - 1; i >= 0; i--)
            {
                var frameworkProgram = this.frameworkProgramSource[i];
                if (!frameworkProgram.FrameworkProgramFormEducations.Any(x => x.IdFormEducation == this.kvDailyFormEducation.IdKeyValue)
                    || frameworkProgram.IdTypeFrameworkProgram != this.kvSPKTypeValue.IdKeyValue)
                {
                    this.frameworkProgramSource.RemoveAll(x => x.IdFrameworkProgram == frameworkProgram.IdFrameworkProgram);
                }
            }
        }

        private void SpecialityDeselectedHandler(RowDeselectEventArgs<CandidateProviderSpecialityVM> args)
        {
            if (this.isSpecialityGridButtonClicked)
            {
                args.Cancel = true;
            }
            else
            {
                this.totalHours = 0;
                this.addedCurriculums = new List<CandidateCurriculumVM>();
                this.selectedSpecialityName = string.Empty;
                this.frameworkProgramVM = new FrameworkProgramVM();
                this.selectedCandidateProviderSpeciality = new CandidateProviderSpecialityVM();
            }
        }

        private async Task EditCurriculumModalBtn(CandidateCurriculumVM candidateCurriculum)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;

                candidateCurriculum.IsEdit = true;
                await this.candidateCurriculumModal.OpenModal(candidateCurriculum, this.selectedCandidateProviderSpeciality.Speciality, this.addedCurriculums, this.selectedCandidateProviderSpeciality);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task AddCurriculumModalBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
                if (!hasPermission) { return; }

                this.loading = true;

                if (this.selectedCandidateProviderSpeciality.IdCandidateProviderSpeciality == 0)
                {
                    await this.ShowErrorAsync("Моля, изберете специалност!");
                }
                else
                {
                    var idCandidateCurriculumModification = await this.CandidateProviderService.GetCandidateCurriculumModificationWhenApplicationByIdCandidateProviderSpecialityAsync(this.selectedCandidateProviderSpeciality.IdCandidateProviderSpeciality);
                    await this.candidateCurriculumModal.OpenModal(new CandidateCurriculumVM() { IdCandidateCurriculumModification = idCandidateCurriculumModification.IdCandidateCurriculumModification }, this.selectedCandidateProviderSpeciality.Speciality, this.addedCurriculums, this.selectedCandidateProviderSpeciality);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void ViewCurriculumsBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.curriculumReadonlyModal.OpenModal(this.addedCurriculums, this.selectedCandidateProviderSpeciality.Speciality, this.frameworkProgramVM);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterSubmitCandidateCurriculumModal()
        {
            this.SpinnerShow();

            this.addedCurriculums = (await this.CandidateProviderService.GetActualCandidateCurriculumByIdCandidateProviderSpecialityAsync(this.selectedCandidateProviderSpeciality.IdCandidateProviderSpeciality)).ToList();
            foreach (var candidateCurriculum in this.addedCurriculums)
            {
                var value = this.professionalTrainingsSource.FirstOrDefault(x => x.IdKeyValue == candidateCurriculum.IdProfessionalTraining).DefaultValue1;
                candidateCurriculum.ProfessionalTraining = value;

                var erus = this.erusSource.Where(x => candidateCurriculum.CandidateCurriculumERUs.Select(x => x.IdERU).ToList().Contains(x.IdERU));
                candidateCurriculum.SelectedERUs.AddRange(erus);
            }

            await this.curriculumsGrid.Refresh();
            this.StateHasChanged();

            this.SpinnerShow();
        }

        private async Task OpenDOCERUModal()
        {
            var selectedCurriculums = await this.curriculumsGrid.GetSelectedRecordsAsync();
            if (selectedCurriculums.Count == 1)
            {
                var curriculum = selectedCurriculums.FirstOrDefault();
                if (curriculum!.ProfessionalTraining == "Б")
                {
                    await this.ShowErrorAsync("Не можете да въведете съответствие с ДОС за тема с вид професионална подтготовка от раздел 'Б'!");
                    return;
                }
            }

            if (this.selectedCandidateProviderSpeciality.Speciality.IdDOC is null)
            {
                await this.ShowErrorAsync("Не можете да въведете съответствие с ДОС, защото за избраната специалност няма въведен ДОС в системата!");
                return;
            }

            if (!selectedCurriculums.Any())
            {
                await this.ShowErrorAsync("Моля, изберете тема/теми от учебната програма!");
            }
            else
            {
                foreach (var selectedCurriculum in selectedCurriculums)
                {
                    if (selectedCurriculums.Any(x => x.IdProfessionalTraining != selectedCurriculum.IdProfessionalTraining))
                    {
                        await this.ShowErrorAsync("Моля, изберете теми от учебната програма от един същи вид професионална подготовка!");

                        return;
                    }
                }

                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    await this.docEruModal.OpenModal(this.selectedCandidateProviderSpeciality.Speciality, selectedCurriculums);
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private void CurriculumDeselectedHandler(RowDeselectEventArgs<CandidateCurriculumVM> args)
        {
            if (this.isCurriculumGridButtonClicked)
            {
                args.Cancel = true;
            }
        }

        private void CurriculumSelectedHandler(RowSelectEventArgs<CandidateCurriculumVM> args)
        {
            if (this.isCurriculumGridButtonClicked)
            {
                args.Cancel = true;
            }
        }

        private async Task AfterSelectedERUHandler(List<ERUVM> erus)
        {
            var selectedCurriculums = await this.curriculumsGrid.GetSelectedRecordsAsync();
            foreach (var selectedCurriculum in selectedCurriculums)
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

        private async Task DownloadDOCBtn(SpecialityVM speciality)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var docVM = await this.DocService.GetDOCByIdAsync(new DocVM() { IdDOC = speciality.IdDOC ?? default });
                var hasFile = await this.UploadService.CheckIfExistUploadedFileAsync<Data.Models.Data.DOC.DOC>(docVM.IdDOC);
                if (hasFile)
                {
                    var documentStream = await this.UploadService.GetUploadedFileAsync<Data.Models.Data.DOC.DOC>(docVM.IdDOC);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, docVM.FileName, documentStream.MS!.ToArray());
                    }
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DownloadCurriculumModificationFileBtn(int idCandidateProviderSpeciality)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var candidateCurriculumModification = await this.CandidateProviderService.GetActualCandidateCurriculumModificationByIdCandidateProviderSpecialityAsync(idCandidateProviderSpeciality);
                if (candidateCurriculumModification is not null)
                {
                    var hasFile = await this.UploadService.CheckIfExistUploadedFileAsync<CandidateCurriculumModification>(candidateCurriculumModification.IdCandidateCurriculumModification);

                    if (hasFile)
                    {
                        var documentStream = await this.UploadService.GetUploadedFileAsync<CandidateCurriculumModification>(candidateCurriculumModification.IdCandidateCurriculumModification);

                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, candidateCurriculumModification.FileName, documentStream.MS!.ToArray());
                        }
                    }
                    else
                    {
                        var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                        await this.ShowErrorAsync(msg);
                    }
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task PrintCurriculum()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                if (!this.selectedCandidateProviderSpeciality.IdFrameworkProgram.HasValue && !this.selectedCandidateProviderSpeciality.IdFormEducation.HasValue)
                {
                    await this.ShowErrorAsync("Моля, въведете рамкова програма и форма на обучение! Необходимо е да запишете промените с бутон Запиши.");
                    return;
                }

                if (!this.selectedCandidateProviderSpeciality.IdFrameworkProgram.HasValue)
                {
                    await this.ShowErrorAsync("Моля, въведете рамкова програма! Необходимо е да запишете промените с бутон Запиши.");
                    return;
                }

                if (!this.selectedCandidateProviderSpeciality.IdFormEducation.HasValue)
                {
                    await this.ShowErrorAsync("Моля, въведете форма на обучение! Необходимо е да запишете промените с бутон Запиши.");
                    return;
                }

                this.loading = true;

                this.SpinnerShow();

                if (this.selectedCandidateProviderSpeciality.IdCandidateProviderSpeciality != 0)
                {
                    if (this.addedCurriculums.Any())
                    {
                        var isCurriculumInvalid = await this.IsCurriculumInvalidAsync();
                        var result = await this.CandidateProviderService.PrintCurriculumAsync(this.frameworkProgramVM, this.selectedCandidateProviderSpeciality.Speciality,
                            this.totalHours, this.compulsoryHours, this.nonCompulsoryHours, this.CandidateProviderVM.ProviderOwner, isCurriculumInvalid, this.selectedCandidateProviderSpeciality, this.CandidateProviderVM, this.addedCurriculums);
                        await FileUtils.SaveAs(this.JsRuntime, "Ucheben-plan-CPO.docx", result.ToArray());
                    }
                    else
                    {
                        await this.ShowErrorAsync("Моля, добавете учебен план и учебни програми!");
                    }
                }
                else
                {
                    await this.ShowErrorAsync("Моля, изберете специалност!");
                }
            }
            finally
            {
                this.loading = false;
                this.SpinnerHide();
            }
        }

        private async Task CurriculumTemplateDownloadHandler()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var documentStream = await this.UploadService.GetCurriculumTemplate();
                var fileName = "Uchebna-programa-CPO.xlsx";

                await FileUtils.SaveAs(this.JsRuntime, fileName, documentStream.ToArray());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task ReloadCurriculumsDataAsync()
        {
            this.SpinnerShow();

            this.addedCurriculums = (await this.CandidateProviderService.GetActualCandidateCurriculumByIdCandidateProviderSpecialityAsync(this.selectedCandidateProviderSpeciality.IdCandidateProviderSpeciality)).ToList();
            foreach (var candidateCurriculum in this.addedCurriculums)
            {
                var value = this.professionalTrainingsSource.FirstOrDefault(x => x.IdKeyValue == candidateCurriculum.IdProfessionalTraining).DefaultValue1;
                candidateCurriculum.ProfessionalTraining = value;

                var erus = this.erusSource.Where(x => candidateCurriculum.CandidateCurriculumERUs.Select(x => x.IdERU).ToList().Contains(x.IdERU));
                candidateCurriculum.SelectedERUs.AddRange(erus);
            }

            await this.curriculumsGrid.Refresh();
            this.StateHasChanged();

            this.SpinnerHide();
        }

        private async Task OnFrameworkProgramSelectedHandlerAsync(SelectEventArgs<FrameworkProgramVM> args)
        {
            this.SpinnerShow();

            var result = new ResultContext<NoResult>();
            if (args.ItemData is null)
            {
                this.selectedCandidateProviderSpeciality.IdFormEducation = null;
                this.selectedCandidateProviderSpeciality.IdFrameworkProgram = null;

                result = await this.CandidateProviderService.UpdateCandidateProviderSpecialityIdFrameworkAndIdFormEducationAsync(this.selectedCandidateProviderSpeciality.IdCandidateProviderSpeciality, null, null);
                result.AddMessage("Рамковата програма е изтрита успешно!");
            }
            else
            {
                this.selectedCandidateProviderSpeciality.IdFormEducation = this.kvDailyFormEducation.IdKeyValue;
                this.selectedCandidateProviderSpeciality.IdFrameworkProgram = args.ItemData.IdFrameworkProgram;

                result = await this.CandidateProviderService.UpdateCandidateProviderSpecialityIdFrameworkAndIdFormEducationAsync(this.selectedCandidateProviderSpeciality.IdCandidateProviderSpeciality, this.selectedCandidateProviderSpeciality.IdFrameworkProgram, this.selectedCandidateProviderSpeciality.IdFormEducation);
                result.AddMessage("Рамковата програма е записана успешно!");
            }

            if (result.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
            }
            else
            {
                await this.ShowSuccessAsync(string.Join("", result.ListMessages));
            }

            this.SpinnerHide();
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

        private void SpecialitiesQueryCellInfo(QueryCellInfoEventArgs<CandidateProviderSpecialityVM> args)
        {
            if (args is not null && args.Data is not null)
            {
                var activeId = this.DataSourceService.GetActiveStatusID();
                if (args.Data.Speciality.IdStatus != activeId)
                {
                    args.Cell.AddClass(new string[] { "color-elements" });
                }
            }
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

                if (!this.selectedCandidateProviderSpeciality.IdFrameworkProgram.HasValue && this.selectedCandidateProviderSpeciality.IdFormEducation.HasValue)
                {
                    await this.ShowErrorAsync("Моля, въведете рамкова програма и форма на обучение! Необходимо е да запишете промените с бутон Запиши.");
                    return;
                }

                if (!this.selectedCandidateProviderSpeciality.IdFrameworkProgram.HasValue)
                {
                    await this.ShowErrorAsync("Моля, въведете рамкова програма! Необходимо е да запишете промените с бутон Запиши.");
                    return;
                }

                if (!this.selectedCandidateProviderSpeciality.IdFormEducation.HasValue)
                {
                    await this.ShowErrorAsync("Моля, въведете форма на обучение! Необходимо е да запишете промените с бутон Запиши.");
                    return;
                }

                if (this.selectedCandidateProviderSpeciality.IdCandidateProviderSpeciality == 0)
                {
                    await this.ShowErrorAsync("Моля, изберете специалност!");
                }
                else
                {
                    if (!this.addedCurriculums.Any())
                    {
                        await this.ShowErrorAsync("Моля, добавете учебен план и учебни програми!");
                    }
                    else
                    {
                        CandidateCurriculumExcelVM candidateCurriculumExcelVM = new CandidateCurriculumExcelVM();
                        var professionalTrainingId = (await this.DataSourceService.GetKeyValueByIntCodeAsync("ProfessionalTraining", "B")).IdKeyValue;
                        IEnumerable<ERUVM> erusFromDoc = new List<ERUVM>();
                        IEnumerable<ERUVM> erusFromSpeciality = new List<ERUVM>();
                        erusFromSpeciality = await this.DocService.GetAllERUsByIdSpecialityAsync(this.selectedCandidateProviderSpeciality.IdSpeciality);

                        if (this.selectedCandidateProviderSpeciality.Speciality.IdDOC != null)
                        {
                            erusFromDoc = await this.DocService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = this.selectedCandidateProviderSpeciality.Speciality.IdDOC ?? default });
                        }

                        var erus = new HashSet<ERUVM>();
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

                        if (erusFromSpeciality.Any())
                        {
                            foreach (var curriculum in this.addedCurriculums)
                            {
                                if (curriculum.IdProfessionalTraining != professionalTrainingId)
                                {
                                    if (!curriculum.SelectedERUs.Any())
                                    {
                                        candidateCurriculumExcelVM.MissingTopicERUs.Add($"Няма добавена Единица резултат от учене (ЕРУ) към темата!->Тема: {curriculum.Topic}");
                                    }
                                }
                            }

                            if (erus.Count != erusFromSpeciality.Count())
                            {
                                var missingErus = erusFromSpeciality.Where(x => erus.All(y => y.IdERU != x.IdERU)).ToList();

                                foreach (var missingEru in missingErus)
                                {
                                    candidateCurriculumExcelVM.MissingDOCERUs.Add($"Единицата резултат от учене (ЕРУ) не е добавена към нито една тема!->ЕРУ: {missingEru.Code}");
                                }
                            }
                        }
                        else
                        {
                            if (this.selectedCandidateProviderSpeciality.Speciality.IdDOC.HasValue)
                            {
                                var docSource = await this.DocService.GetAllActiveDocAsync();
                                var doc = docSource.FirstOrDefault(x => x.IdDOC == this.selectedCandidateProviderSpeciality.Speciality.IdDOC.Value);
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
            }
            finally
            {
                this.loading = false;
                this.SpinnerHide();
            }
        }

        private async Task<bool> IsCurriculumInvalidAsync()
        {
            if (!this.selectedCandidateProviderSpeciality.IdFrameworkProgram.HasValue && !this.selectedCandidateProviderSpeciality.IdFormEducation.HasValue)
            {
                return true;
            }

            if (!this.selectedCandidateProviderSpeciality.IdFrameworkProgram.HasValue)
            {
                return true;
            }

            if (!this.selectedCandidateProviderSpeciality.IdFormEducation.HasValue)
            {
                return true;
            }

            CandidateCurriculumExcelVM candidateCurriculumExcelVM = new CandidateCurriculumExcelVM();
            var professionalTrainingId = (await this.DataSourceService.GetKeyValueByIntCodeAsync("ProfessionalTraining", "B")).IdKeyValue;
            IEnumerable<ERUVM> erusFromDoc = new List<ERUVM>();

            if (this.selectedCandidateProviderSpeciality.Speciality.IdDOC != null)
            {
                erusFromDoc = await this.DocService.GetAllERUsByDocIdAsync(new ERUVM() { IdDOC = this.selectedCandidateProviderSpeciality.Speciality.IdDOC ?? default });
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

            if (this.selectedCandidateProviderSpeciality.Speciality.IdDOC.HasValue)
            {
                var docSource = await this.DocService.GetAllActiveDocAsync();
                var doc = docSource.FirstOrDefault(x => x.IdDOC == this.selectedCandidateProviderSpeciality.Speciality.IdDOC.Value);
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

        // селектира първия ред от грид специалности при зареждане
        private void SelectFirstRowHandler()
        {
            if (!this.firstRender)
            {
                this.firstRender = true;

                if (this.providerSpecialitiesSource.Any())
                {
                    var selectedRows = this.providerSpecialitiesGrid.GetSelectedRecordsAsync().Result.ToList();
                    if (!selectedRows.Any())
                    {
                        this.providerSpecialitiesGrid.SelectRowAsync(0);
                    }
                }
            }
        }

        // проверява дали е натиснат бутон от специалности грида, за да не се маркира целият ред
        private void SpecialityRecordClickHandler(RecordClickEventArgs<CandidateProviderSpecialityVM> args)
        {
            if (args.Column.HeaderText == "ДОС" || args.Column.HeaderText == "")
            {
                this.isSpecialityGridButtonClicked = true;
            }
            else
            {
                this.isSpecialityGridButtonClicked = false;
            }
        }

        // превентва селектирането на ред, ако е натистнат бутон от специалности грида
        private void SpecialitySelectingHandler(RowSelectingEventArgs<CandidateProviderSpecialityVM> args)
        {
            if (this.isSpecialityGridButtonClicked)
            {
                args.Cancel = true;
            }
        }

        // превентва деселектирането на ред, ако е натистнат бутон от специалности грида
        private void SpecialityDeselectingHandler(RowDeselectEventArgs<CandidateProviderSpecialityVM> args)
        {
            if (this.isSpecialityGridButtonClicked)
            {
                args.Cancel = true;
            }
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
                var result = await this.CandidateCurriculumService.DeleteListCandidateCurriculumAsync(selectedCurriculums);
                if (!result.HasErrorMessages)
                {
                    foreach (var curriculum in selectedCurriculums)
                    {
                        this.addedCurriculums.RemoveAll(x => x.IdCandidateCurriculum == curriculum.IdCandidateCurriculum);

                        await this.curriculumsGrid.DeleteRecordAsync("IdSpeciality", curriculum);
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

        protected async Task SpecialitiesToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdf"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;

                ExportProperties.Columns = this.SetGridColumnsForSpecialitiesExport();
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
                ExportProperties.FileName = $"Specialities_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.providerSpecialitiesGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excel"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Specialities_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";

                ExportProperties.Columns = this.SetGridColumnsForSpecialitiesExport();
                ExportProperties.IncludeTemplateColumn = true;
                await this.providerSpecialitiesGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForSpecialitiesExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "Speciality.CodeAndAreaForAutoCompleteSearch", HeaderText = "Специалност", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Speciality.Profession.CodeAndName", HeaderText = "Професия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Speciality.Profession.ProfessionalDirection.DisplayNameAndCode", HeaderText = "ПН", TextAlign = TextAlign.Left });
            if (this.DisableFieldsWhenOpenFromProfile)
            {
                ExportColumns.Add(new GridColumn() { Field = "LicenceDateAsStr", HeaderText = "Дата на лицензиране", TextAlign = TextAlign.Left });
            }

            return ExportColumns;
        }
    }
}
