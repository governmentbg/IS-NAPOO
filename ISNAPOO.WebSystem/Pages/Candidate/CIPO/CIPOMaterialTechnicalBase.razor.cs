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
using ISNAPOO.Core.HelperClasses;
using Microsoft.JSInterop;
using ISNAPOO.WebSystem.Resources;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOMaterialTechnicalBase : BlazorBaseComponent
    {
        public SfGrid<CandidateProviderPremisesVM> mtbsGrid = new SfGrid<CandidateProviderPremisesVM>();
        private SfGrid<CandidateProviderPremisesRoomVM> premisesRoomsGrid = new SfGrid<CandidateProviderPremisesRoomVM>();
        private SfGrid<CandidateProviderPremisesDocumentVM> mtbDocumentsGrid = new SfGrid<CandidateProviderPremisesDocumentVM>();
        private SfAutoComplete<int?, LocationVM> sfAutoCompleteLocation = new SfAutoComplete<int?, LocationVM>();
        private CIPOCandidateProviderPremisesDocumentModal candidateProviderPremisesDocumentModal = new CIPOCandidateProviderPremisesDocumentModal();
        private CandidateProviderPremisesCheckingModal candidateProviderPremisesCheckingModal = new CandidateProviderPremisesCheckingModal();
        private MTBStatusModal mTBStatusModal = new MTBStatusModal();

        private List<CandidateProviderPremisesVM> mtbsSource = new List<CandidateProviderPremisesVM>();
        private List<CandidateProviderPremisesRoomVM> premisesRoomsSource = new List<CandidateProviderPremisesRoomVM>();
        private List<CandidateProviderPremisesDocumentVM> mtbDocumentsSource = new List<CandidateProviderPremisesDocumentVM>();
        private IEnumerable<KeyValueVM> kvMTBOwnershipSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvMTBStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvTrainingTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvPremisesTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvVQSSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvDocumentTypeSource = new List<KeyValueVM>();
        private IEnumerable<LocationVM> locationSource = new List<LocationVM>();
        private IEnumerable<LocationVM> allLocations = new List<LocationVM>();
        private IEnumerable<SpecialityVM> specialitiesSource = new List<SpecialityVM>();
        public CandidateProviderPremisesVM candidateProviderPremisesVM = new CandidateProviderPremisesVM();
        private List<CandidateProviderPremisesVM> candidateProviderPremisesListForGrid = new List<CandidateProviderPremisesVM>();
        private bool isAddButtonClicked = false;
        private bool showUnsavedChangesConfirmDialog = false;
        private int idTest = 0;
        private Dictionary<string, CandidateProviderPremisesVM> mtbsAsDictionary = new Dictionary<string, CandidateProviderPremisesVM>();
        private CIPOCandidateProviderPremisesRoomModal candidateProviderPremisesRoomModal = new CIPOCandidateProviderPremisesRoomModal();
        private RowSelectEventArgs<CandidateProviderPremisesVM> selectArgs = new RowSelectEventArgs<CandidateProviderPremisesVM>();
        private List<CandidateProviderPremisesVM> selectedPremises = new List<CandidateProviderPremisesVM>();
        public double selectedRowIdx = 0;
        private string lastAddedMTB = string.Empty;
        private int selectedMTBId = 0;
        private bool isInitialRender = true;
        private KeyValueVM kvMTBStatusActive;
        private int docTypeOne;
        private int docTypeTwo;
        private int docTypeThree;
        private int docTypeFour;
        private string premisesNameInformation = string.Empty;
        private bool disableNextBtn = false;
        private bool disablePreviousBtn = false;
        private bool isSpecialityGridButtonClicked = false;
        private bool isMTBSelected = false;

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool IsApplicationChange { get; set; } = false;

        [Parameter]
        public bool DisableAllFields { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

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

        protected override async Task OnInitializedAsync()
        {
            this.SpinnerShow();

            this.editContext = new EditContext(this.candidateProviderPremisesVM);
            this.FormTitle = "Материално-техническа база";

            this.kvDocumentTypeSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MTBDocumentType")).Where(x => x.DefaultValue3 != null & x.DefaultValue3!.Contains("CIPO")).ToList();
            this.docTypeFour = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DocumentsForThePresenceOfMTBCIPO").IdKeyValue;
            this.docTypeThree = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DocumentsForThePresenceOfMTBInAccordanceCIPO").IdKeyValue;
            this.docTypeTwo = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DocumentsIssuedByTheCompetentAuthoritiesForMTBComplianceWithHealthRequirements").IdKeyValue;
            this.docTypeOne = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DocumentForComplianceWithFireSafetyRulesAndRegulations").IdKeyValue;
            this.allLocations = await this.LocationService.GetAllLocationsAsync();

            if (!this.IsApplicationChange)
            {
                this.mtbsSource = (List<CandidateProviderPremisesVM>)(await this.CandidateProviderService.GetCandidateProviderByIdAsync(this.CandidateProviderVM)).CandidateProviderPremises;
                foreach (var mtb in this.mtbsSource)
                {
                    if (!this.mtbsAsDictionary.ContainsKey(mtb.PremisesName))
                    {
                        this.mtbsAsDictionary.Add(mtb.PremisesName, new CandidateProviderPremisesVM());
                    }

                    this.mtbsAsDictionary[mtb.PremisesName] = mtb;

                    foreach (var premisesSpeciality in mtb.CandidateProviderPremisesSpecialities)
                    {
                        var speciality = this.specialitiesSource.FirstOrDefault(x => x.IdSpeciality == premisesSpeciality.IdSpeciality);
                        mtb.SelectedSpecialities.Add(speciality);
                    }
                }
            }

            this.candidateProviderPremisesListForGrid = this.mtbsSource;
            this.kvMTBOwnershipSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseOwnership");
            this.kvMTBStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseStatus");
            this.kvMTBStatusActive = this.kvMTBStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "Active");

            if (this.candidateProviderPremisesVM.IdLocation != null)
            {
                LocationVM location = await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.candidateProviderPremisesVM.IdLocation ?? default);
                var locations = new List<LocationVM>();
                locations.Add(location);
                this.locationSource = locations;
            }

            this.kvPremisesTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RoomType");
            this.kvTrainingTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
            this.kvVQSSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");

            this.candidateProviderPremisesVM.IdStatus = this.kvMTBStatusActive.IdKeyValue;

            this.SpinnerHide();
        }

        private async void DeleteChecking(int _idChecking, int idCandidateProviderPremises)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            var candidateProviderPremisesChecking = await this.CandidateProviderService.GetCandidateProviderPremisesCheckingAsync(_idChecking);

            var result = await this.CandidateProviderService.DeleteCandidateProviderPremisesCheckingAsync(candidateProviderPremisesChecking);
            if (result.HasErrorMessages)
            {
                this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
            }
            else
            {
                this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                var checking = this.candidateProviderPremisesListForGrid.First(x => x.IdCandidateProviderPremises == candidateProviderPremisesChecking.IdCandidateProviderPremises).CandidateProviderPremisesCheckings.First(x => x.IdCandidateProviderPremisesChecking == _idChecking);
                this.candidateProviderPremisesListForGrid.First(x => x.IdCandidateProviderPremises == candidateProviderPremisesChecking.IdCandidateProviderPremises).CandidateProviderPremisesCheckings.Remove(checking);
            }

            this.StateHasChanged();
        }
        private async Task OnCheckingModalSubmit(CandidateProviderPremisesCheckingVM candidateProviderTrainerCheckingVM)
        {
            this.candidateProviderPremisesListForGrid
                .FirstOrDefault(c => c.IdCandidateProviderPremises == candidateProviderTrainerCheckingVM.IdCandidateProviderPremises)
                .CandidateProviderPremisesCheckings.Add(candidateProviderTrainerCheckingVM);

            this.StateHasChanged();
        }

        private async Task AddCheckingToMTBsClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            if (!this.selectedPremises.Any())
            {
                this.ShowErrorAsync("Моля, изберете материална база/и от списъка!");
            }
            else
            {
                await this.candidateProviderPremisesCheckingModal.OpenModal(this.selectedPremises);
            }
        }
        public override void SubmitHandler()
        {
            this.editContext = new EditContext(this.candidateProviderPremisesVM);
            this.editContext.EnableDataAnnotationsValidation();

            var isValidEditContextGeneralData = this.editContext.Validate();

            if (isValidEditContextGeneralData)
            {
                var mtb = this.CandidateProviderVM.CandidateProviderPremises.FirstOrDefault(x => x.IdCandidateProviderPremises == this.candidateProviderPremisesVM.IdCandidateProviderPremises);
                ICollection<CandidateProviderPremisesSpecialityVM> specialities = new List<CandidateProviderPremisesSpecialityVM>();
                if (mtb != null)
                {
                    specialities = mtb.CandidateProviderPremisesSpecialities;
                    this.CandidateProviderVM.CandidateProviderPremises.Remove(mtb);
                    this.candidateProviderPremisesVM.CandidateProviderPremisesSpecialities = specialities;
                }

                if (!string.IsNullOrEmpty(this.candidateProviderPremisesVM.PremisesName))
                {
                    this.CandidateProviderVM.CandidateProviderPremises.Add(this.candidateProviderPremisesVM);
                }

                if (this.isAddButtonClicked)
                {
                    this.lastAddedMTB = this.candidateProviderPremisesVM.PremisesName;
                }
            }
        }

        private async Task AddNewMTBClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.isAddButtonClicked = true;

            if (this.editContext.IsModified())
            {
                bool confirmed = await this.ShowConfirmDialogAsync("Имате незапазени промени! Сигурни ли сте, че искате да продължите?");
                if (confirmed)
                {
                    this.candidateProviderPremisesVM = new CandidateProviderPremisesVM()
                    {
                        IdStatus = this.kvMTBStatusActive.IdKeyValue
                    };

                    this.editContext = new EditContext(this.candidateProviderPremisesVM);
                    this.premisesRoomsSource.Clear();
                    this.premisesRoomsGrid.Refresh();
                    this.mtbDocumentsSource.Clear();
                    this.mtbDocumentsGrid.Refresh();
                    this.StateHasChanged();

                }
            }
            else
            {
                this.candidateProviderPremisesVM = new CandidateProviderPremisesVM()
                {
                    IdStatus = this.kvMTBStatusActive.IdKeyValue
                };

                if (this.premisesRoomsSource.Any())
                {
                    this.premisesRoomsSource.Clear();
                    this.premisesRoomsGrid.Refresh();
                    this.mtbDocumentsSource.Clear();
                    this.mtbDocumentsGrid.Refresh();
                    this.StateHasChanged();
                }
            }

            this.premisesNameInformation = string.Empty;
        }

        private async Task MTBSelectedHandler(RowSelectEventArgs<CandidateProviderPremisesVM> args)
        {
            this.selectedRowIdx = (await this.mtbsGrid.GetSelectedRowIndexesAsync()).FirstOrDefault();
            this.selectedMTBId = args.Data.IdCandidateProviderPremises;

            this.isAddButtonClicked = false;
            this.isMTBSelected = true;
            this.lastAddedMTB = string.Empty;
            this.selectedPremises.Clear();
            this.selectedPremises = await this.mtbsGrid.GetSelectedRecordsAsync();

            this.selectArgs = args;

            if (!this.editContext.IsModified())
            {
                this.selectedMTBId = 0;
                this.isMTBSelected = false;

                this.candidateProviderPremisesVM = new CandidateProviderPremisesVM() { IdStatus = this.kvMTBStatusActive.IdKeyValue };
                this.premisesRoomsSource.Clear();
                this.mtbDocumentsSource.Clear();
                await this.ChangeMTB(this.idTest);
                this.idTest = 0;
            }
            else
            {
                bool confirmed = await this.ShowConfirmDialogAsync("Имате незапазени промени! Сигурни ли сте, че искате да смените МТБ?");
                if (confirmed)
                {
                    this.candidateProviderPremisesVM = new CandidateProviderPremisesVM() { IdStatus = this.kvMTBStatusActive.IdKeyValue };
                    this.premisesRoomsSource.Clear();
                    this.mtbDocumentsSource.Clear();
                    await this.ChangeMTB(this.idTest);
                    this.idTest = 0;
                }
            }
        }

        private async Task ChangeMTB(int id)
        {
            if (!this.isInitialRender)
            {
                this.SpinnerShow();
            }

            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;

                this.isInitialRender = false;

                this.candidateProviderPremisesVM = await this.CandidateProviderService.GetCandidateProviderPremisesByIdAsync(new CandidateProviderPremisesVM() { IdCandidateProviderPremises = id });
                if (this.candidateProviderPremisesVM.IdLocation != null)
                {
                    LocationVM location = await this.LocationService.GetLocationWithMunicipalityAndDistrictIncludedByIdAsync(this.candidateProviderPremisesVM.IdLocation ?? default);
                    var locations = new List<LocationVM>();
                    locations.Add(location);
                    this.locationSource = locations;
                }

                this.premisesRoomsSource = (List<CandidateProviderPremisesRoomVM>)this.candidateProviderPremisesVM.CandidateProviderPremisesRooms;
                foreach (var room in this.premisesRoomsSource)
                {
                    if (room.IdPremisesType != 0)
                    {
                        room.PremisesTypeName = this.kvPremisesTypeSource.FirstOrDefault(x => x.IdKeyValue == room.IdPremisesType).Name;
                    }

                    if (room.IdUsage != 0)
                    {
                        room.UsageName = this.kvTrainingTypeSource.FirstOrDefault(x => x.IdKeyValue == room.IdUsage).Name;
                    }
                }

                this.mtbDocumentsSource = (List<CandidateProviderPremisesDocumentVM>)this.candidateProviderPremisesVM.CandidateProviderPremisesDocuments.ToList();
                foreach (var document in this.mtbDocumentsSource)
                {
                    var docTypeName = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType);
                    if (docTypeName is not null)
                    {
                        document.DocumentTypeName = docTypeName.Name;
                    }

                    document.UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);

                    if (document.HasUploadedFile)
                    {
                        await this.SetFileNameAsync(document);
                    }
                }

                this.premisesNameInformation = this.candidateProviderPremisesVM?.PremisesName;

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

                this.editContext = new EditContext(this.candidateProviderPremisesVM);
                this.editContext.MarkAsUnmodified();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public async Task RefreshPremisesData()
        {
            this.mtbsAsDictionary.Clear();

            var premisesFromDb = (List<CandidateProviderPremisesVM>)(await this.CandidateProviderService.GetCandidateProviderByIdAsync(this.CandidateProviderVM)).CandidateProviderPremises;
            this.mtbsSource = premisesFromDb;
            this.CandidateProviderVM.CandidateProviderPremises = premisesFromDb;
            this.candidateProviderPremisesListForGrid = premisesFromDb;
            foreach (var mtb in this.mtbsSource)
            {
                if (!this.mtbsAsDictionary.ContainsKey(mtb.PremisesName))
                {
                    this.mtbsAsDictionary.Add(mtb.PremisesName, new CandidateProviderPremisesVM());
                }

                this.mtbsAsDictionary[mtb.PremisesName] = mtb;

                foreach (var premisesSpeciality in mtb.CandidateProviderPremisesSpecialities)
                {
                    var speciality = this.specialitiesSource.FirstOrDefault(x => x.IdSpeciality == premisesSpeciality.IdSpeciality);
                    mtb.SelectedSpecialities.Add(speciality);
                }
            }

            await this.mtbsGrid.Refresh();
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
            this.premisesRoomsSource = (List<CandidateProviderPremisesRoomVM>)(await this.CandidateProviderService.GetAllCandidateProviderPremisesRoomsByCandidateProviderPremisesIdAsync(new CandidateProviderPremisesRoomVM() { IdCandidateProviderPremises = candidateProviderPremisesRoomVM.IdCandidateProviderPremises }));
            foreach (var room in this.premisesRoomsSource)
            {
                if (room.IdPremisesType != 0)
                {
                    room.PremisesTypeName = this.kvPremisesTypeSource.FirstOrDefault(x => x.IdKeyValue == room.IdPremisesType).Name;
                }

                if (room.IdUsage != 0)
                {
                    room.UsageName = this.kvTrainingTypeSource.FirstOrDefault(x => x.IdKeyValue == room.IdUsage).Name;
                }
            }

            this.StateHasChanged();
        }

        private async Task AddNewRoomClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            await this.candidateProviderPremisesRoomModal.OpenModal(new CandidateProviderPremisesRoomVM() { IdCandidateProviderPremises = this.candidateProviderPremisesVM.IdCandidateProviderPremises }, this.kvTrainingTypeSource, this.kvPremisesTypeSource, this.candidateProviderPremisesVM.PremisesName);
        }

        private async Task DeleteRoom(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (confirmed)
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

                        this.premisesRoomsSource = (List<CandidateProviderPremisesRoomVM>)(await this.CandidateProviderService.GetAllCandidateProviderPremisesRoomsByCandidateProviderPremisesIdAsync(new CandidateProviderPremisesRoomVM() { IdCandidateProviderPremises = candidateProviderPremisesRoomVM.IdCandidateProviderPremises }));
                        foreach (var room in this.premisesRoomsSource)
                        {
                            room.PremisesTypeName = this.kvPremisesTypeSource.FirstOrDefault(x => x.IdKeyValue == room.IdPremisesType).Name;
                            room.UsageName = this.kvTrainingTypeSource.FirstOrDefault(x => x.IdKeyValue == room.IdUsage).Name;
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

        private async Task EditRoom(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM)
        {
            bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
            if (!hasPermission) { return; }

            await this.candidateProviderPremisesRoomModal.OpenModal(candidateProviderPremisesRoomVM, this.kvTrainingTypeSource, this.kvPremisesTypeSource, this.candidateProviderPremisesVM.PremisesName);
        }

        public IEnumerable<string> GetValidationMessages()
        {
            return this.editContext.GetValidationMessages().Select(c => $"{c} ({FormTitle})");
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

        private void MTBSelectingHandler(RowSelectingEventArgs<CandidateProviderPremisesVM> args)
        {
            this.idTest = this.idTest == 0 ? args.Data.IdCandidateProviderPremises : this.idTest;
            this.selectArgs.Data = args.Data;

            if (this.editContext is not null)
            {
                if (this.editContext.IsModified())
                {
                    args.Cancel = true;
                    this.showUnsavedChangesConfirmDialog = !this.showUnsavedChangesConfirmDialog;
                }
            }
        }

        private async Task DeleteDocument(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (confirmed)
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

                        this.mtbDocumentsSource = (List<CandidateProviderPremisesDocumentVM>)(await this.CandidateProviderService.GetAllCandidateProviderPremisesDocumentsByCandidateProviderPremisesIdAsync(new CandidateProviderPremisesDocumentVM() { IdCandidateProviderPremises = candidateProviderPremisesDocumentVM.IdCandidateProviderPremises }));


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

                        this.mtbsSource = (await this.CandidateProviderService.GetCandidateProviderPremisesWithAllIncludedByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider)).ToList();

                        await this.mtbsGrid.Refresh();
                        this.StateHasChanged();
                    }

                    this.candidateProviderPremisesVM.CandidateProviderPremisesDocuments.Remove(candidateProviderPremisesDocumentVM);
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task OnDownloadClick(string fileName, CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM)
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

                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CandidateProviderPremisesDocument>(candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument, fileName);
                if (hasFile)
                {
                    var documentStream = await this.UploadFileService.GetUploadedFileAsync<CandidateProviderPremisesDocument>(candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument, fileName);

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
            this.mtbDocumentsSource = (List<CandidateProviderPremisesDocumentVM>)(await this.CandidateProviderService.GetAllCandidateProviderPremisesDocumentsByCandidateProviderPremisesIdAsync(new CandidateProviderPremisesDocumentVM() { IdCandidateProviderPremises = candidateProviderPremisesDocumentVM.IdCandidateProviderPremises }));
            foreach (var document in this.mtbDocumentsSource)
            {
                document.DocumentTypeName = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType).Name;
                document.UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);

                if (document.HasUploadedFile)
                {
                    await this.SetFileNameAsync(document);
                }
            }

            this.mtbsSource = (await this.CandidateProviderService.GetCandidateProviderPremisesWithAllIncludedByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider)).ToList();

            await this.mtbsGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task AddNewDocumentClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            await this.candidateProviderPremisesDocumentModal.OpenModal(new CandidateProviderPremisesDocumentVM() { IdCandidateProviderPremises = this.candidateProviderPremisesVM.IdCandidateProviderPremises }, this.kvDocumentTypeSource);
        }

        private async Task SetFileNameAsync(CandidateProviderPremisesDocumentVM candidateProviderPremisesDocumentVM)
        {
            var settingResource = (await this.DataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var fileFullName = settingResource + "\\" + candidateProviderPremisesDocumentVM.UploadedFileName;
            if (Directory.Exists(fileFullName))
            {
                var files = Directory.GetFiles(fileFullName);
                files = files.Select(x => x.Split(($"\\{candidateProviderPremisesDocumentVM.IdCandidateProviderPremisesDocument}\\"), StringSplitOptions.RemoveEmptyEntries).LastOrDefault()).ToArray();
                candidateProviderPremisesDocumentVM.FileName = string.Join(Environment.NewLine, files);
            }
            else
            {
                candidateProviderPremisesDocumentVM.FileName = string.Empty;
            }
        }

        private void SetZIPCode(ChangeEventArgs<int?, LocationVM> args)
        {
            if (args.Value != null)
            {
                this.candidateProviderPremisesVM.ZipCode = args.ItemData.PostCode.ToString();
                return;
            }

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

        private void QueryCellInfo(QueryCellInfoEventArgs<CandidateProviderPremisesVM> args)
        {
            if (this.CandidateProviderVM.IdApplicationStatus.HasValue)
            {
                var mtb = this.mtbsSource.FirstOrDefault(x => x.IdCandidateProviderPremises == args.Data.IdCandidateProviderPremises);
                var docTypeFour = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == this.docTypeFour && x.HasUploadedFile);
                var docTypeThree = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == this.docTypeThree && x.HasUploadedFile);
                var docTypeTwo = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == this.docTypeTwo && x.HasUploadedFile);
                var docTypeOne = mtb.CandidateProviderPremisesDocuments.FirstOrDefault(x => x.IdDocumentType == this.docTypeOne && x.HasUploadedFile);

                if (docTypeFour is null || docTypeThree is null
                    || docTypeTwo is null || docTypeOne is null)
                {
                    args.Cell.AddClass(new string[] { "color-elements" });
                }
            }
        }
    }
}
