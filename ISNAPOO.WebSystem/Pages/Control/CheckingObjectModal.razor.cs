using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.SPPOO;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.DropDowns;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Syncfusion.Blazor.Data;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.ViewModels.Register;

namespace ISNAPOO.WebSystem.Pages.Control
{
    partial class CheckingObjectModal : BlazorBaseComponent
    {

        #region Parameters
        [Parameter]

        public EventCallback<List<CandidateProviderPremisesCheckingVM>> CallbackAfterSaveMTB { get; set; }

        [Parameter]

        public EventCallback<List<CandidateProviderTrainerCheckingVM>> CallbackAfterSaveTrainer { get; set; }

        [Parameter]

        public EventCallback<List<CourseCheckingVM>> CallbackAfterSaveCourse { get; set; }

        [Parameter]

        public EventCallback<List<ValidationClientCheckingVM>> CallbackAfterSaveValidation { get; set; }

        [Parameter]

        public bool IsCPO { get; set; } = true;
        #endregion

        #region Inject
        [Inject]
        public ITrainingService TrainingService { get; set; }
        
        [Inject]
        public IControlService ControlService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IProfessionService ProfessionService { get; set; }

        [Inject]
        public ISpecialityService SpecialityService { get; set; }
        #endregion

        #region PageProperties
        private SfGrid<CandidateProviderPremisesVM> sfGridMTB = new SfGrid<CandidateProviderPremisesVM>();
        private SfGrid<CandidateProviderTrainerVM> sfGridTrainer = new SfGrid<CandidateProviderTrainerVM>();
        private SfGrid<CourseVM> sfGridCourse = new SfGrid<CourseVM>();
        private SfGrid<ValidationClientVM> sfGridValidation = new SfGrid<ValidationClientVM>();
        private SfAutoComplete<int?, ProfessionVM> sfAutoCompleteProfession = new SfAutoComplete<int?, ProfessionVM>();
        private SfMultiSelect<List<SpecialityVM>, SpecialityVM> multiSelect = new SfMultiSelect<List<SpecialityVM>, SpecialityVM>();
        #endregion

        #region Source
        private IEnumerable<CandidateProviderPremisesVM> mtbsSource = new List<CandidateProviderPremisesVM>();
        private IEnumerable<CandidateProviderTrainerVM> trainersSource = new List<CandidateProviderTrainerVM>();
        private IEnumerable<CourseVM> courseSource = new List<CourseVM>();
        private IEnumerable<ValidationClientVM> validationSource = new List<ValidationClientVM>();
        List<CandidateProviderPremisesCheckingVM> mtbCheckings = new List<CandidateProviderPremisesCheckingVM>();
        List<CandidateProviderTrainerCheckingVM> trainerCheckings = new List<CandidateProviderTrainerCheckingVM>();
        List<CourseCheckingVM> courseCheckings = new List<CourseCheckingVM>();
        List<ValidationClientCheckingVM> validationCheckings = new List<ValidationClientCheckingVM>();
        private IEnumerable<KeyValueVM> kvTypeFrameworkProgram = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvOwnership = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvTypeOfEducation = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvTypePracticeOrTheory = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvStatus = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvStatusTrainers = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> courseTypeSource = new List<KeyValueVM>();
        public List<ProfessionVM> professionSource = new List<ProfessionVM>();
        private List<SpecialityVM> specialities = new List<SpecialityVM>();
        private List<SpecialityVM> FilterSpecialities = new List<SpecialityVM>();
        private List<SpecialityVM> specialitiesSource = new List<SpecialityVM>();
        private List<CandidateProviderPremisesVM> selectedMTBs = new List<CandidateProviderPremisesVM>();
        private List<CandidateProviderTrainerVM> selectedTrainers = new List<CandidateProviderTrainerVM>();
        private List<CourseVM> selectedCourses = new List<CourseVM>();
        private List<ValidationClientVM> selectedValidation = new List<ValidationClientVM>();
        #endregion

        #region Models
        private CandidateProviderPremisesCheckingVM mtbModel = new CandidateProviderPremisesCheckingVM();
        private CandidateProviderTrainerCheckingVM trainerModel = new CandidateProviderTrainerCheckingVM();
        private FollowUpControlVM followUpControl = new FollowUpControlVM();
        private CourseCheckingVM courseModel = new CourseCheckingVM();
        private ValidationClientCheckingVM validationModel = new ValidationClientCheckingVM(); 
        private FilterPremisesVM FilterPremisesModel = new FilterPremisesVM();
        private FilterTrainersVM FilterTrainersModel = new FilterTrainersVM();
        private FilterCoursesVM FilterCoursesModel = new FilterCoursesVM();
        private FilterValidationVM FilterValidationModel = new FilterValidationVM();
        #endregion

