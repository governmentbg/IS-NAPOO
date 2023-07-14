using System.ComponentModel.DataAnnotations;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.Registers.DocumentsFromCPO
{
    public partial class ChangeRIDPKDocumentStatusModal : BlazorBaseComponent
    {
        private ChangeRIDPKDocumentStatusModel model = new ChangeRIDPKDocumentStatusModel();

        private List<int> documentIds = new List<int>();
        private IEnumerable<KeyValueVM> ridpkDocStatusesSource = new List<KeyValueVM>();
        private string title = string.Empty;
        private bool isCourse = true;

        [Parameter]
        public EventCallback<List<int>> CallbackAfterSubmit { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal(List<int> documentIds, bool isCourse)
        {
            this.editContext = new EditContext(this.model);

            this.documentIds = documentIds.ToList();
            this.isCourse = isCourse;

            this.ridpkDocStatusesSource = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientDocumentStatusType"))
                .Where(x => x.KeyValueIntCode == "Returned" || x.KeyValueIntCode == "Hidden").ToList();

            this.title = this.documentIds.Count == 1
                ? "Промяна на статуса на документ"
                : "Промяна на статуси на документи";

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
                    var result = this.isCourse 
                        ? await this.TrainingService.ChangeRIDPKStatusForListClientCourseDocumentIdsAsync(this.documentIds, this.model.IdDocumentStatus!.Value, this.model.Comment)
                        : await this.TrainingService.ChangeRIDPKStatusForListValidationClientDocumentIdsAsync(this.documentIds, this.model.IdDocumentStatus!.Value, this.model.Comment);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        this.isVisible = false;

                        await this.CallbackAfterSubmit.InvokeAsync(this.documentIds);

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

    internal class ChangeRIDPKDocumentStatusModel
    {
        [StringLength(DBStringLength.StringLength6000, ErrorMessage = "Полето 'Коментар' може да съдържа до 6000 символа!")]
        public string? Comment { get; set; }

        [Required(ErrorMessage = "Полето 'Статус' е задължително!")]
        public int? IdDocumentStatus { get; set; }
    }
}
