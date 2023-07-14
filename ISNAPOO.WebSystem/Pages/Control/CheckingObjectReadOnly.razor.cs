using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Registers.RegisterMTB;
using ISNAPOO.WebSystem.Pages.Registers.RegisterTrainer;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Control
{
    partial class CheckingObjectReadOnly : BlazorBaseComponent
    {
        [Parameter]

        public bool IsCPO { get; set; } = true;
        
        [Parameter]

        public FollowUpControlVM Model { get; set; }

        #region Inject
        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }
        #endregion

        #region Properties
        private ToastMsg toast;
        private SfGrid<CandidateProviderPremisesCheckingVM> mtbSfGrid = new SfGrid<CandidateProviderPremisesCheckingVM>();
        private SfGrid<CandidateProviderTrainerCheckingVM> trainerSfGrid = new SfGrid<CandidateProviderTrainerCheckingVM>();
        private SfGrid<CourseCheckingVM> courseSfGrid = new SfGrid<CourseCheckingVM>();
        private CheckingObjectModal checkingObjectModal = new CheckingObjectModal();
        private MTBInformationModal mTBInformationModal = new MTBInformationModal();
        private TrainerInformationModal trainerInformationModal = new TrainerInformationModal();
        private CurrentTrainingCourseModal currentTrainingCourseModal = new CurrentTrainingCourseModal();
        #endregion

        #region Source
        private List<CandidateProviderPremisesCheckingVM> selectedMTBs = new List<CandidateProviderPremisesCheckingVM>();
        private List<CandidateProviderTrainerCheckingVM> selectedTrainers = new List<CandidateProviderTrainerCheckingVM>();
        private List<CourseCheckingVM> selectedCourses = new List<CourseCheckingVM>();
        private List<CandidateProviderPremisesCheckingVM> mtbsSource = new List<CandidateProviderPremisesCheckingVM>();
        private List<CandidateProviderTrainerCheckingVM> trainersSource = new List<CandidateProviderTrainerCheckingVM>();
        private List<CourseCheckingVM> courseSource = new List<CourseCheckingVM>();
        #endregion

        #region Model&Fields
        private CheckingObjectVM model = new CheckingObjectVM();
        private string TrainersHeader = string.Empty;
        private string AddTrainerMsg = string.Empty;
        private string objType = string.Empty;
        private string LicensingType = string.Empty;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.model);
            if (IsCPO)
            {
                this.TrainersHeader = "Преподаватели";
                this.AddTrainerMsg = "Избери преподавател за проверка";
                this.LicensingType = "LicensingCPO";
            }
            else
            {
                TrainersHeader = "Консултанти";
                this.AddTrainerMsg = "Избери консултант за проверка";
                this.LicensingType = "LicensingCIPO";
            }

            if (this.Model.IdFollowUpControl != 0)
            {
                
                this.mtbsSource = await this.CandidateProviderService.GetAllActiveCandidateProviderPremisesCheckingByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                this.trainersSource = await this.CandidateProviderService.GetAllActiveCandidateProviderTrainerCheckingByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                this.courseSource = await this.TrainingService.GetAllActiveCourseCheckingsByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
            }
            else
            {
                this.Model = new FollowUpControlVM();
            }

        }

        //public async Task OpenModal(FollowUpControlVM _followUpControlVM)
        //{
        //    if (_followUpControlVM.IdFollowUpControl != 0)
        //    {
        //        this.followUpControlVM = _followUpControlVM;

        //        this.mtbsSource = await this.CandidateProviderService.GetAllActiveCandidateProviderPremisesCheckingByIdFollowUpControlAsync(this.followUpControlVM.IdFollowUpControl);
        //        this.trainersSource = await this.CandidateProviderService.GetAllActiveCandidateProviderTrainerCheckingByIdFollowUpControlAsync(this.followUpControlVM.IdFollowUpControl);
        //        this.courseSource = await this.TrainingService.GetAllActiveCourseCheckingsByIdFollowUpControlAsync(this.followUpControlVM.IdFollowUpControl);
        //    }
        //    else
        //    {
        //        this.followUpControlVM = new FollowUpControlVM();
        //    }

        //}

       

       

        private async Task OpenRegisterMTBModalBtn(CandidateProviderPremisesCheckingVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var type = this.IsCPO ? "LicensingCPO" : "LicensingCIPO";
                var registerMTBVM = await this.CandidateProviderService.GetRegisterMTBVMByIdCandidateProviderPremisesAsync(model.IdCandidateProviderPremises!.Value);
                await this.mTBInformationModal.OpenModal(registerMTBVM);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        

        private async Task OpenRegisterTrainerModalBtn(CandidateProviderTrainerCheckingVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.trainerInformationModal.OpenModal(model.IdCandidateProviderTrainer!.Value, this.IsCPO);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }


        private async Task OpenRegisterCourseModalBtn(CourseCheckingVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var courseFromDb = await this.TrainingService.GetTrainingCourseByIdAsync(model.IdCourse!.Value);
                await this.currentTrainingCourseModal.OpenModal(courseFromDb, null, false);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

    }
}



