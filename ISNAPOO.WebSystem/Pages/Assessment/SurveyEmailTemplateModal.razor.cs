using System.ComponentModel.DataAnnotations;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Assessment;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.Assessment
{
    public partial class SurveyEmailTemplateModal : BlazorBaseComponent
    {
        private SurveyEmailTemplateModel model = new SurveyEmailTemplateModel();
        private SurveyVM surveyVM = new SurveyVM();
        private bool openFromResendSurvey = false;

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public IAssessmentService AssessmentService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public void OpenModal(SurveyVM survey, bool openFromResendSurvey = false)
        {
            this.openFromResendSurvey = openFromResendSurvey;

            this.model = new SurveyEmailTemplateModel();
            this.editContext = new EditContext(this.model);

            this.surveyVM = survey;

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
                    this.surveyVM.EmailTemplateHeader = this.model.Header;
                    this.surveyVM.EmailTemplateText = this.model.Text;
                    var inputContext = new ResultContext<SurveyVM>();
                    inputContext.ResultContextObject = this.surveyVM;
                    if (!this.openFromResendSurvey)
                    {
                        var result = await this.AssessmentService.SendSurveyAsync(inputContext);
                        if (!result.HasErrorMessages)
                        {
                            this.isVisible = false;
                            await this.CallbackAfterSubmit.InvokeAsync();

                            await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                        }
                        else
                        {
                            await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                        }
                    }
                    else
                    {
                        var result = await this.AssessmentService.ReSendSurveyAsync(inputContext);
                        if (!result.HasErrorMessages)
                        {
                            this.isVisible = false;
                            await this.CallbackAfterSubmit.InvokeAsync();

                            await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                        }
                        else
                        {
                            await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
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
    }

    internal class SurveyEmailTemplateModel
    {
        [Required(ErrorMessage = "Полето 'Заглавие' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Заглавие' може да съдържа до 100 символа!")]
        public string Header { get; set; }

        [Required(ErrorMessage = "Полето 'Описание' е задължително!")]
        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'Описание' може да съдържа до 1000 символа!")]
        public string Text { get; set; }
    }
}


