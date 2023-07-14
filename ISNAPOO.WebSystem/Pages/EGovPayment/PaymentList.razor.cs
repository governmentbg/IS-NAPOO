using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EGovPayment;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.Services.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace ISNAPOO.WebSystem.Pages.EGovPayment
{
    public partial class PaymentList : BlazorBaseComponent
    {
        [Parameter]
        public string  AccessCode { get; set; }
      
        [Inject]
        public IPaymentService PaymentService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }


        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }
        
        [Inject]
        public NavigationManager navigationManager { get; set; }

        [Parameter]
        public EventCallback<bool> CallbackAfterSubmit { get; set; }

        IEnumerable<PaymentVM> payments;
        SfGrid<PaymentVM> paymentsGrid = new SfGrid<PaymentVM>();
        //IEnumerable<ProcedurePriceVM> procedurePrices;
        PaymentModal paymentModal;

        private IEnumerable<KeyValueVM> kvPaymentStatusSource = new List<KeyValueVM>();

        protected override async Task OnInitializedAsync()
        {
            this.kvPaymentStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("PaymentStatusPayEGov");

        }

        private async Task LoadPaymentsDataAsync()
        {
            this.payments = await this.PaymentService.GetAllPaymentsAsync(this.UserProps.IdCandidateProvider);
       
            foreach (var payment in this.payments)
            {

                var kvPaymentStatus = this.kvPaymentStatusSource.FirstOrDefault(x => x.IdKeyValue == payment.IdPaymentStatus);
                payment.PaymentStatus = kvPaymentStatus.Name;
                payment.PaymentStatusIntCode = kvPaymentStatus.KeyValueIntCode;
            }

            this.StateHasChanged();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await this.LoadPaymentsDataAsync();
            }
        }
        public async Task openNewPaymentModal()
        {

            bool hasPermission = await CheckUserActionPermission("ManagePaymentData", false);
            if (!hasPermission) { return; }


            PaymentVM payment = new PaymentVM();

            //payment.CandidateProvider = await CandidateProviderService.GetCandidateProviderWithoutAnythingIncludedByIdAsync(1071);
            payment.CandidateProvider = new Core.ViewModels.Candidate.CandidateProviderVM();

            KeyValueVM kvPending = await this.DataSourceService.GetKeyValueByIntCodeAsync("PaymentStatusPayEGov", "pending");
            payment.Currency = (await this.DataSourceService.GetSettingByIntCodeAsync("CurrencyPayEGov")).SettingValue;
            payment.IdPaymentStatus = kvPending.IdKeyValue;
            
            var expDay = (await this.DataSourceService.GetSettingByIntCodeAsync("ExpirationDatePayEGov")).SettingValue;//DateTime.Now.AddDays(1);
            var newDate = DateTime.Now;           
            payment.ExpirationDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 23,59,59).AddDays(int.Parse(expDay));

            paymentModal.openPayModal(payment);
        }
        public async Task SelectPayment(PaymentVM payment)
        {
            var pay = await PaymentService.GetPaymentAsync(payment.IdPayment);
            
            await paymentModal.openPayModal(pay);
        }
        
        public async Task ShowPaymentCode(PaymentVM payment)
        {
            
            var resultCode = await PaymentService.GetAccessCode(payment.ReceiptId);

            if (resultCode.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultCode.ListErrorMessages));
            }
            else
            {
                AccessCode = resultCode.ResultContextObject.AccessCode;
                this.showCodeDialog = true;
            }
                      
        }
        public async Task SuspendPaymentRequest(PaymentVM payment)
        {
            var resultText = await PaymentService.SuspendPaymentRequest(payment.ReceiptId);
 
            if (resultText.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultText.ListErrorMessages));
            }
            else
            {               
                await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultText.ListMessages));
                await this.LoadPaymentsDataAsync();
                this.StateHasChanged();
            }         
        }
        public async Task ShowPaymentOrder(PaymentVM payment)
        {

            var resultPaymentOrder = await PaymentService.GetPaymentOrder(payment.ReceiptId);

            if (resultPaymentOrder.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultPaymentOrder.ListErrorMessages));
            }
            else
            {             
                navigationManager.NavigateTo(resultPaymentOrder.ResultContextObject.PaymentOrderURl);               
            }
        }
    }
}
