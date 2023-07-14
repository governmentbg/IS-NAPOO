using System.ComponentModel.DataAnnotations;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class AddSingleClientCourseModal : BlazorBaseComponent
    {
        private AddSingleClientCourseVM model = new AddSingleClientCourseVM();
        private List<ClientCourseVM> clientsCourseSource = new List<ClientCourseVM>();
        private int idCourseProtocol = 0;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Parameter]
        public EventCallback<string> CallbackAfterSubmit { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public void OpenModal(List<ClientCourseVM> clientCourses, int idCourseProtocol)
        {
            this.editContext = new EditContext(this.model);

            this.clientsCourseSource = clientCourses.ToList();
            
            this.idCourseProtocol = idCourseProtocol;

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

                if (this.editContext.Validate())
                {
                    var result = await this.TrainingService.AddCourseClientToCourseProtocolGradeAsync(this.model.IdClientCourse, this.idCourseProtocol);
                    var msg = result.HasErrorMessages ? string.Join("", result.ListErrorMessages) : string.Join("", result.ListMessages);

                    this.isVisible = false;

                    await this.CallbackAfterSubmit.InvokeAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public class AddSingleClientCourseVM
        {
            [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Курсист' е задължително!")]
            public int IdClientCourse { get; set; }
        }
    }
}
