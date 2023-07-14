using System.ServiceModel.Channels;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EGovPayment;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NuGet.Configuration;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.EGovPayment
{
    public partial class PaymentFeeList : BlazorBaseComponent
    {
        [Parameter]
        public string AccessCode { get; set; }

        [Inject]
        public IPaymentService PaymentService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public NavigationManager navigationManager { get; set; }

        [Parameter]
        public EventCallback<bool> CallbackAfterSubmit { get; set; }

        [Parameter]
        public CandidateProviderVM candidateProviderVM { get; set; }


        private IEnumerable<PaymentVM> payments;
        private SfGrid<PaymentVM> paymentFeesGrid = new SfGrid<PaymentVM>();
        private PaymentFeeModal paymentFeeModal;
        private SfDialog sfDialog;
        private bool showCodeDialog = false;
        private IEnumerable<KeyValueVM> kvPaymentStatusSource, kvPaymentLicensingFeeSource;
        private bool isDisabledExamAppBtn = false;
        private bool isDisabledInitProcBtn = false;
        private bool isDisableIssuingLicBtn = false;
        private IEnumerable<ProcedurePriceVM> procedurePrices;
        private string loginURLForPayEGov = string.Empty;
        private string loginWithCodeURLForPayEGov = string.Empty;
        private bool isJustForWatch = false;
        protected override async Task OnInitializedAsync()
        {
            this.kvPaymentStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("PaymentStatusPayEGov");
            this.kvPaymentLicensingFeeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingFee");
            this.procedurePrices = (await this.PaymentService.GetAllProcedurePrices()).ToList();
           
            await this.LoadPaymentsDataAsync();           
        }

        public async Task LoadPaymentsDataAsync()
        {
            isJustForWatch = false;
            if (this.candidateProviderVM.fromPage == "ApplicationList")
            {
                isJustForWatch = true;
            }
            else 
            {
                isJustForWatch = false;
            }
            this.payments = await this.PaymentService.GetPaymentsByCandidateProviderIdAsync(this.candidateProviderVM.IdCandidate_Provider);
            var settingLoginURL = await this.DataSourceService.GetSettingByIntCodeAsync("LoginURLForPayEGov");
            this.loginURLForPayEGov = settingLoginURL.SettingValue;
            var settingLoginWithCodeURL = await this.DataSourceService.GetSettingByIntCodeAsync("LoginWithCodeURLForPayEGov");
            this.loginWithCodeURLForPayEGov = settingLoginWithCodeURL.SettingValue;
            this.isDisabledExamAppBtn = false;
            this.isDisabledInitProcBtn = false;
            this.isDisableIssuingLicBtn = false;

            foreach (var payment in this.payments)
            {
                var kvPaymentStatus = this.kvPaymentStatusSource.FirstOrDefault(x => x.IdKeyValue == payment.IdPaymentStatus);
                payment.PaymentStatus = kvPaymentStatus.Name;
                payment.PaymentStatusIntCode = kvPaymentStatus.KeyValueIntCode;

                var procPrice = this.procedurePrices.FirstOrDefault(x=> x.IdProcedurePrice == payment.IdProcedurePrice);
                var kvPaymentLicensingFee = this.kvPaymentLicensingFeeSource.FirstOrDefault(x => x.IdKeyValue == procPrice.IdTypeApplication);

                payment.ObligationType = kvPaymentLicensingFee.Name;

                if (payment.ExpirationDate > DateTime.Now)
                {
                    if (payment.PaymentStatusIntCode == "pending" || payment.PaymentStatusIntCode == "authorized" || payment.PaymentStatusIntCode == "ordered" || payment.PaymentStatusIntCode  == "paid" || payment.PaymentStatusIntCode == "inprocess")
                    {
                        if (kvPaymentLicensingFee.KeyValueIntCode == "FirstLicenzingCPO" || kvPaymentLicensingFee.KeyValueIntCode == "FirstLicenzingCIPO")
                        {
                            isDisabledExamAppBtn = true;
                        }
                        else if (kvPaymentLicensingFee.KeyValueIntCode == "StartProcedureCPO" || kvPaymentLicensingFee.KeyValueIntCode == "StartProcedureCIPO")
                        {
                            isDisabledInitProcBtn = true;
                        }
                        else if (kvPaymentLicensingFee.KeyValueIntCode == "IssuingLicenseCPO" || kvPaymentLicensingFee.KeyValueIntCode == "IssuingLicenseCIPO")
                        {
                            isDisableIssuingLicBtn = true;
                        }
                    }
                }              
            }
            this.StateHasChanged();
        }

        public async Task OpenPaymentFeeModal(string obligationType)
        {

            //bool hasPermission = await CheckUserActionPermission("ManagePaymentData", false);
            //if (!hasPermission) { return; }

            PaymentVM payment = new PaymentVM();
            payment.Currency = (await this.DataSourceService.GetSettingByIntCodeAsync("CurrencyPayEGov")).SettingValue;

            var expDay = (await this.DataSourceService.GetSettingByIntCodeAsync("ExpirationDatePayEGov")).SettingValue;//DateTime.Now.AddDays(1);
            var newDate = DateTime.Now;
            payment.ExpirationDate = new DateTime(newDate.Year, newDate.Month, newDate.Day, 23, 59, 59).AddDays(int.Parse(expDay));

            paymentFeeModal.OpenPayModal(payment, obligationType, candidateProviderVM);
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
    }
}