        #region Fields
        List<string> validationMessages = new List<string>();
        private string Type = string.Empty;
        private int IdCandidateProvider = 0;
        private string CreationDateStr = "";
        private string ModifyDateStr = "";
        private string CreationPersonName = "";
        private string ModifyPersonName = "";
        #endregion

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.mtbModel);
            this.kvTypeFrameworkProgram = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).ToList().Where(k => k.KeyValueIntCode == "ProfessionalQualification" || k.KeyValueIntCode == "PartProfession" || k.KeyValueIntCode == "CourseRegulation1And7").ToList();
            this.kvStatus = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseStatus", false);
            this.kvStatusTrainers = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderTrainerStatus", false);
            this.kvOwnership = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MaterialTechnicalBaseOwnership", false);
            this.kvTypePracticeOrTheory = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType", false);
            this.kvTypeOfEducation = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType", false);
            this.courseTypeSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).ToList().Where(k => k.KeyValueIntCode == "ValidationOfProfessionalQualifications" || k.KeyValueIntCode == "ValidationOfPartOfProfession").ToList();
            this.professionSource = (await this.ProfessionService.GetAllActiveProfessionsAsync()).ToList();
        }

        public async Task OpenModal(string type, int idCandidateProvider, int IdFollowUpControl, DateTime? endDate, List<CandidateProviderPremisesCheckingVM> _mtbCheckings = null, List<CandidateProviderTrainerCheckingVM> _trainerCheckings = null, List<CourseCheckingVM> _courseCheckings = null, List<ValidationClientCheckingVM> _validationCheckings = null)
        {
            this.validationMessages.Clear();
            this.FilterPremisesModel = new FilterPremisesVM();
            this.FilterCoursesModel = new FilterCoursesVM();
            this.FilterTrainersModel = new FilterTrainersVM();
            this.FilterValidationModel = new FilterValidationVM();
            this.IdCandidateProvider = idCandidateProvider;
            this.mtbModel = new CandidateProviderPremisesCheckingVM();
            this.trainerModel = new CandidateProviderTrainerCheckingVM();
            this.courseModel = new CourseCheckingVM();
            this.validationModel = new ValidationClientCheckingVM();
            this.CreationDateStr = "";
            this.ModifyDateStr = "";
            this.CreationPersonName = "";
            this.ModifyPersonName = "";
            this.selectedMTBs = new List<CandidateProviderPremisesVM>();
            this.selectedTrainers = new List<CandidateProviderTrainerVM>();
            this.selectedCourses = new List<CourseVM>();
            this.selectedValidation = new List<ValidationClientVM>();
            this.mtbCheckings = _mtbCheckings;
            this.trainerCheckings = _trainerCheckings;
            this.courseCheckings = _courseCheckings;
            this.validationCheckings = _validationCheckings;
            this.followUpControl = await this.ControlService.GetControlByIdFollowUpControlAsync(IdFollowUpControl);
            var candidateProviderVM = await this.CandidateProviderService.GetCandidateProviderByIdAsync(new CandidateProviderVM() { IdCandidate_Provider = idCandidateProvider });
            foreach (var CandidateProviderSpeciality in candidateProviderVM.CandidateProviderSpecialities)
            {
                if (!this.specialities.Any(x => x.IdSpeciality == CandidateProviderSpeciality.IdSpeciality))
                {
                    this.specialities.Add(await this.SpecialityService.GetSpecialityByIdAsync(CandidateProviderSpeciality.IdSpeciality));
                }
            }


            specialitiesSource = this.specialities;
            if (type == "MTB")
            {
                this.Type = "Материално-техническа база";
                this.editContext = new EditContext(this.mtbModel);
                this.mtbModel.IdFollowUpControl = IdFollowUpControl;
                this.mtbModel.CheckingDate = endDate;
                await this.LoadDataSource("MTB");
            }
            else if (type == "Trainer")
            {
                if (IsCPO)
                {
                    this.Type = "Преподавател";
                }
                else
                {
                    this.Type = "Консултант";
                }
                this.editContext = new EditContext(this.trainerModel);
                this.trainerModel.IdFollowUpControl = IdFollowUpControl;
                this.trainerModel.CheckingDate = endDate;
                await this.LoadDataSource("Trainer");
            }
            else if (type == "Course")
            {
                this.Type = "Курс";
                this.editContext = new EditContext(this.courseModel);
                this.courseModel.IdFollowUpControl = IdFollowUpControl;
                this.courseModel.CheckingDate = endDate;
                if (this.followUpControl.PeriodFrom.HasValue)
                {
                    this.FilterCoursesModel.endCourseFrom = this.followUpControl.PeriodFrom.Value;
                }
                if (this.followUpControl.PeriodTo.HasValue)
                {
                    this.FilterCoursesModel.endtCourseTo = this.followUpControl.PeriodTo.Value;
                }
                await this.LoadDataSource("Course");
            }
            else if (type == "Validation")
            {
                this.Type = "Валидирано лице";

                this.editContext = new EditContext(this.validationModel);
                this.validationModel.IdFollowUpControl = IdFollowUpControl;
                this.validationModel.CheckingDate = endDate;

                await this.LoadDataSource("Validation");
            }
            this.courseSource = this.courseSource.OrderBy(c => c.CourseNameWithStartAndEndDate).ToList();
            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Save()
        {
            bool hasPermission = await CheckUserActionPermission("ManageFollowUpControlData", false);
            if (!hasPermission) { return; }
            this.SpinnerShow();
            if (this.Type == "Материално-техническа база")
            {
                this.editContext = new EditContext(this.mtbModel);
                if (this.selectedMTBs.Count > 0)
                {
                    this.mtbModel.IdCandidateProviderPremises = 0;
                }
            }
            else if (this.Type == "Преподавател" || this.Type == "Консултант")
            {
                this.editContext = new EditContext(this.trainerModel);
                if (this.selectedTrainers.Count > 0)
                {
                    this.trainerModel.IdCandidateProviderTrainer = 0;
                }
            }
            else if (this.Type == "Курс")
            {
                this.editContext = new EditContext(this.courseModel);
                if (this.selectedCourses.Count > 0)
                {
                    this.courseModel.IdCourse = 0;
                }
            }
            else if (this.Type == "Валидирано лице")
            {
                this.editContext = new EditContext(this.validationModel);
                if (this.selectedValidation.Count > 0)
                {
                    this.validationModel.IdValidationClient = 0;
                }
            }
            this.editContext.EnableDataAnnotationsValidation();

            var result = 0;

            this.validationMessages.Clear();
            bool isValid = this.editContext.Validate();
            this.validationMessages.AddRange(this.editContext.GetValidationMessages());


            if (isValid)
            {
                if (this.Type == "Материално-техническа база")
                {
                    List<CandidateProviderPremisesCheckingVM> resultMTB = new List<CandidateProviderPremisesCheckingVM>();

                    foreach (var mtb in selectedMTBs)
                    {
                        resultMTB.Add(new CandidateProviderPremisesCheckingVM()
                        {
                            IdCandidateProviderPremises = mtb.IdCandidateProviderPremises,
                            CandidateProviderPremises = await this.CandidateProviderService.GetCandidateProviderPremisesByIdAsync(new CandidateProviderPremisesVM() { IdCandidateProviderPremises = mtb.IdCandidateProviderPremises }),
                            IdFollowUpControl = this.mtbModel.IdFollowUpControl,
                            CheckDone = this.mtbModel.CheckDone,
                            CheckingDate = this.mtbModel.CheckingDate,
                            Comment = this.mtbModel.Comment
                        });
                    }

                    await CallbackAfterSaveMTB.InvokeAsync(resultMTB);

                }
                else if (this.Type == "Преподавател" || this.Type == "Консултант")
                {
                    List<CandidateProviderTrainerCheckingVM> resultTrainer = new List<CandidateProviderTrainerCheckingVM>();

                    foreach (var trainer in selectedTrainers)
                    {
                        resultTrainer.Add(new CandidateProviderTrainerCheckingVM()
                        {
                            IdCandidateProviderTrainer = trainer.IdCandidateProviderTrainer,
                            CandidateProviderTrainer = await this.CandidateProviderService.GetCandidateProviderTrainerByIdAsync(new CandidateProviderTrainerVM() { IdCandidateProviderTrainer = trainer.IdCandidateProviderTrainer }),
                            IdFollowUpControl = this.trainerModel.IdFollowUpControl,
                            CheckDone = this.trainerModel.CheckDone,
                            CheckingDate = this.trainerModel.CheckingDate,
                            Comment = this.trainerModel.Comment
                        });
                    }

                    await CallbackAfterSaveTrainer.InvokeAsync(resultTrainer);

                }
                else if (this.Type == "Курс")
                {
                    List<CourseCheckingVM> resultCourse = new List<CourseCheckingVM>();

                    foreach (var course in selectedCourses)
                    {
                        resultCourse.Add(new CourseCheckingVM()
                        {
                            IdCourse = course.IdCourse,
                            Course = await this.TrainingService.GetTrainingCourseByIdAsync(course.IdCourse),
                            IdFollowUpControl = this.courseModel.IdFollowUpControl,
                            CheckDone = this.courseModel.CheckDone,
                            CheckingDate = this.courseModel.CheckingDate,
                            Comment = this.courseModel.Comment
                        });
                    }

                    await CallbackAfterSaveCourse.InvokeAsync(resultCourse);
                }
                else if (this.Type == "Валидирано лице")
                {
                    List<ValidationClientCheckingVM> resultValidation = new List<ValidationClientCheckingVM>();

                    foreach (var validation in selectedValidation)
                    {
                        resultValidation.Add(new ValidationClientCheckingVM()
                        {
                            IdValidationClient = validation.IdValidationClient,
                            ValidationClient = await this.TrainingService.GetValidationClientByIdAsync(validation.IdValidationClient),
                            IdFollowUpControl = this.validationModel.IdFollowUpControl,
                            CheckDone = this.validationModel.CheckDone,
                            CheckingDate = this.validationModel.CheckingDate,
                            Comment = this.validationModel.Comment
                        });
                    }

                    await CallbackAfterSaveValidation.InvokeAsync(resultValidation);
                }

                this.isVisible = false;
                this.StateHasChanged();
            }
                this.selectedMTBs = new List<CandidateProviderPremisesVM>();
                this.selectedTrainers = new List<CandidateProviderTrainerVM>();
                this.selectedCourses = new List<CourseVM>();
                this.selectedValidation = new List<ValidationClientVM>();
            this.SpinnerHide();
        }



        private async Task SumbitFilter(string filteredType)
        {
            this.SpinnerShow();
            if (filteredType == "MTB")
            {
                await this.FilterPremises();
            }
            else if (filteredType == "Trainer")
            {
                await this.FilterTrainers();
            }
            else if (filteredType == "Course")
            {
                await this.FilterCourses();
            }
            else if (filteredType == "Validation")
            {
                await this.FilterValidation();
            }
            this.SpinnerHide();
        }

        public async Task ClearFilter(string filteredType)
        {
            this.FilterPremisesModel = new FilterPremisesVM();
            this.FilterTrainersModel = new FilterTrainersVM();
            this.FilterCoursesModel = new FilterCoursesVM();
            this.FilterValidationModel = new FilterValidationVM();
            this.FilterSpecialities = new List<SpecialityVM>();
            if (filteredType == "MTB")
            {
                await this.LoadDataSource("MTB");
            }
            else if (filteredType == "Trainer")
            {
                await this.LoadDataSource("Trainer");
            }
            else if (filteredType == "Course")
            {
                await this.LoadDataSource("Course");
            }
            else if (filteredType == "Validation")
            {
                await this.LoadDataSource("Validation");
            }
        }

        private async Task LoadDataSource(string type)
        {
            if (type == "MTB")
            {
                this.mtbsSource = (await this.CandidateProviderService.GetAllCandidateProviderPremisesByCandidateProviderIdWithPremisesSpecialitiesIncludedAsync(this.IdCandidateProvider)).ToList();
                if (mtbCheckings != null && mtbCheckings.Count != 0)
                {
                    foreach (var item in mtbsSource)
                    {
                        if (mtbCheckings.Any(x => x.IdCandidateProviderPremises == item.IdCandidateProviderPremises))
                        {
                            this.mtbsSource = mtbsSource.Where(m => m.IdCandidateProviderPremises != item.IdCandidateProviderPremises).ToList();
                        }
                    }
                }
            }
            else if (type == "Trainer")
            {
                this.trainersSource = (await this.CandidateProviderService.GetAllCandidateProviderTrainersByCandidateProviderIdWithTrainerSpecialitiesIncludedAsync(this.IdCandidateProvider)).ToList();
                if (trainerCheckings != null && trainerCheckings.Count != 0)
                {
                    foreach (var item in trainersSource)
                    {
                        if (trainerCheckings.Any(x => x.IdCandidateProviderTrainer == item.IdCandidateProviderTrainer))
                        {
                            this.trainersSource = trainersSource.Where(m => m.IdCandidateProviderTrainer != item.IdCandidateProviderTrainer).ToList();
                        }
                    }
                }
            }
            else if (type == "Course")
            {

                StateExaminationInfoFilterListVM filterListVM = new StateExaminationInfoFilterListVM();
                filterListVM.IdCandidateProvider = this.IdCandidateProvider;


                this.courseSource = (await this.TrainingService.getAllCourses(filterListVM, "")).ToList();
                if (courseCheckings != null && courseCheckings.Count != 0)
                {
                    foreach (var item in courseSource)
                    {
                        if (courseCheckings.Any(x => x.IdCourse == item.IdCourse))
                        {
                            this.courseSource = courseSource.Where(m => m.IdCourse != item.IdCourse).ToList();
                        }
                    }
                }
                this.courseSource = this.courseSource.OrderBy(x => x.CourseNameWithStartAndEndDate).ToList();
            }
            else if (type == "Validation")
            {
                this.validationSource = (await this.TrainingService.getAllValidationClients()).ToList();
                if (validationCheckings != null && validationCheckings.Count != 0)
                {
                    foreach (var item in validationSource)
                    {
                        if (validationCheckings.Any(x => x.IdValidationClient == item.IdValidationClient))
                        {
                            this.validationSource = validationSource.Where(m => m.IdValidationClient != item.IdValidationClient).ToList();
                        }
                    }
                }
                
            }
        }

        #region Select & Deselect

        #region MTB
        private async Task MTBDeselectedHandler(RowDeselectEventArgs<CandidateProviderPremisesVM> args)
        {
            this.selectedMTBs.Clear();
            this.selectedMTBs = await this.sfGridMTB.GetSelectedRecordsAsync();
        }

        private async Task MTBSelectedHandler(RowSelectEventArgs<CandidateProviderPremisesVM> args)
        {
            this.selectedMTBs.Clear();
            this.selectedMTBs = await this.sfGridMTB.GetSelectedRecordsAsync();
        }
        #endregion

        #region Trainer
        private async Task TrainerDeselectedHandler(RowDeselectEventArgs<CandidateProviderTrainerVM> args)
        {
            this.selectedTrainers.Clear();
            this.selectedTrainers = await this.sfGridTrainer.GetSelectedRecordsAsync();
        }

        private async Task TrainerSelectedHandler(RowSelectEventArgs<CandidateProviderTrainerVM> args)
        {
            this.selectedTrainers.Clear();
            this.selectedTrainers = await this.sfGridTrainer.GetSelectedRecordsAsync();
        }
        #endregion

        #region Course
        private async Task CourseDeselectedHandler(RowDeselectEventArgs<CourseVM> args)
        {
            this.selectedCourses.Clear();
            this.selectedCourses = await this.sfGridCourse.GetSelectedRecordsAsync();
        }

        private async Task CourseSelectedHandler(RowSelectEventArgs<CourseVM> args)
        {
            this.selectedCourses.Clear();
            this.selectedCourses = await this.sfGridCourse.GetSelectedRecordsAsync();
        }
        #endregion

        #region Validation

        //TODO: Select & Deselect ValidationClientChecking
        private async Task ValidationDeselectedHandler(RowDeselectEventArgs<ValidationClientVM> args)
        {
            this.selectedValidation.Clear();
            this.selectedValidation = await this.sfGridValidation.GetSelectedRecordsAsync();
        }

        private async Task ValidationSelectedHandler(RowSelectEventArgs<ValidationClientVM> args)
        {
            this.selectedValidation.Clear();
            this.selectedValidation = await this.sfGridValidation.GetSelectedRecordsAsync();
        }
        #endregion

        #endregion

        #region FilterCourse
        private async Task FilterCourses()
        {
            await this.LoadDataSource("Course");
            courseSource = courseSource.Where(d =>
                (this.FilterCoursesModel.IdTrainingCourseType is not null && this.FilterCoursesModel.IdTrainingCourseType != 0 ? d.IdTrainingCourseType == this.FilterCoursesModel.IdTrainingCourseType : true)
                && (this.FilterCoursesModel.IdProfession.HasValue && this.FilterCoursesModel.IdProfession.Value != 0 ? d.Program.Speciality.Profession.IdProfession == this.FilterCoursesModel.IdProfession.Value : true)
                && (this.FilterCoursesModel.IdSpeciality is not null && this.FilterCoursesModel.IdSpeciality != 0 ? d.Program.Speciality.IdSpeciality == this.FilterCoursesModel.IdSpeciality : true)).ToList();

            if (this.FilterCoursesModel.startCourseFrom != null && this.FilterCoursesModel.startCourseTo != null)
            {
                courseSource = courseSource
                    .Where(x => x.StartDate >= this.FilterCoursesModel.startCourseFrom && x.StartDate <= this.FilterCoursesModel.startCourseTo.Value.AddDays(1))
                    .ToList();
            }
            else if (this.FilterCoursesModel.startCourseFrom != null && this.FilterCoursesModel.startCourseTo == null)
            {
                courseSource = courseSource
                                    .Where(x => x.StartDate >= this.FilterCoursesModel.startCourseFrom)
                                    .ToList();
            }
            else if (this.FilterCoursesModel.startCourseFrom == null && this.FilterCoursesModel.startCourseTo != null)
            {
                courseSource = courseSource
                                    .Where(x => x.StartDate <= this.FilterCoursesModel.startCourseTo.Value.AddDays(1))
                                    .ToList();
            }
            if (this.FilterCoursesModel.endCourseFrom != null && this.FilterCoursesModel.endtCourseTo != null)
            {
                courseSource = courseSource
                           .Where(x => x.EndDate >= this.FilterCoursesModel.endCourseFrom && x.EndDate <= this.FilterCoursesModel.endtCourseTo.Value.AddDays(1))
                           .ToList();
            }
            else if (this.FilterCoursesModel.endCourseFrom != null && this.FilterCoursesModel.endtCourseTo == null)
            {
                courseSource = courseSource
                                    .Where(x => x.EndDate >= this.FilterCoursesModel.endCourseFrom)
                                    .ToList();
            }
            else if (this.FilterCoursesModel.endCourseFrom == null && this.FilterCoursesModel.endtCourseTo != null)
            {
                courseSource = courseSource
                                    .Where(x => x.EndDate <= this.FilterCoursesModel.endtCourseTo.Value.AddDays(1))
                                    .ToList();
            }
            courseSource = courseSource.OrderBy(x => x.CourseNameWithStartAndEndDate).ToList();
        }


        #endregion

        #region FilterTrainer
        public async Task OnKvEducationtypeSelect()
        {
            FilterTrainersModel.kvVMPracticeOrTheory = await this.DataSourceService.GetKeyValueByIdAsync(FilterTrainersModel.kvPracticeOrTheory);
        }

        public async Task FilterTrainers()
        {
            await this.LoadDataSource("Trainer");
            this.trainersSource = this.trainersSource.Where(d =>
            (FilterTrainersModel.IdStatus != 0 ? d.IdStatus == FilterTrainersModel.IdStatus : true)
            && (FilterTrainersModel.Specialities is not null && FilterTrainersModel.Specialities.Count != 0 ? d.CandidateProviderTrainerSpecialities.Any(y => this.FilterTrainersModel.Specialities.All(x => x.IdSpeciality == y.IdSpeciality)) : true)
            && ((FilterTrainersModel.Specialities is null || FilterTrainersModel.Specialities.Count == 0) && (FilterTrainersModel.IdProfession != 0) ? d.CandidateProviderTrainerSpecialities.Any(y => y.Speciality.IdProfession == FilterTrainersModel.IdProfession) : true)
            ).ToList();
            if (this.trainersSource.Any() && this.FilterTrainersModel.kvVMPracticeOrTheory != null)
            {
                var list = new List<CandidateProviderTrainerVM>();
                foreach (var speciality in FilterTrainersModel.Specialities)
                {
                    foreach (var trainer in this.trainersSource)
                    {
                        if (trainer.CandidateProviderTrainerSpecialities.Any(x => x.IdSpeciality == speciality.IdSpeciality))
                        {
                            foreach (var trainerSpeciality in trainer.CandidateProviderTrainerSpecialities.Where(x => x.IdSpeciality == speciality.IdSpeciality).ToList())
                            {
                                if (this.FilterTrainersModel.kvVMPracticeOrTheory.KeyValueIntCode == "TrainingInTheoryAndPractice")
                                {
                                    if (this.FilterTrainersModel.kvVMPracticeOrTheory.IdKeyValue == trainerSpeciality.IdUsage)
                                    {
                                        list.Add(trainer);
                                    }
                                }
                                else
                                {
                                    if (this.FilterTrainersModel.kvVMPracticeOrTheory.IdKeyValue == trainerSpeciality.IdUsage || this.kvTypePracticeOrTheory.First(x => x.KeyValueIntCode == "TrainingInTheoryAndPractice").IdKeyValue == trainerSpeciality.IdUsage)
                                    {
                                        list.Add(trainer);
                                    }
                                }
                            }
                        }
                    }

                    this.trainersSource = list;
                }
            }
        }

        private async Task OnProfessionSelectBlur()
        {
            this.FilterSpecialities = this.FilterSpecialities.Where(x => x.IdProfession == FilterTrainersModel.IdProfession).ToList();
        }
        #endregion

        #region FilterMTB

        private async Task FilterPremises()
        {
            await this.LoadDataSource("MTB");
            mtbsSource = mtbsSource.Where(d => (this.FilterPremisesModel.IdOwnerShip is not null && this.FilterPremisesModel.IdOwnerShip != 0 ? d.IdOwnership == this.FilterPremisesModel.IdOwnerShip : true)
                && (this.FilterPremisesModel.IdStatus is not null && this.FilterPremisesModel.IdStatus != 0 ? d.IdStatus == this.FilterPremisesModel.IdStatus : true)
                && (this.FilterPremisesModel.IdProfession is not null && this.FilterPremisesModel.IdProfession != 0 ? d.CandidateProviderPremisesSpecialities.Any(x => x.Speciality.IdProfession == this.FilterPremisesModel.IdProfession) : true)
                && (this.FilterPremisesModel.IdTypeOfEducation is not null && this.FilterPremisesModel.IdTypeOfEducation != 0 ? d.CandidateProviderPremisesSpecialities.Any(x => x.IdUsage == this.FilterPremisesModel.IdTypeOfEducation) : true)
                && (FilterSpecialities != null && FilterSpecialities.Count != 0 ? !d.CandidateProviderPremisesSpecialities.All(x => FilterSpecialities.Any(y => x.IdSpeciality == y.IdSpeciality)) : true)).ToList();
        }

        private async Task OnFilterProfession(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    this.professionSource = (await this.ProfessionService.GetAllActiveProfessionsAsync()).ToList().Where(x => x.CodeAndName.Contains(args.Text)).ToList();

                    var query = new Query().Where(new WhereFilter() { Field = "CodeAndName", Operator = "contains", value = args.Text, IgnoreCase = true });

                    query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                    await this.sfAutoCompleteProfession.FilterAsync(this.professionSource, query);
                }
                catch (Exception ex) { }


            }
        }

        private async Task OnProfessionSelect(SelectEventArgs<ProfessionVM> args)
        {
            this.FilterSpecialities = this.FilterSpecialities.Where(x => x.IdProfession == args.ItemData.IdProfession).ToList();
        }



        private async Task OnFilterSpeciality(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    if (FilterPremisesModel.IdProfession is not null && FilterPremisesModel.IdProfession != 0)
                    {
                        this.specialities = (List<SpecialityVM>)this.specialitiesSource.Where(x => x.CodeAndAreaForAutoCompleteSearch.Contains(args.Text) && x.IdProfession == FilterPremisesModel.IdProfession).ToList();
                    }
                    else
                    {
                        this.specialities = (List<SpecialityVM>)this.specialitiesSource.Where(x => x.CodeAndAreaForAutoCompleteSearch.Contains(args.Text)).ToList();
                    }

                }
                catch (Exception ex) { }

                var query = new Query().Where(new WhereFilter() { Field = "CodeAndAreaForAutoCompleteSearch", Operator = "contains", value = args.Text, IgnoreCase = true });

                query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

                await this.multiSelect.FilterAsync(this.specialities, query);
            }
        }

        private async Task OnFocusSpeciality()
        {
            if (!(FilterPremisesModel.IdProfession == null || FilterPremisesModel.IdProfession == 0))
            {
                this.specialities = (List<SpecialityVM>)this.specialities.Where(x => x.IdProfession == FilterPremisesModel.IdProfession).ToList();
            }
            else
            {
                this.specialities = specialitiesSource;
            }
        }
        #endregion

        #region FilterValidation


        private async Task FilterValidation()
        {
            await this.LoadDataSource("Validation");
            validationSource = validationSource.Where(d => (this.FilterValidationModel.IdCourseType.HasValue && this.FilterValidationModel.IdCourseType.Value != 0 ? d.IdCourseType == this.FilterValidationModel.IdCourseType : true)
                && (this.FilterValidationModel.IdProfession.HasValue && this.FilterValidationModel.IdProfession.Value != 0 ? d.Speciality.Profession.IdProfession == this.FilterValidationModel.IdProfession : true)
                && (this.FilterValidationModel.IdSpeciality.HasValue && this.FilterValidationModel.IdSpeciality.Value != 0 ? d.Speciality.IdSpeciality == this.FilterValidationModel.IdSpeciality : true)).ToList();

            if (this.FilterValidationModel.StartProcedureDateFrom != null && this.FilterValidationModel.StartProcedureDateTo != null)
            {
                validationSource = validationSource
                    .Where(x => x.StartDate >= this.FilterValidationModel.StartProcedureDateFrom && x.StartDate <= this.FilterValidationModel.StartProcedureDateTo.Value.AddDays(1))
                    .ToList();
            }
            else if (this.FilterValidationModel.StartProcedureDateFrom != null && this.FilterValidationModel.StartProcedureDateTo == null)
            {
                validationSource = validationSource
                                    .Where(x => x.StartDate >= this.FilterValidationModel.StartProcedureDateFrom)
                                    .ToList();
            }
            else if (this.FilterValidationModel.StartProcedureDateFrom == null && this.FilterValidationModel.StartProcedureDateTo != null)
            {
                validationSource = validationSource
                                    .Where(x => x.StartDate <= this.FilterValidationModel.StartProcedureDateTo.Value.AddDays(1))
                                    .ToList();
            }
            if (this.FilterValidationModel.EndProcedureDateFrom != null && this.FilterValidationModel.EndProcedureDateTo != null)
            {
                validationSource = validationSource
                           .Where(x => x.EndDate >= this.FilterValidationModel.EndProcedureDateFrom && x.EndDate <= this.FilterValidationModel.EndProcedureDateTo.Value.AddDays(1))
                           .ToList();
            }
            else if (this.FilterValidationModel.EndProcedureDateFrom != null && this.FilterValidationModel.EndProcedureDateTo == null)
            {
                validationSource = validationSource
                                    .Where(x => x.EndDate >= this.FilterValidationModel.EndProcedureDateFrom)
                                    .ToList();
            }
            else if (this.FilterValidationModel.EndProcedureDateFrom == null && this.FilterValidationModel.EndProcedureDateTo != null)
            {
                validationSource = validationSource
                                    .Where(x => x.EndDate <= this.FilterValidationModel.EndProcedureDateTo.Value.AddDays(1))
                                    .ToList();
            }
        }
        #endregion

    }
    public class FilterValidationVM
    {
        public int? IdCourseType { get; set; }
        public int? IdProfession { get; set; }
        public int? IdSpeciality { get; set; }
        public DateTime? StartProcedureDateFrom { get; set; }
        public DateTime? StartProcedureDateTo { get; set; }
        public DateTime? EndProcedureDateFrom { get; set; }
        public DateTime? EndProcedureDateTo { get; set; }
        //TODO: FilterValidationVM ValidationClientChecking
    }
    public class FilterPremisesVM
    {
        public int? IdOwnerShip { get; set; }
        public int? IdTypeOfEducation { get; set; }
        public int? IdStatus { get; set; }
        public int? IdProfession { get; set; }
    }
    public class FilterTrainersVM
    {
        public int IdProfession { get; set; }
        public List<SpecialityVM> Specialities { get; set; } = new List<SpecialityVM>();
        public int? kvPracticeOrTheory { get; set; }
        public int IdStatus { get; set; }
        public KeyValueVM kvVMPracticeOrTheory { get; set; }
    }
    public class FilterCoursesVM
    {
        public int? IdTrainingCourseType { get; set; }
        public int? IdProfession { get; set; }
        public int? IdSpeciality { get; set; }
        public DateTime? startCourseFrom { get; set; }
        public DateTime? startCourseTo { get; set; }
        public DateTime? endCourseFrom { get; set; }
        public DateTime? endtCourseTo { get; set; }
    }
}
