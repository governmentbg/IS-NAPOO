using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using Action = System.Action;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CandidateCurriculumModal : BlazorBaseComponent
    {
        private SfDialog sfDialog = new SfDialog();
        private SfGrid<ERUVM> erusGrid = new SfGrid<ERUVM>();
        private ToastMsg toast = new ToastMsg();

        private CandidateCurriculumVM candidateCurriculumVM = new CandidateCurriculumVM();
        private IEnumerable<KeyValueVM> professionalTrainingSource = new List<KeyValueVM>();
        private List<ERUVM> addedErus = new List<ERUVM>();
        private int idEru = 0;
        private List<ERUVM> eruDataSourceForComboBox = new List<ERUVM>();
        private SpecialityVM speciality = new SpecialityVM();
        private DocVM doc = new DocVM();
        private List<ERUVM> allErus = new List<ERUVM>();
        private List<CandidateCurriculumVM> addedCurriculums = new List<CandidateCurriculumVM>();
        private bool unsavedChangesConfirmed = false;
        private bool showUnsavedChangesConfirmDialog = false;
        private Action ActionToPerform;
        private CandidateProviderSpecialityVM candidateProviderSpecialityVM = new CandidateProviderSpecialityVM();
        private int nextId = 0;
        private int previousId = 0;
        private int idCandidateCurr = 0;
        private string CreationDateStr = "";
        private string ModifyDateStr = "";
        private List<string> validationMessages = new List<string>();
        private bool disableNextBtn = false;
        private bool disablePreviousBtn = false;
        private bool saveAndContinueClicked = false;
        private KeyValueVM kvBPTValue = new KeyValueVM();
        private int idCandidateCurriculumModification = 0;
        private bool isOpenForEdit = true;

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback<ResultContext<CandidateCurriculumVM>> CallbackAfterSubmit { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

        [Parameter]
        public bool DisableAllFields { get; set; }

        [Inject]
        public ICandidateCurriculumService CandidateCurriculumService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IDOCService DOCService { get; set; }

        [Inject]
        public ICandidateCurriculumERUService CandidateCurriculumERUService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IERUSpecialityService ERUSpecialityService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.candidateCurriculumVM);
            this.professionalTrainingSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");
            this.eruDataSourceForComboBox = new List<ERUVM>();
        }

        public async Task OpenModal(CandidateCurriculumVM candidateCurriculumVM, SpecialityVM speciality, List<CandidateCurriculumVM> addedCurriculums, CandidateProviderSpecialityVM candidateProviderSpeciality)
        {
            this.editContext = new EditContext(this.candidateCurriculumVM);

            this.candidateProviderSpecialityVM = candidateProviderSpeciality;
            this.allErus.Clear();
            this.validationMessages.Clear();
            this.addedCurriculums = addedCurriculums;
            var counter = 1;

            this.kvBPTValue = this.professionalTrainingSource.FirstOrDefault(x => x.KeyValueIntCode == "B");

            foreach (var entry in this.addedCurriculums)
            {
                entry.IdForCurriculumModal = counter++;
            }

            this.idEru = 0;
            this.candidateCurriculumVM = candidateCurriculumVM;

            if (this.candidateCurriculumVM.IdCandidateCurriculum == 0)
            {
                this.isOpenForEdit = false;
            }
            else
            {
                this.isOpenForEdit = true;
            }

            this.idCandidateCurriculumModification = this.candidateCurriculumVM.IdCandidateCurriculumModification.HasValue ? this.candidateCurriculumVM.IdCandidateCurriculumModification.Value : 0;

            await SetModalFooterInformationAsync();

            this.editContext = new EditContext(this.candidateCurriculumVM);
            this.speciality = speciality;

            if (this.speciality.IdDOC != null)
            {
                this.doc = await this.DOCService.GetDOCByIdAsync(new DocVM() { IdDOC = this.speciality.IdDOC ?? default });
            }

            //var candidateCurriculumErus = await this.CandidateCurriculumERUService.GetAllCandidateCurriculumERUByCandidateCurriculumIdAsync(candidateCurriculumVM);
            var candidateCurriculumEruIds = candidateCurriculumVM.SelectedERUs.Select(x => x.IdERU).ToList();
            this.addedErus = (List<ERUVM>)await this.DOCService.GetERUsByIdsAsync(candidateCurriculumEruIds);
            this.eruDataSourceForComboBox.Clear();

            await this.SetERUsData();

            this.SetButtonsState();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task SetModalFooterInformationAsync()
        {
            if (this.candidateCurriculumVM.IdCandidateCurriculum != 0)
            {
                this.CreationDateStr = this.candidateCurriculumVM.CreationDate.ToString("dd.MM.yyyy");
                this.ModifyDateStr = this.candidateCurriculumVM.ModifyDate.ToString("dd.MM.yyyy");
                this.candidateCurriculumVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.candidateCurriculumVM.IdModifyUser);
                this.candidateCurriculumVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.candidateCurriculumVM.IdCreateUser);
            }
        }

        private async Task LoadAddedCurriculumsDataAsync()
        {
            var addedCurriculum = this.addedCurriculums.FirstOrDefault();
            if (addedCurriculum is not null)
            {
                this.addedCurriculums = (await this.CandidateCurriculumService.GetAllCurriculumsByIdCandidateProviderSpecialityAsync(addedCurriculum.IdCandidateProviderSpeciality)).ToList();
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

                this.saveAndContinueClicked = false;
                this.editContext = new EditContext(this.candidateCurriculumVM);
                this.editContext.EnableDataAnnotationsValidation();

                if (this.editContext.Validate())
                {
                    this.validationMessages.Clear();

                    ResultContext<CandidateCurriculumVM> inputContext = new ResultContext<CandidateCurriculumVM>();
                    this.candidateCurriculumVM.SelectedERUs = this.addedErus;
                    inputContext.ResultContextObject = this.candidateCurriculumVM;
                    inputContext.ResultContextObject.IdCandidateProviderSpeciality = this.candidateProviderSpecialityVM.IdCandidateProviderSpeciality;

                    if (this.candidateCurriculumVM.IdCandidateCurriculum == 0)
                    {
                        inputContext = await this.CandidateCurriculumService.AddCandidateCurriculumAsync(inputContext);
                    }
                    else
                    {
                        inputContext = await this.CandidateCurriculumService.UpdateCandidateCurriculumAsync(inputContext);
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
                        await this.LoadAddedCurriculumsDataAsync();

                        this.CreationDateStr = inputContext.ResultContextObject.CreationDate.ToString("dd.MM.yyyy");
                        this.ModifyDateStr = inputContext.ResultContextObject.ModifyDate.ToString("dd.MM.yyyy");
                        this.candidateCurriculumVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(inputContext.ResultContextObject.IdModifyUser);
                        this.candidateCurriculumVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(inputContext.ResultContextObject.IdCreateUser);

                        this.SetButtonsState();

                        await this.CallbackAfterSubmit.InvokeAsync(inputContext);
                    }
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
                var candidateCurr = this.addedCurriculums.FirstOrDefault(x => x.IdCandidateCurriculum == this.candidateCurriculumVM.IdCandidateCurriculum);
                candidateCurr.SelectedERUs = this.candidateCurriculumVM.SelectedERUs.ToList();
                candidateCurr.IdCandidateCurriculum = this.candidateCurriculumVM.IdCandidateCurriculum;

                this.idCandidateCurr = this.candidateCurriculumVM.IdCandidateCurriculum;

                this.candidateCurriculumVM = new CandidateCurriculumVM();

                this.isOpenForEdit = false;

                if (this.idCandidateCurriculumModification != 0)
                {
                    this.candidateCurriculumVM.IdCandidateCurriculumModification = this.idCandidateCurriculumModification;
                }

                this.nextId = this.addedCurriculums.Count;
                this.disableNextBtn = true;
                this.previousId = this.addedCurriculums.Count - 1;
                this.disablePreviousBtn = false;
                this.saveAndContinueClicked = true;

                this.addedErus = new List<ERUVM>();

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

                var candidateCurriculumERUVM = this.CandidateCurriculumERUService.GetCandidateCurriculumERUByIdCandidateCurriculumAndIdERU(this.candidateCurriculumVM.IdCandidateCurriculum, eru.IdERU);
                if (candidateCurriculumERUVM is not null)
                {
                    var result = await this.CandidateCurriculumERUService.DeleteCandidateCurriculumERUAsync(candidateCurriculumERUVM.IdCandidateCurriculumERU);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                        return;
                    }
                    else
                    {
                        this.candidateCurriculumVM.SelectedERUs = this.addedErus.ToList();
                        var resultContext = new ResultContext<CandidateCurriculumVM>();
                        resultContext.ResultContextObject = this.candidateCurriculumVM;
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
                this.toast.sfErrorToast.Content = "Моля, изберете ЕРУ!";
                await this.toast.sfErrorToast.ShowAsync();
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
                        this.toast.sfErrorToast.Content = "ЕРУ, с тази стойност, е вече добавено!";
                        await this.toast.sfErrorToast.ShowAsync();
                    }
                }
            }
        }

        private void OnProfessionalTrainingChangeHandler(ChangeEventArgs<int, KeyValueVM> args)
        {
            if (this.candidateCurriculumVM.IdProfessionalTraining != this.kvBPTValue.IdKeyValue)
            {
                this.eruDataSourceForComboBox.Clear();
                this.eruDataSourceForComboBox = this.allErus.Where(x => x.IdProfessionalTraining == this.candidateCurriculumVM.IdProfessionalTraining).OrderBy(x => x.ERUIntCodeSplit).ToList();
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
            this.eruDataSourceForComboBox = this.allErus.Where(x => x.IdProfessionalTraining == this.candidateCurriculumVM.IdProfessionalTraining).OrderBy(x => x.ERUIntCodeSplit).ToList();
        }

        private async Task SetERUSDataAsync()
        {
            var candidateCurriculumErus = await this.CandidateCurriculumERUService.GetAllCandidateCurriculumERUByCandidateCurriculumIdAsync(this.candidateCurriculumVM);
            var candidateCurriculumEruIds = candidateCurriculumErus.Select(x => x.IdERU).ToList();
            this.addedErus = (List<ERUVM>)await this.DOCService.GetERUsByIdsAsync(candidateCurriculumEruIds);

            await this.erusGrid.Refresh();
            this.StateHasChanged();
        }

        private void GetERUsData()
        {
            if (this.candidateCurriculumVM.IdProfessionalTraining != 0)
            {
                this.eruDataSourceForComboBox.Clear();
                this.eruDataSourceForComboBox = this.allErus.Where(x => x.IdProfessionalTraining == this.candidateCurriculumVM.IdProfessionalTraining).OrderBy(x => x.ERUIntCodeSplit).ToList();
            }
        }

        private async void NextCurriculum()
        {
            this.saveAndContinueClicked = false;

            if (this.editContext.IsModified())
            {
                this.ActionToPerform = this.NextCurriculum;
                this.showUnsavedChangesConfirmDialog = !this.showUnsavedChangesConfirmDialog;

                if (this.unsavedChangesConfirmed)
                {
                    this.editContext.MarkAsUnmodified();
                    this.unsavedChangesConfirmed = false;
                    this.nextId = this.addedCurriculums.FindIndex(x => x.IdCandidateCurriculum == this.candidateCurriculumVM.IdCandidateCurriculum) + 1;
                    if (this.nextId < this.addedCurriculums.Count)
                    {
                        this.candidateCurriculumVM = this.addedCurriculums[this.nextId];
                        if (this.candidateCurriculumVM.IdProfessionalTraining != this.kvBPTValue.IdKeyValue)
                        {
                            await this.SetERUSDataAsync();
                            this.GetERUsData();
                        }

                        this.SetButtonsState();

                        await this.SetModalFooterInformationAsync();

                        this.editContext.MarkAsUnmodified();
                    }
                }
            }
            else
            {
                this.nextId = this.addedCurriculums.FindIndex(x => x.IdCandidateCurriculum == this.candidateCurriculumVM.IdCandidateCurriculum) + 1;
                if (this.nextId < this.addedCurriculums.Count)
                {
                    this.candidateCurriculumVM = this.addedCurriculums[this.nextId];
                    if (this.candidateCurriculumVM.IdProfessionalTraining != this.kvBPTValue.IdKeyValue)
                    {
                        await this.SetERUSDataAsync();
                        this.GetERUsData();
                    }

                    this.SetButtonsState();

                    await this.SetModalFooterInformationAsync();

                    this.editContext.MarkAsUnmodified();
                }
            }
        }

        private async void PreviousCurriculum()
        {
            this.saveAndContinueClicked = false;

            var id = this.idCandidateCurr == 0 ? this.candidateCurriculumVM.IdCandidateCurriculum : this.idCandidateCurr;
            if (this.editContext.IsModified())
            {
                this.ActionToPerform = this.PreviousCurriculum;
                this.showUnsavedChangesConfirmDialog = !this.showUnsavedChangesConfirmDialog;

                if (this.unsavedChangesConfirmed)
                {
                    this.editContext.MarkAsUnmodified();
                    this.unsavedChangesConfirmed = false;
                    this.previousId = this.saveAndContinueClicked ? this.addedCurriculums.FindIndex(x => x.IdCandidateCurriculum == id) - 1 : this.addedCurriculums.FindIndex(x => x.IdCandidateCurriculum == id) - 1;
                    if (this.previousId >= 0)
                    {
                        this.candidateCurriculumVM = this.addedCurriculums[this.previousId];
                        if (this.candidateCurriculumVM.IdProfessionalTraining != this.kvBPTValue.IdKeyValue)
                        {
                            await this.SetERUSDataAsync();
                            this.GetERUsData();
                        }

                        this.SetButtonsState();

                        await this.SetModalFooterInformationAsync();

                        this.editContext.MarkAsUnmodified();
                    }
                }
            }
            else
            {
                this.previousId = this.saveAndContinueClicked ? this.addedCurriculums.FindIndex(x => x.IdCandidateCurriculum == id) - 1 : this.addedCurriculums.FindIndex(x => x.IdCandidateCurriculum == id) - 1;
                if (this.previousId == -1)
                {
                    this.previousId = this.addedCurriculums.Count - 1;
                }

                if (this.previousId >= 0)
                {
                    this.candidateCurriculumVM = this.addedCurriculums[this.previousId];
                    if (this.candidateCurriculumVM.IdProfessionalTraining != this.kvBPTValue.IdKeyValue)
                    {
                        await this.SetERUSDataAsync();
                        this.GetERUsData();
                    }

                    this.SetButtonsState();

                    await this.SetModalFooterInformationAsync();

                    this.editContext.MarkAsUnmodified();
                }
            }
        }

        private void SetButtonsState()
        {
            this.nextId = this.addedCurriculums.IndexOf(this.candidateCurriculumVM) + 1;
            this.previousId = this.addedCurriculums.IndexOf(this.candidateCurriculumVM) - 1;

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
    }
}
