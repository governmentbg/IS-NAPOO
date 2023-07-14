using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.Contracts.Training;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using Syncfusion.Blazor.Grids;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.AspNetCore.Components.Forms;
using ISNAPOO.WebSystem.Pages.Candidate;
using ISNAPOO.WebSystem.Pages.Registers.RegisterMTB;
using ISNAPOO.WebSystem.Pages.Registers.RegisterTrainer;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using ISNAPOO.WebSystem.Pages.Registers.Courses;
using ISNAPOO.WebSystem.Pages.Training.Validation;

namespace ISNAPOO.WebSystem.Pages.Control
{
    partial class CheckingObject : BlazorBaseComponent
    {
        [Parameter]

        public bool IsCPO { get; set; } = true;

        [Parameter]

        public bool IsEditable { get; set; } = true;

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
        private SfGrid<ValidationClientCheckingVM> validationSfGrid = new SfGrid<ValidationClientCheckingVM>();
        private CheckingObjectModal checkingObjectModal = new CheckingObjectModal();
        private MTBInformationModal mTBInformationModal = new MTBInformationModal();
        private TrainerInformationModal trainerInformationModal = new TrainerInformationModal();
        private CurrentTrainingCourseModal currentTrainingCourseModal = new CurrentTrainingCourseModal();
        private ValidationClientInformationModal validationClientInformationModal = new ValidationClientInformationModal();
        private MTBChecking mTBCheckingModal = new MTBChecking();
        private TrainerChecking trainerChecking = new TrainerChecking();
        private CourseCheckingsList courseCheckingsList = new CourseCheckingsList();
        private ValidationClientCheckingsList validationClientCheckingsList = new ValidationClientCheckingsList();
        #endregion

        #region Source
        private List<CandidateProviderPremisesCheckingVM> selectedMTBs = new List<CandidateProviderPremisesCheckingVM>();
        private List<CandidateProviderTrainerCheckingVM> selectedTrainers = new List<CandidateProviderTrainerCheckingVM>();
        private List<CourseCheckingVM> selectedCourses = new List<CourseCheckingVM>();
        private List<ValidationClientCheckingVM> selectedValidation = new List<ValidationClientCheckingVM>();
        private List<CandidateProviderPremisesCheckingVM> mtbsSource = new List<CandidateProviderPremisesCheckingVM>();
        private List<CandidateProviderTrainerCheckingVM> trainersSource = new List<CandidateProviderTrainerCheckingVM>();
        private List<CourseCheckingVM> courseSource = new List<CourseCheckingVM>();
        private List<ValidationClientCheckingVM> validationSource = new List<ValidationClientCheckingVM>();
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
            this.LoadData();
        }


        //public async Task OpenModal(FollowUpControlVM _followUpControlVM)
        //{
        //    if (_followUpControlVM.IdFollowUpControl != 0)
        //    {
        //        this.followUpControlVM = _followUpControlVM;

        //        this.mtbsSource = await this.CandidateProviderService.GetAllActiveCandidateProviderPremisesCheckingByIdFollowUpControlAsync(this.followUpControlVM.IdFollowUpControl);
        //        this.trainersSource = await this.CandidateProviderService.GetAllActiveCandidateProviderTrainerCheckingByIdFollowUpControlAsync(this.followUpControlVM.IdFollowUpControl);
        //        this.courseSource = await this.TrainingService.GetAllActiveCourseCheckingsByIdFollowUpControlAsync(this.followUpControlVM.IdFollowUpControl);
        //        this.validationSource = await this.TrainingService.GetAllActiveValidationClientCheckingsByIdFollowUpControlAsync(this.followUpControlVM.IdFollowUpControl);
        //    }
        //    else
        //    {
        //        this.followUpControlVM = new FollowUpControlVM();
        //    }

        //}

        public async Task LoadData()
        {
            if (this.Model.IdFollowUpControl != 0)
            {

                this.mtbsSource = await this.CandidateProviderService.GetAllActiveCandidateProviderPremisesCheckingByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                this.trainersSource = await this.CandidateProviderService.GetAllActiveCandidateProviderTrainerCheckingByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                this.courseSource = await this.TrainingService.GetAllActiveCourseCheckingsByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                this.validationSource = await this.TrainingService.GetAllActiveValidationClientCheckingsByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
            }
            else
            {
                this.Model = new FollowUpControlVM();
            }
        }

