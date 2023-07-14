using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class HandingOverDocumentAddSerialNumberModal : BlazorBaseComponent
    {
        private SfDialog handingOverDocumentAddSerialNumberModal = new SfDialog();
        private ToastMsg toast = new ToastMsg();
        private SfGrid<DocumentSerialNumberVM> fabricNumbersGrid = new SfGrid<DocumentSerialNumberVM>();

        private List<DocumentSerialNumberVM> fabricNumbersSource = new List<DocumentSerialNumberVM>();
        private List<DocumentSerialNumberVM> selectedFabricNumbers = new List<DocumentSerialNumberVM>();

        [Parameter]
        public EventCallback<List<DocumentSerialNumberVM>> CallbackAfterModalSubmit { get; set; }

        public void OpenModal(List<DocumentSerialNumberVM> documentSerialNumbers)
        {
            this.selectedFabricNumbers.Clear();
            this.fabricNumbersSource = documentSerialNumbers.ToList();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Add()
        {
            if (!this.selectedFabricNumbers.Any())
            {
                this.toast.sfErrorToast.Content = "Моля, изберете фабричен номер/фабрични номера!";
                await this.toast.sfErrorToast.ShowAsync();
                return;
            }

            this.isVisible = false;
            await this.CallbackAfterModalSubmit.InvokeAsync(this.selectedFabricNumbers);
        }

        private async Task DocumentSerialNumberSelected(RowSelectEventArgs<DocumentSerialNumberVM> args)
        {
            this.selectedFabricNumbers = await this.fabricNumbersGrid.GetSelectedRecordsAsync();
        }

        private async Task DocumentSerialNumberDeselected(RowDeselectEventArgs<DocumentSerialNumberVM> args)
        {
            this.selectedFabricNumbers = await this.fabricNumbersGrid.GetSelectedRecordsAsync();
        }
    }
}
