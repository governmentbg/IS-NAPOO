using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CandidateProviderTrainerQualificationModal : BlazorBaseComponent
    {
        SfDialog sfDialog = new SfDialog();
        ToastMsg toast = new ToastMsg();

        private CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM = new CandidateProviderTrainerQualificationVM();
        private IEnumerable<KeyValueVM> kvQualificationTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvTrainingQualificationTypeSource = new List<KeyValueVM>();
        private IEnumerable<ProfessionVM> professionSource = new List<ProfessionVM>();
        private List<CandidateProviderTrainerQualificationVM> addedQualifications = new List<CandidateProviderTrainerQualificationVM>();

        private string trainerFullname = string.Empty;
        private bool IsDateValid = true;
        private bool disableNextBtn = false;
        private bool disablePreviousBtn = false;
        private bool saveAndContinueClicked = false;
        private int idCandidateTrainerQ = 0;
        private List<string> validationMessages = new List<string>();
        private int nextId = 0;
        private int previousId = 0;
        private int currentIndex = 0;

        [Parameter]
        public EventCallback<CandidateProviderTrainerQualificationVM> CallbackAfterModalSubmit { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IProfessionService ProfessionService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        public override bool IsContextModified { get => this.IsEditContextModified() ; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.candidateProviderTrainerQualificationVM);
        }

        public async Task OpenModal(CandidateProviderTrainerQualificationVM candidateProviderTrainerQualificationVM, IEnumerable<KeyValueVM> kvTrainingQualificationTypeSource, IEnumerable<KeyValueVM> kvQualificationTypeSource, string trainerFullname, List<CandidateProviderTrainerQualificationVM> addedQualifications)
        {
            
            this.trainerFullname = trainerFullname;
            this.editContext = new EditContext(this.candidateProviderTrainerQualificationVM);

            if (candidateProviderTrainerQualificationVM.IdCandidateProviderTrainerQualification != 0)
            {
                this.addedQualifications = addedQualifications;
                var counter = 1;
                foreach (var entry in this.addedQualifications)
                {
                    entry.IdForQualificationModal = counter++;
                }
                this.candidateProviderTrainerQualificationVM = await this.CandidateProviderService.GetCandidateProviderTrainerQualificationByIdAsync(candidateProviderTrainerQualificationVM);
                this.currentIndex = this.addedQualifications.IndexOf(candidateProviderTrainerQualificationVM);
            }
            else
            {
                this.candidateProviderTrainerQualificationVM = new CandidateProviderTrainerQualificationVM() { IdCandidateProviderTrainer = candidateProviderTrainerQualificationVM.IdCandidateProviderTrainer };
            }

            this.kvQualificationTypeSource = kvQualificationTypeSource;
            this.kvTrainingQualificationTypeSource = kvTrainingQualificationTypeSource;
            this.professionSource = this.DataSourceService.GetAllProfessionsList();
            this.professionSource = this.professionSource.ToList();
            this.professionSource = this.professionSource.OrderBy(x => x.Code).ToList();

            this.SetButtonsState();
            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Submit(bool isClose = true)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.saveAndContinueClicked = false;
            this.editContext = new EditContext(this.candidateProviderTrainerQualificationVM);
            this.editContext.EnableDataAnnotationsValidation();
            var isValid = this.editContext.Validate();
            this.SpinnerShow();
            if (IsDateValid)
            {
                if (isValid)
                {
                    ResultContext<CandidateProviderTrainerQualificationVM> resultContext = new ResultContext<CandidateProviderTrainerQualificationVM>();

                    if (this.candidateProviderTrainerQualificationVM.IdCandidateProviderTrainerQualification != 0)
                    {
                        resultContext = await this.CandidateProviderService.UpdateCandidateProviderTrainerQualificationAsync(this.candidateProviderTrainerQualificationVM);
                    }
                    else
                    {
                        resultContext = await this.CandidateProviderService.CreateCandidateProviderTrainerQualificationAsync(this.candidateProviderTrainerQualificationVM);
                    }

                    if (resultContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                    }
                    else
                    {
                        this.editContext = new EditContext(this.candidateProviderTrainerQualificationVM);
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                    }

                    if(isClose)
                    {
                        this.isVisible = false;
                    }
                    this.StateHasChanged();
                    await this.CallbackAfterModalSubmit.InvokeAsync(this.candidateProviderTrainerQualificationVM);
                }
            }
            else
            {
                toast.sfErrorToast.Content = "Въведената дата в полето 'Дата на провеждане на обучението от' не може да е след 'Дата на провеждане на обучението до'!";
                await toast.sfErrorToast.ShowAsync();
            }
            this.SpinnerHide();
        }
        private void DateValid()
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime(); ;
            if (this.candidateProviderTrainerQualificationVM.TrainingFrom.HasValue)
            {
                startDate = this.candidateProviderTrainerQualificationVM.TrainingFrom.Value.Date;
            }
            if (this.candidateProviderTrainerQualificationVM.TrainingTo.HasValue)
            {
                endDate = this.candidateProviderTrainerQualificationVM.TrainingTo.Value.Date;

            }
            int result = DateTime.Compare(startDate, endDate);

            if (result > 0 && this.candidateProviderTrainerQualificationVM.TrainingTo.HasValue && this.candidateProviderTrainerQualificationVM.TrainingFrom.HasValue)
            {
                this.IsDateValid = false;
            }
            else
            {
                this.IsDateValid = true;
            }
        }
        private async Task SubmitAndContinueBtn()
        {
            await this.Submit(false);

            if (this.editContext.Validate())
            {
                this.candidateProviderTrainerQualificationVM = new CandidateProviderTrainerQualificationVM() { IdCandidateProviderTrainer = candidateProviderTrainerQualificationVM.IdCandidateProviderTrainer };
            }
        }
        private void SetButtonsState()
        {
            this.nextId = currentIndex + 1;
            this.previousId = currentIndex - 1;

            if (this.nextId == this.addedQualifications.Count)
            {
                this.disableNextBtn = true;
            }
            else
            {
                this.disableNextBtn = false;
            }

            if (this.previousId < 0)
            {
                this.disablePreviousBtn = true;
            }
            else
            {
                this.disablePreviousBtn = false;
            }
        }
        private async void PreviousCurriculum()
        {
            this.saveAndContinueClicked = false;

            var id = this.idCandidateTrainerQ == 0 ? this.candidateProviderTrainerQualificationVM.IdCandidateProviderTrainerQualification : this.idCandidateTrainerQ;
           
                this.previousId = this.saveAndContinueClicked ? this.addedQualifications.FindIndex(x => x.IdCandidateProviderTrainerQualification == id) - 1 : this.addedQualifications.FindIndex(x => x.IdCandidateProviderTrainerQualification == id) - 1;
                if (this.previousId == -1)
                {
                    this.previousId = this.addedQualifications.Count - 1;
                }

                if (this.previousId >= 0)
                {
                    this.candidateProviderTrainerQualificationVM = this.addedQualifications[this.previousId];
                    this.currentIndex = this.addedQualifications.IndexOf(this.candidateProviderTrainerQualificationVM);
                    this.SetButtonsState();
                }
        }
        private async void NextCurriculum()
        {
            this.saveAndContinueClicked = false;

                this.nextId = this.addedQualifications.FindIndex(x => x.IdCandidateProviderTrainerQualification == this.candidateProviderTrainerQualificationVM.IdCandidateProviderTrainerQualification) + 1;
                if (this.nextId < this.addedQualifications.Count)
                {
                    this.candidateProviderTrainerQualificationVM = this.addedQualifications[this.nextId];
                    this.currentIndex = this.addedQualifications.IndexOf(this.candidateProviderTrainerQualificationVM);
                    this.SetButtonsState();

                }
        }
    }
}
