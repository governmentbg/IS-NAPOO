using ISNAPOO.Common.Constants;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using ISNAPOO.Common.Framework;
using Action = System.Action;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using Microsoft.JSInterop;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class ValidationCurriculum : BlazorBaseComponent
    {
        private SfGrid<ERUVM> erusGrid;

        private ValidationCurriculumVM validationCurriculumVM = new ValidationCurriculumVM();
        private IEnumerable<KeyValueVM> professionalTrainingSource = new List<KeyValueVM>();
        private List<ERUVM> addedErus = new List<ERUVM>();
        private int idEru = 0;
        private List<ERUVM> eruDataSourceForComboBox = new List<ERUVM>();
        private SpecialityVM speciality = new SpecialityVM();
        private DocVM doc = new DocVM();
        private List<ERUVM> allErus = new List<ERUVM>();
        private List<ValidationCurriculumVM> addedCurriculums = new List<ValidationCurriculumVM>();
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
        private bool disableFieldIfSPK = false;
        private KeyValueVM kvBType = new KeyValueVM();
        private ValidationMessageStore? messageStore;
        private ValidationClientVM clientForValidation = new ValidationClientVM();
        private ValidationCurriculumVM validationCurrForValidation = new ValidationCurriculumVM();
      

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback<ResultContext<ValidationCurriculumVM>> CallbackAfterSubmit { get; set; }
        [Parameter]
        public bool DisableIfSPK { get; set; }

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

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.validationCurriculumVM);
            this.professionalTrainingSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            this.kvBType = this.professionalTrainingSource.FirstOrDefault(x => x.KeyValueIntCode == "B");

            this.eruDataSourceForComboBox = new List<ERUVM>();
        }



        public async Task OpenModal(ValidationCurriculumVM validationCurriculumVM, List<ValidationCurriculumVM> addedCurriculums, int idCandidateProviderSpeciality, SpecialityVM speciality, string type = null, ConcurrencyInfo concurrencyInfo = null)
        {
            this.idCandidateProviderSpeciality = idCandidateProviderSpeciality;
            this.allErus.Clear();
            this.validationMessages.Clear();
            this.addedCurriculums = addedCurriculums;
            this.disableBtnsWhenEnteredFromEdit = false;
            var counter = 1;

            this.disableFieldIfSPK = !DisableIfSPK;

            this.idEru = 0;
            this.validationCurriculumVM = validationCurriculumVM;
            if (this.validationCurriculumVM.IdValidationCurriculum != 0)
            {
                this.IdTrainingProgramCurriculum = this.validationCurriculumVM.IdValidationCurriculum;
                this.CreationDateStr = this.validationCurriculumVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.validationCurriculumVM.ModifyDate.ToString("dd.MM.yyyy");
                this.validationCurriculumVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.validationCurriculumVM.IdModifyUser);
                this.validationCurriculumVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.validationCurriculumVM.IdCreateUser);

                if (this.validationCurriculumVM.IdProfessionalTraining != this.kvBType.IdKeyValue)
                {
                    this.disableBtnsWhenEnteredFromEdit = true;
                }
            }
            else
            {
                this.validationCurriculumVM.IdProfessionalTraining = this.kvBType.IdKeyValue;
            }

            this.editContext = new EditContext(this.validationCurriculumVM);
            this.speciality = speciality;

            if (this.speciality.IdDOC != null)
            {
                this.doc = await this.DOCService.GetDOCByIdAsync(new DocVM() { IdDOC = this.speciality.IdDOC ?? default });
            }

            var candidateCurriculumEruIds = this.validationCurriculumVM.SelectedERUs.Select(x => x.IdERU).ToList();
            this.addedErus = (await this.DOCService.GetERUsByIdsAsync(candidateCurriculumEruIds)).ToList();
            this.eruDataSourceForComboBox.Clear();

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
            if (this.validationCurriculumVM.IdProfessionalTraining != 0)
            {
                this.eruDataSourceForComboBox.Clear();
                this.eruDataSourceForComboBox = this.allErus.Where(x => x.IdProfessionalTraining == this.validationCurriculumVM.IdProfessionalTraining).OrderBy(x => x.ERUIntCodeSplit).ToList();
            }
        }

        private async Task Submit()
        {
            this.validationMessages.Clear();
        
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.validationCurriculumVM);
                this.editContext.EnableDataAnnotationsValidation();
                this.messageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidateSubjTopic;

                if (this.validationCurriculumVM.IdValidationClient != 0)
                {
                    this.clientForValidation = await this.TrainingService.GetValidationClientByIdAsync(this.validationCurriculumVM.IdValidationClient);
                }

                if (this.validationCurriculumVM.IdValidationCurriculum != 0)
                {
                    this.validationCurrForValidation = await this.TrainingService.GetValidationCurriculumByIdAsync(this.validationCurriculumVM.IdValidationCurriculum);
                }

                this.editContext.OnValidationRequested += this.ValidateCourseEndDateWithSummaryHours;

                if (this.editContext.Validate())
                {
                    this.SpinnerShow();

                    this.validationMessages.Clear();

                    ResultContext<ValidationCurriculumVM> inputContext = new ResultContext<ValidationCurriculumVM>();
                    this.validationCurriculumVM.SelectedERUs = this.addedErus;
                    inputContext.ResultContextObject = this.validationCurriculumVM;
                    inputContext.ResultContextObject.IdCandidateProviderSpeciality = this.idCandidateProviderSpeciality;
                    var addForConcurrentCheck = false;
                    if (this.validationCurriculumVM.IdValidationCurriculum == 0)
                    {
                        inputContext = await this.TrainingService.AddValidationCurriculumAsync(inputContext);
                        addForConcurrentCheck = true;
                    }
                    else
                    {
                        inputContext = await this.TrainingService.UpdateValidationCurriculumAsync(inputContext);
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
                        this.validationCurriculumVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(inputContext.ResultContextObject.IdModifyUser);
                        this.validationCurriculumVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(inputContext.ResultContextObject.IdCreateUser);

                        if (addForConcurrentCheck)
                        {
                            await this.AddEntityIdAsCurrentlyOpened(this.validationCurriculumVM.IdValidationCurriculum, "TrainingProgramCurriculum");
                            this.IdTrainingProgramCurriculum = this.validationCurriculumVM.IdValidationCurriculum;
                        }
                    }

                    this.SetButtonsState();

                    await this.CallbackAfterSubmit.InvokeAsync(inputContext);
                }
                else
                {
                   // this.validationMessages.Clear();
                    //foreach (var msg in this.editContext.GetValidationMessages())
                    //{
                    //    if (!this.validationMessages.Contains(msg))
                    //    {
                    //        this.validationMessages.Add(msg);
                    //    }
                    //}
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

        private async Task SubmitAndContinueBtn()
        {
            await this.Submit();

            if (!this.validationMessages.Any())
            {
                var trainingCurr = this.addedCurriculums.FirstOrDefault(x => x.IdValidationCurriculum == this.validationCurriculumVM.IdValidationCurriculum);
                if (trainingCurr is not null)
                {
                    trainingCurr.SelectedERUs = this.validationCurriculumVM.SelectedERUs.ToList();
                    trainingCurr.IdValidationCurriculum = this.validationCurriculumVM.IdValidationCurriculum;
                }

                this.validationCurriculumVM = new ValidationCurriculumVM()
                {
                    IdValidationClient = this.validationCurriculumVM.IdValidationClient,
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
                if(this.erusGrid is not null)
                await this.erusGrid.Refresh();
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

                var validationCurriculumERUVM = this.TrainingService.GetValidationCurriculumERUByIdTrainingCurriculumAndIdERU(this.validationCurriculumVM.IdValidationCurriculum, eru.IdERU);
                if (validationCurriculumERUVM is not null)
                {
                    var result = await this.TrainingService.DeleteValidationCurriculumERUAsync(validationCurriculumERUVM.IdValidationCurriculumERU);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                        return;
                    }
                    else
                    {
                        this.validationCurriculumVM.SelectedERUs = this.addedErus.ToList();
                        var resultContext = new ResultContext<ValidationCurriculumVM>();
                        resultContext.ResultContextObject = this.validationCurriculumVM;
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
            if (this.validationCurriculumVM.IdProfessionalTraining != this.kvBType.IdKeyValue)
            {
                this.eruDataSourceForComboBox.Clear();
                this.eruDataSourceForComboBox = this.allErus.Where(x => x.IdProfessionalTraining == this.validationCurriculumVM.IdProfessionalTraining).OrderBy(x => x.ERUIntCodeSplit).ToList();
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
            this.eruDataSourceForComboBox = this.allErus.Where(x => x.IdProfessionalTraining == this.validationCurriculumVM.IdProfessionalTraining).OrderBy(x => x.ERUIntCodeSplit).ToList();
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
                    this.nextId = this.addedCurriculums.IndexOf(this.validationCurriculumVM) + 1;
                    if (this.nextId < this.addedCurriculums.Count)
                    {
                        this.RemoveEntityIdAsCurrentlyOpened(this.validationCurriculumVM.IdValidationCurriculum, "TrainingProgramCurriculum");

                        this.validationCurriculumVM = this.addedCurriculums[this.nextId];

                        this.SetButtonsState();
                        await this.SetCreateAndModifyInfoAsync();
                        await this.SetERUSDataAsync();
                        await this.SetConcurrencyInfoAsync();
                    }
                }
            }
            else
            {
                this.nextId = this.addedCurriculums.IndexOf(this.validationCurriculumVM) + 1;
                if (this.nextId < this.addedCurriculums.Count)
                {
                    this.RemoveEntityIdAsCurrentlyOpened(this.validationCurriculumVM.IdValidationCurriculum, "TrainingProgramCurriculum");

                    this.validationCurriculumVM = this.addedCurriculums[this.nextId];

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
            var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(this.validationCurriculumVM.IdValidationCurriculum, "TrainingProgramCurriculum");
            if (concurrencyInfoValue == null)
            {
                this.hideBtnsConcurrentModal = false;
                await this.AddEntityIdAsCurrentlyOpened(this.validationCurriculumVM.IdValidationCurriculum, "TrainingProgramCurriculum");
            }
            else if (concurrencyInfoValue != null && concurrencyInfoValue.IdPerson != this.UserProps.IdPerson)
            {
                this.hideBtnsConcurrentModal = true;
                this.SetPersonFullNameAndVisibilityOfDialog(concurrencyInfoValue);
            }
        }

        private async Task SetERUSDataAsync()
        {
            var candidateCurriculumEruIds = this.validationCurriculumVM.SelectedERUs.Select(x => x.IdERU).ToList();
            this.addedErus = (List<ERUVM>)await this.DOCService.GetERUsByIdsAsync(candidateCurriculumEruIds);

            await this.erusGrid.Refresh();
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
                    this.previousId = this.addedCurriculums.IndexOf(this.validationCurriculumVM) - 1;
                    if (this.previousId >= 0)
                    {
                        this.validationCurriculumVM = this.addedCurriculums[this.previousId];

                        this.SetButtonsState();
                        await this.SetERUSDataAsync();
                        await this.SetCreateAndModifyInfoAsync();
                        await this.SetConcurrencyInfoAsync();
                    }
                }
            }
            else
            {
                this.previousId = this.addedCurriculums.IndexOf(this.validationCurriculumVM) - 1;
                if (this.previousId <= -1)
                {
                    this.previousId = this.addedCurriculums.Count - 1;
                }

                if (this.previousId >= 0)
                {
                    this.validationCurriculumVM = this.addedCurriculums[this.previousId];

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
            this.nextId = this.addedCurriculums.IndexOf(this.validationCurriculumVM) + 1;
            this.previousId = this.addedCurriculums.IndexOf(this.validationCurriculumVM) - 1;

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
            this.validationCurriculumVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.validationCurriculumVM.IdModifyUser);
            this.validationCurriculumVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.validationCurriculumVM.IdCreateUser);
            this.CreationDateStr = this.validationCurriculumVM.CreationDate.ToString("dd.MM.yyyy");
            this.ModifyDateStr = this.validationCurriculumVM.ModifyDate.ToString("dd.MM.yyyy");
        }

        private void ValidateCourseEndDateWithSummaryHours(object? sender, ValidationRequestedEventArgs args)
        {
          
        }
        private void ValidateSubjTopic(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            var validCurriculumSubject = this.validationCurriculumVM.Subject;
            var validCurriculumTopic = this.validationCurriculumVM.Topic;

            FieldIdentifier fi = new FieldIdentifier();

            if (string.IsNullOrEmpty(validCurriculumSubject))
            {
                fi = new FieldIdentifier(this.validationCurriculumVM, "Subject");
                this.messageStore?.Add(fi, "Полето 'Предмет' е задължително!");
            }
            if (string.IsNullOrEmpty(validCurriculumTopic))
            {
                fi = new FieldIdentifier(this.validationCurriculumVM, "Topic");
                this.messageStore?.Add(fi, "Полето 'Тема' е задължително!");
            }
        }
    }
}
