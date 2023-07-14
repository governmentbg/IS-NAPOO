using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Request.NAPOO
{
    public partial class DocumentsFromOrdinanceEigth : BlazorBaseComponent
    {

        public SfGrid<DocumentSeriesVM> sfGrid;

        public List<DocumentSeriesVM> documents;

        public DocumentsFromOrdinanceModal modal;

        [Inject]
        public IProviderDocumentRequestService providerDocumentRequestService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            documents = (await providerDocumentRequestService.GetAllDocumentSeriesIncludeAsync()).ToList();
        }

        private async Task openNewModal()
        {
            await modal.openModal(new DocumentSeriesVM());
        }
        private async Task SelectedRow(DocumentSeriesVM model)
        {
            await modal.openModal(model);
        }

        private async Task CallbackAfterSubmit()
        {
            documents = (await providerDocumentRequestService.GetAllDocumentSeriesIncludeAsync()).ToList();
            await sfGrid.Refresh();
        }

    }
}
