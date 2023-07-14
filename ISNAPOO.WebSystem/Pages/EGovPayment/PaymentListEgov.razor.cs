using System.Security.AccessControl;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.EGovPayment;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Org.BouncyCastle.Ocsp;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.EGovPayment
{
    public partial class PaymentListEgov : BlazorBaseComponent
    {
        [Inject]
        public IPaymentService PaymentService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        SfGrid<RequestJson> paymentsListGrid = new SfGrid<RequestJson>();
        IEnumerable<PaymentVM> payments;
        List<PaymentRequest> pReq;
        List<RequestJson> requestObjJson;
        RequestJson requestJ;
        PaymentById resById = new PaymentById();
        private string ApplicantUin;


        //private async Task SearchRequestPayments()
        //{
        //    ResultContext<PaymentVM> resultContext = new ResultContext<PaymentVM>();

        //    resultContext = await this.PaymentService.GetPaymentsByEikWithoutPendingJson(this.ApplicantUin);



        //    this.StateHasChanged();
        //}

        private async Task SearchRequestPayments()
        {
           
            
            List<string> reciepsIds = new List<string>();
            if (this.ApplicantUin != null)
            {
                this.payments = await this.PaymentService.GetAllPaymentsByApplicantUinAsync(this.ApplicantUin);

                foreach (var payment in this.payments)
                {
                    reciepsIds.Add(payment.ReceiptId);
                }

                resById = await this.PaymentService.GetPaymentsByIdJson(reciepsIds);

                this.pReq = resById.paymentRequests;

                this.requestJ = new RequestJson();
                this.requestObjJson = new List<RequestJson>();

                foreach (var request in this.pReq)
                {
                    this.requestJ = new RequestJson();
                    this.requestJ.aisPaymentId = request.id;
                    this.requestJ.paymentTypeCode = request.requestJson.paymentTypeCode;
                    this.requestJ.paymentAmount = request.requestJson.paymentAmount;
                    this.requestJ.paymentReason = request.requestJson.paymentReason;
                    this.requestJ.applicantUinTypeId = request.requestJson.applicantUinTypeId;
                    this.requestJ.applicantUin = request.requestJson.applicantUin;
                    this.requestJ.applicantName = request.requestJson.applicantName;
                    this.requestJ.paymentReferenceType = request.requestJson.paymentReferenceType;
                    this.requestJ.paymentReferenceNumber = request.requestJson.paymentReferenceNumber;
                    this.requestJ.paymentReferenceDate = request.requestJson.paymentReferenceDate;
                    this.requestJ.expirationDate = request.requestJson.expirationDate;
                    this.requestJ.additionalInformation = request.requestJson.additionalInformation;
                    this.requestObjJson.Add(requestJ);
                }

                paymentsListGrid.Refresh();
                this.StateHasChanged();
            }
            else
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, "Моля въведете ЕГН/ЛНЧ или БУЛСТАТ"));
            }
        }
    }
}
