using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ProcedurePriceModal : BlazorBaseComponent
    {
        [Parameter]
        public EventCallback<ResultContext<ProcedurePriceVM>> CallbackAfterSubmit { get; set; }

        #region Inject
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IProviderService providerService { get; set; }

        [Inject]
        public IDataSourceService dataSourceService { get; set; }
        #endregion


        private DateTime expirationDateFrom;
        private DialogEffect AnimationEffect = DialogEffect.Zoom;
        SfDialog sfDialog;
        ResultContext<ProcedurePriceVM> resultContext = new ResultContext<ProcedurePriceVM>();
        ToastMsg toast;
        private IEnumerable<KeyValueVM> kvApplicationStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvTypeApplicationSource = new List<KeyValueVM>();
        private ValidationMessageStore? messageStore;
        private bool IsCountProfessionsFromValid = true;
        private bool IsDateValid = true;
        public override bool IsContextModified => this.editContext.IsModified();

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.resultContext.ResultContextObject);
        }
        public async Task OpenModal(ProcedurePriceVM _model)
        {
            this.kvApplicationStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");
            this.kvTypeApplicationSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingFee");
            this.resultContext.ResultContextObject = _model;
            this.resultContext.ResultContextObject.PriceAsStr = this.resultContext.ResultContextObject.Price.ToString();
            editContext = new EditContext(this.resultContext.ResultContextObject);
            IsCountProfessionsFromValid = true;
            IsDateValid = true;
            this.isVisible = true;
            this.StateHasChanged();
        }
        private void IsValid()
        {
            if (this.resultContext.ResultContextObject.CountProfessionsTo != null)
            {
                if (this.resultContext.ResultContextObject.CountProfessionsFrom > this.resultContext.ResultContextObject.CountProfessionsTo)
                {
                    this.IsCountProfessionsFromValid = false;
                }
                else
                {
                    this.IsCountProfessionsFromValid = true;
                }
            }
        }
        private void IsExpirationDateToValid()
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime(); ;
            if (this.resultContext.ResultContextObject.ExpirationDateFrom.HasValue)
            {
                startDate = this.resultContext.ResultContextObject.ExpirationDateFrom.Value.Date;
            }
            if (this.resultContext.ResultContextObject.ExpirationDateTo.HasValue)
            {
                endDate = this.resultContext.ResultContextObject.ExpirationDateTo.Value.Date;

            }
            int result = DateTime.Compare(startDate, endDate);

            if (result > 0 && this.resultContext.ResultContextObject.ExpirationDateTo.HasValue && this.resultContext.ResultContextObject.ExpirationDateFrom.HasValue)
            {
                IsDateValid = false;
            }
            else
            {
                IsDateValid = true;
            }
        }

        private async Task Save()
        {
            bool hasPermission = await CheckUserActionPermission("ManageProcedurePriceData", false);
            if (!hasPermission) { return; }
            this.SpinnerShow();

            
            this.editContext = new EditContext(this.resultContext.ResultContextObject);
            this.editContext.EnableDataAnnotationsValidation();
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.OnValidationRequested += this.ValidatePriceInput;

            bool isValid = this.editContext.Validate();
            if (IsCountProfessionsFromValid && IsDateValid)
            {
                if (isValid)
                {
                    var price = BaseHelper.ConvertToFloat(this.resultContext.ResultContextObject.PriceAsStr, 2);
                    this.resultContext.ResultContextObject.Price = decimal.Parse(price.ToString());
                    this.resultContext = await this.providerService.SaveProcedurePriceAsync(this.resultContext);

                    this.editContext = new EditContext(this.resultContext.ResultContextObject);
                    await this.CallbackAfterSubmit.InvokeAsync(resultContext);
                }
            }
            else if(IsCountProfessionsFromValid && !IsDateValid)
            {
                toast.sfErrorToast.Content = "'Дата на валидност от' не може да е след 'Дата на валидност до'!";
                await toast.sfErrorToast.ShowAsync();
            }
            else if(!IsCountProfessionsFromValid && IsDateValid)
            {
                toast.sfErrorToast.Content = "Стойността в полето 'Брой професии от' не може да е по-голяма от 'Брой професии до'!";
                await toast.sfErrorToast.ShowAsync();
            }
            else 
            {
                toast.sfErrorToast.Content = "Стойността в полето 'Брой професии от' не може да е по-голяма от 'Брой професии до'!";
                await toast.sfErrorToast.ShowAsync();
                toast.sfErrorToast.Content = "'Дата на валидност от' не може да е след 'Дата на валидност до'!";
                await toast.sfErrorToast.ShowAsync();
            }
            
            this.SpinnerHide();
        }
        private void ValidatePriceInput(object? sender, ValidationRequestedEventArgs args)
        {
            this.messageStore?.Clear();

            if (string.IsNullOrEmpty(this.resultContext.ResultContextObject.PriceAsStr) || this.resultContext.ResultContextObject.PriceAsStr == "0")
            {
                FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "PriceAsStr");
                this.messageStore?.Add(fi, "Полето 'Цена' е задължително!");
                return;
            }

            if (BaseHelper.ConvertToFloat(this.resultContext.ResultContextObject.PriceAsStr, 2) == null)
            {
                FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "PriceAsStr");
                this.messageStore?.Add(fi, "Полето 'Цена' може да бъде само число!");
                return;
            }

            var value = BaseHelper.ConvertToFloat(this.resultContext.ResultContextObject.PriceAsStr, 2);
            if (value <= 0)
            {
                FieldIdentifier fi = new FieldIdentifier(this.resultContext.ResultContextObject, "PriceAsStr");
                this.messageStore?.Add(fi, "Моля, въведете положителна стойност за Цена!");
            }
        }


    }
}
