using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Core.ViewModels.Candidate;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Microsoft.AspNetCore.Components.Forms;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Syncfusion.Blazor.DropDowns;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.Contracts.EKATTE;
using Syncfusion.Blazor.Data;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.Contracts.SPPOO;
using Data.Models.Data.Candidate;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.HelperClasses;
using Microsoft.JSInterop;
using ISNAPOO.WebSystem.Resources;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.Contracts.DOC;
using Syncfusion.DocIO;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Blazor.Navigations;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class MaterialTechnicalBase : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderPremisesVM> mtbsGrid = new SfGrid<CandidateProviderPremisesVM>();
        private SfGrid<CandidateProviderPremisesRoomVM> premisesRoomsGrid = new SfGrid<CandidateProviderPremisesRoomVM>();
        private SfGrid<CandidateProviderPremisesDocumentVM> mtbDocumentsGrid = new SfGrid<CandidateProviderPremisesDocumentVM>();
        private SfAutoComplete<int?, LocationVM> sfAutoCompleteLocation = new SfAutoComplete<int?, LocationVM>();
        private CandidateProviderPremisesDocumentModal candidateProviderPremisesDocumentModal = new CandidateProviderPremisesDocumentModal();
        private CandidateProviderPremisesSearchModal searchModal = new CandidateProviderPremisesSearchModal();
        private MTBStatusModal mTBStatusModal = new MTBStatusModal();
        private ShowDOCInfoModal docInfoModal = new ShowDOCInfoModal();
        private CandidateProviderPremisesRoomModal candidateProviderPremisesRoomModal = new CandidateProviderPremisesRoomModal();
        private CandidateProviderPremisesSpecialityModal candidateProviderPremisesSpecialityModal = new CandidateProviderPremisesSpecialityModal();
        private CandidateProviderPremisesCheckingModal candidateProviderPremisesCheckingModal = new CandidateProviderPremisesCheckingModal();

        private List<CandidateProviderPremisesVM> mtbsSource = new List<CandidateProviderPremisesVM>();
        private List<CandidateProviderPremisesRoomVM> premisesRoomsSource = new List<CandidateProviderPremisesRoomVM>();
        private List<CandidateProviderPremisesDocumentVM> mtbDocumentsSource = new List<CandidateProviderPremisesDocumentVM>();
        private IEnumerable<KeyValueVM> kvMTBOwnershipSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvMTBStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvTrainingTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvPremisesTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvVQSSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvDocumentTypeSource = new List<KeyValueVM>();
        private IEnumerable<LocationVM> locationSource = new List<LocationVM>();
        private IEnumerable<LocationVM> allLocations = new List<LocationVM>();
        private IEnumerable<ProfessionVM> professionsSource = new List<ProfessionVM>();
        private IEnumerable<SpecialityVM> specialitiesSource = new List<SpecialityVM>();
        private IEnumerable<ProfessionalDirectionVM> professionalDirectionsSource = new List<ProfessionalDirectionVM>();
        private CandidateProviderPremisesVM candidateProviderPremisesVM = new CandidateProviderPremisesVM();
        private bool isAddButtonClicked = false;
        private bool isSpecialityGridButtonClicked = false;
        private bool isDetailExpanded = false;
        private KeyValueVM kvTheory = new KeyValueVM();
        private KeyValueVM kvPractice = new KeyValueVM();
        private KeyValueVM kvTheoryAndPractice = new KeyValueVM();
        private KeyValueVM kvDocComplianceYes = new KeyValueVM();
        private KeyValueVM kvDocComplianceNo = new KeyValueVM();
        private KeyValueVM kvDocCompliancePartial = new KeyValueVM();
        private KeyValueVM kvMTBStatusActive = new KeyValueVM();
        private KeyValueVM kvMTBStatusInactive = new KeyValueVM();
        private string lastAddedMTB = string.Empty;
        private bool isInitialRender = true;
        private int dDocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfessionKv;
        private int documentsForThePresenceOfMTBInAccordanceWithTheDOSKv;
        private int documentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirementsKv;
        private int documentForComplianceWithFireSafetyRulesAndRegulationsKv;
        private IEnumerable<DocVM> docSource = new List<DocVM>();
        private string premisesNameInformation = string.Empty;
        private bool disableNextBtn = false;
        private bool disablePreviousBtn = false;

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
        public ILocationService LocationService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IProfessionalDirectionService ProfessionalDirectionService { get; set; }

        [Inject]
        public IProfessionService ProfessionService { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IDOCService DOCService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.candidateProviderPremisesVM);
            this.FormTitle = "Материално-техническа база";

            this.editContext.MarkAsUnmodified();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await this.LoadDataAsync();

                this.editContext.MarkAsUnmodified();

                this.StateHasChanged();
            }
        }

        private async Task LoadDataAsync()
        {
            this.mtbsSource = (await this.CandidateProviderService.GetCandidateProviderPremisesWithStatusByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider)).ToList();

            this.kvMTBStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseStatus");
            this.kvMTBStatusInactive = await this.DataSourceService.GetKeyValueByIntCodeAsync("MaterialTechnicalBaseStatus", "Inactive");
            if (this.IsCPO)
            {
                this.professionalDirectionsSource = await this.ProfessionalDirectionService.GetAllActiveProfessionalDirectionsAsync();
                this.professionsSource = await this.ProfessionService.GetAllActiveProfessionsAsync();
                this.specialitiesSource = this.DataSourceService.GetAllSpecialitiesList();
                this.kvTrainingTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
                this.kvVQSSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
                this.kvTheory = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TheoryTraining");
                this.kvPractice = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "PracticalTraining");
                this.kvTheoryAndPractice = await DataSourceService.GetKeyValueByIntCodeAsync("TrainingType", "TrainingInTheoryAndPractice");
                this.kvDocComplianceYes = await DataSourceService.GetKeyValueByIntCodeAsync("ComplianceDOC", "Does");
                this.kvDocComplianceNo = await DataSourceService.GetKeyValueByIntCodeAsync("ComplianceDOC", "Doesnt");
                this.kvDocCompliancePartial = await DataSourceService.GetKeyValueByIntCodeAsync("ComplianceDOC", "Partial");

                this.docSource = await this.DOCService.GetAllActiveDocAsync();
            }

            this.kvDocumentTypeSource = this.IsCPO
                ? await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MTBDocumentType")
                : (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MTBDocumentType")).Where(x => x.DefaultValue3 != null & x.DefaultValue3!.Contains("CIPO")).ToList();

            if (this.IsCPO)
            {
                this.documentForComplianceWithFireSafetyRulesAndRegulationsKv = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DocumentForComplianceWithFireSafetyRulesAndRegulations").IdKeyValue;
                this.documentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirementsKv = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DocumentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements").IdKeyValue;
                this.documentsForThePresenceOfMTBInAccordanceWithTheDOSKv = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DocumentsForThePresenceOfMTBInAccordanceWithTheDOS").IdKeyValue;
                this.dDocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfessionKv = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfession").IdKeyValue;
            }
           
            this.allLocations = await this.LocationService.GetAllLocationsAsync();
            this.kvMTBOwnershipSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseOwnership");
            this.kvMTBStatusActive = this.kvMTBStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Active");
            this.kvPremisesTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RoomType");

            this.candidateProviderPremisesVM.IdStatus = this.kvMTBStatusActive.IdKeyValue;

            await this.mtbsGrid.Refresh();
        }

        public new async Task SubmitHandler()
        {
            this.editContext = new EditContext(this.candidateProviderPremisesVM);
            this.editContext.EnableDataAnnotationsValidation();

            if (this.editContext.Validate())
            {
                if (this.candidateProviderPremisesVM.IdCandidateProviderPremises == 0)
                {
                    this.candidateProviderPremisesVM.IdCandidate_Provider = this.CandidateProviderVM.IdCandidate_Provider;
                    await this.CandidateProviderService.CreateCandidateProviderPremisesAsync(this.candidateProviderPremisesVM);

                    // селектира и добавя нова база към списъка
                    await this.HandleSelectAndOrderFunctionalityAfterAddNewPremisesAsync();
                }
                else
                {
                    await this.CandidateProviderService.UpdateCandidateProviderPremisesAsync(this.candidateProviderPremisesVM);

                    // ъпдейтва данните на базата в грида след интеракция с БД
                    await this.HandleUpdatePremisesDataInGridAfterUpdateAsync();
                }
            }
        }

        // ъпдейтва данните на базата в грида след интеракция с БД
        private async Task HandleUpdatePremisesDataInGridAfterUpdateAsync()
        {
            var idxOfUpdatedPremises = this.mtbsSource.FindIndex(x => x.IdCandidateProviderPremises == this.candidateProviderPremisesVM.IdCandidateProviderPremises);
            if (idxOfUpdatedPremises != -1)
            {
                this.editContext.MarkAsUnmodified();

                //if (this.TabReference.Items[this.TabReference.SelectedItem].Header.Text == "МТБ")
                //{
                //    await this.mtbsGrid.SetRowDataAsync(this.candidateProviderPremisesVM.IdCandidateProviderPremises, this.candidateProviderPremisesVM);
                //}

                try
                {
                    await this.mtbsGrid.SetRowDataAsync(this.candidateProviderPremisesVM.IdCandidateProviderPremises, this.candidateProviderPremisesVM);
                }
                catch (Exception ex) { }

                this.StateHasChanged();
            }
        }

        // селектира и добавя нова база към списъка
        private async Task HandleSelectAndOrderFunctionalityAfterAddNewPremisesAsync()
        {
            this.mtbsSource.Add(this.candidateProviderPremisesVM);
            this.mtbsSource = this.mtbsSource.OrderBy(x => x.PremisesName).ToList();
            await this.mtbsGrid.Refresh();

            this.editContext.MarkAsUnmodified();
            this.StateHasChanged();

            var premisesIdx = await this.mtbsGrid.GetRowIndexByPrimaryKeyAsync(this.candidateProviderPremisesVM.IdCandidateProviderPremises);
            if (premisesIdx != -1)
            {
                await this.mtbsGrid.SelectRowAsync(premisesIdx);
                if (this.IsCPO)
                {
                    await this.mtbsGrid.ExpandCollapseDetailRowAsync(this.candidateProviderPremisesVM);
                }
            }
        }

        public async Task FilterSelect()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.searchModal.OpenModal(this.CandidateProviderVM, this.mtbsSource, this.IsCPO);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public void FilterApply(List<CandidateProviderPremisesVM> candidateProviderPremisesVMs)
        {
            this.mtbsSource = candidateProviderPremisesVMs.OrderBy(x => x.PremisesName).ToList();
            if (!this.mtbsGrid.SelectedRecords.Any(x => this.mtbsSource.Any(y => y.IdCandidateProviderPremises == x.IdCandidateProviderPremises)))
            {
                this.candidateProviderPremisesVM = new CandidateProviderPremisesVM();
                this.mtbDocumentsSource = new List<CandidateProviderPremisesDocumentVM>();
                this.premisesRoomsSource = new List<CandidateProviderPremisesRoomVM>();
            }

            this.StateHasChanged();
        }

        private async Task AddNewMTBClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            if (this.editContext.IsModified())
            {
                string msg = "Имате незапазени промени! Сигурни ли сте, че искате да продължите?";
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

                        this.candidateProviderPremisesVM = new CandidateProviderPremisesVM()
                        {
                            IdStatus = this.kvMTBStatusActive.IdKeyValue
                        };

                        this.editContext = new EditContext(this.candidateProviderPremisesVM);
                        this.premisesRoomsSource = new List<CandidateProviderPremisesRoomVM>();
                        this.mtbDocumentsSource = new List<CandidateProviderPremisesDocumentVM>();

                        if (this.IsCPO)
                        {
                            await this.mtbsGrid.CollapseAllDetailRowAsync();
                        }

                        await this.mtbsGrid.ClearRowSelectionAsync();

                        this.StateHasChanged();
                    }
                    finally
                    {
                        this.loading = false;
                    }

                    this.SpinnerHide();
                }
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

                    this.candidateProviderPremisesVM = new CandidateProviderPremisesVM()
                    {
                        IdStatus = this.kvMTBStatusActive.IdKeyValue
                    };

                    this.premisesRoomsSource = new List<CandidateProviderPremisesRoomVM>();
                    this.mtbDocumentsSource = new List<CandidateProviderPremisesDocumentVM>();

                    if (this.IsCPO)
                    {
                        await this.mtbsGrid.CollapseAllDetailRowAsync();
                    }

                    await this.mtbsGrid.ClearRowSelectionAsync();

                    this.StateHasChanged();
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }

            this.premisesNameInformation = string.Empty;
        }

        private async Task ChangeMTB(int id)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.candidateProviderPremisesVM = this.IsCPO
                    ? await this.CandidateProviderService.GetCandidateProviderPremisesByIdAsync(new CandidateProviderPremisesVM() { IdCandidateProviderPremises = id })
                    : await this.CandidateProviderService.GetCandidateProviderPremisesWithRoomsAndDocumentsByIdAsync(new CandidateProviderPremisesVM() { IdCandidateProviderPremises = id });
                if (this.candidateProviderPremisesVM is not null)
                {
                    var premisesIdx = this.mtbsSource.FindIndex(x => x.IdCandidateProviderPremises == this.candidateProviderPremisesVM.IdCandidateProviderPremises);
                    if (premisesIdx != -1)
                    {
                        this.mtbsSource[premisesIdx] = this.candidateProviderPremisesVM;
                    }

                    this.premisesRoomsSource = this.candidateProviderPremisesVM.CandidateProviderPremisesRooms.OrderBy(x => x.PremisesRoomName).ToList();
                    foreach (var room in this.premisesRoomsSource)
                    {
                        if (room.IdPremisesType != 0)
                        {
                            room.PremisesTypeName = this.kvPremisesTypeSource.FirstOrDefault(x => x.IdKeyValue == room.IdPremisesType)?.Name;
                        }

                        if (room.IdUsage != 0)
                        {
                            room.UsageName = this.kvTrainingTypeSource.FirstOrDefault(x => x.IdKeyValue == room.IdUsage)?.Name;
                        }
                    }

                    this.mtbDocumentsSource = this.candidateProviderPremisesVM.CandidateProviderPremisesDocuments.ToList();
                    foreach (var document in this.mtbDocumentsSource)
                    {
                        var docName = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType);
                        if (docName is not null)
                        {
                            document.DocumentTypeName = docName.Name;
                        }

                        document.UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);

                        if (document.HasUploadedFile)
                        {
                            await this.SetFileNameAsync(document);
                        }
                    }

                    this.premisesNameInformation = this.candidateProviderPremisesVM.PremisesName;

                    // проверява за последен/пръв запис в грида
                    var idxRow = await this.mtbsGrid.GetRowIndexByPrimaryKeyAsync(this.candidateProviderPremisesVM.IdCandidateProviderPremises);
                    if (idxRow + 1 == this.mtbsSource.Count)
                    {
                        this.disableNextBtn = true;
                        this.disablePreviousBtn = false;
                    }
                    else if (idxRow == 0)
                    {
                        this.disablePreviousBtn = true;
                        this.disableNextBtn = false;
                    }
                    else
                    {
                        this.disableNextBtn = false;
                        this.disablePreviousBtn = false;
                    }

                    if (this.candidateProviderPremisesVM.IdLocation != null)
                    {
                        LocationVM location = await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.candidateProviderPremisesVM.IdLocation ?? default);
                        this.locationSource = new List<LocationVM>()
                        {
                            location
                        };
                    }

                    this.editContext.MarkAsUnmodified();

                    this.StateHasChanged();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnFilterLocation(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    this.locationSource = await this.LocationService.GetAllLocationsByKatiAsync(args.Text);
                }
                catch (Exception ex) { }

                var query = new Syncfusion.Blazor.Data.Query().Where(new WhereFilter() { Field = "kati", Operator = "contains", value = args.Text, IgnoreCase = true });

                query = !string.IsNullOrEmpty(args.Text) ? query : new Syncfusion.Blazor.Data.Query();

                await this.sfAutoCompleteLocation.FilterAsync(this.locationSource, query);
            }
        }

        private async Task OnRoomModalSubmit(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM)
        {
            this.premisesRoomsSource = (await this.CandidateProviderService.GetAllCandidateProviderPremisesRoomsByCandidateProviderPremisesIdAsync(new CandidateProviderPremisesRoomVM() { IdCandidateProviderPremises = candidateProviderPremisesRoomVM.IdCandidateProviderPremises })).OrderBy(x => x.PremisesRoomName).ToList();
            foreach (var room in this.premisesRoomsSource)
            {
                room.PremisesTypeName = this.kvPremisesTypeSource.FirstOrDefault(x => x.IdKeyValue == room.IdPremisesType).Name;
                room.UsageName = this.kvTrainingTypeSource.FirstOrDefault(x => x.IdKeyValue == room.IdUsage).Name;
            }

            this.StateHasChanged();
        }

        private async Task AddNewRoomClickHandler()
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

                await this.candidateProviderPremisesRoomModal.OpenModal(new CandidateProviderPremisesRoomVM() { IdCandidateProviderPremises = this.candidateProviderPremisesVM.IdCandidateProviderPremises }, this.kvTrainingTypeSource, this.kvPremisesTypeSource, this.candidateProviderPremisesVM.PremisesName);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteRoomBtn(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM)
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

                    var resultContext = await this.CandidateProviderService.DeleteCandidateProviderPremisesRoomAsync(candidateProviderPremisesRoomVM);
                    if (resultContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                        this.premisesRoomsSource = (await this.CandidateProviderService.GetAllCandidateProviderPremisesRoomsByCandidateProviderPremisesIdAsync(new CandidateProviderPremisesRoomVM() { IdCandidateProviderPremises = candidateProviderPremisesRoomVM.IdCandidateProviderPremises })).OrderBy(x => x.PremisesRoomName).ToList();
                        foreach (var room in this.premisesRoomsSource)
                        {
                            room.PremisesTypeName = this.kvPremisesTypeSource.FirstOrDefault(x => x.IdKeyValue == room.IdPremisesType)?.Name;
                            room.UsageName = this.kvTrainingTypeSource.FirstOrDefault(x => x.IdKeyValue == room.IdUsage)?.Name;
                        }

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

        private async Task EditRoomBtn(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM)
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

                await this.candidateProviderPremisesRoomModal.OpenModal(candidateProviderPremisesRoomVM, this.kvTrainingTypeSource, this.kvPremisesTypeSource, this.candidateProviderPremisesVM.PremisesName);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnPremisesSpecialityModalSubmit(int idCandidateProviderPremises)
        {
            var premisesIdx = await this.mtbsGrid.GetRowIndexByPrimaryKeyAsync(idCandidateProviderPremises);

            await this.mtbsGrid.SelectRowAsync(premisesIdx);
        }

        private async Task AddCheckingToMTBsClickHandler()
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

                if (this.candidateProviderPremisesVM.IdCandidateProviderPremises == 0)
                {
                    await this.ShowErrorAsync("Моля, изберете материална база/и от списъка!");
                }
                else
                {
                    await this.candidateProviderPremisesCheckingModal.OpenModal(new List<CandidateProviderPremisesVM>() { this.candidateProviderPremisesVM });
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task AddSpecialitiesToMTBsClickHandler()
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

                if (this.candidateProviderPremisesVM.IdCandidateProviderPremises == 0)
                {
                    await this.ShowErrorAsync("Моля, изберете материална база от списъка!");
                }
                else
                {
                    var providerSpecialities = await this.CandidateProviderService.GetProviderSpecialitiesWithoutIncludesByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider);
                    var candidateProviderPremisesFromSource = this.mtbsSource.FirstOrDefault(x => x.IdCandidateProviderPremises == this.candidateProviderPremisesVM.IdCandidateProviderPremises)!;
                    await this.candidateProviderPremisesSpecialityModal.OpenModal(new List<CandidateProviderPremisesVM>() { candidateProviderPremisesFromSource }, providerSpecialities.ToList(), this.professionsSource, this.professionalDirectionsSource);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void PremisesRecordClickHandler(RecordClickEventArgs<CandidateProviderPremisesVM> args)
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

        private async Task MTBSelectingHandler(RowSelectingEventArgs<CandidateProviderPremisesVM> args)
        {
            if (this.IsCPO)
            {
                if (!this.editContext.IsModified())
                {
                    if (!this.isDetailExpanded)
                    {
                        this.isDetailExpanded = true;
                        await this.mtbsGrid.CollapseAllDetailRowAsync();
                        await this.ChangeMTB(args.Data.IdCandidateProviderPremises);
                        await this.mtbsGrid.ExpandCollapseDetailRowAsync(args.Data);
                    }
                }
                else
                {
                    string msg = "Имате незапазени промени! Сигурни ли сте, че искате да смените материлно-техническата база?";
                    bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
                    if (isConfirmed)
                    {
                        if (!this.isDetailExpanded)
                        {
                            this.isDetailExpanded = true;
                            await this.mtbsGrid.CollapseAllDetailRowAsync();
                            await this.ChangeMTB(args.Data.IdCandidateProviderPremises);
                            await this.mtbsGrid.ExpandCollapseDetailRowAsync(args.Data);
                        }
                    }
                    else
                    {
                        args.Cancel = true;
                    }
                }

                this.isDetailExpanded = false;
            }
            else
            {
                if (!this.editContext.IsModified())
                {
                    await this.ChangeMTB(args.Data.IdCandidateProviderPremises);
                }
                else
                {
                    string msg = "Имате незапазени промени! Сигурни ли сте, че искате да смените материлно-техническата база?";
                    bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
                    if (isConfirmed)
                    {
                        await this.ChangeMTB(args.Data.IdCandidateProviderPremises);
                    }
                    else
                    {
                        args.Cancel = true;
                    }
                }
            }
        }

        private async Task DeleteSpecialityBtn(int idSpeciality, int idCandidateProviderPremises, int idUsage)
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

                    var candidateProviderPremisesSpeciality = await this.CandidateProviderService.GetCandidateProviderSpecialityByIdSpecialityAndIdCandidateProviderPremisesAsync(idSpeciality, idCandidateProviderPremises, idUsage);
                    if (candidateProviderPremisesSpeciality is not null)
                    {
                        var resultContext = await this.CandidateProviderService.DeleteCandidateProviderPremisesSpecialityAsync(candidateProviderPremisesSpeciality, idUsage);
                        if (resultContext.HasErrorMessages)
                        {
                            await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                        }
                        else
                        {
                            await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                            this.mtbsSource.First(x => x.IdCandidateProviderPremises == idCandidateProviderPremises).CandidateProviderPremisesSpecialities.Remove(this.mtbsSource.First(x => x.IdCandidateProviderPremises == idCandidateProviderPremises).CandidateProviderPremisesSpecialities.First(x => x.IdSpeciality == idSpeciality));
                        }

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

        private async Task DeleteDocumentBtn(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM)
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

                    var resultContext = await this.CandidateProviderService.DeleteCandidateProviderPremisesDocumentAsync(candidateProviderPremisesDocumentVM);

                    if (resultContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                        this.mtbDocumentsSource = (await this.CandidateProviderService.GetAllCandidateProviderPremisesDocumentsByCandidateProviderPremisesIdAsync(new CandidateProviderPremisesDocumentVM() { IdCandidateProviderPremises = candidateProviderPremisesDocumentVM.IdCandidateProviderPremises })).ToList();
                        foreach (var document in this.mtbDocumentsSource)
                        {
                            var docName = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType);
                            if (docName is not null)
                            {
                                document.DocumentTypeName = docName.Name;
                            }

                            document.UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);

                            if (document.HasUploadedFile)
                            {
                                await this.SetFileNameAsync(document);
                            }
                        }

                        await this.mtbDocumentsGrid.Refresh();
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

        private async Task OnDownloadClick(string fileName, CandidateProviderPremisesDocumentVM candidateProviderPremisesDocument)
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

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderPremisesDocument>(candidateProviderPremisesDocument.IdCandidateProviderPremisesDocument, fileName);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderPremisesDocument>(candidateProviderPremisesDocument.IdCandidateProviderPremisesDocument, fileName);

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

        private async Task OnDocumentModalSubmit(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM)
        {
            this.mtbDocumentsSource = (await this.CandidateProviderService.GetAllCandidateProviderPremisesDocumentsByCandidateProviderPremisesIdAsync(new CandidateProviderPremisesDocumentVM() { IdCandidateProviderPremises = candidateProviderPremisesDocumentVM.IdCandidateProviderPremises })).ToList();
            foreach (var document in this.mtbDocumentsSource)
            {
                var docName = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType);
                if (docName is not null)
                {
                    document.DocumentTypeName = docName.Name;
                }

                document.UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);

                if (document.HasUploadedFile)
                {
                    await this.SetFileNameAsync(document);
                }
            }

            await this.mtbDocumentsGrid.Refresh();
            this.StateHasChanged();
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

                await this.candidateProviderPremisesDocumentModal.OpenModal(new CandidateProviderPremisesDocumentVM() { IdCandidateProviderPremises = this.candidateProviderPremisesVM.IdCandidateProviderPremises }, this.kvDocumentTypeSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task SetFileNameAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocument)
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var fileFullName = settingResource + "\\" + candidateProviderPremisesDocument.UploadedFileName;
            if (Directory.Exists(fileFullName))
            {
                var files = Directory.GetFiles(fileFullName);
                files = files.Select(x => x.Split(($"\\{candidateProviderPremisesDocument.IdCandidateProviderPremisesDocument}\\"), StringSplitOptions.RemoveEmptyEntries).LastOrDefault()).ToArray();
                candidateProviderPremisesDocument.FileName = string.Join(Environment.NewLine, files);
            }
            else
            {
                candidateProviderPremisesDocument.FileName = string.Empty;
            }
        }

        private async Task DeleteCheckingBtn(int _idChecking, int idCandidateProviderPremises)
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

                    var mtb = this.mtbsSource.FirstOrDefault(x => x.IdCandidateProviderPremises == idCandidateProviderPremises);
                    var checking = mtb.CandidateProviderPremisesCheckings.FirstOrDefault(x => x.IdCandidateProviderPremisesChecking == _idChecking);
                    mtb.CandidateProviderPremisesCheckings.Remove(checking);
                    this.mtbsSource.FirstOrDefault(x => x.IdCandidateProviderPremises == idCandidateProviderPremises).CandidateProviderPremisesCheckings.Remove(checking);
                    var candidateProviderPremisesChecking = await this.CandidateProviderService.GetCandidateProviderPremisesCheckingAsync(_idChecking);

                    var resultContext = await this.CandidateProviderService.DeleteCandidateProviderPremisesCheckingAsync(candidateProviderPremisesChecking);
                    if (resultContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();

                this.StateHasChanged();
            }
        }

        private void SetZIPCode(ChangeEventArgs<int?, LocationVM> args)
        {
            if (args.Value != null)
            {
                this.candidateProviderPremisesVM.ZipCode = args.ItemData.PostCode.ToString();
                return;
            }

            if (this.candidateProviderPremisesVM is not null)
            {
                if (this.candidateProviderPremisesVM.IdLocation.HasValue)
                {
                    var location = this.allLocations.FirstOrDefault(x => x.idLocation == this.candidateProviderPremisesVM.IdLocation.Value);
                    if (location is not null)
                    {
                        this.candidateProviderPremisesVM.ZipCode = location.PostCode.ToString();
                    }
                }
                else
                {
                    this.candidateProviderPremisesVM.ZipCode = string.Empty;
                }
            }
        }

        private void QueryCellInfo(QueryCellInfoEventArgs<CandidateProviderPremisesVM> args)
        {
            if (this.CandidateProviderVM.IdApplicationStatus.HasValue)
            {
                var mtb = this.mtbsSource.FirstOrDefault(x => x.IdCandidateProviderPremises == args.Data.IdCandidateProviderPremises);
                var documentForComplianceWithFireSafetyRulesAndRegulations = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == this.documentForComplianceWithFireSafetyRulesAndRegulationsKv && x.HasUploadedFile);
                var documentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == this.documentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirementsKv && x.HasUploadedFile);
                var documentsForThePresenceOfMTBInAccordanceWithTheDOS = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == this.documentsForThePresenceOfMTBInAccordanceWithTheDOSKv && x.HasUploadedFile);
                var dDocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfession = this.mtbsSource.Any(x => x.CandidateProviderPremisesDocuments.Any(y => y.IdDocumentType == this.dDocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfessionKv && y.HasUploadedFile));

                if (documentForComplianceWithFireSafetyRulesAndRegulations is null || documentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements is null
                    || documentsForThePresenceOfMTBInAccordanceWithTheDOS is null || !dDocumentsForThePresenceOfMTBInAccordanceWithTheDOSForTheAcquisitionOfQualificationByProfession)
                {
                    args.Cell.AddClass(new string[] { "color-elements" });
                }
            }
        }

        private async Task ShowDOSInfoBtn(DocVM doc)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (doc is not null)
                {
                    this.docInfoModal.OpenModal(doc.RequirementsMaterialBase, "Изисквания към материалната база");
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

        private void OnCheckingModalSubmit(CandidateProviderPremisesCheckingVM candidateProviderTrainerCheckingVM)
        {
            this.mtbsSource
                .FirstOrDefault(c => c.IdCandidateProviderPremises == candidateProviderTrainerCheckingVM.IdCandidateProviderPremises)
                .CandidateProviderPremisesCheckings.Add(candidateProviderTrainerCheckingVM);

            this.StateHasChanged();
        }

        private async Task NextMTBBtn()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var nextId = await this.mtbsGrid.GetRowIndexByPrimaryKeyAsync(this.candidateProviderPremisesVM!.IdCandidateProviderPremises) + 1;
                if (nextId < this.mtbsSource.Count)
                {
                    this.loading = false;

                    await this.mtbsGrid.SelectRowAsync(nextId);
                }

                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task PreviousMTBBtn()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var previousId = await this.mtbsGrid.GetRowIndexByPrimaryKeyAsync(this.candidateProviderPremisesVM!.IdCandidateProviderPremises) - 1;
                if (previousId == -1)
                {
                    previousId = 0;
                }

                if (previousId >= 0)
                {
                    this.loading = false;

                    await this.mtbsGrid.SelectRowAsync(previousId);
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
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                this.SpinnerShow();

                var result = await ReportWord();
                if (result == null)
                {
                    await this.ShowErrorAsync("Неуспешно генериране на справка!");
                    return;
                }

                if (this.IsCPO)
                {
                    await this.JsRuntime.SaveAs(BaseHelper.ConvertCyrToLatin("Справка_материално-технически бази") + ".pdf", result.ToArray());
                }
                else
                {

                }
            }
            finally
            {
                this.SpinnerHide();
                this.loading = false;
            }
        }

        private async Task<MemoryStream> ReportWord()
        {
            if (this.IsCPO)
            {
                try
                {
                    //Get resource document
                    var resources_Folder = Directory.GetCurrentDirectory() + @"\wwwroot\TemplateDocuments";
                    string documentName = @"\CPOMTB\Spravka_mtb.docx";
                    FileStream template = new FileStream($@"{resources_Folder}{documentName}", FileMode.Open,
                        FileAccess.Read, FileShare.ReadWrite);
                    WordDocument document = new WordDocument(template, FormatType.Docx);
                    DocIORenderer render = new DocIORenderer();

                    //Merge fields
                    string[] fieldNames = new string[] { "ProviderName", "PoviderBulstat" };
                    string title =
                        $"ЦПО {this.CandidateProviderVM?.ProviderName ?? ""} към {CandidateProviderVM?.ProviderOwner ?? ""}";
                    string[] fieldValues = new string[] { title, CandidateProviderVM?.PoviderBulstat ?? "" };

                    document.MailMerge.Execute(fieldNames, fieldValues);

                    //Navigate to first bookmar with list of mtbs
                    BookmarksNavigator bookNav = new BookmarksNavigator(document);
                    bookNav.MoveToBookmark("MTBsList", true, false);

                    #region Paragraphs

                    #region MTBParagraph

                    //Create new paragraph
                    IWParagraphStyle paragraphStyle = document.AddParagraphStyle("MTBParagraph");
                    paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;

                    #endregion

                    #region MTBSubtitle

                    //Create new paragraph
                    paragraphStyle = document.AddParagraphStyle("MTBSubtitle");
                    paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                    paragraphStyle.CharacterFormat.Bold = true;

                    #endregion

                    #region MTBHeadingParagraph

                    paragraphStyle = document.AddParagraphStyle("MTBHeadingParagraph");
                    paragraphStyle.ParagraphFormat.HorizontalAlignment = HorizontalAlignment.Left;
                    paragraphStyle.ParagraphFormat.AfterSpacing = 0;
                    paragraphStyle.ParagraphFormat.BackColor = Color.FromArgb(231, 230, 230);

                    #endregion

                    #endregion

                    ListStyle mtbsListStyle = document.AddListStyle(ListType.Numbered, "MTBsList");

                    WListLevel mtbsListLevel = mtbsListStyle.Levels[0];
                    mtbsListLevel.FollowCharacter = FollowCharacterType.Space;
                    mtbsListLevel.CharacterFormat.FontSize = (float)10;
                    mtbsListLevel.TextPosition = 1;

                    #region CharacterFormat

                    //CharacterFormat
                    WCharacterFormat mtbsCharacterFormat = new WCharacterFormat(document);
                    mtbsCharacterFormat.FontName = "Calibri";
                    mtbsCharacterFormat.FontSize = 12;
                    mtbsCharacterFormat.Position = 0;
                    mtbsCharacterFormat.Italic = false;
                    mtbsCharacterFormat.Bold = false;

                    #endregion

                    #region DrawListOfMTB

                    int row = 0;
                    foreach (var premise in this.mtbsSource)
                    {
                        row++;
                        IWParagraph membersParagraph = new WParagraph(document);

                        bookNav.InsertParagraph(membersParagraph);

                        membersParagraph.AppendText(row.ToString() + ". " + premise?.PremisesName ?? "")
                            .ApplyCharacterFormat(mtbsCharacterFormat);
                        membersParagraph.ApplyStyle("MTBParagraph");
                    }

                    #endregion

                    //Navigate to bookmark with information for every mtb and create new paragraph 
                    bookNav.MoveToBookmark("MTBs", true, false);

                    mtbsListStyle = document.AddListStyle(ListType.Numbered, "MTB");

                    mtbsListLevel = mtbsListStyle.Levels[0];
                    mtbsListLevel.FollowCharacter = FollowCharacterType.Space;
                    mtbsListLevel.CharacterFormat.FontSize = (float)9.5;
                    mtbsListLevel.TextPosition = 1;

                    row = 0;

                    // Draw data for every mtb
                    foreach (var premise in this.mtbsSource)
                    {
                        row++;
                        var premiseFromDb = await CandidateProviderService.GetCandidateProviderPremisesByIdAsync(
                            new CandidateProviderPremisesVM()
                            {
                                IdCandidateProviderPremises = premise.IdCandidateProviderPremises
                            });
                        IWParagraph premisesParagraph = new WParagraph(document);

                        List<CandidateProviderPremisesRoomVM> premisesRoomsSource =
                            (await CandidateProviderService
                                .GetAllCandidateProviderPremisesRoomsByCandidateProviderPremisesIdAsync(
                                    new CandidateProviderPremisesRoomVM()
                                    {
                                        IdCandidateProviderPremises = premise.IdCandidateProviderPremises
                                    })).OrderBy(x => x.PremisesRoomName).ToList();

                        List<CandidateProviderPremisesDocumentVM> mtbDocumentsSource =
                            (await CandidateProviderService
                                .GetAllCandidateProviderPremisesDocumentsByCandidateProviderPremisesIdAsync(
                                    new CandidateProviderPremisesDocumentVM()
                                    {
                                        IdCandidateProviderPremises = premise.IdCandidateProviderPremises
                                    })).ToList();

                        premisesParagraph.AppendText($"{row.ToString()}. {premiseFromDb.PremisesName ?? ""}")
                            .ApplyCharacterFormat(mtbsCharacterFormat);
                        premisesParagraph.ApplyStyle("MTBHeadingParagraph");
                        bookNav.InsertParagraph(premisesParagraph);

                        premisesParagraph = new WParagraph(document);
                        premisesParagraph.AppendText("Данни за материално-техническа база");
                        premisesParagraph.ApplyStyle("MTBSubtitle");
                        bookNav.InsertParagraph(premisesParagraph);

                        #region MTBData

                        premisesParagraph = new WParagraph(document);

                        premisesParagraph.AppendText("Кратко описание: " + premiseFromDb.PremisesNote + "\n" +
                                                     "Населено място: " +
                                                     ((await this.LocationService
                                                             .GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(
                                                                 this.candidateProviderPremisesVM.IdLocation ?? default))
                                                         ?.DisplayJoinedNames ?? "") + "\n");
                        premisesParagraph.AppendText("Адрес: " + premiseFromDb.ProviderAddress + "\t" + "Пощ. код: " +
                                                     premiseFromDb.ZipCode + "\t" + "Телефон:" + premiseFromDb.Phone +
                                                     "\n");
                        premisesParagraph.AppendText("Форма на собственост: " + kvMTBOwnershipSource
                            .FirstOrDefault(x => x.IdKeyValue == premiseFromDb.IdOwnership)?.Name ?? "");
                        premisesParagraph.AppendText("\tСтатус: " + kvMTBStatusSource
                            .FirstOrDefault(x => x.IdKeyValue == premiseFromDb.IdStatus)?.Name ?? "");
                        if (kvMTBStatusInactive != null &&
                            candidateProviderPremisesVM.IdStatus == kvMTBStatusInactive.IdKeyValue)
                        {
                            premisesParagraph.AppendText("\nНеактивен от: " + premiseFromDb.InactiveDate?.ToString() ??
                                                         "");
                        }

                        bookNav.InsertParagraph(premisesParagraph);

                        #endregion

                        premisesParagraph = new WParagraph(document);
                        premisesParagraph.AppendText("Данни за помещения");
                        premisesParagraph.ApplyStyle("MTBSubtitle");
                        bookNav.InsertParagraph(premisesParagraph);

                        #region PremisesRoomsTable

                        if (premisesRoomsSource.Count > 0)
                        {
                            WTable premisesRoomsTable = new WTable(document);
                            premisesRoomsTable.TableFormat.Borders.BorderType = BorderStyle.Thick;
                            WCharacterFormat premisesRoomsTableHeaderCharFormat = new WCharacterFormat(document);
                            WCharacterFormat premisesRoomsTableRowsCharFormat = new WCharacterFormat(document);
                            premisesRoomsTableHeaderCharFormat.Bold = true;
                            premisesRoomsTable.ResetCells(premisesRoomsSource.Count + 1, 3);
                            premisesRoomsTable.Rows[0].Height = 20;

                            premisesRoomsTable[0, 0].AddParagraph().AppendText("Помещение")
                                .ApplyCharacterFormat(premisesRoomsTableHeaderCharFormat);
                            premisesRoomsTable[0, 1].AddParagraph().AppendText("Вид на помещението")
                                .ApplyCharacterFormat(premisesRoomsTableHeaderCharFormat);
                            premisesRoomsTable[0, 2].AddParagraph().AppendText("Провеждано обучение")
                                .ApplyCharacterFormat(premisesRoomsTableHeaderCharFormat);

                            for (int i = 0; i < premisesRoomsSource.Count; i++)
                            {
                                if (premisesRoomsSource[i].IdPremisesType != 0)
                                {
                                    premisesRoomsSource[i].PremisesTypeName = this.kvPremisesTypeSource
                                                                                  .FirstOrDefault(x =>
                                                                                      x.IdKeyValue ==
                                                                                      premisesRoomsSource[i].IdPremisesType)
                                                                                  ?.Name ??
                                                                              "";
                                }

                                if (premisesRoomsSource[i].IdUsage != 0)
                                {
                                    premisesRoomsSource[i].UsageName = this.kvTrainingTypeSource
                                        .FirstOrDefault(x => x.IdKeyValue == premisesRoomsSource[i].IdUsage)?.Name ?? "";
                                }

                                premisesRoomsTable[i + 1, 0].AddParagraph()
                                    .AppendText(premisesRoomsSource[i].PremisesRoomName ?? "")
                                    .ApplyCharacterFormat(premisesRoomsTableRowsCharFormat);
                                premisesRoomsTable[i + 1, 1].AddParagraph()
                                    .AppendText(premisesRoomsSource[i].PremisesTypeName ?? "")
                                    .ApplyCharacterFormat(premisesRoomsTableRowsCharFormat);
                                premisesRoomsTable[i + 1, 2].AddParagraph()
                                    .AppendText(premisesRoomsSource[i].UsageName ?? "")
                                    .ApplyCharacterFormat(premisesRoomsTableRowsCharFormat);
                            }

                            bookNav.InsertTable(premisesRoomsTable);
                        }

                        #endregion

                        premisesParagraph = new WParagraph(document);
                        premisesParagraph.AppendText("Данни за документи");
                        premisesParagraph.ApplyStyle("MTBSubtitle");
                        bookNav.InsertParagraph(premisesParagraph);

                        #region PremisesDocumentsTable

                        if (mtbDocumentsSource.Count > 0)
                        {
                            WTable premiseDocumentsTable = new WTable(document);
                            premiseDocumentsTable.TableFormat.Borders.BorderType = BorderStyle.Thick;
                            WCharacterFormat premisesDocumentsTableHeaderCharFormat = new WCharacterFormat(document);
                            WCharacterFormat premisesDocumentsTableRowsCharFormat = new WCharacterFormat(document);
                            premisesDocumentsTableHeaderCharFormat.Bold = true;
                            premiseDocumentsTable.ResetCells(mtbDocumentsSource.Count + 1, 5);
                            premiseDocumentsTable.Rows[0].Height = 20;

                            premiseDocumentsTable[0, 0].AddParagraph().AppendText("Вид на документа")
                                .ApplyCharacterFormat(premisesDocumentsTableHeaderCharFormat);
                            premiseDocumentsTable[0, 1].AddParagraph().AppendText("Описание на документа")
                                .ApplyCharacterFormat(premisesDocumentsTableHeaderCharFormat);
                            premiseDocumentsTable[0, 2].AddParagraph().AppendText("Прикачен файл")
                                .ApplyCharacterFormat(premisesDocumentsTableHeaderCharFormat);
                            premiseDocumentsTable[0, 3].AddParagraph().AppendText("Дата на прикачване")
                                .ApplyCharacterFormat(premisesDocumentsTableHeaderCharFormat);
                            premiseDocumentsTable[0, 4].AddParagraph().AppendText("Прикачено от")
                                .ApplyCharacterFormat(premisesDocumentsTableHeaderCharFormat);

                            for (int i = 0; i < mtbDocumentsSource.Count; i++)
                            {
                                var docName = this.kvDocumentTypeSource.FirstOrDefault(x =>
                                    x.IdKeyValue == mtbDocumentsSource[i].IdDocumentType);
                                if (docName is not null)
                                {
                                    mtbDocumentsSource[i].DocumentTypeName = docName.Name;
                                }

                                mtbDocumentsSource[i].UploadedByName =
                                    await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(
                                        mtbDocumentsSource[i].IdCreateUser);

                                if (mtbDocumentsSource[i].HasUploadedFile)
                                {
                                    await this.SetFileNameAsync(mtbDocumentsSource[i]);
                                }

                                premiseDocumentsTable[i + 1, 0].AddParagraph()
                                    .AppendText(mtbDocumentsSource[i].DocumentTypeName ?? "")
                                    .ApplyCharacterFormat(premisesDocumentsTableRowsCharFormat);
                                premiseDocumentsTable[i + 1, 1].AddParagraph()
                                    .AppendText(mtbDocumentsSource[i].DocumentTitle ?? "")
                                    .ApplyCharacterFormat(premisesDocumentsTableRowsCharFormat);

                                if (!string.IsNullOrEmpty(mtbDocumentsSource[i].UploadedFileName))
                                {
                                    var files = mtbDocumentsSource[i].FileName
                                        ?.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToArray();
                                    if (files is not null)
                                    {
                                        foreach (var item in files)
                                        {
                                            premiseDocumentsTable[i + 1, 2].AddParagraph().AppendText(item ?? "")
                                                .ApplyCharacterFormat(premisesDocumentsTableRowsCharFormat);
                                        }
                                    }
                                }
                                else
                                {
                                    premiseDocumentsTable[i + 1, 2].AddParagraph()
                                        .AppendText(mtbDocumentsSource[i].FileName ?? "")
                                        .ApplyCharacterFormat(premisesDocumentsTableRowsCharFormat);
                                }

                                premiseDocumentsTable[i + 1, 3].AddParagraph()
                                    .AppendText(mtbDocumentsSource[i].CreationDate.ToString("dd.MMMM.yyyy") ?? "")
                                    .ApplyCharacterFormat(premisesDocumentsTableRowsCharFormat);
                                premiseDocumentsTable[i + 1, 4].AddParagraph()
                                    .AppendText(mtbDocumentsSource[i].UploadedByName ?? "")
                                    .ApplyCharacterFormat(premisesDocumentsTableRowsCharFormat);
                            }

                            bookNav.InsertTable(premiseDocumentsTable);
                        }

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
                catch
                {

                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