        private async Task AddNewChecking(string Obj)
        {
            if (Obj == "MTB")
            {
                this.checkingObjectModal.OpenModal(Obj, this.Model.IdCandidateProvider.Value, this.Model.IdFollowUpControl, this.Model.ControlEndDate, _mtbCheckings: this.mtbsSource);
            }
            else if (Obj == "Trainer")
            {
                this.checkingObjectModal.OpenModal(Obj, this.Model.IdCandidateProvider.Value, this.Model.IdFollowUpControl, this.Model.ControlEndDate , _trainerCheckings: this.trainersSource);
            }
            else if (Obj == "Course")
            {
                this.checkingObjectModal.OpenModal(Obj, this.Model.IdCandidateProvider.Value, this.Model.IdFollowUpControl, this.Model.ControlEndDate, _courseCheckings: this.courseSource);
            }
            else if (Obj == "Validation")
            {
                this.checkingObjectModal.OpenModal(Obj, this.Model.IdCandidateProvider.Value, this.Model.IdFollowUpControl, this.Model.ControlEndDate, _validationCheckings: this.validationSource); 
            }
        }

        private async Task OpenPremisesCheckingModalBtn(CandidateProviderPremisesCheckingVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                await this.mTBCheckingModal.OpenModal(model.CandidateProviderPremises.IdCandidateProviderPremises, model.CandidateProviderPremises.PremisesName, model.CandidateProviderPremises.IdCandidate_Provider);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

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

        private async Task OpenTrainerCheckingModalBtn(CandidateProviderTrainerCheckingVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                await this.trainerChecking.OpenModal(model.IdCandidateProviderTrainerChecking, model.CandidateProviderTrainer.FullName);
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

        private async Task OpenCourseCheckingModalBtn(CourseCheckingVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                await this.courseCheckingsList.OpenModal(model.IdCourse.Value, model.Course.CourseName);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenValidationCheckingModalBtn(ValidationClientCheckingVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                await this.validationClientCheckingsList.OpenModal(model.IdValidationClient.Value, model.ValidationClient.FullName);
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
        private async Task OpenValidationClientModalBtn(ValidationClientCheckingVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var validationFromDb = await this.TrainingService.GetValidationClientByIdAsync(model.IdValidationClient!.Value);
                await this.validationClientInformationModal.OpenModal(validationFromDb, validationFromDb.IdCourseType);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public async Task SubmitHandler()
        {
            foreach (var mtb in mtbsSource)
            {
                if (mtb.IdCandidateProviderPremisesChecking == 0)
                {
                    var resultContext = new ResultContext<CandidateProviderPremisesCheckingVM>();
                    resultContext.ResultContextObject = mtb;
                    resultContext = await this.CandidateProviderService.CreateCandidateProviderPremisesCheckingAsync(resultContext);
                }
            }

            foreach (var trainer in trainersSource)
            {
                if (trainer.IdCandidateProviderTrainerChecking == 0)
                {
                    var resultContext = new ResultContext<CandidateProviderTrainerCheckingVM>();
                    resultContext.ResultContextObject = trainer;
                    resultContext = await this.CandidateProviderService.CreateCandidateProviderTrainerCheckingAsync(resultContext);
                }
            }

            foreach (var course in courseSource)
            {
                if (course.IdCourseChecking == 0)
                {
                    var resultContext = new ResultContext<CourseCheckingVM>();
                    resultContext.ResultContextObject = course;
                    resultContext = await this.TrainingService.CreateCourseCheckingAsync(resultContext);
                }
            }

            foreach (var validation in validationSource)
            {
                if (validation.IdValidationClientChecking == 0)
                {
                    var resultContext = new ResultContext<ValidationClientCheckingVM>();
                    resultContext.ResultContextObject = validation;
                    resultContext = await this.TrainingService.CreateValidationClientCheckingAsync(resultContext); 
                }
            }
            if (this.Model.IdFollowUpControl != 0)
            {
                this.mtbsSource = await this.CandidateProviderService.GetAllActiveCandidateProviderPremisesCheckingByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                this.trainersSource = await this.CandidateProviderService.GetAllActiveCandidateProviderTrainerCheckingByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                this.courseSource = await this.TrainingService.GetAllActiveCourseCheckingsByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                this.validationSource = await this.TrainingService.GetAllActiveValidationClientCheckingsByIdFollowUpControlAsync(this.Model.IdFollowUpControl); 
            }

        }
        #region CallBackAfterSave
        private async Task CallbackAfterSaveMTB(List<CandidateProviderPremisesCheckingVM> mtbs)
        {
            if (this.Model.IdFollowUpControl != 0)
            {
                foreach (var mtb in mtbs)
                {
                    this.mtbsSource.Add(mtb);
                }

                await mtbSfGrid.Refresh();
                this.StateHasChanged();
            }

        }

        private async Task CallbackAfterSaveTrainer(List<CandidateProviderTrainerCheckingVM> trainers)
        {
            if (this.Model.IdFollowUpControl != 0)
            {
                foreach (var trainer in trainers)
                {
                    this.trainersSource.Add(trainer);
                }

                await trainerSfGrid.Refresh();
                this.StateHasChanged();
            }

        }

        private async Task CallbackAfterSaveCourse(List<CourseCheckingVM> courses)
        {
            if (this.Model.IdFollowUpControl != 0)
            {
                foreach (var course in courses)
                {
                    this.courseSource.Add(course);
                }

                await courseSfGrid.Refresh();
                this.StateHasChanged();
            }

        }

        private async Task CallbackAfterSaveValidation(List<ValidationClientCheckingVM> validations)
        {
            if (this.Model.IdFollowUpControl != 0)
            {
                foreach (var validation in validations)
                {
                    this.validationSource.Add(validation);
                }

                await validationSfGrid.Refresh();
                this.StateHasChanged();
            }

        }
        #endregion

        #region Delete
        public async Task DeleteChecking(string Obj)
        {
            if (Obj == "MTB")
            {
                if (!selectedMTBs.Any())
                {
                    toast.sfErrorToast.Content = "Моля изберете запис!";
                    await toast.sfErrorToast.ShowAsync();
                }
                else
                {
                    this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
                    this.objType = Obj;
                }
            }
            else if (Obj == "Trainer")
            {
                if (!selectedTrainers.Any())
                {
                    toast.sfErrorToast.Content = "Моля изберете запис!";
                    await toast.sfErrorToast.ShowAsync();
                }
                else
                {
                    this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
                    this.objType = Obj;
                }
            }
            else if (Obj == "Course")
            {
                if (!selectedCourses.Any())
                {
                    toast.sfErrorToast.Content = "Моля изберете запис!";
                    await toast.sfErrorToast.ShowAsync();
                }
                else
                {
                    this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
                    this.objType = Obj;
                }
            }
            else if (Obj == "Validation")
            {
                if (!selectedValidation.Any())
                {
                    toast.sfErrorToast.Content = "Моля изберете запис!";
                    await toast.sfErrorToast.ShowAsync();
                }
                else
                {
                    this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
                    this.objType = Obj;
                }
            }
        }

        public async void ConfirmDeleteCallback()
        {
            if (objType == "MTB")
            {
                var mtbModelSource = new List<CandidateProviderPremisesCheckingVM>();
                foreach (var mtb in selectedMTBs)
                {
                    if (mtb.IdCandidateProviderPremisesChecking == 0)
                    {
                        this.mtbsSource.Remove(mtb);
                        mtbModelSource = this.mtbsSource;
                    }
                    else
                    {
                        var result = await this.CandidateProviderService.DeleteCandidateProviderPremisesCheckingAsync(mtb);
                        if (result.HasMessages)
                        {
                            toast.sfSuccessToast.Content = "Избраният обект на проверка е премахнат успешно!";
                            await toast.sfSuccessToast.ShowAsync();

                            this.mtbsSource = await this.CandidateProviderService.GetAllActiveCandidateProviderPremisesCheckingByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                        }
                    }
                }
                this.mtbsSource = await this.CandidateProviderService.GetAllActiveCandidateProviderPremisesCheckingByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                if (mtbModelSource.Count > 0)
                {
                    foreach (var mtb in mtbModelSource)
                    {
                        if (!mtbsSource.Any(x => x.IdCandidateProviderPremises == mtb.IdCandidateProviderPremises))
                        {
                            this.mtbsSource.Add(mtb);
                        }
                    }
                }
                await this.mtbSfGrid.Refresh();
                this.StateHasChanged();
            }
            else if (objType == "Trainer")
            {
                var trainerModelSource = new List<CandidateProviderTrainerCheckingVM>();
                foreach (var trainer in selectedTrainers)
                {
                    if (trainer.IdCandidateProviderTrainerChecking == 0)
                    {
                        this.trainersSource.Remove(trainer);
                        trainerModelSource = this.trainersSource;
                    }
                    else
                    {
                        var result = await this.CandidateProviderService.DeleteCandidateProviderTrainerCheckingAsync(trainer);
                        if (result.HasMessages)
                        {
                            toast.sfSuccessToast.Content = "Избраният обект на проверка е премахнат успешно!";
                            await toast.sfSuccessToast.ShowAsync();
                        }
                    }
                }
                this.trainersSource = await this.CandidateProviderService.GetAllActiveCandidateProviderTrainerCheckingByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                if (trainerModelSource.Count > 0)
                {
                    foreach (var trainer in trainerModelSource)
                    {
                        if (!trainersSource.Any(x => x.IdCandidateProviderTrainer == trainer.IdCandidateProviderTrainer))
                        {
                            this.trainersSource.Add(trainer);
                        }
                    }
                }
                await this.trainerSfGrid.Refresh();
                this.StateHasChanged();
            }
            else if (objType == "Course")
            {

                var courseModelSource = new List<CourseCheckingVM>();
                foreach (var course in selectedCourses)
                {
                    if (course.IdCourseChecking == 0)
                    {
                        this.courseSource.Remove(course);
                        courseModelSource = this.courseSource;
                    }
                    else
                    {
                        var result = await this.TrainingService.DeleteCourseCheckingAsync(course);
                        if (result.HasMessages)
                        {
                            toast.sfSuccessToast.Content = "Избраният обект на проверка е премахнат успешно!";
                            await toast.sfSuccessToast.ShowAsync();
                        }
                    }
                }
                this.courseSource = await this.TrainingService.GetAllActiveCourseCheckingsByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                if (courseModelSource.Count > 0)
                {
                    foreach (var course in courseModelSource)
                    {
                        if (!courseSource.Any(x => x.IdCourse == course.IdCourse))
                        {
                            this.courseSource.Add(course);
                        }
                    }
                }
                await this.courseSfGrid.Refresh();
                this.StateHasChanged();
            }
            else if (objType == "Validation")
            {

                var validationModelSource = new List<ValidationClientCheckingVM>();
                foreach (var validation in selectedValidation)
                {
                    if (validation.IdValidationClientChecking == 0)
                    {
                        this.validationSource.Remove(validation);
                        validationModelSource = this.validationSource;
                    }
                    else
                    {
                        var result = await this.TrainingService.DeleteValidationClientCheckingAsync(validation);
                        if (result.HasMessages)
                        {
                            toast.sfSuccessToast.Content = "Избраният обект на проверка е премахнат успешно!";
                            await toast.sfSuccessToast.ShowAsync();
                        }
                    }
                }
                this.validationSource = await this.TrainingService.GetAllActiveValidationClientCheckingsByIdFollowUpControlAsync(this.Model.IdFollowUpControl);
                if (validationModelSource.Count > 0)
                {
                    foreach (var validation in validationModelSource)
                    {
                        if (!validationSource.Any(x => x.IdValidationClient == validation.IdValidationClient))
                        {
                            this.validationSource.Add(validation);
                        }
                    }
                }
                await this.validationSfGrid.Refresh();
                this.StateHasChanged();
            }
        }

        #endregion

        #region Select & Deselect

        #region MTB
        private async Task MTBDeselectedHandler(RowDeselectEventArgs<CandidateProviderPremisesCheckingVM> args)
        {
            this.selectedMTBs.Clear();
            this.selectedMTBs = await this.mtbSfGrid.GetSelectedRecordsAsync();
        }

        private async Task MTBSelectedHandler(RowSelectEventArgs<CandidateProviderPremisesCheckingVM> args)
        {
            this.selectedMTBs.Clear();
            this.selectedMTBs = await this.mtbSfGrid.GetSelectedRecordsAsync();
        }
        #endregion

        #region Trainer
        private async Task TrainerDeselectedHandler(RowDeselectEventArgs<CandidateProviderTrainerCheckingVM> args)
        {
            this.selectedTrainers.Clear();
            this.selectedTrainers = await this.trainerSfGrid.GetSelectedRecordsAsync();
        }

        private async Task TrainerSelectedHandler(RowSelectEventArgs<CandidateProviderTrainerCheckingVM> args)
        {
            this.selectedTrainers.Clear();
            this.selectedTrainers = await this.trainerSfGrid.GetSelectedRecordsAsync();
        }
        #endregion

        #region Course
        private async Task CourseDeselectedHandler(RowDeselectEventArgs<CourseCheckingVM> args)
        {
            this.selectedCourses.Clear();
            this.selectedCourses = await this.courseSfGrid.GetSelectedRecordsAsync();
        }

        private async Task CourseSelectedHandler(RowSelectEventArgs<CourseCheckingVM> args)
        {
            this.selectedCourses.Clear();
            this.selectedCourses = await this.courseSfGrid.GetSelectedRecordsAsync();
        }
        #endregion

        #region Validation
        private async Task ValidationDeselectedHandler(RowDeselectEventArgs<ValidationClientCheckingVM> args)
        {
            this.selectedValidation.Clear();
            this.selectedValidation = await this.validationSfGrid.GetSelectedRecordsAsync();
        }

        private async Task ValidationSelectedHandler(RowSelectEventArgs<ValidationClientCheckingVM> args)
        {
            this.selectedValidation.Clear();
            this.selectedValidation = await this.validationSfGrid.GetSelectedRecordsAsync();
        }
        #endregion

        #endregion

    }
    public class CheckingObjectVM
    {
        public List<int> MTBids { get; set; }
        public List<int> TrainerIds { get; set; }
        public List<int> CourseIds { get; set; }
        public List<int> ValidationIds { get; set; }
    }
}
