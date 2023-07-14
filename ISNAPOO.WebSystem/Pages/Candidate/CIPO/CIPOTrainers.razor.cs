using Data.Models.Data.Candidate;

using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Services.SPPOO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
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
using Syncfusion.DocIORenderer;
using Syncfusion.Drawing;
using Syncfusion.Pdf;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOTrainers : BlazorBaseComponent
    {
        public SfGrid<CandidateProviderTrainerVM> trainersGrid = new SfGrid<CandidateProviderTrainerVM>();
        private SfGrid<CandidateProviderTrainerDocumentVM> trainerDocumentsGrid = new SfGrid<CandidateProviderTrainerDocumentVM>();
        public EditContext editContextGeneralData;
        private ValidationMessageStore? messageStore;
        private ToastMsg toast = new ToastMsg();
        private CIPOCandidateProviderTrainerDocumentModal candidateProviderTrainerDocumentModal = new CIPOCandidateProviderTrainerDocumentModal();
        private TrainerStatusModal trainerStatusModal = new TrainerStatusModal();
        private RegiXDiplomaCheckModal diplomaCheckModal = new RegiXDiplomaCheckModal();
        private CandidateProviderTrainerCheckingModal candidateProviderTrainerCheckingModal = new CandidateProviderTrainerCheckingModal();
        private CIPOCandidateProviderTrainerSearchModal searchModal = new CIPOCandidateProviderTrainerSearchModal();
        public CandidateProviderTrainerVM candidateProviderTrainerVM = new CandidateProviderTrainerVM();
        private List<CandidateProviderTrainerVM> trainersSource = new List<CandidateProviderTrainerVM>();
        private List<CandidateProviderTrainerDocumentVM> trainerDocumentsSource = new List<CandidateProviderTrainerDocumentVM>();
        private List<CandidateProviderTrainerVM> candidateProviderTrainersListForGrid = new List<CandidateProviderTrainerVM>();
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
        private KeyValueVM kvCandidateProviderTrainerStatusActive;
        private KeyValueVM kvMiddleEducation = new KeyValueVM();
        private bool isAddButtonClicked = false;
        private bool isTrainerSelected = false;
        private bool trainerSelectedForSpeciality = false;
        private bool isSpecialityGridButtonClicked = false;
        private int idCandidateProviderTrainer = 0;
        private Dictionary<string, CandidateProviderTrainerVM> trainersAsDictionary = new Dictionary<string, CandidateProviderTrainerVM>();
        private RowSelectEventArgs<CandidateProviderTrainerVM> selectArgs = new RowSelectEventArgs<CandidateProviderTrainerVM>();
        private RowDeselectEventArgs<CandidateProviderTrainerVM> deselectArgs = new RowDeselectEventArgs<CandidateProviderTrainerVM>();
        private List<CandidateProviderTrainerVM> selectedTrainers = new List<CandidateProviderTrainerVM>();
        private BasicEGNValidation egnValidator = new BasicEGNValidation(null);
        public double selectedRowIdx = 0;
        private string lastAddedTrainer = string.Empty;
        private int selectedTrainerId = 0;
        private bool isInitialRender = true;
        private int retrainingDiplomaKv;
        private int declarationOfConsentKv;
        private int autobiographyKv;
        private int certificateKv;
        private int idTest = 0;
        private string identType = "ЕГН";
        private string trainerNameInformation = string.Empty;
        private bool disableNextBtn = false;
        private bool disablePreviousBtn = false;

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool IsApplicationChange { get; set; } = false;

        [Parameter]
        public bool DisableAllFields { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

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

        protected override async Task OnInitializedAsync()
        {
            this.SpinnerShow();

            this.FormTitle = "Консултанти";

            this.editContextGeneralData = new EditContext(this.candidateProviderTrainerVM);

            this.kvCandidateProviderTrainerStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderTrainerStatus");

            if (!this.IsApplicationChange)
            {
                this.trainersSource = (await this.CandidateProviderService.GetCandidateProviderByIdAsync(this.CandidateProviderVM)).CandidateProviderTrainers.OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ThenBy(x => x.SecondName).ToList();
                foreach (var trainer in this.trainersSource)
                {
                    var status = this.kvCandidateProviderTrainerStatusSource.FirstOrDefault(x => x.IdKeyValue == trainer.IdStatus);
                    if (status != null)
                    {
                        trainer.StatusName = status.Name;
                    }

                    if (!this.trainersAsDictionary.ContainsKey(trainer.FullName))
                    {
                        this.trainersAsDictionary.Add(trainer.FullName, new CandidateProviderTrainerVM());
                    }

                    this.trainersAsDictionary[trainer.FullName] = trainer;
                }
            }

            this.candidateProviderTrainersListForGrid = this.trainersSource;
            this.kvQualificationTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("QualificationType");
            this.kvTrainingQualificationTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingQualificationType");
            this.kvDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerDocumentType");
            this.certificateKv = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Certificate").IdKeyValue;
            this.autobiographyKv = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "Autobiography").IdKeyValue;
            this.declarationOfConsentKv = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "DeclarationOfConsent").IdKeyValue;
            this.retrainingDiplomaKv = this.kvDocumentTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "RetrainingDiploma").IdKeyValue;
            this.kvIndentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            this.kvSexSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Sex");

            await this.HandleOrderForNationalitiesSourceAsync();

            this.kvEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("Education");
            this.kvMiddleEducation = this.kvEducationSource.FirstOrDefault(x => x.KeyValueIntCode == "Education13");
            this.kvContractTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerContractType");
            this.kvVQSSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");

            this.kvEGN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "EGN");
            this.kvIDN = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "IDN");
            this.kvLNCh = await this.DataSourceService.GetKeyValueByIntCodeAsync("IndentType", "LNK");
            this.kvCandidateProviderTrainerStatusActive = await DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderTrainerStatus", "Active");

            this.candidateProviderTrainerVM.IdStatus = this.kvCandidateProviderTrainerStatusActive.IdKeyValue;

            this.SpinnerHide();
        }
        private async void DeleteChecking(CandidateProviderTrainerCheckingVM checking)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            var result = await this.CandidateProviderService.DeleteCandidateProviderTrainerCheckingAsync(checking);
            if (result.HasErrorMessages)
            {
                this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
            }
            else
            {
                this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
                this.candidateProviderTrainersListForGrid.First(x => x.IdCandidateProviderTrainer == checking.IdCandidateProviderTrainer).CandidateProviderTrainerCheckings.Remove(checking);
            }


            this.SpinnerHide();

        }
        private async Task AddCheckingToTrainersClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            if (!this.trainerSelectedForSpeciality)
            {
                this.toast.sfErrorToast.Content = "Моля, изберете преподавател/и от списъка!";
                await this.toast.sfErrorToast.ShowAsync();
            }
            else
            {
                CandidateProviderTrainerCheckingVM candidateProviderTrainerCheckingVM = new CandidateProviderTrainerCheckingVM();
                candidateProviderTrainerCheckingVM.IdCandidateProviderTrainer = idCandidateProviderTrainer;
                await this.candidateProviderTrainerCheckingModal.OpenModal(this.selectedTrainers, candidateProviderTrainerCheckingVM);
            }
        }

        private async Task OnCheckingModalSubmit(CandidateProviderTrainerCheckingVM candidateProviderTrainerCheckingVM)
        {

            this.candidateProviderTrainersListForGrid
                .FirstOrDefault(c => c.IdCandidateProviderTrainer == candidateProviderTrainerCheckingVM.IdCandidateProviderTrainer)
                .CandidateProviderTrainerCheckings.Add(candidateProviderTrainerCheckingVM);

            this.StateHasChanged();
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
                }
                else if (args.Value == this.kvLNCh.IdKeyValue)
                {
                    this.identType = "ЛНЧ";
                }
                else
                {
                    this.identType = "ИДН";
                }
            }
            else
            {
                this.identType = "ЕГН";
            }
        }

        private async Task AddNewTrainerClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.isAddButtonClicked = true;

            if (this.editContextGeneralData.IsModified())
            {
                bool confirmed = await this.ShowConfirmDialogAsync("Имате незапазени промени! Сигурни ли сте, че искате да продължите?");
                if (confirmed)
                {
                    this.ResetTrainerGridData();
                }
            }
            else
            {
                this.ResetTrainerGridData();
            }

            this.trainerNameInformation = string.Empty;
        }

        private void ResetTrainerGridData()
        {
            this.candidateProviderTrainerVM = new CandidateProviderTrainerVM()
            {
                IdStatus = this.kvCandidateProviderTrainerStatusActive.IdKeyValue,
                IdIndentType = this.kvEGN.IdKeyValue
            };

            if (this.trainerDocumentsSource.Any())
            {
                this.trainerDocumentsSource.Clear();
            }

            this.StateHasChanged();
        }

        public override void SubmitHandler()
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

            this.editContextGeneralData.EnableDataAnnotationsValidation();

            var isValidEditContextGeneralData = this.editContextGeneralData.Validate();
            if (isValidEditContextGeneralData)
            {
                var trainer = this.CandidateProviderVM.CandidateProviderTrainers.FirstOrDefault(x => x.IdCandidateProviderTrainer == this.candidateProviderTrainerVM.IdCandidateProviderTrainer);
                if (trainer != null)
                {
                    this.CandidateProviderVM.CandidateProviderTrainers.Remove(trainer);
                }

                if (!string.IsNullOrEmpty(this.candidateProviderTrainerVM.FirstName))
                {
                    this.CandidateProviderVM.CandidateProviderTrainers.Add(this.candidateProviderTrainerVM);
                }

                if (this.isAddButtonClicked)
                {
                    this.lastAddedTrainer = $"{this.candidateProviderTrainerVM.FirstName} {this.candidateProviderTrainerVM.FamilyName}";
                }
            }
        }

        private async Task TrainerSelectedHandler(RowSelectEventArgs<CandidateProviderTrainerVM> args)
        {
            this.selectedRowIdx = (await this.trainersGrid.GetSelectedRowIndexesAsync()).FirstOrDefault();
            this.selectedTrainerId = args.Data.IdCandidateProviderTrainer;

            this.isAddButtonClicked = false;
            this.isTrainerSelected = true;
            this.lastAddedTrainer = string.Empty;

            this.trainerSelectedForSpeciality = true;
            this.selectedTrainers.Clear();
            this.selectedTrainers = await this.trainersGrid.GetSelectedRecordsAsync();

            this.selectArgs = args;

            if (this.editContextGeneralData is not null)
            {
                if (!this.editContextGeneralData.IsModified())
                {
                    if (!this.selectedTrainers.Any())
                    {
                        this.trainerSelectedForSpeciality = false;
                    }

                    await this.ChangeTrainer(this.idTest);
                    this.idTest = 0;
                }
                else
                {
                    bool confirmed = await this.ShowConfirmDialogAsync("Имате незапазени промени! Сигурни ли сте, че искате да смените преподавателя?");
                    if (confirmed)
                    {
                        if (!this.selectedTrainers.Any())
                        {
                            this.trainerSelectedForSpeciality = false;
                        }

                        await this.ChangeTrainer(this.idTest);
                        this.idTest = 0;
                    }
                }
            }
        }

        private async Task ChangeTrainer(int id)
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

                this.candidateProviderTrainerVM = await this.CandidateProviderService.GetCandidateProviderTrainerByIdAsync(new CandidateProviderTrainerVM() { IdCandidateProviderTrainer = id });

                this.trainerDocumentsSource = this.candidateProviderTrainerVM.CandidateProviderTrainerDocuments.ToList();
                foreach (var document in this.trainerDocumentsSource)
                {
                    document.DocumentTypeName = this.kvDocumentTypeSource.FirstOrDefault(x => x.IdKeyValue == document.IdDocumentType).Name;
                    document.UploadedByName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(document.IdCreateUser);

                    if (document.HasUploadedFile)
                    {
                        await this.SetFileNameAsync(document);
                    }
                }

                this.editContextGeneralData = new EditContext(this.candidateProviderTrainerVM);

                this.trainerNameInformation = this.candidateProviderTrainerVM?.FullName;

                // проверява за последен/пръв запис в грида
                var idx = await this.trainersGrid.GetRowIndexByPrimaryKeyAsync(this.candidateProviderTrainerVM!.IdCandidateProviderTrainer);
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
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
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

        private void TrainerSelectingHandler(RowSelectingEventArgs<CandidateProviderTrainerVM> args)
        {
            this.idTest = this.idTest == 0 ? args.Data.IdCandidateProviderTrainer : this.idTest;
            this.selectArgs.Data = args.Data;
        }

        private void DeselectTrainer()
        {
            this.trainerDocumentsSource.Clear();
            this.candidateProviderTrainerVM = new CandidateProviderTrainerVM()
            {
                IdStatus = this.kvCandidateProviderTrainerStatusActive.IdKeyValue,
                IdIndentType = this.kvEGN.IdKeyValue
            };
        }

        public async Task RefreshTrainersData()
        {
            this.trainersAsDictionary.Clear();

            var trainersFromDb = (await this.CandidateProviderService.GetCandidateProviderByIdAsync(this.CandidateProviderVM)).CandidateProviderTrainers.OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ThenBy(x => x.SecondName).ToList();

            this.trainersSource = trainersFromDb;
            this.CandidateProviderVM.CandidateProviderTrainers = trainersFromDb;
            this.candidateProviderTrainersListForGrid = trainersFromDb;
            foreach (var trainer in this.trainersSource)
            {
                var status = this.kvCandidateProviderTrainerStatusSource.FirstOrDefault(x => x.IdKeyValue == trainer.IdStatus);
                if (status != null)
                {
                    trainer.StatusName = status.Name;
                }

                if (!this.trainersAsDictionary.ContainsKey(trainer.FullName))
                {
                    this.trainersAsDictionary.Add(trainer.FullName, new CandidateProviderTrainerVM());
                }

                this.trainersAsDictionary[trainer.FullName] = trainer;
            }

            await this.trainersGrid.Refresh();
        }

        private async Task AddNewDocumentClickHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            await this.candidateProviderTrainerDocumentModal.OpenModal(new CandidateProviderTrainerDocumentVM() { IdCandidateProviderTrainer = this.candidateProviderTrainerVM.IdCandidateProviderTrainer }, this.kvDocumentTypeSource);
        }

        private async Task DeleteDocument(CandidateProviderTrainerDocumentVM candidateProviderTrainerDocumentVM)
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

                        this.trainersSource = (await this.CandidateProviderService.GetCandidateProviderTrainersByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider)).OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ThenBy(x => x.SecondName).ToList();

                        await this.trainersGrid.Refresh();
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

            this.trainersSource = (await this.CandidateProviderService.GetCandidateProviderTrainersByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider)).OrderBy(x => x.FirstName).ThenBy(x => x.FamilyName).ThenBy(x => x.SecondName).ToList();

            await this.trainersGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task OnDownloadClick(string fileName, CandidateProviderTrainerDocumentVM candidateProviderTrainerDocument)
        {
            bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
            if (!hasPermission) { return; }

            CandidateProviderTrainerDocumentVM document = this.trainerDocumentsSource.FirstOrDefault(x => x.FileName == fileName);

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

                this.toast.sfErrorToast.Content = msg;
                await this.toast.sfErrorToast.ShowAsync();
            }
        }

        private void IndentChanged()
        {
            var indent = this.candidateProviderTrainerVM.Indent;
            if (indent != null)
            {
                indent = indent.Trim();

                var checkEGN = new BasicEGNValidation(indent);

                if (checkEGN.Validate())
                {
                    var year = int.Parse(indent.Substring(0, 2));
                    var month = int.Parse(indent.Substring(2, 2));
                    var date = int.Parse(indent.Substring(4, 2));

                    this.candidateProviderTrainerVM.BirthDate = DateTime.Parse($"19{year}-{month}-{date}");

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
        }

        private void ValidateEGN(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.candidateProviderTrainerVM.Indent != null)
            {
                this.candidateProviderTrainerVM.Indent = this.candidateProviderTrainerVM.Indent.Trim();

                if (this.candidateProviderTrainerVM.Indent.Length > 10)
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
                        this.messageStore?.Add(fi, checkEGN.ErrorMessage);
                    }
                }
            }
        }

        private void CheckForOtherTrainerWithSameIndent(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            var indentType = this.kvIndentTypeSource.FirstOrDefault(x => x.IdKeyValue == this.candidateProviderTrainerVM.IdIndentType);
            if (this.CandidateProviderVM.CandidateProviderTrainers.Any(x => x.Indent == this.candidateProviderTrainerVM.Indent && x.IdCandidateProviderTrainer != this.candidateProviderTrainerVM.IdCandidateProviderTrainer))
            {
                FieldIdentifier fi = new FieldIdentifier(this.candidateProviderTrainerVM, "Indent");
                this.messageStore?.Add(fi, $"Вече съществува консултант с това {indentType.Name}!");
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

        private void QueryCellInfo(QueryCellInfoEventArgs<CandidateProviderTrainerVM> args)
        {
            if (!this.DisableAllFields)
            {
                var trainer = this.trainersSource.FirstOrDefault(x => x.IdCandidateProviderTrainer == args.Data.IdCandidateProviderTrainer && x.IdStatus == this.kvCandidateProviderTrainerStatusActive.IdKeyValue);
                if (trainer is not null)
                {
                    //var certificate = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == this.certificateKv && x.HasUploadedFile);
                    var autobiography = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == this.autobiographyKv && x.HasUploadedFile);
                    var declaration = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == this.declarationOfConsentKv && x.HasUploadedFile);
                    var diploma = trainer.CandidateProviderTrainerDocuments.FirstOrDefault(x => x.IdDocumentType == this.retrainingDiplomaKv && x.HasUploadedFile);

                    if (/*certificate is null ||*/ autobiography is null || declaration is null || diploma is null)
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
        private async void ExportPDF()
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
                await this.JsRuntime.SaveAs("Spravka_konsultanti" + ".pdf", result.ToArray());

            }
            finally
            {
                this.SpinnerHide();
                this.loading = false;
            }
        }
        public async void FilterSelect()
        {
            this.searchModal.OpenModal(CandidateProviderVM, trainersSource);
        }

        private async Task<MemoryStream> ReportWord()
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
            foreach (var trainer in candidateProviderTrainersListForGrid)
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
            foreach (var prviderTrainer in candidateProviderTrainersListForGrid)
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
        public void FilterApply(List<CandidateProviderTrainerVM> candidateProviderTrainerVMs)
        {
            this.candidateProviderTrainersListForGrid = candidateProviderTrainerVMs.OrderBy(x => x.FullName).ToList();
            if (!this.trainersGrid.SelectedRecords.Any(x => this.candidateProviderTrainersListForGrid.Any(y => y.IdCandidateProviderTrainer == x.IdCandidateProviderTrainer)))
            {
                candidateProviderTrainerVM = new CandidateProviderTrainerVM();
            }
        }
    }
}
