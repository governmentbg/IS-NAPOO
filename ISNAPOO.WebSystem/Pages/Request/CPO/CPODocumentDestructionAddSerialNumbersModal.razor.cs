using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class CPODocumentDestructionAddSerialNumbersModal : BlazorBaseComponent
    {
        private SfDialog cpoDocumentDestructionAddSerialNumbersModal = new SfDialog();
        private ToastMsg toast = new ToastMsg();
        private SfGrid<DocumentSerialNumberVM> fabricNumbersGrid = new SfGrid<DocumentSerialNumberVM>();

        private List<DocumentSerialNumberVM> fabricNumbersSource = new List<DocumentSerialNumberVM>();
        private List<DocumentSerialNumberVM> selectedFabricNumbers = new List<DocumentSerialNumberVM>();
        private List<OperationType> operationTypesSource = new List<OperationType>();
        private List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsSource = new List<TypeOfRequestedDocumentVM>();
        private string operationType = string.Empty;
        private DateTime? destructionDate = new DateTime();
        private ValidationMessageStore? messageStore;
        private ValidationModel documentSerialNumberVM = new ValidationModel();

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback<CPODocumentDestructionAddSerialNumbersReturnInfo> CallbackAfterModalSubmit { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        public async Task OpenModal(List<DocumentSerialNumberVM> documentSerialNumbers, List<TypeOfRequestedDocumentVM> typeOfRequestedDocuments)
        {
            this.editContext = new EditContext(this.documentSerialNumberVM);
            this.destructionDate = DateTime.Today;
            this.operationType = string.Empty;
            this.SetDataForDropDownValues();

            this.selectedFabricNumbers.Clear();
            this.fabricNumbersSource = documentSerialNumbers.ToList();
            this.typeOfRequestedDocumentsSource = typeOfRequestedDocuments.ToList();
            this.SetTypeOfRequestedDocumentDataForGrid();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void SetTypeOfRequestedDocumentDataForGrid()
        {
            foreach (var docSerialNumber in this.fabricNumbersSource)
            {
                var typeOfDoc = this.typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == docSerialNumber.IdTypeOfRequestedDocument);
                if (typeOfDoc is not null)
                {
                    docSerialNumber.TypeOfRequestedDocument = typeOfDoc;
                }
            }
        }

        private void SetDataForDropDownValues()
        {
            this.operationTypesSource.Clear();

            this.operationTypesSource.Add(new OperationType()
            {
                Id = 1,
                Type = "Анулиран"
            });

            this.operationTypesSource.Add(new OperationType()
            {
                Id = 2,
                Type = "Унищожен"
            });
        }

        private async Task AddBtn()
        {
            if (!this.fabricNumbersSource.Any())
            {
                this.toast.sfErrorToast.Content = "Няма налични документи за добавяне!";
                await this.toast.sfErrorToast.ShowAsync();
                return;
            }

            if (string.IsNullOrEmpty(this.operationType))
            {
                this.toast.sfErrorToast.Content = "Моля, изберете вид на операцията!";
                await this.toast.sfErrorToast.ShowAsync();
                return;
            }

            if (!this.selectedFabricNumbers.Any())
            {
                this.toast.sfErrorToast.Content = "Моля, изберете фабричен номер/фабрични номера!";
                await this.toast.sfErrorToast.ShowAsync();
                return;
            }

            this.editContext = new EditContext(this.documentSerialNumberVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.editContext.OnValidationRequested += this.ValidateManually;
            this.messageStore = new ValidationMessageStore(this.editContext);

            if (this.editContext.Validate())
            {
                CPODocumentDestructionAddSerialNumbersReturnInfo cpoDocumentDestructionAddSerialNumbersReturnInfo = new CPODocumentDestructionAddSerialNumbersReturnInfo()
                {
                    DestructionDate = this.destructionDate,
                    OperationType = this.operationType,
                    DocumentSerialNumbers = this.selectedFabricNumbers
                };

                this.isVisible = false;
                await this.CallbackAfterModalSubmit.InvokeAsync(cpoDocumentDestructionAddSerialNumbersReturnInfo);
            }
            else
            {
                await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
            }
        }

        private async Task DocumentSerialNumberSelected(RowSelectEventArgs<DocumentSerialNumberVM> args)
        {
            this.selectedFabricNumbers = await this.fabricNumbersGrid.GetSelectedRecordsAsync();
        }

        private async Task DocumentSerialNumberDeselected(RowDeselectEventArgs<DocumentSerialNumberVM> args)
        {
            this.selectedFabricNumbers = await this.fabricNumbersGrid.GetSelectedRecordsAsync();
        }

        private void ValidateManually(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (string.IsNullOrEmpty(this.operationType))
            {
                FieldIdentifier fi = new FieldIdentifier(this.documentSerialNumberVM, "Model");
                this.messageStore?.Add(fi, "Моля, изберете вид на операцията!");
            }

            if (this.operationType == "Анулиран")
            {
                if (!this.destructionDate.HasValue)
                {
                    FieldIdentifier fi = new FieldIdentifier(this.documentSerialNumberVM, "Model");
                    this.messageStore?.Add(fi, "Моля, изберете дата на анулиране!");
                }
            }
        }
    
        private class OperationType 
        {
            public int Id { get; set; }

            public string Type { get; set; }
        }

        public class CPODocumentDestructionAddSerialNumbersReturnInfo
        {
            public CPODocumentDestructionAddSerialNumbersReturnInfo()
            {
                this.DocumentSerialNumbers = new List<DocumentSerialNumberVM>();
            }

            public string OperationType { get; set; }

            public DateTime? DestructionDate { get; set; }

            public List<DocumentSerialNumberVM> DocumentSerialNumbers { get; set; }
        }

        private class ValidationModel
        {
            public string Model { get; set; }
        }
    }
}
