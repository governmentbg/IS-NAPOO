using System.Collections.Generic;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CurrentCourseInformation : BlazorBaseComponent
    {
        private string professionSpecialityAndSPKValue = string.Empty;
        private IEnumerable<KeyValueVM> vqsSource = new List<KeyValueVM>();
        private string emptyPremises = string.Empty;
        private IEnumerable<CandidateProviderPremisesVM> premisesSource = new List<CandidateProviderPremisesVM>();
        private IEnumerable<KeyValueVM> measureTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> assignTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> originalAssignTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> formEducationSource = new List<KeyValueVM>();
        private ValidationMessageStore? messageStore;
        private string theoryExamLabelTitle = string.Empty;
        private string practiceExamLabelTitle = string.Empty;

        [Parameter]
        public CourseVM CourseVM { get; set; }

        [Parameter]
        public bool IsEditable { get; set; } = true;

        [Parameter]
        public bool EntryFroomLegalCapacityModule { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.CourseVM);
            this.FormTitle = "Информация за курса";

            await this.LoadKVSourcesAsync();

            if (this.CourseVM.Program is not null)
            {
                this.premisesSource = await this.TrainingService.GetAllActiveCandidateProviderPremisesByIdCandidateProviderAndIdSpecialityAsync(this.CourseVM.IdCandidateProvider!.Value, this.CourseVM.Program.IdSpeciality);

                if (this.CourseVM.Program.Speciality is not null)
                {
                    if (this.CourseVM.Program.Speciality.Profession is not null)
                    {
                        var spkValue = this.vqsSource.FirstOrDefault(x => x.IdKeyValue == this.CourseVM.Program.Speciality.IdVQS);
                        if (spkValue is not null)
                        {
                            this.professionSpecialityAndSPKValue = $"{this.CourseVM.Program.Speciality.Profession.CodeAndName}, {this.CourseVM.Program.Speciality.CodeAndName} - {spkValue.Name}";
                        }
                    }
                }
            }

            this.FilterAssignTypeSource();

            await this.SetExamLabelTitlesAsync();

            this.editContext.MarkAsUnmodified();

            this.StateHasChanged();
        }

        private async Task SetExamLabelTitlesAsync()
        {
            var spkCourse = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification");
            this.practiceExamLabelTitle = this.CourseVM.IdTrainingCourseType == spkCourse.IdKeyValue
                ? "Дата за държавен изпит - част по практика:"
                : "Дата за изпит - част по практика:";

            this.theoryExamLabelTitle = this.CourseVM.IdTrainingCourseType == spkCourse.IdKeyValue
                ? "Дата за държавен изпит - част по теория:"
                : "Дата за изпит - част по теория:";
        }

        private void FilterAssignTypeSource()
        {
            if (this.CourseVM.IdMeasureType is not null)
            {
                var kvMeasureType = this.measureTypeSource.FirstOrDefault(x => x.IdKeyValue == this.CourseVM.IdMeasureType.Value);
                if (kvMeasureType is not null)
                {
                    if (kvMeasureType.KeyValueIntCode == "EmploymentProgrammesAndMeasures")
                    {
                        this.assignTypeSource = this.originalAssignTypeSource.Where(x => x.DefaultValue3 != null);
                    }
                    else
                    {
                        this.assignTypeSource = this.originalAssignTypeSource.Where(x => x.DefaultValue3 == null);
                    }
                }
            }
        }

        private async Task LoadKVSourcesAsync()
        {
            this.vqsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("VQS");
            this.measureTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MeasureType");
            this.assignTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType");
            this.originalAssignTypeSource = this.assignTypeSource.ToList();
            this.formEducationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("FormEducation");
        }

        private void PremisesSelectedHandler(ChangeEventArgs<int?, CandidateProviderPremisesVM> args)
        {
            if (args.ItemData is not null)
            {
                this.CourseVM.CandidateProviderPremises = args.ItemData;
            }
            else
            {
                this.CourseVM.CandidateProviderPremises = null;
            }
        }

        public override void SubmitHandler()
        {
            this.editContext = new EditContext(this.CourseVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.OnValidationRequested += this.ValidateCourseEndDate;
            this.editContext.OnValidationRequested += this.ValidateExamDates;

            this.editContext.Validate();
        }

        private void ValidateCourseEndDate(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.CourseVM.StartDate.HasValue && this.CourseVM.EndDate.HasValue)
            {
                if (this.CourseVM.StartDate.Value.Date > this.CourseVM.EndDate.Value.Date)
                {
                    FieldIdentifier field = new FieldIdentifier(this.CourseVM, "EndDate");
                    this.messageStore?.Add(field, $"Полето 'Дата за завършване на курса' не може да бъде преди {this.CourseVM.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г.!");
                    return;
                }

                if (!this.EntryFroomLegalCapacityModule)
                {
                    var minimalDate = this.GetCourseMinimalEndDate();
                    if (minimalDate > this.CourseVM.EndDate.Value.Date)
                    {
                        FieldIdentifier field = new FieldIdentifier(this.CourseVM, "EndDate");
                        this.messageStore?.Add(field, $"Полето 'Дата за завършване на курса' не може да бъде преди {minimalDate.ToString(GlobalConstants.DATE_FORMAT)} г.! Датата на завършване трябва да е съобразена с въведените часове в учебния план.");
                    }
                }
            }
        }

        private void ValidateExamDates(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.CourseVM.ExamPracticeDate.HasValue && this.CourseVM.EndDate.HasValue && this.CourseVM.ExamPracticeDate.Value.Date < this.CourseVM.EndDate.Value.Date)
            {
                FieldIdentifier fi = new FieldIdentifier(this.CourseVM, "ExamPracticeDate");
                this.messageStore?.Add(fi, "'Дата за изпит по практика' не може да е преди 'Дата за завършване на курса'!");
            }

            if (this.CourseVM.ExamTheoryDate.HasValue && this.CourseVM.EndDate.HasValue && this.CourseVM.ExamTheoryDate.Value.Date < this.CourseVM.EndDate.Value.Date)
            {
                FieldIdentifier fi = new FieldIdentifier(this.CourseVM, "ExamTheoryDate");
                this.messageStore?.Add(fi, "'Дата за изпит по теория' не може да е преди 'Дата за завършване на курса'!");
            }
        }

        private DateTime GetCourseMinimalEndDate()
        {
            var totalHours = (this.CourseVM.SelectableHours.HasValue ? this.CourseVM.SelectableHours.Value : 0) + (this.CourseVM.MandatoryHours.HasValue ? this.CourseVM.MandatoryHours.Value : 0);
            var workingDays = Math.Ceiling(totalHours / 8d);
            var additionalDaysFromWeek = (int)(workingDays / 6);
            var totalDaysForCourse = workingDays + additionalDaysFromWeek;
            var minimalDate = this.CourseVM.StartDate!.Value.Date.AddDays(totalDaysForCourse);
            return minimalDate;
        }

        private void OnMeasureTypeSelected(ChangeEventArgs<int?, KeyValueVM> args)
        {
            if (args.ItemData is not null)
            {
                if (args.ItemData.KeyValueIntCode == "EmploymentProgrammesAndMeasures")
                {
                    this.assignTypeSource = this.originalAssignTypeSource.Where(x => x.DefaultValue3 != null);
                }
                else
                {
                    this.assignTypeSource = this.originalAssignTypeSource.Where(x => x.DefaultValue3 == null);
                }
            }
            else
            {
                this.CourseVM.IdAssignType = null;
            }
        }
    }
}
