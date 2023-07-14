using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Request.NAPOO
{
    public partial class DocumentsFromOrdinanceModal : BlazorBaseComponent
    {
        private DocumentSeriesVM model;

        private SfDialog sfDialog;

        private bool isNew;

        private List<TypeOfRequestedDocumentVM> typeOfRequestedDocuments;

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        public List<string> errorMessages = new List<string>();

        [Inject]
        public IProviderDocumentRequestService providerDocumentRequestService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            model = new DocumentSeriesVM();

            isNew = false;  

            typeOfRequestedDocuments = (await providerDocumentRequestService
                .GetAllValidTypesOfRequestedDocumentAsync())
                .Where(x => x.HasSerialNumber == true)
                .ToList();
        }

        public async Task openModal(DocumentSeriesVM seriesVM)
        {
            this.model = seriesVM;
            if (seriesVM.Year == null)
            {
                model.Year = DateTime.Now.Year;
                isNew = true;
            }
            else isNew = false;

            this.editContext = new EditContext(model);
            editContext.EnableDataAnnotationsValidation();
            errorMessages.Clear();
            this.isVisible = true;
            this.StateHasChanged();
        }

        public async Task Save()
        {
            if (loading) return;
           this.loading = true;

            try
            {
                var validate = this.editContext.Validate();

                if (validate)
                {
                    var isAlreadyEntered = await providerDocumentRequestService.GetDocumenSeriestByTypeAndYear(model.IdTypeOfRequestedDocument, model.Year);
                    if (isAlreadyEntered == null)
                    {
                        providerDocumentRequestService.SaveDocumentSeries(model);
                        await this.ShowSuccessAsync("Записът e успешeн!");
                        isNew = false;
                        await CallbackAfterSubmit.InvokeAsync();
                    }
                    else
                    {
                        if (isNew)
                            await this.ShowErrorAsync("Не можете да въведете две серии за един същи вид на документа и календарна година!");
                        else
                        {
                            model.IdDocumentSeries = isAlreadyEntered.IdDocumentSeries;
                            model.TypeOfRequestedDocument = null;
                            providerDocumentRequestService.UpdateDocumentSeries(model);

                            await this.ShowSuccessAsync("Записът e успешeн!");

                            await CallbackAfterSubmit.InvokeAsync();
                        }
                    }
                }
                else
                    errorMessages = this.editContext.GetValidationMessages().ToList();
            }finally
            {
                this.loading = false;
            }

        }
    }
}
