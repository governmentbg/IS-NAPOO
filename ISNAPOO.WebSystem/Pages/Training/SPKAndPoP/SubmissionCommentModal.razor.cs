using System.ComponentModel.DataAnnotations;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class SubmissionCommentModal : BlazorBaseComponent
    {
        private List<ClientCourseDocumentVM> documents = new List<ClientCourseDocumentVM>();
        private SubmissionCommentModel model = new SubmissionCommentModel();
        private List<RIDPKDocumentVM> documentsFromRIDPKNAPOO;
        private string type = string.Empty;
        private string operation = string.Empty;
        private string title = string.Empty;

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Parameter]
        public List<ValidationClientDocumentVM> ValidationDocuments { get; set; } = new List<ValidationClientDocumentVM>();

        [Inject]
        public ITrainingService TrainingService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public void OpenModal(string operation, List<ClientCourseDocumentVM> documents = null, List<RIDPKDocumentVM> documentsFromRIDPKNAPOO = null, string type = null)
        {
            this.model = new SubmissionCommentModel();
            this.editContext = new EditContext(this.model);

            if (documents is not null)
            {
                this.documents = documents.ToList();
            }

            if (documentsFromRIDPKNAPOO is not null)
            {
                this.documentsFromRIDPKNAPOO = documentsFromRIDPKNAPOO.ToList();
            }

            this.operation = operation;

            if (!string.IsNullOrEmpty(type))
            {
                this.type = type;
                if (this.operation == GlobalConstants.RIDPK_OPERATION_APPROVE)
                {
                    this.title = "Коментар при одобрявяне на документите за ПК";
                }
                else if (this.operation == GlobalConstants.RIDPK_OPERATION_RETURN)
                {
                    this.title = "Коментар при връщане за корекция на документите към ЦПО";
                }
                else if (this.operation == GlobalConstants.RIDPK_OPERATION_REJECT)
                {
                    this.title = "Коментар при отказ за публикиване в регистъра на документите за ПК";
                }
            }
            else
            {
                this.title = "Коментар при подаване на документи за проверка към НАПОО";
            }

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
                    var result = new ResultContext<NoResult>();
                    if (this.operation == GlobalConstants.RIDPK_OPERATION_APPROVE)
                    {
                        result = await this.TrainingService.ApproveRIDPKDocumentsAsync(this.type, this.documentsFromRIDPKNAPOO, this.model.Comment);
                    }
                    else if (this.operation == GlobalConstants.RIDPK_OPERATION_RETURN)
                    {
                        result = await this.TrainingService.ReturnRIDPKDocumentsAsync(this.type, this.documentsFromRIDPKNAPOO, this.model.Comment);
                    }
                    else if (this.operation == GlobalConstants.RIDPK_OPERATION_REJECT)
                    {
                        result = await this.TrainingService.RejectRIDPKDocumentsAsync(this.type, this.documentsFromRIDPKNAPOO, this.model.Comment);
                    }
                    else if (this.operation == GlobalConstants.RIDPK_OPERATION_FILE_IN)
                    {
                        if (this.documents is not null && this.documents.Any())
                        {
                            result = await this.TrainingService.SendDocumentsForVerificationAsync(this.documents, this.model.Comment);
                        }
                        else if (this.ValidationDocuments is not null && this.ValidationDocuments.Any())
                        {
                            result = await this.TrainingService.SendValidationDocumentsForVerificationAsync(this.ValidationDocuments, this.model.Comment);
                        }
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        this.isVisible = false;

                        await this.CallbackAfterSubmit.InvokeAsync();

                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));
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

    internal class SubmissionCommentModel
    {
        [StringLength(DBStringLength.StringLength6000, ErrorMessage = "Полето 'Коментар' може да съдържа до 6000 символа!")]
        public string? Comment { get; set; }
    }
}
