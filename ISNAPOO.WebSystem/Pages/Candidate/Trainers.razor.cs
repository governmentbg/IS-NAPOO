using System.Net.NetworkInformation;
using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using RegiX;
using RegiX.Class.RDSO.GetDiplomaInfo.Request;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.DocIORenderer;
using Microsoft.EntityFrameworkCore.Query.Internal;
using ISNAPOO.Common.Framework;
using Action = System.Action;
using Syncfusion.Blazor.Navigations;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class Trainers : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderTrainerVM> trainersGrid = new SfGrid<CandidateProviderTrainerVM>();
        private SfGrid<CandidateProviderTrainerProfileVM> trainerProfilesGrid = new SfGrid<CandidateProviderTrainerProfileVM>();
        private SfGrid<CandidateProviderTrainerQualificationVM> trainerQualificationsGrid = new SfGrid<CandidateProviderTrainerQualificationVM>();
        private SfGrid<CandidateProviderTrainerDocumentVM> trainerDocumentsGrid = new SfGrid<CandidateProviderTrainerDocumentVM>();
        public EditContext editContextGeneralData;
        public EditContext editContextTrainerProfile;
        private ValidationMessageStore? messageStore;
        private CandidateProviderTrainerQualificationModal candidateProviderTrainerQualificationModal = new CandidateProviderTrainerQualificationModal();
        private CandidateProviderTrainerDocumentModal candidateProviderTrainerDocumentModal = new CandidateProviderTrainerDocumentModal();
        private CandidateProviderTrainerSpecialityModal candidateProviderTrainerSpecialityModal = new CandidateProviderTrainerSpecialityModal();
        private CandidateProviderTrainerCheckingModal candidateProviderTrainerCheckingModal = new CandidateProviderTrainerCheckingModal();
        private CandidateProviderTrainerSearchModal searchModal = new CandidateProviderTrainerSearchModal();
        private TrainerStatusModal trainerStatusModal = new TrainerStatusModal();
        private RegiXDiplomaCheckModal diplomaCheckModal = new RegiXDiplomaCheckModal();
        private ShowDOCInfoModal docInfoModal = new ShowDOCInfoModal();
        private CandidateProviderTrainerVM candidateProviderTrainerVM = new CandidateProviderTrainerVM();
        private CandidateProviderTrainerProfileVM candidateProviderTrainerProfileVM = new CandidateProviderTrainerProfileVM();
        private List<CandidateProviderTrainerVM> trainersSource = new List<CandidateProviderTrainerVM>();
        private List<CandidateProviderTrainerProfileVM> trainerProfilesSource = new List<CandidateProviderTrainerProfileVM>();
        private List<CandidateProviderTrainerQualificationVM> trainerQualificationsSource = new List<CandidateProviderTrainerQualificationVM>();
        private List<CandidateProviderTrainerDocumentVM> trainerDocumentsSource = new List<CandidateProviderTrainerDocumentVM>();
        private IEnumerable<KeyValueVM> kvIndentTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvSexSource = new List<KeyValueVM>();
        private List<KeyValueVM> kvNationalitySource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvEducationSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvContractTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvQualificationTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvTrainingQualificationTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvDocumentTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvVQSSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvCandidateProviderTrainerStatusSource = new List<KeyValueVM>();
        private KeyValueVM kvEGN = new KeyValueVM();
        private KeyValueVM kvIDN = new KeyValueVM();
        private KeyValueVM kvLNCh = new KeyValueVM();
        private List<ProfessionalDirectionVM> professionalDirectionsSource = new List<ProfessionalDirectionVM>();
        private IEnumerable<ProfessionVM> professionsSource = new List<ProfessionVM>();
        private IEnumerable<SpecialityVM> specialitiesSource = new List<SpecialityVM>();
        private KeyValueVM kvTheory = new KeyValueVM();
        private KeyValueVM kvPractice = new KeyValueVM();
        private KeyValueVM kvTheoryAndPractice = new KeyValueVM();
        private KeyValueVM kvDocComplianceYes = new KeyValueVM();
        private KeyValueVM kvDocComplianceNo = new KeyValueVM();
        private KeyValueVM kvDocCompliancePartial = new KeyValueVM();
        private KeyValueVM kvCandidateProviderTrainerStatusActive = new KeyValueVM();
        private KeyValueVM kvCandidateProviderTrainerStatusInactive = new KeyValueVM();
        private KeyValueVM kvMiddleEducation = new KeyValueVM();
        private int retrainingDiplomaKv;
        private int declarationOfConsentKv;
        private int autobiographyKv;
        private int certificateKv;
        private bool isAddButtonClicked = false;
        private bool isSpecialityGridButtonClicked = false;
        private bool trainerSelectedForSpeciality = false;
        private List<CandidateProviderTrainerVM> selectedTrainers = new List<CandidateProviderTrainerVM>();
        private CandidateProviderTrainerProfileVM candidateProviderTrainerProfileVMToDelete = new CandidateProviderTrainerProfileVM();
        private BasicEGNValidation egnValidator = new BasicEGNValidation(null);
        public double selectedRowIdx = 0;
        private IEnumerable<DocVM> docSource = new List<DocVM>();
        private string identType = "ЕГН";
        private bool isExpanded = false;
        private bool isAccordionExpanded = true;
        private string trainerNameInformation = string.Empty;
        private bool disableNextBtn = false;
        private bool disablePreviousBtn = false;
        private bool enabled = false;

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool IsCPO { get; set; }

        [Parameter]
        public SfTab TabReference { get; set; }

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
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IRegiXService RegiXService { get; set; }

        [Inject]
        public IDOCService DOCService { get; set; }

        protected override void OnInitialized()
        {
            this.candidateProviderTrainerVM = new CandidateProviderTrainerVM();
            this.FormTitle = "Преподаватели";

            this.editContextGeneralData = new EditContext(this.candidateProviderTrainerVM);

            if (this.IsCPO)
            {
                this.editContextTrainerProfile = new EditContext(this.candidateProviderTrainerProfileVM);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await this.LoadDataAsync();

                this.editContextGeneralData.MarkAsUnmodified();

                if (this.IsCPO)
                {
                    this.editContextTrainerProfile.MarkAsUnmodified();
                }

                this.StateHasChanged();
            }
        }

        private async Task LoadDataAsync()
        {
            this.trainersSource = (await this.CandidateProviderService.GetCandidateProviderTrainersWithStatusByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider)).ToList();

            this.kvEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Education");
            this.kvMiddleEducation = this.kvEducationSource.FirstOrDefault(x => x.KeyValueIntCode == "Education13");

            if (this.IsCPO)
            {
                this.professionalDirectionsSource = this.DataSourceService.GetAllProfessionalDirectionsList().Where(x => x.IdStatus == this.DataSourceService.GetActiveStatusID()).ToList();
                this.professionsSource = this.DataSourceService.GetAllProfessionsList();
                this.specialitiesSource = this.DataSourceService.GetAllSpecialitiesList();
                this.kvQualificationTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("QualificationType");
                this.kvVQSSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                this.kvTheory = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TheoryTraining");
                this.kvPractice = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "PracticalTraining");
                this.kvTheoryAndPractice = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TrainingInTheoryAndPractice");
                this.kvCandidateProviderTrainerStatusActive = await DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");
                this.kvDocComplianceYes = await DataSourceService.GetKeyValueByIntCodeAsync("ComplianceDOC", "Does");
                this.kvDocComplianceNo = await DataSourceService.GetKeyValueByIntCodeAsync("ComplianceDOC", "Doesnt");
                this.kvDocCompliancePartial = await DataSourceService.GetKeyValueByIntCodeAsync("ComplianceDOC", "Partial");
                this.docSource = await this.DOCService.GetAllActiveDocAsync();
            }

            this.kvCandidateProviderTrainerStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderTrainerStatus");
            this.kvCandidateProviderTrainerStatusInactive = await this.DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Inactive");

            this.kvTrainingQualificationTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingQualificationType");
            this.kvDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerDocumentType");
            this.certificateKv = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Certificate").IdKeyValue;
            this.autobiographyKv = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Autobiography").IdKeyValue;
            this.declarationOfConsentKv = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DeclarationOfConsent").IdKeyValue;
            this.retrainingDiplomaKv = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "RetrainingDiploma").IdKeyValue;
            this.kvIndentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            this.kvSexSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");

            await this.HandleOrderForNationalitiesSourceAsync();

            this.kvContractTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerContractType");

            this.kvEGN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
            this.kvIDN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "IDN");
            this.kvLNCh = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "LNK");

            this.candidateProviderTrainerVM.IdStatus = this.kvCandidateProviderTrainerStatusActive.IdKeyValue;

            await this.trainersGrid.Refresh();
        }

        private async Task HandleOrderForNationalitiesSourceAsync()
        {
            this.kvNationalitySource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Nationality")).OrderBy(x => x.Name).ToList();
            var withoutNacionality = this.kvNationalitySource.FirstOrDefault(x => x.Name == "Без гражданство");
            this.kvNationalitySource.Remove(withoutNacionality);
            this.kvNationalitySource.Add(withoutNacionality);
            var bgNationality = this.kvNationalitySource.FirstOrDefault(x => x.Name == "България");
            this.kvNationalitySource.Remove(bgNationality);
            this.kvNationalitySource.Insert(0, bgNationality);
            this.kvNationalitySource.RemoveAll(x => x.Name == "");
        }

        private void IdentValueChangedHandler(ChangeEventArgs<int?, KeyValueVM> args)
        {
            if (args.Value.HasValue)
            {
                if (args.Value == this.kvEGN.IdKeyValue)
                {
                    this.identType = "ЕГН";
                    this.enabled = false;
                }
                else if (args.Value == this.kvLNCh.IdKeyValue)
                {
                    this.identType = "ЛНЧ";
                    this.enabled = true;
                }
                else
                {
                    this.identType = "ИДН";
                    this.enabled = true;
                }
            }
            else
            {
                this.identType = "ЕГН";
                this.enabled = true;
            }
        }

        private async Task OpenFilterModalBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.searchModal.OpenModal(this.CandidateProviderVM, this.IsCPO);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void FilterApply(List<CandidateProviderTrainerVM> candidateProviderTrainerVMs)
        {
            this.trainersSource = candidateProviderTrainerVMs.OrderBy(x => x.FullName).ToList();
            if (!this.trainersGrid.SelectedRecords.Any(x => this.trainersSource.Any(y => y.IdCandidateProviderTrainer == x.IdCandidateProviderTrainer)))
            {
                this.candidateProviderTrainerVM = new CandidateProviderTrainerVM();

                this.trainerDocumentsSource.Clear();
                this.trainerProfilesSource.Clear();
                this.trainerQualificationsSource.Clear();
            }

            this.StateHasChanged();
        }

        private async Task AddProfessionalDirectionHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (this.candidateProviderTrainerProfileVM.IdProfessionalDirection == 0)
                {
                    await this.ShowErrorAsync("Моля, изберете професионално направление!");
                }
                else
                {
                    var result = new ResultContext<CandidateProviderTrainerProfileVM>();
                    this.candidateProviderTrainerProfileVM.IdCandidateProviderTrainer = this.candidateProviderTrainerVM.IdCandidateProviderTrainer;
                    result.ResultContextObject = this.candidateProviderTrainerProfileVM;
                    result = await this.CandidateProviderService.CreateCandidateProviderTrainerProfileAsync(result);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        this.candidateProviderTrainerProfileVM = result.ResultContextObject;
                        var professionalDirection = this.professionalDirectionsSource.FirstOrDefault(x => x.IdProfessionalDirection == this.candidateProviderTrainerProfileVM.IdProfessionalDirection);
                        this.candidateProviderTrainerProfileVM.ProfessionalDirectionCodeAndName = professionalDirection.DisplayNameAndCode;
                        this.trainerProfilesSource.Add(this.candidateProviderTrainerProfileVM);

                        this.professionalDirectionsSource.RemoveAll(x => x.IdProfessionalDirection == this.candidateProviderTrainerProfileVM.IdProfessionalDirection);

                        this.candidateProviderTrainerProfileVM = new CandidateProviderTrainerProfileVM();

                        await this.trainerProfilesGrid.Refresh();
                        this.StateHasChanged();
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task AddNewTrainerClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            if (this.editContextGeneralData.IsModified() || this.editContextTrainerProfile.IsModified())
            {
                string msg = "Имате незапазени промени! Сигурни ли сте, че искате да продължите?";
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
                        this.isExpanded = true;


                        this.isAddButtonClicked = true;

                        this.ResetTrainerGridData();
                        if (this.IsCPO)
                        {
                            await this.trainersGrid.CollapseAllDetailRowAsync();
                        }

                        await this.trainersGrid.ClearRowSelectionAsync();

                        this.trainerNameInformation = string.Empty;
                    }
                    finally
                    {
                        this.loading = false;
                    }
                }
            }
            else
            {
                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;
                    this.isExpanded = true;


                    this.isAddButtonClicked = true;

                    this.ResetTrainerGridData();
                    if (this.IsCPO)
                    {
                        await this.trainersGrid.CollapseAllDetailRowAsync();
                    }

                    await this.trainersGrid.ClearRowSelectionAsync();

                    this.trainerNameInformation = string.Empty;
                }
                finally
                {
                    this.loading = false;
                }
            }

            this.StateHasChanged();
        }

        private void ResetTrainerGridData()
        {
            this.candidateProviderTrainerVM = new CandidateProviderTrainerVM()
            {
                IdStatus = this.kvCandidateProviderTrainerStatusActive.IdKeyValue,
                IdIndentType = this.kvEGN.IdKeyValue
            };

            this.candidateProviderTrainerProfileVM = new CandidateProviderTrainerProfileVM();

            if (this.IsCPO)
            {
                if (this.trainerProfilesSource.Any())
                {
                    this.trainerProfilesSource.Clear();
                }

                if (this.trainerQualificationsSource.Any())
                {
                    this.trainerQualificationsSource.Clear();
                }
            }

            if (this.trainerDocumentsSource.Any())
            {
                this.trainerDocumentsSource.Clear();
            }

            this.StateHasChanged();
        }

        public new async Task SubmitHandler()
        {
            this.editContextGeneralData = new EditContext(this.candidateProviderTrainerVM);

            //Ако е избрано ЕГН проверяваме дали е валидно
            if (this.kvEGN.IdKeyValue == this.candidateProviderTrainerVM.IdIndentType)
            {
                this.editContextGeneralData.OnValidationRequested += this.ValidateEGN;
                this.messageStore = new ValidationMessageStore(this.editContextGeneralData);
            }

            if (!string.IsNullOrEmpty(this.candidateProviderTrainerVM.Indent))
            {
                this.editContextGeneralData.OnValidationRequested += this.CheckForOtherTrainerWithSameIndent;
                this.messageStore = new ValidationMessageStore(this.editContextGeneralData);
            }

            this.messageStore = new ValidationMessageStore(this.editContextGeneralData);
            this.editContextGeneralData.OnValidationRequested += this.ValidateSPKValue;

            this.editContextGeneralData.EnableDataAnnotationsValidation();

            if (this.IsCPO)
            {
                this.editContextTrainerProfile = new EditContext(this.candidateProviderTrainerProfileVM);
                this.editContextTrainerProfile.EnableDataAnnotationsValidation();
            }

            var isValidEditContextGeneralData = this.editContextGeneralData.Validate();
            if (isValidEditContextGeneralData)
            {
                if (this.candidateProviderTrainerVM.IdCandidateProviderTrainer == 0)
                {
                    this.candidateProviderTrainerVM.IdCandidate_Provider = this.CandidateProviderVM.IdCandidate_Provider;
                    await this.CandidateProviderService.CreateCandidateProviderTrainerAsync(this.candidateProviderTrainerVM);

                    // селектира и добавя нов преподавател/консултант към списъка
                    await this.HandleSelectAndOrderFunctionalityAfterAddNewTrainerAsync();
                }
                else
                {
                    await this.CandidateProviderService.UpdateCandidateProviderTrainerAsync(this.candidateProviderTrainerVM);

                    // ъпдейтва данните на преподавателя/консултанта в грида след интеракция с БД
                    await this.HandleUpdateTrainersDataInGridAfterUpdateAsync();
                }
            }
        }

        // ъпдейтва данните на преподавателя/консултанта в грида след интеракция с БД
        private async Task HandleUpdateTrainersDataInGridAfterUpdateAsync()
        {
            var idxOfUpdatedTrainer = this.trainersSource.FindIndex(x => x.IdCandidateProviderTrainer == this.candidateProviderTrainerVM.IdCandidateProviderTrainer);
            if (idxOfUpdatedTrainer != -1)
            {
                this.editContextGeneralData.MarkAsUnmodified();
                if (this.IsCPO)
                {
                    this.editContextTrainerProfile.MarkAsUnmodified();
                }

                //if (this.TabReference.Items[this.TabReference.SelectedItem].Header.Text == "Преподаватели" || this.TabReference.Items[this.TabReference.SelectedItem].Header.Text == "Консултанти")
                //{
                //    await this.trainersGrid.SetRowDataAsync(this.candidateProviderTrainerVM.IdCandidateProviderTrainer, this.candidateProviderTrainerVM);
                //}

                try
                {
                    await this.trainersGrid.SetRowDataAsync(this.candidateProviderTrainerVM.IdCandidateProviderTrainer, this.candidateProviderTrainerVM);
                }
                catch (Exception ex) { }

                this.StateHasChanged();
            }
        }

        // селектира и добавя нов преподавател/консултант към списъка
        private async Task HandleSelectAndOrderFunctionalityAfterAddNewTrainerAsync()
        {
            this.trainersSource.Add(this.candidateProviderTrainerVM);
            this.trainersSource = this.trainersSource.OrderBy(x => x.FirstName).ThenBy(x => x.SecondName).ThenBy(x => x.FamilyName).ToList();
            await this.trainersGrid.Refresh();

            this.editContextGeneralData.MarkAsUnmodified();
            if (this.IsCPO)
            {
                this.editContextTrainerProfile.MarkAsUnmodified();
            }

            this.StateHasChanged();

            var trainerIdx = await this.trainersGrid.GetRowIndexByPrimaryKeyAsync(this.candidateProviderTrainerVM.IdCandidateProviderTrainer);
            if (trainerIdx != -1)
            {
                await this.trainersGrid.SelectRowAsync(trainerIdx);
                if (this.IsCPO)
                {
                    await this.trainersGrid.ExpandCollapseDetailRowAsync(this.candidateProviderTrainerVM);
                }
            }
        }

        private async Task ChangeTrainer(int id)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                if (id != 0)
                {
                    this.candidateProviderTrainerVM = this.IsCPO
                        ? await this.CandidateProviderService.GetCandidateProviderTrainerByIdAsync(new CandidateProviderTrainerVM() { IdCandidateProviderTrainer = id })
                        : await this.CandidateProviderService.GetCandidateProviderTrainerWithDocumentsByIdAsync(new CandidateProviderTrainerVM() { IdCandidateProviderTrainer = id });

                    var professionalDirectionAll = this.DataSourceService.GetAllProfessionalDirectionsList();
                    
                    if (this.candidateProviderTrainerVM is not null)
                    {
                        var trainerIdx = this.trainersSource.FindIndex(x => x.IdCandidateProviderTrainer == this.candidateProviderTrainerVM.IdCandidateProviderTrainer);
                        if (trainerIdx != -1)
                        {
                            this.trainersSource[trainerIdx] = this.candidateProviderTrainerVM;
                        }

                        if (this.IsCPO)
                        {
                            this.trainerProfilesSource = this.candidateProviderTrainerVM.CandidateProviderTrainerProfiles.ToList();
                            foreach (var profile in this.trainerProfilesSource.ToList())
                            {
                                var professionalDirection = professionalDirectionAll.FirstOrDefault(x => x.IdProfessionalDirection == profile.IdProfessionalDirection);
                                if (professionalDirection != null)
                                {
                                    profile.ProfessionalDirectionCodeAndName = professionalDirection.DisplayNameAndCode;
                                }
                                else
                                {
                                    this.trainerProfilesSource.Remove(profile);
                                }
                            }

                            this.trainerQualificationsSource = this.candidateProviderTrainerVM.CandidateProviderTrainerQualifications.ToList();
                            foreach (var qualification in trainerQualificationsSource)
                            {
                                if (qualification.IdQualificationType != 0)
                                {
                                    qualification.QualificationTypeName = this.kvQualificationTypeSource.FirstOrDefault(x => x.IdKeyValue == qualification.IdQualificationType).Name;
                                }

                                if (qualification.IdTrainingQualificationType != 0)
                                {
                                    qualification.TrainingQualificationTypeName = this.kvTrainingQualificationTypeSource.FirstOrDefault(x => x.IdKeyValue == qualification.IdTrainingQualificationType).Name;
                                }

                                var profession = this.professionsSource.FirstOrDefault(x => x.IdProfession == qualification.IdProfession);
                                if (profession != null)
                                {
                                    qualification.ProfessionName = profession.CodeAndName;
                                }
                            }
                        }

                        this.trainerDocumentsSource = this.candidateProviderTrainerVM.CandidateProviderTrainerDocuments.ToList();
                        foreach (var document in this.trainerDocumentsSource)
                        {
                            var documentName = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType);
                            if (documentName is not null)
                            {
                                document.DocumentTypeName = documentName.Name;
                            }

                            document.UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);

                            if (document.HasUploadedFile)
                            {
                                await this.SetFileNameAsync(document);
                            }
                        }

                        if (this.IsCPO)
                        {
                            foreach (var trainerSpeciality in this.candidateProviderTrainerVM.CandidateProviderTrainerSpecialities)
                            {
                                var speciality = this.specialitiesSource.FirstOrDefault(x => x.IdSpeciality == trainerSpeciality.IdSpeciality);
                                this.candidateProviderTrainerVM.SelectedSpecialities.Add(speciality);
                            }

                            await this.SetProfessionalDirectionSourceAsync();
                        }

                        // проверява за последен/пръв запис в грида
                        var idx = await this.trainersGrid.GetRowIndexByPrimaryKeyAsync(this.candidateProviderTrainerVM.IdCandidateProviderTrainer);
                        if (idx + 1 == this.trainersSource.Count)
                        {
                            this.disableNextBtn = true;
                            this.disablePreviousBtn = false;
                        }
                        else if (idx == 0)
                        {
                            this.disablePreviousBtn = true;
                            this.disableNextBtn = false;
                        }
                        else
                        {
                            this.disableNextBtn = false;
                            this.disablePreviousBtn = false;
                        }
                    }

                    this.trainerNameInformation = this.candidateProviderTrainerVM.FullName;
                }

                this.editContextGeneralData.MarkAsUnmodified();
                if (this.IsCPO)
                {
                    this.editContextTrainerProfile.MarkAsUnmodified();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task SetProfessionalDirectionSourceAsync()
        {
            var addedSpecialities = await this.CandidateProviderService.GetProviderSpecialitiesWithProfessionIncludedByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider);
            if (addedSpecialities.Any())
            {
                var professionalDirectionIds = addedSpecialities.Select(x => x.Speciality.Profession.IdProfessionalDirection).Distinct().ToList();
                this.professionalDirectionsSource = this.DataSourceService.GetAllProfessionalDirectionsList().Where(x => x.IdStatus == this.DataSourceService.GetActiveStatusID()).ToList();
                this.professionalDirectionsSource = this.professionalDirectionsSource.Where(x => professionalDirectionIds.Contains(x.IdProfessionalDirection)).ToList();
                if (this.trainerProfilesSource.Any())
                {
                    foreach (var profile in this.trainerProfilesSource)
                    {
                        this.professionalDirectionsSource.RemoveAll(x => x.IdProfessionalDirection == profile.IdProfessionalDirection);
                    }
                }
            }
            else
            {
                this.professionalDirectionsSource.Clear();
            }

            this.StateHasChanged();
        }

        private void TrainerRecordClickHandler(RecordClickEventArgs<CandidateProviderTrainerVM> args)
        {
            if (args.Column.HeaderText == "Специалности")
            {
                this.isSpecialityGridButtonClicked = true;
            }
            else
            {
                this.isSpecialityGridButtonClicked = false;
            }
        }

        private async Task TrainerSelectingHandler(RowSelectingEventArgs<CandidateProviderTrainerVM> args)
        {
            if (this.IsCPO)
            {
                if (!this.editContextGeneralData.IsModified() && !this.editContextTrainerProfile.IsModified())
                {
                    if (!this.isExpanded)
                    {
                        this.isExpanded = true;
                        await this.trainersGrid.CollapseAllDetailRowAsync();
                        await this.ChangeTrainer(args.Data.IdCandidateProviderTrainer);
                        await this.trainersGrid.ExpandCollapseDetailRowAsync(args.Data);
                    }
                }
                else
                {
                    string msg = "Имате незапазени промени! Сигурни ли сте, че искате да смените преподавателя?";
                    bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
                    if (isConfirmed)
                    {
                        if (!this.isExpanded)
                        {
                            this.isExpanded = true;
                            await this.trainersGrid.CollapseAllDetailRowAsync();
                            await this.ChangeTrainer(args.Data.IdCandidateProviderTrainer);
                            await this.trainersGrid.ExpandCollapseDetailRowAsync(args.Data);
                        }
                    }
                    else
                    {
                        args.Cancel = true;
                    }
                }

                this.isExpanded = false;
            }
            else
            {
                if (!this.editContextGeneralData.IsModified())
                {
                    await this.ChangeTrainer(args.Data.IdCandidateProviderTrainer);
                }
                else
                {
                    string msg = "Имате незапазени промени! Сигурни ли сте, че искате да смените консултанта?";
                    bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
                    if (isConfirmed)
                    {
                        await this.ChangeTrainer(args.Data.IdCandidateProviderTrainer);
                    }
                    else
                    {
                        args.Cancel = true;
                    }
                }
            }
        }

        private async Task AddNewQualificationClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var trainerFullname = $"{this.candidateProviderTrainerVM.FirstName} {this.candidateProviderTrainerVM.FamilyName}";
                await this.candidateProviderTrainerQualificationModal.OpenModal(new CandidateProviderTrainerQualificationVM() { IdCandidateProviderTrainer = this.candidateProviderTrainerVM.IdCandidateProviderTrainer }, this.kvTrainingQualificationTypeSource, this.kvQualificationTypeSource, trainerFullname, this.trainerQualificationsSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnQualificationModalSubmit(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM)
        {
            this.trainerQualificationsSource = (await this.CandidateProviderService.GetAllCandidateProviderTrainerQualificationsByCandidateProviderTrainerIdAsync(new CandidateProviderTrainerQualificationVM() { IdCandidateProviderTrainer = candidateProviderTrainerQualificationVM.IdCandidateProviderTrainer })).ToList();
            foreach (var qualification in trainerQualificationsSource)
            {
                if (qualification.IdProfession != null)
                {
                    qualification.ProfessionName = !String.IsNullOrEmpty(this.professionsSource.FirstOrDefault(x => x.IdProfession == qualification.IdProfession)?.CodeAndName) ? this.professionsSource.FirstOrDefault(x => x.IdProfession == qualification.IdProfession)?.CodeAndName : "";
                }
                if (qualification.IdQualificationType != 0)
                {
                    qualification.QualificationTypeName = !String.IsNullOrEmpty(this.kvQualificationTypeSource.FirstOrDefault(x => x.IdKeyValue == qualification.IdQualificationType)?.Name) ? this.kvQualificationTypeSource.FirstOrDefault(x => x.IdKeyValue == qualification.IdQualificationType)?.Name : "";
                }
                if (qualification.IdTrainingQualificationType != 0)
                {
                    qualification.TrainingQualificationTypeName = !String.IsNullOrEmpty(this.kvTrainingQualificationTypeSource.FirstOrDefault(x => x.IdKeyValue == qualification.IdTrainingQualificationType)?.Name) ? this.kvTrainingQualificationTypeSource.FirstOrDefault(x => x.IdKeyValue == qualification.IdTrainingQualificationType)?.Name : "";
                }
            }

            this.StateHasChanged();
        }

        private void OnCheckingModalSubmit(CandidateProviderTrainerCheckingVM candidateProviderTrainerCheckingVM)
        {
            this.trainersSource
                .FirstOrDefault(c => c.IdCandidateProviderTrainer == candidateProviderTrainerCheckingVM.IdCandidateProviderTrainer)
                .CandidateProviderTrainerCheckings.Add(candidateProviderTrainerCheckingVM);

            this.StateHasChanged();
        }

        private async Task DeleteQualificationBtn(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
            if (isConfirmed)
            {
                var resultContext = await this.CandidateProviderService.DeleteCandidateProviderTrainerQualificationAsync(candidateProviderTrainerQualificationVM);
                if (resultContext.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                    this.trainerQualificationsSource = (await this.CandidateProviderService.GetAllCandidateProviderTrainerQualificationsByCandidateProviderTrainerIdAsync(new CandidateProviderTrainerQualificationVM() { IdCandidateProviderTrainer = candidateProviderTrainerQualificationVM.IdCandidateProviderTrainer })).ToList();
                    foreach (var qualification in this.trainerQualificationsSource)
                    {
                        var profession = this.professionsSource.FirstOrDefault(x => x.IdProfession == qualification.IdProfession);
                        if (profession is not null)
                        {
                            qualification.ProfessionName = profession.CodeAndName;
                        }

                        var qualificationType = this.kvQualificationTypeSource.FirstOrDefault(x => x.IdKeyValue == qualification.IdQualificationType);
                        if (qualificationType is not null)
                        {
                            qualification.QualificationTypeName = qualificationType.Name;
                        }

                        var trainingQualificationType = this.kvTrainingQualificationTypeSource.FirstOrDefault(x => x.IdKeyValue == qualification.IdTrainingQualificationType);
                        if (trainingQualificationType is not null)
                        {
                            qualification.TrainingQualificationTypeName = trainingQualificationType.Name;
                        }
                    }

                    this.StateHasChanged();
                }
            }
        }

        private async Task EditQualificationBtn(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM)
        {
            bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var trainerFullname = $"{this.candidateProviderTrainerVM.FirstName} {this.candidateProviderTrainerVM.FamilyName}";
                await this.candidateProviderTrainerQualificationModal.OpenModal(candidateProviderTrainerQualificationVM, this.kvTrainingQualificationTypeSource, this.kvQualificationTypeSource, trainerFullname, this.trainerQualificationsSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task AddNewDocumentClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.candidateProviderTrainerDocumentModal.OpenModal(new CandidateProviderTrainerDocumentVM() { IdCandidateProviderTrainer = this.candidateProviderTrainerVM.IdCandidateProviderTrainer }, this.kvDocumentTypeSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteDocumentBtn(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM)
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

                    var resultContext = await this.CandidateProviderService.DeleteCandidateProviderTrainerDocumentAsync(candidateProviderTrainerDocumentVM);
                    if (resultContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                        this.trainerDocumentsSource = (await this.CandidateProviderService.GetAllCandidateProviderTrainerDocumentsByCandidateProviderTrainerIdAsync(new CandidateProviderTrainerDocumentVM() { IdCandidateProviderTrainer = candidateProviderTrainerDocumentVM.IdCandidateProviderTrainer })).ToList();
                        foreach (var document in trainerDocumentsSource)
                        {
                            document.DocumentTypeName = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType).Name;
                            document.UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);

                            if (document.HasUploadedFile)
                            {
                                await this.SetFileNameAsync(document);
                            }
                        }

                        await this.trainerDocumentsGrid.Refresh();
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

        private async Task OnDocumentModalSubmit(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM)
        {
            this.trainerDocumentsSource = (await this.CandidateProviderService.GetAllCandidateProviderTrainerDocumentsByCandidateProviderTrainerIdAsync(new CandidateProviderTrainerDocumentVM() { IdCandidateProviderTrainer = candidateProviderTrainerDocumentVM.IdCandidateProviderTrainer })).ToList();
            foreach (var document in trainerDocumentsSource)
            {
                document.DocumentTypeName = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType).Name;
                document.UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);

                if (document.HasUploadedFile)
                {
                    await this.SetFileNameAsync(document);
                }
            }

            await this.trainerDocumentsGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task OnDownloadClick(string fileName, CandidateProviderTrainerDocumentVM candidateProviderTrainerDocument)
        {
            bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderTrainerDocument>(candidateProviderTrainerDocument.IdCandidateProviderTrainerDocument, fileName);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderTrainerDocument>(candidateProviderTrainerDocument.IdCandidateProviderTrainerDocument, fileName);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, fileName, documentStream.MS!.ToArray());
                    }
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload");

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteChecking(CandidateProviderTrainerCheckingVM checking)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            var result = await this.CandidateProviderService.DeleteCandidateProviderTrainerCheckingAsync(checking);
            if (result.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
            }
            else
            {
                await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                this.trainersSource.First(x => x.IdCandidateProviderTrainer == checking.IdCandidateProviderTrainer).CandidateProviderTrainerCheckings.Remove(checking);
            }

            this.SpinnerHide();
        }

        private async Task DeleteSpecialityBtn(int idSpeciality, int idCandidateProviderTrainer, int idUsage)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
            if (isConfirmed)
            {
                this.SpinnerShow();

                var trainer = this.trainersSource.FirstOrDefault(x => x.IdCandidateProviderTrainer == idCandidateProviderTrainer);
                var speciality = trainer.SelectedSpecialities.FirstOrDefault(x => x.IdSpeciality == idSpeciality);
                trainer.SelectedSpecialities.Remove(speciality);
                this.candidateProviderTrainerVM.SelectedSpecialities.Remove(speciality);

                var candidateProviderTrainerSpeciality = await this.CandidateProviderService.GetCandidateProviderSpecialityByIdSpecialityAndIdCandidateProviderTrainerAsync(idSpeciality, idCandidateProviderTrainer, idUsage);
                if (candidateProviderTrainerSpeciality is not null)
                {
                    var resultContext = await this.CandidateProviderService.DeleteCandidateProviderTrainerSpecialityAsync(candidateProviderTrainerSpeciality, idUsage);

                    if (resultContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                        this.trainersSource.First(x => x.IdCandidateProviderTrainer == idCandidateProviderTrainer).CandidateProviderTrainerSpecialities.Remove(this.trainersSource.First(x => x.IdCandidateProviderTrainer == idCandidateProviderTrainer).CandidateProviderTrainerSpecialities.First(x => x.IdSpeciality == speciality.IdSpeciality));
                    }
                }

                this.SpinnerHide();
                this.StateHasChanged();
            }
        }

        private async Task AddCheckingToTrainersClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (this.candidateProviderTrainerVM.IdCandidateProviderTrainer == 0)
                {
                    await this.ShowErrorAsync("Моля, изберете преподавател/и от списъка!");
                }
                else
                {
                    CandidateProviderTrainerCheckingVM candidateProviderTrainerCheckingVM = new CandidateProviderTrainerCheckingVM();
                    candidateProviderTrainerCheckingVM.IdCandidateProviderTrainer = this.candidateProviderTrainerVM.IdCandidateProviderTrainer;
                    await this.candidateProviderTrainerCheckingModal.OpenModal(this.selectedTrainers, candidateProviderTrainerCheckingVM);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task AddSpecialitiesToTrainersClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (this.candidateProviderTrainerVM.IdCandidateProviderTrainer == 0)
                {
                    await this.ShowErrorAsync("Моля, изберете преподавател/и от списъка!");
                }
                else
                {
                    var providerSpecialities = await this.CandidateProviderService.GetProviderSpecialitiesWithoutIncludesByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider);
                    var trainerFromSource = this.trainersSource.FirstOrDefault(x => x.IdCandidateProviderTrainer == this.candidateProviderTrainerVM.IdCandidateProviderTrainer)!;
                    await this.candidateProviderTrainerSpecialityModal.OpenModal(trainerFromSource, providerSpecialities.ToList());
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnTrainerSpecialityModalSubmit(int idCandidateProviderTrainer)
        {
            var trainerIdx = await this.trainersGrid.GetRowIndexByPrimaryKeyAsync(idCandidateProviderTrainer);

            await this.trainersGrid.SelectRowAsync(trainerIdx);
        }

        private async Task DeleteProfessionalDirectionBtn(CandidateProviderTrainerProfileVM candidateProviderTrainerProfileVM)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var professions = this.professionsSource.Where(x => x.IdProfessionalDirection == candidateProviderTrainerProfileVM.IdProfessionalDirection);
                var professionIds = professions.Select(x => x.IdProfession).ToList();
                var specialities = this.specialitiesSource.Where(x => professionIds.Contains(x.IdProfession)).ToList();
                var doesSpecialityExist = false;
                var trainer = this.trainersSource.FirstOrDefault(x => x.IdCandidateProviderTrainer == candidateProviderTrainerProfileVM.IdCandidateProviderTrainer);
                if (trainer is not null)
                {
                    if (trainer.CandidateProviderTrainerSpecialities.Any(x => specialities.Any(y => y.IdSpeciality == x.IdSpeciality)))
                    {
                        doesSpecialityExist = true;
                    }
                }

                if (doesSpecialityExist)
                {
                    await this.ShowErrorAsync("Не можете да изтриете професионално направление, към което има добавена специалност!");
                }
                else
                {
                    this.candidateProviderTrainerProfileVMToDelete = candidateProviderTrainerProfileVM;
                    string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
                    bool isConfirmed = await this.ShowConfirmDialogAsync(msg);

                    if (isConfirmed)
                    {
                        await this.CandidateProviderService.DeleteCandidateProviderTrainerProfileAsync(this.candidateProviderTrainerProfileVMToDelete);
                        this.trainerProfilesSource.Remove(this.candidateProviderTrainerProfileVMToDelete);

                        await this.ShowSuccessAsync("Професионалното направление е изтрито успешно!");

                        await this.trainerProfilesGrid.Refresh();
                        this.StateHasChanged();
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void IndentChanged(Microsoft.AspNetCore.Components.ChangeEventArgs args)
        {
            var indent = this.candidateProviderTrainerVM.Indent;
            if (indent != null)
            {
                if (this.candidateProviderTrainerVM.IdIndentType == this.kvEGN.IdKeyValue)
                {
                    indent = indent.Trim();

                    var checkEGN = new BasicEGNValidation(indent);

                    if (checkEGN.Validate())
                    {
                        char charLastDigit = indent[indent.Length - 2];
                        int lastDigit = Convert.ToInt32(new string(charLastDigit, 1));
                        int year = int.Parse(indent.Substring(0, 2));
                        int month = int.Parse(indent.Substring(2, 2));
                        int day = int.Parse(indent.Substring(4, 2));
                        if (month < 13)
                        {
                            year += 1900;
                        }
                        else if (month > 20 && month < 33)
                        {
                            year += 1800;
                            month -= 20;
                        }
                        else if (month > 40 && month < 53)
                        {
                            year += 2000;
                            month -= 40;
                        }
                        var BirthDate = new DateTime(year, month, day);

                        this.candidateProviderTrainerVM.BirthDate = BirthDate;

                        var beforeLastNumber = int.Parse(indent.Substring(indent.Length - 2, 1));

                        if (beforeLastNumber % 2 == 0)
                        {
                            this.candidateProviderTrainerVM.IdSex = this.kvSexSource.FirstOrDefault(x => x.Name == "Мъж").IdKeyValue;
                        }
                        else
                        {
                            this.candidateProviderTrainerVM.IdSex = this.kvSexSource.FirstOrDefault(x => x.Name == "Жена").IdKeyValue;
                        }
                    }
                    else
                    {
                        this.candidateProviderTrainerVM.BirthDate = null;
                        this.candidateProviderTrainerVM.IdSex = null;
                    }
                }
                else
                {
                    this.candidateProviderTrainerVM.BirthDate = null;
                    this.candidateProviderTrainerVM.IdSex = null;
                }
            }
        }

        private void ValidateEGN(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.candidateProviderTrainerVM.Indent != null)
            {
                this.candidateProviderTrainerVM.Indent = this.candidateProviderTrainerVM.Indent.Trim();

                if (this.candidateProviderTrainerVM.Indent.Length != 10)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.candidateProviderTrainerVM, "Indent");
                    this.messageStore?.Add(fi, "Полето 'ЕГН/ЛНЧ/ИДН' трябва да съдържа 10 символа!");
                }
                else
                {
                    var checkEGN = new BasicEGNValidation(this.candidateProviderTrainerVM.Indent);

                    if (!checkEGN.Validate())
                    {
                        FieldIdentifier fi = new FieldIdentifier(this.candidateProviderTrainerVM, "Indent");
                        this.messageStore?.Add(fi, checkEGN.ErrorMessage + (this.IsCPO ? " (Преподаватели)" : " (Консултанти)"));
                    }
                }
            }
        }

        private void CheckForOtherTrainerWithSameIndent(object? sender, ValidationRequestedEventArgs args)
        {
            var indentType = this.kvIndentTypeSource.FirstOrDefault(x => x.IdKeyValue == this.candidateProviderTrainerVM.IdIndentType);
            if (this.CandidateProviderVM.CandidateProviderTrainers.Any(x => x.Indent == this.candidateProviderTrainerVM.Indent && x.IdCandidateProviderTrainer != this.candidateProviderTrainerVM.IdCandidateProviderTrainer))
            {
                var trainer = this.IsCPO ? "Преподавател" : "Консултант";
                FieldIdentifier fi = new FieldIdentifier(this.candidateProviderTrainerVM, "Indent");
                this.messageStore?.Add(fi, $"{trainer} с това {indentType.Name} вече е въведен в информационната система!");
            }
        }

        private void ValidateSPKValue(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.candidateProviderTrainerVM.IdEducation == this.kvMiddleEducation.IdKeyValue)
            {
                if (string.IsNullOrEmpty(this.candidateProviderTrainerVM.ProfessionalQualificationCertificate))
                {
                    FieldIdentifier fi = new FieldIdentifier(this.candidateProviderTrainerVM, "ProfessionalQualificationCertificate");
                    this.messageStore?.Add(fi, "Полето 'Свидетелство за професионална квалификация' е задължително!");
                }
            }
        }

        private async Task SetFileNameAsync(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocument)
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var fileFullName = settingResource + "\\" + candidateProviderTrainerDocument.UploadedFileName;
            if (Directory.Exists(fileFullName))
            {
                var files = Directory.GetFiles(fileFullName);
                files = files.Select(x => x.Split(($"\\{candidateProviderTrainerDocument.IdCandidateProviderTrainerDocument}\\"), StringSplitOptions.RemoveEmptyEntries).LastOrDefault()).ToArray();
                candidateProviderTrainerDocument.FileName = string.Join(Environment.NewLine, files);
            }
            else
            {
                candidateProviderTrainerDocument.FileName = string.Empty;
            }
        }

        private void QueryCellInfo(QueryCellInfoEventArgs<CandidateProviderTrainerVM> args)
        {
            if (!this.DisableFieldsWhenOpenFromProfile)
            {
                var trainer = this.trainersSource.FirstOrDefault(x => x.IdCandidateProviderTrainer == args.Data.IdCandidateProviderTrainer && x.IdStatus == this.kvCandidateProviderTrainerStatusActive.IdKeyValue);
                if (trainer is not null)
                {
                    //var certificate = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == this.certificateKv && x.HasUploadedFile);
                    var autobiography = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == this.autobiographyKv && x.HasUploadedFile);
                    var declaration = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == this.declarationOfConsentKv && x.HasUploadedFile);
                    var diploma = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == this.retrainingDiplomaKv && x.HasUploadedFile);

                    if (/*certificate is null || */autobiography is null || declaration is null || diploma is null)
                    {
                        args.Cell.AddClass(new string[] { "color-elements" });
                    }
                }
            }
        }

        private async Task OpenRegiXDiplomaCheckModalAsync()
        {
            if (this.candidateProviderTrainerVM.IdIndentType is null)
            {
                await this.ShowErrorAsync("Моля, въведете данни за вид на идентификатор!");
                return;
            }

            if (string.IsNullOrEmpty(this.candidateProviderTrainerVM.Indent))
            {
                await this.ShowErrorAsync("Моля, въведете данни за номер на идентификатор!");
                return;
            }

            if (string.IsNullOrEmpty(this.candidateProviderTrainerVM.DiplomaNumber))
            {
                await this.ShowErrorAsync("Моля, въведете данни за номер на диплома за средно образование!");
                return;
            }

            if (!long.TryParse(this.candidateProviderTrainerVM.DiplomaNumber, out long value))
            {
                await this.ShowErrorAsync("Полето 'Номер на диплома' може да съдържа само цяло число!");
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

                var identifierType = string.Empty;
                IdentifierType identifier;
                if (this.candidateProviderTrainerVM.IdIndentType == this.kvEGN.IdKeyValue)
                {
                    identifierType = "ЕГН";
                    identifier = IdentifierType.EGN;
                }
                else if (this.candidateProviderTrainerVM.IdIndentType == this.kvIDN.IdKeyValue)
                {
                    identifierType = "ИДН";
                    identifier = IdentifierType.IDN;
                }
                else
                {
                    identifierType = "ЛНЧ";
                    identifier = IdentifierType.LNCh;
                }

                var callContext = await this.GetCallContextAsync(this.SecondarySchoolDiplomaCheckKV);
                var diplomaRequest = this.RegiXService.DiplomaDocumentsType(this.candidateProviderTrainerVM.Indent, identifier, this.candidateProviderTrainerVM.DiplomaNumber, callContext);
                await this.LogRegiXRequestAsync(callContext, diplomaRequest != null);

                if (diplomaRequest is not null)
                {

                    this.diplomaCheckModal.OpenModal(diplomaRequest, identifierType);
                }
                else
                {
                    await this.ShowErrorAsync("Невалидни данни за справка в RegiX!");
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async void ShowDOSInfo(DocVM doc)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                this.SpinnerShow();

                if (doc is not null)
                {
                    this.docInfoModal.OpenModal(doc.RequirementsТrainers, "Изисквания към обучаващите");
                }
                else
                {
                    await this.ShowErrorAsync("Към избраната специалност няма въведен ДОС!");
                }
            }
            finally
            {
                this.SpinnerHide();
                this.loading = false;
            }
        }

        private async Task NextTrainerBtn()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var nextId = await this.trainersGrid.GetRowIndexByPrimaryKeyAsync(this.candidateProviderTrainerVM!.IdCandidateProviderTrainer) + 1;
                if (nextId < this.trainersSource.Count)
                {
                    this.loading = false;

                    await this.trainersGrid.SelectRowAsync(nextId);
                }

                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task PreviousTrainerBtn()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var previousId = await this.trainersGrid.GetRowIndexByPrimaryKeyAsync(this.candidateProviderTrainerVM!.IdCandidateProviderTrainer) - 1;
                if (previousId == -1)
                {
                    previousId = 0;
                }

                if (previousId >= 0)
                {
                    this.loading = false;

                    await this.trainersGrid.SelectRowAsync(previousId);
                }

                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task ExportPDF()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var result = await ReportWord();
                if (this.IsCPO)
                {
                    await this.JsRuntime.SaveAs("Spravka_prepodavateli" + ".pdf", result.ToArray());
                }
                else
                {
                    await this.JsRuntime.SaveAs("Spravka_konsultanti" + ".pdf", result.ToArray());
                }
            }
            finally
            {
                this.loading = false;
            }
                
            this.SpinnerHide();
        }

        private async Task<MemoryStream> ReportWord()
        {
            if (this.IsCPO)
            {
                //Get resource document
                var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
                string documentName = @"\CPOTrainers\Spravka_prepodavateli.docx";
                FileStream template = new FileStream($@"{resources_Folder}{documentName}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WordDocument document = new WordDocument(template, FormatType.Docx);
                DocIORenderer render = new DocIORenderer();

                //Merge fields
                string[] fieldNames = new string[]
                {
                "ProviderName",
                "PoviderBulstat"
                };
                string[] fieldValues = new string[]
                {
                 CandidateProviderVM.ProviderName,
                 CandidateProviderVM.PoviderBulstat
                };

                document.MailMerge.Execute(fieldNames, fieldValues);

                //Navigate to first bookmar with list of trainers
                BookmarksNavigator bookNav = new BookmarksNavigator(document);
                bookNav.MoveToBookmark("TrainersList", true, false);

                #region Paragraphs

                #region TrainersParagraph
                //Create new paragraph
                IWParagraphStyle paragraphStyle = document.AddParagraphStyle("TrainersParagraph");
                paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                #endregion

                #region TrainerHeadingParagraph
                paragraphStyle = document.AddParagraphStyle("TrainerHeadingParagraph");
                paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                paragraphStyle.ParagraphFormat.AfterSpacing = 0;
                paragraphStyle.ParagraphFormat.BackColor = Color.FromArgb(231, 230, 230);
                #endregion

                #region TrainerParagraph
                paragraphStyle = document.AddParagraphStyle("TrainerParagraph");
                paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                paragraphStyle.ParagraphFormat.AfterSpacing = 0;
                #endregion
                #endregion

                ListStyle trainersListStyle = document.AddListStyle(ListType.Numbered, "TrainersList");

                WListLevel trainersListLevel = trainersListStyle.Levels[0];
                trainersListLevel.FollowCharacter = FollowCharacterType.Space;
                trainersListLevel.CharacterFormat.FontSize = (float)10;
                trainersListLevel.TextPosition = 1;

                #region CharacterFormat

                //CharacterFormat
                WCharacterFormat trainersCharacterFormat = new WCharacterFormat(document);
                trainersCharacterFormat.FontName = "Calibri";
                trainersCharacterFormat.FontSize = 12;
                trainersCharacterFormat.Position = 0;
                trainersCharacterFormat.Italic = false;
                trainersCharacterFormat.Bold = false;

                #endregion

                #region DrawListOfTrainers
                int row = 0;
                foreach (var trainer in this.trainersSource)
                {
                    row++;
                    IWParagraph membersParagraph = new WParagraph(document);

                    bookNav.InsertParagraph(membersParagraph);

                    membersParagraph.AppendText(row.ToString() + ". " + trainer.FullName).ApplyCharacterFormat(trainersCharacterFormat);
                    membersParagraph.ApplyStyle("TrainersParagraph");
                }
                #endregion

                //Navigate to bookmark with information for every trainer and create new paragraph 
                bookNav.MoveToBookmark("Trainers", true, false);




                trainersListStyle = document.AddListStyle(ListType.Numbered, "Trainers");

                trainersListLevel = trainersListStyle.Levels[0];
                trainersListLevel.FollowCharacter = FollowCharacterType.Space;
                trainersListLevel.CharacterFormat.FontSize = (float)9.5;
                trainersListLevel.TextPosition = 1;

                row = 0;

                //Draw data for every trainer
                foreach (var prviderTrainer in this.trainersSource)
                {
                    row++;
                    var trainer = await CandidateProviderService.GetCandidateProviderTrainerByIdAsync(new CandidateProviderTrainerVM() { IdCandidateProviderTrainer = prviderTrainer.IdCandidateProviderTrainer });
                    IWParagraph membersParagraph = new WParagraph(document);
                    string gender = string.Empty;

                    bookNav.InsertParagraph(membersParagraph);

                    #region FillingVariables
                    if (trainer.IdSex != null)
                    {
                        gender = (await this.DataSourceService.GetKeyValueByIdAsync(trainer.IdSex)).Name;
                    }
                    string nationality = string.Empty;
                    if (trainer.IdNationality != null)
                    {
                        nationality = (await this.DataSourceService.GetKeyValueByIdAsync(trainer.IdNationality)).Name;
                    }
                    string education = string.Empty;
                    if (trainer.IdEducation != 0)
                    {
                        education = (await this.DataSourceService.GetKeyValueByIdAsync(trainer.IdEducation)).Name;
                    }
                    #endregion

                    membersParagraph.AppendText($"{row.ToString()}. {trainer.FullName}").ApplyCharacterFormat(trainersCharacterFormat);
                    membersParagraph.ApplyStyle("TrainerHeadingParagraph");

                    membersParagraph = new WParagraph(document);
                    bookNav.InsertParagraph(membersParagraph);
                    membersParagraph.AppendText("EГН: " + trainer.Indent + "       " + "Година на раждане: " + (trainer.BirthDate.HasValue ? trainer.BirthDate.Value.Year.ToString() : string.Empty) + "       " + "Пол: " + gender + "\n").ApplyCharacterFormat(trainersCharacterFormat);

                    membersParagraph.AppendText("Гражданство: " + nationality + "\n").ApplyCharacterFormat(trainersCharacterFormat);
                    membersParagraph.AppendText("\n").ApplyCharacterFormat(trainersCharacterFormat);

                    membersParagraph.AppendText("Общи данни: \n").ApplyCharacterFormat(trainersCharacterFormat);
                    membersParagraph.AppendText("Образование: \n").ApplyCharacterFormat(trainersCharacterFormat);
                    membersParagraph.ApplyStyle("TrainersParagraph");

                    #region EducataionTable
                    WTable tableEducation = new WTable(document);
                    tableEducation.TableFormat.Borders.BorderType = BorderStyle.Thick;
                    WCharacterFormat nameColumnCharStyleEducation = new WCharacterFormat(document);
                    tableEducation.ResetCells(2, 1);
                    tableEducation.Rows[0].Height = 20;
                    tableEducation[0, 0].AddParagraph().AppendText("Образователно-квалификационна степен: " + education).ApplyCharacterFormat(nameColumnCharStyleEducation);
                    tableEducation[1, 0].AddParagraph().AppendText("Специалност по диплома: " + trainer.EducationSpecialityNotes).ApplyCharacterFormat(nameColumnCharStyleEducation);
                    bookNav.InsertTable(tableEducation);
                    #endregion

                    membersParagraph = new WParagraph(document);
                    bookNav.InsertParagraph(membersParagraph);
                    membersParagraph.AppendText("Преподавателска дейност: \n").ApplyCharacterFormat(trainersCharacterFormat);
                    membersParagraph.ApplyStyle("TrainersParagraph");

                    #region TableOfProviderTrainerProfiles
                    foreach (var trainerSpeciality in trainer.CandidateProviderTrainerProfiles)
                    {
                        ProfessionalDirectionVM professionalDirection = new ProfessionalDirectionVM();
                        string practice = "Не";
                        string theory = "Не";
                        string isProfessionalDirectionQualified = "Не";

                        #region FillingVariables
                        if (trainerSpeciality.IdProfessionalDirection != 0)
                        {
                            professionalDirection = this.professionalDirectionsSource.FirstOrDefault(x => x.IdProfessionalDirection == trainerSpeciality.IdProfessionalDirection);
                        }
                        if (trainerSpeciality.IsPractice)
                        {
                            practice = "Да";
                        }
                        if (trainerSpeciality.IsTheory)
                        {
                            theory = "Да";
                        }
                        if (trainerSpeciality.IsProfessionalDirectionQualified)
                        {
                            isProfessionalDirectionQualified = "Да";
                        }
                        string professionalDirectionName = professionalDirection != null ? professionalDirection.Name : string.Empty;
                        #endregion

                        WTable table = new WTable(document);
                        table.TableFormat.Borders.BorderType = BorderStyle.Thick;
                        WCharacterFormat nameColumnCharStyle = new WCharacterFormat(document);
                        nameColumnCharStyle.FontSize = 12;

                        table.ResetCells(3, 1);
                        table.Rows[0].Height = 20;
                        table[0, 0].AddParagraph().AppendText("Професионално направление: " + professionalDirectionName).ApplyCharacterFormat(nameColumnCharStyle);
                        table[1, 0].AddParagraph().AppendText("Отговаря на изискванията за това ПН: " + isProfessionalDirectionQualified).ApplyCharacterFormat(nameColumnCharStyle);
                        var nestedTable = table[2, 0].AddTable();
                        nestedTable.ResetCells(1, 2);
                        nestedTable[0, 0].AddParagraph().AppendText("Преподава по теория: " + theory).ApplyCharacterFormat(nameColumnCharStyle);
                        nestedTable[0, 1].AddParagraph().AppendText("Преподава по практика: " + practice).ApplyCharacterFormat(nameColumnCharStyle);
                        bookNav.InsertTable(table);

                        membersParagraph = new WParagraph(document);
                        bookNav.InsertParagraph(membersParagraph);
                        membersParagraph.AppendText("\n").ApplyCharacterFormat(trainersCharacterFormat);
                        membersParagraph.ApplyStyle("TrainersParagraph");
                    }
                    #endregion

                    membersParagraph = new WParagraph(document);
                    bookNav.InsertParagraph(membersParagraph);
                    membersParagraph.AppendText("Преминати квалификационни курсове: \n").ApplyCharacterFormat(trainersCharacterFormat);
                    membersParagraph.ApplyStyle("TrainersParagraph");

                    #region TableOfProviderTrainerQualifications
                    foreach (var qualification in trainer.CandidateProviderTrainerQualifications)
                    {
                        ProfessionVM profession = new ProfessionVM();
                        KeyValueVM educationType = new KeyValueVM();
                        KeyValueVM qualificationType = new KeyValueVM();
                        if (qualification.IdProfession.HasValue)
                        {
                            profession = this.professionsSource.FirstOrDefault(x => x.IdProfession == qualification.IdProfession.Value);
                        }
                        if (qualification.IdTrainingQualificationType != 0)
                        {
                            educationType = await DataSourceService.GetKeyValueByIdAsync(qualification.IdTrainingQualificationType);
                        }
                        if (qualification.IdQualificationType != 0)
                        {
                            qualificationType = await DataSourceService.GetKeyValueByIdAsync(qualification.IdQualificationType);
                        }
                        string professionName = profession != null ? profession.Name : string.Empty;
                        string educationTypeName = educationType != null ? educationType.Name : string.Empty;
                        string qualificationTypeName = qualificationType != null ? qualificationType.Name : string.Empty;
                        WTable table = new WTable(document);
                        table.TableFormat.Borders.BorderType = BorderStyle.Thick;
                        WCharacterFormat nameColumnCharStyle = new WCharacterFormat(document);
                        nameColumnCharStyle.FontSize = 12;

                        table.ResetCells(5, 1);
                        table.Rows[0].Height = 20;
                        table[0, 0].AddParagraph().AppendText("Наименование на курса: " + qualification.QualificationName).ApplyCharacterFormat(nameColumnCharStyle);
                        table[1, 0].AddParagraph().AppendText("Вид на курса: " + qualificationTypeName).ApplyCharacterFormat(nameColumnCharStyle);
                        table[2, 0].AddParagraph().AppendText("Професия: " + professionName).ApplyCharacterFormat(nameColumnCharStyle);
                        table[3, 0].AddParagraph().AppendText("Вид на обучението: " + educationTypeName).ApplyCharacterFormat(nameColumnCharStyle);
                        var nestedTable = table[4, 0].AddTable();
                        nestedTable.ResetCells(1, 2);
                        nestedTable[0, 0].AddParagraph().AppendText("Продължителност (в часове): " + qualification.QualificationDuration).ApplyCharacterFormat(nameColumnCharStyle);
                        nestedTable[0, 1].AddParagraph().AppendText("Дата на започване: " + (qualification.TrainingFrom.HasValue ? qualification.TrainingFrom.Value.ToString("dd.MM.yyyy") : string.Empty)).ApplyCharacterFormat(nameColumnCharStyle);
                        bookNav.InsertTable(table);

                        membersParagraph = new WParagraph(document);
                        bookNav.InsertParagraph(membersParagraph);
                        membersParagraph.AppendText("\n").ApplyCharacterFormat(trainersCharacterFormat);
                        membersParagraph.ApplyStyle("TrainersParagraph");
                    }
                    #endregion

                    membersParagraph = new WParagraph(document);
                    bookNav.InsertParagraph(membersParagraph);
                    membersParagraph.AppendText("Списък с приложените за този учител документи:").ApplyCharacterFormat(trainersCharacterFormat);
                    membersParagraph.ApplyStyle("TrainersParagraph");

                }


                //Convert to document - WORD To PDF
                PdfDocument pdfDocument = render.ConvertToPDF(document);
                using (MemoryStream stream = new MemoryStream())
                {
                    pdfDocument.Save(stream);
                    return stream;
                }
            }
            else
            {
                //Get resource document
                var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
                string documentName = @"\CIPOTrainers\Spravka_konsultanti.docx";
                FileStream template = new FileStream($@"{resources_Folder}{documentName}", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                WordDocument document = new WordDocument(template, FormatType.Docx);
                DocIORenderer render = new DocIORenderer();

                //Merge fields
                string[] fieldNames = new string[]
                {
                "ProviderName",
                "PoviderBulstat"
                };
                string[] fieldValues = new string[]
                {
                 CandidateProviderVM.ProviderName,
                 CandidateProviderVM.PoviderBulstat
                };

                document.MailMerge.Execute(fieldNames, fieldValues);

                //Navigate to first bookmar with list of trainers
                BookmarksNavigator bookNav = new BookmarksNavigator(document);
                bookNav.MoveToBookmark("TrainersList", true, false);

                #region Paragraphs

                #region TrainersParagraph
                //Create new paragraph
                IWParagraphStyle paragraphStyle = document.AddParagraphStyle("TrainersParagraph");
                paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                #endregion

                #region TrainerHeadingParagraph
                paragraphStyle = document.AddParagraphStyle("TrainerHeadingParagraph");
                paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                paragraphStyle.ParagraphFormat.AfterSpacing = 0;
                paragraphStyle.ParagraphFormat.BackColor = Color.FromArgb(231, 230, 230);
                #endregion

                #region TrainerParagraph
                paragraphStyle = document.AddParagraphStyle("TrainerParagraph");
                paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                paragraphStyle.ParagraphFormat.AfterSpacing = 0;
                #endregion
                #endregion

                ListStyle trainersListStyle = document.AddListStyle(ListType.Numbered, "TrainersList");

                WListLevel trainersListLevel = trainersListStyle.Levels[0];
                trainersListLevel.FollowCharacter = FollowCharacterType.Space;
                trainersListLevel.CharacterFormat.FontSize = (float)10;
                trainersListLevel.TextPosition = 1;

                #region CharacterFormat

                //CharacterFormat
                WCharacterFormat trainersCharacterFormat = new WCharacterFormat(document);
                trainersCharacterFormat.FontName = "Calibri";
                trainersCharacterFormat.FontSize = 12;
                trainersCharacterFormat.Position = 0;
                trainersCharacterFormat.Italic = false;
                trainersCharacterFormat.Bold = false;

                #endregion

                #region DrawListOfTrainers
                int row = 0;
                foreach (var trainer in this.trainersSource)
                {
                    row++;
                    IWParagraph membersParagraph = new WParagraph(document);

                    bookNav.InsertParagraph(membersParagraph);

                    membersParagraph.AppendText(row.ToString() + ". " + trainer.FullName).ApplyCharacterFormat(trainersCharacterFormat);
                    membersParagraph.ApplyStyle("TrainersParagraph");
                }
                #endregion

                //Navigate to bookmark with information for every trainer and create new paragraph 
                bookNav.MoveToBookmark("Trainers", true, false);




                trainersListStyle = document.AddListStyle(ListType.Numbered, "Trainers");

                trainersListLevel = trainersListStyle.Levels[0];
                trainersListLevel.FollowCharacter = FollowCharacterType.Space;
                trainersListLevel.CharacterFormat.FontSize = (float)9.5;
                trainersListLevel.TextPosition = 1;

                row = 0;

                //Draw data for every trainer
                foreach (var prviderTrainer in this.trainersSource)
                {
                    row++;
                    var trainer = await CandidateProviderService.GetCandidateProviderTrainerByIdAsync(new CandidateProviderTrainerVM() { IdCandidateProviderTrainer = prviderTrainer.IdCandidateProviderTrainer });
                    IWParagraph membersParagraph = new WParagraph(document);
                    string gender = string.Empty;

                    bookNav.InsertParagraph(membersParagraph);

                    #region FillingVariables
                    if (trainer.IdSex != null)
                    {
                        gender = (await this.DataSourceService.GetKeyValueByIdAsync(trainer.IdSex)).Name;
                    }
                    string nationality = string.Empty;
                    if (trainer.IdNationality != null)
                    {
                        nationality = (await this.DataSourceService.GetKeyValueByIdAsync(trainer.IdNationality)).Name;
                    }
                    string education = string.Empty;
                    if (trainer.IdEducation != 0)
                    {
                        education = (await this.DataSourceService.GetKeyValueByIdAsync(trainer.IdEducation)).Name;
                    }
                    #endregion

                    membersParagraph.AppendText($"{row.ToString()}. {trainer.FullName}").ApplyCharacterFormat(trainersCharacterFormat);
                    membersParagraph.ApplyStyle("TrainerHeadingParagraph");

                    membersParagraph = new WParagraph(document);
                    bookNav.InsertParagraph(membersParagraph);
                    membersParagraph.AppendText("EГН: " + trainer.Indent + "       " + "Година на раждане: " + (trainer.BirthDate.HasValue ? trainer.BirthDate.Value.Year.ToString() : string.Empty) + "       " + "Пол: " + gender + "\n").ApplyCharacterFormat(trainersCharacterFormat);

                    membersParagraph.AppendText("Гражданство: " + nationality + "\n").ApplyCharacterFormat(trainersCharacterFormat);

                    membersParagraph.AppendText("Образование:").ApplyCharacterFormat(trainersCharacterFormat);
                    membersParagraph.ApplyStyle("TrainersParagraph");

                    #region EducataionTable
                    WTable tableEducation = new WTable(document);
                    tableEducation.TableFormat.Borders.BorderType = BorderStyle.Thick;
                    WCharacterFormat nameColumnCharStyleEducation = new WCharacterFormat(document);
                    tableEducation.ResetCells(2, 1);
                    tableEducation.Rows[0].Height = 20;
                    tableEducation[0, 0].AddParagraph().AppendText("Образователно-квалификационна степен: " + education).ApplyCharacterFormat(nameColumnCharStyleEducation);
                    tableEducation[1, 0].AddParagraph().AppendText("Специалност по диплома: " + trainer.EducationSpecialityNotes).ApplyCharacterFormat(nameColumnCharStyleEducation);
                    bookNav.InsertTable(tableEducation);
                    membersParagraph = new WParagraph(document);
                    bookNav.InsertParagraph(membersParagraph);

                    #endregion



                }






                //Convert to document - WORD To PDF
                PdfDocument pdfDocument = render.ConvertToPDF(document);
                using (MemoryStream stream = new MemoryStream())
                {
                    pdfDocument.Save(stream);
                    return stream;
                }
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<CandidateProviderTrainerVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(trainersGrid, args.Data.IdCandidateProviderTrainer).Result.ToString();
            }
        }

        private void ChangeExpand()
        {
            this.isAccordionExpanded = !this.isAccordionExpanded;
        }
    }
}
