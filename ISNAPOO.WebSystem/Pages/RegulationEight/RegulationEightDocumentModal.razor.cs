using System.Globalization;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.RegulationEight
{
    public partial class RegulationEightDocumentModal : BlazorBaseComponent
    {
        private TypeOfRequestedDocumentVM typeOfRequestedDocumentVM = new TypeOfRequestedDocumentVM();

        private List<ValidValue> validSource = new List<ValidValue>();
        private ValidationMessageStore? messageStore;
        private IEnumerable<KeyValueVM> typeFrameworkPrograms = new List<KeyValueVM>();

        public override bool IsContextModified => this.editContext.IsModified();

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.typeOfRequestedDocumentVM);

            if (!this.validSource.Any())
            {
                this.validSource.Add(
                new ValidValue()
                {
                    IsValid = true,
                    Name = "Активен"
                });

                this.validSource.Add(
                new ValidValue()
                {
                    IsValid = false,
                    Name = "Неактивен"
                });
            }
        }

        public void OpenModal(TypeOfRequestedDocumentVM typeOfRequestedDocument, IEnumerable<KeyValueVM> typeFrameworkPrograms)
        {
            if (this.messageStore != null)
            {
                this.messageStore.Clear();
            }
            this.typeFrameworkPrograms = typeFrameworkPrograms;

            this.typeOfRequestedDocumentVM = typeOfRequestedDocument;
            if (this.typeOfRequestedDocumentVM.IdTypeOfRequestedDocument == 0)
            {
                this.typeOfRequestedDocumentVM.IsValid = true;
            }
            if (this.typeOfRequestedDocumentVM.Price.HasValue)
            {
                this.typeOfRequestedDocumentVM.PriceAsStr = typeOfRequestedDocument.Price.Value.ToString();
            }

            this.editContext.MarkAsUnmodified();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Submit()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.typeOfRequestedDocumentVM);
                this.editContext.EnableDataAnnotationsValidation();
                this.messageStore = new ValidationMessageStore(this.editContext);
                this.editContext.OnValidationRequested += this.ValidatePriceInput;

                if (this.editContext.Validate())
                {
                    var inputContext = new ResultContext<TypeOfRequestedDocumentVM>();

                    this.typeOfRequestedDocumentVM.Price = BaseHelper.ConvertToFloat(this.typeOfRequestedDocumentVM.PriceAsStr, 2);
                    inputContext.ResultContextObject = this.typeOfRequestedDocumentVM;

                    var result = new ResultContext<TypeOfRequestedDocumentVM>();
                    if (this.typeOfRequestedDocumentVM.IdTypeOfRequestedDocument == 0)
                    {
                        result = await this.ProviderDocumentRequestService.CreateTypeOfRequestedDocumentAsync(inputContext);
                    }
                    else
                    {
                        result = await this.ProviderDocumentRequestService.UpdateTypeOfRequestedDocumentAsync(inputContext);
                    }

                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        this.typeOfRequestedDocumentVM.IdTypeOfRequestedDocument = result.ResultContextObject.IdTypeOfRequestedDocument;
                        this.typeOfRequestedDocumentVM.PriceAsStr = result.ResultContextObject.PriceAsStr;
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));
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

        private void ValidatePriceInput(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (string.IsNullOrEmpty(this.typeOfRequestedDocumentVM.PriceAsStr))
            {
                FieldIdentifier fi = new FieldIdentifier(this.typeOfRequestedDocumentVM, "PriceAsStr");
                this.messageStore?.Add(fi, "Полето 'Единична цена' е задължително!");
                return;
            }

            if (BaseHelper.ConvertToFloat(this.typeOfRequestedDocumentVM.PriceAsStr, 2) == null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.typeOfRequestedDocumentVM, "PriceAsStr");
                this.messageStore?.Add(fi, "Полето 'Единична цена' може да бъде само число!");
                return;
            }

            var value = BaseHelper.ConvertToFloat(this.typeOfRequestedDocumentVM.PriceAsStr, 2);
            if (value < 0)
            {
                FieldIdentifier fi = new FieldIdentifier(this.typeOfRequestedDocumentVM, "PriceAsStr");
                this.messageStore?.Add(fi, "Полето 'Единична цена' не може да има стойност по-малка от 0!");
            }
        }

        private class ValidValue
        {
            public bool IsValid { get; set; }

            public string Name { get; set; }
        }
    }
}
