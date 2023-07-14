using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.EGovPayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.EGovPayment
{
    public interface IPaymentService
    {
        Task<ResultContext<PaymentVM>> CreatePaymentAsync(ResultContext<PaymentVM> resultContext);
        Task<ResultContext<PaymentVM>> CreatePaymentToPayEGov(ResultContext<PaymentVM> resultContext);
        Task<ResultContext<PaymentVM>> GetAccessCode(string recieptId);//ResultContext<PaymentVM> resultContext);
        Task<IEnumerable<PaymentVM>> GetAllPaymentsAsync(int idCandidateProvider);
        Task<IEnumerable<PaymentVM>> GetPaymentsByCandidateProviderIdAsync(int idCandidateProvider);
        Task<IEnumerable<PaymentVM>> GetAllPaymentsByApplicantUinAsync(string applicantUin);
        Task<PaymentVM> GetPaymentAsync(int idPayment);
        Task<IEnumerable<ProcedurePriceVM>> GetAllProcedurePrices();
        Task<PaymentById> GetPaymentsByIdJson(List<string> recieptIds);
        Task<ResultContext<PaymentVM>> GetPaymentsByEikWithoutPendingJson(string applicantUin);
        Task<ResultContext<PaymentStatus>> GetPaymentsStatus(ResultContext<PaymentVM> resultContext);
        Task<ResultContext<PaymentVM>> SetStatusPaymentRequest(ResultContext<PaymentVM> resultContext);
        Task<ResultContext<PaymentVM>> SuspendPaymentRequest(string recieptId);
        Task<ResultContext<PaymentVM>> UpdateSuspendPaymentStatusAsync(string recieptId);
        Task<ResultContext<PaymentVM>> UpdatePaymentAsync(ResultContext<PaymentVM> resultContext);
        Task<ResultContext<PaymentVM>> UpdatePaymentStatusAsync(ResultContext<PaymentStatus> resultContext);
        Task<ResultContext<PaymentVM>> PrepareNewPayment(int idCandidateProvider, string licensingFeeValue);
        Task<ResultContext<PaymentVM>> GetPaymentOrder(string receiptId);   
        Task<int> GetProfCount (CandidateProviderVM candidateProviderVM);
    }
}