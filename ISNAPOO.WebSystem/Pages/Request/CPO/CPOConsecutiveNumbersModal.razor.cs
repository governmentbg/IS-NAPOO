using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class CPOConsecutiveNumbersModal : BlazorBaseComponent
    {
        private SfDialog cpoConsecutiveNumbersModal = new SfDialog();
        private ToastMsg toast = new ToastMsg();

        private ConsecutiveNumbersVM consecutiveNumbersVM = new ConsecutiveNumbersVM();
        private ValidationMessageStore? messageStore;
        private List<DocumentSerialNumberVM> addedDocumentSerialNumbers = new List<DocumentSerialNumberVM>();
        private List<DocumentSerialNumberVM> addedConsecutiveDocumentSerialNumbers = new List<DocumentSerialNumberVM>();
        private RequestDocumentManagementVM requestDocumentManagementVM = new RequestDocumentManagementVM();

        #region KV Document operation fields
        private IEnumerable<KeyValueVM> kvActionType;
        private KeyValueVM kvReceived;
        private KeyValueVM kvSubmitted;
        private KeyValueVM kvPrinted;
        private KeyValueVM kvCancelled;
        private KeyValueVM kvDestroyed;
        private KeyValueVM kvAwaitingConfirmation;
        private KeyValueVM kvLost;
        #endregion

        #region KV Request document status fields
        private IEnumerable<KeyValueVM> kvRequestDocumentStatus;
        private KeyValueVM kvCreated;
        private KeyValueVM kvFiledIn;
        private KeyValueVM kvProcessed;
        private KeyValueVM kvSummarized;
        private KeyValueVM kvFulfilled;
        #endregion

        [Parameter]
        public EventCallback CallbackAfterNumbersAdd { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        public override bool IsContextModified => this.editContext.IsModified();

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.consecutiveNumbersVM);
        }

        public async void OpenModal(List<DocumentSerialNumberVM> addedDocumentSerialNumbers, RequestDocumentManagementVM requestDocumentManagementVM)
        {
            await this.LoadKVSources();
            this.editContext = new EditContext(this.consecutiveNumbersVM);
            this.addedConsecutiveDocumentSerialNumbers.Clear();
            this.consecutiveNumbersVM = new ConsecutiveNumbersVM();
            this.addedDocumentSerialNumbers = addedDocumentSerialNumbers;
            this.requestDocumentManagementVM = requestDocumentManagementVM;

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task LoadKVSources()
        {
            this.kvActionType = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
            this.kvReceived = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Received");
            this.kvSubmitted = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Submitted");
            this.kvPrinted = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Printed");
            this.kvCancelled = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Cancelled");
            this.kvDestroyed = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Destroyed");
            this.kvAwaitingConfirmation = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "AwaitingConfirmation");
            this.kvLost = this.kvActionType.FirstOrDefault(x => x.KeyValueIntCode == "Lost");

            this.kvRequestDocumentStatus = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RequestDocumetStatus");
            this.kvCreated = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Created");
            this.kvFiledIn = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Submitted");
            this.kvProcessed = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Processed");
            this.kvSummarized = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Summarized");
            this.kvFulfilled = this.kvRequestDocumentStatus.FirstOrDefault(x => x.KeyValueIntCode == "Fulfilled");
        }

        private async Task AddClickHandler()
        {
            this.editContext = new EditContext(this.consecutiveNumbersVM);
            this.editContext.EnableDataAnnotationsValidation();
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.OnValidationRequested += this.ValidateInputs;
            this.editContext.OnValidationRequested += this.ValidateStartAndEndNumberIfEqual;
            this.editContext.OnValidationRequested += this.ValidateDocumentCountWithDocumentSerialNumbersAdded;

            if (this.editContext.Validate())
            {
                if (!this.ValidateStartAndEndNumbersWithCountConsecutiveNumbers())
                {
                    await this.ShowErrorAsync("Въведеният диапазон за фабрични номера не съвпада с контролната стойност за брой поредни номера!");
                }
                else
                {
                    var length = string.Empty;
                    for (int i = 0; i < this.consecutiveNumbersVM.StartNumber.Length; i++)
                    {
                        var charEl = this.consecutiveNumbersVM.StartNumber[i];
                        if (charEl == '0')
                        {
                            length += '0';
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int i = 0; i < this.consecutiveNumbersVM.CountConsecutiveNumbers; i++)
                    {
                        var serialNumberAsInt = int.Parse(this.consecutiveNumbersVM.StartNumber) + i;
                        var serialNumberAsStr = string.Concat(length, serialNumberAsInt.ToString());

                        if (!this.addedDocumentSerialNumbers.Any(x => x.SerialNumber == serialNumberAsStr))
                        {
                            DocumentSerialNumberVM documentSerialNumber = new DocumentSerialNumberVM()
                            {
                                IdDocumentOperation = this.kvReceived.IdKeyValue,
                                IdRequestDocumentManagement = this.requestDocumentManagementVM.IdRequestDocumentManagement,
                                IdCandidateProvider = this.requestDocumentManagementVM.IdCandidateProvider,
                                IdTypeOfRequestedDocument = this.requestDocumentManagementVM.IdTypeOfRequestedDocument,
                                DocumentDate = this.requestDocumentManagementVM.DocumentDate.Value,
                                SerialNumber = serialNumberAsStr
                            };

                            this.addedConsecutiveDocumentSerialNumbers.Add(documentSerialNumber);
                        }
                    }

                    var result = await this.ProviderDocumentRequestService.AddConsecutiveDocumentSerialNumbersAsync(this.addedConsecutiveDocumentSerialNumbers);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        this.isVisible = false;
                        await this.CallbackAfterNumbersAdd.InvokeAsync();
                    }
                }
            }
        }

        private void OnEndNumberValueChanged(ChangeEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.consecutiveNumbersVM.StartNumber) && int.TryParse(this.consecutiveNumbersVM.EndNumber, out int endNumber))
            {
                this.consecutiveNumbersVM.CountConsecutiveNumbers = (int.Parse(this.consecutiveNumbersVM.EndNumber) - int.Parse(this.consecutiveNumbersVM.StartNumber)) + 1;
            }
        }

        private void OnStartNumberValueChanged(ChangeEventArgs args)
        {
            if (!string.IsNullOrEmpty(this.consecutiveNumbersVM.EndNumber) && int.TryParse(this.consecutiveNumbersVM.StartNumber, out int startNumber))
            {
                this.consecutiveNumbersVM.CountConsecutiveNumbers = (int.Parse(this.consecutiveNumbersVM.EndNumber) - int.Parse(this.consecutiveNumbersVM.StartNumber)) + 1;
            }
        }

        private bool ValidateStartAndEndNumbersWithCountConsecutiveNumbers()
        {
            var startNumber = int.Parse(this.consecutiveNumbersVM.StartNumber);
            var endNumber = int.Parse(this.consecutiveNumbersVM.EndNumber);

            return endNumber - startNumber == (this.consecutiveNumbersVM.CountConsecutiveNumbers - 1);
        }

        private void ValidateInputs(object? sender, ValidationRequestedEventArgs args)
        {
            for (int i = 0; i < this.consecutiveNumbersVM.StartNumber.Length; i++)
            {
                var charEl = this.consecutiveNumbersVM.StartNumber[i];
                if (!char.IsDigit(charEl))
                {
                    FieldIdentifier fi = new FieldIdentifier(this.consecutiveNumbersVM.StartNumber, "StartNumber");
                    this.messageStore?.Add(fi, "Полето 'Начален номер' може да съдържа само числа!");
                    break;
                }
            }

            for (int i = 0; i < this.consecutiveNumbersVM.EndNumber.Length; i++)
            {
                var charEl = this.consecutiveNumbersVM.EndNumber[i];
                if (!char.IsDigit(charEl))
                {
                    FieldIdentifier fi = new FieldIdentifier(this.consecutiveNumbersVM.EndNumber, "EndNumber");
                    this.messageStore?.Add(fi, "Полето 'Краен номер' може да съдържа само числа!");
                    break;
                }
            }
        }

        private void ValidateStartAndEndNumberIfEqual(object? sender, ValidationRequestedEventArgs args)
        {
            if (int.Parse(this.consecutiveNumbersVM.StartNumber) > int.Parse(this.consecutiveNumbersVM.EndNumber))
            {
                FieldIdentifier fi = new FieldIdentifier(this.consecutiveNumbersVM.StartNumber, "StartNumber");
                this.messageStore?.Add(fi, "Полето 'Начален номер' не може да има по-голяма стойност от полето 'Краен номер'!");
            }
        }

        private void ValidateDocumentCountWithDocumentSerialNumbersAdded(object? sender, ValidationRequestedEventArgs args)
        {
            if (this.requestDocumentManagementVM.DocumentCount < this.addedDocumentSerialNumbers.Count + this.consecutiveNumbersVM.CountConsecutiveNumbers)
            {
                FieldIdentifier fi = new FieldIdentifier(this.requestDocumentManagementVM, "DocumentCount");
                this.messageStore?.Add(fi, "Въведените фабрични номера не могат да надвишават общия въведен брой получени документи!");
            }
        }
    }
}
