using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CourseClientFinishedDataModal : BlazorBaseComponent
    {
        private FinishedDataModel model = new FinishedDataModel();
        private CourseVM course = new CourseVM();
        private string title = string.Empty;
        private List<ClientCourseVM> clientCourses = new List<ClientCourseVM>();
        private IEnumerable<KeyValueVM> finishedTypeSource = new List<KeyValueVM>();
        private ValidationMessageStore? messageStore;

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public void OpenModal(List<ClientCourseVM> clientCourses, IEnumerable<KeyValueVM> finishedTypeSource, CourseVM course)
        {
            this.model = new FinishedDataModel();
            this.editContext = new EditContext(this.model);

            this.course = course;
            this.clientCourses = clientCourses;
            this.title = this.clientCourses.Count > 1 ? "курсисти" : "курсист";
            this.finishedTypeSource = finishedTypeSource;

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task SubmitBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.model);
                this.editContext.EnableDataAnnotationsValidation();
                this.messageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidateInputs;

                if (this.editContext.Validate())
                {
                    ClientCourseVM clientCourse = new ClientCourseVM()
                    {
                        IdFinishedType = this.model.IdFinishedType
                    };

                    var result = new ResultContext<ClientCourseVM>();
                    result.ResultContextObject = clientCourse;
                    result = await this.TrainingService.UpdateClientCoursesListFinishedDataAsync(result, this.clientCourses);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        this.isVisible = false;

                        await this.CallbackAfterSubmit.InvokeAsync();
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }

        private void ValidateInputs(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (this.model.IdFinishedType is null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.model, "IdFinishedType");
                this.messageStore?.Add(fi, "Полето 'Статус на завършване' е задължително!");
            }
        }
    }

    public class FinishedDataModel
    {
        public int? IdFinishedType { get; set; }
    }
}
